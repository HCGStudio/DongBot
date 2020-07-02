using System;

namespace HCGStudio.DongBot.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScheduleTaskAttribute : Attribute
    {
        public ScheduleTaskAttribute(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}