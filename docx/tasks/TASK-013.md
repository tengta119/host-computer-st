# TASK-013：把读取逻辑从窗口移到 ViewModel

- 阶段：第二阶段：WPF（起步）
- 状态：🟡 进行中
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中 Command 后的 MVVM；本任务只练习同一历史界面的最小拆分。
- 选择依据：TASK-012 已实际验证命令条件、执行清空与按钮状态变化；读取仍由窗口 Click 方法处理。下一步复用已有场景，把输入值和读取动作连接到同一个界面数据对象，继续巩固 Binding、属性、集合和 Command。委托基础与通知工具仍需解释，不设额外口试门槛。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

界面仍包含设备 ID、读取温度、历史列表、清空历史与状态提示，行为沿用 TASK-012。窗口负责呈现界面；设备 ID、历史和状态保存在普通 C# 对象中，读取及清空按钮通过命令使用它，读取逻辑不再直接访问 TextBox 或 ListBox。

## 开始前先解释

- MVVM 是组织代码的一种方式，不是新的 C# 关键字或必须安装的框架。结合当前文件说明：View 是 XAML 窗口；Model 是已用的 Reading 读数类；ViewModel 是我们自己创建、保存界面数据并提供按钮命令的普通 C# 类。名字由教练取，不能当成已有 API。先画清创建哪个实例、DataContext 指向谁、命令持有谁，再讲连接。
- 对照 TASK-012 的 ReadingCommandState：它已保存历史、提示及清空命令，后续数据对象再保存设备 ID 并公开读取命令。不是要求学员先自写整套通用框架。
- 现有 OneWay 显示绑定是“对象属性 → 控件”；输入的 TwoWay 还让“控件编辑 → 对象属性”。先用完整 DeviceId getter/setter 解释 TextBox 中的文字怎样进入属性，再讲读取命令如何读取它。
- 明确 TextBox.Text 的更新时机。若骨架采用 `UpdateSourceTrigger=PropertyChanged`，须先说明这是 WPF Binding 的设置，表示编辑文字时更新来源属性；它不是自定义 OnPropertyChanged 方法，也不是要求新建一个同名事件。
- 读取命令继续实现已接触的 ICommand；本任务读取按钮一直可用，空白校验放在 Execute 中提示并返回，以保持既有行为。构造函数、事件声明、通知工具和完整属性实现由骨架提供并解释，核心校验、构造读数、Add 和提示留给学员。

## 用户实践与产出

1. 用新任务专属 ViewModel 保存 DeviceId、Readings、StatusText，并提供读取/清空两个命令。复用已验证的清空条件与动作，明确新窗口和两个命令使用同一对象及集合。
2. TextBox.Text 绑定 DeviceId，读取按钮 Command 绑定读取命令；历史与状态继续从同一 DataContext 绑定显示。
3. 在读取命令内读取 DeviceId、Trim 并判断空白：无效输入更新提示且提前返回；有效输入构造 Reading（当前 ID、26.5、°C），追加原历史集合并更新成功提示。
4. 窗口代码只保留必要的初始化和 DataContext 连接，不再包含读取 Click 方法；不直接改控件 Text/Items 来完成业务。
5. 在实际窗口观察输入改变被读取命令使用、连续读取保留旧记录、空白保护、清空禁用与再次读取恢复。

## 目录与开始约定

2026-10-12 按用户“开启task013”开始任务。确认同名目录不存在后，在 `WpfBae/Exercises/TASK-013-reading-viewmodel/` 创建以下骨架：

- [Task013Window.xaml](../../WpfBae/Exercises/TASK-013-reading-viewmodel/Task013Window.xaml)：沿用历史界面布局；DeviceId 输入的 TwoWay Binding、两个按钮的 Command、历史和状态绑定均由教练提供。
- [Task013Window.xaml.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/Task013Window.xaml.cs)：创建一个 ReadingViewModel 实例并设置 DataContext；没有读取/清空 Click 方法。
- [ReadingViewModel.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)：完整 DeviceId、StatusText 属性、原历史集合、两个命令对象、属性通知与集合通知连接由教练提供。
- [ReadTemperatureCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)：读取命令签名、对象保存和始终可用条件由骨架提供；核心读取留 TODO 1～4。
- [ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs)：复用 TASK-012 已验证的清空条件、动作与提示，连接新 ViewModel，完整普通语法通知工具由骨架提供。
- 复用 [TASK-010 中已有的 Reading](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)，不搬移原作品。共享入口例外：[App.xaml](../../WpfBae/App.xaml) 仅将 StartupUri 切换到 TASK-013；项目默认递归编译，无需改 csproj 或新增依赖。

### 对象连接与输入写回

MVVM 是组织代码的方法，不是 C# 关键字。本任务 View 为 Task013Window 的 XAML 界面，Model 复用已有 Reading 读数类，ViewModel 为教练自定义的 ReadingViewModel 普通 C# 类。它在 TASK-012 数据对象基础上增加 DeviceId 和 ReadCommand，保存界面数据并提供按钮动作。

窗口执行 `new ReadingViewModel()` 创建对象，字段 `_viewModel` 保存其引用，`DataContext = _viewModel` 让内部 Binding 从此对象查找公开属性。ViewModel 构造函数执行 `new ReadTemperatureCommand(this)` 和 `new ClearHistoryCommand(this)`；此处 this 是同一个 ViewModel 自己，命令构造函数将传来的引用保存到各自字段。窗口、读取命令、清空命令共用同一个对象及其历史集合。

TextBox.Text、Button.Command、DataContext 是 WPF 属性；DeviceId、ReadCommand、ClearCommand、Readings、StatusText 是我们自定义的数据属性名。两个命令属性的完整 getter 返回原命令对象，Binding 不读取私有字段，也不凭类名创建命令。

输入连接为 `Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"`：TwoWay 支持对象到输入框的显示，以及输入框到对象的写回。编辑文字时，WPF 调用 DeviceId 的 setter，新的字符串成为 value；`_deviceId = value` 保存文字，完整 getter 返回字段中的当前字符串。读取命令因此可以直接取 `_viewModel.DeviceId`，无需访问输入控件。

UpdateSourceTrigger 是 WPF Binding 的设置；PropertyChanged 表示每次文字变化时更新来源属性。TextBox.Text 默认在失去焦点时写回，本骨架明确采用文字变化时写回，参见 [Microsoft 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-control-when-the-textbox-text-updates-the-source)。它不等于自定义 OnPropertyChanged 方法：前者控制输入何时写回；后者发布对象属性变化通知，让绑定重新读取数据。属性通知、集合事件、命令条件通知的完整工具均已提供，机制继续待复习，不要求先自行设计。

读取按钮 Command 从 ReadCommand getter 取得 ReadTemperatureCommand 对象；WPF 调用其 CanExecute 得到 true，正常点击时调用 Execute。空白输入也允许点击，后续由 Execute 提示并 return。状态赋值经过 StatusText setter 发属性通知；历史 Add/Clear 由原 ObservableCollection 发集合通知，列表更新，同时骨架通知清空按钮重新检查 Count > 0。

### TODO 与实践/验收对应

| TODO | 实际位置与操作 | 对应范围 |
| :--- | :--- | :--- |
| 1 | ReadTemperatureCommand.Execute：取得 DeviceId，保存到局部 string id，状态显示该 ID | 实践 1～2 的输入/命令连接；DoD 1 的基础观察 |
| 2 | 取得 ID 时 Trim，空白时提示并提前 return，保留历史 | 实践 3；DoD 3 空白保护 |
| 3 | 有效输入创建已有 Reading（当前 ID、26.5、°C），Add 到原集合 | 实践 3；DoD 2 当前 ID、旧记录保留和同集合 |
| 4 | 追加成功后设置成功 StatusText | 实践 3；DoD 2 状态绑定更新 |

首个动作只改 ReadTemperatureCommand.cs 中 Execute 的 TODO 1：

```csharp
string id = _viewModel.DeviceId;
_viewModel.StatusText = "命令取得的设备 ID：" + id;
```

将这两行放到原占位状态赋值处，替换那一行。在工作区运行 `dotnet run --project .\WpfBae\WpfBae.csproj`，或 IDE 选择 WpfBae 后 F5。把输入改为 TEMP-002，点击“读取温度”，提示应显示“命令取得的设备 ID：TEMP-002”；再改一个 ID 重试。此时不新增历史，清空按钮仍禁用，属于尚未完成 TODO 2～4 的正常状态。看清输入连接后再完成后续读取功能。实际运行结果才是观察证据，设计预览不计验收。完成后可请求“检查 TASK-013”。

## 验收标准（DoD）

- [ ] 项目可编译并实际打开任务窗口；DataContext 指向同一 ViewModel，设备 ID 输入通过 TwoWay Binding 更新其属性，两个按钮通过 Command 连接命令；读取业务不留在窗口 Click 中，不直接访问控件完成业务。
- [ ] 连续有效读取及修改设备 ID 后，新记录使用当前输入且包含温度和单位，旧记录保留；状态和历史经绑定更新，读取/清空使用同一历史集合。
- [ ] 空字符串/纯空格有提示且不改变历史；初始空历史清空禁用，读取后启用，清空后列表为空并禁用，再次读取恢复；全链路有实际窗口运行证据。

## 可选巩固

- 对照 TASK-012 指出原窗口读取代码现在位于哪个对象中；不要求单独考试。
- 暂不要求独立服务层、依赖注入、通用命令库或异步命令。

## 状态与证据

- 2026-10-12 启动：已创建上述五个专属文件并切换共享入口，状态 🟡 进行中；读取核心 TODO 1～4 尚未由学员实现。实际执行 `dotnet build .\WpfBae\WpfBae.csproj --no-restore`，0 警告、0 错误；通过 PowerShell `Start-Process -WindowStyle Hidden -PassThru` 启动构建后的原 WpfBae.exe，确认主窗口标题“TASK-013：读取逻辑与 ViewModel”，调用 CloseMainWindow 后正常退出，退出码 0。仅验证骨架编译和原应用启动，未验证输入写回、读取追加、空白保护及清空恢复全链路；三个 DoD 保持未勾选，不新增已掌握能力。`git diff --` 核对 TASK-009～012 原目录无改动。
- 2026-10-11：TASK-012 三项必需 DoD 全部实际审查通过后，按计划与初学者反馈仅创建这一张下一任务卡，状态 ⚪ 未开始。没有准备 TASK-013 源码、切换入口、构建或验收；已有阶段和待复习概念范围不变。
