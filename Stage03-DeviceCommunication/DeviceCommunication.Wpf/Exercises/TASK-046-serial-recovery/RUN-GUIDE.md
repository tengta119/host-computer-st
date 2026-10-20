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


### TODO 2里的“Reset、清除标记和编号”具体写什么

2026-10-19用户指出这三个动作的含义不清，原注释没有把动作对应到具体字段，现补下面三行说明，不把教练表达不清记为学员错误。

```csharp
_requestWatch.Reset();
_waitingForResponse = false;
_waitingSessionId = -1;
```

_requestWatch是保存本次等待时长的秒表对象，Reset是.NET提供的方法；例如已经计了1.8秒，调用后停止计时并变回0秒。
_waitingForResponse是教练起名的bool字段，true表示现在正在等回复，赋false表示这次已经不等了；不是删掉这个变量，RefreshConnectionControls会据此在串口仍连接时恢复发送按钮。
_waitingSessionId是教练起名的int字段，例如等待开始时保存连接编号5，结束时赋-1表示现在没有哪次连接正在等待；-1只是本题选的标记值，不是C#特殊指令，也不会改变_connection.CurrentSessionId或断开端口。
已经写的_timeoutTimer.Stop()停止定时检查，_requestWatch.Reset()停止并归零秒表，这两个对象负责的事情不同；其后还需要显示message和刷新按钮，才会让界面反映新的等待状态。
与结束时的false相对应，BeginWaiting要把_waitingForResponse设成true并刷新按钮，当前学员代码已记录编号/时间并启动两对象，但这两处尚未连接，本轮只解释和改注释，保留学员已有实现。


### TODO 3每次检查到底做什么

CheckResponseTimeout是教练命名的普通方法，_timeoutTimer启动后会反复安排它执行；每次进来先判断这次等待是否还需要处理，最后才看是否已等够3秒。
原注释把多个判断挤成一句，现改成下面这些具体情况；命中需要结束本次检查的分支后写return，只离开本次方法，不关闭整个程序。

| 按顺序检查的代码 | 现在是什么情况 | 应做什么 |
| --- | --- | --- |
| `_isClosing || !_waitingForResponse` | 窗口已开始关闭，或者当前根本没在等回复。 | 直接return。 |
| `!_connection.BelongsToCurrentConnection(_waitingSessionId)` | 记录的等待不属于现在保留的连接。 | EndWaiting说明原连接已结束，然后return。 |
| `!_connection.IsConnected` | 等待仍属于这次连接，但端口已经不处于打开状态。 | RecoverConnection说明端口失效，然后return。 |
| `_requestWatch.Elapsed >= _responseTimeout` | 当前连接仍打开，但已经等了至少3秒，没有结束等待。 | EndWaiting显示响应超时，然后return，保留串口连接以便重发。 |
| 上面都不满足。 | 仍在等待，并且还没到3秒。 | 显示已等待时长，等下一次检查。 |

例如_waitingSessionId是5，而当前_connection.CurrentSessionId已经是7，就说明等待记录还留在上一次连接5；结束这份等待记录即可，不要为连接5调用RecoverConnection把现在的连接7也关掉。
BelongsToCurrentConnection和IsConnected是教练提供的两个检查，前者确认等待编号对应当前保留的端口对象，后者确认这个端口是否仍打开，因此先后两步分别处理“等待过期”和“当前端口失效”。
EndWaiting只清等待和更新显示，不关闭串口；RecoverConnection用于要释放连接的情况，这两个方法不是同一用途。
当前源码BeginWaiting尚未把_waitingForResponse设为true或刷新按钮，如果直接加第一个判断，每次检查都会因false返回；开始时需要设true并刷新，结束时设false并刷新，才能让整个等待流程连接起来。
本轮只澄清注释和说明，保留学员已有实现，未构建/运行/测试或改变任务验收，Q4仍待复习，不记学员错题。


### 怎样写“窗口已关闭，或者现在没在等回复”的判断

在CheckResponseTimeout里写下面这个判断，成立就直接结束本次检查。

```csharp
if (_isClosing || !_waitingForResponse)
{
    return;
}
```

_isClosing和_waitingForResponse都是教练在当前窗口类里定义的bool字段，不是WPF自动维护的属性。
_isClosing为true时跳过检查；当前代码在OnClosed中将它设为true，准确说表示窗口已经关闭，原说明“正在关闭”是对这个收尾标记的宽泛描述。
_waitingForResponse为true表示正在等待回复，前面的!是C#取反运算符，因此!_waitingForResponse在该字段为false时成立，表示现在没有在等待。
||是C#的“或者”，左右两个条件只要任意一个为true，就进入花括号执行return；return只退出这一次CheckResponseTimeout，不退出程序。
开始等待时必须在BeginWaiting将_waitingForResponse赋true，结束时EndWaiting赋false，判断本身只读取字段，不会自动替你进入等待。
2026-10-19本轮只补说明，未修改学员代码或运行，Q4仍待复习，任务与DoD保持。


### TODO 4怎样结束等待、释放连接并显示原因

2026-10-19用户询问TODO 4如何完成，当前源码已有EndWaiting和Close的try框架，但catch仍Console.WriteLine后throw，finally为空；这里给针对该方法的参考写法并解释，不直接替换学员源码。
RecoverConnection是教练命名的普通方法，当前目标是把失败的连接收拾好，允许用户之后点击连接；它没有调用Open，不会自动重新连接，因此原“开始重新连接”的提示不符合实际行为。
reason是调用者传入的说明，例如“发送失败：……”或“已主动断开连接。”，显示时要保留它，关闭端口又报错时再追加关闭错误。

```csharp
private void RecoverConnection(string reason)
{
    string result = reason;
    EndWaiting(reason);

    try
    {
        _connection.Close();
    }
    catch (Exception exception)
    {
        result = reason + "；关闭串口时发生错误：" + exception.Message;
    }
    finally
    {
        RefreshConnectionControls();
        OperationText.Text = result;
    }
}
```

EndWaiting(reason)先结束这次等待并显示传入原因，后面的Close负责让旧端口退出当前连接、清掉旧缓冲并释放资源。
result是方法里新建的普通字符串变量，先保存原原因；Close成功时保持不变，Close失败时追加异常的Message，避免用关闭错误覆盖最初的发送或接收错误。
catch接住这里的关闭异常后把原因留在界面即可，原来的throw会继续向外抛异常，不符合这条“显示错误后允许手动重连”的处理路径；WPF程序也不应只靠Console.WriteLine给用户看错误。
finally在Close正常结束或catch处理关闭异常后都会执行，RefreshConnectionControls读取关闭后的实际连接状态以刷新按钮，OperationText.Text再显示最终原因。
EndWaiting中的刷新发生在Close之前，finally中的刷新发生在Close之后，因此这里最后一次刷新用于把按钮改为允许重新连接。
本轮只提供针对TODO 4的示例说明，学员源文件未改，未构建/运行/测试，TASK-046继续🟡 进行中，DoD不变，Q4仍待复习，不将提供的参考代码算学员自主掌握。
