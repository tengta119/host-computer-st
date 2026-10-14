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

## Q2：TASK-026怎样让全部温度历史与最新三点共用一个集合？

- 日期：2026-10-14
- 来源：TASK-026 开启的必要讲解。
- 状态：已掌握（限已有骨架中连接原温度集合、配置单列标题/单位、关闭自动列及设置只读/禁止新增的实际应用）

- 2026-10-14最终复查：9个数据状态/37项技术检查全通过，最小860×600可读，三个DoD全部勾选；根据上述自主应用与实际证据升级本题有限掌握范围，教练提供的点路径Binding/图表/布局、未完成星号列宽与完整定时采集机制不一并升级，首次待复习/最小遮挡记录保留为历史。

- 2026-10-14当前实践证据：学员已自主连接ItemsSource、关闭自动列、标注温度单位、设置只读/禁止新增，实际9个数据状态全通过，同一集合/各行显示/清空恢复均正常；仅骨架最小高度不足待修复，Q2整体暂保持待复习，提供的点路径Binding、绘图和布局不计自主设计，星号列宽未完成也不列为掌握。
- 当前文件：[后台](../../WpfBae/Exercises/TASK-026-history-and-chart/Task026Window.xaml.cs)、[界面](../../WpfBae/Exercises/TASK-026-history-and-chart/Task026Window.xaml)。

这次左边画最新最多三条，右边保存全部温度。第四次添加后，右边应有 24、26.5、25、28 四行，左边只画 26.5、25、28。不要删掉第一条来限制图点数：后台的绘图循环已从完整历史里选择最后三条。

先找到后台的 `_temperatures`。这是教练命名的字段，指向一份 .NET 的 `ObservableCollection<double>`；集合每项直接是 24 这样的温度数值，double 是 C# 的数值类型。按钮对这份集合 Add/Clear，计数读它的 Count，绘图也从它取温度。这些已完成代码沿用 TASK-025。

右侧 `HistoryDataGrid` 是教练给具体表格起的名字，DataGrid 才是 WPF 提供的表格类。表格目前还不知道行从哪里来，所以点按钮时图和计数会变，右侧仍没有温度记录。

### 先接通行来源，只改后台一处

在构造方法的 TODO 1 处补：

```csharp
HistoryDataGrid.ItemsSource = _temperatures;
```

左边的 ItemsSource 是 WPF 提供的表格属性，指定这张表格显示哪个集合的记录；右边是原温度集合。等号是 C# 普通赋值，没有复制温度或创建新集合。放在 InitializeComponent 后，是因为这个 WPF 方法先创建了 XAML 中的具体表格，之后才能设置它的属性。

接好以后，Add 保存新温度并发集合变化通知，表格收到后增加行。同一次变化还调用已有的 Temperatures_CollectionChanged 方法，更新条数并重新画图。表格自动处理自己的行，蓝线仍需要已有绘图方法将温度换算成坐标；二者共用原温度，不要求表格也只留三行。

在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，确认标题是 TASK-026，点一次添加，预期表格出现 24、计数为 1、左侧显示 24 度单点。首步只观察这条连接；其余 TODO 随后完成。

### 这一行直接是数值，为什么 Binding 不写 Value？

教练为了让首步能立即观察数值，先提供这一个临时文字列：

```xml
<DataGridTextColumn Header="待设置列标题"
                    Binding="{Binding Path=., Mode=OneWay}" Width="100" />
```

DataGridTextColumn 是 WPF 的文字列，负责把每一行的数据取出显示。TASK-019 每行是一条自定义 Reading，所以温度列取 Reading.Value；这里第一行直接是 double 数值 24，没有 Reading.Value 这个属性。

`Path=.` 中的点表示读取当前行本身：第一行取 24，第二行取 26.5。它是 WPF Binding 的路径写法，不是 C# 的对象成员访问点。Mode=OneWay 让格子只取这个值显示，不把文字写回数值。点路径的规则见[Microsoft Binding 声明说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/binding-declarations-overview#binding-path-syntax)。这个绑定由教练提供，理解和使用不直接算自主编写证据。

Header 目前只让表头显示“待设置列标题”，不会改任何温度。TODO 2 将标题改为“温度 (°C)”，告诉读表的人数值单位；将这列 Width 从 100 改为星号，让它使用表格内部可用宽度。

同一 TODO 还要把 HistoryDataGrid 上的 AutoGenerateColumns 改为 False。它管这张表是否另外根据数据属性自动生成列；False 表示只用手写的这一列。直接数值没有 TASK-019 的三个 Reading 属性，不能依赖自动生成得到明确的温度列。

### 只读与禁止新增分别管什么？

TODO 3 在 HistoryDataGrid 的开始标签内设置 `IsReadOnly="True"` 和 `CanUserAddRows="False"`。前者禁止在格子里编辑温度，后者禁止从表格新建记录，也关闭用于新增的占位行；这些是 WPF 属性，不是后台的新方法。是否出现占位行还取决于数据源能否新增，不能把界面中的占位当作集合真实历史。

按钮仍能调用集合 Add/Clear，这两个设置只是限制用户直接在表格里操作。骨架已提供 CanUserDeleteRows=False 和 CanUserSortColumns=False，保留按添加顺序对照历史。[Microsoft DataGrid 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid)说明表格数据来源与这些编辑行为。

### 两边怎样放得下？

骨架中 ChartRegion 包住左侧标题和 560 宽的 Canvas，放在外层两列 Grid 的第 0 列；HistoryRegion 包住右侧标题和表格，放在第 1 列。它们都是教练命名的具体容器。左列 Auto 按图表与右边距需要安排宽度，右列星号取得剩余宽度；窗口拉宽时右区增宽。

HistoryRegion 内部另有两行，标题在第 0 行、表格在第 1 行，后者取得内部剩余高度。表格没有固定宽高，因此可以使用右区提供的空间。默认窗口 960×620，约定最小 860×520；这些布局由教练准备，仍需学员实际运行确认可读。

完成后连续添加五次：第四次表格四行、计数 4，图是第 2/3/4 条；第五次表格五行、计数 5，图是第 3/4/5 条的 25/28/24。清空使同一集合零项，表格清空，已有方法清除旧点/线/标签并显示计数 0；再添加恢复一行 24 与单点。以上是预期现象，本次仅构建骨架，尚无 TASK-026 运行验收证据。

## 历史证据与掌握范围

2026-10-14 TASK-026最终复查通关：复查读取到MinHeight已600，原编译窗口实际Show，9个数据状态/37项技术检查全通过，表格完整历史/计数/最新三点、只读/无新增行、清空恢复及默认/最小/恢复尺寸正确，原exe启动关闭退出0；57项源码在复查期间保持不变，原DLL与审查器DLL一致，Q2仅按已有骨架内原集合连接、单列标题与编辑限制应用标已掌握，MinHeight修正不计自主设计，教练骨架缺陷由教练负责规则已补充，无新学员错题，见[最终证据](../tasks/evidence/TASK-026/recheck-01/README.md)。

2026-10-14 TASK-026审查：实际原编译窗口9个数据状态全部通过，第四/第五条完整表格与最新三点正确，所有实际单元格Binding为Active，表格BeginEdit=False，无新增占位行；默认尺寸与恢复通过，最小860×520遮挡原值标签，属于教练提供骨架MinHeight过小。额外诊断860×600可读，但未改源码或勾选最后DoD，任务仍进行中，Q2保持待复习，已验证局部应用证据保留；57项源码哈希不变、DLL一致，无新学员错题，见[审查证据](../tasks/evidence/TASK-026/README.md)。

2026-10-14 TASK-026 开启：Q2 新增为待复习，沿用 Q1 已验证的有限应用范围。两个独立窗口文件与入口已准备，核心数据来源与最终列/编辑设置留 TODO；当前行 Binding 和旧绘图由教练提供。仅构建成功，未实际运行或验收，不新增自主掌握、错题或阶段通关。

Q1 保持已掌握，范围限提供骨架内 Header、ItemsSource 和三列字段 Binding。2026-10-13 原编译窗口 11 个实际场景通过，各单元格 Active、读取各条 Reading，旧行保留、同集合清空/恢复、空白保护、最小尺寸可读、BeginEdit 为 false；43 个源码哈希不变。只读设置、窗口和原命令不计本次自主设计，编辑/排序机制不扩大。

相关任务卡与验收来源：[TASK-011](../tasks/TASK-011.md)、[TASK-019](../tasks/TASK-019.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-datagrid.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft DataGrid 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid)
