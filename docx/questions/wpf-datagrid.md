# WPF 读数表格

> [返回问答索引](./README.md) ｜ [TASK-019](../tasks/TASK-019.md)

## Q1：怎样把读取历史显示为三列？表头和 Binding 分别给谁用？

- 日期：2026-10-13
- 来源：TASK-019 启动的必要讲解。
- 状态：已掌握（限提供的表格骨架中 Header、ItemsSource 与逐行字段绑定应用）

2026-10-13 实践复查：学员自主补齐设备表头、Readings 集合连接与 DeviceId/Value/Unit 三列 Binding，原编译窗口 11 个实际场景全通过。各单元格绑定 Active，DataItem/DataContext 均为该条 Reading；换 ID 后旧行保持，原集合清空/恢复与空白保护正常，最小尺寸文字可读。按此应用范围标已掌握；只读设置、窗口对象连接由骨架提供，原命令复用既有作品，不计本次自主设计，不扩大为框架内部机制或编辑/排序掌握。列 Binding 未写 Mode，本次只读实际显示有效，不要求为了示例强补 OneWay。证据见 [结果](../tasks/evidence/TASK-019/results.json)及[核对](../tasks/evidence/TASK-019/verification.json)。下方“首步/尚未连接/未运行”是启动讲解与当时证据。

希望实际读取 TEMP-001、TEMP-002 后看到：

| 设备 ID | 温度 | 单位 |
| :--- | :--- | :--- |
| TEMP-001 | 26.5 | °C |
| TEMP-002 | 26.5 | °C |

此前 ListBox 展示历史，本次由 WPF 提供的 DataGrid 表格展示同一集合。每条 Reading 占一行，三个属性分别放入三列。Reading 是已有自定义 C# 类，DeviceId、Value、Unit 是它的公开属性；这两个示例行是本次目标效果，尚不是运行证据。

### 先只改第一列表头

打开 [Task019Window.xaml](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml) 的 TODO 1，第一列目前写成：

```xml
<DataGridTextColumn Header="设备列（待命名）" Width="*" />
```

DataGridTextColumn 是 WPF 提供的文字列。Header 设置这列顶部显示的标题；把 Header 的值改成“设备 ID”，影响的是第一列顶上的文字，不会创建读数或连接数据。Width 的星号让三列分配表格剩余宽度，骨架已提供，不需要先改布局。

这一步只改 Header。关闭旧程序，从工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，预期在实际 TASK-019 窗口看到新的第一列表头，表格仍为空。编辑器提示和设计预览不能代替实际运行。

### 再给整张表格指定历史集合

窗口后台的 `new ReadingViewModel()` 创建本窗口使用的数据对象，字段 `_viewModel` 保存它；ReadingViewModel 是此前自定义类，不是 WPF 的表格类型。`DataContext = _viewModel` 设置 WPF 窗口的数据来源，输入框和命令按钮已从该对象取属性。

TODO 2 在 DataGrid 开始标签中添加：

```xml
ItemsSource="{Binding Readings, Mode=OneWay}"
```

ItemsSource 是表格取得整份行数据的 WPF 属性；这处 Binding 从窗口的数据对象取得 Readings 属性，得到原来的 ObservableCollection<Reading>。Readings 是已有自定义属性名；OneWay 让表格从该属性取集合。本行设置只连上整份集合，尚未告诉每列取哪个字段。

### 然后指定每一行的设备列取什么

TODO 3a 的第一列可以写成：

```xml
<DataGridTextColumn Header="设备 ID"
                    Binding="{Binding DeviceId, Mode=OneWay}" Width="*" />
```

这里 Header 的“设备 ID”仍是标题；列的 Binding 告诉这列：显示这一行 Reading 的 DeviceId。显示第一行时，取第一条 Reading.DeviceId，得到 TEMP-001；显示第二行时，取第二条 Reading.DeviceId，得到 TEMP-002。这处 DeviceId 不读取当前输入框对应的 ReadingViewModel.DeviceId，所以之后继续改输入，不会修改旧行。

温度列和单位列采用同样的写法，TODO 3b/3c 分别使用 Reading 的 Value、Unit 属性名。温度列的 Header 可以写“温度”，Binding 要写 Value；中文标题不是属性名。三个字段分别取值，不依靠 Reading.ToString() 拼接整行；不需要为每行再创建 ViewModel。

### 为什么骨架只显示三列，且不能直接编辑

DataGrid 默认可以自动生成列。骨架设置 `AutoGenerateColumns="False"`，让 HistoryDataGrid 只用 DataGrid.Columns 内声明的三列，避免与自己列出的列重复。`DataGrid.Columns` 是 XAML 给这张表格列集合添加列的写法，里面的三个 DataGridTextColumn 不是三个表格。

`IsReadOnly="True"` 禁止在单元格里修改读数。CanUserAddRows/CanUserDeleteRows=False 进一步禁止从表格直接新增或删除记录。这些 WPF 设置已由骨架提供，不影响“读取温度”命令往集合 Add，也不影响“清空历史”命令调用 Clear。

### 集合变化与列取值各负责一件事

原命令每次有效读取创建一条 Reading，调用同一个 Readings 集合的 Add。ObservableCollection 发出新增通知，已经连接它的表格增加一行；然后三列的 Binding 从新行对应的 Reading 取三个属性。Clear 移除所有记录并通知表格清空，集合对象保留，之后还能 Add。不要每次读取另建集合，或给表格手动添加行来刷新。

完整连接后，依次读取 TEMP-001、TEMP-002，检查两条旧记录各自的三字段；清空再读取应恢复。空字符串/纯空格输入应保留已有行并显示原校验提示。这些是后续实际验证步骤，不在改表头这一步同时要求完成。修改已有 Reading 内部属性的通知、编辑和排序策略不在本次范围。

依据：[Microsoft DataGrid 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid)，确认整集合与逐行属性绑定、手动列、只读和集合变化通知的分工。

### 本次证据边界

仅准备骨架与实际构建，退出 0、0 警告/错误；未运行窗口，TODO 2/3 尚未实现，DoD 全部未勾选。不把构建成功或提供了只读设置计为自主掌握；TASK-011/013/016 的原已验证应用范围保留。
