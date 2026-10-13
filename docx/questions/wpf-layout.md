# WPF 窗口、控件与布局

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：StackPanel 和 Grid 是什么？刚开始 WPF 应该怎样读这段 XAML？

- 日期：2026-10-09
- 状态：已掌握
- 掌握范围：限 TASK-021 骨架内三行高度、区域行号、移除表格固定高度的应用；完整布局机制、列布局、DockPanel 和独立 StackPanel/完整界面设计不一并认定掌握。
- 来源：TASK-009 启动后，学员明确反馈是 WPF 初学者，尚不理解 StackPanel/Grid；此前讲解直接要求调整布局，缺少必要入门说明。Java 后端经验不作为桌面界面经验的证据。

先看 TASK-009 中“设备 ID 文字、输入框、读取按钮”这三个具体元素。它们从上到下出现，是因为被放在同一个 StackPanel 内，而不是靠文件里的名字自动找到位置。

```xml
<StackPanel>
    <TextBlock Text="设备 ID" />
    <TextBox Text="TEMP-001" />
    <Button Content="读取温度" />
</StackPanel>
```

XAML 是描述界面的标记语言。这段在窗口内部创建一个 StackPanel，并在其中按顺序放三个控件。它们都由 WPF 提供：TextBlock 显示固定文字，TextBox 是输入框，Button 是按钮。Text/Content 分别给这个文字控件、输入框、按钮指定显示内容。

StackPanel 默认从上往下摆放内部元素；设置 Orientation="Horizontal" 后改成从左往右。它只沿一个方向排，不自动在窗口边缘换行。StackPanel 是负责排列子控件的容器，Window 则是打开的整个窗口。

再看 Grid：它先划分格子，然后放控件，不按书写顺序自动堆起来。

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row="0" Text="设备 ID" />
    <TextBox Grid.Row="1" Text="TEMP-001" />
</Grid>
```

Grid.RowDefinitions 为这个 Grid 定义两行，每个 RowDefinition 描述一行，Auto 按内容需要分配高度。TextBlock 的 Grid.Row=0 放第一行，TextBox 的 1 放第二行，编号从 0 开始。若多个控件都放默认的第 0 行/第 0 列，就可能重叠。Grid 负责摆位置，不是 DataGrid 那种读数表格。

当前 [TASK-009 原文件](../../WpfBae/Exercises/TASK-009-window-events/Task009Window.xaml)使用 StackPanel，无需为了复习改成 Grid。原首步观察是把输入框 Margin 的 `0,8,0,12` 改为 `0,8,0,24`：四个数按左、上、右、下解释，只增加输入框下方外部空隙。Padding 则是按钮文字与按钮边缘的内部留白，作用位置不同。后来改 Title 是另一步，不能一次堆进首个动作。

历史观察应在实际 TASK-009 窗口核对；当前启动入口是 TASK-021，直接运行项目会打开当前任务，设计预览也不能代替原窗口运行证据。这里保留单处观察方法，不要求重做已完成任务。

### TASK-021：窗口拉高后，怎样让历史表格变高？

2026-10-13 启动时增补，仍属 Q1，当时状态待复习；同日 TASK-021 实践审查通过后按上述应用范围标已掌握。这里讲的是 [Task021Window.xaml](../../WpfBae/Exercises/TASK-021-grid-layout/Task021Window.xaml)。已有设备选择/表格/命令连接继续复用；本次学习如何分配它们占用的窗口空间。下述占位、首步和修改预期是启动时说明，当前源码已经完成最终配置，不要求重新做首步。

目标界面分成三块：顶部选择设备和读取按钮，中间读取历史表格，底部清空按钮和状态文字。`Grid` 是 WPF 提供的摆放容器；`DataGrid` 是已经显示设备 ID、温度、单位三列的表格，二者用途不同。`LayoutGrid`、`SelectionRegion`、`HistoryRegion`、`ActionsRegion` 是教练给本窗口具体容器起的名字，不是新 API。

先看怎样给一个 Grid 分两行。这是语法说明，不要求把整段覆盖进当前文件：

```xml
<Grid.RowDefinitions>
    <RowDefinition Height="Auto" />
    <RowDefinition Height="Auto" />
</Grid.RowDefinitions>
```

这段写在某个 Grid 内，给**这个 Grid**设置行列表；每条 `RowDefinition` 描述一行。它们是 WPF 的 XAML 写法，不是 C# 方法。`Height="Auto"` 表示这行根据里面的控件需要分配高度：若行里放选择设备的文字、下拉框和按钮，就按这些内容及其间距需要的高度来安排。它不表示“把整个窗口占满”。

再看 `Height="*"`：在本任务只有一个星号行的情况下，其他行取得所需高度后，剩下的高度交给这一行。最终中间历史区域应取得这部分空间，所以拉高窗口时它有空间变高。固定数值如 `Height="160"` 则给这一行预留 160 个 WPF 界面单位，不随内容需要自动改变。Auto/Star 含义参考 [Microsoft GridUnitType 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.gridunittype?view=windowsdesktop-10.0)。

行分好后，还要告诉 Grid 谁放在哪一行：

```xml
<StackPanel x:Name="SelectionRegion" Grid.Row="0">
```

`SelectionRegion` 是已经装着下拉框和读取按钮的那个 StackPanel。给它写 `Grid.Row="0"`，是告诉它外面的 `LayoutGrid`：把**整块选择区域**放进第 1 行。1 对应第 2 行，2 对应第 3 行，编号从 0 开始；这是 WPF 的行位置设置，[官方 Grid.Row 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.grid.row?view=windowsdesktop-10.0)也说明它指定子内容所在行。Grid 不会按控件在文件中的先后顺序排队；若多个区域都不写行号，它们默认挤在第 0 行，可能重叠。

新骨架暂把选择放第 0 行、清空放第 1 行、历史放第 2 行，方便逐步观察。后续调整的是这三个区域的外层行号，使它们成为顶部选择、中间历史、底部清空。选择区和清空区内继续用原 StackPanel 从上到下排列各自的控件，这不妨碍外层 Grid 分配区域高度。

历史区内还有一个教练准备的小 Grid：第 0 行放“读取历史”标题，第 1 行放表格。小 Grid 的两条 RowDefinition 只给**历史区内部**分行。因此表格上的 `Grid.Row="1"` 表示历史区内部的第 2 行，不是整个窗口的第 2 行；调整外层顺序时改 `HistoryRegion` 自己的行号，先保留内部安排。

为什么还要移除 `HistoryDataGrid` 的 `Height="210"`？行变高表示容器提供了更多空间，但这项仍要求表格自身只取 210 高度。删掉固定 Height 后，表格才能按骨架默认的伸展方式填满分给它的空间。原 `ItemsSource` 和三列字段 Binding 都保留，不需要新建历史集合或手工刷新。

另一个必要条件是主 Grid 得到窗口的可用高度。骨架已把 `LayoutGrid` 直接放在 Window 中；不要再把整个 Grid 包到纵向 StackPanel 里。那种外层排列方式按内容测量高度，不能据此实现本题“拿窗口剩余高度”的目标。DataContext 仍由后台设置为本窗口的一份原 ReadingViewModel，区域换位置不会因此另创建对象。

现在只做首个小改动：找到主 Grid 的 TODO 1，把紧接着的 `<RowDefinition Height="160" />` 改成 190，其他项先保留。先运行未改骨架，再关闭、修改、重新运行，保持同样窗口尺寸；预期选择区下方空隙增大，清空/提示区向下移动。这里改的是第 0 行的预留高度，尚未让表格自动增长，也不代表整个任务完成。

在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`。确认实际窗口标题为 TASK-021；编辑器提示和设计预览不是程序运行证据。先把行高与看到的位置对应起来，再按任务卡处理后续 TODO。

## 历史证据与掌握范围

2026-10-09 时 Q1 待复习：学员明确尚未理解 StackPanel/Grid，之前直接要求“调整布局”是教练漏讲；TASK-009 的按钮读取与输入保护后来已验证通关，但容器与布局机制不因此全部掌握。原教学反馈保留。

2026-10-13 TASK-021 已准备两个骨架文件并切换入口，实际构建 0 警告/错误；本轮未运行窗口或验收。三行容器、区域内小 Grid 和业务连接由教练提供，讲解与源码存在不作为学员自主掌握证据。上述界面变化是待学员实际观察的预期，不记为已经验证的结果。

2026-10-13 TASK-021 最终审查：学员配置三行 Auto / * / Auto、三个区域 0/1/2，并移除表格固定高度。实际原窗口与同一编译程序集显示/业务 17 个场景通过；窗口拉高 200 时表格高 369.2267→569.2267，上下行高保持 85.72/68.48，最小 420×600 内容可读。按这几处自主应用及运行证据标已掌握，不把区域内小 Grid、既有 Binding/命令连接、完整框架测量或独立界面设计计入自主范围。首次审查工具漏计 Margin 的误报已修正，不作为学员错误。

相关任务卡与验收来源：[TASK-009](../tasks/TASK-009.md)、[TASK-021](../tasks/TASK-021.md)，本次证据见[运行结果](../tasks/evidence/TASK-021/results.json)与[核对](../tasks/evidence/TASK-021/verification.json)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-layout.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。
