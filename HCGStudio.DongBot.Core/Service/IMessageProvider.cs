using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    /// <summary>
    ///     消息提供源
    /// </summary>
    public interface IMessageProvider
    {
        /// <summary>
        ///     注册私聊信息事件
        /// </summary>
        /// <param name="action">注册的事件，参数分别为消息以及发送用户Id</param>
        void SubscribePrivateMessage(Action<Message, long> action);

        /// <summary>
        ///     注册群组消息事件
        /// </summary>
        /// <param name="action">注册的事件，参数分别为消息，群组Id，发送者Id，该消息是否为指定消息</param>
        void SubscribeGroupMessage(Action<Message, long, long, bool> action);

        /// <summary>
        ///     获取加入所有组的Id
        /// </summary>
        /// <returns>所有加入组的集合</returns>
        Task<List<long>> GetAllGroupsAsync();

        /// <summary>
        ///     获取群组的名称
        /// </summary>
        /// <param name="groupId">群组Id</param>
        /// <returns>群组名称</returns>
        Task<string> GetGroupNameAsync(long groupId);

        /// <summary>
        ///     获取群组中用户名称
        /// </summary>
        /// <param name="groupId">群组Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns>群组中用户名称</returns>
        Task<string> GetGroupUserNameAsync(long groupId, long userId);
    }
}