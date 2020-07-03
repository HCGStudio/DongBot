using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     本地储存的图片
    /// </summary>
    public class LocalPictureMessage : Message, IPictureMessage
    {
        /// <summary>
        ///     初始化本地储存图片
        /// </summary>
        /// <param name="url">图片的地址</param>
        public LocalPictureMessage(string url)
        {
            Url = new Uri(url);
        }

        /// <summary>
        ///     初始化本地储存的图片
        /// </summary>
        /// <param name="url">图片的地址</param>
        public LocalPictureMessage(Uri url)
        {
            Url = url;
        }


        /// <inheritdoc />
        public Uri? Url { get; }

        /// <inheritdoc />
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