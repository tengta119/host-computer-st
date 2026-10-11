# WPF 按钮命令

## Q1：Command 是什么，CanExecute、Execute 和 CanExecuteChanged 怎样控制清空按钮？

- 日期：2026-10-11
- 状态：待复习
- 来源：TASK-012 开始前的必要讲解；遵循学员对框架类型、自定义名字和完整语法来源的反馈。

**现象与场景**：希望有历史记录时可以清空，没记录时按钮禁用。TASK-011 已使用 Click 处理器执行清空；本任务把可用条件和动作放进一个命令对象，让 WPF 按钮使用它。

**来源和身份**：ICommand 是 .NET 提供、WPF 使用的接口，命名空间 System.Windows.Input；Button.Command 是 WPF 按钮属性。ClearHistoryCommand、ReadingCommandState、ClearCommand、NotifyCanExecuteChanged 是本任务自定义的类、属性或辅助方法，不是理应预先知道的 API。`: ICommand` 表示自定义类实现接口，提供约定的 CanExecute、Execute、CanExecuteChanged。

**两个方法**：CanExecute 返回 bool，WPF 用其 true/false 决定按钮是否可用；Execute 返回 void，按钮可用并正常点击时调用它执行动作。`object? parameter` 是接口规定的参数；object 是 C# 类型名，? 允许为 null，本任务没有 CommandParameter，参数不使用。集合 Count 表示记录数量；比较 `Count > 0` 得到 bool，可直接 return，无需另套 if。

**对象创建和连接**：窗口创建 ReadingCommandState 并保存到 `_state`。数据对象中 `new ClearHistoryCommand(this)` 把当前数据对象自己传给命令构造函数；命令内部字段保存此对象。因此窗口与命令都取得同一历史集合。数据对象公开 ClearCommand 属性，完整 getter 返回实际命令对象，声明为 ICommand 接口。窗口设置 DataContext 后，按钮 `Command="{Binding ClearCommand, Mode=OneWay}"` 就能取得该对象；ClearCommand 是属性名，与类名 ClearHistoryCommand 区分。

**重新检查条件**：CanExecuteChanged 是接口要求的事件，意思是“请重新检查是否可以执行”，不是执行清空。WPF 按钮订阅它。骨架订阅集合自带的 CollectionChanged；Add/Clear 后调用 NotifyCanExecuteChanged，工具发布命令事件，按钮再次调用 CanExecute。NotifyCanExecuteChanged 是教练提供的普通方法；EventHandler 与 EventArgs.Empty 来自 .NET 的 System，完整 null 判断与发布实现由骨架提供。列表也同时通过集合通知增删行；两种通知作用不同。

**最小实践**：在 ClearHistoryCommand.CanExecute 的 TODO 1，将 `return false;` 改成 `return _state.Readings.Count > 0;`。实际运行 TASK-012，初始 0 条记录按钮禁用；读取后 Count 变成 1，骨架触发重新判断，按钮应可用。此时 Execute 尚未 Clear，要随后完成 TODO 2、3；设计预览及骨架存在不能替代运行验收。

**证据边界**：本轮仅验证未完成 TODO 的骨架编译和原应用启动，未验证学员自主命令实现；问答待复习，不更新已掌握范围。
