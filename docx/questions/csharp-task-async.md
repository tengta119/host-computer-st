# C# Task 与 async/await

## Q1：什么是 Task，Task.Delay 怎么用？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)
- 现象：学员在 TASK-004 开始后询问 `Task` 的含义和 `Task.Delay` 的用法；骨架中的 `ReadAsync` 仍使用 `Task.CompletedTask` 占位。
- 原因与机制：`Task` 表示一个异步操作及其完成状态，不等于一个专属线程；`Task<T>` 还携带完成后的 `T` 类型结果。`Task.Delay(500)` 返回约 500 毫秒后完成的 `Task`，单位为毫秒；`await Task.Delay(500)` 会暂停当前异步方法的后续执行，等待期间不阻塞线程。`Task.Delay` 只模拟耗时，不读取设备。`await` 作用于 `Task<Reading>` 时取得 `Reading`。
- 类比：可把 `Task<Reading>` 暂时类比 Java 的 `CompletableFuture<Reading>`；这是结果载体的类比，不表示每个 Task 都会新开线程。
- 易错点：`Task.Delay(500)` 不加 `await` 只得到一个 Task，后续代码不会因此等 500 毫秒；`Task.CompletedTask` 已经完成，不能模拟等待；`Thread.Sleep(500)` 会阻塞当前线程。
- 复查点：在 TASK-004 自主把占位改为 `await Task.Delay(...)`，返回有效读数，结合运行输出说明 `Task<Reading>` 与 `Reading` 及执行顺序。
- 依据：[Microsoft Learn：Task 类](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task)、[Task.Delay](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task.delay)、[async 返回类型](https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/async/async-return-types)。

## Q2：返回类型是 Task<Reading>，为什么可以直接 return Reading？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)
- 现象：`ReadAsync` 声明为 `async Task<Reading>`，但方法体写的是 `return new Reading { ... };`，编译没有报错。
- 原因与机制：`async Task<T>` 方法体里的 `return` 应给出 `T` 类型值；编译器生成异步状态机，把这个值作为返回给调用方的 `Task<T>` 的完成结果。因此这里 `return Reading` 合法，调用方收到 `Task<Reading>`，`await` 后得到 `Reading`。这只是解释行为的概念模型，不代表源码字面上调用了 `Task.FromResult`。
- 类比：与 Java 方法手动返回 `CompletableFuture<Reading>` 相比，C# 的 `async` 负责生成和完成返回给调用方的任务对象。
- 易错点：去掉 `async` 后，方法体直接 `return Reading` 不再符合 `Task<Reading>` 返回类型；非 `async` 方法若要返回已完成任务，可显式使用 `Task.FromResult(reading)`。`async` 并不表示每次调用都新开线程。
- 复查点：能指出 `ReadAsync` 中 `return` 的值、调用方 `pendingReading` 的类型和 `await pendingReading` 的结果类型。
- 依据：[Microsoft Learn：异步返回类型](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types)、[Task 异步编程模型](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model)。

## Q3：Task.CompletedTask 加不加 await 有什么区别？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)
- 现象：学员询问骨架中 `await Task.CompletedTask;` 的 `await` 是否必要，以及 `CompletedTask` 的含义。
- 原因与机制：`Task.CompletedTask` 是 `Task` 类的静态属性，取得一个已经成功完成、没有结果值的任务。对它使用 `await` 会立即继续，不会产生等待或让出执行权。若不使用 `await`，可以把任务赋给变量或在普通返回 `Task` 的方法中直接 `return Task.CompletedTask;`；单独写 `Task.CompletedTask;` 不是合法的 C# 语句，因为单纯读取属性不能构成语句。
- 类比：接近 Java 中一个已经完成的 `CompletableFuture<Void>`；再等待它也不会产生时间上的等待。
- 易错点：`await Task.CompletedTask` 不是异步延迟，也不能提供 `Reading` 结果。TASK-004 中它只是可运行占位；要模拟设备响应，应在练习中使用 `await Task.Delay(...)`。
- 复查点：能区分“已经完成的任务”和“未来才完成的延迟任务”，并说明现有骨架为何没有真实等待。
- 依据：[Microsoft Learn：Task.CompletedTask](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.completedtask)、[await 运算符](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await)。

## Q4：TASK-004 中两个 async 方法和两个 await 的执行顺序是什么？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)
- 现象：学员已在 `ReadAsync` 中改用 `await Task.Delay(1000)` 并打印返回前信息，但仍不理解 `Task` 的执行逻辑。2026-09-29 实际运行输出依次为“开始读取设备 001”“读取方法已调用，准备等待结果 System.Threading.Thread”“已返回 System.Threading.Thread”“reading 100 .C”“TODO 2：输出取得的读数”。
- 原因与机制：调用 `ReadAsync(deviceId)` 时，方法体立即开始执行，直到遇到尚未完成的 `await Task.Delay(1000)`；此时 `ReadAsync` 暂停并把尚未完成的 `Task<Reading>` 返回给 `RunAsync`。`RunAsync` 继续打印“读取方法已调用”，到 `await pendingReading` 时也暂停，控制权回到等待它的顶层入口。延迟完成后，`ReadAsync` 从 `await` 后继续，打印“已返回”并 `return Reading`，于是 `pendingReading` 完成；`RunAsync` 随后取得 `Reading`、打印读数并结束，顶层入口也随之结束。
- 类比：`Task<Reading>` 类似 Java `CompletableFuture<Reading>` 这种代表将来结果的对象；方法暂停点类似把后续步骤登记为任务完成时的继续动作。类比只帮助理解流程，不代表每个 Task 有专属线程。
- 易错点：`ReadAsync` 的方法体不会等到 `await pendingReading` 才开始；`Task.Delay` 的等待期间没有线程一直执行该方法。两次打印 `Thread.CurrentThread` 只显示线程对象的类型，不能据此判断线程 ID 或是否切换线程；线程切换也不是异步成立的必要条件。
- 复查点：能按当前代码指出 `ReadAsync` 何时开始、何时返回未完成的 `Task<Reading>`、两处 `await` 各暂停哪个方法、`Reading` 何时进入 `pendingReading` 的结果。
- 依据：[Microsoft Learn：Task 异步编程模型](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model)、[await 运算符](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await)。
- 2026-09-29 追问：学员能按代码复述 `ReadAsync` 在 `Task.Delay` 处暂停、`RunAsync` 继续并在 `await pendingReading` 处暂停、延时完成后 `ReadAsync` 恢复并返回结果的顺序。补充澄清：约 1000 毫秒从 `Task.Delay(1000)` 调用时开始计，`await pendingReading` 不会再启动一次延时；此口头复述暂不作为自主实践掌握证据。

## Q5：方法声明中的 Task 指的是哪件事？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)
- 现象：学员能复述暂停和恢复顺序，但询问 `Task` 写在方法返回类型里时具体代表哪件事。
- 原因与机制：声明里的 `Task` 是方法的返回类型，规定调用者拿到代表**这次方法调用整体完成**的任务对象。`ReadAsync` 的 `Task<Reading>` 从调用开始代表“读取最终完成并产生一条 Reading”；其中 `Task.Delay` 返回另一个只代表计时完成的 `Task`。`RunAsync` 的 `Task` 代表整个运行流程完成，没有结果值。每次调用方法都会得到本次调用对应的任务；任务可以在返回时未完成，也可以已经完成。
- 类比：`Task<Reading>` 接近 Java `CompletableFuture<Reading>`，是本次读取的结果凭据，而不是某个线程或某条语句本身。
- 易错点：不要把声明中的 `Task<Reading>` 理解成 `Task.Delay` 的返回任务，或理解成“ReadAsync 尚未开始执行”。`Task` 是类型名，`pendingReading` 是调用后保存本次任务的变量。
- 复查点：能分别指出 `Task.Delay(1000)`、`ReadAsync(deviceId)`、`RunAsync()` 各自返回的任务代表什么、何时完成、有没有结果值。
- 依据：[Microsoft Learn：异步返回类型](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types)、[Task 异步编程模型](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model)。

## Q6：为什么 RunAsync 声明为 async Task？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Program.cs](../../Base/Program.cs)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)
- 现象：学员进一步询问 `public static async Task RunAsync()` 为什么需要 `Task` 返回类型。
- 原因与机制：`RunAsync` 内部要 `await pendingReading`，所以声明 `async`；它只完成读取和输出，没有把读数返回给调用方，因此返回无结果值的 `Task`。这个 `Task` 代表整个 `RunAsync` 调用的完成，入口 `Program.cs` 能用 `await RunAsync()` 等到所有输出结束。相对地，`ReadAsync` 要把读数交给调用方，返回 `Task<Reading>`。
- 类比：可把无结果值的 `Task` 暂时类比为 Java `CompletableFuture<Void>`，把 `Task<Reading>` 类比为 `CompletableFuture<Reading>`。
- 易错点：`Task` 不是 `RunAsync` 内部某一行的任务，也不是读取的值；普通异步方法若写成 `async void`，调用方不能 `await` 它来跟踪完成。`async` 允许 `await`，`Task` 是向调用方公开完成状态的返回类型，两者作用不同。
- 复查点：能解释 `Program.cs` 的 `await RunAsync()` 等待什么，以及为什么 `RunAsync` 不需要 `Task<Reading>`。
- 依据：[Microsoft Learn：异步返回类型](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types)。

## Q7：TASK-004 的 Task.Delay 和 await 涉及多线程吗？

- 状态：待复习
- 关联：[TASK-004](../tasks/TASK-004.md)；[Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)
- 现象：学员在理解 `Task` 返回类型后，询问当前练习是否涉及多线程；代码在等待前后打印 `Thread.CurrentThread`。
- 原因与机制：`async`、`await` 本身不创建新线程，`Task` 也不等于线程。`Task.Delay(1000)` 表示非阻塞计时；等待期间没有一个线程专门执行 `ReadAsync` 或被它占住。当前控制台程序未安装 UI 同步上下文，未完成的 `await` 之后通常由线程池线程继续；本次实测等待前线程 ID 为 2，之后为 5。一般而言 `await` 不保证必定换线程：若等待对象已完成，方法不会暂停；在 WPF UI 上，默认会把后续代码安排回 UI 线程。代码没有调用 `Task.Run`、`new Thread`，也没有安排两段计算同时并行执行。
- 类比：与 Java 的 `CompletableFuture` 一样，结果凭据和执行它的线程是两个概念；异步等待是控制流程，多线程是执行资源的安排。
- 易错点：`Thread.CurrentThread` 的默认字符串只显示 `System.Threading.Thread`，不能核对线程身份；若要观察线程 ID，可查看 `Environment.CurrentManagedThreadId`，但 ID 变化也不能代替对异步控制流程的理解。
- 复查点：能解释当前代码等待期间是否占用线程、`Task` 与线程的区别，以及 `Task.Run` 与 `Task.Delay` 的用途差异。
- 依据：[Microsoft Learn：Task 异步编程模型中的线程说明](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model)、[异步编程场景](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios)。
- 2026-09-29 追问与实测：学员已改用 `Environment.CurrentManagedThreadId` 输出线程 ID。实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0；等待前打印 ID 2，延时后打印 ID 5，说明这次运行的两段代码由不同线程先后执行，没有证据表明它们并行执行同一段方法。通用说法中的“可能不换”适用于已有同步上下文并安排回原线程的环境（如 WPF UI），或其他由运行时选择线程的情形；当前控制台程序没有 UI 同步上下文，恢复通常在线程池线程上。参见 [Microsoft Learn：控制台应用与同步上下文](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/synchronizationcontext-console-apps)。
