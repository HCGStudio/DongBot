using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HCGStudio.DongBot.Core.Message
{
    public interface IPictureMessage
    {
        Uri? Url { get; }
        Task<string> ToBase64StringAsync();
    }
}
