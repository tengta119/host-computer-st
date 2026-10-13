# TASK-011：用 ObservableCollection 显示温度读取历史

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中 Binding 后的 ObservableCollection；具体场景及验收按当前实践设计，原文未指定此任务。
- 选择依据：TASK-010 已验证单条结果/状态的绑定与属性变化通知。监控窗口还需要保留多条读数，下一步从一段文字推进到历史列表，学习集合的变化如何更新界面，继续在实践中巩固属性基础。
- 难度/预计时间：待补充（计划未给出）。
- 知识笔记：[ObservableCollection 与温度读取历史](../notes/TASK-011-wpf-reading-history.md)（2026-10-13 按当前源码、本卡与问答详细整理；仅静态核对，未重新运行，通关状态与原掌握范围保留）。

## 场景与目标

操作员输入设备 ID，点击读取后，将一条固定模拟温度追加到窗口的历史列表。重复读取或改变 ID 后，原记录保留，新记录依次增加；点击清空按钮后，列表清空且仍可继续读取。

新任务开始时必须先解释，再要求使用：

- ObservableCollection<T> 是 .NET 提供的集合类，可保存多个 T 类型的对象，并在新增、删除、清空项目时发出集合变化通知。结合已有 List<T> 经验说明差别，不默认学员已学会。
- ListBox 是 WPF 提供的列表控件，逐项显示对象。本次先使用读数对象的 ToString 展示文本，不要求 DataTemplate。
- ItemsSource 是列表控件用于指定数据集合的属性；通过已有 Binding 概念，把它连接到数据对象公开的历史集合属性。
- 集合增删通知与 TASK-010 的单个属性变化通知职责不同；不扩展到修改已有读数对象内部属性自动刷新的要求。

沿用 WpfBae、同步模拟温度和输入校验，不引入通信、异步、Command 或 MVVM 框架。C# 新语法先使用完整普通写法；自定义数据类、字段与辅助方法都在准备骨架时交代来源和用途。

## 用户实践与产出

1. 数据对象包含一项公开的 ObservableCollection<Reading> 历史集合，窗口设置正确 DataContext。
2. XAML 包含设备 ID 输入、读取按钮、历史 ListBox、清空按钮及状态显示，ListBox.ItemsSource 绑定到该集合。
3. 正常读取时构造当前 ID、固定温度和单位的 Reading，添加到同一个集合，列表保留旧记录并出现新记录；不直接调用 ListBox.Items.Add，不反复重设 ItemsSource/DataContext 代替集合通知。
4. 空白输入只提示，不新增成功记录或清除已有历史；正常输入仍可继续新增。
5. 清空按钮清空同一个集合，界面随之清空；再次读取可以重新显示新记录。

## 目录与开始约定

2026-10-11 按用户“开始 task011”准备骨架，专属目录为 `WpfBae/Exercises/TASK-011-reading-history/`：

- [ReadingHistoryState.cs](../../WpfBae/Exercises/TASK-011-reading-history/ReadingHistoryState.cs)：教练新建的数据类，内部创建空集合，通过完整 getter 的 Readings 属性返回同一个集合。数据类名称、字段名和属性名均为自定义。
- [Task011Window.xaml](../../WpfBae/Exercises/TASK-011-reading-history/Task011Window.xaml)：窗口、输入框、按钮、历史列表和提示；ListBox.ItemsSource 的 Binding 已由骨架连接到 Readings。
- [Task011Window.xaml.cs](../../WpfBae/Exercises/TASK-011-reading-history/Task011Window.xaml.cs)：实例创建、DataContext 连接、已有输入校验和读数构造由骨架提供；新增、清空及相应成功提示保留 TODO。
- 复用 [TASK-010 的 Reading](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)，包括已有 ToString；通过 using 引入其命名空间，不复制或改动旧文件。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml) 仅将 StartupUri 切换到 TASK-011；TASK-009/010 学员作品保留。状态提示直接设置 Text，沿用 TASK-009 已验证方法；本任务要求集合列表使用 Binding，不追加另一套属性通知骨架。

### 先理解本次的数据路径

`ObservableCollection<Reading>` 来自 .NET 的 `System.Collections.ObjectModel`。尖括号内的 Reading 表示集合里保存读数对象；`new ObservableCollection<Reading>()` 创建一份空集合。与已有 List<T> 的新增用法相似，但这个集合还能在 Add/Clear 时发出集合变化通知，让 WPF 列表知道记录变了，无需自己声明这个通知事件。

`ReadingHistoryState` 是教练自定义的普通 C# 类，不是框架 API。`new ReadingHistoryState()` 创建一个对象，窗口字段 `_state` 保存这个对象；对象内部字段 `_readings` 保存历史集合。读取 `_state.Readings` 会执行 `get { return _readings; }`，得到原集合，而不是创建副本。这里没有 setter，仍能对取得的集合新增和清空。

`ListBox` 是 WPF 提供的列表控件；它的 `ItemsSource` 属性指定用于显示的集合。`DataContext = _state` 告诉窗口内的 Binding 去哪个对象找属性；`ItemsSource="{Binding Readings, Mode=OneWay}"` 从该对象读取 Readings，取得同一个历史集合。本次 ListBox 使用 Reading 已有的 ToString 展示 ID、温度及单位。

按钮中的 `_state.Readings.Add(reading)` 表示：取得集合，再调用它的 Add 方法，将变量 reading 指向的读数对象追加进去。集合发出新增通知，ListBox 增加一行。Clear() 清除集合中的全部记录，列表收到通知后清空；集合对象仍然存在，之后还可 Add。TASK-010 的 PropertyChanged 通知的是某个属性值改变；本次自动通知的是集合中记录增删，不要求已有记录的内部属性变化自动刷新。

### TODO 与实践/验收对应

| TODO | 文件与操作 | 对应范围 |
| :--- | :--- | :--- |
| 1 | ReadButton_Click 中向 `_state.Readings` Add 刚创建的 reading | 实践 3；DoD 1 同集合、DoD 2 历史追加 |
| 2 | 新增之后把占位状态改为成功提示 | 实践 3 的操作反馈，不单设验收项 |
| 3 | ClearButton_Click 中 Clear 原集合 | 实践 5；DoD 3 清空及恢复 |
| 4 | 清空之后把占位状态改为已清空提示 | 实践 5 的操作反馈，不单设验收项 |

首个动作只完成 TODO 1：在 `Task011Window.xaml.cs` 的 ReadButton_Click 中，读数对象创建之后添加一行 `_state.Readings.Add(reading);`。这是本次新 API 的最小示例，其余 TODO 留给后续实践。运行 WpfBae 项目（IDE 选择该项目后 F5，或在工作区执行 `dotnet run --project .\WpfBae\WpfBae.csproj`），点击“读取温度”，实际窗口列表应出现 `TEMP-001 26.5 °C`；再点击应保留第一行并增加第二行。未完成 TODO 2 时占位提示仍会保留，不影响先观察新增。XAML 设计预览不作为运行证据。

## 验收标准（DoD）

- [x] 项目可编译并实际打开任务窗口；ListBox.ItemsSource 通过 Binding 连接到 DataContext 上的 ObservableCollection<Reading>，数据来源为按钮修改的同一个集合。
- [x] 连续两次有效读取后，列表出现两条完整的 ID、温度和单位，修改 ID 后新记录使用新 ID，旧记录仍保留；集合变化通知驱动列表更新，无直接改控件项目或反复重设来源的替代路径。
- [x] 空字符串/纯空格输入有提示且历史数量不变；清空后集合与列表都为空，再次有效读取可重新增加记录。

## 可选巩固

- 观察列表中记录数量与集合 Count 的对应。
- 结合后续实践区分“集合增删”与“集合中某条读数的属性变化”，不追加本次必需验收。

## 状态与证据

- 2026-10-11 最终审查：学员已补充 `_state.Readings.Add(reading)`、`_state.Readings.Clear()` 及“读取成功”/“已清空”提示。全部必需 DoD 有代码与运行证据，状态 ✅ 已通关。审查未修改学员源码，5 个相关文件运行前后 SHA256 一致；证据见 [运行场景](./evidence/TASK-011/results.json) 与 [构建、原应用启动和源码哈希](./evidence/TASK-011/verification.json)。
  - DoD 1 **通过**：实际执行 `dotnet build .\WpfBae\WpfBae.csproj --no-restore`，0 警告/错误；原 WpfBae.exe 启动标题“TASK-011：温度读取历史”，正常退出码 0。临时 WPF 审查程序引用原程序集，实际显示 Task011Window，确认 DataContext 与处理器字段为同一个 ReadingHistoryState，ItemsSource OneWay Binding 路径为 Readings，来源与按钮修改的集合相同。
  - DoD 2 **通过**：在实际窗口触发原按钮的 Click 路由事件，连续两次 TEMP-001 读取后集合/列表均为 2 条；更换 TEMP-002 后均为 3 条，前两条保留。生成的界面行文本均含当前 ID、26.5、°C；每次新增收到 Add 通知。集合、DataContext、ItemsSource 和 BindingExpression 引用全过程保留，源码没有直接修改列表项目或重设数据来源。
  - DoD 3 **通过**：空字符串与纯空格分别有输入提示，无集合通知，原 3 条记录保留；清空后集合/列表均为 0，收到 Reset 通知；输入两侧带空格的 TEMP-003 再读取后均为 1，ID 已 Trim，显示完整且收到 Add 通知。
  - 掌握范围：验证学员在已提供的绑定和集合骨架上使用 Add/Clear 及成功提示。输入校验、getter、集合创建、绑定与窗口结构由骨架提供，不扩大为自主设计整个绑定模型或掌握所有集合事件；集合中已有对象内部属性自动刷新仍未验证。
  - 下一步依据：历史集合增删已验证，可按计划 ObservableCollection → Command 继续学习按钮动作与执行条件。仅创建 [TASK-012](./TASK-012.md)，状态 ⚪ 未开始；不创建新源码、不切换入口，不宣称 WPF 阶段整体完成。
- 2026-10-11 过程答疑：已将“ObservableCollection 与 List 有什么区别”合并进 [WPF 集合 Q1](../questions/wpf-collections.md)，说明共同的集合操作、内置 CollectionChanged 通知及列表连接后的差异，并明确不自动转发元素内部属性变化。本轮仅更新记录，未修改或运行源码，不新增验收证据，任务状态及 DoD 保持不变。
- 2026-10-09：TASK-010 三项必需 DoD 通过后，按计划 Binding → ObservableCollection 创建这一张递增 ID 任务卡，状态 ⚪ 未开始。未创建新骨架、未切换入口、未运行或验收 TASK-011；新增概念须先讲来源、用途、普通语法和可观察现象，不把现有属性骨架视为自主设计能力。
- 2026-10-11：用户明确要求开始 TASK-011。已准备上述三个专属文件并切换 App.xaml，状态 🟡 进行中。实际执行 `dotnet build .\WpfBae\WpfBae.csproj --no-restore`，0 警告、0 错误；通过 PowerShell Start-Process 启动编译后的原应用，确认主窗口标题为“TASK-011：温度读取历史”，CloseMainWindow 后退出码 0。本次只验证骨架编译和启动，Add/Clear 仍是 TODO，未验证学员新增、清空、恢复行为，三项必需 DoD 均保持未勾选。集合创建、完整 getter、绑定与输入校验由教练提供，不计新增自主掌握证据。
