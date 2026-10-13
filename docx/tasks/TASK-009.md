# TASK-009：用 WPF 窗口与按钮事件显示设备读数

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」的 Window、Button/TextBox、Grid/StackPanel、XAML 与 Event；具体练习及验收为按当前实践设计，原文未指定此任务。
- 选择依据：TASK-008 的接口和多态读取必需验收已通过，先前事件、异步与配置均已有实践。按路线把设备数据的展示从控制台推进到窗口，从基础控件和按钮事件入手。Task/线程、属性和 using 等概念继续结合后续实践复习，不因 TASK-001～008 通关就认定第一阶段无待补内容。
- 难度/预计时间：待补充（计划未给出）。
- 知识笔记：[WPF 窗口、布局与按钮事件](../notes/TASK-009-wpf-window-events.md)（2026-10-13 根据本卡、当前源码与问答详细整理；本轮未重新运行，保留既有通关状态与掌握边界）。

## 场景与目标

设备操作员在窗口中填写设备 ID，点击按钮后，在同一窗口看到该设备的一条模拟温度读数。这次先建立 WPF 窗口、XAML 布局与事件处理的对应关系。

本任务使用现有 .NET 环境中的 WPF，不接真实设备、不引入新框架。先通过按钮事件更新控件；Binding、Command、MVVM 按计划后续逐步学习，不在本任务预建后续任务。

## 用户实践与产出

1. 准备可运行的 WPF 窗口，窗口标题体现设备读数场景。
2. 使用 XAML 的 Grid 或 StackPanel 布局，包含设备 ID 的 TextBox、触发读取的 Button，以及显示结果和状态的 TextBlock；控件有必要的名称，调整窗口大小时文本不重叠、不被遮挡。
3. 在窗口的 C# 代码中实现 Button.Click 处理器，读取当前输入的设备 ID，创建一条模拟温度读数并更新界面，包含设备 ID、数值及合理单位。可使用固定样例数值，本任务不要求真实通信或异步等待。
4. 修改输入 ID，再次点击，结果应使用新的 ID；空白输入时显示提示，不显示该次成功读数。观察 XAML 中事件关联与代码中处理器的对应，不设单独口试。

## 目录与开始约定

2026-10-09 用户要求“开启task009”，并提供新建项目 `WpfBae/`。已检查源码树、解决方案、项目文件、App.xaml 和 MainWindow：项目使用 `net10.0-windows`，启用 WPF；MainWindow 是空模板。实际复用用户项目，不另建此前拟定的 WpfPractice。

实际专属目录：`WpfBae/Exercises/TASK-009-window-events/`，创建前确认不存在。

- 新建 [Task009Window.xaml](../../WpfBae/Exercises/TASK-009-window-events/Task009Window.xaml)：最小窗口及 StackPanel、具名输入/结果/状态控件与按钮 Click 关联；布局练习保留 TODO 1。
- 新建 [Task009Window.xaml.cs](../../WpfBae/Exercises/TASK-009-window-events/Task009Window.xaml.cs)：构造函数、InitializeComponent 和事件处理器签名；输入校验、模拟温度与显示留作 TODO 2/3，初始按钮仅显示进入事件的占位提示。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml) 仅将 StartupUri 从 MainWindow.xaml 改为任务窗口路径；保留原 [MainWindow.xaml](../../WpfBae/MainWindow.xaml) 和 [MainWindow.xaml.cs](../../WpfBae/MainWindow.xaml.cs)。复用 [WpfBae.csproj](../../WpfBae/WpfBae.csproj) 和 [WpfBae.sln](../../WpfBae/WpfBae.sln)，默认包含新文件，无须增加依赖或改动 Base。

## 骨架、首个动作与运行方式

首个动作：先阅读[WPF 布局入门 Q1](../questions/wpf-layout.md)，理解 Window、基础控件、StackPanel 与 XAML 属性。打开 Task009Window.xaml，原样运行观察控件从上到下的顺序；关闭窗口，把 TextBox 的 `Margin="0,8,0,12"` 改为 `Margin="0,8,0,24"` 后重新运行，观察输入框到按钮的间距。再修改 Window.Title，观察标题栏。先沿用 StackPanel，不要求一开始改成 Grid；理解当前布局后再在配套 .xaml.cs 中完成 TODO 2/3。

| TODO | 实践步骤 | 对应 DoD |
| :--- | :--- | :--- |
| 1：修改窗口标题，调整 StackPanel 或 Grid 布局及间距，检查窗口缩小时是否可读 | 步骤 1/2 | 第一项的实际窗口与布局 |
| 2：每次点击读取当前 Text，Trim 后检查空白，提示、清除旧结果并提前返回 | 步骤 3/4 | 第二项的当前 ID；第三项的空白校验与恢复 |
| 3：构造模拟温度数值和单位，更新结果与成功状态，移除末尾占位赋值 | 步骤 3/4 | 第二项的完整读数及修改 ID 后更新；第三项的正常恢复 |

在工作区根目录 `D:\host-computer-st` 执行以下命令可启动窗口（此处为学员运行方式，实际验证另见证据）：

```powershell
dotnet run --project .\WpfBae\WpfBae.csproj
```

也可在 Rider 打开 WpfBae.sln，运行 WpfBae 项目；当前 App.xaml 会启动 Task009Window，编辑原 MainWindow 不会改变当前任务窗口。

完成后依次观察：`TEMP-001` → 改为 `TEMP-002` → 清空输入或仅输入空格 → 再输入 `TEMP-003`。正常情况显示对应 ID、温度和 °C；空白情况显示提示且清除旧结果；再次输入有效 ID 能恢复。调整窗口大小检查布局，记录运行现象或截图，然后发送“检查 TASK-009”。初始骨架不产生温度读数，编译成功不代表 DoD 通过。

### 必要概念与常见坑

- XAML 描述窗口的控件树及布局；配套 C# 文件处理行为。`x:Class` 与 C# 的命名空间和类名必须一致，InitializeComponent 加载此窗口的 XAML。
- `x:Name` 让代码可以访问对应控件；输入框和 TextBlock 都有 Text 属性，本任务直接读取或赋值即可。
- `Click="ReadButton_Click"` 是骨架已提供的事件关联。点击按钮时 WPF 调用同一窗口的处理器，可联系 TASK-002 的事件订阅经验理解；sender 是事件来源，e 是路由事件参数，本任务暂不需要使用它们。
- StackPanel 按顺序排列子控件，Grid 按行列分配空间；避免通过固定坐标摆放所有内容。TextWrapping 可以让结果文字换行。
- `Console.WriteLine` 不会把读数显示到这些控件上；应更新 Text。空白输入要提前返回，成功后也要移除占位状态赋值，避免它覆盖自己设置的提示。

## 验收标准（DoD）

- [x] WPF 项目可编译并实际打开窗口；XAML 中存在设备 ID 输入、读取按钮及结果/状态控件，常用窗口尺寸下可读，无重叠或遮挡。
- [x] 点击按钮实际进入已关联的事件处理器，窗口显示当前设备 ID、模拟温度数值与单位；修改 ID 后再次点击，结果随输入变化。
- [x] 空白设备 ID 输入被合理处理，有清晰提示且不显示该次成功读数；正常输入仍可再次读取，窗口保持可用。

## 可选巩固

- 对照已有 C# 事件练习，结合当前代码说明 XAML 的 Click 关联如何找到 C# 处理器，作为实践注释即可。
- 对过长的设备 ID 设置合理的换行或宽度约束，检查缩小窗口后的布局。

## 状态与证据

### 2026-10-09 审查：✅ 已通关

本轮仅审查、运行并更新记录，未修改学员源码。学员自主增加 Reading 类型与 ToString，读取当前 Text.Trim()，空白时提示、清除结果并提前 return；正常时显示绑定当前 ID 的固定温度和成功状态。核心实现符合本任务要求。

| 必需 DoD | 结果 | 实际证据 |
| :--- | :--- | :--- |
| 编译、实际窗口与布局 | 通过 | `dotnet build .\WpfBae\WpfBae.csproj --no-restore` 退出码 0，0 警告、0 错误。通过 Start-Process 启动原 WpfBae.exe，检测到非零窗口句柄 4918754，标题为“TASK-009：设备读数练习”，CloseMainWindow 后退出码 0。原窗口在默认 560×360、最小 360×300 下的渲染已目视核对，各控件位于可用内容区域内，无重叠或遮挡。 |
| 按钮关联、完整读数及修改 ID | 通过 | 临时验证程序引用本次构建的原 WpfBae.dll，加载原任务 XAML 和代码，通过 ButtonAutomationPeer 的 IInvokeProvider.Invoke 触发实际按钮事件链路，没有直接调用处理器或替换逻辑。TEMP-001 → `TEMP-001 °C 26.5`；TEMP-002 → `TEMP-002 °C 26.5`，两次状态均为“读取成功”。 |
| 空白输入与恢复 | 通过 | 输入空字符串和三个空格，结果均为空，状态均为“ID 为空或全是空白，请输入正确的ID”；随后输入带两侧空格的 `  TEMP-003  `，显示 `TEMP-003 °C 26.5` 和“读取成功”。验证程序退出码 0。 |

验证方式及证据留存：

- 原应用启动命令为 `Start-Process -FilePath 'D:\host-computer-st\WpfBae\bin\Debug\net10.0-windows\WpfBae.exe' -WindowStyle Hidden -PassThru`；等待窗口创建后检查句柄/标题，并正常关闭。本轮创建的验证进程已结束。
- 临时程序位于 `C:\Users\lbwxxc\AppData\Local\Temp\TASK-009-review-819c75ff613f49ba9411d57bad1ac884/`，实际执行 `dotnet build <该目录>\Review.csproj --no-restore` 和 `Start-Process <该目录>\bin\Debug\net10.0-windows\Review.exe -WindowStyle Hidden -PassThru`，最终均退出 0。宿主使用包含原程序集名的绝对 pack URI 加载同一窗口，避免临时宿主的资源路径与原入口程序集混淆；原 App.xaml 相对启动路径另由原 WpfBae.exe 实际启动验证。
- [运行结果 JSON](./evidence/TASK-009/results.json)、[默认尺寸渲染](./evidence/TASK-009/default.png)、[最小尺寸渲染](./evidence/TASK-009/minimum.png) 已保存。渲染图为 WPF RenderTargetBitmap 的窗口内容，不包含系统标题栏；标题另由进程窗口信息验证。事件验证为自动化调用按钮 Invoke，无手工鼠标点击，不扩大到未验证的键盘或其他输入方式。
- 运行前后 SHA256 一致：XAML `DDB1372AABF0455A3A3591454878C92D7D3B50CF91178C912D06B2BB7E5B232C`；配套 C# `9F15DF19170979192FF205E208710E7AC812103787B50E3C84DC8A609BFDF3C5`；App.xaml `8E5CC3E83E44EAD17609B5AF161A35E3AECBE86598825988939A4C509659990E`。
- 可选整理：ToString 的显示顺序可由“ID 单位 数值”改为“ID 数值 单位”；初始状态文字和已完成 TODO 注释也可整理，不追加验收门槛。末尾 return 合法，不列为错误。
- 可选长 ID 场景未运行。StackPanel/Grid 与 Margin 的布局由骨架提供，本轮只观察到学员修改文字，布局问答 Q1 继续待复习，不据此认定自主布局能力或 Grid 已掌握。
- 下一步：[TASK-010：用 Binding 更新模拟温度读数](./TASK-010.md)。当前按钮事件与控件 Text 更新已通过，按计划下一个主题学习 Binding，把结果控件的更新连接到数据属性；只创建这一张任务卡，状态 ⚪ 未开始，不创建新骨架或切换入口。

### 启动与答疑历史

- 2026-10-09 教学说明补充：学员明确反馈 WPF 初学者，尚不理解 StackPanel/Grid。已补充窗口、控件、XAML 标签与属性、两种布局容器及 Margin 的基础说明，并把首个动作细化为原样运行与单个间距属性对照；详见[布局问答 Q1](../questions/wpf-layout.md)，状态待复习。本轮未修改已有任务源码、未运行或验收；任务保持 🟡 进行中，DoD 不变。
- 2026-10-09 启动：复用学员 WpfBae 项目，创建任务专属窗口骨架并连接 App.xaml。状态 🟡 进行中；核心 TODO 未实现，全部必需 DoD 保持未勾选。学习进度、任务索引与导航同步启动状态；未生成下一任务。
- 2026-10-09 骨架验证：在工作区根目录实际执行 `dotnet build .\WpfBae\WpfBae.csproj`，退出码 0，0 个警告、0 个错误。实际执行 `git diff --check`，退出码 0，无差异格式错误，仅有 Git 行尾转换提醒；任务卡文件链接核对通过。本轮未启动窗口或操作按钮，布局、点击、输入变化和空白恢复尚待实际验证，不勾选 DoD。
- 2026-10-07 创建项目答疑：学员截图显示 Rider 已选择 WPF Application、C# 与 net10.0。当前源码树仍仅有 Base/Base.csproj 和 Base/Base.slnx，未见 WPF 项目。建议单独创建 WpfPractice 并放在工作区目录，已说明项目与解决方案的区别及创建设置，详见[项目问答](../questions/dotnet-projects.md)。本轮只答疑和记录，未创建项目或开始任务，状态保持 ⚪ 未开始，所有 DoD 未勾选。
- 2026-10-05：TASK-008 最终复查通过后，依据计划 WPF 起步顺序创建这一张递增 ID 任务卡。状态 ⚪ 未开始；未创建项目或骨架，未运行、未验收。待用户明确开始。
