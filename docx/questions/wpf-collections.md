# WPF 集合与历史列表

## Q1：ObservableCollection、ListBox 和 ItemsSource 分别是什么，记录怎样出现在列表里？

- 日期：2026-10-11
- 状态：已掌握（限在已有绑定骨架上应用集合 Add/Clear 驱动列表更新）
- 来源：TASK-011 开始时的必要讲解；结合学员此前对自定义类型来源和属性语法的反馈，不将骨架提供视为理解。

**现象**：任务刚启动时历史为空；骨架已连接列表的数据来源，但读取处理器尚未向集合添加读数，所以不会出现历史行。这与 XAML 编辑器提示或设计预览不是一回事，要在实际运行窗口观察。

**来源和身份**：`ObservableCollection<T>` 是 .NET 自带的集合类，命名空间为 `System.Collections.ObjectModel`，using 让本文件可以使用短类型名。`ObservableCollection<Reading>` 的尖括号指定元素类型为 Reading；Reading 复用学员 TASK-010 自定义的读数类。`ListBox` 是 WPF 自带的列表控件，`ItemsSource` 是它指定数据集合的属性。`ReadingHistoryState`、`_state`、`_readings`、`Readings` 是本任务骨架自行命名的类、变量或属性，不是要求学员事先知道的 API。

**集合对象与公开属性**：数据类里的 `new ObservableCollection<Reading>()` 创建一个空集合，字段 `_readings` 保存它。公开属性采用完整语法：

```csharp
public ObservableCollection<Reading> Readings
{
    get
    {
        return _readings;
    }
}
```

读取该属性时，执行 getter 中的 return，得到 `_readings` 保存的集合对象。没有 setter 表示不能给这个属性重新赋一个集合；仍能调用取得的集合对象的方法。`new ReadingHistoryState()` 创建数据对象，窗口字段 `_state` 保存它；因此 `_state.Readings` 会取得那个数据对象内部的集合。

**数据连接与通知**：窗口设置 `DataContext = _state`，ListBox 的 `ItemsSource="{Binding Readings, Mode=OneWay}"` 就从该对象取得 Readings 属性。按钮创建一条 Reading 后，调用 `_state.Readings.Add(reading)`，将这条读数追加到同一个集合末尾。集合自动发出新增通知，WPF 列表自动增加一行，并用 Reading 已有的 ToString 显示 ID、温度和单位。

**已有经验对照**：与已使用的 `List<Reading>` 一样，Add 方法保存一条读数；ObservableCollection 额外提供集合增删通知，不需要本任务自己写发布集合通知的事件。调用 Clear() 无需参数，会移除所有记录，但保留集合对象，之后还能 Add。TASK-010 的 PropertyChanged 通知某个属性值变了；本次通知集合中新增/移除了记录。修改已有记录的内部属性是否自动刷新是另一件事，不纳入本次要求。

### 2026-10-11 追问：ObservableCollection 与 List 有什么区别？

两者都是 .NET 自带的泛型集合类，能保存多个元素，支持 Add、Remove、Clear、Count 和按下标读取。List<T> 来自 System.Collections.Generic，ObservableCollection<T> 来自 System.Collections.ObjectModel。本次关键区别是集合内容改变时有没有通知，而不是是否能够保存数据。

```csharp
List<string> list = new List<string>();
list.Add("TEMP-001");

ObservableCollection<string> history = new ObservableCollection<string>();
history.Add("TEMP-001");
```

上面的两次 Add 都能把字符串保存进去。List 的 Add 不会发出 WPF 列表需要的集合变化通知；ObservableCollection 的 Add 会发出通知。ObservableCollection 自带一个叫 CollectionChanged 的事件，表示集合新增、删除、替换、移动或重置等变化；已经连接该集合的 WPF 列表会监听它，本任务无需手动订阅。

对于已通过 ItemsSource 连接的列表，普通 List 中原来已有的数据可以显示；连接之后再调用 List.Add，数据确实增加，但列表通常不会自动增加一行，因为它没有收到集合变化通知。ObservableCollection.Add 则会通知已连接的列表增加一行，Clear 会通知列表清空。集合本身不负责绘制界面，仍需要 ListBox 与集合建立连接。

用在当前 TASK-011 中：`_state.Readings.Add(reading)` 修改的是原历史集合，集合自动通知 HistoryListBox，列表随之新增记录。后台临时保存、遍历或处理一批数据通常用 List 就够；希望已绑定的界面跟随集合增删自动变化时，可用 ObservableCollection。

通知边界：ObservableCollection 负责集合中的记录增删等变化，不会自动替记录对象转发内部属性通知。例如已经存在的某条 Reading 的 Value 改变，不能仅凭集合类型保证对应显示自动更新，通常需要读数对象自己的属性通知，并绑定对应属性；本次不扩展该实现。

本轮为概念答疑，仅增量更新学习记录，未修改或运行源码，TASK-011 仍 🟡 进行中，问答仍待复习。

**首个动作与预期**：在 Task011Window.xaml.cs 的 TODO 1 只补 Add 那一行，运行 WpfBae 后点一次读取，列表应出现一条 `TEMP-001 26.5 °C`，再次读取保留旧行并追加新行。不要改 HistoryListBox.Items 或反复设置 ItemsSource/DataContext 来刷新。成功状态提示与清空仍由后续 TODO 完成。

**证据边界**：2026-10-11 仅验证未完成 TODO 的骨架构建与启动；尚无学员自主新增、清空及恢复的运行证据，本问答保持待复习。

**2026-10-11 最终实践复查**：学员现已补充原集合的 Add/Clear 和成功提示；实际任务窗口按钮事件验证连续追加、修改 ID、空白保护、清空和恢复全部通过。每次 Add 均有集合新增通知并显示完整行，清空发出 Reset，集合与列表数量一致且绑定与来源保持同一引用。Q1 按“使用集合通知驱动已有列表绑定”范围改为已掌握；集合创建、getter、DataContext 和 ItemsSource 连接由骨架提供，不据此认定自主设计，也不扩展到已有元素内部属性通知或事件内部实现。证据见 [TASK-011 运行结果](../tasks/evidence/TASK-011/results.json)。
