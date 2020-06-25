namespace HCGStudio.DongBot.Core.Messages
{
    public class AtMessage : Message, IAtMessage
    {
        public AtMessage(long atAccount, bool atAll)
        {
            Content = atAccount;
            AtAll = atAll;
        }

        public long Content { get; }
        public bool AtAll { get; }
    }
}