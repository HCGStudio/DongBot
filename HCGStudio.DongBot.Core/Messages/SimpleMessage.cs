namespace HCGStudio.DongBot.Core.Messages
{
    public class SimpleMessage : Message, ISimpleMessage
    {
        public SimpleMessage(string content)
        {
            Content = content;
        }

        public string Content { get; }

        public static SimpleMessage FromString(string message)
        {
            return new SimpleMessage(message);
        }

        public static explicit operator SimpleMessage(string message)
        {
            return new SimpleMessage(message);
        }

        public static implicit operator string(SimpleMessage message)
        {
            return message?.Content ?? string.Empty;
        }

        public override string ToString()
        {
            return Content;
        }
    }
}