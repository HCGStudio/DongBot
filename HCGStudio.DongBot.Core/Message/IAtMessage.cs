using System;
using System.Collections.Generic;
using System.Text;

namespace HCGStudio.DongBot.Core.Message
{
    public interface IAtMessage
    {
        long Content { get; }
        bool AtAll { get; }
    }
}
