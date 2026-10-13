# TASK-019：用 DataGrid 展示设备读数列表

- 阶段：第二阶段：WPF（起步）
- 状态：⚪ 未开始
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

本卡只规划，尚未创建 TASK-019 源码、切换入口或构建。用户明确开启后检查现有项目与拟用目录 `WpfBae/Exercises/TASK-019-reading-datagrid/`，按既有惯例创建独立任务窗口；已有作品保留。

复用实际存在的 [ReadingViewModel.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[ReadTemperatureCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs)。开始时再检查读数类型和原命令状态；新建前确认目标文件不存在，不重置学员源码。共享 App.xaml 仅作必要入口连接。

在开始时登记真实文件、每个 TODO 对应 DoD、首个动作及实际运行命令；当前拟用目录不是已准备骨架。

## 验收标准（DoD）

- [ ] 项目编译并实际打开 TASK-019 窗口；表格只读，明确显示设备 ID、温度、单位三列，表头与行内容可读。
- [ ] DataGrid 连接原 Readings 集合；连续读取与换设备后正确追加，每一行的三列来自该条 Reading，修改输入不改旧行。
- [ ] 实际清空使同一集合与表格变空，按钮条件和提示正确；再读取恢复；空字符串/纯空格输入不新增行或清除旧行。

## 可选巩固

- 后续考虑行选中后显示详细信息，本次未做不计必需验收。
- 不以本卡通关推定编辑表格、完整设备监控界面或 WPF 阶段全部掌握。

## 状态与证据

- 2026-10-13：TASK-018 三个必需 DoD 通过后，依据计划中 DataGrid 控件与现有集合/命令实践只创建本卡。状态 ⚪ 未开始；未准备 TASK-019 源码、未切换 App.xaml、未运行或验收新任务。
