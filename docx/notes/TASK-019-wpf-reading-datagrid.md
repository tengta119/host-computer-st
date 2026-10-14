# TASK-019：把每条读数显示成表格的一行

> 2026-10-14 整理。先认清整张表、一个列和一条 Reading，再读它们的连接。任务已于 2026-10-13 通关；历史验收放在文末，本轮未运行。

## 1. 先看三列要显示什么

读取 TEMP-001，再读取 TEMP-002，表格应有两行：

| 设备 ID | 温度 | 单位 |
| :--- | :--- | :--- |
| TEMP-001 | 26.5 | °C |
| TEMP-002 | 26.5 | °C |

一条 Reading 占一行，三个属性分别填进三个单元格。HistoryDataGrid 是当前这张表的自定义名字，DataGrid 是 WPF 提供的表格类。

这里不再靠 ToString 把整行拼成一句，也不修改原读数集合的类型。

## 2. Grid 与 DataGrid 是两种东西

DataGrid 用来显示记录、列标题和单元格。Grid 则是安排界面位置的布局容器，后面的 TASK-021 会用它分窗口区域。

这一题外层仍是 StackPanel，历史表格高210。把列表换成表格，解决的是记录显示形式，不自动解决窗口拉高后的空间分配。

## 3. 窗口先创建一份原 ViewModel

```csharp
private readonly ReadingViewModel _viewModel = new ReadingViewModel();

public Task019Window()
{
    InitializeComponent();
    DataContext = _viewModel;
    _viewModel.StatusText = "先完成 TODO 1：把第一列表头改为设备 ID。表格数据连接尚未完成。";
}
```

ReadingViewModel 是 TASK-013 已有的自定义类，new 创建本窗口的一份对象。readonly 限制字段初始化后换引用，不禁止原对象追加记录或修改提示。

DataContext 是 WPF 属性，告诉窗口里的默认 Binding 从这个对象读取公开属性。后台连接由教练提供，原读取/清空命令继续使用这份对象的 Readings。

初始提示中的“尚未完成”是旧教学文案；当前实际绑定已经完成，不能凭这句话把任务退回未完成。

## 4. ItemsSource 让表格知道有哪些 Reading

当前表格：

```xml
<DataGrid x:Name="HistoryDataGrid" Height="210"
          ItemsSource="{Binding Readings, Mode=OneWay}"
          AutoGenerateColumns="False" IsReadOnly="True"
          CanUserAddRows="False" CanUserDeleteRows="False">
    <!-- 三列定义 -->
</DataGrid>
```

左侧 ItemsSource 属于这张表，右侧 Readings 是 _viewModel 的公开集合属性。getter 返回原集合，表格据此知道现在有几条记录。

OneWay 表示集合来源从数据对象交给表格，不通过这项绑定替数据类换集合。空集合时没有数据行，是正常初始状态，不表示列设置失效。

## 5. DataGrid.Columns 给这张表定义列

```xml
<DataGrid.Columns>
    <DataGridTextColumn Header="设备 ID" Binding="{Binding DeviceId}" Width="*" />
    <DataGridTextColumn Header="温度" Binding="{Binding Value}" Width="*" />
    <DataGridTextColumn Header="单位" Binding="{Binding Unit}" Width="*" />
</DataGrid.Columns>
```

DataGrid.Columns 是给表格的 Columns 属性提供列列表的 XAML 写法。三个 DataGridTextColumn 是 WPF 的文字列定义，不是三张表。

每列规定标题、字段来源和列宽。它会用于多行，不需要为每次读数再手动创建三列。

## 6. Header 只给标题，Binding 才读取记录

先看温度列：

```xml
<DataGridTextColumn Header="温度" Binding="{Binding Value}" Width="*" />
```

Header="温度" 让顶部写中文“温度”。Binding Value 则取本行 Reading.Value，当前值为26.5。

中文标题不等于数据属性名。Reading 没有一个名为“温度”的属性，所以不能为了标题中文而把路径改成 Binding 温度。

DeviceId/Value/Unit 是自定义 Reading 的公开属性，拼写要与类一致。可以先用显式写法理解：`Binding="{Binding Path=Value}"`，省去Path=含义相同。

## 7. 整表绑定与列绑定读取的对象不同

```text
整表 ItemsSource → ViewModel.Readings → 原集合
第一行设备列 → 第一条 Reading.DeviceId
第二行设备列 → 第二条 Reading.DeviceId
```

窗口当前输入 DeviceId 也叫这个名字，但列读取的是已保存 Reading 的属性。

用户只改输入为TEMP-EDIT-ONLY，未点击读取时，旧行仍显示各自旧ID。只有新读取创建一条新 Reading，才增加新行。

## 8. 当前列没写 Mode，仍可以正常显示

原源码是 `Binding="{Binding Value}"`，没有显式Mode=OneWay。这次只读表格的实际单元格显示、绑定状态和来源已经验证正确。

说明时可以写OneWay让方向更明确，但不能把省略Mode的合法实现判错，更不能由此推定所有WPF绑定都有同一个默认方向。

本题没有要求编辑数据；Reading属性是get/init，不能据只读显示成功推定支持编辑后的回写。

## 9. AutoGenerateColumns=False 避免再自动添列

我们已经手写三列。AutoGenerateColumns=False 告诉WPF使用这些定义，不再按数据对象属性自动生成额外列。

它不改变Readings，也不创建读数。若发现列数量或标题不符合预期，先看手写Columns和自动生成配置，而不是改读取命令。

## 10. 只读限制作用于用户编辑，不阻止程序 Add/Clear

IsReadOnly=True 不允许用户直接编辑单元格。

CanUserAddRows=False、CanUserDeleteRows=False 不让用户从表格直接增删记录。它们来自骨架，不是本次学员新增实现。

原命令仍可对集合Add/Clear，表格照样更新。只读的是用户操作表格的编辑方式，不是把数据集合冻结。

## 11. 三个星号宽度怎么分表格内部空间

每个 DataGridTextColumn 都写 Width="*"，让列分享表格可用宽度。当前三个相同星号给相同分配权重；具体显示还可能受最小宽度等限制。

这是表格内部列宽，不是把整个窗口划成三列。后面Grid.ColumnDefinitions分的是窗口区域，别把两处星号混成一个对象。

## 12. 新记录怎样成为新行

原读取命令：

```csharp
_viewModel.Readings.Add(new Reading
{
    DeviceId = id,
    Value = 26.5,
    Unit = "°C"
});
_viewModel.StatusText = "读取成功";
```

Add原ObservableCollection后，它发成员新增通知，表格增加一行，再按三列读取这条Reading的属性。重复ID也会创建新Reading，所以不会自动去重。

不用手工给表格加单元格，不重新设ItemsSource，也不新建集合覆盖旧历史。集合通知负责“多了一项”，列Binding负责“这一项每格写什么”。

## 13. 清空、空白和恢复沿用原命令

```text
有效读取：Add → 表格多行 → 清空条件Count>0
空白输入：提示并return → 原表格行保持
清空：Clear原集合 → 表格为空 → 清空禁用
再读取：新行出现 → 清空恢复
```

原输入框TwoWay写回DeviceId，读取命令Trim后校验。空字符串/纯空格不会新增或删除历史。StatusText属性通知更新窗口提示，命令条件通知重查是否允许清空。

## 14. 按屏幕问题找对应代码

| 现象 | 优先看 |
| :--- | :--- |
| 有表头但没有数据行 | 原集合Count、整表ItemsSource |
| 有行但温度格空 | Reading.Value、该列Binding路径 |
| 改输入使旧行ID一起变化 | 列是否误读取窗口当前输入 |
| 多出自动列 | AutoGenerateColumns与手写列定义 |
| 点击读取后旧行消失 | 是否替换集合或清空后再Add |
| 用户不能编辑 | 本题IsReadOnly是预期配置 |

## 文末查阅：文件与历史范围

实际文件：[窗口](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml)、[后台](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml.cs)、[原ViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[Reading](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)、[任务卡](../tasks/TASK-019.md)、[表格问答](../questions/wpf-datagrid.md)。

当前入口为TASK-025，旧题复跑需明确临时StartupUri，观察后恢复。本轮没有改入口、源码或运行。可选小观察是只改第一列Header，重跑本题看标题改变而原行数据仍来自DeviceId，再恢复。

已读取 [历史结果](../tasks/evidence/TASK-019/results.json)：11个场景passed，fatalErrors为空，同集合保持，三列正确、只读限制有效，换ID/只改输入/空白保护/清空恢复通过。原2026-10-13构建及启动记录见 [核对](../tasks/evidence/TASK-019/verification.json)、[启动](../tasks/evidence/TASK-019/launch.json)。图像见 [三条历史](../tasks/evidence/TASK-019/history-three-readings.png)、[最小尺寸](../tasks/evidence/TASK-019/minimum-size.png)。这些是历史实际窗口内容，不是本轮新运行。

自主掌握限骨架中的Header、ItemsSource与逐行字段Binding；只读配置、后台和原命令由骨架提供。不扩大为编辑、排序、分页或完整表格框架机制。

官方查阅：[Microsoft DataGrid](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid)。下一篇 [TASK-020](./TASK-020-wpf-device-combobox.md)把自由输入换成选设备。
