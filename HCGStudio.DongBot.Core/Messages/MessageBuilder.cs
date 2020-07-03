using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     用于构建复杂消息
    /// </summary>
    public class MessageBuilder
    {
        private readonly List<Message> _messages;

        /// <summary>
        ///     初始化构建复杂消息的类
        /// </summary>
        /// <param name="capacity">初始元素个数</param>
        public MessageBuilder(int capacity = 8)
        {
            _messages = new List<Message>(capacity);
        }

        /// <summary>
        ///     将消息加入到消息构建器中
        /// </summary>
        /// <param name="message">新添加的消息</param>
        /// <returns>添加消息后的本实例</returns>
        public MessageBuilder Append(Message message)
        {
            if (message is IUnionMessage ms)
                _messages.AddRange(ms.Messages);
            else
                _messages.Add(message);
            return this;
        }

        /// <summary>
        ///     获取构建的复合消息
        /// </summary>
        /// <returns>构建的复合消息</returns>
        public UnionMessage ToMessage()
        {
            return new UnionMessage(_messages);
        }
    }
}