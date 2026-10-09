# WPF 窗口、控件与布局

> [返回问答索引](./README.md)

## Q1：StackPanel 和 Grid 是什么？刚开始 WPF 应该怎样读这段 XAML？

- 日期：2026-10-09
- 状态：待复习
- 来源：TASK-009 启动后，学员明确反馈是 WPF 初学者，尚不理解 StackPanel/Grid；此前讲解直接要求调整布局，缺少必要入门说明。Java 后端经验不作为桌面界面经验的证据。

### 现象与原因

任务窗口中已经出现 StackPanel、TextBox 和 Button，但尚未解释各自作用，直接要求“调整布局”无法形成明确的实践动作。先理解窗口、控件和容器，再通过一个属性变化观察界面。

### 机制与示例

WPF 用于创建 Windows 桌面界面。Window 是窗口；TextBlock 显示文字，TextBox 接收输入，Button 是按钮。StackPanel 和 Grid 是布局容器：容纳子控件，并计算它们的位置和大小。

XAML 是描述界面的标记语言。`<Button Content="读取温度" />` 表示一个按钮对象，Content 属性设置显示内容。开始标签与结束标签之间的元素是放在此容器里的子元素：

```xml
<StackPanel>
    <TextBlock Text="设备 ID" />
    <TextBox Text="TEMP-001" />
    <Button Content="读取温度" />
</StackPanel>
```

StackPanel 默认沿垂直方向按书写顺序排列，因此这三个控件从上到下出现。`Orientation="Horizontal"` 改为从左到右排列。它只沿一个方向排列，不自动在窗口边缘换行。

Grid 把空间划分成行和列，再把控件放进指定格子，可类比表格的行列划分，但它负责摆放控件，不是显示业务数据的表格控件。下面定义两行，Auto 表示按内容需要分配行高，Grid.Row 指定控件所在行，编号从 0 开始：

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

`Grid.RowDefinitions` 是设置 Grid 行定义的写法，不是另一个可见控件。不定义行列、不指定位置就把多个控件放入 Grid，它们默认位于同一个格子，可能重叠；Grid 不像 StackPanel 那样自动按顺序排列。列的定义用 Grid.ColumnDefinitions，控件用 Grid.Column 指定列。

当前练习已使用 StackPanel，先沿用即可。Margin 是控件与外部之间的间距；一个数值表示四边相同，四个数按“左、上、右、下”解释。Padding 是控件内容与自身边缘之间的间距。`x:Name` 提供 C# 可访问的控件名字，Click 关联按钮事件处理器，后续实践再展开。

### 本次首个实践动作

打开 `WpfBae/Exercises/TASK-009-window-events/Task009Window.xaml`，先原样运行。找到 TextBox 的 `Margin="0,8,0,12"`，只将最后的 12 改成 24，关闭旧窗口再运行，观察输入框与下方按钮之间的间距增加。再修改 Window 的 Title，观察标题栏。暂不要求改成 Grid；掌握当前排列和间距后继续处理点击事件。

### 复查依据

后续依据学员实际布局修改和窗口运行现象核对是否理解。当前只有答疑，无自主应用证据，不标已掌握，不勾选 TASK-009 DoD。
