using System;
using System.Collections.Generic;
using System.Text;

namespace HCGStudio.DongBot.Core.Message
{
    public interface IUnionMessage
    {
        IEnumerable<Message> Messages { get; }
    }
}
