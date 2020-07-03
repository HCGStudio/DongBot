using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HCGStudio.DongBot.App.Models;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;
using HCGStudio.DongBot.Resolver;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HCGStudio.DongBot.App
{
    public class Program
    {
        private static ILogger<Program> _logger;

        private static async Task Main(string[] args)
        {
            //Build config
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "config.json"));
            //Add service config files
            foreach (var configFile in Directory.CreateDirectory("PluginConfig").GetFiles()
                .Where(f => f.Extension == ".json").Select(f => f.FullName))
                configBuilder.AddJsonFile(configFile, true, true);
            var config = configBuilder.Build();
            var startup = new Startup(config);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            services.AddTransient<PluginResolver>();
            var resolver = services.BuildServiceProvider().GetRequiredService<PluginResolver>();
            _logger = services.BuildServiceProvider().GetService<ILogger<Program>>();

            try
            {
                //Init EF core
                await using var context = new ApplicationContext();
                await context.Database.EnsureCreatedAsync();

                await using var provider = services.BuildServiceProvider();
                var messageProvider = provider.GetRequiredService<IMessageProvider>();
                var groups = await messageProvider.GetAllGroupsAsync();


                //Load service
                _logger.LogInformation("Now loading builtin services.");
                resolver.Load(services, Assembly.GetExecutingAssembly(), true);

                //Load plugins
                _logger.LogInformation("Now load plugins from plugins dir.");

                foreach (var file in Directory.CreateDirectory("plugins").GetFiles()
                    .Where(file => file.Extension == ".dll").Select(file => file.FullName))
                    try
                    {
                        var assembly = Assembly.LoadFrom(file);
                        resolver.Load(services, assembly);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"File {file} is not a valid plugin.");
                        _logger.LogError(e.ToString());
                    }


                //Load Light-Plugins
                Directory.CreateDirectory("temp");
                _logger.LogInformation("Now compiling light plugins.");
                foreach (var file in Directory.CreateDirectory("light-plugins").GetFiles()
                    .Where(file => file.Extension == ".cs" || file.Extension == ".lpg").Select(file => file))
                {
                    var compile = CSharpCompilation.Create($"{file.Name}.g",
                        new[] {CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(file.FullName))},
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
                        resolver.Load(services, Assembly.Load(stream.ToArray()));
                    }
                    else
                    {
                        _logger.LogError($"Compile failed on file {file.Name}.");
                        _logger.LogError($"{compileResult.Diagnostics}");
                    }
                }

                //Add database record for new service.
                foreach (var service in resolver.Services)
                {
                    var added = from record in context.ServiceRecords
                        where record.ServiceName == service.Name
                        select record.GroupId;
                    foreach (var g in groups.Where(g => !added.Contains(g)))
                        await context.ServiceRecords.AddAsync(new ServiceRecord
                            {GroupId = g, IsEnabled = service.AutoEnable, ServiceName = service.Name});
                }

                //Save changes
                await context.SaveChangesAsync();

                //Load superusers
                var superUsers = config.GetSection("SuperUsers").GetChildren().Select(s => s.Value)
                    .SelectMany(s => long.TryParse(s, out var uid) ? new[] {uid} : null).ToArray();

                messageProvider.SubscribePrivateMessage(async (message, userId) =>
                {
                    var pureText = message.ToPureString();
                    _logger.LogInformation($"Received a private message {pureText} from {userId}.");

                    foreach (var (type, methodInfo) in resolver.PrivateMethodList)
                    {
                        var attribute = methodInfo.GetCustomAttribute<OnKeywordAttribute>();
                        if (attribute == null)
                            continue;
                        if (attribute.RequireSuperUser && !superUsers.Contains(userId))
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
                        var instance = provider.GetService(type);
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
                                _logger.LogError(
                                    $"Unsupported parameter type on method {methodInfo.Name}, type {instance.GetType().Name}");
                                break;
                        }
                    }
                });

                messageProvider.SubscribeGroupMessage(async (message, groupId, userId, atMe) =>
                {
                    var pureText = message.ToPureString();
                    _logger.LogInformation(atMe
                        ? $"Received an atMe message {pureText} from {userId} at group {groupId}."
                        : $"Received a group message {pureText} from {userId} at group {groupId}.");


                    var dict = atMe ? resolver.GroupAtMeMethodDirectory : resolver.GroupMethodDirectory;
                    var enabled = from record in context.ServiceRecords
                        where record.GroupId == groupId && record.IsEnabled
                        select record.ServiceName;

                    foreach (var service in enabled.AsEnumerable().Where(dict.ContainsKey))
                    foreach (var (type, methodInfo) in dict[service])
                    {
                        var attribute = methodInfo.GetCustomAttribute<OnKeywordAttribute>();
                        if (attribute == null)
                            continue;
                        if (attribute.RequireSuperUser && !superUsers.Contains(userId))
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
                        var instance = provider.GetService(type);
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
                                _logger.LogError(
                                    $"Unsupported parameter type on method {methodInfo.Name}, type {type.Name}");
                                break;
                        }
                    }
                });


                _logger.LogInformation("Start ended, now listening.");

                //Start service loop
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    _logger.LogInformation("Ctrl+C pressed, ending.");
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

                    if (resolver.ScheduledTaskDictionary.TryGetValue(time, out var list))
                        foreach (var (type, methodInfo) in list)
                        {
                            var instance = provider.GetService(type);
                            var task = (Task) methodInfo.Invoke(instance, null);
                            if (task != null)
                                await task;
                        }

                    lastTime = time;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
    }
}