# WPF 读数表格

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：怎样把读取历史显示为三列？表头和 Binding 分别给谁用？

- 日期：2026-10-13
- 来源：TASK-019 启动的必要讲解。
- 状态：已掌握（限提供的表格骨架中 Header、ItemsSource 与逐行字段绑定应用）

TASK-019 的 HistoryDataGrid 是一张具体表格，用 WPF 的 DataGrid 类创建。读取 TEMP-001、再读取 TEMP-002 后，一条 Reading 占一行，设备/温度/单位分列；不是调用 ToString 把整行拼成一个格子。

先看第一列的现有写法：

```xml
<DataGridTextColumn Header="设备 ID" Binding="{Binding DeviceId}" Width="*" />
```

DataGridTextColumn 是 WPF 文字列。Header 只指定顶部标题“设备 ID”，不读取数据。Binding 指定本列各行的字段：第一行读第一条 Reading.DeviceId，第二行读第二条 Reading.DeviceId。Width 的星号让列分配剩余宽度。

再看表格取整份历史的连接：

```xml
ItemsSource="{Binding Readings, Mode=OneWay}"
```

左边是这张 DataGrid 的 ItemsSource；右边从窗口 DataContext 指定的原 ReadingViewModel 读取 Readings，取得历史集合。这让表格知道“有哪几条 Reading”，上面列 Binding 再说明“这条 Reading 哪个属性放这列”。

温度列 Header 可写中文“温度”，Binding 却要写 Reading.Value；单位列取 Unit。列名和属性名不是同一用途。换当前输入 ID 只改 ViewModel.DeviceId，旧行仍读各自 Reading.DeviceId，所以不会全变成新设备。

AutoGenerateColumns=False 只用手写三列；DataGrid.Columns 保存这张表的列，内部三个列标签不是三张表。IsReadOnly=True 禁止单元格编辑，CanUserAddRows/CanUserDeleteRows=False 禁止用户从表直接增删记录；不妨碍原命令 Add/Clear 集合。

原 ObservableCollection 的 Add 发新增通知，表格增加行，再按列取新 Reading 三个属性；Clear 移除所有项并通知，表格清空，集合仍保留供下次读取。无需给表手工添加行或换集合。

旧首步只把第一列 Header 改为设备 ID，观察标题变化。现在数据连接和三列已经完成，见 [原XAML](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml)，不继续把它称作“表格尚未连接”。列未写 Mode 的现有只读显示已验证，不为重写文档强迫改源码。

## 历史证据与掌握范围

Q1 保持已掌握，范围限提供骨架内 Header、ItemsSource 和三列字段 Binding。2026-10-13 原编译窗口 11 个实际场景通过，各单元格 Active、读取各条 Reading，旧行保留、同集合清空/恢复、空白保护、最小尺寸可读、BeginEdit 为 false；43 个源码哈希不变。只读设置、窗口和原命令不计本次自主设计，编辑/排序机制不扩大。

相关任务卡与验收来源：[TASK-011](../tasks/TASK-011.md)、[TASK-019](../tasks/TASK-019.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-datagrid.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft DataGrid 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid)
