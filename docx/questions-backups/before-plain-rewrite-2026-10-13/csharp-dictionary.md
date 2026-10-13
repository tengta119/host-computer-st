# C# Dictionary

## Q1：`Dictionary<TKey, TValue>` 如何存储和查找设备读数？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[`Task001Exercise.cs`](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)
- 现象：练习中已有 `Dictionary<string, Device>` 与 `Dictionary<string, Reading>`，需要按设备 ID 查找，并处理不存在的设备及没有读数的设备。
- 原因与机制：`Dictionary<TKey, TValue>` 按唯一键关联值；这里键是设备 ID，值可以是 `Device`、单条 `Reading`，或多条读数的集合。`Add` 遇重复键抛异常，索引器赋值可新增或覆盖；用索引器读取缺失键会抛异常，`TryGetValue` 可安全判断。两个字典分别查询，才能区分设备不存在与设备存在但没有读数。
- 类比：类似 Java `Map<K,V>`；C# 的 `TryGetValue(key, out var value)` 与 Java 常用的 `containsKey` / `get` 组合有相近用途，但一次查询即可取得结果。
- 用法：`if (dictDevice.TryGetValue(id, out var device)) { /* 使用 device */ }`；随后对 `dictReading` 做 `TryGetValue`。遍历时 `foreach (var pair in dictReading)`，用 `pair.Key` 和 `pair.Value`。如一台设备有多条读数，可选 `Dictionary<string, List<Reading>>`。
- 复查点：学员在任务实践中自主完成两层查找，并演示存在、不存在、无读数三种情况。

## Q2：`out var device` 写在 `if` 条件里，为什么后面仍能访问？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[`Task001Exercise.cs`](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)
- 现象：`if (!dictDevice.TryGetValue(deviceId, out var device))` 后面的 `Console.WriteLine` 仍能读取 `device.Id` 和 `device.Name`。
- 原因与机制：`out var device` 的声明位于 `if` 的条件表达式中，它的作用域延伸到当前 `foreach` 循环体的后续语句，不限于 `if` 的花括号。`TryGetValue` 在返回前给 `out` 参数赋值；查找失败时进入 `if` 并执行 `continue`，该轮循环直接结束。因此能到达后面输出语句的路径上，设备查找必定成功。第二次 `TryGetValue` 对 `reading` 也按同样方式处理。
- 类比：接近先在循环体中声明一个局部变量，再把它传给查询方法；变量仍在循环体范围内。这里的声明写在条件里，容易误以为只属于 `if` 的花括号。
- 复查点：能区分条件表达式中的 `out var` 声明与 `if` 代码块内声明，并判断去掉 `continue` 后缺失设备路径是否仍会执行后续输出。
