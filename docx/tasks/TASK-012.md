# TASK-012：用 Command 清空历史并控制按钮是否可用

- 阶段：第二阶段：WPF（起步）
- 状态：🟡 进行中
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中 ObservableCollection 后的 Command；具体场景与验收按当前实践设计。
- 选择依据：TASK-011 已验证同一集合的 Add/Clear、列表自动更新及空白保护。下一步让“清空历史”既描述执行动作，又能说明当前是否允许执行，沿用熟悉场景，只引入必要的命令知识。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

历史为空时，“清空历史”按钮禁用；读取产生记录后按钮自动可用；执行清空后历史消失，按钮再次禁用，重新读取仍能恢复可用。读取按钮继续沿用已学的 Click 处理器。

## 开始前先解释

- 先对照已有 Click：现在点击按钮执行窗口中的处理方法。Command 则让按钮连接一个对象，该对象负责回答“执行什么”与“现在能否执行”。不是 C# 关键字，也不是要求先使用整套 MVVM 框架。
- `ICommand` 是 .NET 提供、WPF 使用的接口，位于 `System.Windows.Input`。先结合 TASK-008 的接口经验，逐个讲 Execute、CanExecute 与 CanExecuteChanged 的来源及职责，不一次堆叠未解释的简写。
- Execute 是执行动作的方法，本任务动作是 Clear 原历史集合；CanExecute 返回 bool，本任务判断历史数量是否大于 0；CanExecuteChanged 是通知按钮重新检查可用状态的事件，不是执行清空动作。
- `Button.Command` 是 WPF 属性；通过已接触的 Binding 从 DataContext 取得命令对象。按钮依据 CanExecute 更新可用状态，并在正常点击时调用 Execute。
- 命令实现类、公开命令属性名、字段与通知辅助方法均为教练自定义，开始时明确说明它们如何创建、持有哪个历史数据对象，和已有集合怎样连接。先用完整普通语法，不要求自写通用 RelayCommand 或 Lambda 工具。

## 用户实践与产出

1. 新任务数据对象保留历史集合，并公开清空命令对象；窗口与命令使用同一份集合。
2. 命令实现中根据历史 Count 判断是否允许执行；执行动作清空原集合并给出完成提示。
3. 清空按钮 Command 绑定到公开命令属性，移除该按钮原 Click 动作；读取仍使用 Click，保留输入校验和新增历史功能。
4. 集合从空变为非空、从非空变为空时，通知按钮重新检查 CanExecute。开始时提供必要的集合事件连接与通知工具骨架，核心判断与动作留给学员。
5. 观察初始禁用、读取后启用、清空后禁用、再次读取后启用；空白输入提示且保留原历史。

## 目录与开始约定

2026-10-11 按用户“开启 task012”开始任务，专属目录为 `WpfBae/Exercises/TASK-012-clear-history-command/`，已创建：

- [ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-012-clear-history-command/ClearHistoryCommand.cs)：自定义命令类，实现框架 ICommand；核心判断、清空与完成提示留 TODO 1～3，事件通知工具由教练提供。
- [ReadingCommandState.cs](../../WpfBae/Exercises/TASK-012-clear-history-command/ReadingCommandState.cs)：自定义数据类，保存历史、状态和命令对象；完整 getter/setter、StatusText 属性通知、集合事件订阅及转发命令条件变化通知由骨架提供。
- [Task012Window.xaml](../../WpfBae/Exercises/TASK-012-clear-history-command/Task012Window.xaml)：布局、输入、列表和状态绑定复用已有做法，清空按钮 Command 已绑定到 ClearCommand，没有清空 Click 处理器。
- [Task012Window.xaml.cs](../../WpfBae/Exercises/TASK-012-clear-history-command/Task012Window.xaml.cs)：创建数据对象并设置 DataContext，复用已验证的输入校验、模拟读数及 Add 操作。
- 复用 [TASK-010 的 Reading](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)，保留 TASK-010/011 原文件；共享入口例外：[App.xaml](../../WpfBae/App.xaml) 仅切换 StartupUri 到 TASK-012。没有新增项目依赖。

### 先理解对象和调用路径

Command 在这里是“由按钮使用的命令对象”。框架的 ICommand 接口规定它要提供判断和执行的方法，以及条件变化通知事件。`ClearHistoryCommand : ICommand` 表示我们自定义的类实现这个接口；名称 ClearHistoryCommand 由教练取，CanExecute/Execute/CanExecuteChanged 的名称来自接口约定。

窗口通过 `new ReadingCommandState()` 创建一个数据对象，字段 `_state` 保存它。数据对象创建原历史集合，然后执行 `new ClearHistoryCommand(this)`：这里 this 是当前数据对象本身，作为构造函数参数传给命令。命令内部的 `_state` 保存传来的对象，窗口字段和命令字段因此指向同一个数据对象；命令取得的 Readings 就是读取按钮 Add 的同一个集合。

数据类的 ClearCommand 是自定义公开属性，声明类型为 ICommand，完整 getter 返回创建好的 ClearHistoryCommand 对象。窗口 `DataContext = _state` 之后，按钮的 `Command="{Binding ClearCommand, Mode=OneWay}"` 从这个对象取得 ClearCommand 属性。Button.Command 是 WPF 属性，Binding 路径 ClearCommand 是我们自己取的名字；无需把命令对象的类名写成绑定路径。

三个成员各有作用：

| 成员 | 谁调用/触发 | 本任务作用 |
| :--- | :--- | :--- |
| CanExecute | WPF 按钮询问 | 返回 bool：true 可用，false 禁用；判断历史 Count 是否大于 0 |
| Execute | 按钮可用并正常点击时由 WPF 调用 | 清空集合，更新完成提示 |
| CanExecuteChanged | 命令发出，按钮监听 | 告诉按钮“条件可能变了，请重新调用 CanExecute”；本身不清空记录 |

`public bool CanExecute(object? parameter)` 是普通方法声明：bool 表示返回 true/false；object? parameter 是接口规定的参数，? 允许它为 null。本任务没有设置命令参数，正常按钮调用时它为 null，无需使用。`Count` 是集合的记录数量，`_state.Readings.Count > 0` 是一个比较表达式，结果本身就是 bool：0 条为 false，1 条或更多为 true。

骨架已连接集合的 CollectionChanged 事件到普通方法 Readings_CollectionChanged；每次 Add/Clear 后，该方法调用自定义通知工具 NotifyCanExecuteChanged，工具发布 CanExecuteChanged，按钮重新检查条件。NotifyCanExecuteChanged 不是 ICommand 必需成员，是教练命名的方法。EventHandler 和 EventArgs.Empty 来自 .NET 的 System；通知工具采用完整 null 判断，暂不要求学员自主设计。StatusText 使用 TASK-010 的属性通知机制，由骨架完整提供；它控制提示文字，与命令可用状态的通知职责不同。

### TODO 与实践/验收对应

| TODO | 文件与操作 | 对应范围 |
| :--- | :--- | :--- |
| 1 | ClearHistoryCommand.CanExecute：按 Count > 0 返回判断结果 | 实践 2、4、5；DoD 2 可用条件 |
| 2 | ClearHistoryCommand.Execute：Clear 同一历史集合 | 实践 2、5；DoD 1 同集合、DoD 3 清空和禁用 |
| 3 | Execute 中把占位提示改为已清空 | 实践 2 的完成反馈，不单设新验收项 |

首个动作只改 TODO 1：在 ClearHistoryCommand.cs 的 CanExecute 中，把 `return false;` 替换成 `return _state.Readings.Count > 0;`。在工作区运行 `dotnet run --project .\WpfBae\WpfBae.csproj`，或 IDE 选择 WpfBae 后 F5。实际窗口初始历史为空，清空按钮应禁用；点击读取增加一条记录后，清空按钮应可用。此时 Execute 仍是占位，点击清空不会移除记录，会显示尚未完成的提示，这是 TODO 2、3 留给后续实现的部分，不把首个动作当作完整验收。设计预览不作为运行结论。

## 验收标准（DoD）

- [ ] 项目可编译并实际打开任务窗口；清空按钮 Command 通过 Binding 取得 ICommand 对象，与读取动作使用同一历史集合；无旧清空 Click 或直接修改控件项目的替代路径。
- [ ] 空集合时清空按钮禁用；有效读取产生记录后按钮可用；CanExecute 的条件为历史 Count > 0，集合变化后的通知能使按钮重新检查条件。
- [ ] 点击已启用的清空按钮执行命令后，集合与列表均为空且按钮禁用；再次读取恢复历史及按钮可用；空字符串/纯空格提示且不新增、不删除已有记录。

## 可选巩固

- 在实践中区分 CanExecuteChanged（重新检查条件）、CanExecute（判断）与 Execute（执行）的调用职责。
- 暂不要求通用命令类、命令参数、读取命令化或完整 MVVM。

## 状态与证据

- 2026-10-11：TASK-011 全部必需 DoD 实际审查通过后，按计划创建下一张递增 ID 任务卡，状态 ⚪ 未开始。未创建 TASK-012 源码、未运行或验收、未切换入口。
- 2026-10-11 启动：用户明确要求“开启 task012”，已创建上述四个专属文件、切换 App.xaml 并同步状态为 🟡 进行中。实际执行 `dotnet build .\WpfBae\WpfBae.csproj --no-restore`，0 警告、0 错误；通过 PowerShell Start-Process 启动原 WpfBae.exe，确认窗口标题“TASK-012：清空历史命令”，CloseMainWindow 后退出码 0。仅验证骨架编译和原应用启动，CanExecute 为 false 占位，清空动作及完成提示仍留 TODO，尚未验证学员实现的按钮状态切换和清空/恢复，三项 DoD 保持未勾选；不新增已掌握能力。
- 同轮一致性检查：TASK-010/011 共 6 个原文件创建骨架前后 SHA256 一致，旧作品未修改；相关文档链接全部存在，TASK-012 在任务卡、任务索引、进度与导航中均为进行中，三个必需 DoD 均未勾选。
