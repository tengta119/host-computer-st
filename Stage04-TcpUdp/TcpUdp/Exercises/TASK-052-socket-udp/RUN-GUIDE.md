# TASK-052：完成Socket收发对照与UDP设备查询

## [模块 1] 核心任务目标

修改Task052SocketExercise.cs和Task052UdpExercise.cs，完成Socket按实际字节数收发，以及UDP按来源、编号和内容匹配查询回复。
使用Task052Simulator.cs核对TCP控制与UDP查询得到的设备状态。


**本题知识点**

| 知识点 | 当前代码中的含义与作用 |
| --- | --- |
| Socket与TcpClient | Socket是.NET提供的网络类，socket变量指向本次创建的TCP通信对象；本题直接调用它连接、发送和接收，替代此前TcpClient配合NetworkStream的调用方式。 |
| 实际发送数量 | SendAsync返回本次实际发出的字节数sent；offset保存累计已发数量，下次从offset继续，避免漏发或重复发送。 |
| AsMemory | request.AsMemory(offset, count)把数组中指定的一段交给发送方法；received.AsMemory()把接收数组交给接收方法写入，不另外复制数组。 |
| 有效接收与LF | ReceiveAsync返回本次写入数组的有效字节数，0表示对端已结束发送；TCP接收结果可能只有半条消息，因此外层继续用Task049LineBuffer累计到LF再取完整回复。 |
| UDP与UdpClient | UdpClient是.NET提供的UDP通信类，_udp指向本题用于查询的对象；一次SendAsync发送一条完整数据报，接收后直接检查这一条，不拼接TCP式LF缓冲。 |
| 接收来源 | UdpReceiveResult是.NET的UDP接收结果，Buffer是本条内容，RemoteEndPoint是发送者地址和端口；两者共同用于判断回复是否可信且符合约定。 |
| 查询编号 | requestId是本题生成的GUID字符串，随请求发出并要求回复带回；旧编号或其他编号被拒绝，每轮只接受一次匹配回复。 |
| 截止与取消 | deadline是教练创建的CancellationTokenSource对象，token把停止信号传给网络等待；到期结束本轮，Ctrl+C结束当前运行，收到错误报文不会重新计时。 |
| async与返回值 | async Task方法可在内部await网络操作；async Task<int>仍返回int数量，调用者通过await取得它，原Task.FromException占位已移除。 |


- 状态：✅ 已通关；创建/开启/完成日期：2026-10-20。
- 专属目录：`Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/`。
- TCP：命令末尾加LF，完整消息最多256字节，单次连接至完整回复共用3秒截止。
- TCP发送：每次最多提供3字节，按SendAsync实际返回数量推进。
- UDP请求：`Q|32位GUID编号|GET_TEMP`或`Q|32位GUID编号|GET_STATUS`。
- UDP回复：`R|同编号|同命令|值`，恰好四段，不加LF。
- UDP长度：1～128字节；每字节范围`[32, 126]`。
- UDP来源：地址和端口均须匹配目标。
- UDP值：GET_TEMP为0～100有限小数，按InvariantCulture解析；GET_STATUS为RUNNING或STOPPED。
- UDP等待：每轮2秒，忽略无效报文不重置截止；各轮编号不同，每轮只完成一次。
- UDP运行：同一个UdpClient连续查询两轮，外部取消后结束等待。

## [模块 2] 执行清单（按 TODO 拆分）

### TODO 1：连接Socket（学员已完成）

1. **文件与签名**：`Task052SocketExercise.cs`，`ConnectCoreAsync(Socket socket, IPEndPoint endpoint, CancellationToken token)`，返回声明由`Task`改为`async Task`。
2. **关键调用**：删除占位，执行`await socket.ConnectAsync(endpoint, token);`。
3. **推进现象**：连接成功，继续执行发送流程。

### TODO 2：完整发送命令（教练协助完成）

1. **文件与签名**：`Task052SocketExercise.cs`，`SendAllAsync(Socket socket, byte[] request, CancellationToken token)`，返回声明由`Task`改为`async Task`。
2. **核心伪代码**：`offset=0 → 循环至offset达到request.Length → count=Math.Min(3, request.Length-offset) → sent=await socket.SendAsync(request.AsMemory(offset,count), SocketFlags.None, token) → sent<=0则抛IOException → offset+=sent`。
3. **推进现象**：模拟器收到完整GET_TEMP，客户端进入回复接收流程。

### TODO 3：返回实际接收数量（教练协助完成）

1. **文件与签名**：`Task052SocketExercise.cs`，`ReceiveCoreAsync(Socket socket, byte[] received, CancellationToken token)`，返回声明由`Task<int>`改为`async Task<int>`。
2. **关键调用**：删除占位，执行`return await socket.ReceiveAsync(received.AsMemory(), SocketFlags.None, token);`。
3. **推进现象**：Socket客户端取得完整`35.6`，对端提前关闭时报告EOF。

### TODO 4：发送UDP查询（教练协助完成）

1. **文件与签名**：`Task052UdpExercise.cs`，`SendQueryAsync(UdpClient udp, byte[] request, IPEndPoint expected, CancellationToken token)`，返回声明由`Task`改为`async Task`。
2. **关键调用**：删除占位，执行`await udp.SendAsync(request.AsMemory(), expected, token);`，完整request只发一个数据报。
3. **推进现象**：模拟器收到完整UDP查询，客户端进入筛选与匹配流程。

### TODO 5：筛选当前查询的回复（教练协助完成）

1. **文件与签名**：`Task052UdpExercise.cs`，`TryAcceptDatagram(UdpReceiveResult datagram, IPEndPoint expected, string requestId, string command, out string reply)`，签名保持。
2. **核心伪代码**：`检查RemoteEndPoint地址/端口 → 检查Buffer长度/字节范围 → ASCII解码并Split('|') → 检查四段及R/编号/命令/值 → reply=第四段，返回true`；任一不符返回false。
3. **推进现象**：正常模式每轮输出一次接受结果，其他模式按下表过滤或超时。

## [模块 3] 调试与验证命令

从工作区根目录执行构建，每次修改源码后重新构建。

```powershell
dotnet build .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj --nologo
```

终端一启动服务端，切换模式前用Ctrl+C停止自己这份服务端。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 server 5000 normal
```

终端二依次运行客户端命令。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 socket 5000 GET_TEMP
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 socket 5000 START_DEVICE
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 socket 5000 GET_STATUS
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 udp 5000 GET_STATUS
```

- 5000已占用时，两端统一改用5001。
- 切换故障模式：替换服务端命令末尾的normal。
- 综合核对：既有WPF连接同一TCP端口，启停设备后对比窗口状态、UDP状态和模拟器日志。

## [模块 4] 故障模式速查表

| 模式名 | 触发机制 | 客户端预期行为 |
| --- | --- | --- |
| normal | TCP回复分两次写出，UDP正常回复。 | Socket取得完整回复，UDP每轮接受一次。 |
| silent | UDP不回复。 | 两轮查询分别在约2秒后超时。 |
| duplicate | 同一UDP回复发送两遍。 | 每轮只完成一次，下轮忽略上轮副本。 |
| stale-first | 暂存首轮回复，下一轮先发旧回复再发当前回复。 | 首轮超时，次轮忽略旧编号并接受当前编号。 |
| foreign-first | 另一个UDP端口先发内容正确的回复。 | 忽略错误来源，接受目标端口回复。 |
| wrong-id-first | 先发错误编号，再发当前编号。 | 只接受当前查询编号。 |
| malformed-first | 先发129字节和非法值，再发合法回复。 | 忽略超长内容和非法值，接受合法回复。 |
| close-tcp | TCP收到命令后关闭连接。 | 报告完整回复前对端关闭，释放连接。 |

- [x] DoD1：Socket连接、完整发送、有效接收、LF处理及关闭符合规则。
- [x] DoD2：UDP匹配来源/编号/格式/值，正确处理超时、重复、旧回复与取消。
- [x] DoD3：核对TCP控制、WPF显示、恢复路径和UDP查询的设备状态与记录。

- 准备证据：构建0警告/0错误，38项准备检查及6项骨架检查通过，详见[准备记录](./Verification/preparation-2026-10-20.json)。
- 教练范围：模拟器、入口、截止/取消、单请求保护和已有LF缓冲连接；TODO 2/3/4/5按用户要求由教练补齐，学员连接实现保留。
- 历史记录：[开启与验证记录](./Verification/task-description-before-rewrite-2026-10-20.md)；计划来源：[第四阶段](../../../../docx/plan.md)。

- 2026-10-20 TODO 3协助：仅改ReceiveCoreAsync签名与接收返回，临时构建0警告/0错误、8项局部检查通过，详见[协助验证](./Verification/todo3-assisted-2026-10-20.json)；任务仍🟡 进行中，DoD未勾选。

- 2026-10-20完成验证：控制台/WPF构建均0警告/0错误，111项实际Socket/UDP与WPF综合检查通过，195个受保护旧源码保持；详见[完成记录](./Verification/completion-review-2026-10-20.md)。
- 第四阶段：✅ 已通关；后续任务等待用户补充计划后确定。
