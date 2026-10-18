# WPF 按钮命令

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q2：TASK-034 怎样把添加和清空按钮连接到当前ViewModel的命令？

- 日期：2026-10-17。
- 状态：已掌握。
- 来源：TASK-034开启，复用Q1已经验证的命令应用范围，本题同一监控对象与运行状态连接尚无自主证据。

### 首步只替换添加按钮的调用入口

原AddButton点击后调用窗口的AddButton_Click，后者转到AppendNextSample追加一条温度。
本题希望按钮直接取得ViewModel提供的追加命令，再由命令调用同一个AppendNextSample，不再经由旧Click处理器。
TemperatureMonitorViewModel是沿用的自定义数据类，当前对象保存在窗口_viewModel中，并已作为DataContext。
AppendTemperatureCommand是教练为本题新建的命令类，ICommand是.NET提供的接口，WPF按钮能按照这个接口询问是否可执行，并请求执行动作。

ViewModel构造方法中的这一行已由骨架提供。

```csharp
_addCommand = new AppendTemperatureCommand(this);
```

this指当前正在创建的ViewModel对象，这个对象传入命令构造方法，再由命令的_viewModel字段保存引用。
命令里不再new另一个ViewModel，因此它操作的还是表格、条数、绘图和CSV共用的原集合。
AddCommand是自定义公开属性，get返回刚才保存的_addCommand，按钮通过绑定读取这个属性，不直接访问私有字段。

首步在AppendTemperatureCommand.cs的Execute方法TODO 1a填写：

```csharp
_viewModel.AppendNextSample();
```

AppendNextSample是教练从原窗口迁入ViewModel的普通方法，内部仍使用原24、26.5、25、28样例和下一条位置，每次只追加一条。
Execute是ICommand规定的执行方法，正常点击已连接的按钮时WPF会调用它；object? parameter是接口要求的参数，本题没有传入额外数据，可以暂不使用。
CanExecute是接口规定的允许条件，返回true表示按钮允许执行，返回false表示不允许；添加按钮沿用原本运行和停止时都可追加的行为，骨架固定返回true。

接着在Task034Window.xaml的AddButton标签中删除Click="AddButton_Click"，添加：

```xml
Command="{Binding AddCommand, Mode=OneWay}"
```

左边Command是WPF按钮的属性，右边AddCommand是当前DataContext对象的公开属性名，Binding读取该属性得到追加命令对象。
不能同时保留Click追加和Command追加，否则一次点击可能执行两次，写入两条温度。
运行项目，在停止状态点一次“添加下一条”，应新增一行24、Count为1，再点一次得到26.5，先确认这一小步。
首步完成后再做清空，不要求先实现所有运行状态通知。

### 清空命令分别回答能不能执行与真正执行

ClearTemperatureHistoryCommand是本题第二个自定义命令类，同样由ViewModel构造方法传入this，公开ClearCommand属性返回同一个清空命令对象。
清空的CanExecute在TODO 2a应读取_viewModel.IsSampling，正在采集就拒绝，停止就允许；本题沿用原空历史也可清空的行为，不额外增加Count大于0要求。
IsSampling是本题自定义bool属性，保存在ViewModel里，true表示窗口告诉它正在采集，false表示已停止，它本身不是定时器。
Execute在TODO 2b先检查CanExecute(parameter)，不允许时return，允许时再调用已有_viewModel.ClearHistory()。
ClearHistory是教练从原清空方法迁移的普通方法，内部还检查IsSampling，再复位下一条位置并清空原集合，以保留运行保护。
TODO 2c将ClearButton从旧Click改为Command绑定ClearCommand，接通后再删除两个无用的旧Click方法。

### ViewModel怎样知道窗口定时器已经启动

窗口_samplingTimer是真实WPF定时器，IsEnabled是它当前的启动状态；ViewModel.IsSampling只保存一个bool值，两者不会因意思相近自动同步。
TODO 4在原UpdateSamplingControls里，用普通赋值把真实状态交给同一个ViewModel。

```csharp
_viewModel.IsSampling = _samplingTimer.IsEnabled;
```

右侧先读取实际定时器状态，再把这个true或false交给左边自定义属性的set，set中的value就得到这次新状态。
窗口已有启动/停止调用UpdateSamplingControls的路径，所以只需在这个方法同步，不另建定时器或重复登记Tick。
停止/启动文字仍可由窗口设置，但完成命令连接时应移除这里两处ClearButton.IsEnabled赋值，把按钮是否可执行交给清空命令。

### 保存新状态以后让按钮重新检查条件

IsSampling的set已把value保存到_isSampling字段，TODO 3还需调用_clearCommand.NotifyCanExecuteChanged()。
NotifyCanExecuteChanged是教练提供的普通辅助方法，它发出ICommand规定的CanExecuteChanged事件，让已经连接清空命令的按钮重新调用CanExecute。
这个通知不会执行清空；它只让按钮根据新状态变为可用或不可用。
窗口启动后状态赋值true，发通知，按钮重查条件得到false而禁用；停止后状态赋值false，按钮重查条件得到true而恢复。
提供的通知方法先用EventHandler?变量保存事件接收方法，非null才调用，this在这里是发通知的命令对象，EventArgs.Empty表示没有额外事件数据；这些基础代码沿用原项目完整写法，首步无需改它。

### 原数据更新链保持

ViewModel.AppendNextSample对原Temperatures执行Add，原CollectionChanged处理器继续重画并调用RefreshDisplay，已完成的属性通知让Count和报警文字更新。
定时器Tick仍经窗口AppendNextSample转到同一个ViewModel方法，手动命令与自动采集不各维护一份样例位置。
配置和文件保存窗口暂留窗口，当前只迁移两个按钮的业务入口，不需要引入新MVVM框架。

### 开启时证据与边界

两个命令的类型、签名、事件通知基础代码与同一ViewModel连接由教练提供，追加/清空算法从原作品迁移；命令执行、清空条件/通知、运行状态同步与按钮绑定仍留TODO。
实际74项骨架检查和原exe启动关闭通过，原Click路径与定时采集可运行，不等于Command已实现或DoD已通过。
三个DoD保持未勾选，Q2待复习，Q1原掌握范围保持，当前25分类/61条、待复习33/已掌握28；路径与实际命令见[TASK-034](../tasks/TASK-034.md)。

### 2026-10-17 TASK-034审查：正在采集和允许清空是两个不同的问题

当前CanExecute返回_viewModel.IsSampling，实际表现为运行清空按钮可点、停止反而变灰，停止直接请求Execute也因CanExecute为false直接返回。
IsSampling回答“现在是否正在采集”，清空CanExecute回答“现在是否允许清空”，本题两者应取相反值。
可用return _viewModel.IsSampling == false;表达“已停止才允许”，也可写return !_viewModel.IsSampling;，感叹号是C#布尔取反，在这里仅改变本次返回结果，不改原状态字段。
窗口状态同步、IsSampling setter里的NotifyCanExecuteChanged、按钮Command连接及Execute先检查条件的控制关系已实际验证，不需要交换状态或删除保护。
原ClearHistory仍在运行时拒绝清空，所以目前按钮虽可点但没有真正删历史；停止时则命令提前返回，不能把运行中未误删理解成整条清空命令已正确。
93项检查86通过、7项失败均归因于此条件，DoD1/3通过、2未通过，Q2继续待复习；追加/命令绑定/旧Click移除/状态同步和条件通知的局部自主应用证据保留，原Q1不撤销。
本轮源码未代改，修复后观察启动按钮清空应变灰、停止后应恢复，点清空后Count0/暂无温度/空图，下一条24，再请求审查，详见[TASK-034](../tasks/TASK-034.md)。
25分类/61条、待复习33/已掌握28保持，前述命令未接通为开启时历史。
### 2026-10-17 TASK-034最终复查与掌握边界

学员已自主将清空CanExecute改为!IsSampling，最终105项实际检查全通过，三个DoD通关，见[最终结果](../tasks/evidence/TASK-034/review-final/results.json)。
Q2按现有骨架的AddCommand/ClearCommand绑定、Execute调用原VM操作、清空条件取反、窗口状态同步与setter通知的自主应用标已掌握；真实按钮/直接命令的运行保护、停止恢复、清空复位和混合追加均已验证，本轮未代改源码。
事件通知基础、命令类型及签名/构造对象连接、原样例追加清空算法由教练提供或迁移，不扩大为所有接口/委托/事件机制与完整MVVM架构已独立掌握。
Q1原范围保持，当前25分类/61条、待复习32/已掌握29，前述Q2待复习和首次条件相反为历史。
### 2026-10-18 TASK-038审查：Command里的名字取自哪里？

实际AddButton写Command="{Binding AddButton}"，这会让WPF去当前DataContext即新VM里查找公开属性AddButton，而不是拿窗口里x:Name叫AddButton的按钮；VM公开的是AddCommand，所以实际Command为空，点击不追加。
ClearButton同理要读取ClearCommand，两个原命令的Execute和清空CanExecute方法已通过独立诊断，不需要重写成Click算法。
IsSampling.set还只发了CanSelectDevice属性通知，未调用_clearCommand.NotifyCanExecuteChanged()，启停时实际条件通知为0；CanExecute是询问能否清空，NotifyCanExecuteChanged是要求按钮重新询问，两者各有作用。
本轮没有修补绑定或条件通知，TASK-038三个DoD未通过；原TASK-034已验证的局部掌握保留，不把此新项目连接未完成等同于旧证据不存在。
### 2026-10-18 TASK-038第二次审查通过

AddCommand/ClearCommand绑定和IsSampling后的清空条件重查已自主修复，真实按钮添加/清空、运行禁用/停止恢复、命令直接执行保护均通过，264项功能全部通过，前轮缺口已修复。
原Q2局部掌握范围保留，接口和命令构造仍为教练提供，不推定全部事件机制独立掌握；TASK-038仅因运行记录未填写保持进行中，见[结果](../tasks/evidence/TASK-038/review-02/results.json)。

## Q1：Command 是什么，CanExecute、Execute 和 CanExecuteChanged 怎样控制清空按钮？

- 日期：2026-10-11
- 状态：已掌握（限已提供骨架上的命令条件与执行动作；委托、事件及框架订阅原理继续待复习）
- 来源：TASK-012 开始前的必要讲解；遵循学员对框架类型、自定义名字和完整语法来源的反馈。

清空历史按钮应在没记录时禁用，有记录时可点。ClearHistoryCommand 是本项目自定义命令类，WPF 按钮把“能不能清空”和“真正清空”交给这个命令对象。

先看条件，来自原 ClearHistoryCommand 的简化等价写法：

```csharp
public bool CanExecute(object? parameter)
{
    return _state.Readings.Count > 0;
}
```

_state 保存原 ReadingCommandState 数据对象，Readings 是它的历史集合。Count>0 得到 true/false，WPF 用它更新按钮是否可用；object? parameter 是 .NET ICommand 接口要求的参数，本任务未提供额外命令参数，所以不使用它。CanExecute 本身不清空。

按钮正常点击执行 Execute，其中 `_state.Readings.Clear();` 清空同一集合，`_state.StatusText = "已清空";` 保存完成提示。这与判断条件是两个方法，不能发一次条件通知就当作用户点清空。

窗口创建一个 ReadingCommandState；它构造时用 `new ClearHistoryCommand(this)` 把自己交给命令。this 在这里指当前数据对象，命令字段保存它的引用。公开属性 ClearCommand 的完整 getter 返回原 _clearCommand，不每次创建新命令。

```xml
Command="{Binding ClearCommand, Mode=OneWay}"
```

左边设置具体清空 Button 的 Command 属性；右边从窗口 DataContext 指向的原数据对象读 ClearCommand，取得那个命令对象。Button.Command 是 WPF 属性，ICommand 是 .NET 接口，ClearCommand/ClearHistoryCommand 是项目自定属性/类名。

### 2026-10-12 理解确认：按钮绑定对象与集合变化后的条件检查链

实际来源是“Binding 读公开 ClearCommand → getter 返回 _clearCommand → 按钮持有这个命令”，不是 XAML 直接访问私有字段。初次连接时 WPF 就会检查条件，因此空集合的按钮不必等第一次集合变化才禁用。

新增记录后，骨架监听原集合 CollectionChanged，调用自定义 NotifyCanExecuteChanged；命令通知 WPF 再问 CanExecute，Count>0 后按钮可用。新增从 1 变 2 仍会发通知，但检查结果依旧 true，通知不保证条件值改变。

### 2026-10-11 追问：public event EventHandler? CanExecuteChanged 有什么用？

它为这个命令公开一个事件，让按钮接收“条件可能变化，请重新检查”。event/public 是 C# 关键字，EventHandler 是 .NET 的委托类型，CanExecuteChanged 是 ICommand 规定的事件名，? 允许没有接收者。

事件声明只提供登记入口，不自动通知，也不是保存 bool。发通知的完整说明写法是：

```csharp
EventHandler? handler = CanExecuteChanged;
if (handler != null)
{
    handler(this, EventArgs.Empty);
}
```

这在命令内部取得已登记方法，检查存在后调用。这里 this 指发通知的命令对象，EventArgs.Empty 表示没额外数据。当前原 TASK-012 已用 ?.Invoke 简写，完整对应见 [事件 Q6](./csharp-events.md)。CollectionChanged 报集合增删；CanExecuteChanged 让按钮重查条件，两项通知职责不同。

读取 Count、遍历集合不会触发集合通知，改变旧 Reading 内部属性也不自动触发集合通知。实际 Add/Clear 才走本题已有的集合通知连接。

### 2026-10-11 追问：CanExecuteChanged 指向 CanExecute 吗，类似 Java 反射吗？

它登记的是 WPF 接收通知的方法。接收方法再通过 ICommand 调用 CanExecute；事件没有直接存 CanExecute，也不是按 Changed 名字找到对应方法。CanExecute 返回 bool、接收一个参数，本身也不符合 EventHandler 的两个参数/void 要求。

Java 监听器接通知、再调用检查方法的类比更合适。ICommand 已约定 CanExecute，WPF 可以按接口调用，不需要反射按字符串找它。

### 2026-10-11 追问：示例中的 command 是由事件参数 this 取得的吗？

说明代码 `ICommand command = _state.ClearCommand;` 的 command 来自数据属性；事件 `handler(this, EventArgs.Empty)` 的 this 被传给接收方法的 sender。这是两次不同的参数/属性动作，虽然可以指向同一命令，不能把属性读取说成来自 sender。

按钮早已从 Command 绑定取得命令。官方 ButtonBase 接收通知时调用 UpdateCanExecute，通过按钮已持有的 Command 重查，不依赖 sender 来取命令。不同位置的 this 也不同：数据构造中是数据对象，命令通知方法中是命令对象。

### 2026-10-11 追问：当前已注册的处理方法具体是什么？

按本次核对的 WPF 官方源码，命令事件直接连接内部 CanExecuteChangedEventManager.HandlerSink 的 OnCanExecuteChanged，它再转发到按钮基类 ButtonBase.OnCanExecuteChanged。前者负责把通知转给按钮，后者再检查条件。它们是框架方法，不在学员源码中，也不是学员的 CanExecute。

这里说明源码里的实现路径，未在本轮运行程序枚举实际订阅列表；不扩展弱引用工具内部设计。正式依据附在文末，阅读主线只需先理解“接通知的方法收到后再检查条件”。

### 2026-10-11 追问：“WPF 把处理方法订阅到事件上”具体是什么意思？

先看一个自定义说明方法，放在能使用 _state 的同一示例类中：

```csharp
void OnCommandChanged(object? sender, EventArgs e)
{
    Console.WriteLine("收到命令变化通知");
}
```

在该类另一个方法里可登记它：

```csharp
ICommand command = _state.ClearCommand;
command.CanExecuteChanged += new EventHandler(OnCommandChanged);
```

new EventHandler 保存 OnCommandChanged 的引用，+= 把它登记到原命令的事件。登记时不执行；之后命令调用当前事件的处理方法，才会输出收到通知。这个方法只是说明订阅动作，不要求加到当前项目。

WPF 本身也是写好的 C# 类。按钮连接命令时，框架用相同的 += 动作登记内部转发方法，所以你的项目没手写这行也能收到通知。框架接收后的动作是更新按钮条件，不是这里的 Console 输出。

### 2026-10-11 追问：handler(this, EventArgs.Empty) 为什么传 this？

EventHandler 接收方法有 sender/e 两个参数。this 是正在发通知的 ClearHistoryCommand；传给第一项后，接收方法 sender 得到这个对象的引用，表示“通知来自谁”。不复制类源码，也不创建第二个命令。

多个命令可共用一个接收方法，用 sender 区分来源。标准约定传发送者，但接收者可以不用；本题按钮仍从已持有的 Command 检查条件。第二项 EventArgs.Empty 对应 e，没有 true/false，条件要另问 CanExecute。

“发布通知会调用 WPF 按钮接收方法”的复述主线有效；仍要补上它只重新判断，既不模拟点击，也不执行清空。所有通知步骤由既有骨架/框架提供，不要求在本次整理中新增练习。

## 历史证据与掌握范围

Q1 保持已掌握，范围限原骨架上自主实现 Count>0、Execute.Clear 与提示。2026-10-11 TASK-012 的 8 个实际场景已验证禁用/启用/清空/恢复和空白保护；委托基础、接口事件声明、通知工具、框架登记与转发继续待复习。2026-10-11/12 的复述与源码核对保留为过程反馈，不扩展自主实践证据。旧稿各追问时“进行中/未运行”属于当时记录，任务最终通关状态保持。

相关任务卡与验收来源：[TASK-002](../tasks/TASK-002.md)、[TASK-011](../tasks/TASK-011.md)、[TASK-012](../tasks/TASK-012.md)、[TASK-013](../tasks/TASK-013.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-commands.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [ButtonBase.cs](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Primitives/ButtonBase.cs#L276)
- [CommandHelpers.cs](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/MS/Internal/Commands/CommandHelpers.cs#L94)
- [ButtonBase 连接及处理代码](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Primitives/ButtonBase.cs#L270)
- [事件工具订阅代码](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Input/Command/CanExecuteChangedEventManager.cs#L329)
- [事件工具实际 += 订阅](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Input/Command/CanExecuteChangedEventManager.cs#L354)
- [EventHandler 的 sender 参数](https://learn.microsoft.com/en-us/dotnet/api/system.eventhandler?view=net-10.0)
