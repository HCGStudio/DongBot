using System;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     图片消息的接口
    /// </summary>
    public interface IPictureMessage
    {
        /// <summary>
        ///     图片在本地或者在远程储存的地址
        /// </summary>
        Uri? Url { get; }

        /// <summary>
        ///     将图片转化为Base64字符串
        /// </summary>
        /// <returns>转化的Base64字符串</returns>
        Task<string> ToBase64StringAsync();
    }
}