# TASK-021：让顶部和底部按内容摆放，中间表格随窗口变高

> 2026-10-14 整理。先对应窗口中的三块区域，再讲行高和行号。任务已于2026-10-13通关，当前源码已完成布局，不要求改回占位状态。

## 1. 为什么换掉外层纵向StackPanel

TASK-020的表格固定高210。窗口拉高后，多出来的空间不会仅凭这个配置自动交给表格。

这次希望：选择设备和读取按钮在上面；历史标题/表格在中间；清空和提示在下面。窗口增加高度时，主要增大中间历史显示区。

Grid是WPF的布局容器；DataGrid才是显示Reading行和三列数据的表格。两者名字相似，当前作用不同。

## 2. 先认三个实际区域

```text
LayoutGrid（主Grid）
  第0行：SelectionRegion，标签/下拉框/读取按钮
  第1行：HistoryRegion，历史标题/表格
  第2行：ActionsRegion，清空按钮/状态文字
```

这几个Region和LayoutGrid名字是教练取的，标识具体容器。区域里面仍有自己的StackPanel或小Grid，不是三个独立窗口。

## 3. RowDefinitions给主Grid划行

当前主Grid直接放在Window中：

```xml
<Grid x:Name="LayoutGrid" Margin="20">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <!-- 三块区域 -->
</Grid>
```

Grid.RowDefinitions是设置这个Grid的行列表，每个RowDefinition描述一行。第一条对应第0行，第二条第1行，第三条第2行。

Window把可用内容空间交给主Grid，Margin让它与窗口边缘四面留20。行分配发生在这个可用范围里，不把整个Window.Height数值直接等同于表格高度。

## 4. Auto具体看这行放了多少内容

第一行Height=Auto，根据选择标签、下拉框、按钮和间距需要的高度安排。第三行Auto根据清空按钮、提示和间距安排。

Auto不是固定数字。提示变成多行，底部可能需要更多高度；控件字号变大，顶部也可能变化。

因此比较窗口拉高前后，要保持内容相同，例如都使用“读取成功”提示，才能判断多出来的空间分到了哪里。

## 5. 星号行取得这次剩余高度

中间Height="*"在本题只有这一行使用星号时，取得顶部/底部分配后剩下的高度。

```text
窗口可用高度
  减顶部所需高度
  减底部所需高度
  → 余下给中间历史区
```

窗口拉高200，其他内容不变，中间可以多拿200。本题不要求多个星号权重，先理解一个剩余区。

## 6. Grid.Row把区域放进指定行

```xml
<StackPanel x:Name="SelectionRegion" Grid.Row="0">
    <!-- 选择和读取 -->
</StackPanel>
<Grid x:Name="HistoryRegion" Grid.Row="1" Margin="0,16,0,0">
    <!-- 历史标题与表格 -->
</Grid>
<StackPanel x:Name="ActionsRegion" Grid.Row="2">
    <!-- 清空和提示 -->
</StackPanel>
```

Grid.Row写在子区域上，告诉外层Grid把整块区域放在哪行，这种写法是WPF附加属性。编号0表示第一行。

不写时通常在第0行，多个区域可能重叠。Grid不会像StackPanel那样仅按标签先后自动排成三行。

## 7. 外层行号与历史区内部行号分别读

HistoryRegion本身在主Grid第1行，里面又有一个小Grid：

```xml
<Grid.RowDefinitions>
    <RowDefinition Height="Auto" />
    <RowDefinition Height="*" />
</Grid.RowDefinitions>
<TextBlock Grid.Row="0" Text="读取历史" Margin="0,0,0,8" />
<DataGrid x:Name="HistoryDataGrid" Grid.Row="1">
    <!-- 原三列 -->
</DataGrid>
```

这是历史区域内部的两行。标题占所需高度，表格取得该区域内部剩余高度。

外层Grid.Row=1安排“整块历史区域”；内部表格Grid.Row=1安排“标题下面的表格”。编号相同，不表示两个属性由同一个父Grid解释。

## 8. 移除表格Height，才能用它已分到的空间

即使历史区变高，表格若仍写Height="210"，它自身还要求固定高度。

当前HistoryDataGrid已经没有这项：

```xml
<DataGrid x:Name="HistoryDataGrid" Grid.Row="1"
          ItemsSource="{Binding Readings, Mode=OneWay}"
          AutoGenerateColumns="False" IsReadOnly="True"
          CanUserAddRows="False" CanUserDeleteRows="False">
    <!-- DeviceId / Value / Unit三列 -->
</DataGrid>
```

保留原伸展对齐，让它使用所在格子给出的高度。只删固定Height但外层没分好剩余区，仍不能保证增长；只改外层星号但表格固定，也不能达到目标。

## 9. 为什么主Grid不再包进外层纵向StackPanel

主Grid需要取得窗口有限的可用高度，才能说“分完其他行之后还有多少”。当前它直接作为窗口内容。

若再用一个纵向StackPanel包住主Grid，父布局会沿竖向按内容需要来测量，可能改变它得到高度的方式，星号行不能按原目的分窗口剩余高度。

选择区和底部区内仍可用StackPanel排列自己的小控件，因为那两行本来就是Auto。容器应对着它所在那一层判断，不把所有StackPanel都删掉。

## 10. 三块区域仍使用同一份数据

后台仍是：

```csharp
DeviceComboBox.ItemsSource = _deviceIds;
DataContext = _viewModel;
```

_deviceIds是001/002两个字符串候选项；_viewModel是本窗口新建的原ReadingViewModel。

三个区域没有另设DataContext，所以原选择DeviceId、ReadCommand、Readings、ClearCommand、StatusText都从同一对象取得。换容器不需要复制读数集合。

## 11. 布局与业务分别验看

拉高窗口不Add记录，不改当前DeviceId；读取/清空也不因为换Grid而重写。

应分别观察：区域有没有正确位置和伸展，以及改选读取、旧行保留、清空禁用/再读恢复是否正常。表格变高不单独证明业务连接正确。

## 12. 数字行高是观察步骤，当前不再使用

骨架曾用160/Auto/*并把清空放中间，方便先改160→190观察空隙。当前实际已经是Auto/*/Auto、区域0/1/2，表格去掉固定Height。

旧TODO注释仍描述数字占位，不代表当前布局未完成，也不要求为了复习把完成代码退回占位。

## 13. 用实际尺寸理解伸展

历史同一“读取成功”提示下：

| 窗口 | 表格高度 | 说明 |
| :--- | :--- | :--- |
| 640×640 | 369.2267 | 默认 |
| 640×840 | 569.2267 | 增高200，表格也增200 |
| 420×600 | 329.2267 | 最小尺寸可读 |

拉高比较时顶部85.72、底部68.48保持。数值是该次实际字体/窗口测量，不是应写死进RowDefinition的新常数。

## 14. 不增长时，先沿高度传递找原因

```text
Window给主Grid空间
→ 外层中间星号行
→ 历史Grid
→ 内层表格星号行
→ 未固定Height的DataGrid
```

先看哪个环节改成按内容或固定高度。若多个区域挤在上面，先检查外层Grid.Row；若历史标题与表格挤在一起，检查内层行号。

## 文末查阅：文件与历史范围

实际文件：[窗口](../../WpfBae/Exercises/TASK-021-grid-layout/Task021Window.xaml)、[后台](../../WpfBae/Exercises/TASK-021-grid-layout/Task021Window.xaml.cs)、[任务卡](../tasks/TASK-021.md)、[布局问答 Q1](../questions/wpf-layout.md)。

可选小观察是按本题入口运行后只拉高窗口，保持设备和提示不变，比较表格高度，再恢复尺寸；本轮未做新运行。当前入口TASK-025，旧题复跑需明确临时入口并恢复。

已读取 [17场景结果](../tasks/evidence/TASK-021/results.json)，全部passed、无运行异常，含拉高/恢复/最小布局和设备选择/读取/清空。构建、原启动和程序集核对见 [verification](../tasks/evidence/TASK-021/verification.json)、[launch](../tasks/evidence/TASK-021/launch.json)。历史显示见 [默认](../tasks/evidence/TASK-021/default-layout.png)、[拉高](../tasks/evidence/TASK-021/tall-window.png)、[最小](../tasks/evidence/TASK-021/minimum-size.png)。

工具首次漏计文字Margin导致误报，工具修正后通过，不能记学员布局错误。自主范围限主Grid三行高度、区域行号与移除固定表格高度；内层两行/数据连接由骨架提供，完整布局机制不一并升级。

官方查阅：[Microsoft 布局容器](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/panel)。下一篇 [TASK-022](./TASK-022-wpf-dockpanel-layout.md)用靠边与剩余区实现类似界面。
