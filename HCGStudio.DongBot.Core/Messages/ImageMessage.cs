using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    /// 图片消息
    /// </summary>
    public class ImageMessage : Message, IImageMessage
    {
        /// <inheritdoc />
        public Uri? Url { get; }

        /// <inheritdoc />
        public IReadOnlyList<byte> Content
        {
            get
            {
                if (_content != null)
                    return _content;
                _content = CacheNow();
                return _content;
            }
        }

        private byte[]? _content;

        /// <summary>
        /// 立刻缓存图像
        /// </summary>
        private byte[] CacheNow()
        {
            if (Url != null)
            {
                var wr = WebRequest.Create(Url);
                var buffer = new byte[wr.ContentLength];
                wr.GetRequestStream().Read(buffer, 0, buffer.Length);
                return buffer;
            }
            else
            {
                return new byte[256];
            }
        }

        /// <summary>
        /// 从WebUrl中创建图片消息
        /// </summary>
        /// <param name="url">文件在远程服务器的位置</param>
        /// <param name="cacheNow">是否立即从远程服务器中下载图片的缓存</param>
        public ImageMessage(Uri url, bool cacheNow = false)
        {
            Url = url;
            if (cacheNow)
                _content = CacheNow();
        }

        /// <summary>
        /// 从本地文件中创建图像消息
        /// </summary>
        /// <param name="path">文件路径</param>
        public ImageMessage(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            _content = File.ReadAllBytes(path);
        }
    }
}
