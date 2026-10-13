# 知识笔记索引

> [返回学习导航](../README.md) ｜ [学习进度](../learning.md) ｜ [任务索引](../tasks/README.md)
>
> 笔记详细解释任务代码与相关知识；状态和验收以任务卡、学习进度为准。阅读笔记不等于新增掌握证据。

| 任务 | 笔记 | 内容 | 整理日期 |
| :--- | :--- | :--- | :--- |
| [TASK-009：用 WPF 窗口与按钮事件显示设备读数](../tasks/TASK-009.md) | [WPF 窗口、布局与按钮事件](./TASK-009-wpf-window-events.md) | 项目入口、XAML/后台连接、控件与布局、Click 订阅、输入校验、Reading/属性语法、显示流程与历史验证边界 | 2026-10-13 |
| [TASK-010：用 Binding 更新模拟温度读数](../tasks/TASK-010.md) | [Binding、DataContext 与属性变化通知](./TASK-010-wpf-binding.md) | 自定义数据对象、完整属性语法、数据上下文、OneWay/Path、接口/事件/参数类、通知时序、真实分支、绑定保留、Rider 提示与证据边界 | 2026-10-13 |
| [TASK-011：用 ObservableCollection 显示温度读取历史](../tasks/TASK-011.md) | [ObservableCollection 与温度读取历史](./TASK-011-wpf-reading-history.md) | 泛型集合、完整 getter、同一对象与集合、ListBox/ItemsSource、Add/Clear、集合与属性通知、空白保留历史、状态显示及验证边界 | 2026-10-13 |
| [TASK-012：用 Command 清空历史并控制按钮是否可用](../tasks/TASK-012.md) | [ICommand、按钮可用条件与清空历史](./TASK-012-wpf-commands.md) | 接口与实现、同一状态/集合/命令、完整 getter、Command/OneWay、CanExecute/Execute、委托与事件、订阅/发布、this/sender、WPF 重查链、三种通知及验收边界 | 2026-10-13 |
| [TASK-013：把读取逻辑从窗口移到 ViewModel](../tasks/TASK-013.md) | [ViewModel、双向输入绑定与读取命令](./TASK-013-wpf-viewmodel.md) | MVVM 角色、同一对象/双命令、DeviceId 完整属性、TwoWay/写回时机、属性通知用途、读取命令、Trim/return、当前输入与历史、通知链及修复/证据边界 | 2026-10-13 |
| [TASK-014：用 UserControl 封装读取历史区域](../tasks/TASK-014.md) | [UserControl 封装、DataContext 继承与历史区域复用](./TASK-014-wpf-usercontrol.md) | 区域封装、类/实例复用、x:Class/partial/初始化、命名空间前缀、同对象继承、ItemsSource/Command、名称范围、布局、通知链及本地历史证据核对 | 2026-10-13 |
| [TASK-015：用 Style 统一读取与清空按钮的外观](../tasks/TASK-015.md) | [Style、Setter 与按钮外观资源](./TASK-015-wpf-styles.md) | Padding/对齐、显式样式、资源键与作用域、StaticResource/DynamicResource、控件初始化时机、直接属性优先级、同一 Style、业务绑定及历史布局/验收证据 | 2026-10-13 |
| [TASK-016：用 DataTemplate 展示结构化读取历史](../tasks/TASK-016.md) | [DataTemplate、逐行数据绑定与文字间距](./TASK-016-wpf-datatemplate.md) | Reading/初始化、原集合、ItemsSource/ItemTemplate、行内数据源、同名 DeviceId、字段路径/OneWay、横向布局、Margin、路径与间距自主修复、通知分工及历史证据 | 2026-10-13 |
| [TASK-017：用 ControlTemplate 定义按钮内部外观](../tasks/TASK-017.md) | [ControlTemplate、属性连接与禁用外观](./TASK-017-wpf-controltemplate.md) | Template Setter、Border/圆角、ContentPresenter 默认/显式内容连接、TemplateBinding/Padding、模板实例与名称范围、Trigger/Opacity、命令状态链、原数据模板及历史诊断证据 | 2026-10-13 |
