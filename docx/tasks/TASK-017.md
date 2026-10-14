# TASK-017：用 ControlTemplate 定义按钮内部外观

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」的 Style / Template。本卡继续控件模板；数据图表仍未覆盖。
- 选择依据：TASK-015 的共用 Style 和 TASK-016 的数据行模板已实际验收，数值路径与行内间距已自主修复。继续学习按钮本身如何由内部界面组成，沿用原命令与数据，避免同时引入通信或图表依赖；保留已有 C# 与 WPF 机制复习范围。
- 难度/预计时间：待补充（计划未给出）。
- 知识笔记：[ControlTemplate、属性连接与禁用外观](../notes/TASK-017-wpf-controltemplate.md)（2026-10-13 白话重写，2026-10-14 更新入口与后续状态；任务状态及掌握范围不变）。

## 场景与目标

在设备监控窗口里，让读取和清空按钮共用一个简单的圆角边框外观，显示各自原 Content，保留原 Padding、命令以及清空禁用/恢复。历史列表继续使用 TASK-016 的三字段 DataTemplate。

## 开始前先解释

- ControlTemplate 是 WPF 提供的控件内部界面描述；Button.Template 是接收它的属性。TASK-016 的 DataTemplate 展示一条 Reading，本卡的 ControlTemplate 展示一个 Button 的内部结构，逐个对应真实对象，不只列名词。
- Border 是 WPF 容器，可放一个内部元素并显示边框、背景、圆角；CornerRadius 控制圆角，BorderBrush/BorderThickness 控制边框。先只改变一个可见属性观察，不一次要求完整模板。
- ContentPresenter 是 WPF 的内容显示控件，在按钮模板中展示原按钮 Content。因此“读取温度”和“清空历史”仍来自各自 Button，不把两份文字写死在共享模板里。
- TemplateBinding 是 WPF 模板内部读取所应用控件属性的写法；例如内部 Border 的 Padding 读取外部 Button.Padding。此处来源是正在应用模板的 Button，与历史行 Binding 读取 Reading、窗口 Binding 读取 ViewModel 分开解释。
- 共享 Style 可通过 Template Setter 赋一个 ControlTemplate。Setter.Value 是把对象作为 Setter 的值的 XAML 写法，不是新的可见控件；先解释与已有普通 Setter 的对应关系再给最小例子。
- 自定义模板会替换原内部外观，需要明确保留禁用的可见区别。后续才解释 Trigger：当 Button.IsEnabled 为 False 时设置较低 Opacity，启用时恢复；它只设置外观，不执行命令或改历史。IsEnabled 已有运行证据，Trigger/Opacity 为新概念，须逐项解释。

## 用户实践与产出

1. 在教练提供的可运行模板连接上，先将共享 Border 的 CornerRadius 改为一个容易观察的值，运行确认两个按钮外观同步变化。
2. 理解 ContentPresenter 默认显示各自 Button.Content；显式 Content TemplateBinding 可选。补齐必要 Padding 连接，使内部留白取得共同 Style 的 Padding，保留原 Command。
3. 补禁用外观设置，实际确认初始清空禁用、读取后启用、清空后禁用、再次读取恢复，并验证历史模板与空白保护保持。

## 目录与开始约定

2026-10-12 用户明确“开启task17”。已检查任务卡、源码树、项目和原 App.xaml 入口；目标目录此前不存在，现已准备任务专属骨架：

- [Task017Window.xaml](../../WpfBae/Exercises/TASK-017-button-controltemplate/Task017Window.xaml)：窗口、共用 ActionButtonStyle、Template Setter、Border 与可读的 ContentPresenter 占位。
- [Task017Window.xaml.cs](../../WpfBae/Exercises/TASK-017-button-controltemplate/Task017Window.xaml.cs)：创建一个原 ReadingViewModel 实例，并连接窗口 DataContext。
- [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-017-button-controltemplate/ReadingHistoryControl.xaml)：沿用 TASK-016 三字段 Binding 和间距，清空按钮继续用 DynamicResource 取得窗口共同样式。
- [ReadingHistoryControl.xaml.cs](../../WpfBae/Exercises/TASK-017-button-controltemplate/ReadingHistoryControl.xaml.cs)：任务专属控件加载入口，继承父窗口数据源。

复用已有 [ReadingViewModel.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[ReadTemperatureCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs) 和 [Reading 所在文件](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)。原任务作品未改。共享入口例外：[App.xaml](../../WpfBae/App.xaml) 仅把 StartupUri 从 Task016Window 改为 `Exercises/TASK-017-button-controltemplate/Task017Window.xaml`；项目自动包含新文件，无项目文件/依赖修改。

| TODO | 真实位置 | 实践步骤 / DoD |
| :--- | :--- | :--- |
| 1：只改 CornerRadius 0 → 12，实际观察两个按钮 | Task017Window.xaml 的 Border | 步骤 1 / DoD 1 |
| 2：理解默认内容连接；显式 Content TemplateBinding 可选 | 同文件的 ContentPresenter | 步骤 2 / DoD 1（显示各自内容即可，不要求冗余显式写法） |
| 3：把 Border 固定 Padding 改为读取 Button.Padding | 同文件的 Border | 步骤 2 / DoD 1 |
| 4：理解 Trigger/Opacity 后补禁用外观及启用恢复 | 同文件的 ControlTemplate.Triggers | 步骤 3 / DoD 3 |
| 5：实际运行复查原业务和按钮状态链 | ReadingHistoryControl.xaml 末尾说明 | 步骤 3 / DoD 2、3 |

教练已提供外层模板与共享连接；ContentPresenter 在当前 Button 模板内默认会连接 Button.Content，TODO 2 以理解和观察此规则为主，显式写法可选，不能要求为了显示原内容必须添加它。2026-10-12 根据学员删除后仍可显示的观察及官方文档，已修正此前必写要求；源码旧 TODO 注释为原骨架说明，以本卡最新更正为准。开始时 Border.Padding 固定为 `24,10`、禁用外观待补；现学员已补 Padding TemplateBinding 和禁用 Trigger，实际窗口已验证通过。不能将骨架已有默认显示计为自主实现。

**开始时首个动作（现已完成）只做 TODO 1**：打开 Task017Window.xaml，把 `CornerRadius="0"` 改为 `CornerRadius="12"`。关闭旧程序，在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，观察读取/清空两个按钮都从直角变为圆角，各自文字仍可读。设计预览不能替代实际窗口证据。此段保留开始时操作，后续连接与状态链已于审查验证；必要解释见 [控件模板 Q1](../questions/wpf-controltemplates.md)。

## 验收标准（DoD）

- [x] 项目编译并实际打开任务窗口；两个按钮通过共享 Style 使用同一个自定义 ControlTemplate，内部边框/圆角可见，各自原 Content 由 ContentPresenter 显示，必要 Padding 连接有效。
- [x] 实际读取、换 ID、只改输入不改旧行及两种空白保护保持；历史 ListBox 的三字段 DataTemplate、原 Readings 集合与窗口输入/状态/命令绑定正常。
- [x] 初始清空禁用、读取启用、实际清空后列表为空并禁用、再次读取恢复；禁用与启用有清楚外观区别，正常状态文字与布局可读，有实际窗口证据。

## 2026-10-12 审查结果

三个必需 DoD 全部通过，无阻塞问题，状态 ✅ 已通关。未修改学员源码或原注释。

| DoD | 结论 | 已验证证据 |
| :--- | :--- | :--- |
| 1：共用控件模板与属性连接 | 通过 | 构建 0 警告/错误；原应用打开 TASK-017 窗口并关闭退出 0；实际显示两个按钮引用同一 Style 和同一 Template，圆角 12、边框 1、各自原文字正常。原 Padding 为 24,10；审查窗口内临时设 Button.Padding=32,14，内部 Border 同步取值，两个按钮由 98×37.24 变为 114×45.24，随后恢复。该临时诊断不计学员自主修改共同 Setter 的证据。 |
| 2：历史、输入与业务绑定 | 通过 | 原编译窗口经输入控件写回、按钮自动化调用原命令：同 ID 连读、换 TEMP-002、仅改输入保留旧行、空字符串/纯空格保护均正确。列表三字段分别绑定对应 Reading，间距可读，同一 ViewModel/集合/命令及输入/状态绑定始终保持。 |
| 3：禁用外观与恢复 | 通过 | 初始清空 IsEnabled=False、内部 Border.Opacity=0.4；读取后 True/1；实际清空后空列表且 False/0.4；带空格 TEMP-003 再读恢复 True/1。最小尺寸 360×460 文字与布局可读；再次清空也正确。 |

实际命令：

- `dotnet build WpfBae/WpfBae.csproj --nologo`：退出 0，0 警告、0 错误。
- PowerShell `Start-Process` 启动原 `WpfBae.exe`，取得 TASK-017 标题与原窗口句柄，`CloseMainWindow` 后退出 0；详见 [启动记录](./evidence/TASK-017/launch.json)。
- `dotnet run --project docx/tasks/evidence/TASK-017/Review.csproj -- docx/tasks/evidence/TASK-017`：退出 0；引用原程序集，实际 `Task017Window.Show()`，通过输入控件和 `ButtonAutomationPeer/IInvokeProvider` 调用按钮，11 个显示/业务场景全部通过、0 运行异常；另有 Padding 临时属性诊断通过。截图为已显示窗口的内容渲染，不是设计预览。

证据：[场景结果](./evidence/TASK-017/results.json)、[源码/程序集核对](./evidence/TASK-017/verification.json)、[初始禁用](./evidence/TASK-017/initial-disabled.png)、[三条历史](./evidence/TASK-017/history-three-readings.png)、[清空后禁用](./evidence/TASK-017/history-cleared.png)、[再次读取恢复](./evidence/TASK-017/reading-restored.png)、[最小窗口](./evidence/TASK-017/minimum-size.png)、[Padding 诊断](./evidence/TASK-017/padding-diagnostic.png)。39 个源码文件哈希前后一致，原程序集与审查引用 SHA256 相同。

掌握范围限在教练模板骨架中调整圆角、应用 Padding TemplateBinding、配置禁用 Trigger/Opacity。外层模板/资源/窗口/业务连接由教练提供；显示内容的默认规则与框架内部撤销/属性机制仍可复习，不推定独立完整控件设计。未观察到新增典型认知错误；未完成的悬停/按下可选项不计必需验收。

下一步依据：本任务在既有模板骨架内应用已验证，计划第二阶段仍有“数据图表”未覆盖。只生成下一张 [TASK-018：用 Canvas 与 Polyline 绘制模拟温度折线](./TASK-018.md)，状态 ⚪ 未开始，先以固定样例学习数据到坐标的转换；不引入新图表框架，不因已有任务全部通关宣称 WPF 阶段完成。

## 可选巩固

- 后续可加鼠标悬停或按下状态外观；本次不要求动画或完整主题，未做不勾选。
- 不以本卡通关推定完整控件设计、资源机制或 WPF 阶段全部掌握。

## 状态与证据

- 2026-10-12 审查完成：三个必需 DoD 与 11 个实际窗口场景全部通过，39 个源码哈希不变，状态 ✅ 已通关；详细证据见上方。只创建 TASK-018 卡片（⚪ 未开始），未准备其源码或切换 App.xaml。下方进行中/未运行记录为此前历史。
- 2026-10-12 TODO 4 答疑：当前学员已补 Border.Padding 的 TemplateBinding，ControlTemplate.Triggers 仍仅注释。已在 [控件模板 Q4](../questions/wpf-controltemplates.md) 解释两个新概念、真实添加位置、TargetName 对应既有 ButtonBorder 名称及临时设置/自动恢复，给出实际状态链观察方法。未代改/构建/运行，不据代码存在勾选 DoD，状态仍进行中。
- 2026-10-12 TODO 3 答疑：当前两个 Padding 均固定为 24,10，尚未见内部 Padding 连接。已在 [控件模板 Q3](../questions/wpf-controltemplates.md) 说明真实位置与首步替换，以及随后修改共同 Setter 观察留白变化的方法；建议操作不计已执行证据。本轮只答疑和记录更新，未代改/构建/运行，任务状态与 DoD 不变。
- 2026-10-12 默认内容连接更正：学员报告删除显式 Content TemplateBinding 后按钮内容仍正常显示；当前磁盘源码已见其添加显式连接。核对 Microsoft ContentPresenter/ContentSource 文档，确认在当前 Button 模板内默认连接原 Content。TODO 2 显式添加改为可选，原 DoD 1 仍要求各自内容实际正常显示，不要求某一种冗余语法；其他验收项不变。未代改学员源码或本地运行，不以观察报告替代完整 DoD 证据，任务仍进行中。
- 2026-10-12 TODO 2 答疑：当前源码 CornerRadius 已为 12，ContentPresenter 尚未显式设置 Content；未据此验证实际圆角或勾选 DoD。学员反馈注释含义不清，已在 [控件模板 Q2](../questions/wpf-controltemplates.md) 补充两个 Content 的对象来源、TemplateBinding 语法与单处改动；显式连接后默认文字通常保持不变。未代改源码、未构建或运行，本任务仍 🟡 进行中，验收范围不变。
- 2026-10-12 启动：四个骨架文件已创建，App.xaml 入口已切换。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0，0 警告、0 错误。未运行实际窗口或执行 DoD 验收；三个必需项均未勾选，状态 🟡 进行中。首步仅圆角观察，不新增下一任务，不升级掌握范围。
- 2026-10-12：TASK-016 三个必需 DoD、10 个实际窗口场景全部通过后，按计划只创建本卡。状态 ⚪ 未开始；未准备源码、未切换 App.xaml、未构建或验收本任务。

参考：[Microsoft WPF 样式与模板说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/styles-templates-overview)。
