# TASK-022：上面选设备，下面放操作，中间表格占剩余空间

> 2026-10-14 整理。根据当前已完成的 XAML、任务卡与历史运行材料解释。先讲三个实际区域怎样摆放，再讲曾经出现的表格不长高问题。历史验收与掌握范围在文末。

## 1. 这次窗口要解决什么问题

窗口仍然有下拉框、“读取温度”、“清空历史”、提示文字和历史表格。读取与清空继续用 TASK-013 的同一份数据和命令；TASK-022 改的是这些控件在窗口中的位置。

希望看到的是：选择设备和读取按钮在上面，清空按钮和提示在下面，历史表格留在中间。把窗口拉高时，增加的高度主要给中间表格，上下两块只使用放下自己内容所需的高度。

TASK-021 用 Grid 的三行实现这种安排。这次用 WPF 的 DockPanel 容器：先让一块内容靠上，再让另一块靠下，最后把剩下的区域交给历史。

## 2. 先认清三个区域，名字不是 WPF API

当前源码的包含关系可以这样读：

```text
Window 窗口
└─ LayoutDockPanel：主 DockPanel
   ├─ SelectionRegion：下拉框、读取按钮所在的 StackPanel
   ├─ ActionsRegion：清空按钮、提示所在的 StackPanel
   └─ HistoryRegion：历史标题、DataGrid 所在的 Grid
```

DockPanel、StackPanel、Grid、Button、DataGrid 都是 WPF 提供的控件或容器。LayoutDockPanel、SelectionRegion、ActionsRegion、HistoryRegion 是项目自行起的名字，便于找到具体对象。

“直接子元素”在这里就是上图主 DockPanel 下面紧挨着的三块。DataGrid 放在 HistoryRegion 里面，它属于 HistoryRegion，不是主 DockPanel 的直接子元素。

这一区别影响最后填满的规则：主 DockPanel 直接安排整块 HistoryRegion，再由 HistoryRegion 安排里面的标题和表格。

## 3. 主 DockPanel 先从窗口取得空间

```xml
<DockPanel x:Name="LayoutDockPanel"
           Margin="20" LastChildFill="True">
```

这段代码创建主容器，并给它取名 LayoutDockPanel。它直接放在 Window 中，使用窗口内容区；Margin="20" 在它的四周各留下 20 个 WPF 长度单位的空隙。

窗口的 Width="640"、Height="640" 包括窗口自身占用的部分，不能把它们直接当成表格尺寸。窗口边框、标题栏、主容器 Margin 和上下区域都会占空间。

LastChildFill="True" 暂时先记成一条具体规则：主 DockPanel 里按 XAML 顺序写在最后的那块内容，要使用前面区域安排后剩下的空间。本窗口最后那块是 HistoryRegion。

## 4. 第一步：选择区靠上

```xml
<StackPanel x:Name="SelectionRegion" DockPanel.Dock="Top">
```

SelectionRegion 是装着设备标签、下拉框和读取按钮的那一整块。DockPanel.Dock="Top" 告诉外面的主 DockPanel：先把这一整块放在当前可用区域的上边。

DockPanel.Dock 是 WPF 的附加属性：代码写在 SelectionRegion 上，但由外面的 DockPanel 读取，用来决定怎么安排 SelectionRegion。它不是让下拉框自己飞到窗口顶部，也不是改变下拉框的数据。

里面仍用竖直 StackPanel，所以标签、下拉框、读取按钮依次往下排。外面的 DockPanel 管整块的位置，里面的 StackPanel 管三个控件的顺序。

## 5. 第二步：操作区靠下

```xml
<StackPanel x:Name="ActionsRegion" DockPanel.Dock="Bottom">
```

主 DockPanel 先安置上面的 SelectionRegion，再从剩余区域的底边安置 ActionsRegion。清空按钮和提示一起靠下，中间仍留下可用空间。

ActionsRegion 在 XAML 中写在第二个，并不意味着它必须显示在第二行。DockPanel.Dock="Bottom" 决定了它靠底边；XAML 顺序决定主容器处理各块的先后。

提示设置了 TextWrapping="Wrap"。提示变长、需要多行时，下面这块会需要更多高度，中间表格可用的高度就会相应减少。所以比较拉高窗口前后的表格高度时，要尽量保持同样的提示内容。

## 6. 第三步：历史区写在最后，使用剩余区域

当前三块在主容器中的顺序是：

```xml
<DockPanel LastChildFill="True">
    <StackPanel DockPanel.Dock="Top">…选择与读取…</StackPanel>
    <StackPanel DockPanel.Dock="Bottom">…清空与提示…</StackPanel>
    <Grid>…历史标题与表格…</Grid>
</DockPanel>
```

这是说明顺序的简化片段，省略号不能直接粘贴运行。第一块取上面的高度，第二块取下面的高度，最后这个 Grid 使用剩下的中间区域。

“最后”说的是它在 DockPanel 的直接子元素列表中写在最后，不是说它出现在屏幕最下面。当前屏幕最下面的是 ActionsRegion，中间的是 HistoryRegion。

当 LastChildFill 为 True 时，最后这一块按照填满规则安排；即使给它写 DockPanel.Dock，也不能用那个值推翻最后填满的规则。当前 HistoryRegion 没有另外写 Dock 属性。

## 7. 为什么只打开 LastChildFill 还不一定对

假设把三块按“选择区、历史区、操作区”写在 DockPanel 中，再打开 LastChildFill。此时最后直接子元素是 ActionsRegion，填满剩余区域的会是清空与提示所在的整块。

要让历史区取得中间剩余空间，必须同时核对两件具体事实：LastChildFill 是 True；HistoryRegion 是主 DockPanel 最后一个直接子元素。

这就是为什么不能只背“启用填满”。先找哪块在最后，再看它是不是你想让它伸展的历史区域。

## 8. 历史区内部再分标题与表格

```xml
<Grid x:Name="HistoryRegion" Margin="0,16,0,0">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row="0" Text="读取历史" Margin="0,0,0,8" />
    <!-- DataGrid 放在第 1 行 -->
</Grid>
```

HistoryRegion 上边留出 16 单位空隙，与选择区分开。它内部第 0 行放标题，Auto 表示这一行按标题需要的高度安排；标题下面的 Margin 还包含 8 单位间距。

第 1 行的 * 表示：标题安排好后，历史区剩余高度给这一行。DataGrid 写 Grid.Row="1"，进入这块剩余区域。

这里有两步分配：主 DockPanel 把中间空间交给 HistoryRegion；HistoryRegion 再把标题以外的空间交给 DataGrid。缺少其中一步，表格就可能没有得到预期的高度。

## 9. 表格不写 Height="210"，才会跟着区域长高

当前表格开头是：

```xml
<DataGrid x:Name="HistoryDataGrid" Grid.Row="1"
          AutoGenerateColumns="False" IsReadOnly="True"
          ItemsSource="{Binding Readings, Mode=OneWay}"
          CanUserAddRows="False" CanUserDeleteRows="False">
```

这里没有固定 Height。历史区得到更多高度时，表格可以使用所在第 1 行的空间。

如果仍保留 Height="210"，外面区域即使变高，表格也仍要求 210 的高度，下面可能出现空白。删掉固定高度，是让表格使用外面已经分给它的空间；不会改变它绑定的 Readings。

AutoGenerateColumns=False 和 IsReadOnly=True 等设置继续保留，三个手写列仍读每条 Reading 的 DeviceId、Value、Unit。

## 10. 多套一层竖直 StackPanel，为什么会坏掉

曾经实际出现过这一结构：

```text
主 DockPanel
├─ SelectionRegion
├─ ShowHistory：额外的竖直 StackPanel
│  └─ HistoryRegion
└─ ActionsRegion
```

这里主 DockPanel 直接看到的是 ShowHistory，不是里面的 HistoryRegion。再加上当时 LastChildFill=False，历史区没有按最终规则得到中间剩余高度。

额外的竖直 StackPanel 按子内容所需高度往下排列，沿竖直方向测量时不会像当前 Grid 的 * 行那样把有限剩余高度明确交给表格。把窗口拉高，并不会自动让这层包裹里的表格长高。

修复后的源码去掉 ShowHistory 这一层，让 HistoryRegion 直接成为主 DockPanel 最后那块，并启用 LastChildFill。HistoryRegion 内部的 Auto/* 两行与表格的无固定高度一起发挥作用。

这不是“所有 StackPanel 都不能用”。当前选择区和操作区仍适合用 StackPanel 排列少量内容；需要随窗口伸展的是历史区。

## 11. 数据为什么没有随着位置改变而丢失

后台仍创建原 ReadingViewModel，把它交给窗口 DataContext，并给下拉框放入 TEMP-001、TEMP-002 两个字符串。

SelectedItem 的绑定继续把当前设备写入 DeviceId；ReadCommand 继续往原 Readings 集合追加 Reading；表格从这个集合显示行；ClearCommand 清空同一个集合。

把表格从一个布局位置移到另一个位置，并不需要创建新集合。窗口 DataContext 仍能传到这些普通子控件，原绑定路径继续使用。

所以布局和业务要分别观察：表格是否长高是一件事；选 TEMP-002 后新行是否保存 TEMP-002，是另一件事。读数正确不等于布局正确。

## 12. 排查时按空间传递顺序看

如果表格不随窗口长高，先看主 DockPanel 是否直接位于 Window 内容区；然后看 LastChildFill 和三个直接子元素的顺序；接着看 HistoryRegion 中 Auto/* 两行和 DataGrid 的 Grid.Row；最后看表格是否还留着固定 Height。

当前后台初始提示和源码注释还写着曾经的首个 TODO、False 占位以及删除固定高度的要求。它们记录的是原实践安排。判断当前实现时，要看实际属性值和包含关系：现在已经是 True、历史区最后、表格没有固定 Height。

如以后专门复跑这一题，可只拉高窗口 200 单位，保持同一句提示，比较表格底部是否下移、上下区域是否仍贴相应边。当前入口是 TASK-025，本轮没有切换入口或重新运行 TASK-022。

## 文末查阅：真实文件、历史结果与掌握范围

实际文件：[窗口布局](../../WpfBae/Exercises/TASK-022-dockpanel-layout/Task022Window.xaml)、[后台连接](../../WpfBae/Exercises/TASK-022-dockpanel-layout/Task022Window.xaml.cs)、[任务卡](../tasks/TASK-022.md)、[布局问答](../questions/wpf-layout.md)。

历史首次检查中，13 项业务场景通过、4 项布局场景未通过，表格高度在窗口拉高前后都是约 43.48。学员修正容器结构、顺序和填满设置后，[最终 17 项场景](../tasks/evidence/TASK-022/results.json) 全部通过。默认 640×640 时表格高度约 369.23，拉到 640×840 后约 569.23；最小 420×600 时约 329.23。数字属于当时的实际窗口与提示，不能当作任意文字和系统环境下的固定值。

本题已通关。布局问答 Q2 的掌握范围是这份窗口中实际应用的 DockPanel 上下靠边、最后直接子区域填满、去除额外包裹与固定高度；不据此扩展为所有 DockPanel 测量规则均已掌握。原绑定和业务骨架继续沿用。

官方查阅：[DockPanel 最后子元素填满规则](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.dockpanel.lastchildfill?view=windowsdesktop-10.0)、[WPF 布局容器](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/panel)。本轮只整理笔记，没有产生新验收结论。
