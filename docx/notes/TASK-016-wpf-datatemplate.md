# TASK-016 知识笔记：DataTemplate、逐行数据绑定与文字间距

> 整理日期：2026-10-13。任务：用 DataTemplate 展示结构化读取历史。状态：✅ 已通关（2026-10-12）。
>
> 根据当前源码、任务卡、数据模板问答、错题记录及本地历史验收材料详细提取。代码区分“当前实现”“展开说明”“历史反例”，说明示例不表示已经写入项目或运行。本轮仅整理文档与核对历史材料，不改学员源码、启动入口、任务状态或掌握范围。

## 来源与阅读顺序

| 来源 | 用途 |
| :--- | :--- |
| [TASK-016 任务卡](../tasks/TASK-016.md) | 任务范围、骨架来源、TODO、DoD 和自主修复记录 |
| [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml) | ItemsSource、ItemTemplate、三个字段绑定及实际 Margin |
| [ReadingHistoryControl.xaml.cs](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml.cs) | 控件初始化与父级 DataContext 继承 |
| [Task016Window.xaml](../../WpfBae/Exercises/TASK-016-reading-datatemplate/Task016Window.xaml) | 输入/状态/命令绑定、窗口 Style、历史控件嵌入 |
| [Task016Window.xaml.cs](../../WpfBae/Exercises/TASK-016-reading-datatemplate/Task016Window.xaml.cs) | 创建一个 ViewModel、初始提示与 DataContext |
| [Reading 定义](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs) | DeviceId、Value、Unit 与原 ToString |
| [ReadingViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs) | 同一集合、输入属性、命令对象和通知 |
| [读取命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs) | Trim、空白 return、创建 Reading 并 Add |
| [清空命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs) | Count 条件、Clear 和状态更新 |
| [数据模板问答 Q1/Q2](../questions/wpf-templates.md) | 每行数据源、数值路径与右边距解释 |
| [错题本](../mistakes.md) | 已修复路径错误及证据边界 |
| [最终 results.json](../tasks/evidence/TASK-016/results.json) | 十个实际窗口场景、字段状态和位置 |
| [最终 verification.json](../tasks/evidence/TASK-016/verification.json) | 构建、原程序集/审查程序集一致性及最终通过 |
| [路径修复前结果](../tasks/evidence/TASK-016/results-before-value-fix.json) | FallbackValue 路径、PathError、数值为空 |
| [间距修复前结果](../tasks/evidence/TASK-016/results-before-spacing-fix.json) | Value 已正确但 ID 与数值紧贴 |
| [审查程序](../tasks/evidence/TASK-016/Review.cs) | 实际控件输入、按钮自动化、每行数据源与布局检查 |
| [历史源码哈希清单](../tasks/evidence/TASK-016/source-hashes-before.json) | 历史文件与当前源码对照 |
| [TASK-013 笔记](./TASK-013-wpf-viewmodel.md) | 当前输入与原读取业务 |
| [TASK-014 笔记](./TASK-014-wpf-usercontrol.md) | 自定义控件与 DataContext 继承 |
| [TASK-015 笔记](./TASK-015-wpf-styles.md) | 同一 Style、两种资源引用与属性优先级 |
| [App.xaml](../../WpfBae/App.xaml) | 当前入口仍为 TASK-017 |

建议依次看：一条 Reading 的三个属性 → 集合与列表 → ItemTemplate 的赋值语法 → 每行 Binding 的真实对象 → 横向布局与间距 → 两次自主修复及验收证据。Java 类/对象和集合经验可用于理解数据身份，WPF 模板及 XAML 仍需独立说明。

## 1. 任务解决什么问题：从整行字符串到三个显示控件

此前历史列表没有本题的 ItemTemplate，显示每条 Reading 的字符串表示。Reading 自己的 ToString 把 ID、数值和单位拼成一段文字：

```csharp
public override string ToString()
{
    return $"{DeviceId} {Value} {Unit} ";
}
```

上方是现有方法排版展开，结尾空格也来自原实现。它适合把对象整体转成字符串，但无法分别给 ID 和数值设置界面间距。

TASK-016 使用三个 TextBlock：一个显示 ID，一个显示 Value，一个显示 Unit。原数据与集合继续保留，变化发生在显示方式。

```text
一条 Reading：DeviceId=TEMP-001，Value=26.5，Unit=°C
                         ↓ 数据模板
一行界面：[TEMP-001]  [26.5]  [°C]
           控件 A     控件 B   控件 C
```

WPF 未指定数据展示模板时的字符串显示与 DataTemplate 的作用，可参见 [Microsoft 数据模板概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/data-templating-overview)。当前模板显示路径不调用 Reading.ToString；保留该方法不会与本题冲突。

## 2. 专属文件、原类型和本题的改动位置

任务专属目录：`WpfBae/Exercises/TASK-016-reading-datatemplate/`。

| 文件 | 当前职责 | 来源边界 |
| :--- | :--- | :--- |
| Task016Window.xaml | 窗口布局、输入/状态/命令、共同按钮样式 | 教练准备，复用此前连接 |
| Task016Window.xaml.cs | 创建 ViewModel 并交给窗口 DataContext | 教练准备 |
| ReadingHistoryControl.xaml | 列表、内联行模板、三个字段和 Margin | 模板外层由教练提供；字段绑定/间距由学员完成 |
| ReadingHistoryControl.xaml.cs | 只做控件初始化 | 教练准备 |

Task016 的 ReadingHistoryControl 位于 `WpfBae.Exercises.Task016`，与 Task014/Task015 同名控件不是同一个类。ViewModel 与双命令直接复用 Task013 类型；Reading 直接复用 Task010 类型，没有新增一套业务模型。

本题的核心字段改动集中在历史控件 XAML 中，不要求把 ViewModel、集合或设备读取逻辑搬入 DataTemplate。

## 3. 关键名称的身份：框架类型、属性和自定义名字

| 名称 | 来源/身份 | 当前用途 |
| :--- | :--- | :--- |
| DataTemplate | WPF 类型，System.Windows | 描述一个数据项的显示结构 |
| ListBox、TextBlock、StackPanel | WPF 控件类型 | 列表、文字显示、布局 |
| ItemsSource、ItemTemplate | WPF 属性，ListBox 继承使用 | 数据集合、每项显示模板 |
| DataContext | WPF 属性 | 默认 Binding 的数据对象 |
| Text、Orientation、Margin | WPF 属性 | 文字、排列方向、外部留白 |
| Binding、Path、Mode | WPF 绑定机制及配置 | 数据源属性到目标属性的连接 |
| OneWay、TwoWay | WPF 绑定方向值 | 显示读取、输入读写 |
| Reading | 原自定义 C# 类 | 保存一条历史读数 |
| ReadingViewModel | 原自定义 C# 类 | 保存当前输入、集合、提示和命令 |
| DeviceId、Value、Unit | Reading 的自定义公开属性 | 这一条读数的三个字段 |
| Readings | ViewModel 的自定义公开属性 | 返回原 ObservableCollection |
| HistoryListBox、HistoryControl | 自定义控件名称 x:Name | 标识列表实例与历史控件实例 |
| ActionButtonStyle | 自定义资源键 | 标识共同按钮样式 |

DataTemplate 不是数据库表、Java 泛型模板参数或一个新 Reading 类；名字含 Template，也不能直接当作按钮 ControlTemplate。

## 4. Reading 的公开属性：先理解值，再读模板

当前 Reading 中的属性：

```csharp
public string DeviceId { get; init; } = "";
public double Value { get; init; }
public string Unit { get; init; } = "";
```

public 使外部代码及绑定能访问属性；string 保存文字，double 保存数值。get 允许读出，init 允许在初始化阶段赋值；这些是 C# 语法，不是 WPF 提供的读数字段。

为说明完整访问器写法，DeviceId 可按下方方式理解。此例仅用于解释，不替换原类：

```csharp
private string _deviceId = "";

public string DeviceId
{
    get
    {
        return _deviceId;
    }
    init
    {
        _deviceId = value;
    }
}
```

读取 `reading.DeviceId` 时 getter 返回字段；初始化赋值右侧的字符串进入 value。自动属性由编译器提供存储，省略显式字段。init 与普通 set 不同，当前外部代码不能在初始化完成后随意再次赋值这三个属性。依据：[C# init 文档](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init)。这不表示所有类只要有 init 就是深度不可变对象。

## 5. 每次有效读取都新建一条记录，历史有自己的值

原读取命令的关键动作排版展开：

```csharp
_viewModel.Readings.Add(new Reading
{
    DeviceId = id,
    Value = 26.5,
    Unit = "°C"
});
```

new Reading 创建一个对象；花括号是 C# 对象初始化器，为新对象公开属性赋值。id 是已 Trim 的局部字符串；Value 是固定模拟数值，不是实时设备返回值。

两次读取同一个 ID，仍会创建两个 Reading 对象并追加两条历史。换 ID 不会回头重写第一条对象：

```text
读取 TEMP-001 → R1：TEMP-001 / 26.5 / °C
读取 TEMP-001 → R2：TEMP-001 / 26.5 / °C
读取 TEMP-002 → R3：TEMP-002 / 26.5 / °C
```

模板显示 R1/R2/R3 各自保存的值，不能把每行 ID 都接回当前输入属性。当前没有“按 ID 去重后覆盖”的业务规则。

## 6. Readings getter 返回同一集合，不生成新集合

ViewModel 中已有：

```csharp
private ObservableCollection<Reading> _readings =
    new ObservableCollection<Reading>();

public ObservableCollection<Reading> Readings
{
    get
    {
        return _readings;
    }
}
```

ObservableCollection 是 .NET 提供的可通知集合，Reading 是其中元素的类型。这个 getter 每次返回原集合 C；它没有调用 new，也没有把对象转成字符串。

Readings 只有 get，不代表集合内容不能变。它限制属性本身的外部赋值；原集合仍可以 Add、Clear。模板变化也不需要把集合改成 `ObservableCollection<string>`。

```text
ViewModel V.Readings ──→ 集合 C ──→ R1、R2、R3
ListBox.ItemsSource ──→ 集合 C
```

## 7. ItemsSource 与 ItemTemplate 分别回答什么

| 配置 | 当前值 | 负责的问题 |
| :--- | :--- | :--- |
| ItemsSource | Binding Readings | 显示哪些数据项 |
| ItemTemplate | 一个 DataTemplate | 每个数据项用什么结构显示 |

ItemsSource 连接集合；ItemTemplate 不添加、删除或清空记录。只有模板而没有数据时，列表仍然没有历史行；只有集合而没有本题模板时，显示也不会自动变成三个可单独布局的字段。

DataTemplate 是 ItemTemplate 属性的值。当前一个模板定义应用到列表数据项，不需要为 R1、R2、R3 各写一份 XAML 模板。依据：[ItemsControl.ItemTemplate 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.itemscontrol.itemtemplate?view=windowsdesktop-10.0)。

## 8. ListBox.ItemTemplate 的点号表示属性赋值

最小结构说明：

```xml
<ListBox ItemsSource="{Binding Readings, Mode=OneWay}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Path=DeviceId, Mode=OneWay}" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

`<ListBox>` 创建列表；`<ListBox.ItemTemplate>` 是 XAML 的属性元素写法，表示给这一个 ListBox 的 ItemTemplate 属性赋值。内部 `<DataTemplate>` 是具体值，里面描述它要生成的界面。

ListBox.ItemTemplate 不是第二个列表，也不是名为 ItemTemplate 的自定义控件。它与 Window.Resources 的“类型名.属性名”形式相似，但一个设置行模板，一个设置资源字典。

## 9. 当前完整行模板与原写法

下面去掉历史 TODO 注释，保留当前属性值：

```xml
<ListBox x:Name="HistoryListBox"
         ItemsSource="{Binding Readings, Mode=OneWay}"
         Height="170">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding DeviceId, Mode=OneWay}"
                           Margin="0,0,12,0" />
                <TextBlock Text="{Binding Value, Mode=OneWay}"
                           Margin="0, 0 12 0"></TextBlock>
                <TextBlock Text="{Binding Unit, Mode=OneWay}"
                           Margin="0, 0 12 0"></TextBlock>
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

这三个 TextBlock 分别读取字段，没有把三项拼进同一个 Text，也没有绑定到 Reading.ToString。

当前模板直接写在列表内部，属于内联定义。它没有 x:Key、DataType 或模板选择器；不需要因为上一题学习了资源就强行把它移入资源字典。

## 10. 为什么列表的数据源与模板的数据源不同

窗口创建 ViewModel V 并赋给 DataContext。历史控件没有创建另一份 V，因此列表和清空按钮沿用 V。

WPF 展示 C 中的一条 Reading 时，让这条数据成为该项显示内容的来源；当前模板内部三个文字控件从对应 Reading 读取属性：

```text
窗口 / 历史控件 / 列表 / 清空按钮：DataContext → V
列表 ItemsSource：读取 V.Readings → C

第 1 行内部文字：DataContext → R1
第 2 行内部文字：DataContext → R2
第 3 行内部文字：DataContext → R3
```

这不是在构造函数里创建三份 ViewModel，也不要求自己给每行执行 DataContext 赋值。当前最终审查检查每个 TextBlock.DataContext 和 BindingExpression.DataItem 都与本行 Reading 为同一对象。

“父窗口 DataContext 继承”解释区域级绑定；“列表的数据项展示上下文”解释行内绑定。不能因为行在窗口里面，就推定所有层都从窗口 V 取属性。

## 11. 同样写 DeviceId，为什么得到不同值

假设已有 R1=TEMP-001、R2=TEMP-002，现在只把输入改为 TEMP-EDIT-ONLY：

| 位置 | Binding 数据源 | DeviceId 的具体含义 | 应显示 |
| :--- | :--- | :--- | :--- |
| 窗口 TextBox | ViewModel V | 当前编辑的输入 | TEMP-EDIT-ONLY |
| 第一行 TextBlock | Reading R1 | 第一次读数保存的 ID | TEMP-001 |
| 第二行 TextBlock | Reading R2 | 第二次读数保存的 ID | TEMP-002 |

Binding 路径只写属性名并不代表全局只有一份这个属性。先确定“当前对象是谁”，再找它的 DeviceId。

输入框使用 TwoWay，并在文字改变时写回 V；行内 TextBlock 使用 OneWay，读取 R1/R2。修改 V.DeviceId 没有执行 `R1.DeviceId = ...`，所以旧行保持历史值。

## 12. 一条字段绑定的来源、路径、目标和方向

以数值字段为例，先用显式 Path 写法说明，再对照当前简写：

```xml
<TextBlock Text="{Binding Path=Value, Mode=OneWay}" />

<!-- 当前等价简写 -->
<TextBlock Text="{Binding Value, Mode=OneWay}" />
```

| 部分 | 当前含义 |
| :--- | :--- |
| 来源对象 | 本行 Reading |
| Path=Value | 读取 Reading.Value |
| 目标对象 | 本行第二个 TextBlock |
| 目标属性 Text | 放入要显示的文字 |
| Mode=OneWay | 从数据到界面的方向 |

花括号里的第一个未写 `名称=` 的 Value 是属性路径，不是固定字符串“Value”，也不是 Setter.Value。来源：[Microsoft Binding 声明与路径语法](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/binding-declarations-overview)。

## 13. OneWay 规定方向，不保证任意数据改变都自动通知

OneWay 表示绑定把来源值提供给目标，不通过这个 TextBlock 把文字反写为数据。它适合当前只读历史显示。

当前 Reading 没实现 INotifyPropertyChanged，属性为 get/init。初次显示可以读取属性，不需要先为每个 getter 发通知；本题也没有要求运行中修改历史属性后刷新。

若未来换成可变数据类并在运行中更新同一条记录的 Value，还需相应的属性变化通知。把 Mode 写成 OneWay，不会自动给自定义对象补通知实现。

“能初次显示”与“支持后续属性变化”应分别判断。来源：[WPF 数据绑定概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)。当前集合 Add/Clear 的刷新是另一层机制，见第 27 点。

## 14. Value 是 double，Text 显示字符串

Reading.Value 实际为 double 26.5，TextBlock.Text 是文字。当前绑定把数值提供给文字目标，本次运行显示为 `26.5`。

这不意味着 Reading 已将 Value 改成字符串，也不是命令调用了 `Reading.ToString()` 再拆开三项。单位 °C 来自另一个 Unit 属性，当前 Value 绑定本身不带单位。

显示格式还可能受文化和格式设置影响。本任务没有设置 StringFormat、Converter 或小数位规则，因此不能把本次 `26.5` 推定为所有系统区域设置下固定的格式承诺。后续格式化应作为单独显示需求处理，本轮不扩充验收。

## 15. 一份模板定义可以生成各自的行内控件

```text
模板定义 T：横向 StackPanel + 三个 TextBlock
  ├─ 用于 R1 → 第 1 行自己的文字控件
  ├─ 用于 R2 → 第 2 行自己的文字控件
  └─ 用于 R3 → 第 3 行自己的文字控件
```

共用模板定义不表示所有行共用同一个 TextBlock 对象。当前显示的每行各有三个文字控件和对应的绑定，因此第一行可显示 TEMP-001，第三行可显示 TEMP-002。

生成行内界面的是 WPF 的数据展示机制，不需要在命令中手动 new TextBlock。不能把 Reading 添加到集合误写为把界面控件添加到业务集合。

这里描述当前少量、已显示的历史行；不据此推断长列表必须一次创建全部行控件。列表容器与实际界面生成可参见 [ItemTemplate 的工作过程](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.itemscontrol.itemtemplate?view=windowsdesktop-10.0)，虚拟化不属于本任务验收。

## 16. 横向 StackPanel 管一行，列表仍有多行

Orientation="Horizontal" 将模板内部三个 TextBlock 从左到右排列。默认未指定方向的外层 StackPanel 继续从上到下排列标题、列表、清空按钮。

```text
外层：读取历史
      ListBox
         第 1 行：ID → Value → Unit
         第 2 行：ID → Value → Unit
      清空历史按钮
```

模板里的 Horizontal 不表示所有历史记录都放在同一横行，也不负责改变 ListBox 的项目排列容器。

当前每行文字按自身所需宽度排列。不同长度的 ID 可能把该行数值推到不同横坐标，固定 Margin 只保证留白，不保证所有行成为对齐的表格列。本任务要求可读间距，没有要求新增 Grid 列或表格控件。

## 17. Margin 的一个、两个、四个数

Margin 是控件外部留白，用 WPF 的 Thickness 表示：

| 写法 | 左 | 上 | 右 | 下 |
| :--- | :--- | :--- | :--- | :--- |
| 12 | 12 | 12 | 12 | 12 |
| 12,4 | 12 | 4 | 12 | 4 |
| 0,0,12,0 | 0 | 0 | 12 | 0 |

四个数按左、上、右、下；两个数按左右、上下对称。当前 `0,0,12,0` 只在这个文字控件右侧留白，没有上下行距或左侧留白。

WPF 默认这里使用设备无关单位，每单位为 1/96 英寸；不应无条件说“12 一定是屏幕物理像素 12”。依据：[Thickness 语法与单位](https://learn.microsoft.com/en-us/dotnet/api/system.windows.thickness?view=windowsdesktop-10.0)。

## 18. 当前混合分隔的 Margin 已解析成功

原源码的三个写法：

```xml
<!-- ID -->
Margin="0,0,12,0"

<!-- 数值、单位 -->
Margin="0, 0 12 0"
```

后者混合逗号和空格，书写不够统一，但本次 WPF 实际成功解析为 `0,0,12,0`，最终 JSON 三个字段的有效 Margin 都是这一值。

因此历史失败不是“这串 Margin 导致编译或启动异常”。真正先发生的是数值路径不存在，后发生的是 ID 与数值之间未留空隙。

统一逗号写法更方便阅读，但本轮笔记保留作品实际写法，不修改源码，不把书写格式误记为已有运行错误。

## 19. 右边距放在哪个控件上，隔开哪两个字段

横向排列中：

```text
[ID 控件] -- ID 的右 Margin -- [数值控件] -- 数值的右 Margin -- [单位控件]
```

给数值加右边距，隔开的是数值与单位，不能回头隔开 ID 与数值。给单位加右边距，留白位于单位之后。

想隔开 ID 与数值，可在 ID 右侧留白，也可在数值左侧留白。当前学员采用前者，只改第一个 TextBlock 的 Margin 为 `0,0,12,0`。

一般按当前横向排列，相邻文字可见边界之间的留白受到前项右 Margin 与后项左 Margin 共同影响。若两侧都加 12，空隙可能变成两份，不能为了“补间距”盲目给所有边都设同值。

## 20. 三个 TextBlock 的实际布局职责

| 当前字段 | 有效 Margin | 直接留白的位置 |
| :--- | :--- | :--- |
| DeviceId | 0,0,12,0 | ID 与后面数值之间 |
| Value | 0,0,12,0 | 数值与后面单位之间 |
| Unit | 0,0,12,0 | 单位之后 |

最后一项的右边距不是 ID/数值间距来源。当前并非只有前两个控件写 Margin，原作品给单位也保留了右边距；这在最终结果里可见，不需要本轮替它删除。

设置右留白也不改变 Text 的内容。ID 仍是 TEMP-001，Value 仍显示 26.5，不是在字符串里插入多个空格模拟布局。

## 21. Margin 与 Padding：文字间距和按钮留白是两件事

当前文字控件的 Margin 控制字段之间的外部空间；共同按钮 Style 的 Padding 控制按钮文字与按钮边界之间的内部空间。

```text
历史行：TextBlock 的 Margin → 字段分开
按钮：Button 的 Padding → 按钮文字四周留白
```

调整 ActionButtonStyle.Padding 不会修复历史行 TEMP-00126.5 紧贴；给行字段加 Margin 也不会自动统一两个按钮内边距。

外层 HistoryControl 的 Margin="0,16,0,0" 用于历史区域与上方按钮间的位置，不等于行内字段间距。每个布局属性要先找它属于哪个控件。依据：[WPF 对齐、Margin 与 Padding](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/alignment-margins-and-padding-overview)。

## 22. 历史错误：Binding FallbackValue 读的是不存在的属性

以下为已经修复的历史反例，当前源码已是 Value：

```xml
<TextBlock Text="{Binding FallbackValue, Mode=OneWay}" />
```

这里等同 `Path=FallbackValue`，表示“从本行 Reading 查找名为 FallbackValue 的公开属性”。Reading 只有 DeviceId、Value、Unit，没有这个属性。

历史审查证实：Reading.Value 实际为 26.5，数值文字却为空，绑定状态 PathError；ID、单位绑定 Active。因此不是设备没有读到数值，也不是 °C 遮挡数字，而是显示路径查找失败。

OneWay 仅决定方向，不能把错误路径猜成 Value。根因有运行证据；为什么最初误写未确定，不推断为学员完全不懂 Binding。错题已自主修复，见 [错题本](../mistakes.md)。

## 23. 属性路径 FallbackValue 与绑定设置 FallbackValue 的区别

为理解同名术语，以下均是说明示例，未新增到项目：

```xml
<!-- 去数据源查找 FallbackValue 属性：这是历史错误路径 -->
<TextBlock Text="{Binding Path=FallbackValue, Mode=OneWay}" />

<!-- 去数据源查找 Value；查找不能得到值时，绑定使用指定后备显示 -->
<TextBlock Text="{Binding Path=Value, Mode=OneWay, FallbackValue=未取得数值}" />
```

前者的 FallbackValue 是 Path 的值；后者的 `FallbackValue=...` 是 Binding 的配置属性，为不能返回有效值的情形提供后备值。两者语法身份不同。

当前任务不需要后备值设置；正确动作是对照 Reading 修正路径为 Value。后备文字即使让空白变成“未取得数值”，也不能证明数值路径已经正确。来源：[BindingBase.FallbackValue 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.data.bindingbase.fallbackvalue?view=windowsdesktop-10.0)。

## 24. 编译通过、窗口显示、绑定有效需要分开核对

本题普通 Binding 的属性路径在运行时针对具体数据对象求值。XAML 语法可编译，并不意味着 FallbackValue 一定存在于运行时 Reading。

| 观察 | 能支持什么 | 不能单独支持什么 |
| :--- | :--- | :--- |
| 构建退出 0 | 当前构建成功 | 每项 Binding 路径正确 |
| 窗口打开 | 初始化/显示已完成 | 每个文字字段都显示成功 |
| Reading.Value=26.5 | 数据对象确实有数值 | TextBlock 已取得该数值 |
| Binding Active | 该次绑定状态有效 | 间距可读、整个任务通过 |
| 实际三字段与间距正确 | 当前行显示达标 | 所有业务分支都验证完毕 |

本题历史中业务十个场景都正常，但七个含历史行的场景显示失败。空历史没有行可检查，不能因“没有显示错误”就宣称数值字段已验证。

## 25. 两次自主修复的顺序与历史版本

| 阶段 | 数值显示 | ID→数值间距 | 当时必需 DoD |
| :--- | :--- | :--- | :--- |
| 修复数值路径前 | FallbackValue，PathError，空文字 | ID 右边距为 0 | 1/2 未通过，3 通过 |
| 已改为 Value，间距未修 | Active，26.5 | 实测 0，显示 TEMP-00126.5 | 1/3 通过，2 未通过 |
| 已加 ID 右边距 | Active，26.5 | 实测 12，可读 | 三项全部通过 |

两个改动都由学员自主完成，教练没有代改源码。当前是最后一行的状态，任务卡下方旧“进行中”“未通过”是过程历史。

对应结果分别保存在 results-before-value-fix.json、results-before-spacing-fix.json、最终 results.json。最终结果不覆盖掉修复过程，旧问题也不能覆盖最后的已通关结论。

## 26. 用实际坐标解释为什么加 ID 右边距有效

取三条历史场景的第一行，位置来自本地历史 JSON，单位为 WPF 布局单位：

| 版本 | ID 左边 | ID 宽度 | 数值左边 | 数值宽度 | 单位左边 | ID→数值 | 数值→单位 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| Value 已修、ID 间距未修 | 27 | 58.8433 | 85.8433 | 24 | 121.8433 | 0 | 12 |
| 最终版本 | 27 | 58.8433 | 97.8433 | 24 | 133.8433 | 12 | 12 |

间距按后一控件左边减去前一控件右边：

```text
修复前：85.8433 - (27 + 58.8433) = 0
修复后：97.8433 - (27 + 58.8433) = 12
数值→单位：133.8433 - (97.8433 + 24) = 12
```

表中小数做了四位显示舍入，原 JSON 保留更多位。当前最后改动只在 ID 控件加右边距，所以后面的数值、单位整体右移 12，数值与单位原有间距继续保持。

这些是本次默认字体/窗口的实际数值，不作为所有屏幕上固定文字宽度的承诺，也不据此给模板硬编码这些坐标。

## 27. 追加、清空和字段变化：通知分工仍然保留

```text
有效读取：C.Add(R)
 → CollectionChanged
 → 列表更新数据项，显示对应 Reading 的行
 → ViewModel 的集合事件处理方法
 → 清空命令 NotifyCanExecuteChanged
 → WPF 重查 CanExecute，清空启用

清空：C.Clear()
 → 列表移除历史行
 → 同一通知链，Count=0，清空禁用
```

| 通知 | 当前负责什么 |
| :--- | :--- |
| CollectionChanged | 集合增加/清空与列表数据项更新 |
| ViewModel.PropertyChanged | 当前输入/状态属性通知，例如 StatusText |
| CanExecuteChanged | 请求重查清空是否允许执行 |
| Reading 属性通知 | 当前类没有实现，本题不要求 |

ObservableCollection 通知集合成员变化，不替集合中的每个 Reading 自动发布属性通知。DataTemplate 提供显示结构，也不替代通知实现。集合与元素通知的区别见 [WPF 绑定集合说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)。

## 28. 原输入、校验和读取命令没有因模板变化而改变

当前窗口仍使用：

```xml
Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
Command="{Binding ReadCommand, Mode=OneWay}"
Text="{Binding StatusText, Mode=OneWay}"
```

原读取流程仍为：取得 V.DeviceId → 显示中间提示 → 对局部 id Trim → 空白时提示并 return → 有效时 Add(new Reading) → 提示读取成功。

Trim 只处理局部字符串，原代码没有将修剪后的 id 写回 V.DeviceId。最终恢复场景输入仍含两侧空格，而新历史记录保存 TEMP-003。

空白 return 阻止 Add，因此原三条历史保留，模板仍显示原对象。读取按钮 CanExecute 固定 true，空白校验仍在 Execute 内完成。DataTemplate 没承担这些业务判断。

## 29. 原 UserControl 与共同 Style 连接保持

控件后台只有 InitializeComponent，没有设置自己的 DataContext 或新建集合。窗口后台创建一个 V、设置初始提示，再将 DataContext 指向 V。

当前共同样式仍为：

```xml
<Style x:Key="ActionButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="24,10" />
    <Setter Property="HorizontalAlignment" Value="Left" />
</Style>
```

读取按钮使用 StaticResource；控件内清空按钮使用 DynamicResource，在接入窗口后取得窗口样式。两按钮保持各自 Content 和 Command，没有因为 ItemTemplate 改变而复制 Style。

样式引用从资源字典取得外观对象，Binding 从数据对象取得属性。模板内部每行的来源是 Reading；模板外部清空按钮的来源仍是 V，不会变成最后一条 Reading。资源时机详见 [TASK-015 笔记](./TASK-015-wpf-styles.md)。

## 30. DataTemplate、Style、ControlTemplate 和 UserControl 的区别

| 概念 | 关注点 | 本任务对应 |
| :--- | :--- | :--- |
| UserControl | 封装一块可组合的界面区域 | 历史标题、列表、清空按钮 |
| Style | 为目标控件提供属性设置 | 两按钮共同 Padding/对齐 |
| DataTemplate | 一个数据对象用哪些界面元素展示 | Reading 的三个字段文字 |
| ControlTemplate | 控件自身内部视觉结构 | TASK-016 未自行定义 |

三个字段变成三个 TextBlock，改变的是数据项展示。按钮依然使用已有内部外观，本题没有自行设计 Button 的 Border、ContentPresenter 或禁用 Trigger。

这些范围是 TASK-016 当时的实践范围；最新学习记录中 TASK-017 已通关，不能把旧卡“ControlTemplate 尚未覆盖”当成当前全部学习进度。此表仅帮助分辨职责，不新增任务或验收。

## 31. 十个历史实际窗口场景与集合计数

最终 results.json 记录 windowActuallyShown=true、fatalErrors 为空，十个场景 businessPassed/displayPassed 均为 true：

| 场景 | 历史条数 | 最终提示 | 清空可用 | 累计集合变化 |
| :--- | :--- | :--- | :--- | :--- |
| 初始空历史 | 0 | TASK-016 的 TODO 1 提示 | false | 0 |
| 首次读取 | 1 | 读取成功 | true | 1 |
| 同 ID 连续读取 | 2 | 读取成功 | true | 2 |
| 换 ID 追加 | 3 | 读取成功 | true | 3 |
| 仅改输入不改旧行 | 3 | 读取成功 | true | 3 |
| 空字符串保留历史 | 3 | 该 Id 不合法 | true | 3 |
| 纯空格保留历史 | 3 | 该 Id 不合法 | true | 3 |
| 清空后禁用 | 0 | 已清空 | false | 4 |
| 两侧空格 TEMP-003 恢复 | 1 | 读取成功 | true | 5 |
| 再次清空 | 0 | 已清空 | false | 6 |

“累计集合变化”是整个审查持续计数，不是每个场景各发布那么多次。仅改输入与两种空白都停在 3，说明这些步骤没有修改集合。

三条记录为 TEMP-001、TEMP-001、TEMP-002；每条数值 26.5、单位 °C。审查保存第一条对象引用并在仅改输入后核对它未替换，同时检查 ItemsSource 与 V.Readings 仍为原集合。

## 32. 审查工具、截图与源码核对能证明什么

[Review.cs](../tasks/evidence/TASK-016/Review.cs)加载原 WpfBae 程序集，实际创建并 Show Task016Window。输入使用 TextBox.SetCurrentValue 保留原绑定，按钮通过 ButtonAutomationPeer/IInvokeProvider.Invoke 进入 WPF 命令路径，不是直接调用 Execute 替代界面验证。

它逐行取得文字控件，检查绑定路径、OneWay、Active、实际 Text、TextBlock.DataContext 和 BindingExpression.DataItem 与 Reading 为同一对象，再比较真实布局位置。工具中这些检查 API 不算学员独立实现能力。

本轮已查看四张历史内容渲染：

- [路径修复前](../tasks/evidence/TASK-016/history-three-readings-before-value-fix.png)：只有 ID 与 °C，数值缺失。
- [间距修复前](../tasks/evidence/TASK-016/history-three-readings-before-spacing-fix.png)：显示 TEMP-00126.5，ID 与数值紧贴。
- [最终三条历史](../tasks/evidence/TASK-016/history-three-readings.png)：三个字段正确且分开，旧 ID 保留。
- [最终清空](../tasks/evidence/TASK-016/history-cleared.png)：列表为空，清空按钮变灰，提示已清空。

图片是当时窗口内容渲染，未包含完整桌面/标题栏；实际 Show 的依据还包括 JSON/审查代码。它们不是本轮重新启动的截图。

最终 verification.json 记录 2026-10-12 构建退出 0、0 警告/错误；审查退出 0；十个业务与显示场景通过；当轮 35 个源码哈希前后一致，原程序集/审查程序集 SHA256 相同。审查程序退出码只取决于运行异常，早期显示失败时也退出 0，不能把退出 0 单独解释为 DoD 通过。

本轮将历史哈希清单 35 个文件与当前静态对照：34 个一致，App.xaml 因入口已推进 TASK-017 而不同。清单不含后来新增的全部文件，不能说整个当前工作区与历史完全相同；本轮没有重新构建或运行。

## 33. 排查顺序、当前入口与一个可观察的小步骤

| 现象 | 先核对 | 本题线索 |
| :--- | :--- | :--- |
| 没有历史行 | Readings.Count、ItemsSource 和空白校验 | 模板本身不采集或添加数据 |
| 有 ID/单位但数值为空 | 本行 Reading.Value、数值 Path、绑定状态 | 历史 FallbackValue 路径错误 |
| 三字段正确但紧贴 | 哪个控件的哪一侧 Margin | ID 右侧或数值左侧留白 |
| 所有旧行随输入变 ID | 行内数据源是否误接回 V | 每行应读取自己的 Reading |
| 仅改变输入就新增记录 | 是否误将输入通知当读取动作 | 当前仅按钮命令执行 Add |
| 字段间距有了，列却不齐 | 每行 ID 自身宽度 | Horizontal StackPanel 不提供跨行列对齐 |
| 清空后按钮不禁用 | 原集合/CanExecute 通知链 | 行模板不控制清空条件 |
| 两按钮样式不同 | 原资源引用与同 Style 对象 | ItemTemplate 不应替换按钮样式 |
| 运行显示 TASK-017 | App.xaml.StartupUri | 当前真实入口已推进 |

当前入口为：

```xml
StartupUri="Exercises/TASK-017-button-controltemplate/Task017Window.xaml"
```

因此从工作区根目录执行下方命令，现在打开的是 TASK-017：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

以后若专门复跑 TASK-016，应先关闭旧窗口，将 App.xaml 的 StartupUri 临时指向 `Exercises/TASK-016-reading-datatemplate/Task016Window.xaml`，重跑并核对标题“TASK-016：用 DataTemplate 展示读取历史”，观察后恢复当前入口。本轮未执行这些修改或运行。

一个可选小观察：只把模板第一个 TextBlock 的右 Margin 从 12 改为 20，不改字段或业务；重跑后预期 ID 与数值之间增大，数值与单位原间距保持。真实文件见本笔记来源表，位置是 DataTemplate 内第一个 TextBlock；观察后恢复原值。这是复习说明，不是已运行结果或新增验收要求。

先观察一个字段间距，再单独观察换 ID 保留旧行，避免同时改数据源、资源作用域和命令。设计器预览及任务注释不能代替实际窗口证据。

## 34. 当前掌握边界与知识连接

| 内容 | 已有证据所支持的范围 |
| :--- | :--- |
| 三字段 Binding、对应 Reading 数据源 | 学员自主补齐并运行验证；Q1 按骨架中的应用范围已掌握 |
| Value 路径纠正、横向 Margin | 学员自主修正并运行验证；Q2 按本次应用范围已掌握 |
| DataTemplate/ItemTemplate 外层、窗口/控件与数据连接 | 教练提供，不计独立模板/控件设计 |
| 原业务和 Style | 复用已验证实现，本题确认未破坏 |
| 绑定引擎、完整模板机制、通知原理 | 不因任务通过扩大为全部掌握 |
| 属性运行时更新、StringFormat、选择器、DataTrigger | 本题未要求，不新增验收 |

```text
V：当前输入 DeviceId + Readings 集合 C + 命令 + 状态
  ├─ 输入框 TwoWay → V.DeviceId
  ├─ 列表 ItemsSource → C
  │    ├─ R1 → 模板里的 DeviceId / Value / Unit
  │    ├─ R2 → 模板里的 DeviceId / Value / Unit
  │    └─ R3 → 模板里的 DeviceId / Value / Unit
  └─ 清空命令 → C.Clear，通知列表与按钮条件

DataTemplate 决定每项显示结构
Binding 决定具体文字从哪个对象的哪个属性取得
Margin 决定控件之间的外部留白
原 Style 决定共同按钮属性
```

复习时重点分清：集合与数据项、模板定义与界面实例、窗口输入与历史 ID、ItemsSource 与 ItemTemplate、属性路径与绑定配置、Value 与 Setter.Value、数值存在与文字显示、编译通过与实际绑定、右边距与左边距、集合通知与元素属性通知。

TASK-016 保持 ✅ 已通关，Q1/Q2 只保留历史最终证据支持的掌握范围；原机制复习和阶段状态不改变，TASK-018 保持 ⚪ 未开始。本笔记不开始新实践或新增任务卡。
