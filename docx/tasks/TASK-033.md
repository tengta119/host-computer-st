# TASK-033：把温度历史与报警显示移到ViewModel

- 阶段：第二阶段：WPF（MVVM与监控界面整合）。
- 状态：✅ 已通关
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

- [x] 窗口使用同一个ViewModel，表格与采集/清空/导出读取同一份完整历史，连续五条Count为5，表格五行，图中最新三点，清空与恢复正确。
- [x] Count与报警文字通过Binding取得ViewModel属性，新增/清空/改变有效阈值后显示立即正确，运行路径不再直接给这两处文字控件赋结果，空历史不访问最后一项。
- [x] 原启停与运行清空保护、配置保存恢复、CSV选择位置/取消/覆盖确认/写入失败恢复保持，导出不改变数据，默认及约定最小布局可读，实际窗口正常关闭。

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
## 2026-10-17实际审查与最终通关

当前状态✅ 已通关，三个DoD均通过；首次97项94通过、三项失败均为空历史报警未复位，教练按既有状态文案协助授权补一行后，最终97项全通过，原失败/未开始为历史。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 通过 | DataContext为窗口唯一_viewModel，窗口兼容引用/VM.Temperatures/表格ItemsSource同一引用；完整五条顺序正确，Count为5，表格全部行，图表最新三点，清空与恢复正确。 |
| 2 | 通过 | 两个公开属性实际发出正确名称的PropertyChanged通知，绑定界面跟随属性变化；初始/五条后/全操作后绑定均存活且来源为原ViewModel；窗口执行代码无两处Text直接赋值，新增/阈值改变/清空均更新，空集合安全。 |
| 3 | 通过 | 真实采集/停止等待/清空按钮及方法保护、参数即时报警与恢复、配置字节保留和重开恢复、两个目录真实保存窗口导出/取消/覆盖确认/de-DE格式、写入方法失败与另选路径恢复、默认/最小布局与实际关闭均通过。 |

### 协助范围与自主能力

学员已自主完成DataContext/表格ItemsSource/两处文字Binding、RefreshDisplay条数与最新温度判断、两个setter通过nameof传入正确属性名通知，以及窗口刷新入口转交同一ViewModel并删除旧Count直接赋值。
首次空集合分支只return，虽然Count已更新为0，但原报警文字仍保持正常或超限；教练仅在return前补`AlarmText = "暂无温度";`，不要求学员为一处状态文案反复提交。
该复位行不计自主实现，其余核心逻辑保留，未把本次状态文字收尾新增为学员典型错误。
原ViewModel类/集合创建/兼容引用/属性骨架和通知辅助方法由教练准备，不将本次通过扩大为独立完整MVVM架构设计，原Q1和C#/WPF待复习范围保持。

### 实际命令与证据

- 审查前保存[82项源码与既有数据基线](./evidence/TASK-033/review-01/before.json)及[原ViewModel](./evidence/TASK-033/review-01/TemperatureMonitorViewModel-before.cs.txt)。
- 实际执行`dotnet build WpfBae/WpfBae.csproj --nologo`，初次增量输出0警告/错误；补复位文字后构建退出0、0错误、8条CS8600报告，均为TASK-030/031/032/033原加载声明在临时/正式编译重复报告，未消除历史警告。
- 实际执行`dotnet run --project WpfBae/Exercises/TASK-033-monitor-viewmodel/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-033/review-01`退出1，共97项、94通过，三个失败为正常清空/超限清空/空历史改阈值后的暂无温度复位；[首次结果](./evidence/TASK-033/review-01/results.json)。
- 实际执行`dotnet run --project WpfBae/Exercises/TASK-033-monitor-viewmodel/Verification/TaskReviewFinal.csproj -- docx/tasks/evidence/TASK-033/review-final`退出0，最终97项全通过；[最终结果](./evidence/TASK-033/review-final/results.json)。
- 验证项目运行原编译Task033Window，未在进程中补绑定代替学员实现；保存窗口通过实际控件确认、取消、拒绝/接受覆盖，两种失败用反射调用原WriteCsvToPath注入，随后走真实保存窗口恢复，两段异常是原catch内的预期日志。
- 实际执行`& WpfBae/Exercises/TASK-033-monitor-viewmodel/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-033/review-final/launch.json`退出0，原exe标题TASK-033，正常启动关闭退出0；[启动记录](./evidence/TASK-033/review-final/launch.json)。
- [核对结果](./evidence/TASK-033/review-final/verification.json)：82项中仅ViewModel文件变化，对应上述复位一行；其余源码/既有用户CSV和配置未变，原/审查DLL一致。
- 已实际查看[最小尺寸图](./evidence/TASK-033/review-final/minimum.png)，绑定结果与路径、表格和图表可读，默认960×960/最小860×920边界通过；[默认图](./evidence/TASK-033/review-final/default.png)保留。
- [MVVM Q2](../questions/wpf-mvvm.md)按本骨架同对象绑定/状态更新/属性通知调用/窗口入口转交的自主范围标已掌握，复位协助单列；当前25分类/60条、待复习32/已掌握28。
- 依据计划Command/MVVM与独立监控界面目标，仅创建[TASK-034：用Command连接追加与清空操作](./TASK-034.md)（⚪ 未开始），未准备源码或切换入口；App.xaml保留TASK-033，阶段仍进行中。