using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using cqhttp.Cyan.Clients;
using cqhttp.Cyan.Events.CQEvents;
using cqhttp.Cyan.Events.CQEvents.Base;
using cqhttp.Cyan.Events.CQResponses;
using cqhttp.Cyan.Events.MetaEvents;
using HCGStudio.DongBot.Core.Message;
using HCGStudio.DongBot.Core.Service;

namespace HCGStudio.DongBot.CqHttp
{
    public class CqMessageProvider : IMessageProvider
    {
        protected CQApiClient Client { get; set; }
        public void SubscribePrivateMessage(Action<Message, long> action)
        {
            Client.OnEvent += (client, e) =>
            {
                switch (e)
                {
                    case DiscussMessageEvent discussMessageEvent:
                        break;
                    case GroupMessageEvent groupMessageEvent:
                        break;
                    case PrivateMessageEvent privateMessageEvent:
                        var builder = new MessageBuilder();
                        foreach (var element in privateMessageEvent.message.data)
                        {
                            if (element.type == "text")
                            {
                                builder.Append(new SimpleMessage(element.data["text"]));
                            }
                        }

                        action(builder.ToMessage(), privateMessageEvent.sender_id);
                        break;
                    case MessageEvent messageEvent:
                        break;
                    case GroupAdminEvent groupAdminEvent:
                        break;
                    case GroupBanEvent groupBanEvent:
                        break;
                    case GroupMemberChangeEvent groupMemberChangeEvent:
                        break;
                    case GroupUploadEvent groupUploadEvent:
                        break;
                    case GroupNoticeEvent groupNoticeEvent:
                        break;
                    case FriendAddEvent friendAddEvent:
                        break;
                    case NoticeEvent noticeEvent:
                        break;
                    case FriendAddRequestEvent friendAddRequestEvent:
                        break;
                    case GroupAddRequestEvent groupAddRequestEvent:
                        break;
                    case RequestEvent requestEvent:
                        break;
                    case HeartbeatEvent heartbeatEvent:
                        break;
                    case LifecycleEvent lifecycleEvent:
                        break;
                    case MetaEvent metaEvent:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(e));
                }

                return new EmptyResponse();
            };
        }

        public void SubscribeGroupMessage(Action<Message, long, long, bool> action)
        {
            Client.OnEvent += (client, e) =>
            {
                switch (e)
                {
                    case DiscussMessageEvent discussMessageEvent:
                        break;
                    case GroupMessageEvent groupMessageEvent:
                        var builder = new MessageBuilder();
                        var atMe = false;
                        foreach (var element in groupMessageEvent.message.data)
                        {
                            if (element.type == "text")
                            {
                                builder.Append(new SimpleMessage(element.data["text"]));
                            }
                            else if (element.type == "at")
                            {
                                var account = element.data["qq"];
                                builder.Append(new AtMessage(account == "all" ? -1 : long.Parse(account),
                                    account == "all"));
                                if (account == client.self_id.ToString())
                                    atMe = true;
                            }
                        }
                        action(builder.ToMessage(), groupMessageEvent.group_id,
                            groupMessageEvent.isAnonymous ? 0 : groupMessageEvent.sender.user_id, atMe);
                        break;
                    case PrivateMessageEvent privateMessageEvent:
                        break;
                    case MessageEvent messageEvent:
                        break;
                    case GroupAdminEvent groupAdminEvent:
                        break;
                    case GroupBanEvent groupBanEvent:
                        break;
                    case GroupMemberChangeEvent groupMemberChangeEvent:
                        break;
                    case GroupUploadEvent groupUploadEvent:
                        break;
                    case GroupNoticeEvent groupNoticeEvent:
                        break;
                    case FriendAddEvent friendAddEvent:
                        break;
                    case NoticeEvent noticeEvent:
                        break;
                    case FriendAddRequestEvent friendAddRequestEvent:
                        break;
                    case GroupAddRequestEvent groupAddRequestEvent:
                        break;
                    case RequestEvent requestEvent:
                        break;
                    case HeartbeatEvent heartbeatEvent:
                        break;
                    case LifecycleEvent lifecycleEvent:
                        break;
                    case MetaEvent metaEvent:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(e));
                }

                return new EmptyResponse();
            };
        }
    }
}
