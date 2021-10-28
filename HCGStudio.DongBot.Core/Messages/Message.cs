using System.Text;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     消息类
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        ///     默认的初始化
        /// </summary>
        protected Message()
        {
            MessageType = GetType().Name;
        }

        /// <summary>
        ///     消息的类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        ///     将消息转化为纯字符串，非文字类型转化为空格
        /// </summary>
        /// <returns>纯文字的消息</returns>
        public string ToPureString()
        {
            switch (this)
            {
                case SimpleMessage simpleMessage:
                    return simpleMessage.Content;
                case UnionMessage unionMessage:
                    var sb = new StringBuilder();
                    foreach (var unionMessageMessage in unionMessage.Messages)
                        sb.Append(unionMessageMessage.ToPureString());

                    return sb.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}