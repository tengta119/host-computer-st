# 问答库

> [返回进度](../learning.md) ｜ [当前教学规则](../../AGENTS.md)

这里保留你的原问题、Q 编号及追问。回答先说原练习里的具体对象和现象，再给少量代码、逐处解释含义，最后说明可观察结果；历史验收与掌握边界放到分类文末。

2026-10-13 已重构全部 22 个分类文件。重构时保留原统计；此后TASK-020实际审查通过，[设备下拉框Q1](./wpf-combobox.md)按骨架中SelectedItem连接/写回读取的应用证据标已掌握，当前待复习30、已掌握19。默认双向元数据和完整框架机制不一并升级。文档重构本身没有新增验收；本次TASK-020运行证据见对应任务卡。

2026-10-13 TASK-021 启动：已在[布局 Q1](./wpf-layout.md)增量说明当前窗口三个区域、Auto/星号行、行号、表格固定高度、内外 Grid 与首个行高观察。Q1 仍待复习，条目数量与统计不变；骨架只构建，未运行或验收。

| 分类 | 文件 | 待复习 | 已掌握 |
| :--- | :--- | :---: | :---: |
| C# 属性与类型组织 | [csharp-properties.md](./csharp-properties.md) | 3 | 0 |
| C# Dictionary | [csharp-dictionary.md](./csharp-dictionary.md) | 2 | 0 |
| C# 事件 | [csharp-events.md](./csharp-events.md) | 2 | 4 |
| C# Lambda 与 LINQ | [csharp-lambda-linq.md](./csharp-lambda-linq.md) | 1 | 1 |
| C# Task 与 async/await | [csharp-task-async.md](./csharp-task-async.md) | 6 | 1 |
| C# CancellationToken | [csharp-cancellation.md](./csharp-cancellation.md) | 1 | 2 |
| C# using 与资源释放 | [csharp-resources.md](./csharp-resources.md) | 1 | 0 |
| C# JSON 与配置文件 | [csharp-json.md](./csharp-json.md) | 0 | 1 |
| 学习路线 | [learning-route.md](./learning-route.md) | 1 | 0 |
| .NET 项目与解决方案 | [dotnet-projects.md](./dotnet-projects.md) | 1 | 0 |
| WPF 窗口、控件与布局 | [wpf-layout.md](./wpf-layout.md) | 1 | 0 |
| WPF 数据绑定 | [wpf-binding.md](./wpf-binding.md) | 3 | 1 |
| WPF 集合与历史列表 | [wpf-collections.md](./wpf-collections.md) | 1 | 1 |
| WPF 按钮命令 | [wpf-commands.md](./wpf-commands.md) | 0 | 1 |
| WPF 窗口与 ViewModel | [wpf-mvvm.md](./wpf-mvvm.md) | 1 | 0 |
| WPF 界面区域与 UserControl | [wpf-usercontrol.md](./wpf-usercontrol.md) | 1 | 0 |
| WPF 样式与资源 | [wpf-styles.md](./wpf-styles.md) | 1 | 1 |
| WPF 数据模板 | [wpf-templates.md](./wpf-templates.md) | 0 | 2 |
| WPF 控件模板 | [wpf-controltemplates.md](./wpf-controltemplates.md) | 2 | 2 |
| WPF 坐标与折线 | [wpf-charts.md](./wpf-charts.md) | 2 | 0 |
| WPF 读数表格 | [wpf-datagrid.md](./wpf-datagrid.md) | 0 | 1 |
| WPF 设备下拉框 | [wpf-combobox.md](./wpf-combobox.md) | 0 | 1 |

## 阅读与运行说明

先打开与你的问题对应的分类，再读该 Q。同一个主题的追问放在该问题里；不必先读历史日志或名词表。代码中自定义类/变量与 .NET/WPF 类型会就地说明，完整普通写法先于简写。

旧任务的修改和界面现象是当时练习的说明，不要求重做已经通关的作品。当前 App.xaml 启动 TASK-021；直接运行 WpfBae 项目会打开当前任务，不能把它当成任意历史窗口的运行结果。需要复查旧任务时按对应任务卡读取真实入口。

未运行的代码效果均是说明或预期；编辑器提示、设计预览、项目编译与实际运行证据分别看待。没有因文字重写更改掌握状态；后续实践审查则按真实新证据增量更新。

## 原稿与历史记录

[重构备份说明](../questions-backups/before-plain-rewrite-2026-10-13/manifest.md)列出原稿与核对记录。原文件名、Q 标题和当前链接保留；旧问答索引中的逐次日志完整保存在[原索引](../questions-backups/before-plain-rewrite-2026-10-13/README.md)，不再放在当前阅读入口前面。

已失效的独立 Device.cs/Reading.cs 链接改为当前 Task001Exercise.cs。历史问答中 WpfPractice 是当时拟用名称，当前真实项目为 WpfBae；仅修正文档表述，没有创建第二套项目或搬移作品。
