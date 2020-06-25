using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Message
{
    public class LocalPictureMessage :Message, IPictureMessage
    {
        public Uri? Url { get; }

        public LocalPictureMessage(string url)
        {
            Url = new Uri(url);
        }

        public LocalPictureMessage(Uri url)
        {
            Url = url;
        }

        public async Task<string> ToBase64StringAsync()
        {
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
