# TASK-014：把“历史标题、列表、清空按钮”放进一个控件

> 2026-10-13 白话重写。先看窗口怎样放入历史区域，再看区域里面的数据从哪里来。原任务已通关，正文中的示例与当前源码分别说明，验收材料在文末。

## 1. 这次移动的是界面，读取动作仍用原命令

TASK-013 窗口直接写历史标题、ListBox 和清空按钮。TASK-014 把这三项放到 ReadingHistoryControl 里，窗口只放这个控件。

```text
窗口
  设备 ID 输入框
  读取按钮
  ReadingHistoryControl
    读取历史标题
    ListBox
    清空按钮
  状态文字
```

你看到的读取/清空行为继续沿用原命令，温度仍是模拟 26.5 °C。新地方是“这一块界面放在哪里写”，不需要重写业务。

## 2. UserControl 是 WPF 类，ReadingHistoryControl 是自定义类

UserControl 是 WPF 提供的基础类，帮助我们把几个控件组合成一块可放到窗口中的界面。

ReadingHistoryControl 是教练给本题历史区域取的类名。它继承 UserControl：

```csharp
public partial class ReadingHistoryControl : UserControl
{
    public ReadingHistoryControl()
    {
        InitializeComponent();
    }
}
```

new 这个类时，会创建一块具体的历史区域。它是窗口里的控件，不是另一个独立窗口；当前不调用 Show 来打开它。

Task014Window 继承 Window，负责整个窗口；ReadingHistoryControl 继承 UserControl，负责其中一块区域。

## 3. 控件的 XAML 与 C# 怎样对应

控件 XAML 开头：

```xml
<UserControl x:Class="WpfBae.Exercises.Task014.ReadingHistoryControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

x:Class 指明这份界面属于哪个 C# 类。它必须对应后台 namespace 和类名。

partial 是 C# 语法，让手写后台与 XAML 构建生成的部分一起组成这个类。InitializeComponent 是 WPF 生成的初始化方法，执行后创建 XAML 描述的内部控件。

using System.Windows.Controls 让后台能用短名称 UserControl。ReadingHistoryControl 这个自定义名字没有藏在 using 里，它就是我们声明的类。

## 4. history 前缀帮助窗口找到自定义控件

窗口起始标签里有：

```xml
xmlns:history="clr-namespace:WpfBae.Exercises.Task014"
```

history 是教练自取的 XAML 前缀，右边指向控件所在的 C# 命名空间。

窗口随后写：

```xml
<history:ReadingHistoryControl x:Name="HistoryControl"
                               Margin="0,16,0,0" />
```

WPF 按这个标签创建 ReadingHistoryControl 对象，放在当前窗口的 StackPanel 里。HistoryControl 是这个具体控件的名字。

一个是类名 ReadingHistoryControl，一个是前缀 history，一个是当前对象名 HistoryControl；各自负责“创建什么”“到哪找类”“怎样标识这一个”。

## 5. 控件里面，一个 StackPanel 放三项内容

当前代码去掉旧 TODO 后，可这样读：

```xml
<StackPanel>
    <TextBlock Text="读取历史" />
    <ListBox ItemsSource="{Binding Readings, Mode=OneWay}"
             Height="170" />
    <Button x:Name="ClearButton"
            Content="清空历史"
            Command="{Binding ClearCommand, Mode=OneWay}"
            HorizontalAlignment="Left"
            Padding="12,6"
            Margin="0,12,0,0" />
</StackPanel>
```

外层 UserControl 包住一个 StackPanel，StackPanel 再从上到下排标题、列表、按钮。这样可以在 UserControl 的一个内容位置组合多项界面。

这次 ListBox 实际没有 x:Name，仍能绑定、显示和更新；不能把骨架建议的 HistoryListBox 名称说成当前已经写入。

## 6. 窗口先创建自己的一份 ViewModel

窗口后台：

```csharp
private ReadingViewModel _viewModel = new ReadingViewModel();

public Task014Window()
{
    InitializeComponent();
    _viewModel.StatusText = "TASK-014：先完成控件内的历史标题（TODO 1）。";
    DataContext = _viewModel;
}
```

ReadingViewModel 复用 Task013 的类，new 创建的却是这个 Task014 窗口自己的对象。复用类定义，不表示多个任务窗口必须共用一份历史。

字段 _viewModel 保存它，DataContext 再把它交给窗口里的绑定。初始 TODO 提示是实际源码文案，不能从注释或文案推断功能还没完成。

## 7. 历史控件没有另设 DataContext，会用窗口那份对象

控件构造函数只有 InitializeComponent，没有写新的 DataContext。

窗口设置 DataContext 后，当前外层 StackPanel、历史控件及其内部元素通过父级关系取得同一个 ReadingViewModel。可以这样看当前查找来源：

```text
窗口 DataContext：_viewModel
  → 外层 StackPanel
    → HistoryControl
      → 内层 StackPanel
        → ListBox / ClearButton
```

这里说“继承 DataContext”，意思是没有另设值时，使用父元素给的属性值。没有复制 ViewModel，也没有为每个控件创建新的集合。

## 8. “继承 UserControl”和“取得父级 DataContext”不是同一件事

类声明中的：

```csharp
ReadingHistoryControl : UserControl
```

说的是 C# 类继承，让自定义类拥有 WPF 控件能力。

界面中取得父级 DataContext，说的是 WPF 按父子元素关系使用已有属性值。ReadingHistoryControl 的类并没有继承 ReadingViewModel。

窗口把数据对象交给控件使用，不会把控件变成那个数据类。先分清类定义与界面里具体对象的关系，再读“继承”这个词。

## 9. 列表怎样取得窗口正在追加的那份集合

内部 ListBox：

```xml
ItemsSource="{Binding Readings, Mode=OneWay}"
```

它从已取得的 ReadingViewModel 读取公开 Readings。该 getter：

```csharp
public ObservableCollection<Reading> Readings
{
    get
    {
        return _readings;
    }
}
```

返回原集合，没有复制。所以窗口读取命令 Add 的记录，就是内部列表连接的记录。

控件的摆放位置变了，数据集合没有因此搬到另一个对象中。列表仍按 Reading.ToString 显示整行；DataTemplate 是后面 TASK-016 的任务。

## 10. 清空按钮怎样取得原清空命令

```xml
Command="{Binding ClearCommand, Mode=OneWay}"
```

左侧是这个清空 Button 的 Command，右侧读取同一 ViewModel 的 ClearCommand getter，取得原 ClearHistoryCommand。

命令保存同一个 ViewModel，Clear 的也是原集合。控件内部不用再写 ClearButton_Click，也不用直接操作 ListBox.Items。

按钮 Condition/执行/事件的基础在 TASK-012 已逐步说明。本题只复用：Count>0 才能清空，Clear 后通知条件重查。

## 11. 为什么提示在窗口，清空在控件里，两边仍能同步

点击控件内清空后，命令执行：

```csharp
_viewModel.Readings.Clear();
_viewModel.StatusText = "已清空";
```

集合通知让内部列表变空；StatusText 的属性通知让窗口里的状态 TextBlock 重读。

控件不需要去找窗口的 StatusTextBlock，也不需要调用窗口的显示方法。两边都从同一数据对象取得各自需要的内容。

如果控件后台另 new ReadingViewModel 并设 DataContext，内部绑定会改用第二份对象：窗口读到新记录，控件却看另一个空集合。当前骨架正是通过不另建对象来保持连接。

DataContext=this 同样会让内部绑定找控件自己的 Readings/ClearCommand；当前控件没有提供这些公开属性，因此不这样写。

## 12. 读取与清空条件的通知链继续保留

```text
窗口读取按钮 → ReadCommand.Execute
→ 原集合 Add → 内部 ListBox 新增一行
→ 集合事件通知清空命令 → WPF 重查 Count>0 → 清空启用

内部清空按钮 → ClearCommand.Execute
→ 原集合 Clear → 内部列表为空
→ WPF 重查 Count>0 → 清空禁用
→ StatusText="已清空" → 窗口提示更新
```

集合事件、命令条件事件、状态属性事件各管一件事。UserControl 负责组合界面，没有替这些对象添加另一套通知。

空字符串/纯空格在读取命令中提示并 return，不触发 Add/Clear，旧历史保持。改输入只改 ViewModel.DeviceId，不改旧 Reading 的 ID。

## 13. 名字的作用与查找范围

HistoryControl 是窗口中这个控件对象的名字；ClearButton 是控件内部按钮的名字。公开数据属性 Readings/ClearCommand 又是另一类名字。

Binding Readings 按数据属性找集合，不按 ListBox 的 x:Name 找。因此没有 ListBox 名称不影响本题绑定。

窗口与单独加载的 UserControl 各有自己的名称范围。程序查内部 ClearButton 时，应以控件的范围查，不假设从窗口直接查同名元素总能取得它。

这叫名称范围，先理解“从哪一块 XAML 查名字”即可。本题并没有要求你为了绑定实现额外名称查找代码。

## 14. 每一层的间距要对着对象看

| 属性所在位置 | 当前值 | 留白在哪里 |
| :--- | :--- | :--- |
| 外层窗口 StackPanel | Margin=20 | 整块内容与窗口边缘 |
| HistoryControl | Margin=0,16,0,0 | 整个历史区域上面 |
| 内部清空按钮 | Margin=0,12,0,0 | 列表与清空按钮之间 |
| 状态 TextBlock | Margin=0,12,0,0 | 提示文字上面 |
| 按钮 Padding | 12,6 | 按钮文字与边缘之间 |

内部列表 Height=170。布局上下排列，每项按当前大小占空间；名字是否存在不决定间距。

若只改 HistoryControl 上 Margin，观察的是整块区域与上方读取按钮；不会只改变某一行的文字间距。

## 15. 这种复用现在具备什么条件

窗口可以通过一个标签放入历史区域，不必重复写三个控件。再放另一个标签会创建另一个控件对象，不是重复显示同一个对象。

当前控件依赖 DataContext 上有 Readings 和 ClearCommand。要在其他窗口使用，需要给它满足这些属性的对象。它还没有设计独立的数据接口或通用控件 API。

这次没有实际验收多个历史控件同时使用，也不要求额外依赖属性、模板或第三方框架。不要从“封装成了控件”推定已经是完整通用组件。

## 文末查阅：文件、运行与历史依据

实际文件：[窗口 XAML](../../WpfBae/Exercises/TASK-014-history-usercontrol/Task014Window.xaml)、[窗口后台](../../WpfBae/Exercises/TASK-014-history-usercontrol/Task014Window.xaml.cs)、[控件 XAML](../../WpfBae/Exercises/TASK-014-history-usercontrol/ReadingHistoryControl.xaml)、[控件后台](../../WpfBae/Exercises/TASK-014-history-usercontrol/ReadingHistoryControl.xaml.cs)、[任务卡](../tasks/TASK-014.md)。

一个可选小观察：只把控件标题改成“温度读取历史”，重跑本任务看文字是否出现在该区域，再恢复。当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-017，复跑旧题需明确临时入口并恢复。本轮未运行或改入口。

2026-10-12 历史结果：[9 个场景](../tasks/evidence/TASK-014/results.json)、[构建/启动/源码核对](../tasks/evidence/TASK-014/verification.json)、[审查程序](../tasks/evidence/TASK-014/Review.cs)、[三条历史](../tasks/evidence/TASK-014/history-three-readings.png)、[清空后](../tasks/evidence/TASK-014/history-cleared.png)。连续读取/换 ID、两种空白、清空和恢复全部通过，同 ViewModel/集合/命令保持。

图片是当时已显示窗口的内容渲染；引用与绑定的证据结合 JSON/审查代码，不把图片说成本轮新运行。先前整理核对八个任务/复用业务文件与历史哈希一致，App.xaml 因后续入口推进不同。

自主范围是骨架中的内部界面组合与 Readings/ClearCommand 绑定。类定义、命名空间和父级数据连接由教练提供，UserControl Q1 机制仍待复习。未发现新的典型错误，也不把无 ListBox 名称追加成验收障碍。

原答疑见 [UserControl 问答](../questions/wpf-usercontrol.md)，名称机制见 [Microsoft 名称范围](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-xaml-namescopes)。下一篇把 [重复按钮属性放到 Style](./TASK-015-wpf-styles.md)。
