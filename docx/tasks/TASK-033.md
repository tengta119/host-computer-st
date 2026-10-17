# TASK-033：把温度历史与报警显示移到ViewModel

- 阶段：第二阶段：WPF（MVVM与监控界面整合）。
- 状态：🟡 进行中
- 计划来源：[计划原文](../plan.md)中Binding、ObservableCollection、MVVM与能够自己编写设备监控界面的目标。
- 选择依据：TASK-032已验证选择位置导出，当前窗口继续直接管理温度集合并给Count与报警文字赋值；复用此前绑定和ViewModel实践，把同一份监控数据与显示状态集中管理，减少功能累积后的窗口职责。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

窗口仍显示完整温度历史、真实条数和最新温度报警，改由一个ViewModel保存原温度集合并提供条数/报警文字，界面通过绑定取得这些内容。
定时器、文件保存窗口与绘图可以暂留窗口，不要求本题一次改成完整MVVM，也不新增框架或第三方库。
本题只整理已有数据与显示连接，不增加设备通信或新的业务功能。

## 开启时准备约定

目前仅建本卡，未准备源码或切换入口，App.xaml保留TASK-032。
明确开启后读取当前代码和属性/绑定/集合/MVVM问答，准备独立任务目录，保留学员TASK-032作品。
先具体解释自定义ViewModel类保存什么、窗口如何创建这个对象、表格和文字从哪里取数据，再解释DataContext与属性通知。
对get/set、value和通知使用完整普通写法，不因以前骨架出现过而默认全部掌握。
教练准备必要类型/接口/签名与安全连接占位，核心集合引用、状态更新和绑定留TODO；首步仅让表格绑定同一份原历史，添加一条显示一行。
复用已验证的CSV内容、路径选择和配置行为，不把教练新写的辅助类算为学员自主设计。

## 实践与产出

1. 将完整原历史集中到一个ViewModel，表格与追加/清空使用同一集合。
2. 用绑定显示真实条数和报警文字，在历史或阈值变化时更新相关属性并通知界面。
3. 实际验证自动采集、清空恢复、阈值即时判断与原CSV导出/配置行为保持。

## 验收标准（DoD）

- [ ] 窗口使用同一个ViewModel，表格与采集/清空/导出读取同一份完整历史，连续五条Count为5，表格五行，图中最新三点，清空与恢复正确。
- [ ] Count与报警文字通过Binding取得ViewModel属性，新增/清空/改变有效阈值后显示立即正确，运行路径不再直接给这两处文字控件赋结果，空历史不访问最后一项。
- [ ] 原启停与运行清空保护、配置保存恢复、CSV选择位置/取消/覆盖确认/写入失败恢复保持，导出不改变数据，默认及约定最小布局可读，实际窗口正常关闭。

## 状态与证据

- 2026-10-17：TASK-032三个DoD通过后仅创建本卡，未创建本题源码、切换入口或运行验收。

## 2026-10-17明确开启：实际骨架与首步

当前🟡 进行中，上方仅建卡片/未准备源码/旧入口为开启前历史，App.xaml现已切到Task033Window。

- 专属目录：`WpfBae/Exercises/TASK-033-monitor-viewmodel/`。
- 实际窗口：[Task033Window.xaml](../../WpfBae/Exercises/TASK-033-monitor-viewmodel/Task033Window.xaml)、[Task033Window.xaml.cs](../../WpfBae/Exercises/TASK-033-monitor-viewmodel/Task033Window.xaml.cs)。
- 新数据类：[TemperatureMonitorViewModel.cs](../../WpfBae/Exercises/TASK-033-monitor-viewmodel/TemperatureMonitorViewModel.cs)，教练提供唯一集合、公开getter、两段文字完整get/set、INotifyPropertyChanged基础事件与通知辅助方法，RefreshDisplay与setter通知留TODO。
- 配置复用：[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-033-monitor-viewmodel/AlarmThresholdSettings.cs)，独立命名空间与`AppContext.BaseDirectory/Exercises/TASK-033-monitor-viewmodel/alarm-threshold.json`；不复制或修改TASK-032配置。
- 共享例外：[App.xaml](../../WpfBae/App.xaml)仅切换StartupUri，[项目文件](../../WpfBae/WpfBae.csproj)仅增加本题Verification编译排除。
- 代码连接：窗口_viewModel只new一次，窗口_temperatures在构造方法指向_viewModel.Temperatures，同一集合供原采集/清空/CSV/绘图使用，此兼容连接由教练提供，不算学员自主设计。
- 表格直接ItemsSource赋值已移除，当前尚未绑定，因此添加后图/条数可变化而表格为空；这不是需要额外修复的骨架缺陷，TODO 1就是建立该数据连接。
- 原Count/报警直接赋值临时保留，待学员完成新属性更新/通知/绑定后，在TODO 5移除旧显示；本轮可运行不等于DoD已通过。

| TODO | 实践对应 | DoD对应 | 内容 |
| :--- | :--- | :--- | :--- |
| 1a/1b | 实践1 | 1 | 构造方法设置DataContext为已有_viewModel，XAML的HistoryDataGrid添加ItemsSource绑定Temperatures。 |
| 2 | 实践2 | 1/2 | ViewModel.RefreshDisplay先更新Count文字、再空集合保护与最新温度比较，将结果写入属性。 |
| 3a/3b | 实践2 | 2 | 两个setter存value后调用OnPropertyChanged，分别传对应公开属性名。 |
| 4a/4b | 实践2 | 2 | XAML两个TextBlock.Text分别绑定AlarmText与ReadingCountText。 |
| 5a/5b | 实践2/3 | 2/3 | 移除旧Count直接赋值，将原UpdateTemperatureAlert主体替为调用_viewModel.RefreshDisplay(_alarmThreshold)，复用原变化和阈值入口。 |

第一步只做TODO 1：在Task033Window.xaml.cs构造方法InitializeComponent后写DataContext = _viewModel;，并在XAML的HistoryDataGrid开始标签添加ItemsSource="{Binding Temperatures, Mode=OneWay}"。
_viewModel指向教练已创建的数据对象，DataContext给当前窗口的绑定指定这个来源，表格ItemsSource从对象公开Temperatures属性取得同一份原集合；不需要另new集合或改内部列Binding。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，停止状态添加一条，预期表格一行24；继续添加到五条，表格五行，图中最多三点。
这条学员运行命令本轮未执行，本轮执行的是下方独立验证项目与原exe启动脚本。
必要说明见[MVVM Q2](../questions/wpf-mvvm.md)，先讲对象与首步表格，再讲完整get/set、value、属性通知及原判断迁移，不以口试作为动手门槛。

### 实际骨架验证

- `dotnet build WpfBae/WpfBae.csproj --nologo`退出0、0错误、8条既有CS8600报告（TASK-030/031/032和本题加载声明在临时/正式编译各一次）。
- `dotnet run --project WpfBae/Exercises/TASK-033-monitor-viewmodel/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-033/startup`退出0，71项骨架检查全通过；[结果](./evidence/TASK-033/startup/results.json)。
- 验证包括同一ViewModel集合、表格未连接的预期占位、验证进程临时连接首步后1行/5行正确、三点图与CSV完整组行、空历史/采中拒绝、阈值和配置缺失、默认/最小布局与清空关闭；临时绑定仅存在于验证进程，随后撤销，未写进学员源码，不算学员完成或DoD通过。
- `& WpfBae/Exercises/TASK-033-monitor-viewmodel/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-033/startup/launch.json`退出0，原exe标题TASK-033，正常启动关闭退出0；[启动记录](./evidence/TASK-033/startup/launch.json)。
- 已实际查看[最小布局图](./evidence/TASK-033/startup/minimum.png)，默认960×960/最小860×920布局检查通过，表格留空是首步绑定未实现的可观察状态；[默认图](./evidence/TASK-033/startup/default.png)保留。
- 原TASK-032源码未改，未在本轮重新操作保存窗口确认/取消/覆盖，不把复用代码的历史验收当成本题DoD已通过；三个DoD保持未勾选，不新增自主掌握、典型错误或下一任务。