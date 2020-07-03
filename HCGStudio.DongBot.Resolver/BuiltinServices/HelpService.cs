using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.Resolver.BuiltinServices
{
    /// <summary>
    ///     为Dong! Bot提供内置的帮助功能
    /// </summary>
    [Service("Core")]
    public class HelpService
    {
        internal static readonly HashSet<InformationAttribute> AllHelps = new HashSet<InformationAttribute>();
        private readonly IMessageSender _messageSender;

        /// <summary>
        ///     构建新的帮助服务，请使用依赖注入的框架创建实例，直接调用可能会出现问题
        /// </summary>
        /// <param name="messageSender">消息发送器</param>
        public HelpService(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        /// <summary>
        ///     获取所有帮助
        /// </summary>
        /// <param name="groupId">发送群组的Id</param>
        /// <param name="userId">发送用户的Id</param>
        /// <returns></returns>
        [OnKeyword("帮助", InvokePolicies = InvokePolicies.Group)]
        [Information("帮助", "核心", "显示所有指令的帮助")]
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
                    sb.AppendLine($"{informationAttribute.Name}: {informationAttribute.Content}");
            }

            await _messageSender.SendGroupAsync(groupId, new SimpleMessage(sb.ToString())).ConfigureAwait(false);
        }
    }
}