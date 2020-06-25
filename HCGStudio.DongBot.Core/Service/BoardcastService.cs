using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    public interface IBroadcastService
    {
        Task BroadcastAllEnabled(Message message, int interval = 100);
    }
}