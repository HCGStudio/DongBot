using System;

namespace HCGStudio.DongBot.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InformationAttribute : Attribute
    {
        public InformationAttribute(string name, string group, string content)
        {
            Name = name;
            Group = group;
            Content = content;
        }

        public string Name { get; set; }
        public string Group { get; set; }
        public string Content { get; set; }
    }
}