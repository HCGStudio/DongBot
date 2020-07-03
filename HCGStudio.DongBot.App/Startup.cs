using HCGStudio.DongBot.App.SystemService;
using HCGStudio.DongBot.Core.Service;
using HCGStudio.DongBot.CqHttp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HCGStudio.DongBot.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddConsole();
                    builder.AddEventLog();
                })
                .UseCqWs(config =>
                {
                    config.AccessUrl = Configuration["Cq:AccessUrl"];
                    config.AccessToken = Configuration["Cq:AccessToken"];
                    config.EventUrl = Configuration["Cq:EventUrl"];
                })
                .AddSingleton(typeof(IBroadcastMessageSender<>), typeof(BroadcastMessageSender<>));

            services.Add(new ServiceDescriptor(typeof(IConfiguration), Configuration));
        }
    }
}