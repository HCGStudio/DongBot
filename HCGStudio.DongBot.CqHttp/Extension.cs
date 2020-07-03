using System;
using cqhttp.Cyan.Clients;
using HCGStudio.DongBot.Core.Service;
using Microsoft.Extensions.DependencyInjection;

namespace HCGStudio.DongBot.CqHttp
{
    public static class Extension
    {
        public static IServiceCollection UseCqHttpClient(this IServiceCollection builder, Action<CqConfig> action)
        {
            var config = new CqConfig();
            action(config);
            var client = new CQHTTPClient(config.AccessUrl, config.AccessToken, config.ListenPort, config.Secret,
                config.UseGroupTable, config.UseMessageTable);
            var provider = new CqMessageProvider(client);
            var sender = new CqMessageSender(client);

            builder.Add(new ServiceDescriptor(typeof(IMessageProvider), provider));
            builder.Add(new ServiceDescriptor(typeof(IMessageSender), sender));

            return builder;
        }

        public static IServiceCollection UseCqWs(this IServiceCollection builder, Action<CqConfig> action)
        {
            var config = new CqConfig();
            action(config);
            var client = new CQWebsocketClient(config.AccessUrl, config.AccessToken, config.EventUrl,
                config.UseGroupTable, config.UseMessageTable);
            var provider = new CqMessageProvider(client);
            var sender = new CqMessageSender(client);

            builder.Add(new ServiceDescriptor(typeof(IMessageProvider), provider));
            builder.Add(new ServiceDescriptor(typeof(IMessageSender), sender));

            return builder;
        }

        public static IServiceCollection UseCqWsReserve(this IServiceCollection builder, Action<CqConfig> action)
        {
            var config = new CqConfig();
            action(config);
            var client = new CQReverseWSClient(config.BindPort, config.ApiPath, config.EventPath, config.AccessToken,
                config.UseGroupTable, config.UseMessageTable);
            var provider = new CqMessageProvider(client);
            var sender = new CqMessageSender(client);

            builder.Add(new ServiceDescriptor(typeof(IMessageProvider), provider));
            builder.Add(new ServiceDescriptor(typeof(IMessageSender), sender));

            return builder;
        }
    }
}