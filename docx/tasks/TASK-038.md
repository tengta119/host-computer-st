# TASK-038：独立完成模拟设备监控小项目

- 阶段：第二阶段：WPF（监控功能综合实践）。
- 状态：🟡 进行中
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

- [ ] 学员自主连接新窗口与一份新监控ViewModel，设备对象与各自集合不串用，001五条和002两条后切回各自恢复数据与样例位置，表格、Count、报警和最新三点同源，切换不额外追加或清空。
- [ ] 学员自主接通命令与定时器生命周期，手动和自动共用当前设备追加入口，每次通知只追加一次；重复启停不重复订阅，停止不再追加，运行期间切换/清空/导出受保护，停止恢复操作，关闭停止定时器并解除必要监听。
- [ ] 公共阈值合法应用即时刷新、非法输入保留有效值，配置只保存有效值并安全恢复，真实CSV选择位置/取消/覆盖和当前设备完整历史格式正确；实际默认/最小窗口结果及路径可读，提交代码与简短运行记录，教练提供/协助与学员自主实现范围分别记录。

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
| 2a → 2b | VM的SelectedDeviceId与SwitchToSelectedDevice。 | 实践1 / DoD1/2。 | setter先做运行保护，再更新/通知/调用切换；切换找已有设备对象、通知CurrentDevice/Temperatures，不new、追加或清空。 |
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
