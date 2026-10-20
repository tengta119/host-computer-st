# TASK-050：把TCP读数和设备控制接入WPF监控

- 阶段：第四阶段：TCP/UDP（WPF监控接入）。
- 状态：🟡 进行中
- 创建日期：2026-10-20。
- 计划来源：[第四阶段计划](../plan.md)“把网络结果接入已有WPF监控能力”及实践顺序第2项。
- 选择依据：[TASK-049](./TASK-049.md)已按静态源码口径通关，连接、发送与LF解析路径完整，下一步将同一通信流程接到窗口，复用已有WPF绑定、历史、曲线、报警和采集启停经验，不再重复控制台收发或另拆字段小题。
- 开启日期：2026-10-20，用户明确要求“开启task50”；本题独立WPF工程与骨架现已准备，初次只建卡的安排保留为历史，不预设学习时长。

## 要完成的一个完整功能

窗口能够填写本机模拟器地址和端口，连接后读取温度、启动/停止远端设备、查询设备状态，并显示最新温度、历史、曲线、报警与通信记录。
“启动设备”发送START_DEVICE给模拟器，“开始采集”定期查询GET_TEMP，“断开连接”结束本次通信，三个动作分别说明，不把本地按钮文字变化当作设备状态确认。
先接通一次读温度，再连接远端控制与状态，最后加入可停止的周期查询和已有监控显示；同一连接始终一次只等待一条请求，接收过程中界面保持可操作。
自动重连、独立心跳机制、Socket对照和UDP沿第四阶段后续路线推进，本题先保留TASK-049的手动重连与请求失败后的清理约定。

## 开启时准备的骨架与讲解

开启时先核对Stage04-TcpUdp下现有项目和学员代码，再准备独立WPF项目与任务目录，具体实际路径届时记录；保留TASK-049和旧串口/WPF作品，不链接尚未创建的文件。
教练负责从控制台菜单中整理出可供窗口调用的连接、请求回复和断开辅助，复用既有LF解析与协议，不让学员为更换界面重写整套通信或引入新网络框架。
提供能运行的最小窗口、必要的数据/通知对象和已学绘图辅助，先说明每个自定义对象怎样创建、窗口通过什么属性读取它，再留下核心按钮调用、结果连接、历史和采集控制TODO。
先解释真实连接辅助的返回结果如何交给窗口，再结合具体执行位置解释界面线程更新；等待请求时禁止重复发送，周期查询与手动命令共用同一请求入口，不启动重叠读取。
首次动手只接通连接操作，在模拟器显示接受连接、窗口显示成功后，再逐步接读温度与历史；开启时提供实际运行命令和可观察结果。

## 验收标准（DoD）

- [ ] 连接/断开、单次温度读取、设备启动/停止与状态查询正确连接到同一TCP通信流程，失败反馈明确，写入成功与远端确认分别显示，界面保持可操作。
- [ ] 有效温度更新最新值、历史、曲线和报警，开始/停止采集能控制后续查询，不重复启动、不与手动请求争抢回复，不把接收批次当作采样条数。
- [ ] 断开、失败后手动重连和窗口关闭正确停止本次查询并清理连接，旧结果不污染新状态或已关闭窗口；源码审查、实际运行与教练辅助范围如实分开记录。

## 当前证据

本卡尚未开始，无本题源码、构建或运行结果，三个DoD均未勾选；TASK-049的静态通关和教练辅助的历史局部检查均不作为本题证据。

## 2026-10-20 实际开启与任务骨架

上节“尚未开始”是初次建卡时的历史，当前状态为🟡 进行中。
已创建独立项目[TcpUdp.Wpf.csproj](../../Stage04-TcpUdp/TcpUdp.Wpf/TcpUdp.Wpf.csproj)，目标net10.0-windows，不增加第三方依赖；共享[App.xaml](../../Stage04-TcpUdp/TcpUdp.Wpf/App.xaml)只负责指向本题窗口，[阶段解决方案](../../Stage04-TcpUdp/TcpUdp.slnx)增量加入新项目，旧入口不变。
本题专属目录为Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/，包含[窗口布局](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Task050Window.xaml)、[窗口后台](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Task050Window.xaml.cs)、[连接辅助](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Task050TcpConnection.cs)、[数据类](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Task050Reading.cs)、[绘图辅助](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Task050Chart.cs)和[运行说明](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/RUN-GUIDE.md)。
项目只链接编译已有Task049LineBuffer.cs，未改动它；设备模拟器继续运行TASK-049控制台中的Task049Simulator，不创建新协议或重复模拟器。
连接辅助由教练整理原TcpClient/NetworkStream收发，提供3秒保护；窗口提供串行请求、取消、连接编号、采集轮次保护、日志上限、表格绑定、绘图和阈值显示，不计这些骨架为学员独立设计。

| TODO | 实际位置、核心动作与可观察结果 | 对应DoD |
| --- | --- | --- |
| 1 | ConnectCoreAsync等待_connection.ConnectAsync，替换占位后，点连接能在两端看到成功。 | 1、3 |
| 2a、2b | SendCoreAsync等待并返回RequestAsync的完整回复，DisplayDeviceState把确认后的state显示到DeviceStatusText和OperationText。 | 1 |
| 3 | ApplyTemperature创建含接收时间和temperature的Task050Reading并加入_history，集合变化自动刷新表格、最新值、曲线和报警。 | 2 |
| 4 | StartCollectionClick连接_collecting、轮次编号、定时器Start和状态刷新，停止/断开/关窗的安全停止已提供。 | 2、3 |

首个动作只完成ConnectCoreAsync中的TODO 1，代码与具体对象解释先见运行说明；初始骨架会明确提示尚未连接，不将可启动窗口当作已完成通信。
运行时先在一个终端执行dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --simulator，再构建dotnet build .\Stage04-TcpUdp\TcpUdp.Wpf\TcpUdp.Wpf.csproj --nologo并运行dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp.Wpf\TcpUdp.Wpf.csproj，两个终端均从工作区根目录执行。
待四组TODO完成后提交源码并说“审查TASK-050”；本轮不创建TASK-051，三个DoD仍未勾选。

## 2026-10-20 骨架准备验证

实际执行dotnet build Stage04-TcpUdp/TcpUdp.Wpf/TcpUdp.Wpf.csproj --nologo，退出0、0警告/0错误。
实际执行python .\Stage04-TcpUdp\TcpUdp.Wpf\Exercises\TASK-050-tcp-wpf\Verification\verify_startup.py，最终退出0，临时输出构建0警告/0错误、23项教练准备检查全部通过，193个旧源码文件哈希不变，结果见[startup-2026-10-20.json](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Verification/startup-2026-10-20.json)。
检查覆盖真实窗口Show与关闭、初始按钮和占位反馈、默认/最小尺寸、注入样例的表格/曲线/阈值显示、日志200条上限、通信辅助与模拟器的分段回复/状态/重连、关闭时取消未完成读取与旧结果拒绝。
已查看[最小尺寸截图](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Verification/startup-minimum-2026-10-20.png)，三条历史与日志是教练注入的布局样例，通信检查直接调用教练提供的辅助，不执行或验收学员四组TODO，不计完整任务联调通过。
准备过程中修正了教练验证脚本的工作区定位，并将按钮样式放回任务窗口，使直接显示窗口与正常启动一致；一次验证宿主错误清空StartupUri的失败已保存为[startup-host-before-fix-2026-10-20.json](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-050-tcp-wpf/Verification/startup-host-before-fix-2026-10-20.json)，最终验证已通过，不归为学员错误。
验证仅创建并关闭自己的临时窗口与本机模拟器，没有停止用户原有程序，旧串口与WPF作品保持，原问答和已验证能力范围不变。
