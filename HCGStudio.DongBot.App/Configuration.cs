﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HCGStudio.DongBot.App
{
    public enum ServiceType
    {
        CqHttpHttp,
        CqWs,
        CqWsReserve
    }

    public class Configuration
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceType ServiceType { get; set; }

        public string AccessUrl { get; set; }
        public string AccessToken { get; set; }
        public int ListenPort { get; set; }
        public string Secret { get; set; }
        public string EventUrl { get; set; }
        public string ApiPath { get; set; }
        public string EventPath { get; set; }
    }
}