# WPF 数据绑定

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：Binding、DataContext 和 INotifyPropertyChanged 分别做什么？

- 日期：2026-10-09
- 状态：已掌握
- 来源：TASK-010 启动的必要入门讲解；按学员 WPF 初学者背景，先说明用途和最小语法，再提供动手步骤。

TASK-010 点击读取后，要让结果文字从“尚未读取”变成当前读数。结果先保存在数据对象 ResultText 中，再让结果控件从这里取值。先看最短连接：

```xml
<TextBlock Text="{Binding ResultText, Mode=OneWay}" />
```

TextBlock 是 WPF 文字控件。等号左边 Text 设置这块文字显示什么；右边 Binding 要求从数据来源对象读取 ResultText 属性。ResultText 是本项目自定义属性名，OneWay 表示从数据到文字控件。写成 `Text="ResultText"` 只显示这十个字母，不会读属性。

数据对象先这样创建与连接，下面是采用完整 new 的等价写法：

```csharp
private readonly ReadingDisplayState _state = new ReadingDisplayState();
```

ReadingDisplayState 是教练定义的类，保存结果/状态两段文字。new 创建对象，_state 保存它的引用。窗口构造中 `DataContext = _state;` 设置窗口的 WPF 数据来源属性；内部 TextBlock 没另设来源，就沿用这个对象。绑定首次读取 _state.ResultText，显示“尚未读取”。这条来源关系展开见 Q2。

之后按钮执行 `_state.ResultText = "读取完成";`，必须让绑定知道该属性变化。原 setter 的两个动作是：

```csharp
_resultText = value;
OnPropertyChanged("ResultText");
```

第一行保存此次赋值的文字，value 来自等号右侧；第二行调用项目自定义通知方法，报告 ResultText 变了。ReadingDisplayState 实现 .NET 的 INotifyPropertyChanged 接口，提供 PropertyChanged 事件；声明事件本身不会发通知。WPF 监听到通知后，再读公开 ResultText，更新 TextBlock.Text。

实际链是“按钮赋值 → setter 保存并发通知 → WPF 重读同一个对象的属性 → 结果文字更新”。OneWay 仍可持续更新，不表示只读初始一次。

原工具若出现 `nameof(ResultText)`，它得到属性名字符串 "ResultText"，不读取属性值。通知中的 ?.Invoke 是“有接收方法才调用”的简写，完整解释见 [事件 Q6](./csharp-events.md)。这些工具由骨架提供，不把每个语法都捆成首次 Binding 的前提。

不要另建一个数据对象后修改它，界面仍监听原 _state。也不要给结果控件 Text 直接赋值来刷新，普通赋值可能取代原绑定。当前 TASK-010 只绑定结果/状态输出，输入框仍由按钮代码直接取 Text。

## Q2：ReadingDisplayState 是什么？DataContext = _state 究竟做了什么？

- 日期：2026-10-09
- 状态：待复习
- 来源：学员反馈前述讲解仍默认了解 DataContext 和骨架中的 ReadingDisplayState。应先说明自定义类型、实例和 WPF 属性各自的身份，再解释它们的连接。

先回答这个陌生名字：ReadingDisplayState 是教练新建的普通 C# 类，保存窗口的 ResultText 和 StatusText；不是 WPF 自带 API，也不是某个输入框。它的对象只保存显示文字与通知代码。

```csharp
private readonly ReadingDisplayState _state = new ReadingDisplayState();
```

ReadingDisplayState 是类名，new 创建一个该类的具体对象，字段 _state 保存引用。private 让字段供当前窗口类内部使用；readonly 限定初始化后不把字段换成另一个引用，仍可改这个对象的属性。原 `new()` 省略了左边已经确定的类型名，含义相同。

窗口构造里执行：

```csharp
DataContext = _state;
```

DataContext 是 WPF 窗口已有的属性，这里设置的是“这个窗口的绑定默认从哪个对象取值”。赋的是 _state 指向的对象，不是 ReadingDisplayState 类名、文件名或待显示文字；没有复制它，也没有创建第二份。

只有再配上文字控件的 `Text="{Binding ResultText}"`，该控件才按属性名从这个对象读取“尚未读取”。只设置 DataContext、但 Text 仍写死为“结果待显示”，界面就继续显示固定文字。

取值可以具体读成：“这个 TextBlock 沿用窗口 DataContext → 找到原 _state → 读它的 ResultText → 设置自己 Text”。后续属性变化如何刷新是 Q1 的下一层关系，先把来源讲清即可。

## Q3：Rider 的 Binding 后面为什么有 (???)？预览没有显示属性值怎么办？

- 日期：2026-10-09
- 状态：待复习
- 来源：学员截图中 Binding 后显示灰色 (???) 与 Path= 提示，下方 XAML 预览的结果和状态区域没有文字。

你看到的灰色 `(???)`、Path= 与下方空白预览，先分成三个地方看：磁盘 XAML、编辑器附加提示、真正运行的窗口。

当时磁盘保存的是 `Text="{Binding ResultText, Mode=OneWay}"`，并没有 (???)。Rider 会在 XAML 周围显示数据来源类型等内嵌提示；结合当时位置与源码，(???) 被解释为编辑器尚不能确定 DataContext 类型。这是对那次截图的判断，不是 WPF 异常，不能把灰色提示复制进代码。

下方设计预览也没呈现构造函数里设置的运行时数据，不能据此判定实际取值失败。不同设计器行为有差别，不把这个现象说成“设计器永远不执行 C#”。官方设置说明见文末。

当时实际打开 Task010Window 检查，结果控件已经取到“尚未读取”；真正的问题在第二块文字。状态控件也写了 ResultText，便显示相同文字。对应关系应是：

```xml
<TextBlock Text="{Binding ResultText, Mode=OneWay}" />
<TextBlock Text="{Binding StatusText, Mode=OneWay}" />
```

第一块取结果，第二块取状态。修复一个控件路径后，应观察实际原任务窗口的两段文字，不凭编译成功或设计预览作结论。这个路径问题后来已修复，当前不要照旧记录再宣称错误仍存在。

## Q4：PropertyChangedEventArgs 这个类从哪里来？

- 日期：2026-10-09
- 状态：待复习
- 来源：学员询问属性变化通知代码中的类型来源。已核对现有源码和本机 .NET 10.0.12 参考文档 System.ObjectModel.xml。

当 setter 调用 `OnPropertyChanged("ResultText")`，它要告诉 WPF“重新读取 ResultText”，而不是把新温度放进通知。携带这个属性名的参数对象来自 .NET：

```csharp
PropertyChangedEventArgs args = new PropertyChangedEventArgs("ResultText");
```

PropertyChangedEventArgs 是 System.ComponentModel 中的 .NET 类，不是教练新建类，不需要安装额外包。`using System.ComponentModel;` 让代码使用短类名，类似 Java import。new 创建一个参数对象，args 保存引用；它的 args.PropertyName 得到 "ResultText"。

自定义 OnPropertyChanged 接到 string propertyName 后，用同样方式 `new PropertyChangedEventArgs(propertyName)` 创建参数，并随 PropertyChanged 事件发出。WPF 接到名称，再从原数据对象读取 ResultText 的当前值。属性名与实际结果文字用途不同，通知不替你保存 _resultText。

ReadingDisplayState、OnPropertyChanged 是本项目自定义类/方法；PropertyChangedEventArgs、PropertyChangedEventHandler 和 INotifyPropertyChanged 则来自 .NET。这里先明确参数来自哪里、装什么，不要求重写整套通知工具或强制把合法字符串换成 nameof。

## 历史证据与掌握范围

Q1 保持已掌握，Q2/Q3/Q4 保持待复习。2026-10-09 TASK-010 学员自主连接同一 DataContext、两个 OneWay Binding、调用原通知工具并修改数据属性，五种输入场景通过，局部应用计入 Q1；get/set、类结构、工具由骨架提供。Q3 初始检查中状态也绑定 ResultText，后来已自主修正为 StatusText；编辑器提示与设计预览机制不因此升级。旧临时检查路径、当时控件值及后续验收保存在原稿，本轮不重新运行。

相关任务卡与验收来源：[TASK-002](../tasks/TASK-002.md)、[TASK-009](../tasks/TASK-009.md)、[TASK-010](../tasks/TASK-010.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-binding.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [XAML 内嵌提示](https://www.jetbrains.com/help/rider/Settings_Inlay_Hints_XAML.html)
- [Rider 官方说明](https://www.jetbrains.com/help/rider/Languages_XAML.html)
