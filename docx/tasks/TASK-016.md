# TASK-016：用 DataTemplate 展示结构化读取历史

- 阶段：第二阶段：WPF（起步）
- 状态：⚪ 未开始
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

本卡仅规划，尚未创建 TASK-016 源码或切换入口。用户明确要求“开启 TASK-016”后，先检查 `WpfBae/Exercises/TASK-016-reading-datatemplate/` 是否已存在，再创建任务专属窗口/历史控件最小骨架。保留 TASK-015 学员作品，复用原 ReadingViewModel、命令及 Reading；已有按钮 Style 与资源连接由教练沿用，行模板内的文字和字段绑定留给学员。

开始时登记实际文件、共享 App.xaml 入口例外、TODO 与 DoD 对应及真实构建结果。首个动作只完成模板中设备 ID 的一项显示，读取 TEMP-001 再换 TEMP-002，观察两行各自保留 ID；之后才补数值、单位和间距。初始化本卡不开始任务，不将骨架或预览当作运行证据。

## 验收标准（DoD）

- [ ] 项目编译并实际打开任务窗口；历史 ListBox 使用一个 DataTemplate，三个文字控件通过各自 Binding 显示每条 Reading 的 DeviceId、Value、Unit，显示路径不依靠拼接整行或调用 ToString。
- [ ] 实际连续读取并换 ID 后，新增行显示当前读数，旧行保持原 ID/数值/单位；各部分文字可读且间距正常，ItemsSource 仍引用原 Readings。
- [ ] 两种空白输入提示且不改变历史；实际清空后列表为空、清空禁用，再次读取恢复。窗口原输入/状态/命令绑定及两个按钮的共用样式保持有效，有实际运行证据。

## 可选巩固

- 对照具体文件区分 ItemTemplate、ItemsSource 和每一行的 Binding 来源；不单设口试。
- 暂不要求修改历史记录的属性后实时刷新，不默认 Reading 已支持属性变更通知。

## 状态与证据

- 2026-10-12：TASK-015 三个必需 DoD、9 个实际窗口场景与共用 Setter 布局对比全部通过后，按计划只生成本卡。状态 ⚪ 未开始；未准备 TASK-016 源码、未切换 App.xaml、未构建或验收本任务。
