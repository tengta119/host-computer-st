# TASK-015 知识笔记：Style、Setter、资源引用与共同按钮外观

> 整理日期：2026-10-13。任务：用 Style 统一读取与清空按钮的外观。状态：✅ 已通关（2026-10-12）。
>
> 根据当前源码、任务卡、学习记录、样式问答及本地历史验收材料详细提取。“当前实现”来自现有作品；“说明示例”“排查反例”用于理解，不表示已写入项目或已经运行。本轮只整理文档、核对历史材料，不修改源码、入口或任务状态。

## 来源与阅读顺序

| 来源 | 用途 |
| :--- | :--- |
| [TASK-015 任务卡](../tasks/TASK-015.md) | 目标、TODO、加载时机纠正、DoD 与历史审查 |
| [Task015Window.xaml](../../WpfBae/Exercises/TASK-015-button-style/Task015Window.xaml) | Window.Resources、两个 Setter、读取按钮资源引用 |
| [Task015Window.xaml.cs](../../WpfBae/Exercises/TASK-015-button-style/Task015Window.xaml.cs) | 创建原 ViewModel 类型的新实例、初始状态、DataContext |
| [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-015-button-style/ReadingHistoryControl.xaml) | 历史列表、清空按钮 DynamicResource 与原绑定 |
| [ReadingHistoryControl.xaml.cs](../../WpfBae/Exercises/TASK-015-button-style/ReadingHistoryControl.xaml.cs) | 独立控件初始化和父级数据连接 |
| [原 ReadingViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs) | 复用的数据、集合、命令和通知连接 |
| [原读取命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs) | 输入校验、读取追加、提示 |
| [原清空命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs) | Count 条件、Clear、条件变化通知 |
| [样式问答 Q1/Q2](../questions/wpf-styles.md) | 已验证应用与待复习的资源加载机制 |
| [学习进度](../learning.md) | 掌握范围、指导纠正及当前入口 |
| [最终运行结果](../tasks/evidence/TASK-015/results.json) | 9 个业务场景和 Padding 对比数据 |
| [原程序启动](../tasks/evidence/TASK-015/original-startup.json) | 当时原窗口打开、标题与关闭结果 |
| [构建/源码/程序集核对](../tasks/evidence/TASK-015/verification.json) | 历史构建及审查工具纠正 |
| [最终审查程序](../tasks/evidence/TASK-015/Review.cs) | 同对象、本地值、绑定与实际布局检查 |
| [资源查找诊断](../tasks/evidence/TASK-015/resource-lookup-diagnostic/results.json) | 修复前加载异常和内存诊断边界 |
| [独立 Padding 副本](../tasks/evidence/TASK-015/padding-variant/Task015Window.xaml) | 审查使用的 32,14 设置 |
| [TASK-014 笔记](./TASK-014-wpf-usercontrol.md) | UserControl、命名空间、DataContext 继承 |
| [TASK-013 笔记](./TASK-013-wpf-viewmodel.md) | 原输入和读取业务连接 |
| [App.xaml](../../WpfBae/App.xaml) | 当前真实启动入口 |

先理解 Padding 与对齐的重复，再看资源对象、键、Setter 和按钮连接，最后读查找时机与优先级。既有 Java 类/对象经验可以帮助区分对象身份，但 Style、XAML 资源与 WPF 属性系统需要分别说明。

## 1. 解决的问题：两份重复外观变成一份设置

TASK-014 中读取和清空按钮各自写 Padding 与 HorizontalAlignment。共同留白要调整时，原来需要改两处。

TASK-015 将这些属性放进同一份 Style，两个按钮引用它：

```text
共同 Style
  ├─ Padding：24,10
  └─ HorizontalAlignment：Left
       ↓                    ↓
    读取按钮              清空按钮
    ReadCommand           ClearCommand
```

统一的是共同属性来源，两个按钮仍是独立对象，显示不同文字、调用不同命令，清空可用条件仍取决于历史数量。

当前业务仍为固定 26.5 °C 的同步模拟读取。本任务没有新增设备通信、第三方主题库或异步采集。

## 2. 文件分工与原作品复用

专属目录：`WpfBae/Exercises/TASK-015-button-style/`。

| 文件 | 当前职责 |
| :--- | :--- |
| Task015Window.xaml | 窗口布局、样式资源、读取按钮引用、历史控件嵌入 |
| Task015Window.xaml.cs | 创建一份 ReadingViewModel、设置状态和 DataContext |
| ReadingHistoryControl.xaml | 新任务历史区域，清空按钮引用窗口样式 |
| ReadingHistoryControl.xaml.cs | UserControl 的类定义与初始化 |

TASK-014 四个原文件保留。TASK-015 的 ReadingHistoryControl 是新任务命名空间 `WpfBae.Exercises.Task015` 中的类，避免与 Task014 的同名类冲突。

ViewModel、两个命令仍直接复用 TASK-013 源码；Reading 继续使用 TASK-010 的类型。窗口复用类定义，但 `new ReadingViewModel()` 创建 TASK-015 自己的实例，不是所有历史窗口共用一份数据。

## 3. 所有关键名字先分清来源

| 名称 | 身份和来源 | 当前作用 |
| :--- | :--- | :--- |
| Style | WPF 的类型，System.Windows | 保存一组属性设置 |
| Setter | WPF 的类型，System.Windows | 保存一个属性/值设置项 |
| Window.Resources | WPF 属性，资源字典 | 保存可按键查找的对象 |
| Button.Style | WPF 属性 | 指定这个按钮使用哪份 Style |
| TargetType | Style 的属性 | 指定样式适用的控件类型 |
| Property、Value | Setter 的属性 | 指定设置哪一项、设成什么值 |
| x:Key | XAML 指令 | 给资源分配键 |
| ActionButtonStyle | 教练选择的自定义资源键 | 标识当前共同 Style |
| StaticResource、DynamicResource | WPF 的资源取值写法 | 按键取得资源，查找时机不同 |
| Padding、HorizontalAlignment、Margin | WPF 控件/元素属性 | 内留白、控件位置对齐、外部间距 |
| ReadCommand、ClearCommand | 自定义 ViewModel 属性 | 提供业务命令对象 |

ActionButtonStyle 不是 WPF 固定 API、C# 类名或命令名。StaticResource 也不是 C# 的 static 关键字；它是 XAML 中的资源引用机制。

## 4. Padding 的真实作用与 Thickness 数值

Padding 是控件内容与控件边界之间的内留白。对当前按钮，可以看作四个字周围与按钮边缘之间的空间。

| 写法 | 左、上、右、下的含义 |
| :--- | :--- |
| 12,6 | 12、6、12、6 |
| 24,10 | 24、10、24、10 |
| 32,14 | 32、14、32、14 |

两个数分别指定左右与上下。底层值类型是 WPF 的 Thickness，XAML 把字符串形式转换为该类型；不需要学员自己定义一个 Padding 类。

当前 Setter 写 `Value="24, 10"`，逗号后的空格不改变这组数的含义，历史运行记录实际为 `24,10,24,10`。

增加 Padding 会使当前自动大小按钮需要更多空间，不会改文字内容、增大温度数值或改变命令条件。具体显示仍受控件模板与布局约束影响，不能从这次结果推定所有控件只要设置 Padding 都同样显示。来源：[Microsoft Control.Padding 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.control.padding?view=windowsdesktop-10.0)。

## 5. HorizontalAlignment 与 Margin 各负责什么

HorizontalAlignment=Left 使按钮控件在父布局提供的横向空间内靠左，不是设置按钮文字的文本对齐，也不是让命令在“左侧线程”运行。依据：[HorizontalAlignment 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.horizontalalignment?view=windowsdesktop-10.0)。

Margin 是控件外部间距，Padding 是控件内部留白。

```text
邻近元素 —— 外部 Margin —— [按钮边缘 | Padding | 文字 | Padding | 按钮边缘]
```

当前共同 Style 只提取 Padding、HorizontalAlignment，没有提取 Margin。历史区整体上方的 Margin 和状态提示上方的 Margin 仍由各自位置决定。

当前 ClearButton 没写 Margin，有效值为 0；不能照 TASK-014 的旧参数把本任务清空按钮说成 Margin="0,12,0,0"。历史审查确认它位于列表下方、没有重叠或裁切，不追加固定间距验收要求。

## 6. Window.Resources 是保存对象的资源字典

当前资源放在窗口 XAML 中：

```xml
<Window.Resources>
    <Style x:Key="ActionButtonStyle" TargetType="Button">
        <!-- 两个 Setter -->
    </Style>
</Window.Resources>
```

Window.Resources 是给窗口 Resources 属性赋值的 XAML 属性元素写法，不是一个显示文字的控件。里面定义可按键查找的资源对象。

可以用字典的经验理解：键 ActionButtonStyle 对应一个 Style 对象。窗口加载时建立这份资源，按钮之后引用它。

资源对象不需要出现在界面树上才有用；只有应用到目标属性后，相应设置才影响按钮。定义在 Window.Resources 也不等于全应用自动可见，作用范围与查找上下文有关。依据：[WPF 资源概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/systems/xaml-resources-overview)。

## 7. x:Key、x:Name、属性路径与类名的区别

| 当前名字 | 身份 | 用在哪条连接上 |
| :--- | :--- | :--- |
| ActionButtonStyle | 资源键 x:Key | 资源字典查找 Style |
| ReadButton、ClearButton | 控件 x:Name | 标识具体界面对象 |
| ReadCommand、ClearCommand | Binding.Path | 在 DataContext 上读公开属性 |
| ReadingHistoryControl | 自定义类型名 | 创建历史控件实例 |
| history | 自定义 XAML 前缀 | 映射控件的 C# 命名空间 |

资源引用中的键要与 x:Key 一致，字符串键区分大小写。改键名也要改引用；改控件 x:Name 不会自动改资源键或命令属性。

当前不需要写 `Binding ActionButtonStyle`，因为 Style 存在资源字典中，未作为 ViewModel 属性公开。样式名字看起来像一个对象名字，不代表要在 C# 中定义同名类。

## 8. TargetType 与显式样式

当前定义同时有：

```xml
x:Key="ActionButtonStyle" TargetType="Button"
```

TargetType 指定这份 Style 用于 Button，使内部 Setter 能按这个类型的属性理解 Padding 和 HorizontalAlignment。它不是查找名为 Button 的具体实例。

因为资源使用了自定义键，当前两个按钮需要显式写 Style 资源引用。把 Style 定义好不会自动让本窗口所有 Button 使用它。

WPF 也支持按类型自动应用的隐式样式，但本任务只练当前显式方式，没有要求新增隐式样式、BasedOn 或多层继承。样式用途依据：[WPF 样式概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/styles-templates-overview)。

## 9. Setter 的 Property 和 Value 分别是谁

当前两项：

```xml
<Setter Property="Padding" Value="24, 10"></Setter>
<Setter Property="HorizontalAlignment" Value="Left"></Setter>
```

第一项表达“使用此 Style 的按钮，其 Padding 由这里提供 24,10”；第二项表达“其 HorizontalAlignment 由这里提供 Left”。

Property 指明 WPF 属性，Value 提供匹配类型的值。Left 是对齐枚举值，24,10 会转换为 Thickness。不是通过反射去调用自定义方法，也不是声明 ViewModel 上的两个同名字段。

Setter 是 Style 中的设置项。它与 C# 属性访问器 `set { ... }` 不同：前者描述 WPF 属性设置，后者是在 C# 类中接收属性赋值的代码。

没有子内容的 Setter 可以写成自闭合形式，等价说明如下，本轮不改作品：

```xml
<Setter Property="Padding" Value="24,10" />
```

来源：[Microsoft Setter 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.setter?view=windowsdesktop-10.0)。

## 10. 当前完整的共同样式

去掉注释后的当前定义：

```xml
<Window.Resources>
    <Style x:Key="ActionButtonStyle" TargetType="Button">
        <Setter Property="Padding" Value="24, 10"></Setter>
        <Setter Property="HorizontalAlignment" Value="Left"></Setter>
    </Style>
</Window.Resources>
```

只有一个 Padding Setter 和一个对齐 Setter，没有在历史控件资源里复制第二份样式。

当前 Style 没设置 Content、Command、IsEnabled，也没有 ControlTemplate、Trigger 或颜色配置。它通过现有按钮和默认外观系统应用共同属性；“样式”并不意味着这里已经设计了按钮全部内部结构。

## 11. 读取按钮的 StaticResource 引用逐侧解释

当前读取按钮：

```xml
<Button x:Name="ReadButton"
        Style="{StaticResource ActionButtonStyle}"
        Content="读取温度"
        Command="{Binding ReadCommand, Mode=OneWay}" />
```

左侧 Style 是 Button 的属性；右侧花括号表示让 XAML 特殊取值机制计算属性值。StaticResource 以 ActionButtonStyle 为资源键，查找已有对象并交给 Button.Style。

```text
窗口资源字典中的 ActionButtonStyle → Style 对象 S
读取按钮 Style 属性 → S
```

读取按钮在窗口 XAML 的加载上下文中，窗口资源已经定义，当前能找到 S。StaticResource 是加载时取得引用，不是 C# 的 static，也不是每次点击时重新创建样式。依据：[StaticResource 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/staticresource-markup-extension)。

## 12. 内部清空按钮为什么当前写 DynamicResource

当前清空按钮：

```xml
<Button x:Name="ClearButton"
        Content="清空历史"
        Command="{Binding ClearCommand, Mode=OneWay}"
        Style="{DynamicResource ActionButtonStyle}" />
```

DynamicResource 仍使用同一个键，不新建另一份 Style。它保留资源查找表达式，在需要取得值时查找，并能随相关资源或父级上下文变化重新求值。

本任务控件先独立初始化，再接入窗口；这使内部按钮可以在父子关系建立后找到窗口资源 S。读取按钮继续 StaticResource，无需全部改成 DynamicResource。

DynamicResource 不负责等待设备数据、调用命令或执行属性通知。它解决的是当前资源引用的取值时机。依据：[DynamicResource 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/dynamicresource-markup-extension)。

## 13. 资源作用域需要同时考虑“放在哪里”和“什么时候找”

已接入窗口后，内部按钮有这条当前父级路径：

```text
ClearButton
  → 内部 StackPanel
  → ReadingHistoryControl
  → 外层 StackPanel
  → Task015Window.Resources 中的 ActionButtonStyle
```

这只是当前找到资源的相关路径，不是完整列出 WPF 主题、系统及模板等所有查找来源。实际更近的资源定义和加载上下文也会影响查找。

命名空间、文件夹和资源作用域不同：两个文件在同一目录或同一 C# namespace，不保证其中一个的 Window.Resources 在另一个构造阶段可访问。

如果复制同键 Style 到控件局部资源，内部可能先取得那份对象，外观暂时一样却不再满足本任务“同一个窗口 Style”的目标。当前不复制资源，而是保持窗口一份样式和内部延后引用。资源范围依据：[WPF 资源查找说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/systems/xaml-resources-overview)。

## 14. 历史 StaticResource 异常的真实加载顺序

这里描述任务过程中的已修复问题，当前代码已经使用 DynamicResource。

```text
创建 Task015Window
 → InitializeComponent 加载窗口 XAML
 → 建立窗口 Resources；读取按钮可以找到 Style
 → 遇到 ReadingHistoryControl 标签，创建控件
 → 控件构造函数调用自己的 InitializeComponent
 → 独立加载控件 XAML，此时尚未接入父窗口
 → 若内部 ClearButton 此处用 StaticResource
 → 找不到仅定义在窗口中的 ActionButtonStyle
 → 抛出 XamlParseException，窗口初始化失败
```

不能因为控件最终要位于窗口内部，就推定它在自身构造中已经有父窗口关系。这是“最终结构”和“创建时刻”的区别。

当前控件构造函数只初始化界面，不创建 ViewModel 或复制样式；DynamicResource 连接后取得原资源。详细历史解释见 [样式问答 Q2](../questions/wpf-styles.md)。

## 15. 诊断材料能证明什么，不能证明什么

[资源查找诊断 results.json](../tasks/evidence/TASK-015/resource-lookup-diagnostic/results.json)记录：原编译窗口重现找不到 ActionButtonStyle 的异常；控件接入前无父级；内存控件副本改用 DynamicResource 后接入诊断窗口，实际显示并取得同一窗口 Style、Padding 24,10、Left。

这是当时的机制诊断，没有代改学员源文件，也不是完整修复后的原窗口九个业务场景验收。最终修复后原窗口通过的依据是后续最终 results.json。

XamlParseException 表示加载 XAML 失败，内层异常指出资源键未找到。键名核对正确、构建通过，也不能排除加载时的上下文问题。

原 TODO 3 和起步讲解曾漏掉独立 UserControl 的初始化时机。这是教学指导需要纠正，已记入问答和学习进度，不记为学员典型错误。

## 16. StaticResource 与 DynamicResource 的必要区别

| 项目 | StaticResource | DynamicResource |
| :--- | :--- | :--- |
| 当前用途 | 窗口读取按钮 | 独立控件内部清空按钮 |
| 取值机制 | 加载时查找已可访问的资源引用 | 保留延后资源表达式，按运行上下文取值 |
| 取得的目标 | 当前窗口 Style S | 同一个窗口 Style S |
| 资源/上下文后续变化 | 不因字典键替换自动重新查找 | 可因相关变化重新查找 |
| 编辑磁盘 XAML 是否自动生效 | 不会 | 不会 |

StaticResource 取得的是对象引用，不是把所有外观值复制后“冻结成截图”。DynamicResource 也不是每次用户操作都无条件 new 一个对象。

当前选择是由资源位置和 UserControl 加载路径决定，不推出所有内层控件都只能 DynamicResource。若资源本来在独立控件加载时可访问，StaticResource 可以正常使用；若未来选择应用级资源，会是另一种设计，本轮不改资源作用域。

## 17. 直接属性为什么会覆盖 Style Setter

说明反例，不是当前残留代码：

```xml
<Button Style="{StaticResource ActionButtonStyle}"
        Padding="12,6"
        Content="读取温度" />
```

虽然样式 Padding 是 24,10，按钮自己直接设置的 Padding 是本地值，优先级高于 Style Setter，所以最终仍为 12,6。

```text
Button.Padding 的本地值 12,6
           优先于
Style 中的 Padding Setter 24,10
```

当前两个按钮都已移除直接 Padding/HorizontalAlignment，才能由共同 Setter 提供这两项值。没有本地值不代表有效值为零，框架会继续从样式等来源求值。

这里是针对本任务属性来源的比较，不是“直接设置永远高于所有机制”；WPF 还有强制值、动画等更高优先级，本任务不要求背完整列表。依据：[WPF 属性值优先级](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/properties/dependency-property-value-precedence)。

## 18. Button.Style 是本地引用，Padding 却来自样式

需要按“哪一个属性”讨论本地值。当前按钮显式写了 Style，因此 Style 属性上有资源引用；但没有直接写 Padding 和 HorizontalAlignment，所以这两个属性由 Setter 提供。

| 属性 | 当前来源 |
| :--- | :--- |
| Style | 按钮自身的 StaticResource/DynamicResource 引用 |
| Padding | 被引用 Style 中的 Setter |
| HorizontalAlignment | 被引用 Style 中的 Setter |
| Content | 按钮自己的文字 |
| Command | 按钮自己的 Binding |

不能把“按钮写了一个 Style 属性”理解成“它所有属性都是本地值”。历史审查用 ReadLocalValue 检查 Padding/HorizontalAlignment 为 UnsetValue，同时检查有效值与 Setter 相符。

UnsetValue 是 WPF 用于表示这里未设置本地值的标记，不是实际 Padding 值，也不是样式没生效。它属于审查工具的检查机制，不要求为了学习而把检查 API 添加到项目。

## 19. 提取共同外观时哪些属性保留

本任务提取 Padding 和 HorizontalAlignment 后，保留了各按钮的 x:Name、Content、Command 和需要的位置设置。

```text
读取按钮：名称 ReadButton、文字 读取温度、命令 ReadCommand
清空按钮：名称 ClearButton、文字 清空历史、命令 ClearCommand
共同部分：Style 中的 Padding、HorizontalAlignment
```

不要为了“抽样式”删除命令绑定或把两按钮都接到同一业务命令；也不要把 Content 放入共同 Setter 导致两按钮显示一样的文字。

当前位置间距与共同内留白属于不同职责。当前清空按钮没有显式 Margin，这是源码实际结果；笔记不替它补回旧参数或改变验收范围。

## 20. 资源引用与 Binding 是两条不同的连接

```text
外观来源：Window.Resources[ActionButtonStyle] → Style S
          ├─ ReadButton.Style
          └─ ClearButton.Style

业务来源：Window.DataContext → ViewModel V
          ├─ DeviceId：输入 TwoWay
          ├─ ReadCommand：读取按钮 Command
          ├─ Readings：内部列表 ItemsSource
          ├─ ClearCommand：清空按钮 Command
          └─ StatusText：窗口提示
```

Style S 不保存设备 ID 或历史。ViewModel V 没有保存 ActionButtonStyle；它保存输入、记录、提示并提供两个命令。

DataContext 沿当前父级传入内部控件，帮助 Binding 找属性；资源查找帮助 Style 引用找外观对象。两者都与界面层次有关，但不是同一个查找机制，修改 DataContext 不能修复缺失资源键。

## 21. 窗口初始化与原 ViewModel 保持

当前窗口核心代码：

```csharp
private ReadingViewModel _viewModel = new ReadingViewModel();

public Task015Window()
{
    InitializeComponent();
    _viewModel.StatusText = "TASK-015：先完成 TODO 1，观察读取按钮的内边距。";
    DataContext = _viewModel;
}
```

InitializeComponent 完成窗口与控件初始化，XAML 资源在这一过程中连接。随后保存初始提示，再设置数据上下文。

V 在构造时创建两个命令并把自己传给它们；控件后台只调用 InitializeComponent，没有新建 V。列表和命令继续使用原集合与原命令实例。

Style 改变不会自动导致原集合替换。输入写回、Trim、空白 return、new Reading、Add 和成功提示仍在 TASK-013 的原实现中。详见 [TASK-013 笔记](./TASK-013-wpf-viewmodel.md)。

## 22. 外观统一后禁用/恢复为什么仍正常

读取命令 CanExecute 固定 true；清空命令 CanExecute 为 `Readings.Count > 0`。共同 Style 没设置 IsEnabled，原业务条件继续决定清空可用性。

```text
有效读取 Add
 → CollectionChanged
 → ViewModel 集合处理方法
 → 清空命令 NotifyCanExecuteChanged
 → WPF 重查 CanExecute
 → 清空启用

实际清空 Clear
 → 同样的通知链
 → Count=0
 → 清空禁用
```

PropertyChanged 继续更新状态文字；CollectionChanged 更新列表；CanExecuteChanged 请求条件重查。Style 只提供当前共同属性，不代替这三种通知，也不触发按钮点击。

当前清空变灰使用按钮已有外观机制，本任务没有自己写禁用 Trigger、Opacity 或 ControlTemplate。这些不能因为界面已经显示禁用效果就算已自主实现。

## 23. 两个按钮共用 Style，仍然是两个对象

```text
ReadButton 实例 A  ── Style → S ← Style ── ClearButton 实例 B
        │                                      │
   ReadCommand                             ClearCommand
```

共用的是样式引用，不是把按钮 A 和 B 合成同一个实例。各自的 Content、Command、状态及位置仍独立。

同一窗口中引用同键当前取得同一 Style；不同窗口实例各自的 Window.Resources 通常各有自己的资源实例。历史对比的原窗口和副本窗口也各有自己的共同 Style；sameStyleBefore/After 表示各自窗口内部的两按钮相同，不表示两个窗口跨窗口引用同一个对象。

共同 Padding 与对齐也不保证所有按钮宽高都相等。内容长度、字体、其他约束都可能影响尺寸；当前读取/清空按钮尺寸相同是本次实际观测结果。

## 24. 修改 Setter 的方式与“动态”边界

任务卡的观察方式是：关闭旧窗口，修改一处 XAML Padding Setter，再运行新版本。当前原作品保留 24,10；历史审查用独立副本验证 32,14。

```text
改磁盘 XAML → 构建/加载新界面 → 创建新的资源与控件 → 观察结果
```

DynamicResource 不监听磁盘文件，不保证编辑器保存后正在运行的窗口立即更新。实时热重载是开发工具的另外机制，本任务不依赖它。

也不要把“共用 Style”理解为运行中可以任意修改已应用 Style 的 Setters。WPF 使用中的 Style 会进入不可修改的状态，IsSealed 用来表示这一点。当前通过重跑或独立新副本观察，未实现运行时修改/替换样式。依据：[Microsoft Style.IsSealed 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.style.issealed?view=windowsdesktop-10.0)。

## 25. 历史布局数据：一处变化影响两个按钮

已读取 [results.json 的 styleComparison](../tasks/evidence/TASK-015/results.json)：

| 窗口版本 | 共同 Padding | 读取按钮宽×高 | 清空按钮宽×高 |
| :--- | :--- | :--- | :--- |
| 原编译 Task015Window | 24,10,24,10 | 98 × 37.24 | 98 × 37.24 |
| 独立编译副本 | 32,14,32,14 | 114 × 45.24 | 114 × 45.24 |

两按钮各增宽 16、增高 8。这与左右各增加 8、上下各增加 4 的 Padding 变化一致，是本次窗口布局的实测关系，不是任意布局的通用尺寸公式。

共同对齐均为 Left，前后各按钮 Margin 均为 0，文字保持原样；初始读取可用、空历史清空禁用。原窗口与副本均在各自窗口内共用同一份 Style。

副本源码只改变共同 Padding，并更换命名空间以避免与原类冲突。该变化由审查工具准备，不计学员自主实现多主题、运行时切换或独立副本工程能力。

## 26. 审查怎样证明样式真的来自同一对象

[Review.cs](../tasks/evidence/TASK-015/Review.cs)实际显示原窗口，取得 Window.Resources 中的 Style，检查：

1. 两按钮 Style 与资源对象 ReferenceEquals，相同的不只是数值。
2. TargetType 为 Button，只有一个共同 Padding Setter、一个共同对齐 Setter。
3. 两按钮有效 Padding/HorizontalAlignment 正确，本地对应属性均为 UnsetValue。
4. Content、Command、同 ViewModel、原集合与各 Binding 保持有效。
5. 显示独立副本后，两按钮实际尺寸增大，位置和禁用状态正常。

如果只看两个按钮都是 Padding=24,10，无法排除各自硬编码或复制两份 Style；引用相同和没有本地覆盖的检查补足了这个证据。

ReadLocalValue、ReferenceEquals 和自动化提供程序属于审查代码使用的工具，不作为本任务要求学员独立实现的 API。本轮没有运行该审查工程。

## 27. 九个历史业务场景仍然通过

以下直接来自最终 results.json，均 passed=true，sameViewModel=true、sameCollection=true：

| 场景 | 条数 | 最终提示 | 清空可用 | 累计集合变化 |
| :--- | :--- | :--- | :--- | :--- |
| 初始 | 0 | TASK-015 的 TODO 1 提示 | false | 0 |
| 首次 TEMP-001 | 1 | 读取成功 | true | 1 |
| 同 ID 再读 | 2 | 读取成功 | true | 2 |
| 改 TEMP-002 | 3 | 读取成功 | true | 3 |
| 空字符串 | 3 | 该 Id 不合法 | true | 3 |
| 纯空格 | 3 | 该 Id 不合法 | true | 3 |
| 控件内清空 | 0 | 已清空 | false | 4 |
| 两侧空格 TEMP-003 恢复 | 1 | 读取成功 | true | 5 |
| 再次清空 | 0 | 已清空 | false | 6 |

集合事件数字是整个审查的累计计数，不是每个场景各发布表中次数。空白保持累计 3，说明本次没有集合变更。

历史输入通过 TextBox.SetCurrentValue 保留绑定，按钮通过 ButtonAutomationPeer/IInvokeProvider.Invoke 走实际 WPF 命令路径；不是直接调用 Execute 代替绑定验证，也不是学员手工点击记录。

## 28. 截图、构建和历史程序集核对

本轮已查看：

- [24,10 原窗口布局](../tasks/evidence/TASK-015/padding-24-10.png)：两个按钮共同留白，空历史清空变灰。
- [32,14 副本布局](../tasks/evidence/TASK-015/padding-32-14.png)：两按钮明显增大，文字与历史区域可见。
- [三条历史](../tasks/evidence/TASK-015/history-three-readings.png)：TEMP-001 两条、TEMP-002 一条，清空启用，提示读取成功。

截图是当时已显示窗口的内容渲染，不包含完整桌面或标题栏。图片帮助观察显示，引用相同、绑定和本地属性来源仍要结合 JSON 与审查代码。

[original-startup.json](../tasks/evidence/TASK-015/original-startup.json)记录原程序打开、标题为“TASK-015：用 Style 统一按钮外观”、正常关闭退出 0。[verification.json](../tasks/evidence/TASK-015/verification.json)记录 2026-10-12 构建 0 警告/错误、审查最终退出 0、原程序集与审查所用程序集哈希一致。

首次审查工具曾混入旧同名程序集，记录已保留在 [旧程序集审查输出](../tasks/evidence/TASK-015/results-harness-stale-assembly.json)。随后显式同步当前原程序集并重新审查，最终结果通过。这是审查工具纠正，不计学员失败，不能拿首次旧程序集结果覆盖最终任务状态。

本轮将历史 sourceHashesAfter 的 11 项对照当前文件：除 App.xaml 外，其余 10 项一致；当前 App.xaml 已推进到 TASK-017。该列表含四个 TASK-015 文件、五个 TASK-013 文件及 ReadingDisplayState，不包含全部工作区源码，不能说所有文件都与历史相同。

## 29. 当前注释与实现不一致时怎样阅读

当前 ReadingHistoryControl.xaml 的旧 TODO 3 注释仍写 StaticResource；实际按钮属性已经是 DynamicResource。以实际实现及任务卡/问答的指导更正为准。

Task015Window.xaml 的注释仍提到“空 Style”和待完成 Setter，但 Setter 和读取按钮引用已存在。TODO 注释保留是历史，不自动证明功能未完成。

TODO 4 提到 32,14，当前原 Setter 仍是 24,10；验收的等效变化证据来自独立副本。不能把这条注释当成原源码已改为 32,14，也不能把副本变化描述成运行时改原窗口。

原状态仍保留 TODO 1 提示，这是当前初始文案，不影响历史必需验收。笔记如实记录，本轮不清理注释或改写提示。

## 30. 常见现象与排查顺序

| 现象 | 优先核对 | 关键区别 |
| :--- | :--- | :--- |
| 找不到 ActionButtonStyle | 键名/大小写、资源位置、独立控件加载时机 | 定义存在不等于此刻可访问 |
| 构建成功，启动 XAML 异常 | 内层异常与资源引用上下文 | 编译通过不保证运行加载通过 |
| 已引用 Style，留白仍是旧值 | 按钮是否残留 Padding 本地值 | Style 引用与 Setter 生效不同 |
| 只有一个按钮变化 | 另一按钮引用、局部覆盖、是否复制 Style | 数值相同不保证对象同一 |
| 按钮撑满整行 | HorizontalAlignment 来源是否 Left | 控件对齐不等于文字对齐 |
| 用 Binding 找 ActionButtonStyle 失败 | 属性是否在 ViewModel 中；当前应走资源键 | 资源对象与业务来源不同 |
| 改 XAML 后旧窗口不变 | 是否仍运行旧进程、是否加载新构建 | DynamicResource 不监控磁盘 |
| 样式正常但清空不能恢复 | 原集合、CanExecute 与通知连接 | 外观正确不代替业务验证 |
| 运行显示 TASK-017 | App.xaml 当前入口 | 当前入口已推进 |

动态引用未找到资源时不一定与 StaticResource 同样立即抛异常，可能暂时没有取得预期样式值；所以“窗口能打开”也不能单独证明 Style 成功应用，应核对有效值和对象引用。

这些是排查提示，不将尚未发生的问题记入学员错题。TASK-015 没有新增已观察的典型错误，资源异常的教学遗漏与审查旧程序集问题均有明确归属。

## 31. 当前运行入口与一个小的复习观察

当前 App.xaml 为：

```xml
StartupUri="Exercises/TASK-017-button-controltemplate/Task017Window.xaml"
```

因此从工作区根目录执行下面命令，现在打开的是 TASK-017：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

以后单独复跑 TASK-015，需要明确使用真实入口 `Exercises/TASK-015-button-style/Task015Window.xaml`，观察后恢复当前入口。本轮没有切换、构建或运行。

最小观察可先不修改业务：确认实际窗口标题，然后看两个按钮共同内留白、读取可用而空历史清空禁用。若想复习 Setter，只在 Task015Window.xaml 把 Padding Value 改一处，关闭旧窗口后重跑，观察两个按钮是否一起改变，再恢复原值；这是可选说明，不是本轮已做操作或新增验收要求。

一次观察共同属性，再单独观察业务恢复，避免同时改资源位置、命令和样式。设计器预览不能代替运行窗口，直接调用 Execute 也不能单独证明样式与按钮绑定链正确。

## 32. 掌握范围与复习主线

| 内容 | 当前范围 |
| :--- | :--- |
| 补齐 Padding/HorizontalAlignment Setter、两按钮引用、移除直接属性 | 学员自主完成，历史运行已验证；样式 Q1 限此应用标已掌握 |
| Window.Resources/空 Style/控件连接与原业务骨架 | 教练提供，不算独立框架设计 |
| 内部 DynamicResource 的应用 | 已验证取得同一 Style，应用范围计入 Q1 |
| 独立初始化、资源时机与完整优先级机制 | Q2 等继续待复习，不因通过自动扩大 |
| 32,14 独立副本、自动化和程序集同步 | 审查工具准备，不计学员自主能力 |

```text
共同属性 → Setter → Style 对象 S
S 存入 Window.Resources，键为 ActionButtonStyle
窗口读取按钮 StaticResource → S
内部按钮 DynamicResource，接入窗口后 → S
移除 Padding/对齐本地值 → 两按钮从 S 取得共同值

Style 负责当前外观设置
DataContext/Binding 负责输入、历史、提示和命令连接
Command/集合与属性通知负责原业务和禁用恢复
```

应能区分：资源键与控件名称、Style 与 Command、Setter 与 C# setter、Padding 与 Margin、控件对齐与文字对齐、定义与引用、资源作用域与构造时机、共用对象与数值恰好一致、改磁盘源码与运行时资源变化。

TASK-015 保持 ✅ 已通关。本笔记不新增隐式样式、BasedOn、Trigger、ControlTemplate、多主题或完整优先级验收，也不开始后续任务；机制复习与阶段状态以最新学习记录为准。
