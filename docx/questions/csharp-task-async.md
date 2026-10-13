# C# Task 与 async/await

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：什么是 Task，Task.Delay 怎么用？

- 状态：已掌握
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)

TASK-004 先输出“开始读取”，过一段时间再得到一条 Reading。Task 用来让调用方跟踪这次操作是否结束；它是 .NET 提供的类，ReadAsync 是练习自定义的方法，pendingReading 是保存本次返回任务的变量。

```csharp
Task<Reading> pendingReading = ReadAsync(deviceId);
Reading reading = await pendingReading;
```

第一行调用读取方法，拿到表示本次读取完成状态的 Task<Reading>。尖括号里的 Reading 指明成功完成后会交出一条读数。第二行用 C# 的 await 等本次任务完成，得到 Reading，保存到 reading。Task 与最后取得的读数是两件东西。

读取方法里的 `await Task.Delay(8000)` 使用 .NET 的计时方法，约 8000 毫秒后才继续后面的代码。这是模拟等待，不是真读设备。若 Delay 的任务还未完成，await 暂停当前方法的后续部分，等待期间不靠一条线程一直阻塞着。

只写 `Task.Delay(8000)` 而丢弃返回任务，读取方法就可以继续 return，并不会自动等八秒；外层 await ReadAsync 也不会替内部补上丢掉的等待。Task.CompletedTask 已经完成，不能模拟耗时；Thread.Sleep 会阻塞当前线程。Task 可类比 Java CompletableFuture 的完成结果载体，不等于新建线程。

## Q2：返回类型是 Task<Reading>，为什么可以直接 return Reading？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)

先看“返回一条读数”和“向调用方返回任务”分别发生在什么位置。下面按当前方法形式简化，温度和单位来自已有 Reading：

```csharp
static async Task<Reading> ReadAsync(string deviceId)
{
    await Task.Delay(8000);
    return new Reading { DeviceId = deviceId, Value = 100, Unit = "℃" };
}
```

`async Task<Reading>` 规定调用方拿到 Task<Reading>，而方法内部成功结束时用 return 提供 Reading。编译器为 async 方法生成完成任务所需代码，把这条 Reading 作为该任务的成功结果。调用方先得到任务，再通过 await 取得它的 Reading。

它不是把 Reading 当成 Task 强制转换，也不要求你在这里再手写一层 Task。去掉 async 后，这种直接 return Reading 就不符合普通 Task<Reading> 方法的返回类型了；普通方法可用 `return Task.FromResult(reading);` 明确返回已完成任务，FromResult 是 .NET 方法。

Java 普通方法常手写返回 CompletableFuture；此处 C# async 帮助管理返回任务及完成结果。复习时先对应三个具体名字：ReadAsync 的返回类型是 Task<Reading>，pendingReading 保存任务，`await pendingReading` 的结果是 Reading。

## Q3：Task.CompletedTask 加不加 await 有什么区别？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)

骨架的 `await Task.CompletedTask;` 不会模拟读取耗时：CompletedTask 是 .NET Task 类的静态属性，取得一个已经成功完成、没有读数结果的任务。

```csharp
await Task.CompletedTask;
Console.WriteLine("继续执行");
```

这段放在 async 方法中，await 发现任务已完成，就立即执行后面的输出，不会等待一秒，也不会因此必定切换线程。

不用 await 时，可以把任务保存到变量；普通返回 Task 的方法也可以写 `return Task.CompletedTask;`。但单独一行 `Task.CompletedTask;` 只是读取属性，不是 C# 允许的独立表达式语句，会报编译错误。

该属性适合作为“无需等待、已成功完成”的任务返回值。TASK-004 原先用它保留可构建骨架，后来已经改为 Delay；复习不需要把当前代码改回 CompletedTask。

## Q4：TASK-004 中两个 async 方法和两个 await 的执行顺序是什么？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)

这两处 await 等待的对象不同，也暂停不同的方法。以下是 TASK-004 的流程简图式代码，省去已有日志：

```csharp
Task<Reading> pendingReading = ReadAsync(deviceId);
Console.WriteLine("读取方法已调用，准备等待结果");
Reading reading = await pendingReading;
Console.WriteLine(reading.Value);
```

1. 第一行一调用 ReadAsync，它就开始执行。内部到达尚未完成的 `await Task.Delay(8000)`，读取方法的后续部分暂停，向外返回尚未完成的 Task<Reading>。
2. RunAsync 拿到这个任务，继续打印“准备等待结果”。到第三行 await pendingReading 时，若读取还没完成，RunAsync 的后续部分也暂停。
3. 计时结束，ReadAsync 从内部 await 后继续，构造并 return Reading；pendingReading 因此成功完成。
4. RunAsync 的 await 取得这条 Reading，再输出数值。入口等待的 RunAsync 最后完成。

计时从调用 Task.Delay 开始，不是从外面的 await pendingReading 开始，外层 await 不会再加一轮八秒。当前文件等待参数为 8000；旧记录里的 1000 是当时教学样例。

暂停不是把整台电脑暂停，也不是等待到外层 await 才启动方法。若等待对象已完成，await 不必暂停。历史输出中的 `System.Threading.Thread` 只是线程对象的默认文字，不能当作线程 ID。

## Q5：方法声明中的 Task 指的是哪件事？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)

方法签名里的 Task 表示“这个方法这次调用整体什么时候完成”。它不是自动指向方法内部某一行 Delay。

| 调用 | 返回的任务表示什么 | 成功后取得什么 |
| :--- | :--- | :--- |
| Task.Delay(8000) | 这次计时结束 | 无额外结果值 |
| ReadAsync(deviceId) | 这次模拟读取结束 | 一条 Reading |
| RunAsync() | 读取、取得结果、输出这一整段流程结束 | 无额外结果值 |

Task、Task<Reading> 是 .NET 类型；Delay 是 .NET 方法，ReadAsync/RunAsync 是本项目自定义方法。pendingReading 是变量名，保存 ReadAsync 返回的那一个任务。成功/失败/取消属于任务的完成结果，调用方可在 await 时观察它。

“每次调用拿到表示本次完成的任务”不保证每次都分配不同对象；某些已完成任务可以复用。这里理解它代表哪次操作即可，不把类型名 Task 理解成线程名称。

## Q6：为什么 RunAsync 声明为 async Task？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Program.cs](../../Base/Program.cs)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)

RunAsync 要等读取结果再输出，但不把一条 Reading 再返回给入口。它因此使用 `async Task`：async 允许方法内部写 await，Task 让入口能等待整段流程结束。

```csharp
public static async Task RunAsync()
{
    Reading reading = await ReadAsync("001");
    Console.WriteLine(reading.Value);
}
```

该说明片段位于原任务类中。`await RunAsync()` 等到这里的读取与输出完成，不取得额外读数值。普通 `async void` 方法无法供入口这样 await，所以不能仅因为“不返回读数”就改成 void。

### 追问：await File.WriteAllTextAsync(path, configJson) 为什么不能这样写？

这句调用本身合法：File 是 .NET 文件类，两个 string 参数分别是路径和待写文字。问题出在包含它的 SaveAsync 当时没有 async，以及末尾 return 使用了不存在的变量。

```csharp
static async Task SaveAsync(string path, string configJson)
{
    await File.WriteAllTextAsync(path, configJson);
}
```

这是用两个字符串简化的说明片段；原 SaveAsync 接收 DeviceConfig 并先序列化。写入任务成功后走到方法末尾，整个 SaveAsync 完成，不需要 `return task;`，也不应改成 Task<Task>。失败会通过任务在调用方 await 处被观察到。

### 追问：普通 Task 方法可以直接 return 写入任务吗？

可以。若里面不再使用 await，可写普通方法 `static Task SaveTextAsync(string path, string text) { return File.WriteAllTextAsync(path, text); }`。SaveTextAsync 是这里说明用的名字，它把 .NET 写入 API 的任务直接交给调用者。Async 后缀和 Task 返回类型不强制要求 async；使用 await 的普通方法则需相应 async 声明。

### 追问：async Task SaveAsync 末尾没有 return，为什么仍有返回任务？

返回任务由编译器生成的异步方法代码管理。调用者仍可写 `Task saving = SaveAsync(...); await saving;`；这里省略号仅表示原方法所需参数，不是可复制代码。方法走到末尾，表示成功完成，没有额外结果值。

普通 Task 方法由你显式 return Task；async Task 可自然结束或裸 `return;`；async Task<Reading> 成功路径要提供 Reading。这三个规则分别对应“交出任务”“完成但无读数值”“完成并提供读数值”。不要把“没有手写 return”解释为“没有返回给调用方的 Task”。

## Q7：TASK-004 的 Task.Delay 和 await 涉及多线程吗？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)

Task-004 的等待前后日志在历史运行中出现不同线程 ID，但并没有两段读取代码同时计算。先区分“方法暂停，之后继续”与“几条线程同时执行”。

`async`、`await` 本身不创建一个专属新线程。Task.Delay 依靠计时完成，等待八秒期间没有一条线程一直执行 ReadAsync 的空等待。Task 记录操作结果，线程执行某段代码，它们不是同一对象。

当前控制台没有 WPF 的 UI 调度环境，尚未完成的 await 后通常在线程池线程继续；历史实测 ID 从 2 变 5，只证明那次前后由不同线程先后执行，不能证明并行。在通常从 WPF UI 事件中开始的异步方法里，默认捕获 UI 上下文并安排回 UI 线程；已经完成的 await 也可能直接继续。不能保证每次 await 都换线程。

要看身份，用 .NET 的 `Environment.CurrentManagedThreadId` 输出整数；`Thread.CurrentThread` 的默认文字只会类似 System.Threading.Thread。Task.Run 可安排计算在线程池执行，是另一种用途；本题 Delay 没有因此变成并行计算。本次不新增线程实验或改已有日志。

## 历史证据与掌握范围

Q1 保持已掌握，Q2～Q7 保持待复习。TASK-004 自主等待/返回读数有证据，结果输出与单位修正有教练协助；2026-09-29 线程 ID 2→5 为历史实测。2026-10-05 TASK-006 曾缺 await，3500 ms 配置整个进程仅 62 ms；自主修复后实测 3565 ms，50 ms 对照 120 ms，200 ms 为 265 ms，CS4014 消失。SaveAsync 当时有 CS4032/CS0103，普通 Task 转交写法曾编译通过，现在源码为 async Task 写入。以上是旧记录/源码核对，不是本轮重跑。

相关任务卡与验收来源：[TASK-004](../tasks/TASK-004.md)、[TASK-006](../tasks/TASK-006.md)、[TASK-007](../tasks/TASK-007.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-task-async.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft Learn：Task 类](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)
- [Task.Delay](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task.delay)
- [async 返回类型](https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/async/async-return-types)
- [Microsoft Learn：异步返回类型](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types)
- [Task 异步编程模型](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model)
- [Microsoft Learn：Task.CompletedTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.completedtask)
- [await 运算符](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await)
- [Microsoft Learn：async/await 编译错误](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/async-await-errors)
- [File.WriteAllTextAsync](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.writealltextasync?view=net-10.0)
- [Microsoft Learn：异步返回类型（Task 与 Task<TResult>）](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types)
- [异步编程场景](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios)
- [Microsoft Learn：控制台应用与同步上下文](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/synchronizationcontext-console-apps)
