# C# CancellationToken

## Q1：cancellationToken 是什么，有什么用？

- 状态：待复习
- 日期：2026-10-02
- 关联：[TASK-005](../tasks/TASK-005.md)；[练习文件](../../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs)
- 现象：学员询问骨架参数 `cancellationToken` 的含义与用途；当前代码仍使用 `CancellationToken.None`，`Task.Delay(2000)` 未接收令牌。
- 原因与机制：`CancellationToken` 是取消令牌类型，`cancellationToken` 是变量/参数名；令牌让操作观察取消请求。调用方用 `CancellationTokenSource` 创建令牌，传递其 `Token`，并通过 `Cancel()` 或 `CancelAfter(...)` 发出请求；接收方必须检查或把令牌传给支持取消的 API 才能响应。仅在方法签名中加入参数不会自动取消操作。
- 当前练习：`await Task.Delay(2000, cancellationToken)` 能响应等待期间的取消；取消时 await 抛出 `TaskCanceledException`（属于 `OperationCanceledException`），若不在读取方法中捕获，后面的 `return Reading` 不执行，调用方可捕获 `OperationCanceledException` 区分取消与成功。`CancellationToken.None` 是不可取消的令牌，不是“目前尚未取消但以后可取消”的令牌。
- 类比：取消源像“停止”按钮，令牌像连接按钮和操作的信号线，操作需要监听信号并配合停止。
- 易错点：取消是协作式请求，不会强杀线程；发出请求不等于操作已结束；本练习取消的是模拟等待，不表示真实设备也收到了停止命令。
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
