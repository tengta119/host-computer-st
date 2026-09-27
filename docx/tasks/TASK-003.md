# TASK-003：用 Lambda 与 LINQ 筛选设备读数

- 阶段：第一阶段：C# 基础
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「二、第一阶段：C# 基础」中的 Lambda/LINQ；本任务是据此设计的练习，不是计划原文指定任务。
- 先修：可复用 TASK-001 的 `Reading` 模型；计划原文未规定强制先修。
- 难度/预计时间：待补充（计划未给出）。

## 场景与学习目标

监控程序积累多台设备的读数后，需要找出某台设备的读数以及超过阈值的读数。通过同一组样例数据，练习用 Lambda 表达筛选条件，使用 LINQ 查询和整理结果，并观察 Lambda 如何作为委托传给方法。本任务先处理内存中的数据，不涉及真实通信、WPF 或异步。

## 用户实践与产出

- 在现有 .NET 项目中完成独立练习，准备至少两台设备、合计至少四条读数，其中包括符合和不符合筛选条件的样例。
- 专属目录：`Base/Exercises/TASK-003-reading-query/`；已创建 [Task003Exercise.cs](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)，复用 TASK-001 的 [Reading 模型](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)。
- 共享入口例外：[Base/Program.cs](../../Base/Program.cs) 已切换为调用 `Task003Exercise.Run()`；复用 [Base/Base.csproj](../../Base/Base.csproj)，未更改项目文件或增加依赖。

## 练习骨架与起步

- TODO 1：在 `readings` 中准备至少两台设备、至少四条读数，包含目标设备的阈值两侧读数 → 实践步骤 1、DoD 1、2。
- TODO 2：把 `matchingReadings` 的空结果占位替换为 Lambda + LINQ 查询，同时检查设备 ID 和数值阈值 → 实践步骤 2、4，DoD 1、2。
- TODO 3：在调用 `CountMatching` 时传入相同筛选条件的 Lambda，并在方法内调用 `predicate` 计数 → 实践步骤 3，DoD 3；比较计数与 LINQ 结果也帮助核对 DoD 1。
- 首个动作：打开 [Task003Exercise.cs](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)，从 TODO 1 添加样例读数开始。于工作区根目录运行 `dotnet run --project .\Base\Base.csproj`，完成后可说“检查 TASK-003”。

## 建议实践步骤

1. 准备多台设备的读数集合，明确要查询的设备 ID 和阈值。
2. 用 Lambda 表达筛选条件，通过 LINQ 取出匹配读数并输出设备 ID、数值和单位。
3. 再用一个接收委托的简单方法处理筛选结果或输出，说明 Lambda 是如何作为参数传入的。
4. 展示查询结果，并解释同一条读数为何被保留或排除。

## 验收标准（DoD）

- [x] 练习可运行；至少两台设备、合计至少四条读数，输出可核对目标设备和阈值条件的筛选结果。
- [x] 使用 Lambda 和 LINQ 完成查询，代码中的筛选条件与结果清晰；能用样例说明符合和不符合条件的读数。
- [x] 至少一次将 Lambda 传给接收委托的方法，并在代码或简短说明中指出参数、返回值和调用时机。

## 状态与证据

- 2026-09-27：TASK-002 通关后，按计划顺序生成本任务卡。
- 2026-09-27：按用户“开启task003”开始；创建独立练习骨架并切换共享入口。实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，输出“设备 001 中高于 30 的读数：”和“匹配数量：0”。这是空样例及占位逻辑的骨架运行结果，三个 DoD 均未验收。

- 2026-09-27 审查：实际执行 `dotnet run --project .\Base\Base.csproj`，退出码 0，输出“设备 001 中高于 30 的读数：”“001: 70 km”“匹配数量：1”。DoD 1 通过：样例有设备 001、002 各两条读数，ID、数值、单位齐全；输出与目标设备和阈值一致。DoD 2 通过：`Where` 的 Lambda 同时判断 `Value > threshold` 和 `DeviceId == targetDeviceId`；001 的 10 不达阈值，002 的 40、60 不属于目标设备，只有 001 的 70 保留。DoD 3 通过：同一判断条件的 Lambda 传给 `CountMatching` 的 `Func<Reading, bool> predicate`；方法在逐条遍历时调用 `predicate(reading)`，以当前读数为参数、布尔值为返回值，匹配时计数，最终返回 1。
- 审查结论：✅ 已通关。完成日期：2026-09-27。TODO 1、2、3 均由学员在骨架上实现。
