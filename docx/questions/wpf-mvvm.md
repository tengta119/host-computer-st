# WPF 窗口与 ViewModel

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：ViewModel 是什么，输入框的文字怎样进入读取命令？

- 日期：2026-10-12
- 状态：待复习
- 来源：TASK-013 启动的必要讲解；沿用 TASK-012 的对象与命令场景。

TASK-013 希望读取命令拿到当前设备 ID，而不去访问窗口 TextBox。ReadingViewModel 是教练定义的普通 C# 类，保存当前 DeviceId、历史 Readings、提示 StatusText 和两个命令。

```csharp
private readonly ReadingViewModel _viewModel = new ReadingViewModel();
```

new 创建这个窗口使用的一个数据对象，_viewModel 保存引用。窗口执行 `DataContext = _viewModel`，使输入框、表格、按钮从同一个对象取属性。MVVM 是这种分工的名称：本例窗口 XAML 负责显示，Reading 保存一条读数，ReadingViewModel 提供显示所用数据与动作；这些类名都是项目自定义，DataContext 是 WPF 属性。

输入框连接如下：

```xml
Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
```

左边是这个 TextBox 的 Text，右边 DeviceId 是同一个 ReadingViewModel 的公开属性。TwoWay 让初始 TEMP-001 显示到输入框，也让用户改字后写回对象。UpdateSourceTrigger=PropertyChanged 使文字一变就写回；它是 WPF Binding 的设置，不是项目通知方法 OnPropertyChanged。

ReadingViewModel 构造函数用 `new ReadTemperatureCommand(this)` 把自己交给读取命令。命令把传入引用存到自己的 _viewModel 字段，因此 `string id = _viewModel.DeviceId;` 取得当前 ID。两个地方的字段分别属于窗口与命令，但指向同一个对象，不另建第二份。

### 2026-10-12 追问：在 TextBox 更改值，会自动更新 ViewModel.DeviceId 吗？

当前连接会。用户改成 TEMP-002，WPF 写入原对象的 DeviceId，效果相当于 `_viewModel.DeviceId = "TEMP-002"`，不要求再补这行。

```csharp
set
{
    _deviceId = value;
    OnPropertyChanged("DeviceId");
}
```

这是原 DeviceId 的写入部分。value 接到新字符串，第一行保存到字段，之后 getter 返回该字段即可取到 TEMP-002。第二行报告属性变化，作用与“取得输入框新文字”不同。写回不需要等点读取，读取按钮之后使用已保存 ID。XAML 的 TwoWay/PropertyChanged 不应加入字面星号。

### 2026-10-12 追问：文字已经在 TextBox 改过，为什么 setter 还要通知界面？

只看用户在这一个输入框输入、setter 原样保存，第一行已完成保存；通知不是取得新文字的动作。还要考虑另一个来源：程序直接执行 `_viewModel.DeviceId = "TEMP-003"`。输入框没有被用户编辑，需要对象报告变化，WPF 才重读 DeviceId 并把新 ID 显示过去。

若另一块文字也绑定 DeviceId，它也靠这个通知刷新。当前项目没增加这块文字，这是说明用途的例子。通知携带属性名 "DeviceId"，不携带新 ID，WPF 再读值。TwoWay 允许双向连接，不能替代对象变化通知。

### 2026-10-12 追问：StatusText 为什么不采用 TwoWay 和 UpdateSourceTrigger=PropertyChanged？

StatusText 显示在 TextBlock，用户不在这块提示里输入，只需对象到控件的 OneWay。命令执行 `_viewModel.StatusText = "读取成功"` 后，setter 保存、通知，WPF 重读提示并更新文字。

DeviceId 在可编辑 TextBox，需要把用户输入写回，所以用 TwoWay。UpdateSourceTrigger 另外决定“何时写回”：PropertyChanged 是文字变时，LostFocus 是离开输入框时，也是 TextBox.Text 的默认时机。失焦写回只更新数据对象，不保证已保存文件或发送设备。StatusText 的 OneWay 无写回方向，就无需这项时机设置。

旧 Reading 中的 DeviceId 是当时创建那条读数时保存的值。改变当前 ViewModel.DeviceId 不会自动把旧历史改掉，原读取命令继续创建新 Reading 并 Add 同一集合。

## Q2：TASK-033 怎样让监控表格、条数和报警从同一个ViewModel取值？

- 日期：2026-10-17。
- 状态：已掌握。
- 来源：TASK-033开启；先前ViewModel/绑定骨架不代表完整属性通知已经自主掌握，沿用已有集合和CSV实践。

### 先找到温度实际存在哪里

TemperatureMonitorViewModel是教练为本题新建的普通C#类，定义在同名.cs文件中，不是WPF自带类型。
这个类里创建了一个ObservableCollection<double>保存全部温度，公开的Temperatures属性返回这一份集合。
窗口里的_viewModel指向一个TemperatureMonitorViewModel对象，教练提供的创建代码如下。

```csharp
private readonly TemperatureMonitorViewModel _viewModel = new TemperatureMonitorViewModel();
```

右边的new创建这一个对象，左边的字段保存它的引用，后续整个窗口复用这一份，不在点击按钮时再创建。
窗口构造方法中的_temperatures = _viewModel.Temperatures已经由骨架提供，它让原采集、清空、绘图、CSV继续通过旧名字操作同一集合，没有复制温度，也没有创建第二份集合。
因此点一次添加时，新增的温度实际保存在_viewModel那份对象的Temperatures集合里。

### 首步只接通表格的来源

在Task033Window.xaml.cs构造方法InitializeComponent之后的TODO 1a填写：

```csharp
DataContext = _viewModel;
```

DataContext是WPF窗口提供的属性，这里给当前窗口指定“绑定从这一个_viewModel对象查找数据”，不是创建对象，也不是复制集合。
HistoryDataGrid会从窗口继承这个来源，除非另外给它设置自己的DataContext，本题不需要额外设置。
接着在Task033Window.xaml的HistoryDataGrid开始标签中添加：

```xml
ItemsSource="{Binding Temperatures, Mode=OneWay}"
```

左边ItemsSource是这个WPF表格的行数据来源，右边的Temperatures是自定义ViewModel公开属性名，Binding让表格去读取它。
OneWay在这里让数据对象提供表格来源，界面不反向替换这个集合；集合中的新增与清空仍通过原ObservableCollection的通知更新表格。
表格内部那条Binding Path=.负责显示每行自己的double温度，首步不修改它。
骨架已移除原HistoryDataGrid.ItemsSource直接赋值，所以尚未完成首步时，左图和条数可以变化，表格仍为空，这是等待连接的占位效果。
运行项目，在停止状态点击一次添加，应看到表格一行24，再加到五条应看到五行，折线仍最多三点。
不需要先改RefreshDisplay或通知代码才能完成这一观察。

### 后续文字怎样进入属性

ViewModel中的ReadingCountText保存“完整历史条数：5”这段文字，AlarmText保存“温度正常”“温度超限”或“暂无温度”。
这两个名字都是自定义属性，窗口里两个同名TextBlock是另外两个具体控件；相同名字不代表同一个对象，也不会自动连接。
后续TODO 4才给这两个TextBlock的Text设置绑定，分别读取ViewModel同名属性，方向均为OneWay。
先看已有属性的完整普通写法，省略通知时如下。

```csharp
public string AlarmText
{
    get
    {
        return _alarmText;
    }
    set
    {
        _alarmText = value;
    }
}
```

_alarmText字段实际保存文字，get在读取AlarmText时返回当前字段值，set在给AlarmText赋值时执行。
例如AlarmText = "温度正常"会让C#把右侧字符串交给value，_alarmText = value才把它保存到这个ViewModel对象里。
仅保存字段还没有告诉已绑定的文字控件重新读取，这正是TODO 3需要补通知的原因。

### 通知让已经绑定的文字重新读取哪个属性

INotifyPropertyChanged是.NET提供的属性变化通知接口，教练已准备PropertyChanged事件和OnPropertyChanged辅助方法。
OnPropertyChanged是本题自定义普通方法，参数是发生变化的公开属性名，不是控件名、字段名或要显示的结果。
例如AlarmText的set存好value后，调用OnPropertyChanged("AlarmText")，通知会让已经绑定这个属性的控件重新执行get取得新文字。
这与集合Add发出的通知不同，ObservableCollection负责表格新增/删除行，ViewModel的属性通知负责ReadingCountText与AlarmText这两段文字变化。
原通知方法中的PropertyChanged(this, new PropertyChangedEventArgs(propertyName))表示发出属性变化事件，this是当前ViewModel，参数对象携带要重读的属性名；这段基础代码已提供，不要求首步重写。

### 最后才把原判断搬到数据对象

RefreshDisplay(double threshold)是教练预留的普通方法，threshold参数接收窗口已经验证生效的阈值。
TODO 2在这里先更新真实条数，再处理空历史，最后取原集合最新温度与threshold比较，分别给ViewModel的两个文字属性赋值，不访问TextBlock。
Count更新要放在空集合return之前，保证清空后条数能变回0。
TODO 5完成以后，窗口原UpdateTemperatureAlert仅转交_viewModel.RefreshDisplay(_alarmThreshold)，原集合变化、应用阈值和加载配置的调用入口仍可复用。
窗口原ReadingCountText.Text赋值也要移除，否则直接赋值会覆盖已经建立的Text绑定；先把属性更新/通知/绑定接通，再完成这些替换。
定时器、绘图、阈值输入和文件保存对话框本题可以暂留窗口，不需要一次引入命令或重写整套配置。

### 开启时证据与掌握边界

实际71项骨架检查和原exe启动关闭通过，验证进程曾临时设置DataContext与表格绑定确认一条显示一行、五条显示五行，随后撤销；没有把该运行内诊断写进学员源码或算作学员已完成。
源码中首步连接、RefreshDisplay、通知调用、文字绑定与旧显示迁移仍留TODO，三个DoD均未勾选，Q2待复习，原Q1和其他已验证能力保持。
任务路径与命令见[TASK-033](../tasks/TASK-033.md)，当前25分类/60条、待复习33/已掌握27。

### 2026-10-17 TASK-033实际通关与掌握边界

Q2按同一对象DataContext/表格与文字绑定、RefreshDisplay读取完整集合更新条数和报警、两个setter正确名称通知、窗口转交刷新与移除直接赋值的自主范围标已掌握。
最终97项实际检查全通过，绑定在初始/五条后/全操作后均存活且指向原ViewModel，两个公开属性写入后的实际通知和界面刷新已验证，见[TASK-033](../tasks/TASK-033.md)。
首次清空时Count已为0但报警残留，原因是空分支return前没有改变AlarmText；return只停止后续执行，不会自动恢复属性初始文字。
教练依用户既有状态文案收尾要求仅补AlarmText = "暂无温度"，该行不计学员自主实现，其余状态计算/通知/绑定保留。
自定义类、唯一集合及窗口兼容引用、完整属性骨架和通知辅助方法为教练提供，不扩大为独立完整架构设计，也不因已使用nameof就推定相关语法全部掌握。
Q1继续待复习，当前25分类/60条、待复习32/已掌握28，前述Q2待复习/骨架未接通为历史，下一张仅建TASK-034未开始卡片。
## Q3：TASK-035 怎样把输入框文字和应用阈值交给同一个ViewModel？

- 日期：2026-10-17。
- 状态：已掌握。
- 来源：TASK-035开启，沿用TASK-033/034已验证的同对象绑定和命令应用；输入双向写回与本次参数迁移尚无自主证据。

### 先分清输入文字和真正生效的数值

当前窗口已有一个TemperatureMonitorViewModel，_viewModel指向它，DataContext也使用这个对象，不需要再创建另一份。
本题在这个自定义类中新增ThresholdInputText、AlarmThreshold、ThresholdFeedbackText和CurrentThresholdText属性，这些名字是教练命名的，不是WPF内置属性。
ThresholdInputText保存输入框文字，例如用户正在输入的29；AlarmThreshold保存已经验证并生效的double数值，例如当前27。
因此只输入29而未点击应用，输入文字应为29，有效阈值仍为27，报警和配置保存继续使用27。
ThresholdFeedbackText保存输入是否合法或应用是否成功的说明，CurrentThresholdText的get用当前有效数值组成显示文字。
本题已提供这些属性的完整get/set和通知代码；输入属性的set只存value并报告变化，不自动解析或应用阈值。

### 首步只观察输入文字进入数据对象

在Task035Window.xaml中，把ThresholdInput原来的Text="27"替换为：

```xml
Text="{Binding ThresholdInputText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
```

左边是这个TextBox的Text属性，右边ThresholdInputText是当前ViewModel的公开属性。
TwoWay表示两个方向：初始从ViewModel取文字显示到输入框，用户修改输入框后再把新文字写入同一ViewModel。
UpdateSourceTrigger决定用户编辑后何时写回，PropertyChanged在这里表示文字一变就写回，不等输入框失去焦点。
这里的PropertyChanged是WPF绑定设置的选项名，不是要求再调用一次我们的OnPropertyChanged方法。
例如键入29，WPF会把字符串"29"交给ThresholdInputText的set，value得到它，_thresholdInputText = value保存这段文字。
这个动作没有执行double.Parse，也没有修改AlarmThreshold。

再把新增ThresholdInputObservationText的Text改为：

```xml
Text="{Binding ThresholdInputText, Mode=OneWay}"
```

这块TextBlock只显示同一ViewModel收到的输入，用户不在这里编辑，所以用OneWay。
输入框写回属性后，提供的setter发属性变化通知，这块观察文字重新读取ThresholdInputText，应立即显示29。
从工作区根目录运行WpfBae，输入29但不点应用，应看到观察文字29、当前有效阈值27；再输入abc，观察文字显示abc，有效阈值仍为27。
这一步只验证文字连接，不要求先实现命令或数值验证，也不应点击保存配置。

### 后续才把应用按钮连接到命令

ApplyThresholdCommand是教练新增的命令类，也保存在原ViewModel里，命令构造时收到同一个this，逻辑连接与TASK-034相同。
TODO 2在命令Execute里调用_viewModel.ApplyThreshold()，同时把应用按钮Click删除并改为Command绑定ApplyThresholdCommand，避免两条应用路径并存。
ApplyThreshold是ViewModel的新普通方法，目前只有安全占位，不改变有效阈值。
TODO 3参照保留在旧窗口ApplyThresholdButton_Click中的原验证算法，把读取ThresholdInput.Text改为读取ThresholdInputText，把反馈控件赋值改为给ThresholdFeedbackText赋文字。
原Parse/catch、NaN检查、20至30范围检查按原规则复用；验证失败时返回，保留AlarmThreshold，全部检查通过才把候选值写入有效属性。
成功后调用RefreshDisplay(AlarmThreshold)重判已有温度，不调用Add或Clear，也不自动保存配置。
AlarmThreshold的setter已提供对CurrentThresholdText的通知，因此读取这个显示属性的文字控件可以重新显示新有效值。

### 显示绑定和旧直接赋值要一起整理

TODO 4将CurrentThresholdText与ThresholdFeedbackText两个控件的Text分别绑定到ViewModel同名属性。
新命令接通后删除旧ApplyThresholdButton_Click和UpdateThresholdDisplay及其调用，避免后续旧代码直接给Text赋值而替换绑定。
此处ViewModel.CurrentThresholdText是数据属性，窗口的CurrentThresholdText是一个TextBlock，名字相同但不是同一对象，必须通过Binding连接。
不要仅删除原验证算法却不把它移入新方法，也不要只绑定显示而继续保留旧控件写入路径。

### 配置和报警只保留一个有效值来源

骨架把窗口原_alarmThreshold字段换成一个兼容属性，它的get/set直接转交_viewModel.AlarmThreshold，不再单独存值，所以过渡阶段也只有一个有效数值。
TODO 5把保存/报警等旧入口改为直接读取_viewModel.AlarmThreshold，所有引用改完后删除窗口这个兼容属性。
保存只读有效数值，不能把ThresholdInputText直接当成有效配置；未应用或拒绝的文字不应写入文件。
加载仍先处理缺失/损坏/null/范围，合法后恢复到ViewModel.AlarmThreshold和ThresholdInputText，再刷新报警，移除LoadThresholdSettings中的输入框及有效显示Text直接赋值。
加载是在程序代码中给输入属性赋值，提供的属性通知会让TwoWay绑定的输入框显示恢复后的文字，不需要再写ThresholdInput.Text。
本题文件读写仍可留窗口，不需要再设计配置服务或新框架。

### 开启时证据与边界

实际74项骨架检查和原exe启动关闭通过，验证进程临时接通两个输入观察绑定后，输入29会进入原VM并显示29，有效值保持原26且不写配置，随后撤销临时绑定。
该诊断未写入学员源码，不算学员已完成；源码中的输入绑定、应用命令/验证迁移、显示绑定和旧配置连接仍留五组TODO。
旧Click应用路径临时保留以便原功能可运行，不能把它的成功当成本题命令验收通过；Q3待复习、三个DoD未勾选，当前25分类/62条、待复习33/已掌握29。
实际文件路径和命令见[TASK-035](../tasks/TASK-035.md)。

### 2026-10-18追问：为什么输入框TwoWay，观察文字OneWay，PropertyChanged有什么用？

这两个控件读取的是同一个ViewModel.ThresholdInputText，但用户只能在ThresholdInput输入框编辑文字，不能在ThresholdInputObservationText这块TextBlock里编辑。
TwoWay让ViewModel属性中的27先显示到输入框，也让用户在输入框改成29后，把字符串29写回同一个属性，所以输入框用双向。
OneWay让观察TextBlock从这个属性取得文字，属性后续变成29也继续显示29；它没有用户编辑写回的需求，所以只需要单向，不表示只读取一次。
Mode决定数据允许往哪些方向走，UpdateSourceTrigger决定输入框到ViewModel这一个方向在什么时机写回。
这里Source指ViewModel的ThresholdInputText，PropertyChanged选项表示输入框Text一变就写回，不等用户离开输入框；它本身不执行数值解析、应用阈值或保存文件。
如果输入框省略UpdateSourceTrigger，TextBox.Text默认在失去焦点时写回，例如点击另一控件或用Tab移开，观察文字通常到这时才变化。
当前链为用户编辑输入框→绑定把新文字交给ThresholdInputText的set→value存入字段→提供的OnPropertyChanged通知属性变化→观察TextBlock重新读取并显示。
UpdateSourceTrigger=PropertyChanged负责输入框往属性写入的时机，setter的OnPropertyChanged("ThresholdInputText")负责属性变化后让显示控件重读，两者不是同一个调用，也不能互相替代。
先把输入改成29且光标仍在输入框，预期观察文字已经是29；本题首步不点应用时AlarmThreshold仍27。
本轮只根据可见代码答疑，没有运行新验收或修改学员代码；已见学员写了两处绑定，仅此不推定已自主运行验证，Q3继续待复习，TASK-035状态与DoD保持，25分类/62条、待复习33/已掌握29不变。
### 2026-10-18实际审查：字段值改了，为什么显示和报警没有立即更新？

当前ApplyThreshold成功路径给_alarmThreshold字段赋值，AlarmThreshold的get能返回新数值，所以保存文件可得到新值，但没有执行AlarmThreshold的set，也就没有执行其中的CurrentThresholdText通知。
改为AlarmThreshold = threshold会进入该属性的set，set先保存字段，再通知读取CurrentThresholdText的控件重新取文字；不要另建第二份有效参数。
报警还需要RefreshDisplay(AlarmThreshold)按已存在的最新温度重新比较，通知显示有效阈值不会自动替你执行这次判断。
首个小改动是在当前成功路径用公开属性赋值替换字段赋值，接着调用已有RefreshDisplay，原输入验证与反馈保留；停止状态同一26.5应用26/27应立即超限/正常，不应等下一次采集。
本轮182项173通过、9失败，6项为20/30/26.5成功后的有效显示与通知，3项为停止/运行中即时报警；DoD1/2未通过、3通过，详见[TASK-035](../tasks/TASK-035.md)。
已验证局部自主范围包括TwoWay即时输入写回/观察显示、命令调用原VM、合法与非法检查/NaN拒绝、失败保留有效值、保存只读有效值与加载恢复属性和原绑定存活；不因两处未完成撤销这些证据。
窗口兼容_alarmThreshold属性仍转交VM同一值，没有第二份有效存储；删除无用兼容入口可在核心修复后按原步骤整理，不新增失败理由。
Q3待复习，Q1/Q2范围保持，本轮未代改核心代码，以上结果来自实际审查，25分类/62条、待复习33/已掌握29不变，上方骨架未接通为历史。
### 2026-10-18 TASK-035最终复查与掌握边界

学员已自主改为通过AlarmThreshold属性赋值并调用RefreshDisplay，182项实际检查全通过，TASK-035三个DoD通关，见[最终结果](../tasks/evidence/TASK-035/review-final/results.json)。
Q3按现有骨架的输入TwoWay即时写回/观察显示、原VM应用命令、输入验证迁移/拒绝保留有效值、属性赋值触发有效显示通知、已有温度即时刷新、保存读取有效值与合法加载恢复属性及绑定存活的自主范围标已掌握。
教练提供了属性get/set及通知基础、新命令类签名和旧有效值兼容入口，不扩大为独立完整参数架构设计；nameof等其他语法机制不因代码存在一并升级，Q1仍待复习、Q2已掌握范围保持。
本轮源码未代改，原用户配置和CSV未动，25分类/62条、待复习32/已掌握30，前述首次失败/Q3待复习为历史；只创建TASK-036未开始卡片。
## 历史证据与掌握范围

Q1 保持待复习。2026-10-12 TASK-013 的 10 个场景验证读取、Trim/空白 return、Reading 创建、Add 与提示，学员自主修复漏 return。输入绑定/完整属性/通知/对象连接由骨架提供，综合机制不按通关全部升级。教练未展开修改来源导致的追问保留为教学反馈，不记错题；漏 return 的实际实践错误保留原错题记录。

相关任务卡与验收来源：[TASK-012](../tasks/TASK-012.md)、[TASK-013](../tasks/TASK-013.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-mvvm.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-control-when-the-textbox-text-updates-the-source)
- [Microsoft 属性通知文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-implement-property-change-notification)
- [WPF 数据绑定概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)
