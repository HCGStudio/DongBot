using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using HCGStudio.DongBot.App.Models;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Service;
using HCGStudio.DongBot.CqHttp;
using Newtonsoft.Json;
using NLog;

namespace HCGStudio.DongBot.App
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Dictionary<string, List<(object, MethodInfo)>> GroupMethodDirectory = new Dictionary<string, List<(object, MethodInfo)>>();
        private static readonly List<(object, MethodInfo)> PrivateMethodList = new List<(object, MethodInfo)>();

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
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //Init EF core
                await using var context = new ApplicationContext();
                await context.Database.EnsureCreatedAsync();

                await using var container = builder.Build();
                var messageProvider = container.Resolve<IMessageProvider>();
                var messageSender = container.Resolve<IMessageSender>();
                Logger.Info("Now loading builtin services.");
                var groups = await messageProvider.GetAllGroupsAsync();

                //Load service
                Logger.Info("Now loading builtin services.");
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    var service = type.GetCustomAttribute<ServiceAttribute>();
                    if (service == null)
                        continue;
                    if (string.IsNullOrWhiteSpace(service.Name))
                    {
                        Logger.Error($"Service name {service.Name} is invaild, not loading.");
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
                        { GroupId = groupId, IsEnabled = service.AutoEnable });
                    }

                    //Add to dictionary if service name not added before
                    if (!GroupMethodDirectory.ContainsKey(service.Name))
                        GroupMethodDirectory.Add(service.Name, new List<(object, MethodInfo)>());

                    //Create instance
                    object instance = null;
                    var constructor = type.GetConstructor(new[] { typeof(IMessageSender) });
                    if (constructor != null)
                    {
                        instance = constructor.Invoke(new object[] { messageSender });
                    }
                    else
                    {
                        constructor = type.GetConstructor(new Type[] { });
                        if (constructor != null)
                            instance = constructor.Invoke(new object[] { messageSender });
                    }

                    if (instance == null)
                    {
                        Logger.Error(
                            $"Unsupported parameter type on constructor {type.Name}.");
                        continue;
                    }
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
                    }
                }

                //Save changes
                await context.SaveChangesAsync();

                messageProvider.SubscribePrivateMessage(async (message, userId) =>
                {
                    var pureText = message.ToPureString();
                    Logger.Info($"Received message {pureText}.");

                    foreach (var (instance, methodInfo) in PrivateMethodList)
                    {
                        var attribute = methodInfo.GetCustomAttribute<OnKeywordAttribute>();
                        if(attribute == null)
                            continue;
                        switch (attribute.KeywordPolicy)
                        {
                            case KeywordPolicy.AllMatch:
                                if(!attribute.Keywords.Contains(pureText))
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
                                if(!regexs.Any(r => r.IsMatch(pureText)))
                                    continue;
                                break;
                            default:
                                continue;
                        }

                        var parameters = methodInfo.GetParameters();
                        if (parameters.Length == 0)
                        {
                            if (methodInfo.ReturnType == typeof(Task))
                                // ReSharper disable once PossibleNullReferenceException
                                await (Task)methodInfo.Invoke(instance, null);
                            else
                                methodInfo.Invoke(instance, null);
                        }
                        else if (parameters[0].ParameterType == typeof(long))
                        {

                            if (methodInfo.ReturnType == typeof(Task))
                                // ReSharper disable once PossibleNullReferenceException
                                await (Task)methodInfo.Invoke(instance, new object[] { userId });
                            else
                                methodInfo.Invoke(instance, new object[] { userId });
                        }
                        else
                            Logger.Error(
                                $"Unsupported parameter type on method {methodInfo.Name}, type {instance.GetType().Name}");
                    }
                });

                Logger.Info("Start endded, now listening.");

                //Start service loop
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Logger.Info("Ctrl+C pressed, ending.");
                    Environment.Exit(0);
                };
                while (true)
                {
                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
            }
        }
    }
}