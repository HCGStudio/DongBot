using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.Resolver.BuiltinServices
{
    [Service("Core")]
    public class HelpService
    {
        internal static readonly HashSet<InformationAttribute> AllHelps = new HashSet<InformationAttribute>();
        private readonly IMessageSender _messageSender;

        public HelpService(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [OnKeyword("帮助",InvokePolicies = InvokePolicies.Group)]
        [Information("帮助","核心","显示所有指令的帮助")]
        public async Task GetHelpAsync(long groupId, long userId)
        {
            var sb = new StringBuilder();
            var helpMessage = from info in AllHelps
                orderby info.Name
                group info by info.Group
                into newGroup
                orderby newGroup.Key
                select newGroup;
            sb.AppendLine("Dong! Bot 帮助");
            foreach (var helpGroup in helpMessage)
            {
                sb.AppendLine($"===={helpGroup.Key}模块====");
                foreach (var informationAttribute in helpGroup)
                {
                    sb.AppendLine($"{informationAttribute.Name}: {informationAttribute.Content}");
                }
            }

            await _messageSender.SendGroupAsync(groupId, new SimpleMessage(sb.ToString()));
        }
    }
}
