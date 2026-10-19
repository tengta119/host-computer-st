# TASK-046：处理串口超时、断开与重新连接

- 阶段：第三阶段：设备通信（通信稳定性）。
- 状态：🟡 进行中
- 创建日期：2026-10-19。
- 开启日期：2026-10-19，用户明确要求“开启task046”。
- 计划来源：[计划](../plan.md)第三阶段中超时、设备断开、恢复连接及窗口关闭后的正确收尾。
- 选择依据：TASK-045已按用户静态口径通关，串口收发与WPF读数连接齐全，下一步把“对端不回复或通信中断时怎样恢复”纳入同一个完整功能，保留原源码与实际证据边界。
- 项目：复用[DeviceCommunication.Wpf.csproj](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/DeviceCommunication.Wpf.csproj)和TASK-044的可控模拟端，不新增通信框架。
- 难度/预计时间：待补充，不预设时长。

## 独立骨架与复用范围

此前只建卡的安排已由本次明确开启覆盖，原TASK-045文件保留，当前实际目录为Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/。
新增[窗口源码](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/Task046Window.xaml.cs)、[布局](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/Task046Window.xaml)、[串口辅助](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/Task046SerialConnection.cs)和[运行说明](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/RUN-GUIDE.md)。
Task046SerialConnection复用上一题的串口/缓冲思路，直接引用Task045Reading及两种事件参数，保留学员已有按钮、Dispatcher、历史、曲线和报警连接，不要求重写。
共享文件只修改DeviceCommunication.Wpf/App.xaml的StartupUri，切到本题窗口；项目依赖与控制台Program.cs不改，模拟端仍用TASK-044的Task044Simulator。
教练提供CurrentSessionId、HasConnection和BelongsToCurrentConnection辅助、串口失效错误通知路径、等待时禁用按钮的显示连接及关闭窗口的最终保护，核心等待/恢复逻辑留给学员。

## 本题的明确约定

发送55 01成功后开始计时，最多等待3秒，同一连接只允许一个等待请求，之后到达的下一条完整读数结束等待；接收处已用连接编号和发送时间排除旧连接或发送前产生的排队读数。
检查定时器每200毫秒安排一次窗口检查，秒表保存实际经过时长，窗口繁忙时检查可能延后；超时不依赖Tick次数，不靠等待时长判断帧是否完整。
超时后结束等待并允许重发，保留当前连接、历史及半帧；主动断开或通信失败则结束等待并释放旧连接，关闭时清理旧半帧。
没有请求编号就不能严格区分迟到的旧回复与新请求的回复，本题不声称解决这种协议限制；虚拟串口对端退出也不一定让本端IsOpen变false，可能只能观察到请求超时。

## TODO与首个动作

| TODO | 实际位置与产出 | 对应DoD |
| --- | --- | --- |
| 1 | BeginWaiting记录等待标记、连接编号与发送时间，启动秒表/检查定时器，显示等待并刷新按钮。 | 1 |
| 2 | EndWaiting停止检查、归零秒表、清等待标记和编号，显示传入结果并恢复按钮，供回复/超时/断开共用。 | 1、3 |
| 3 | CheckResponseTimeout排除关闭和旧连接，处理当前连接失效，并按实际时长决定继续等待或显示超时。 | 1、2、3 |
| 4 | RecoverConnection统一结束等待、释放端口、保留失败原因，即使释放异常也刷新按钮，允许手动重连。 | 2、3 |

首个动作只完成Task046Window.xaml.cs的BeginWaiting；连接后点击发送，看到等待提示且发送按钮变灰，之后再把其余出口接起来，TODO 2/3未完成时暂时不会自动恢复按钮。
新对象、Restart/Reset/Elapsed、定时器Start/Stop及错误编号判断先在运行说明中按当前窗口解释，不把前一题写过事件或Dispatcher推定为已掌握全部机制。
运行命令和模拟端1/2/3/r的实际操作见运行说明，沿用已有虚拟串口对，无需为每个任务重复创建端口。

## 完成一个能够恢复的通信流程

发送请求后窗口明确显示正在等待，按本题约定收到了完整读数就结束等待；没有回复时能显示超时，之后仍可重新发送或主动断开，而不是永远卡在等待状态。
接收或发送失败时连接状态与按钮相符，释放旧连接并显示原因，条件恢复后能手动重新连接；重新连接不混入旧半帧、旧超时动作或已排队的旧读数。
窗口关闭、主动断开和接收通知碰到一起时有一致清理路径，不因为取消登记或释放端口遗漏而继续更新已经关闭的窗口；自动无限重连不是本题默认要求。

## 开启时先明确的规则

1. 先约定“发送后什么时候开始计时、哪条完整读数算响应、超时后状态是什么”，超时时间在实际骨架里明确，不用等待时长判断一帧边界。
2. 当前教学协议没有请求编号，先约定同一连接内只允许一个等待中的请求，并用之后到达的下一条完整读数结束等待，不声称具备真实设备协议的严格请求匹配能力。
3. 若使用WPF已有定时器观察等待时长，先解释它只负责检查是否超时，不负责生成测量数据；若采用其他方式，也要先说明停止与旧连接失效的关系，不能只给取消或线程名词。
4. 保留TASK-045的窗口线程更新和连接编号保护，开启时检查异常通知在端口已失效时是否仍能被正确处理，不把“IsOpen为false”当作忽略所有失败提示的理由。

## 验收标准（DoD）

- [ ] 请求的等待、收到完整响应、超时和再次发送状态连接完整，等待期间不会并行重复发送造成响应混淆，半帧不冒充完整响应，超时不清掉其他连接的新状态。
- [ ] 打开/发送/接收失败或连接失效后有明确原因和一致的按钮状态，旧资源能释放，条件恢复后可以手动重连并继续收到读数，历史与协议解析规则保持。
- [ ] 主动断开、重连和窗口关闭能结束旧等待并丢弃旧连接通知，接收与关闭交错时有可追踪的清理路径，不重复回调或向已关闭窗口更新。

后续审查沿用用户最近的源码偏好时，明确按静态代码路径验收，不虚构实际断开、超时或串口测试结果；需要实际联调时再按用户要求执行，完整阶段目标的证据边界保留。

## 当前证据

2026-10-19实际执行dotnet build .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\DeviceCommunication.Wpf.csproj --nologo，0警告/0错误；随后执行python .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\Exercises\TASK-046-serial-recovery\Verification\verify_startup.py，构建和临时窗口宿主均退出0。
教练仅检查本题窗口实际Show、最小尺寸渲染和关闭，已查看[最小布局](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/Verification/startup-minimum-2026-10-19.png)，未打开串口、注入读数或测试学员TODO；[准备记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/Verification/startup-2026-10-19.json)记录25个既有文件哈希不变。
四个TODO尚未由学员完成，三个DoD未勾选，准备检查不计通信超时/重连的学员证据，第三阶段仍🟡 进行中，不创建下一任务。
