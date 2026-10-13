# C# Lambda 与 LINQ

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：没有接触过 C# Lambda 和 LINQ，如何理解 TASK-003 中的筛选？

- 状态：待复习
- 关联：[TASK-003](../tasks/TASK-003.md)；[Task003Exercise.cs](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)

TASK-003 的四条样例中，目标是只留下“设备 001 且数值大于 30”的读数。001 的 10 不满足，001 的 70 满足；002 的两条即使大于 30，也不属于目标设备。

先把判断写成普通方法，下面是说明用的自定义方法，不要求替换已有作品：

```csharp
static bool IsTargetReading(Reading reading)
{
    return reading.DeviceId == "001" && reading.Value > 30;
}
```

Reading 是已有读数类；参数 reading 接到一条具体读数。`==` 判断设备 ID，`>` 不包含等于 30；`&&` 要求两个判断都为 true。方法把一个 bool 结果交回调用者。

再看同一判断的 Lambda 写法：

```csharp
Func<Reading, bool> predicate = reading =>
    reading.DeviceId == "001" && reading.Value > 30;
```

Func 是 .NET 提供的委托类型；这里规定接收 Reading、返回 bool。predicate 是保存判断方法的变量。箭头左边 reading 是参数名，右边是计算返回值的表达式；此时保存判断，之后调用 `predicate(oneReading)` 才判断那条读数。可联系 Java 的 `reading -> ...` 理解。

`readings.Where(predicate)` 使用 .NET LINQ 的 Where 方法，让每条读数接受这个判断，保留结果为 true 的项。它通常在 foreach 遍历时执行，而不是写出 Where 时就筛完整个列表；ToList 则会把查询结果执行并收集成列表。本题当前源码用 foreach 展示，见 [查询代码](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)。

`reading => false` 是永不保留任何读数的占位。既要数值满足，也要设备满足，不能只替换其中一个判断。

## Q2：“把相同条件的 Lambda 传给 CountMatching”和“委托调用”是什么意思？

- 状态：已掌握
- 关联：[TASK-003](../tasks/TASK-003.md)；[Task003Exercise.cs](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)

同一批数据既要列出符合条件的读数，又要算数量，两处必须用相同判断。“传给 CountMatching”是把判断方法交给它，不是先把某一条读数判断后的 true/false 交进去。

```csharp
Func<Reading, bool> predicate = reading =>
    reading.DeviceId == targetDeviceId && reading.Value > threshold;
int count = CountMatching(readings, predicate);
```

targetDeviceId、threshold、readings 均是练习中已有的变量。predicate 保存接收一条 Reading、返回 bool 的判断。CountMatching 是本项目自定义方法，第二个参数用同样的 Func<Reading, bool> 接收它。

该方法每次循环的核心代码是：

```csharp
if (predicate(reading))
{
    count++;
}
```

这里 reading 是本轮那条读数；`predicate(reading)` 调用保存的判断，得到 true 才把 count 加 1。传入方法时还没有逐条判断，遍历中带括号调用时才执行。这就是此处的委托调用。

可联系 Java 的 Predicate 参数和 `predicate.test(reading)` 理解。它与发布事件不同：此处一个普通方法明确调用传入的判断，返回值用来决定计数。已有样例最终应该只有设备 001 的 70，数量为 1；这是样例预期和历史验证结果，本轮没有重新运行。

## 历史证据与掌握范围

Q1 保持待复习，Q2 保持已掌握。2026-09-27 TASK-003 学员自主实现双条件 Where、传递相同 Lambda 和逐条 predicate 调用，运行匹配数量 1。已有应用范围保留；延迟执行等机制没有因此全部掌握。

相关任务卡与验收来源：[TASK-003](../tasks/TASK-003.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-lambda-linq.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft：Where 与延迟查询](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.where?view=net-10.0)
