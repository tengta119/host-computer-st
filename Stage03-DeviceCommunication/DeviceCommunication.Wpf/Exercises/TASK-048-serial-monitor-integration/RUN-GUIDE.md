# TASK-048：在同一窗口看清串口发生了什么

这次保留TASK-047的协议判断、坏帧恢复、历史、曲线、报警和3秒等待，增加下方通信日志，不要求重新写前几题。
三组TODO分别完成“把日志放进表格”“只保存本次有效字节”“在连接和等待变化时补一条状态记录”，事件登记、窗口线程交接、协议拒绝原因和旧连接保护由教练准备。

## 先分清三种看起来相近的记录

窗口显示“接收，2字节，AA 01”，只表示本次Read确实拿到两个字节，不说明一帧已经完整。
随后看到“等待半帧，缓冲保留2字节”，表示现有解析要继续等待剩余内容，不会新增温度。
后来收到03 00 FA A8，已有组包得到完整AA 01 03 00 FA A8，检查通过才记录“有效帧”并发出读数，表格增加25.0℃。
如果完整候选是AA 01 03 03 E9 9A，就记录“拒绝候选，原始值1001超过1000”，恢复循环继续寻找后面的好帧，请求也不会因为这条坏帧结束等待。
本次Read和一个完整候选是不同对象，所以收到4字节的日志与有效帧6字节的日志可能同时出现，二者不是在重复报告同一层含义。

## 第一步：TODO 1让一条记录出现在表格里

打开Task048Window.xaml.cs，找到AppendLogOnUi，先完成TODO 1。
构造方法已经让CommunicationLogGrid.ItemsSource指向_communicationLogs，窗口启动时也已经调用RecordStatus创建一条“窗口已就绪”记录，当前缺少把它加入集合的操作。
_communicationLogs是这个窗口的ObservableCollection<Task048LogEntry>对象，给它添加条目后，已有ItemsSource连接会让下方日志表出现对应行。
entry是AppendLogOnUi这次收到的一条日志对象，可以先用下面一行观察它进入表格。

```csharp
_communicationLogs.Add(entry);
```

接着完成同一TODO里的数量控制：超过MaximumLogCount时删除下标0的最旧记录，直到数量不超过200，再让LogCountText显示实际条数和上限。
MaximumLogCount是教练声明为const int的字段，值固定为200，表示日志上限；_communicationLogs.Count是现在实际有几条，不要用温度历史_history的数量代替。
这只限制通信日志，不清空或截短温度历史；清空日志按钮也由教练接好，只清通信日志。
首个可观察结果是重新启动窗口后，下方出现“窗口已就绪，连接端口后开始通信。”一行，不需要先连接串口。
在TODO 1完成前，骨架表格为空是预期占位，不能用教练临时布局截图中的三条示例当作你已经接好日志的证据。

## 这一行日志到底是什么对象

Task048LogEntry是教练新建的普通C#类，不是WPF自带日志API，一条对象对应下方表格的一行。
例如下面创建一条表示“连接5的COM1已经发送55 01”的记录，new负责创建对象，构造方法把这些具体内容保存到属性里，CreatedAt由构造方法读取电脑当前时间。

```csharp
Task048LogEntry entry = new Task048LogEntry(5, "COM1", "发送", 2, "55 01", "请求已写入串口。");
```

| 构造方法里的位置 | 这份日志具体保存什么 |
| --- | --- |
| 1，sessionId | 哪次连接产生的记录，例如5，不是请求编号。 |
| 2，portName | 这次操作的端口名，例如COM1。 |
| 3，category | 是发送、接收、拒绝候选、有效帧，还是一条状态说明。 |
| 4，byteCount | 这条字节记录真正保存了几个字节，状态说明传0并在表格里留空。 |
| 5，bytesHex | 这些字节的十六进制文字，例如55 01。 |
| 6，message | 给人看的解释，例如原始值超范围或请求已写入。 |

CreatedAt记录电脑生成日志的时间，不是设备测量时间；SessionText和ByteCountText只是教练提供的表格显示辅助，未连接的窗口就绪记录不显示连接编号，纯状态记录不显示字节数。
表格里的长原始字节或说明会省略显示，把鼠标停上去可看完整文字，避免一次256字节把整行撑出窗口。

## 第二步：TODO 2只保留本次有效的字节

打开Task048SerialConnection.cs，找到AddByteLog，完成TODO 2。
这个方法由教练命名，参数data是当前要记录的数组，count是这次真正有效的字节数量，logs是暂时收集本批日志的List<Task048LogEntry>对象。
例如data有256个位置，本次Read只返回2，那么只有data[0]和data[1]属于本次接收，后面254个位置不能写进这条日志。
你要新建长度为count的byte数组，使用for把下标0到count-1复制进去，交给教练的FormatBytes方法得到文字，然后创建Task048LogEntry并加入logs。
FormatBytes是本文件里已提供的普通方法，接收你复制好的byte[]，用X2把每个数转成两位十六进制文字并加空格；它不读取串口，也不判断协议是否合法。
新日志构造方法的六个实参按上表填写，使用当前方法收到的sessionId、portName、category、count、生成的文字和message，不要重新去猜此刻的连接编号或端口。
发送时调用点已经放在Write成功之后，收到数据时使用Read返回的bytesRead，完整候选使用frame.Length；这些调用位置由教练接好，不要再添加一套SerialPort对象。
最终条目保存文字和实际数量，不保留那个可能继续复用的接收数组，所以后续Read不会把已经显示的日志改成别的内容。

## 第三步：TODO 3记录连接和等待的结果

回到Task048Window.xaml.cs，四个TODO 3注释共同完成同一件事：在已有状态变化后调用RecordStatus。
RecordStatus是教练命名的窗口辅助，四个参数依次是category、message、sessionId、portName，内部创建一条不带原始字节的Task048LogEntry，再调用你已完成的AppendLogOnUi。
例如在连接成功的位置，可以这样记录已经发生的结果。

```csharp
RecordStatus("连接成功", OperationText.Text, _connection.CurrentSessionId, portName);
```

| 位置 | 应记录的内容 | 使用的连接信息 |
| --- | --- | --- |
| ConnectClick的Open成功之后。 | 连接成功与已有OperationText.Text。 | 当前CurrentSessionId和方法里的portName。 |
| ConnectClick的catch中。 | 连接失败与已有OperationText.Text。 | 本次尝试的CurrentSessionId和portName，不能因为打开失败就丢掉尝试的端口名。 |
| EndWaiting的wasWaiting分支里。 | 等待结束与传入的message，包括完整回复、超时或主动收尾。 | 方法开头已经保存的waitingSessionId和waitingPortName。 |
| RecoverConnection的finally里。 | 连接清理与最终result，保留原错误及可能追加的关闭错误。 | 关闭之前已保存的oldSessionId和oldPortName。 |

EndWaiting会把_waitingSessionId改成-1并清掉_waitingPortName，所以日志必须使用清理前保存的局部变量，不能到最后再读取已清空的字段。
RecoverConnection会Close并改变当前连接编号、撤掉端口对象，所以也要用关闭前保存的编号和端口，避免把连接5的清理结果标成后来连接6或7的结果。
wasWaiting是教练在EndWaiting开头保存的bool值，只有原本正在等待才写“等待结束”，这样连接之前调用一次EndWaiting不会凭空多出一条请求结束记录。
这些状态记录在窗口线程调用，不需要再套BeginInvoke；接收线程来的批量日志则有教练提供的另一条交接路径。

## 接收日志怎样到达窗口

Task048LogsEventArgs是教练新建的通知参数类，Entries是一批日志对象的数组，SessionId说明这批日志来自哪次连接；LogsReceived是串口辅助新增的事件，窗口构造方法已登记SerialLogsReceived。
串口辅助先在处理数据时收集条目，离开lock后PublishLogs才发出通知，不在持锁期间调用窗口代码。
SerialLogsReceived沿用上一题的Dispatcher.BeginInvoke，让窗口线程处理这批条目，并在真正执行时再用BelongsToCurrentConnection检查编号，关闭后或旧连接的排队条目会被丢弃。
该检查不要求IsOpen为true，因此当前端口已经失效时仍能报告这次连接的错误；重连后的新编号则不会被旧条目冒充。
原来已经显示的旧日志可以保留，因为每行有自己的编号和端口，这与丢弃尚未显示的旧通知不同；温度历史也继续保留。
窗口关闭时先设置关闭标记，再停止计时、撤掉日志/读数/错误事件并Dispose，已经关闭的窗口不再追加日志；不要为了显示一条关闭结果又强行更新关闭窗口。

## 已有协议为什么多了一个返回原因的方法

Task048Protocol保留TASK-047的所有条件，只把原来的true/false结果同时整理为具体文字，GetRejectionReason也是教练命名的方法。
它返回空字符串表示符合规则，返回非空文字表示拒绝原因；串口辅助只调用这一份判断，不为日志再执行第二套不同的协议解析。
IsValidFrame只是判断GetRejectionReason是否为空的兼容辅助，不是另一份规则。
返回原因只说明第一项发现的不符合，不声称一条坏候选只有一个错误；恢复循环仍按上一题删首字节继续，合法帧仍消费6字节。
本题没有新增设备协议、框架、数据库或新驱动，TASK-044至047原文件保留，修改的共享入口仅为WPF项目App.xaml。

## 运行与逐步观察

先关闭旧任务窗口及占用串口的旧模拟端，沿用已有虚拟串口对，模拟端仍使用TASK-047的专用参数，COM2只是实际对端端口的示例。

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --task047-simulator COM2
```

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\DeviceCommunication.Wpf.csproj
```

1. 先完成TODO 1，启动窗口，观察就绪记录与日志条数；这一处不需要串口。
2. 完成TODO 2，再让窗口连接另一端，例如COM1，模拟端输入2和3，观察本次接收2/4字节、半帧等待与最终有效6字节帧，温度只新增一次。
3. 模拟端输入5至8，观察校验、设备、命令和范围的拒绝原因；输入10、11、12再用3补齐，确认坏候选后恢复和内部AA规则保留。
4. 完成TODO 3，先点发送但不发有效回复，约3秒后观察等待结束/超时记录；再发请求并在3秒内让模拟端输入1，观察有效回复结束等待，按钮仍可再次发送。
5. 主动断开再重连，检查记录显示各自连接编号，历史保留，旧半帧不拼进新连接；关闭窗口后不再追加旧通知。

菜单r只读取请求，不自动回复；菜单14发送2048个99后接A，可观察多次接收记录与噪声汇总，不能预设一次Write对应一次接收。
只发坏帧也应有原始接收和拒绝说明，但不会新增温度或结束等待；没有任何字节的设备无回复则只会走等待超时，不应伪造一条接收记录。
超过200条日志后只保留最近200条，清空日志不清温度历史，重新连接也不应该重复登记日志事件。
只看样例仍可使用控制台原有--task047-list参数，本题未改Program.cs或模拟端源码。

## 准备证据与验收边界

教练仅构建新骨架、显示/关闭窗口，并在临时宿主直接放三条布局示例检查最小尺寸；示例不是通过你未完成的AppendLogOnUi写入，也不是实际串口数据。
三个TODO尚未完成，DoD均未勾选，教练提供的日志通知、格式辅助与拒绝原因整理不计学员独立掌握。
后续审查继续遵从用户“只看源码，源码正确即可通过”，本题可合并必要收尾，不把每个日志字段另拆任务；实际串口联调只有获授权并真实执行后才记录，不擅自宣称阶段通关。
