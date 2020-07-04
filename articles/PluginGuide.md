# 插件编写指南

## 前提条件

安装`Visual Studio`并且启用`.Net Core跨平台开发`工作负载，或者安装[.Net Core SDK](https://dotnet.microsoft.com/download/dotnet-core)与`Visual Studio Code`。并且添加对包[HCGStudio.DongBot.Core](https://www.nuget.org/packages/HCGStudio.DongBot.Core/)的引用。

## API保证

我们保证，在所有的次要版本中的更新中，如`v1.1.0.0`->`v1.1.1.0`，不会减少API，仅会增加必要的API。在所有的修补版本中，如`v1.1.0.0`->`v1.1.0.1`，API不会有任何变化，仅会有安全性修补。在主要版本更新中，如`v1.1.0.0`->`v1.2.0.0`，肯能会减少API，但是插件的加载方式不会发生变化，删去减少的API插件理论上仍然会继续工作。在大版本更新中，如`v1.1.0.0`->`v2.0.0.0`，插件的加载方式及定义可能会发生改变，原有的插件可能需要重新编写才能继续使用。

## 定义插件类

所有的插件，都是由服务进行控制，插件选择的服务名需要使用注解的形式在编译时进行指定。注意：服务控制只在群组中可用，即一个插件的私聊应对不会受群组中服务启用的是否控制。

下面是插件类注解示例：

``` C#
[Service("Simple", AutoEnable = true)]
public class Simple
```

其中，`AutoEnable`表示这个插件是否会自动启用（默认为否）。

**注意：**`Core`为预留的内置插件名称，如果一个插件使用`Core`作为服务名，则其不会被加载。

## 编写构造函数

所有的依赖都会使用构造函数进行依赖注入，如`IBroadcastMessageSender<TService>`、`IMessageProvider`、`IMessageSender`以及来自[.NET Extensions](https://github.com/dotnet/extensions)的`ILogger<TCategoryName>`与`IConfiguration`。

譬如

``` C#
private readonly IMessageSender _messageSender;
public VersionService(IMessageSender messageSender)
{
	_messageSender = messageSender;
}
```

## 定义相应方法

在指定条件下触发的响应方法即当接收到满足指定条件的消息才会触发的方法，这个方法也需要靠注解进行定义。目前版本一共提供了两种注解，一个为`OnKeywordAttribute`，用于规定指定关键词触发，另一个为`ScheduleTaskAttribute`用于规定定时触发。

譬如

``` C#
[OnKeyword("版本", "Version", InvokePolicies = InvokePolicies.Private, KeywordPolicy = KeywordPolicy.Trim)]
[Information("版本", "核心", "查看Dong! Bot版本，需私聊")]
public async Task VersionQuery(long senderUserId)
{
	await _messageSender.SendPrivateAsync(senderUserId,
	(SimpleMessage) $"DongBot {Assembly.GetExecutingAssembly().GetName().Version}");
}
```

其中，`InformationAttribute`为帮助提供信息，带有此注解的方法将会在显示帮助时提供相关信息。

此外，每种触发方式都对方法的签名进行了限制，具体如下

| 形式                                             | 可使用的返回值 | 可以使用的方法签名                                           |
| ------------------------------------------------ | -------------- | ------------------------------------------------------------ |
| InvokePolicies.Private                           | void, Task     | 无，`long userId`，或 `long userId, Message message`         |
| InvokePolicies.Group \| InvokePolicies.GroupAtMe | void, Task     | 无， `long groupId, long userId`，或`long groupId, long userId, Message message` |
| ScheduleTaskAttribute                            | Task           | 无                                                           |

## 插件位置

编译后的插件应放在`plugin`目录中，Dong! Bot会在启动时自动加载这些插件。

你也可以选择不编译插件，将其放在`light-plugins`目录中，运行时会自动进行编译及加载。

## 其他

关于接口的定义、方法，请查看[详细文档](https://hcgstudio.github.io/DongBot/api/HCGStudio.DongBot.Core.Service.html)，此处不再赘述。