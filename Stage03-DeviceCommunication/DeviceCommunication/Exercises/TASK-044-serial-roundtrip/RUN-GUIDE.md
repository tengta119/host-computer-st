# TASK-044：运行与动手说明

这次完成同一个程序的完整收发流程，五个TODO分别对应打开、发送、接收、关闭重开和读数输出；菜单、端口参数、模拟设备与已通关的缓冲算法由教练准备，不要求重新编写。

## 先准备虚拟串口对

你已确认没有实体设备，采用本机虚拟串口对；2026-10-19实际检查只列出蓝牙COM5、COM6、COM7，常用安装位置和卸载登记未发现已安装的虚拟串口工具，尚未创建本题端口对。
可从[HHD Software官方Free Virtual Serial Ports页面](https://freevirtualserialports.com/)下载并安装，选择本机互相连接的一对端口，本题只需免费版的本机配对功能，不需要远程或永久设备等付费功能；安装步骤中的系统确认由你在本机完成，当前没有替你安装软件。
2026-10-19用户截图已显示HHD Virtual Serial Port Tools主界面，当前第一步点击左上角Local Bridges下面的Create Local Bridge，这个入口用于准备本机互连的两个端口；尚未看到下一页，不猜测其中的选项或声称端口已创建。
随后用户截图已显示Create Local Bridge配置页，Local Bridges已选中，First port name为COM1、Second port name为COM2；本次保留这两个名字和其他默认设置，点击Create，拟让上位机用COM1、模拟设备用COM2，待创建成功后再核对系统端口，不把当前配置页当作已创建成功的证据。
在软件中创建互连的端口对并记下两端的实际名字，然后运行下面的列举命令核对Windows是否能看到它们；如果软件自动分配了别的名字，就使用实际名字。
这不是分别输入两个COM名字就算配好，两端必须被工具连接为同一对；COM10、COM11在下文仅作为例子，目前本机没有观察到这两个口。

从工作区根目录D:\host-computer-st运行：

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --ports
```

需要记下的是新建的那一对端口名，不是检查时已经存在的蓝牙COM口；端口准备好后，先完成下面的TODO 1，再启动两端联调。

## 第一处改动：真的打开选中的端口

打开同目录的Task044Exercise.cs，找到OpenPort方法的TODO 1，替换该方法里的TODO占位输出，先添加这两行：

```csharp
_port.Open();
Console.WriteLine("已打开：" + _port.PortName + "，状态：" + _port.IsOpen);
```

SerialPort是.NET提供的串口类，教练在Task044Exercise中写的`private SerialPort _port = new SerialPort();`创建一个负责串口操作的C#对象，变量名_port由教练自行命名，不是Windows端口名。
Run方法已经把你选择的实际COM名字赋给这个对象的PortName属性，但这只是在选择要使用哪个端口；调用_port.Open()才真正尝试打开该端口，不能凭空创建一个系统串口。[Microsoft：SerialPort.Open](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.open)
IsOpen是.NET提供的状态属性，成功后应为True，菜单输入o会调用你修改的OpenPort方法；如果端口不存在或被占用，Open会抛异常，已有菜单catch会显示原因并允许继续尝试。
本题双方都用115200、8、无校验、1停止位，骨架已经设置；115200是双方约定的传输速率，8表示每个数据单元使用8个数据位，无校验表示不额外加入串口层校验位，1停止位用于标记数据单元结束，这些配置必须与另一端一致，和教学帧末尾那个字节不是同一件事。
本题不启用额外的流量控制，读超时500毫秒、写超时1000毫秒是教练准备的操作上限，不是判断一帧是否完整的规则。

## 开两个终端，让两个程序各用一端

先构建一次，之后两端都加--no-build，避免同时运行时争用构建输出：

```powershell
dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj
```

假设实际配对端口是COM10、COM11，在第一个终端运行模拟设备：

```powershell
dotnet run --no-build --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --simulator COM11
```

在第二个终端运行你的上位机，输入o：

```powershell
dotnet run --no-build --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --port COM10
```

模拟设备应显示它打开了另一端，你的程序完成TODO 1后应显示选择端口及True；两个程序使用不同端，窗口都保留运行，不能把同一端同时分给两个程序。
命令中第一个`--`后面的内容交给Program.cs的args，教练已把它传给exercise.Run(args)；--simulator选择已写好的对端工具，--port选择学员的收发程序，--ports只列出现有端口。

## 后续四处连接在同一个收发流程里完成

### 发送请求：TODO 2

SendRequest里的request数组已经准备为55 01，它是本练习自行约定的两个请求字节，模拟设备收到后可显示，不代表真实设备的通用命令。
SerialPort.Write(byte[]数组, 起始位置, 数量)把数组指定部分发送出去；本题从位置0发送request.Length个原始字节，模拟设备输入r查看收到的内容，不发送文字"55 01"。[Microsoft：SerialPort.Write](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.write)
PrintBytes是教练准备的显示方法，调用PrintBytes("已发送：", request, request.Length)只把同一数组按十六进制打印，真正发送仍由前面的Write完成，不把控制台显示当作发送证据。
模拟设备是手动控制的工具，不会因为收到请求就自动发回温度；看到55 01后，在模拟端输入1发送完整读数帧，再去上位机输入r读取。

### 接收并复用缓冲：TODO 3

ReceiveAvailable先确认端口已打开且BytesToRead不为0，BytesToRead是.NET提供的当前待收字节数量；没有新数据时保持已有半帧并返回。
data是教练创建的256位置接收数组，Read(data, 0, data.Length)从串口读字节并写进这个数组，返回int表示本次真正读入多少字节；把返回值保存到你声明的普通int变量，例如bytesRead，再传给HandleReceivedBytes(data, bytesRead)，而不是传data.Length。[Microsoft：SerialPort.Read](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.read)
HandleReceivedBytes是教练新写的连接方法，负责显示本次字节、按有效数量追加并调用已验证的缓冲逻辑；它不是.NET接口，也不是重新实现TASK-043的要求。
本题输入r才执行一次读取，不使用后台接收回调，因此可以先观察“串口收到什么”和“缓冲留下什么”；模拟端发送不决定Read一定返回几次，数组未装下的内容可再次输入r继续读，只有帧规则决定是否已收齐。

### 关闭后重开：TODO 4

ClosePort中的_port.Close()负责主动关闭该串口，_receivedBytes.Clear()清掉这次连接留下的半帧；这个列表属于你的程序，不会因为操作系统端口关闭就自动清空。[Microsoft：SerialPort.Close](https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.close)
输入c后再输入o应该能重新打开同一个端口；程序退出时finally中的Dispose是教练提供的最后清理，不代替你实现c时的关闭与半帧清理。
已取帧计数可以累计，关闭后不混入旧半帧、不会凭空多出读数才是本次重点。

### 输出读数：TODO 5

PrintReading接收的frame已经是一帧完整6字节，来源是已接好的ExtractOneFrame方法，frame[1]按本题协议表示设备编号。
frame[3]乘256再加frame[4]得到原始整数，再除以10.0得到温度，例如01 04对应260和26.0℃，01 00对应256和25.6℃，00 00对应0和0.0℃。
这套字段含义是本题新约定，不是从帧里猜出来的；frame[2]固定03，不是长度，frame[5]暂不解释，本题没有校验算法。
显示温度时复用已经用过的ToString("F1")，把double数值显示为一位小数，不使用整数除法丢掉25.6的小数部分。

## 用模拟端现场验证

先补完串口调用和读数计算，再按下面顺序操作，组内不要为了凑结果主动清空缓冲。

| 要观察什么 | 模拟端操作 | 上位机操作与预期 |
| :--- | :--- | :--- |
| 发送原始请求。 | 输入r。 | 先输入s发送55 01，对端显示同样两个字节。 |
| 一个完整读数。 | 输入1。 | 输入r，设备1、原始250、25.0℃，剩0。 |
| 分段到达。 | 先2，稍后3。 | 在2与3之间先输入r，应留AA 01两项；3之后再r，输出一条25.0℃，剩0。 |
| 连续两帧。 | 输入4。 | 输入r，依次输出25.0℃、26.0℃，剩0，之后再r无新增。 |
| 四组数值。 | 输入5。 | 收完后依次为25.0℃、26.0℃、25.6℃、0.0℃。 |
| 无效前导与半帧。 | 先6，再3。 | 先r清99 88只留AA 01，补齐后再r只出一帧。 |
| 取帧后继续清理。 | 先7，再8。 | 先r取A并清99只留AA，补齐后再r取B。 |
| 旧半帧不跨连接。 | 先2，重开后1。 | 先r留半帧，c关闭、o重开，再收完整A应只得一条完整读数。 |
| 打开失败能重试。 | 上位机先c关闭，再启动一个临时模拟端占住上位机要用的那一端，之后q退出释放。 | 自己的程序o应看到失败；临时模拟端释放后再次o可打开，正常模拟端继续使用另一端。 |

如果分次发送在一次读取前已经合并到达，先后总结果仍应正确；要观察明确半帧，必须按表在模拟端2和3之间读取一次，不能仅凭对端分两次Write就声称实际Read也分了两次。
当前只完成骨架与教练辅助验证，端口对尚未创建，TODO仍未完成，不把本表中的预期当作实际串口运行记录。
