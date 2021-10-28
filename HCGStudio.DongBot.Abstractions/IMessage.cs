namespace HCGStudio.DongBot.Abstractions;

public interface IMessage
{
    public string Sender { get; }
    public bool IsGroup { get; }
    public string? GroupId { get; }
    public string Mime { get; }
    public ValueTask<string> ToText();
    public ValueTask<byte[]> ToBinary();
}