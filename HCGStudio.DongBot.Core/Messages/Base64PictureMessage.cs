using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Messages
{
    public class Base64PictureMessage : Message, IPictureMessage
    {
        public Base64PictureMessage(Uri url)
        {
            BaseUri = url;
        }

        public Base64PictureMessage(string base64Content)
        {
            Base64 = base64Content;
        }

        private string? Base64 { get; }
        private Uri? BaseUri { get; }
        public Uri? Url => null;

        public async Task<string> ToBase64StringAsync()
        {
            if (Base64 != null)
                return Base64;
            if (Url == null)
                return string.Empty;
            if (Url.IsFile)
            {
                var bytes = await File.ReadAllBytesAsync(Url.AbsolutePath).ConfigureAwait(false);
                return Convert.ToBase64String(bytes);
            }

            using var http = new HttpClient();
            var file = await http.GetAsync(Url.AbsoluteUri).ConfigureAwait(false);
            return Convert.ToBase64String(await file.Content.ReadAsByteArrayAsync().ConfigureAwait(false));
        }
    }
}