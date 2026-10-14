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

## Q2：TASK-022怎样让选择区靠顶部、清空区靠底部、历史区占中间？

- 日期：2026-10-13
- 状态：已掌握
- 掌握范围：限 TASK-022 骨架内 Dock=Top/Bottom、直接子区域顺序/LastChildFill、移除历史外层容器与表格固定高度的自主应用并经运行验证；完整测量机制、左右列布局与独立界面设计不一并认定。
- 来源：TASK-022 启动所需的 DockPanel 入门；原 Q1 的已掌握范围保留，不把 Grid 实践推定为已理解 DockPanel。

看 [Task022Window.xaml](../../WpfBae/Exercises/TASK-022-dockpanel-layout/Task022Window.xaml) 中的三个区域：SelectionRegion 装着选择设备文字、下拉框和读取按钮；ActionsRegion 装着清空按钮和状态文字；HistoryRegion 装着读取历史标题和三列表格。这些名字由教练起，用来指出具体区域，不是 WPF API。

当前骨架把整块选择区放到底部，是为了提供一个清楚的修改前现象。目标是让它到顶部、清空区到底部，让历史区取得中间空间。DockPanel 是 WPF 提供的摆放容器，这里直接放在 Window 中，得到窗口内容区的有限空间。它按 XAML 中直接子区域的先后顺序，从尚未分配的空间边缘安排区域。

### 先看选择区的一行代码

```xml
<StackPanel x:Name="SelectionRegion" DockPanel.Dock="Bottom">
```

这行创建的是装着下拉框和读取按钮的那个 StackPanel；其中控件仍按原规则从上到下排列。`DockPanel.Dock="Bottom"` 写在这个 StackPanel 上，是告诉外面的主 DockPanel：把**整块选择区域**靠到剩余空间的底边。不会单独移动某个按钮，也没有修改设备数据。

把这一项改成 `DockPanel.Dock="Top"` 后，同一整块区域改靠顶边。因为 SelectionRegion 在主容器中最先出现，它会先占用顶部所需高度，后面的区域再使用剩下的空间。这是 WPF 的 XAML 属性写法，不是 C# 方法；`DockPanel.Dock` 这种写在子区域上、供外层容器读取的设置称为附加属性。先对应当前区域和效果即可。

首步只改这一项：先运行未改骨架，观察选择区在底部；关闭后将 Bottom 改成 Top，再以相同窗口尺寸运行。预期下拉框与读取按钮一起移到顶部。文件里其他 TODO 暂保留；此时整体布局仍未完成。

### 然后看清空区

```xml
<StackPanel x:Name="ActionsRegion" DockPanel.Dock="Bottom">
```

这段是 TODO 2 的目标写法，当前骨架这里暂为 Top。改为 Bottom 后，主容器把**清空按钮和状态文字所在整块区域**放到当时剩余空间的底边，按按钮、文字和间距所需高度安排。相比 TASK-021 的 Grid，这里没有先定义三行再写行号，而是在处理各区域时逐步留下空间。位置设置的规则见 [Microsoft DockPanel.Dock 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.dockpanel.dock)。

### 最后给历史区剩余空间

主 DockPanel 目前写着 `LastChildFill="False"`，三个区域都按各自 Dock 设置靠边。TODO 3 需要先把 HistoryRegion 从 `<Grid ...>` 到对应 `</Grid>` 的**完整区域**移到 ActionsRegion 后面，使它成为主 DockPanel 的最后一个直接子区域；再把主容器的 LastChildFill 改为 True。

True 表示：前面的区域安排完后，把余下空间交给最后这个区域。这里需要最后是 HistoryRegion，历史区才取得中间空间。最后说的是主 DockPanel 里面直接放入的三个区域，不是内部最后一个按钮或表格列。若最后仍是 ActionsRegion，得到余下空间的就会是清空区。

此时历史区上的 Dock=Top 不决定它只靠顶部：最后子区域的填满规则优先生效。因此单改历史区的 Dock，不能代替正确的顺序和 LastChildFill 设置。规则与默认 True 见 [Microsoft LastChildFill 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.dockpanel.lastchildfill?view=windowsdesktop-10.0)。骨架显式设 False 是观察占位，需学员完成连接。

### 历史区变高之后，表格还需要跟上

HistoryRegion 内的小 Grid 由教练沿用 TASK-021 提供：第 0 行按标题所需高度安排，第 1 行取得内部剩余高度。表格的 Grid.Row=1 只属于这个小 Grid。移动整块区域时保留这两行及内部行号，不把它们改成外层 Dock 设置。

HistoryDataGrid 当前还有 `Height="210"`，仍要求表格自身保持固定高度。TODO 4 移除这一项，让它按原伸展方式使用内层第 1 行空间。只有外层历史区得到剩余空间、表格自身也不固定高度，窗口拉高时表格才可以随之变高。不要把整个主 DockPanel 再包进纵向 StackPanel，以免改变它获得窗口高度的方式。

后台的 ReadingViewModel 是 TASK-013 已有的自定义数据类，`new ReadingViewModel()` 为本窗口创建一份对象，`_viewModel` 指向它。后台把窗口的 WPF 属性 DataContext 设为这个对象，三个区域继续通过原 Binding 读它的设备 ID、历史集合、命令和提示。布局位置变化不需要在区域里再创建数据对象，保留全部业务连接。

在工作区根目录运行：

```powershell
dotnet run --project WpfBae/WpfBae.csproj
```

确认实际窗口标题为 TASK-022。先完成首步位置观察，再逐个做其余 TODO；最后实际拉高、恢复、缩到最小尺寸并读取/清空复核。上面的界面变化是预期效果；本轮仅构建，未用设计预览或编译结果冒充运行验证。

### 本次审查：加了 ShowHistory，为什么表格仍不变高？

2026-10-13 当前学员文件把 HistoryRegion 放进新增的 `ShowHistory` StackPanel。实际运行中窗口从 640 拉高到 840，表格仍是 43.48 高，中间增加空白；读取和清空业务正常。

主 DockPanel 现在直接看到的是 SelectionRegion、ShowHistory、ActionsRegion 三块。HistoryRegion 是 ShowHistory 的内部内容，因此主容器不会把它当作自己的直接子区域安排。`ShowHistory` 是学员给具体 StackPanel 起的名字，名字本身不会赋予它特殊布局行为。

这个纵向 StackPanel 会按内部内容需要来测量高度；历史小 Grid 没有因此取得窗口剩余高度，星号行也没有得到可以随窗口增长的高度。原来那块 Grid 已经能包含标题和表格，此处再加纵向 StackPanel 会改变高度传递。容器高度约束差异见 [Microsoft Panel 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/panel)。

先只移除 `<StackPanel x:Name="ShowHistory" ...>` 和它对应的 `</StackPanel>` 两个外层标签，里面的完整 HistoryRegion Grid 保留。这样主 DockPanel 才直接安排历史 Grid。这个小步骤只修复对象之间的外层关系，仍需完成后面的填满连接。

随后检查主 DockPanel 的三个直接子区域：选择区先放、清空区其次、历史 Grid 最后。确认顺序后启用主容器的 LastChildFill；如果最后仍是清空区，就会把剩余空间给清空区。保留已经移除固定 Height 的表格，再实际运行拉高/恢复窗口核对。

本次已确认顶部/底部设置正确、固定表格高度已去除、业务场景正常；这些已完成部分无需重写。原 Q1 掌握范围保留，Q2 因填满布局尚未自主验证完成，保持待复习。

### 追问：“最后一个直接子区域，再启用 LastChildFill”具体是什么意思？

2026-10-13 学员明确追问这句话的含义。此前教练把嵌套关系、代码顺序、属性修改三个动作压在一句里，未展开当前标签对应的具体对象；这属于讲解需改进，不新增学员错题，不撤销已有实践证据。

先只看当前文件里的包含关系，省略各区域内部控件：

```text
LayoutDockPanel
├─ SelectionRegion（选择区）
├─ ShowHistory（外层 StackPanel）
│  └─ HistoryRegion（历史 Grid）
└─ ActionsRegion（清空区）
```

SelectionRegion、ShowHistory、ActionsRegion 是直接写在主 DockPanel 标签里面的三个区域，中间没有另一层容器，称为它的直接子区域。HistoryRegion 写在 ShowHistory 标签里面，外面隔着这一层 StackPanel，因此当前不是主 DockPanel 的直接子区域。首步去掉 ShowHistory 的外层两标签、保留内部 Grid，就是去掉这层间隔。

接着再看“最后”：这里指 XAML 文件中的先后顺序。把整个 HistoryRegion 从 `<Grid x:Name="HistoryRegion" ...>` 到对应 `</Grid>`（标题和表格一并保留）移动到 ActionsRegion 的 `</StackPanel>` 后面、主容器的 `</DockPanel>` 前面。它就成为这个 DockPanel 最后写入的区域。不是把历史表格显示到窗口最底部；清空区仍按它自己的 Dock=Bottom 靠底边。

顺序变成“选择区、清空区、历史区”后，再修改主容器原来那行的 `LastChildFill="False"` 为 `LastChildFill="True"`。LastChildFill 是 WPF 给 DockPanel 提供的属性；True 表示启用“把余下空间交给最后区域”的规则。这里前面的选择区取顶部所需高度、清空区取底部所需高度，最后历史 Grid 取得中间剩余空间。属性不是加在 DataGrid 或 HistoryRegion 上。

结构示意如下，内部内容用注释省略，只用于读顺序，不能整段覆盖原文件：

```xml
<DockPanel x:Name="LayoutDockPanel" Margin="20" LastChildFill="True">
    <StackPanel x:Name="SelectionRegion" DockPanel.Dock="Top">
        <!-- 保留原选择区内容 -->
    </StackPanel>
    <StackPanel x:Name="ActionsRegion" DockPanel.Dock="Bottom">
        <!-- 保留原清空区内容 -->
    </StackPanel>
    <Grid x:Name="HistoryRegion" Margin="0,16,0,0">
        <!-- 保留原行定义、标题和完整表格 -->
    </Grid>
</DockPanel>
```

当前先做第一步：仅去掉 ShowHistory 的外层两标签，不删除历史 Grid。逐步完成顺序和属性修改后，以 `dotnet run --project WpfBae/WpfBae.csproj` 实际运行，再拉高窗口；预期历史表格增长。本轮只解释和同步记录，未改学员源码或重新构建/运行验收；TASK-022/DoD/Q2 状态保留。

### 追问：历史 Grid 写在最后，为什么显示在中间？

2026-10-13 学员继续询问历史 Grid 的代码位置与画面位置之间的关系。已重读当前源码：ShowHistory 已移除，三个直接子区域现在依次为 SelectionRegion、ActionsRegion、HistoryRegion，LastChildFill=True。以下解释针对本次新源码；此前 False/外层 StackPanel 是首次审查时的历史状态，不再作为当前源码描述。

DockPanel 按代码顺序处理这三块，但每块最终放在哪里，要看其 Dock 设置以及最后区域的填满规则。代码最后出现，表示最后分配空间，不表示排在窗口下方。

1. 先处理 SelectionRegion。它写 DockPanel.Dock=Top，主容器把它放在顶部，并留出整块选择区需要的高度；尚未分配的空间在其下方。
2. 再处理 ActionsRegion。它写 DockPanel.Dock=Bottom，主容器把它放在尚未分配空间的底边，清空按钮和提示取得所需高度；余下空间现在夹在选择区和清空区之间。
3. 最后处理 HistoryRegion。主容器已设置 LastChildFill=True，表示把余下空间交给最后的直接子区域，因此历史 Grid 得到中间这块空间。这里最后是分配顺序，居中的是剩余空间的实际位置；不是通过名字识别历史或自动设置居中对齐。

```text
┌────────────────────────┐
│ 选择区：先取顶部        │
├────────────────────────┤
│                        │
│ 历史区：最后取余下空间  │
│                        │
├────────────────────────┤
│ 清空区：其次取底部      │
└────────────────────────┘
```

规则依据：[Microsoft DockPanel.Dock](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.dockpanel.dock)、[LastChildFill](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.dockpanel.lastchildfill?view=windowsdesktop-10.0)。最后区域填满余下空间，不需要额外写 Dock=Center；WPF 的 Dock 没有 Center 选项。

学员贴出的 Grid.RowDefinitions 和内部 Grid.Row=0/1，只负责在已得到中间空间的历史区里面，分别放标题和表格。决定整块历史区在主窗口哪里的是外层 DockPanel；决定标题/表格如何在历史区里面排列的是这段小 Grid。

本轮只解释和同步记录，未修改学员代码、构建或重新运行验收；新源码存在不直接视为 DoD 通过。TASK-022 保持进行中，Q2 待复习，已有业务与 Q1 应用证据保留。

## 历史证据与掌握范围

2026-10-13 TASK-022 最终复查：学员自主修改后的17个实际场景全通过，窗口640→840时表格369.2267→569.2267，上下区85.72/68.48保持；恢复及最小420×600可读，13个原业务场景正常。50个源码哈希未变，引用原编译DLL；见[最新结果](../tasks/evidence/TASK-022/results.json)与[核对](../tasks/evidence/TASK-022/verification.json)。Q2仅按上述骨架内应用范围标已掌握，不计骨架后台/内部Grid为新的自主设计，完整框架机制仍复习。下方Q2待复习/失败/未验收描述是对应时点历史，首次资料已单独保存。

2026-10-13 TASK-022 首次审查：17 个实际场景、13 个业务通过、4 个布局未通过；DoD 3 已通过，1/2 未通过。主容器 LastChildFill=False，历史被 ShowHistory 包裹且不在主容器最后；窗口 640→840 时表格均 43.48。50 个源码哈希未变，同一编译 DLL 已核对；见[首次 TASK-022 结果](../tasks/evidence/TASK-022/first-review/results.json)和[核对](../tasks/evidence/TASK-022/first-review/verification.json)。未代改源码，不扩展已掌握范围。

2026-10-13 TASK-022 启动：独立窗口两个骨架文件与入口已准备，构建 0 警告/错误。DockPanel 摆放核心留 TODO，未运行实际窗口或验收；Q2 待复习，不改变 Q1 的已掌握应用范围。

2026-10-09 时 Q1 待复习：学员明确尚未理解 StackPanel/Grid，之前直接要求“调整布局”是教练漏讲；TASK-009 的按钮读取与输入保护后来已验证通关，但容器与布局机制不因此全部掌握。原教学反馈保留。

2026-10-13 TASK-021 已准备两个骨架文件并切换入口，实际构建 0 警告/错误；本轮未运行窗口或验收。三行容器、区域内小 Grid 和业务连接由教练提供，讲解与源码存在不作为学员自主掌握证据。上述界面变化是待学员实际观察的预期，不记为已经验证的结果。

2026-10-13 TASK-021 最终审查：学员配置三行 Auto / * / Auto、三个区域 0/1/2，并移除表格固定高度。实际原窗口与同一编译程序集显示/业务 17 个场景通过；窗口拉高 200 时表格高 369.2267→569.2267，上下行高保持 85.72/68.48，最小 420×600 内容可读。按这几处自主应用及运行证据标已掌握，不把区域内小 Grid、既有 Binding/命令连接、完整框架测量或独立界面设计计入自主范围。首次审查工具漏计 Margin 的误报已修正，不作为学员错误。

相关任务卡与验收来源：[TASK-009](../tasks/TASK-009.md)、[TASK-021](../tasks/TASK-021.md)，本次证据见[运行结果](../tasks/evidence/TASK-021/results.json)与[核对](../tasks/evidence/TASK-021/verification.json)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-layout.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。
