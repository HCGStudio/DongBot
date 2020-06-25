using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Message
{
    public class Base64PictureMessage : Message, IPictureMessage
    {
        public Uri? Url => null;
        private string? Base64 { get; }
        private Uri? BaseUri { get; }

        public Base64PictureMessage(Uri url)
        {
            BaseUri = url;
        }

        public Base64PictureMessage(string base64Content)
        {
            Base64 = base64Content;
        }

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
