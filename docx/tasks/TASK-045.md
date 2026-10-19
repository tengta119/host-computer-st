# TASK-045：把串口读数接入WPF监控界面

- 阶段：第三阶段：设备通信（WPF串口联动）。
- 状态：✅ 已通关
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
- 通关日期：2026-10-19。
- 最终验收口径：沿用用户最近“源码没问题就通过”的要求，仅静态源码审查，不构建/运行/测试，不声称实际串口或界面联调已经验证。
- 计划来源：[计划](../plan.md)第三阶段中有效读数进入新WPF界面、历史和监控显示，接收时界面保持可操作。
- 选择依据：TASK-044已按用户要求完成源码口径通关，收发/缓冲/读数代码可作为下一步接入基础，实际串口往返不冒充已验证证据。
- 项目：使用既有[DeviceCommunication.Wpf.csproj](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/DeviceCommunication.Wpf.csproj)，复用TASK-044的串口参数、协议和模拟设备，不重写已完成的缓冲算法。
- 难度/预计时间：待补充，不预设时长。

## 已开启与实际入口

用户于2026-10-19明确“开启45”，已在Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/准备独立窗口、读数对象、通信辅助和运行说明；学员现已完成五处核心连接，最终静态审查通过，旧作品保留。
共享App.xaml是必要入口例外，StartupUri已切到Exercises/TASK-045-serial-monitor/Task045Window.xaml；WPF项目文件仅加入System.IO.Ports 10.0.0，控制台Program.cs仍是TASK-044模拟器/上位机入口，旧MainWindow和WpfBae未改。

## 做出一个完整的串口监控窗口

窗口能够选择端口、连接/断开、发送请求，收到有效读数后显示设备编号和温度，追加历史并更新曲线/报警，连接状态清楚，接收时仍能操作窗口。
这是一个完整功能任务，字段换算和缓冲直接复用TASK-044，已有WPF表格/绘图/报警知识在真实读数进入时使用，不再把绑定、按钮或单个API各建一张任务卡。
教练负责准备必要窗口入口、可复用绘图/显示辅助和通信基础骨架，学员完成界面操作到通信的连接以及有效读数到显示/历史的连接；开启时再按真实源码写明各TODO位置，不预先堆叠细碎编号。

## 先解释新的接收与界面关系

控制台中的r是手动读取，窗口需要在接收数据时仍响应按钮，不能把一个长期等待接收的循环直接放在按钮点击里。
开启时先结合实际准备的接收代码，说明哪段代码负责收字节、哪段负责窗口操作，再介绍WPF的Dispatcher怎样安排窗口更新，不能直接要求使用尚未解释的后台事件或线程调用。
只有已按约定组好的完整有效帧才生成读数并追加历史，收到AA 01之类半帧时只保留缓冲，不在界面增加伪读数；本题仍沿用TASK-044明确的教学协议，帧尾不是已实现的校验。

## 实践顺序

1. 在一个窗口中连接端口选择、连接/断开、发送和状态反馈，复用已完成的串口基本操作。
2. 让教练提供的模拟端发数据，经同一缓冲/解析流程更新编号和温度，说明并连接接收代码与窗口更新之间的交接。
3. 将有效读数追加到历史并更新已有曲线与报警显示，核对整帧、半帧和连续帧，不用定时器生成假数据代替串口输入。
4. 检查断开、重新连接和关闭窗口时的基础收尾，避免重复登记导致重复读数或向已关闭窗口继续更新；更完整的超时/设备断开恢复在后续完整任务中处理。

## 验收标准（DoD）

以下勾选表示沿用用户最近要求的静态源码验收通过，不表示本轮实际运行通过；原卡完整保存在[静态验收前记录](./history/TASK-045-before-static-acceptance-2026-10-19.txt)。

- [x] 源码中的端口选择、连接/断开和发送与同一个_connection相连，成功后刷新实际状态，未连接时发送禁用或有方法保护，没有额外复制假串口状态。
- [x] 源码经有效字节、半帧保留/连续取帧、字段换算得到读数，窗口把同一e.Reading追加到已绑定_history并刷新曲线/报警，没有生成假读数的定时器，已有小数与组包顺序保持。
- [x] 接收代码由DataReceived通知执行，通过Dispatcher.BeginInvoke将ApplyReadingOnUi交回窗口线程，旧连接/关闭保护、主动断开清理、再次打开和窗口退出取消订阅/释放路径完整，静态未发现任务范围内阻塞问题。

后续审查沿用用户最近的源码审查偏好时，明确记录为源码口径，不要求重复确认、不虚构界面或通信运行结果；本轮仅验证教练准备的骨架与显示辅助，不是对学员实现验收，阶段实际联调目标仍保留。

## 当前证据

学员五处连接已完成，三个静态DoD已勾选、TASK-045✅ 已通关；本轮未构建/运行/测试，没有新增串口往返证据，第三阶段仍🟡 进行中，教练准备时的窗口运行与更早任务证据保留其原范围。

## 2026-10-19最终静态源码审查

ConnectClick、DisconnectClick和SendClick均调用同一个_connection，状态更新位于操作成功之后；SerialReadingReceived使用Dispatcher.BeginInvoke(new Action(() => ApplyReadingOnUi(e)))交回窗口，ApplyReadingOnUi保留旧连接/关闭判断后向_history.Add(e.Reading)并刷新显示，五处核心连接正确。
静态检查发现通信辅助第64行事件登记后误多一个x，教练直接删除此字符，并清理XAML初始化提示和页脚两处过期TODO文案；未改学员五处核心逻辑，不把误输入或教练旧提示记为学员典型认知错误。
修改后仅重读源码，未执行构建、运行或测试；详细依据见[最终静态审查](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Verification/static-review-final-2026-10-19.md)，未发现本任务范围内其他阻塞源码问题，状态同步✅ 已通关。
记录学员完成源码连接，不因静态通关推定串口实测或委托/线程全部原理已掌握，串口Q3和委托Q6仍待复习；只创建[TASK-046：处理串口超时、断开与重新连接](./TASK-046.md)⚪ 未开始卡，不准备源码或切换入口，阶段未整体通关。

## 已准备的文件与学员连接

| 实际文件 | 用途与边界 |
| :--- | :--- |
| [Task045Window.xaml](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Task045Window.xaml) | 教练提供完整布局、端口控件、历史列、曲线区域和报警区域，首题不要求重排旧WPF布局。 |
| [Task045Window.xaml.cs](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Task045Window.xaml.cs) | 学员的五处核心连接集中于此，菜单/按钮登记、状态刷新、绘图/报警/阈值辅助和基础窗口关闭由教练提供。 |
| [Task045SerialConnection.cs](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Task045SerialConnection.cs) | 教练按TASK-044和已验证缓冲顺序准备的收发辅助，使用DataReceived通知自动读取，保护关闭/重连和旧通知，不作为学员独立编写并发代码的证据。 |
| [Task045Reading.cs](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Task045Reading.cs) | 教练新建读数对象及通知参数对象，声明编号、原始值、温度、本机接收时间和连接编号。 |
| [RUN-GUIDE.md](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/RUN-GUIDE.md) | 首个动作、两个项目运行命令、具体对象身份、接收线程与Dispatcher、五处TODO的关系及联调预期。 |
| [App.xaml](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/App.xaml) / [项目文件](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/DeviceCommunication.Wpf.csproj) | 共享文件必要例外，仅连接新窗口入口及串口依赖，未修改原MainWindow和旧控制台入口。 |

| TODO位置 | 对应实践/DoD | 完成后观察什么 |
| :--- | :--- | :--- |
| 1：ConnectClick把所选端口交给_connection.Open并刷新实际状态。 | 实践1 / DoD1。 | 真正连接后显示已连接，发送/断开启用，不能重复连接。 |
| 2：DisconnectClick调用Close并刷新控件。 | 实践1、4 / DoD1、3。 | 主动断开，保留历史，服务内部丢弃旧半帧。 |
| 3：SendClick调用SendRequest，成功后提示。 | 实践1 / DoD1。 | 对端能核对55 01，不创建第二个假通信对象。 |
| 4：SerialReadingReceived通过Dispatcher.BeginInvoke安排ApplyReadingOnUi(e)。 | 实践2 / DoD2、3。 | 接收线程不直接访问控件或历史，读数交回窗口线程处理。 |
| 5：ApplyReadingOnUi把e.Reading追加到_history并刷新显示。 | 实践3 / DoD2。 | 同一条完整读数进入表格、最新值、曲线与报警，不重复造数据。 |

TODO 4负责让动作在窗口线程执行，TODO 5负责这个动作拿读数更新哪个集合，位于两个不同方法，不是重复要求；Read、有效数量、缓冲和换算已经由辅助类承接，不再重做TASK-043/044。

## 第一个动作与运行

第一处只改Task045Window.xaml.cs中ConnectClick的TODO 1，保留端口检查与try/catch，把占位提示替换为_connection.Open(portName)以及RefreshConnectionControls()；前者是自定义通信辅助的方法，内部调用.NET串口打开，后者按实际连接状态更新控件。
从工作区根目录运行新WPF窗口：

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\DeviceCommunication.Wpf.csproj
```

模拟端继续用已有控制台，例如实际互连端口为COM1/COM2时，WPF选COM1，另一终端运行：

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --simulator COM2
```

端口名以实际创建结果为准，首次动作完成后应看到真实连接状态变化；默认窗口历史为0，未完成TODO 1时只显示TODO提示，不把可点击按钮当作已连接。
必要的新知识先看运行说明“收到数据的代码不能直接改窗口”一段：SerialReadingReceived仍跟随接收通知在辅助线程执行，窗口的Dispatcher才把ApplyReadingOnUi安排到界面线程，不用一个尚未解释的线程名词跳过具体调用关系。

## 2026-10-19教练骨架准备验证

- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\DeviceCommunication.Wpf.csproj --nologo`，退出0、0警告/0错误。
- 实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\Exercises\TASK-045-serial-monitor\Verification\verify_startup.py`，首次临时诊断宿主因重复设置Application.ResourceAssembly退出，失败记录保留；教练移除该宿主设置，不改学员TODO，不记学员错题。
- 修正后实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\Exercises\TASK-045-serial-monitor\Verification\verify_startup.py startup-02-final-2026-10-19.json`，再次构建0警告/0错误，临时STA宿主加载同一窗口并Show/关闭退出0，13项准备检查全部通过，16个旧文件哈希不变。
- 检查覆盖空历史、真实TODO状态、连接按钮启用和发送禁用、单点可见、表格/报警/曲线辅助、最小尺寸区域可用、复用半帧与25.6解析、关闭连接编号失效和窗口释放；诊断注入数据只验证教练辅助，不冒充串口接收或学员完成历史追加。
- 已查看默认及最小尺寸渲染，布局可用；结果见[最终准备证据](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Verification/startup-02-final-2026-10-19.json)、[最小尺寸图](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Verification/startup-minimum.png)及[旧文件基线](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-045-serial-monitor/Verification/baseline-2026-10-19.json)。
- 当前未打开实际串口，未验证后台串口往返，学员五处TODO尚未完成，三个DoD均不勾选，不创建下一题。
