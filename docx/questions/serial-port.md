# 串口通信

> [返回问答索引](./README.md)

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
