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

## Q3：`get => _resultText;`、`set { ... }` 和 `value` 是什么意思？

- 日期：2026-10-09
- 状态：待复习
- 关联：[TASK-010](../tasks/TASK-010.md)、[ReadingDisplayState.cs](../../WpfBae/Exercises/TASK-010-reading-binding/ReadingDisplayState.cs)
- 现象：学员明确反馈此前未学过带实现的 get/set，无法理解骨架中的箭头简写。既有属性问答仍待复习，不能根据前面使用过 get/init 或填过数据模型就默认其已掌握。本次是教学缺少铺垫，不记录为学员认知错误。

### 先看普通写法

```csharp
private string _resultText = "尚未读取";

public string ResultText
{
    get
    {
        return _resultText;
    }
    set
    {
        _resultText = value;
    }
}
```

`_resultText` 是实际保存字符串的私有字段；`ResultText` 是公开属性，规定外部读取与赋值时要执行的代码。它们不是同一个名字的变量，属性通过自己的 get/set 访问字段。

读取 `string text = state.ResultText;` 时进入 get，返回 `_resultText` 当前值。执行 `state.ResultText = "读取完成";` 时进入 set，赋值右侧的“读取完成”就是这次 setter 中的 value；`_resultText = value` 将新值存进字段。之后再次读取属性会返回新值。

value 是 setter 中 C# 自动提供的隐式参数，类型与属性一致，当前为 string，不需要另外声明它。赋值会调用 setter，但保存到哪个字段、是否保存由 setter 的代码决定。若真的写空的 `set { }`，赋值时只执行空代码块，不会自动更新 `_resultText`；原任务完整代码有 `_resultText = value`，所以保存动作存在。

### 箭头写法

`get => _resultText;` 等价于这里的 `get { return _resultText; }`，是读取部分的简写。此处箭头用于简写访问器，不是在给 Binding 传 Lambda，也不是给字段赋值。首次讲解先用完整花括号与 return，再解释简写。

### 与普通方法的类比

逻辑上可以类比熟悉的 getter/setter 方法：

```csharp
public string GetResultText()
{
    return _resultText;
}

public void SetResultText(string newText)
{
    _resultText = newText;
}
```

访问属性时写 `state.ResultText` 和 `state.ResultText = "读取完成"`；使用普通方法时写 `state.GetResultText()` 和 `state.SetResultText("读取完成")`。这是帮助理解的逻辑对照，示例不要求把任务属性替换成方法，XAML Binding 仍按公开属性取值。

### 当前教学与复查范围

先理解读取进入 get、赋值进入 set、value 来自赋值右侧，再继续学习 setter 中的 PropertyChanged 通知。不引入新口试、不要求独立新练习。后续依据本任务自主属性实现与实际读写行为核对掌握；本轮未改源码、未运行或验收，状态待复习，TASK-010 保持进行中。

2026-10-09 后续 TASK-010 审查：学员在已有 setter 中增加通知调用，按钮赋值进入这些 setter，实际界面更新正确；这项属性使用证据已记录在任务与绑定 Q1。get、字段赋值/value 及属性结构仍由骨架提供，尚无自主编写这些语法的新证据，本条完整属性语法保持待复习，不因任务通关全部标已掌握。
