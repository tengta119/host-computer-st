# TASK-022：用 DockPanel 组织设备读取窗口

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」明确列出的 Grid / StackPanel / DockPanel 与独立设备监控界面目标。
- 选择依据：TASK-021 已验证本骨架的 Grid 三行位置和表格随窗口增长；计划中的 DockPanel 尚无实践证据。保持相同设备读取场景，学习让顶部/底部区域靠窗口边缘，历史区域取得中间剩余空间，不加入通信、实时采集或新业务。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

继续使用两个字符串设备选项、三列只读历史表格和原读取/清空命令。在独立窗口里用 DockPanel 安排顶部选择/读取、底部清空/提示，让历史区域填满剩余空间，比较原高度与拉高后的布局。

只练 DockPanel 的区域摆放与内容顺序，不要求列布局、左右设备列表或新框架；保留 TASK-021 原作品。

## 开始前必须先解释

1. 指向真实的选择区、清空区、历史区：DockPanel 是 WPF 的摆放容器，DockPanel.Dock 设置具体区域靠这个容器的哪条边，不是表格数据属性。
2. 分别解释顶部区域 Dock=Top、底部区域 Dock=Bottom 的代码与效果；不把 DockPanel 的安排解释为 Grid.Row 的另一种写法。
3. 指向最后一个历史区域：LastChildFill 为 true 时最后一个子区域填满已经分配后剩余的空间，代码中子区域的顺序会影响结果。先说明规则，再要求安排顺序。
4. 历史区域仍需移除固定表格高度并取得有限窗口空间；保留标题/表格内部 Grid 与原 Binding，不新建另一份历史集合或数据对象。

## 用户实践与产出

1. 用户明确开启后，教练准备独立窗口，复用已完成业务连接，把 DockPanel 核心摆放留 TODO。先给一个可观察的单处摆放改动，记录具体文件、位置、运行方式和预期效果。
2. 学员安排顶部选择、底部清空与最后的历史区域，使中间表格填满剩余空间。
3. 实际拉高/恢复窗口，核对最小尺寸；通过实际设备选择和按钮读取/清空复核历史保持及恢复。

## 目录与开始约定

2026-10-13 用户明确开启；已核对 TASK-021 窗口/后台、原 ViewModel、源码树与运行入口，目标目录此前不存在。

专属目录：`WpfBae/Exercises/TASK-022-dockpanel-layout/`。

- 新建 [Task022Window.xaml](../../WpfBae/Exercises/TASK-022-dockpanel-layout/Task022Window.xaml)：主 DockPanel、三个区域、原下拉框/按钮/三列表格连接；核心布局留 TODO。
- 新建 [Task022Window.xaml.cs](../../WpfBae/Exercises/TASK-022-dockpanel-layout/Task022Window.xaml.cs)：为本窗口创建一份原 [ReadingViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)，提供 TEMP-001/TEMP-002 选项。各区域继承该对象，读取/清空使用其原集合；未在历史区另建数据对象。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml)仅将 StartupUri 切到本窗口；项目按现有 WPF 默认规则编译新文件，无新增依赖。
- 原 [TASK-021 窗口](../../WpfBae/Exercises/TASK-021-grid-layout/Task021Window.xaml)、[后台](../../WpfBae/Exercises/TASK-021-grid-layout/Task021Window.xaml.cs)与业务文件保留。

## 本次布局讲解与动手步骤

先读[布局 Q2](../questions/wpf-layout.md#q2task-022怎样让选择区靠顶部清空区靠底部历史区占中间)。其中逐处说明当前文件中的三个区域、Dock 设置、最后区域填满、内部 Grid 和固定表格高度；以下操作在理解对应说明后逐步做，不一次修改全部。

| TODO | 真实位置与操作 | 可观察结果 / 对应 DoD |
| :--- | :--- | :--- |
| 1（首步） | XAML 的 SelectionRegion：只把 DockPanel.Dock 从 Bottom 改为 Top | 整块选择区从底部移到顶部；认识靠边设置，属于 DoD 1 的起步观察 |
| 2 | XAML 的 ActionsRegion：按讲解把清空/提示区域安排到底边 | 清空按钮与状态文字一起靠底部；DoD 1 |
| 3 | 主 DockPanel：把 HistoryRegion 完整区域移到最后，再启用 LastChildFill | 历史区取得顶部/底部分配后剩余空间；DoD 1/2 |
| 4 | HistoryDataGrid：移除固定 Height=210，保留内层 Grid 行号与所有 Binding | 窗口拉高时表格可以一起变高；DoD 2 |
| 原业务复核（无新业务 TODO） | 全部布局完成后，两设备实际改选、连续读取、清空和再读 | 旧行保持、原集合追加、清空禁用和再读恢复；DoD 3 |

首步具体做法：先在工作区根目录运行未改骨架，确认标题为 TASK-022，观察选择区目前在底部；关闭窗口后只改 TODO 1，再以相同窗口尺寸运行比较。预期下拉框与读取按钮整块移到顶部。此时历史区/清空区仍是占位安排，首步不等于任务完成。

运行命令（供学员执行，本轮教练未执行）：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

完成后说“检查 TASK-022”；实际拉高、恢复及最小尺寸核对以运行窗口为准，设计预览和编译成功不代替运行证据。

## 验收标准（DoD）

- [x] 构建并实际打开 TASK-022 窗口，主容器为 DockPanel；选择/读取在顶部、清空/提示在底部、最后的历史区域填满中间，无重叠。
- [x] 实际拉高窗口后历史表格高度增加，顶部/底部按内容需要占高度；恢复原高度与最小尺寸时下拉框、三列、按钮和提示可读，无裁切，表格不受固定 Height 限制。
- [x] 两设备默认选择/改选读取、旧行保持、原集合连续追加、清空禁用和再次读取恢复实际通过，原数据对象/命令/Binding 连接保留。

## 2026-10-13 最终复查：三个必需 DoD 全部通过

学员已自主移除 ShowHistory 的外层容器，将主 DockPanel 的直接子区域改为 SelectionRegion、ActionsRegion、HistoryRegion，设置 LastChildFill=True，表格无固定 Height。原窗口实际打开、同一编译窗口的区域安排与伸展验证通过。

| 必需 DoD | 结果 | 实际证据 |
| :--- | :--- | :--- |
| 1：区域靠边与历史填满 | 通过 | 顶部选择、底部清空按内容占高度，最后的历史 Grid 填满中间，无重叠或越界 |
| 2：拉高/恢复/最小尺寸 | 通过 | 窗口高 640→840，表格高 369.2267→569.2267；顶部 85.72、底部 68.48 保持，恢复后表格回到 369.2267；最小 420×600 时表格 329.2267，三列/下拉框/按钮/提示可读 |
| 3：原设备选择/读取/清空 | 通过 | 13 个业务场景通过，改选写回、连续追加、旧行引用、原集合/对象/命令、清空禁用与再读恢复保持 |

原项目与审查工程构建退出 0、0 警告/错误；原 exe 打开 TASK-022 并正常关闭退出 0。17 个实际场景全通过、无运行异常，50 个源码哈希未变，审查引用 DLL 与原 DLL 一致。没有代改学员源码，也没有以设计预览代替实际运行。

当前证据：[结果](./evidence/TASK-022/results.json)、[核对](./evidence/TASK-022/verification.json)、[启动关闭](./evidence/TASK-022/launch.json)、[图像/方法说明](./evidence/TASK-022/README.md)。首次未通过的完整证据另存 [first-review](./evidence/TASK-022/first-review/README.md)，历史记录保留。

布局 Q2 按本骨架内靠边、子区域顺序/填满、移除外层容器和表格固定高度的自主应用证据标已掌握；完整测量机制、独立界面设计仍不一并认定。错题本本条已修复。按计划 Grid 与设备监控界面目标，只创建下一张 [TASK-023：用 Grid 列布局组织设备监控界面](./TASK-023.md)（⚪ 未开始），巩固左右空间分配；未创建源码或切换入口，不宣称 WPF 阶段整体完成。

## 2026-10-13 首次审查与修复动作（修复前历史）

任务保持 🟡 进行中；不创建下一任务，不代改学员源码。

| 必需 DoD | 结果 | 可见证据 |
| :--- | :--- | :--- |
| 1：DockPanel 顶部/底部与中间历史填满 | 未通过 | 原窗口实际打开，顶部选择/底部清空正确，无重叠；但 LastChildFill=False，直接子区域为 SelectionRegion、ShowHistory、ActionsRegion，历史未填满中间 |
| 2：窗口拉高/恢复与最小尺寸 | 未通过 | 表格已无固定 Height；窗口 640→840 时表格仍为 43.48，上下区高 85.72/68.48 保持；最小 420×600 三列、下拉框、按钮提示可读，但自动增长未实现 |
| 3：原设备选择/读取/清空 | 通过 | 13 个实际业务场景通过：默认/改选、连续追加、旧行引用保持、原集合清空、禁用及再读恢复、最小尺寸选择与显示均正确 |

原因定位：新增的 ShowHistory 是纵向 StackPanel，HistoryRegion 是它的内部内容，主 DockPanel 直接安排的是 ShowHistory；历史内部小 Grid 按内容高度取得空间。把 LastChildFill 单独设 True 时，当前最后的 ActionsRegion 会取得余下空间，也不能达到历史填满目标。

首个修复动作：在 [Task022Window.xaml](../../WpfBae/Exercises/TASK-022-dockpanel-layout/Task022Window.xaml) 中只移除 ShowHistory 外层 StackPanel 的开始/结束标签，保留内部 HistoryRegion 完整 Grid 和原代码。先让主 DockPanel 直接看到历史 Grid；这一步仍未完成填满，暂不以单步运行判通关。随后按[布局 Q2 的本次审查说明](../questions/wpf-layout.md)，调整三个直接子区域顺序和主容器 LastChildFill，再运行拉高比较。

首次验证资料：[结果](./evidence/TASK-022/first-review/results.json)、[源码/程序集核对](./evidence/TASK-022/first-review/verification.json)、[原 exe 启动关闭](./evidence/TASK-022/first-review/launch.json)、[运行方式与图像说明](./evidence/TASK-022/first-review/README.md)。原项目与审查工程均构建 0 警告/错误，原 exe 正常关闭退出 0；同一编译窗口实际显示并完成 17 个场景，13 通过、4 布局未通过，无运行异常。50 个源码哈希未变，原/审查引用 DLL 哈希相同。

## 状态与证据

- 2026-10-13 最终复查：三个必需 DoD 与 17 个实际场景全通过，状态 ✅ 已通关，证据见上。布局 Q2 按应用范围升级、错题修复；只创建 TASK-023 未开始卡片，App.xaml 保留 Task022Window。下方进行中/未验收/失败描述为对应时点历史。
- 2026-10-13 后续答疑：学员追问 Grid 写最后为何显示中间。重读新源码已见 ShowHistory 移除、直接子区域改为选择/清空/历史、LastChildFill=True，原业务连接保留；改动未代做。已按空间依次分配解释，并区分小 Grid 内部行号。未构建或重新运行，新代码待复查，DoD 1/2 未新增勾选、3 保留，状态仍进行中；下方首次审查的 False/外层容器/高度结果属于修改前证据。
- 2026-10-13 答疑：学员追问“最后一个直接子区域，再启用 LastChildFill”的含义。已按当前源码分步解释包含关系、完整 Grid 移动到 ActionsRegion 后/主 DockPanel 结束前，以及主容器 False→True；见布局 Q2。此前压缩说法已展开，属于教练表达改进。本轮未改源码、构建或运行，状态和已有 DoD 结果保留。
- 2026-10-13 首次审查：DoD 1/2 未通过、DoD 3 已通过并勾选；证据与首个修复见上。布局 Q2 保持待复习，原 Q1/其他已有能力证据保留；典型容器导致历史只按内容高度的问题记录至错题本，不推断新增容器的动机。
- 2026-10-13 启动：实际创建上述两个骨架文件并切换入口；执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0、0 警告/错误。未运行实际窗口或进行 DoD 验收；三个必需项均未勾选。骨架与讲解不计自主掌握证据，DockPanel Q2 为待复习；状态 🟡 进行中。下条仅创建卡片的描述为开启前历史。
- 2026-10-13：TASK-021 三个必需 DoD 与 17 个实际场景通过后，按计划剩余 DockPanel 范围只创建本卡。状态 ⚪ 未开始，未准备源码、未切换入口或运行；当前 App.xaml 保留 Task021Window，不认定 WPF 阶段整体完成。
