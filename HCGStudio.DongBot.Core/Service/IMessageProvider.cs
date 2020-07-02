using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    public interface IMessageProvider
    {
        void SubscribePrivateMessage(Action<Message, long> action);
        void SubscribeGroupMessage(Action<Message, long, long, bool> action);
        Task<List<long>> GetAllGroupsAsync();
        Task<string> GetGroupNameAsync(long groupId);
        Task<string> GetGroupUserNameAsync(long groupId, long userId);
    }
}