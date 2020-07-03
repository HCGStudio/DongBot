namespace HCGStudio.DongBot.Core.Messages
{
    /// <summary>
    ///     简单消息的接口
    /// </summary>
    public interface ISimpleMessage
    {
        /// <summary>
        ///     消息的文字内容
        /// </summary>
        string Content { get; }
    }
}