# TASK-038：独立完成模拟设备监控小项目

- 阶段：第二阶段：WPF（监控功能综合实践）。
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)中的Binding、ObservableCollection、Command、MVVM与能够自己编写设备监控界面的目标。
- 选择依据：TASK-037已经验证学员自行连接界面绑定、安排分行分列和表格伸展，长结果布局由教练协助收尾；下一步把之前分散练过的设备状态、命令、采集与界面刷新在一个独立小项目中串起来，补充完整功能连接的自主证据。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

为TEMP-001与TEMP-002构建一个独立的模拟温度监控窗口，两台设备分别保存历史和下一条样例位置，界面只显示当前设备的数据。
沿用24、26.5、25、28样例与公共报警阈值，完成手动追加、启动/停止、清空、阈值应用、配置和CSV导出的完整操作流程。
允许参考已通关作品，教练继续复用既有绘图坐标、设备数据类、配置类及文件对话框/读写辅助，不增加通信库、数据库、图表框架或新架构。
本题重点是由学员在新窗口和新ViewModel中接通数据来源、属性通知、命令和采集生命周期，并验证这些部分共同工作，不重复要求自行实现未教过的图形或文件框架细节。

## 明确开启后的准备约定

本轮只创建任务卡，不准备代码或切换入口，App.xaml保留TASK-037。
明确开启后先核对已有代码与复习范围，在独立目录`WpfBae/Exercises/TASK-038-monitor-project/`准备最小可运行窗口、必要类型和方法签名。
数据状态与命令连接、设备切换通知、手动/自动追加共享入口、运行保护、阈值应用和XAML绑定留给学员；教练提供的绘图及文件辅助与待实现部分分别标明。
不把完整已连接的旧窗口直接作为新骨架，保留所有旧作品，任务自己的配置和验证数据使用独立目录。
开始前先结合实际对象解释各部分怎样连接，再给第一个可观察的小步骤；TODO按执行依赖顺序编号，不要求跳序操作。

## 实践与产出

1. 先完成窗口持有的一份监控数据对象与设备选择/编号绑定，运行观察001与002切换。
2. 在同一当前设备历史上接通追加和清空命令、条数/报警通知与表格/最新三点显示，再连接原定时器的启停和运行保护。
3. 接通公共阈值输入与应用、配置辅助及当前设备CSV导出，安排可读布局，按固定场景自行运行并整理简短记录。

## 验收标准（DoD）

- [x] 学员自主连接新窗口与一份新监控ViewModel，设备对象与各自集合不串用，001五条和002两条后切回各自恢复数据与样例位置，表格、Count、报警和最新三点同源，切换不额外追加或清空。
- [x] 学员自主接通命令与定时器生命周期，手动和自动共用当前设备追加入口，每次通知只追加一次；重复启停不重复订阅，停止不再追加，运行期间切换/清空/导出受保护，停止恢复操作，关闭停止定时器并解除必要监听。
- [x] 公共阈值合法应用即时刷新、非法输入保留有效值，配置只保存有效值并安全恢复，真实CSV选择位置/取消/覆盖和当前设备完整历史格式正确；实际默认/最小窗口结果及路径可读，提交代码，运行记录按用户要求由教练依据实际验收整理，教练提供/协助与学员自主实现范围分别记录。

## 状态与证据

- 2026-10-18：TASK-037三个必需DoD最终通过后仅创建本卡，尚未创建源码、切换入口或运行本题验收，三个DoD未勾选。
- 本题用于评估自主综合能力，是否完成WPF阶段还须核对计划目标及实际待补内容，不因任务编号或预计剩余数量自动判定阶段完成。

## 2026-10-18明确开启：实际骨架与首步

当前🟡 进行中，上方“仅建卡片/未准备代码/旧入口”为开启前历史，App.xaml现已启动Task038Window，三个DoD均未勾选。

### 本题与前题的区别

TASK-037直接复用了旧ViewModel和命令，学员主要连接界面；本题新建自己的TemperatureMonitorViewModel与三个命令类，核心设备切换、属性通知、追加/清空、阈值应用及定时器启停均留TODO。
界面采用已验证的基本排版作为显示辅助，长配置/导出路径各占下面的位置，不要求重新重复设计上一题布局，也不把这个辅助布局计为本题自主设计。
开始时下拉框是空的，按钮和方法大多没有实际动作，这是已明确标出的安全占位，不代表完整监控功能已实现。

### 实际文件与身份

- 专属目录：`WpfBae/Exercises/TASK-038-monitor-project/`。
- 新窗口：[Task038Window.xaml](../../WpfBae/Exercises/TASK-038-monitor-project/Task038Window.xaml)、[Task038Window.xaml.cs](../../WpfBae/Exercises/TASK-038-monitor-project/Task038Window.xaml.cs)，先编辑后台构造方法里的TODO1，再按顺序接通界面与数据。
- 新数据对象类：[TemperatureMonitorViewModel.cs](../../WpfBae/Exercises/TASK-038-monitor-project/TemperatureMonitorViewModel.cs)，完整名字位于Task038命名空间，不是直接使用TASK-036的同名类；字段、普通属性、初始设备与命令构造及属性通知基础由教练提供，方法中的核心更新和通知留TODO。
- 新命令类：[AppendTemperatureCommand.cs](../../WpfBae/Exercises/TASK-038-monitor-project/AppendTemperatureCommand.cs)、[ClearTemperatureHistoryCommand.cs](../../WpfBae/Exercises/TASK-038-monitor-project/ClearTemperatureHistoryCommand.cs)、[ApplyThresholdCommand.cs](../../WpfBae/Exercises/TASK-038-monitor-project/ApplyThresholdCommand.cs)，接口、构造参数与通知基础由教练预留，执行和清空条件由学员完成。
- 直接复用旧数据结构：[DeviceMonitorState.cs](../../WpfBae/Exercises/TASK-036-device-monitor/DeviceMonitorState.cs)与[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-036-device-monitor/AlarmThresholdSettings.cs)，前者保存每台编号/历史/位置，后者承载文件中的Threshold，不修改旧作品。
- 新运行记录：[RUN-RECORD.md](../../WpfBae/Exercises/TASK-038-monitor-project/RUN-RECORD.md)，目前全部待运行，TODO8时只填实际操作与证据。
- 独立配置路径：`AppContext.BaseDirectory/Exercises/TASK-038-monitor-project/alarm-threshold.json`，不读写TASK-036/037配置。
- 共享例外：[App.xaml](../../WpfBae/App.xaml)只切换StartupUri，[WpfBae.csproj](../../WpfBae/WpfBae.csproj)只增加本题Verification编译排除；未增加框架或包依赖。

### 现在只做TODO1：让下拉框读到已有对象

Task038Window.xaml.cs字段区已经有private readonly TemperatureMonitorViewModel _viewModel = new TemperatureMonitorViewModel();，这行C#用new创建本题数据对象，并把它保存在窗口自定义字段_viewModel中。
这个对象的DeviceIds属性已返回TEMP-001和TEMP-002两个字符串，XAML里的设备选择框已由教练写好ItemsSource="{Binding DeviceIds}"，表示该选择框的选项要从数据对象的DeviceIds读取。
当前窗口尚未设置DataContext，所以这条Binding还没有读取对象，下拉框才会是空的；不是DeviceIds数组没有内容。
DataContext是WPF提供给窗口的属性，设置它后，窗口内这条Binding就能从指定对象读取DeviceIds。
在public Task038Window()构造方法里找到InitializeComponent();后的TODO1，补上下面一行，使用已有字段，不再new另一个对象。

```csharp
DataContext = _viewModel;
```

InitializeComponent是WPF生成的界面初始化方法，前面已经调用；当前改动只给这个窗口设置Binding的数据读取对象，不会启动采集、读设备或写文件。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，预期下拉框从空白变成TEMP-001，展开后有TEMP-001/TEMP-002两个选项，窗口标题为TASK-038。
本步“当前编号待绑定”仍不变，改选也暂时不会切换真实设备；TODO2才接通SelectedDeviceId、CurrentDevice.Id及切换逻辑，不要因此另建第二份VM。
这条dotnet run命令是学员接下来的运行方式，本轮教练没有执行该命令；下方列出实际执行的骨架验证命令。

### 后续TODO按1至8执行

下表是跨文件的执行索引，先完成一组再观察对应现象，不按文件里注释出现的位置跳号。

| TODO | 实际位置 | 实践 / DoD | 内容与可观察结果 |
| :--- | :--- | :--- | :--- |
| 1 | 窗口构造方法。 | 实践1 / DoD1准备。 | DataContext读取已有_viewModel，展开下拉框看见两项。 |
| 2a → 2b | VM的SelectedDeviceId与SwitchToSelectedDevice。 | 实践1 / DoD1/2。 | setter在_selectedDeviceId = value前判断IsSampling，采中return保留原编号，停止时才更新/通知/调用切换；切换找已有设备对象、通知CurrentDevice/Temperatures，不new、追加或清空。 |
| 2c → 2d | XAML设备选择与当前编号。 | 实践1 / DoD1/2。 | 删除SelectedIndex，选择双向写回SelectedDeviceId，编号读取CurrentDevice.Id，改选002显示002。 |
| 3a → 3b | VM显示属性setter与RefreshDisplay。 | 实践2 / DoD1/3。 | 发条数/报警属性通知，按Count、空历史、最新原温度更新显示。 |
| 3c → 3d | VM的AppendNextSample与ClearHistory。 | 实践2 / DoD1/2。 | 每台独立位置循环样例，清空只复位当前设备且运行时拒绝。 |
| 4a → 4b → 4c | 追加命令Execute、清空命令CanExecute/Execute。 | 实践2 / DoD1/2。 | 同一新VM执行一次追加；停止允许清空、采中拒绝，直接Execute也受保护。 |
| 4d → 4e | XAML按钮、报警、条数和表格。 | 实践2 / DoD1/2。 | 连接AddCommand/ClearCommand与同名显示属性/Temperatures，点击追加后表格、Count、曲线同源。 |
| 5a → 5b | VM的IsSampling/CanSelectDevice与窗口UpdateSamplingControls。 | 实践2 / DoD2。 | VM状态改变后通知清空条件与选择条件，窗口把原定时器状态交给VM并显示状态。 |
| 5c → 5d | SamplingTimer_Tick与窗口构造方法。 | 实践2 / DoD2。 | 每次通知只调用一次原VM追加入口，构造方法登记一次Tick。 |
| 5e → 5f | StartButton_Click与StopButton_Click。 | 实践2 / DoD2。 | 操作原定时器并同步状态，重复启动无重复登记，停止保留历史和位置。 |
| 6a → 6b | VM阈值属性/ApplyThreshold与应用命令Execute。 | 实践3 / DoD3。 | 输入/有效值/反馈通知完整；先完成VM验证与应用，再由命令调用，成功立即重判报警。 |
| 6c | XAML阈值输入、应用按钮、有效值和反馈。 | 实践3 / DoD3。 | 输入TwoWay即时写回，命令和显示连接同一VM，未应用输入不改变有效值。 |
| 7a → 7b | 窗口SaveThresholdSettings与ApplyLoadedThreshold。 | 实践3 / DoD3。 | 有效数值传给WriteThresholdToFile，合法加载候选值恢复同一VM并刷新显示。 |
| 7c | 窗口ExportTemperatureHistory。 | 实践3 / DoD2/3。 | 拒绝采中/空历史，选择后只在确认时写当前完整历史，取消与覆盖规则保持。 |
| 8 | RUN-RECORD.md及实际窗口。 | 实践3 / 全部DoD。 | 验证两设备、混合追加、启停保护、阈值配置、真实CSV选择和默认/最小布局，提交实际记录。 |

2c先可连接CanSelectDevice，但其骨架暂返回true以方便先验证停止时切换；5a再根据真实IsSampling实现运行禁用，不能把此占位当成已完成保护。
3b中的RefreshDisplay在窗口集合变化和切换后由已有连接调用，里面使用属性赋值让已绑定文字重读；给字段赋值而遗漏通知会再次出现数据变了、文字不更新的现象。
6a中AlarmThreshold的setter还要通知CurrentThresholdText，因为有效数值改变后，显示文字需要重新读取；输入文字和有效数值仍分开保存，相关具体说明见[MVVM Q3](../questions/wpf-mvvm.md)。
5a使用命令已提供的NotifyCanExecuteChanged，不在外部直接触发命令事件；相关已有例子见[命令Q2](../questions/wpf-commands.md)。
5c/5d要区分普通接收方法和事件登记：仅写SamplingTimer_Tick不会自动收到通知，登记一次后每次Tick才调用它；现有基础解释见[周期通知Q1](../questions/wpf-timer.md)。

### 教练已接好的辅助与边界

窗口只持有一份新VM，三个新命令也保存这同一份VM，构造和公开属性已经预留，学员不需要再造数据对象或集合。
集合变化到绘图、VM通知Temperatures后窗口改接集合的监听、560×300固定坐标图表和单点/最新三条标签由教练复用，学员负责在正确位置发通知并更新原集合。
WriteThresholdToFile(double threshold)是教练从旧配置保存提取的普通方法，参数threshold接收调用者传入的有效数值，内部只负责序列化、创建专属目录、写文件及结果反馈。
LoadThresholdSettings已处理文件缺失、JSON解析、null与范围验证，合法候选值才传到ApplyLoadedThreshold(double threshold)，该方法不是WPF自动功能，而是本题留给学员把合法数值恢复进VM的普通连接方法。
CreateCsvSaveDialog负责创建已配置的保存窗口，BuildCsvText负责从当前历史生成固定小数格式，WriteCsvToPath负责写文件及结果；ExportTemperatureHistory怎样保护运行状态、显示窗口、判断确认/取消和传路径由学员完成。
关闭时停止定时器、解除Tick/集合/VM监听的安全收尾由教练提供，不计学员独立编写；未来学员登记的Tick须使用同一个SamplingTimer_Tick和原定时器，让此收尾能够解除。
Start/Stop/Save/Export按钮的Click入口已提供，Add/Clear/Apply的Command和所有核心数据绑定尚未提供，不能把当前可见按钮当成完整功能已接通。
实际默认尺寸960×1000、约定最小860×960；路径与结果位于竖直容器，可换行，保留名字和现有辅助的连接，不重新给后台直接写入的状态/路径Text添加Binding。

### 实际骨架验证

- 两次实际执行dotnet build WpfBae/WpfBae.csproj --nologo均退出0，0错误、14条既有CS8600报告，来自TASK-030至036的原配置声明，未修改旧文件；[最终构建输出](./evidence/TASK-038/startup-final/build.log)。
- 首次dotnet run --project WpfBae/Exercises/TASK-038-monitor-project/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-038/startup退出0，77项检查全部通过；发现新窗口标题仍沿用前题名称后，教练将标题统一为本卡名称，并将最终运行记录TODO移到XAML末尾，不更改核心实现。
- 随后实际执行dotnet run --project WpfBae/Exercises/TASK-038-monitor-project/Verification/StartupReviewFinal.csproj -- docx/tasks/evidence/TASK-038/startup-final退出0，最终77项检查全部通过，见[最终结果](./evidence/TASK-038/startup-final/results.json)，[初次结果](./evidence/TASK-038/startup/results.json)保留。
- 实际验证初始数据上下文为空、下拉框无选项、命令/表格/显示未绑定、核心追加/清空/应用/启停/保存/导出均留占位，初始不写配置，三个新命令持有同一新VM，两个设备存储独立；这些是骨架状态检查，不是DoD验收。
- 独立验证进程临时赋DataContext后，下拉框出现两台设备及默认001，临时选择绑定能写回字符串，真实CurrentDevice仍未实现切换；临时连接随后撤销，不写入源码、不算学员已完成TODO1/2。
- 验证进程直接改已有设备对象和当前对象引用，并发通知，仅用于检查教练的集合换接、旧集合解除、单点/最新三点、CSV组行/写入、配置辅助参数写入及长结果布局，最后清空诊断数据并恢复原对象；不把这些诊断当成学员核心实现。
- 默认/最小尺寸必要控件及长结果均在窗口内且区域不重叠，已实际查看[初始窗口图](./evidence/TASK-038/startup/default-initial.png)与[最终最小诊断图](./evidence/TASK-038/startup-final/minimum.png)，[最终默认诊断图](./evidence/TASK-038/startup-final/default.png)保留；诊断图中的曲线来自运行内测试数据，而表格/Count仍留绑定TODO，不能用它证明数据同源已完成。
- & WpfBae/Exercises/TASK-038-monitor-project/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-038/startup-final/launch.json实际退出0，原exe标题为TASK-038独立完成模拟设备监控小项目，正常启动关闭退出0，见[启动记录](./evidence/TASK-038/startup-final/launch.json)。
- [核对](./evidence/TASK-038/startup-final/verification.json)确认TASK-036/037十个原文件哈希均不变，原/验证DLL一致；诊断配置和CSV仅写入独立证据目录，没有触碰旧配置或导出。
- 本轮只检查保存对话框的配置，没有交互真实保存窗口选择/取消/覆盖；没有验证学员尚未实现的真实采集、阈值应用或任务DoD，三个DoD未勾选，不新增掌握或典型错误，不创建TASK-039。
- 问答仍25分类/63条、待复习32/已掌握31，原C#/WPF复习范围保持，当前任务完成与WPF阶段完成分别按实际证据判断。


### 2026-10-18 TODO2a语句澄清

2026-10-18 TASK-038 TODO2a答疑：在设备选择Q2用001改002解释value、IsSampling与setter中的提前return，要求先检查再赋值以保留原编号，return不会撤销已执行赋值；只澄清TODO注释与文档，未代写核心判断或运行验收，原验证能力/问答统计和任务DoD保持，不记新错题。


### 2026-10-18 TODO7b具体操作澄清

2026-10-18 TASK-038 TODO7b答疑：以保存26、重开默认27的场景解释文件已读出threshold=26后，恢复有效数值/输入文字、重判报警并报告加载完成；不再写文件，另提示TODO6a的CurrentThresholdText通知连接。只澄清注释与记录，未代写实现或运行验收，任务/DoD和问答统计保持，不记新错题。

## 2026-10-18首次实际审查

当前状态🟡 进行中；131项实际检查114通过、17失败，三个必需DoD均未通过，不创建TASK-039，上方未实现描述为骨架历史，当前已写的正确实现及局部证据保留。

| DoD | 结论 | 可见与实际证据 |
| :--- | :--- | :--- |
| 1 | 未通过。 | DataContext、选择写回/实际设备切换、同一表格/Count/报警/图表和独立位置方法验证通过，但AddButton/ ClearButton的Command路径写为控件名，实际点击不追加/清空，不能把直接调用VM命令后的数据当成按钮已接通。 |
| 2 | 未通过。 | 原定时器启停、状态同步、选择禁用/入口保护、底层清空保护、导出运行拒绝、停止与关闭均通过；真实Tick到达但没有追加温度，IsSampling没有通知清空命令重查，按钮本身又未绑定命令，运行禁用未完成。 |
| 3 | 未通过。 | 原阈值方法的合法非法处理、即时报警、有效值配置保存及7b数值/输入恢复、真实CSV位置/取消/覆盖和固定小数/失败恢复、默认最小布局通过；输入未即时写回，有效值/反馈仍是占位文字，派生显示缺通知，RUN-RECORD仍全为待运行。 |

### 按实际修复顺序处理

1. TODO4d：Task038Window.xaml第33/34行的Command="{Binding AddButton}"和Command="{Binding ClearButton}"要分别读取AddCommand和ClearCommand；Binding此处从窗口DataContext即新VM读取公开属性，AddButton/ClearButton只是具体按钮的名字，VM没有这两个命令属性，当前按钮点击没有动作。
2. TODO5c：Task038Window.xaml.cs的SamplingTimer_Tick目前只累加_tickCount和显示次数，补上对同一_viewModel.AppendNextSample的一次调用；不要重复登记Tick或new数据对象，真实运行中当前通知次数增加而历史不变。
3. TODO5a：VM的IsSampling.set保存状态后，还需调用_clearCommand.NotifyCanExecuteChanged()，要求已绑定按钮重查CanExecute；当前只通知CanSelectDevice，启停过程中清空条件通知为0，原CanExecute与ClearHistory保护本身正确。
4. TODO6c：ThresholdInput的Text显式TwoWay并用UpdateSourceTrigger=PropertyChanged，CurrentThresholdText和ThresholdFeedbackText的Text分别读取VM的同名属性；目前输入沿用TextBox默认失焦写回，输入瞬间VM仍是原文字，而两个结果控件仍显示“待绑定”。
5. TODO6a：AlarmThreshold.set已有AlarmThreshold通知，还需通知CurrentThresholdText；该显示文字读取有效数值，即使数值已应用或恢复，绑定也要接到同名通知才能重读。
6. TODO8：修复后实际运行并填写RUN-RECORD.md，不把教练诊断调用冒充学员已做按钮测试；记录真实日期和现象，来源分清学员核心与已提供布局/文件/绘图/安全收尾。

现在先完成第1项，运行后点击添加应从0变1，点击清空应回0；确认这一处再依次接定时器和通知，文件助手和已经通过的独立位置算法不需要重写。
7b目前已把读取的合法阈值恢复进AlarmThreshold/ThresholdInputText，并重新判断报警和报告加载完成，数值与输入恢复通过；有效值显示不更新归因于上述绑定/通知缺口，不要求重写7b或再次保存文件。

### 实际验证与边界

- dotnet build WpfBae/WpfBae.csproj --nologo退出0，0错误、14条既有CS8600报告，见[构建输出](./evidence/TASK-038/review-01/build.log)与[退出记录](./evidence/TASK-038/review-01/build.json)。
- dotnet run --project WpfBae/Exercises/TASK-038-monitor-project/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-038/review-01退出1，131项114通过/17失败，见[实际结果](./evidence/TASK-038/review-01/results.json)；退出1来自断言失败，不是程序崩溃。
- 原编译Task038Window实际显示，真实按钮自动化调用确认添加/清空无动作；实际输入26后VM未即时改变，真实Tick已到达却未增加历史，启停未发清空条件通知；没有临时改Command、Binding、Tick或VM实现。
- 为继续分辨已实现能力，验证后半段明确使用原VM命令准备历史并独立检查方法，结果标为“方法诊断”；001五条、002两条/第三条25、切回恢复/独立继续、仅清当前与反复切换不重复刷新均通过，不把这些准备动作冒充添加按钮已接通。
- 有已准备历史后，实际点击原导出按钮并操作真实保存窗口，验证两个目录、取消、覆盖拒绝/接受、自动扩展名、de-DE固定小数与完整行数；占用文件/目录冲突只通过原写入方法注入，随后真实再次选路径恢复通过，未伪称保存窗口选择了不可写目标。
- 阈值合法值与非法输入通过原新VM的ApplyThresholdCommand独立诊断，数值保护及报警方法正确；配置保存用实际保存按钮，未应用29仍保存有效26，重建窗口实际恢复26到VM和输入，坏JSON/null/非法范围保留默认和原文件。
- & WpfBae/Exercises/TASK-038-monitor-project/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-038/review-01/launch.json退出0，原exe实际启动TASK-038并正常关闭退出0，见[启动记录](./evidence/TASK-038/review-01/launch.json)。
- 已实际查看[最小尺寸图](./evidence/TASK-038/review-01/minimum.png)，默认/最小必要控件和长路径均可读，表格随窗口伸展通过，[默认图](./evidence/TASK-038/review-01/default.png)与[界面失败图](./evidence/TASK-038/review-01/ui-findings.png)保留；图中的温度由明确标注的VM命令诊断准备，不算原添加按钮操作已成功。
- [核对](./evidence/TASK-038/review-01/verification.json)确认112项源码、既有配置/CSV及运行记录哈希均不变，原/审查DLL一致；本轮没有代改学员实现或代填运行记录，测试数据仅写入独立证据目录。
- 本轮只增量记录已验证的局部能力与连接缺口，问答仍25分类/63条、待复习32/已掌握31，不撤销旧题已验证实践，不把已提供辅助或局部方法通过升级为完整自主项目；WPF阶段仍未完成。

## 2026-10-18第二次实际审查：代码功能全部通过，仅待运行记录

状态仍🟡 进行中；本轮265项检查中264项功能全部通过，唯一未通过项为RUN-RECORD.md仍是待运行/待填写模板，DoD1/2通过并勾选，DoD3的全部功能通过但提交记录未完成，因此暂不通关、不创建下一题。
上次五组代码连接已由学员自主补齐，本轮没有代改源码或代填运行记录；此前131项中17失败的记录保留为历史。

| DoD | 结论 | 本轮实际证据 |
| :--- | :--- | :--- |
| 1 | 通过。 | 实际添加按钮/清空命令、001五条和002两条、切回保留与各自继续位置、真实表格/Count/报警/最新三点同源和反复切换不重复刷新均通过，未临时替换Binding或用方法诊断代替按钮路径。 |
| 2 | 通过。 | 真实Tick追加、手动/自动混合顺序、重复启停、每次通知仅一条、停止后不增加、运行选择/清空/导出保护、停止恢复及正常/采中关闭均通过；教练预留的安全收尾仍按原范围记录。 |
| 3 | 功能通过，提交未完成。 | 输入即时双向写回、有效值和反馈同步、派生显示通知、合法/非法输入、即时报警、配置只保存有效值/合法恢复/坏值保护、实际CSV选择/取消/覆盖/固定小数/写入失败恢复及默认最小布局均通过；RUN-RECORD.md尚无学员实际操作记录。 |

### 唯一剩余提交

打开[RUN-RECORD.md](../../WpfBae/Exercises/TASK-038-monitor-project/RUN-RECORD.md)，将已有六组场景的实际日期、操作和观察结果填上，并说明学员完成的核心连接及教练提供辅助的范围；未亲自做过的场景运行后再填，不把本轮教练自动检查冒充学员操作。
任务卡DoD3明确包含“提交代码与简短运行记录”，所以当前仅此提交项暂未完成，不要求重写已经全部通过的代码，也不因空记录再记一条认知错误。

### 本轮实际命令与证据

- dotnet build WpfBae/WpfBae.csproj --nologo退出0，0错误、14条既有CS8600报告，见[构建输出](./evidence/TASK-038/review-02/build.log)与[退出记录](./evidence/TASK-038/review-02/build.json)。
- dotnet run --project WpfBae/Exercises/TASK-038-monitor-project/Verification/FullReview.csproj -- docx/tasks/evidence/TASK-038/review-02退出1，265项264通过/1未通过，见[完整结果](./evidence/TASK-038/review-02/results.json)，唯一失败名为“提交记录/实际运行记录已填写”；退出1来自此项，功能无失败，无未捕获运行异常。
- 本轮改用完整真实界面验收，按钮通过自动化命令路径执行，输入通过真实Text控件写回，未临时补绑定或核心调用；前轮为分辨遗漏而使用的独立VM诊断不再替代正常按钮。
- 真实保存窗口覆盖两目录、取消、拒绝/接受覆盖、自动扩展名、de-DE固定小数、完整行数；占用文件/目录冲突仍只在原写入方法注入，再通过真实重新选路径恢复，不伪称选择了不可写目标。
- & WpfBae/Exercises/TASK-038-monitor-project/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-038/review-02/launch.json退出0，原exe实际启动TASK-038并正常关闭退出0，见[启动记录](./evidence/TASK-038/review-02/launch.json)。
- 已实际查看[最小尺寸图](./evidence/TASK-038/review-02/minimum.png)，有效阈值和反馈已更新，路径可读，表格与曲线正常；[默认图](./evidence/TASK-038/review-02/default.png)保留，表格伸展与边界检查通过。
- [核对](./evidence/TASK-038/review-02/verification.json)确认112项原源码、既有数据及运行记录哈希均未变化，原/审查DLL一致，本轮未代写实现、未填模板，测试配置和CSV仅在独立证据目录。
- 错题中TASK-038的代码连接遗漏标记为已修复，任务因提交记录保持进行中；保留教练提供的数据结构、命令构造/接口、布局、绘图、文件辅助及安全收尾边界，不计为新的自主设计。
- 新窗口中的DataContext/选择通知/独立位置/Command/清空条件重查/Tick连接/阈值输入与显示/配置和CSV入口已有自主应用及实际验证证据，问答原范围保持，不由代码通过推定完整框架机制已掌握；25分类/63条、待复习32/已掌握31保持，尚未判定WPF阶段通关。

## 2026-10-18用户委托教练完成六组记录：任务通关

用户明确要求六组验收与记录直接由教练完成，并反馈重复填写对学习没有意义；本轮按该要求调整DoD3的记录执行者，取消“学员亲自重复填写表格”作为单独门槛，代码功能与实际验证要求保持。
教练已将第二次审查真实执行的六组场景整理到[RUN-RECORD.md](../../WpfBae/Exercises/TASK-038-monitor-project/RUN-RECORD.md)，逐项标明2026-10-18、执行者为教练、实际观察和证据，保留学员代码与教练辅助边界，不冒充学员亲自运行或撰写。
264项功能此前全部通过，本轮记录已补齐，三个必需DoD现均通过，状态✅ 已通关；上方“仅待学员记录/继续进行中”为保留的历史安排，现由用户指示取代。

- 本轮实际核对[完成记录](./evidence/TASK-038/completion/record-completion.json)：沿用第二次审查264项功能全通过结果，112项基线只有RUN-RECORD.md变化，功能源码及既有数据均未变化，六组模板已填实，记录完成检查通过。
- 本轮未重新执行整套UI验收，未把历史265项中的1项未完成静默改写成旧结果全部通过，旧results.json原样保留；新增完成记录明确引用原功能证据和当前文档检查。
- 这份验收报告由教练代整理，不算新增学员测试报告写作能力；学员自主完成并已验证的核心连接证据保持，7b曾给参考示例及既有辅助的范围继续明确记录。
- 用户偏好已记录：学习重点放在有学习价值的代码与理解，实际验收及机械结果整理由教练承担，同类重复抄录不再单独作为通关条件，不取消对代码的实际功能验证。
- 同步任务卡、任务索引、学习进度、问答与错题状态，只创建下一张TASK-039（当时未开始，现已按用户要求删除），依据计划后续串口/TCP路线先做本机TCP请求响应；未创建源码、启动监听或切换入口。
- 未将任务通关等同于所有C#/WPF机制全部掌握，原25分类/63条、待复习32/已掌握31及已标明复习范围保留，后续结合实际问题巩固。

## 2026-10-18后续安排撤销

用户明确要求删除TASK-039，后续阶段将另建学习系统；任务卡已删除、索引及当前下一步入口已清理，本系统不再自动创建后续任务，以上曾规划TASK-039为保留的历史，TASK-038与WPF阶段通关结论不变。
