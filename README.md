# Dong! Bot

Dong! Bot is a multi backend chat bot framework.

## Backend support:

- [x] CqHttp (Only websocket tested, through [cqhttp.Cyan](https://github.com/frank-bots/cqhttp.Cyan))
- [ ] mirai-api-http
- [ ] Other chat software.

## Plugin write guide

Add refrence to nuget package [HCGStudio.DongBot.Core](https://www.nuget.org/packages/HCGStudio.DongBot.Core/) on your csproj file.
``` XML
<PackageReference Include="HCGStudio.DongBot.Core" Version="1.0.0" />
```

Add class for your service, and add `ServiceAttribute`.

``` C#
[Service("Simple", AutoEnable = true)]
public class SimpleVersion
```

**Notice**: Name "Core" is reserved for built-in services and cannot be used.

Add provided interface properties you need.

``` C#
public IMessageSender MessageSender { get; set; }
```

Write your response to the message, or schedule task, and add correct attribute.

For examle:

``` C#
[OnKeyword("Hello", "你好", InvokePolicies = InvokePolicies.Private, KeywordPolicy = KeywordPolicy.Trim)]
public async Task SimpleTask(long senderUserId)
{
    await MessageSender.SendPrivateAsync(senderUserId, (SimpleMessage)"Hello!");
}
```

There are some requirment of your method:

| Type                                             | Accepted returnvalue type | Accepted parameter lists                                     |
| ------------------------------------------------ | ------------------------- | ------------------------------------------------------------ |
| InvokePolicies.Private                           | void, Task                | empty,  `long userId`,or `long userId, Message message`      |
| InvokePolicies.Group \| InvokePolicies.GroupAtMe | void, Task                | empty, `long groupId, long userId`,or `long groupId, long userId, Message message` |
| ScheduleTaskAttribute                            | Task                      | empty                                                        |

Filnally build, put your dll file under the plugins folder of Dong! Bot and test your work!
