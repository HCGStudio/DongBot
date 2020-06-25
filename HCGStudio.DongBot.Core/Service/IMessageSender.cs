using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Message;

namespace HCGStudio.DongBot.Core.Service
{
    public interface IMessageSender
    {
        Task<bool> SendGroupAsync(int groupId,Message.Message message);
        Task<bool> SendPrivateAsync(int userId,Message.Message message);
    }
}
