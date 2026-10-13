# C# 属性

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：`Reading` 中的 `get; init;` 有什么用？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[当前 Reading 定义](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)

一条设备读数创建时，需要记住“设备 001、数值 35.6、单位 ℃”。以后查看这条旧读数，不能因为当前设备变了就把它改掉。[当前 Reading 定义](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)是本项目自定义的 C# 类，不是 .NET 自带设备类。

```csharp
Reading reading = new Reading
{
    DeviceId = "001",
    Value = 35.6,
    Unit = "℃"
};
double temperature = reading.Value;
```

`new Reading` 创建一条读数；花括号在创建时给这个对象的三个属性赋值，这种写法叫对象初始化器。`reading` 保存该对象的引用。最后一行从这条读数的 Value 取出 35.6。

类中的 `public double Value { get; init; }` 使用 C# 属性语法。`get` 允许上面最后一行读取 Value；`init` 允许在创建对象的初始化阶段设置它。创建结束后再写 `reading.Value = 40;` 会报编译错误。普通 `set` 则允许以后继续赋值。这里没有把字段重新赋值的实现写出来，编译器为这种自动属性生成存储。

`public string Unit { get; init; } = "";` 中的 `= ""` 给未填写的 Unit 一个空字符串；未赋值的 double Value 默认是 0。限制的是这些属性的后续赋值；若属性保存一个可修改的集合，init 不会连集合内部一起冻结。

可联系 Java 中构造时设值、以后只暴露 getter 的读数对象理解。这里主要看“创建时能写，创建后能读”，不需要为了复习把已完成模型重新写一遍。

## Q2：另一个 `.cs` 文件里的 `Device` 如何在 `Task001Exercise.cs` 使用？

- 状态：待复习
- 关联：[TASK-001](../tasks/TASK-001.md)；[当前 Device 定义](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)；[`Task001Exercise.cs`](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)

先区分历史提问与当前文件：当时问题中的 Device 在另一个文件、尚未写 namespace；现在 Device 与 Reading 都定义在 [Task001Exercise.cs](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)，属于 `Base.Exercises.Task001`。旧的独立 Device.cs 链接已失效，不能再把它当成当前源码位置。

```csharp
using Base.Exercises.Task001;

Device device = new Device { Id = "001", Name = "温度传感器" };
```

这是跨命名空间使用 Device 的说明片段；`using` 放在文件开头，创建对象那行放在一个方法中。Device 是项目自定义类；`using Base.Exercises.Task001;` 让本文件可用短名字 Device。`new Device` 创建对象，初始化器设置 Id、Name，变量 device 保存它的引用。

另一个文件能使用这个类，是因为它属于同一项目的编译内容、访问权限允许，且代码能找到对应命名空间。C# 不靠“文件在同一个文件夹”建立类型引用；也不是运行时先打开 Device.cs 再读取类。SDK 项目默认把项目目录内的 .cs 源码纳入编译，另有排除配置时按配置处理。

如果两个类已经在同一命名空间，通常不用这行 using。若类没有写 namespace，它处在全局命名空间，当时可以直接用 Device。C# 允许在同一 .cs 文件放多个类；Java 的包/import 可帮助理解名字查找，但文件与类的规则不同。

## Q3：`get => _resultText;`、`set { ... }` 和 `value` 是什么意思？

- 日期：2026-10-09
- 状态：待复习
- 关联：[TASK-010](../tasks/TASK-010.md)、[ReadingDisplayState.cs](../../WpfBae/Exercises/TASK-010-reading-binding/ReadingDisplayState.cs)

先看实际赋值：按钮执行 `_state.ResultText = "读取完成";` 后，数据对象应保存这段文字；之后绑定读取 `_state.ResultText`，得到刚保存的内容。ReadingDisplayState 是本项目自定义的数据类，ResultText 是它的公开属性名。

下面是简化后的完整写法，只解释保存与读取；原类还保留变化通知：

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

`_resultText` 是真正保存字符串的字段；ResultText 是外部读写它的入口。读取 `string text = _state.ResultText;` 时执行 get，return 把字段当前值交给 text。

赋值 `_state.ResultText = "读取完成";` 时执行 set。C# 自动把等号右侧的“读取完成”交给 `value`，这里 value 的类型是 string。`_resultText = value` 才是实际保存动作；若写成空的 `set { }`，这次赋值不会自动保存到字段。

理解完整写法后，再看 `get => _resultText;`：它是 `get { return _resultText; }` 的简写。这个箭头在此简写属性读取，不是在执行 Lambda 回调。

可以类比普通 Java getter/setter：`GetResultText()` 返回字段，`SetResultText(string newText)` 保存传入文字。C# 属性把调用写成读取或赋值的形式，不要求把当前项目属性改成普通方法。后续 setter 中的 OnPropertyChanged 用于通知界面，先分清保存值与发通知两件事。

## 历史证据与掌握范围

三个问答均保持待复习。TASK-001 模型创建/填充有实践证据，但按 ID 查询由教练实现；TASK-010 的通知调用与数据属性更新已验证，完整 get/set、value、new()/readonly 等来自骨架，不能随任务通关全部升级。2026-10-09“未学过完整属性语法”的反馈是教学起点，不记为学员错误。本轮只重写文档和核对源码。

相关任务卡与验收来源：[TASK-001](../tasks/TASK-001.md)、[TASK-010](../tasks/TASK-010.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-properties.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft：init 属性初始化](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init)
