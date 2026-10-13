# TASK-019：用 DataGrid 展示设备读数列表

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」的 Button / TextBox / ComboBox / DataGrid，以及 Binding、ObservableCollection 与设备监控界面目标。
- 选择依据：TASK-018 的固定样例绘图已实际验收；学员在骨架中接通坐标集合与折线的应用已验证。图表换算与框架机制继续复习，不立即叠加实时采集。计划明确列出的 DataGrid 尚无自主应用证据，下一步复用已有读数集合和命令，以表格展示数据。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

在独立窗口中，继续使用已有设备 ID 输入、读取和清空功能，把每条读数显示为表格的一行。表格明确显示“设备 ID”“温度”“单位”三列，能看到不同设备的旧记录；清空后表格随同一集合变空，继续读取恢复。

本任务先学习只读表格，不要求编辑读数、排序策略、分页、真实通信、数据库或第三方控件。

## 开始前必须先解释

1. 先给出实际表格的三列效果，再说明 DataGrid 是 WPF 提供的表格控件；每条 Reading 是一行，DeviceId/Value/Unit 分别填入三列。它与已用 ListBox 展示同一份集合，只是显示形式不同。
2. `ItemsSource` 给表格提供整个 Readings 集合。`DataGridTextColumn` 是 WPF 提供的文字列；列里的 Binding 取当前行 Reading 的字段，不取当前设备输入框的值。用 TEMP-001 与 TEMP-002 两条具体读数说明。
3. `Header` 设置列顶部显示的标题，“温度”是显示文字，`Value` 是读数对象的属性名；不能把中文列标题直接当成属性路径。
4. `AutoGenerateColumns="False"` 让本次表格使用自己列出的三列；`IsReadOnly="True"` 防止在表格内改读数。先解释实际效果，再要求添加设置。
5. 继续沿用原 ObservableCollection 的 Add/Clear。表格的数据源与原集合相连，不在每次读取时创建新集合；集合通知与列字段取值的具体作用分别说明。

## 用户实践与产出

1. 教练准备窗口、输入/命令连接与 DataGrid 骨架后，首步只设置设备列的 Header，运行观察表头变化。
2. 在解释三列与逐行 Binding 后，给表格连接 Readings，补齐三列的字段路径；读取 TEMP-001 和 TEMP-002，观察两行均保留各自的设备 ID、温度和单位。
3. 清空并再次读取，验证空表与恢复；输入空字符串/纯空格时不新增行，旧记录保留，原提示正常。

## 目录与开始约定

2026-10-13 用户明确开启，检查目标目录原先不存在后，已创建专属目录 `WpfBae/Exercises/TASK-019-reading-datagrid/`，准备独立窗口；已有作品保留。

复用实际存在的 [ReadingViewModel.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[ReadTemperatureCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs)。开始时再检查读数类型和原命令状态；新建前确认目标文件不存在，不重置学员源码。共享 App.xaml 仅作必要入口连接。

实际文件与复用关系：

- 新建 [Task019Window.xaml](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml)：输入、两个命令按钮、提示和三列表格骨架；数据连接与列字段留 TODO。
- 新建 [Task019Window.xaml.cs](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml.cs)：创建一个原 ReadingViewModel 对象，设置窗口 DataContext 与本窗口初始提示。
- 复用上方 TASK-013 的三个业务文件以及 [Reading 定义](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)，不复制或修改学员原实现。原读取命令已包含 Trim、空白 return、原集合 Add 与成功提示，原清空命令已包含 Count 条件、Clear 与提示。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml) 的 StartupUri 从 Task018Window 改为 `Exercises/TASK-019-reading-datagrid/Task019Window.xaml`；项目已有 WPF 自动包含规则，不改 csproj 或引入依赖。

| TODO | 文件位置与操作 | 对应实践 / DoD |
| :--- | :--- | :--- |
| 1（首个动作） | XAML 第一列的 Header 从 `设备列（待命名）` 改为 `设备 ID` | 实践 1 / DoD 1 的表头可读性 |
| 2 | XAML 的 DataGrid 开始标签添加 ItemsSource，连接原 Readings | 实践 2 / DoD 2 的原集合连接 |
| 3a | 第一列添加 Binding，读取当前行 Reading.DeviceId | 实践 2 / DoD 1、2 的设备列与旧行 ID |
| 3b | 温度列添加 Binding，读取当前行 Reading.Value | 实践 2 / DoD 1、2 的温度列 |
| 3c | 单位列添加 Binding，读取当前行 Reading.Unit | 实践 2 / DoD 1、2 的单位列 |

只读与手动列设置由骨架提供：`AutoGenerateColumns="False"` 只显示声明的三列，`IsReadOnly="True"` 禁止编辑单元格；CanUserAddRows/CanUserDeleteRows=False 禁止从表格直接增删记录。这些设置不阻止原命令 Add/Clear，运行时仍须检查实际效果，不能算学员自主实现。清空/恢复、空字符串/纯空格保护是实践 3 / DoD 3，复用原实现并在新表格连接完成后实际验证，无需重写命令。

首步只完成 TODO 1，关闭旧程序后，在工作区根目录运行：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

预期实际打开标题为“TASK-019：设备读数表格”的窗口，第一列表头变为“设备 ID”，其他表头仍为“温度”“单位”，表格仍为空。不要把设计预览当成运行结果；读取此时只更新集合和提示，直到 TODO 2/3 完成才显示完整行。后续绑定含义和两条读数示例见 [DataGrid 讲解](../questions/wpf-datagrid.md)。上述 run 命令是给学员的运行方式，本轮教练未执行。

## 验收标准（DoD）

- [x] 项目编译并实际打开 TASK-019 窗口；表格只读，明确显示设备 ID、温度、单位三列，表头与行内容可读。
- [x] DataGrid 连接原 Readings 集合；连续读取与换设备后正确追加，每一行的三列来自该条 Reading，修改输入不改旧行。
- [x] 实际清空使同一集合与表格变空，按钮条件和提示正确；再读取恢复；空字符串/纯空格输入不新增行或清除旧行。

## 2026-10-13 实际审查

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1：构建、窗口与三列只读展示 | 通过 | 原项目构建 0 警告/错误，原 exe 实际打开 TASK-019 并关闭退出 0；审查程序 Show 原编译窗口。实际三列表头正确，行内文字与最小尺寸均可读，有记录时 BeginEdit 返回 false。 |
| 2：原集合、追加与逐行字段 | 通过 | 连读 TEMP-001 两次再读 TEMP-002 为三行；每个文字绑定状态 Active，DataItem/DataContext 为相应 Reading，值为各自 ID、26.5、°C；只改输入不改旧行，旧 Reading 与集合引用保持。 |
| 3：清空、恢复与空白保护 | 通过 | 有记录时清空可用，清空后集合/表格均为 0 并禁用；重新读取带空格的 TEMP-003 经 Trim 恢复；空字符串/纯空格保留原三行、提示“该 Id 不合法”，未发集合变化通知。再次清空正常。 |

共 11 个实际窗口场景通过，0 运行异常。通过 TextBox.SetCurrentValue 改实际控件输入，通过 ButtonAutomationPeer/IInvokeProvider 调用原按钮命令；图像是已实际显示窗口的内容渲染，不是设计预览。审查程序集与原构建 DLL 的 SHA256 一致，审查前后 43 个源码文件哈希不变。

实际执行的命令：

```powershell
dotnet build WpfBae/WpfBae.csproj --nologo
dotnet run --project docx/tasks/evidence/TASK-019/Review.csproj -- docx/tasks/evidence/TASK-019
& docx/tasks/evidence/TASK-019/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-019/launch.json
& docx/tasks/evidence/TASK-019/verify.ps1
```

四条命令均退出 0。证据：[11 个场景结果](./evidence/TASK-019/results.json)、[原程序启动记录](./evidence/TASK-019/launch.json)、[源码与程序集核对](./evidence/TASK-019/verification.json)、[三条读数](./evidence/TASK-019/history-three-readings.png)、[最小窗口](./evidence/TASK-019/minimum-size.png)、[审查源码](./evidence/TASK-019/Review.cs)。

学员自主补齐 Header、ItemsSource 和 DeviceId/Value/Unit 三列 Binding，本骨架内表格数据连接应用已验证。只读限制、后台对象连接和复用命令由骨架提供，不算本次自主实现；不扩大为编辑、排序、框架内部机制或完整监控界面掌握。列 Binding 未显式写 Mode，本次只读单元格实际显示和绑定正常，无需为了示例的 OneWay 写法判为错误。

骨架初始提示与注释仍有“数据连接尚未完成”的旧文字，这是教练提供的练习提示，不是学员业务实现错误，不记错题或追加验收门槛；源码保留。根据计划明确列出的 ComboBox 尚无自主实践证据，只建立下一张 [TASK-020：用 ComboBox 选择设备并读取温度](./TASK-020.md)（⚪ 未开始），不创建源码或切换入口。上方首步操作与下方启动记录保留为此前历史。

## 可选巩固

- 后续考虑行选中后显示详细信息，本次未做不计必需验收。
- 不以本卡通关推定编辑表格、完整设备监控界面或 WPF 阶段全部掌握。

## 状态与证据

- 2026-10-13 审查通关：三个必需 DoD、11 个实际窗口场景全通过，构建与原程序启动关闭正常，源码未代改；状态 ✅ 已通关。DataGrid 问答按本骨架内数据连接应用范围标已掌握；只创建 TASK-020 未开始卡片，原入口仍为 Task019Window。
- 2026-10-13 启动：两个任务专属骨架文件已创建，共享 App.xaml 已切换；复用源码未修改。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0，0 警告、0 错误。未运行实际窗口或验收；三个 DoD 保持未勾选，不新增自主掌握证据。首步只改 Header，数据源与逐行字段仍留 TODO。下方仅创建卡片的记录为启动前历史。
- 2026-10-13：TASK-018 三个必需 DoD 通过后，依据计划中 DataGrid 控件与现有集合/命令实践只创建本卡。状态 ⚪ 未开始；未准备 TASK-019 源码、未切换 App.xaml、未运行或验收新任务。
