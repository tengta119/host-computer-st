# TASK-035：把阈值输入与应用移到ViewModel

- 阶段：第二阶段：WPF（Binding、Command与MVVM整合）。
- 状态：🟡 进行中
- 计划来源：[计划原文](../plan.md)中Binding、Command、MVVM与独立设备监控界面目标。
- 选择依据：TASK-034已验证追加/清空命令及状态同步，接着整理当前仍由窗口读取输入框并验证的阈值应用，让输入、有效参数和反馈由同一ViewModel管理，巩固双向绑定与命令。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

阈值输入文字通过绑定进入当前ViewModel，点击应用通过Command验证并更新当前有效阈值和报警文字。
输入框文字和已经生效的阈值仍是两个不同的数据，只有合法输入成功应用后才改变有效阈值。
配置保存/加载文件和保存CSV窗口可暂留窗口，但读取或恢复的有效阈值须与ViewModel保持同一来源，不保留两份互相不一致的有效字段。
继续沿用20至30含端点、拒绝NaN和无效文字、已有读数即时重判的规则，不增加新参数或新框架。

## 开启时准备约定

目前仅创建卡片，未准备源码或切换入口，App.xaml保留TASK-034。
明确开启后读取当前代码与属性/绑定/报警/配置问答，准备独立目录，保留学员作品。
先具体说明输入文字、有效数值与反馈三个属性分别给谁使用，再解释TwoWay、输入写回时机和应用命令，不能用未展开的新术语代替连接过程。
原验证算法和已完成追加/清空功能优先复用，教练准备必要类与签名、安全占位，核心输入绑定、应用命令和有效值同步留TODO。
首步只观察输入文字是否进入同一个ViewModel，不改变有效阈值或写文件；不以口试作为继续实践门槛。

## 实践与产出

1. 将阈值输入文字、有效数值与反馈集中到当前ViewModel，接通输入和显示绑定。
2. 应用按钮通过Command执行原有效性验证，成功后立即刷新已有温度报警。
3. 调整原配置保存/恢复连接，验证输入未应用与无效输入不改变有效参数，保持原采集/清空/导出行为。

## 验收标准（DoD）

- [ ] 输入框通过Binding写回同一个ViewModel，应用按钮通过Command验证并更新有效阈值；20/30端点及26.5等合法值可应用，空白/非数字/范围外/NaN被拒绝，失败保留原有效值，反馈和有效值显示正确。
- [ ] 同一条26.5在应用26/27后立即超限/正常，采集中应用也即时更新而不追加或清空历史；未应用的输入和拒绝输入不影响保存值，保存及重启恢复仍使用同一有效参数来源。
- [ ] 原追加/清空命令与启停保护、同集合表格/Count/最新三点、CSV选择路径/取消/覆盖/写入失败恢复保持，配置缺失/无效处理保留原规则，默认及约定最小尺寸可读，实际窗口正常关闭。

## 状态与证据

- 2026-10-17：TASK-034三个DoD通过后仅创建本卡，未准备代码、切换入口或运行本题验收。

## 2026-10-17明确开启：实际骨架与首步

当前状态🟡 进行中，上方仅建卡片/未准备代码/旧入口为开启前历史，App.xaml现已切到Task035Window。

- 专属目录：`WpfBae/Exercises/TASK-035-threshold-viewmodel/`。
- 实际窗口：[Task035Window.xaml](../../WpfBae/Exercises/TASK-035-threshold-viewmodel/Task035Window.xaml)、[Task035Window.xaml.cs](../../WpfBae/Exercises/TASK-035-threshold-viewmodel/Task035Window.xaml.cs)。
- 数据类：[TemperatureMonitorViewModel.cs](../../WpfBae/Exercises/TASK-035-threshold-viewmodel/TemperatureMonitorViewModel.cs)，复用原集合/样例/追加清空命令/状态与通知，教练新增ThresholdInputText、AlarmThreshold、CurrentThresholdText、ThresholdFeedbackText完整属性与通知，以及新应用方法安全占位。
- 新命令：[ApplyThresholdCommand.cs](../../WpfBae/Exercises/TASK-035-threshold-viewmodel/ApplyThresholdCommand.cs)，CanExecute固定允许尝试应用，Execute留TODO，传入原VM不创建第二份。
- 原命令复用：[AppendTemperatureCommand.cs](../../WpfBae/Exercises/TASK-035-threshold-viewmodel/AppendTemperatureCommand.cs)、[ClearTemperatureHistoryCommand.cs](../../WpfBae/Exercises/TASK-035-threshold-viewmodel/ClearTemperatureHistoryCommand.cs)，保持已验证清空条件取反。
- 配置复用：[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-035-threshold-viewmodel/AlarmThresholdSettings.cs)，独立命名空间与`AppContext.BaseDirectory/Exercises/TASK-035-threshold-viewmodel/alarm-threshold.json`，不复制或改写TASK-034原配置。
- 共享例外：[App.xaml](../../WpfBae/App.xaml)仅切换StartupUri，[项目文件](../../WpfBae/WpfBae.csproj)仅增加本题Verification编译排除。
- 原窗口_alarmThreshold已由教练改为转交VM.AlarmThreshold的兼容属性，不再另存一份有效值；旧应用验证/显示和配置控件赋值暂留作迁移参考，全部接通后由TODO 4/5移除。

| TODO | 实践对应 | DoD对应 | 内容 |
| :--- | :--- | :--- | :--- |
| 1a/1b | 实践1首步 | 1准备 | 输入框Text双向绑定ThresholdInputText并即时写回，观察TextBlock单向绑定同一属性，不应用。 |
| 2a/2b | 实践2 | 1 | 新应用命令Execute调用VM.ApplyThreshold，应用按钮改Command并移除Click。 |
| 3 | 实践2 | 1/2 | 将旧验证迁入VM.ApplyThreshold，失败保留有效值，成功更新AlarmThreshold/报警/反馈，不改历史。 |
| 4a/4b/4c | 实践2/3 | 1/2 | 绑定有效显示和反馈，删除旧Click及UpdateThresholdDisplay和调用，避免直接Text赋值覆盖绑定。 |
| 5a/5b/5c | 实践3 | 2/3 | 保存/报警直接使用VM有效值，加载合法值恢复VM有效值与输入属性、移除控件直接赋值，最后删除窗口兼容属性。 |

第一步只做TODO 1：在XAML的ThresholdInput把Text="27"替换为Text="{Binding ThresholdInputText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"；把新增ThresholdInputObservationText的Text改为Text="{Binding ThresholdInputText, Mode=OneWay}"。
输入框是可编辑TextBox，TwoWay让输入变化写回同一VM，PropertyChanged让每次文字变化立即写回；观察控件是只显示的TextBlock，所以OneWay读取收到的文字即可。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，输入29但不点应用或保存，预期观察文字立即为29、当前有效阈值仍为27；再输入abc，观察文字显示abc而有效值仍为27。
这条学员运行命令本轮未执行，本轮实际执行的是下方独立验证项目和原exe脚本；本题新配置初次缺失时默认27，不沿用TASK-034用户配置。
必要说明见[MVVM Q3](../questions/wpf-mvvm.md)，按输入文字/有效数值/反馈对象、写回时机、命令验证与配置恢复逐步展开，不要求先完成后续TODO才能观察首步。

### 实际骨架验证

- `dotnet build WpfBae/WpfBae.csproj --nologo`两次实际构建均退出0、0错误、12条既有CS8600报告，TASK-030至035配置加载声明在临时/正式编译各报告一次，未修复历史警告。
- `dotnet run --project WpfBae/Exercises/TASK-035-threshold-viewmodel/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-035/startup`退出0，74项骨架检查全通过；[结果](./evidence/TASK-035/startup/results.json)。
- 检查包含唯一VM有效数值与旧兼容入口同步、新应用命令同对象/安全占位、输入绑定尚未实现，验证进程临时接通首步后29进入VM和观察文字且有效26不变/不写文件，随后撤销；原追加/清空命令、五条原集合/表格/Count/三点/CSV组行、采中导出保护、原旧Click阈值应用与缺失配置、清空关闭和布局通过。
- 临时绑定仅存在于验证进程，不写入学员源码，不算学员完成；本轮未重新交互真实CSV保存窗口或验收本题命令。
- `& WpfBae/Exercises/TASK-035-threshold-viewmodel/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-035/startup/launch.json`退出0，原exe标题TASK-035，正常启动关闭退出0；[启动记录](./evidence/TASK-035/startup/launch.json)。
- 已实际查看[最小尺寸图](./evidence/TASK-035/startup/minimum.png)，新增输入观察行可读，默认960×960/最小860×920边界通过；[默认图](./evidence/TASK-035/startup/default.png)保留。
- 原TASK-034作品未改，三个DoD未勾选，不新增自主掌握、典型错误或下一任务，原C#/WPF待复习范围保持。