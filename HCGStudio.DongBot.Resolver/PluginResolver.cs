using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Resolver.BuiltinServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HCGStudio.DongBot.Resolver
{
    /// <summary>
    ///     为Dong! Bot提供插件解析器
    /// </summary>
    public class PluginResolver
    {
        private readonly ILogger<PluginResolver> _logger;

        /// <summary>
        ///     创建一个新的插件解析器，请使用依赖注入的框架创建实例，直接调用可能会出现问题
        /// </summary>
        /// <param name="logger">日志记录类</param>
        public PluginResolver(ILogger<PluginResolver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     服务名称与群组内触发方法对应的字典
        /// </summary>
        public Dictionary<string, List<(Type, MethodInfo)>> GroupMethodDirectory { get; } =
            new Dictionary<string, List<(Type, MethodInfo)>>();

        /// <summary>
        ///     服务名称与群组内指定触发方法对应的字典
        /// </summary>
        public Dictionary<string, List<(Type, MethodInfo)>> GroupAtMeMethodDirectory { get; } =
            new Dictionary<string, List<(Type, MethodInfo)>>();

        /// <summary>
        ///     私聊触发方法的列表
        /// </summary>
        public List<(Type, MethodInfo)> PrivateMethodList { get; } = new List<(Type, MethodInfo)>();

        /// <summary>
        ///     计划任务的时间与方法对应的字典
        /// </summary>
        public Dictionary<(int, int), List<(Type, MethodInfo)>> ScheduledTaskDictionary { get; } =
            new Dictionary<(int, int), List<(Type, MethodInfo)>>();

        /// <summary>
        ///     储存所有服务名称的哈希表
        /// </summary>
        public HashSet<ServiceAttribute> Services { get; } = new HashSet<ServiceAttribute>();

        /// <summary>
        ///     加载自带自带服务
        /// </summary>
        /// <param name="services">依赖注入服务集合</param>
        public void LoadBuiltinServices(IServiceCollection services)
        {
#pragma warning disable CA1303 // 请不要将文本作为本地化参数传递
            _logger.LogInformation("Now loading builtin services.");
#pragma warning restore CA1303 // 请不要将文本作为本地化参数传递
            Load(services, Assembly.GetCallingAssembly(), true);
            Load(services, typeof(HelpService).Assembly, true);
        }

        /// <summary>
        ///     加载指定插件
        /// </summary>
        /// <param name="services">依赖注入服务集合</param>
        /// <param name="assembly">依赖的汇编</param>
        /// <param name="builtIn">是否为额外添加的内置插件</param>
        public void Load(IServiceCollection services, Assembly assembly, bool builtIn = false)
        {
            foreach (var type in assembly?.GetTypes()!)
            {
                var service = type.GetCustomAttribute<ServiceAttribute>();
                if (service == null)
                    continue;
                if (string.IsNullOrWhiteSpace(service.Name) ||
                    service.Name.Contains(' ', StringComparison.CurrentCulture))
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
                    //Add help
                    var info = methodInfo.GetCustomAttribute<InformationAttribute>();
                    if (info != null)
                        HelpService.AllHelps.Add(info);
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