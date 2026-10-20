# TASK-050静态源码审查

- 日期：2026-10-20。
- 范围：只检查当前窗口、XAML、通信辅助、数据类、绘图、项目入口及复用的LF解析，不构建、不运行、不测试、不建立TCP连接。
- 结论：四组核心连接完整，三个DoD按已约定的静态源码口径通过，任务✅ 已通关；本轮未验证完整窗口与模拟器的实际联调。

## 逐项验收

| DoD | 结论 | 源码依据 |
| --- | --- | --- |
| 1：窗口连接、读数、远端控制和状态确认。 | 通过（静态）。 | ConnectCoreAsync等待既有ConnectAsync，SendCoreAsync等待并返回RequestAsync；XAML四个操作按钮分别进入同一RunCommandAsync，ApplyReply核对完整回复后才处理状态；DisplayDeviceState已把确认状态赋给真实控件，教练只将原英文显示改为中文和明确的确认文案；连接失败与协议失败由已有处理反馈。 |
| 2：有效温度、历史/曲线/报警和采集启停。 | 通过（静态）。 | ApplyTemperature在检查有效小数后创建含DateTime.Now和temperature的Task050Reading并加入原_history；同集合绑定与CollectionChanged连接到RefreshPresentation和绘图，阈值可立即复算；StartCollectionClick设置_collecting、正确递增轮次、Start定时器和刷新控件；已有停止方法终止后续通知，_busy和轮次判断防止请求重叠及停止后的旧温度追加。 |
| 3：断开、失败恢复、关窗与旧结果隔离。 | 通过（静态）。 | ResetConnection递增连接编号、停止采集、取消并释放会话、关闭TCP连接；ConnectClick重新创建会话，旧请求完成后先检查IsCurrentSession；旧轮次采集结果被过滤，OnClosed设置关闭标记并撤销定时器/集合事件，RefreshControls与显示路径均受关闭保护。 |

## 学员实现与教练协助

学员已在现有骨架中接通连接调用、返回完整请求结果、确认状态显示、带时间的温度记录追加、采集标记与轮次递增和定时器启动，本轮确认其静态代码正确，不扩大为实际运行证据。
原DisplayDeviceState把RUNNING/STOPPED原样赋给DeviceStatusText和OperationText，状态来源与连接时机正确；教练按低影响文案收尾授权改为“运行/停止”和确认说明，不要求为两处文字重新提交。
教练还移除了构造日志与XAML初始提示中已经过时的“先完成TODO 1”文字，未修改四个核心方法ConnectCoreAsync、SendCoreAsync、ApplyTemperature、StartCollectionClick，学员注释保留。
读取循环、LF解析、超时/取消、串行保护、会话与采集轮次保护、绑定/通知/绘图/阈值和模拟器仍按原范围记为教练支持，不因任务通关认定完整线程机制、TCP Server、解析或生命周期独立设计已掌握。
审查前后源文件哈希、限定文案变更与四个核心方法未变的记录保存在static-review-before-2026-10-20.json和static-review-2026-10-20.json，本轮没有新增构建或运行结果。

## 证据边界与后续安排

23项准备检查及截图是开启任务时的教练骨架结果，不能替代本轮学员实现的实际窗口/TCP联调，不重复执行原占位检查，也不把设计预览当运行证据。
本轮没有学员认知错误需要记录，不新增错题或问答掌握状态；只创建TASK-051未开始卡片，下一步按计划处理完整通信稳定性，第四阶段仍🟡 进行中。
