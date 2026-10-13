# C# CancellationToken

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：cancellationToken 是什么，有什么用？

- 状态：已掌握
- 日期：2026-10-02
- 关联：[TASK-005](../tasks/TASK-005.md)；[练习文件](../../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs)

TASK-005 开始一轮两秒模拟读取，调用方可能希望提前停下。真正发停止请求的是 `cts.Cancel()`；cancelEarly 只是“这轮要不要取消”的 bool 条件，不会自行发信号。

```csharp
CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken cancellationToken = cts.Token;
```

CancellationTokenSource 是 .NET 的取消源类，new 创建取消源，cts 保存它；CancellationToken 是 .NET 的令牌类型，cancellationToken 是本例变量名。Token 属性给出与这个取消源关联的令牌，把它传给读取方法，再传给内部等待。

```csharp
await Task.Delay(2000, cancellationToken);
```

这是 ReadAsync 内部真正接收令牌的等待。取消源请求取消后，该等待能观察请求、以取消结束；await 表现为取消异常，当前方法后面的 return Reading 不再执行。只在方法签名写 CancellationToken 却没传给等待或检查它，不会自动停止读取。

可以把 cts 看成停止开关，令牌是让操作观察这个开关状态的方式。CancellationToken.None 不会变成可取消令牌；它是不可取消的令牌。

### 追问：CancelAfter 做了什么，与 await/using 有什么关系？

`cts.CancelAfter(500)` 安排约 500 毫秒后请求取消，计时从调用开始；调用立即返回，本身不需要 await。cts.Cancel 则现在请求。源尚未取消时再次 CancelAfter 可重设计时；具体时刻受调度影响。

调用方之后仍要 await 采集任务，才能观察成功或取消，并让 using 的范围覆盖操作结束。Dispose 清理资源，不能代替 Cancel；两者分别解释见 [using 问答](./csharp-resources.md)。已完成的某条读数不会被后来取消撤回，取消也不会强杀线程或自动通知真实设备停机。

## Q2：如何使用 cancellationToken？

- 状态：已掌握
- 日期：2026-10-02
- 关联：[TASK-005](../tasks/TASK-005.md)

先把“创建、传入、请求、观察结果”串成一个最小例子。下面是 async 方法内的说明片段，不是要求重写已经通关的 TASK-005：

```csharp
using (CancellationTokenSource cts = new CancellationTokenSource())
{
    CancellationToken token = cts.Token;
    cts.CancelAfter(500);
    try
    {
        await Task.Delay(2000, token);
        Console.WriteLine("等待完成");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("等待被取消");
    }
}
```

cts 是取消源；token 接到它的令牌；CancelAfter 安排停止请求；Delay 的第二个参数让等待观察这个请求。通常等待尚未到两秒就收到取消，await 进入 catch，跳过“等待完成”。OperationCanceledException 是 .NET 的取消异常类，用于区分预期取消。

using 块在这段操作完成后调用取消源 Dispose，不在 await 暂停时释放。删除 CancelAfter 后，这个例子通常等待约两秒输出完成；两种时间都不是精确计时保证。

原 TASK-005 把 token 经 ReadAsync 传到 Delay，cancelEarly 为 true 时调用 Cancel；原 TASK-007 还经采集循环传递同一取消状态。终端场景调用方专门处理预期取消后可安全结束；若 catch 提示后又 throw，异常仍会离开入口，不能把“出现取消提示”当成正常退出。

## Q3：执行 cts.Cancel 后，什么时候会进入 catch，是到 await pendingReading 后吗？

- 状态：待复习
- 日期：2026-10-07
- 关联：[TASK-005](../tasks/TASK-005.md)；[练习代码](../../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs)

就当前 TASK-005，调用方的取消 catch 是 `await pendingReading` 观察到读取任务取消后进入的。`cts.Cancel()` 发请求，不会把程序指针直接跳到下方 catch。

```csharp
Task<Reading> pendingReading = ReadAsync(deviceId, cts.Token);
cts.Cancel();
Reading reading = await pendingReading;
```

这三行是 try 块中的流程说明。第一行已启动 ReadAsync，它执行到内部尚未完成的 `await Task.Delay(2000, token)` 后暂停。第二行请求取消，内部等待无需等外面的第三行才响应。

内部 Delay 取消后，ReadAsync 的 await 观察到取消异常；当前 ReadAsync 没捕获它，因此不 return Reading，返回的读取任务也以取消完成。外层 await 再观察到取消，跳过成功输出，交给调用方的 OperationCanceledException 分支。

若第三行到达时任务已取消，会直接进入异常路径；若响应/传播尚未完成，会先暂停，再在任务完成取消后继续到 catch。不能保证 Cancel 返回的一瞬间 pendingReading 已取消，也不能把发请求时刻等同于 catch 执行时刻。

await pendingReading 不负责启动读取，也不发请求，它取得完成结果时把取消表现为异常。这个结论限定当前练习，不意味着 Cancel API 在所有其他回调情形下绝不会抛异常。

## 历史证据与掌握范围

Q1/Q2 保持已掌握，Q3 保持待复习。2026-10-02 TASK-005 自主令牌传递、Cancel 和专门 catch 已运行，取消源角色说明曾由教练修正；2026-10-05 任务按用户要求通关，未据此升级 Q1。随后 TASK-007 学员自主实现取消条件/源/令牌/响应/调用方处理，1000 ms 两次、10 ms 六次、1 ms 六次共 14 次取消安全结束，Q1 才升级。请求前一条或两条已完成读数允许保留；不保证取消第一轮，不扩大为精确调度或真实设备停止。catch 重抛及调度过晚的修复前历史完整存于旧稿，本轮未运行。

相关任务卡与验收来源：[TASK-005](../tasks/TASK-005.md)、[TASK-006](../tasks/TASK-006.md)、[TASK-007](../tasks/TASK-007.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-cancellation.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft Learn：协作式取消](https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads)
- [Task.Delay](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay)
- [CancellationToken.None](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken.none)
- [Microsoft Learn：CancelAfter](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancelafter)
- [Microsoft Learn：await 运算符](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await)
- [CancellationTokenSource.Cancel](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancel)
