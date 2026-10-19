# C# 事件

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：TASK-002 中的“读数事件”是什么意思？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)

在 TASK-002，设备执行 ProduceReading 产生一条读数后，监控处理方法要立即收到它。Reading 是读数数据；ReadingReceived 表示“刚产生新读数，请调用已登记的接收方法”。这就是此处的读数事件。

```csharp
SimulatedDevice device = new SimulatedDevice("001");
device.ReadingReceived += OnReadingReceived;
device.ProduceReading(5.0, "℃");
```

SimulatedDevice、ProduceReading、OnReadingReceived 是本项目自定义类/方法。new 创建设备 001，device 保存它。第二行把接收方法登记到这个设备的事件上，尚不调用；第三行让设备产生读数，设备内部发通知，才调用接收方法并传入读数。

它可类比 Java 设备对象登记监听器、数据到来调用回调。当前通知发生在程序内部，不是串口消息或硬件中断。外部代码可登记、取消登记，发通知由设备类内部负责。

## Q2：在 C# 中如何声明一个事件？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)

当前自定义设备类需要公开一个通知入口，让监控侧登记“新读数到来后做什么”。类内这行就是声明：

```csharp
public event EventHandler<ReadingReceivedEventArgs>? ReadingReceived;
```

public/event 是 C# 关键字；ReadingReceived 是练习自定义事件名。EventHandler<T> 是 .NET 提供的委托类型，规定接收方法接收 sender 与一个事件参数、返回 void。这里 T 填入自定义 ReadingReceivedEventArgs，里面保存 Reading。问号表示可能没有接收方法，此时值可为 null。

先看配套接收方法的形状：`void OnReadingReceived(object? sender, ReadingReceivedEventArgs e)`。sender 接收发通知的设备，e 接收这次事件数据，返回 void。只有参数与返回要求符合，才能按此事件类型登记。

声明不会自动发送读数，也不创建事件参数。还要在 ProduceReading 中创建参数并调用当前登记的方法，具体用法见 Q4。事件参数类属于预备骨架，不把它的存在算成学员自主设计。

## Q3：生产者通过事件发送，订阅者通过事件接收数据吗？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)

可以说设备发通知、订阅方收到数据，但实际动作是“设备调用已登记的处理方法，把数据作为参数传进去”。事件不是供以后主动取读数的存储区。

```csharp
private static void OnReadingReceived(object? sender, ReadingReceivedEventArgs e)
{
    Reading reading = e.Reading;
    Console.WriteLine(reading.Value);
}
```

这在已有任务类中是接收方法的简化说明。e 是本次接到的自定义参数对象；e.Reading 取得其中那条读数。变量 reading 保存它，再取 Value 输出。sender 是来源设备，读数保存在 e 的 Reading 属性中，两者用途不同。

设备触发时只调用当时已登记的方法。之后才登记的接收方不会自动补收旧通知；若要保留历史，需要另用集合。取消订阅后，后续通知不再调用被移除的处理方法。

## Q4：`ReadingReceived` 声明后如何使用？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)

先看当前任务的三步：登记、设备内部发布、移除同一接收方法。以下片段来自同一场景，放置位置不同，不能直接串成一个方法：

```csharp
device.ReadingReceived += OnReadingReceived;
device.ReadingReceived -= OnReadingReceived;
```

第一行在监控侧登记，第二行在不再监听时移除。它们都操作原 device 的 ReadingReceived，传入没有调用括号的方法名；`+=` 本身不会立即处理读数。

设备类内部发布时，先用已有 reading 创建参数，再以完整写法理解：

```csharp
ReadingReceivedEventArgs args = new ReadingReceivedEventArgs(reading);
EventHandler<ReadingReceivedEventArgs>? handler = ReadingReceived;
if (handler != null)
{
    handler(this, args);
}
```

args 包装这条 Reading；handler 保存当前接收方法，null 表示没有。最后一行调用登记的方法，this 是发通知的设备，args 交给接收方的 e。原源码 `ReadingReceived?.Invoke(this, args)` 是同类调用的空值判断简写，有接收方才调用。它不吞掉接收方法自己抛出的异常。

TASK-002 已用产生两条、移除、再产生第三条的流程验证：接收方法只输出前两条。复习时读 [真实文件](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)，无需重置已有 TODO 或再写整份答案。

## Q5：上位机 C# 中，完成 TASK-002 后还需学哪些委托和事件内容？

- 状态：待复习
- 关联：[TASK-002](../tasks/TASK-002.md)；[TASK-003](../tasks/TASK-003.md)；[计划原文](../plan.md)

TASK-002 已验证“一个设备、一个处理方法、取消订阅后不再收到”。后续要学的内容，应从实际问题引出，不能把已通关当作所有事件机制都学完。

例如同一温度既要显示又要检查报警，读取代码可以接收调用方传来的处理方法，Q6 就用这个例子讲委托。多个接收方法同时登记时，普通调用会依次同步执行；一个耗时处理会拖慢发通知的方法。长期保留设备对象时，还需在界面不再使用后取消相应订阅，避免对象被订阅关系持续引用。

将来真实设备数据若从后台线程来，WPF 界面更新要安排到界面线程。Dispatcher 是 WPF 用于把操作安排给界面线程的工具，到真实场景再展开。异步事件处理的异常边界同样待后续实践，不在本次整理里突然要求使用 async void。

当时后续顺序是 TASK-003 委托参数与 Lambda，再到 Task/async/await；这些任务现在已完成各自验收。尚未验证的生命周期、线程与持续设备数据问题仍继续复习，课程顺序以计划和最新进度为准。

## Q6：委托是什么，为什么 handler 变量可以像方法一样调用？

- 日期：2026-10-11
- 状态：待复习
- 关联：[TASK-012](../tasks/TASK-012.md)、[WPF 命令 Q1](./wpf-commands.md)
- 来源：学员追问“EventHandler 是 .NET 提供的委托类型，那委托是什么意思”。此前已验证的事件及 Lambda 使用不等于已理解所有委托基础；本轮从普通方法调用补充，不撤销既有实践证据。

普通方法 `ShowMessage("读取成功")` 会立即输出文字。若另一段代码需要“先保存要执行的处理方法，等读数到来再调用”，可以把方法引用保存到委托变量中。

先声明说明用的普通方法和委托类型，均放在同一个示例类内：

```csharp
void ShowMessage(string text)
{
    Console.WriteLine(text);
}
public delegate void MessageHandler(string text);
```

ShowMessage 与 MessageHandler 是例子自定义名字；Console.WriteLine 是 .NET 输出方法。delegate 是 C# 声明关键字，MessageHandler 规定能保存的方法“接收 string，返回 void”。它不是又声明了一个可直接输出的普通方法。

在同类另一个方法里：

```csharp
MessageHandler handler = new MessageHandler(ShowMessage);
handler("读取成功");
```

第一行创建委托，让 handler 保存 ShowMessage 的引用；没有调用 ShowMessage。第二行才调用保存的方法，把文字交给其 text 参数。变量保存的是可调用方法引用，不是输出结果或方法名字符串。

### 2026-10-19 TASK-045：两个EventHandler声明在做什么

串口辅助Task045SerialConnection收到并解析出完整读数后，需要告诉窗口“这是一条读数”；读取中遇到需要报告的异常时，需要告诉窗口“接收失败，原因在这里”，两个事件分别通知这两种结果。

```csharp
public event EventHandler<Task045ReadingEventArgs>? ReadingReceived;
public event EventHandler<Task045SerialErrorEventArgs>? ReceiveFailed;
```

ReadingReceived和ReceiveFailed是教练自行命名的两个事件，event是C#关键字；EventHandler<T>是.NET已经定义好的委托类型，本题不用再写一份delegate声明。
EventHandler<Task045ReadingEventArgs>规定可以登记的方法接收object? sender、Task045ReadingEventArgs e并返回void，实际登记的是窗口的SerialReadingReceived；EventHandler<Task045SerialErrorEventArgs>则要求第二个参数为Task045SerialErrorEventArgs，实际登记的是SerialReceiveFailed。
Task045ReadingEventArgs和Task045SerialErrorEventArgs都是教练新建的参数类，不是.NET内置类型；前者的Reading保存设备编号、原始值和温度，后者的Message保存错误说明，SessionId是教练提供的旧连接通知保护，不在本次用途解释里追加其内部机制。

```csharp
_connection.ReadingReceived += SerialReadingReceived;
_connection.ReceiveFailed += SerialReceiveFailed;
```

这两行已经写在Task045Window构造方法中，第一行把“收到读数时要调用的窗口方法”登记进去，第二行把“读取错误时要调用的窗口方法”登记进去；+=此时只登记，不立即执行这些方法。
辅助类以后通过局部委托handler调用登记的方法，例如handler(this, new Task045ReadingEventArgs(reading, sessionId))把当前辅助对象作为sender，把此次读数放入e交给SerialReadingReceived；没有任何方法登记时可能为null，因此声明带?且调用前判断handler != null。
这样串口辅助只负责读数据、报告结果，不需要知道窗口里哪个TextBlock显示温度、哪个位置显示错误；窗口登记自己的处理方法决定收到结果后做什么，这就是当前代码使用委托事件的具体用途。
普通事件通知本身不会切换到窗口线程，调用仍跟着发通知的接收线程执行，TASK-045的Dispatcher负责后续界面交接，两者用途分开讲。
本轮只是解释教练提供的声明与连接，未改代码或运行示例，Q6仍待复习，不撤销TASK-002已有事件登记/发布/移除的实践能力，也不记学员错题。

### 2026-10-11 追问：为什么不直接调用，委托只能指向一个方法有什么用？

若固定只调用一个方法，直接调用就够。委托的用处是让调用方决定读取后的动作，读取方法不写死显示或报警。下面是自定义说明方法：

```csharp
public delegate void ReadingHandler(double temperature);

void ReadTemperature(ReadingHandler afterReading)
{
    double temperature = 26.5;
    afterReading(temperature);
}
```

ReadingHandler 规定接收一个 double、返回 void；ReadTemperature 完成模拟读取后调用参数 afterReading 保存的方法。调用方可选下面两个普通方法，也放在同一个说明类中：

```csharp
void ShowTemperature(double temperature)
{
    Console.WriteLine("当前温度：" + temperature + " °C");
}
void CheckAlarm(double temperature)
{
    if (temperature > 25)
    {
        Console.WriteLine("温度报警");
    }
}
```

ShowTemperature 输出传入读数，CheckAlarm 在超过 25 时输出报警；两个名字由例子自定，不是框架 API。在同类另一个方法调用 `ReadTemperature(new ReadingHandler(ShowTemperature));` 就读取后显示，换成 CheckAlarm 就读取后检查报警，ReadTemperature 本身不用写死选哪个。

还可以组合，在同类另一个方法中：

```csharp
ReadingHandler readingHandler = new ReadingHandler(ShowTemperature);
readingHandler += new ReadingHandler(CheckAlarm);
readingHandler(26.5);
```

readingHandler 保存两个匹配的方法，调用时在正常完成的情况下先显示 26.5、再输出温度报警。+= 登记第二个方法，不立即调用；最后一行才执行。委托并非只能引用一个方法。这与 Java 传入回调、监听器接收通知有相似用途，不用反射按名字查方法。片段仅作说明，本轮未运行。

### 2026-10-11 追问：EventHandler 既是委托又是事件吗？

回到命令类的 `public event EventHandler? CanExecuteChanged;`：EventHandler 是 .NET 提供的委托类型；event 是 C# 关键字；CanExecuteChanged 才是事件名。EventHandler 要求方法接收 object? sender、EventArgs e，返回 void。

类内部的 `EventHandler? handler = CanExecuteChanged;` 使用已有类型声明普通变量，并取得当前登记的方法。handler 是变量，事件是 CanExecuteChanged，它们身份不同。外部对这类事件用 +=/-= 登记或移除；不能自行调用事件、也不能用 = 替换全部订阅关系。

### 2026-10-11 追问：delegate 声明委托，为什么又有 EventHandler？

delegate 定义一个委托类型，例如上面的 MessageHandler。.NET 已经定义 EventHandler；你只是在用它声明事件或变量，不需要再在项目中定义同名类型。可以用“C# 语法定义类型 → 用这个类型创建/保存对象”理解两行的先后关系。

CanExecute 接收一个参数并返回 bool，不符合 EventHandler 的两个参数/void 要求，所以命令事件并不直接保存 CanExecute。

### 2026-10-11 追问：handler(...) 和 handler?.Invoke(...) 有什么区别？

先看当前骨架的完整调用：

```csharp
EventHandler? handler = CanExecuteChanged;
if (handler != null)
{
    handler(this, EventArgs.Empty);
}
```

`handler(...)` 与 `handler.Invoke(...)` 都调用委托保存的方法；Invoke 是委托的调用方法。直接调用时若 handler 为 null，会抛空引用异常。`handler?.Invoke(...)` 的 ?. 是 C# 空值条件访问：没有方法就跳过，有则调用，与这里局部变量的完整 if 写法对应。

EventArgs.Empty 是 .NET 的无额外数据参数。?. 只处理 handler 为 null 的情况，不能吞掉被调用方法自己的异常。当前例子的 this 指命令对象，详情见 [命令问答](./wpf-commands.md)。

## 历史证据与掌握范围

Q1～Q4 保持已掌握，Q5/Q6 保持待复习。2026-09-27 TASK-002 学员自主声明、发布、登记、移除，当前设备 ID 随读数传入，实际前两次完整输出、移除后第三次无接收输出；参数类与入口由骨架提供。Q6 与各追问是 2026-10-11 的委托基础补充，没有独立示例运行证据。教练示例用途不足、术语混用属于教学改进，不撤销已有事件应用证据、不记为学员错题。

相关任务卡与验收来源：[TASK-002](../tasks/TASK-002.md)、[TASK-003](../tasks/TASK-003.md)、[TASK-012](../tasks/TASK-012.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-events.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [微软委托说明](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/)
- [EventHandler 定义](https://learn.microsoft.com/en-us/dotnet/api/system.eventhandler?view=net-10.0)
