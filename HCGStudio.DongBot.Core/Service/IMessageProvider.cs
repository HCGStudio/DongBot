using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    public interface IMessageProvider
    {
        void SubscribePrivateMessage(Action<Message, long> action);

        //OnGroupMessage(Message message, long groupId, long userId, bool atMe)
        void SubscribeGroupMessage(Action<Message, long, long, bool> action);

        Task<List<long>> GetAllGroupsAsync();
    }
}