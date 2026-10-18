# TASK-036：选择模拟设备并切换监控历史

- 阶段：第二阶段：WPF（设备选择与监控界面整合）。
- 状态：✅ 已通关

2026-10-18最终复查已通过全部三个必需DoD，235项检查全部通过；下方开启时及首次审查的进行中、未完成和失败描述均为保留的历史。
- 计划来源：[计划原文](../plan.md)中的ComboBox、Binding、MVVM与带设备列表的监控界面目标。
- 选择依据：TASK-035已验证同一ViewModel的历史、命令和有效阈值，下一步复用早期设备选择经验，把设备选择与当前完整监控界面结合，练习切换集合来源及保留各设备数据。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

窗口可选择TEMP-001和TEMP-002两个模拟设备，每个设备保留自己的历史和下一条样例位置，停止时切换设备立即显示其表格、Count、报警和最新三点。
追加、采集、清空和CSV导出都作用于当前设备，不影响另一设备已经保存的历史。
采集中禁止切换设备，停止后恢复；同一有效报警阈值继续供两个设备使用，配置只保存这个公共阈值，不新增设备配置或真实通信。
CSV格式仍为Sequence,TemperatureC，不新增列，导出当前设备全部历史，成功结果能识别当前设备及实际文件路径。

## 开启时准备约定

目前只创建本卡，未准备源码或切换入口，App.xaml保留TASK-035。
明确开启后读取当前代码、设备选择/集合通知/MVVM问答，再准备独立任务目录，保留原作品。
先解释每台设备的数据对象具体保存什么、选择属性指向谁、表格如何改读当前设备的集合，再解释集合内容变化和集合来源更换需要不同通知。
教练准备必要设备数据类/签名与安全连接骨架，核心选择绑定、设备数据访问与切换刷新留TODO，首步只观察选择的设备编号，不启动采集或改历史。
窗口绘图若订阅了旧集合，切换时解除旧集合再登记新集合的连接由教练先解释并提供可行骨架，不把未知框架入口当成学员额外步骤。
不引入新框架、数据库或通信依赖，先沿用原24、26.5、25、28样例，每台设备独立从24开始并记录自己的位置。

## 实践与产出

1. 为两台模拟设备分别保存历史和样例位置，接通设备选择与当前设备显示。
2. 切换时同步表格、Count、报警和图表，原命令读取当前设备，保持采集中禁止切换。
3. 实际验证两台设备数据不串、采集/清空/导出目标正确以及原参数与配置保持。

## 验收标准（DoD）

- [x] 默认TEMP-001，给它添加五条后切到TEMP-002显示空历史、Count0/暂无温度/空图；给TEMP-002添加两条后切回，TEMP-001五条及最新三点恢复，各自继续追加的样例位置正确，切换不自行追加或清空。
- [x] 当前设备的手动/自动追加、清空和CSV导出操作同一当前历史，另一设备数据不变；采集中选择控件禁用且切换入口有保护，停止后恢复切换，反复切换/启停不重复订阅或一次追加多次。
- [x] 有效阈值与输入/应用命令、报警即时判断、配置保存恢复规则保持，切换后按公共有效阈值重判；CSV完整历史/固定小数/选择位置/取消/覆盖/写入失败恢复保持，结果能识别当前设备与路径，默认及约定最小布局可读，实际窗口正常关闭。

## 状态与证据

- 2026-10-18：TASK-035三个DoD通过后仅创建本卡，未准备代码、切换入口或运行本题验收。

## 2026-10-18明确开启：实际骨架与首步

当前状态🟡 进行中，上方仅建卡片/未准备代码/旧入口为开启前历史，App.xaml现已切到Task036Window。

- 专属目录：`WpfBae/Exercises/TASK-036-device-monitor/`。
- 实际窗口：[Task036Window.xaml](../../WpfBae/Exercises/TASK-036-device-monitor/Task036Window.xaml)、[Task036Window.xaml.cs](../../WpfBae/Exercises/TASK-036-device-monitor/Task036Window.xaml.cs)。
- 新数据类：[DeviceMonitorState.cs](../../WpfBae/Exercises/TASK-036-device-monitor/DeviceMonitorState.cs)，教练提供Id、独立Temperatures、NextSampleIndex完整属性，构造两个已有设备对象。
- 当前监控数据：[TemperatureMonitorViewModel.cs](../../WpfBae/Exercises/TASK-036-device-monitor/TemperatureMonitorViewModel.cs)，教练提供DeviceIds、SelectedDeviceId存储与通知、CurrentDevice和Temperatures按当前对象读取；切换方法、逐设备位置与运行保护留TODO。
- 原功能复用：[AppendTemperatureCommand.cs](../../WpfBae/Exercises/TASK-036-device-monitor/AppendTemperatureCommand.cs)、[ClearTemperatureHistoryCommand.cs](../../WpfBae/Exercises/TASK-036-device-monitor/ClearTemperatureHistoryCommand.cs)、[ApplyThresholdCommand.cs](../../WpfBae/Exercises/TASK-036-device-monitor/ApplyThresholdCommand.cs)、[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-036-device-monitor/AlarmThresholdSettings.cs)。
- 配置仍只保存公共有效阈值，独立路径`AppContext.BaseDirectory/Exercises/TASK-036-device-monitor/alarm-threshold.json`，不复制或改写TASK-035原配置/CSV。
- 共享例外：[App.xaml](../../WpfBae/App.xaml)仅切换StartupUri，[项目文件](../../WpfBae/WpfBae.csproj)仅增加本题Verification编译排除。
- 教练提供集合切换连接：窗口登记VM.PropertyChanged，Temperatures变化时先解除旧集合事件、改指新集合、登记新事件并重画，关闭解除当前集合与VM事件；不把这些框架连接留给学员猜测。
- 安全占位：SelectedDeviceId只存选择字符串，SwitchToSelectedDevice为空，真正CurrentDevice暂001；原追加仍沿用VM过渡位置字段，后续改为每设备位置，首步不操作历史。
- 导出成功文字已由教练带上CurrentDevice.Id及实际路径，CSV内容仍为原两列，不改变格式要求。

| TODO | 实践对应 | DoD对应 | 内容 |
| :--- | :--- | :--- | :--- |
| 1a/1b | 实践1首步 | 1准备 | ComboBox删除SelectedIndex并双向绑定SelectedDeviceId，观察TextBlock单向读取同一编号，先不切换历史。 |
| 2a/2b/2c | 实践2 | 1/2/3 | setter调用切换入口，运行保护后找到已有设备对象并赋_currentDevice，通知CurrentDevice/Temperatures与刷新报警，不new或清空。 |
| 3a → 3b → 3c | 实践2 | 1/2 | 3a修改追加方法的位置读写，3b修改清空时的位置复位，3c最后删除VM共用位置字段。 |
| 4a/4b/4c/4d | 实践2/3 | 2 | 选择setter/切换方法入口保护，CanSelectDevice按真实IsSampling取反，状态改变时通知，ComboBox.IsEnabled绑定该属性。 |

第一步只做TODO1：在DeviceComboBox删除SelectedIndex="0"，添加SelectedItem="{Binding SelectedDeviceId, Mode=TwoWay}"，把SelectedDeviceObservationText的Text改为Text="{Binding SelectedDeviceId, Mode=OneWay}"。
DeviceIds已提供两个字符串选项，改选002应写回同一ViewModel.SelectedDeviceId，观察文字读取这个编号；CurrentDevice是另一个具体设备对象引用，首步未接通真正切换，所以“当前监控”仍001。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，不启动采集、不添加温度，改选TEMP-002，预期观察文字显示TEMP-002，再选回001应跟随；先确认这一小步。
这条学员运行命令本轮未执行，本轮实际执行下方独立验证项目和原exe脚本；背景、代码身份与后续切换机制见[设备选择Q2](../questions/wpf-combobox.md)。

### 实际骨架验证

- `dotnet build WpfBae/WpfBae.csproj --nologo`两次实际构建均退出0、0错误、14条既有CS8600报告，TASK-030至036配置加载声明在临时/正式编译各一次，未修复历史警告。
- `dotnet run --project WpfBae/Exercises/TASK-036-device-monitor/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-036/startup`最终退出0，87项骨架检查全通过；[结果](./evidence/TASK-036/startup/results.json)。
- 首次验证误把已完成应用命令后的有效值预期写成27，实际输入已是26，修正教练验证断言后重新运行；[首轮结果](./evidence/TASK-036/startup/results-initial.json)保留，不记学员错误。
- 实际检查涵盖两台对象独立集合/位置、默认001/同集合、首步未绑定，验证进程临时接通002选择写回/观察后撤销；另临时改当前对象并发属性通知，确认教练窗口连接新集合、旧设备变化不刷新当前视图、切回恢复原记录，最后恢复初始状态。
- 上述临时诊断只存在于验证进程，不写入学员源码，不算学生实现或任务验收；原追加/清空/阈值应用、五条表格Count/三点/CSV组行、空历史与采中导出保护、配置缺失、清空关闭、默认960×1000/最小860×960边界通过。
- `& WpfBae/Exercises/TASK-036-device-monitor/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-036/startup/launch.json`退出0，原exe标题TASK-036，正常启动关闭退出0；[启动记录](./evidence/TASK-036/startup/launch.json)。
- 已实际查看[最小尺寸图](./evidence/TASK-036/startup/minimum.png)，设备栏、标识、表格和图表可读，新增设备栏后默认/最小高度比前题增加40以保持布局；[默认图](./evidence/TASK-036/startup/default.png)保留。
- 原TASK-035作品未改，本轮未重新交互CSV保存窗口；三个DoD未勾选，不新增自主掌握、典型错误或下一题，原C#/WPF复习范围保持。
### 2026-10-18 TODO3顺序澄清

编号已按实际执行顺序修正：TODO3a将AppendNextSample所有位置读写改为CurrentDevice.NextSampleIndex，TODO3b将ClearHistory的复位改为当前设备位置，TODO3c最后删除TemperatureMonitorViewModel.cs的共用_nextSampleIndex字段。
DeviceMonitorState.cs里每台设备各自的_nextSampleIndex字段不能删，其NextSampleIndex属性正是保存独立位置的入口；具体两台设备例子见[设备选择Q2](../questions/wpf-combobox.md)。
本轮仅澄清教练说明并改该TODO注释，没有代改学员核心实现或运行验收，状态和DoD保持。
2026-10-18用户指出编号不应要求跳序执行，教练已直接统一源码注释、任务卡和问答编号为3a追加、3b清空、3c删除，取代此前绕编号的说明；仅修改注释与文档，已核对非注释代码逐字不变，未运行新验收，状态与DoD保持。

## 2026-10-18首次实际审查

当前🟡 进行中；235项实际检查228通过、7失败，分别对应样例位置被固定赋为1，以及选择控件没有连接禁用条件；DoD1/2未通过、3通过，不创建下一题，上方骨架未完成为开启历史。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 未通过 | 默认001、切到002空历史/空图、两台已有数据保留、集合来源和表格/Count/三点切换正确，但001五条为24/26.5/26.5/26.5/26.5，002第三条仍26.5，样例位置未正确递增。 |
| 2 | 未通过 | 当前历史追加/清空/CSV与另一设备隔离、切换入口运行保护、停止恢复、反复切换无重复刷新均通过；CanSelectDevice运行时正确返回false且有通知，但ComboBox未绑定IsEnabled仍可操作，另混合样例顺序与清空后序列受位置错误影响。 |
| 3 | 通过 | 公共有效阈值/即时报警/配置保护恢复正确，切换按当前实际读数重判；两个设备当前实际历史CSV及设备标识/路径、取消/覆盖/写入失败恢复、原参数合法非法输入、默认最小布局/关闭通过；固定样例未出现28造成的超限场景失败归属于DoD1的数据生成问题，不把它说成所有报警业务失败。 |

### 按顺序修复两处

1. TODO3a：[TemperatureMonitorViewModel.cs](../../WpfBae/Exercises/TASK-036-device-monitor/TemperatureMonitorViewModel.cs)第245行当前为CurrentDevice.NextSampleIndex =+ 1，C#把它解析为赋值正数1，每次都将位置固定到1，后面就反复取样例下标1的26.5。
2. 只将该行改为完整普通写法`CurrentDevice.NextSampleIndex = CurrentDevice.NextSampleIndex + 1;`，保留原达到Length回0的判断与清空当前设备位置。
3. TODO4d：[Task036Window.xaml](../../WpfBae/Exercises/TASK-036-device-monitor/Task036Window.xaml)第21行的DeviceComboBox添加`IsEnabled="{Binding CanSelectDevice, Mode=OneWay}"`；VM的条件、状态通知和两处运行入口保护已经正确，不需要交换状态或重写。

=+不是“在原值上加一”，等号右边的+1是正数1；先读取旧值再加一，才会让下标依次变成1、2、3、0。
IsEnabled是这一个WPF选择控件是否可操作的属性，绑定后才会读取原VM的CanSelectDevice，不能因为VM已有false就默认控件自动禁用。
本轮未代改学员代码，保留设备对象/历史隔离、选择绑定、切换通知、独立位置存储、运行保护和原参数导出的已验证局部能力；Q2继续待复习，旧Q1与原任务能力不撤销。

### 实际命令与证据

- `dotnet build WpfBae/WpfBae.csproj --nologo`退出0、0错误、14条既有CS8600报告，未消除历史配置加载声明警告。
- `dotnet run --project WpfBae/Exercises/TASK-036-device-monitor/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-036/review-01`退出1，235项228通过、7失败，无未处理异常；[结果](./evidence/TASK-036/review-01/results.json)、[验证源码](../../WpfBae/Exercises/TASK-036-device-monitor/Verification/TaskReview.cs.txt)。
- 7项失败为001五条固定顺序、002第三条、运行选择控件应禁用、原五条顺序、混合顺序、清空后四条顺序、第四条应28的超限观察；后四项同样由位置固定为1引起。
- 选择通过SetCurrentValue保留SelectedItem绑定并验证VM/当前对象/编号，命令按钮经ButtonAutomationPeer/IInvokeProvider执行；未用反射改设备对象、位置或替学员补绑定。
- 设备001五条后切002添加两条并导出，切回恢复、各自继续、只清空002、公共阈值、8次切换后单次追加一次刷新、实际定时只影响当前设备与运行入口拒绝均实际执行；随后用真实清空命令恢复两台数据，再独立复测原功能。
- [001导出CSV](./evidence/TASK-036/review-01/chosen/A/first.csv)实际包含24和重复26.5，CSV忠实保存实际历史，不能把CSV内容与实际集合一致当作样例顺序正确；[002专属CSV](./evidence/TASK-036/review-01/chosen/B/device002.csv)为当前002完整记录。
- 写入失败通过原WriteCsvToPath方法注入，恢复经过真实保存窗口；参数解析/NaN/写入失败/损坏配置日志由原catch捕获，不是程序崩溃。
- `& WpfBae/Exercises/TASK-036-device-monitor/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-036/review-01/launch.json`退出0，原exe标题TASK-036，正常启动关闭退出0；[启动记录](./evidence/TASK-036/review-01/launch.json)。
- [核对](./evidence/TASK-036/review-01/verification.json)：103项源码和既有数据哈希未变、原/审查DLL一致，本轮未改学员代码、原配置或CSV；[基线](./evidence/TASK-036/review-01/before.json)。
- 已实际查看[最小布局图](./evidence/TASK-036/review-01/minimum.png)，默认960×1000/最小860×960边界可读；[默认图](./evidence/TASK-036/review-01/default.png)保留，结束截图不能代替运行时禁用状态检查。

## 2026-10-18最终实际复查：全部通过

- 学员已将追加位置改为CurrentDevice.NextSampleIndex = CurrentDevice.NextSampleIndex + 1，并将DeviceComboBox.IsEnabled单向绑定CanSelectDevice；本轮未代改学员源码。
- DoD1通过：001五条24/26.5/25/28/24，002初始空历史与空图，002两条后切回001仍五条和最新三点，各自继续样例顺序正确，切换不改历史。
- DoD2通过：手动/真实定时器/清空/真实保存窗口导出针对当前设备，另一台保持；运行时选择框禁用且方法拒绝切换，停止恢复，重复切换和启停不重复追加或通知。
- DoD3通过：公共有效阈值、合法/非法输入、即时报警与切换重判、配置保存/合法恢复/非法配置保护保持；真实CSV保存窗口两目录/取消/覆盖拒绝与接受、完整历史/固定小数、写入方法失败注入后重新选择恢复均通过，导出成功文字包含设备和路径。
- 实际命令dotnet build WpfBae/WpfBae.csproj --nologo退出0，0错误、14条既有CS8600报告，未修复历史可空声明警告。
- 实际命令dotnet run --project WpfBae/Exercises/TASK-036-device-monitor/Verification/TaskReviewFinal.csproj -- docx/tasks/evidence/TASK-036/review-final退出0，235项全通过，见[结果](./evidence/TASK-036/review-final/results.json)；非法输入、文件冲突和损坏JSON测试产生的是预期捕获输出，不是进程崩溃。
- 实际命令& WpfBae/Exercises/TASK-036-device-monitor/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-036/review-final/launch.json退出0，原exe实际启动TASK-036并正常关闭退出0，见[记录](./evidence/TASK-036/review-final/launch.json)。
- 原103项源码与既有数据哈希不变，审查副本与原DLL一致，见[核对](./evidence/TASK-036/review-final/verification.json)；验证数据位于独立证据目录，不改既有作品或配置。
- 默认960×1000和约定最小860×960布局边界检查通过，已实际查看[最小尺寸图](./evidence/TASK-036/review-final/minimum.png)，必要控件、表格和曲线可读，[默认图](./evidence/TASK-036/review-final/default.png)保留。
- 设备选择Q2按本骨架内选择绑定、对象查找与通知、独立样例位置和运行保护/禁用连接的自主应用标已掌握，教练提供的数据类、集合订阅切换与绘图不计新的自主设计，完整WPF阶段仍未宣称完成。
- 同步任务卡、索引、学习进度、问答和错题；仅创建下一张[TASK-037](./TASK-037.md)⚪ 未开始，按计划的独立监控界面目标练自主区域布局，未准备源码或切换入口。