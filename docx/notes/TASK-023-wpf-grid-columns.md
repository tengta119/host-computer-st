# TASK-023：左边操作，右边表格，窗口变大时空间给谁

> 2026-10-14 整理。围绕当前窗口的两列、左右区域和实际尺寸变化讲解；示例来自已完成代码。历史验收和骨架范围放在文末。

## 1. 先看想要的窗口效果

TASK-021、022 把设备操作放在上面、历史放在中间。TASK-023 改成左右排列：左边选择设备、读取、清空并显示提示；右边显示历史标题和表格。

窗口拉宽时，左边下拉框不必跟着变宽，新增宽度给右边表格；窗口拉高时，表格使用更多高度。读取、清空和设备选择仍然使用原数据与命令。

这题要认识两次空间分配：主 Grid 把窗口分为左右两列；右侧小 Grid 把右列分为标题、表格两行。

## 2. Grid 的“列”先分区域，不是数据表格的列

```xml
<Grid x:Name="LayoutGrid" Margin="20">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
</Grid>
```

Grid 是 WPF 布局容器，负责给控件安排空间；LayoutGrid 是项目给主容器起的名字。ColumnDefinitions 里这两项把主容器分成左、右两列，编号从 0 开始。

这里的列装的是“整块操作区”和“整块历史区”。右侧 DataGrid 自己还有设备 ID、温度、单位三个数据列，它们属于另一层。不要把主界面的两列和表格的三个列标题混在一起。

Margin=20 是主 Grid 四周的外部间距。窗口宽度不是两列宽度直接相加的值，还要扣掉窗口自身和这些间距。

## 3. 左列 Auto：先看左侧内容需要多宽

```xml
<ColumnDefinition Width="Auto" />
```

这句话设置第 0 列的宽度。当前左侧有一个宽 260 的下拉框、宽 260 的提示，以及右边 20 的区域间距，因而左列会按这些内容需要的宽度安排。

Auto 不等于“固定 260”。如果把左侧某个控件改得更宽，左列可能随它变宽。当前左列历史测量为 280，正好对应 260 的内容宽度与右侧 20 的 Margin。

也不必把最早观察用的 Width=320 或 Width=360 当成最终值。它们是先观察右区起点如何移动的练习数值，当前实现已改成 Auto。

## 4. 右列 *：左列安排后，剩余宽度给右边

```xml
<ColumnDefinition Width="*" />
```

这句话设置第 1 列。主 Grid 已有一份有限宽度，先满足左列 Auto，再把剩余宽度给这一列。

本题只有一列用 *，因此右列取得该方向的剩余宽度。它不是“自动无限大”，也不是某个固定像素值。窗口可用宽度增加 200、左侧内容不变时，右列就能增加约 200。

以后若有两个 * 列，才需要进一步讨论它们怎样按权重分配；理解当前 Auto/* 两列不需要先引入那些组合。

## 5. 用 Grid.Column 把整块操作区放入第 0 列

```xml
<StackPanel x:Name="OperationsRegion"
            Grid.Column="0" Margin="0,0,20,0">
```

OperationsRegion 是项目给左侧整块区域起的名字，里面有设备标签、下拉框、读取按钮、清空按钮和提示。Grid.Column=0 告诉它外面的 LayoutGrid：把整块放在第 0 列。

Grid.Column 是 WPF 的附加属性，写在被摆放的区域上，由外面的 Grid 读取。左边这块内部使用竖直 StackPanel，所以几个操作控件还是从上到下排列。

Margin 的四个数依次是左、上、右、下。0,0,20,0 给操作区右边留 20 的间距，隔开右侧表格；不是给下拉框内部增加留白。

## 6. 为什么提示固定宽度 260，同时设置换行

```xml
<TextBlock Text="{Binding StatusText, Mode=OneWay}"
           Width="260" HorizontalAlignment="Left"
           Margin="0,12,0,0" TextWrapping="Wrap" />
```

提示文字从原 ViewModel 的 StatusText 取得。Width=260 让这个 TextBlock 在左栏内保持与下拉框接近的宽度；TextWrapping=Wrap 让较长文字在这个宽度内换行。

这里固定的是左侧提示宽度，目的在于让左栏稳定。如果不限制长提示所需的宽度，Auto 左列可能为了它变得很宽，挤走右侧表格空间。

HorizontalAlignment=Left 让这个有明确宽度的提示靠左。提示换成多行后会更高，但不会因此把右列挤得越来越窄。

这与删除表格固定 Width 并不冲突：左侧小块有稳定宽度，右侧大块使用剩余空间，各自服务于当前界面目的。

## 7. 历史区放进第 1 列，内部再分两行

```xml
<Grid x:Name="HistoryRegion" Grid.Column="1">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row="0" Text="读取历史" Margin="0,0,0,8" />
    <!-- DataGrid 位于 Grid.Row="1" -->
</Grid>
```

HistoryRegion 是右侧那一整块，Grid.Column=1 让它进入主 Grid 的右列。里面这个 Grid 的 RowDefinitions 只分配自己的高度，不会把整个窗口再次切成两行。

第 0 行 Auto 放下历史标题和下面 8 的间距；第 1 行 * 接受余下高度。DataGrid 放在第 1 行，所以窗口拉高时可以长高，同时保留上方标题。

按真实对象读就是：主 Grid 给 HistoryRegion 一块右侧空间，HistoryRegion 再给标题一小行，把余下高度交给表格。

## 8. 表格同时去掉固定 Width 和 Height

```xml
<DataGrid x:Name="HistoryDataGrid" Grid.Row="1"
          AutoGenerateColumns="False" IsReadOnly="True"
          ItemsSource="{Binding Readings, Mode=OneWay}"
          CanUserAddRows="False" CanUserDeleteRows="False">
```

当前没有 Width=300，也没有 Height=210。表格在右侧第 1 行中按默认伸展安排，使用这一行获得的宽度和高度。

只改主 Grid 的右列为 *、却保留表格 Width=300，会出现“右边区域变宽，表格仍只有原来那么宽”。只去掉 Width 但保留 Height=210，则拉高窗口时表格仍可能停在固定高度。

因此要连着检查主列分配、右侧行分配和表格本身尺寸要求。外面愿意给空间，里面还要允许使用。

## 9. 表格三个 Width="*" 分的是表格内部宽度

```xml
<DataGridTextColumn Header="设备 ID" Binding="{Binding DeviceId}" Width="*" />
<DataGridTextColumn Header="温度" Binding="{Binding Value}" Width="*" />
<DataGridTextColumn Header="单位" Binding="{Binding Unit}" Width="*" />
```

DataGridTextColumn 是 WPF 的文本数据列。三列都写 *，让表格内部可分配的列宽按相同权重分配；表格变宽时，列也能随之变宽。

Header 是屏幕上的标题文字，Binding 指明每行 Reading 取哪个属性。设备 ID 列读该行记录的 DeviceId，温度列读 Value，单位列读 Unit。

两层 * 的具体对象不同：主 ColumnDefinition 的 * 给历史区域分空间；DataGridTextColumn 的 * 给表格内部的数据列分空间。前者不会自动替你写出后者。

## 10. 选设备、读数与旧历史继续怎样连接

后台继续建立一份 ReadingViewModel，并把它设为窗口 DataContext；下拉框候选仍是 TEMP-001、TEMP-002 两个字符串。

SelectedItem="{Binding DeviceId}" 继续把选择写进 DeviceId。当前没有显式 Mode，SelectedItem 默认支持双向绑定，详见 TASK-020。读取按钮取原 ReadCommand，清空按钮取原 ClearCommand。

表格 ItemsSource 继续取原 Readings。先读取 TEMP-001、再选 TEMP-002，旧行仍是 TEMP-001；再次读取才追加 TEMP-002 的新行。左右分栏没有改变“读取当时保存一条记录”的业务。

IsReadOnly=True 限制用户直接编辑单元格；CanUserAddRows=False、CanUserDeleteRows=False 限制表格中的用户增删入口。命令仍可通过集合 Add/Clear 改变显示内容。

## 11. 怎样读拉宽与拉高的实际结果

历史结果中，默认窗口 900×640 时，表格约为 565.33×539.43；拉宽到 1100×640 后，约为 765.33×539.43。左列保持 280，表格增加 200 宽，高度基本不变。

另一次从默认尺寸拉高到 900×840，表格约为 565.33×739.43。增加的 200 高度给了表格，宽度基本不变。

这些数值支持的是当前结构确实把增加的空间交给右侧表格。不能用窗口 Height 减一个随手估算的常量代替实际观察，也不必要求不同系统下小数尺寸完全相同。

最小窗口 760×420 时，表格约为 425.33×319.43，左侧内容和三列表格仍可读；历史材料还验证了恢复尺寸后的行为。

## 12. 常见问题按对应对象找原因

右侧起点随长提示大幅右移，先看左侧提示是否仍 Width=260 且允许换行；表格周围空白变大，先看它是否残留固定 Width/Height。

左右区域重叠，检查它们各自的 Grid.Column 是否为 0 和 1；表格没有占剩余高度，检查右侧小 Grid 的 Auto/* 两行以及 DataGrid 的 Grid.Row=1。

当前源码注释还提到 320→360、删除 Width=300/Height=210 的 TODO，后台初始提示也仍描述第一步。这些是实践起点文字。最终属性已是左 Auto、右 *，表格没有固定宽高。

如以后专门观察，可在同一窗口只拉宽，再只拉高，分别看表格变化。当前入口是 TASK-025，本轮没有切换入口、构建或运行本题。

## 文末查阅：真实文件、历史证据与掌握范围

实际文件：[窗口左右分栏](../../WpfBae/Exercises/TASK-023-grid-columns/Task023Window.xaml)、[后台连接](../../WpfBae/Exercises/TASK-023-grid-columns/Task023Window.xaml.cs)、[任务卡](../tasks/TASK-023.md)、[布局问答](../questions/wpf-layout.md)。

[最终 19 项场景](../tasks/evidence/TASK-023/results.json) 包括 13 项原业务场景与 6 项布局场景，均通过。[拉宽后的历史图](../tasks/evidence/TASK-023/wide-window.png) 可以对照左栏与表格。所有尺寸是历史运行结果，本轮没有重新验收。

本题已通关。布局问答 Q3 的已掌握范围对应这份界面中实际应用的 Auto/* 主列与去除表格固定宽高；左右区域初始位置、右侧 Auto/* 行结构来自骨架，不能据此认定所有布局均已自主设计。任务状态和问答范围保持原记录。

官方查阅：[WPF 布局容器](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/panel)、[DataGrid](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid)。
