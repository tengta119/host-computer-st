# TASK-034：用Command连接追加与清空操作

- 阶段：第二阶段：WPF（Command与MVVM整合）。
- 状态：🟡 进行中
- 计划来源：[计划原文](../plan.md)中Command、MVVM及独立设备监控界面目标。
- 选择依据：TASK-033已验证ViewModel保存同一历史并通过属性绑定显示状态，下一步把追加/清空动作交给同一个ViewModel的命令，复用TASK-012/013的命令经验。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

“添加下一条”和“清空”按钮改用Command连接ViewModel中的操作，不再由这两个按钮的Click处理器直接修改集合。
样例顺序和下一条位置由ViewModel管理，手动追加与定时采集共用一个追加入口，仍按24、26.5、25、28循环。
清空命令在采集中不可执行，停止后可以清空并将下一条复位到24，界面和方法内部均保留保护。
定时器、绘图、配置和保存窗口仍可暂留窗口，复用既有功能，不要求本题一次迁移全部窗口行为。

## 开启时准备约定

目前只创建任务卡，不准备源码或切换入口，App.xaml保留TASK-033。
明确开启后读当前代码与Command/MVVM问答，准备独立任务目录，保留原作品。
先用真实按钮说明Command替代哪一次Click调用，再解释ICommand、Execute、CanExecute和状态改变后的通知，不假定旧任务中的命令骨架都已理解。
优先沿用已有项目的普通完整命令写法，不为整理引入新MVVM框架或未说明的泛型辅助类。
教练准备必要签名与安全占位，核心命令执行、启用条件和按钮连接留TODO；首步只接通“添加下一条”命令，点一次增加一条。
运行状态如何从窗口交给ViewModel须先具体解释；状态供命令判断使用，不再新建另一份定时器或集合。

## 实践与产出

1. 将样例位置与追加/清空动作集中到当前ViewModel，原定时通知复用同一追加入口。
2. 两个按钮通过Command绑定调用操作，采集中清空禁用，停止后恢复。
3. 验证手动/自动混合追加的顺序、清空复位、命令保护和原导出/配置/报警显示保持。

## 验收标准（DoD）

- [ ] 两个按钮使用当前同一个ViewModel的Command，实际按钮执行不再走旧追加/清空Click，连续手动五条、自动与手动混合追加均只追加一次且顺序正确，表格/Count/最新三点来自同一历史。
- [ ] 采集中清空命令不可执行且按钮禁用，直接请求执行也不清空；停止后恢复可执行，实际清空后Count为0、报警暂无温度、图表为空，下一条从24开始，重复启停不重复登记或追加。
- [ ] 原有效阈值即时刷新、配置保存恢复、CSV选择位置/取消/覆盖/写入失败恢复保持，导出不改数据，默认及约定最小尺寸可读，实际窗口正常关闭。

## 状态与证据

- 2026-10-17：TASK-033三个DoD通过后仅创建本卡，未准备代码、切换入口或运行本题验收。

## 2026-10-17明确开启：实际骨架与首步

当前状态🟡 进行中，上方仅建卡片/未准备代码/旧入口为开启前历史，App.xaml现已切到Task034Window。

- 专属目录：`WpfBae/Exercises/TASK-034-monitor-commands/`。
- 实际窗口：[Task034Window.xaml](../../WpfBae/Exercises/TASK-034-monitor-commands/Task034Window.xaml)、[Task034Window.xaml.cs](../../WpfBae/Exercises/TASK-034-monitor-commands/Task034Window.xaml.cs)。
- 复用数据类：[TemperatureMonitorViewModel.cs](../../WpfBae/Exercises/TASK-034-monitor-commands/TemperatureMonitorViewModel.cs)，教练迁入原样例/位置/追加与清空算法，提供IsSampling存储、两个命令对象及公开getter，原集合/状态计算/属性绑定保持。
- 新命令类：[AppendTemperatureCommand.cs](../../WpfBae/Exercises/TASK-034-monitor-commands/AppendTemperatureCommand.cs)、[ClearTemperatureHistoryCommand.cs](../../WpfBae/Exercises/TASK-034-monitor-commands/ClearTemperatureHistoryCommand.cs)，使用项目已有ICommand普通完整写法，基础通知由教练提供，核心执行/条件留TODO。
- 配置类：[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-034-monitor-commands/AlarmThresholdSettings.cs)，独立命名空间与`AppContext.BaseDirectory/Exercises/TASK-034-monitor-commands/alarm-threshold.json`，不复制或改写TASK-033原配置。
- 共享例外：[App.xaml](../../WpfBae/App.xaml)仅切换StartupUri，[项目文件](../../WpfBae/WpfBae.csproj)仅增加本题Verification编译排除。
- 安全占位：添加命令CanExecute固定true但Execute为空，清空命令CanExecute暂false且Execute为空；旧按钮Click与原清空禁用路径暂保留，供逐步替换，当前原功能可用不代表命令已完成。

| TODO | 实践对应 | DoD对应 | 内容 |
| :--- | :--- | :--- | :--- |
| 1a/1b | 实践1 | 1 | 添加命令Execute调用原VM.AppendNextSample一次，AddButton删除Click并绑定AddCommand。 |
| 2a/2b/2c | 实践2 | 1/2 | 清空条件读取VM.IsSampling、Execute检查条件后调用ClearHistory，ClearButton删除Click并绑定ClearCommand。 |
| 3 | 实践2 | 2 | IsSampling setter保存状态后调用_clearCommand.NotifyCanExecuteChanged。 |
| 4 | 实践2 | 2/3 | 窗口UpdateSamplingControls同步实际定时器状态到VM.IsSampling，移除两处ClearButton.IsEnabled直接赋值。 |
| 5a/5b | 实践1/2/3 | 1/2 | 命令连接后删除窗口两处旧Click方法，定时器保留原共享AppendNextSample入口。 |

第一步只做TODO 1：在AppendTemperatureCommand.Execute里写_viewModel.AppendNextSample();，在XAML的AddButton上删除Click="AddButton_Click"并添加Command="{Binding AddCommand, Mode=OneWay}"，不能保留两条追加路径。
AddCommand是当前ViewModel公开属性，返回教练创建的那个追加命令对象；Button.Command让WPF调用它的Execute，命令保存的_viewModel仍是表格使用的同一个数据对象。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，停止状态点一次添加，预期只有一行24、Count为1，再点一次为26.5；先确认这一步再做清空。
这条学员运行命令本轮未执行，本轮实际执行的是下方独立验证项目与原exe脚本。
必要说明见[Command Q2](../questions/wpf-commands.md)，先具体说明按钮到命令的对象连接，再说明CanExecute/Execute/IsSampling赋值与CanExecuteChanged通知，不用未解释的简写代替说明。

### 实际骨架验证

- `dotnet build WpfBae/WpfBae.csproj --nologo`退出0、0错误、10条既有CS8600报告，TASK-030至034各配置加载声明在临时/正式编译各报告一次，未修改原作品消除历史警告。
- `dotnet run --project WpfBae/Exercises/TASK-034-monitor-commands/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-034/startup`退出0，74项骨架检查全通过；[结果](./evidence/TASK-034/startup/results.json)。
- 真实窗口检查包含两个命令保存唯一ViewModel、命令安全占位未执行、按钮尚未Command连接、原Click连续五条/完整表格和Count/最新三点/CSV组行、真实定时第6条26.5和再手动第7条25共用样例位置、原空历史/采中导出保护、阈值与缺失配置、清空复位和关闭、默认960×960/最小860×920边界。
- `& WpfBae/Exercises/TASK-034-monitor-commands/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-034/startup/launch.json`退出0，原exe标题TASK-034，正常启动关闭退出0；[启动记录](./evidence/TASK-034/startup/launch.json)。
- 已实际查看[最小尺寸图](./evidence/TASK-034/startup/minimum.png)，结果/图表/表格可读；[默认图](./evidence/TASK-034/startup/default.png)保留。
- 原TASK-033源码未改，本轮未重新交互CSV保存窗口，不把原功能迁移可运行或占位检查当成命令验收；三个DoD未勾选，不新增自主掌握、典型错误或下一题。