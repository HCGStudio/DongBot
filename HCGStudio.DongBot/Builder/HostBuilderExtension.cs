using Microsoft.Extensions.Hosting;

namespace HCGStudio.DongBot.Builder;

public static class HostBuilderExtension
{
    public static IHostBuilder ConfigDongBotDefaults(this IHostBuilder builder)
    {
        return builder;
    }
}