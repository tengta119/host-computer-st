# WPF 界面区域与 UserControl

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：TASK-014 的历史控件是什么，内部 Binding 从哪里取数据？

- 日期：2026-10-12
- 来源：TASK-014 开始时的必要讲解。
- 状态：待复习

TASK-014 的窗口里，“读取历史”标题、列表和清空按钮被装进同一个区域。ReadingHistoryControl 是本项目自定义控件类，继承 WPF 的 UserControl；它放进窗口显示，不是又打开一个 Window。

`ReadingHistoryControl.xaml` 描述这块区域的元素，后台 .xaml.cs 定义配对的 C# 类。x:Class 指向完整类名，partial 是 C# 关键字，让手写与构建生成部分共同组成这个类。构造中的 InitializeComponent 是 WPF 构建生成的方法，用来创建 XAML 描述的元素。

窗口起始标签已有：

```xml
xmlns:history="clr-namespace:WpfBae.Exercises.Task014"
```

history 是教练选的前缀，右边指出控件的 C# 命名空间。之后窗口内部 `<history:ReadingHistoryControl />` 创建并放置这个类的一个控件；不是按文件路径读取一段文本。标题、列表、按钮由它自己的 XAML 提供。

窗口后台创建一份数据对象：

```csharp
private ReadingViewModel _viewModel = new ReadingViewModel();
```

ReadingViewModel 是已有自定义类，new 创建 TASK-014 窗口使用的对象，字段保存引用。`DataContext = _viewModel` 给窗口指定数据来源。历史控件没另设 DataContext，接到窗口后沿用这个对象；内部列表和清空按钮再取它的 Readings 与 ClearCommand。

这叫 DataContext 的属性值继承：沿用父元素设置的同一个对象。不是复制历史，也不是 ReadingHistoryControl 类继承 ReadingViewModel 类。若控件自己又 new 一份 ViewModel 并设为来源，就会显示另一份空集合，窗口读取可能成功而内部列表没有行。

原首个观察是在控件 StackPanel 内放 `<TextBlock Text="读取历史" />`，对应屏幕上这块区域的标题。当前内容已完成，复习直接读 [控件 XAML](../../WpfBae/Exercises/TASK-014-history-usercontrol/ReadingHistoryControl.xaml)，不要求重做。列表没写 x:Name 仍能 Binding 取属性；x:Name 用于命名控件，不是数据绑定必需条件。

## 历史证据与掌握范围

Q1 保持待复习。2026-10-12 学员在骨架中组合标题/列表/按钮、应用 ItemsSource/Command 已通过 9 个真实窗口场景，窗口与内部控件同一个 DataContext/集合/命令。类定义、命名空间、窗口对象连接和属性继承机制由骨架提供，保留实践范围而不升级综合条目。

相关任务卡与验收来源：[TASK-013](../tasks/TASK-013.md)、[TASK-014](../tasks/TASK-014.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-usercontrol.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [UserControl 官方说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol?view=windowsdesktop-10.0)
- [DataContext 属性继承说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.datacontext?view=windowsdesktop-10.0)
