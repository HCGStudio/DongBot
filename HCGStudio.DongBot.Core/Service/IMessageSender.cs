using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    public interface IMessageSender
    {
        Task<bool> SendGroupAsync(long groupId, Message message);
        Task<bool> SendPrivateAsync(long userId, Message message);
    }
}