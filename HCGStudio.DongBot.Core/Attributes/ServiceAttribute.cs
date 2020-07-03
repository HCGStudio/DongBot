using System;
using System.Reflection;

namespace HCGStudio.DongBot.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(string serviceName)
        {
            Name = serviceName;
        }

        public string Name { get; }
        public bool AutoEnable { get; set; } = false;

        public static string GetServiceName(Type type)
        {
            var attribute = type.GetCustomAttribute<ServiceAttribute>();
            return attribute == null ? string.Empty : attribute.Name;
        }
    }
}