# TASK-014：用 UserControl 封装读取历史区域

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中 MVVM 后的 UserControl。
- 选择依据：TASK-013 已验证读取命令使用同一 ViewModel，以及输入、历史、提示的绑定与完整运行链路；继续复用历史场景，把窗口的一块界面封装为控件，观察绑定来源如何传到内部。不要求独立设计通用 MVVM 框架，属性与事件机制继续复习。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

以后设备监控窗口可能在不同位置显示读数历史。把“读取历史”标题、列表和清空按钮组合成一个自定义控件，窗口可以像放置 Button 一样放置它。设备 ID、读取按钮和状态仍在窗口；原历史集合与命令保持同一个数据来源，封装后行为不变。

## 开始前先解释

- UserControl 是 WPF 提供的控件类型，用来组合一块界面；它放在窗口内部，不是另一个独立 Window，不使用 Show 来打开。ReadingHistoryControl 是开始时将自定义的类名，不是现成 API。
- 窗口设置 DataContext 后，内部控件通常继承这个对象；控件内部的列表和按钮继续按属性名取得 Readings 与 ClearCommand。先对照实际对象与 XAML 层次说明继承，不先引入尚未解释的依赖属性或自定义控件框架。
- 在自定义控件里另建 ViewModel 并设置 DataContext，会使它使用另一份历史。本任务控件使用父窗口传来的原对象，沿用已验证的集合与命令。
- 开始时解释新控件 XAML/后台文件如何配对、窗口怎样声明其命名空间并放置实例；完整普通语法及必要连接由骨架提供。

## 用户实践与产出

1. 在专属 ReadingHistoryControl 中组织历史标题、列表和清空按钮；保留窗口输入、读取按钮和状态。
2. 在窗口嵌入该控件，内部列表绑定 Readings，清空按钮绑定 ClearCommand；不在控件创建另一份数据或编写清空业务 Click。
3. 实际运行验证重复读取、修改 ID、空白保护、清空禁用与恢复；窗口和内部控件始终使用同一 ViewModel 及历史集合。

## 目录与开始约定

2026-10-12 用户明确要求开启 TASK-014，已确认专属目录不存在后创建骨架。专属目录：`WpfBae/Exercises/TASK-014-history-usercontrol/`。

| 身份 | 实际文件 / 作用 |
| :--- | :--- |
| 新建窗口 | [Task014Window.xaml](../../WpfBae/Exercises/TASK-014-history-usercontrol/Task014Window.xaml)：输入、读取、状态及嵌入控件的连接由教练提供 |
| 新建窗口后台 | [Task014Window.xaml.cs](../../WpfBae/Exercises/TASK-014-history-usercontrol/Task014Window.xaml.cs)：创建一个原 ReadingViewModel 类型的实例并设置窗口 DataContext |
| 新建控件 | [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-014-history-usercontrol/ReadingHistoryControl.xaml)：启动时提供 StackPanel 占位；学员现已补齐标题、列表和清空按钮及两项绑定，保留原 TODO 注释 |
| 新建控件后台 | [ReadingHistoryControl.xaml.cs](../../WpfBae/Exercises/TASK-014-history-usercontrol/ReadingHistoryControl.xaml.cs)：UserControl 定义和 InitializeComponent 由教练提供 |
| 复用原数据类 | [ReadingViewModel.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)：保留原文件及原集合、属性通知连接 |
| 复用原读取命令 | [ReadTemperatureCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)：直接沿用学员已验证的读取实现 |
| 复用原清空命令 | [ClearHistoryCommand.cs](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs)：沿用条件判断、清空及通知 |
| 复用原读数类 | [Task010Window.xaml.cs](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs) 中已有 Reading |
| 共享入口例外 | [App.xaml](../../WpfBae/App.xaml)：仅将 StartupUri 改为 TASK-014 窗口；项目默认编译新文件，无需修改 csproj |

这里复用的是 TASK-013 的类和实现；TASK-014 窗口创建自己的一份 ViewModel 实例，窗口与内部控件共用它。原 TASK-013 源码与学员注释未修改。

| TODO | 实践步骤 / 对应 DoD |
| :--- | :--- |
| TODO 1 | 控件内部添加历史标题；步骤 1，DoD 1 的标题部分 |
| TODO 2 | 控件内部添加历史列表并绑定 Readings；步骤 1/2，DoD 1/2 |
| TODO 3 | 控件内部添加清空按钮并绑定 ClearCommand；步骤 1/2，DoD 1/2/3 |

完成后按步骤 3 运行连续读取、换 ID、两种空白、清空与恢复场景；TODO 写完不自动代表 DoD 通过。不引入新框架、依赖属性、Style/Template、真实通信或异步采集。

## 当前必要讲解与首个动作

`UserControl` 是 WPF 提供的类型，用已有控件组合一块界面；`ReadingHistoryControl` 是教练在本题自定义的类。它嵌在 Task014Window 内部。控件的 `.xaml` 描述内部界面，`.xaml.cs` 定义同一个类：`x:Class` 写完整类名，C# `partial` 允许该类由手写部分与构建生成的部分共同组成；构造函数调用 WPF 生成的 `InitializeComponent()`，创建 XAML 中的界面。

窗口声明 `xmlns:history="clr-namespace:WpfBae.Exercises.Task014"`，给这个 C# 命名空间起一个 XAML 前缀 `history`；前缀是教练取的名字。窗口里的 `<history:ReadingHistoryControl ... />` 让 WPF 创建这个类的一个控件实例并放到布局中。

窗口后台的 `new ReadingViewModel()` 创建数据对象，`_viewModel` 字段保存引用，`DataContext = _viewModel` 把它交给窗口的绑定。历史控件没有另设 DataContext，所以沿界面层次继承同一个对象；内部列表的 Binding 从它读取 Readings，按钮的 Binding 从它读取 ClearCommand。继承这里指使用父元素的属性值，不是复制数据，也不是让控件类继承 ReadingViewModel。若在控件后台另建 ViewModel 并设 DataContext，窗口读取与控件历史就会使用不同对象。本题保留已有连接，后续只在 XAML 中补界面和绑定。

首次只做 TODO 1：在 [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-014-history-usercontrol/ReadingHistoryControl.xaml) 的 `<StackPanel>` 内添加：

```xml
<TextBlock Text="读取历史" />
```

TextBlock 是 WPF 文字控件，Text 是它要显示的文字；StackPanel 默认把内部元素从上往下排。这行属于控件内部，因此窗口嵌入此控件时也显示这段文字。

从工作区根目录 `D:\host-computer-st` 运行：

```powershell
dotnet run --project .\WpfBae\WpfBae.csproj
```

预期实际窗口标题为“TASK-014：用 UserControl 封装读取历史”，新增的“读取历史”出现在“读取温度”按钮与状态提示之间。现在列表和清空按钮尚未实现，先确认标题后再继续；编辑器预览不替代运行证据。完成后可回复现象；全部实现后用“检查 TASK-014”提交检查。

参考：[UserControl 官方说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.usercontrol?view=windowsdesktop-10.0)、[DataContext 继承说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.datacontext?view=windowsdesktop-10.0)。本轮教学不计自主掌握证据。

## 验收标准（DoD）

- [x] 项目可编译并实际打开任务窗口；窗口中确实嵌入自定义 UserControl，历史标题、列表及清空按钮位于该控件中，窗口输入和读取仍可使用。
- [x] 控件使用父窗口的同一 ViewModel，内部列表/清空按钮分别通过 Binding 使用原 Readings/ClearCommand；连续读取及修改 ID 后当前读数追加、旧记录保留、状态正确更新。
- [x] 空字符串/纯空格有提示且不改变历史；历史为空时清空禁用，读取后启用，实际点击控件内清空后历史为空且禁用，再读取恢复；有实际窗口运行证据。

## 可选巩固

- 对照窗口和控件文件说明历史区域封装的位置；不单设口试。
- 暂不要求控件独立数据 API、多个控件同时复用、依赖属性或模板化设计。

## 状态与证据

- 2026-10-12 最终审查：三项必需 DoD 全部通过，状态 ✅ 已通关。学员补齐控件内标题、列表和清空按钮，列表绑定 Readings、按钮绑定 ClearCommand；教练未修改学员源码。证据：[9 个实际窗口场景](./evidence/TASK-014/results.json)、[构建、原程序启动及源码哈希](./evidence/TASK-014/verification.json)、[审查程序](./evidence/TASK-014/Review.cs)、[审查项目](./evidence/TASK-014/Review.csproj)、[三条历史的窗口内容渲染](./evidence/TASK-014/history-three-readings.png)、[清空后的窗口内容渲染](./evidence/TASK-014/history-cleared.png)。以下启动/创建记录为历史状态。
  - DoD 1 **通过**：实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出码 0、0 警告/错误；以 `Start-Process -WindowStyle Hidden -PassThru` 启动原 WpfBae.exe，观察标题“TASK-014：用 UserControl 封装读取历史”，CloseMainWindow 正常关闭、退出码 0。审查项目引用已构建的原程序集，实际 Show 原 Task014Window；确认标题、列表和按钮都在 ReadingHistoryControl 内，窗口输入、读取及状态控件有可见尺寸且位于窗口范围内。图片由实际已显示窗口的内容渲染导出，不是设计预览；导出时补白底用于显示透明内容的文字。
  - DoD 2 **通过**：窗口、历史控件、内部列表及清空按钮的 DataContext 为同一个原 ReadingViewModel；列表 ItemsSource 为原 Readings 集合，两个按钮为原命令对象，完整输入/列表/命令/状态绑定全程有效。通过 TextBox.SetCurrentValue 保留绑定并验证 DeviceId 写回，再经 ButtonAutomationPeer/IInvokeProvider.Invoke 执行实际按钮的 WPF 命令路径，不直接调用 Execute。TEMP-001 连读两次、改 TEMP-002 追加第三条，旧记录对象保留；实际行文字包含当前 ID、26.5、°C，状态“读取成功”。
  - DoD 3 **通过**：空字符串、纯空格均提示“该 Id 不合法”，原 3 条历史保留且无集合变化；初始清空禁用，读取后启用。实际调用控件内清空按钮后列表/集合均为 0、状态“已清空”、按钮禁用；输入两侧带空格的 TEMP-003 再读，追加 Trim 后 ID 并启用，再清空恢复禁用。9 个场景全部通过。源码前后 9 个 SHA256 全部一致，审查未代改实现。
  - 边界：ListBox 未填写骨架建议的 x:Name，但绑定、显示及行为均正常；命名不在必需 DoD 中，不作为阻碍。初始提示仍是教练提供的 TODO 文字，不影响功能验收。未观察到新的典型认知错误，不新增错题。可选巩固未单独验收，不计已完成。
  - 掌握范围：学员在已有 UserControl/窗口连接上自主组织三个内部元素，并将 ItemsSource/Command 连接原公开属性，已验证完整运行行为。类定义、XAML 命名空间、父窗口实例创建和 DataContext 连接由教练提供；不据此认定自主设计通用控件或理解全部继承机制，[WPF 控件 Q1](../questions/wpf-usercontrol.md) 的机制仍待复习。
  - 下一步依据：计划 UserControl → Style/Template，已有界面组合和绑定应用证据，先练把读取/清空按钮的重复外观设置提取成一个显式 Style。仅创建 [TASK-015](./TASK-015.md)，状态 ⚪ 未开始，未创建其源码或切换 App.xaml；Template、图表及先前待复习内容仍需后续实践，不认定阶段整体通关。
- 2026-10-12：TASK-013 学员自主修复空白分支后，三个必需 DoD 全部验证通过，按计划只创建本任务卡。状态 ⚪ 未开始；未创建 TASK-014 源码、未切换 App.xaml、未运行或验收本任务。阶段与待复习机制范围保持原状。
- 2026-10-12 启动：用户要求“开启task014”；确认目标目录不存在后创建上述四个文件，复用 TASK-013 原实现，仅切换 App.xaml 启动入口。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出码 0，0 警告 / 0 错误。未启动或验证实际窗口，历史标题、列表和清空按钮仍为 TODO；三个必需 DoD 均未勾选，状态 🟡 进行中。首个动作仅实现并运行观察标题，UserControl/DataContext 继承等机制待复习，不记学员错题。
