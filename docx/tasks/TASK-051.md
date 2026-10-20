# TASK-051：完善TCP超时、重连与心跳恢复

- 阶段：第四阶段：TCP/UDP（通信稳定性）。
- 状态：✅ 已通关
- 创建日期：2026-10-20。
- 计划来源：[第四阶段计划](../plan.md)的超时、断开、手动/自动重连、心跳、旧连接保护及关闭收尾目标。
- 选择依据：[TASK-050](./TASK-050.md)已按静态源码口径通关，WPF按钮、温度历史与采集连接完整；现有3秒保护和连接清理由教练提供，接下来围绕模拟器故障后的完整恢复学习，不重做正常收发或单拆每个判断。
- 开启日期：2026-10-20，用户明确要求“开启task051”；本题独立骨架与故障对端已准备，初次仅建卡的安排保留为历史，不预设学习时长。

## 一个完整的恢复场景

上位机正在监控模拟设备，模拟器退出或停止回复时，界面明确显示原因，结束旧等待并停止采集；模拟器恢复后可以手动连接，也可以按用户开启的自动恢复选项重新连接。
连接成功不表示远端设备已启动，恢复后先查询设备状态，自动重连不会重发上次的START_DEVICE或STOP_DEVICE，采集保持停止，由用户明确开始。
连接空闲时按约定发送PING并等待PONG，正在等待业务回复时不能再发一条抢读回复的心跳，不能只依据本地保留了_stream就显示远端仍存活。
主动断开或关闭窗口后，取消接收、心跳、重连等待和后续尝试，不让旧结果改写新连接状态，不重复启动定时器或连接流程。

## 开启时准备与讲解

开启时读取现有TASK-050代码与运行入口，在独立任务目录准备新窗口和必要辅助，复用已完成的协议、显示与历史，保留原作品和学员核心实现。
教练提供可控的延迟、不回复、主动关闭与恢复等模拟场景及清楚的日志，说明哪些验证本机对端行为，哪些仍不足以证明所有真实网络故障已覆盖。
先从“当前正在等连接还是正在等某条完整回复”解释两种截止时间，再引入心跳和重连的等待，避免把所有等待都叫读取超时。
自动重连的间隔、上限和停止规则、心跳间隔及无回复判定在动手前明确；参数与辅助方法先结合真实对象解释，核心状态和操作连接留TODO，不引入新网络框架。
先完成一次“模拟器不回复 → 当前请求失败 → 旧连接清理 → 手动恢复”的可观察结果，再在同一完整任务里接自动重连和空闲心跳，不设口试或重复抄录记录的门槛。
开始时再记录实际工程、任务目录、入口、每个TODO和运行命令，此处不链接尚未创建的源码。

## 验收标准（DoD）

- [x] 连接失败、业务请求无回复、对端关闭和心跳无回复能分别进入明确状态，按约定结束等待并清理旧连接，不误报设备执行成功，不混用迟到回复。
- [x] 手动与可停止的自动重连按约定恢复，连接尝试不重叠，空闲心跳与业务请求共用单一收发路径；恢复后查询实际设备状态，不自动重放控制命令或自动恢复采集。
- [x] 主动断开和关窗取消所有等待、心跳、采集与重连，旧连接/轮次结果被拒绝，原有效历史与正常显示保持；源码、实际运行和教练辅助证据分别注明。

2026-10-20最终复查三个DoD均按用户既有静态源码口径通过，任务✅ 已通关，不代表本轮完成运行验证；早期待修及未勾选描述保留为历史，详细依据见文末。

## 当前证据

当前最终静态复查三个DoD通过，学员已自主补齐CanSendHeartbeat的未采集条件，任务✅ 已通关；TODO 2及上轮状态查询取消检查/日志仍为教练协助，本轮未改源码、构建或运行，详细证据见文末。

## 2026-10-20 实际开启与文件范围

上节“未开始”是初次建卡记录，当前状态为🟡 进行中。
复用已有[TcpUdp.Wpf项目](../../Stage04-TcpUdp/TcpUdp.Wpf/TcpUdp.Wpf.csproj)，本题WPF专属目录为Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/，包含[窗口后台](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Task051Window.xaml.cs)、[界面](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Task051Window.xaml)、[带超时位置的连接辅助与异常类](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Task051TcpConnection.cs)及[运行说明](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/RUN-GUIDE.md)。
窗口复制复用TASK-050已完成的正常连接、读数/控制、温度历史与采集实现，继续引用Task050Reading、Task050LogEntry、Task050Chart和已有Task049LineBuffer，原任务文件未改。
故障对端放在独立目录Stage04-TcpUdp/TcpUdp/Exercises/TASK-051-fault-simulator/，实际文件为[Task051Simulator.cs](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-051-fault-simulator/Task051Simulator.cs)，旧Task049Simulator保持不变。
共享文件只增量调整App.xaml启动Task051Window、WPF项目排除本题Verification中的测试源码、控制台Program.cs新增--task051-simulator模式；旧命令模式保留，不引入新依赖或网络框架。
连接/请求的3秒截止、带Operation的自定义超时异常、单一路径、旧连接/采集轮次保护、取消与关窗收尾和重试次数上限由教练提供，不计学员自主设计。

## TODO、配置与首个动作

| TODO | 实际方法与核心动作 | 对应DoD |
| --- | --- | --- |
| 1 | DescribeFailure区分超时正在等待的位置、连接/收发失败和协议错误，返回明确原因，外层再执行统一清理。 | 1 |
| 2 | ScheduleReconnect在已有意图、开关和最多5次检查后设置等待提示并启动同一个_reconnectTimer，不递归连接或阻塞线程。 | 2 |
| 3 | QueryStateAfterConnectAsync等待GET_STATUS，记录完整回复并交ApplyReply，不重发控制命令或恢复采集。 | 2、3 |
| 4a、4b | CanSendHeartbeat判断未采集且空闲达到HeartbeatIdle；ValidateHeartbeatReply只接受PONG，已有入口负责单请求和失败清理。 | 1、2 |

本题明确采用连接与完整回复各3秒、每次失败结束后等待2秒再自动重连、最多5次连续恢复失败、空闲至少2秒再心跳、一次心跳无回复就结束本连接；成功连接并查询状态后尝试计数归零，采集保持停止。
主动断开、取消自动恢复、窗口关闭以及正在连接/读取时的取消由教练提供安全保护；当前自动调度和心跳条件尚未接通，不将可运行骨架当作这些TODO完成。
首个动作只补DescribeFailure中Task051TimeoutException的原因分支，具体类、Operation、is与转换写法先见运行说明，再用silent-temp模式观察约3秒后的业务请求超时与清理，并手动切回normal恢复。
从工作区根目录先构建控制台，运行dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task051-simulator 5000 silent-temp，再构建并启动TcpUdp.Wpf项目；完整双终端命令、五种故障模式和四组步骤见运行说明。
当前三个DoD未勾选，完成后说“审查TASK-051”，继续按源码审查偏好记录；本轮不创建TASK-052。

## 2026-10-20 教练骨架准备验证

实际执行dotnet build Stage04-TcpUdp/TcpUdp.Wpf/TcpUdp.Wpf.csproj --nologo，退出0、0警告/0错误。
实际执行python .\Stage04-TcpUdp\TcpUdp.Wpf\Exercises\TASK-051-tcp-recovery\Verification\verify_startup.py，退出0，临时输出构建0警告/0错误、40项窗口与辅助准备检查通过，198个受保护旧源码哈希不变，结果见[startup-2026-10-20.json](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Verification/startup-2026-10-20.json)。
检查覆盖真实窗口Show/关闭、默认与最小尺寸、注入样例的显示、正常通信辅助、业务/心跳等待到期、4秒迟到温度不进入新状态查询、主动关闭、外部取消不误记超时，以及直接设置场景的重试上限、取消恢复和关窗清理。
另用临时输出实际构建控制台入口，0警告/0错误；帮助、非法端口、非法模式及新命令行实际状态回复共4项检查通过，实际命令和输出见[cli-preparation-2026-10-20.json](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Verification/cli-preparation-2026-10-20.json)。
已查看[最小窗口截图](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Verification/startup-minimum-2026-10-20.png)，温度历史是教练注入的布局数据；本轮直接验证了教练辅助及故障对端，没有实现或验收学员TODO、自动恢复查询与完整心跳流程，不勾选DoD。
本机对端的连接建立超时没有实际诱发，只按源码保留该分支，不能把无监听时的立即拒绝或业务超时当作其运行证据；当前本机故障也不代表真实断网场景全覆盖。
只创建并关闭本轮自己的临时窗口与模拟器，不停止用户原有程序，TASK-049/050及原串口/WPF作品保留，问答待复习和已有能力边界保持。

## 2026-10-20 TODO 2教练协助

用户要求“帮我完成todo 2”，仅替换ScheduleReconnect的未完成提示，显示2秒后进行第几次自动连接并启动已有_reconnectTimer，原入口检查、5次上限、ReconnectTick以及学员其他实现和注释均保留。
实际执行python .\Stage04-TcpUdp\TcpUdp.Wpf\Exercises\TASK-051-tcp-recovery\Verification\verify_todo2.py，退出0，临时输出构建0警告/0错误，12项针对性检查通过，结果见[todo2-assisted-2026-10-20.json](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Verification/todo2-assisted-2026-10-20.json)。
检查包含等待提示、同一个2秒定时器、调度不立即连接或增加次数、失败清理期间_busy仍为true时可调度、实际延后触发后忙状态继续等待，以及未勾选、无连接意图、取消勾选、主动断开、5次上限和关闭窗口的停止保护。
本轮未验证完整自动连接成功、恢复后状态查询或心跳流程，TODO 3/4未改，已有TODO 1不在本轮审查范围；这次协助不计学员自主掌握，任务保持🟡 进行中，三个DoD未勾选，不创建TASK-052，问答和错题状态保持。

## 2026-10-20 静态源码审查：待修心跳条件

按用户已有源码审查偏好检查，未构建、运行或执行测试；DoD1/3按静态口径通过，DoD2未通过，详见[审查记录](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Verification/static-review-2026-10-20.md)。
学员已完成GET_STATUS查询及状态显示、UTC空闲时间计算和只接受PONG的验证，自动调度保留此前教练协助；当前CanSendHeartbeat只看空闲时间，缺少_collecting为false，采集开启但超过2秒无有效通信时仍会允许自动PING。
下一步只修改CanSendHeartbeat，使正在采集时返回false，未采集时继续按已有HeartbeatIdle判断；保留其他实现后提交复查，不重写重连或PONG验证。
教练直接补GET_STATUS的完整回复日志，并修复骨架中查询await后缺少取消检查的遗漏，保留学员原请求和ApplyReply调用；已按源码核对顺序及差异，未运行验证，不计学员自主掌握或典型错误。
任务和第四阶段保持🟡 进行中，不创建TASK-052，问答状态与统计保持。

## 2026-10-20 最终静态复查：✅ 已通关

学员已将心跳条件改为idle >= HeartbeatIdle && !_collecting，采集中即使空闲时间达到2秒也不会允许自动心跳，未采集时仍按既有空闲时间判断，上次问题已修复。
三个DoD按静态源码口径通过，详见[最终复查记录](../../Stage04-TcpUdp/TcpUdp.Wpf/Exercises/TASK-051-tcp-recovery/Verification/static-review-final-2026-10-20.md)；本轮未修改功能源码、构建、运行或测试，不新增完整TCP恢复联调或全面自主掌握证据。
TODO 2、查询后取消检查和日志等协助范围保留，问答统计与状态保持，错题本仅关闭这次心跳条件漏项，不抹除历史。
按计划选择推进到Socket/UDP尚余目标，仅创建[TASK-052：完成Socket收发对照与UDP设备查询](./TASK-052.md)⚪ 未开始卡片，未准备代码、切换入口或启动实践，第四阶段仍🟡 进行中。
