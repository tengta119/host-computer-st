# WPF 设备下拉框

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：下拉框的两个设备从哪里来，怎样把选中的设备交给读取命令？

- 日期：2026-10-13
- 来源：TASK-020 启动的必要讲解。
- 状态：已掌握（限骨架中的选中字符串绑定、默认选择与写回后读取应用）

TASK-020 顶部选择框展开后有 TEMP-001、TEMP-002，点一项后收起显示它。ComboBox 是 WPF 下拉框类，DeviceComboBox 是教练给这个具体控件起的名字。Width 决定这块选择框多宽。

原第一个动作是 [Task020Window.xaml](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml) 的 TODO1：只把 Width 从 200 改成 260。整理期间重读源码，Width 已经是 260，保留这项修改，不再要求重复改写。在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，预期实际 TASK-020 窗口下拉框变宽；本次未核对运行结果，设计预览不能代替这个现象。

选项来自后台已准备的数组：

```csharp
private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };
```

string[]是 C#字符串数组写法，new 创建含两个 ID 的数组，_deviceIds 保存引用。private 限定窗口类内部使用，readonly 限定初始化后不换引用，不冻结数组内部。然后 `DeviceComboBox.ItemsSource = _deviceIds;` 把数组交给具体下拉框的 WPF 数据列表属性。它存“可选设备”，Readings 存“已读温度”，用途不同。

本次选项都是字符串，选 TEMP-002 时，SelectedItem 取得字符串"TEMP-002"。SelectedItem 是这个下拉框的 WPF 属性，不是整个控件或整份历史。

后台 new ReadingViewModel 创建本窗口的一个已有自定义数据对象，_viewModel 保存引用，DataContext 指向它，DeviceId 初始为 TEMP-001。完成 TODO2 时给 ComboBox 添加：

```xml
SelectedItem="{Binding DeviceId, Mode=TwoWay}"
```

左边设置下拉框 SelectedItem；右边从原数据对象读 DeviceId。初始值 TEMP-001 在选项中，就选中它；TwoWay 允许用户改选时把新字符串写回同一个对象。原 DeviceId setter 中的 value 接到新字符串，保存到_deviceId，原读取命令下次取它就用新 ID。

原 Reading 保存当时的 ID，表格各行取各自 Reading 属性，所以改选不改旧行。IsEditable=False 允许选已有项，不能自由输入第三个 ID；键盘可帮助选择已有项，仍不会新增任意字符串。它不决定读取按钮是否可用，按钮条件仍由原命令处理。

### SelectedItem 没写 Mode=TwoWay，为什么仍能写回？

你实际写的是 `SelectedItem="{Binding DeviceId}"`。它仍把这个下拉框当前选中的字符串连接到原数据对象DeviceId；没有显式Mode时，WPF使用这个控件属性的默认方向。SelectedItem默认就是双向，所以开始时对象的TEMP-001选到下拉框，改选TEMP-002也会写回原DeviceId，不需要额外的SelectionChanged处理器。

2026-10-13实际窗口已验证：绑定Active，声明Mode=Default，属性元数据BindsTwoWayByDefault=True；通过展开下拉框选择TEMP-002后，原DeviceId即时变为TEMP-002，下一条读数记录002，旧001行保留。显式TwoWay更直观，但本次不要求为了示例替换合法实现。这个默认规则属于SelectedItem，不能推定所有Binding都默认双向。[Microsoft属性说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.selector.selecteditem?view=windowsdesktop-10.0)列出该默认元数据。

前面的Width首步与显式TwoWay示例是启动讲解；选中值连接现已由学员完成并实际通过，不能再把“尚未连接/初始为空”当作当前结果。

## Q2：TASK-036 怎样选择设备，并保留每台设备自己的监控历史？

- 日期：2026-10-18。
- 状态：已掌握。
- 来源：TASK-036开启，Q1已验证字符串设备选择，本题进一步连接独立设备对象、历史切换和运行保护，尚无自主证据。

### 首步只观察选中的字符串

Task036Window顶部新增DeviceComboBox，是一个WPF的ComboBox；它的ItemsSource已由教练绑定到原ViewModel的DeviceIds。
DeviceIds返回字符串数组，只有TEMP-001和TEMP-002两项；它是可选编号列表，和保存温度记录的集合用途不同。
SelectedDeviceId是教练为本题新增的字符串属性，初始为TEMP-001，它保存用户想选的编号。
CurrentDevice则指向实际正在监控的那个设备对象，首步暂时保持第一台，后续才接通历史切换。

在Task036Window.xaml的DeviceComboBox标签删除SelectedIndex="0"，添加：

```xml
SelectedItem="{Binding SelectedDeviceId, Mode=TwoWay}"
```

SelectedIndex原来用数字0指定第一项，删除后改由ViewModel的SelectedDeviceId决定选哪一项，避免保留两个选择入口。
SelectedItem是WPF下拉框属性，本题选项为字符串，所以选第二项后它取得字符串"TEMP-002"。
TwoWay让初始TEMP-001选到界面，也让改选TEMP-002时写回同一个ViewModel的SelectedDeviceId；setter里的value会接到这个新字符串。
再把SelectedDeviceObservationText的Text改为：

```xml
Text="{Binding SelectedDeviceId, Mode=OneWay}"
```

这块TextBlock只读取数据对象收到的编号，因此使用OneWay，与TASK-035观察输入的方向关系相同。
运行后不启动采集、不添加温度，只选TEMP-002，应看到观察文字为TEMP-002；此时“当前监控”仍显示TEMP-001，因为首步还没有接通真正切换。
先确认字符串进入同一个ViewModel，再做后续对象和历史连接，不把首步未切换视为程序缺陷。

### 每台设备的数据对象实际保存什么

DeviceMonitorState是教练在DeviceMonitorState.cs新建的普通C#类，不是.NET自带的设备API。
一个对象保存Id编号、Temperatures完整温度集合和NextSampleIndex下一条样例位置，代码中的两个new分别创建TEMP-001和TEMP-002的对象。
_devices数组保存这两个对象，_currentDevice指向其中实际监控的一个；切换时改成指向已有另一个对象，不能再new，否则原历史会丢失。
CurrentDevice公开属性返回_currentDevice，VM的Temperatures公开属性返回_currentDevice.Temperatures，因此表格和原命令读取的是当前设备那份集合。
Id创建后固定，NextSampleIndex允许读写，两个对象各有自己的字段；例如001下一条位置为1，并不意味着002也要从位置1开始。

### 后续切换要改变哪个对象

TODO 2在SwitchToSelectedDevice中先拒绝采集中切换，再遍历已有_devices，找到Id与SelectedDeviceId相同的对象，赋给_currentDevice。
这个普通方法由教练预留，目前为空；在首步观察通过后，才从SelectedDeviceId的setter调用它。
选择002时应找到已存在的002对象，切回001则找回原001对象，不清空历史也不额外追加温度。
给_currentDevice换引用后，需要通知CurrentDevice和Temperatures两个公开属性变化，再调用RefreshDisplay(AlarmThreshold)让新设备的Count和报警立即正确。
CurrentDevice通知让“当前监控”编号重读CurrentDevice.Id，Temperatures通知让表格重读新集合来源，这和给原集合Add一条不同。
ObservableCollection的CollectionChanged报告的是“这一份集合内部加了或删了记录”，它不会代替ViewModel报告“Temperatures现在返回另一台设备的集合”。

### 绘图和导出怎样跟上新集合

原窗口_temperatures用于绘图和CSV；如果它一直指着001集合，即使表格换到002，图表和导出仍可能读错设备。
教练已提供ViewModel_PropertyChanged接收方法，并在构造方法登记一次，它发现Temperatures属性变化时会更新这个兼容引用。
具体顺序是从旧集合解除Temperatures_CollectionChanged登记，把窗口_temperatures改指VM.Temperatures，再在新集合登记同一接收方法，最后重画和刷新报警。
解除旧登记的-=和新登记的+=沿用已有事件写法，防止旧设备的新增还触发当前图表，也避免反复切换重复通知。
关闭窗口时该接收方法和当前集合的登记均解除，这部分骨架由教练准备并实际验证，不要求学员额外猜框架入口。
CSV内容继续遍历窗口当前_temperatures，格式和小数点保持，教练已让成功文字包含CurrentDevice.Id和真实路径，当前设备按实际监控对象识别。

### 样例位置和清空也要跟设备走

TODO 3将AppendNextSample与ClearHistory中的过渡_nextSampleIndex改为CurrentDevice.NextSampleIndex，再删除VM里旧的共用位置字段。
这样001追加五条后下一条是26.5，002第一次追加仍从24开始；清空002只复位002的下一条位置，001的历史和位置不变。
Temperatures的getter已按当前设备返回，所以追加或清空继续操作这个公开集合即可，不额外给表格或CSV各建副本。
共有的样例数值数组可以继续复用，因为它只是24、26.5、25、28这组固定值，各设备需要独立的是下一条位置和历史。

### 采集中怎样阻止切换

TODO 4在SelectedDeviceId的set和SwitchToSelectedDevice入口做IsSampling保护，保护必须发生在修改选择或实际监控对象之前。
IsSampling已由窗口根据真实定时器同步，保持原含义true为正在采集，不要为使选择框禁用而交换状态值。
CanSelectDevice是新增bool属性，TODO 4让它在停止时返回true、采集中返回false，并把DeviceComboBox.IsEnabled绑定它。
IsSampling改变后还需通知CanSelectDevice变化，让已连接的选择框重新读取能否选择；控件禁用与方法入口保护各自负责界面操作和代码请求。
公共AlarmThreshold继续给当前设备判断，切换后只重判新设备最新值，不把输入文字自动应用，也不新增每设备配置。

### 开启时证据与边界

实际87项骨架检查和原exe启动关闭通过，验证进程临时连接首步，确认002字符串写回与观察；另外临时改当前对象并发通知，验证教练提供的旧集合解除/新集合连接可用，随后恢复初始数据。
这些诊断未写入学员源码，不算学员已实现切换；真正选择绑定、对象切换、独立位置与运行保护仍留四组TODO，三个DoD均未勾选。
Q2待复习，Q1已有设备字符串选择能力保留，当前25分类/63条、待复习33/已掌握30；实际路径与命令见[TASK-036](../tasks/TASK-036.md)。

### 2026-10-18追问：TODO 2c具体怎样发两个属性通知？

当前SwitchToSelectedDevice已找到对象并给_currentDevice赋值，也已有RefreshDisplay(AlarmThreshold)，但缺少通知公开CurrentDevice和Temperatures属性已改变的两次调用。
在循环结束后、已有RefreshDisplay之前加入下面两行，保留后面的刷新调用。

```csharp
OnPropertyChanged("CurrentDevice");
OnPropertyChanged("Temperatures");
RefreshDisplay(AlarmThreshold);
```

OnPropertyChanged是教练已放在这个ViewModel类底部的普通辅助方法，它通过.NET的PropertyChanged事件发出“请重新读取这个公开属性”的通知，不需要再创建另一个通知方法。
字符串"CurrentDevice"是公开属性名，不是TEMP-002这个编号；通知让绑定CurrentDevice.Id的当前监控文字重读新设备Id。
字符串"Temperatures"也是公开属性名；通知让历史表格重读新设备集合，同时触发教练提供的窗口ViewModel_PropertyChanged，改接绘图/导出的集合引用和事件监听。
RefreshDisplay负责按新设备历史重算Count与报警，它不会代替这两个来源变化通知，原调用保留一次即可。
给_currentDevice字段赋值只改变这个变量指向谁，不自动发属性通知；即使get已经能返回新对象，旧绑定和窗口集合引用也需要接到通知才重新读取。
本轮仅按可见代码答疑与补充说明，未代改学员代码、未运行验收，Q2仍待复习，TASK-036/DoD与25分类/63条、待复习33/已掌握30保持，不把当前局部代码存在当成全题自主验证通过。
### 2026-10-18追问：TODO 3a要干什么，为什么看不懂？（该旧编号现已改为3c）

原编号把最后的删除排成3a，是教练安排错误；用户指出后已直接重编号为3a修改追加、3b修改清空、3c删除字段，不要求学员跳序执行，不归因为学员基础问题。
ViewModel里的_nextSampleIndex目前只有一份，所有设备共用；数组下标0/1/2/3分别对应24/26.5/25/28。
例如001已加两条，旧共用位置是2；切到002后若还用它，002第一条就取到25，而应从自己的位置0取24。
DeviceMonitorState里每个对象已经各有一个_nextSampleIndex字段，并通过NextSampleIndex属性读写；001和002各保存自己的位置，不能把这个字段一并删除。
CurrentDevice指向实际选中的那个DeviceMonitorState对象，CurrentDevice.NextSampleIndex就是这台设备下一条应取的数组下标，不是温度值也不是历史Count。
先做3a：在ViewModel.AppendNextSample中把所有_nextSampleIndex读写改为CurrentDevice.NextSampleIndex，保持原取值、加一和达到样例数组Length时回到0的算法不变。
例如读值行改为double temperature = _sampleTemperatures[CurrentDevice.NextSampleIndex];，加一行改为CurrentDevice.NextSampleIndex = CurrentDevice.NextSampleIndex + 1;，条件判断和回0赋值也要使用同一设备属性。
再做3b：ViewModel.ClearHistory中原_nextSampleIndex = 0改为CurrentDevice.NextSampleIndex = 0，只复位当前设备；原IsSampling保护和Temperatures.Clear保留。
最后做3c：只删除TemperatureMonitorViewModel.cs中的private int _nextSampleIndex = 0;，因为这个共用字段已不再使用；DeviceMonitorState.cs同名字段和NextSampleIndex属性保留。
这三步一起完成后再运行，001添加两条应为24/26.5，002第一次添加应为24，切回001继续应为25；这说明两台设备分别记住了自己的下一条位置。
本轮已将代码注释和文档编号按操作顺序统一，没有替学员迁移核心读写或运行验收，Q2待复习，TASK-036/DoD及25分类/63条、待复习33/已掌握30保持，既有能力不撤销。
### 2026-10-18实际审查：位置反复为1与选择框未禁用

当前AppendNextSample写成CurrentDevice.NextSampleIndex =+ 1，它在C#中是赋值正数1，不读取旧值，因此原样例从24后一直重复下标1的26.5。
用完整普通写法CurrentDevice.NextSampleIndex = CurrentDevice.NextSampleIndex + 1明确先读旧值加一再写回，后面的Length检查才有机会达到4后复位0。
每台设备的NextSampleIndex存储已经独立，数据对象不串；问题发生在更新这个位置的表达式，不需要重新设计设备类或集合。
CanSelectDevice条件与IsSampling变化通知已经正确，运行时返回false，但DeviceComboBox.IsEnabled未绑定它，所以控件仍可点；添加IsEnabled="{Binding CanSelectDevice, Mode=OneWay}"才会让该控件跟随属性禁用。
SelectedDeviceId setter和SwitchToSelectedDevice的运行保护已实际拒绝切换，保留它们，不因缺界面连接撤销内部保护能力。
235项实际检查228通过，7失败归因于上述两处，DoD1/2未通过、3通过，Q2待复习；已验证选择写回、当前对象查找、两属性通知/集合连接、独立历史/位置、清空隔离和原导出能力保留，Q1不变，见[TASK-036](../tasks/TASK-036.md)。
本轮未代改代码，25分类/63条、待复习33/已掌握30保持，上方“切换尚未接通”为开启历史。
### 2026-10-18最终复查通过

学员自主修复位置递增和选择框IsEnabled绑定，235项实际检查全通过，Q2按当前骨架中选择写回、已有设备对象查找、CurrentDevice/Temperatures通知、各自样例位置与清空、运行保护和控件禁用连接的实际应用标已掌握。
设备数据类、选项提供、窗口集合订阅切换、绘图与关闭收尾由教练提供，不计新的自主设计，不扩大为完整MVVM或WPF机制掌握；本轮未改学员源码，见[最终结果](../tasks/evidence/TASK-036/review-final/results.json)。
当前25分类/63条，待复习32/已掌握31，Q1保持原范围，上方未通过与待复习为历史。

### 2026-10-18 TASK-038追问：TODO2a的“赋值前拒绝采集中选择”是什么意思？

当前SelectedDeviceId的setter里有_selectedDeviceId = value;，这行会把原编号改成外部传进来的新编号；例如原为TEMP-001，试图选择TEMP-002时，value就是TEMP-002。
“赋值前拒绝”是先检查IsSampling，如果为true表示正在采集，就执行return结束本次setter，让后面的赋值、通知和切换都不执行，原编号继续保持TEMP-001。
最小判断写法是if (IsSampling) { return; }，位置放在_selectedDeviceId = value;前面；IsSampling为false时跳过这个if的内容，再执行原赋值，允许停止时切换。
这里return只结束这一次属性setter，不是关闭窗口或退出程序；若先赋值再return，新编号已经写入，return不会自动恢复旧值。
IsSampling后续由TODO5将定时器实际状态同步进VM，现在先写好保护；本轮仅解释当前语句并澄清注释，没有代写判断或运行验收。
TASK-036已有运行保护的自主验证证据保留，不因本次措辞追问撤销；Q2原局部已掌握、TASK-038进行中/DoD以及25分类/63条、待复习32/已掌握31保持，不记新错题。
### 2026-10-18再次解释：先说界面要发生什么

用户反馈上一段仍难理解，后续先只描述具体操作：当前选001，点启动后正在给001记录温度，这时尝试选002，应保持001；点停止后再选002，才允许切换。
TODO2a要做的是在修改编号前先问“现在还在采集吗”，若还在采集就结束本次修改，若已停止才执行原来的编号赋值。
IsSampling保存“当前有没有在采集”的状态，true表示有，false表示没有；此处只用它决定是否继续执行下面那行编号修改，先不堆叠value、setter或事件机制。
教练此前讲解堆叠术语已明确更正，未运行新的验收、未代写判断，原掌握证据、任务和统计保持；真实启停同步仍由后续TODO5接通。
## 历史证据与掌握范围

2026-10-13 TASK-020审查：13个实际原窗口场景与三个必需DoD均通过，默认001、展开两项、改选写回、两ID读取与连续追加、旧行保留、清空/恢复、最小尺寸均验证。45个源码哈希不变，原exe启动关闭退出0，见[结果](../tasks/evidence/TASK-020/results.json)与[核对](../tasks/evidence/TASK-020/verification.json)。本条按学员在骨架中自主SelectedItem连接及其读取效果标已掌握；选项数组、ItemsSource、DataContext、原命令/表格由既有作品提供，不算新的自主设计，完整默认元数据/框架机制不一并认定掌握。骨架初始提示仍有旧文字，不记学员错题。下方未运行/未连接是重构问答时的历史。

Q1 保持待复习，TASK-020 保持🟡进行中。2026-10-13 启动时教练提供两个选项、ItemsSource、DataContext 和复用表格/命令，骨架构建退出 0、0 警告/错误。本次整理期间发现 Width 已 260，保留工作区这项修改；未运行新代码或验收，SelectedItem 核心 TODO 仍未完成，不把单纯源码改动标为已掌握。上述运行步骤是当前任务建议，不是本轮执行证据。

相关任务卡与验收来源：[TASK-019](../tasks/TASK-019.md)、[TASK-020](../tasks/TASK-020.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-combobox.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft SelectedItem 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.selector.selecteditem?view=windowsdesktop-10.0)
- [Microsoft IsEditable 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox.iseditable?view=windowsdesktop-10.0)

2026-10-18教学反馈：任务编号必须按实际执行依赖顺序排列，发现顺序错误直接修正编号及所有当前说明，不以额外文字让学员绕编号操作；本次仅修正注释和记录，不新增问答或错题，不改变任务状态与掌握证据。
