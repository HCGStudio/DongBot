using System;
using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Attributes
{
    /// <summary>
    ///     提供方法的触发信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OnKeywordAttribute : Attribute
    {
        /// <summary>
        ///     初始化方法的触发条件
        /// </summary>
        /// <param name="keywords">触发关键词</param>
        public OnKeywordAttribute(params string[] keywords)
        {
            Keywords = new List<string>(keywords);
        }

        /// <summary>
        ///     触发关键词
        /// </summary>
        public List<string> Keywords { get; }

        /// <summary>
        ///     关键词策略
        /// </summary>
        public KeywordPolicy KeywordPolicy { get; set; } = KeywordPolicy.Trim;

        /// <summary>
        ///     触发策略
        /// </summary>
        public InvokePolicies InvokePolicies { get; set; } = InvokePolicies.Private;

        /// <summary>
        ///     触发是否需要权限
        /// </summary>
        public bool RequireSuperUser { get; set; } = false;
    }

    /// <summary>
    ///     关键词策略
    /// </summary>
    public enum KeywordPolicy
    {
        /// <summary>
        ///     必须完全一致才触发
        /// </summary>
        AllMatch,

        /// <summary>
        ///     去掉空格后一致才触发
        /// </summary>
        Trim,

        /// <summary>
        ///     关键词为消息的子串就触发
        /// </summary>
        Contains,

        /// <summary>
        ///     关键词为消息的前缀就触发
        /// </summary>
        Begin,

        /// <summary>
        ///     无论消息如何都触发
        /// </summary>
        AcceptAll,

        /// <summary>
        ///     消息额能够匹配关键词中的正则就触发
        /// </summary>
        Regex
    }

    /// <summary>
    ///     触发策略
    /// </summary>
    [Flags]
    public enum InvokePolicies
    {
        /// <summary>
        ///     在群组中发送的消息符合关键词策略就触发
        /// </summary>
        Group = 1 << 0,

        /// <summary>
        ///     在群组中收到At自己的消息就触发
        /// </summary>
        GroupAt = 1 << 1,

        /// <summary>
        ///     私聊触发
        /// </summary>
        Private = 1 << 2
    }
}