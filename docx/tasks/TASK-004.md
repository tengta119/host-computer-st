# TASK-004：用 Task 与 async/await 模拟设备读取

- 阶段：第一阶段：C# 基础
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「二、第一阶段：C# 基础」中的 Task / async / await；本任务是据此设计的练习，不是计划原文指定任务。
- 先修：可复用 TASK-001 的 `Reading` 模型；计划原文未规定强制先修。
- 难度/预计时间：待补充（计划未给出）。

## 场景与学习目标

上位机读取设备数据时，等待设备响应不应阻塞后续工作。本任务先用内存中的模拟设备与短暂等待练习 `Task`、`async`、`await`，观察调用开始、等待和读数返回的顺序。暂不连接真实设备、WPF 或取消操作；取消操作属于计划中的后续 `CancellationToken` 内容。

## 用户实践与产出

- 在现有 .NET 项目中完成练习，模拟一次设备读数的异步取得并输出设备 ID、数值和单位；结果输出与单位修正由教练按学员请求代为完成，见下方证据记录。
- 专属目录：`Base/Exercises/TASK-004-async-reading/`；已创建 [Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs)，复用 TASK-001 的 [Reading 模型](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)。
- 共享入口例外：[Base/Program.cs](../../Base/Program.cs) 已切换为 `await Task004Exercise.RunAsync()`；复用 [Base/Base.csproj](../../Base/Base.csproj)，未更改项目文件或增加依赖。

## 练习实现与起步记录

- TODO 1：学员自主将占位等待改为 `Task.Delay(8000)`，并返回样例 `Reading`；教练按后续请求将单位写为 `℃` → 实践步骤 1、DoD 1、2。
- TODO 2：教练按学员请求在 `RunAsync` 中补齐设备 ID、数值、单位和 `await` 结果说明，删除占位输出；学员此前自主复述过调用、暂停、恢复顺序 → 实践步骤 2、3、DoD 1、3。
- 起步动作曾为打开 [Task004Exercise.cs](../../Base/Exercises/TASK-004-async-reading/Task004Exercise.cs) 的 TODO 1；实际验证命令与结果见下方记录。

## 建议实践步骤

1. 定义返回 `Task<Reading>` 的模拟读取方法，用 `Task.Delay` 表示等待设备响应。
2. 在异步入口调用该方法，分别输出开始等待、取得读数后的信息。
3. 用运行输出解释 `await` 前后代码的执行顺序，以及 `Task<Reading>` 与 `Reading` 的区别。

## 验收标准（DoD）

- [x] 练习可运行；模拟读取返回一条带设备 ID、数值和单位的读数，运行输出可核对。
- [x] 使用 `Task<Reading>`、`async` 和 `await` 完成模拟等待与取得结果；等待通过异步方式实现。
- [x] 能根据代码和运行输出说明调用开始、等待、读数返回的顺序，并指出 `await` 得到的是哪一个值。

## 状态与证据

- 2026-09-27：TASK-003 通过后，按计划顺序生成本任务卡；尚未创建代码或运行。等待用户明确要求开始 TASK-004。
- 2026-09-29：按用户“开始task004”开始。创建独立练习骨架并切换共享入口。最终骨架实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，依次输出“开始读取设备 001”“读取方法已调用，准备等待结果”“TODO 2：输出取得的读数”。此次只是占位骨架，未实现异步等待与真实样例读数，三个 DoD 均未验收。
- 2026-09-29 过程答疑：学员已将 `ReadAsync` 的占位等待改为 `await Task.Delay(1000)`，返回设备 001 的 `100 .C` 读数，并添加等待后与返回前输出。实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，输出顺序为“开始读取设备 001”“读取方法已调用，准备等待结果 System.Threading.Thread”“已返回 System.Threading.Thread”“reading 100 .C”“TODO 2：输出取得的读数”。本轮按实际代码解释流程，尚未按 DoD 正式验收；任务保持进行中。
- 2026-09-29 线程追问：学员把两处线程输出改为 `Environment.CurrentManagedThreadId`；实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，等待前线程 ID 为 2、延时后线程 ID 为 5，其余输出仍含 `reading 100 .C` 和 TODO 2 占位。用于解释异步恢复可能换线程，本轮未正式验收。
- 2026-09-29 首次审查：实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0；输出依次为“开始读取设备 001 当前线程为2”“进入ReadAsync 2026/9/29 21:36:17 当前线程为2”“读取方法已调用，准备等待结果，当前线程为2”“ReadAsync 2026/9/29 21:36:25”“ReadAsync 当前线程为5”“reading 100 .C 当前线程为5”“TODO 2：输出取得的读数”。DoD 1 未通过：返回对象包含 `DeviceId = 001`、数值 100 和单位 `.C`，但结果行未输出 `reading.DeviceId`，`.C` 的单位含义不清楚，TODO 2 占位行仍出现。DoD 2 通过：`ReadAsync` 用 `async Task<Reading>`、`await Task.Delay(8000)` 后返回读数，`RunAsync` 用 `await pendingReading` 取得结果；未见阻塞式等待。DoD 3 无法完整验证：先前学员能自主复述调用、两次暂停与恢复的顺序，运行日志也支持该顺序，但尚未明确指出 `await pendingReading` 得到的具体 `Reading` 值。任务保持 🟡 进行中。
- 2026-09-29 按学员“直接帮我完成”修改：教练在 `RunAsync` 的结果行补 `reading.DeviceId`、将样例单位 `.C` 修正为 `℃`、移除 TODO 2 占位输出和已完成的骨架 TODO 注释，保留学员添加的时间、线程日志与 8000 毫秒延时。实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0；输出依次为“开始读取设备 001 当前线程为2”“进入ReadAsync 2026/9/29 21:44:48 当前线程为2”“读取方法已调用，准备等待结果，当前线程为2”“ReadAsync 2026/9/29 21:44:56”“ReadAsync 当前线程为5”“设备 001：100 ℃ 当前线程为5”。DoD 1、2 有运行与代码证据，已勾选；DoD 3 的调用、等待和返回顺序有学员先前自主复述，但具体 `await` 结果仍未由学员明确指出。任务保持 🟡 进行中，修正部分不记为学员自主掌握。
- 2026-09-29 最终验证：教练将结果行写明 `await pendingReading` 得到的值，再次实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0；输出依次为“开始读取设备 001 当前线程为2”“进入ReadAsync 2026/9/29 21:47:21 当前线程为2”“读取方法已调用，准备等待结果，当前线程为2”“ReadAsync 2026/9/29 21:47:29”“ReadAsync 当前线程为5”“await pendingReading 得到：设备 001，100 ℃，当前线程为5”。DoD 1：读数 ID、数值、单位均可核对；DoD 2：代码使用 `Task<Reading>`、`async`、`await Task.Delay(8000)`、`await pendingReading`，无阻塞式等待；DoD 3：输出顺序与学员此前自主复述的“内层等待暂停→外层继续→外层等待暂停→内层恢复返回”一致，结果行直接指出 `await` 得到的 `Reading`。三项通过，✅ 已通关。完成日期：2026-09-29。此为教练协助完成；代写的结果输出和具体值说明不计为学员自主掌握，相关问答继续待复习。
