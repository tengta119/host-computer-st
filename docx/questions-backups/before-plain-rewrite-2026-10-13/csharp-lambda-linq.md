# C# Lambda 与 LINQ

## Q1：没有接触过 C# Lambda 和 LINQ，如何理解 TASK-003 中的筛选？

- 状态：待复习
- 关联：[TASK-003](../tasks/TASK-003.md)；[Task003Exercise.cs](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)
- 现象：任务骨架有 `reading => false`、`Func<Reading, bool>` 和待实现的读数筛选；学员说明此前未接触 C# Lambda 与 LINQ。
- 原因与机制：Lambda 是可传递的匿名函数，`x => x > 30` 的左边是参数，右边是返回布尔值的表达式。`Func<Reading, bool>` 表示接收一条 `Reading`、返回 `bool` 的委托类型；调用该委托时才执行 Lambda。LINQ 为集合提供 `Where` 等查询方法，`Where(predicate)` 对每个元素应用条件，保留结果为 `true` 的元素；它通常延迟到遍历或 `ToList()` 时执行。
- 类比：Lambda 接近 Java 的 `x -> x > 30`；对集合使用 LINQ `Where` 接近 Java Stream 的 `filter`。
- 易错点：`>` 不包含等于阈值的读数；`reading => false` 会排除所有读数；只写 `Where` 而从不遍历或物化结果时，尚未实际执行筛选。设备 ID 与阈值应分别作为筛选条件。
- 复查点：在 TASK-003 自主完成 Lambda/LINQ 查询，把 Lambda 传给接收 `Func<Reading, bool>` 的方法，并解释参数、返回值与调用时机。

## Q2：“把相同条件的 Lambda 传给 CountMatching”和“委托调用”是什么意思？

- 状态：已掌握
- 关联：[TASK-003](../tasks/TASK-003.md)；[Task003Exercise.cs](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)
- 现象：LINQ `Where` 已用 Lambda 判断读数，但 `CountMatching(readings, reading => false)` 仍是占位，方法中的 `predicate` 也尚未调用。
- 原因与机制：“相同条件”是指与 `Where` 一致的业务判断：设备 ID 等于目标 ID，且数值大于阈值。调用 `CountMatching` 时把表达该判断的 Lambda 作为第二个参数传入。`Func<Reading, bool> predicate` 是接收这个函数的委托参数；方法遍历读数时执行 `predicate(reading)`，就是委托调用。每调用一次，就把当前 `Reading` 传给 Lambda，得到 `true` 或 `false`；传入 Lambda 本身不会立即执行全部判断。
- 类比：类似 Java 把 `Predicate<Reading>` 传给方法，再在方法内部用 `predicate.test(reading)` 对每条读数求值。
- 易错点：`reading => false` 永远返回 `false`；若计数条件与 `Where` 条件不同，两处结果可能不一致。委托调用是执行传入的函数，与触发 `event` 不同。
- 复查点：学员自主替换占位 Lambda、在 `CountMatching` 中调用 `predicate`，并用当前样例说明目标设备的高阈值读数被计入，其他读数被排除。
- 掌握依据：2026-09-27 TASK-003 审查中，学员将相同条件的 Lambda 传入 `CountMatching`，在 `foreach` 内调用 `predicate(reading)` 并计数；实际运行输出匹配数量 1。参数、布尔返回值和逐条调用时机在代码中可核对。参见[验收记录](../tasks/TASK-003.md)。
