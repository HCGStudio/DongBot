using System;
using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnKeywordAttribute : Attribute
    {
        public OnKeywordAttribute(params string[] keywords)
        {
            Keywords = new List<string>(keywords);
        }

        public List<string> Keywords { get; }
        public KeywordPolicy KeywordPolicy { get; set; } = KeywordPolicy.Trim;
        public InvokePolicy InvokePolicy { get; set; } = InvokePolicy.Private;
    }

    public enum KeywordPolicy
    {
        AllMatch,
        Trim,
        Contains,
        Begin,
        AcceptAll,
        Regex,
        ToLower
    }

    public enum InvokePolicy
    {
        Group,
        GroupAt,
        Private
    }
}