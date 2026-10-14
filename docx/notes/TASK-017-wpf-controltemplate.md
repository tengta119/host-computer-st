# TASK-017：按钮的圆角、文字和禁用外观怎样画出来

> 2026-10-13 白话重写，2026-10-14 结合 TASK-017～025 整理更新入口与后续状态。先认识按钮里的两个元素，再看它们怎样取得按钮属性，最后解释禁用规则。当前代码已经通关；历史诊断、工具和掌握范围在文末。

## 1. 先看屏幕上的两个按钮

窗口里有“读取温度”和“清空历史”。它们都是 WPF 的 Button 按钮，分别执行原 ReadCommand 和 ClearCommand。

这次让两者都显示成圆角边框，文字仍各自不同，清空不能用时外观变淡。

我们仍使用原 Button。ControlTemplate 是 WPF 提供的“这个按钮里面用什么元素、怎样画”的描述，不需要为了圆角重新写一个设备按钮类。

## 2. Border 画背景和边框，不执行读取

先看内部外框的基础说明版，后面才补当前属性连接：

```xml
<Border CornerRadius="12"
        Background="#E8F0FE"
        BorderBrush="#4267A9"
        BorderThickness="1"
        Padding="24,10">
    <!-- 内容放这里 -->
</Border>
```

Border 是 WPF 提供的容器，直接放一个子元素。Background 填背景，BorderBrush 指边框颜色，BorderThickness=1 给四边边框厚度。

CornerRadius=12 让四角圆起来，0 则是直角；一个值应用到四角。颜色用 #RRGGBB 形式，WPF 转换成画笔；边框厚度使用 Thickness，圆角使用 CornerRadius 值类型，分别是不同属性。

Padding=24,10 是内部留白，左右各 24，上下各 10。上方用于先认识属性，当前作品的 Border.Padding 后面会从 Button 取得，已经不写固定值。

## 3. ContentPresenter 让按钮上的四个字出现

“内容”在本题就是按钮上的文字。读取按钮 Content="读取温度"，清空按钮 Content="清空历史"。

ContentPresenter 是 WPF 的内容显示元素，放到 Border 里面，让这些文字出现在按钮内部。它不执行命令，也不是第二个按钮。

```text
Border：画浅色背景、蓝色边框、圆角
ContentPresenter：在里面显示读取温度或清空历史
```

它可以支持其他内容类型，但当前实际只有字符串，不能由此说已实现图标或复杂内容模板。

## 4. ControlTemplate 把两者组成按钮内部

先看只表达结构的说明版：

```xml
<ControlTemplate TargetType="Button">
    <Border CornerRadius="12" Background="#E8F0FE"
            BorderBrush="#4267A9" BorderThickness="1" Padding="24,10">
        <ContentPresenter HorizontalAlignment="Center"
                          VerticalAlignment="Center" />
    </Border>
</ControlTemplate>
```

模板让一个 Border 包住一个 ContentPresenter。WPF 把这个描述用到一个 Button 时，会创建那一个按钮自己的内部元素。

这里没有历史记录或 ViewModel 的字段。它描述的是按钮内部，不是 TASK-016 中一条 Reading 的显示。

## 5. Style 的 Template Setter 怎样把模板交给按钮

前一题已有 Padding Setter：

```xml
<Setter Property="Padding" Value="24,10" />
```

这次再设 Button 的 Template：

```xml
<Setter Property="Template">
    <Setter.Value>
        <ControlTemplate TargetType="Button">
            <!-- 放刚解释的内部结构 -->
        </ControlTemplate>
    </Setter.Value>
</Setter>
```

Template 是 Button 接收内部模板的属性。Setter.Value 是 XAML 给 Setter 的 Value 属性赋值的写法；里面放一个 ControlTemplate 对象。

Padding 的值可以写为字符串，Template 的值是一个对象，所以用这层写法表达。Setter.Value 不是可见控件，也不是 Reading.Value。

这个 Setter 放在共同 ActionButtonStyle 里，按钮使用该 Style 时，也取得里面的 Template。

## 6. 两处 TargetType="Button"，都可以读成“给按钮用”

先指出具体对象：Button 是 WPF 按钮类；ReadButton、ClearButton 是当前两个具体按钮的名字。

```xml
<Style x:Key="ActionButtonStyle" TargetType="Button">
```

这句可以读成：“这组属性设置给 Button 按钮用。”里面的 Padding Setter 设置的是使用它的按钮的 Padding。

```xml
<ControlTemplate TargetType="Button">
```

这句可以读成：“这份内部布局给 Button 按钮用。”用于读取按钮时，画读取按钮的内部；用于清空按钮时，画清空按钮的内部。

两处都写 Button，是因为它们都为按钮服务：一处集中保存属性值，一处描述里面怎么画。先理解这件事，取值与禁用机制在后面各自讲，不在同一句再堆新术语。

## 7. 同一模板，两按钮有各自的内部元素

当前读取按钮用：

```xml
Style="{StaticResource ActionButtonStyle}"
```

内部清空按钮用：

```xml
Style="{DynamicResource ActionButtonStyle}"
```

两者都取得窗口同一 Style，其中 Template Setter 也提供同一份模板定义。内部资源查找时机沿用 TASK-015。

但 WPF 会分别创建读取按钮的 Border/ContentPresenter、清空按钮的 Border/ContentPresenter。这些内部对象不同，才能各显示自己的文字和可用状态。

模板定义相同，不是把同一个 Border 同时放两处；共享外观也没有合并 ReadCommand 与 ClearCommand。

## 8. 两个 Content 的来源，和它的默认规则

当前作品写：

```xml
<ContentPresenter Content="{TemplateBinding Content}"
                  HorizontalAlignment="Center"
                  VerticalAlignment="Center" />
```

先理解左右对象：左边 Content 属于内部 ContentPresenter；右边取得当前这个 Button.Content。读取按钮给“读取温度”，清空给“清空历史”。

TemplateBinding 是模板内部从当前 Button 取得属性的写法，下一节用确实需要连接的 Padding 具体展开。

在当前 Button 模板内，ContentPresenter 默认也能取得 Button.Content，ContentSource 的默认值是 Content。所以删掉显式 Content 连接，文字仍可显示。显式写法在本题可选，当前作品保留它。

学员曾提出这个有效观察，任务卡已纠正原必写要求。它属于教练练习设计问题，不记错题。不要把这项特定默认规则扩大成 Border 所有同名属性都会自动连接。

共享模板也不能把 Content 固定写成“读取温度”，否则清空也会显示相同文字。

## 9. Button.Padding 和 Border.Padding，属于两个对象

骨架里原来两处写 24,10：

```xml
<Setter Property="Padding" Value="24,10" />
```

这个 Setter 给 Button.Padding 提供值。内部旧写法：

```xml
<Border Padding="24,10">
```

则单独固定 Border.Padding。虽然数值相同，彼此没有这项连接。

只改 Button.Padding，内部固定 Border 仍可能继续留原来的空隙。当前改成：

```xml
<Border Padding="{TemplateBinding Padding}">
```

可以读成：“内部 Border 的 Padding，使用当前按钮的 Padding。”左侧是 Border 属性，右侧是 Button 属性。

## 10. TemplateBinding 会跟随源值，是单向连接

当前取值链：

```text
Style 提供 Button.Padding=24,10
→ 内部 Border 通过 TemplateBinding 取得它
→ Border 给文字周围留相应空间
```

Button.Padding 改变后，Border 跟着取得新值；不是只在构造时复制一次。它也不会从 Border 反写 Button。

完整参数形式 `{TemplateBinding Property=Padding}` 与当前 `{TemplateBinding Padding}` 在这里含义相同。两端使用 WPF 依赖属性，即这里 Button/Border 提供的 Padding 属性。

来源是这份内部界面所服务的 Button，WPF 称它为 TemplatedParent；不是从 ViewModel 或 Reading 读 Padding。行内 Value 仍用普通 Binding，不能拿这项写法代替它。

第一次把固定值改成连接，源值仍为 24,10，外观可能不变。来源关系变了，随后改源属性才能明显观察跟随。

## 11. 按钮靠左，内部文字居中，各有位置

Style 的 HorizontalAlignment=Left 让 Button 在窗口布局中靠左。

ContentPresenter 的 HorizontalAlignment/VerticalAlignment=Center 让它在按钮内部按居中方式安排。分别属于外部按钮和内部显示元素，不冲突。

当前 Border 的 Background/BorderBrush 是固定颜色，没有连接 Button.Background。以后只改 Button.Background，不能凭同名就保证内部颜色变化；模板需要明确哪些属性固定、哪些取自按钮。

同样，当前内部 Center 是固定设置，没有连接全部 Button 内容对齐属性。本题不是完整主题参数系统。

## 12. Opacity 让外观变淡，不会禁止点击

Opacity 是不透明度。1 完全不透明；0.4 比原来更淡；0 完全透明。

本题要在清空禁用时，把内部 Border.Opacity 设成 0.4。它的背景、边框和里面文字视觉上一起变淡。

子元素受到祖先不透明度的显示影响，不代表子元素自己 Opacity 属性值也改为 0.4；Button.Opacity 也没有被这个设置修改。

降低 Opacity 本身不能禁用按钮，透明元素也可能继续参与输入。真正的“能不能用”看 Button.IsEnabled 和命令条件。

## 13. Trigger：禁用条件成立期间，设置淡外观

当前规则：

```xml
<ControlTemplate.Triggers>
    <Trigger Property="IsEnabled" Value="False">
        <Setter TargetName="ButtonBorder" Property="Opacity" Value="0.4" />
    </Trigger>
</ControlTemplate.Triggers>
```

ControlTemplate.Triggers 保存这份模板的条件规则，不是另一个显示控件。

外层 Trigger 检查当前 Button.IsEnabled 是否为 False。内部 Setter 在条件成立期间给目标 Border 提供 Opacity=0.4。

它是持续条件规则，不是“点击时执行一次”的事件。它不设置 IsEnabled，也不执行清空；先有按钮状态，外观再跟随。

## 14. TargetName 指向哪一个 Border

Border 当前有：

```xml
x:Name="ButtonBorder"
```

ButtonBorder 是教练自取名字。Setter 的 TargetName="ButtonBorder" 指向当前按钮内部这一个 Border。

读取与清空的两份内部元素都可以用此名，因为各有自己的模板名称范围。清空变淡只改变它自己的 Border，不会把读取也变淡。

它与资源键 ActionButtonStyle 不同。审查查内部元素时用 `button.Template.FindName("ButtonBorder", button)` 指明哪个按钮的模板；不默认从窗口 FindName 能直接找到它。检查代码属于工具，不要求加到业务里。

## 15. 重新启用时，为什么不用再写 Opacity=1

Button.IsEnabled 变为 True 后，禁用 Trigger 条件不成立，WPF 撤销它提供的 0.4。

当前没有其他 Opacity 设置，Border 回到默认 1。机制是撤销这项临时来源，不是你另写了一次赋值方法。

若以后有其他基础值、动画或本地设置，恢复值要看当时来源，不能概括为所有 Trigger 都强行恢复 1 或压过全部属性规则。

本题不需要额外的启用 Setter，也不要把 Border 固定 Opacity=0.4，导致可用状态也一直淡。

## 16. CanExecute 怎样把禁用状态传到外观

清空命令条件仍为 Readings.Count>0：

```text
初始 Count=0 → CanExecute=false → ClearButton.IsEnabled=false
→ 其禁用规则成立 → 内部 Border.Opacity=0.4

Add 新记录 → CollectionChanged
→ 通知清空条件变化 → WPF 重查 CanExecute=true
→ ClearButton.IsEnabled=true → 撤销变淡设置 → 恢复1
```

点击清空后 Count=0，经同一链再次禁用变淡；再读取又恢复。

CollectionChanged 管记录变化，CanExecuteChanged 请求条件重查，PropertyChanged 更新提示文字，Trigger 响应 UI 状态。模板没有替代原通知或命令。

读取命令条件固定 true，所以它在这些场景中保持可用、不淡。

## 17. 把各部分放在一起：当前完整外观

以下去掉旧注释，空 Setter 改为等价自闭合排版：

```xml
<Style x:Key="ActionButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="24,10" />
    <Setter Property="HorizontalAlignment" Value="Left" />
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border x:Name="ButtonBorder" CornerRadius="12"
                        Background="#E8F0FE" BorderBrush="#4267A9"
                        BorderThickness="1"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter Content="{TemplateBinding Content}"
                                      HorizontalAlignment="Center"
                                      VerticalAlignment="Center" />
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsEnabled" Value="False">
                        <Setter TargetName="ButtonBorder"
                                Property="Opacity" Value="0.4" />
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

创建自定义 Template 替换原内部视觉。原按钮行为仍在，但原模板的悬停/按下视觉不会自动全部复制进来。当前只要求禁用区别，悬停/按下与动画仍为可选，未做不影响必需验收。

## 18. 原输入、历史和提示仍然使用同一数据对象

窗口创建一份 ReadingViewModel，设置 TASK-017 的 TODO 1 初始提示，再放入 DataContext。历史控件仅初始化界面，不另建对象。

输入 TwoWay 写回 DeviceId；ReadCommand 取得它、Trim 局部 id、空白提示并 return、有效时 Add 新 Reading。清空用原 ClearCommand，StatusText 用属性通知。

历史 ListBox 仍有 TASK-016 的 DataTemplate：三个 OneWay TextBlock 分别读对应 Reading 的 DeviceId/Value/Unit，每项右 Margin=12。

Button 的 ControlTemplate 与 Reading 的 DataTemplate 可以并存，分别处理按钮内部和数据行。只改输入不改旧记录；两侧空格会被局部 Trim 后保存为 TEMP-003，原输入不被命令自动修剪回写。

若界面出问题，先确认是哪层：数据对象、Button 属性、内部 Border/文字，还是实际布局。不要同时改三层来试错。

## 文末查阅：文件、诊断与历史依据

实际文件：[窗口模板](../../WpfBae/Exercises/TASK-017-button-controltemplate/Task017Window.xaml)、[窗口后台](../../WpfBae/Exercises/TASK-017-button-controltemplate/Task017Window.xaml.cs)、[历史控件](../../WpfBae/Exercises/TASK-017-button-controltemplate/ReadingHistoryControl.xaml)、[控件后台](../../WpfBae/Exercises/TASK-017-button-controltemplate/ReadingHistoryControl.xaml.cs)、[任务卡](../tasks/TASK-017.md)。

当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-025。从工作区执行 `dotnet run --project WpfBae/WpfBae.csproj` 会启动 TASK-025，不会自动打开这篇的 TASK-017。若以后专门复跑旧题，先明确切换到 Task017Window 的入口、关闭旧窗口并运行新版本，观察后恢复现入口。可选小观察：只把禁用 Opacity 从 0.4 改为 0.6，重跑看清空仍不能用但更不透明，读取后仍恢复 1，再还原。本轮没有改源码或运行。

2026-10-12 已通关：[11 个场景及诊断](../tasks/evidence/TASK-017/results.json)、[构建/源码/程序集核对](../tasks/evidence/TASK-017/verification.json)、[原启动](../tasks/evidence/TASK-017/launch.json)、[审查程序](../tasks/evidence/TASK-017/Review.cs)。场景含初始、首次、重复、换 ID、仅改输入、两种空白、清空、Trim 后恢复、最小尺寸、再次清空；均显示/业务通过。空历史清空 False/0.4，有历史 True/1，读取保持 True/1，最小窗口为 360×460。

Padding 诊断是在已显示的原窗口内，临时将两个 Button.Padding 设为 32,14；各自 Border 同步，两按钮从 98×37.24 增到 114×45.24，随后清除临时属性恢复 Style 的 24,10。未改共同 Setter 或源码，与 TASK-015 独立副本对比不同，不能当作学员自主运行时主题切换证据。

六张历史内容渲染：[初始](../tasks/evidence/TASK-017/initial-disabled.png)、[三条历史](../tasks/evidence/TASK-017/history-three-readings.png)、[清空](../tasks/evidence/TASK-017/history-cleared.png)、[恢复](../tasks/evidence/TASK-017/reading-restored.png)、[最小尺寸](../tasks/evidence/TASK-017/minimum-size.png)、[Padding 诊断](../tasks/evidence/TASK-017/padding-diagnostic.png)。不含完整桌面/标题栏；Show 与原启动有独立记录，不是本轮重新运行。

2026-10-13 整理时，对照历史清单的 39 个源码哈希全部一致，包括当时入口；历史原程序集/审查程序集哈希也一致。这是当时的核对记录，不能用来宣称 2026-10-14 的 App.xaml 仍指向 TASK-017。本轮笔记整理未产生新验收结论。

旧注释仍写固定 Padding、显式 Content 必写等原安排，实际已连接，最新卡片已更正必写要求。教学表达曾让学员难以阅读，已要求从具体对象和效果解释，不记为学员错误。

Q3/Q4 按骨架中的 Padding 连接和禁用规则应用已掌握；Q1/Q2 的外层模板、默认内容及框架机制继续待复习。完整控件设计/全部属性规则、悬停/按下等没有因此一起掌握。最新进度中 TASK-018～025 均已通关，TASK-026 未开始；后续内容从 [TASK-018：三条温度画成折线](./TASK-018-wpf-temperature-polyline.md) 继续阅读。

答疑见 [控件模板问答](../questions/wpf-controltemplates.md)。官方查阅：[创建模板](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-create-apply-template)、[TemplateBinding](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/templatebinding-markup-extension)、[默认内容来源](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.contentpresenter.contentsource?view=windowsdesktop-10.0)、[Trigger](https://learn.microsoft.com/en-us/dotnet/api/system.windows.trigger?view=windowsdesktop-10.0)、[Opacity](https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.opacity?view=windowsdesktop-10.0)。
