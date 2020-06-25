using Autofac;
using cqhttp.Cyan.Clients;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.CqHttp
{
    public static class DependencyInject
    {
        public static ContainerBuilder UseCqHttpClient(this ContainerBuilder builder, string accessUrl,
            string accessToken = "", int listenPort = -1, string secret = "", bool useGroupTable = false,
            bool useMessageTable = false)
        {
            var client = new CQHTTPClient(accessUrl, accessToken, listenPort, secret, useGroupTable, useMessageTable);
            var provider = new CqMessageProvider(client);
            var sender = new CqMessageSender(client);

            builder.Register(p => provider).As<IMessageProvider>();
            builder.Register(s => sender).As<IMessageSender>();
            return builder;
        }

        public static ContainerBuilder UseCqWs(this ContainerBuilder builder, string accessUrl,
            string accessToken = "", string eventUrl = "", bool useGroupTable = false, bool useMessageTable = false)
        {
            var client = new CQWebsocketClient(accessUrl, accessToken, eventUrl, useGroupTable, useMessageTable);
            var provider = new CqMessageProvider(client);
            var sender = new CqMessageSender(client);

            builder.Register(p => provider).As<IMessageProvider>();
            builder.Register(s => sender).As<IMessageSender>();
            return builder;
        }

        public static ContainerBuilder UseCqWsReserve(this ContainerBuilder builder, int bindPort, string apiPath,
            string eventPath, string accessToken = "", bool useGroupTable = false, bool useMessageTable = false)
        {
            var client = new CQReverseWSClient(bindPort, apiPath, eventPath, accessToken, useGroupTable,
                useMessageTable);
            var provider = new CqMessageProvider(client);
            var sender = new CqMessageSender(client);

            builder.Register(p => provider).As<IMessageProvider>();
            builder.Register(s => sender).As<IMessageSender>();
            return builder;
        }
    }
}