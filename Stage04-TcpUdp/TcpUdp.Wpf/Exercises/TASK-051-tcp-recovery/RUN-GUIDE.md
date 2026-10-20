# TASK-051：模拟器故障后恢复连接

TASK-050的连接、温度历史、曲线、报警和采集已经复制到本题独立窗口，原作品不改。
本题只围绕一个变化：模拟器不再正常回应时，窗口应结束旧等待、显示明确原因，之后按用户选择恢复，主动停止后不再偷偷重连。
自动重连和心跳判断目前留有TODO，初始不自动发心跳，也不假装已经安排了重连。

## 首个动作：把本次失败原因说清楚

打开[Task051Window.xaml.cs](./Task051Window.xaml.cs)，找到DescribeFailure；外层HandleFailure把这次捕获到的异常对象作为exception传进来，要你返回一段原因文字。
例如刚才已经连上模拟器，发送GET_TEMP后连续3秒没有完整回复，传进来的会是教练新建的Task051TimeoutException对象，它的Operation属性保存“业务请求”。
这个自定义异常类定义在[Task051TcpConnection.cs](./Task051TcpConnection.cs)，不是.NET内置异常，也不是“你本来就应该知道”的属性名。
你可以先补这一种情况，再保留其他错误的原有返回路径。

```csharp
if (exception is Task051TimeoutException)
{
    Task051TimeoutException timeout = (Task051TimeoutException)exception;
    return timeout.Operation + "等待超时，将结束旧连接。";
}
```

`is`是C#语法，这里检查实际传进来的异常是不是Task051TimeoutException。
确认之后，括号中的写法把同一个异常对象按这个具体类来使用，局部变量timeout仍指向原来的异常；因此下一行能够读取这个类才提供的Operation属性。
Operation的三个可能值分别是“连接”“业务请求”“心跳”，保留这个词就能区分到底在等谁，不能全部改成同一句“读取超时”。
这次return只把原因文字交回HandleFailure，不会退出窗口，也没有自己关闭连接；HandleFailure接着调用已经提供的ResetConnection清理，再进入重连安排。

随后在同一TODO 1中补清其他原因：InvalidDataException表示回复不符合当前协议，SocketException表示连接或底层Socket失败，IOException表示对端关闭或其他收发失败，返回原因时保留exception.Message中的具体说明。
这些是.NET提供的异常类；相关命名空间已在窗口文件中引用，不要求新建同名类。
当前骨架会把其他错误显示成“通信失败（原因分类待完成）”并清理，不会因为TODO未完成让窗口失去关闭能力。

## 运行一次“无回复 → 清理 → 手动恢复”

两个PowerShell终端都从`D:\host-computer-st`运行，先退出自己此前运行的旧模拟器，再在终端A构建控制台并启动本题故障模式。

```powershell
dotnet build .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj --nologo
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task051-simulator 5000 silent-temp
```

终端B先关闭自己运行的旧WPF窗口，构建并启动新入口。

```powershell
dotnet build .\Stage04-TcpUdp\TcpUdp.Wpf\TcpUdp.Wpf.csproj --nologo
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp.Wpf\TcpUdp.Wpf.csproj
```

当前App.xaml已指向Task051Window，TASK-050文件和窗口保留；也可以打开[阶段解决方案](../../../TcpUdp.slnx)，选择TcpUdp.Wpf运行。
先保持“故障后自动重连”未勾选，点击连接，再点击读温度。
模拟器会明确打印本次故意不回复温度，约3秒后窗口应显示业务请求超时、连接变为未连接，原历史保留，采集停止。
本题连接和发送的正常功能已复用，所以这一步不需要你重写创建TcpClient或UTF-8编码。
接着在终端A用Ctrl+C停止故障模拟器，改为正常模式，再在窗口手动连接并读温度，应恢复35.6℃。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task051-simulator 5000 normal
```

改变故障模式需要先结束自己运行的那一个模拟器，不要同时启动两个程序抢同一端口；5000被其他程序占用时，两端可统一改用5001。
模拟设备状态只在同一个模拟器进程内保留，重启模拟器后初始状态为STOPPED，不把上次客户端显示的运行状态当作当前设备事实。

## 本题采用的等待与恢复规则

| 当前在等什么 | 约定规则 |
| --- | --- |
| ConnectAsync建立连接。 | 最多3秒，立即拒绝连接可能更早失败，不能把所有连接失败都说成超时。 |
| 已发送的一条业务命令的完整回复。 | 最多3秒，失败就结束旧连接，不让迟到回复交给下一条请求。 |
| PING对应的PONG。 | 最多3秒，本题一次心跳无回应就结束本连接，不把无回应误写成设备已停止。 |
| 自动重连。 | 用户先启用选项，失败结束后等待2秒再尝试，最多5次连续失败；连接建立并完成状态查询后计数归零。 |
| 空闲心跳。 | 距离最近一次有效通信至少2秒、没有采集、没有业务请求在等时，才通过同一请求入口发送PING。 |
| 恢复后的采集与设备控制。 | 查询真实设备状态，采集保持停止，不重发上次启动/停止命令，由用户决定后续操作。 |

这些秒数是本题明确的教学配置，不是所有设备通用的参数，定时器实际回调时刻仍受窗口队列调度影响。
_connection.IsConnected当前只表示辅助对象还保留着_stream，不是网络探活；静默故障需要通过实际请求或心跳发现。

通信辅助创建了一个带3秒截止时间的取消源，同时监听当前窗口会话的取消。
如果3秒到了、外层会话没有被用户取消，辅助才把OperationCanceledException转换成Task051TimeoutException；用户点击断开或关窗则按外部取消处理，不伪装成设备超时。
这套底层取消与清理由教练提供，先从实际异常的Operation读懂当前等待位置，不把骨架存在就当作已掌握全部取消机制。

## TODO 2：安排下一次尝试，不在catch里无限重试

2026-10-20按用户要求由教练补齐TODO 2，以下说明保留为代码阅读依据，本次协助不计学员自主实现。
当前RecoveryText.Text显示下一次是第几次自动连接，_reconnectAttempts + 1只用于提示，不提前增加实际尝试次数。
_reconnectTimer是窗口已有的.NET DispatcherTimer对象，调用它的Start()会开启已配置的2秒等待，到时由ReconnectTick检查状态并执行连接，当前方法不会立刻连接或阻塞窗口。

失败处理会先清理旧连接，再调用ScheduleReconnect，当前方法已经检查窗口是否关闭、用户是否还希望保持连接、自动选项是否勾选以及是否已达到5次上限。
这些检查之后，TODO 2要设置RecoveryText.Text为下一次等待说明，并启动同一个_reconnectTimer。
_reconnectTimer是教练创建的DispatcherTimer对象，Interval已经设为2秒，Start会安排之后的通知，不是立即在当前方法里再连接一次。
到时调用的ReconnectTick已经提供：它先Stop这个定时器，检查没有其他操作在忙，增加尝试次数，再等待AttemptConnectAsync，失败才回到下一次安排。
因此不要在ScheduleReconnect里写while或直接递归调用ConnectClick，也不要在窗口线程Thread.Sleep两秒，避免无法点击停止或不停重叠建立连接。
ScheduleReconnect可能在一次失败处理尚未执行finally时调用，_busy此时仍为true；不要自行增加“只要_busy就永远不安排”的判断，真正开始尝试前的检查已经在ReconnectTick里。

完成后先勾选自动选项再制造故障，停止模拟器时应看到每次等待和第几次尝试，恢复正常模拟器后连接成功；持续无法连接时，第5次失败后停止自动尝试，仍可手动连接。
5次限制针对连续未能成功恢复的尝试，不是程序整个生命周期只能连5次；一次成功恢复后重新计数。

## TODO 3：每次恢复先询问状态

AttemptConnectAsync创建并连好新会话后，已经调用QueryStateAfterConnectAsync，本题手动连接和自动连接都走这里。
当前方法里的Task.CompletedTask只是占位，没有发送状态查询，也不会帮你取得远端设备状态。
你要等待_connection.RequestAsync发送GET_STATUS，取得完整的reply，调用RecordLog记录，再交给ApplyReply处理GET_STATUS对应的真实状态，移除占位。
RequestAsync是自定义通信辅助的方法，ApplyReply是复制自TASK-050的本窗口方法，它已经能检查RUNNING/STOPPED并更新状态控件，不需要重复写协议解析。
此时连接步骤还占着_busy，所以这里直接使用_connection.RequestAsync，不调用会因为_busy返回的RunCommandAsync；同一时刻仍只有这一条读取路径。
不能把“重连成功”当作“设备正在运行”，也不要补发START_DEVICE、STOP_DEVICE或启动_pollingTimer来猜测恢复前的意图。

## TODO 4：只在合适时机发心跳，并核对PONG

已有HeartbeatTick每隔约500毫秒检查一次，窗口已关闭、正在忙、没有连接或不再希望保持连接时都会先返回。
只有你补好的CanSendHeartbeat返回true，它才调用同一个RunCommandAsync发送PING；这个方法返回false并不是断开，只表示这次不发心跳。
TODO 4a要判断_collecting为false，并且当前时间距离_lastActivity已经达到HeartbeatIdle。

```csharp
TimeSpan idle = DateTime.UtcNow - _lastActivity;
```

DateTime.UtcNow是.NET提供的当前UTC时间，_lastActivity是教练字段，保存最近连接/有效回复结束时的UTC时间，二者相减得到实际空闲时间idle。
HeartbeatIdle是教练配置的TimeSpan，值为2秒；例如最近通信结束于12:00:00，当前12:00:01还不够，12:00:02才达到该条件，不用对比本地时间字符串。

PING的回复交给ApplyReply后，会先调用ValidateHeartbeatReply，TODO 4b要求只有PONG才能正常返回，其他文字抛出InvalidDataException。
后面的“心跳正常”提示只有验证方法正常结束后才显示，PONG不会进入温度历史。
当前验证占位会明确抛出“待完成”错误，CanSendHeartbeat也先返回false，因此需要补齐4a和4b后再观察自动心跳；可先用“检查一次心跳”按钮核对正确PONG。
正在采集时已有周期GET_TEMP在持续验证业务通信，本题不额外插入心跳争抢同一回复，停止采集并空闲后才开始PING。

## 可控故障模式

| 模式参数 | 模拟器具体做什么 | 主要观察内容 |
| --- | --- | --- |
| normal | 按协议正常返回温度、状态和PONG。 | 正常连接、恢复后的状态查询及心跳。 |
| silent-temp | 收到GET_TEMP后不回复，仍保留连接。 | 业务请求等待到期，而不是立即断开。 |
| delay-temp | GET_TEMP回复推迟4秒。 | 客户端3秒结束旧连接，迟到35.6不能当成新连接的状态回复。 |
| close-temp | 收到GET_TEMP后关闭当前连接。 | 读取结束或收发失败反馈，与无回应超时区分。 |
| silent-ping | PING不回复，其他命令仍可回应。 | 心跳等待到期，与设备运行状态分别显示。 |

改变模式时只替换命令中的最后一个词，使用的是新[Task051Simulator.cs](../../../TcpUdp/Exercises/TASK-051-fault-simulator/Task051Simulator.cs)，旧TASK-049模拟器没有改动。
这些是本机可控故障，不能因为看见它们生效就认定已覆盖所有断网、路由或硬件故障；本机没有服务监听时常是立即拒绝，不是连接建立超时的实测证据。

## 主动停止与准备证据

主动断开会清除_wantConnected并停止恢复与心跳定时器，随后取消旧会话；取消自动选项会停止重连等待，正在自动连接时也会取消，正常已建立的连接保留。
关窗会先禁止后续动作，再停止所有定时器并撤销通知；旧连接的异常或结果不能在后来改写新连接/已关闭窗口，这些安全收尾由教练提供，不要求重复实现。
本次40项窗口/辅助准备检查、4项控制台入口检查和截图只验证教练部分，历史数据是布局注入，重连等待/次数保护也是直接设置场景，不代表你的自动调度、恢复查询或心跳TODO已通过。
当前三个DoD仍未勾选，完成后说“审查TASK-051”，继续按已有的源码审查口径记录；实际运行、教练辅助与自主实现分别注明。
