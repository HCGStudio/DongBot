namespace HCGStudio.DongBot.Abstractions;

public interface IMessageSender
{
    ValueTask SendMessage(string userId, IMessage message);
    ValueTask SendGroupMessage(string groupId, IMessage message);
}