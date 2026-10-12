# 问答库

> [返回进度](../learning.md) ｜ 答疑时按实际问题创建分类文件。

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

提问不等于掌握；仅在自主实践/提交检查有应用证据后更新状态。

2026-10-12 TASK-013 最终审查：10 个实际窗口场景全通过，学员自主修复空白分支缺少 return；读取逻辑应用记入进度。ViewModel/绑定/属性通知由骨架提供，[WPF ViewModel Q1](./wpf-mvvm.md) 的机制继续待复习，分类统计不变。

2026-10-12：StatusText 的 OneWay 与 DeviceId 的 TwoWay/UpdateSourceTrigger 使用场景合并至 [WPF ViewModel Q1](./wpf-mvvm.md)，区分方向与控件写回时机，补充 OneWay 也能动态显示及 PropertyChanged/LostFocus 的适用场景。统计、任务状态与 DoD 不变。

2026-10-12：TextBox 已改值为何仍调用 OnPropertyChanged 的追问合并至 [WPF ViewModel Q1](./wpf-mvvm.md)，区分输入写回保存、代码修改属性后的界面更新及其他绑定控件同步；此前讲解未展开修改来源，本轮补充边界，不记错题。统计、任务状态与 DoD 不变。

2026-10-12：输入框改值是否自动更新 ViewModel.DeviceId 的追问合并至 [WPF ViewModel Q1](./wpf-mvvm.md)，以属性赋值效果及完整 setter 说明 value 保存、文字变化时写回和对象通知的分工；只读源码，未运行输入验证，统计与任务状态不变。

2026-10-12：TASK-013 启动必要讲解记录至 [WPF ViewModel Q1](./wpf-mvvm.md)，说明自定义类与 WPF 属性身份、同一对象连接、完整属性写回及 TwoWay/UpdateSourceTrigger 的用途。状态待复习；仅验证骨架构建和启动，未新增自主掌握证据。

2026-10-12：TASK-012 的绑定对象与条件检查链复述已合并至 [WPF 命令 Q1](./wpf-commands.md)，确认 ClearCommand getter 返回原 _clearCommand 对象，并串起集合事件、命令通知、WPF 处理方法与 CanExecute；补充初次连接会检查条件、通知不执行清空及不保证 bool 改变。保留已验证应用范围，机制继续待复习；统计、任务状态与 DoD 不变。

2026-10-12：新增 [WPF 集合 Q2](./wpf-collections.md#q2都有-datacontext--_state为什么-task-010-的数据类实现-inotifypropertychangedtask-011-的不用)，对照 TASK-010 文字属性赋值与 TASK-011 原集合 Add/Clear，区分属性通知和集合通知及通知发送对象；补充更换集合与当前直接更新状态控件的边界。Q2 待复习，集合分类统计为待复习 1、已掌握 1；保留 Q1 已验证范围，两任务通关状态和 DoD 不变。

2026-10-11 TASK-012 最终审查：8 个实际窗口场景全部通过，WPF 命令 Q1 按自主实现的 CanExecute 条件、Execute 清空与提示范围标为已掌握，当前统计为待复习 0、已掌握 1。提供的绑定与事件工具不计自主设计，委托基础和框架订阅原理继续复习；C# 事件 Q6 及分类统计保持原状。下方同日答疑为验收前的历史记录。

2026-10-11：委托直接调用、Invoke 与 ?. 空值检查的区别合并至 [C# 事件 Q6](./csharp-events.md)，使用完整 if 写法对照，分类统计不变。

2026-10-11：EventHandler 委托类型、CanExecuteChanged 事件和 handler 委托变量的身份区别，以及 delegate 定义自定义类型、使用 .NET 已有类型的区别，合并至 [C# 事件 Q6](./csharp-events.md)，WPF 命令 Q1 交叉引用，统计不变。

2026-10-11：通知为什么传 this 的追问已合并至 [WPF 命令 Q1](./wpf-commands.md)，说明 this 作为 sender 报告事件来源、多个来源可共享接收方法，以及接收方可以不使用该参数；统计不变。

2026-10-11：学员对事件通知调用 WPF 按钮处理方法的复述及状态检查链已合并至 [WPF 命令 Q1](./wpf-commands.md)，分类统计不变。

2026-10-11：“WPF 把处理方法订阅到事件上”的含义已补充至 [WPF 命令 Q1](./wpf-commands.md)，从自定义普通处理方法与完整 += new EventHandler(...) 登记讲起，明确登记时不调用、发通知后才调用，再核对框架内部的实际 += 连接。命令分类统计仍待复习 1、已掌握 0。

2026-10-11：为何不用直接调用、委托是否只能引用一个方法的追问已合并进 [C# 事件 Q6](./csharp-events.md)，补充读取流程由调用者传入显示或报警处理方法的实际用途，以及 += 组合多个方法；固定 ShowMessage 示例用途不充分是教学示例问题，不记为学员错误。统计保持待复习 2、已掌握 4。

2026-10-11：已注册的处理方法具体是什么的追问合并至 [WPF 命令 Q1](./wpf-commands.md)，核对官方源码明确事件直接订阅内部转发方法 HandlerSink.OnCanExecuteChanged，再转给按钮 ButtonBase.OnCanExecuteChanged；这些方法来自框架，不能把它们与学员 CanExecute 混为一谈，统计不变。

2026-10-11：新增 [C# 事件 Q6](./csharp-events.md)“委托是什么，为什么 handler 变量可以像方法一样调用”，从普通方法→自定义 delegate 类型→完整 new 创建与调用→EventHandler 的参数要求补充必要基础。状态待复习，分类为待复习 2、已掌握 4；保留 TASK-002 已验证的事件应用，不把使用过 Lambda 当作全部委托概念已掌握。

2026-10-11：command 是否由事件参数 this 取得的追问已合并到 [WPF 命令 Q1](./wpf-commands.md)，区分 ClearCommand 属性取对象与 sender 接收通知发送者；核对 WPF 官方 ButtonBase/CommandHelpers 源码，确认重查条件使用按钮已绑定的 Command，统计仍待复习 1、已掌握 0。

2026-10-11：CanExecuteChanged 是否指向 CanExecute、是否类似 Java 反射的追问已合并至 [WPF 命令 Q1](./wpf-commands.md)，明确命令事件调用 WPF 注册的处理方法，随后通过 ICommand 调用条件方法；用监听器/回调作类比，统计仍待复习 1、已掌握 0。

2026-10-11：CanExecuteChanged 声明、EventHandler、局部 handler 调用及 WPF 自动订阅的追问已合并至 [WPF 命令 Q1](./wpf-commands.md)；补充事件声明与发布的区别、重新检查 CanExecute 的调用路径及查询不会触发集合变化的边界，统计仍为待复习 1、已掌握 0。

2026-10-11 TASK-011 最终复查：[WPF 集合 Q1](./wpf-collections.md) 的 Add/Clear 与已绑定列表自动更新已有学员实现及运行证据，按此范围改为已掌握，统计为待复习 0、已掌握 1。骨架中的数据模型、getter 和绑定不计自主设计，元素内部属性通知不随之升级。

2026-10-11：ObservableCollection 与 List 的区别追问已合并到 [WPF 集合 Q1](./wpf-collections.md)，补充相同的保存操作、集合变化事件、普通 List 初始显示与后续新增的差别、元素属性通知边界；统计保持待复习 1、已掌握 0。

2026-10-05：SaveAsync 的“async Task 为什么无需手写 return”追问已合并到 [Task/async 问答 Q6](./csharp-task-async.md)，保持待复习，统计不变。

2026-10-05：TASK-007 的 CancelAfter 追问已去重补充到 [CancellationToken Q1](./csharp-cancellation.md)，定时请求、Token 响应及 await/using 生命周期关系保持待复习，统计不变。

2026-10-05：TASK-007 实践已验证取消角色、Token 传递与作用域，补充终端 catch 的处理边界及短配置下的调度时机；Q1 仍待复习，Q2 仍已掌握，统计不变。

2026-10-05 同轮复跑：终端 catch 重抛已自主修复并验证安全退出，Q1 当前重点为短配置下的取消时机，统计不变。

2026-10-05 TASK-007 最终复查：配置关联的长短取消共 14 次通过，Q1 协作取消链路已有自主应用证据，改为已掌握；本分类统计更新为待复习 0、已掌握 2。不扩展到精确计时或真实设备停止，资源释放问答仍待复习。

2026-10-07：新增 [CancellationToken Q3](./csharp-cancellation.md) 请求取消与 await/catch 的时机区别，状态待复习；保留 Q1、Q2 的已掌握范围，分类统计为待复习 1、已掌握 2。
