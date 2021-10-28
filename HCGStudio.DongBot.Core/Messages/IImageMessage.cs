using System;
using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     图片消息的接口
    /// </summary>
    public interface IImageMessage
    {
        /// <summary>
        ///     图片的路径
        /// </summary>
        public Uri? Url { get; }

        /// <summary>
        ///     图片的内容
        /// </summary>
        public IReadOnlyList<byte> Content { get; }
    }
}