# TASK-052：从Socket往返到UDP查询

## 当前代码里谁负责什么

你已写过TCP连接调用、完整回复处理、WPF显示和心跳条件，本题复用这些经验，但不假定你已经用过.NET Socket、AsMemory或UdpReceiveResult。
[Task052SocketExercise.cs](./Task052SocketExercise.cs)是教练新建的练习类，QueryAsync为一次命令创建一个Socket，完成或失败后关闭它，ConnectCoreAsync/SendAllAsync/ReceiveCoreAsync是等你补齐的三个普通方法，不是.NET内置方法名。
[Task052UdpExercise.cs](./Task052UdpExercise.cs)也是教练新建的类，内部_udp是.NET UdpClient对象，保留同一本地端口连续查两次，让旧数据报有机会留给下一次查询，再由编号过滤。
[Task052Simulator.cs](./Task052Simulator.cs)是完整教练对端，TCP接收START_DEVICE/STOP_DEVICE改变_running，UDP的GET_STATUS读取同一个_running；它不是两个各自保存状态的模拟设备。
[Task052Entry.cs](./Task052Entry.cs)负责命令行选择和结果输出，Program.cs只增量加入--task052入口，旧任务命令与WPF启动窗口保持。

## 先只做TODO 1：让Socket连上模拟器

Task052SocketExercise.QueryAsync里的这行创建了本次要连接的对象。

```csharp
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
```

Socket是.NET的网络通信类，socket是教练命名的变量，指向刚创建的具体网络对象；三个参数分别选IPv4、字节流和TCP，本题都连接127.0.0.1。
以前你通过TcpClient.ConnectAsync连接，现在这一步直接对socket调用ConnectAsync，后续再直接对它发送和接收字节。
endpoint是.NET IPEndPoint对象，保存127.0.0.1与端口，已经由QueryAsync创建并传进ConnectCoreAsync；token由外层提供，用于3秒截止和Ctrl+C取消。

只改ConnectCoreAsync：将签名中的public Task改为public async Task，在方法中等待下面这个调用，再删掉Task.FromException占位。

```csharp
await socket.ConnectAsync(endpoint, token);
```

async和await仍是你前面用过的C#语法，ConnectAsync是.NET Socket的方法，ConnectCoreAsync是本练习自己命名的连接方法。
当前Task.FromException只是把NotImplementedException未完成错误交给外层await显示，不会联网，也不会自动补上实现。
先不改另外四个TODO，运行后应从“TODO 1：尚未连接Socket”推进到“TODO 2：尚未发送完整命令”；这是第一个小动作的预期现象，还不是收到35.6。
外层using块会在方法结束时释放socket，避免失败后留着当前连接；当前3秒是一次Socket连接、发送和完整接收共用的总截止，不是每次收到一点就重新计时。

## 从工作区根目录运行

先构建一次，终端一启动模拟器。

```powershell
dotnet build .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj --nologo
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 server 5000 normal
```

终端二运行Socket练习，命令省略时默认GET_TEMP，地址固定为127.0.0.1。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 socket 5000 GET_TEMP
```

每次改源码后重新构建，--no-build只运行上次构建结果；本机原5000端口已有模拟器时，本题两端可统一改用5001，不停止其他正在使用的程序。
结束自己这一轮程序使用Ctrl+C，看到模拟器停止日志后再切换故障模式，不在同一端口启动两份服务。
若需要重建正在运行的控制台程序，先结束自己这份模拟器，避免Windows锁住exe；WPF工程与启动窗口不需要改。

## TODO 2：按实际发送数量推进

GET_TEMP加LF共9字节，TCP发送不保证一次把你提供的所有字节都交出去，SendAsync的返回值表示本次实际发送了几个字节。
本题主动限制每次最多提供3字节，使你能在本机观察循环推进；这是人为分块，不冒充操作系统实际发生了部分发送。[Microsoft Learn：Socket.SendAsync](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.sendasync?view=net-10.0)
offset是需要你创建的普通int局部变量，表示已经成功发了几个字节，初始为0；下一次从这个位置继续，剩余数量是request.Length - offset。

```csharp
int count = Math.Min(3, request.Length - offset);
int sent = await socket.SendAsync(request.AsMemory(offset, count), SocketFlags.None, token);
```

Math.Min返回两数较小者，防止最后一段不足3字节时越界；request.AsMemory(offset, count)表示“从request数组的offset位置起提供count字节”，没有再复制一份数组，也没有改变offset。
SocketFlags.None是.NET提供的枚举值，表示本次不额外启用特殊发送选项；await得到的sent才是实际推进量，不能一律按count推进。
在SendAllAsync里改为async Task，自己连接循环、sent小于等于0时抛IOException、offset按sent增加这几步，移除占位；不要重复从数组开头发送，也不要再添加LF，外层已加好。
完成后应推进到TODO 3未接收提示，配合模拟器输出能看到完整GET_TEMP，尚未完成整次客户端回复读取。

## TODO 3：接收返回数量不是数组长度

ReceiveCoreAsync改为async Task<int>，等待Socket.ReceiveAsync并返回它实际读到的字节数；调用形式已写在源码TODO，received.AsMemory()表示把现有received数组作为可写区域交给接收方法。
外层received数组只有3字节，故意让35.6加LF至少需要两次接收；实际每次读多少仍由网络决定，不假定两次正好按某个位置分开。
外层已复用Task049LineBuffer，只有count个有效字节会追加，收到LF前继续读；count为0时抛出对端关闭异常，不将空数组当温度。
TODO 1至3完成后，GET_TEMP应输出完整35.6，GET_STATUS初始为STOPPED，START_DEVICE确认OK之后再次GET_STATUS应为RUNNING。
本题每条Socket命令新建连接，但模拟器状态保存在模拟器对象中，因此重新连接不会重置_running，重启模拟器才恢复STOPPED。

## UDP：先看一条数据报

UDP阶段完成TODO 4/5后，再运行下面命令，入口会用同一个UdpClient连续查询两次。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 udp 5000 GET_STATUS
```

例如请求Q|aabb...|GET_STATUS，其中aabb...代表本轮实际生成的32位十六进制编号；正常回复为R|同一个编号|GET_STATUS|RUNNING。
Q/R和竖线分隔符是本题自定义教学协议，不属于UDP内置规则；实际编号由Guid.NewGuid().ToString("N")生成，各轮不复用，模拟器把它原样带回。
每个UDP消息是一条完整数据报，不加LF，不经过Task049LineBuffer，也不把两条数据报拼成一条回复。
本题数据报只接受1至128字节的可打印ASCII，来源必须同时匹配127.0.0.1与配置端口，回复必须恰好四段，第一段R、第二段当前编号、第三段当前命令、第四段合法值。
GET_TEMP值用CultureInfo.InvariantCulture解析，只接受0至100的有限小数；GET_STATUS只接受RUNNING或STOPPED，NaN、Infinity、空内容和其他值均拒绝。
可用Double.TryParse并明确传NumberStyles.AllowDecimalPoint和CultureInfo.InvariantCulture，Double.IsFinite检查有限值；涉及这两个名字时需在文件添加using System.Globalization，它们是.NET提供的解析规则，不是本题自定义类。

## TODO 4：一条查询发一个数据报

SendQueryAsync里udp指向当前Task052UdpExercise内部的_udp，request已有编码后的完整查询，expected是目标地址和端口。
将方法改为async Task，等待源码TODO给出的UdpClient.SendAsync调用，移除未完成异常；与TCP不同，这里不要把同一条查询按3字节循环拆成多个数据报。
底层ReceiveAsync已经由教练连接，await得到的UdpReceiveResult是.NET提供的接收结果，Buffer保存这一条数据报字节，RemoteEndPoint保存发送者地址和端口。[Microsoft Learn：UdpClient.ReceiveAsync](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient.receiveasync?view=net-10.0)
只做TODO 4时，TODO 5仍拒绝所有回复，因此会显示忽略日志并在2秒后超时，这是骨架尚未完成的预期结果，不代表模拟器没有发送。

## TODO 5：只完成当前查询一次

TryAcceptDatagram是需要你实现的自定义筛选方法，参数datagram就是刚收到的.NET接收结果，expected/requestId/command是当前正在等待的目标、编号和命令。
先检查来源Address.Equals(expected.Address)及Port，再检查Buffer.Length和字节范围32至126，符合后用Encoding.ASCII.GetString解码和Split('|')拆段，按协议逐项验证。
reply是out参数，外层声明的reply变量会收到你赋的值；已提供String.Empty初始值，任何不符返回false，全部符合才将第四段赋给reply并返回true。
外层只在返回true后输出接受日志并结束这轮QueryAsync，重复数据报即使还在本地队列，下轮编号不同也应被拒绝，不要在筛选方法里改变当前编号。
外层2秒截止只创建一次，忽略错误回复不重置截止，超时结束后下一轮生成新编号；外部取消直接传播，不显示为设备超时，入口等待结束后才Dispose。
SemaphoreSlim是.NET提供的互斥等待工具，教练用_gate确保同一UdpClient一次只处理一轮QueryAsync，不让两个接收者抢同一回复；本题不要求你重写这层保护。

## 使用教练故障模式核对

| server模式 | 已提供的对端行为 | TODO完成后的预期结果 |
| --- | --- | --- |
| normal | 正常回复，TCP回复分两段写出。 | Socket得到完整值，UDP每轮接受一次。 |
| silent | UDP不回复，TCP仍正常。 | 两轮UDP分别在约2秒后超时，不误报成功。 |
| duplicate | 每条UDP回复发送两遍。 | 每轮只接受一次，上轮残留副本不能成为下轮结果。 |
| stale-first | 暂存一个来源的第一次回复，下一查询先发旧回复再发当前回复。 | 第一轮超时，第二轮忽略超过上一轮截止的旧编号，再接受当前回复。 |
| foreign-first | 另一个UDP端口先发送内容正确的回复，再由真实端口回复。 | 错误来源被忽略，真实来源被接受。 |
| wrong-id-first | 先发不同编号，再发匹配回复。 | 只接受当前编号。 |
| malformed-first | 先发129字节和非法值，再发正常回复。 | 超长与非法值被忽略，合法回复被接受。 |
| close-tcp | TCP收到命令便关闭，UDP仍正常。 | Socket报告完整回复前对端关闭，释放本次连接。 |

改变模式需要重启自己这一份模拟器，stale-first专门配合同一个UdpClient两轮查询；它确定性暂存旧回复，不宣称模拟了所有实际网络丢包或乱序。
最后可以让既有WPF窗口连接本题同一TCP端口，手动启停远端设备，再查询UDP状态与模拟端日志对应；采集是否开启和远端设备是否运行仍是两个不同状态。
本题不自动完成第四阶段，整题审查时按计划综合核对目标与真实证据，静态审查不能冒充实际Socket/UDP往返或完整WPF联调。

## 证据记录

当前教练准备检查详见[preparation-2026-10-20.json](./Verification/preparation-2026-10-20.json)，其中直接使用测试端收发模拟器不代表学员TODO已实现。
学员提交时可直接说“审查task052”，继续按既有源码审查偏好记录；运行观察与静态结论分别保留，不额外要求重复抄录已有日志。
