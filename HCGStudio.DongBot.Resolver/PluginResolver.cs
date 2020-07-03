using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HCGStudio.DongBot.Resolver
{
    public class PluginResolver
    {
        public Dictionary<string, List<(Type, MethodInfo)>> GroupMethodDirectory { get; } =
            new Dictionary<string, List<(Type, MethodInfo)>>();

        public Dictionary<string, List<(Type, MethodInfo)>> GroupAtMeMethodDirectory { get; } =
            new Dictionary<string, List<(Type, MethodInfo)>>();

        public List<(Type, MethodInfo)> PrivateMethodList { get; } = new List<(Type, MethodInfo)>();

        public Dictionary<(int, int), List<(Type, MethodInfo)>> ScheduledTaskDictionary { get; } =
            new Dictionary<(int, int), List<(Type, MethodInfo)>>();

        public HashSet<ServiceAttribute> Services { get; } = new HashSet<ServiceAttribute>();
        private readonly ILogger<PluginResolver> _logger;

        public PluginResolver(ILogger<PluginResolver> logger)
        {
            _logger = logger;
        }

        public void Load(IServiceCollection services, Assembly assembly, bool builtIn = false)
        {
            foreach (var type in assembly.GetTypes())
            {
                var service = type.GetCustomAttribute<ServiceAttribute>();
                if (service == null)
                    continue;
                if (string.IsNullOrWhiteSpace(service.Name) || service.Name.Contains(' '))
                {
                    _logger.Log(LogLevel.Error, $"Service name {service.Name} is invalid, not loading.");
                    continue;
                }

                if (!builtIn && service.Name == "Core")
                {
                    _logger.Log(LogLevel.Error,
                        $"Non builtin class {type.FullName} has service name Core, will not loading.");
                    continue;
                }

                _logger.Log(LogLevel.Information, $"Now loading service {service.Name}.");

                Services.Add(service);

                //Add to dictionary if service name not added before
                if (!GroupMethodDirectory.ContainsKey(service.Name))
                    GroupMethodDirectory.Add(service.Name, new List<(Type, MethodInfo)>());

                if (!GroupAtMeMethodDirectory.ContainsKey(service.Name))
                    GroupAtMeMethodDirectory.Add(service.Name, new List<(Type, MethodInfo)>());


                services.AddTransient(type);

                //Find marked method
                foreach (var methodInfo in type.GetMethods())
                {
                    var keyword = methodInfo.GetCustomAttribute<OnKeywordAttribute>();
                    if (keyword == null)
                        continue;
                    if ((keyword.InvokePolicies & InvokePolicies.Private) != 0)
                        PrivateMethodList.Add((type, methodInfo));
                    if ((keyword.InvokePolicies & InvokePolicies.Group) != 0)
                        GroupMethodDirectory[service.Name].Add((type, methodInfo));
                    if ((keyword.InvokePolicies & InvokePolicies.GroupAt) != 0)
                        GroupAtMeMethodDirectory[service.Name].Add((type, methodInfo));
                }

                foreach (var methodInfo in type.GetMethods())
                {
                    var schedules = methodInfo.GetCustomAttributes<ScheduleTaskAttribute>();
                    foreach (var schedule in schedules)
                    {
                        if (methodInfo.ReturnType != typeof(Task) || methodInfo.GetParameters().Length != 0)
                        {
                            _logger.Log(LogLevel.Error,
                                $"Schedule task only supports async Task with no parameters, {methodInfo.Name} will not load.");
                            continue;
                        }

                        if (ScheduledTaskDictionary.TryGetValue((schedule.Hour, schedule.Minute), out var list))
                            list.Add((type, methodInfo));
                        else
                            ScheduledTaskDictionary.Add((schedule.Hour, schedule.Minute),
                                new List<(Type, MethodInfo)>
                                {
                                    (type, methodInfo)
                                });
                    }
                }
            }
        }
    }
}