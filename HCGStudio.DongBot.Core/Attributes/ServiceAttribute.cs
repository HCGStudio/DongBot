using System;

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
    }
}