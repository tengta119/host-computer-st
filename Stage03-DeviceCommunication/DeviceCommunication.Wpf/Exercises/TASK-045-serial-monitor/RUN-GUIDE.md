# TASK-045：串口读数进入WPF窗口

本题只做一个完整串口监控窗口：连接、断开、发送、自动接收，再把有效读数交给表格、曲线和报警；五个TODO是这个流程里的五处连接，不是五张任务卡。

## 已经准备了什么

Task045Window.xaml是教练准备的完整窗口布局，Task045Window.xaml.cs是对应后台，按钮和处理方法已经连接；旧MainWindow、TASK-044与WpfBae成果保留。
Task045SerialConnection是教练新建的串口辅助类，内部沿用TASK-044的115200/8/无校验/1停止位、55 01请求和两个字节除以10.0的规则，并复用TASK-043的非AA清理、半帧等待和连续取帧顺序。
Task045Reading是教练新建的数据类，保存一次完整帧对应的设备编号、原始值、温度和本机接收时间，ReceivedAt不是设备自带时间；Task045ReadingEventArgs是本次通知携带的对象，Reading属性指向这条读数。
这些自定义名字不是.NET或WPF原生API，实例由骨架创建：窗口的_connection指向一个Task045SerialConnection对象，_history指向一个ObservableCollection<Task045Reading>历史集合，HistoryGrid.ItemsSource已经指向同一集合。
绘图、报警、阈值输入、历史表格列和基础关闭保护由教练准备，沿用此前学过的WPF显示方式，不让你重新实现一套旧功能；窗口没有用于生成假读数的定时器。

## 第一处改动：连接按钮真的连接串口

打开Task045Window.xaml.cs，找到ConnectClick的TODO 1，保留现有端口名检查和try/catch，将占位提示替换为：

```csharp
_connection.Open(portName);
RefreshConnectionControls();
```

portName来自窗口里端口选择框的Text，_connection.Open是教练辅助类的方法，它内部才调用.NET的SerialPort.Open并登记串口接收通知；不能在这里再创建第二个SerialPort对象。
RefreshConnectionControls也是教练写的普通方法，它读取实际IsConnected状态，成功后显示“已连接：端口名”，禁用重复连接与端口切换，启用发送/断开，失败时仍保持未连接，不要求你手动给控件编一套真假连接状态。
完成这一处后运行窗口，选择虚拟串口对中给上位机使用的一端，再点连接；若你实际创建的是COM1/COM2，窗口选COM1，模拟设备用COM2，这两个名字仍需以你实际创建的结果为准。

在工作区根目录D:\host-computer-st运行新WPF项目：

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\DeviceCommunication.Wpf.csproj
```

另一个终端继续运行已有模拟器，不需要重写：

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --simulator COM2
```

模拟器与窗口各用互连的不同一端，原控制台上位机若仍占着同一个端口，先退出或主动关闭它。
刚准备的骨架只显示真实TODO状态，未补TODO 1时按钮不会真正连接；初次窗口不预填或猜测你的端口，刷新列表后选择实际的一端。

## 断开与发送：连接已有操作

DisconnectClick的TODO 2调用_connection.Close，再调用RefreshConnectionControls；服务内部清空旧半帧并取消本次接收登记，已经显示的历史可以保留，不清空窗口历史来假装关闭成功。
SendClick的TODO 3调用_connection.SendRequest，成功后再写发送提示；它内部已经按原始byte数组发送55 01，模拟器输入r观察收到的请求，再由你在模拟器输入1至8发送数据，模拟器并不自动回复。

## 这次新关系：收到数据的代码不能直接改窗口

TASK-044中你输入r才调用Read，TASK-045改为串口有数据时通知Task045SerialConnection的PortDataReceived方法，不用界面按钮一直等Read返回。
DataReceived是.NET的串口事件，收到数据时它的处理方法通常在辅助线程执行，不是在负责响应窗口按钮的界面线程执行；一次通知也不等于一帧，因此这里仍先读取有效字节、进缓冲、取完整帧。[Microsoft：SerialPort.DataReceived](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.datareceived)
完成组包后，辅助类通过自己定义的ReadingReceived事件通知窗口的SerialReadingReceived方法，e.Reading就是本次完整读数；虽然这个处理方法写在窗口类里，它仍跟着接收通知在辅助线程执行，不能因为文件属于窗口就认为已经回到界面线程。
WPF的TextBlock和DataGrid等窗口对象应由创建它们的界面线程操作，_history又正在被DataGrid使用，所以这个接收方法不直接修改文本或向_history.Add。[Microsoft：WPF线程模型](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/threading-model)

### TODO 4负责把这条读数交给窗口线程

Dispatcher是这个窗口用于安排界面操作的WPF对象，BeginInvoke把一个动作放到窗口线程待处理的队列里，接收代码安排后就能返回，不在这里等待窗口执行完。
Action是.NET提供的委托类型，表示一个无参数、无返回值的动作；下面的new Action把“稍后调用ApplyReadingOnUi(e)”保存成动作，`() =>`沿用已经用过的Lambda写法，这一行不是直接在接收线程调用ApplyReadingOnUi。
在SerialReadingReceived的TODO 4处，用下面的关系把当前这次e交回窗口，不改上方关闭保护：

```csharp
Dispatcher.BeginInvoke(new Action(() =>
{
    ApplyReadingOnUi(e);
}));
```

ApplyReadingOnUi是教练准备的普通方法名，不是WPF接口；BeginInvoke安排这个动作之后，WPF会在窗口线程上执行它，所以随后才可以接着更新集合与显示。
若一行写法不易读，可以把它看成先声明Action updateWindow保存“用本次e调用ApplyReadingOnUi”的动作，再调用Dispatcher.BeginInvoke(updateWindow)；动作外面的空括号表示不再接收新参数，里面的e来自这一次接收通知，创建动作时不立即执行ApplyReadingOnUi。
错误通知的界面交接、关闭标记、旧连接通知的保护和串口内部锁由教练准备，本题不要求重新设计这些机制，也不因此推定你已经掌握全部并发原理。

### TODO 5负责把窗口线程上的读数放进历史

在ApplyReadingOnUi中保留上方的关闭/旧连接保护，把e.Reading追加到现有_history，再调用RefreshPresentation；这一次Add后表格会观察到集合变化，RefreshPresentation负责最新读数、历史条数、曲线和报警的重算。
TODO 4安排“在哪执行”，TODO 5连接“这次读数放进哪个集合”，它们在两个不同的方法里做不同的事，不要在后台直接Add，也不要只刷新空集合却没有追加读数。
e.SessionId是教练准备的连接编号，断开或重开就会变化，IsCurrentSession用于忽略已排队但属于旧连接的通知；保留这个判断，避免关闭后又显示旧读数，不要求另写一份判断。

## 现场观察整个流程

| 动作 | 完成相关TODO后的预期 |
| :--- | :--- |
| 窗口连接选定端口。 | 显示已连接，发送/断开可用，重复连接不可用。 |
| 点击发送，再在模拟端输入r。 | 模拟端收到55 01。 |
| 模拟端输入1。 | 窗口自动显示设备1、25.0℃，历史增加1，无需输入控制台r。 |
| 模拟端先2，再3，中间留时间观察。 | 前2字节不新增历史，补齐后增加1条完整读数。 |
| 模拟端输入4或5。 | 连续帧按到达顺序追加，25.6的小数和0.0都保留。 |
| 模拟端输入6再3，或7再8。 | 非AA清理与合法半帧保留沿用TASK-043，不生成伪读数。 |
| 曲线设备选择1或2。 | 曲线显示该设备最新最多30条，历史表仍显示所有设备，最新读数和报警对应最后到达的那条。 |
| 保持默认报警阈值25.5，模拟端发送25.0再26.0。 | 最新读数由正常变为超限，应用新阈值后重新计算显示。 |
| 接收时拖动窗口或操作阈值。 | 窗口仍响应，接收方法没有把等待循环放在按钮点击里。 |
| 有半帧时断开再连，或者关闭窗口。 | 半帧随连接清理，旧通知不会追加，关闭时取消登记并释放端口。 |

后续审查沿用你最近偏好的源码口径时，会明确记录为静态通过，不把源码阅读写成实际通信或界面联调；本轮教练只检查自己准备的骨架可构建、窗口可显示以及显示/缓冲辅助可用，五个TODO仍留给你。

## 本轮准备检查的边界

构建0警告/0错误，教练在临时STA宿主中加载同一Task045Window并实际Show，在默认/最小尺寸检查布局与辅助功能；诊断程序注入的三条读数只用于检查教练的表格、曲线、报警，不会写入正式程序，也不算串口收发或学员已完成Add。
初次诊断宿主曾因重复设置Application.ResourceAssembly退出，教练已修正宿主并重新验证，未把该问题交给学员；第一次失败记录保留，最终13项准备检查通过，16个旧文件哈希不变。
最终准备结果见Verification/startup-02-final-2026-10-19.json，图片保存在同一目录；没有通过本次准备声称虚拟串口对已经可用、真实收发已验证或任务DoD通过。
