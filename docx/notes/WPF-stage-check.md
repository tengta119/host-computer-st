# WPF阶段目标核对

- 日期：2026-10-18。
- 结论：当前[计划原文](../plan.md)的第二阶段WPF基础应用与设备监控界面实践目标已覆盖，阶段状态✅ 已通关；用户已决定后续阶段另建学习系统，本系统不继续安排任务。
- 核对方式：按计划逐项对应已验证的任务与最终综合实践，不仅凭已有任务全部完成作判断，本轮未重跑历史窗口，也未增加新考试或补写报告要求。

## 计划目标与实际证据

| 计划目标 | 对应实践 | 已验证范围 |
| :--- | :--- | :--- |
| Window。 | [TASK-009](../tasks/TASK-009.md)、[TASK-038](../tasks/TASK-038.md)。 | 实际窗口创建、输入和按钮操作、尺寸与关闭。 |
| Button / TextBox / ComboBox / DataGrid。 | [TASK-009](../tasks/TASK-009.md)、[TASK-019](../tasks/TASK-019.md)、[TASK-020](../tasks/TASK-020.md)、[TASK-038](../tasks/TASK-038.md)。 | 按钮动作、输入、设备选择及完整历史表格，最终综合窗口实际运行。 |
| Grid / StackPanel / DockPanel。 | [TASK-021](../tasks/TASK-021.md)、[TASK-022](../tasks/TASK-022.md)、[TASK-023](../tasks/TASK-023.md)、[TASK-037](../tasks/TASK-037.md)。 | 分行、分列、横向组合、靠边和填充、表格伸展；长文字布局两处由教练协助，范围保留。 |
| XAML。 | [TASK-009](../tasks/TASK-009.md)至[TASK-038](../tasks/TASK-038.md)。 | 自主连接控件属性、布局和绑定，并以实际窗口验证。 |
| Event。 | [TASK-009](../tasks/TASK-009.md)、[TASK-027](../tasks/TASK-027.md)、[TASK-038](../tasks/TASK-038.md)。 | Click、Tick单次登记、通知到动作、启停与关闭后的事件连接效果。 |
| Binding。 | [TASK-010](../tasks/TASK-010.md)、[TASK-013](../tasks/TASK-013.md)、[TASK-035](../tasks/TASK-035.md)、[TASK-038](../tasks/TASK-038.md)。 | 同一DataContext、选择写回、输入即时更新、属性通知和结果显示。 |
| ObservableCollection。 | [TASK-011](../tasks/TASK-011.md)、[TASK-025](../tasks/TASK-025.md)、[TASK-038](../tasks/TASK-038.md)。 | 同集合追加、清空、完整历史、Count和曲线更新，设备集合切换。 |
| Command。 | [TASK-012](../tasks/TASK-012.md)、[TASK-034](../tasks/TASK-034.md)、[TASK-038](../tasks/TASK-038.md)。 | 执行、可执行条件、条件重查通知和按钮绑定，运行保护。 |
| MVVM。 | [TASK-013](../tasks/TASK-013.md)、[TASK-033](../tasks/TASK-033.md)至[TASK-038](../tasks/TASK-038.md)。 | 新VM中的状态、业务方法、命令、属性通知和界面连接共同工作。 |
| UserControl。 | [TASK-014](../tasks/TASK-014.md)。 | 历史区域封装、嵌入窗口及同数据/集合/命令连接的实际应用。 |
| Style / Template。 | [TASK-015](../tasks/TASK-015.md)、[TASK-016](../tasks/TASK-016.md)、[TASK-017](../tasks/TASK-017.md)。 | 按钮共享外观、行数据显示模板、按钮内部模板与条件显示的实际应用。 |
| 数据图表。 | [TASK-018](../tasks/TASK-018.md)、[TASK-024](../tasks/TASK-024.md)至[TASK-027](../tasks/TASK-027.md)、[TASK-038](../tasks/TASK-038.md)。 | 温度和坐标的对应、集合驱动最新三点、完整历史与实时采集更新；固定画布和辅助绘图的教练来源保留。 |
| 能编写设备监控界面。 | [TASK-037](../tasks/TASK-037.md)、[TASK-038](../tasks/TASK-038.md)。 | 自主主要布局/绑定及新VM/命令/设备状态/采集连接，实际两设备历史、参数、报警、配置和CSV完整操作；最终264项功能全通过。 |

## 完成边界

核对范围是计划中的基础应用和监控实践，不是所有WPF API、框架内部机制、图表库、通用控件框架或真实设备通信；这些扩展不是原计划第二阶段的逐项必需验收目标。
各任务中教练提供的字段、类型签名、构造/接口基础、布局协助、绘图、文件助手和安全收尾仍按原证据区分，不因阶段通关变成学员独立编写。
当前没有未完成的第二阶段必需实践目标，问答中事件、属性继承、模板绑定、坐标等已注明的原理复习继续保留，结合后续项目按需巩固，不把复习列表等同于新增WPF阶段任务。
保留25分类/63条、待复习32/已掌握31的问答状态，不因阶段通过把所有概念自动升级为已掌握；第一阶段C#概念复习状态也不随之修改。
2026-10-18用户要求删除此前规划的后续TCP任务卡，已执行；后续阶段由用户另建学习系统，原计划保留作来源，本系统的WPF完成结论和历史证据保持。
