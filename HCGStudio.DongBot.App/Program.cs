using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using HCGStudio.DongBot.CqHttp;
using Newtonsoft.Json;
using NLog;

namespace HCGStudio.DongBot.App
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static async Task Main(string[] args)
        {
            if (!File.Exists("config.json"))
            {
                await File.WriteAllTextAsync("config.json",
                    JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));
                Logger.Error("Configuration file not fount, aborting. Please modify the generated config.");
                return;
            }

            var config = JsonConvert.DeserializeObject<Configuration>(await File.ReadAllTextAsync("config.json"));
            Logger.Info($"Now using {config.ServiceType}.");
            //build 
            var builder = new ContainerBuilder();
            try
            {
                switch (config.ServiceType)
                {
                    case ServiceType.CqHttpHttp:
                        builder.UseCqHttpClient(config.AccessUrl, config.AccessToken, config.ListenPort);
                        break;
                    case ServiceType.CqWs:
                        builder.UseCqWs(config.AccessUrl, config.AccessToken, config.EventUrl);
                        break;
                    case ServiceType.CqWsReserve:
                        builder.UseCqWsReserve(config.ListenPort, config.ApiPath, config.EventPath, config.AccessToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
            }
        }
    }
}