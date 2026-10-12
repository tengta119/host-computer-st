# WPF 界面区域与 UserControl

> [返回问答索引](./README.md)

## Q1：TASK-014 的历史控件是什么，内部 Binding 从哪里取数据？

- 日期：2026-10-12
- 来源：TASK-014 开始时的必要讲解。
- 状态：待复习
- 证据边界：2026-10-12 学员自主补齐控件内标题、列表和清空按钮及 ItemsSource/Command 绑定，9 个实际窗口场景全部通过；此界面组合和既有绑定应用范围已记入学习进度。控件类定义、命名空间与父窗口 DataContext 连接由教练提供，本问答关于控件构造和属性值继承的机制保持待复习，不据此认定自主设计或全部概念理解。

原 TASK-013 窗口直接放置历史标题、ListBox 和清空按钮。TASK-014 把这三个元素放进一个 UserControl，窗口只需放置这个控件，就能显示它内部的区域。UserControl 是 WPF 提供的基类，ReadingHistoryControl 是本题自定义的类名；后者继承前者，放在窗口内部，不用 Show 打开。

`ReadingHistoryControl.xaml` 描述界面，`ReadingHistoryControl.xaml.cs` 写对应的 C# 类。XAML 的 `x:Class` 对应完整类名；`partial` 是 C# 关键字，让手写后台与 XAML 构建生成部分共同组成同一个类。构造函数中 `InitializeComponent()` 是 WPF 构建生成的方法，负责创建 XAML 里描述的界面。

窗口里的连接由骨架提供：

```xml
xmlns:history="clr-namespace:WpfBae.Exercises.Task014"
```

这是 Window 起始标签中的命名空间声明，`history` 是自定义前缀，指向控件所在的 C# 命名空间；随后窗口用 `<history:ReadingHistoryControl ... />` 创建并放置一个控件实例。新控件内部已有 StackPanel 容器，它默认把内部元素从上往下排列。

窗口后台复用原 ReadingViewModel 类型，但创建的是 TASK-014 自己的一份实例：

```csharp
private ReadingViewModel _viewModel = new ReadingViewModel();
```

`new` 是 C# 创建对象的语法；字段 `_viewModel` 保存该对象的引用。窗口调用 `DataContext = _viewModel` 设置 WPF 提供的 DataContext 属性。内部控件没有自行设置这个属性，会从父元素取得同一个对象；列表和按钮再从此对象的公开 Readings/ClearCommand 属性取集合和命令。这里的属性值继承不是复制数据，也不是让控件类继承数据类。

若控件后台另建一个 ReadingViewModel 并设置 DataContext，读取按钮操作窗口那份集合，历史控件显示另一个对象的集合，可能出现“读取成功但列表没有新增”的现象。原因是两边的数据来源不同。当前骨架让窗口、历史控件及其内部元素共用一个 ViewModel，控件只负责组合界面，读取和清空继续交给原命令。

首个实践只在控件的 StackPanel 内添加 `<TextBlock Text="读取历史" />`。TextBlock 是 WPF 文字控件，Text 是显示文字的属性；实际运行后，标题应位于窗口的读取按钮与状态提示之间。后续再复用 TASK-013 已有 ListBox.ItemsSource 和 Button.Command 的 Binding 写法；读数追加与清空的完整行为仍须实际运行验证。

依据：[UserControl 官方说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol?view=windowsdesktop-10.0)、[DataContext 属性继承说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.datacontext?view=windowsdesktop-10.0)。

2026-10-12 TASK-014 审查补充：实际窗口、历史控件、内部 ListBox/清空按钮 DataContext 相同，ItemsSource 引用原 Readings，Command 引用原 ClearCommand。连续读取、换 ID、空白保护、清空和恢复均通过，见 [运行结果](../tasks/evidence/TASK-014/results.json)；源码未修改，见 [核对](../tasks/evidence/TASK-014/verification.json)。ListBox 未命名仍能通过 ItemsSource 绑定显示与刷新；x:Name 用于标识控件，不是 Binding 按数据属性取值的前提。本项为提交检查依据，不新增单独口试或必需验收项。
