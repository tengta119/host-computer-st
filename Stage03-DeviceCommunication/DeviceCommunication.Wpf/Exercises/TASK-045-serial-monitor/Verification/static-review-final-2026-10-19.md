# TASK-045：静态源码验收

- 日期：2026-10-19。
- 范围：沿用用户最近“源码没问题就通过”的要求，本轮只阅读源码、XAML、项目配置和任务规则，没有构建、运行、执行测试或打开串口。
- 结论：完成三处低影响收尾后，未发现本任务范围内的阻塞源码问题，按静态口径通关，不声称实际界面或串口往返已验证。

## 学员五处核心连接

| 部分 | 源码依据与结论 |
| :--- | :--- |
| ConnectClick。 | 对端口Text做空白检查后调用同一_connection.Open(portName)，成功再刷新实际连接状态和提示，异常返回菜单显示失败。 |
| DisconnectClick。 | 调用_connection.Close并刷新控件，不删除历史，服务内部清空旧半帧、取消该次接收登记。 |
| SendClick。 | 使用同一_connection.SendRequest发送原始55 01，异常有反馈，没有另建第二套假串口状态。 |
| SerialReadingReceived。 | 保留关闭保护，通过Dispatcher.BeginInvoke和Action将本次e交给ApplyReadingOnUi，未在接收线程直接修改控件或集合。 |
| ApplyReadingOnUi。 | 保留关闭/旧连接编号判断，把e.Reading追加到原_history，再刷新最新值、条数、曲线和报警。 |

## 关联辅助静态检查

- XAML按钮Click与方法名对应，HistoryGrid指向同一ObservableCollection，最新值/报警/按设备曲线读取这份历史，无生成假读数的定时器。
- SerialPort.DataReceived先读取实际数量再进缓冲，非AA先清理，合法半帧保留，完整帧取出后继续处理，原始值除以10.0得到double温度。
- Open只为每次新建端口登记一次接收，Close取消登记、失效连接编号并清半帧，Dispose和OnClosed取消窗口订阅，排队的旧通知由ApplyReadingOnUi的保护判断丢弃。
- App.xaml指向任务窗口，项目引用System.IO.Ports 10.0.0，旧控制台模拟器入口保持。
- 更完整的请求超时、设备断开恢复和高频接收压力属于后续任务，不把本次源码阅读扩展成已经验证全部通信稳定性。

## 教练低影响收尾

- Task045SerialConnection.cs事件登记行原为port.DataReceived += PortDataReceived;x，末尾多余x会造成语法错误，教练直接删除该字符，未改学员五处核心连接，不将误输入记为认知错题。
- Task045Window.xaml中初始化提示和页脚仍写“待完成TODO”，教练改成当前可用流程的说明，避免误导已完成连接的学员。
- 修改后只重读相应源文件核对，未执行构建、测试或运行，不提供虚构的编译退出码或窗口操作结果。

## 验收与证据边界

三项静态DoD通过，TASK-045标为✅ 已通关，原运行描述的卡片保存在docx/tasks/history/TASK-045-before-static-acceptance-2026-10-19.txt。
记录学员完成按钮、Dispatcher交接与历史追加的源码连接，不因静态通关推定事件/委托原理和真实串口已自主运行验证，串口Q3及委托Q6仍待复习，旧已验证能力保持。
只创建下一张TASK-046未开始卡，安排串口超时、断开与重新连接的完整稳定性实践，本轮不准备源码或切换入口，第三阶段仍🟡 进行中。
