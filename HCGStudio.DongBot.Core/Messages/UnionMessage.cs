using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Messages
{
    public class UnionMessage : Message, IUnionMessage
    {
        internal UnionMessage(IEnumerable<Message> messages)
        {
            Messages = messages;
        }

        public IEnumerable<Message> Messages { get; }
    }
}