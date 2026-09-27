# C# 属性

## Q1：`Reading` 中的 `get; init;` 有什么用？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[`Reading.cs`](../../Base/Exercises/TASK-001-device-model/Reading.cs)
- 现象：`Reading` 的 `DeviceId`、`Value`、`Unit` 使用了 `get; init;`。
- 原因与机制：`get` 允许读取属性；`init` 允许在对象初始化器或构造阶段赋值，但初始化结束后不能再次赋值。`= ""` 给字符串属性提供默认值；`Value` 未显式赋值时为 `0`。
- 类比：接近 Java 构造对象时设定字段、之后只提供 getter；不同于普通 `set`，它限制后续修改。
- 用法：`var r = new Reading { DeviceId = "D1", Value = 35.6, Unit = "℃" };` 可行；之后 `r.Value = 40;` 会产生编译错误。限制的是属性赋值，不保证引用类型指向的对象内部不可变。
- 复查点：在任务实践中能正确初始化读数，并解释 `init` 与 `set` 的区别。

## Q2：另一个 `.cs` 文件里的 `Device` 如何在 `Task001Exercise.cs` 使用？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[`Device.cs`](../../Base/Exercises/TASK-001-device-model/Device.cs)；[`Task001Exercise.cs`](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)
- 现象：`Device.cs` 声明 `public class Device`，目前没有命名空间；`Task001Exercise.cs` 位于 `Base.Exercises.Task001` 命名空间。
- 原因与机制：同一项目的 `.cs` 文件默认参与编译；访问类型按命名空间和可见性，不按文件名或目录。当前 `Device` 属于全局命名空间，`Task001Exercise` 可以直接使用 `new Device { Id = "D1", Name = "设备一" }`。如希望组织一致，可把 `namespace Base.Exercises.Task001;` 写在 `Device.cs` 顶部，之后仍可直接使用；跨命名空间可写 `using` 或完整类型名。
- 类比：Java 通常要求公共顶级类与文件同名，通过包和 import 找类型；C# 无一个文件只能有一个类的要求，namespace 才是组织类型的机制，目录本身不自动决定 namespace。
- 复查点：自主创建 `Device` 实例，说明文件、命名空间与项目编译的关系。
