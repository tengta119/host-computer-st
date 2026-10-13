# TASK-010：用 Binding 更新模拟温度读数

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中 Event 后的 Binding；具体练习与验收按当前实践设计，原文未指定此任务。
- 选择依据：TASK-009 已验证当前输入读取、按钮事件、空白处理及结果显示。下一步以相同设备场景学习将控件文字连接到数据属性，减少同时引入的新内容；StackPanel/Grid 与间距继续随实践复习。
- 难度/预计时间：待补充（计划未给出）。
- 知识笔记：[Binding、DataContext 与属性变化通知](../notes/TASK-010-wpf-binding.md)（2026-10-13 根据当前源码、本卡与问答详细整理；本轮仅静态核对，未重新运行，通关状态与原掌握边界保留）。

## 场景与目标

仍在窗口输入设备 ID，点击读取后显示固定模拟温度。此前是在 C# 中直接给结果 TextBlock.Text 赋值；这次把结果和状态保存到数据对象的属性中，由 Binding 更新显示。

本任务先解释三个概念，再按最小示例连接骨架：

- Binding（绑定）：在 XAML 中指定控件的某个属性从哪个数据属性取值，例如 `Text="{Binding ResultText}"`。
- DataContext：为窗口中的绑定提供默认数据对象。
- INotifyPropertyChanged：当数据对象的属性变化时通知界面重新取值；只修改普通属性不会自动让已显示的文字持续更新。

沿用现有 WPF 和模拟温度，不引入通信、异步、Command、MVVM 框架或新依赖。输入框仍可在按钮处理器中读取，本任务先练习结果和状态的单向显示绑定。

## 用户实践与产出

1. 建立任务专属窗口和简单数据类，包含结果文字与状态文字两个公开属性。
2. 设置窗口 DataContext，在 XAML 中将结果和状态 TextBlock.Text 分别绑定到对应属性；保留输入框与按钮事件。
3. 数据类实现 INotifyPropertyChanged，在属性更新时发出对应名称的通知。按钮处理器只修改数据对象的结果与状态属性，不直接给这两个 TextBlock.Text 赋值。
4. 正常读取、修改 ID、空白清除与恢复继续保持 TASK-009 的行为，用多次操作确认通知确实使界面更新。

## 目录与开始约定

2026-10-09 用户明确要求“开启task010”后，已重读导航、进度、任务索引、任务卡及 WpfBae 项目、启动入口、TASK-009 学员源码；确认目标目录不存在，复用现有 net10.0-windows WPF 项目，不增加依赖。

实际专属目录：`WpfBae/Exercises/TASK-010-reading-binding/`。

- 新建 [ReadingDisplayState.cs](../../WpfBae/Exercises/TASK-010-reading-binding/ReadingDisplayState.cs)：两项显示属性、保存值的字段、INotifyPropertyChanged 事件及通知工具；关键的 setter 通知保留 TODO 3a/3b。
- 新建 [Task010Window.xaml](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml)：沿用已熟悉的 StackPanel、输入框和按钮；结果/状态暂为静态占位文字，Binding 由 TODO 2a/2b 完成。
- 新建 [Task010Window.xaml.cs](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)：数据实例、构造函数及按钮事件签名；DataContext 留 TODO 1，实际读取与数据属性更新留 TODO 4a/4b。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml) 仅将 StartupUri 改为新任务窗口；原 TASK-009 目录和 MainWindow 均未修改。沿用 [WpfBae.csproj](../../WpfBae/WpfBae.csproj) 默认包含规则，无须改项目文件。

## 从一个绑定开始

属性语法先补充：[C# 属性 Q3](../questions/csharp-properties.md)解释字段与属性、完整 get/set、value、箭头简写及与普通方法的对应。学员尚未学过这部分，不能把“已有属性代码”视为已理解。讲解先用普通花括号和 return，再联系本任务的公开显示属性及通知；保留已有源码和 TODO，不要求另外考试或重写骨架。

先读[数据类、对象与 DataContext 的来源说明 Q2](../questions/wpf-binding.md#q2readingdisplaystate-是什么datacontext--_state-究竟做了什么)，再读同文件 Q1 的绑定与通知。ReadingDisplayState 是本任务自行定义的普通 C# 类，保存结果和状态文字；_state 是由 new 创建的实例引用；DataContext 是窗口已有的 WPF 属性，赋值为 _state 后，默认绑定从这个对象取数据。单独设置 DataContext 不会改变仍使用静态 Text 的控件。先看清“类型 → 对象 → 窗口的数据来源 → 读取对象属性”这条连接，再动手。

你在 TASK-009 使用 `ResultTextBlock.Text = ...` 指定控件文字。本次把文字放在数据对象的 ResultText 属性中，让 WPF 把属性值显示到控件。

下面只是一个属性的语法示例，Caption 表示数据对象上的公开属性：

```xml
<TextBlock Text="{Binding Caption, Mode=OneWay}" />
```

- Text 是目标控件的属性。
- 大括号中的 Binding 表示从数据来源取值；Caption 是来源属性名，不是控件名字，也不会把单词 Caption 直接显示到窗口。
- DataContext 提供默认数据对象；窗口设置后，内部控件通常继承这个对象。当前练习的数据对象已经创建为 `_state`。
- Mode=OneWay 表示数据属性传向控件，本次无需让结果控件反向写入数据对象。
- 初次连接时 WPF 能读取属性值；之后要持续显示新的属性值，需要 PropertyChanged 事件通知。声明接口和事件之后，还要在 setter 实际发出通知。

属性 setter 中的 `value` 是赋入的新值；`_resultText = value` 保存值。提供的 OnPropertyChanged 方法负责发布事件，你在 setter 保存值之后调用它，并用 `nameof(ResultText)` 指定发生变化的属性名。StatusText 同理。

## TODO、首个动作与运行方式

首个动作：先对照 ReadingDisplayState.cs 找到 ResultText/StatusText 两项属性及初始文字，再对照窗口代码中的 `_state = new()` 理解实例创建，结合上文理解 DataContext 的对象来源。2026-10-09 答疑时已见学员写入 `DataContext = _state`，保留已有实现，不再要求重复填写 TODO 1。接着完成 XAML 的 TODO 2a，把结果 Text 从静态占位文字改为对 ResultText 的单向绑定。运行后结果区域应出现“尚未读取”，这是数据字段中的初始值。完成 TODO 2b 后，状态应出现“请输入设备 ID，然后读取温度。”。先观察这个小步骤，再学习通知和按钮逻辑。

| TODO | 实践步骤 | 对应 DoD |
| :--- | :--- | :--- |
| 1：设置窗口 DataContext 为同一个 _state 实例 | 步骤 2 | 第一项的数据来源 |
| 2a/2b：结果和状态 Text 分别绑定 ResultText / StatusText，Mode=OneWay | 步骤 2 | 第一项的两个 XAML Binding |
| 3a/3b：两个 setter 保存值后调用通知工具，携带各自属性名 | 步骤 3 | 第二项的属性变化通知；第三项的清除、提示与恢复 |
| 4a：读取当前输入并 Trim，空白时修改数据属性、清除旧结果、提前返回 | 步骤 4 | 第三项的空字符串/纯空格与后续恢复 |
| 4b：有效时修改数据属性，显示当前 ID、温度、单位和成功状态 | 步骤 3/4 | 第二项的正常读取及修改 ID；不直接更新显示控件 |

建议顺序：TODO 1 → 2a/2b → 运行观察初始值 → 3a/3b → 4a/4b → 运行核对输入场景。

在工作区根目录 `D:\host-computer-st` 启动（下方是学员运行方式，实际验证见状态与证据）：

```powershell
dotnet run --project .\WpfBae\WpfBae.csproj
```

也可在 Rider 运行 WpfBae 项目，当前入口已切换为 Task010Window。每次重新编译运行前关闭上一轮窗口。

完成后依次输入 TEMP-001、TEMP-002、空字符串、纯空格、带两侧空格的 TEMP-003 并点击按钮。正常读数应随 ID 更新；空白时提示并清除结果；恢复有效输入后正常显示。保留观察结果，然后发送“检查 TASK-010”。骨架可编译或只显示初始值不代表必需 DoD 通过。

常见坑：属性名大小写必须一致；按钮修改的对象必须是 DataContext 指向的同一个对象；不要在代码中给已绑定的结果/状态 Text 直接赋值，否则可能替换掉该属性上的绑定。输入框继续直接读取 Text 即可。

## 验收标准（DoD）

- [x] 项目可编译并实际打开任务窗口；结果与状态控件通过 XAML Binding 取值，DataContext 正确指向任务数据对象。
- [x] 按钮处理器修改数据属性，通过 INotifyPropertyChanged 更新界面；两次不同 ID 都显示对应 ID、模拟温度与单位，无直接更新结果/状态控件 Text 的替代路径。
- [x] 空字符串与纯空格输入提示且清除旧读数，再输入有效 ID 能恢复正常显示；属性通知覆盖结果和状态的这些变化。

## 可选巩固

- 结合实际代码观察属性名拼写错误时的绑定表现及调试输出。
- 继续通过调整间距和窗口尺寸巩固 StackPanel，不要求同时改用 Grid。

## 状态与证据

### 2026-10-09 审查：✅ 已通关

本轮未修改学员源码。学员自主连接 DataContext 和两项 OneWay Binding，修正了状态绑定路径；在已有 setter 中分别调用 OnPropertyChanged("ResultText") / OnPropertyChanged("StatusText")；按钮通过当前输入构造 Reading，再更新同一个 _state 的属性。字符串名称调用合法，未强制改为 nameof。

| 必需 DoD | 结果 | 实际证据 |
| :--- | :--- | :--- |
| 编译、实际窗口、两个绑定与 DataContext | 通过 | `dotnet build .\WpfBae\WpfBae.csproj --no-restore` 退出 0，0 警告、0 错误。原 WpfBae.exe 检测到窗口句柄 4655442，标题“TASK-010：温度读数绑定练习”，正常关闭退出 0。原窗口数据上下文与私有 _state 是同一对象；两项 Binding.Path 为 ResultText / StatusText，Mode 均为 OneWay。初始文字分别为“尚未读取”和“请输入设备 ID，然后读取温度。”。 |
| 数据属性修改、通知与不同 ID | 通过 | 通过实际按钮的自动化 Invoke 路径，TEMP-001 与 TEMP-002 分别显示 `TEMP-001 26.5 °C ` 和 `TEMP-002 26.5 °C `（末尾空格来自学员 ToString），状态“读取成功”。每次监听到 ResultText 和 StatusText 各一次 PropertyChanged，事件来源与 DataContext 相同；源属性与控件文字一致，绑定始终存在。源码无直接给两项显示控件 Text 赋值的替代路径。 |
| 空白清除、提示及恢复 | 通过 | 空字符串和三个空格均显示“Id 为空或者异常”，结果为空；每次两个属性都发出通知，绑定仍保留。随后输入 `  TEMP-003  `，结果为 `TEMP-003 26.5 °C `，状态恢复“读取成功”。 |

实际验证与证据：

- 验证程序位于 `C:\Users\lbwxxc\AppData\Local\Temp\TASK-010-review-2450352a98884cd480ef32ca162d4ba3/`。实际执行 `dotnet build <该目录>\Review.csproj` 和 `Start-Process <该目录>\bin\Debug\net10.0-windows\Review.exe -WindowStyle Hidden -PassThru`，均退出 0，0 编译警告/错误；临时进程已结束。程序仅引用原构建 DLL，在 WPF 消息循环中加载原 App 和任务窗口，临时宿主的资源 URI 补充原程序集名，不修改项目启动文件。
- 按钮验证使用 ButtonAutomationPeer 的 IInvokeProvider.Invoke，经原 XAML 关联调用原处理器，不直接调用私有处理器、不替换实现。观察同一数据对象的 PropertyChanged 事件并核对名称、来源、源属性、控件文字及绑定保留情况。最后单独修改同一对象的结果/状态属性，界面分别更新为“属性变化验证”和“状态变化验证”，确认属性通知能够独立驱动显示。
- 原应用启动实际执行 `Start-Process -FilePath 'D:\host-computer-st\WpfBae\bin\Debug\net10.0-windows\WpfBae.exe' -WindowStyle Hidden -PassThru`，创建上述窗口，CloseMainWindow 返回 true，退出 0；本轮创建的验证进程均已关闭。事件测试为按钮自动化调用，不扩大到手工鼠标或其他输入方式。
- [运行结果 JSON](./evidence/TASK-010/results.json) 已保存。
- 运行前后 SHA256 一致：Task010Window.xaml `D9417073B745D4B5C771AAE332D2F03B833AF1A7186602A11926EB3CB79EDE3F`；配套 C# `1B924D24EEFD098F051680AE2236C456C116473DD1B8E16D84C91BABE3370180`；ReadingDisplayState.cs `021D997C18D06852A3D4BD2D0776B90E3F2CF74261B470053E7C47D4656F53DC`；App.xaml `8FF21B0F09F8286197EA7572B492EC54346ACE601B63C33E31BB76FBA798D052`。
- 可选整理：已完成 TODO 与“点击暂时不改变任何值”旧注释可以整理；ToString 的末尾空格可删除，不影响本次验收。可选的故意写错绑定名及布局调整未运行，不增加为通关门槛。
- 掌握范围限自主连接的基本绑定、正确的数据对象来源、setter 内通知调用与按钮修改数据属性。类结构、get/set 保存逻辑、事件声明和发布工具由骨架提供；new()/readonly、nameof、?.Invoke、框架参数类型及自主设计数据类等不随任务通关一并标已掌握。绑定 Q1 按本次应用范围更新；其他概念问答继续待复习。
- 下一步：[TASK-011：用 ObservableCollection 显示温度读取历史](./TASK-011.md)。本次单条属性绑定与变化通知已验证，按计划 Binding → ObservableCollection，学习一组数据新增/清空时的界面更新。只创建这一张任务卡，状态 ⚪ 未开始；尚未创建骨架或切换 App.xaml，不宣称 WPF 阶段整体通关。

### 启动与答疑历史

- 2026-10-09 参数类型来源答疑：补充[绑定 Q4](../questions/wpf-binding.md)，说明 PropertyChangedEventArgs 是 .NET 的 System.ComponentModel 命名空间中的框架类，using 的作用、构造参数为属性名、PropertyName 携带该名称及 WPF 收到后再读取属性值。本轮已见学员在两个 setter 添加正确的字符串名称通知调用，保留源码，未运行或验收；字符串写法合法，不强制改为 nameof。问答待复习，TASK-010 保持 🟡 进行中，全部 DoD 未新增勾选。
- 2026-10-09 属性语法答疑：学员明确反馈未学过 `get => _resultText` 与 `set` 实现。已补充[C# 属性 Q3](../questions/csharp-properties.md)，说明完整 get/return 写法、赋值调用 setter、value 的来源、私有字段存值及空 setter 不保存数据，与普通 getter/setter 方法对照。首次讲解采用完整代码块，不默认属性简写已掌握。本轮未修改源码、未运行或验收，问答待复习，TASK-010 保持 🟡 进行中，DoD 不变。
- 2026-10-09 Rider 提示与初始绑定答疑：实际 XAML 没有灰色 (???) 与 Path=，这些是编辑器内嵌提示；预览空白不作为运行时绑定失败证据。实际 `dotnet build .\WpfBae\WpfBae.csproj --no-restore` 退出 0、0 警告、0 错误。临时程序引用原构建 DLL 并打开原 Task010Window，验证 DataContext 为 ReadingDisplayState，结果 Text 为“尚未读取”；状态控件当前也绑定 ResultText，因此同样显示“尚未读取”。检查程序 `C:\Users\lbwxxc\AppData\Local\Temp\TASK-010-binding-check-f5e00680187a483cb5f1d469e16661a2/Check.csproj` 实际构建通过，Check.exe 启动后退出 0，进程已关闭；结果在该目录的 `bin/Debug/net10.0-windows/initial-binding.json`。未修改学员源码。首个修正动作是 XAML 第 16 行将状态控件 Binding.Path 改为 StatusText。仅初始结果绑定局部有证据，状态路径待修、通知和按钮逻辑未实现，不勾选任何完整 DoD。详见[绑定 Q3](../questions/wpf-binding.md)，状态待复习，任务仍 🟡 进行中。
- 2026-10-09 数据来源答疑：学员明确反馈尚不理解 DataContext 与骨架自定义的 ReadingDisplayState。已补充 Q2，先解释自定义类与 TASK-009 Reading 的关系、new()/实例引用、Window.DataContext 的作用及 Binding 初次取值路径；后续按这个顺序讲解。源码中已见学员新增 DataContext = _state，未改动或覆盖。本轮仅答疑与同步记录，未运行或验收，状态保持 🟡 进行中，全部 DoD 未勾选；两个绑定问答均待复习，不记录为学员典型错误。
- 2026-10-09 启动：按用户明确请求创建上述三个专属文件并增量修改 App.xaml；核心 TODO 均保留，未覆盖 TASK-009。状态 🟡 进行中，全部必需 DoD 未勾选；进度、任务索引和导航同步，未生成下一任务。Binding 入门说明记录为待复习，未认定学员已掌握。
- 2026-10-09 骨架验证：在工作区根目录实际执行 `dotnet build .\WpfBae\WpfBae.csproj --no-restore`，退出码 0，0 警告、0 错误。实际用 `Start-Process -FilePath 'D:\host-computer-st\WpfBae\bin\Debug\net10.0-windows\WpfBae.exe' -WindowStyle Hidden -PassThru` 启动，检测到标题“TASK-010：温度读数绑定练习”的非零窗口句柄，CloseMainWindow 后退出码 0；验证进程已关闭。实际执行 `git diff --check` 无差异格式错误，仅 Git 行尾转换提醒；相关文件链接核对通过，TASK-009 和 MainWindow 无源码差异。仅验证骨架编译与启动，DataContext、Binding、setter 通知和按钮逻辑尚未实现，未进行交互验收，所有 DoD 保持未勾选。
- 2026-10-09：TASK-009 三项必需 DoD 通过后，依据计划 Event → Binding 的顺序创建这一张递增 ID 任务卡。状态 ⚪ 未开始；未创建新任务骨架、未切换 App.xaml、未运行或验收 TASK-010。保留 WPF 初学者教学要求：解释用途、最小语法和可观察效果后再动手。
