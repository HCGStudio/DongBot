using System.Reflection;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.App.SystemService
{
    [Service("Core", AutoEnable = true)]
    public class VersionService
    {
        private IMessageSender MessageSender { get; set; }


        [OnKeyword("版本", "Version", InvokePolicies = InvokePolicies.Private, KeywordPolicy = KeywordPolicy.Trim)]
        public async Task VersionQuery(long senderUserId)
        {
            await MessageSender.SendPrivateAsync(senderUserId,
                (SimpleMessage) $"DongBot {Assembly.GetExecutingAssembly().GetName().Version}");
        }
    }
}