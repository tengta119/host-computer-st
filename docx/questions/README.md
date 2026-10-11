# 问答库

> [返回进度](../learning.md) ｜ 答疑时按实际问题创建分类文件。

| 分类 | 文件 | 待复习 | 已掌握 |
| :--- | :--- | :---: | :---: |
| C# 属性与类型组织 | [csharp-properties.md](./csharp-properties.md) | 3 | 0 |
| C# Dictionary | [csharp-dictionary.md](./csharp-dictionary.md) | 2 | 0 |
| C# 事件 | [csharp-events.md](./csharp-events.md) | 1 | 4 |
| C# Lambda 与 LINQ | [csharp-lambda-linq.md](./csharp-lambda-linq.md) | 1 | 1 |
| C# Task 与 async/await | [csharp-task-async.md](./csharp-task-async.md) | 6 | 1 |
| C# CancellationToken | [csharp-cancellation.md](./csharp-cancellation.md) | 1 | 2 |
| C# using 与资源释放 | [csharp-resources.md](./csharp-resources.md) | 1 | 0 |
| C# JSON 与配置文件 | [csharp-json.md](./csharp-json.md) | 0 | 1 |
| 学习路线 | [learning-route.md](./learning-route.md) | 1 | 0 |
| .NET 项目与解决方案 | [dotnet-projects.md](./dotnet-projects.md) | 1 | 0 |
| WPF 窗口、控件与布局 | [wpf-layout.md](./wpf-layout.md) | 1 | 0 |
| WPF 数据绑定 | [wpf-binding.md](./wpf-binding.md) | 3 | 1 |
| WPF 集合与历史列表 | [wpf-collections.md](./wpf-collections.md) | 0 | 1 |
| WPF 按钮命令 | [wpf-commands.md](./wpf-commands.md) | 1 | 0 |

提问不等于掌握；仅在自主实践/提交检查有应用证据后更新状态。

2026-10-11 TASK-011 最终复查：[WPF 集合 Q1](./wpf-collections.md) 的 Add/Clear 与已绑定列表自动更新已有学员实现及运行证据，按此范围改为已掌握，统计为待复习 0、已掌握 1。骨架中的数据模型、getter 和绑定不计自主设计，元素内部属性通知不随之升级。

2026-10-11：ObservableCollection 与 List 的区别追问已合并到 [WPF 集合 Q1](./wpf-collections.md)，补充相同的保存操作、集合变化事件、普通 List 初始显示与后续新增的差别、元素属性通知边界；统计保持待复习 1、已掌握 0。

2026-10-05：SaveAsync 的“async Task 为什么无需手写 return”追问已合并到 [Task/async 问答 Q6](./csharp-task-async.md)，保持待复习，统计不变。

2026-10-05：TASK-007 的 CancelAfter 追问已去重补充到 [CancellationToken Q1](./csharp-cancellation.md)，定时请求、Token 响应及 await/using 生命周期关系保持待复习，统计不变。

2026-10-05：TASK-007 实践已验证取消角色、Token 传递与作用域，补充终端 catch 的处理边界及短配置下的调度时机；Q1 仍待复习，Q2 仍已掌握，统计不变。

2026-10-05 同轮复跑：终端 catch 重抛已自主修复并验证安全退出，Q1 当前重点为短配置下的取消时机，统计不变。

2026-10-05 TASK-007 最终复查：配置关联的长短取消共 14 次通过，Q1 协作取消链路已有自主应用证据，改为已掌握；本分类统计更新为待复习 0、已掌握 2。不扩展到精确计时或真实设备停止，资源释放问答仍待复习。

2026-10-07：新增 [CancellationToken Q3](./csharp-cancellation.md) 请求取消与 await/catch 的时机区别，状态待复习；保留 Q1、Q2 的已掌握范围，分类统计为待复习 1、已掌握 2。
