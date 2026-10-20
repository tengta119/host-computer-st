# TASK-049：让控制台上位机连接设备模拟器

2026-10-20本题已按静态源码口径✅ 已通关，下文保留原操作讲解；TODO 4和协议异常捕获由教练协助，本轮未构建、运行或测试，完整TCP联调尚未验证。

本题先让两个控制台程序完成“连接 → 读温度 → 启动和停止设备 → 查状态 → 断开再连接”，暂不接WPF或真实设备。
设备模拟器已经由教练提供，你要完成的是客户端的连接、发送，以及收到字节后按换行取出完整回复。

## 先认清这几个真实文件

| 文件 | 当前作用 |
| :--- | :--- |
| [Program.cs](../../Program.cs) | 教练提供的共享入口，根据`--client`、`--simulator`或`--observe`选择运行内容。 |
| [Task049ClientExercise.cs](./Task049ClientExercise.cs) | 上位机菜单及收发流程，TODO 1连接模拟器，TODO 2发送命令并等待回复。 |
| [Task049LineBuffer.cs](./Task049LineBuffer.cs) | 本题自定义的缓冲类，TODO 3保存有效字节，TODO 4按LF换行取消息。 |
| [Task049BufferObservation.cs](./Task049BufferObservation.cs) | 教练提供的人工输入，观察半条、连续消息及尾部保留，不需要联网。 |
| [Task049Simulator.cs](./Task049Simulator.cs) | 教练提供的设备对端，接收命令、保存运行状态并回复。 |

`Task049ClientExercise`和`Task049LineBuffer`都是本题自行命名的普通C#类，名字不是.NET内置API。
入口在客户端模式创建一个`Task049ClientExercise`对象，再调用它的`RunAsync`显示菜单。
这个上位机对象里的`_lineBuffer`始终指向同一个`Task049LineBuffer`对象，用来保留尚未处理完的字节。

## 首个动作：只完成TODO 1，先看到连接成功

打开`Task049ClientExercise.cs`，找到`OpenAsync`，先只替换TODO 1下面的两行占位。
方法开头已有的`CloseConnection()`保留，它会先释放之前的连接并清空旧缓冲。

```csharp
_client = new TcpClient();
await _client.ConnectAsync(host, port, cancellationToken);
_stream = _client.GetStream();
Console.WriteLine("已连接模拟器：" + host + ":" + port);
```

`TcpClient`是.NET提供的TCP客户端类，`new TcpClient()`使用C#的`new`写法创建一个新的客户端对象，此时还没有连接模拟器。
`_client`是教练命名的字段，第一行把这个新对象保存到当前上位机对象里，后面的连接和关闭流程才能继续找到它。
`_client.ConnectAsync(...)`让这个客户端按`host`和`port`连接对端，本题默认分别是`127.0.0.1`和`5000`。
这一行前面的`await`表示等待连接操作完成，成功后才继续执行下一行；它不表示每次调用都会新建一条线程。
`cancellationToken`是当前方法收到的取消标记，传给连接方法后，程序通过Ctrl+C发出停止请求时，这个等待可以响应取消。
这里没有另外设置连接等待秒数，不能把菜单里请求收发的3秒保护理解为连接操作也有3秒限制。

`NetworkStream`同样是.NET提供的类型，当前连接建立后，通过它发送和接收字节。
`GetStream()`取得这条连接的收发对象，第三行把它保存到教练命名的`_stream`字段中，后面的TODO 2和教练读取循环都会使用它。
字段声明中的`?`允许变量暂时保存`null`，本题用它表示当前没有保留可用的对象，关闭时也会重新赋`null`。
成功文案放在这三行之后，连接失败时会进入已有异常处理，不能在连接之前就打印成功。

原来的`await Task.CompletedTask;`只等待一个已经完成的任务，是让未实现的方法可以编译的骨架占位，没有执行任何连接。
TODO 2里也有同类占位，完成对应实现时再移除，不把占位能运行算作已经完成收发。

## 用两个PowerShell终端先观察连接

每次修改源码后先退出自己运行的本题上位机和模拟器，再从`D:\host-computer-st`执行下面的构建，避免Windows下旧程序占用构建输出。

```powershell
dotnet build .\Stage04-TcpUdp\TcpUdp.slnx --nologo
```

两个终端都从`D:\host-computer-st`运行，使用`--no-build`启动这份刚构建的代码，先在终端A启动模拟器并保持它运行。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --simulator
```

终端A应显示模拟器正在监听`127.0.0.1:5000`，然后等待上位机连接。
再在终端B启动上位机。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --client
```

在终端B输入`o`并回车，TODO 1完成后，上位机应显示连接成功，终端A应显示接受连接。
再输入`c`断开，随后输入`o`重新连接，应该能再次看到模拟器接受新连接。
此时只完成TODO 1，输入`1`还不会得到温度，这是TODO 2尚未实现的正常结果。

如果5000端口已被其他程序占用，可以用5001启动两端，两个命令里的端口必须一致。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --simulator 5001
```

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --client 5001
```

`127.0.0.1`表示当前这台电脑，因此本题的两个程序可以直接通信，不需要创建虚拟串口对。
终端B输入`q`退出上位机，终端A使用Ctrl+C停止模拟器。

## TODO 2：把一条命令发送出去，再读取完整回复

上位机菜单中的`1`会被已有`GetDeviceCommand`换成字符串`GET_TEMP`，发送方法收到的`command`就是这段文字。
双方约定使用不带BOM的UTF-8，每条命令和回复末尾都有且只有一个LF换行字节。
在C#字符串中，`"\n"`表示一个实际换行字符，对应字节`0A`，不是反斜杠和字母n两个字符。

```text
发送文字：GET_TEMP\n
实际字节：47 45 54 5F 54 45 4D 50 0A
回复文字：35.6\n
实际字节：33 35 2E 36 0A
```

`Encoding.UTF8.GetBytes`是.NET提供的文字转字节方法，把`command`加一个`"\n"`后交给它，得到本次要发送的`byte[]`。
本题菜单的命令都是可打印ASCII字符，它们也能用相同的UTF-8字节表达，不需要另外添加编码头。
模拟器只接受可打印ASCII命令和结尾LF，遇到CR字节`0D`会关闭当前连接，所以不要使用Windows的CRLF代替本题约定的LF。

在`SendRequestAsync`的TODO 2里，先准备发送数组，再等待`_stream.WriteAsync`完成写入，最后等待已有的`ReadReplyAsync`并把它返回的字符串交回菜单。
使用`WriteAsync`的数组重载时，数组参数是刚才的发送数组，起始位置是0，数量是该数组的实际长度，取消标记使用本方法收到的`cancellationToken`。
发送日志放在写入成功之后，写入完成只说明字节已交给这条连接，只有读到匹配的回复，已有`PrintReply`才会显示温度或控制确认。
教练提供的`ReadReplyAsync`会重复读取并调用缓冲对象，不需要你在TODO 2里另写一条接收循环。

当前模拟器协议如下，表里的`\n`仍表示实际LF字节。

| 命令 | 回复 | 具体效果 |
| :--- | :--- | :--- |
| `GET_TEMP\n` | `35.6\n` | 返回固定温度35.6℃，设备停止时也可以查询。 |
| `START_DEVICE\n` | `OK\n` | 模拟器把保存的运行状态改为运行。 |
| `STOP_DEVICE\n` | `OK\n` | 模拟器把保存的运行状态改为停止。 |
| `GET_STATUS\n` | `RUNNING\n`或`STOPPED\n` | 查询模拟器实际保存的状态。 |
| `PING\n` | `PONG\n` | 模拟器已支持，但本题菜单不提供心跳操作。 |
| 空命令，即仅有LF | `ERR_EMPTY_COMMAND\n` | 模拟器报告空命令。 |
| 未约定的命令 | `ERR_UNKNOWN_COMMAND\n` | 模拟器报告未知命令。 |

模拟器允许一条命令最多256字节，这个数量不包括末尾LF，超过上限或出现非法字节会关闭当前连接。
运行状态保存在模拟器对象中，同一个模拟器进程里的断开再连接不会重置状态，停止并重新启动模拟器才会回到初始停止状态。

## TODO 3：把本次有效字节接到旧半条后面

教练的读取循环使用`_readBuffer`，这个数组有64个位置，但`ReadAsync`每次返回的`count`才是本次实际收到的字节数量。
循环已经调用`_lineBuffer.Append(_readBuffer, count)`，TODO 3只负责把数组中前`count`个字节按顺序追加到当前`_bytes`列表。
这部分复用你在TASK-040和041已验证的有效范围与累计追加能力，不按64个位置全加，也不在每次追加前清掉旧半条。
`count`为0时没有新字节可加，原列表应保持原样；参数超范围的检查已经由教练提供。

例如第一次收到`35`时，列表先保留这两个字节，下一次收到`.6\n`时，继续追加后才得到完整的`35.6\n`。
网络读取并不保证一次得到一条消息，收到一次数据也不等于已经收到一个温度值。
教练循环遇到读取返回0会结束本连接，它表示对端已正常结束发送且数据已读尽，不是让你继续空转等下一批。

## TODO 4：从列表中取出第一个LF之前的消息

2026-10-20按你的明确要求，这个方法已由教练补齐，以下保留原理讲解供对照源码；18项局部检查通过，本次辅助不计为你自主实现，完整任务尚未验收。

`TryTakeMessage`是本题自定义的方法，一次最多从`_bytes`里取出一条完整消息。
它有两份结果：返回的`bool`说明有没有取出消息，`out string message`把取出的文字交给调用者。

```csharp
string reply;
bool hasMessage = _lineBuffer.TryTakeMessage(out reply);
```

这里的`out`是C#语法，表示方法执行时负责给调用者的`reply`变量赋值，调用前不需要先给`reply`放入一个温度字符串。
取出`35.6`时，应把这段文字赋给`message`并返回`true`，于是调用者的`reply`得到`35.6`，`hasMessage`得到`true`。
没有完整消息时，保留骨架开头给`message`设置的空字符串并返回`false`，调用者继续读取，不能把这次的空字符串当作有效回复。
真实`ReadReplyAsync`把这个调用放在`while`条件里，返回`false`才去读下一批，返回`true`就结束等待并返回已经取出的那一条。

先找列表中的第一个LF，可以从这一行看起。

```csharp
int lineFeedIndex = _bytes.IndexOf(0x0A);
```

`IndexOf`是.NET列表提供的方法，找到时返回从0开始的位置，找不到时返回-1。
当前列表如果是`35.6\nOK\nST`，第一个LF的位置是4，因此前4个字节组成`35.6`，第5个字节是它的结束标记。
这里位置4同时也是这条消息不含LF的字节数量，不要把整份列表的数量误当成第一条消息的长度。

按下面的规则判断，再完成复制、解码与移除。

| 当前列表情况 | 本次应做什么 |
| :--- | :--- |
| 没有LF，数量为0。 | 返回`false`，等待输入，不读取不存在的元素。 |
| 没有LF，数量不超过256，包括恰好256。 | 保留全部字节，返回`false`，后续仍可能补上LF。 |
| 没有LF，数量达到257。 | 抛出`InvalidDataException`，不能继续无限保留。 |
| 第一个LF的位置为0。 | 这是空消息，抛出`InvalidDataException`。 |
| 第一个LF的位置为1至256，包括恰好256。 | 取出该LF之前的文字，移除这条消息和它的一个LF，返回`true`。 |
| 第一个LF的位置为257或更大。 | 这条消息超过256字节，抛出`InvalidDataException`。 |

复制时先创建能容纳这条消息的`byte[]`，长度就是第一个LF的位置，再从`_bytes`的位置0开始把同样数量的字节复制到新数组的位置0。
可以使用`List<byte>.CopyTo`的带范围重载，它的四个参数依次说明列表起点、目标数组、数组起点和复制数量。
再用.NET的`Encoding.UTF8.GetString`把这个完整数组解码，并把结果保存到`message`，本题不要求把每次网络到达的片段分别转成文字。
移除使用`RemoveRange`，从列表的位置0开始，数量是“消息字节数加1”，多出的1正是这条消息末尾的LF。
不能直接`Clear()`，因为同一列表后面可能还留着完整的下一条消息或尚未收齐的半条。

例如`35.6\nOK\nST`第一次取出后应剩`OK\nST`，第二次应取出`OK`并剩`ST`，第三次因没有LF而返回`false`并保留`ST`。
再追加`OPPED\n`后，才应取出`STOPPED`，最后列表数量为0。
有LF时判断的是第一条消息长度，不能因为列表里同时装了多条合法消息、总数量超过256，就先把它们都判为超长。

## 完成后依次观察完整收发、人工输入和分段回复

两端保持同一端口运行后，在上位机依次输入下面这些菜单项，每项输入后按回车并等到本次结果出现。

```text
o → 1 → 2 → 4 → 3 → 4 → c → o
```

预期依次看到连接成功、温度35.6℃、启动确认、状态RUNNING、停止确认、状态STOPPED，以及断开后可以再次连接。
模拟器一侧应显示相同命令与对应回复，设备控制的成功依据是对端回复和后续状态查询，不能只看客户端打印了一句文字。

然后可以在一个终端运行人工缓冲观察，它不会连接模拟器。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --observe
```

这组现有输入先只追加`33 35 EE FF`中的前两个字节，再追加`.6\nOK\nST`，最后追加`OPPED\n`。
完成后应依次取出`35.6`、`OK`和`STOPPED`，第一次输入后剩2字节，前两条取完后保留`ST`两字节，最后剩0字节，`EE FF`不应混入结果。
这里的连续回复专门用于观察同一个解析器，实际菜单仍一次只等待一条请求，不要求同时发出两条请求。
`--observe`没有自动覆盖空行和256/257字节的全部边界，上面的规则还需要在后续检查中单独核对，不能把它的一组输出当作全部边界已通过。

要观察真实连接里的分段写入，先停止原模拟器，再用下面的命令启动它，上位机重新连接后输入`1`。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --simulator 5000 split
```

模拟器先写回复的前2字节，等待150毫秒，再写余下字节，上位机应在凑齐LF之后才显示完整温度。
网络实际读取次数可能与模拟器写入次数不同，既可能看到多次接收，也可能一次读到完整回复，不要求日志必须恰好分成两次。
人工输入证明的是解析器面对指定分段的行为，两端真实日志才能说明这次TCP往返实际发生了什么，二者分别记录。

## 本题已提供的支持与证据边界

服务端监听和回复、菜单、单次读取循环、3秒请求保护、取消与关闭清理、回复显示都由教练提供，本题不要求重写，也不据此认定你已经独立掌握TCP Server、网络超时或完整资源管理设计。
3秒保护包住菜单的一次发送和读回复，超时后关闭旧连接并要求重新连接，避免没有请求编号时把迟到回复错交给下一条命令。
本题的核心实现仅为四组TODO，控制台支持代码能编译或打印菜单不代表这些TODO或DoD已经通过。
本文件列的是操作方式与预期结果，实际执行命令、输出和检查结论由后续真实记录补充，不把预期写成已发生的证据。
