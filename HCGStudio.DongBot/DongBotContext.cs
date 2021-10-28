using System.Diagnostics;
using HCGStudio.DongBot.Abstractions;

namespace HCGStudio.DongBot;

public class DongBotContext
{
    private readonly IMessageSender _sender;

    public DongBotContext(IMessage message, IMessageSender sender)
    {
        Message = message;
        _sender = sender;
    }

    public IMessage Message { get; }

    public async Task Response(IMessage message)
    {
        if (Message.IsGroup)
        {
            Debug.Assert(Message.GroupId != null);
            await _sender.SendGroupMessage(Message.GroupId, message);
        }
        else
        {
            await _sender.SendMessage(Message.Sender, message);
        }
    }
}