using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;

namespace HCGStudio.DongBot.Core.Message
{
    public class SimpleMessage :Message, ISimpleMessage
    {
        public string Content { get; }
        public SimpleMessage(string content)
        {
            Content = content;
        }

        public static SimpleMessage FromString(string message)
        {
            return new SimpleMessage(message);
        }

        public static explicit operator SimpleMessage(string message)
        {
            return new SimpleMessage(message);
        }

        public static implicit operator string(SimpleMessage message)
        {
            return message?.Content ?? string.Empty;
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
