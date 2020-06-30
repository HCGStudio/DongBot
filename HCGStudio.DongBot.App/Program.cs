using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using HCGStudio.DongBot.App.Models;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;
using HCGStudio.DongBot.CqHttp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;

namespace HCGStudio.DongBot.App
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, List<(object, MethodInfo)>> GroupMethodDirectory =
            new Dictionary<string, List<(object, MethodInfo)>>();

        private static readonly Dictionary<string, List<(object, MethodInfo)>> GroupAtMeMethodDirectory =
            new Dictionary<string, List<(object, MethodInfo)>>();

        private static readonly List<(object, MethodInfo)> PrivateMethodList = new List<(object, MethodInfo)>();

        private static readonly Dictionary<(int, int), List<(object, MethodInfo)>> ScheduledTaskDictionary =
            new Dictionary<(int, int), List<(object, MethodInfo)>>();

        private static IContainer _container;

        private static async Task LoadService(Assembly assembly, IReadOnlyCollection<long> groups, bool builtIn = false)
        {
            await using var context = new ApplicationContext();
            foreach (var type in assembly.GetTypes())
            {
                var service = type.GetCustomAttribute<ServiceAttribute>();
                if (service == null)
                    continue;
                if (string.IsNullOrWhiteSpace(service.Name) || service.Name.Contains(' '))
                {
                    Logger.Error($"Service name {service.Name} is invaild, not loading.");
                    continue;
                }

                if (!builtIn && service.Name == "Core")
                {
                    Logger.Error($"Non builtin class {type.FullName} has service name Core, will not loading.");
                    continue;
                }

                Logger.Info($"Now loading service {service.Name}.");

                var added = from record in context.ServiceRecords
                    where record.ServiceName == service.Name
                    select record.GroupId;

                //Add not in the database
                foreach (var groupId in groups.SkipWhile(g => added.Contains(g)))
                {
                    Logger.Info($"Service {service.Name} not on group {groupId}, now adding.");
                    await context.ServiceRecords.AddAsync(new ServiceRecord
                        {GroupId = groupId, ServiceName = service.Name, IsEnabled = service.AutoEnable});
                }

                await context.SaveChangesAsync();

                //Add to dictionary if service name not added before
                if (!GroupMethodDirectory.ContainsKey(service.Name))
                    GroupMethodDirectory.Add(service.Name, new List<(object, MethodInfo)>());

                if (!GroupAtMeMethodDirectory.ContainsKey(service.Name))
                    GroupAtMeMethodDirectory.Add(service.Name, new List<(object, MethodInfo)>());
                //Create instance
                //IMessage sender only
                var instance = Activator.CreateInstance(type);
                _container.InjectProperties(instance);

                //Find marked method
                foreach (var methodInfo in type.GetMethods())
                {
                    var keyword = methodInfo.GetCustomAttribute<OnKeywordAttribute>();
                    if (keyword == null)
                        continue;
                    if ((keyword.InvokePolicies & InvokePolicies.Private) != 0)
                        PrivateMethodList.Add((instance, methodInfo));
                    if ((keyword.InvokePolicies & InvokePolicies.Group) != 0)
                        GroupMethodDirectory[service.Name].Add((instance, methodInfo));
                    if ((keyword.InvokePolicies & InvokePolicies.GroupAt) != 0)
                        GroupAtMeMethodDirectory[service.Name].Add((instance, methodInfo));
                }

                foreach (var methodInfo in type.GetMethods())
                {
                    var schedule = methodInfo.GetCustomAttribute<ScheduleTaskAttribute>();
                    if (schedule == null)
                        continue;
                    if (methodInfo.ReturnType != typeof(Task) || methodInfo.GetParameters().Length != 0)
                    {
                        Logger.Error(
                            $"Schedule task only supports async Task with no parameters, {methodInfo.Name} will not load.");
                        continue;
                    }

                    if (ScheduledTaskDictionary.TryGetValue((schedule.Hour, schedule.Minute), out var list))
                        list.Add((instance, methodInfo));
                    else
                        ScheduledTaskDictionary.Add((schedule.Hour, schedule.Minute),
                            new List<(object, MethodInfo)>
                            {
                                (instance, methodInfo)
                            });
                }
            }
        }

        private static ContainerBuilder UseDefaultService(this ContainerBuilder builder, string serviceName)
        {
            return builder;
        }

        private static async Task Main(string[] args)
        {
            //Read config
            if (!File.Exists("config.json"))
            {
                await File.WriteAllTextAsync("config.json",
                    JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));
                Logger.Error("Configuration file not fount, aborting. Please modify the generated config.");
                return;
            }

            var config = JsonConvert.DeserializeObject<Configuration>(await File.ReadAllTextAsync("config.json"));
            Logger.Info($"Now using {config.ServiceType}.");

            try
            {
                //Build container for dependency inject
                var builder = new ContainerBuilder();
                //register config.json
                builder.Register(c => new ConfigurationBuilder().AddJsonFile("config.json").Build())
                    .As<IConfiguration>();
                switch (config.ServiceType)
                {
                    case ServiceType.CqHttpHttp:
                        builder.UseCqHttpClient(config.AccessUrl, config.AccessToken, config.ListenPort);
                        break;
                    case ServiceType.CqWs:
                        builder.UseCqWs(config.AccessUrl, config.AccessToken, config.EventUrl);
                        break;
                    case ServiceType.CqWsReserve:
                        builder.UseCqWsReserve(config.ListenPort, config.ApiPath, config.EventPath, config.AccessToken);
                        break;
                    case ServiceType.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //Init EF core
                await using var context = new ApplicationContext();
                await context.Database.EnsureCreatedAsync();

                _container = builder.Build();
                var messageProvider = _container.Resolve<IMessageProvider>();
                var groups = await messageProvider.GetAllGroupsAsync();

                //Load service
                Logger.Info("Now loading builtin services.");
                await LoadService(Assembly.GetExecutingAssembly(), groups, true);

                //Load plugins
                Logger.Info("Now load plguins from plugins dir.");

                foreach (var file in Directory.CreateDirectory("plugins").GetFiles()
                    .Where(file => file.Extension == ".dll").Select(file => file.FullName))
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        await LoadService(assembly, groups);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"File {file} is not a vaild plugin.");
                        Logger.Error(e);
                    }


                //Load Light-Plugins
                Directory.CreateDirectory("temp");
                Logger.Info("Noe compling light plugins.");
                foreach (var file in Directory.CreateDirectory("light-plugins").GetFiles()
                    .Where(file => file.Extension == ".cs" || file.Extension == ".lpg").Select(file => file))
                {
                    var compile = CSharpCompilation.Create($"{file.Name}.g",
                        new[] { CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(file.FullName)) },
                        AppDomain.CurrentDomain.GetAssemblies().Select(x =>
                        {
                            try
                            {
                                return MetadataReference.CreateFromFile(x.Location);
                            }
                            catch
                            {
                                return null;
                            }
                        }).TakeWhile(x => x != null),
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                    await using var stream = new MemoryStream();
                    var compileResult = compile.Emit(stream);
                    await stream.FlushAsync();
                    if (compileResult.Success)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        await LoadService(Assembly.Load(stream.ToArray()), groups);
                    }
                    else
                    {
                        Logger.Error($"Compile failed on file {file.Name}.");
                        Logger.Error($"{compileResult.Diagnostics}");
                    }
                }


                //Save changes
                await context.SaveChangesAsync();

                messageProvider.SubscribePrivateMessage(async (message, userId) =>
                {
                    var pureText = message.ToPureString();
                    Logger.Info($"Received a private message {pureText} from {userId}.");

                    foreach (var (instance, methodInfo) in PrivateMethodList)
                    {
                        var attribute = methodInfo.GetCustomAttribute<OnKeywordAttribute>();
                        if (attribute == null)
                            continue;
                        if (attribute.RequireSuperUser && !config.SuperUserIds.Contains(userId))
                            continue;
                        switch (attribute.KeywordPolicy)
                        {
                            case KeywordPolicy.AllMatch:
                                if (!attribute.Keywords.Contains(pureText))
                                    continue;
                                break;
                            case KeywordPolicy.Trim:
                                if (!attribute.Keywords.Contains(pureText.Trim()))
                                    continue;
                                break;
                            case KeywordPolicy.Contains:
                                if (!attribute.Keywords.Any(k => pureText.Contains(k)))
                                    continue;
                                break;
                            case KeywordPolicy.Begin:
                                if (!attribute.Keywords.Any(k => pureText.StartsWith(k)))
                                    continue;
                                break;
                            case KeywordPolicy.AcceptAll:
                                break;
                            case KeywordPolicy.Regex:
                                var regexs = from s in attribute.Keywords select new Regex(s);
                                if (!regexs.Any(r => r.IsMatch(pureText)))
                                    continue;
                                break;
                            default:
                                continue;
                        }

                        var parameters = methodInfo.GetParameters();
                        switch (parameters.Length)
                        {
                            case 0 when methodInfo.ReturnType == typeof(Task):
                            {
                                var task = (Task) methodInfo.Invoke(instance, null);
                                if (task != null)
                                    await task;
                            }
                                break;
                            case 0:
                                methodInfo.Invoke(instance, null);
                                break;
                            case 1 when parameters[0].ParameterType == typeof(long):
                            {
                                if (methodInfo.ReturnType == typeof(Task))
                                {
                                    var task = (Task) methodInfo.Invoke(instance, new object[] {userId});
                                    if (task != null)
                                        await task;
                                }
                                else
                                {
                                    methodInfo.Invoke(instance, new object[] {userId});
                                }

                                break;
                            }
                            case 2 when parameters[0].ParameterType == typeof(long) &&
                                        parameters[1].ParameterType == typeof(Message):
                            {
                                if (methodInfo.ReturnType == typeof(Task))
                                {
                                    var task = (Task) methodInfo.Invoke(instance, new object[] {userId, message});
                                    if (task != null)
                                        await task;
                                }
                                else
                                {
                                    methodInfo.Invoke(instance, new object[] {userId, message});
                                }

                                break;
                            }
                            default:
                                Logger.Error(
                                    $"Unsupported parameter type on method {methodInfo.Name}, type {instance.GetType().Name}");
                                break;
                        }
                    }
                });

                messageProvider.SubscribeGroupMessage(async (message, groupId, userId, atMe) =>
                {
                    var pureText = message.ToPureString();
                    Logger.Info(atMe
                        ? $"Received an atMe message {pureText} from {userId} at group {groupId}."
                        : $"Received a group message {pureText} from {userId} at group {groupId}.");


                    var dict = atMe ? GroupAtMeMethodDirectory : GroupMethodDirectory;
                    var enabled = from record in context.ServiceRecords
                        where record.GroupId == groupId && record.IsEnabled
                        select record.ServiceName;

                    foreach (var service in enabled)
                    foreach (var (instance, methodInfo) in dict[service])
                    {
                        var attribute = methodInfo.GetCustomAttribute<OnKeywordAttribute>();
                        if (attribute == null)
                            continue;
                        if (attribute.RequireSuperUser && !config.SuperUserIds.Contains(userId))
                            continue;
                        switch (attribute.KeywordPolicy)
                        {
                            case KeywordPolicy.AllMatch:
                                if (!attribute.Keywords.Contains(pureText))
                                    continue;
                                break;
                            case KeywordPolicy.Trim:
                                if (!attribute.Keywords.Contains(pureText.Trim()))
                                    continue;
                                break;
                            case KeywordPolicy.Contains:
                                if (!attribute.Keywords.Any(k => pureText.Contains(k)))
                                    continue;
                                break;
                            case KeywordPolicy.Begin:
                                if (!attribute.Keywords.Any(k => pureText.StartsWith(k)))
                                    continue;
                                break;
                            case KeywordPolicy.AcceptAll:
                                break;
                            case KeywordPolicy.Regex:
                                var regexs = from s in attribute.Keywords select new Regex(s);
                                if (!regexs.Any(r => r.IsMatch(pureText)))
                                    continue;
                                break;
                            default:
                                continue;
                        }

                        var parameters = methodInfo.GetParameters();
                        switch (parameters.Length)
                        {
                            case 0 when methodInfo.ReturnType == typeof(Task):
                            {
                                var task = (Task) methodInfo.Invoke(instance, null);
                                if (task != null)
                                    await task;
                            }
                                break;
                            case 0:
                                methodInfo.Invoke(instance, null);
                                break;
                            case 2 when parameters[0].ParameterType == typeof(long) &&
                                        parameters[1].ParameterType == typeof(long):
                            {
                                if (methodInfo.ReturnType == typeof(Task))
                                {
                                    var task = (Task) methodInfo.Invoke(instance,
                                        new object[] {groupId, userId});
                                    if (task != null)
                                        await task;
                                }
                                else
                                {
                                    methodInfo.Invoke(instance, new object[] {groupId, userId});
                                }

                                break;
                            }
                            case 3 when parameters[0].ParameterType == typeof(long) &&
                                        parameters[1].ParameterType == typeof(long) &&
                                        parameters[2].ParameterType == typeof(Message):
                            {
                                if (methodInfo.ReturnType == typeof(Task))
                                {
                                    var task = (Task) methodInfo.Invoke(instance,
                                        new object[] {groupId, userId, message});
                                    if (task != null)
                                        await task;
                                }
                                else
                                {
                                    methodInfo.Invoke(instance, new object[] {groupId, userId, message});
                                }

                                break;
                            }
                            default:
                                Logger.Error(
                                    $"Unsupported parameter type on method {methodInfo.Name}, type {instance.GetType().Name}");
                                break;
                        }
                    }
                });


                Logger.Info("Start endded, now listening.");

                //Start service loop
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Logger.Info("Ctrl+C pressed, ending.");
                    Environment.Exit(0);
                };
                var lastTime = (-1, -1);
                while (true)
                {
                    var time = (DateTime.Now.Hour, DateTime.Now.Minute);
                    if (time == lastTime)
                    {
                        await Task.Delay(100);
                        continue;
                    }

                    if (ScheduledTaskDictionary.TryGetValue(time, out var list))
                        foreach (var (instance, methodInfo) in list)
                        {
                            var task = (Task) methodInfo.Invoke(instance, null);
                            if (task != null)
                                await task;
                        }

                    lastTime = time;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
            }
        }
    }
}