# 安装指南

## 安装需求

本框架默认使用酷Q作为聊天收发后端，请先下载并安装[酷Q Air](https://dlsec.cqp.me/cqa-full)、[酷QPro](https://dlsec.cqp.me/cqp-xiaoi)或者[mirai](https://github.com/mamoe/mirai)+[cqhttp-mirai](https://github.com/yyuueexxiinngg/cqhttp-mirai)。

然后安装`CqHttp`插件，详细请参考[CqHttp文档](https://cqhttp.cc/docs/)。请在酷Q`data/app/io.github.richardchien.coolqhttpapi`目录中新建`config.ini`，输入以下内容

``` ini
[使用的QQ账号]
access_token = 你想设置的随机字符串
ws_port = 5800
use_ws = yes
serve_data_files = no
host = 0.0.0.0
```

如果您觉得上述步骤过于繁琐，可以使用`CqHttp`的[docker镜像](https://cqhttp.cc/docs/#/Docker)（仅支持`Linux`容器），docker参考启动命令：

``` bash
sudo docker run -d --name=cqhttp \
-v $(pwd)/coolq:/home/user/coolq \
-p 9000:9000 \
-p 5800:5800 \
-e VNC_PASSWD=qwertyui \
-e COOLQ_ACCOUNT=000000 \
-e COOLQ_URL=https://dlsec.cqp.me/cqp-full \
-e CQHTTP_SERVE_DATA_FILES=no \
-e CQHTTP_USE_WS=yes \
-e CQHTTP_WS_PORT=5800 \
-e CQHTTP_HOST=0.0.0.0 \
-e CQHTTP_ACCESS_TOKEN=asdf \
richardchien/cqhttp:latest
```

然后安装`dotnet`，请按照[这个页面](https://dotnet.microsoft.com/download/dotnet-core/current/runtime)的步骤进行安装。

## 下载程序

请从[Release页面](https://github.com/HCGStudio/DongBot/releases)下载最新的发行版，如果是`Windows`，运行请双击`HCGStudio.DongBot.App.exe`，如果使用`Linux`或者在`Windows`的命令行中启动，请使用`dotnet HCGStudio.DongBot.App.dll`。

## 配置文件

本程序的配置使用运行目录下的`config.json`以及`PluginConfig`文件夹内所有的`JSON`文件作为配置文件。安装之后，请按照如下实例创建`config.json`。

``` json
{
  "Cq": {
    "AccessUrl": "ws://127.0.0.1:5800",
    "AccessToken": "你上面填的随机字符串",
    "EventUrl": "ws://127.0.0.1:5800"
  },
  "SuperUsers": [
    "超级管理员的用户Id"
  ]
}
```

你也可以将其拆分，比如将以下文件放在`config.json`中：

``` json
{
  "SuperUsers": [
    "超级管理员的用户Id"
  ]
}
```

将以下文件放在`PluginConfig/cq.json`中：

``` json
{
  "Cq": {
    "AccessUrl": "ws://127.0.0.1:5800",
    "AccessToken": "你上面填的随机字符串",
    "EventUrl": "ws://127.0.0.1:5800"
  }
}
```

然后启动程序，在群里发送`帮助`或者私聊`版本`进行测试。

## 完成安装

在你完成安装之后，安装插件并且测试没用问题，那就可以将其作为服务安装。

如果你使用的是`Systemd`管理的现代Linux发行版，可以使用以下命令进行安装：

``` bash
sudo mkdir -p /var/dongbot/app
sudo cp DongBot.Service /etc/systemd/system
sudo cp -r . /var/dongbot/app
```

然后启用并启动服务

``` bash
sudo systemctl enable DongBot.Service
sudo systemctl start DongBot.Service
```

在你每次更新之后，都可以运行上述指令进行更新，但是记得先停止服务

``` bash
sudo systemctl stop DongBot.Service
```

如果你使用的`Windows`服务器，建议直接双击启动，有问题远程登录重启就好。

