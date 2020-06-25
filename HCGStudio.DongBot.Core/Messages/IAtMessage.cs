namespace HCGStudio.DongBot.Core.Messages
{
    public interface IAtMessage
    {
        long Content { get; }
        bool AtAll { get; }
    }
}