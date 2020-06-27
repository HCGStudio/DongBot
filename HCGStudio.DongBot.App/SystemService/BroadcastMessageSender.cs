using System.Linq;
using System.Threading.Tasks;
using HCGStudio.DongBot.App.Models;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.App.SystemService
{
    public class BroadcastMessageSender : IBroadcastMessageSender
    {
        private readonly IMessageSender _messageSender;
        public string ServiceName { get; }
        public BroadcastMessageSender(IMessageSender messageSender, string serviceName)
        {
            _messageSender = messageSender;
            ServiceName = serviceName;
        }
        public async Task BroadcastAllEnabled(Message message, int interval = 100)
        {
            await using var context = new ApplicationContext();
            var enabledGroup = from record in context.ServiceRecords
                               where record.ServiceName == ServiceName && record.IsEnabled
                               select record.GroupId;
            foreach (var groupId in enabledGroup)
            {
                await _messageSender.SendGroupAsync(groupId, message);
                await Task.Delay(interval);
            }
        }
    }
}
