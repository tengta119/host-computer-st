# TASK-037：独立组织模拟设备监控界面

- 阶段：第二阶段：WPF（已学控件、布局和绑定的综合应用）。
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)中的Grid、StackPanel、DockPanel、XAML、Binding及能够自己编写设备监控界面的目标。
- 选择依据：TASK-036已经验证设备切换、独立历史和运行保护，但完整窗口布局与不少连接来自教练骨架，因此下一步减少布局提示，在已有监控功能上自主安排界面，不据此宣称整个WPF阶段完成。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

为已有两台模拟设备重新组织一个可操作的监控窗口，把设备选择、采集操作、阈值设置、历史表格和最新三点曲线安排成清楚的区域。
沿用TASK-036已经验证的数据类、ViewModel、命令和样例，不增加真实通信或新的框架，重点验证自己编写布局并连接已有数据的能力。
不要求重写已经通关的CSV算法、定时器、配置读写或坐标换算，也不把教练提供的这些实现计为新的自主设计。

## 明确开启后的准备约定

本轮只创建任务卡，不创建源码或切换App.xaml，当前入口继续TASK-036。
明确开启后先读取现有窗口与各控件连接，准备独立任务目录`WpfBae/Exercises/TASK-037-monitor-layout/`，复用原逻辑并保留TASK-036作品。
教练提供能运行的最小窗口和必要后台连接，解释哪些控件名称被现有代码读取以及各Binding读取哪个对象，避免因重新排版破坏入口。
教练负责绘图、文件对话框及定时器等既有后台连接的安全复用，学员负责可见区域布局、控件摆放和已有属性/命令绑定，不直接提供整份最终XAML。
如需尚未解释的布局属性，先说明它改变哪个区域和实际效果，再安排一个可观察的小改动。

## 实践与产出

1. 先只搭建设备选择与采集操作区域，运行观察默认设备编号，切换后编号跟随，停止与运行时选择状态正确。
2. 按自己的区域安排补上公共阈值输入、有效值与反馈、完整历史表格和最新三点曲线，逐个连接已有数据及命令。
3. 调整默认和最小窗口尺寸，运行验证设备切换、手动/自动追加、停止、清空、阈值、配置和CSV导出，说明各主要控件读取的数据来源。

## 验收标准（DoD）

- [x] 学员自主编写主要区域布局与已学控件绑定，设备选择、采集操作、阈值、历史及曲线区域在实际默认和约定最小窗口中均可读可操作，尺寸缩小后不遮挡必要按钮或数据。
- [x] 新窗口沿用同一当前设备数据来源，001添加五条与002添加两条后切回各自保留历史和位置，表格、Count、报警及最新三点一致，采集中禁止切换且停止后恢复，清空只影响当前设备。
- [x] 公共阈值应用/非法输入保护与配置保存恢复、手动/自动追加及停止、当前设备完整历史CSV选择位置/取消/覆盖与正常关闭保持，实际运行证据能够对应新窗口，复用实现和自主编写范围分别记录。

## 状态与证据

- 2026-10-18：TASK-036全部必需DoD通过后仅创建本卡，未准备源码、切换入口或运行本题验收，三个DoD未勾选。

## 2026-10-18明确开启：实际骨架

当前🟡 进行中，上方“仅建卡片”和旧入口是开启前历史；App.xaml现已切到Task037Window，TASK-036原作品保留。

### 本题具体写什么

旧窗口的控件位置已经由教练排好，本题把设备区、操作区、阈值区、导出区和监控区的位置交给你安排，再由你写回已学过的Binding和Command连接。
例如让设备选择与当前编号并排，让表格利用窗口剩余空间，把最新三点曲线放在自己选择的区域；最终不要求照搬TASK-036的排版。
现在只显示设备区，其余四区的Visibility暂设为Collapsed，这是WPF提供的属性值，表示不显示也不占位置，不表示这些控件不存在。
后台已经能找到这些控件并完成初始化；轮到某区时先给它安排位置，再把它的Visibility改为Visible，避免全部挤在Grid默认第0行。
原始骨架仍可能改选下拉框但“当前监控”不变，这是SelectedItem和当前编号绑定尚未完成的可观察占位，不是已实现设备切换。

### 实际文件与复用边界

- 专属目录：`WpfBae/Exercises/TASK-037-monitor-layout/`。
- 主要编辑：[Task037Window.xaml](../../WpfBae/Exercises/TASK-037-monitor-layout/Task037Window.xaml)，区域位置和核心绑定留TODO，没有提供整份最终布局。
- 教练复用后台：[Task037Window.xaml.cs](../../WpfBae/Exercises/TASK-037-monitor-layout/Task037Window.xaml.cs)，从TASK-036复制实际采集、集合切换监听、配置、导出、绘图与关闭收尾；本题不要求重写。
- 直接引用原有类型：[TemperatureMonitorViewModel.cs](../../WpfBae/Exercises/TASK-036-device-monitor/TemperatureMonitorViewModel.cs)、[DeviceMonitorState.cs](../../WpfBae/Exercises/TASK-036-device-monitor/DeviceMonitorState.cs)、[AppendTemperatureCommand.cs](../../WpfBae/Exercises/TASK-036-device-monitor/AppendTemperatureCommand.cs)、[ClearTemperatureHistoryCommand.cs](../../WpfBae/Exercises/TASK-036-device-monitor/ClearTemperatureHistoryCommand.cs)、[ApplyThresholdCommand.cs](../../WpfBae/Exercises/TASK-036-device-monitor/ApplyThresholdCommand.cs)、[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-036-device-monitor/AlarmThresholdSettings.cs)；不复制第二套数据实现，不修改旧题文件。
- 教练新建的Task037Window仍创建自己的TemperatureMonitorViewModel对象，不与另一个窗口共享运行中的设备历史；复用的是类的实现。
- 新窗口只读写`AppContext.BaseDirectory/Exercises/TASK-037-monitor-layout/alarm-threshold.json`，不改TASK-036配置；保存窗口显式使用当前窗口作所有者，复制过来的可空配置局部变量已由教练补上问号并保留原null判断，未改旧题声明。
- 清理了新后台副本里过时的TODO注释，学员操作以新XAML的TODO1至5为准；原作品注释保留。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml)只切换StartupUri，[WpfBae.csproj](../../WpfBae/WpfBae.csproj)只增加本题Verification目录编译排除，不引入新依赖。
- 独立验证：[StartupReview.csproj](../../WpfBae/Exercises/TASK-037-monitor-layout/Verification/StartupReview.csproj)、[验证源码](../../WpfBae/Exercises/TASK-037-monitor-layout/Verification/StartupReview.cs.txt)、[原程序启动脚本](../../WpfBae/Exercises/TASK-037-monitor-layout/Verification/launch.ps1)。

### 先区分布局、名字和数据来源

RootLayout、DeviceArea、OperationsArea、ThresholdArea、ExportArea和MonitorArea都是教练给当前具体容器起的名字，不是WPF的新API。
Grid、StackPanel、Button等才是WPF提供的控件类；x:Name给这一个具体控件起名字，方便后台或验证找到它，不会自动决定位置或绑定数据。
例如StartButton已经通过Click连接原启动方法，所以移动这个按钮后只要保留名字和Click，点击仍会启动原定时器。
AddButton和ClearButton尚未设置Command，需要你把它们分别连接已有AddCommand和ClearCommand；不能另写一套按钮追加算法。
窗口构造方法已经将DataContext设置为自己持有的_viewModel，这行把当前窗口的Binding读取对象设置为同一份监控数据，不需要在XAML或后台再new一个ViewModel。
例如设备选择框的SelectedItem应写回这个对象的SelectedDeviceId，当前编号文字应读取这个对象的CurrentDevice.Id；选项列表ItemsSource已经由教练连接DeviceIds。
已有采集/绘图后台还会直接读写部分命名控件，因此保留现有x:Name和Click，可以改它们的父容器、位置和间距，不删除后再另造同名控件，也不复制出两份同名控件。
ChartRegion内部的固定560×300画布和命名图形已接通原坐标算法，本题只安排这整块图表的位置，不改变内部坐标或缩小画布冒充布局完成。

| 控件 | 学员连接的数据或操作 | 已有后台负责的部分 |
| :--- | :--- | :--- |
| DeviceComboBox | SelectedItem双向连接SelectedDeviceId，IsEnabled读取CanSelectDevice。 | ItemsSource已连接DeviceIds，VM实现切换与运行保护。 |
| CurrentDeviceText | Text读取CurrentDevice.Id。 | VM发出切换通知。 |
| AddButton / ClearButton | Command分别读取AddCommand / ClearCommand。 | 已有命令与当前设备历史操作。 |
| StartButton / StopButton | 保留已有Click，自行摆放。 | 原定时器启停与IsSampling同步。 |
| ThresholdInput / ApplyThresholdButton | Text双向即时写回ThresholdInputText，Command读取ApplyThresholdCommand。 | 原验证规则与应用逻辑。 |
| CurrentThresholdText / ThresholdFeedbackText | Text读取各自同名属性。 | VM有效值通知和应用反馈。 |
| SaveThresholdButton / ExportHistoryButton | 保留已有Click，自行摆放。 | 独立配置与当前设备CSV保存。 |
| ConfigStatusText / ConfigPathText / ExportStatusText / ExportPathText | 保留名字并安排位置，不另加Text绑定。 | 后台直接显示实际结果与路径。 |
| SamplingStateText / TickCountText | 保留名字并安排位置，不另加Text绑定。 | 后台直接显示状态和通知次数。 |
| AlarmText / ReadingCountText | Text读取各自同名属性。 | VM按当前设备重算状态与条数。 |
| HistoryDataGrid | ItemsSource读取Temperatures。 | 温度列的单条数值Binding已复用。 |

### TODO顺序与验收对应

| 顺序 | 实践 / DoD | 本步产出 |
| :--- | :--- | :--- |
| 1a | 实践1 / DoD1准备 | 标题保留上方，在标题下添加横向StackPanel并把后面四个现有控件移进去，让它们在窗口中从左到右并排显示。 |
| 1b → 1c | 实践1 / DoD2 | 接通选择写回、选择禁用与当前编号显示，停止时改选002后编号跟随。 |
| 2a → 2b | 实践1 / DoD1/2 | 自行规划RootLayout的行/列，显示并安排操作区，连接追加和清空命令。 |
| 3a → 3b → 3c | 实践2 / DoD1/3 | 自行安排阈值与导出区，补输入/应用/显示绑定，保留保存与导出Click。 |
| 4a → 4b | 实践2 / DoD1/2 | 自行安排状态、曲线和历史区域，绑定报警/Count/表格并使表格获得可用空间。 |
| 5 | 实践3 / DoD1/2/3 | 实际验证默认与最小布局及两设备完整操作，按DoD提交。 |

布局可以选已学Grid或DockPanel，不强制列数、颜色、固定行数或与旧窗口一致；一个区域先可运行，再继续下一处。
Grid的Auto行按控件需要占高度，星号行取其他行分配后剩余的高度；Grid.Row从0开始，只对当前控件的直接父Grid中的行起作用，相关具体解释见[布局Q1](../questions/wpf-layout.md)。
竖直StackPanel只负责从上往下排列，不负责把剩余高度交给表格；轮到4b时需要自己安排装着表格的容器与可伸展行，不能只在外层设置星号行就认为内部表格一定伸展。
本题最初窗口沿用960×1000、最小860×960，若自行调整须记录实际约定尺寸并运行验证，必要控件和560×300曲线不能被遮挡。

### 现在只做TODO1a

打开Task037Window.xaml，找到DeviceArea里的标题、选择设备文字、DeviceComboBox、当前监控文字和CurrentDeviceText。
标题保留为DeviceArea的第一个元素，另外在它后面新建一个StackPanel，将其Orientation设为Horizontal，把后面四个现有元素移进去，不复制它们。
这个新StackPanel让其中四个具体控件从左向右排列，外面的DeviceArea仍默认从上向下排列，所以标题继续独占上面一行。
需要的写法只有下例，里面放刚才移动的四个现有元素，暂不修改它们的绑定。

```xml
<StackPanel Orientation="Horizontal">
    <!-- 在这里放移动后的四个现有元素。 -->
</StackPanel>
```

可用Margin给相邻控件留间距，但第一步不展开其他区，也不启动采集。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，预期标题仍在上方，设备标签、下拉框和当前编号占位文字改为同一行；本步编号仍显示等待连接是正常的，1b/1c才接通它。
该命令是学员下一步运行方式，本轮没有执行它；本轮教练实际执行的命令与结果如下。

### 实际骨架验证与掌握边界

- `dotnet build WpfBae/WpfBae.csproj --nologo`实际退出0，0错误、14条既有CS8600报告，来自TASK-030至036的原配置声明，未修改旧作品。
- `dotnet run --project WpfBae/Exercises/TASK-037-monitor-layout/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-037/startup`实际退出0，44项骨架检查全部通过，见[结果](./evidence/TASK-037/startup/results.json)。
- 实际显示新窗口，验证两项设备/初始001/同一VM/一秒定时器/独立配置路径，确认所有核心绑定和最终布局仍留TODO，首步只有设备区可见，默认与最小边界通过，已实际查看[最小尺寸图](./evidence/TASK-037/startup/minimum.png)，[默认图](./evidence/TASK-037/startup/default.png)保留。
- 仅在独立验证进程临时接通选择与编号绑定，验证002写回/编号跟随、窗口集合换接、各设备独立样例/清空、单点与最新三点、真实定时器一次追加、运行禁用与方法保护、停止恢复、公共阈值应用/非法输入保留、独立配置保存恢复及当前设备CSV格式/写入结果，随后撤销临时绑定；未写入学员源码，不计学员实现或DoD通过。
- 本轮检查了保存窗口配置，没有交互真实保存窗口的选择/取消/覆盖；仅验证写入方法，不伪称本题新界面完整导出已验收。
- `& WpfBae/Exercises/TASK-037-monitor-layout/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-037/startup/launch.json`实际退出0，原exe显示TASK-037标题，正常启动关闭退出0，见[记录](./evidence/TASK-037/startup/launch.json)。
- TASK-036八个原文件哈希均不变，原与验证DLL一致，见[核对](./evidence/TASK-037/startup/verification.json)；诊断配置与CSV仅在独立证据目录，未触碰旧配置或导出。
- 当前三个DoD均未勾选，未新增学员自主掌握或错题，问答仍25分类/63条、待复习32/已掌握31，原有复习范围保持；按实际操作顺序编号1至5，不要求跳序完成。


### 2026-10-18 TODO1a措辞澄清

2026-10-18 TASK-037布局答疑：用户追问“另建一行”，已在布局Q1和TODO1a改为具体说明：标题后新增横向StackPanel，把后面四个现有控件移进去，在实际窗口中并排显示；区分界面横排、源码换行与Grid行定义，教练含糊表述不记学员错误。保留已写的选择/禁用/编号绑定，仅修改TODO注释与文档，未运行验证，任务🟡 进行中、DoD和问答统计保持。

## 2026-10-18首次实际审查

状态仍🟡 进行中；264项实际检查258通过、6失败，失败均对应默认/最小窗口中配置结果、配置路径、导出路径越过窗口右边界，DoD1未通过、2/3通过，不创建下一题，上方未绑定与收起区域为骨架历史。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 未通过。 | 自主设备横排、区域分行、曲线左/表格右与表格随窗口增高伸展已验证；ThresholdArea和ExportArea的长结果文字横向超出，默认和最小各3项边界失败。 |
| 2 | 通过。 | 001五条/002两条及独立位置、切回恢复、同一表格/Count/最新三点、清空隔离、真实定时器、运行禁用和方法保护、停止恢复、反复切换与启停不重复均通过。 |
| 3 | 通过。 | 实际控件绑定与命令、公共有效阈值/合法非法输入/即时报警、独立配置保存与有效/无效恢复、当前设备完整历史CSV与真实保存窗口两目录/取消/覆盖拒绝与接受、固定小数/自动扩展名、写入失败注入后重新选择恢复、停止和运行中关闭均通过；结果文字存在且正确，其显示边界另归DoD1。 |

### 需要自主修复的位置

1. Task037Window.xaml第50行的ThresholdArea是横向StackPanel，输入、按钮、有效值、反馈、保存结果和完整配置路径全部向右排列；最小窗口可用宽约797，实际该区宽约2309，ConfigStatusText宽约915、ConfigPathText起点约1460，文字被裁掉。
2. 第62行的ExportArea同样把按钮、成功结果和完整路径放在一排，实际宽约1289，ExportPathText从约750开始，后面大部分超出可用宽度。

TextWrapping="Wrap"只处理一个TextBlock内部的文字，不会让横向StackPanel把后面的控件自动移到下一排；这两处需要调整装着文字和按钮的容器，不需要重写Binding或导出算法。
先修阈值区：把需要并排的输入/按钮留在内部横排，把ThresholdFeedbackText、ConfigStatusText、ConfigPathText移到这个横排后面的独立显示位置，让外层按竖直方向排列，保留原名字和绑定。
然后修导出区：将ExportStatusText和ExportPathText放到按钮下面，保留TextWrapping与后台写入连接，实际导出一次后缩到最小窗口核对完整结果和路径可读。
这里不提供整份最终XAML，不代改学员布局；本轮105项源码与既有数据哈希均未变化。

### 本轮实际命令与证据

- dotnet build WpfBae/WpfBae.csproj --nologo退出0，实际增量构建0警告/0错误，见[构建输出](./evidence/TASK-037/review-01/build.log)与[退出记录](./evidence/TASK-037/review-01/build.json)；本轮没有修复此前原文件的可空声明。
- dotnet run --project WpfBae/Exercises/TASK-037-monitor-layout/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-037/review-01退出1，264项258通过/6失败，见[结果](./evidence/TASK-037/review-01/results.json)，退出1来自布局断言，不是窗口崩溃。
- 验证加载原编译Task037Window，通过实际输入控件写回、按钮自动化命令路径和真实保存对话框操作完成场景，没有给学员临时添加绑定、改布局或修补实现。
- 非法阈值、占用文件/目录冲突、损坏JSON产生的是预期捕获输出，相关保护和恢复断言通过；文件失败由直接调用原写入方法注入，不伪称真实保存窗口选择了不可写目标。
- & WpfBae/Exercises/TASK-037-monitor-layout/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-037/review-01/launch.json退出0，原exe实际显示TASK-037并正常关闭退出0，见[启动记录](./evidence/TASK-037/review-01/launch.json)。
- 已实际查看[初始图](./evidence/TASK-037/review-01/initial.png)和[最小尺寸图](./evidence/TASK-037/review-01/minimum.png)，[默认图](./evidence/TASK-037/review-01/default.png)保留，裁切可直接观察，具体边界见[最小布局记录](./evidence/TASK-037/review-01/最小-layout.json)和[默认布局记录](./evidence/TASK-037/review-01/默认-layout.json)。
- [核对](./evidence/TASK-037/review-01/verification.json)确认105项原源码/既有配置及CSV哈希不变，审查和原DLL一致；验证配置与CSV只写入独立证据目录，旧作品不变。
- 原ViewModel、命令和设备存储实现及后台绘图/采集/导出来自已验证作品，保留复用边界；学员新窗口的绑定连接、设备横排、根区域分行/监控分列与表格伸展有自主实践证据，完整长文字布局尚未通过，不因局部失败撤销已有能力。
- 布局Q1/Q3仅增量记录这些已验证应用和长文字修复线索，原已掌握状态按范围保留，问答统计仍25分类/63条、待复习32/已掌握31，典型错误记录横向堆放长文字导致越界，不记录此前教练含糊措辞为学员错误。
## 2026-10-18按用户要求协助修复与最终复查

用户明确要求“直接帮我修改吧”，教练已直接修复ThresholdArea与ExportArea的长文字布局，全部必需DoD现有证据通过，状态✅ 已通关；此前首次失败记录保留。

- 协助修改仅位于Task037Window.xaml：ThresholdArea改为竖直容器，内部保留输入/按钮/有效值横排，应用反馈、配置结果和路径分别放在其下方；ExportArea改为竖直容器，结果和路径放在左对齐的导出按钮下方，并补必要间距。
- 原绑定、控件名字、Click、根区域分行、曲线/历史分列和表格伸展保持；没有修改后台、数据类、ViewModel、命令或旧题文件。
- 上述两处布局修复是教练按明确授权完成，不计学员自主修复或完整长文字布局掌握；此前已验证的自主绑定、设备横排、区域分行/分列与表格伸展证据保留，复用的后台仍不计新自主设计。
- dotnet build WpfBae/WpfBae.csproj --nologo实际退出0，0错误、14条既有CS8600报告，见[构建输出](./evidence/TASK-037/review-final/build.log)和[退出记录](./evidence/TASK-037/review-final/build.json)。
- dotnet run --project WpfBae/Exercises/TASK-037-monitor-layout/Verification/TaskReviewFinal.csproj -- docx/tasks/evidence/TASK-037/review-final实际退出0，264项检查全部通过，见[最终结果](./evidence/TASK-037/review-final/results.json)。
- 默认和最小尺寸中所有必要控件/结果/路径均在窗口内，区域不相互覆盖，表格有效宽高和随窗口增高伸展通过；已实际查看[默认图](./evidence/TASK-037/review-final/default.png)与[最小尺寸图](./evidence/TASK-037/review-final/minimum.png)，完整配置与导出路径会按宽度换行。
- 两设备独立历史/位置/清空、选择保护/停止恢复、真实定时器/反复启停、公共阈值与合法非法输入/即时报警、配置有效/无效恢复、真实CSV保存窗口两目录/取消/覆盖拒绝与接受/固定小数及失败恢复均再次通过，未给验证进程临时修补实现。
- & WpfBae/Exercises/TASK-037-monitor-layout/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-037/review-final/launch.json实际退出0，原exe实际启动TASK-037并正常关闭退出0，见[启动记录](./evidence/TASK-037/review-final/launch.json)。
- [核对](./evidence/TASK-037/review-final/verification.json)确认105项基线仅Task037Window.xaml这一文件变化，验证副本与原DLL一致，诊断配置和CSV仅写入独立证据目录。
- 任务卡、索引、学习进度、问答和错题同步，问答仍25分类/63条、待复习32/已掌握31，不把协助修复升级为自主掌握；只创建下一张[TASK-038](./TASK-038.md)⚪ 未开始，未准备代码或切换入口，App.xaml继续TASK-037。