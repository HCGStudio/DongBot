using System.Text;

namespace HCGStudio.DongBot.Core.Messages
{
    public abstract class Message
    {
        public string ToPureString()
        {
            switch (this)
            {
                case SimpleMessage simpleMessage:
                    return simpleMessage.Content;
                case UnionMessage unionMessage:
                    var sb = new StringBuilder();
                    foreach (var unionMessageMessage in unionMessage.Messages)
                        sb.Append(unionMessageMessage.ToPureString());

                    return sb.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}