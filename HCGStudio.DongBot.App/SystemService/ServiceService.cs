using System.Linq;
using System.Threading.Tasks;
using HCGStudio.DongBot.App.Models;
using HCGStudio.DongBot.Core.Attributes;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.App.SystemService
{
    [Service("Core", AutoEnable = true)]
    public class ServiceService
    {
        private readonly IMessageSender _messageSender;

        public ServiceService(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [OnKeyword("查看服务", InvokePolicies = InvokePolicies.Group, RequireSuperUser = true)]
        [Information("查看服务", "核心", "列出所有服务，需要权限")]
        public async Task ListServices(long groupId, long userId)
        {
            await using var context = new ApplicationContext();
            var services = from record in context.ServiceRecords where record.GroupId == groupId select record;
            var messageBuilder = new MessageBuilder();
            messageBuilder.Append(new AtMessage(userId));
            messageBuilder.Append((SimpleMessage) $"群{groupId}服务列表：\n");
            foreach (var serviceRecord in services)
                messageBuilder.Append(
                    (SimpleMessage) $"{(serviceRecord.IsEnabled ? "√" : "×")} {serviceRecord.ServiceName}\n");

            await _messageSender.SendGroupAsync(groupId, messageBuilder.ToMessage());
        }

        [OnKeyword("启用", InvokePolicies = InvokePolicies.Group, KeywordPolicy = KeywordPolicy.Begin,
            RequireSuperUser = true)]
        [Information("启用", "核心", "启用一个服务")]
        public async Task EnableService(long groupId, long userId, Message message)
        {
            var name = message.ToPureString().Substring(2);
            await using var context = new ApplicationContext();
            var services = from record in context.ServiceRecords
                where record.GroupId == groupId && record.ServiceName == name
                select record;
            var messageBuilder = new MessageBuilder();
            messageBuilder.Append(new AtMessage(userId));
            if (!services.Any())
            {
                messageBuilder.Append((SimpleMessage) $"未能找到服务{services.First().ServiceName}！");
            }
            else
            {
                services.First().IsEnabled = true;
                messageBuilder.Append((SimpleMessage) $"服务{services.First().ServiceName}启用成功！");
                await context.SaveChangesAsync();
            }

            await _messageSender.SendGroupAsync(groupId, messageBuilder.ToMessage());
        }

        [OnKeyword("禁用", InvokePolicies = InvokePolicies.Group, KeywordPolicy = KeywordPolicy.Begin,
            RequireSuperUser = true)]
        [Information("禁用", "核心", "禁用一个服务")]
        public async Task DisableService(long groupId, long userId, Message message)
        {
            var name = message.ToPureString().Substring(2);
            await using var context = new ApplicationContext();
            var services = from record in context.ServiceRecords
                where record.GroupId == groupId && record.ServiceName == name
                select record;
            var messageBuilder = new MessageBuilder();
            messageBuilder.Append(new AtMessage(userId));
            if (!services.Any())
            {
                messageBuilder.Append((SimpleMessage) $"未能找到服务{services.First().ServiceName}！");
            }
            else if (services.First().ServiceName != "Core")
            {
                services.First().IsEnabled = false;
                messageBuilder.Append((SimpleMessage) $"服务{services.First().ServiceName}禁用成功！");
                await context.SaveChangesAsync();
            }
            else
            {
                messageBuilder.Append((SimpleMessage) "不能禁用Core服务！");
            }

            await _messageSender.SendGroupAsync(groupId, messageBuilder.ToMessage());
        }
    }
}