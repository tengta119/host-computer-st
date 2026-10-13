# TASK-015：把两个按钮的重复设置放到一份 Style

> 2026-10-13 白话重写。先理解按钮留白与共同设置，再看两个按钮怎样取得它。资源加载问题、历史比较和掌握范围分开放在后面。

## 1. 先看原来重复写了什么

读取和清空按钮都需要内部留白、靠左排列。以前两个标签各写一遍，调整共同外观要改两处。

Style 是 WPF 提供的一组属性设置。当前把 Padding 和 HorizontalAlignment 放进一份 Style，然后让两个按钮都使用它。

两个按钮仍是两个对象，分别显示“读取温度”“清空历史”，执行不同命令；共同的是属性设置的来源。

## 2. Padding 增大，具体应该看哪里

Padding 是按钮文字与按钮边缘之间的内部留白：

```text
[按钮边缘 | 左留白 | 读取温度 | 右留白 | 按钮边缘]
```

24,10 表示左右各 24、上下各 10，底层是 WPF 的 Thickness 值。四个数则按左、上、右、下：24,10,24,10。

当前 Value="24, 10" 中的空格不改变数值。改成 32,14 后，左右每边多 8，上下每边多 4，当前自动大小按钮会需要更多空间。

Padding 不改字体或 Command。具体怎样显示还受按钮内部模板与布局影响，不能把本次尺寸变化当作所有控件的通用公式。

## 3. HorizontalAlignment 和 Margin 各管什么

HorizontalAlignment=Left 让按钮本身在父布局的横向空间内靠左；不是把按钮上的文字另设成左对齐。

Margin 是控件外部空间。历史区域上方 Margin 留出区域与读取按钮之间的间距，按钮 Padding 留出它内部文字四周的间距。

本题共同 Style 只提取 Padding/HorizontalAlignment。清空按钮当前没有显式 Margin，有效值为 0；历史审查确认位置正常，无重叠，不把 TASK-014 的旧 Margin 数值当作本题已经保留的值。

## 4. 当前共同 Style：先读一项 Setter

当前定义：

```xml
<Style x:Key="ActionButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="24, 10"></Setter>
    <Setter Property="HorizontalAlignment" Value="Left"></Setter>
</Style>
```

Style、Setter 都是 WPF 类型。ActionButtonStyle 是教练取的资源名字，稍后解释怎样找它。

TargetType="Button" 可以读成：“这组设置给 Button 按钮用。”Button 是 WPF 按钮类，读取与清空是由它创建的两个具体按钮。

第一项 Setter 表达：“给使用这份 Style 的按钮提供 Padding=24,10。”Property 指明哪个属性，Value 指明值。第二项给按钮提供靠左设置。

Setter 与 C# 的 `set { ... }` 不同：这里描述控件属性值，不是在数据类里写接收赋值的代码。

## 5. Window.Resources 是窗口保存可查找对象的地方

这份 Style 实际放在：

```xml
<Window.Resources>
    <Style x:Key="ActionButtonStyle" TargetType="Button">
        <Setter Property="Padding" Value="24, 10"></Setter>
        <Setter Property="HorizontalAlignment" Value="Left"></Setter>
    </Style>
</Window.Resources>
```

Window.Resources 是给窗口 Resources 属性赋值的 XAML 写法。里面的对象按键保存，类似“名字 → 对象”的字典。

ActionButtonStyle 是键，右边存一份 Style 对象。资源不需要作为可见控件出现在屏幕上，按钮用到它时才产生外观效果。

放在当前窗口的 Resources，不表示自动成为所有窗口都能用的全局对象。

## 6. x:Key 与 x:Name 先分清用途

当前有两种名字：

```xml
<Style x:Key="ActionButtonStyle" ... />
<Button x:Name="ReadButton" ... />
```

省略号只用于说明，不能原样粘贴。x:Key 给资源字典里的 Style 起查找键；x:Name 给一个具体按钮起名称。

Binding 的 ReadCommand 又是 ViewModel 公开属性名。资源键、控件名、数据属性名分别用于三种查找，不需要互相同名。

ActionButtonStyle 不是自定义 C# 类名。改键名后引用也要对应，字符串资源键区分大小写。

## 7. StaticResource 把 Style 交给读取按钮

当前读取按钮：

```xml
<Button x:Name="ReadButton"
        Style="{StaticResource ActionButtonStyle}"
        Content="读取温度"
        Command="{Binding ReadCommand, Mode=OneWay}" />
```

左侧 Style 是这个 Button 的属性。右侧 StaticResource 按键 ActionButtonStyle 找已经可访问的资源，把那份 Style 对象交给它。

读取按钮在窗口 XAML 中，当前加载时可找到前面定义的窗口资源。StaticResource 是 XAML 的资源引用机制，不是 C# static，也不在每次点击时新建 Style。

有自定义 x:Key 的当前样式需要按钮显式引用。只定义 Style，并不会自动把当前窗口所有 Button 都改成它。

## 8. 内部清空按钮当前用 DynamicResource

```xml
<Button x:Name="ClearButton"
        Content="清空历史"
        Command="{Binding ClearCommand, Mode=OneWay}"
        Style="{DynamicResource ActionButtonStyle}" />
```

DynamicResource 同样按这个键找 Style。不同之处是保留可以延后取值、随相关上下文变化重新查找的引用。

它解决的是“此刻还找不到，稍后建立父子关系后再取”的资源问题，不等待温度数据，不调用命令。

为什么内部按钮需要这样处理，要看它是在什么时候创建的。

## 9. 历史控件初始化时，还没放进窗口

历史 ReadingHistoryControl 有自己的构造函数和 InitializeComponent。窗口加载遇到它的标签时，过程为：

```text
窗口正在创建界面
→ 创建 ReadingHistoryControl
→ 控件构造函数加载它自己的 XAML
→ 创建内部清空按钮
→ 控件之后才接入窗口布局
```

如果内部 ClearButton 在自己加载时用 StaticResource 立即找资源，它还没有当前窗口这条父级关系，不能直接找到只存于窗口 Resources 的 ActionButtonStyle。

因此曾出现运行时 XamlParseException。键名正确、构建通过，仍可能因这个加载时机而失败。

当前内部改为 DynamicResource，接到窗口后取得同一 Style；外部读取按钮继续 StaticResource。这个异常源于教练初始指导遗漏加载时机，已有更正，不记为学员典型错误。

## 10. 资源放在哪里，与什么时候找，都有影响

当前接入后的相关查找路径：

```text
清空按钮 → 控件内部 StackPanel → ReadingHistoryControl
→ 窗口外层 StackPanel → 窗口 Resources
```

这是本题相关路径，不是完整枚举 WPF 的主题/系统资源规则。

同文件夹、同 C# 命名空间，不保证控件构造时就能访问窗口资源。它们回答的是代码在哪里，不是此刻对象已连接到谁。

若为了解决查找在控件内又复制一份同键 Style，内部可能取得那份更近的对象。外观可以暂时相同，但不再是本题要求的“两个按钮取得同一窗口 Style”。

## 11. 直接写 Padding，会覆盖 Style 的 Padding Setter

说明反例，当前两个按钮已去掉对应直接属性：

```xml
<Button Style="{StaticResource ActionButtonStyle}"
        Padding="12,6"
        Content="读取温度" />
```

Style 提供 24,10，但按钮自己直接提供 12,6。这两项争的是同一个 Button.Padding；此处按钮本地值优先，所以仍用 12,6。

移除直接 Padding 后，按钮才从共同 Setter 取值。HorizontalAlignment 同理。

这是本地值与 Style Setter 的比较，不说它高于 WPF 所有机制；动画、强制值等还有各自规则。理解当前冲突即可。

## 12. 写了 Style 属性，不表示所有属性都是“直接值”

当前 Button.Style 上有资源引用；Button.Padding 却从 Style 内的 Setter 得到。

```text
Button.Style：这一个按钮明确选择的 Style
Button.Padding：该 Style 中的 Setter 提供
Button.Content：各自文字
Button.Command：各自 Binding
```

讨论属性来源，要指明是哪一个属性。不能因为标签写了 Style，就说 Padding 也在按钮上直接写死了。

审查用 ReadLocalValue 检查 Padding/对齐为 UnsetValue。这个标记表示没有本地设置，不表示最终 Padding 等于零；有效值仍来自样式。检查 API 属于审查工具，本题不要求写进业务代码。

## 13. 共用 Style，两按钮仍可执行不同动作

读取按钮保留 ReadCommand 和“读取温度”，清空保留 ClearCommand 和“清空历史”。

Style 没设置 Content、Command 或 IsEnabled。所以原业务保持：清空 Count>0 才能用；列表空时禁用，Add 后启用，Clear 后禁用。

别为了“统一外观”删除命令或把两种文字放进一个共同 Setter。现在提取的只是共同属性。

同一窗口两按钮取得一份 Style 对象，各有自己的按钮实例。不同窗口实例各有窗口资源；不会因为键名相同就必定跨窗口共用一个对象。

## 14. Style 与 DataContext，分别存着什么

```text
窗口 Resources：ActionButtonStyle → Style
按钮从这里取得 Padding/对齐

窗口 DataContext：ReadingViewModel
输入、列表、命令、状态从这里取得数据
```

Style 没保存 DeviceId 或 Readings；ViewModel 没有 ActionButtonStyle 属性。用 Binding 去 ViewModel 找这个资源键，会走错地方。

两者都可能与界面父子关系有关，但各自寻找不同对象。改 DataContext 不能自动补出缺失资源。

## 15. 修改磁盘 XAML，要重新加载相应界面

任务观察方式是关闭旧窗口，改共同 Padding Setter，再运行。DynamicResource 不会监听磁盘文件，保存 XAML 不代表当前旧进程立即改变。

开发工具的热重载是另一项能力，本题不依赖它。运行中已经应用的 Style 还会进入不可修改状态，不能推定可任意修改它的 Setters。

本题验证的是共享设置的作用，不是运行时主题切换、全应用样式或多主题系统。

## 16. 先按现象定位，不把所有引用都换成动态

| 现象 | 先看哪一处 |
| :--- | :--- |
| 找不到 ActionButtonStyle | 键名、定义位置、内部控件加载时机 |
| 引了 Style 但留白仍旧 | 按钮是否残留直接 Padding |
| 只有一个按钮跟随 | 引用对象、局部值、是否复制另一份样式 |
| 改数据上下文没修好外观 | 是否把资源查找与业务绑定混用 |
| 改文件但旧窗口不变 | 是否仍是旧进程/旧构建 |
| 清空不能恢复 | 原集合、CanExecute 与通知连接 |

DynamicResource 找不到资源时不一定与 StaticResource 一样立即报错，也可能没取得预期值。窗口能开仍要核对实际 Padding 和同一对象引用。

当前旧 TODO 注释有 StaticResource/空 Style 等过程文字，实际内部已用 DynamicResource、Setter 已补齐，以实现和最新任务卡为准。

## 文末查阅：文件、比较与历史范围

文件：[窗口 XAML](../../WpfBae/Exercises/TASK-015-button-style/Task015Window.xaml)、[窗口后台](../../WpfBae/Exercises/TASK-015-button-style/Task015Window.xaml.cs)、[内部清空按钮](../../WpfBae/Exercises/TASK-015-button-style/ReadingHistoryControl.xaml)、[控件后台](../../WpfBae/Exercises/TASK-015-button-style/ReadingHistoryControl.xaml.cs)、[任务卡](../tasks/TASK-015.md)。

当前后台复用原 ViewModel，新建本窗口的一份实例，初始提示仍为 TODO 1；控件后台只初始化，不另建数据。当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-017，复跑 TASK-015 要明确临时入口并恢复，本轮未运行。

历史 2026-10-12 已通关：[结果](../tasks/evidence/TASK-015/results.json)、[构建/源码/程序集核对](../tasks/evidence/TASK-015/verification.json)、[原启动](../tasks/evidence/TASK-015/original-startup.json)、[审查程序](../tasks/evidence/TASK-015/Review.cs)。

| 历史版本 | Padding | 两按钮各自宽×高 |
| :--- | :--- | :--- |
| 原源码/原编译窗口 | 24,10 | 98 × 37.24 |
| 独立编译比较副本 | 32,14 | 114 × 45.24 |

副本改变共同 Setter 并另换命名空间，原作品保持 24,10。比较由教练准备，不能说学员已自主实现多主题或原窗口运行时改 Style。布局见 [原版](../tasks/evidence/TASK-015/padding-24-10.png)、[副本](../tasks/evidence/TASK-015/padding-32-14.png)。

9 个业务场景仍通过：初始、首次/重复/换 ID、空串/纯空格、清空、Trim 后恢复、再次清空。同对象/集合/命令保持，清空 Margin=0 但无重叠。此前审查工具混入旧程序集后已同步并重查，不计学员失败。

加载问题的内存诊断见 [诊断结果](../tasks/evidence/TASK-015/resource-lookup-diagnostic/results.json)，那项诊断与后续完整原窗口验收分开。先前整理对照历史清单中十个任务/业务文件一致，App.xaml 因入口推进不同。

自主范围限 Setter、两按钮引用和移除直接属性；Q1 保留应用已掌握，Q2 的资源机制继续待复习。可选小观察是只改共同 Padding 一项、关闭旧窗口重跑，看两按钮留白再恢复。

答疑见 [样式问答](../questions/wpf-styles.md)；官方查阅：[资源](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/systems/xaml-resources-overview)、[StaticResource](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/staticresource-markup-extension)、[DynamicResource](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/dynamicresource-markup-extension)。下一篇学习 [把历史拆成三个字段显示](./TASK-016-wpf-datatemplate.md)。
