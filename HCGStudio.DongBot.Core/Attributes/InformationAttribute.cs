using System;

namespace HCGStudio.DongBot.Core.Attributes
{
    /// <summary>
    ///     提供方法的帮助信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InformationAttribute : Attribute
    {
        /// <summary>
        ///     初始化帮助信息
        /// </summary>
        /// <param name="name">触发命令</param>
        /// <param name="group">帮助分组</param>
        /// <param name="content">帮助详情</param>
        public InformationAttribute(string name, string group, string content)
        {
            Name = name;
            Group = group;
            Content = content;
        }

        /// <summary>
        ///     触发命令
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     帮助分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     帮助详情
        /// </summary>
        public string Content { get; set; }
    }
}