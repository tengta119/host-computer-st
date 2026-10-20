# 串口通信

> [返回问答索引](./README.md)

2026-10-19新增Q4讲解TASK-046的等待与超时，Q1至Q4均待复习；TASK-046骨架已准备，四个TODO尚未完成，不新增自主掌握。

## Q1：串口是什么，需要在电脑上创建一个吗？

- 日期：2026-10-19。
- 状态：待复习。
- 来源：用户在TASK-044开启前询问串口与本机创建端口的关系，本次问题未要求不记录。
- 关联：[TASK-044](../tasks/TASK-044.md)。

串口是电脑与设备收发数据的一种接口，例如电脑发出读取指令，温度设备把测量数据发回来；Windows中程序通常用COM3这样的名字选择具体串口，COM3只是举例，不代表本机已经存在这个端口。
程序打开选定串口后，可以发送字节并读取另一端发来的字节，前面练习的AA 01等就是收到后要处理的内容；“串行”说的是数据的位依次传输，先理解具体收发关系，不要求在这里展开电气细节。

### 接真实设备时，通常使用系统已经识别出来的端口

电脑自带串口，或连接具有串口功能的USB设备/USB转串口适配器后，在相应驱动识别正常时，Windows通常会分配COM号；程序选用这个现有端口，不需要自己用代码造一个端口。
普通USB插孔本身不等于一个COM串口，需要连接支持相应功能的设备或适配器；实际接口和接线应依据设备资料确认。[Microsoft：串口驱动概览](https://learn.microsoft.com/en-us/windows-hardware/drivers/serports/)

### 没有设备时，可以用一对虚拟串口让两个程序互相收发

虚拟串口由软件和驱动提供给Windows上的程序使用，不需要真实串口接线；练习时可以创建互相连接的一对端口，而不是只给两个无关联的端口随便起名字。[com0com项目：虚拟串口对](https://com0com.sourceforge.net/)

```text
你的上位机程序 ←→ COM10 ←→ COM11 ←→ 模拟设备的程序
```

上图COM10、COM11只是举例，你的程序打开一端，另一个程序打开另一端；写入COM10的数据能从COM11读到，反方向同样可以收发，另一个程序负责模拟设备返回数据。
这样可以练习Windows串口接口的打开、发送、接收、关闭以及已有缓冲解析，真实设备的电气连接和设备行为仍需之后接设备验证。
在C#程序里填写一个COM名字只是选择要打开的端口，不会创建系统端口；没有设备且选择虚拟对端时，才需要用相应工具先准备虚拟串口对。

### 当前安排与证据

用户于2026-10-19明确开启TASK-044并确认无实体设备、选择本机虚拟串口对；实际列出蓝牙COM5、COM6、COM7，没有观察到本题需要的互连端口对，常用安装位置/卸载登记也未发现相应工具，不据此声称全盘不存在。
当前已准备源码和模拟设备，但没有安装虚拟串口软件或创建端口对，没有实际串口往返证据；TASK-044🟡 进行中，Q1仍待复习，先按运行说明准备端口对，既有通关与能力保持，前面的COM10/COM11仍为示例。

## Q2：TASK-044的串口对象与五个TODO怎样连成收发流程？

- 日期：2026-10-19。
- 状态：待复习。
- 来源：TASK-044开启前核对学员只知道串口/虚拟端口概念，尚无串口API实践证据，必要基础随本题讲解。
- 对应：[上位机源码](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Task044Exercise.cs)、[完整运行与讲解](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/RUN-GUIDE.md)。

SerialPort是.NET提供的串口类，_port是教练创建并命名的具体操作对象；PortName选择已存在的系统端口，Open才打开，IsOpen反映是否已打开，new SerialPort()不创建虚拟COM端口。
菜单输入o/s/r/c分别调用OpenPort/SendRequest/ReceiveAvailable/ClosePort，这些方法名是教练命名的本题普通方法，里面的Open/Write/Read/Close才由.NET提供。
发送使用已准备的55 01数组，Write传数组、起始位置0、数量；读取把字节放入256位置数组，Read返回的实际数量才传给自定义HandleReceivedBytes，后者已经接好旧缓冲算法，不重新编写TASK-043。
模拟设备由教练写好，打开互连的另一端，手动r查看请求、1至8发样例；它不会自动回复，上位机也由手动r触发一次读取，本题尚未引入后台事件或WPF线程。
Close关闭系统端口，列表Clear清掉自己保存的半帧，两者各管一处，不能以为关闭会自动清空自己的列表；退出时的finally/Dispose由教练提供，不冒充学员实现主动关闭。
完整帧到达PrintReading后，按本题明确规则读取设备编号、两字节整数和温度；这些规则与第一处Open动作、每个API参数以及两个终端运行方式已放在RUN-GUIDE中，按实际使用处阅读，不要求先背全部名词。
五个TODO是同一个完整串口功能内的操作位置，不是五个独立任务；本轮构建和辅助检查通过只说明骨架可用，串口调用和换算尚未自主完成，Q2待复习，不新增掌握或错题。

### 2026-10-19仅静态源码审查的增量记录

学员已写入Open、Write、Read返回值传入HandleReceivedBytes以及Close后Clear，本轮按用户要求仅阅读源码，不构建、运行或测试，不能把这些代码存在直接标为串口实践已验证，Q2仍待复习。
PrintReading中int value = (frame[3] * 256 + frame[4]) / 10先执行整数除法，例如原始256会变成25，F1只补显示形式，不能恢复25.6；本次是按C#语义推导，未实际运行输入。
原始整数与温度分别保存在int和double变量中，温度用原始值除以10.0，再分别输出编号、原始值、温度，可以避免把原始值覆盖成换算后的整数；核心修复留给学员，其他已验证能力保留。

### 2026-10-19按用户静态口径通关

用户进一步明确“源码没问题就通过”，学员已自行分开int rawValue与double temperature，并使用10.0进行浮点除法；收发调用和缓冲清理源码未发现任务范围内的阻塞问题，TASK-044按调整后的静态DoD标为✅ 已通关。
教练只补输出中的原始值、温度单位/标识以及关闭文案错字，没有代写学员计算，修改后仅重读源码，未构建、运行或测试，详见[最终静态记录](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Verification/static-review-final-2026-10-19.md)。
Q2继续待复习，记录学员已完成源码连接与浮点换算修正，但不把本次静态任务通关扩展成真实串口往返已自主验证；Q1和既有字节实践能力保持。

## Q3：TASK-045收到串口数据后怎样交给WPF窗口更新？

- 日期：2026-10-19。
- 状态：待复习。
- 来源：TASK-045开启必要讲解，旧WPF模拟采集与事件实践保留，不推定已经掌握串口辅助线程与窗口交接。
- 对应：[窗口后台](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Task045Window.xaml.cs)、[具体讲解与运行](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/RUN-GUIDE.md)。

窗口的_connection是教练自定义Task045SerialConnection对象，Open方法内部打开.NET串口并登记DataReceived，后台读取有效字节、进已有缓冲、取完整帧后产生Task045Reading读数，再发出自定义ReadingReceived通知。
窗口SerialReadingReceived里的e是Task045ReadingEventArgs，e.Reading保存此次完整读数；虽然这个方法写在窗口后台文件里，它仍跟着串口通知在辅助线程执行，不能直接修改LatestReadingText或向正在被DataGrid使用的_history追加。[Microsoft：DataReceived](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.datareceived)
Dispatcher是WPF用于安排该窗口界面操作的对象，TODO 4使用BeginInvoke把调用ApplyReadingOnUi(e)的动作交给窗口线程，动作由.NET的Action委托保存，不是在接收线程当场执行；具体最小代码和逐处解释已写在运行说明，先解释再要求使用。[Microsoft：WPF线程模型](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/threading-model)
ApplyReadingOnUi是教练命名的普通方法，在窗口线程执行后，TODO 5才将e.Reading追加到已有ObservableCollection，再调用教练的RefreshPresentation更新最新值、条数、曲线和报警；TODO 4决定在哪里执行，TODO 5连接读数与集合，两个方法不是同一件事的重复拆分。
数据类、事件登记、锁/旧连接通知保护、关闭安全收尾和绘图辅助由教练准备，不要求重写已验证缓冲或先口试线程原理；五个TODO尚未由学员实现，教练的13项准备检查和注入数据显示不计串口往返或学员已掌握，Q3待复习，旧能力保持。

2026-10-19用户追问两条EventHandler声明的用途，已在[委托基础Q6](./csharp-events.md#q6委托是什么为什么-handler-变量可以像方法一样调用)补当前真实对象与方法：ReadingReceived报告完整读数，ReceiveFailed报告需要显示的接收异常，窗口通过+=登记SerialReadingReceived与SerialReceiveFailed，事件本身不自动切换线程；委托类型由.NET提供，两个参数类和事件名由教练定义，未改代码或改变掌握/任务状态。

### 一行BeginInvoke怎样拆开看

2026-10-19用户追问Dispatcher.BeginInvoke(new Action(() => ApplyReadingOnUi(e)))，当前源码已见学员写入这一行，只有源码使用证据，尚未据此认定已经掌握界面线程交接，Q3仍待复习。
先看具体效果：串口刚收到一条设备1、25.0℃的读数，当前e带着这条读数，这一行把“拿e调用ApplyReadingOnUi”的动作交给窗口线程去执行，接收代码安排后即可继续，不需要等窗口更新完。
同一行可以分开写成：

```csharp
Action updateWindow = new Action(() =>
{
    ApplyReadingOnUi(e);
});
Dispatcher.BeginInvoke(updateWindow);
```

ApplyReadingOnUi是教练命名的普通方法，调用时把当前e传给它，方法内部才有历史追加和显示连接；这里不是某个WPF内置更新接口。
`() => { ApplyReadingOnUi(e); }`是一个待执行的小动作，这时先描述要做什么，不马上调用方法；外面空括号表示这个动作不需要再接收新参数，里面仍可以使用本次接收方法已有的e。
Action是.NET提供的无参数/void委托类型，new Action把这个小动作保存为可交给别处执行的委托对象，updateWindow是说明示例中新起的变量名，并非新增API。
Dispatcher是当前窗口的WPF调度对象，BeginInvoke把updateWindow放入这个窗口线程待处理的队列，之后由窗口线程执行；因此真正运行ApplyReadingOnUi时才回到了可以操作控件和绑定历史集合的线程。
直接写ApplyReadingOnUi(e)会在当前接收线程执行，同样的方法名不会自己带来线程切换；本轮仅解释这一关系，没有改动学员代码、运行或改变任务DoD，不追加新的考试或错题。

### 2026-10-19 TASK-045最终静态审查

学员已完成按钮连接/断开/发送、BeginInvoke安排ApplyReadingOnUi和_history.Add(e.Reading)后的显示刷新，保持旧连接/关闭判断，TASK-045按最近源码偏好通关；教练仅删事件登记行末多余x并清理旧界面提示，没有代写核心连接，详见[静态依据](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Verification/static-review-final-2026-10-19.md)。
本轮未构建、运行或测试，记录已完成源码应用，不将静态通关推定为实际串口往返或委托/线程完整原理已验证，Q3与委托Q6仍待复习，既有应用证据保持，不因一个多余字符追加认知错题。


## Q4：发送成功后怎样等待回复，ReadTimeout为什么不是这次请求的超时？

- 日期：2026-10-19。
- 状态：待复习。
- 来源：TASK-046开启时先补的必要说明，不是学员认知错误。
- 关联：[TASK-046](../tasks/TASK-046.md)与[实际运行说明](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/RUN-GUIDE.md)。

窗口调用_connection.SendRequest写出55 01成功，设备仍可能不回复，所以窗口还要单独记住“现在正在等”，并在完整读数到达或等待超时后恢复发送按钮。
串口辅助中的ReadTimeout = 200限制某一次Read等字节的时间，若对端完全不发数据，可能连DataReceived和Read都没有执行，因此它不能负责判断点击发送后3秒仍没有完整回复。
本题新增的_requestWatch是.NET的Stopwatch秒表对象，BeginWaiting里Restart从零计时，之后用Elapsed读取经过时长；EndWaiting里Reset停止并归零，不需要Thread.Sleep阻塞窗口。
_timeoutTimer是WPF的DispatcherTimer对象，Start之后每隔约200毫秒安排窗口执行CheckResponseTimeout，Stop结束检查；检查读取秒表是否已到3秒，不生成温度，不把半帧变成完整帧，窗口忙时也不保证恰好每200毫秒执行。
收到完整读数、超时、主动断开和错误处理共用EndWaiting，后者是教练命名的普通方法，学员在TODO 2里接好停止检查、清标记和刷新显示，避免多个出口遗漏一个。
接收错误时使用BelongsToCurrentConnection核对编号即可，不能同时要求IsOpen为true，否则已经失效的当前端口恰好会被当作无须处理；这些编号保护由教练提供，旧连接的排队读数或错误不能影响重连后的窗口。
没有请求编号就无法严格匹配迟到回复，当前仅约定同一连接一次等待、下一条发送后完整读数算响应；虚拟对端退出不一定令本端IsOpen变false，此时通常靠无回复超时发现问题。
具体对象、最小秒表示例、四个TODO和运行步骤先读实际运行说明，当前仅有骨架启动/关闭证据，未验证学员超时或重连，不扩大既有掌握范围。


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

本次说明并入串口Q4，仍待复习，TASK-046继续🟡 进行中，未构建、运行、测试或变更DoD。


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


### 2026-10-20 TASK-046静态审查

当前BeginWaiting尚未把_waitingForResponse设为true，源码中该字段从初始false开始只会再被赋false，因此定时检查总会直接返回，重复发送也不会被挡住；开始方法还要刷新按钮及等待提示。
CheckResponseTimeout当前在BelongsToCurrentConnection返回true时结束等待，方向相反，应该在等待不属于当前连接时才清理旧等待；这会在入口修好后立即暴露为第一次检查就结束正常等待，不能只修前一处。
EndWaiting、RecoverConnection和关闭保护源码路径保留，DoD1未通过、2/3静态通过，TASK-046仍🟡 进行中；教练仅修一处提示，不代改核心，未构建/运行/测试，详见本题Verification/static-review-2026-10-20.md，Q4仍待复习，不新增掌握。


### 2026-10-20 TASK-046最终静态复查通过

学员已补BeginWaiting的true等待标记，并将旧连接检查改为!BelongsToCurrentConnection，原等待入口与条件方向问题已修复；教练只补等待提示、调用已有按钮刷新和清理旧占位文案。
三个DoD按静态源码口径通过，状态✅ 已通关，本轮未构建/运行/测试，不声称真实超时、重连或并发关闭已验证；RecoverConnection参考示例的使用与显示协助不计独立设计能力，Q4继续待复习，27分类/74条、待复习38/已掌握36保持。
下一张TASK-047仅建未开始卡，准备补字段/校验与坏帧恢复，第三阶段仍进行中，当前入口仍TASK-046，之前失败与待修说明均保留为历史。
