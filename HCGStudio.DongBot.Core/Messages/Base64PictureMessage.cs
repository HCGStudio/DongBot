using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     Base64形式储存的图片消息
    /// </summary>
    public class Base64PictureMessage : Message, IPictureMessage
    {
        /// <summary>
        ///     从本地或远程路径构建新的图片消息
        /// </summary>
        /// <param name="url">图片路径</param>
        public Base64PictureMessage(Uri url)
        {
            Url = url;
        }

        /// <summary>
        ///     从Base64字符串构建新的图片消息
        /// </summary>
        /// <param name="base64Content"></param>
        public Base64PictureMessage(string base64Content)
        {
            Base64 = base64Content;
        }

        private string? Base64 { get; }

        /// <inheritdoc />
        public Uri? Url { get; }

        /// <summary>
        ///     获取表示图片的Base64字符串
        /// </summary>
        /// <returns>表示图片的Base64字符串</returns>
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