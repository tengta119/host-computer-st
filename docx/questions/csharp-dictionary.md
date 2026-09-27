# C# Dictionary

## Q1：`Dictionary<TKey, TValue>` 如何存储和查找设备读数？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[`Task001Exercise.cs`](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)
- 现象：练习中已有 `Dictionary<string, Device>` 与 `Dictionary<string, Reading>`，需要按设备 ID 查找，并处理不存在的设备及没有读数的设备。
- 原因与机制：`Dictionary<TKey, TValue>` 按唯一键关联值；这里键是设备 ID，值可以是 `Device`、单条 `Reading`，或多条读数的集合。`Add` 遇重复键抛异常，索引器赋值可新增或覆盖；用索引器读取缺失键会抛异常，`TryGetValue` 可安全判断。两个字典分别查询，才能区分设备不存在与设备存在但没有读数。
- 类比：类似 Java `Map<K,V>`；C# 的 `TryGetValue(key, out var value)` 与 Java 常用的 `containsKey` / `get` 组合有相近用途，但一次查询即可取得结果。
- 用法：`if (dictDevice.TryGetValue(id, out var device)) { /* 使用 device */ }`；随后对 `dictReading` 做 `TryGetValue`。遍历时 `foreach (var pair in dictReading)`，用 `pair.Key` 和 `pair.Value`。如一台设备有多条读数，可选 `Dictionary<string, List<Reading>>`。
- 复查点：学员在任务实践中自主完成两层查找，并演示存在、不存在、无读数三种情况。
