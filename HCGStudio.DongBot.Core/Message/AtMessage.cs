using System;
using System.Collections.Generic;
using System.Text;

namespace HCGStudio.DongBot.Core.Message
{
    public class AtMessage : Message, IAtMessage
    {
        public long Content { get; }
        public bool AtAll { get; }

        public AtMessage(long atAccount, bool atAll)
        {
            Content = atAccount;
            AtAll = atAll;
        }
    }
}
