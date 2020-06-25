using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Service
{
    public interface IMessageProvider
    {
        void SubscribePrivateMessage(Action<Message.Message, long> action);

        //OnGroupMessage(Message message, long groupId, long userId, bool atMe)
        void SubscribeGroupMessage(Action<Message.Message, long, long, bool> action);
    }
}
