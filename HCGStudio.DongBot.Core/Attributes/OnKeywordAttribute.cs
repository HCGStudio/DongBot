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
        public InvokePolicies InvokePolicies { get; set; } = InvokePolicies.Private;
    }

    public enum KeywordPolicy
    {
        AllMatch,
        Trim,
        Contains,
        Begin,
        AcceptAll,
        Regex
    }

    [Flags]
    public enum InvokePolicies
    {
        Group = 1 << 0,
        GroupAt = 1 << 1,
        Private = 1 << 2
    }
}