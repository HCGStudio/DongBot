using System.Threading.Channels;

namespace HCGStudio.DongBot.Abstractions;

public interface IMessageProvider
{
    Channel<IMessage> MessageChannel { get; }
}