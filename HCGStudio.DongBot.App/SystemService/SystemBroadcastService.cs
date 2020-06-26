using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.App.SystemService
{
    public class SystemBroadcastMessageSender : IBroadcastMessageSender
    {
        public string ServiceName { get; }

        public SystemBroadcastMessageSender(string name)
        {
            ServiceName = name;
        }

        public Task BroadcastAllEnabled(Message message, int interval = 100)
        {
            throw new NotImplementedException();
        }
    }
}
