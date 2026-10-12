# WPF 窗口与 ViewModel

> [返回问答索引](./README.md)

## Q1：ViewModel 是什么，输入框的文字怎样进入读取命令？

- 日期：2026-10-12
- 状态：待复习
- 来源：TASK-013 启动的必要讲解；沿用 TASK-012 的对象与命令场景。

**2026-10-12 最终实践证据**：TASK-013 的 10 个实际窗口场景全部通过，学员自主完成读取属性、Trim/空白提前返回、Reading 构造、原集合 Add 与提示，首次遗漏 return 也已自主修复。见 [运行结果](../tasks/evidence/TASK-013/results.json)。输入写回、同对象绑定及代码改属性后的界面显示均已由审查验证，但这些连接和通知实现由骨架提供，不能据此认定独立设计或机制全部掌握；本综合条目继续待复习，已验证动作范围记入学习进度。下文为启动与过程答疑时的历史记录。

**现象与原因**：TASK-012 的读取 Click 方法位于窗口，直接取得 DeviceIdTextBox.Text；清空已经通过命令使用数据对象。希望输入和两个按钮统一使用界面数据，让读取动作不依赖控件。

**来源与身份**：MVVM 是代码组织方式。本任务 View 是 Task013Window 的 XAML 界面，Model 是复用的 Reading 读数类，ViewModel 是教练新建的普通 C# 类 ReadingViewModel。类名、DeviceId/ReadCommand 等属性名、命令类名及 OnPropertyChanged 工具名是自行命名的；DataContext、TextBox.Text、Button.Command、Binding 是 WPF 提供，ICommand/INotifyPropertyChanged 等是 .NET 接口。无需安装新框架。

**对象连接**：窗口 `new ReadingViewModel()` 创建实例，字段 `_viewModel` 保存它，DataContext 指向它。该对象构造时用 `new ReadTemperatureCommand(this)` 和 `new ClearHistoryCommand(this)` 创建两个命令；此处 this 指当前 ViewModel。命令保存传入引用，使用同一个对象及原历史集合。ReadCommand/ClearCommand 的完整 getter 返回原命令对象，按钮的 Binding 按属性名读取；不直接读取私有字段。

**输入机制**：`Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"` 表示双向连接：对象初始 DeviceId 显示到输入框；编辑输入框也将字符串写回 DeviceId。WPF 调用 setter 时，新字符串成为 value，`_deviceId = value` 保存它；getter 的 `return _deviceId` 返回当前文字。读取 Execute 取 `_viewModel.DeviceId` 即可，无需访问 TextBox。

UpdateSourceTrigger 是 WPF Binding 的更新时机设置。TextBox.Text 默认失去焦点才写回；本骨架的 PropertyChanged 表示文字变化时写回，依据 [Microsoft 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-control-when-the-textbox-text-updates-the-source)。此设置不等于自定义 OnPropertyChanged 方法：前者决定控件何时写到对象；后者发布对象属性变化通知，让绑定重新读取公开属性。骨架提供完整 get/set 和通知工具，暂不要求自主重写。

**最小实践**：在 ReadTemperatureCommand.Execute 的 TODO 1 以两行替换占位状态赋值：

```csharp
string id = _viewModel.DeviceId;
_viewModel.StatusText = "命令取得的设备 ID：" + id;
```

实际运行后将输入改为 TEMP-002，再点击读取，预期提示显示当前 ID。赋值经过 StatusText setter 保存并通知，TextBlock 的绑定重新读取提示。此步骤只验证输入→对象→命令→提示，尚不追加历史；后续再完成 Trim、空白保护、Reading 构造、Add 和成功提示。

**证据边界**：启动时教练只验证骨架构建和原程序窗口启动，未验证学员的输入写回或读取功能，DoD 均未勾选。本条待复习；TASK-012 已验证应用范围保留，委托/事件及一般属性机制不因新骨架存在而认定掌握。

### 2026-10-12 追问：在 TextBox 更改值，会自动更新 ViewModel.DeviceId 吗？

当前代码会：窗口 DataContext 指向原 `_viewModel`，输入 Text 的 TwoWay 绑定允许写回，UpdateSourceTrigger=PropertyChanged 指定文字变化时写回。用户将文字改为 TEMP-002 时，WPF 的绑定会调用同一对象 DeviceId 的 setter，可以理解为执行 `_viewModel.DeviceId = "TEMP-002"`；这是赋值效果示意，不是项目需补写的代码，也不意味着框架源码直接使用此变量名。

完整 setter 中 value 接到新字符串，`_deviceId = value` 保存它；随后 OnPropertyChanged("DeviceId") 通知监听对象属性的绑定重新读取。输入写入属性由 TwoWay 和更新时机设置负责，不是靠 OnPropertyChanged 去取得输入框文字。下一次读取 getter 时得到已保存的新值。无需等点击读取才保存；读取按钮只是之后使用当前属性值。

用户消息里的星号若用于强调，应去掉；实际 XAML 枚举值应写 TwoWay 和 PropertyChanged，不能保留字面星号。当前源码已是正确写法。本轮只读源码与答疑，未运行输入场景、未修改学员源码，Q1 继续待复习，TASK-013 状态与 DoD 不变。

### 2026-10-12 追问：文字已经在 TextBox 改过，为什么 setter 还要通知界面？

就当前 setter 原样保存字符串这一情况，用户输入框已经显示新文字，绑定调用 setter 后 `_deviceId = value` 足以保存文字；OnPropertyChanged 不是这一步保存的必要条件。上次只说“通知界面”没有展开修改来源，需明确输入写回与对象向界面通知的方向。

DeviceId 也可以被普通代码赋值，例如 `_viewModel.DeviceId = "TEMP-003"`（解释用，不要求加入项目）。这次 TextBox 没有被用户编辑，setter 中字段虽已更新，WPF 仍需要对象发布属性通知，才能在当前 INotifyPropertyChanged 绑定路径上得知应重新读取 DeviceId、更新输入框。TwoWay 允许两种方向，不能替代对象后续变化通知。依据 [Microsoft 属性通知文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-implement-property-change-notification)。

如果同一 DeviceId 还被另一个 TextBlock 绑定显示，用户改 TextBox 后，另一个控件也需要对象通知来跟随变化。当前任务没有这个额外 TextBlock，此为用途示意。通知工具发布 PropertyChanged，参数中的 "DeviceId" 是属性名，含义为“此属性变化，请重新读取”；不携带新 ID，不直接设置某个控件，也不表示点击读取。

setter 统一保存并通知，保证经属性赋值的不同来源都可以更新绑定界面。仅针对输入框改文字并原样保存，去掉通知仍可写回字段；不能将此局部现象推广为所有绑定均不需要通知。本轮仅答疑和源码/官方文档核对，未修改学员代码、未运行该场景；Q1 待复习、TASK-013 进行中及未勾选 DoD 保持不变，不记录为学员错题。

### 2026-10-12 追问：StatusText 为什么不采用 TwoWay 和 UpdateSourceTrigger=PropertyChanged？

当前 StatusText 通过 TextBlock 显示程序提示，用户没有在这个控件输入的操作，因此只需对象属性→控件的 OneWay。DeviceId 使用可编辑 TextBox，既显示对象初始值，也接收用户输入写回对象，所以采用 TwoWay。不是字符串属性名决定模式，应按界面是否需要写回数据选择。

Mode 决定数据流方向：OneWay 为对象→控件，适合状态、温度、结果显示；TwoWay 为对象↔控件，适合设备 ID 和可编辑参数。OneWay 仍会在对象发出适当属性变化通知后更新界面，不表示仅显示一次。UpdateSourceTrigger 是另一个独立设置，只决定控件→对象这一方向何时写回，不决定对象→界面的更新时间，也不能替代 OnPropertyChanged。

TwoWay 输入可选 PropertyChanged（文字一变就写回，适合当前 ID 输入或输入时就需更新数据的场景）或 LostFocus（离开输入框时写回，适合希望暂存编辑过程、离开时再同步的场景；TextBox.Text 的默认时机）。LostFocus 不保证已提交到文件/设备，也不等于“按保存按钮”，只控制绑定写回。StatusText 的 OneWay 没有控件→对象写回路径，所以无需 UpdateSourceTrigger。

示例 `_viewModel.StatusText = "读取成功"` 调用 setter 保存并发布 StatusText 属性通知，WPF 的 OneWay Binding 重读属性、更新 TextBlock.Text。来源：[WPF 数据绑定概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)、[TextBox 更新来源时机](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-control-when-the-textbox-text-updates-the-source)。本轮仅源码/文档核对和答疑，未修改/运行学员代码；Q1 待复习，任务与 DoD 不变。
