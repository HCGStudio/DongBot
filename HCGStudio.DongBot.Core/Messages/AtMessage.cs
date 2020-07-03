namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     At消息
    /// </summary>
    public class AtMessage : Message, IAtMessage
    {
        /// <summary>
        ///     创建一个新的At消息
        /// </summary>
        /// <param name="atAccount">被at人的账号Id</param>
        /// <param name="atAll"></param>
        public AtMessage(long atAccount, bool atAll = false)
        {
            Content = atAccount;
            AtAll = atAll;
        }

        /// <inheritdoc />
        public long Content { get; }

        /// <inheritdoc />
        public bool AtAll { get; }
    }
}