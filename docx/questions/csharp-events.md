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
