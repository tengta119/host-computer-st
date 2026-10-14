# TASK-020：用 ComboBox 选择设备并读取温度

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」的 ComboBox，以及 Binding、Command 与设备监控界面目标。
- 选择依据：TASK-019 的三列表格、逐行字段与同集合追加/清空已实际验收，学员在骨架中自主连接数据源有证据。计划列出的 ComboBox 仍无自主应用证据；下一步把自由输入改为从两个固定设备中选择，复用已完成读数表格与原命令，不叠加实时采集或真实通信。
- 难度/预计时间：待补充（计划未给出）。
- 知识笔记：[设备候选、选中项与 DeviceId 写回](../notes/TASK-020-wpf-device-combobox.md)（2026-10-14 整理；具体代码与效果见正文，历史证据及掌握范围见文末）。

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

2026-10-13 用户明确开启；确认目录此前不存在后，已在 `WpfBae/Exercises/TASK-020-device-combobox/` 创建任务专属窗口。两个字符串选项在新窗口后台准备，核心选中值连接留 TODO。TASK-019 原作品和原 ViewModel/命令文件未修改。

实际路径与产出：

- 新建 [Task020Window.xaml](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml)：设备下拉框、原三列表格、读取/清空按钮和提示。
- 新建 [Task020Window.xaml.cs](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml.cs)：一个原 ReadingViewModel、含两个 ID 的字符串数组、下拉框 ItemsSource 与窗口 DataContext 连接。
- 复用下方 TASK-013 三个业务文件和 TASK-019 已完成的表格连接；原文件未修改。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml) 的 StartupUri 改到 `Exercises/TASK-020-device-combobox/Task020Window.xaml`。项目文件无需修改，现有 WPF 项目自动包含新文件。

| 位置 | 用户动作 | 步骤/DoD 对应 |
| :--- | :--- | :--- |
| XAML TODO 1 | 只把 ComboBox.Width 从 200 改成 260，实际运行观察变宽 | 实践步骤 1；DoD 1/3 的预备观察，不单独通关 |
| XAML TODO 2 | 读完[下拉框讲解](../questions/wpf-combobox.md)，把 SelectedItem 双向连接原 DeviceId | 实践步骤 2；DoD 1 默认选择、DoD 2 写回与读取 |
| 连接完成后 | 依次选择两个设备读取，连续切换读取并核对旧行；实际清空后再读 | 实践步骤 3；DoD 2/3 运行验证，无需新增业务代码 |

首个动作只改 Width。关闭旧程序，在工作区根目录执行 `dotnet run --project WpfBae/WpfBae.csproj`，观察实际 TASK-020 窗口下拉框变宽，不能以设计预览代替。TODO 2 尚未完成时，初始显示为空是骨架占位，展开仍有两项；改选不会写回 DeviceId，原读取仍使用 TEMP-001。完成连接后初始 TEMP-001 应来自同一个数据对象，不另设 SelectedIndex 或另建 ViewModel。

开始时复用并再次核对 [ReadingViewModel.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[ReadTemperatureCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs) 与 [TASK-019 表格](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml)。共享 App.xaml 只做必要入口连接；新建前检查目标文件，已有源码和 TODO 不覆盖。启动时登记实际路径、TODO/DoD 对应、首个动作及真实构建命令。

## 验收标准（DoD）

- [x] 项目编译并实际打开 TASK-020 窗口；下拉框恰有 TEMP-001、TEMP-002 两个选项，默认显示 TEMP-001，能够改选且不允许自由输入。
- [x] 下拉框选中字符串与原数据对象 DeviceId 正确连接；先后选择两个设备并读取，表格各行三列来自相应 Reading，切换选项不修改旧读数，连续读取追加到同一 Readings 集合。
- [x] 实际清空使原集合和表格变空，清空按钮条件与提示正确；在当前选择下再次读取恢复，窗口中选择区、表格、按钮与提示可读。

## 可选巩固

- 以后再学习带名称、地址的设备对象选项；本次未做不影响验收。
- 不以本卡通关推定所有选择控件、完整监控界面或 WPF 阶段全部掌握。

## 状态与证据

- 2026-10-13 审查通关：三个必需 DoD 全部通过，状态 ✅ 已通关。学员在原骨架中将 Width 改为260，添加 `SelectedItem="{Binding DeviceId}"`；实际绑定 Mode=Default，SelectedItem 的框架元数据 BindsTwoWayByDefault=True，改选已写回同一个原 ReadingViewModel，不强制补显式 Mode=TwoWay。
- 实际构建：`dotnet build WpfBae/WpfBae.csproj --nologo` 退出0、0警告/错误。审查命令 `dotnet run --project docx/tasks/evidence/TASK-020/Review.csproj -- D:\host-computer-st\docx\tasks\evidence\TASK-020` 最终退出0，实际Show原编译窗口，13个场景全部通过：默认001、展开两项、001读取、只改选002保留旧行、002追加/连读、切回001读取、清空禁用、空集合改选、002恢复、最小尺寸可读及改选、再次清空。改选通过下拉框展开和项选择的自动化接口，读取/清空通过按钮自动化接口触发，未直接改ViewModel来模拟选择。
- DoD 1通过：原exe实际打开TASK-020并正常关闭退出0，展开时两个选项容器可见；不允许自由编辑，实际控件无可见编辑TextBox，也无可编辑Value模式。DoD 2通过：绑定Active、原数据对象/Readings/命令引用一致，每行三字段来源是相应Reading；前两行分别001/002、旧Reading引用保持，连续追加至4条。DoD 3通过：清空集合/表格均0并禁用，提示“已清空”；当前002再读恢复，提示“读取成功”，最小尺寸420×520可读。
- 证据：[运行结果](./evidence/TASK-020/results.json)、[源码与程序集核对](./evidence/TASK-020/verification.json)、[原程序启动](./evidence/TASK-020/launch.json)、[三条读数](./evidence/TASK-020/history-three-readings.png)、[清空](./evidence/TASK-020/history-cleared.png)、[最小尺寸](./evidence/TASK-020/minimum-size.png)。原45个源码SHA256不变，审查加载程序集与原DLL哈希一致。审查工具首次编译因误用不存在的ComboBoxItemAutomationPeer失败，工具修正为实际项AutomationPeer后运行通过，不计学员失败。骨架初始提示仍含“尚未连接”旧文案，来源是教练准备的提示，不记学员错题；读取/清空业务提示正确，未追加验收要求或代改源码。
- 掌握范围：[下拉框Q1](../questions/wpf-combobox.md)按骨架中选中字符串连接、默认选择与写回后读取的自主应用标已掌握；选项数组、ItemsSource、DataContext、原命令/表格来自骨架/既有作品，不扩大到独立设备对象设计或完整框架机制。只创建下一张[TASK-021：用 Grid 组织设备读取窗口](./TASK-021.md)（⚪ 未开始），依据计划Grid和布局仍待复习的证据；未准备源码或切换App.xaml。下方启动/未运行描述为此前历史。
- 2026-10-13 启动：状态 🟡 进行中；两个专属骨架文件和共享入口已准备，原表格/业务命令复用，选中值连接尚未完成。必要讲解见 [WPF 下拉框 Q1](../questions/wpf-combobox.md)（待复习）。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0、0 警告/错误；未运行窗口或验收，三个 DoD 均未勾选，不新增自主掌握或错题。下方仅创建卡片的描述为此前规划历史。
- 2026-10-13：TASK-019 三个必需 DoD 全通过后，依据计划中的 ComboBox 和已有表格/命令实践只创建本卡。状态 ⚪ 未开始；未准备 TASK-020 源码、未切换 App.xaml、未构建或运行新任务。
