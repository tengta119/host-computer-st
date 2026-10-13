# TASK-020：用 ComboBox 选择设备并读取温度

- 阶段：第二阶段：WPF（起步）
- 状态：⚪ 未开始
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」的 ComboBox，以及 Binding、Command 与设备监控界面目标。
- 选择依据：TASK-019 的三列表格、逐行字段与同集合追加/清空已实际验收，学员在骨架中自主连接数据源有证据。计划列出的 ComboBox 仍无自主应用证据；下一步把自由输入改为从两个固定设备中选择，复用已完成读数表格与原命令，不叠加实时采集或真实通信。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

在独立窗口里，用下拉框从 TEMP-001、TEMP-002 选择设备。默认选中 TEMP-001；改选 TEMP-002 后点击读取，新行应记录 TEMP-002，旧行仍为 TEMP-001。下拉框不允许自由输入；表格仍显示设备 ID、温度、单位，清空和再次读取保持原行为。

本次只使用字符串设备 ID，不引入新的设备对象、SelectedValuePath、SelectionChanged 事件、未选择/null 状态、通信或数据库。原任务的空白输入保护仍保留，但新窗口的固定选项场景不要求制造无效输入，也不修改原命令。

## 开始前必须先解释

1. 先指向实际下拉框：展开后能选两个设备，选中一个后收起并显示它。ComboBox 是 WPF 自带控件；窗口中的具体控件名由教练自行命名。
2. 教练准备一个含两个字符串的 `string[]`，先用完整普通写法说明数组保存什么、谁保存引用，再连接 ComboBox.ItemsSource。此列表是可选设备 ID，历史 Readings 是实际读数，两份内容用途不同。
3. 在本次选项为字符串的条件下，SelectedItem 是用户当前选中的字符串，例如 TEMP-002；不是下拉框控件或整份历史集合。再说明怎样连接它和原 ReadingViewModel.DeviceId。
4. 默认数据对象的 DeviceId 为 TEMP-001，应显示为初始选项；TwoWay 让默认值显示到下拉框，也让用户改选后的字符串写回同一个数据对象。读取命令继续从该对象的 DeviceId 取当前 ID，不额外创建另一个 ViewModel。
5. IsEditable=False 禁止在下拉框内自由输入，但仍能展开并选择已有项；先说明效果再动手，不把它当作读取按钮是否可用的条件。

## 用户实践与产出

1. 用户明确开启后，教练准备独立窗口、两个字符串选项、原表格与命令连接。首步只改变下拉框 Width（例如 200 改为 260），运行观察同一个控件变宽，认识其真实位置；这只是首步观察，不单独作为完整通关证据。
2. 解释 SelectedItem 的来源与当前设备属性后，学员将下拉框选中值连接原 DeviceId。实际启动检查默认 TEMP-001，先读取一条，再改选 TEMP-002 并读取；新行使用新 ID，旧行不变。
3. 连续切换并读取，检查每行三列；清空后继续读取恢复。检查只能选择两个选项，不能自由输入设备 ID。

## 目录与开始约定

本卡只规划，未创建 TASK-020 源码或切换入口。明确开启后先检查拟用目录 `WpfBae/Exercises/TASK-020-device-combobox/`，按现有项目惯例创建任务窗口；保留 TASK-019 原作品。需要的两个字符串列表在新任务窗口后台准备，不修改原 ViewModel；核心选中值连接留 TODO。

开始时复用并再次核对 [ReadingViewModel.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[ReadTemperatureCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs) 与 [TASK-019 表格](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml)。共享 App.xaml 只做必要入口连接；新建前检查目标文件，已有源码和 TODO 不覆盖。启动时登记实际路径、TODO/DoD 对应、首个动作及真实构建命令。

## 验收标准（DoD）

- [ ] 项目编译并实际打开 TASK-020 窗口；下拉框恰有 TEMP-001、TEMP-002 两个选项，默认显示 TEMP-001，能够改选且不允许自由输入。
- [ ] 下拉框选中字符串与原数据对象 DeviceId 正确连接；先后选择两个设备并读取，表格各行三列来自相应 Reading，切换选项不修改旧读数，连续读取追加到同一 Readings 集合。
- [ ] 实际清空使原集合和表格变空，清空按钮条件与提示正确；在当前选择下再次读取恢复，窗口中选择区、表格、按钮与提示可读。

## 可选巩固

- 以后再学习带名称、地址的设备对象选项；本次未做不影响验收。
- 不以本卡通关推定所有选择控件、完整监控界面或 WPF 阶段全部掌握。

## 状态与证据

- 2026-10-13：TASK-019 三个必需 DoD 全通过后，依据计划中的 ComboBox 和已有表格/命令实践只创建本卡。状态 ⚪ 未开始；未准备 TASK-020 源码、未切换 App.xaml、未构建或运行新任务。
