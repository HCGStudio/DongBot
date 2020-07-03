namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     简单消息的
    /// </summary>
    public class SimpleMessage : Message, ISimpleMessage
    {
        /// <summary>
        ///     构建一个简单消息
        /// </summary>
        /// <param name="content">消息的内容</param>
        public SimpleMessage(string content)
        {
            Content = content;
        }

        /// <inheritdoc />
        public string Content { get; }

        /// <summary>
        ///     从字符串转换简单消息
        /// </summary>
        /// <param name="message">消息的内容</param>
        /// <returns>转换的消息</returns>
        public static SimpleMessage FromString(string message)
        {
            return new SimpleMessage(message);
        }

        /// <summary>
        ///     将字符串转化为简单消息
        /// </summary>
        /// <param name="message">消息的内容</param>
        public static explicit operator SimpleMessage(string message)
        {
            return new SimpleMessage(message);
        }

        /// <summary>
        ///     将消息转化为字符串
        /// </summary>
        /// <param name="message">转化的消息</param>
        public static implicit operator string(SimpleMessage message)
        {
            return message?.Content ?? string.Empty;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Content;
        }
    }
}