# C# 事件

掌握依据：2026-09-27 TASK-002 最终复查中，学员自主完成事件声明、发布、订阅和取消订阅，并将当前设备 ID 与读数传入处理器；实际运行输出两次完整读数，取消订阅后无第三次处理器输出。参见 [验收记录](../tasks/TASK-002.md)。

## Q1：TASK-002 中的“读数事件”是什么意思？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)
- 现象：`SimulatedDevice.ProduceReading(...)` 模拟设备产生新读数，任务要求通过事件通知监控侧。
- 原因与机制：读数 `Reading` 是数据；“读数事件”是“设备产生了一条新读数”的通知。设备声明事件，监控侧用 `+=` 订阅处理器。设备产生读数时触发事件，并通过 `ReadingReceivedEventArgs` 把读数传给处理器；用 `-=` 取消订阅后，该处理器不再收到后续通知。事件在此是程序内的发布/订阅机制，并非真实硬件中断或串口消息。
- 类比：类似 Java 中设备对象持有监听器并在数据到达时调用回调；C# `event` 将订阅和触发的职责分开，外部代码只能订阅或取消订阅，不能替设备触发事件。
- 复查点：在 TASK-002 中自主完成事件发布、订阅与取消订阅，并用运行输出说明前两次通知和第三次不通知。

## Q2：在 C# 中如何声明一个事件？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)
- 现象：`SimulatedDevice` 中留有声明携带 `ReadingReceivedEventArgs` 的读数事件的 TODO。
- 原因与机制：常见写法是 `public event EventHandler<TEventArgs>? EventName;`。`event` 表示对外提供订阅和取消订阅；`EventHandler<TEventArgs>` 规定处理器接收 `object? sender` 与事件数据 `TEventArgs e`；`?` 表示当前可能没有订阅者。通常只在声明事件的类内部触发它。
- 本任务示例：在 `SimulatedDevice` 中可声明 `public event EventHandler<ReadingReceivedEventArgs>? ReadingReceived;`。声明只定义通知通道；还需在产生读数时触发，监控侧才会收到。
- 类比：接近 Java 中定义并保存监听器回调，但 C# 的 `event` 专门约束外部代码只做订阅或取消订阅。
- 复查点：自主声明事件，使处理器签名与事件类型匹配，并通过运行验证通知。

## Q3：生产者通过事件发送，订阅者通过事件接收数据吗？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)
- 现象：容易把 `ReadingReceived` 理解成存放读数、供订阅者主动读取的容器。
- 原因与机制：设备在自己的方法中触发 `ReadingReceived`，把 `ReadingReceivedEventArgs` 交给当前订阅的处理器；订阅者先用 `+=` 注册处理器，再在处理器的 `e` 参数中取得读数。事件本身不是读数存储区或消息队列；触发时会调用当时已注册的处理器。
- 类比：类似 Java 调用已注册监听器的回调方法，并把读数作为回调参数传入。
- 复查点：自主完成发布和订阅，并能指出读数在处理器的哪个参数中。

## Q4：`ReadingReceived` 声明后如何使用？

- 状态：已掌握
- 关联：[TASK-002](../tasks/TASK-002.md)；[`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)
- 现象：已在 `SimulatedDevice` 声明 `ReadingReceived`，但还需要连接设备和监控处理器。
- 原因与机制：监控侧用 `device.ReadingReceived += OnReadingReceived;` 注册方法；设备内部在构造事件参数后用 `ReadingReceived?.Invoke(this, args);` 触发；监控侧用 `device.ReadingReceived -= OnReadingReceived;` 取消同一方法的订阅。`?.` 使没有订阅者时跳过调用；`sender` 是设备对象，`args` 携带读数。
- 类比：类似 Java 中 addListener、调用监听器回调、removeListener 的过程。
- 复查点：自主在正确位置写出订阅、发布、取消订阅，并用两次通知和一次取消后的读数验证效果。

## Q5：上位机 C# 中，完成 TASK-002 后还需学哪些委托和事件内容？

- 状态：待复习
- 关联：[TASK-002](../tasks/TASK-002.md)；[TASK-003](../tasks/TASK-003.md)；[计划原文](../plan.md)
- 现象：TASK-002 已验证单设备、单处理器的声明、发布、订阅和取消订阅；还未覆盖委托作为普通方法参数、多个订阅者、长期订阅的生命周期、WPF 线程切换或异步处理。
- 原因与机制：委托是与特定参数/返回值签名匹配的可调用对象，`Action`、`Func<T>` 和 `EventHandler<T>` 都是常见形式；事件基于委托，发布者触发时调用当前订阅的处理器。多个处理器默认同步依次执行，因此慢处理器会延迟发布调用。长期持有发布者时，订阅关系还可能让订阅者继续被引用；应在合适的生命周期点取消订阅。将来设备回调若来自后台线程，WPF 界面更新需切回 UI Dispatcher；异步事件处理涉及 `async void` 的异常处理。这些属于后续场景，不因 TASK-002 通关而视为已掌握。
- 建议顺序：当前先在 TASK-003 练 Lambda 与委托参数；随后按原计划学习 Task/async/await。到 WPF 阶段结合控件事件、路由事件与 UI 线程练习；到设备通信阶段再处理持续订阅、异常与吞吐。
- 复查点：能解释委托与事件的关系，用 Lambda 传递处理方法；在后续实际界面或设备场景中正确处理订阅生命周期和线程边界。

## Q6：委托是什么，为什么 handler 变量可以像方法一样调用？

- 日期：2026-10-11
- 状态：待复习
- 关联：[TASK-012](../tasks/TASK-012.md)、[WPF 命令 Q1](./wpf-commands.md)
- 来源：学员追问“EventHandler 是 .NET 提供的委托类型，那委托是什么意思”。此前已验证的事件及 Lambda 使用不等于已理解所有委托基础；本轮从普通方法调用补充，不撤销既有实践证据。

**现象与用途**：普通调用直接写方法名；委托允许先保存一个方法的引用，之后通过变量调用它，也可以将这种引用传给其他代码作为回调。委托是一种 C# 类型，规定能引用的方法需要接收哪些参数、返回什么类型。变量保存的是符合约定的方法引用，不是方法执行后的结果，也不是方法名字符串。

**普通方法**：为了讲解自定义 ShowMessage 方法，放在一个类中：

```csharp
void ShowMessage(string text)
{
    Console.WriteLine(text);
}
```

ShowMessage 是例子自取的名字，Console 是 .NET 提供的类型，WriteLine 输出文字。该类的另一个方法内可直接执行 `ShowMessage("读取成功");`，立即调用它。

**定义委托类型**：在同一个类里，与方法并列声明：

```csharp
public delegate void MessageHandler(string text);
```

delegate 是 C# 关键字，MessageHandler 是例子自定义的委托类型名。void 规定方法不返回结果，string 规定接收一个字符串参数；不是在声明一个名为 MessageHandler 的普通方法。ShowMessage 的参数和返回值符合要求，因而可以使用。

**创建并调用**：在该类的另一个方法内执行：

```csharp
MessageHandler handler = new MessageHandler(ShowMessage);
handler("读取成功");
```

第一行创建委托对象，让变量 handler 保存对 ShowMessage 的引用；传入的是没有括号的方法名，此时不调用 ShowMessage。第二行通过委托调用它，把字符串作为 text 参数传入，输出“读取成功”。采用完整 new 写法，不引入方法组简写、Action 或 Lambda 新语法。此为讲解示例，不添加到学员项目、不增加验收步骤。

**回到 EventHandler**：.NET 已提供委托类型，其声明形状是 `public delegate void EventHandler(object? sender, EventArgs e);`，不需自己重复定义。它规定处理方法接收 object? 与 EventArgs 两个参数，返回 void。当前任务的 `EventHandler? handler = CanExecuteChanged;` 取得已注册的事件处理方法引用；`handler(this, EventArgs.Empty);` 通过这个委托调用它们。handler 是变量名，不是一个另外声明的普通方法名。EventHandler 规定形状、handler 保存引用、event 约束对外订阅/取消订阅，三者职责不同。CanExecute 返回 bool 且接收一个参数，不符合 EventHandler 的要求，不能将它直接当作此事件的处理方法。

**类比与依据**：可以类比 Java 的回调，通过保存可调用的方法引用，稍后执行；这里不依赖反射按名称找方法。依据：[微软委托说明](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/)、[EventHandler 定义](https://learn.microsoft.com/en-us/dotnet/api/system.eventhandler?view=net-10.0)。

**证据边界**：本轮为必要概念补充，仅更新学习记录，未修改或运行源码；Q6 待复习，TASK-012 状态与 DoD 不变，不因为讲解就认定掌握，也不将未讲清的基础记为学员错误。

### 2026-10-11 追问：为什么不直接调用，委托只能指向一个方法有什么用？

前面的 ShowMessage 示例只展示了保存引用和调用的语法；在固定只调用一个已知方法的场景，直接调用确实足够，并不需要为了使用委托而增加中间变量。需要补充的是“调用者选择行为”的真实用途，不能只重复委托可以保存方法的定义。

使用固定模拟温度说明：委托类型由例子自行命名为 ReadingHandler，要求接收 double、不返回结果；ReadTemperature 完成读取后执行传入的方法，调用者可选择显示读数或检查报警。同一类内的声明和三个普通方法：

```csharp
public delegate void ReadingHandler(double temperature);

void ReadTemperature(ReadingHandler afterReading)
{
    double temperature = 26.5;
    afterReading(temperature);
}

void ShowTemperature(double temperature)
{
    Console.WriteLine($"当前温度：{temperature} °C");
}

void CheckAlarm(double temperature)
{
    if (temperature > 25)
    {
        Console.WriteLine("温度报警");
    }
}
```

在该类的另一个方法里，可分别调用：

```csharp
ReadTemperature(new ReadingHandler(ShowTemperature));
ReadTemperature(new ReadingHandler(CheckAlarm));
```

第一次在读取后显示“当前温度：26.5 °C”；第二次在读取后检查温度并输出“温度报警”。ReadTemperature、ShowTemperature、CheckAlarm、ReadingHandler 和参数名 afterReading 都是讲解例子自定义的名字；固定温度是模拟值，Console.WriteLine 是 .NET 输出方法。afterReading 参数保存调用者传来的委托，调用 afterReading(temperature) 时把读数传给所选方法；读取方法无需写死某个后续处理方法。这里没有引入 Lambda 或新框架，也不把示例添加到学员项目。

委托也可以组合多个方法，不是只能引用一个：

```csharp
ReadingHandler handler = new ReadingHandler(ShowTemperature);
handler += new ReadingHandler(CheckAlarm);
handler(26.5);
```

这里 += 把符合相同要求的第二个方法加入委托，调用时在正常完成的情况下先显示再检查报警。参数和返回值约定保持一致；此例全部返回 void，暂不扩展多方法返回值与异常规则。依据沿用已读的 [微软委托说明](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/)。

当前命令的事件也体现这一用途：ClearHistoryCommand 不需要知道 WPF 内部事件处理方法的具体名称，只调用已注册的处理方法；接收方由订阅连接。前面的语法示例未展示必要性属于教练示例需要改进，不记为学员错题。Q6 保持待复习，本轮未运行或修改源码，TASK-012 与 DoD 不变。

### 2026-10-11 追问：EventHandler 既是委托又是事件吗？

EventHandler 只是一种 .NET 提供的委托类型，不是事件。`public event EventHandler? CanExecuteChanged;` 中，event 是 C# 声明事件的关键字，EventHandler 是这个事件使用的委托类型，CanExecuteChanged 才是事件名称；当前实现的 ICommand 接口要求这个名称。? 表示可以没有订阅者。

委托类型规定处理方法的参数和返回值：EventHandler 要求两个参数 object? sender、EventArgs e，并返回 void。事件使用该委托类型连接处理方法，同时限制外部代码：对当前这种普通事件，外部可以用 += 登记、-= 取消，但不能直接调用事件发通知，也不能用 = 把全部登记关系替换掉。发通知由声明事件的类内部负责。不要用“事件就是委托”抹去这层区别。

回到原代码：`EventHandler? handler = CanExecuteChanged;` 中 handler 是普通委托变量，类型是 EventHandler；在命令类内部，它取得事件当前登记的处理方法，随后 `handler(this, EventArgs.Empty)` 调用它们。CanExecuteChanged 是事件，handler 是委托变量，EventHandler 是委托类型，三者不是同一个身份。仅补概念说明，未修改或运行源码，Q6 保持待复习；教练此前术语需拆开解释，不记为学员错题。

### 2026-10-11 追问：delegate 声明委托，为什么又有 EventHandler？

这里要分清“定义委托类型”和“使用已有委托类型”。delegate 是 C# 的声明关键字，后面的名字才是定义出来的类型；`public delegate void MessageHandler(string text);` 定义示例自命名的 MessageHandler，要求方法接收一个 string、返回 void。.NET 已经用同样的机制提供了 EventHandler，声明形状为 `public delegate void EventHandler(object? sender, EventArgs e);`，要求两个参数、返回 void；这行仅展示已有类型定义，不要求在项目中重复声明。

MessageHandler 和 EventHandler 是两个不同的委托类型，来源和参数要求不同，并非两套委托机制。在 `public event EventHandler? CanExecuteChanged;` 中没有定义新的委托类型，而是使用已有的 EventHandler 类型声明一个事件。在 `EventHandler? handler = CanExecuteChanged;` 中也是使用已有类型声明变量。先定义类型，再用类型声明变量或事件；不能把 delegate 关键字与 EventHandler 类型名视为两个同等身份的“委托”。本轮仅答疑，未修改或运行源码，Q6 和 TASK-012 状态不变。

### 2026-10-11 追问：handler(...) 和 handler?.Invoke(...) 有什么区别？

分开看调用方式与空值检查。`handler(this, EventArgs.Empty)` 和 `handler.Invoke(this, EventArgs.Empty)` 都通过委托执行已登记的方法，调用效果相同；Invoke 是委托类型的调用方法，由编译器为委托类型生成，不是教练新建的辅助方法。两种直接调用在 handler 为 null 时都会发生空引用异常。

?. 是 C# 空值条件访问语法，`handler?.Invoke(this, EventArgs.Empty)` 在 handler 为 null 时跳过调用，不为 null 时调用 Invoke。对于当前局部变量 handler，这与完整写法 `if (handler != null) { handler(this, EventArgs.Empty); }` 效果相同。handler 为 null 表示没有已登记的处理方法；?. 只处理这个空值情况，并不会吞掉处理方法执行中产生的异常。

任务骨架采用局部变量、普通 if 判断和直接委托调用，是为了先展示“取得已登记方法、判断是否存在、再调用”的步骤，减少尚未讲过的简写；并不是 ?.Invoke 在这里不能使用。学员现在可理解两种写法的对应关系，无需为了本次答疑修改代码，不新增验收要求。Q6 保持待复习，本轮未修改或运行源码，TASK-012/DoD 不变。
