using System;

namespace HCGStudio.DongBot.Core.Attributes
{
    /// <summary>
    ///     计划任务信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScheduleTaskAttribute : Attribute
    {
        /// <summary>
        ///     初始化新的计划任务
        /// </summary>
        /// <param name="hour">计划任务触发的小时</param>
        /// <param name="minute">计划任务触发的分钟</param>
        public ScheduleTaskAttribute(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        /// <summary>
        ///     触发小时
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        ///     触发分钟
        /// </summary>
        public int Minute { get; set; }
    }
}