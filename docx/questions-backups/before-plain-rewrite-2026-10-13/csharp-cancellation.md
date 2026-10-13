# C# CancellationToken

## Q1：cancellationToken 是什么，有什么用？

- 状态：已掌握
- 日期：2026-10-02
- 关联：[TASK-005](../tasks/TASK-005.md)；[练习文件](../../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs)
- 现象：学员询问骨架参数 `cancellationToken` 的含义与用途；当前代码仍使用 `CancellationToken.None`，`Task.Delay(2000)` 未接收令牌。
- 原因与机制：`CancellationToken` 是取消令牌类型，`cancellationToken` 是变量/参数名；令牌让操作观察取消请求。调用方用 `CancellationTokenSource` 创建令牌，传递其 `Token`，并通过 `Cancel()` 或 `CancelAfter(...)` 发出请求；接收方必须检查或把令牌传给支持取消的 API 才能响应。仅在方法签名中加入参数不会自动取消操作。
- 当前练习：`await Task.Delay(2000, cancellationToken)` 能响应等待期间的取消；取消时 await 抛出 `TaskCanceledException`（属于 `OperationCanceledException`），若不在读取方法中捕获，后面的 `return Reading` 不执行，调用方可捕获 `OperationCanceledException` 区分取消与成功。`CancellationToken.None` 是不可取消的令牌，不是“目前尚未取消但以后可取消”的令牌。
- 类比：取消源像“停止”按钮，令牌像连接按钮和操作的信号线，操作需要监听信号并配合停止。
- 易错点：取消是协作式请求，不会强杀线程；发出请求不等于操作已结束；本练习取消的是模拟等待，不表示真实设备也收到了停止命令。
- 2026-10-05 TASK-007 追问 CancelAfter：`cts.CancelAfter(500)` 安排从调用时开始约 500 ms 后由该取消源请求取消，调用本身立即返回，不需要 await；`Cancel()` 则立即请求取消。可类比给停止按钮设倒计时。等待操作仍必须接收 `cts.Token`，如 `await Task.Delay(1000, cts.Token)`，取消响应使 await 进入异常路径，而不是把等待当作成功完成。在源尚未取消时再次调用 CancelAfter 会重设倒计时；本任务只在取消场景调度一次即可。
- 本次代码关联：TASK-007 的 RunScenarioAsync 已创建取消源并保存 `AcquireAsync` 返回的 task，但尚未 await task 或请求取消。调度取消之后仍需在调用方 await 采集任务并专门捕获 OperationCanceledException，让 `using` 的作用域覆盖采集结束，否则作用域提前退出会释放取消源，计时取消和后续令牌使用可能受影响。本轮只静态读取，不运行或代改学员代码；CancelAfter 时机、与 await/using 的关系仍待复习，纳入 Q1，不扩大 Q2 已掌握范围。
- 复查点：自主完成 TODO 1～3，通过两种运行结果解释取消源 → Token → ReadAsync → Task.Delay 的信号路径。
- 依据：[Microsoft Learn：协作式取消](https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads)、[Task.Delay](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay)、[CancellationToken.None](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken.none)。

## Q2：如何使用 cancellationToken？

- 状态：已掌握
- 日期：2026-10-02
- 关联：[TASK-005](../tasks/TASK-005.md)
- 现象：学员追问具体用法；当前源码已将等待改为 `Task.Delay(2000, cancellationToken)`，调用方仍使用 `CancellationToken.None`。
- 原因与机制：使用流程是创建 `CancellationTokenSource` → 获取并传递 `Token` → 用 `Cancel()` 立即请求取消或 `CancelAfter(500)` 安排稍后取消 → 在调用方 await 并捕获取消异常。`CancelAfter` 调用后立即返回，倒计时从调用时开始。`using var` 会在退出作用域时释放取消源；释放资源不等于发出取消请求。
- 最小教学示例（在 async 方法内使用，仅演示等待取消；未作为工作区代码运行）：

```csharp
using var cts = new CancellationTokenSource();
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
```

- 输出机制：通常约 500 ms 后收到取消请求，await 进入异常路径，跳过成功输出；若删除 `CancelAfter`，等待约 2000 ms 后成功。两者都受调度影响，不保证精确时刻。
- 类比：调用方持有停止按钮（取消源），被调用方法接收信号线（令牌）。
- 复查点：在 TASK-005 中把同一取消源的 Token 传入 ReadAsync，仅在 `cancelEarly` 为 true 时安排取消，再自主完成读数和取消输出；不提供整份任务答案。
- 依据：[Microsoft Learn：CancelAfter](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancelafter)、[Task.Delay](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay)。

## 2026-10-02 实践复查记录

- 学员已自主使用取消源 Token，经 ReadAsync 传递至 Task.Delay，在取消情形等待开始后调用 Cancel；运行确认正常输出完整读数、取消路径无成功读数。
- 当前 catch 捕获通用 Exception 并打印堆栈；专门取消处理及结合输出的信号路径说明仍待完善。Q1、Q2 暂保持待复习，不把部分应用证据扩展为全部概念已掌握。

## 2026-10-02 第二次实践复查记录

- 学员自主将 catch 改为 OperationCanceledException，并输出“读取已取消”；再次实际执行 `dotnet run --project Base/Base.csproj`，退出码 0，正常输出 `reading 001 100 ℃`，取消输出 `读取已取消 A task was canceled.`，无成功读数。
- Q2 的取消源创建、令牌传递、请求发出与专门异常处理均有自主代码及运行证据，标为已掌握；实际使用的是 Cancel，未将 CancelAfter 定时调度单独列为掌握技能。
- Q1 的结合实践原理说明尚未提交，保持待复习；TASK-005 的 DoD 3 仍待验证。

## 2026-10-02 提交说明反馈

- 学员正确说明 Task.Delay 的取消响应会在 await 处产生异常，并由调用方 OperationCanceledException 分支捕获；但把 cancelEarly 当作发信号者，路径仅写 Cancel。
- 澄清：cancelEarly 是判断是否请求取消的 bool；真正发请求的是取消源 cts 上的 Cancel 方法。cts.Token 在请求之前已作为参数经 ReadAsync 传入 Task.Delay，取消请求通过关联取消状态被等待操作观察；不是 bool 自动广播或 Cancel 方法按调用栈逐层执行。
- 当前 ReadAsync 没有捕获异常，所以它不返回 Reading，调用方 await pendingReading 也抛出取消异常，跳过成功输出进入 catch。正常情形未请求取消，等待完成后返回 Reading 并输出读数。
- Q1 保持待复习，Q2 的正确用法实践证据仍有效；在原提交说明中修正取消源角色与信号路径后复查。

## 2026-10-02 按用户请求协助修正

- 用户请求“帮我修正”，教练已在[任务卡](../tasks/TASK-005.md)补全提交说明：cancelEarly 是 bool 条件；cts.Cancel 发请求；cts.Token 经 ReadAsync 传到 Task.Delay；取消异常经两处 await 使成功输出跳过并进入 catch，正常情形则返回并输出 Reading。
- 修正版由教练整理，不作为学员已自主理解取消源角色的证据；Q1 保持待复习，Q2 保留已验证的用法掌握状态。后续结合自主应用复核，不要求重复抄写或另做口试。

## 2026-10-05 任务通关与复习范围

- 用户明确要求 TASK-005 通关；复跑正常读取与取消场景通过，任务级验收采用既有协助修正说明，任务状态更新为已通关。
- Q1 仍为待复习：取消源角色与完整路径的说明由教练整理，尚无自主理解的新证据。Q2 仍为已掌握：已实现的令牌传递、Cancel 请求与专门取消异常处理再次运行验证。
- 后续可在 TASK-006 自主应用中复核原理与概念错题；不因任务通关变更问答掌握统计。

## 2026-10-05 TASK-007 CancelAfter 答疑

- 本次追问已去重补充至 Q1，关联 [TASK-007](../tasks/TASK-007.md)；Q1 保持待复习，Q2 保持原有已掌握范围。CancelAfter 示例仅用于解释，未作为工作区代码运行。

## 2026-10-05 TASK-007 实践审查

- 学员自主补充调用方 await task、条件分支下 CancelAfter(100)，using 管理取消源；同一 Token 经采集循环、读取方法传入 Task.Delay。1000 ms 配置的 cancel 实际响应取消，无成功读数；条件、信号源、令牌、响应操作及作用域已有自主应用证据，原 TASK-005 角色混淆错题已修复。
- 新问题：RunScenarioAsync 的 catch 提示后再次 throw，异常逃出入口导致退出码 -532462766；调度 100 ms 对 10 ms 的读取配置太晚，三次读取在场景 45 ms 内完成，无取消提示。仅调度取消不代表一定在等待期间停止，专门 catch 后继续 throw 也不代表安全结束。
- 原理复习重点：循环层传递取消以停止后续轮次，场景调用方处理预期取消；取消延迟与实际等待时间匹配。Q1 保持待复习，Q2 保持已掌握，不把已验证的角色、Token 与正常等待应用扩大为整个取消流程通过。不设额外口试，修复并复跑 TASK-007 即可。
- 同轮最新复查：用户自主移除调用方 catch 中的 throw，重新构建后 1000 ms 的 cancel 退出码 0、取消提示完整且无成功读数；终端处理边界已有修复证据。10 ms 的 cancel 仍在场景 50 ms 内输出三条成功读数，无取消提示。Q1 复习重点收敛为取消时机与配置匹配，统计不变。

## 2026-10-05 TASK-007 最终复查

- 自主实现将取消调度与配置关联，源的 Token 经循环、读取方法传到可取消等待，调用方 await 并处理 OperationCanceledException，using 覆盖任务生命周期。1000 ms 两次、10 ms 六次、1 ms 六次共 14 次均取消并安全结束，无取消后成功读数；正常三次读取、配置异常处理均通过。
- Q1 的取消条件、取消源、Token、响应等待及异常传播/处理已有完整自主应用证据，标为已掌握，Q2 保持已掌握。计时精确、真实设备停止、其他未实践 API 细节不在掌握范围内；资源释放其他概念问答维持原状态。
- 边界补充：CancelAfter(config.ReadDelayMs) 与每次等待时间相等，实际先完成一条（一次先完成两条）再取消当前读取；已完成的读数不因后续取消而撤回。这符合 TASK-007 现有 DoD，不把“必须取消第一轮”追加为门槛。若要固定取消第一轮，宜采用更短延迟并预留调度余量；14 次观察不构成所有调度情形的保证。

## Q3：执行 cts.Cancel 后，什么时候会进入 catch，是到 await pendingReading 后吗？

- 状态：待复习
- 日期：2026-10-07
- 关联：[TASK-005](../tasks/TASK-005.md)；[练习代码](../../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs)
- 现象：学员询问执行 cts.Cancel 后，调用方什么时候捕获取消异常。
- 原因与机制：当前 RunScenarioAsync 的取消 catch 是在 `await pendingReading` 观察到任务取消并抛出取消异常后进入的；cts.Cancel 发出请求，不直接把当前代码跳到这个 catch。ReadAsync 调用时已经开始执行，并已在内部的 await Task.Delay 处等待；内部等待的取消响应不依赖调用方开始 await pendingReading。
- 两处 await：Task.Delay 响应令牌取消后，其任务被取消；ReadAsync 在内部 await 观察到取消并抛出异常，未捕获异常，返回的 pendingReading 随之取消。调用方在 await pendingReading 处观察到这个取消结果，跳过成功读数输出并进入 OperationCanceledException 分支。
- 时机边界：调用方到达 await 时，若 pendingReading 已取消，则立即抛出并进入 catch，无须先暂停；若内部取消传播尚未完成，调用方先暂停，待任务取消后恢复并进入 catch。内部 await 的继续代码可能在调用方到达 await 前或后执行，不保证 cts.Cancel 返回瞬间 pendingReading 已取消，也不能把取消请求时间当成 catch 执行时间。
- 简短类比：Task 保存这次操作的完成结果或取消状态，await 在取得完成结果时把取消表现为异常。
- 易错点：await pendingReading 不启动读取、不发出取消请求；“异常在 await 被调用方观察到”不等于“读取要等 await 才开始”。本段解释限当前练习的取消异常路径，不作 Cancel 方法本身永远不会抛出任何异常的通用断言。
- 复查点：在后续自主实践中区分请求发出、内部操作响应、返回任务取消及调用方 await 观察结果；本轮未改动或运行学员代码，不新增口试要求，不改变 Q1/Q2 的已掌握证据。
- 依据：[Microsoft Learn：await 运算符](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await)、[CancellationTokenSource.Cancel](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.cancel)。
