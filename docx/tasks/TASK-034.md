# TASK-034：用Command连接追加与清空操作

- 阶段：第二阶段：WPF（Command与MVVM整合）。
- 状态：✅ 已通关
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

- [x] 两个按钮使用当前同一个ViewModel的Command，实际按钮执行不再走旧追加/清空Click，连续手动五条、自动与手动混合追加均只追加一次且顺序正确，表格/Count/最新三点来自同一历史。
- [x] 采集中清空命令不可执行且按钮禁用，直接请求执行也不清空；停止后恢复可执行，实际清空后Count为0、报警暂无温度、图表为空，下一条从24开始，重复启停不重复登记或追加。
- [x] 原有效阈值即时刷新、配置保存恢复、CSV选择位置/取消/覆盖/写入失败恢复保持，导出不改数据，默认及约定最小尺寸可读，实际窗口正常关闭。

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
## 2026-10-17首次实际审查

当前状态🟡 进行中；93项实际检查86通过、7失败，失败均由清空CanExecute方向相反引起；DoD1/3通过、2未通过，不创建下一任务，上方命令未实现为开启时历史。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 通过 | 两按钮Command引用原ViewModel命令，旧Click方法已删除；实际按钮通过WPF自动化调用触发命令，手动五条完整值和顺序正确，采集中手动仅追加一次，原定时器继续追加，共用集合/样例位置，表格/Count/三点正确。 |
| 2 | 未通过 | 停止时CanExecute为false、按钮禁用、直接Execute提前返回无法清空；采集中CanExecute为true且按钮可点，与要求相反。VM.IsSampling同步和CanExecuteChanged通知已正确，原ClearHistory内部保护使运行直接请求仍不清空；停止后的命令清空/复位无法通过，保留待复查。 |
| 3 | 通过 | 参数即时报警/恢复、配置字节保留与新窗口恢复、真实保存窗口两目录导出/取消/拒绝与接受覆盖/de-DE格式、原写入方法占用和目录失败/重选恢复、默认和最小布局/原exe正常关闭通过；后续场景准备使用已提供底层ClearHistory重置，不把它算作清空命令通过。 |

唯一核心修复位置为[ClearTemperatureHistoryCommand.cs](../../WpfBae/Exercises/TASK-034-monitor-commands/ClearTemperatureHistoryCommand.cs)第18行，当前return _viewModel.IsSampling把“正在采集”直接当成“允许清空”。
IsSampling在采集中为true，而CanExecute应返回false；停止时IsSampling为false，CanExecute应返回true，所以返回值需要取反。
首步只将该返回表达式改为`return !_viewModel.IsSampling;`，也可以用完整比较`return _viewModel.IsSampling == false;`，其他已通过代码保持。
感叹号是C#的布尔取反运算，在这里把true变false、false变true，不改变IsSampling保存的状态。
不能通过交换IsSampling赋值、删除通知或移除ClearHistory内部保护修复，已验证这些部分正确；运行状态与按钮允许条件承担不同用途。
本轮未修改学员核心代码或文案，Q2继续待复习，已通过的追加/命令连接/通知同步局部能力保留，原Q1掌握范围不变。

### 实际命令与证据

- `dotnet build WpfBae/WpfBae.csproj --nologo`退出0、0错误、10条既有CS8600报告，未修复历史配置可空声明。
- `dotnet run --project WpfBae/Exercises/TASK-034-monitor-commands/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-034/review-01`退出1，93项86通过、7失败，无未处理异常；[结果](./evidence/TASK-034/review-01/results.json)。
- 两个命令按钮通过ButtonAutomationPeer/IInvokeProvider调用实际OnClick和Command路径，未以单纯RaiseEvent当作命令执行证据；禁用时按实际界面不触发。
- 为继续独立验证原导出与报警，记录清空失败后用已提供的VM.ClearHistory仅重置后续数据，不修补CanExecute、不切换命令或按钮绑定，相关准备在结果与验证源码中单列。
- 写入失败通过反射调用原WriteCsvToPath注入独占文件及目录冲突，后续恢复经过真实保存窗口；两段异常日志由原catch捕获，不是程序崩溃。
- `& WpfBae/Exercises/TASK-034-monitor-commands/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-034/review-01/launch.json`退出0，原exe标题TASK-034，启动关闭退出0；[启动记录](./evidence/TASK-034/review-01/launch.json)。
- [源码与既有文件核对](./evidence/TASK-034/review-01/verification.json)：88项哈希未变，原/审查DLL一致；[基线](./evidence/TASK-034/review-01/before.json)，原配置与CSV未动。
- 已实际查看[最小布局图](./evidence/TASK-034/review-01/minimum.png)，默认960×960和最小860×920边界可读，停止时清空按钮呈禁用也可见；[默认图](./evidence/TASK-034/review-01/default.png)保留。
## 2026-10-17最终复查通关

学员已自主将清空CanExecute改为return !_viewModel.IsSampling，最终105项实际检查全通过，三个DoD全部勾选，状态✅ 已通关；上方失败/进行中为修复前历史。

- DoD1通过：两个按钮实际使用原VM命令，旧Click已删除，手动五条/运行中手动单次/定时追加共享序列正确；重复启动没有额外追加，历史条数等于手动次数加实际Tick次数，表格/Count/三点共用同一原历史。
- DoD2通过：初始和停止CanExecute为true、按钮可用，采集中CanExecute为false且按钮禁用；直接请求执行仍不清空，停止后命令实际清空，Count0/暂无温度/空图/集合引用保留，超限后清空及恢复24正确，重复启停不重复登记或追加。
- DoD3通过：阈值即时刷新、配置字节保留与新窗口恢复、真实保存窗口两目录/取消/拒绝与接受覆盖/de-DE格式、写入方法失败与重新选择恢复、默认最小布局与关闭收尾保持。
- 本次最终验证已删除首次审查用于准备后续场景的底层清空回退，只使用实际按钮命令或直接命令Execute清空，不以诊断方法代替学员实现。
- 实际执行`dotnet build WpfBae/WpfBae.csproj --nologo`退出0、0错误、10条既有CS8600报告，未消除历史配置可空声明警告。
- 实际执行`dotnet run --project WpfBae/Exercises/TASK-034-monitor-commands/Verification/TaskReviewFinal.csproj -- docx/tasks/evidence/TASK-034/review-final`退出0，105项全通过；[最终结果](./evidence/TASK-034/review-final/results.json)、[验证源码](../../WpfBae/Exercises/TASK-034-monitor-commands/Verification/FinalReview.cs.txt)。
- 写入失败通过反射调用原WriteCsvToPath注入，恢复经过真实保存窗口，预期异常由原catch捕获；实际命令按钮通过WPF ButtonAutomationPeer/IInvokeProvider调用。
- 实际执行`& WpfBae/Exercises/TASK-034-monitor-commands/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-034/review-final/launch.json`退出0，原exe标题TASK-034，正常启动关闭退出0；[启动记录](./evidence/TASK-034/review-final/launch.json)。
- [核对结果](./evidence/TASK-034/review-final/verification.json)：88项源码和既有数据哈希未变、原/审查DLL一致，本轮未代改源码或原用户文件；[基线](./evidence/TASK-034/review-final/before.json)。
- 已实际查看[最小尺寸图](./evidence/TASK-034/review-final/minimum.png)，停止后清空按钮可用，真实路径/表格/图表可读，默认960×960与最小860×920边界检查通过；[默认图](./evidence/TASK-034/review-final/default.png)保留。
- [Command Q2](../questions/wpf-commands.md)按当前骨架的按钮命令连接、原追加/清空调用、清空条件取反、状态同步与条件通知的自主应用标已掌握；命令类/接口签名/事件通知基础与样例算法迁移由教练提供，不扩大为完整架构独立设计。
- 只创建[TASK-035：把阈值输入与应用移到ViewModel](./TASK-035.md)（⚪ 未开始），依据计划Binding/Command/MVVM目标继续整合有效参数，未准备源码或切换入口，App.xaml仍TASK-034，阶段仍进行中。