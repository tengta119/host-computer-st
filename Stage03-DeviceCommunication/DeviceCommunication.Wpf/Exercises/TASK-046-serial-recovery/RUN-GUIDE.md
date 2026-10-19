# TASK-046：让串口在等不到回复后还能继续使用

这次直接沿用TASK-045的连接、收发、表格、曲线和报警，新增代码集中在Task046Window.xaml.cs的四个TODO，不重写旧功能。
Task046SerialConnection是教练从上一题辅助复制后调整的类，Task045Reading和两种事件参数仍直接复用上一题，原TASK-045文件不修改。

## 先看这次要解决的现象

窗口把55 01写入串口成功，只能说明这次写入成功，设备可能还没处理，也可能根本不回复。
因此发送成功后，窗口先显示“等待回复”，暂时禁止再次发送；同一连接收到下一条完整读数，就恢复发送按钮。
本题把等待上限定为3秒，超过后显示“响应超时”，结束这次等待，但保留串口连接，允许重发或主动断开。
半帧不能结束等待，也不能因为等了3秒就被当作完整帧；本题超时后保留半帧，关闭连接才清理旧半帧。
当前帧没有请求编号，所以这里只做“同一连接一次等待、发送后收到下一条完整读数”的教学约定，迟到的上次回复仍可能被当作下次回复，不能用于声称严格匹配真实设备请求。

## 第一步只做TODO 1：发送后进入等待

打开本目录Task046Window.xaml.cs，找到BeginWaiting，这是教练自己命名的方法，SendClick已经在SendRequest成功之后调用它。
你要让这个方法记录本次连接和发送时间、启动计时、显示等待状态，然后刷新按钮；先完成这一处即可观察“点发送后，发送按钮变灰”。
本次用到的字段都已建好，下面先说明它们分别保存什么。

| 代码中的对象或字段 | 在这个窗口里具体负责什么 |
| --- | --- |
| `_waitingForResponse` | 你给它赋true表示正在等回复，已有RefreshConnectionControls会据此禁用发送按钮。 |
| `_waitingSessionId` | 保存发送时`_connection.CurrentSessionId`的值，用来确认等待属于哪次连接。 |
| `_requestSentAt` | 保存发送成功时的`DateTime.Now`，已有接收代码用它排除发送前已产生、但尚未显示的旧读数。 |
| `_requestWatch` | .NET的Stopwatch对象，记录这次请求已经等了多久。 |
| `_timeoutTimer` | WPF的DispatcherTimer对象，启动后约每200毫秒安排窗口执行CheckResponseTimeout。 |
| `RequestStatusText` | 界面上显示“等待、收到回复或超时”的TextBlock控件。 |

Stopwatch是.NET提供的秒表类，_requestWatch是教练给这个具体秒表起的字段名。
看下面一个独立的小例子，先理解每个操作，再把对应操作用在当前窗口的_requestWatch上。

```csharp
Stopwatch watch = new Stopwatch();
watch.Restart();
double seconds = watch.Elapsed.TotalSeconds;
watch.Reset();
```

第一行创建一个秒表对象，第二行把它归零后开始计时，第三行读取目前经过了多少秒，第四行停止并归零。
这四行连续执行时几乎没有经过时间，例子只说明方法用途；当前窗口是在发送后Restart，在之后的CheckResponseTimeout里读取Elapsed。
`_requestWatch.Elapsed >= _responseTimeout`可以比较已等待时长与3秒上限，显示小数秒可以使用`_requestWatch.Elapsed.TotalSeconds.ToString("F1")`。
读取秒表不会让线程睡眠，也不要用Thread.Sleep等3秒，否则窗口会被阻塞。

DispatcherTimer是WPF提供的定时器类，_timeoutTimer是当前窗口创建的具体对象，构造方法已经设置Interval并通过Tick += CheckResponseTimeout登记了检查方法。
调用_timeoutTimer.Start()之后，WPF才会反复安排CheckResponseTimeout；调用Stop()就停止后续定时检查，已经排队的检查还要靠方法内的等待标记和连接编号判断是否有效。
CheckResponseTimeout在窗口线程执行，因此可以更新RequestStatusText，不需要再包一层BeginInvoke。
这个定时器只检查等待时间，不生成温度，也不靠时间判断帧边界；窗口忙时检查会晚于预定时刻，200毫秒不是严格实时保证。

TODO 1完成后调用RefreshConnectionControls即可让按钮读取新的_waitingForResponse，OperationText可以说明55 01已经发送，RequestStatusText显示正在等待。
此时TODO 2和3还未完成，收到回复或过3秒不会自动恢复按钮；这只是第一步观察，不能当作本题完成。

## 接着把收到回复、超时和恢复接成一个流程

TODO 2在EndWaiting里完成共用收尾：Stop定时器、Reset秒表、_waitingForResponse恢复false、_waitingSessionId恢复-1、显示传入的message，再刷新按钮。
ApplyReadingOnUi已经检查当前连接及发送时间，并在完整读数到达时调用EndWaiting，之后继续原有历史、曲线与报警更新；不用在串口辅助里重复写等待逻辑。

TODO 3在CheckResponseTimeout里先排除已关闭窗口和没有等待的情况，再确认_waitingSessionId仍属于当前连接。
发现等待属于已经结束的连接，就结束旧等待；若编号仍是当前连接但IsConnected已经false，就调用RecoverConnection说明连接失效；其余情况读取秒表，未到3秒显示进度，到时调用EndWaiting说明响应超时。
超时分支不调用Close，也不清历史或半帧，所以EndWaiting必须恢复发送按钮；超时后收到的完整帧仍可正常进入历史。

TODO 4在RecoverConnection里处理主动断开、发送失败或接收失败：先结束等待，再尝试Close，即使Close报错也要刷新控件并保留reason中的原始原因。
普通try/catch/finally即可完成，finally无论try成功还是catch处理异常后都会执行，所以把刷新按钮和最终显示放在那里，不要在catch里把原失败原因覆盖掉。
串口辅助的Close已经先撤掉旧端口、增加连接编号并清缓冲，再在锁外释放端口，因此释放过程中发生异常，也不会继续把旧对象当成可用的新连接。

窗口关闭时的停表、取消事件登记和最终Dispose由教练提供保护，保留即可；你的EndWaiting完成后，关闭也会走同一个等待收尾方法。

## 为什么处理错误要另查连接编号

收到读数时，IsCurrentSession要求“编号相同、端口存在、仍然打开”，避免显示已经失效端口的读数。
收到错误时，端口可能恰好已经关闭，所以BelongsToCurrentConnection只检查“编号相同、端口对象还属于这次连接”，不能因为IsOpen为false就把本来应该处理的错误忽略掉。
这两个方法都是教练定义的普通辅助方法，CurrentSessionId也是教练提供的属性，不是SerialPort自带的请求编号。
错误仍沿用上一题的Dispatcher.BeginInvoke安排窗口处理，进入窗口后再查编号，防止排队的旧错误断开新连接。

## 运行方式与可观察现象

在仓库根目录分别使用两个终端，模拟端端口替换成你实际虚拟串口对的另一端，COM2只是示例。

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --simulator COM2
```

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\DeviceCommunication.Wpf.csproj
```

窗口选择虚拟串口对给上位机的一端，例如COM1；两边不能打开同一个端口，串口对沿用TASK-044的，无需每个任务重新创建。
先做TODO 1并观察发送按钮变灰，然后完成其余TODO再观察下面这些完整场景。

| 操作 | 完成实现后应看到什么 |
| --- | --- |
| 点击发送，模拟端不发数据。 | 窗口先显示等待，约3秒后的检查显示超时，发送按钮恢复，仍可断开。 |
| 点击发送，在3秒内让模拟端用1发完整A帧。 | 等待结束，历史增加一条25.0℃读数，曲线和报警保持原逻辑。 |
| 模拟端用2发A的前半帧，暂不发3。 | 不新增读数，半帧不会结束等待；补3后才成为完整读数。 |
| 等待中主动断开，再重新连接。 | 旧等待结束，旧半帧和旧通知无效，新连接可以重新发请求。 |
| 发送或接收确实发生异常。 | 显示原失败原因，释放旧连接，按钮允许再次连接。 |
| 等待中关闭窗口。 | 定时器停止、事件取消登记，旧通知不能继续更新已关闭窗口。 |

模拟端r只读取请求，不自动回数据，使用1等发送命令才会回复。
在某些虚拟串口驱动上，模拟端退出不会令窗口这端的IsOpen变false，此时观察到的通常是“请求超时”，不能把它说成已验证真实拔线检测。
本题在发送/接收操作和等待期间检查连接问题，不承诺空闲且没有系统通知时立即发现硬件拔出，也不要求自动无限重连。

## 来源与证据边界

定时器与秒表行为依据[Microsoft DispatcherTimer文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?view=windowsdesktop-10.0)和[Microsoft Stopwatch文档](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-10.0)。
TASK-046当前只准备骨架，四个TODO和三个DoD均未完成，教练的准备检查不计学员超时、恢复或真实串口联调证据。
后续审查沿用用户“只检查源码，源码正确即可通过”的偏好，并明确保留静态审查与实际运行的区别。
