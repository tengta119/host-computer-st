# 学习进度：上位机开发

> 计划原文：[plan.md](./plan.md)（复制自 `D:\tmp\上位机.md`）｜已知背景：Java 后端；计划提及 Java、并发、网络、数据库经验，具体熟练度待补充｜总目标：迁移已有能力，逐步具备上位机界面、通信、设备控制、数据采集与稳定性开发能力。
> 当前阶段：第二阶段 WPF（起步）；第一阶段概念继续复习｜最近通关：TASK-022｜下一任务：TASK-023（⚪ 未开始）

## 阶段与路线概览

| 来源 | 学习目标与顺序 | 状态 |
| :--- | :--- | :--- |
| 计划「二、第一阶段：C# 基础」 | 对照 Java 学 C# 语法、面向对象、委托/事件、Lambda/LINQ、Task/async/await、CancellationToken、文件/JSON/配置；委托、事件、异步为重点。 | 🟡 进行中 |
| 计划「三、第二阶段：WPF」 | 依次接触 Window、控件、布局、XAML、Event、Binding、ObservableCollection、Command、MVVM、UserControl、Style/Template、数据图表；目标是能独立编写设备监控界面。 | 🟡 进行中 |

计划开头另有后续方向：串口/TCP → Modbus → 并发与设备通信 → 数据采集/监控 → 完整上位机项目 → 再补 C++/Qt/嵌入式；原文未为这些方向给出独立阶段、具体任务或验收细则，此处只记录路线，不分配阶段状态或预建任务。学习顺序和范围以[原文](./plan.md)为准。

## 任务进度

详见[任务索引](./tasks/README.md)。任务依计划与实际反馈逐张生成。

| 任务 | 阶段 | 状态 | 证据 / 备注 |
| :--- | :--- | :--- | :--- |
| [TASK-001：用 C# 表达设备数据模型](./tasks/TASK-001.md) | 第一阶段：C# 基础 | ✅ 已通关 | 2026-09-26 按用户调整后的验收范围通过；运行覆盖两条读数、设备不存在、设备无读数；查询由教练代做 |
| [TASK-002：用事件传递设备读数](./tasks/TASK-002.md) | 第一阶段：C# 基础 | ✅ 已通关 | 2026-09-27 复查：事件数据使用当前设备 ID；两次通知输出完整，取消订阅后无第三次处理器输出，三项 DoD 通过 |
| [TASK-003：用 Lambda 与 LINQ 筛选设备读数](./tasks/TASK-003.md) | 第一阶段：C# 基础 | ✅ 已通关 | 2026-09-27 运行输出仅保留设备 001 的 70 km，委托计数为 1；三项 DoD 通过 |
| [TASK-004：用 Task 与 async/await 模拟设备读取](./tasks/TASK-004.md) | 第一阶段：C# 基础 | ✅ 已通关 | 2026-09-29 最终运行输出完整读数与 `await` 结果，结合学员先前顺序说明，三项 DoD 通过；结果输出由教练代写，不计自主掌握 |
| [TASK-005：用 CancellationToken 取消模拟设备读取](./tasks/TASK-005.md) | 第一阶段：C# 基础 | ✅ 已通关 | 2026-10-05 复跑正常读数与取消输出通过；按用户明确要求采用协助修正说明完成任务级验收，取消原理仍待复习 |
| [TASK-006：用 JSON 文件保存和加载设备配置](./tasks/TASK-006.md) | 第一阶段：C# 基础 | ✅ 已通关 | 2026-10-05 最终复查三项 DoD 通过；3500 ms 配置实测 3565 ms，缺失/损坏均提示并退出 0，原文件保留；3 处可空声明警告可整理 |
| [TASK-007：用配置驱动可取消的连续采集](./tasks/TASK-007.md) | 第一阶段：C# 基础（巩固） | ✅ 已通关 | 2026-10-05 最终复查三项 DoD 通过；正常/修改配置、14 次长短取消、缺失/损坏均安全结束，文件内容不变；取消前已完成读数符合验收范围 |
| [TASK-008：用接口统一模拟设备读取](./tasks/TASK-008.md) | 第一阶段：C# 基础（面向对象巩固） | ✅ 已通关 | 2026-10-05 最终复查：压力单位自主修正为 kpa，实际两条读数输出正确，三项 DoD 全部通过；参数对照沿用同日已验证记录 |
| [TASK-009：用 WPF 窗口与按钮事件显示设备读数](./tasks/TASK-009.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-09 审查：构建、原应用启动、按钮自动化调用、不同 ID、空白及恢复、默认与最小尺寸布局均验证通过，三项 DoD 已勾选 |
| [TASK-010：用 Binding 更新模拟温度读数](./tasks/TASK-010.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-09 审查：编译与原应用启动、同一 DataContext、两个 OneWay Binding、五种输入场景和两项通知均验证通过，三项 DoD 已勾选 |
| [TASK-011：用 ObservableCollection 显示温度读取历史](./tasks/TASK-011.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-11 实际审查：构建 0 警告/错误，原窗口启动退出正常；列表绑定、连续追加、修改 ID、空白保护、清空与恢复均通过，同集合与通知已验证，三项 DoD 已勾选 |
| [TASK-012：用 Command 清空历史并控制按钮是否可用](./tasks/TASK-012.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-11 实际审查：构建 0 警告/错误、原窗口启动退出正常；8 个场景验证同对象/集合、绑定、按钮条件切换、点击清空、恢复与空白保护，三项 DoD 通过 |
| [TASK-013：把读取逻辑从窗口移到 ViewModel](./tasks/TASK-013.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-12 自主补上空白分支 return 后，重新构建和 10 个实际窗口场景全部通过，三个 DoD 已勾选 |
| [TASK-014：用 UserControl 封装读取历史区域](./tasks/TASK-014.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-12 构建 0 警告/错误、原程序启动退出正常；9 个实际窗口场景验证控件封装、同对象/集合绑定、追加、空白保护与清空恢复，三个 DoD 已勾选 |
| [TASK-015：用 Style 统一读取与清空按钮的外观](./tasks/TASK-015.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-12 构建/原程序启动通过；同一 Style、独立副本共同 Setter 布局变化、9 个实际窗口业务场景通过，三个 DoD 已勾选，源码未代改 |
| [TASK-016：用 DataTemplate 展示结构化读取历史](./tasks/TASK-016.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-12 自主修正数值路径及 ID 右边距后，构建 0 警告/错误、10 个实际窗口场景显示/业务全通过，三个 DoD 已勾选，源码未代改 |
| [TASK-017：用 ControlTemplate 定义按钮内部外观](./tasks/TASK-017.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-12 构建 0 警告/错误、原应用启动退出 0；11 个实际窗口场景和 Padding 临时属性诊断通过，三个 DoD 已勾选，源码未代改 |
| [TASK-018：用 Canvas 与 Polyline 绘制模拟温度折线](./tasks/TASK-018.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-13 原窗口与独立 28 度副本实际验证三项 DoD；中间点 y=110→80、端点不变，标签可读，41 个原源码哈希不变 |
| [TASK-019：用 DataGrid 展示设备读数列表](./tasks/TASK-019.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-13 构建 0 警告/错误，原程序启动关闭正常，11 个实际窗口场景全通过，43 个源码哈希不变 |
| [TASK-020：用 ComboBox 选择设备并读取温度](./tasks/TASK-020.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-13构建0警告/错误，原exe正常启动关闭，13个实际窗口场景通过，SelectedItem默认双向写回有效，45个源码哈希不变 |
| [TASK-021：用 Grid 组织设备读取窗口](./tasks/TASK-021.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-13 构建 0 警告/错误、原程序启动关闭退出 0；17 个实际场景全通过，拉高表格等量增长，最小尺寸/原业务正确，48 个源码哈希未变 |
| [TASK-022：用 DockPanel 组织设备读取窗口](./tasks/TASK-022.md) | 第二阶段：WPF（起步） | ✅ 已通关 | 2026-10-13 最终复查17个实际场景全通过，构建0警告/错误；表格随拉高200等量增长、最小尺寸/原业务正确，50个源码哈希未变 |
| [TASK-023：用 Grid 列布局组织设备监控界面](./tasks/TASK-023.md) | 第二阶段：WPF（起步） | ⚪ 未开始 | 计划Grid与设备监控界面目标的左右列布局巩固；仅创建卡片，未准备源码或切换入口 |

## 🟡 正在学习

- 2026-10-13 TASK-022 最终复查通关：学员已移除 ShowHistory，按选择/清空/历史直接子区域顺序安排主 DockPanel，LastChildFill=True，表格无固定 Height。原项目/审查工程构建 0 警告/错误，原 exe 打开 TASK-022 正常关闭退出 0；同一编译窗口实际显示并执行17个场景全部通过。窗口高640→840，表格369.2267→569.2267，顶部85.72/底部68.48保持；恢复后表格回原高度，最小420×600时表格329.2267，三列/设备/按钮/提示可读。13个业务场景验证选择写回、原集合连续追加/旧行引用、清空禁用与再读恢复。50个源码哈希未变、原/审查引用DLL相同，源码未代改。证据见[结果](./tasks/evidence/TASK-022/results.json)与[核对](./tasks/evidence/TASK-022/verification.json)；首次失败资料已保留在[first-review](./tasks/evidence/TASK-022/first-review/README.md)。三个DoD全勾选，布局Q2限本骨架的靠边/顺序/填满与表格伸展应用标已掌握，错题本本条修复；当前50条问答，待复习29、已掌握21。依据计划Grid和独立设备监控界面目标，只创建[TASK-023](./tasks/TASK-023.md)（⚪ 未开始），巩固尚无证据的左右列布局与组合，未准备源码/切换入口。当前无进行中实践任务，App.xaml保留Task022Window；列布局、独立完整界面、图表机制与C#原复习范围保留，不宣称WPF阶段整体完成。下方失败/待复查/进行中描述为此前历史。

- 2026-10-13 TASK-022 代码顺序与画面位置追问：学员询问历史 Grid 写最后为何显示中间。重读当前源码，已见自主移除 ShowHistory、选择/清空/历史直接子区域顺序、LastChildFill=True；先保留这些改动。按“先取顶边、再取底边、最后取得夹在两者之间的剩余空间”逐块解释，区分外层区域位置和小 Grid 内部标题/表格行号，记录至[布局 Q2](./questions/wpf-layout.md)。本轮仅答疑/同步文档，未代改源码、构建或重新运行；历史首次审查结果保留，新源码待复查，TASK-022/DoD/Q2 状态不变，不新增错题或掌握。

- 2026-10-13 TASK-022 布局用语追问：学员询问“最后一个直接子区域，再启用 LastChildFill”的意思。已重读当前 XAML，拆成当前嵌套关系、完整区域在文件中的移动位置、主容器 False→True 三步，以包含图和省略内部内容的结构示意说明“最后”指代码顺序、历史仍占中间。此前一句压缩多个动作是教练表达需改进，不新增错题或撤销已有能力证据；记录至[布局 Q2](./questions/wpf-layout.md)。本轮只答疑/同步文档，未改源码、构建或运行验收；TASK-022 保持 🟡 进行中，DoD 1/2 未通过、3 已通过，Q2 待复习，问答数量/统计不变。

- 2026-10-13 TASK-022 首次审查：原项目构建 0 警告/错误、原 exe 实际打开正常关闭退出 0，同一编译 Task022Window 显示并执行 17 个实际场景，无运行异常。13 个业务场景全通过，设备改选/连续追加/旧行引用/原集合清空禁用与再读恢复、最小尺寸可读性保持；DoD 3 已勾选。4 个布局场景未通过：LastChildFill=False，直接子区域顺序 SelectionRegion、ShowHistory、ActionsRegion；HistoryRegion 被新增纵向 StackPanel 包裹，窗口高 640→840 时表格仍 43.48，历史未填满余下空间。顶部/底部高 85.72/68.48 保持，最小 420×600 可读；DoD 1/2 未勾选。50 个源码哈希未变、审查与原 DLL 相同。证据见[结果](./tasks/evidence/TASK-022/first-review/results.json)与[核对](./tasks/evidence/TASK-022/first-review/verification.json)。首个修复仅去除 ShowHistory 的外层标签，保留内部 Grid；后续再处理顺序/LastChildFill，解释写入[布局 Q2](./questions/wpf-layout.md)并记录[错题本](./mistakes.md)。Q2 仍待复习，原技能证据保留，未代改源码或创建下一任务；TASK-022 保持 🟡 进行中。下方未运行/未验收描述为此前历史。

- 2026-10-13 TASK-022 启动：用户明确开启；核对当前源码/入口后新建 `WpfBae/Exercises/TASK-022-dockpanel-layout/` 两个窗口文件，App.xaml 切到 Task022Window。沿用原设备选项、ReadingViewModel、命令与三列表格，旧作品/业务文件未改。靠边位置、最后子区域顺序/填满设置、去掉固定表格高度留 TODO；首步只把 SelectionRegion 的 DockPanel.Dock 从 Bottom 改为 Top，重新运行观察整块选择区移动。具体对象与代码效果写入[布局 Q2](./questions/wpf-layout.md)（待复习），原 Q1 已验证范围保留；当前主问答 50 条，待复习 30、已掌握 20。实际 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误；未运行窗口或验收，三个 DoD 未勾选，不新增自主掌握或错题。状态 🟡 进行中；下方 TASK-022 未开始/旧入口描述为此前历史。

- 2026-10-13 TASK-021 审查通关：三个必需 DoD 和 17 个实际窗口场景全部通过。学员已配置 Auto / * / Auto，区域分别位于 0/1/2 行并去除固定表格高度。原项目构建 0 警告/错误、原 exe 打开 TASK-021 并正常关闭退出 0；同一编译窗口实际显示，设备选择和按钮自动化核对通过。窗口高 640→840，表格高 369.2267→569.2267；顶部行 85.72、底部行 68.48 保持，恢复高度和最小 420×600 可读性通过。两设备选择/读取、原集合连续追加、旧行保持、清空禁用和恢复均正确，48 个源码哈希未变、审查程序集与原 DLL 一致。证据见[结果](./tasks/evidence/TASK-021/results.json)与[核对](./tasks/evidence/TASK-021/verification.json)。首次工具漏计文字 Margin 的误报已修正，不记学员错题；骨架旧提示/注释不扩大验收。布局 Q1 按本骨架三行配置与去除固定高度的应用范围标已掌握，完整机制/独立设计仍复习；问答当前待复习 29、已掌握 20。按计划剩余 DockPanel 仅创建 [TASK-022](./tasks/TASK-022.md)（⚪ 未开始），未准备源码或切换入口；当前无进行中实践任务，App.xaml 保留 Task021Window，不宣称阶段整体完成。下方 TASK-021 启动/未验收/无源码描述均为此前历史。

- 2026-10-13 TASK-021 启动：用户明确开启，目标目录此前不存在；已创建 `WpfBae/Exercises/TASK-021-grid-layout/` 的 XAML/后台两个文件，App.xaml 切到 Task021Window。复用原设备选择、ReadingViewModel、读取/清空命令与三列表格，原作品未改。主 Grid 三个区域先用安全占位，核心行高/区域位置/移除固定表格高度留 TODO；历史区内标题/表格小 Grid 由教练提供，不计自主设计。首步只改第 0 行 Height 160→190，观察选择区下方空隙和清空区位置；Auto、星号行、行号、固定表格高度与内外两个 Grid 的关系增量写入[布局 Q1](./questions/wpf-layout.md)，仍待复习。实际 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误；未运行窗口或验收，三个 DoD 未勾选。状态 🟡 进行中，不新增自主掌握或错题。下方无进行中/仅创建 TASK-021/旧入口描述为此前历史。

- 2026-10-13 TASK-020审查通关：三个必需DoD和13个实际窗口场景通过，构建0警告/错误；原exe实际打开TASK-020、正常关闭退出0。下拉框默认TEMP-001，实际展开两个可见字符串选项、不允许自由编辑；学员SelectedItem绑定Mode=Default，框架BindsTwoWayByDefault=True，实际改选TEMP-002写回原DeviceId，读取/连续追加/切回001/旧行保持/清空禁用与002恢复/最小尺寸可读均通过。原数据对象、Readings和命令引用保持，三列从对应Reading取值，45个源码SHA256未变，审查程序集与原DLL相同，证据见[结果](./tasks/evidence/TASK-020/results.json)与[核对](./tasks/evidence/TASK-020/verification.json)。仅骨架内选中值连接及效果按自主证据计入[下拉框Q1](./questions/wpf-combobox.md)，默认框架机制不一并认定掌握。骨架旧初始提示和首次审查工具编译错误不记学员错题。本次只创建[TASK-021](./tasks/TASK-021.md)未开始卡片：依据计划Grid与[布局Q1](./questions/wpf-layout.md)仍待复习，学习三行分配与窗口变化；未准备源码、未切换入口，WPF阶段不宣称整体完成。下方TASK-020进行中/未连接/未运行是此前历史。
- 2026-10-13 问答库重构：按用户要求，依据当前 AGENTS.md 重写 `docx/questions/` 全部22个分类、49条主问答及原有追问，更新[问答索引](./questions/README.md)。回答从真实对象/现象、少量代码、逐处白话说明到可观察结果展开，完整语法先于简写；历史证据与掌握边界移到文末，重复解释合并。保留原Q标题/编号与状态（待复习31、已掌握18），完整23份原稿另存[备份目录说明](./questions-backups/before-plain-rewrite-2026-10-13/manifest.md)。修正失效的独立Device.cs/Reading.cs链接与历史WpfPractice拟用路径，原任务状态/DoD/证据/错题未改；本轮未运行示例或重新验收。整理期间发现TASK-020的Width已为260，保留工作区修改，SelectedItem仍未连接，不能把源码存在当运行验证或新增掌握。文档写入阶段520个受保护文件哈希一致，问答标题/状态/追问、本地链接和代码块格式检查通过，详见[核对结果](./questions-backups/before-plain-rewrite-2026-10-13/verification.json)。

- 2026-10-13 TASK-020 启动：用户明确开启，目标目录此前不存在；已创建 `WpfBae/Exercises/TASK-020-device-combobox/` 的 XAML/后台两个文件，App.xaml 切到 Task020Window。复用原 ReadingViewModel、两个命令与 TASK-019 已完成表格连接，原文件未修改；两个字符串数组/ItemsSource/DataContext 由骨架提供，SelectedItem 双向连接 DeviceId 留 TODO。首步只把 Width 从 200 改为 260，实际运行观察变宽；先具体解释选项来源、当前选择、同一个对象写回和禁止自由输入，详见 [设备下拉框 Q1](./questions/wpf-combobox.md)（待复习）。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0、0 警告/错误；未运行窗口或验收，三个 DoD 均未勾选，不新增自主掌握/错题。下方 TASK-020 未开始/仅创建卡片的描述为此前历史。

- 2026-10-13 TASK-019 审查通关：构建 0 警告/错误，原 exe 打开 TASK-019、关闭退出 0；原编译窗口 Show 后 11 个场景全通过。恰好三列，单元格绑定 Active/DataItem 为各条 Reading，连续/换 ID 追加、只改输入保留旧行、两种空白不改变历史、清空禁用与再次读取恢复、最小尺寸均验证，BeginEdit 返回 false。审查程序集与原 DLL 一致，43 个源码哈希不变；三个 DoD 全勾选，详见 [结果](./tasks/evidence/TASK-019/results.json)与[核对](./tasks/evidence/TASK-019/verification.json)。学员自主应用限骨架内 Header、ItemsSource 和三列字段连接，[读数表格 Q1](./questions/wpf-datagrid.md) 按该范围标已掌握。骨架旧初始提示不记学员错误；不扩大为框架机制/编辑/排序掌握。依据计划明确列出的 ComboBox 尚无自主实践证据，只建立 TASK-020（⚪ 未开始），不切换入口或准备源码；下方 TASK-019 进行中/未运行记录为此前历史。

- 2026-10-13 TASK-019 启动：用户明确开启，确认专属目录此前不存在后，新建 `WpfBae/Exercises/TASK-019-reading-datagrid/` 的 XAML/后台两个文件，共享 App.xaml 切到 Task019Window。复用原 ReadingViewModel、两个已完成命令与 Reading，不修改学员原业务文件。窗口连接、输入/命令与三列只读设置由骨架提供，整集合 ItemsSource 与逐行字段 Binding 留 TODO。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0、0 警告/错误；未运行窗口或验收，DoD 均未勾选。首步只把 Header 改为“设备 ID”，运行观察表头；讲解见 [读数表格 Q1](./questions/wpf-datagrid.md)（待复习）。不新增自主掌握/错题，不扩大此前实践证据；下方仅创建卡片与保留旧入口的描述为此前历史。

- 2026-10-13 TASK-018 审查通关：原项目和独立 28 度副本均构建 0 警告/错误；原程序与副本 exe 实际打开 TASK-018 窗口、关闭退出 0，审查程序实际 Show() 原编译窗口并检查两份内容图像。原坐标 (100,160)/(280,110)/(460,140)，副本只改中间样例为 28 后坐标 (100,160)/(280,80)/(460,140)，中间上移 30、两端不变、标签更新且可读，三个 DoD 已勾选。41 个原源码哈希不变，副本文本唯一改动为数组中间值；证据见 [TASK-018](./tasks/TASK-018.md)。工具首次标签中心容差过严已修正并重跑，不记学员错误。只创建下一张 TASK-019（⚪ 未开始），原入口保留 TASK-018；本题函数换算/标签布局等完整机制继续复习，不认定阶段整体完成。下方进行中/未运行记录为此前历史。

- 2026-10-13 TASK-018 标签位置答疑：学员询问折线点为什么与三个 TextBlock 对应，已核对当前 XAML/后台源码，按实际坐标讲解 `标签中心 = Canvas.Left + Width / 2`：55/235/415 加半宽 45 得到 100/280/460，与点的 x 相同；Top=260 固定放在绘图区下方，TextAlignment 只控制文字在框内居中。说明这些位置由骨架手动安排、名字没有自动匹配关系；记录至 [WPF 坐标与折线 Q2](./questions/wpf-charts.md)（待复习）。本轮只答疑与文档同步，未代改源码、构建或运行验收，TASK-018 保持 🟡 进行中、DoD 未新增勾选，不新增掌握或错题。

- 2026-10-13 TASK-018 启动：用户明确要求开启，目标目录此前不存在，已创建 `WpfBae/Exercises/TASK-018-temperature-polyline/` 下 XAML/后台两个文件，共享 App.xaml 切到 Task018Window。固定温度数组、绘图区/标签/占位线由教练提供；转换与循环绘图核心留 TODO。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0、0 警告/错误；未运行实际窗口，三个 DoD 均未勾选。首步只将占位中间点 y 从 180 改为 100，重新运行观察上移；用具体对象和少量代码先讲坐标方向，再分步说明温度换算，记录至 [WPF 坐标与折线 Q1](./questions/wpf-charts.md)（待复习）。未新增自主掌握或错题，不改变原阶段/机制复习范围；下方 TASK-018 未开始描述为此前历史。

- 2026-10-13 全部笔记重写：按用户明确要求，已按 AGENTS.md 中“具体对象与效果优先、禁止抽象定义套定义、一次讲清一个关系”的规则，重写 [docx/notes 索引](./notes/README.md)及 TASK-009～017 九篇笔记。正文从真实操作开始，结合少量代码逐处解释来源、对象和效果，历史验收/哈希/掌握边界移到文末；保留文件名和原任务卡/问答入口。重写前十份旧稿已单独备份到 `docx/notes-backups/before-plain-rewrite-2026-10-13/`。本轮只改学习文档，未修改源码、切换入口、重新运行或新增验收；既有任务/DoD/问答掌握范围与 TASK-018 未开始状态保留。

- 2026-10-13 笔记表达反馈：学员明确表示知识点难以读下去，指出“样式应用对象类型”“模板所应用的控件类型”含义不清。已重写 [TASK-017 笔记第 6 点](./notes/TASK-017-wpf-controltemplate.md)，并合并到[控件模板 Q1](./questions/wpf-controltemplates.md)：先用读取/清空两个真实 Button 说明类与具体按钮，再将 TargetType 解释为“这组设置/这份内部布局给 Button 按钮用”，分别对应 Padding 设置和 Border/文字显示。后续讲解和笔记先说具体对象、用途与代码对应，避免用抽象名词互相解释，必要概念分步展开，不在一个句子中追加多个未解释机制。此反馈属于教练表达问题，不记错题、不撤销已验证应用；未改源码或运行，任务、DoD 和问答掌握范围不变。

- 2026-10-13 TASK-017 知识整理：已按用户要求保存[详细笔记](./notes/TASK-017-wpf-controltemplate.md)，展开 Style/Template/DataTemplate/UserControl 分工、Template Setter/Setter.Value、Border 与圆角/颜色、ContentPresenter 默认和显式连接、TemplateBinding 来源/单向取值、Button→Border.Padding、共享模板与内部实例/名称范围、Trigger/Opacity 及命令禁用恢复链。已读取本地结果/核对/原启动记录和审查源码，查看六张历史内容渲染；历史 39 项源码 SHA256 与当前全部一致，包括 App.xaml。区分本题原窗口临时 Padding 诊断与 TASK-015 独立副本对比，保留 Content 显式写法可选的教学更正。本轮仅整理文档和静态核对，未改源码、切换入口或重新运行；TASK-017 保持 ✅ 已通关，Q3/Q4 仅保留骨架中属性连接/禁用规则应用已掌握范围，Q1/Q2 继续待复习，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 TASK-016 知识整理：已按用户要求保存[详细笔记](./notes/TASK-016-wpf-datatemplate.md)，展开 Reading/get/init/对象初始化、原集合、ItemsSource 与 ItemTemplate、属性元素语法、每行 Reading 数据源与同名 DeviceId、字段路径/OneWay、横向布局与 Margin、FallbackValue 路径和绑定配置区别、数值/间距两次自主修复及通知分工。已读取本地三版结果/验收记录/审查源码并查看四张历史内容渲染；历史哈希清单 35 项与当前对照，34 项一致，仅 App.xaml 因入口推进 TASK-017 不同。本轮仅整理文档和静态核对历史材料，未改源码、切换入口或重新运行；TASK-016 保持 ✅ 已通关，Q1/Q2 保留已有骨架中的字段绑定/逐条数据源和路径修正/Margin 应用掌握范围，机制复习不扩大，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 TASK-015 知识整理：已按用户要求保存[详细笔记](./notes/TASK-015-wpf-styles.md)，展开 Padding/Thickness/对齐、Style/Setter/TargetType、资源键与作用域、StaticResource/DynamicResource、内部控件初始化与资源查找时机、直接属性优先级、同一 Style 和原业务绑定。已读取本地验收 JSON/审查源码并查看三张历史内容渲染；历史列出的十个任务/业务文件 SHA256 与当前一致，App.xaml 已推进到 TASK-017；区分原源码 `24,10` 与独立审查副本 `32,14` 的布局对比。本轮仅整理文档和静态核对历史材料，未改源码、切换入口或重新运行；TASK-015 保持 ✅ 已通关，样式 Q1 仅保留已有应用范围，资源机制 Q2 继续待复习，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 TASK-014 知识整理：已按用户要求保存[详细笔记](./notes/TASK-014-wpf-usercontrol.md)，展开 UserControl/窗口与区域封装、复用类与新建实例、x:Class/partial/InitializeComponent、xmlns 前缀与嵌入标签、DataContext 同对象继承、类继承与属性值继承、ItemsSource/Command/getter、x:Name/名称范围、布局参数及读取/清空通知链。已读取本地 TASK-014 验收 JSON/审查源码并查看两张历史内容渲染；八个任务/业务文件 SHA256 与历史核对记录一致，App.xaml 已推进到 TASK-017。仅整理文档和静态核对历史材料，未改源码、切换入口或重新运行；TASK-014 保持 ✅ 已通关，掌握限已有骨架中的界面组合与绑定应用，UserControl Q1 机制继续待复习，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 TASK-013 知识整理：已按用户要求保存[详细笔记](./notes/TASK-013-wpf-viewmodel.md)，展开 MVVM 角色与源码分工、同一 ViewModel/集合/双命令、DeviceId 完整属性、TwoWay、UpdateSourceTrigger 与 PropertyChanged 区别、输入写回与对象通知、读取 Execute、局部 Trim、空白 return 的历史自主修复、历史读数与当前输入、中间/最终提示及通知时序。注明当前入口仍是 TASK-017、原 TASK-013 证据目录缺失，历史通过依据任务卡和错题本。本轮仅整理文档与静态核对，未改源码、切换入口或重新运行；TASK-013 保持 ✅ 已通关，已掌握限骨架上的读取逻辑，ViewModel Q1 及机制继续待复习，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 TASK-012 知识整理：已按用户要求保存[详细笔记](./notes/TASK-012-wpf-commands.md)，展开 ICommand/实现类、状态与命令构造、同一集合、完整 getter、Command/OneWay、CanExecute/Execute、防护检查、object?/null、委托/EventHandler/event、订阅与触发、?.Invoke、两个 this/sender、WPF 重查条件链及三种通知分工。注明当前入口仍是 TASK-017、原 TASK-012 证据目录缺失，历史通过依据任务卡。本轮仅整理文档与静态核对，未修改学员源码、切换入口或重新运行；TASK-012 保持 ✅ 已通关，命令 Q1 限已验证的条件/动作应用，委托与框架订阅继续待复习，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 TASK-011 知识整理：已按用户要求保存[详细笔记](./notes/TASK-011-wpf-reading-history.md)，展开 ObservableCollection/泛型、复用 Reading、状态对象/集合/元素三层关系、完整 getter 与只读属性、DataContext/ItemsSource、Add/Clear/Count、CollectionChanged 与属性通知边界、空白保留历史和直接状态提示。注明当前入口仍是 TASK-017、原 TASK-011 两份验收 JSON 缺失，历史通过依据任务卡。本轮仅整理文档与静态核对，未修改学员源码、切换入口或重新运行；TASK-011 保持 ✅ 已通关，集合 Q1 的应用范围和 Q2 等待复习状态不变，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 TASK-010 知识整理：已按用户要求保存[详细笔记](./notes/TASK-010-wpf-binding.md)，展开自定义数据类、字段/属性/get/set/value、new()/readonly、DataContext 与同一实例、Binding.Path/OneWay、INotifyPropertyChanged、委托/事件/参数类、nameof/?.Invoke、通知时序及输入分支。三个任务源码 SHA256 与历史审查记录一致；当前入口仍为 TASK-017，原 TASK-010 验收 JSON 缺失已注明。仅整理文档与静态核对，未修改源码、切换入口或重新运行；TASK-010 保持 ✅ 已通关，绑定 Q1 的应用范围及其他待复习状态不变，TASK-018 保持 ⚪ 未开始。[笔记索引](./notes/README.md)已追加入口。

- 2026-10-13 知识整理：按用户要求，将 TASK-009 的项目入口、XAML/后台连接、基础控件、StackPanel/Grid 与间距、Click 订阅、输入校验、Reading/属性/对象初始化及显示流程详细写入[知识笔记](./notes/TASK-009-wpf-window-events.md)，并建立[笔记索引](./notes/README.md)。注明当前 App.xaml 指向 TASK-017，TASK-009 原验收 JSON/图片在当前工作区缺失，历史通过结论引用任务卡。本轮仅整理文档，未修改学员源码、切换入口或重新运行；TASK-009 保持 ✅ 已通关，布局/属性/事件等原待复习范围不升级，TASK-018 保持 ⚪ 未开始。

- 当前无进行中的实践任务。[TASK-019](./tasks/TASK-019.md) 已于 2026-10-13 审查通关，App.xaml 保留 Task019Window；下一张 [TASK-020](./tasks/TASK-020.md)（⚪ 未开始）只规划 ComboBox 选择设备，未准备源码。原任务作品保留，C#/WPF 机制复习范围及两个阶段整体状态不变。下方启动/答疑记录为此前历史，不表示当前任务状态。
- 2026-10-12 TODO 4 答疑：已补 [控件模板 Q4](./questions/wpf-controltemplates.md)（待复习），先解释 Trigger 是属性条件生效期间的规则、Opacity 是 0～1 不透明度，再逐项说明 Button.IsEnabled 条件、自定义 ButtonBorder 名称与 Border.Opacity 设置及恢复机制。给出仅添加一条规则的操作和初始/读取/清空/恢复观察步骤，不引入动画。重读代码已见学员 Border.Padding 的 TemplateBinding，尚未见 Trigger；未代改、构建或运行，任务继续进行中，DoD/掌握范围不变。
- 2026-10-12 TODO 3 答疑：已新增 [控件模板 Q3](./questions/wpf-controltemplates.md)（待复习），先解释 Padding 是文字与边框之间的内部留白，分别指明共同 Setter 设置 Button.Padding、固定值设置 Border.Padding。第一步只把 Border.Padding 改为 TemplateBinding；当前原值相同，预期外观不变，再单独调整 Setter 观察两按钮一起变化。当前仍见固定 Padding，未代改/构建/运行，TASK-017 与 DoD 保持原状。
- 2026-10-12 TODO 2 默认连接更正：学员报告删除显式 Content TemplateBinding 仍能显示按钮原内容，官方文档确认当前 ContentPresenter 默认连接 Button.Content。原“必写”教学要求不必要，已在任务卡改为可选；验收仍看实际内容显示，其他 DoD 不变。当前代码已见学员显式连接；本轮未代改/构建/运行，不升级掌握或勾选 DoD。[控件模板 Q2](./questions/wpf-controltemplates.md) 已去重补充。后续优先用实际缺少连接的 Padding 说明 TemplateBinding，说明框架默认行为与手动配置的区别，不用冗余修改当功能门槛。
- 2026-10-12 TODO 2 基础追问：学员仍不理解“内部显示控件要展示的内容”，已增量补充至 [控件模板 Q2](./questions/wpf-controltemplates.md)。用实际“读取温度”四个字解释 Content，用 Border 的背景/边框与 ContentPresenter 的文字显示分工解释内部结构，再说明固定文字和取各按钮文字的区别；后续避免用抽象术语互相解释。此轮只答疑，不追加动手要求，未改或运行源码，状态/DoD/掌握范围不变，不记学员错题。
- 2026-10-12 TODO 2 答疑：学员明确反馈操作含义不清楚，已补 [控件模板 Q2](./questions/wpf-controltemplates.md)（待复习）。先分别说明 Button.Content 与 ContentPresenter.Content，再逐侧解释 TemplateBinding，并给出唯一改动及实际运行预期；骨架默认显示已存在，显式连接后通常外观不变。后续先解释新语法的属性所属对象，再要求使用，不把注释视为已教会；教学遗漏不记学员错误。当前见 CornerRadius=12，尚未见显式内容连接，未运行或代改，TASK-017 仍进行中，DoD 未勾选。
- [TASK-017](./tasks/TASK-017.md) 于 2026-10-12 按用户“开启task17”启动，状态 🟡 进行中。专属目录 `WpfBae/Exercises/TASK-017-button-controltemplate/` 四个窗口/历史控件骨架文件已准备，App.xaml 指向 Task017Window。复用 TASK-013 原业务和 TASK-016 已验收三字段历史显示；核心模板属性连接和禁用外观留作 TODO，原作品未修改。实际构建 0 警告/错误；未运行窗口或验收，DoD 均未勾选。首步只把 Border.CornerRadius 从 0 改为 12 并观察两个按钮；数据图表及原机制复习范围保留，两个阶段整体状态不变。

## ✅ 已掌握

- TASK-022 中学员在既有窗口骨架内自主设置选择区 Dock=Top、清空区 Dock=Bottom，移除历史区外层 ShowHistory，将历史 Grid 放到主 DockPanel 最后并启用 LastChildFill，移除表格固定 Height。2026-10-13 最终实际验证顶部/底部稳定、历史填满中间、拉高200时表格等量增长、恢复和最小尺寸可读、原业务保持，17个场景全通过。[布局 Q2](./questions/wpf-layout.md)按上述应用范围标已掌握；历史内部Grid、后台数据对象/命令/绑定由教练或旧作品提供，不计新的自主设计，也不推定完整测量机制、列布局和独立设备监控界面全部掌握。证据见[结果](./tasks/evidence/TASK-022/results.json)与[核对](./tasks/evidence/TASK-022/verification.json)。

- TASK-021 中学员在既有骨架里自主将主 Grid 三行改为 Auto / * / Auto，将选择/历史/清空区域放至 0/1/2 行，移除 DataGrid 固定 Height。2026-10-13 实际窗口位置、窗口拉高 200 时表格等量增长、上下行稳定、恢复尺寸和最小尺寸可读性均验证，17 个场景全通过。掌握限此骨架内的垂直三行分配/区域位置/表格伸展应用；窗口、历史区内部 Grid、数据对象/命令/Binding 由教练或既有作品提供，不计自主完整界面设计，也不扩展为 DockPanel、列布局、全部测量机制或独立 StackPanel 设计。[布局 Q1](./questions/wpf-layout.md)按此范围标已掌握，原教学反馈和待补边界保留。

- TASK-020中学员在已有骨架里调整下拉框宽度并添加 `SelectedItem="{Binding DeviceId}"`，2026-10-13实际默认选择与改选写回原对象后读取均验证，13个场景通过，旧行、原集合、清空恢复保持。掌握限字符串选项的SelectedItem连接与当前设备读取应用；选项数组/ItemsSource/DataContext/原业务和表格由骨架或既有作品提供，不计新的自主设计，也不扩大为所有Binding默认方向或设备对象选项。下拉框Q1按该范围标已掌握，问答统计待复习30/已掌握19，其他复习范围保留。
- TASK-019 中学员在教练提供的三列表格骨架里自主设置“设备 ID”表头、ItemsSource 连接原 Readings、DeviceId/Value/Unit 三列 Binding。2026-10-13 原编译窗口 11 个实际场景通过，各单元格绑定 Active 并读取相应 Reading，旧行/原集合保持，清空恢复与空白保护正确，最小尺寸可读。掌握限本骨架内表格集合和逐行字段连接应用，[读数表格 Q1](./questions/wpf-datagrid.md) 按该范围标已掌握；只读限制、窗口后台与原命令由既有骨架/作品提供，不计新的自主实现，也不扩大为编辑、排序或完整框架机制。证据见 [结果](./tasks/evidence/TASK-019/results.json)与[核对](./tasks/evidence/TASK-019/verification.json)。

- TASK-018 中学员在教练提供的绘图骨架里补上循环内 `points.Add(new Point(x, y))`、循环后 `TemperatureLine.Points = points`，并将构造调用切到 DrawTemperatureLine；2026-10-13 原窗口实际验证三个坐标按顺序连接、原温度对应、文字可读。自主掌握限本骨架内坐标集合与折线的连接应用；窗口/数组/转换计算/标签布局由既有实现提供，不升级为自主设计。28 度副本修改与运行由教练执行，不计学员自行修改数据的证据；[坐标与折线 Q1/Q2](./questions/wpf-charts.md) 的完整机制仍待复习。证据见 [原窗口结果](./tasks/evidence/TASK-018/original-26_5.json)与[核对](./tasks/evidence/TASK-018/verification.json)。

- TASK-017 中学员在教练提供的按钮模板骨架内将圆角设为 12、显式连接 Border.Padding 到 Button.Padding、配置 IsEnabled=False 时 ButtonBorder.Opacity=0.4 的 Trigger，于 2026-10-12 实际审查通过。原应用启动退出 0，原编译窗口 11 个显示/业务场景全部通过；同一 Style/Template、各自按钮内容、禁用变淡/读取恢复/清空再禁用、三字段历史/空白保护/最小尺寸均已验证。临时调 Button.Padding 时 Border 同步更新，诊断由教练执行，不计学员自主修改共同 Setter 的证据。39 个源码哈希不变。掌握限此骨架中的 Padding 连接与禁用外观规则应用，[控件模板 Q3/Q4](./questions/wpf-controltemplates.md) 按此范围标已掌握；外层模板、窗口/资源/业务连接由教练提供，Q1/Q2 默认内容与框架机制仍待复习，不推定独立完整控件设计。证据见 [场景结果](./tasks/evidence/TASK-017/results.json)及[核对](./tasks/evidence/TASK-017/verification.json)。
- TASK-016 中学员在教练提供的 DataTemplate/横向 StackPanel 骨架中自主配置 DeviceId/Value/Unit 三个 OneWay Binding、增加数值/单位文字控件并调整右侧 Margin；数值路径和 ID 右边距已自主修复。2026-10-12 构建 0 警告/错误，当前原编译窗口 10 个业务/显示场景全部通过，各字段绑定 Active 且数据源为对应 Reading，旧行保留、原集合连接保持，ID→数值/数值→单位间距均为 12，空白保护/清空恢复/共用样式正常。掌握限此模板骨架中的字段绑定、逐条数据源应用及横向间距，不计教练提供的外层模板、类定义或业务连接为自主设计。数据模板 Q1/Q2 按此应用范围标已掌握，绑定引擎/资源等机制不一并升级。证据见 [结果](./tasks/evidence/TASK-016/results.json)和[核对](./tasks/evidence/TASK-016/verification.json)。
- TASK-015 中学员在教练提供的窗口资源/控件骨架上自主补齐显式 Style 的 Padding/HorizontalAlignment Setter、读取按钮 StaticResource 与内部清空按钮 DynamicResource 引用，并移除两按钮对应直接属性，于 2026-10-12 实际运行验证。两个按钮取得同一窗口 Style；原窗口 `24,10` 时均 `98 × 37.24`，独立编译副本把共同 Setter 改为 `32,14` 后均 `114 × 45.24`，文字/布局/禁用状态正常，9 个读取/清空场景全通过，原集合/对象/命令保持。Style 应用限此任务骨架，不计独立副本由教练设置的变化为学员已自主完成运行时切换；[样式 Q1](./questions/wpf-styles.md) 标已掌握，Q2 资源加载时机机制仍待复习。证据见 [结果](./tasks/evidence/TASK-015/results.json)及[源码/程序集核对](./tasks/evidence/TASK-015/verification.json)。
- TASK-014 中学员在教练提供的 UserControl/窗口连接上自主补齐历史标题、列表与清空按钮，并将内部 ItemsSource/Command 连接原 Readings/ClearCommand，于 2026-10-12 实际运行验证。9 个场景通过：TEMP-001 连读两次、换 TEMP-002 后为 3 条，两种空白保留历史，实际清空为 0 条并禁用，再读带空格 TEMP-003 恢复，第二次清空正确。窗口、控件与内部元素始终使用同一 ViewModel 和历史集合。掌握范围限在现有骨架中组合历史界面及应用既有绑定；类定义、命名空间和 DataContext 连接由教练提供，不计自主设计，[WPF 控件 Q1](./questions/wpf-usercontrol.md) 的机制仍待复习。证据见 [结果](./tasks/evidence/TASK-014/results.json)和[构建/启动/源码核对](./tasks/evidence/TASK-014/verification.json)。
- TASK-013 中学员自主在读取命令内取得 ViewModel.DeviceId、Trim、校验空白并提前 return、创建 Reading、Add 原历史集合及设置成功提示于 2026-10-12 最终验证：TEMP-001 连续两次、换 TEMP-002 保留旧记录，两种空白均保留 3 条历史，清空与带空格 TEMP-003 恢复正确。首次遗漏 return 已由学员在审查期间自主修复，最终 10 个场景全通过。限在教练提供的 ViewModel/绑定/命令连接上应用读取逻辑；完整属性、TwoWay 设置、对象构造和通知工具仍不计自主设计，[WPF ViewModel Q1](./questions/wpf-mvvm.md) 的机制保持待复习。
- TASK-012 中学员自主补充的 CanExecute 历史 Count > 0 判断、Execute 清空原集合和“已清空”提示于 2026-10-11 实际验证。实际按钮通过绑定命令执行，空→读取启用→清空禁用→再次读取启用完整链路正确；两种空白输入保留历史。同一对象、集合和命令引用以及集合/条件通知均已核对，8 个场景全部通过。[WPF 命令 Q1](./questions/wpf-commands.md) 限此应用范围标为已掌握；提供的构造连接、属性、事件声明、通知工具和框架订阅不计自主设计，C# 事件 Q6 继续待复习。
- TASK-011 中学员补充的原历史集合 Add/Clear 与成功提示已于 2026-10-11 实际验证。在已提供的列表绑定上，TEMP-001 连续读取显示两条，换 TEMP-002 后保留旧记录并追加第三条；空字符串/纯空格提示且维持 3 条；清空后集合与列表均为 0，再读取两侧带空格的 TEMP-003 后均为 1。每次 Add 有新增通知，Clear 有 Reset，数据对象、集合、ItemsSource 与绑定始终保留。[WPF 集合 Q1](./questions/wpf-collections.md) 按此应用范围标为已掌握；数据类、集合初始化、完整 getter、输入校验与绑定由骨架提供，不计自主设计，不扩展到元素内部属性更新或所有事件原理。
- TASK-010 中自主连接的 DataContext、ResultText/StatusText 两个 OneWay Binding、setter 内对应名称的通知调用，以及按钮修改同一数据对象的结果/状态属性已于 2026-10-09 实际验证。TEMP-001/002 显示当前 ID、26.5 °C；空字符串/纯空格提示并清除结果；带两侧空格的 TEMP-003 恢复成功；每次收到两项 PropertyChanged，绑定始终保留。掌握范围限此基本绑定与通知工具的应用，类结构、get/set 字段保存、事件声明/发布工具由骨架提供，不计自主设计；new()/readonly、nameof、?.Invoke、PropertyChangedEventArgs 和完整属性语法等保持原复习范围。[绑定 Q1](./questions/wpf-binding.md) 按本次范围标已掌握，其他问答不一并升级。
- TASK-009 中自主实现的当前输入 Text.Trim()、空白判断、提示和清除旧结果、提前返回、Reading 对象构造与 ToString、结果/状态控件 Text 更新，已于 2026-10-09 通过实际 WPF 窗口的按钮自动化调用验证。TEMP-001/002 随输入变化，空字符串及纯空格清除结果，带两侧空格的 TEMP-003 恢复成功。窗口骨架、控件名称及 Click 关联由教练提供，不计自主设计；StackPanel/Grid、Margin 的布局问答仍待复习，不扩大为所有 WPF 概念已掌握。
- TASK-008 中自主补充的接口成员、两个构造函数的实例参数赋值、基于实例 ID/等待时间的带 Token 异步读取，以及接口集合中的共用 await 循环于 2026-10-05 实际运行验证。通过原采集方法的不同 ID 与等待参数对照，确认实例绑定和等待生效；压力单位已自主修正并复跑通过，三项 DoD 全部通过。骨架预备的类结构与方法签名不列为自主设计，掌握范围限本次模拟设备实践，不含未运行的可选取消场景。
- TASK-007 中自主实现的配置驱动三次循环与逐次 await、配置修改生效且不写回、缺失/损坏提示及安全结束于 2026-10-05 最终复查通过。取消条件、取消源、Token 经循环与读取传到等待、调用方专门处理、using 覆盖任务等待均有自主代码及运行证据。按配置调度取消后 1000/10/1 ms 共 14 次安全退出；允许请求前已完成的读数，取消当前读取后不继续输出成功。掌握范围为模拟设备的协作取消链路，不含计时精确保证、真实设备停止或 using 全部概念。
- TASK-006 中学员自主实现并经 2026-10-05 实际运行验证的配置属性、JSON 序列化/反序列化、WriteIndented 格式化、异步文件保存/读取、按加载配置应用设备 ID 与等待时间，以及缺失/损坏时捕获专门异常、提示并返回 null；修改 JSON 后加载字段及等待生效，文件内容不变。可空声明的规范仍可整理，不因任务通关认定取消原理已掌握。
- TASK-001 中自主编写并经运行核对的基础设备/读数模型、`List<T>` 与 `Dictionary<TKey,TValue>` 的创建和填充。按 ID 查询及边界处理由系统实现，不列为自主掌握。
- TASK-002 中自主完成并经运行核对的事件声明、发布、订阅、取消订阅，以及将当前设备 ID 随读数传给处理器。事件参数类型与入口由系统预备，不计作自主设计。
- TASK-003 中自主完成并经运行核对的 Lambda/LINQ 双条件筛选、将 Lambda 传给 `Func<Reading, bool>` 参数及在遍历中调用委托计数。延迟执行等扩展概念仍待复习。
- TASK-004 中学员自主将占位等待改为 `await Task.Delay(8000)`、构造并返回样例读数，添加时间与线程日志观察暂停和恢复。结果输出、单位修正和明确标注 `await` 结果由教练代写，不列为自主掌握。
- TASK-005 中学员自主把取消源的 Token 经 `ReadAsync` 传给 `Task.Delay`、在读取等待期间按条件调用 `Cancel()`、在成功路径输出完整读数，并按反馈修正为专门捕获 `OperationCanceledException` 输出取消结果；2026-10-02 两次实际运行验证，2026-10-05 复跑通过。学员能说明取消响应产生异常并被捕获；取消源角色与完整路径的修正版由教练整理，不列为自主掌握；任务按用户明确要求已通关。

## 待复习与下一步

- 2026-10-13：TASK-021 已验证本骨架 Grid 三行布局应用；计划明确列出的 DockPanel 尚无实践证据，因此只规划 [TASK-022](./tasks/TASK-022.md)（⚪ 未开始），继续同一读取场景学习靠边摆放与最后区域填满。未开始新任务、未切换入口；独立设备监控界面、完整布局机制及既有 C# 机制继续学习，不宣称 WPF 阶段整体通关。下方下一任务/未通过结论是当时历史。

- 2026-10-13：TASK-018 的骨架内 Points 连接已验证，坐标换算和标签位置机制继续复习；计划明确列出的 DataGrid 尚无自主应用证据，因此只规划 TASK-019，复用已完成的 ReadingViewModel、集合与读取/清空命令。实时曲线、更多控件/布局以及独立完整监控界面仍需后续实践，WPF 阶段不宣称整体完成。

- 2026-10-12 TASK-016 最终审查：学员添加 ID 右边距后，重新构建并验证 10 个实际窗口场景，显示/业务全通过；三个 DoD 已勾选、任务 ✅ 已通关。35 个源码哈希不变，当前程序集哈希匹配，未代改源码。两段间距实测均为 12，截图可读，路径错误已自主修复，未新增典型错误。结合计划 Style / Template，仅创建 TASK-017（⚪ 未开始），下一次明确开启再准备 ControlTemplate 骨架；数据图表及原复习范围保留，不认定阶段整体完成。下方本日未通过记录是此前历史。
- 2026-10-12 TASK-016 同轮复查：学员自主把数值路径改为 Value，三字段显示已验证，路径错误复查完成。DoD 1/3 通过；第 2 项的间距仍未通过，当前截图与位置检查均证明 ID 末尾与数值开头紧贴，尚需自主补 ID 右边距。Q1/Q2 整体继续待复习，完整任务未通关；下方首次 PathError 为已修复历史。
- 2026-10-12 TASK-016 审查：原编译窗口经控件输入/按钮自动化执行 10 个场景，集合、旧记录、两种空白和清空恢复正常；DeviceId/Unit 已有局部自主应用与运行证据，数值错绑导致整体未通过，问答 Q1 保持待复习，不把整题技能迁入已掌握。新增 [数据模板 Q2](./questions/wpf-templates.md) 说明属性路径按当前行对象查找、编译成功与运行绑定成功不同、右边距的实际作用。35 个源码文件哈希不变，程序集哈希一致；错误机制记入错题本，不推断误写来源。任务仍进行中，DoD 3 已勾选；下方启动时“未运行”等为历史情况。
- 2026-10-12 TASK-016 启动：先区分已有自定义 Reading/ReadingViewModel 与 WPF 的 DataTemplate/ItemTemplate，再说明集合数据源和每行数据源不同；用 TEMP-001/TEMP-002 两条实例解释模板中的 DeviceId 不读取当前输入框。横向 StackPanel、Margin 两值/四值及首个最小绑定示例记入 [WPF 数据模板 Q1](./questions/wpf-templates.md)，状态待复习。骨架构建通过不计运行或自主应用证据，不新增错题；下方“仅创建 TASK-016”表述为此前历史。
- 2026-10-12 TASK-015 最终审查：构建 0 警告/错误、原 WpfBae.exe 打开并正常关闭退出 0，原编译 Task015Window 的 9 个真实窗口场景、同一 Style 检查及独立编译副本 Setter 布局对比通过。11 个相关源码哈希前后一致，教练未代改；首次审查工具混入旧程序集已纠正并核对当前程序集哈希，不计学员失败。清空按钮 Margin 为 0 但位置正常、无重叠，不增加固定间距要求；未新增典型错误。只创建 [TASK-016](./tasks/TASK-016.md) 卡片，先覆盖计划 Template 中的数据行展示，不开始下一题；Q1 仅应用范围升级，Q2 及其他复习范围保留，不宣称阶段整体完成。下方同日启动/异常记录为此前历史。
- 2026-10-12 TASK-015 资源异常答疑：已在 [WPF 样式 Q2](./questions/wpf-styles.md) 展开“构造控件 → 独立 InitializeComponent → 内部 StaticResource 立即查找 → 尚未接到窗口”的顺序，纠正此前指导遗漏，不记为学员错误。DynamicResource 为 WPF 的延后资源引用，内部按钮接入窗口后可取得同一 Style，窗口按钮继续 StaticResource；不泛化为所有引用都需更换。原编译窗口失败已重现，诊断内存副本实际 Show 后同对象/Padding/对齐正确；构建 0 警告/错误、诊断退出 0，学员源码未改，任务/DoD 不变，两条样式问答均待复习。当前读取按钮的直接 Padding/HorizontalAlignment 仍覆盖 Setter，恢复启动后再引导移除；不扩充验收或掌握范围。
- 2026-10-12 TASK-015 启动：先解释 Padding 的内留白、Style/Setter 是 WPF 类型、ActionButtonStyle 为自定义资源名、Window.Resources 保存外观对象、StaticResource 找资源及直接属性值优先于 Setter。记录至 [WPF 样式 Q1](./questions/wpf-styles.md)（待复习）；使用现有普通对象创建与连接，不把历史任务通关当作样式已掌握。四个任务专属文件和空 Style 由教练提供，核心 Setter/资源引用留给学员；实际 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误，未运行或验收窗口。下方同日 TASK-014 审查及创建 TASK-015 的表述为此前历史记录。
- 2026-10-12 TASK-014 最终审查：构建 0 警告/错误，原 WpfBae.exe 启动及正常关闭退出 0；审查项目显示原 Task014Window，通过真实输入控件写回和按钮自动化命令路径验证 9 个场景，三项 DoD 全部通过。9 个源码 SHA256 前后一致，未代改实现，未发现需要新增错题的问题。ListBox 未写建议名称不影响必需行为。仅创建 [TASK-015](./tasks/TASK-015.md)，先练显式 Style 共享按钮外观；不开始下一题，不宣称整个 WPF 阶段完成。以下同日启动记录描述当时情况，UserControl/属性继承机制继续待复习。
- 2026-10-12 TASK-014 启动：用户明确要求开启，目标目录此前不存在；已创建最小骨架并切换共享 App.xaml 入口，实际 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误。仅验证构建，未运行或验收实际窗口。先讲 UserControl 与自定义类身份、XAML/后台配对、命名空间前缀及父窗口 DataContext 同对象继承，记录至 [WPF 控件 Q1](./questions/wpf-usercontrol.md)（待复习）。首个实践只显示历史标题；原读取、清空命令及 ReadingViewModel 未修改，骨架和讲解不计自主掌握证据。
- 2026-10-12 TASK-013 最终审查：首次空白保护失败后学员补上 return，重新构建及 10 个实际窗口场景全通过；三个 DoD 已勾选，任务 ✅ 已通关。问题与修复证据已写 [错题本](./mistakes.md)，教练未修改学员源码。以下同日启动/答疑记录描述当时状态，相关通知机制仍待复习；按计划仅创建 TASK-014，未开始。
- 2026-10-12 TASK-013 绑定设置答疑：[WPF ViewModel Q1](./questions/wpf-mvvm.md) 按 StatusText 只显示、DeviceId 可编辑对照 OneWay/TwoWay；UpdateSourceTrigger 只控制控件写回对象的时机，OneWay 动态刷新仍依靠对象属性通知。解释文字变化即写回与失焦写回两种场景，未引入新框架或要求额外实践。未改/运行源码，问答与任务状态、DoD 不变。
- 2026-10-12 TASK-013 属性通知用途答疑：[WPF ViewModel Q1](./questions/wpf-mvvm.md) 补充“TextBox 原样输入写回已能保存”与“代码修改属性需通知绑定刷新”的区别，再以其他绑定控件说明通知用途；补足此前讲解的修改来源，不把追问当认知错误。未改/运行源码，Q1 待复习，任务与 DoD 不变。
- 2026-10-12 TASK-013 输入写回答疑：[WPF ViewModel Q1](./questions/wpf-mvvm.md) 补充 TextBox 的 TwoWay/PropertyChanged 自动调用 DeviceId setter，value 保存文字，OnPropertyChanged 负责对象通知；使用赋值效果示意而非要求新增代码。仅核对源码，未验证实际输入，Q1 待复习，TASK-013 状态与 DoD 不变。
- 2026-10-12 TASK-013 启动：先解释 MVVM 三种角色、自定义 ReadingViewModel 实例、DataContext 与两个命令共用同对象，再通过完整 DeviceId getter/setter 说明输入写回；TwoWay/UpdateSourceTrigger 与自定义通知工具的区别记入 [WPF ViewModel Q1](./questions/wpf-mvvm.md)，状态待复习。骨架连接由教练提供，首个实践只显示命令取得的当前 ID；不将骨架或讲解计为学员自主掌握，不追加考试门槛。
- 2026-10-12 TASK-012 调用链理解确认：学员已正确复述集合变化经命令事件通知 WPF、再调用 CanExecute 的主线；[WPF 命令 Q1](./questions/wpf-commands.md) 补充 Binding 读取 ClearCommand 公开属性、getter 返回 _clearCommand 原对象，及初次检查和重复通知边界。此为概念反馈，不扩展自主应用的已掌握范围；框架订阅与通知机制继续待复习。本轮未修改或运行源码，TASK-012 状态及 DoD 不变，TASK-013 仍未开始。

- 2026-10-12 DataContext 与通知差异追问：新增 [WPF 集合 Q2](./questions/wpf-collections.md#q2都有-datacontext--_state为什么-task-010-的数据类实现-inotifypropertychangedtask-011-的不用)，从实际变化位置解释 TASK-010 的 ResultText/StatusText 属性通知与 TASK-011 同一集合 Add/Clear 自带通知；先区分自定义外层数据对象和 .NET 集合，再说明初次取值、后续刷新及集合替换边界。状态待复习，不因任务已通关默认掌握这层机制；本轮未修改或运行源码，不改变 TASK-010/011 状态、DoD 或启动新任务。
- 2026-10-11 最终审查更新：TASK-012 已通关；下方同日过程答疑记录中的“未运行/进行中/状态不变”均描述当时情况。委托基础、框架内部订阅与通知工具原理不随功能验收升级，后续继续用完整普通语法和实际对象连接解释，不追加口试。
- 2026-10-11 委托调用写法追问：[C# 事件 Q6](./questions/csharp-events.md) 补充 handler(...) 与 handler.Invoke(...) 同为委托调用，?. 在局部变量为 null 时跳过，对照当前完整 if 判断；说明骨架完整写法用于先展示步骤，并非简写不可用。?. 不吞掉处理方法异常。本轮未修改或运行源码，问答仍待复习，TASK-012/DoD 不变。
- 2026-10-11 委托与事件身份追问：学员需要分清 EventHandler 委托类型、event 声明关键字、CanExecuteChanged 事件及 handler 委托变量；已在 [C# 事件 Q6](./questions/csharp-events.md) 拆开解释，并补外部订阅/取消与类内部发通知的区别。后续追问补充 delegate 用来定义委托类型，MessageHandler 自定义、EventHandler 由 .NET 提供，事件声明使用已有类型；两者不是两套机制。不因事件实践通关默认掌握类型关系，不记录为学员错题；本轮未改或运行源码，问答仍待复习，TASK-012/DoD 不变。
- 2026-10-11 事件来源参数追问：[WPF 命令 Q1](./questions/wpf-commands.md) 补充 handler(this, EventArgs.Empty) 按普通参数传递，把当前命令对象引用交给 sender，报告通知来源；EventHandler 约定两个参数，传 this 是来源约定而非 C# 限制。多个来源可由同一处理方法通过 sender 区分；本任务按钮已持有 Command，可以不使用 sender。仅补必要机制，未运行或改源码，问答和 TASK-012 状态不变。
- 2026-10-11 理解确认：学员正确复述事件通知会调用 WPF 按钮的处理方法；补充接收通知后重新调用 CanExecute、更新可用状态，不执行清空。记录至 [WPF 命令 Q1](./questions/wpf-commands.md)，属于概念反馈，问答仍待复习；未修改或运行源码，TASK-012 和 DoD 不变。
- 2026-10-11 WPF 订阅动作追问：[WPF 命令 Q1](./questions/wpf-commands.md) 补充“订阅就是登记之后通知时要调用的方法”，使用完整普通处理方法与 += new EventHandler(...) 区分登记和调用。说明 WPF 控件也是框架 C# 类，Command 连接时框架通过内部工具完成 += 登记，而非事件名称自动寻找方法。学员当前需要看到实际连接动作，不能只重复“自动监听”；保留 TASK-002 已验证应用，不扩展为 WPF 订阅工具全部掌握。本轮未修改或运行源码，TASK-012、DoD 与问答状态不变。
- 2026-10-11 委托用途追问：学员指出固定委托调用看不出相对直接调用的用途，已在 [C# 事件 Q6](./questions/csharp-events.md) 补充“同一个模拟读取流程，由调用者传入显示/报警处理方法”的完整普通写法及多个方法组合。先承认固定已知方法直接调用足够，前例仅展示语法未展示必要性，属于教学示例需改进，不记为学员错误；不再仅以定义解释用途。概念待复习，既有 TASK-002/003 应用证据保留，本轮未运行或改源码，TASK-012 和 DoD 不变。
- 2026-10-11 处理方法来源追问：[WPF 命令 Q1](./questions/wpf-commands.md) 已通过官方源码具体指出命令事件直接订阅内部转发方法 HandlerSink.OnCanExecuteChanged，再交给按钮基类 ButtonBase.OnCanExecuteChanged，最后检查学员的 CanExecute。说明注册在 Button.Command 连接时由 WPF 完成，框架方法不在学员文件中；给出完整 new EventHandler 和 += 对应。只解释当前转发路径，不引入弱引用细节。未运行本地订阅枚举或改源码，问答和任务状态不变。
- 2026-10-11 委托基础反馈：学员明确追问“委托是什么意思”，新增 [C# 事件 Q6](./questions/csharp-events.md)，状态待复习。从完整普通方法、自定义 delegate 类型的参数/返回值要求、完整 new 保存方法引用、通过变量调用依次说明，再对应 EventHandler/handler/event 的角色。引用方法时不执行方法，带括号调用时执行；不以 TASK-002/003 的已验证事件或 Lambda 使用推定该基础全部理解。保留既有掌握范围，本轮未修改或运行源码，TASK-012 与 DoD 不变。
- 2026-10-11 命令对象来源追问：[WPF 命令 Q1](./questions/wpf-commands.md) 进一步区分示例 `_state.ClearCommand` 属性取得命令与 handler 的 this→sender 参数传递。同一对象有不同引用来源；按钮通过 Binding 已持有命令，事件只请求重查条件。已核对官方 WPF 源码，重查从按钮的 Command 取得对象，不依赖 sender。明确前条两行是接口调用示意而非框架源码，避免把省略的调用路径当成隐藏赋值。未运行或改源码，任务及问答状态不变。
- 2026-10-11 命令事件/反射追问：[WPF 命令 Q1](./questions/wpf-commands.md) 补充 CanExecuteChanged 与 CanExecute 是不同成员，事件调用 WPF 注册的处理方法，后者通过 ICommand 直接调用条件方法；此路径是事件回调与接口调用，不能把事件理解为 CanExecute 的方法引用或反射查找。结合 Java 监听器说明，Q1 保持待复习。本轮未运行或改源码，TASK-012 与 DoD 不变。
- 2026-10-11 命令事件追问：[WPF 命令 Q1](./questions/wpf-commands.md) 已补充事件声明供按钮订阅、EventHandler 引用处理方法、? 与无订阅者 null、handler(this, EventArgs.Empty) 调用订阅方法以及 WPF 重新调用 CanExecute 的机制。学员当前知道集合变化到通知工具的连接，但此声明/调用角色仍需解释，Q1 保持待复习，不覆盖 TASK-002 已验证的范围。集合查询和元素内部属性修改不自动触发 CollectionChanged；本轮仅更新记录，未运行或改源码。
- 2026-10-11 TASK-012 起点：[WPF 命令 Q1](./questions/wpf-commands.md) 记录 ICommand 的来源、Button.Command 与自定义 ClearCommand 属性的连接、this 传递同一数据对象、CanExecute 的 bool/Count 判断、Execute 动作和 CanExecuteChanged 重查通知，状态待复习。属性通知、集合订阅与通知辅助方法由骨架提供，不因新代码存在认定自主掌握；首个动作只补 CanExecute 判断，不要求口试。
- 2026-10-11 集合区别答疑：[WPF 集合 Q1](./questions/wpf-collections.md) 已增量补充 ObservableCollection 与 List 的共同操作及集合变化通知差别；普通 List 可以显示已有数据，但后续 Add 不会提供列表自动更新所需的集合通知；集合增删与已有读数内部属性改变分开解释。问答保持待复习，本轮未修改或运行源码，TASK-011 仍进行中，DoD 不变。
- 学习语言偏好（2026-10-05）：讲解、练习代码注释、TODO 说明及运行提示使用中文；保留 C# 关键字、API 和代码标识符的原名。
- 长期教学要求（2026-10-09）：按用户明确请求，已将本轮教学教训写入[AGENTS.md](../AGENTS.md)“教学起点与回答前核对”。回答前先考虑 Java 后端背景与最新掌握证据，不把已有骨架、讲过或使用过视为理解；新概念先讲来源、用途和完整语法，再给单个可观察的动手步骤。明确区分自定义类与框架 API，理解反馈不足时回补基础，不设置口试门槛。本轮仅更新规则与记录，未改源码、未运行或验收，TASK-010 和 DoD 状态不变。
- 2026-10-09 已明确 WPF 初学者背景：尚不理解 StackPanel/Grid。新概念需先解释用途、最小语法和可观察的例子，再给具体改动步骤；不把 Java 后端经验视为桌面界面经验。已补充[WPF 布局入门 Q1](./questions/wpf-layout.md)，状态待复习；TASK-009 首个动作细化为沿用 StackPanel，观察 TextBox.Margin 修改前后间距，未修改学员源码或新增验收证据。
- 2026-10-09 TASK-010 教学反馈：DataContext 及自定义 ReadingDisplayState 的来源仍未说明充分，新增[绑定 Q2](./questions/wpf-binding.md)，状态待复习。须先说明哪些是教练新建的普通类/实例，哪些是 WPF 提供的属性，再解释数据如何被界面读取，不用一个未解释的新概念解释另一个。本轮已见学员新增 DataContext = _state，保留代码；未运行或验收，不以代码存在认定理解或通过 DoD，TASK-010 仍进行中。
- 2026-10-09 C# 属性基础反馈：学员尚未学过带实现的 get/set、value 和 `get =>` 简写。已新增[属性 Q3](./questions/csharp-properties.md)，状态待复习；后续先用普通方法与完整花括号解释读写，再介绍属性简写和通知，不因使用过自动属性或骨架带有属性就默认理解。本轮未改源码、未运行或验收，任务和必需 DoD 状态不变。
- 2026-10-09 框架类型来源答疑：[绑定 Q4](./questions/wpf-binding.md)解释 .NET 自带的 System.ComponentModel.PropertyChangedEventArgs、using、构造函数传入属性名称与 PropertyName 的作用，状态待复习。已见两个 setter 的字符串属性名通知调用，尚未运行或验收；不因代码存在认定通知机制已掌握，TASK-010 仍进行中。
- 2026-10-05 路线答疑：[为上位机学习 C#，TASK-008 后能否进入 WPF](./questions/learning-route.md)。结合计划与任务级验收，当前基础已足够开始 WPF 起步，C# 集中入门可先告一段落；异步、属性、可空类型及资源释放继续随实践巩固。待复习问答不作为进入 WPF 的单独考试，第一阶段整体状态保持原状；当时 TASK-009 未开始，已于 2026-10-09 按用户要求启动。
- 下一步：[TASK-018](./tasks/TASK-018.md)（⚪ 未开始），按计划开始数据图表入门，只有卡片，用户明确开启再准备源码。TASK-017 三个必需 DoD 已全部通过；控件模板 Q3/Q4 限已验证应用范围标已掌握，Q1/Q2 默认规则与完整机制仍复习。数据图表、委托/事件、完整属性语法、Task/线程、using 与布局机制仍需后续学习，不认定阶段整体通关。
- 巩固：TASK-005 角色混淆及 TASK-007 重抛/固定取消延迟错题均已有自主修复证据；CancellationToken Q1 标为已掌握。若希望固定取消第一轮，应选择更短延迟并预留调度余量，这是后续实践优化，不追加本任务验收门槛。
- 待复习：[Task、async/await 与线程关系](./questions/csharp-task-async.md)；TASK-004 的最终结果输出由教练代写，后续实践继续核对这些概念。
- Task/Delay 用法复查：[Q1](./questions/csharp-task-async.md) 已由 TASK-006 自主等待实现及 3500/50 ms 对照运行验证，标为已掌握；其他 Task 问答维持原状态，不扩大为全部概念掌握。
- 问答复查：[CancellationToken](./questions/csharp-cancellation.md) Q1 于 2026-10-05 经 TASK-007 自主代码与长短配置运行验证标为已掌握，Q2 保持已掌握；不将相等计时竞争或本次 14 次结果说成精确/通用时序保证。
- 待复习：[using 与资源释放](./questions/csharp-resources.md)（2026-10-02 答疑）；已有 `using var` 管理取消源的实践代码，命名空间引入、Dispose 与 Cancel 的区别仍待复习。
- 已验证：[JSON 格式化与 WriteIndented](./questions/csharp-json.md) Q1（2026-10-05）；学员自主配置选项并生成带缩进 JSON，已标为已掌握。Task/await 的完成跟踪仍需结合漏等待问题继续复查。
- 已补充背景：学员此前未接触 C# Lambda 与 LINQ；2026-09-27 已通过 TASK-003 的实践验收。其他 C#/.NET 基础、可用开发环境、目标设备或仿真条件、学习节奏仍待补充。
- 2026-10-05 返回机制追问：[Task/async Q6](./questions/csharp-task-async.md) 已补充 SaveAsync 无手写 return 的原因、编译器负责返回完成任务及 Task/结果值的区别，概念保持待复习；本轮未运行或修改源码，不改变 TASK-006 通关或当前任务进度。
- 2026-10-07 取消时机追问：[CancellationToken Q3](./questions/csharp-cancellation.md) 新增为待复习。区分 Cancel 发出请求、ReadAsync 内部等待响应、pendingReading 取消及调用方 await 观察后进入 catch；已取消任务在 await 处无需暂停即可抛出。保留 Q1/Q2 已验证的掌握范围，本轮未修改或运行源码，不改变任务状态或启动 TASK-009。
