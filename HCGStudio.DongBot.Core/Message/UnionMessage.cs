using System;
using System.Collections.Generic;
using System.Text;

namespace HCGStudio.DongBot.Core.Message
{
    public class UnionMessage : Message, IUnionMessage
    {
        public IEnumerable<Message> Messages { get; }

        internal UnionMessage(IEnumerable<Message> messages)
        {
            Messages = messages;
        }
    }
}
