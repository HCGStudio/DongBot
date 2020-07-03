using System;
using System.Reflection;

namespace HCGStudio.DongBot.Core.Attributes
{
    /// <summary>
    ///     服务信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        ///     初始化新的服务信息
        /// </summary>
        /// <param name="serviceName"></param>
        public ServiceAttribute(string serviceName)
        {
            Name = serviceName;
        }

        /// <summary>
        ///     服务的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     是否需要自动启用该服务
        /// </summary>
        public bool AutoEnable { get; set; } = false;

        /// <summary>
        ///     获取服务名称
        /// </summary>
        /// <param name="type">服务的类型</param>
        /// <returns>服务名称</returns>
        public static string GetServiceName(Type type)
        {
            var attribute = type.GetCustomAttribute<ServiceAttribute>();
            return attribute == null ? string.Empty : attribute.Name;
        }
    }
}