# Dong! Bot

Dong! Bot is a multi backend chat bot framework.

**WARNING:** This framework is useable but still under construction and may have many bugs.

## Backend Support:

- [x] CqHttp (Only websocket tested, through [cqhttp.Cyan](https://github.com/frank-bots/cqhttp.Cyan))
- [ ] mirai (Not in recent plan, you can use [cqhttp-mirai](https://github.com/yyuueexxiinngg/cqhttp-mirai) instead)
- [ ] Custom backend (Working on it)
- [ ] Other chat software.

## Plugin Write guide

Add refrence to nuget package [HCGStudio.DongBot.Core](https://www.nuget.org/packages/HCGStudio.DongBot.Core/) on your csproj file.
``` XML
<PackageReference Include="HCGStudio.DongBot.Core" Version="1.0.1" />
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

You can also view our official plugin:

[Dong! Bot Reminder](https://github.com/HCGStudio/DongBot-Reminder)

## Light Plugin Guide

If you don't want to build every time you update your plugin, try light-plugin. When Dong! Bot starts, we check every file end with `.cs` or `.plg`ungder `light-plugin`folder and compile it and load it as plugin!

Light Plugin does everything like normal Dong! Bot plugin, but you can't reference any type in other light plugin.  

Tip: If you are starting Dong! bot through `dotnet run` or `vscode`, you should use `.plg` suffix, because light-plugin will  compiled by `dotnet`.