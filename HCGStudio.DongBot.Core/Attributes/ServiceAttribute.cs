using System;
using System.Collections.Generic;
using System.Text;

namespace HCGStudio.DongBot.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public string Name { get; }
        public bool AutoEnable { get; set; } = false;
        public ServiceAttribute(string serviceName)
        {
            Name = serviceName;
        }
    }
}
