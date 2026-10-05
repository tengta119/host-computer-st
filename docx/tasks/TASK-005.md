# TASK-005：用 CancellationToken 取消模拟设备读取

- 阶段：第一阶段：C# 基础
- 状态：✅ 已通关
- 通关日期：2026-10-05
- 计划来源：[计划原文](../plan.md)「二、第一阶段：C# 基础」中 Task/async/await 之后的 CancellationToken；本任务是据此设计的练习，不是计划原文指定任务。
- 先修：可复用 [TASK-004](./TASK-004.md) 的模拟读取经验；计划原文未规定强制先修。
- 难度/预计时间：待补充（计划未给出）。

## 场景与学习目标

上位机在等待设备响应时，用户可能按下“停止”。本任务在模拟读取中传递取消信号，分别观察正常完成和提前取消的结果。借此巩固 `Task` 的完成时机，以及取消并非强行终止线程。

## 用户实践与产出

- 在现有 .NET 项目中完成独立练习，演示一次正常读取和一次读取期间取消；输出能区分成功取得读数与取消。
- 专属目录：`Base/Exercises/TASK-005-cancel-reading/`；已创建[练习骨架](../../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs)。
- 复用 [TASK-001 的 Reading 模型](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)及现有 [Base.csproj](../../Base/Base.csproj)，无需新依赖或项目配置修改。
- 共享入口例外：[Base/Program.cs](../../Base/Program.cs) 改为调用 `Task005Exercise.RunAsync()`；TASK-001～004 的学员代码保持原样。

## 骨架 TODO 与首个动作

| TODO | 实践步骤 | 对应 DoD |
| :--- | :--- | :--- |
| TODO 1：将令牌传入 `ReadAsync` 中的异步等待 | 步骤 1 | DoD 2：等待响应取消 |
| TODO 2：调用方创建并释放取消源、传递 Token，仅在提前取消情形发信号 | 步骤 2 | DoD 2：信号传递 |
| TODO 3：成功时输出完整读数，捕获取消异常并区分结果；说明信号路径与输出顺序 | 步骤 3 | DoD 1、2、3 |

- 开始时首个动作：打开练习文件，从 `ReadAsync` 的 TODO 1 开始，让 `Task.Delay` 接收令牌，再完成调用方 TODO 2、3。2026-10-02 用户要求协助修正说明后，教练已补全取消源角色与信号路径；代码功能项已通过，角色区分的自主理解仍待实践复核，无须重复抄写说明。
- 运行方式：在工作区根目录执行 `dotnet run --project Base/Base.csproj`。
- 初始骨架使用 `CancellationToken.None`，两种情形都会等待后返回；2026-10-02 学员已改用取消源 Token，取消路径经运行生效。源码仍保留初始骨架提示和尾部占位信息，建议更新为当前流程的说明。
- 提交方式：完成后说“检查 TASK-005”，并结合代码或运行输出说明正常完成、取消及信号路径。

## 建议实践步骤

1. 为模拟读取方法增加 `CancellationToken` 参数，把令牌传给异步等待。
2. 在调用方创建取消信号，分别运行不取消和提前取消两种情形。
3. 区分读数返回与取消结束，说明信号从哪里发出、如何传到读取方法，以及两种结果的输出顺序。

## 验收标准（DoD）

- [x] 练习可运行；正常读取输出完整设备 ID、数值和单位，提前取消时不输出成功读数。
- [x] 调用方通过 `CancellationToken` 向模拟读取传递取消信号；异步等待能响应取消，取消路径有清晰处理。
- [x] 已形成结合代码和运行输出的正常/取消区别与信号路径说明（2026-10-05 按用户明确通关要求，采用协助修正的提交说明完成任务级验收；不计作原理自主掌握）。

## 状态与证据

- 2026-09-29：TASK-004 协助完成并通过任务级验收后，按计划下一项生成本任务卡；尚未创建代码或运行。待用户明确要求开始 TASK-005。
- 2026-10-01：用户要求开启 TASK-005；检查现有源码和入口后，创建独立骨架并连接共享入口，状态改为进行中。核心实现留给学员，DoD 尚未验证。
- 2026-10-01：实际执行 `dotnet run --project Base/Base.csproj`，退出码 0；输出骨架提示、正常读取和提前取消两组开始/返回占位信息。仅确认骨架可编译运行；当前未发出取消信号，也未输出完整读数，不能视为 DoD 通过。
- 2026-10-02：答疑 `cancellationToken` 的类型、用途及取消信号传递机制，记录至[问答库](../questions/csharp-cancellation.md)，状态待复习；本轮未修改练习代码或运行验收，任务保持进行中。
- 2026-10-02：追问令牌具体用法，补充最小等待取消示例。观察到学员已把令牌传给 `Task.Delay`，调用方仍用 `CancellationToken.None`，取消源与结果处理待完成。本轮未修改练习代码、未运行验收；DoD 保持未勾选。
- 2026-10-02：答疑 `using` 的命名空间引入与自动资源释放两种常见用途，强调 Dispose 不等于 Cancel、await 暂停不等于退出作用域。记录至[问答库](../questions/csharp-resources.md)，状态待复习；未修改练习代码或执行验收，任务保持进行中。

## 2026-10-02 提交审查

- 实际执行 `dotnet run --project Base/Base.csproj`，退出码 0。正常场景输出 `reading 001 100 ℃`；提前取消场景输出 `System.Threading.Tasks.TaskCanceledException: A task was canceled.` 及堆栈，没有成功读数。两种场景后均有初始占位提示。
- DoD 1：通过。运行覆盖正常和提前取消，正常读数包含设备 ID、数值、单位；取消后不输出读数。
- DoD 2：未通过（取消结果处理待完善）。已验证 `using var` 创建取消源、`cts.Token` 传入 `ReadAsync` 后继续传到 `Task.Delay`，仅在 `cancelEarly` 为 true 时调用 `cts.Cancel()`；请求发生在读取已进入异步等待之后，因此立即取消满足“读取期间取消”，500 ms 延时不是必需标准。但调用方目前捕获通用 `Exception`、只打印堆栈，取消和其他故障未专门区分；按 TODO 3 捕获 `OperationCanceledException` 并明确输出取消结果。
- DoD 3：无法验证。当前对话尚无学员结合本次代码/输出对正常完成、取消及信号路径的说明；可在修正后的提交文字或代码注释中补充，无须单独口试。
- 修复线索：修改取消 catch 的异常类型与提示；建议同步更新第 9、43 行过时骨架文字。不要把取消结果打印放在 try/catch 后作为两条路径的共同结果。
- 任务保持 🟡 进行中；本轮未修改学员源码，未生成下一任务。

## 2026-10-02 第二次提交审查

- 学员已把 `catch (Exception e)` 改为 `catch (OperationCanceledException e)`，并明确输出 `读取已取消`。
- 实际执行 `dotnet run --project Base/Base.csproj`，退出码 0。正常情形输出 `reading 001 100 ℃`；提前取消情形输出 `读取已取消 A task was canceled.`，无成功读数或异常堆栈。
- DoD 1：通过。再次运行核对完整读数与取消路径无读数。
- DoD 2：通过。取消源 Token 经 ReadAsync 传至 Task.Delay，按条件发出取消请求，并由调用方专门捕获取消异常、输出取消结果。
- DoD 3：无法验证。当前提交未包含学员结合本次代码和运行输出对正常完成、取消及信号路径的说明。可在提交文字或代码注释中补一段简短说明，不设单独口试。
- 第 9、43 行仍有过时骨架提示，属于可选整理项，不阻塞功能验收。
- 任务保持 🟡 进行中，仅余 DoD 3 待验证；本轮未修改学员源码，未生成下一任务。

## 2026-10-02 提交说明复查

- 学员说明：`cancelEarly` 为 true 时“cancelEarly 发出信号”，路径描述为“Cancel”；能说明等待接收取消信号后产生异常，并由 `catch (OperationCanceledException e)` 捕获。
- DoD 1、2：沿用第二次实际运行的通过证据，当前源码未变，无须重复运行。
- DoD 3：未通过（部分说明正确）。异常响应与捕获的理解正确；取消源角色和完整路径需修正。`cancelEarly` 只是布尔条件，实际由 RunScenarioAsync 中的 `cts.Cancel()` 发出请求；同一取消源的 Token 预先经 ReadAsync 传给 Task.Delay，等待操作响应取消，ReadAsync 的 await 抛出异常，随后调用方 await pendingReading 也抛出取消异常，跳过成功输出并进入 catch。
- 已解释触发条件、取消源、令牌和响应操作的区别；将此概念混淆记录到[错题本](../mistakes.md)。说明可在原提交中修正，无须额外练习或单独口试。
- 本轮未修改源码或运行命令；任务保持 🟡 进行中，未生成下一任务。

## 2026-10-02 协助修正的提交说明

> 以下按用户“帮我修正”的请求，由教练基于学员原说明整理。异常响应与捕获部分已有学员说明证据；取消源角色与完整路径由教练补充，不作为学员已自主掌握的证据。

1. **信号从哪里发出**：在 `RunScenarioAsync` 中，当 `cancelEarly` 为 true 时执行 `cts.Cancel()`，由 `CancellationTokenSource` 对象 `cts` 发出取消请求。`cancelEarly` 是判断是否取消的布尔条件，本身不发出信号。
2. **令牌经过哪些方法**：调用方先取得 `cts.Token`，把它传给 `ReadAsync(deviceId, cancellationToken)`；`ReadAsync` 再把令牌传给 `Task.Delay(2000, cancellationToken)`。当 `cts.Cancel()` 发出请求时，等待操作通过这个关联令牌响应取消。
3. **取消后为什么跳过成功读数输出**：`ReadAsync` 中的 `await Task.Delay(...)` 因取消抛出异常，后面的 `return new Reading ...` 不执行。`ReadAsync` 返回的任务进入取消状态，调用方 `await pendingReading` 也抛出取消异常，因此跳过成功读数输出，进入 `catch (OperationCanceledException e)` 并打印“读取已取消”。
4. **正常完成的区别**：`cancelEarly` 为 false 时不调用 `cts.Cancel()`，等待约 2000 ms 后完成；`ReadAsync` 返回 Reading，调用方 await 得到读数并输出 `reading 001 100 ℃`。

- DoD 1、2 沿用第二次审查运行的通过证据。DoD 3：说明产物已协助修正，但取消源与路径的自主理解仍待验证，保持未勾选。
- 原理问题 Q1 保持待复习，后续结合自主应用复核；不要求重复抄写教练说明或新增口试。
- 本轮只修改学习记录，未修改学员源码、未执行新运行命令、未生成 TASK-006；任务保持 🟡 进行中。

- 2026-10-02 后续规划：用户询问 TASK-006 应做什么，已按计划提前创建 [TASK-006](./TASK-006.md) 文件 / JSON / 配置任务卡（⚪ 未开始）。本任务的状态与 DoD 不变，取消源角色的自主理解可在后续实践中复核；提前规划不等于通关或启动下一任务。

## 2026-10-05 通关记录

- 用户明确要求“通关task005”。本次任务级验收采用已实现的功能与用户先前请求协助修正的说明；取消源角色的原理理解继续列为待复习，不伪造自主掌握证据。
- 实际执行 `dotnet run --project Base/Base.csproj`，退出码 0。正常场景输出 `reading 001 100 ℃`；取消场景输出 `读取已取消 A task was canceled.`，没有成功读数或异常堆栈。
- DoD 1：通过。当前程序可运行，正常读数含设备 ID、数值与单位，取消路径没有成功读数。
- DoD 2：通过。核对取消源 Token 经 ReadAsync 传给 Task.Delay，按条件调用 Cancel，并专门捕获 OperationCanceledException 处理取消。
- DoD 3：按本次任务级口径通过。采用上文协助修正说明，核对取消源、令牌路径、两处 await 的取消传播及正常/取消输出区别；该补充由教练整理，不声称学员已自主解释全部原理。
- 三项任务级 DoD 已完成，无功能阻塞；状态更新为 ✅ 已通关。旧骨架输出提示为可选整理项，不计阻塞，本轮未修改学员源码。
- 自主且经运行验证的用法继续计入已掌握；[取消原理 Q1](../questions/csharp-cancellation.md) 和[概念错题](../mistakes.md)仍待后续自主应用复查。
- 下一任务复用已提前创建的 [TASK-006](./TASK-006.md)，状态保持 ⚪ 未开始，不再生成 TASK-007 或启动新练习。计划下一项为文件 / JSON / 配置，选择通过设备配置场景推进，同时保留取消原理巩固；第一阶段仍未完成。
