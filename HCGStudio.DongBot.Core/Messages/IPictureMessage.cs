using System;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Messages
{
    public interface IPictureMessage
    {
        Uri? Url { get; }
        Task<string> ToBase64StringAsync();
    }
}