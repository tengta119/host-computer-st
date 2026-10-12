# TASK-017：用 ControlTemplate 定义按钮内部外观

- 阶段：第二阶段：WPF（起步）
- 状态：⚪ 未开始
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」的 Style / Template。本卡继续控件模板；数据图表仍未覆盖。
- 选择依据：TASK-015 的共用 Style 和 TASK-016 的数据行模板已实际验收，数值路径与行内间距已自主修复。继续学习按钮本身如何由内部界面组成，沿用原命令与数据，避免同时引入通信或图表依赖；保留已有 C# 与 WPF 机制复习范围。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

在设备监控窗口里，让读取和清空按钮共用一个简单的圆角边框外观，显示各自原 Content，保留原 Padding、命令以及清空禁用/恢复。历史列表继续使用 TASK-016 的三字段 DataTemplate。

## 开始前先解释

- ControlTemplate 是 WPF 提供的控件内部界面描述；Button.Template 是接收它的属性。TASK-016 的 DataTemplate 展示一条 Reading，本卡的 ControlTemplate 展示一个 Button 的内部结构，逐个对应真实对象，不只列名词。
- Border 是 WPF 容器，可放一个内部元素并显示边框、背景、圆角；CornerRadius 控制圆角，BorderBrush/BorderThickness 控制边框。先只改变一个可见属性观察，不一次要求完整模板。
- ContentPresenter 是 WPF 的内容显示控件，在按钮模板中展示原按钮 Content。因此“读取温度”和“清空历史”仍来自各自 Button，不把两份文字写死在共享模板里。
- TemplateBinding 是 WPF 模板内部读取所应用控件属性的写法；例如内部 Border 的 Padding 读取外部 Button.Padding。此处来源是正在应用模板的 Button，与历史行 Binding 读取 Reading、窗口 Binding 读取 ViewModel 分开解释。
- 共享 Style 可通过 Template Setter 赋一个 ControlTemplate。Setter.Value 是把对象作为 Setter 的值的 XAML 写法，不是新的可见控件；先解释与已有普通 Setter 的对应关系再给最小例子。
- 自定义模板会替换原内部外观，需要明确保留禁用的可见区别。后续才解释 Trigger：当 Button.IsEnabled 为 False 时设置较低 Opacity，启用时恢复；它只设置外观，不执行命令或改历史。IsEnabled 已有运行证据，Trigger/Opacity 为新概念，须逐项解释。

## 用户实践与产出

1. 在教练提供的可运行模板连接上，先将共享 Border 的 CornerRadius 改为一个容易观察的值，运行确认两个按钮外观同步变化。
2. 补齐内容展示及必要模板属性连接，使各按钮显示原 Content，内部留白取得共同 Style 的 Padding，保留原 Command。
3. 补禁用外观设置，实际确认初始清空禁用、读取后启用、清空后禁用、再次读取恢复，并验证历史模板与空白保护保持。

## 目录与开始约定

本卡仅规划，未创建 TASK-017 源码或切换入口。用户明确开启后，先检查目标目录 `WpfBae/Exercises/TASK-017-button-controltemplate/`、当前源码树及运行入口，保留 TASK-016 学员作品。按现有惯例准备任务专属窗口/历史控件，复用原 ReadingViewModel、命令和 Reading，并保留已验收的三字段历史模板。共享 App.xaml 仅在启动时作必要入口连接，不引入框架或整套新依赖。

教练提供最小可运行外观及共享资源连接，关键内容/属性连接与禁用外观留作中文 TODO；不能让首步运行只出现无内容的空白按钮。开始时登记实际文件路径、共享入口例外、TODO 与 DoD 对应、真实构建证据。首个动作只改一项圆角并观察实际窗口；后续概念解释后再要求使用。

## 验收标准（DoD）

- [ ] 项目编译并实际打开任务窗口；两个按钮通过共享 Style 使用同一个自定义 ControlTemplate，内部边框/圆角可见，各自原 Content 由 ContentPresenter 显示，必要 Padding 连接有效。
- [ ] 实际读取、换 ID、只改输入不改旧行及两种空白保护保持；历史 ListBox 的三字段 DataTemplate、原 Readings 集合与窗口输入/状态/命令绑定正常。
- [ ] 初始清空禁用、读取启用、实际清空后列表为空并禁用、再次读取恢复；禁用与启用有清楚外观区别，正常状态文字与布局可读，有实际窗口证据。

## 可选巩固

- 后续可加鼠标悬停或按下状态外观；本次不要求动画或完整主题，未做不勾选。
- 不以本卡通关推定完整控件设计、资源机制或 WPF 阶段全部掌握。

## 状态与证据

- 2026-10-12：TASK-016 三个必需 DoD、10 个实际窗口场景全部通过后，按计划只创建本卡。状态 ⚪ 未开始；未准备源码、未切换 App.xaml、未构建或验收本任务。

参考：[Microsoft WPF 样式与模板说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/styles-templates-overview)。
