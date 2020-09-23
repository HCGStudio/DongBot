using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     复合消息
    /// </summary>
    public class UnionMessage : Message, IUnionMessage
    {
        internal UnionMessage(IReadOnlyList<Message> messages)
        {
            Messages = messages;
        }

        /// <inheritdoc />
        public IReadOnlyList<Message> Messages { get; }
    }
}