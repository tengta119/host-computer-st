# TASK-012：有历史才能清空，按钮怎样知道条件变了

> 2026-10-13 白话重写。正文先讲“是否允许”和“实际执行”，再分步讲通知。原任务已通关，文件与历史证据在文末。

## 1. 先看按钮应该怎么表现

列表为空时，清空按钮应不能用；读取一条后，应能用；点击清空后，列表变空，按钮再次不能用。

我们把这两件事交给一个 ClearHistoryCommand 对象：

- 询问是否允许清空：CanExecute。
- 真正清空历史：Execute。

ClearHistoryCommand 是教练自定义的类，方法名字来自 .NET 的 ICommand 接口。Command 在本题可以先理解成“按钮拿到的一个对象，里面有条件判断和执行动作”。

读取按钮这次仍用 Click。先让清空按钮通过命令工作，不同时改全部按钮。

## 2. ICommand 规定按钮可以怎样使用这个对象

当前类声明：

```csharp
public class ClearHistoryCommand : ICommand
```

ICommand 是 .NET 提供的接口，来自 System.Windows.Input。`: ICommand` 表示这个类按接口要求提供方法和事件。

这里必须有三个成员：

| 名称 | 当前作用 |
| :--- | :--- |
| CanExecute | 返回 true/false，说明能否执行 |
| Execute | 执行清空动作 |
| CanExecuteChanged | 告诉使用方：条件可能变了，请重查 |

接口没有替你写 `Count > 0` 或 Clear。规则由自定义类实现，WPF 按接口约定来调用。

如果有 Java 接口经验，可把它理解成：按钮知道 ICommand 提供哪些成员，不需要知道你的自定义类内部怎样存历史。

## 3. 先确保命令与读取按钮使用同一份历史

窗口创建 ReadingCommandState：

```csharp
private ReadingCommandState _state = new ReadingCommandState();
```

这是教练自定义数据类，里面有历史集合、状态文字和清空命令。

它的构造函数创建命令：

```csharp
_clearCommand = new ClearHistoryCommand(this);
```

这里 this 是正在创建的这个 ReadingCommandState。将它交给命令后，命令构造函数保存：

```csharp
private ReadingCommandState _state;

public ClearHistoryCommand(ReadingCommandState state)
{
    _state = state;
}
```

右边 state 是传入对象，左边 _state 是命令自己的字段。窗口和命令各有一个字段，但都指向同一数据对象。读取往它的 Readings 追加，清空也操作它的 Readings。

命令中若再 new 一个状态对象，会得到另一份空历史，那就看不到窗口读取的记录。

## 4. ClearCommand 属性把已经创建的命令交出去

数据类已有：

```csharp
public ICommand ClearCommand
{
    get
    {
        return _clearCommand;
    }
}
```

ClearCommand 是自定义属性名；声明为 ICommand，说明外部可以按命令接口使用返回值。实际对象还是之前创建的 ClearHistoryCommand，没有因为接口类型而换成另一个对象。

getter 每次返回原对象，不重新创建命令。与 Readings 返回原集合的道理相同。

## 5. Button.Command 与 ClearCommand 分别是谁的属性

当前清空按钮：

```xml
<Button x:Name="ClearButton"
        Content="清空历史"
        Command="{Binding ClearCommand, Mode=OneWay}" />
```

左边 Command 是 WPF 的 Button 属性；右边 ClearCommand 是 _state 的公开属性。

窗口先设置 `DataContext = _state`。WPF 读取 _state.ClearCommand，取得命令对象，再放到 ClearButton.Command。

OneWay 表示这个对象从数据类交给按钮，不把 Button.Command 反写回数据类。它不表示 CanExecute 只调用一次。

当前这个按钮没有清空 Click 处理器。WPF 自己询问绑定的命令条件，并在允许点击时执行命令。

## 6. CanExecute 就是一次普通条件判断

当前作品：

```csharp
public bool CanExecute(object? parameter)
{
    if (_state.Readings.Count > 0)
    {
        return true;
    }
    return false;
}
```

Count 是历史记录数量。大于 0 就返回 true，否则 false。空列表时没有可清空内容，所以返回 false。

bool 是 C# 的 true/false 类型。object? parameter 是接口规定的参数，问号允许 null；本题没有设置 CommandParameter，正常调用不用这个参数。

上方可简写为 `return _state.Readings.Count > 0;`，但原 if/return 写法已经正确，没有必要为了简写改作品。

## 7. Execute 清空原集合，设置完成提示

当前完整动作：

```csharp
public void Execute(object? parameter)
{
    if (!CanExecute(parameter))
    {
        return;
    }

    _state.Readings.Clear();
    _state.StatusText = "已清空";
}
```

感叹号表示取反。`!CanExecute(parameter)` 成立，说明当前不允许执行，就提前结束。这个防护避免其他代码直接调用 Execute 时绕过条件。

Clear 清掉原集合内容，列表跟着变空；集合对象还在。然后改数据类 StatusText，由原属性通知更新提示文字。

CanExecute 判断不会自动清空。只有进入 Execute 并执行 Clear 那句才清空。

## 8. 集合变了，为什么还需要通知按钮

WPF 先问一次 CanExecute，初始得到 false。之后读取按钮 Add 了记录，集合 Count 变成 1。

“现在再问会得到 true”与“按钮已经知道需要再问”是两件事。当前通过 CanExecuteChanged 事件告诉 WPF 条件可能改变。

发这个事件不直接返回 true，也不执行清空。按钮收到后重新调用 CanExecute，才能取得此刻的条件结果。

## 9. CollectionChanged 怎样连接到条件通知

数据类构造函数已经写：

```csharp
_readings.CollectionChanged += Readings_CollectionChanged;
```

CollectionChanged 是 ObservableCollection 自带的事件。+= 表示订阅：集合变化后，让它调用 Readings_CollectionChanged。

这个方法当前只有：

```csharp
private void Readings_CollectionChanged(
    object? sender,
    NotifyCollectionChangedEventArgs e)
{
    _clearCommand.NotifyCanExecuteChanged();
}
```

sender 是发出集合变化的集合；e 是 .NET 提供的集合事件参数。这里不用读参数细节，只转告清空命令通知按钮。

NotifyCanExecuteChanged 是教练自定义辅助方法名，不是 ICommand 的第四个必需成员。

## 10. 事件声明与“发通知”分开理解

命令类声明：

```csharp
public event EventHandler? CanExecuteChanged;
```

EventHandler 是 .NET 已有的委托类型，规定监听方法接收 sender 和 EventArgs 参数。CanExecuteChanged 是这个类的事件名；问号允许目前没有监听者。

event 让外部可以订阅/取消订阅这个事件，实际发通知由命令内部完成。声明这一行只提供通知位置，并没有自动调用 CanExecute。

WPF 将按钮连接到 Command 后负责登记监听。命令发通知时，WPF 的处理流程重查按钮 Command 的 CanExecute，并更新可用状态。

## 11. 委托可以先从“保存一个方法”理解

说明示例，未加入项目：

```csharp
private delegate void MessageHandler(string text);

private void ShowMessage(string text)
{
    Console.WriteLine(text);
}

// 以下两句放在一个普通方法里执行：
MessageHandler handler = new MessageHandler(ShowMessage);
handler("条件变化了");
```

delegate 声明一种可保存方法的类型。MessageHandler 在此规定：方法接收一个 string、不返回值。ShowMessage 符合这个要求。

new MessageHandler(ShowMessage) 记住方法，并不会当场执行 ShowMessage。调用 handler(...) 时才执行它。

EventHandler 也是委托类型，只是 .NET 已经定义好参数要求，不需要你另写 delegate 声明。委托能组合多个方法；事件通过 += 增加监听，-= 移除监听。

## 12. 当前 ?.Invoke 怎样调用已登记的监听方法

原工具方法：

```csharp
public void NotifyCanExecuteChanged()
{
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
```

先看它的普通展开说明：

```csharp
EventHandler? handler = CanExecuteChanged;

if (handler != null)
{
    handler(this, EventArgs.Empty);
}
```

handler 保存当前监听方法。有监听者才调用，没有就跳过。

this 在这里是当前 ClearHistoryCommand，作为 sender 传给监听方法；前面构造命令的 this 是 ReadingCommandState，因为代码写在不同类中。

EventArgs.Empty 提供一个没有额外内容的参数对象。调用 handler(...) 与 handler.Invoke(...) 的委托调用作用相同；?.Invoke 把非空检查缩成一行。

## 13. 通知不会通过名称自动跳到 CanExecute

CanExecuteChanged 事件中登记的是 WPF 的监听/转发处理流程，不是把你的 CanExecute 方法直接当成事件监听者。

它们的关系是：

```text
命令发 CanExecuteChanged
→ WPF 的监听流程收到通知
→ 从按钮已经连接的 Command 再调用 CanExecute
→ 更新按钮是否可用
```

这不是按字符串反射寻找同名方法，也不是收到事件后调用 Execute。

框架内部有通知转发细节，先把上方行为理解清楚即可；具体源码记录在文末问答链接，不必为了写 Count 判断先记住内部类名。

## 14. 从读取到禁用恢复，完整走一遍

```text
初始 Count=0 → CanExecute=false → 清空禁用
读取 Add → CollectionChanged → 通知条件变化
重查 Count>0=true → 清空启用
点击清空 → Execute → Clear → CollectionChanged
重查 Count>0=false → 清空禁用
再读 Add → 重查为 true → 恢复启用
```

因此读取按钮只要往原集合 Add，已提供的通知连接就会让清空重新判断；不需要在窗口 Click 中另写 ClearButton.IsEnabled=true。

通知重新判断条件不等于模拟一次点击，不会在 Add 后自动清空刚得到的记录。

## 15. 状态文字、列表和按钮分别靠哪种通知

StatusText 使用完整属性：

```csharp
set
{
    _statusText = value;
    OnPropertyChanged("StatusText");
}
```

先保存新提示，再通过 PropertyChanged 让状态 TextBlock 重读。

| 屏幕变化 | 当前负责的通知 |
| :--- | :--- |
| 历史增加/清空 | 集合 CollectionChanged |
| “读取成功/已清空”等提示更新 | 属性 PropertyChanged |
| 清空是否可用重新判断 | 命令 CanExecuteChanged |

PropertyChanged 不能代替条件通知，CanExecuteChanged 也不代替列表成员通知。这三条各自有明确对象与目的。

## 16. 无效输入没有改变集合，已有历史继续保留

读取仍在窗口 ReadButton_Click 中。它先 Trim 输入，空白时设置：

```csharp
_state.StatusText = "请输入设备 ID；本次没有新增记录。";
return;
```

没有 Add/Clear，就没有这次集合变化通知或由它触发的条件通知。若之前有两条历史，仍为两条，清空仍能用；若本来为空，仍禁用。

这一题把清空命令化，还没有把读取移到命令中，也未要求通用命令库、命令参数或异步命令。

## 文末查阅：文件、观察和历史范围

实际文件：[数据类](../../WpfBae/Exercises/TASK-012-clear-history-command/ReadingCommandState.cs)、[清空命令](../../WpfBae/Exercises/TASK-012-clear-history-command/ClearHistoryCommand.cs)、[窗口 XAML](../../WpfBae/Exercises/TASK-012-clear-history-command/Task012Window.xaml)、[窗口读取方法](../../WpfBae/Exercises/TASK-012-clear-history-command/Task012Window.xaml.cs)、[任务卡](../tasks/TASK-012.md)。

复习先只观察“初始禁用 → 读取启用”，再点击清空看恢复；不需新增通知类。当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-017，复跑旧题需明确临时入口并恢复，本轮未运行。

任务卡记录 2026-10-11 已通关：构建/原应用启动、同状态/集合/命令、8 个实际窗口场景通过。按钮自动化走 WPF 命令路径，不是只直接调用 Execute。原 evidence/TASK-012 当前不存在，历史结论引用卡片，不提供缺失附件的链接。

自主应用范围为 Count 条件、Clear 和提示。对象构造、属性、事件、集合订阅、通知工具与框架监听由骨架提供；读过此说明不扩大为已独立设计这些机制。

详细答疑见 [命令问答](../questions/wpf-commands.md)、[C# 事件 Q6](../questions/csharp-events.md)，接口定义见 [Microsoft ICommand](https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.icommand?view=net-10.0)。下一篇把 [读取移到 ViewModel 的命令](./TASK-013-wpf-viewmodel.md)。
