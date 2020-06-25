using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Messages
{
    public class MessageBuilder
    {
        private readonly List<Message> _messages;

        public MessageBuilder(int capacity = 8)
        {
            _messages = new List<Message>(capacity);
        }

        public MessageBuilder Append(Message message)
        {
            if (message is IUnionMessage ms)
                _messages.AddRange(ms.Messages);
            else
                _messages.Add(message);
            return this;
        }

        public UnionMessage ToMessage()
        {
            return new UnionMessage(_messages);
        }
    }
}