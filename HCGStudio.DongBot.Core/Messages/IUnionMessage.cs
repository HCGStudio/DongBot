using System.Collections.Generic;

namespace HCGStudio.DongBot.Core.Messages
{
    public interface IUnionMessage
    {
        IEnumerable<Message> Messages { get; }
    }
}