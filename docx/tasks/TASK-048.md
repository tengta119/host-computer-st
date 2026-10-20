# TASK-048：整合串口监控与通信日志

- 阶段：第三阶段：设备通信（串口综合收尾）。
- 状态：✅ 已通关
- 创建日期：2026-10-20。
- 开启日期：2026-10-20，用户明确要求“开启task048”。
- 计划来源：[计划](../plan.md)第三阶段中正确发送并记录原始接收字节/有效数量、完整组包与有效读数显示、超时和断开重开的综合目标。
- 选择依据：TASK-044至047已按用户静态口径通关，收发、协议判断、显示与恢复源码已连接，但当前WPF窗口主要显示温度，缺少同一窗口里可核对“发了什么、收到什么、为什么未得到读数”的通信日志，实际联调证据也尚未补齐。
- 项目：复用[WPF项目](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/DeviceCommunication.Wpf.csproj)和[TASK-047](./TASK-047.md)已完成协议/模拟端，不新增设备协议、通信框架、数据库或额外串口驱动。
- 难度/预计时间：待补充，不预设时长。

## 已准备的独立骨架

此前只建卡的安排已由本次明确开启覆盖，专属目录为Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/，共享文件只修改WPF的App.xaml切到Task048Window。
新增[窗口源码](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Task048Window.xaml.cs)、[布局](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Task048Window.xaml)、[串口辅助](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Task048SerialConnection.cs)、[日志数据与参数类](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Task048LogEntry.cs)、[返回拒绝原因的协议辅助](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Task048Protocol.cs)和[运行说明](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/RUN-GUIDE.md)。
复制后复用TASK-047的窗口监控、缓冲恢复及生命周期，仍引用TASK-045读数/事件参数；协议辅助由教练把原有条件整理为可返回原因的一条判断路径，不改变字段、范围或加和规则。
模拟端直接复用已有[Task047Simulator.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-047-protocol-simulator/Task047Simulator.cs)及其14组样例，控制台Program.cs和两个项目文件/依赖均不改，TASK-044至047原作品保留。
教练已准备日志区域、事件登记/撤销、锁外批量通知、Dispatcher与连接编号保护、格式辅助和清空日志按钮，日志追加/数量控制、有效字节保存和窗口状态记录留给学员，不要求重写旧功能。

## 三组TODO与首个动作

| TODO | 真实位置与动作 | 对应DoD |
| --- | --- | --- |
| 1 | Task048Window.AppendLogOnUi将entry加入_communicationLogs，超过200删除最旧条目，更新LogCountText；绑定已提供。 | 1、3 |
| 2 | Task048SerialConnection.AddByteLog复制data中count个有效字节，用FormatBytes得到文字，创建含count的Task048LogEntry并加入本批logs。 | 1、2 |
| 3 | 窗口ConnectClick成功/失败、EndWaiting的wasWaiting分支及RecoverConnection的finally中调用RecordStatus，记录结果并使用已保存的实际连接编号和端口。 | 1、2、3 |

首个动作只改AppendLogOnUi，把entry追加到已有集合并完成200条上限/条数显示，重新启动窗口应先出现一条“窗口已就绪”记录，不需要连接串口。
TODO 2完成后再用模拟端2/3观察本次接收2/4字节与完整6字节有效帧，TODO 3连接原有状态结果，不把连接成功或写入成功误写成已收到回复。
新的Task048LogEntry、Task048LogsEventArgs、RecordStatus和GetRejectionReason均是教练命名的普通类/方法，具体参数来源与对象作用先见运行说明；协议条件和已有收尾代码不重复考察。
窗口最小尺寸调整为1040乘840以容纳日志区，长字节/说明可悬停查看全文，历史不截断，日志上限固定200。


## 完成一个可定位通信问题的监控窗口

窗口保留原监控功能，并显示本次连接的通信日志：发送55 01、收到的原始字节和本次有效数量、生成有效读数、请求超时与连接错误。
有原始接收记录却没有新温度时，能够分清尚未收齐与完整候选不符合协议；日志只描述可见操作和检查结果，不把每次接收通知都误称为完整一帧。
拒绝原因按已有检查给出具体内容，例如设备不合法、命令不合法、原始值超范围或校验不符；只增加对已有判断结果的说明，不改变TASK-047规则或为坏帧另造一套解析流程。
日志在窗口线程更新，并带连接归属保护，关闭/重连后排队的旧日志不能伪装成新连接结果；日志条数设明确上限，例如最近200条，避免持续接收让诊断列表无限增长，不擅自截断原温度历史。
正常读数、半帧/连续帧、坏帧后恢复、只发坏帧直到超时、断开重开与关闭后的清理仍在同一个功能中保持一致。

## 开启时先准备与解释

1. 提供最小日志区域、日志数据对象和接收通知辅助，先说明哪些是教练新建的名字、每条记录携带哪些内容，以及哪段代码运行在接收线程，之后再留核心连接TODO。
2. 收到数组只记录Read返回数量以内的字节，复制需要保留的内容，不能把256字节容量或旧数组残留当成本次接收；发送日志仅在Write成功后记为发送成功。
3. 日志通知在锁外发出，避免窗口线程更新与关闭锁相互等待，复用连接编号和Dispatcher，不用日志功能重新引入一套收发对象或定时器。
4. 继续使用TASK-047的14组专用模拟样例，不修改旧任务文件；真实运行只按届时用户授权安排，用户只要求源码审查时保留明确的静态与运行证据边界，不阻塞于重复填写记录。

## 验收标准（DoD）

- [x] 同一窗口能连接原监控和有界通信日志，正确记录发送、原始接收字节/有效数量、候选拒绝原因、有效读数与超时/错误；不把半帧误记为完整有效帧，也不把写入失败记成成功。
- [x] 原TASK-047的字段/范围/校验、半帧/连续帧、无效候选恢复与合法数据内AA规则保持，只有有效读数更新历史/曲线/报警并结束等待，坏帧等待到有效回复或3秒超时，之后可重新发送。
- [x] 主动断开、端口失败、手动重连和窗口关闭的清理与日志归属一致，旧缓冲/通知不污染新连接，不重复登记，旧TASK-044至047成果保留；记录具体代码或获授权的运行证据，不伪造实际串口结果。

## 当前证据与阶段边界

2026-10-20实际执行python .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\Exercises\TASK-048-serial-monitor-integration\Verification\verify_startup.py，脚本使用临时--output目录构建新WPF源代码，0警告/0错误，未停止用户原有进程；临时窗口实际Show、日志最小尺寸检查和关闭均通过、退出0。
已查看[最小布局](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Verification/startup-minimum-2026-10-20.png)，[准备记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Verification/startup-2026-10-20.json)保存真实命令/输出及48个既有文件哈希不变的结果。
临时宿主直接给集合注入三条布局示例，不调用学员未完成的日志追加方法，不是实际串口数据；本轮未打开串口、运行模拟端或测试学员TODO，三组TODO尚未完成、三个DoD未勾选，状态🟡 进行中，不创建下一题。
后续任务审查继续遵从用户“只看源码，源码正确即可通过”的口径；第三阶段计划中串口往返、坏帧与关闭恢复的实际运行证据尚未验证，不因为任务源码通关就宣布阶段完成，也不擅自安排测试。
TASK-048是把既有串口功能连成可诊断的整体，不提前扩展TCP/Modbus，后续路线仍以计划和用户意图为准。


## 2026-10-20 静态审查：仍🟡 进行中

按用户偏好仅审源码，未构建、运行、测试或打开串口，[审查记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Verification/static-review-2026-10-20.md)与[协助前后哈希](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Verification/static-review-2026-10-20.json)已保存。
TODO 1的200条上限/集合显示、TODO 2的count复制/格式化和多数状态连接已正确，DoD1/2按静态源码路径通过并勾选；原协议、等待、恢复与显示均保留，不要求重写。
DoD3未通过：EndWaiting已经保存waitingSessionId/waitingPortName，但日志第三、第四个实参仍读取当前连接，旧等待清理分支可能归错连接；只将这两个实参改用已保存局部值即可。
教练按已有低影响收尾授权直接把结束日志说明改为message，把“回复连接”类别改成“连接清理”，未代改两个归属参数，未执行运行验证，文案协助不计学员独立掌握。
任务仍进行中，不创建TASK-049；串口Q6待复习和27分类/76条统计保持，旧准备检查不作学员实际联调证据。


## 2026-10-20 最终静态复查：✅ 已通关

学员已将等待结束日志改为使用message、waitingSessionId和waitingPortName，原有效字节复制、200条上限、协议/显示/等待与连接清理保持，三个DoD按静态源码口径全部通过、勾选已同步。
[最终记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Verification/static-review-final-2026-10-20.md)与[本轮源码哈希](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Verification/static-review-final-2026-10-20.json)已保存，本轮未改源码、构建、运行或测试；上一轮两处文案由教练协助，不冒充学员独立掌握，原待修结论保留为历史。
当前串口源码任务已收尾，实际串口联调未验证，阶段整体仍进行中，详见[阶段源码覆盖核对](../notes/serial-stage-source-check.md)；不自动生成TASK-049或开启下一阶段，WPF继续TASK-048，后续实际收尾可合并本题，Q6待复习和统计保持。
