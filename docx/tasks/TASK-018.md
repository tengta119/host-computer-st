# TASK-018：用 Canvas 与 Polyline 绘制模拟温度折线

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」中的数据图表。
- 选择依据：TASK-017 的共享模板、Padding 连接和禁用外观已实际验收，历史与命令仍正常。继续计划中的图表入门，先理解温度数值如何决定线条的高度；采用 WPF 原生元素，避免同时引入通信、定时采集或新图表框架。完整实时曲线仍待后续实践，不认定阶段整体完成。
- 难度/预计时间：待补充（计划未给出）。
- 知识笔记：[温度折线、坐标集合与原值标签](../notes/TASK-018-wpf-temperature-polyline.md)（2026-10-14 整理；具体代码与效果见正文，历史证据及掌握范围见文末）。

## 场景与目标

在独立练习窗口中，用三条固定模拟温度 `24、26.5、25 °C` 绘制一条有两个线段的折线。图中能分辨读取顺序、温度单位与高低，温度越高，点越靠上。先学习图表的基本表示，不改 TASK-017 已完成的业务或历史集合。

## 开始前先解释

- Canvas 是 WPF 提供的布局容器，可用坐标安排内部元素；学员有 StackPanel 的使用证据，但尚无 Canvas 证据，先解释坐标与实际窗口的位置对应。
- WPF 界面坐标从左上开始，x 向右增大、y 向下增大。因此温度越高要画得越靠上，需要把数值转换成较小的 y；用两个具体数值先算再看，不只给公式。
- Polyline 是 WPF 提供的折线图形，把多个点按顺序连接；Points 是它的坐标集合属性，Stroke 控制线条颜色，StrokeThickness 控制线宽。图形与温度数组不是同一个对象。
- Point 是 WPF 的坐标类型，PointCollection 是多个坐标的集合；先用完整 `new Point(x, y)` 和普通循环说明，再要求使用。自定义数组、变量、辅助方法、控件名由教练逐个交代，不视为框架 API。
- TextBlock 可显示原始样例数值、温度单位、顺序标签；Canvas.Left/Top 若用于定位标签，必须先解释其具体作用。外层窗口与必要标签骨架由教练提供。
- 图表使用固定温度范围 20～30 °C，固定绘图区与三个横向位置；先给具体边界与点对应关系，不引入自动缩放、坐标轴库或动画。

## 用户实践与产出

1. 教练准备能运行的窗口、绘图区和可见线条占位后，先只调整一个点的 y，实际观察它上下移动，理解屏幕坐标方向。
2. 在已解释的普通方法/循环骨架内，把三条固定温度转为三个坐标，再赋给同一条 Polyline 的 Points。文字标注与点对应，温度高的点在上方。
3. 单独修改中间温度为 `28 °C`，重新运行，确认中间点上移而其他两点不变，随后可恢复；用运行现象与实际点坐标验收。

## 目录与开始约定

2026-10-13 用户明确开启后，确认目标目录不存在，已新建专属目录 `WpfBae/Exercises/TASK-018-temperature-polyline/`。沿用现有 `net10.0-windows` WPF 项目，不新增第三方包。

| 文件 | 来源与用途 |
| :--- | :--- |
| [Task018Window.xaml](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml) | 教练新建：窗口、Canvas、绘图区背景、蓝色 Polyline、范围与顺序标签。 |
| [Task018Window.xaml.cs](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml.cs) | 教练新建：样例数组、占位线、标签显示、转换方法与普通循环骨架；核心计算和绘图留 TODO。 |
| [App.xaml](../../WpfBae/App.xaml) | 共享入口例外：只将 StartupUri 从 TASK-017 切到 Task018Window。 |
| [WpfBae.csproj](../../WpfBae/WpfBae.csproj) | 复用，不修改；已有项目会编译新目录中的 C# 与 XAML。 |

原任务源码、学员注释与历史记录保留。下面先说明实际对象，再按顺序动手。

## 首个动作：只移动中间点

窗口中的浅灰色区域是画温度的位置。`ChartCanvas` 是教练给这个 Canvas 起的名字；Canvas 是 WPF 提供的容器。它的左上角是 `(0,0)`，向右走 x 增大，向下走 y 增大。坐标单位是 WPF 的界面长度单位，不必等同于显示器物理像素。

蓝线叫 `TemperatureLine`，是 XAML 中的一个 Polyline。Polyline 由 WPF 提供，用来按顺序连接多个坐标。`Stroke="#2563EB"` 给这条线设置蓝色，`StrokeThickness="3"` 给它设置线宽。

打开上方后台文件的 `ShowPlaceholderLine()`，找到 TODO 1 后这行：

```csharp
points.Add(new Point(280, 180));
```

`Point` 是 WPF 使用的坐标类型。`new Point(280, 180)` 用 C# 的 `new` 写法创建一个坐标，表示向右 280、向下 180 的位置。`points` 是这个方法自己创建的坐标集合变量；`Add` 把这个坐标加入集合。它前后各有一个坐标，分别对应左、右端点。

**先原样运行一次，再只把这行的第二个数 180 改成 100。** 不改其他点，不改温度数组。从工作区根目录运行：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

修改后保存，关闭旧窗口，再执行同一命令。原样骨架应显示一条水平蓝线；修改后中间点应上移 80 个界面单位，两端不变，蓝线变成有一个峰的折线。此处是待学员实际观察的预期，教练本轮未运行窗口；占位点与标签上的温度尚未对应，不能据此勾选 DoD。

## 后续动作：把温度转换成坐标

先完成上面的单点观察，再进行本节。图中的绘图区从 x=100 到 460，从 y=40 到 240。上边 y=40 表示 30 °C，下边 y=240 表示 20 °C；第 1、2、3 次读取的横坐标固定为 100、280、460。

从 20 到 30 度共有 10 度，绘图区高度是 `240 - 40 = 200`，所以升高 1 度，点应向上移动 `200 / 10 = 20`。例如：

- 24 °C 比 20 °C 高 4 度，从底部向上移动 `4 × 20 = 80`，所以 y 为 `240 - 80 = 160`。
- 26.5 °C 比 20 °C 高 6.5 度，向上移动 130，所以 y 为 110；它比 24 °C 的点靠上。

这里减去的距离是“比最低温度高出多少度”对应的距离，不能直接把 26.5 当作 y。按同样办法处理输入参数，这就是 TODO 2 的计算。

`ConvertTemperatureToY` 是教练自定义的普通方法名，不是 WPF API。`double temperature` 接收调用时传入的温度，`double` 返回值表示计算好的 y。目前 `return 140;` 只是保证方法能编译的固定返回值，要换成使用参数的计算。

`_temperatures` 是教练定义的 C# 数组字段，保存三个原始温度；`_xPositions` 是另一个数组，保存三个横坐标。它们都不是屏幕上的线。`DrawTemperatureLine()` 也是教练定义的方法，其中 `for` 是 C# 普通循环：i 依次为 0、1、2，取出同一次读取的温度和 x，再调用转换方法取得 y。

接下来才需要把坐标交给线：`PointCollection` 是 WPF 提供的坐标集合，`new PointCollection()` 创建一份空集合。每轮循环把 `new Point(x, y)` 加进去，最后把集合赋给 `TemperatureLine.Points`。`Points` 是蓝线的 WPF 属性；赋值后蓝线按集合中的先后顺序连接。温度数组负责保存数值，坐标集合负责保存位置，这两个对象各有用途。

骨架中的 `ShowPlaceholderLine()` 已展示集合创建、单点加入及属性赋值的普通写法。完成 TODO 2、3 后，再将构造方法中唯一的 `ShowPlaceholderLine()` 调用替换为 `DrawTemperatureLine()`。窗口构造时只运行一次绘图；修改源码之后必须重新运行才能看到新结果。

标签由教练提供的 `ShowReadingLabels()` 从同一温度数组取值，重新运行时会随温度变化。XAML 中 `Canvas.Left="235"`、`Canvas.Top="260"` 设置中间标签的左上角距离 Canvas 左侧 235、上侧 260，使它位于中间点的下方；它们设置的是标签位置，没有修改蓝线的坐标。不要给整条蓝线另加 Left/Top，否则点还会整体偏移。

## TODO 与实践、DoD 对应

| TODO | 位置与工作 | 对应 |
| :--- | :--- | :--- |
| 1 | `ShowPlaceholderLine()` 中只改中间点 y，运行观察向上移动。 | 实践 1；DoD 1 的坐标方向准备，不能单独通过 DoD。 |
| 2 | `ConvertTemperatureToY()` 根据传入温度返回 y，替换固定返回值。 | 实践 2 / DoD 2。 |
| 3 | `DrawTemperatureLine()` 循环内加入 Point，循环后赋给 Points；最后替换构造方法中的绘图调用。 | 实践 2 / DoD 1、2。 |
| 4 | 仅将 `_temperatures` 中间值 26.5 改为 28，重新运行并比较前后；标签自动跟随。 | 实践 3 / DoD 3。 |

完成后提交源码，并保留 26.5 与 28 两次实际运行的图或观察记录，再请求“检查 TASK-018”。可恢复数组中间值；未实际观察的现象不要记录为通过。

## 验收标准（DoD）

- [x] 项目编译并实际打开 TASK-018 窗口；Canvas 中折线可见，三个点按读取顺序连接，单位/范围/顺序标注可读。
- [x] 原始三条温度与三个点一一对应，坐标来自对样例数值的转换；温度越高，点越靠上，三个点均位于约定绘图区内。
- [x] 中间温度单独改为 28 °C 后实际重新运行，中间点上移、另外两点不变，有修改前后运行证据；图形和文字保持可读。

## 2026-10-13 审查结果

三个必需 DoD 全部通过，状态 ✅ 已通关。当前源码已在循环中添加 `Point`、赋给原 Polyline.Points，并在构造时调用温度绘图方法。原样例仍为 24、26.5、25；审查未修改学员源码。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1：构建与实际显示 | 通过 | 原项目构建退出 0，0 警告/错误；原 WpfBae.exe 打开 TASK-018 标题窗口、取得窗口句柄并正常关闭退出 0。审查程序加载原程序集，实际 Show() 原 Task018Window，Canvas 560×300 与蓝色折线可见；范围、单位、三次读取说明和标签完整可读，已检查内容图像。 |
| 2：温度与坐标 | 通过 | 原样例三个实际坐标依次为 (100,160)、(280,110)、(460,140)，来自源码温度转换后加入集合，均在 x=100～460、y=40～240 内。26.5 度的点最高、25 度其次、24 度最低；标签显示对应原始值和 °C。 |
| 3：中间温度变化 | 通过 | 独立审查副本只将数组中间值 26.5 改成 28，重新构建和实际打开窗口；点变为 (100,160)、(280,80)、(460,140)。中间点上移 30，两个端点不变，中间标签为 28 °C；图形与文字可读。副本 exe 启动/关闭退出 0，前后内容图像均保留。副本修改及运行由教练执行，不计学员自行修改温度的掌握证据。 |

实际运行命令（均已执行）：

```powershell
dotnet build WpfBae/WpfBae.csproj --nologo
& docx/tasks/evidence/TASK-018/prepare-variant.ps1
dotnet run --project docx/tasks/evidence/TASK-018/Review.csproj -- D:/host-computer-st/WpfBae/bin/Debug/net10.0-windows/WpfBae.dll 26.5 D:/host-computer-st/docx/tasks/evidence/TASK-018 original-26_5
dotnet build docx/tasks/evidence/TASK-018/variant-28/WpfBae/WpfBae.csproj --nologo
dotnet run --no-build --project docx/tasks/evidence/TASK-018/Review.csproj -- D:/host-computer-st/docx/tasks/evidence/TASK-018/variant-28/WpfBae/bin/Debug/net10.0-windows/WpfBae.dll 28 D:/host-computer-st/docx/tasks/evidence/TASK-018 variant-28
& docx/tasks/evidence/TASK-018/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-018/launch-original.json
& docx/tasks/evidence/TASK-018/launch.ps1 -Executable docx/tasks/evidence/TASK-018/variant-28/WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-018/launch-variant.json
& docx/tasks/evidence/TASK-018/verify.ps1
```

证据：[原窗口坐标/显示](./evidence/TASK-018/original-26_5.json)、[28 度副本坐标/显示](./evidence/TASK-018/variant-28.json)、[原 exe 启动](./evidence/TASK-018/launch-original.json)、[副本 exe 启动](./evidence/TASK-018/launch-variant.json)、[源码与前后比较核对](./evidence/TASK-018/verification.json)、[原窗口内容图像](./evidence/TASK-018/original-26_5.png)、[28 度内容图像](./evidence/TASK-018/variant-28.png)。图像来自实际已显示窗口的内容渲染，非设计预览。41 个原项目源码文件 SHA256 前后一致；副本仅 TASK-018 后台文件有变化，文本核对确认唯一变化为数组中间值。

审查工具首次把标签中心的允许误差设为 0.1 个界面单位，原中间 TextBlock 的实际宽度为 90.6067、中心偏差约 0.3033；文字完整可读，不影响本卡要求。工具改为允许 1 个界面单位后重新验证，原/副本均通过。首次工具结果单独保留为 [工具修正前记录](./evidence/TASK-018/review-tool-before-tolerance-fix.json)，不记学员错误、不要求改学员布局。

自主掌握限本骨架中循环创建并加入坐标、给 Polyline.Points 赋值、接通绘图调用。坐标换算、原数据与标签布局不据此升级为自主设计；问答 Q1/Q2 的完整机制仍待复习，不扩大为实时图表、自动缩放或 WPF 阶段整体完成。未观察到新的典型认知错误。

下一步依据：计划明确列出的 DataGrid 尚无自主应用证据；已有 Reading/ObservableCollection/读取与清空命令可复用。只创建 [TASK-019：用 DataGrid 展示设备读数列表](./TASK-019.md)（⚪ 未开始），先巩固同一集合的表格显示，不提前开始新任务。

## 可选巩固

- 后续将真实历史集合连接折线、考虑更多点与范围变化；本次未做不勾选，不计必需验收。
- 不以本卡通关推定完整图表框架使用、自动布局/缩放或完整监控系统已掌握。

## 状态与证据

- 2026-10-13 审查完成：三个必需 DoD 全通过，41 个原源码哈希不变；原项目与独立 28 度副本构建均 0 警告/错误，两个 exe 启动/关闭退出 0，实际窗口坐标和可读性验证通过。详细结果见上方；状态 ✅ 已通关，仅创建 TASK-019 卡片（⚪ 未开始）。下方进行中/未运行为此前历史。
- 2026-10-13 标签位置答疑：静态核对点的 x=100/280/460 与三个标签 Left=55/235/415、Width=90，说明左边加半宽得到相同横向中心。讲解保存至 [坐标与折线 Q2](../questions/wpf-charts.md)（待复习）。本轮未修改源码、构建或运行，三个 DoD 未新增验收，任务保持 🟡 进行中。
- 2026-10-13：按用户“开启 task018”准备两个任务专属骨架文件，App.xaml 切到 TASK-018；坐标转换与温度绘图核心留 TODO。实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0，0 警告、0 错误。仅验证构建，未运行实际窗口、未验收三个 DoD，均不勾选；状态 🟡 进行中，不新增自主掌握证据。首步仅移动占位中间点。下方未开始描述为创建时历史。
- 2026-10-12：TASK-017 三个必需 DoD 与 11 个实际窗口场景通过后，按原计划只创建本卡。状态 ⚪ 未开始，未准备源码、未切换 App.xaml、未构建或验收本任务。

参考：[Microsoft WPF 基本图形说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview)。
