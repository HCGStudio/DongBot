using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HCGStudio.DongBot.App
{
    public enum ServiceType
    {
        CqHttpHttp,
        CqWs,
        CqWsReserve,
        Custom
    }

    public class Configuration
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceType ServiceType { get; set; }

        public string CustomServiceName { get; set; }
        public string AccessUrl { get; set; }
        public string AccessToken { get; set; }
        public int ListenPort { get; set; }
        public string EventUrl { get; set; }
        public string ApiPath { get; set; }
        public string EventPath { get; set; }
        public List<long> SuperUserIds { get; set; }
    }
}