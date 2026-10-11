# WPF 数据绑定

> [返回问答索引](./README.md)

## Q1：Binding、DataContext 和 INotifyPropertyChanged 分别做什么？

- 日期：2026-10-09
- 状态：已掌握
- 来源：TASK-010 启动的必要入门讲解；按学员 WPF 初学者背景，先说明用途和最小语法，再提供动手步骤。

### 场景与原因

TASK-009 在按钮处理器中直接给 TextBlock.Text 赋值。TASK-010 把待显示文字保存到数据对象的属性中，由 WPF 将数据属性连接到控件属性。当数据变化时，界面需要知道哪些属性应重新读取。

### 机制与最小语法

```xml
<TextBlock Text="{Binding Caption, Mode=OneWay}" />
```

Text 是目标属性，Caption 是数据对象的公开属性；大括号表示绑定表达式，不是普通文字。OneWay 是数据对象到控件的单向显示。普通的 `Text="Caption"` 只显示单词 Caption。

DataContext 提供默认来源对象。设置窗口 DataContext 后，内部控件通常继承它，所以这里不必在每个 TextBlock 上重复指定来源。Binding 根据属性名读取该对象的值，属性名大小写必须匹配。

第一次连接时，WPF 读取初始值。之后数据对象内部某个字段变了，WPF 不会持续轮询字段；对象应通过属性变化通知让绑定重新读取公开属性。本次使用的 INotifyPropertyChanged 是 .NET 提供的接口，它约定对象具有 PropertyChanged 事件，事件参数告诉监听方哪个属性变了。声明接口和事件并不会自动发出事件，还要在 setter 保存新值之后主动通知。

任务中的属性拆成三个部分：

- `_resultText` 字段：保存当前字符串。
- `get`：返回当前值；绑定从公开 ResultText 属性读取它。
- `set`：接收赋入的新值，`value` 表示这个新值；保存后调用提供的 OnPropertyChanged 工具。

`nameof(ResultText)` 得到字符串 `"ResultText"`；它不会读取属性值，用它可以减少手写名称的错误。工具中的 `PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))` 负责发布事件：this 是发出事件的数据对象，参数携带属性名；?.Invoke 表示仅在有订阅者时调用。

可以联系 TASK-002 的事件经验理解：数据对象发布变化事件，WPF 的绑定监听事件并读取新值；监听和更新控件的工作由 WPF 完成，不需在窗口中手工订阅。

数据流为：按钮修改数据属性 → setter 保存值并通知 → Binding 重新读值 → 控件更新。当前任务输入框仍直接读取 Text，只绑定结果与状态两个输出。

### 具体起步与常见坑

先把窗口 DataContext 指向骨架中的 `_state`，再把结果 Text 绑定到 ResultText；运行后应显示字段中的初始值“尚未读取”。然后连接 StatusText，完成两个 setter 的通知和按钮逻辑。仅显示初始值还不能证明通知有效，必须通过多次读取及空白/恢复场景验证。

按钮要修改 DataContext 指向的同一个对象；如果另建对象修改，界面仍监听旧对象。不要再直接给结果/状态控件的 Text 赋值，它可能替换原先的绑定；也不要把字段名 `_resultText` 写进 Binding 路径。

### 复查依据

2026-10-09 TASK-010 审查：学员自主设置 DataContext、两个 OneWay Binding、setter 中对应名称的通知调用及按钮数据属性更新，实际五种输入场景均通过；收到两个 PropertyChanged，控件跟随同一数据对象变化，绑定始终保留。标为已掌握的范围仅限本次基本单向显示绑定与调用已提供工具通知变化，证据见[任务验收](../tasks/TASK-010.md)及[运行结果](../tasks/evidence/TASK-010/results.json)。事件声明、get/set 保存逻辑和发布工具由骨架提供，nameof、?.Invoke、框架参数类及一般化设计能力不在掌握范围内，相关问答不随之全部更新。

## Q2：ReadingDisplayState 是什么？DataContext = _state 究竟做了什么？

- 日期：2026-10-09
- 状态：待复习
- 来源：学员反馈前述讲解仍默认了解 DataContext 和骨架中的 ReadingDisplayState。应先说明自定义类型、实例和 WPF 属性各自的身份，再解释它们的连接。

### 现象与原因

ReadingDisplayState 是教练在本任务中新建、自行命名的 C# 类。前述说明没有先交代它的来源，就用它解释另一个新概念 DataContext，造成理解断层。这是教学步骤缺失，不作为学员错题。

### 先识别类和对象

学员在 TASK-009 自己定义了 Reading 类，其中保存 DeviceId、Value、Unit。ReadingDisplayState 的角色相似，也是普通 C# 类，只是它保存窗口需要显示的两段文字：ResultText（结果）和 StatusText（状态）。它的名字可理解为“读数显示状态”，不是 WPF 固定 API 名；名称可以自行选择，当前骨架统一使用这个名字。

窗口中的现有代码为：

```csharp
private readonly ReadingDisplayState _state = new();
```

其中核心的对象创建部分等价于：

```csharp
ReadingDisplayState _state = new ReadingDisplayState();
```

ReadingDisplayState 是类型，new 创建该类型的对象，_state 是保存对象引用的字段名。new() 是省略已知类型名称的写法；private 表示这个字段供本窗口类内部使用，readonly 表示初始化后不重新赋予它另一个对象引用，不妨碍修改对象的属性。

当前对象创建后，读取 `_state.ResultText` 会得到“尚未读取”，读取 `_state.StatusText` 会得到“请输入设备 ID，然后读取温度。”；初始字符串来自 ReadingDisplayState.cs 中的两个字段。它不包含窗口或输入框，只保存要显示的内容及后续变化通知。

### 再解释 DataContext

DataContext 是 WPF 提供的属性。Task010Window 继承 Window，因此它具有这个属性。作用是给绑定提供默认来源对象；它接受对象引用，不是文件名、类名字符串或待显示的文字。

在本窗口的构造函数中：

```csharp
DataContext = _state;
```

等价于写 `this.DataContext = _state;`。这句话把当前窗口的默认数据来源设为刚创建的那个对象，不会复制对象，也不会创建第二个对象，更不会立即把全部属性显示到窗口。

### 和 Binding 接起来

```xml
<TextBlock Text="{Binding ResultText}" />
```

这里 TextBlock 没有自己另外指定 DataContext，便继承窗口的数据来源，也就是 _state。Binding 再按 ResultText 这个属性名读值，所以初次连接会显示“尚未读取”。在本例中取值路径可理解为“控件继承窗口 DataContext → 得到 _state → 读取 _state.ResultText → 用作 TextBlock.Text”。仅设置 DataContext 而保留普通静态 Text，界面仍显示原静态文字。

本轮只说明数据对象和初次取值。以后属性改变时的刷新属于 Q1 中的通知机制，应在这个来源关系清楚后再学习。

### 当前证据与下一个动作

2026-10-09 读取现有源码发现学员已增加 DataContext = _state；保留该行，未重置或修改源码。本轮未运行或验收，不以已经写入这行认定理解或 DoD 通过。下一步结合上述来源关系，在 XAML 的结果控件完成 TODO 2a，运行观察是否显示“尚未读取”。

2026-10-09 后续审查补充：DataContext 与按钮修改的 _state 同一实例、两个绑定的运行时取值及更新已验证，局部应用计入 Q1。类结构和对象实例声明由骨架提供，new()/readonly 等说明暂无自主应用新证据，本条整体仍待复习，不影响 TASK-010 通关。

## Q3：Rider 的 Binding 后面为什么有 (???)？预览没有显示属性值怎么办？

- 日期：2026-10-09
- 状态：待复习
- 来源：学员截图中 Binding 后显示灰色 (???) 与 Path= 提示，下方 XAML 预览的结果和状态区域没有文字。

### 现象与原因

实际磁盘 XAML 是 `Text="{Binding ResultText, Mode=OneWay}"`，不存在 (???) 或灰色 Path=。这些灰色内容是 Rider 编辑器附加的内嵌提示。JetBrains 官方说明：[XAML 内嵌提示](https://www.jetbrains.com/help/rider/Settings_Inlay_Hints_XAML.html)包括隐含属性名及 DataContext 类型。结合提示位置及源码，判断此处 (???) 表示编辑器尚不能确定该绑定的数据来源类型；这是对当前提示的解释，不是 WPF 运行错误的证据。

截图下方为 XAML 预览；[Rider 官方说明](https://www.jetbrains.com/help/rider/Languages_XAML.html)将其定义为编辑时的 XAML 预览。当前数据来源在窗口 C# 构造函数中设置，截图的预览没有呈现这些运行时数据。不能据预览空白判定实际程序取值失败，也不泛化为所有设计器都绝不执行 C#。

### 实际运行依据

2026-10-09 实际执行 `dotnet build .\WpfBae\WpfBae.csproj --no-restore`，退出码 0，0 警告、0 错误。临时检查程序引用本次构建的原 WpfBae.dll，在 STA/WPF 消息循环中打开原 Task010Window，等待 Loaded 和绑定处理后读取控件值：

- DataContext 的实际类型：`WpfBae.Exercises.Task010.ReadingDisplayState`。
- 数据对象 ResultText：`尚未读取`；StatusText：`请输入设备 ID，然后读取温度。`。
- 结果控件 Binding.Path：ResultText；实际 Text：`尚未读取`。
- 状态控件 Binding.Path：也为 ResultText；实际 Text：也为 `尚未读取`。
- 窗口 IsVisible 为 true；临时检查进程退出码 0，已结束，未修改学员源码。

检查程序及结果位于 `C:\Users\lbwxxc\AppData\Local\Temp\TASK-010-binding-check-f5e00680187a483cb5f1d469e16661a2/`；结果文件为 `bin/Debug/net10.0-windows/initial-binding.json`。该验证只证明当前初始取值，不证明 setter 通知或按钮行为已完成。

### 修复线索与总结

初始结果绑定已经取到值；编辑器的 (???) 不是需要传入的数据，也不是 Binding 的参数。当前需要修正的是第二个控件的路径：StatusTextBlock 应绑定 StatusText，不能仍绑定 ResultText。改完后实际运行项目观察两段初始文字，再继续实现属性变化通知。当前问答状态待复习，TASK-010 保持进行中。

2026-10-09 后续审查：状态路径已由学员修正为 StatusText，实际初始状态与多次更新均正确，TASK-010 已通关；编辑器提示及预览机制的说明仍待复习，不根据运行通过认定这一类工具概念全部掌握。

## Q4：PropertyChangedEventArgs 这个类从哪里来？

- 日期：2026-10-09
- 状态：待复习
- 来源：学员询问属性变化通知代码中的类型来源。已核对现有源码和本机 .NET 10.0.12 参考文档 System.ObjectModel.xml。

### 来源与用途

PropertyChangedEventArgs 是 .NET 自带的类，完整名称为 `System.ComponentModel.PropertyChangedEventArgs`。项目已有标准框架引用，不需要自行定义这个类或安装额外包。文件开头的 `using System.ComponentModel;` 允许用短名称访问这个命名空间中的类型，可联系 Java 的 import 理解。

它用作 PropertyChanged 事件携带的参数，告诉监听方哪个属性发生了变化。比如创建一个参数对象：

```csharp
PropertyChangedEventArgs args = new PropertyChangedEventArgs("ResultText");
```

new 创建 .NET 提供的这个类型的实例，构造函数接收属性名字符串；这个对象的 `args.PropertyName` 就是 `"ResultText"`。传入属性名，不传温度值或整段结果文字。

### 放回当前代码

自定义的 OnPropertyChanged 方法接收 string propertyName；现有 setter 中调用 `OnPropertyChanged("ResultText")` 时，该参数为 `"ResultText"`。方法内部的 `new PropertyChangedEventArgs(propertyName)` 把这个名称装进事件参数对象，随 PropertyChanged 通知传给监听方。WPF 收到后，再读取数据对象的 ResultText 属性，得到要显示的当前文字。

ReadingDisplayState 与 OnPropertyChanged 是本练习自行定义的类/辅助方法；PropertyChangedEventArgs、PropertyChangedEventHandler 和 INotifyPropertyChanged 均由 .NET 提供。此次只讲解参数类的来源与携带内容，不默认已经理解其他类型或事件调用简写。

### 证据边界

本轮读取源码发现学员已经在两个 setter 添加 `OnPropertyChanged("ResultText")` 和 `OnPropertyChanged("StatusText")`；保留这些实现，字符串写法合法，不强制替换为 nameof。本轮未修改源码、未运行或验收，不将通知链路标已掌握，TASK-010 保持进行中。

2026-10-09 后续审查：两个通知调用已实际验证，PropertyName 与来源正确；调用通知工具的应用范围计入 Q1。PropertyChangedEventArgs 的创建仍在教练提供的辅助方法中，本条类型来源与参数机制保持待复习，不因任务通关自动扩大掌握范围。
