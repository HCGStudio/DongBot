using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    public interface IMessageSender
    {
        Task<bool> SendGroupAsync(int groupId, Message message);
        Task<bool> SendPrivateAsync(int userId, Message message);
    }
}