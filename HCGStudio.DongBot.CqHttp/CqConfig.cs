namespace HCGStudio.DongBot.CqHttp
{
    public class CqConfig
    {
        public string AccessUrl { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string EventUrl { get; set; } = string.Empty;
        public int ListenPort { get; set; } = -1;
        public int BindPort { get; set; }
        public string ApiPath { get; set; }
        public string EventPath { get; set; }
        public string Secret { get; set; } = string.Empty;
        public bool UseGroupTable { get; set; } = false;
        public bool UseMessageTable { get; set; } = false;
    }
}