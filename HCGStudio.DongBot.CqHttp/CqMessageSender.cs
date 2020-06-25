using System;
using System.Threading.Tasks;
using cqhttp.Cyan.Clients;
using cqhttp.Cyan.Enums;
using cqhttp.Cyan.Messages.CQElements;
using HCGStudio.DongBot.Core.Messages;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.CqHttp
{
    internal class CqMessageSender : IMessageSender
    {
        public CqMessageSender(CQApiClient client)
        {
            Client = client;
        }

        protected CQApiClient Client { get; set; }

        public async Task<bool> SendGroupAsync(int groupId, Message message)
        {
            var result =
                await Client.SendMessageAsync((MessageType.group_, groupId), await MessageToCqMessage(message));
            return result.raw_data["status"]?.ToString() == "ok";
        }

        public async Task<bool> SendPrivateAsync(int userId, Message message)
        {
            var result =
                await Client.SendMessageAsync((MessageType.private_, userId), await MessageToCqMessage(message));
            return result.raw_data["status"]?.ToString() == "ok";
        }

        private async Task<cqhttp.Cyan.Messages.Message> MessageToCqMessage(Message message)
        {
            var msg = new cqhttp.Cyan.Messages.Message();
            switch (message)
            {
                case IAtMessage atMessage:
                    if (atMessage.AtAll)
                        msg += new ElementAt();
                    else
                        msg += new ElementAt(atMessage.Content);
                    break;
                case IPictureMessage pictureMessage:
                    if (pictureMessage.Url != null)
                        msg += new ElementImage(pictureMessage.Url.AbsoluteUri);
                    else
                        msg += new ElementImage(Convert.FromBase64String(await pictureMessage.ToBase64StringAsync()));
                    break;
                case ISimpleMessage simpleMessage:
                    msg += new ElementText(simpleMessage.Content);
                    break;
                case IUnionMessage unionMessage:
                    foreach (var unionMessageMessage in unionMessage.Messages)
                        msg += await MessageToCqMessage(unionMessageMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message));
            }

            return msg;
        }
    }
}