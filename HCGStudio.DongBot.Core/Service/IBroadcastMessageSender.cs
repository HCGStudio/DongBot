using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;

namespace HCGStudio.DongBot.Core.Service
{
    /// <summary>
    ///     播报服务
    /// </summary>
    /// <typeparam name="TService">使用播报服务的类</typeparam>
    public interface IBroadcastMessageSender<TService>
    {
        /// <summary>
        ///     播报所有已经启用指定服务的类
        /// </summary>
        /// <param name="message">播报的信息</param>
        /// <param name="interval">播报间隔（毫秒）</param>
        /// <returns></returns>
        Task BroadcastAllEnabled(Message message, int interval = 100);
    }
}