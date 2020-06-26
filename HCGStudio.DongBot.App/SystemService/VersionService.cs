using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.App.SystemService
{
    [Service("Core", AutoEnable = true)]
    public class VersionService
    {
        private readonly IMessageSender _messageSender;

        public VersionService(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [OnKeyword("版本", "Version", InvokePolicies = InvokePolicies.Private, KeywordPolicy = KeywordPolicy.Trim)]
        public async Task VersionQuery(long senderUserId)
        {
            await _messageSender.SendPrivateAsync(senderUserId, (SimpleMessage)$"DongBot {Assembly.GetExecutingAssembly().GetName().Version}");
        }

        [OnKeyword("图灵测试", InvokePolicies = InvokePolicies.Private, KeywordPolicy = KeywordPolicy.Trim)]
        public async Task Turing(long senderUserId)
        {
            await _messageSender.SendPrivateAsync(senderUserId, (SimpleMessage)"Hello, world! 父亲！");
        }
    }
}
