# TASK-017 知识笔记：ControlTemplate、属性连接与禁用外观

> 整理日期：2026-10-13。任务：用 ControlTemplate 定义按钮内部外观。状态：✅ 已通关（2026-10-12）。
>
> 根据当前源码、任务卡、控件模板问答、学习记录及本地历史验收材料详细提取。代码分为“当前实现”“展开说明”“历史反例”，说明示例不表示已经写入项目或运行。本轮只整理文档与核对历史材料，不改学员源码、启动入口、任务状态或掌握范围。

## 来源与阅读顺序

| 来源 | 用途 |
| :--- | :--- |
| [TASK-017 任务卡](../tasks/TASK-017.md) | 场景、TODO、教学更正、DoD、验收与掌握边界 |
| [Task017Window.xaml](../../WpfBae/Exercises/TASK-017-button-controltemplate/Task017Window.xaml) | Style、Template Setter、Border、ContentPresenter 与禁用 Trigger |
| [Task017Window.xaml.cs](../../WpfBae/Exercises/TASK-017-button-controltemplate/Task017Window.xaml.cs) | 同一个 ViewModel 与初始状态 |
| [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-017-button-controltemplate/ReadingHistoryControl.xaml) | 原数据模板、字段间距、清空命令与动态样式引用 |
| [ReadingHistoryControl.xaml.cs](../../WpfBae/Exercises/TASK-017-button-controltemplate/ReadingHistoryControl.xaml.cs) | 控件初始化与父级数据源 |
| [ReadingViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs) | 集合、输入/状态、命令及通知连接 |
| [读取命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs) | 输入校验、创建记录、Add 与提示 |
| [清空命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs) | Count 条件、Clear 和状态更新 |
| [Reading 定义](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs) | 每条历史的 DeviceId、Value、Unit |
| [控件模板问答 Q1～Q4](../questions/wpf-controltemplates.md) | 模板结构、默认内容规则、Padding 连接和 Trigger/Opacity |
| [学习进度](../learning.md) | 教学反馈与应用掌握边界 |
| [最终 results.json](../tasks/evidence/TASK-017/results.json) | 11 个场景、按钮内部属性、字段状态及 Padding 诊断 |
| [verification.json](../tasks/evidence/TASK-017/verification.json) | 历史构建、39 个源码与程序集核对 |
| [原应用启动记录](../tasks/evidence/TASK-017/launch.json) | 当时原窗口标题、打开和正常关闭 |
| [审查程序](../tasks/evidence/TASK-017/Review.cs) | 实际按钮路径、同模板与内部元素检查 |
| [历史源码哈希清单](../tasks/evidence/TASK-017/source-hashes-before.json) | 当前与历史源码静态对照 |
| [TASK-015 笔记](./TASK-015-wpf-styles.md) | 共同样式、资源时机与属性优先级 |
| [TASK-016 笔记](./TASK-016-wpf-datatemplate.md) | Reading 行模板、逐行数据源与间距 |
| [App.xaml](../../WpfBae/App.xaml) | 当前真实入口 |

建议依次读：Button 与内部元素 → Template 的赋值 → Border 与文字 → TemplateBinding 的来源 → 禁用条件与外观规则 → 原命令/数据链 → 历史证据。Java 对象经验可以帮助分清“哪个对象的属性”，WPF 模板、默认连接及条件设置仍需分别说明。

## 1. 任务目标：两个按钮共用圆角外观，保留各自动作

已有按钮通过 Style 取得共同 Padding 和对齐。TASK-017 进一步说明按钮内部怎样画背景、边框和文字：

```text
读取按钮 A                              清空按钮 B
Content=读取温度                         Content=清空历史
Command=ReadCommand                      Command=ClearCommand
        └──── 同一 Style S / 模板 T ────┘
                 内部 Border + ContentPresenter
```

当前目标是圆角边框、各自文字、共同留白，以及清空禁用时变淡、启用时恢复。读取仍是固定 26.5 °C 的同步模拟业务；模板不新增设备通信或数据采集。

圆角改变的是外观。读取和清空动作仍属于原 Button 的 Command，不需要在内部 Border 上再实现按钮业务。

## 2. 专属目录与文件分工

专属目录：`WpfBae/Exercises/TASK-017-button-controltemplate/`。

| 文件 | 当前职责 |
| :--- | :--- |
| Task017Window.xaml | 共用 Style、ControlTemplate、窗口输入/命令/状态与控件嵌入 |
| Task017Window.xaml.cs | 创建一个 ReadingViewModel、设置初始提示、连接 DataContext |
| ReadingHistoryControl.xaml | 原三字段历史、行内 Margin、清空按钮和样式引用 |
| ReadingHistoryControl.xaml.cs | 初始化控件，不另建业务对象 |

窗口、模板外层、资源/控件/业务连接由教练提供。学员在骨架内调整圆角、连接 Padding、配置禁用 Trigger/Opacity；显式 Content 连接虽然在当前源码存在，但已改为可选。

Task017 的 ReadingHistoryControl 是本题命名空间中的类。ViewModel、双命令和 Reading 复用原类型，未复制出新的业务实现。

## 3. 类型、属性、语法和自定义名字的身份

| 名称 | 身份 | 当前作用 |
| :--- | :--- | :--- |
| ControlTemplate | WPF 类型，System.Windows.Controls | 描述 Button 内部视觉结构 |
| Button.Template | WPF 属性 | 接收模板对象 |
| Style、Setter、Trigger | WPF 类型，System.Windows | 共同属性设置与条件规则 |
| Setter.Value | Setter 的属性及其 XAML 属性元素写法 | 放入模板对象作为值 |
| Border、ContentPresenter | WPF 类型，System.Windows.Controls | 画外框、显示内容 |
| CornerRadius、Background、BorderBrush、BorderThickness、Padding | Border 的 WPF 属性 | 圆角、填充、边框与留白 |
| Content | Button/ContentPresenter 各自的 WPF 属性 | 原内容、内部要显示的内容 |
| TemplateBinding | WPF 模板取值机制 | 内部元素读取所应用 Button 的属性 |
| IsEnabled、Opacity | WPF 属性 | 是否可用、不透明度 |
| TargetName | Setter 的 WPF 属性 | 指定模板中要设置的元素 |
| ButtonBorder | 教练自定义的 x:Name | 标识模板内部 Border |
| ActionButtonStyle | 自定义资源键 | 查找共同 Style |
| ReadCommand、ClearCommand | ViewModel 的自定义属性 | 提供命令对象 |

ButtonBorder 不是框架类，也不是要求学员预先知道的 API。TemplateBinding 不是 C# 关键字，不能仅凭它出现在注释里就默认已经理解。

## 4. Style、DataTemplate、ControlTemplate、UserControl 的分工

| 概念 | 它描述什么 | 当前对应 |
| :--- | :--- | :--- |
| Style | 控件的属性设置 | Button.Padding、对齐和 Template |
| DataTemplate | 数据项如何显示 | 一条 Reading 的 ID/数值/单位 |
| ControlTemplate | 控件内部用哪些元素组成 | Button 内的 Border 与内容显示 |
| UserControl | 一块可以组合到窗口的区域 | 历史标题、列表和清空按钮 |

Style 可以把 Template 作为一个属性值交给 Button，两者不是互斥方案。DataTemplate 和 ControlTemplate 在本题同时存在，分别作用于历史读数和按钮。

不能把“按钮改为圆角”理解为需要新建 Reading 模板，或把“历史行三字段”理解为按钮内部结构。模板用途参见 [Microsoft 创建控件模板](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-create-apply-template)。

## 5. Template Setter 为什么使用 Setter.Value

先对照熟悉的普通值：

```xml
<Setter Property="Padding" Value="24,10" />
```

Property 指定设置哪项；Value 用字符串形式提供可转换为 Thickness 的值。Template 需要一个 ControlTemplate 对象，因此当前写为：

```xml
<Setter Property="Template">
    <Setter.Value>
        <ControlTemplate TargetType="Button">
            <!-- 模板结构和条件规则 -->
        </ControlTemplate>
    </Setter.Value>
</Setter>
```

Setter.Value 是给 Setter 的 Value 属性赋值，不是一个可见控件，也不是 Reading.Value。它把内部 ControlTemplate 对象交给这个 Setter，最终设置使用样式的 Button.Template。

```text
Setter.Property = Button 的 Template 属性
Setter.Value    = 模板对象 T
应用 Style      → Button.Template = T
```

这是概念连接图，不是已经执行的一段 C# 赋值代码。

## 6. TargetType="Button"：这份设置或内部布局给按钮用

先看当前界面上的两个实际按钮：“读取温度”和“清空历史”。它们都是用 WPF 的 Button 类创建的按钮，源码中分别叫 ReadButton、ClearButton。Button 是类名，后两个名字用来标识这两个具体按钮。

Style 可以先理解成“一组放在一起的属性设置”。当前这组设置包含按钮留白和靠左排列：

```xml
<Style x:Key="ActionButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="24,10" />
    <Setter Property="HorizontalAlignment" Value="Left" />
    <!-- 原文件这里还有 Template 设置 -->
</Style>
```

这里的 TargetType="Button" 就是说：“这组设置给 Button 按钮用。”所以 Setter 中的 Padding 是要设置 Button.Padding。当前读取和清空按钮各自引用了这份 Style，于是两者都取得这组设置；仅仅定义 Style 不会自动让所有按钮都使用它。

ControlTemplate 可以先理解成“按钮里面怎样画出来”。当前模板让 Border 画背景和圆角边框，让 ContentPresenter 显示按钮文字：

```xml
<ControlTemplate TargetType="Button">
    <!-- 原文件这里放 Border、ContentPresenter 和禁用外观规则 -->
</ControlTemplate>
```

这里的 TargetType="Button" 就是说：“这份内部布局给 Button 按钮用。”用在读取按钮上，就画读取按钮的内部；用在清空按钮上，就画清空按钮的内部。

因此两处都写 Button，是因为它们都为按钮服务：Style 集中保存要给按钮的属性值，ControlTemplate 描述按钮里面的界面。两者通过前面解释的 Template Setter 连在一起。至于内部怎样取得按钮的 Padding、怎样跟随禁用状态，分别在第 19 点和第 23 点展开，不需要在理解 TargetType 时同时引入这些规则。

## 7. 当前共同 Style 与完整模板

下方去掉旧 TODO 注释并整理空白；原空 Setter 的长写法缩为等价自闭合写法，不改作品：

```xml
<Style x:Key="ActionButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="24,10" />
    <Setter Property="HorizontalAlignment" Value="Left" />
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border x:Name="ButtonBorder"
                        CornerRadius="12"
                        Background="#E8F0FE"
                        BorderBrush="#4267A9"
                        BorderThickness="1"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter Content="{TemplateBinding Content}"
                                      HorizontalAlignment="Center"
                                      VerticalAlignment="Center" />
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsEnabled" Value="False">
                        <Setter TargetName="ButtonBorder"
                                Property="Opacity"
                                Value="0.4" />
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

此 Style 定义在 Task017Window 的 Window.Resources 中。当前没有独立给模板分配第二个资源键；模板作为这个 Style 的 Template Setter 值被复用。

## 8. 两个按钮如何找到同一 Style 和模板

当前引用：

```xml
<!-- 窗口读取按钮 -->
<Button x:Name="ReadButton"
        Style="{StaticResource ActionButtonStyle}"
        Content="读取温度"
        Command="{Binding ReadCommand, Mode=OneWay}" />

<!-- 历史控件内清空按钮 -->
<Button x:Name="ClearButton"
        Content="清空历史"
        Command="{Binding ClearCommand, Mode=OneWay}"
        Style="{DynamicResource ActionButtonStyle}" />
```

读取按钮在窗口加载上下文中取得 S；内部清空按钮沿用延后资源引用，在控件接入窗口后取得 S。S 的 Template Setter 提供 T，因此当前两按钮 Template 也为同一对象。

StaticResource/DynamicResource 按资源键找外观对象；Command 的 Binding 从 ViewModel 找命令。这是两条连接，不能把 ActionButtonStyle 当作 ReadCommand 的属性。

内部控件初始化时机已在 TASK-015 解释，参见 [样式笔记](./TASK-015-wpf-styles.md)。本题没有复制第二份局部 Style 或全应用主题。

## 9. 共用模板对象，不共用同一个内部 Border

```text
模板定义 T
  ├─ 应用到 ReadButton A  → Border A1 + ContentPresenter A2
  └─ 应用到 ClearButton B → Border B1 + ContentPresenter B2
```

A1 与 B1 是不同界面对象，各属于自己的按钮。读取始终可用时 A1 可以 Opacity=1；空历史清空禁用时 B1 为 0.4，二者不会因为共用 T 而同时变淡。

同模板表示复用结构描述，不表示把一个 Border 放到两处，也不是两个按钮共享同一个命令。当前审查证明 Template 引用相同；实例和状态分别检查。

## 10. Border 负责背景、外框和内部留白

Border 是 WPF 容器，可以直接包含一个子元素。当前唯一直接子元素是 ContentPresenter，文字由该子元素显示。

| 当前属性 | 值 | 作用 |
| :--- | :--- | :--- |
| Background | #E8F0FE | 填充内部背景 |
| BorderBrush | #4267A9 | 边框画笔/颜色 |
| BorderThickness | 1 | 四边边框厚度 |
| CornerRadius | 12 | 四角圆角半径 |
| Padding | TemplateBinding Padding | 边框与内容之间的内部留白 |

`#RRGGBB` 是颜色表达，XAML 将这里的颜色值转换成可用画笔。BorderThickness 使用 Thickness；CornerRadius 使用同名的 CornerRadius 值类型，不能因为都写一个数就混为同一种属性。

当前 Border 不是 Button 的业务替代品；它负责实际绘制。依据：[Border 官方说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.border?view=windowsdesktop-10.0)。

## 11. CornerRadius 的变化与首个可观察步骤

骨架的 CornerRadius 为 0，学员改为 12。一个数表示四角统一半径，最终 JSON 为 `12,12,12,12`。

这个改动使两个按钮都呈圆角矩形，没有设置“圆形按钮”的宽高约束，也不改变文字或 Command。半径是 WPF 布局单位，不是温度数值。

只改圆角作为首步，能让学员对应真实 XAML 与运行现象；骨架已有其他结构，不要求先一次写完完整模板。当前实际圆角的验证来自历史审查与截图，不能仅凭源码出现 12 就推定运行已验证。

## 12. 固定 Border 颜色与 Button 属性并非自动同名连接

当前 Border.Background 和 BorderBrush 是模板里的固定值。Style 没为 Button.Background 定义相应 Setter，Border 也没有写 Background 的 TemplateBinding。

因此以后若单独设置 Button.Background，不能只凭名字相同就保证当前内部 Border 颜色跟随。模板作者要明确决定哪些属性固定，哪些从 Button 取得。

说明示例，未加入项目：

```xml
<!-- 若以后希望内部背景跟随所应用按钮的背景，可单独设计这样的连接 -->
<Border Background="{TemplateBinding Background}" />
```

本题只要求 Padding 的必要连接。固定颜色是当前设计，不把它描述为背景绑定错误，也不扩充验收为完整主题配置。

## 13. ContentPresenter 具体负责显示什么

读取按钮的“内容”在本题就是四个字“读取温度”；清空按钮是“清空历史”。Button.Content 保存这些文字，ContentPresenter 放在模板中使相应文字显示出来。

```text
Border：背景 + 线条 + 圆角 + 内容周围留白
ContentPresenter：把当前按钮的那四个字放在里面
```

ContentPresenter 不是第二个按钮，不执行读取或清空。其 Content 可支持不同内容类型，但本题实际只有字符串，不据此推定已实现图标/复杂内容模板。

审查会在 ContentPresenter 生成的显示结构中检查实际文字，而不仅检查 Button.Content 的赋值存在。

## 14. 两个 Content 的所属对象与赋值左右两侧

当前连接：

```xml
<ContentPresenter Content="{TemplateBinding Content}"
                  HorizontalAlignment="Center"
                  VerticalAlignment="Center" />
```

等号左边 Content 属于内部 ContentPresenter；右侧 TemplateBinding Content 读取正在使用模板的 Button.Content：

| 应用位置 | 取值来源 | 内部显示 |
| :--- | :--- | :--- |
| ReadButton | ReadButton.Content | 读取温度 |
| ClearButton | ClearButton.Content | 清空历史 |

同名属性分属两个对象，连接后值可以相同；不是 DataContext 上有一个名为 Content 的业务属性。

若把共享模板写成 `Content="读取温度"`，清空按钮也会显示这段固定文字。这种反例只用于说明来源，不是当前实现。

## 15. 删除显式 Content 连接仍显示的原因与教学更正

学员曾报告删除显式 Content TemplateBinding 后文字仍正常。当前 Button 的 ControlTemplate 中，ContentPresenter 在未另设内容来源时有默认内容连接；ContentSource 默认值为 Content 支持这一规则。

因此在本题，两种写法都可显示各按钮原内容：

```xml
<!-- 依赖当前 Button 模板中的默认内容规则 -->
<ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center" />

<!-- 当前源码的显式写法 -->
<ContentPresenter Content="{TemplateBinding Content}"
                  HorizontalAlignment="Center" VerticalAlignment="Center" />
```

当前源码保留显式版本，验收要求是实际文字正确；任务卡已将显式添加改为可选，不强迫删除或重写。

此前把显式连接列为必做属于教练练习设计问题，学员观察有效，不记错题。此默认规则不代表所有模板内部属性自动连接，Border.Padding 不具有同样规则。依据：[ContentPresenter Remarks](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.contentpresenter?view=windowsdesktop-10.0#remarks)、[ContentSource 默认值](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.contentpresenter.contentsource?view=windowsdesktop-10.0)。

## 16. TemplateBinding 的来源与单向更新

TemplateBinding 用于控件模板内部，从当前应用模板的控件取得属性。这里的来源控件叫作 TemplatedParent，可以先理解为“这一份内部界面所服务的 Button”，不是随意找最近一个布局父元素。

先列显式属性参数，再对照当前常用写法：

```xml
Padding="{TemplateBinding Property=Padding}"

<!-- 当前等价简写 -->
Padding="{TemplateBinding Padding}"
```

连接是单向的：Button.Padding 的有效值提供给 Border.Padding，并随源值更新。不是只在构造时复制一次，也不从 Border 反写 Button。

TemplateBinding 两端使用 WPF 依赖属性，适用于当前 Button/Border.Padding、Button/ContentPresenter.Content；不能拿它直接替代从 Reading 取任意 CLR 属性的 Binding。依据：[TemplateBinding 官方说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/templatebinding-markup-extension)。本题不要求引入完整 RelativeSource 写法。

## 17. Binding 与 TemplateBinding 分别读取哪一个对象

| 当前写法 | 来源对象 | 目标 |
| :--- | :--- | :--- |
| 窗口 TextBox 的 Binding DeviceId | ViewModel V | 输入框 Text |
| 行内 Binding Value | 对应 Reading R | 该行数值 TextBlock.Text |
| 按钮 Binding ClearCommand | ViewModel V | Button.Command |
| Border 的 TemplateBinding Padding | 当前 Button A 或 B | 内部 Border.Padding |
| ContentPresenter 的 TemplateBinding Content | 当前 Button A 或 B | 内部 Content |

```text
窗口数据 V ── 普通 Binding ──→ 输入 / 命令 / 状态
历史数据 R ── 普通 Binding ──→ 行内文字
按钮实例 B ── TemplateBinding → B 内部的留白 / 内容
```

设置窗口 DataContext 不能修复缺失的 Border.Padding 连接；TemplateBinding 也不能代替 ItemsSource 取得历史集合。理解语法前先指出具体来源对象，可以避免用“连接数据”解释另一层“连接属性”的抽象混淆。

## 18. 两个 Padding 碰巧相同，不表示已经连接

以下为骨架中的旧状态：

```xml
<Setter Property="Padding" Value="24,10" />

<!-- 模板内部旧的固定值 -->
<Border Padding="24,10">
    <ContentPresenter />
</Border>
```

Style 给 Button.Padding 提供 24,10；Border 另有自己的固定 24,10。它们属于不同对象，只是数值相同。

若只改 Button 的值，旧固定 Border 仍使用自己的值，不能靠“属性名都叫 Padding”自动跟随。首次换成 TemplateBinding 时数值仍是 24,10，外观通常不变，但来源关系已经变化。

所以“连接后第一眼没变”不代表修改无效；后续改变源属性才能观察跟随。当前任务通过的依据包含这项实际诊断。

## 19. 当前 Padding 的完整取值链

```text
ActionButtonStyle 的 Padding Setter：24,10
        ↓ 当前两按钮没有自己的对应固定值
ReadButton.Padding / ClearButton.Padding
        ↓ 各自模板的 TemplateBinding Padding
各自 ButtonBorder.Padding
        ↓ Border 布局实际留白
文字与内部边界之间出现对应空间
```

Button 与 Border 的 Padding 都使用 Thickness；24,10 表示左/右各 24、上/下各 10。Button.Padding 是可配置来源，Border.Padding 是当前内部布局实际使用的值。

必要连接让共同 Style 的设置真正进入模板，而非只存在于按钮属性。Padding 是内部留白，行内 Margin 是外部间距，两者用途沿用 TASK-016 的区分。

## 20. 历史 Padding 诊断：原窗口运行中临时改属性

results.json 的 diagnostics 记录：审查在已显示的原 Task017Window 内，对两个 Button 临时设置 Padding 为 32,14，再检查各自 Border 跟随，随后清除临时设置恢复 Style 来源。

| 时刻 | Button.Padding | Border.Padding | 读取宽×高 | 清空宽×高 |
| :--- | :--- | :--- | :--- | :--- |
| 原状态 | 24,10,24,10 | 24,10,24,10 | 98 × 37.24 | 98 × 37.24 |
| 临时诊断 | 32,14,32,14 | 32,14,32,14 | 114 × 45.24 | 114 × 45.24 |
| 恢复后 | 24,10,24,10 | 24,10,24,10 | 后续场景恢复原状态 | 后续场景恢复原状态 |

本次宽增 16、高增 8，与左右各增加 8、上下各增加 4 的变化一致；是当前布局结果，不是所有模板的通用尺寸公式。

审查用 SetCurrentValue 改运行实例属性，再用 ClearValue 撤销临时属性来源，未改源码、未改 Style.Setters、未创建独立 Padding 副本。TASK-015 使用独立编译副本对比，TASK-017 使用原窗口临时属性诊断，不能混为同一种证据。

这证明学员写的连接有效，不计审查工具操作为学员自主实现 Setter 修改或运行时主题切换。

## 21. Button 靠左与内部文字居中不矛盾

共同 Style 的 HorizontalAlignment=Left 指定按钮本身在外部布局中的横向位置。ContentPresenter 的 HorizontalAlignment=Center、VerticalAlignment=Center 指定内部内容显示元素的布局。

```text
外层横向可用空间：[靠左的 Button] ................
Button 内部空间：[Padding | 居中的文字 | Padding]
```

这三个对齐设置属于两个不同对象，不能把“按钮靠左”误读为“文字必须靠左”。当前没有连接 Button.HorizontalContentAlignment；内部 Center 是模板固定设置，不宣称所有公开内容对齐属性都已透传。

自动大小按钮下居中效果还取决于实际可用空间；本题不要求另设固定宽度。

## 22. Trigger 是条件持续期间的规则，不是点击事件

当前条件：

```xml
<Trigger Property="IsEnabled" Value="False">
    <!-- 仅在条件成立期间提供外观设置 -->
</Trigger>
```

它检查当前应用模板的 Button.IsEnabled 是否为 False。条件成立期间应用内部 Setter，条件不成立则撤销这项设置；不是“点击一次后永久赋值”的代码。

Trigger 不执行 ReadCommand/ClearCommand，不发布集合事件，也不负责把按钮设为禁用。IsEnabled 先因已有命令条件变化，外观规则再响应这个状态。

这里是普通属性 Trigger，不是 DataTrigger、EventTrigger 或动画。依据：[Trigger 官方说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.trigger?view=windowsdesktop-10.0#remarks)。

## 23. 条件与 Setter 目标分别属于 Button 和 Border

当前规则的完整含义：

```xml
<ControlTemplate.Triggers>
    <Trigger Property="IsEnabled" Value="False">
        <Setter TargetName="ButtonBorder" Property="Opacity" Value="0.4" />
    </Trigger>
</ControlTemplate.Triggers>
```

| 部分 | 实际指向 |
| :--- | :--- |
| ControlTemplate.Triggers | 模板保存条件规则的属性 |
| Trigger.Property=IsEnabled | 当前 Button 的可用状态 |
| Trigger.Value=False | 条件值，bool False |
| Setter.TargetName=ButtonBorder | 当前模板实例里的 Border |
| Setter.Property=Opacity | 该 Border 的不透明度 |
| Setter.Value=0.4 | 条件成立期间的外观值 |

不能把 Trigger 的 Value=False 读成 Opacity=False，也不能把 Setter 的 Value=0.4 当作 CanExecute 返回值。

ControlTemplate.Triggers 是属性元素，不是第二个可见控件；规则不作为 Border 的内容参与排版。

## 24. ButtonBorder 的名称范围与模板复用

Border 使用 `x:Name="ButtonBorder"`，Setter 用同一字符串 TargetName 找它。这个名字可以自行选择，但引用必须对应，名字不是资源键 ActionButtonStyle。

两个按钮应用 T 后各有自己的模板名称范围，因此两份内部 Border 都可以叫 ButtonBorder，不会变成窗口级同名冲突。

审查实际使用：

```csharp
Border border = (Border)button.Template.FindName("ButtonBorder", button);
```

此行来自审查工具。button 是当前检查的具体按钮；第二参数说明要找哪一份模板实例里的元素。它不是本题要求写进窗口后台的代码。

窗口 FindName、控件 FindName 与模板 FindName 的上下文不同，不能默认 `window.FindName("ButtonBorder")` 就能得到内部元素。依据：[WPF 模板名称范围](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-xaml-namescopes)。

## 25. Opacity 改显示效果，不改变命令条件

Opacity 是不透明度：1 表示完全不透明，0.4 表示当前元素以较低不透明度显示，0 表示完全透明。

当前 Setter 改的是 Border.Opacity，所以背景、边框和其中文字的视觉效果一起变淡。它不是改变 Background 颜色值，也不改变温度数据。

子元素受祖先 Opacity 的显示影响，并不意味着子元素自身 Opacity 属性读数一定等于 0.4。同样，Button.Opacity 未被此 Setter 修改；历史 JSON 记录的是 borderOpacity。

单独设置 Opacity=0.4 不能禁止点击，Opacity=0 也不自动取消输入或命令参与。真正禁用来自 Button.IsEnabled，外观只是把该状态表现出来。依据：[Opacity 官方说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.opacity?view=windowsdesktop-10.0)。

## 26. 启用后为什么自动回到 1，不需要另写启用 Setter

当前规则只在 Button.IsEnabled=False 时提供 Border.Opacity=0.4。当 Button 变为 True，WPF 撤销这条条件设置；当前没有其他 Opacity 来源，于是 Border 有效值回到默认 1。

```text
False：禁用条件成立 → Trigger 提供 0.4
True ：禁用条件消失 → 撤销该来源 → 当前默认 1
```

机制是“撤销这项临时来源”，不是规则暗中执行了一段 `Opacity = 1` 的 C#。若以后有其他基础值、动画或设置来源，恢复值要按当时属性系统判断，不保证永远是 1。

本任务不需要再添加 True 条件 Setter，也不要直接把 Border 固定为 Opacity=0.4 让所有状态都淡。当前模板 Trigger 与其他来源的优先级应按对象和属性区分，不能泛称“Trigger 永远覆盖全部设置”。参考：[Trigger 撤销规则](https://learn.microsoft.com/en-us/dotnet/api/system.windows.trigger?view=windowsdesktop-10.0#remarks)、[依赖属性值优先级](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/properties/dependency-property-value-precedence)。

## 27. 命令状态到禁用外观的完整链路

原清空条件为 `Readings.Count > 0`，读取条件固定 true：

```text
初始 C.Count=0
 → ClearCommand.CanExecute=false
 → ClearButton.IsEnabled=false
 → 该按钮模板 Trigger 成立
 → 其内部 Border.Opacity=0.4

有效读取 C.Add(R)
 → CollectionChanged
 → ViewModel 的集合事件处理方法
 → ClearCommand.NotifyCanExecuteChanged
 → WPF 重查 CanExecute=true
 → ClearButton.IsEnabled=true
 → Trigger 撤销 → Border.Opacity=1
```

实际清空 C.Clear 后 Count=0，通过同一链又变为 False/0.4；再次读取恢复 True/1。

CollectionChanged 更新列表并经原连接请求清空条件重查；PropertyChanged 更新状态文字；CanExecuteChanged 请求命令条件重查；模板 Trigger 响应 UI 状态。这些职责不能互相替代。

## 28. 自定义模板替换原内部视觉，状态外观需要明确设计

Button 仍是 Button，原命令/可用状态功能保留；内部视觉结构已经换成当前 Border/ContentPresenter。不能假定原模板的悬停、按下、禁用视觉全部自动复制到新模板。

当前明确实现了禁用的变淡规则，没有鼠标悬停、按下动画或完整主题状态。它们在任务卡是可选巩固，未做不影响必需 DoD，也不能描述成已自主实现。

IsEnabled 正常与“看起来有禁用区别”需要分别验证。没有禁用 Trigger 时按钮可能实际不能操作却仍看起来相同；仅把颜色变淡而仍可用则是另一问题。本题最终两层都有证据。

## 29. 同一 ViewModel 与每条 Reading 仍然保留

当前窗口后台核心：

```csharp
private ReadingViewModel _viewModel = new ReadingViewModel();

public Task017Window()
{
    InitializeComponent();
    _viewModel.StatusText = "TASK-017：先只改 TODO 1 的圆角，观察两个按钮。";
    DataContext = _viewModel;
}
```

new 创建一个 V，窗口、历史控件、列表和清空按钮继续使用它；控件后台仅 InitializeComponent，不另建 V/集合/命令。

模板并未创建另一份业务数据。ReadCommand 仍 Trim 局部 id、空白时提示并 return、有效时 new Reading/Add、成功提示；清空命令仍防护检查、Clear 原集合、提示已清空。

只改变输入 V.DeviceId 不改旧 Reading；带两侧空格输入的历史记录保存 Trim 后 ID，但当前命令没有把 Trim 后值回写输入属性。TemplateBinding 只关心按钮相关属性，未介入这些分支。

## 30. 三字段 DataTemplate 沿用，并与 Button 模板并存

历史控件当前每行仍为：

```xml
<DataTemplate>
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="{Binding DeviceId, Mode=OneWay}" Margin="0,0,12,0" />
        <TextBlock Text="{Binding Value, Mode=OneWay}" Margin="0,0,12,0" />
        <TextBlock Text="{Binding Unit, Mode=OneWay}" Margin="0,0,12,0" />
    </StackPanel>
</DataTemplate>
```

各行 Binding 来源是对应 Reading，列表 ItemsSource 来源是 V.Readings。当前任务副本的三项 Margin 写法统一为逗号分隔，TASK-016 原作品仍保留其实际写法，本轮不改旧任务源码。

新增 ControlTemplate 不应删除 ItemTemplate、改变 Readings 的元素类型或重新拼接整行。最终审查同时验证按钮内部外观与历史行字段，说明两层模板正常并存。

## 31. 十一个历史实际窗口场景

最终 results.json 记录 windowActuallyShown=true、fatalErrors 为空，下列 businessPassed/displayPassed 均为 true：

| 场景 | 条数 | 最终提示 | 清空可用 | 清空 Border.Opacity | 累计集合变化 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 初始空历史 | 0 | TASK-017 的 TODO 1 提示 | false | 0.4 | 0 |
| 首次读取 | 1 | 读取成功 | true | 1 | 1 |
| 同 ID 连续读取 | 2 | 读取成功 | true | 1 | 2 |
| 换 ID 追加 | 3 | 读取成功 | true | 1 | 3 |
| 仅改输入不改旧行 | 3 | 读取成功 | true | 1 | 3 |
| 空字符串保留历史 | 3 | 该 Id 不合法 | true | 1 | 3 |
| 纯空格保留历史 | 3 | 该 Id 不合法 | true | 1 | 3 |
| 清空后禁用 | 0 | 已清空 | false | 0.4 | 4 |
| 再读两侧空格 TEMP-003 | 1 | 读取成功 | true | 1 | 5 |
| 最小窗口尺寸可读 | 1 | 读取成功 | true | 1 | 5 |
| 再次清空 | 0 | 已清空 | false | 0.4 | 6 |

读取按钮在所有这些场景均启用，其内部 Border.Opacity 为 1；共同模板不会让它跟随清空状态变淡。

“累计集合变化”是同一次审查持续计数；只改输入、空白和改窗口尺寸没有新增集合变化。前三条读数为 TEMP-001、TEMP-001、TEMP-002，每条 26.5 °C；恢复条目为 TEMP-003。

最小窗口为 360×460。该场景证明本次已显示内容与布局可读，不宣称任意长 ID、无限历史或所有屏幕尺寸均已验收。

## 32. 审查怎样验证同模板、内部连接和真实按钮路径

[Review.cs](../tasks/evidence/TASK-017/Review.cs)引用原编译程序集，创建并 Show 原 Task017Window。输入通过 TextBox.SetCurrentValue 写入并保持绑定，按钮通过 ButtonAutomationPeer/IInvokeProvider.Invoke 进入 WPF 命令路径，没有直接调用 Execute 替代整个界面连接。

检查包括：

1. 两个 Style 与窗口资源 S 为同一对象，两 Template 为同一对象且等于 S 的 Template Setter.Value。
2. 各自找到模板中的 ButtonBorder；圆角 12、边框厚度 1、背景/画笔存在。
3. Border.Padding 等于当前 Button.Padding；临时改变后真实尺寸增大，再恢复来源。
4. ContentPresenter.Content 与 Button.Content 对应，内部实际显示文字可见。
5. Button.IsEnabled 与各自 Border.Opacity 形成预期 False/0.4、True/1 关系。
6. 原输入/状态/命令绑定 Active，原 ViewModel/集合/命令与历史记录引用保持；字段读取对应 Reading。
7. 控件可见且在窗口范围内，最小尺寸仍满足本次布局检查。

ReferenceEquals、Template.FindName、视觉树遍历和自动化提供程序都是审查工具使用的 API，不计学员独立实现范围。本轮未运行该审查工程。

## 33. 六张历史渲染、原程序启动与哈希核对

本轮已查看：

- [初始禁用](../tasks/evidence/TASK-017/initial-disabled.png)：两按钮圆角可见，清空较淡，历史为空。
- [三条历史](../tasks/evidence/TASK-017/history-three-readings.png)：旧 ID 保留、数值/单位分开，清空恢复正常外观。
- [清空后禁用](../tasks/evidence/TASK-017/history-cleared.png)：空列表、提示已清空、清空再次变淡。
- [再次读取恢复](../tasks/evidence/TASK-017/reading-restored.png)：TEMP-003 的 26.5 °C、清空启用。
- [最小窗口](../tasks/evidence/TASK-017/minimum-size.png)：360×460 时当前内容可读。
- [Padding 诊断](../tasks/evidence/TASK-017/padding-diagnostic.png)：临时属性改变时两个按钮更大，清空仍禁用变淡。

这些是当时实际 Show 后窗口内容的渲染，不包含完整桌面/标题栏，不是设计预览，也不是本轮重新启动截图。图片帮助观察，内部值与引用仍要结合 JSON 和审查代码判断。

launch.json 记录原 WpfBae.exe 的 TASK-017 标题、非零窗口句柄、请求关闭并正常退出 0。verification.json 记录 2026-10-12 构建 0 警告/错误、审查退出 0、39 个源码哈希前后一致、原程序集与审查引用 SHA256 相同。

本轮将历史 source-hashes-before.json 的 39 项 SHA256 与当前文件静态对照，39 项全部一致，包括 App.xaml。本次结论限该清单，不表示所有未列文件或生成输出都核对；本轮未构建、启动或验收新运行结果。

## 34. 旧 TODO 注释、初始提示与教学反馈怎样理解

当前源码仍有“固定 Padding 是占位”“显式添加 Content 连接”等原骨架注释，但实际 Padding 已是 TemplateBinding，禁用 Trigger 也存在；最新任务卡已明确 Content 显式写法可选。

旧注释是过程历史，不单独证明功能未完成。当前初始状态仍写 TODO 1 提示，历史验收按这个实际文案检查；本轮不清理作品注释或提示。

学员曾明确反馈“内容/内部显示”等抽象措辞听不懂。后续讲解应先用四个实际文字、具体元素及赋值左右两侧解释，再介绍规则；不把读过骨架注释视为理解。

Content 必写要求的更正和讲解不足归属教学问题，不记学员典型错误。TASK-017 未观察到新增典型认知错误，笔记中的排查反例也不自动写入错题本。

## 35. 常见现象与排查顺序

| 现象 | 优先核对 | 当前机制 |
| :--- | :--- | :--- |
| 两按钮仍是旧外观 | 实际窗口/程序集、Style 引用与 Template Setter | 定义模板还要应用到按钮 |
| 圆角只有一个按钮生效 | 两按钮 Style/Template、是否有局部覆盖 | 数值相同不保证模板同一 |
| 两按钮文字都变成读取温度 | ContentPresenter 是否写死字符串 | 应显示各自 Button.Content |
| 删显式 Content 后文字仍显示 | 当前 ContentPresenter 默认规则 | 此处显式版本可选 |
| Button.Padding 已改但内部不变 | Border 是否仍固定 Padding | 同名/同值不等于连接 |
| 连接后外观没有变化 | 源值与旧固定值是否相同 | 来源变了，值可能没变 |
| 改 Button.Background 无效果 | Border 是固定色还是连接 | 当前 Background 固定 |
| 清空不能点但外观相同 | 禁用 Trigger、条件和 TargetName | 功能禁用与外观区别分开检查 |
| 所有状态都一直很淡 | 是否把 Opacity 直接固定、是否有其他来源 | 当前只在 False 条件提供 0.4 |
| 文字淡了但能点击 | IsEnabled/CanExecute，而非只看 Opacity | 不透明度不决定业务条件 |
| 找不到 ButtonBorder | 模板名称范围、所应用按钮、名称对应 | 不是窗口级资源键 |
| 模板正常但历史不刷新 | 原集合/ItemsSource/通知 | ControlTemplate 不管理读数集合 |

先确认运行的是 TASK-017，再分清“数据值、按钮属性、内部属性、可见结果”四层，不一次改窗口 DataContext、资源位置和模板结构。

## 36. 当前真实入口与一个可选的小观察

App.xaml 当前为：

```xml
StartupUri="Exercises/TASK-017-button-controltemplate/Task017Window.xaml"
```

从工作区根目录运行：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

按当前入口会打开标题“TASK-017：用 ControlTemplate 定义按钮外观”。本轮没有执行该命令，也没有改入口。

最小复习观察可只在 Task017Window.xaml 的共享模板中，将禁用 Setter 的 Opacity 从 0.4 改为 0.6，关闭旧窗口后重跑：初始清空应仍禁用但比原来更不透明，读取后仍恢复 1。观察后恢复原值，再单独看清空/再读恢复，不同时改命令或 Padding。

以上为可选说明，不是已执行验证或新增任务验收。设计器预览不替代实际按钮状态链；已有任务通过也不要求因此另做考试。

## 37. 掌握范围与本题知识连接

| 内容 | 当前范围 |
| :--- | :--- |
| 在骨架内调整 CornerRadius | 学员完成，历史实际外观已验证 |
| Border.Padding 的 TemplateBinding | Q3 按本骨架中的属性连接应用已掌握 |
| 禁用 Trigger/Opacity 配置 | Q4 按本骨架中的规则配置应用已掌握 |
| 外层模板、Setter.Value、资源和业务连接 | 教练提供，不计独立完整控件设计 |
| 默认 Content 规则、完整模板机制 | Q1/Q2 继续待复习，不因任务通过自动升级 |
| 框架内部规则撤销与属性引擎 | 可继续复习，不计已完整理解 |
| 临时 Padding 诊断与自动化检查 | 教练审查工具操作，不计学员自主实现 |
| 悬停/按下、动画、完整主题 | 可选或未要求，未做不勾选 |

```text
资源键 ActionButtonStyle → Style S
S：Padding/对齐 Setter + Template Setter → 模板 T
T：各按钮自己的 Border + ContentPresenter
Button.Padding → TemplateBinding → 内部 Border.Padding
Button.Content → 默认/显式连接 → 内部文字

原集合 Count → CanExecute → Button.IsEnabled
Button.IsEnabled=False → Trigger → 该按钮 Border.Opacity=0.4
Button.IsEnabled=True  → 撤销规则 → 当前恢复 1

历史数据 Reading → 原 DataTemplate → 三字段文字
```

复习重点：模板定义与内部实例、Style 与 Template、DataTemplate 与 ControlTemplate、Setter.Value 与 Reading.Value、Button 与 Border 的 Padding、两个 Content 的对象来源、Binding 与 TemplateBinding、IsEnabled 与 Opacity、条件撤销与永久赋值、资源键与模板名称。

TASK-017 保持 ✅ 已通关，问答统计保留待复习 2/已掌握 2 的原范围，TASK-018 保持 ⚪ 未开始。计划第二阶段仍有数据图表等未完成范围，不以本笔记或已完成任务数宣称阶段整体通关。
