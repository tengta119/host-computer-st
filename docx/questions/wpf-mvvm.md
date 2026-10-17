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
- 状态：待复习。
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

## 历史证据与掌握范围

Q1 保持待复习。2026-10-12 TASK-013 的 10 个场景验证读取、Trim/空白 return、Reading 创建、Add 与提示，学员自主修复漏 return。输入绑定/完整属性/通知/对象连接由骨架提供，综合机制不按通关全部升级。教练未展开修改来源导致的追问保留为教学反馈，不记错题；漏 return 的实际实践错误保留原错题记录。

相关任务卡与验收来源：[TASK-012](../tasks/TASK-012.md)、[TASK-013](../tasks/TASK-013.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-mvvm.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-control-when-the-textbox-text-updates-the-source)
- [Microsoft 属性通知文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-implement-property-change-notification)
- [WPF 数据绑定概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)
