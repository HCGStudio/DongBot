namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     At消息的接口
    /// </summary>
    public interface IAtMessage
    {
        /// <summary>
        ///     被At人账号Id
        /// </summary>
        long Content { get; }

        /// <summary>
        ///     是否为At全体成员消息
        /// </summary>
        bool AtAll { get; }
    }
}