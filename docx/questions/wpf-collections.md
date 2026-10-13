# WPF 集合与历史列表

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：ObservableCollection、ListBox 和 ItemsSource 分别是什么，记录怎样出现在列表里？

- 日期：2026-10-11
- 状态：已掌握（限在已有绑定骨架上应用集合 Add/Clear 驱动列表更新）
- 来源：TASK-011 开始时的必要讲解；结合学员此前对自定义类型来源和属性语法的反馈，不将骨架提供视为理解。

TASK-011 每点一次读取，历史列表应增加一行；清空则所有行消失。先找到实际保存记录的对象：ReadingHistoryState 是教练自定义类，它里面创建了一个 .NET 的 ObservableCollection<Reading>，保存多条 Reading。

```csharp
private ObservableCollection<Reading> _readings = new ObservableCollection<Reading>();

public ObservableCollection<Reading> Readings
{
    get
    {
        return _readings;
    }
}
```

new 创建空集合，_readings 保存引用；尖括号中的 Reading 是已有自定义读数类，规定集合中每项是一条读数。Readings 是自定义公开属性名，get 返回原集合，不创建另一份。没有 set 限制重新给属性赋集合，但仍可调用这个集合的 Add/Clear。

窗口的 _state 保存一个 ReadingHistoryState，DataContext 指向它。HistoryListBox 是窗口具体列表，ListBox 是 WPF 类；`ItemsSource="{Binding Readings, Mode=OneWay}"` 把这个列表的行数据来源连接到原 _state.Readings。

读取处理器创建 reading 后执行 `_state.Readings.Add(reading)`：集合保存这一条并发新增通知，已绑定的列表增加一行，用 Reading.ToString 的文字显示设备、数值和单位。Clear 移除所有项并发重置通知，原集合保留，之后还可 Add。不需要手工修改列表 Items 或反复重设 DataContext。

### 2026-10-11 追问：ObservableCollection 与 List 有什么区别？

两者都能保存多个元素，也都有 Add/Clear/Count。关键是绑定之后新增一条，列表是否收到通知。

```csharp
List<string> list = new List<string>();
list.Add("TEMP-001");
ObservableCollection<string> history = new ObservableCollection<string>();
history.Add("TEMP-001");
```

上面都保存了字符串。List 的 Add 不发 WPF 列表需要的集合变化通知；ObservableCollection 的 Add 会发 CollectionChanged。已存在的 List 数据可以通过 ItemsSource 显示，只是之后 List.Add 通常不会自动显示新行；ObservableCollection 会报告增删、替换、移动、重置。

集合通知不负责画屏幕，也不自动转发某条 Reading 内部 Value 的变化。列表必须先连接集合；如果以后要改旧读数的字段，还要考虑读数对象自己的属性通知，本次不追加这个实现。

## Q2：都有 DataContext = _state，为什么 TASK-010 的数据类实现 INotifyPropertyChanged，TASK-011 的不用？

- 日期：2026-10-12
- 状态：待复习
- 关联：[TASK-010](../tasks/TASK-010.md)、[TASK-011](../tasks/TASK-011.md)、[绑定 Q1/Q2](./wpf-binding.md)
- 来源：学员比较 ReadingDisplayState 与 ReadingHistoryState 的通知要求；已读取两个任务的当前 C#、XAML 和学习证据。

两处都设置 DataContext，但变化的东西不同。TASK-010 改的是数据对象里一段结果文字；TASK-011 增删的是同一个历史集合中的记录。

```csharp
_state.ResultText = "读取完成";
```

这是 TASK-010 的情形：_state 指向 ReadingDisplayState，它的 ResultText 属性改值。绑定需要对象发 PropertyChanged 告诉它重读 ResultText；INotifyPropertyChanged 是 .NET 约定这种属性通知的接口。

TASK-011 则是：

```csharp
_state.Readings.Add(reading);
```

这里 _state 是另一个任务的 ReadingHistoryState。它的 Readings 仍返回原集合，属性指向没变；只是集合里多了一条。ObservableCollection 自己发 CollectionChanged，列表听集合通知即可，外层数据类当前无需为这次 Add 另发 Readings 属性通知。

如果将来把 Readings 换成新集合，外层对象要报告 Readings 属性变了，让列表去取新集合；新集合增删通知不能替它报告“换了集合”。若再增加持续变化的绑定 StatusText，也要报告文字属性变化。当前 TASK-011 直接设置状态控件 Text，不是绑定数据属性，所以与 TASK-010 的需求不同。

可以把 Readings 看成指向同一本清单：清单添一行，由清单报告；换一本清单，由保存清单引用的对象报告。已有记录内部属性变化又是第三种问题。DataContext 只指定来源，不会替这些变化自动发所有通知。

## 历史证据与掌握范围

Q1 保持已掌握，Q2 保持待复习。2026-10-11 原窗口验证学员 Add/Clear、连续追加、换 ID、空白保护和恢复，集合与列表始终同一引用；集合创建/getter/绑定来自骨架。已有元素内部属性与更换集合机制不扩大掌握。2026-10-12 Q2 为比较两份源码的概念答疑，无新运行证据。

相关任务卡与验收来源：[TASK-010](../tasks/TASK-010.md)、[TASK-011](../tasks/TASK-011.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-collections.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [WPF 数据绑定概述](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)
- [ObservableCollection<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1?view=net-10.0)
