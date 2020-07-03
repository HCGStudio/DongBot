using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    /// <summary>
    ///     消息发送
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        ///     发送群组信息
        /// </summary>
        /// <param name="groupId">发送群组的Id</param>
        /// <param name="message">要发送的信息</param>
        /// <returns>消息是否发送成功</returns>
        Task<bool> SendGroupAsync(long groupId, Message message);

        /// <summary>
        ///     发送私聊信息
        /// </summary>
        /// <param name="userId">发送到的用户Id</param>
        /// <param name="message">要发送的消息</param>
        /// <returns>消息是否发送成功</returns>
        Task<bool> SendPrivateAsync(long userId, Message message);
    }
}