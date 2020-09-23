using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     复合消息的接口
    /// </summary>
    public interface IUnionMessage
    {
        /// <summary>
        ///     复合消息内部组成消息
        /// </summary>
        IReadOnlyList<Message> Messages { get; }
    }
}