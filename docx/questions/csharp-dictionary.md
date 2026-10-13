# C# Dictionary

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：`Dictionary<TKey, TValue>` 如何存储和查找设备读数？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[`Task001Exercise.cs`](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)

在 [TASK-001 查询代码](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)中，设备 003 存在但没有读数，999 连设备都不存在。要给这两种情况不同提示，所以分别用两本表：dictDevice 按 ID 保存设备对象，dictReading 按 ID 保存读数对象。

```csharp
Dictionary<string, Device> dictDevice = new Dictionary<string, Device>();
dictDevice.Add("001", device001);

Device device;
bool found = dictDevice.TryGetValue("001", out device);
```

Dictionary 是 .NET 的字典类；`<string, Device>` 表示用字符串作键、保存 Device 对象作值。Device 和 device001 来自当前练习，device001 指向已创建的设备。Add 把键 "001" 和这个对象关联起来。键必须唯一；重复 Add 会报错。

TryGetValue 是 .NET 的查询方法。它返回是否找到的 bool，同时把找到的对象交给 out 后的 device；找到时 found 为 true。查找失败则返回 false，Device 是引用类型，out 得到默认的 null，所以失败路径不能继续访问 device.Id。

`dictDevice["001"]` 可取已有对象，但读缺失的键会抛异常。`dictDevice["001"] = device001` 则可新增或替换该键对应的值。先用 TryGetValue 查询设备，设备存在后再查 dictReading，才能区分“无设备”和“设备无读数”。

这类似 Java Map 的按键关联；C# 的 TryGetValue 一次查询同时报告是否找到并取得值。遍历字典时每个 pair 的 Key 是 ID、Value 是对象；若一台设备要保留多条历史，可另设计保存读数列表的字典，本题不追加该实现。

## Q2：`out var device` 写在 `if` 条件里，为什么后面仍能访问？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[`Task001Exercise.cs`](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)

当前 foreach 中，查询失败后会执行 continue，直接开始下一个 ID，不会走到下方输出 device.Id 的代码。这是“为什么后面能安全使用”的关键，除了变量可见，还必须确认查找成功。

先把声明分开看，下面等价于当前查询的主要流程：

```csharp
Device device;
if (!dictDevice.TryGetValue(deviceId, out device))
{
    Console.WriteLine("设备不存在");
    continue;
}
Console.WriteLine(device.Id);
```

片段位于 foreach 循环体内；deviceId 是本轮待查 ID。`out device` 让 TryGetValue 在返回前给 device 赋值。`!` 把“找到了”反过来，因此找不到才进入 if。continue 结束当前这一轮，能到最后一行就意味着已找到设备。

原代码的 `out var device` 将声明合并到查询位置。var 是 C# 的类型推断，编译器从字典的值类型推断这里是 Device，并非任意动态类型。这种 if 条件中的 out 声明在当前循环体后面的语句仍可见；若在 if 花括号内部另外声明局部变量，范围则不同。

去掉 continue 后，失败路径也可能来到输出行，此时 Device 的默认值为 null，读取其属性会失败。变量“可以访问”不等于“已经查询成功”。第二次查询 reading 使用同样的判断与提前跳过。

## 历史证据与掌握范围

Q1/Q2 均保持待复习。TASK-001 的字典创建与填充是学员作品，查询、缺失设备和无读数的处理由教练提供；已有运行覆盖不能作为学员自主查询能力证据。本次没有修改或执行练习。

相关任务卡与验收来源：[TASK-001](../tasks/TASK-001.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-dictionary.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft：TryGetValue 的返回值与 out 参数](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.trygetvalue?view=net-10.0)
