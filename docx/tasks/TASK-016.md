# TASK-016：用 DataTemplate 展示结构化读取历史

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中 Style / Template；本卡先覆盖用于数据行展示的 DataTemplate，ControlTemplate 与数据图表尚未覆盖。
- 选择依据：TASK-015 的共享 Style、窗口资源引用与原业务行为已实际验证。当前历史列表依靠 Reading.ToString() 输出整行字符串，适合继续学习 Template 如何组合数据的显示方式；沿用已使用的 Binding 和历史集合，避免同时引入通信或第三方组件。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

历史列表目前把每条 Reading 直接转成一段字符串。设备监控界面通常需要把设备 ID、数值和单位分别展示，方便调整各部分间距。使用一个 DataTemplate 描述每条读数的界面，保留原 Readings 集合、读取/清空命令和 TASK-015 按钮样式。

## 开始前先解释

- Reading 是已有自定义读数类，DeviceId/Value/Unit 是它的公开属性；DataTemplate 是 WPF 提供的“一个数据对象如何显示”的界面描述，不是新读数对象，也不负责采集。
- ListBox.ItemsSource 决定显示哪些数据，ListBox.ItemTemplate 决定每个数据项如何显示。先用一条 Reading 与一行界面的对应关系说明，再展示最小语法。
- 模板里每一行的 Binding 数据来源是对应的 Reading，而非窗口 ReadingViewModel。因此 `{Binding DeviceId}` 在模板里读该条历史记录的 ID；窗口输入框同名 Binding 仍读窗口对象的 DeviceId。必须结合真实实例解释，不仅列定义。
- StackPanel 是已用过的布局容器；Orientation="Horizontal" 是 WPF 提供的排列方向属性，把这一行的三个文字控件从左到右排列。Margin 用于行内间距，先解释两值与四值含义，再要求调整。
- TextBlock.Text 的 Binding 沿用既有语法，分别读取 Reading 的三个属性；不要求修改 ToString、不引入 StringFormat、属性实时更新、ControlTemplate 或 Trigger。

## 用户实践与产出

1. 在历史 ListBox 的 ItemTemplate 中定义一个 DataTemplate，用横向 StackPanel 描述一行。
2. 放置三个 TextBlock，分别通过 OneWay Binding 显示 DeviceId、Value 和 Unit，使用必要间距使各部分清楚可读。
3. 实际读取不同 ID，确认每行显示自己的历史数据；后续输入变化不改旧行，新增与清空仍跟随原集合，按钮样式与禁用/恢复保持正常。

## 目录与开始约定

2026-10-12 用户明确开启 TASK-016。已确认目标目录不存在，创建专属目录 `WpfBae/Exercises/TASK-016-reading-datatemplate/`，实际文件如下：

- [Task016Window.xaml](../../WpfBae/Exercises/TASK-016-reading-datatemplate/Task016Window.xaml)：任务窗口、原输入/状态/命令绑定及 TASK-015 已完成的按钮样式。
- [Task016Window.xaml.cs](../../WpfBae/Exercises/TASK-016-reading-datatemplate/Task016Window.xaml.cs)：创建一个原 ReadingViewModel 实例并连接 DataContext。
- [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml)：保留原集合/清空绑定，提供 ItemTemplate、DataTemplate 与横向 StackPanel，固定文字作为可编译占位；核心字段绑定留给学员。
- [ReadingHistoryControl.xaml.cs](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml.cs)：仅初始化控件，沿用父窗口数据源。

复用 [ReadingViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[读取命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[清空命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs) 与 [Reading 定义](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)。TASK-010/013/014/015 学员源码未修改。共享入口例外：[App.xaml](../../WpfBae/App.xaml) 的 StartupUri 切到 Task016Window；项目默认包含新目录，不改项目文件或引入依赖。

| TODO | 实践步骤 / DoD | 具体动作 |
| :--- | :--- | :--- |
| 1（首个动作） | 步骤 1/2；DoD 1/2 的设备 ID 部分 | 在历史控件模板内，将唯一 TextBlock 的固定 Text 改为 `{Binding DeviceId, Mode=OneWay}`；读取 TEMP-001 再换 TEMP-002，观察两行各自保留 ID。 |
| 2 | 步骤 2；DoD 1/2 的数值与单位部分 | 再添加两个 TextBlock，分别绑定 Value、Unit。 |
| 3 | 步骤 2；DoD 2 的可读性 | 用 Margin 调整行内间距；两值是左右/上下，四值是左/上/右/下。 |
| 4 | 步骤 3；DoD 2/3 | 实际验证追加、换 ID、两种空白保护、清空禁用/恢复及原样式和绑定。 |

首步仅做 TODO 1。关闭旧程序，在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`；未改占位时，每次读取都显示“设备 ID 待绑定”，这是骨架预期现象。改绑定后应依次显示 TEMP-001、TEMP-002，输入框继续变化不改旧行。之后才补 TODO 2/3/4。必要讲解见 [WPF 数据模板 Q1](../questions/wpf-templates.md)。完成后提交“检查 TASK-016”，按原 DoD 核对；不以设计预览代替运行。

## 验收标准（DoD）

- [x] 项目编译并实际打开任务窗口；历史 ListBox 使用一个 DataTemplate，三个文字控件通过各自 Binding 显示每条 Reading 的 DeviceId、Value、Unit，显示路径不依靠拼接整行或调用 ToString。
- [x] 实际连续读取并换 ID 后，新增行显示当前读数，旧行保持原 ID/数值/单位；各部分文字可读且间距正常，ItemsSource 仍引用原 Readings。
- [x] 两种空白输入提示且不改变历史；实际清空后列表为空、清空禁用，再次读取恢复。窗口原输入/状态/命令绑定及两个按钮的共用样式保持有效，有实际运行证据。

## 可选巩固

- 对照具体文件区分 ItemTemplate、ItemsSource 和每一行的 Binding 来源；不单设口试。
- 暂不要求修改历史记录的属性后实时刷新，不默认 Reading 已支持属性变更通知。

## 状态与证据

- 2026-10-12 最终审查：学员自主为 DeviceId 的 TextBlock 添加 `Margin="0,0,12,0"`，三个必需 DoD 全部通过，状态 ✅ 已通关。重新执行 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误；`dotnet run --project docx/tasks/evidence/TASK-016/Review.csproj -- D:\host-computer-st\docx\tasks\evidence\TASK-016` 退出 0，当前原编译 Task016Window 实际打开，10 个场景的业务及显示检查全部通过，没有运行异常。

| DoD | 最终结论 | 证据 |
| :--- | :--- | :--- |
| 1 | 通过 | 单个 DataTemplate 的三个 TextBlock 分别以 OneWay Binding 读取 DeviceId/Value/Unit，状态全部 Active，数据源均为对应 Reading；原窗口实际显示 ID、26.5、°C，不拼接整行或调用 Reading.ToString。 |
| 2 | 通过 | TEMP-001 连读两次、换 TEMP-002 为第三条；仅改输入仍保留旧行，集合/读数对象引用保持；实际文字正确，ID→数值和数值→单位的布局间距均为 12，截图可读。 |
| 3 | 通过 | 空字符串/纯空格提示并保留原 3 条、无集合变更；清空为 0 条且禁用，带空格 TEMP-003 再读恢复 1 条并启用，再清空正确；窗口输入/状态/命令绑定、原集合与两按钮同一 Style 均保持。 |

最新证据：[10 场景与每行字段状态/位置](./evidence/TASK-016/results.json)、[构建及 35 个源码文件/程序集哈希核对](./evidence/TASK-016/verification.json)、[三条历史实际窗口截图](./evidence/TASK-016/history-three-readings.png)、[清空截图](./evidence/TASK-016/history-cleared.png)。35 个源码文件哈希前后一致，当前原程序集和审查引用程序集哈希一致，教练未代改源码。此前数值路径错误已自主修复，间距已自主修复验证，未新增典型错误。

掌握仅限提供的模板骨架中三项字段 Binding、同一历史记录的数据源应用及横向 Margin 调整；模板外层、窗口/控件定义、DataContext 与业务连接由教练提供，不计独立模板/控件设计。问答 Q1/Q2 按上述应用范围标为已掌握，资源/属性/委托等原复习范围保留。下一张只创建 [TASK-017](./TASK-017.md)（⚪ 未开始），继续计划 Style / Template 的 ControlTemplate；未准备源码或切换入口，不宣称 WPF 阶段完成。下方未通过表格及修复指引均为此前历史。

- 2026-10-12 同轮复查：发现学员已自主将 FallbackValue 改为 Value，未代改源码；保留 [修复前结果](./evidence/TASK-016/results-before-value-fix.json) 和 [修复前截图](./evidence/TASK-016/history-three-readings-before-value-fix.png)，重新执行同一构建/审查命令，均退出 0，构建 0 警告/错误。当前三个字段绑定全部 Active，分别正确显示每条 Reading 的 ID、26.5、°C；DoD 1/3 通过并勾选。DoD 2 仍未通过：第一个 TextBlock 的 Margin 为 0，数值控件只有右边距，实际 ID 右边缘为 x=85.8433，数值左边缘也为 x=85.8433，两者没有间距；实际截图显示 TEMP-00126.5 连在一起。10 个业务场景继续通过，7 个非空历史场景只剩此间距问题。状态仍 🟡 进行中，不生成下一任务。

当前首个动作：只在第一个（DeviceId）TextBlock 添加右边距，例如 `Margin="0,0,12,0"`，关闭旧窗口重跑，预期 ID 和 26.5 之间出现清楚空隙。完成后复查 DoD 2。最新证据为 [results.json](./evidence/TASK-016/results.json)、[verification.json](./evidence/TASK-016/verification.json) 和 [当前窗口截图](./evidence/TASK-016/history-three-readings.png)；下方表格与首次修复建议是修正 Value 前的历史结论。

- 2026-10-12 审查：状态仍为 🟡 进行中。实际 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误；`dotnet run --project docx/tasks/evidence/TASK-016/Review.csproj -- D:\host-computer-st\docx\tasks\evidence\TASK-016` 退出 0，审查程序打开当前原编译 Task016Window 并经输入控件/按钮自动化执行 10 个场景。退出 0 表示审查完成，不代表任务通关。当前原程序集与审查引用程序集哈希一致，35 个相关源码哈希前后一致，未代改学员源码。

| DoD | 审查结论 | 可见证据 |
| :--- | :--- | :--- |
| 1 | 未通过 | 窗口确实打开、DataTemplate 与三个 TextBlock 存在；DeviceId/Unit 绑定 Active，数值绑定路径却是 FallbackValue，Reading 没有此属性，实际状态 PathError、文字为空。 |
| 2 | 未通过 | 三条 Reading 正确保存 TEMP-001/TEMP-001/TEMP-002、26.5、°C；换 ID 和只改输入均保留旧记录，同集合连接正确。但行内数值没有显示，完整三字段展示未达标；设备 ID 未留右间距，修复数值后还需运行核对 ID 与数值间距。 |
| 3 | 通过 | 空字符串/纯空格提示“该 Id 不合法”，历史仍为原 3 条、没有集合变更；点击清空为 0 条并禁用；带空格 TEMP-003 再读后 Trim、列表恢复 1 条且清空启用，再次清空正确。输入/状态/命令绑定与两按钮同一 Style 均有效。 |

证据：[10 场景和每个字段的绑定状态/文字/布局](./evidence/TASK-016/results.json)、[构建及源码/程序集核对](./evidence/TASK-016/verification.json)、[三条历史实际窗口截图](./evidence/TASK-016/history-three-readings.png)、[清空截图](./evidence/TASK-016/history-cleared.png)。10 个场景业务检查通过，7 个含非空历史的场景均发现数值显示失败；不把空历史时没有失败解释为三个字段已通过。

首个修复动作：只核对历史控件第二个 TextBlock 的绑定属性名，与 Reading 的公开属性对应，将数值路径从 FallbackValue 改为 Value；重跑观察是否显示 26.5。之后将 ID 与数值隔开（例如在第一个 TextBlock 加右边距），再实际核对完整行。当前 `Margin="0, 0 12 0"` 已被 WPF 成功解析成 `0,0,12,0`，不是编译或启动错误；它留的是该控件右边空隙。相关原因见 [数据模板 Q2](../questions/wpf-templates.md)。未通关，不创建下一张任务卡。下方启动/规划记录为历史。

- 2026-10-12 启动：四个任务专属骨架文件与共享入口已准备，任务卡、索引和进度同步为 🟡 进行中。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0、0 警告/错误。未运行实际任务窗口或验收，三个 DoD 均未勾选；骨架与首步示例不计自主掌握。下条是此前仅规划时的历史记录。
- 2026-10-12：TASK-015 三个必需 DoD、9 个实际窗口场景与共用 Setter 布局对比全部通过后，按计划只生成本卡。状态 ⚪ 未开始；未准备 TASK-016 源码、未切换 App.xaml、未构建或验收本任务。
