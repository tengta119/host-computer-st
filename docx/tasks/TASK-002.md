# TASK-002：用事件传递设备读数

- 阶段：第一阶段：C# 基础
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「二、第一阶段：C# 基础」的委托/事件重点；本任务是据此设计的练习，不是计划原文指定任务。
- 先修：TASK-001 的设备与读数模型练习；计划原文未规定强制先修。
- 难度/预计时间：待补充（计划未给出）。

## 场景与学习目标

模拟设备产生新读数时通知监控端：设备负责发出通知，监控端负责订阅、处理和取消订阅。通过这个小场景理解 C# 委托与事件的关系，以及事件数据如何从产生处传到处理处。本任务暂不引入 WPF、真实通信或异步。

## 用户实践与产出

- 在现有 .NET 项目中完成独立练习，模拟至少两次读数到达；订阅者收到通知后输出设备 ID 与读数。
- 专属目录：`Base/Exercises/TASK-002-device-reading-event/`；已创建 [`Task002Exercise.cs`](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)。复用 TASK-001 中的 [`Reading` 模型](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)，不修改已有练习。
- 共享入口例外：[`Base/Program.cs`](../../Base/Program.cs) 仅切换为调用 `Task002Exercise.Run()`；复用 [`Base/Base.csproj`](../../Base/Base.csproj)，未更改项目文件或增加依赖。

## 练习骨架与起步

- TODO 1：在 `SimulatedDevice` 声明携带 `ReadingReceivedEventArgs` 的事件；`ProduceReading` 用设备 ID 和输入值构造 `Reading` 并发布通知 → 实践步骤 1、DoD 2。
- TODO 2：在 `Run` 中订阅 `OnReadingReceived`；处理器输出设备 ID、数值与单位 → 实践步骤 2、DoD 1、2。
- TODO 3：前两次读数之后取消同一个处理器的订阅；第三次读数不应再触发该处理器 → 实践步骤 3、DoD 3。
- 首个动作：打开 `Task002Exercise.cs`，从 TODO 1 的事件声明和发布入手。随后完成订阅、处理器和取消订阅。运行命令：`dotnet run --project .\Base\Base.csproj`（在工作区根目录执行）。

## 建议实践步骤

1. 定义一次读数到达所需的数据，并在设备侧声明事件。
2. 在监控侧订阅事件；模拟设备产生至少两次读数，观察处理器输出。
3. 取消订阅后再产生一次读数，验证该处理器不再收到通知。
4. 用自己的代码说明事件由谁发出、谁订阅，以及取消订阅的效果。

## 验收标准（DoD）

以下为建议验收项，可按实际学习反馈调整：

- [x] 练习可运行；至少两次模拟读数触发订阅者，输出中可核对设备 ID 与读数。
- [x] 使用 C# 的委托/事件机制实现通知，事件数据与处理器职责清晰。
- [x] 演示取消订阅后的效果，并用运行输出证明该处理器不再收到后续读数。

## 状态与证据

- 2026-09-27：按用户“开启task002”开始；已准备独立代码骨架并切换共享入口。实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0、无输出；这是尚未实现事件发布和订阅的预期骨架状态，所有 DoD 均未验收。
- 2026-09-27 首次审查：实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，输出两行 `eReading: 5 ℃`、`eReading: 6 ℃`。源码中前两次读数前用 `+=` 订阅，`ProduceReading` 通过 `ReadingReceived?.Invoke` 发布事件；随后用 `-=` 取消同一处理器订阅并产生第三次读数，运行输出没有第三行。
- DoD 1 未通过：程序可运行且收到两次读数，但创建 `Reading` 时没有设置 `DeviceId`，处理器输出也没有设备 ID，无法核对“设备 ID 与读数”。
- DoD 2 未通过（部分实现）：事件声明、发布、订阅及处理器调用链已成立；事件数据中的设备 ID 仍为空，尚未完整传递本场景所需数据。
- DoD 3 通过：源码明确在第三次 `ProduceReading(7.0, "℃")` 之前取消订阅，运行仅输出前两次读数，没有 `7.0 ℃` 对应的处理器输出。
- 修复线索：检查 `ProduceReading` 构造 `Reading` 时用到了哪些可用的设备信息，再检查 `OnReadingReceived` 输出了 `Reading` 的哪些属性。保留学员实现，待修复后复查；当前状态仍为进行中。
- 2026-09-27 第二次审查：实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，输出 `eReading: 001 5 ℃`、`eReading: 001 6 ℃`；第三次产生 `7.0 ℃` 后仍无处理器输出。
- DoD 1 通过：当前样例两次通知均输出可核对的设备 ID、数值与单位。DoD 3 继续通过：取消订阅后的第三次读数没有处理器输出。
- DoD 2 未通过：`ProduceReading` 构造 `Reading` 时将 `DeviceId` 写死为 `"001"`，没有取当前 `SimulatedDevice` 的 `Id`。当前单设备样例恰好一致，但换用其他设备实例会传出错误 ID，事件数据仍未准确表达发布者。修复后需再验证，任务保持进行中。
- 2026-09-27 最终复查：`ProduceReading` 现用设备实例的 `Id` 创建 `Reading.DeviceId`；事件由设备通过 `ReadingReceived?.Invoke(this, readingEventArgs)` 发出，`Run` 中以 `+=` 订阅、以 `-=` 取消同一处理器，处理器从 `e.Reading` 输出 ID、数值和单位。实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，输出 `eReading: 001 5 ℃`、`eReading: 001 6 ℃`；第三次 `ProduceReading(7.0, "℃")` 未产生处理器输出。DoD 1、2、3 全部通过。
- 审查结论：✅ 已通关。完成日期：2026-09-27。事件声明、发布、订阅、取消订阅和设备 ID 修复均为学员在骨架上完成；最初的事件数据类型与运行入口由系统预备。
