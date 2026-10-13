# TASK-015：用 Style 统一读取与清空按钮的外观

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中 UserControl 后的 Style / Template；本卡先覆盖 Style 的最小应用，Template 后续按实践需要安排。
- 选择依据：TASK-014 已通过三个必需 DoD，历史区域封装和内部集合/命令绑定已验证。读取、清空按钮当前重复设置 Padding 与 HorizontalAlignment，适合用同一外观规则继续练习窗口和内部控件的关系；绑定及通知机制仍按原范围复习。
- 难度/预计时间：待补充（计划未给出）。
- 知识笔记：[Style、Setter 与按钮外观资源](../notes/TASK-015-wpf-styles.md)（2026-10-13 整理；知识解释与历史证据核对，任务状态及掌握范围不变）。

## 场景与目标

设备监控界面有多个操作按钮。如果每个按钮都重复设置内边距和对齐，调整时容易遗漏。把共同外观放到一份 Style 中，读取按钮和历史控件内的清空按钮显式引用它；修改一个 Setter 后，实际窗口里的两个按钮都能观察到变化，同时保持已验证的读取、历史及禁用/恢复行为。

## 开始前先解释

- Style 是 WPF 提供的外观设置对象，不是新的 C# 类名或业务命令；Setter 是 WPF 提供的属性设置项。先用当前 Button.Padding 的一处重复设置说明它们的用途，再讲 TargetType 指定这种样式用于哪类控件。
- Window.Resources 是 WPF 提供的资源集合，用于保存可查找和复用的对象。ActionButtonStyle 将是本题自定义资源名；x:Key 给资源起标识，StaticResource 按标识查找现有资源，Button.Style 把找到的 Style 交给按钮。先给完整最小例子，逐个说明身份和作用，不能只要求“抽样式”。
- 原按钮直接写的属性值可能覆盖 Style 中同一属性的 Setter。先对照具体 Padding 说明，再引导只移除已提取的重复设置；Margin 可保留为各自所在位置的间距。不要把命令、数据绑定或控件名称一起移走。
- 样式资源查找与 DataContext 查找是不同用途：前者找外观对象，后者找业务数据和命令。还需讲解时机：历史控件在自己的 InitializeComponent 中独立加载 XAML，尚未接到父窗口，内部 StaticResource 不能直接找到仅定义在窗口的资源；清空按钮用 DynamicResource 延后查找，读取按钮仍用 StaticResource。不另建 ViewModel，不写业务 Click。

## 用户实践与产出

1. 在任务窗口资源中定义一个带自定义键、用于 Button 的显式 Style，以 Setter 保存共同的 Padding、HorizontalAlignment。
2. 窗口读取按钮与内部历史控件清空按钮引用同一份 Style，移除已提取的重复外观属性，保留各自 Content、Command、位置间距和必要名称。
3. 实际运行后修改一处 Padding Setter，再运行确认两个按钮的内边距都改变；验证读取、换 ID、空白保护、清空和再次读取恢复，按钮禁用仍正常。

## 目录与开始约定

2026-10-12 用户明确开启。目标目录此前不存在，现已创建专属目录 `WpfBae/Exercises/TASK-015-button-style/`，包含四个文件：

- [Task015Window.xaml](../../WpfBae/Exercises/TASK-015-button-style/Task015Window.xaml)：窗口界面，已提供 Window.Resources 与带键的空 Style；Setter 和读取按钮的引用留为 TODO。
- [Task015Window.xaml.cs](../../WpfBae/Exercises/TASK-015-button-style/Task015Window.xaml.cs)：创建一个已有 ReadingViewModel 实例并设置窗口 DataContext。
- [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-015-button-style/ReadingHistoryControl.xaml)：沿用 TASK-014 完成的历史界面与绑定，新副本的清空按钮样式引用留为 TODO。
- [ReadingHistoryControl.xaml.cs](../../WpfBae/Exercises/TASK-015-button-style/ReadingHistoryControl.xaml.cs)：提供控件初始化，沿用父窗口 DataContext。

复用原 [ReadingViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[读取命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[清空命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs)及 TASK-010 的 Reading 类型，不改原实现。TASK-014 四个原文件完整保留；新控件在 Task015 命名空间中，避免与原类冲突。共享入口例外：[App.xaml](../../WpfBae/App.xaml) 的 StartupUri 切到 `Exercises/TASK-015-button-style/Task015Window.xaml`；无需改项目文件或添加依赖。

| TODO | 位置及操作 | 对应实践 / DoD |
| :--- | :--- | :--- |
| 1（首个动作） | 窗口的空 Style 内补 Padding Setter，值先用 `24,10`；ReadButton 添加资源引用并移除原 Padding。运行观察读取按钮内边距增大，清空按钮暂时不变。 | 实践 1/2 起步；DoD 1 部分 |
| 2 | 同一 Style 内补 HorizontalAlignment Setter，值 Left；移除 ReadButton 的同名属性，运行确认仍靠左。 | 实践 1/2；DoD 1 部分 |
| 3 | 控件内 ClearButton 用 `Style="{DynamicResource ActionButtonStyle}"` 引用同一窗口 Style，移除其 Padding/HorizontalAlignment，保留 Margin、Content、Command。先解释独立控件加载时机，避免初始化时找不到父窗口资源。 | 实践 2；DoD 1 |
| 4 | 两按钮接好后，关闭程序，将共同 Padding 改为 `32,14`，重新运行观察两按钮变化；再验证原业务行为。 | 实践 3；DoD 2/3 |

首个动作所需语法与各名称身份见 [WPF 样式 Q1](../questions/wpf-styles.md)。先运行未改的骨架观察原 `12,6`，再做 TODO 1 并重跑；每次修改 XAML 后关闭旧窗口再运行。工作区根目录命令：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

完成单步可反馈实际现象；完成全卡后回复“检查 TASK-015”。设计器预览和骨架构建不代替实际窗口证据。

2026-10-12 指导更正：初始骨架 TODO 3 注释让内部按钮直接使用 StaticResource，遗漏 UserControl 尚未连接窗口的加载时机。以本卡更新后的 TODO 3 和 [WPF 样式 Q2](../questions/wpf-styles.md) 为准；本轮保留学员代码和注释，先引导仅将 ClearButton 的资源引用改为 DynamicResource 并重跑。此更正保持窗口共用 Style 的目标及三个 DoD，不新增验收门槛。

不引入第三方主题库、隐式样式、BasedOn、Trigger、ControlTemplate、真实通信或异步采集；这些不属于本卡的必需验收范围。

## 验收标准（DoD）

- [x] 项目可编译并实际打开任务窗口；一个显式 Button Style 定义共同 Padding/HorizontalAlignment，读取与清空按钮均通过资源引用使用同一个 Style 对象，已提取的属性不再重复写在按钮上。
- [x] 修改一处共同 Padding Setter 后，实际运行的两个按钮都显示对应内边距变化；各自文字、位置间距及禁用状态正常，具备运行前后或等效实际布局证据。
- [x] 原输入/读取/历史/状态及清空命令绑定保持有效；连续读取与换 ID 保留旧记录，两种空白提示且不改变历史，空历史清空禁用、读取启用、清空禁用、再次读取恢复，有实际窗口运行证据。

## 可选巩固

- 对照具体文件区分样式资源与业务数据对象；不单设口试。
- 暂不要求全应用样式、多主题、模板或所有属性值优先级知识。

## 状态与证据

- 2026-10-12 最终审查：三个必需 DoD 全部通过，状态 ✅ 已通关。学员已将内部按钮改为 DynamicResource，并移除两按钮的直接 Padding/HorizontalAlignment；教练未代改学员源码，11 个相关文件 SHA256 前后一致。构建命令 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误，原 WpfBae.exe 已创建 Task015Window 并正常关闭退出 0。审查命令 `dotnet run --project docx/tasks/evidence/TASK-015/Review.csproj -- D:\host-computer-st\docx\tasks\evidence\TASK-015` 最终退出 0，9 个实际窗口场景与样式对比通过。首次审查工具混入旧程序集，已显式同步当前原程序集并核对哈希后重跑，不计学员失败；下方异常/启动记录为此前历史。

| 必需验收 | 结论 | 可见证据 |
| :--- | :--- | :--- |
| DoD 1：编译、实际打开、同一显式样式、无重复直接属性 | 通过 | 原程序启动退出正常；原编译 Task015Window 实际 Show，两个按钮 Style 与 Window.Resources 中对象 ReferenceEquals；同一 Padding/对齐 Setter，按钮本地对应属性为 UnsetValue。 |
| DoD 2：一处 Setter 变化同时影响两个按钮 | 通过 | 原窗口 `24,10` 时两按钮均 `98 × 37.24`；独立编译副本仅改变共同 Padding 为 `32,14`（另换命名空间以避免类型冲突），实际显示后两按钮均 `114 × 45.24`，样式仍同对象、文字/靠左/禁用状态正确。前后截图已查看，无裁切。 |
| DoD 3：原绑定和业务行为 | 通过 | TextBox.SetCurrentValue 保留输入绑定，ButtonAutomationPeer/IInvokeProvider 触发实际按钮命令；9 个场景全部通过，同一 ViewModel、原历史集合和两个命令均保留。 |

9 个场景：初始空历史禁用、首次读取启用、TEMP-001 连读两次、换 TEMP-002 保留三条、空字符串保护、纯空格保护、实际清空并禁用、带两侧空格 TEMP-003 恢复、再次清空禁用。两种空白均提示“该 Id 不合法”，没有集合变更；有效读数为当前 ID、26.5 °C。

清空按钮当前 Margin 为 0，仍在列表下方，实际布局无重叠或裁切；本卡 DoD 未指定固定 Margin 数值，不追加间距数值门槛。未发现新增典型认知错误，教练此前漏讲资源加载时机已另记教学纠正。

证据：[最终运行结果](./evidence/TASK-015/results.json)、[原程序启动](./evidence/TASK-015/original-startup.json)、[构建与源码/程序集核对](./evidence/TASK-015/verification.json)、[24,10 实际布局](./evidence/TASK-015/padding-24-10.png)、[32,14 副本实际布局](./evidence/TASK-015/padding-32-14.png)、[三条历史](./evidence/TASK-015/history-three-readings.png)。对比副本的 Setter 变化由教练为审查设置，不计学员已自主完成多主题或运行时切换能力。

按计划 Style / Template 继续推进，已验证共用外观与业务绑定保留，下一张只规划 [TASK-016：用 DataTemplate 展示结构化读取历史](./TASK-016.md)，状态 ⚪ 未开始；尚未准备源码或切换 App.xaml。不宣称 WPF 阶段整体通关，资源加载原理仍按原范围复习。

- 2026-10-12 资源异常答疑：学员已补两个 Setter 和两个按钮引用，实际报告内部 StaticResource 找不到 ActionButtonStyle。核对键名与入口一致，原编译 Task015Window 诊断重现相同异常；当前构建仍退出 0、0 警告/错误。独立诊断仅替换内存 XAML 为 DynamicResource，连接并显示诊断窗口后验证同一 Style 引用、Padding `24,10,24,10` 和 Left，命令退出 0，见 [诊断结果](./evidence/TASK-015/resource-lookup-diagnostic/results.json)。未修改学员源码，尚无修复后完整原窗口证据，三个 DoD 仍未勾选、状态 🟡 进行中。教练先前漏讲加载时机已纠正，不记学员典型错误。
- 2026-10-12 启动：已创建上述四个骨架文件并切换 App.xaml；实际运行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0、0 警告、0 错误。未运行实际窗口、未验收样式变化或业务场景，三个 DoD 均未勾选；状态 🟡 进行中。样式和资源基础讲解记录为待复习，已有代码/骨架不计自主掌握。下条为创建卡片时的历史记录。
- 2026-10-12：TASK-014 三个必需 DoD 和 9 个实际窗口场景全部通过后，按计划只创建本任务卡。状态 ⚪ 未开始；未创建 TASK-015 源码、未切换 App.xaml、未构建或验收本任务。阶段整体与先前待复习范围保持原状。
