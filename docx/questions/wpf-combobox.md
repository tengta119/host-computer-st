# WPF 设备下拉框

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：下拉框的两个设备从哪里来，怎样把选中的设备交给读取命令？

- 日期：2026-10-13
- 来源：TASK-020 启动的必要讲解。
- 状态：已掌握（限骨架中的选中字符串绑定、默认选择与写回后读取应用）

TASK-020 顶部选择框展开后有 TEMP-001、TEMP-002，点一项后收起显示它。ComboBox 是 WPF 下拉框类，DeviceComboBox 是教练给这个具体控件起的名字。Width 决定这块选择框多宽。

原第一个动作是 [Task020Window.xaml](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml) 的 TODO1：只把 Width 从 200 改成 260。整理期间重读源码，Width 已经是 260，保留这项修改，不再要求重复改写。在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，预期实际 TASK-020 窗口下拉框变宽；本次未核对运行结果，设计预览不能代替这个现象。

选项来自后台已准备的数组：

```csharp
private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };
```

string[]是 C#字符串数组写法，new 创建含两个 ID 的数组，_deviceIds 保存引用。private 限定窗口类内部使用，readonly 限定初始化后不换引用，不冻结数组内部。然后 `DeviceComboBox.ItemsSource = _deviceIds;` 把数组交给具体下拉框的 WPF 数据列表属性。它存“可选设备”，Readings 存“已读温度”，用途不同。

本次选项都是字符串，选 TEMP-002 时，SelectedItem 取得字符串"TEMP-002"。SelectedItem 是这个下拉框的 WPF 属性，不是整个控件或整份历史。

后台 new ReadingViewModel 创建本窗口的一个已有自定义数据对象，_viewModel 保存引用，DataContext 指向它，DeviceId 初始为 TEMP-001。完成 TODO2 时给 ComboBox 添加：

```xml
SelectedItem="{Binding DeviceId, Mode=TwoWay}"
```

左边设置下拉框 SelectedItem；右边从原数据对象读 DeviceId。初始值 TEMP-001 在选项中，就选中它；TwoWay 允许用户改选时把新字符串写回同一个对象。原 DeviceId setter 中的 value 接到新字符串，保存到_deviceId，原读取命令下次取它就用新 ID。

原 Reading 保存当时的 ID，表格各行取各自 Reading 属性，所以改选不改旧行。IsEditable=False 允许选已有项，不能自由输入第三个 ID；键盘可帮助选择已有项，仍不会新增任意字符串。它不决定读取按钮是否可用，按钮条件仍由原命令处理。

### SelectedItem 没写 Mode=TwoWay，为什么仍能写回？

你实际写的是 `SelectedItem="{Binding DeviceId}"`。它仍把这个下拉框当前选中的字符串连接到原数据对象DeviceId；没有显式Mode时，WPF使用这个控件属性的默认方向。SelectedItem默认就是双向，所以开始时对象的TEMP-001选到下拉框，改选TEMP-002也会写回原DeviceId，不需要额外的SelectionChanged处理器。

2026-10-13实际窗口已验证：绑定Active，声明Mode=Default，属性元数据BindsTwoWayByDefault=True；通过展开下拉框选择TEMP-002后，原DeviceId即时变为TEMP-002，下一条读数记录002，旧001行保留。显式TwoWay更直观，但本次不要求为了示例替换合法实现。这个默认规则属于SelectedItem，不能推定所有Binding都默认双向。[Microsoft属性说明](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.selector.selecteditem?view=windowsdesktop-10.0)列出该默认元数据。

前面的Width首步与显式TwoWay示例是启动讲解；选中值连接现已由学员完成并实际通过，不能再把“尚未连接/初始为空”当作当前结果。

## 历史证据与掌握范围

2026-10-13 TASK-020审查：13个实际原窗口场景与三个必需DoD均通过，默认001、展开两项、改选写回、两ID读取与连续追加、旧行保留、清空/恢复、最小尺寸均验证。45个源码哈希不变，原exe启动关闭退出0，见[结果](../tasks/evidence/TASK-020/results.json)与[核对](../tasks/evidence/TASK-020/verification.json)。本条按学员在骨架中自主SelectedItem连接及其读取效果标已掌握；选项数组、ItemsSource、DataContext、原命令/表格由既有作品提供，不算新的自主设计，完整默认元数据/框架机制不一并认定掌握。骨架初始提示仍有旧文字，不记学员错题。下方未运行/未连接是重构问答时的历史。

Q1 保持待复习，TASK-020 保持🟡进行中。2026-10-13 启动时教练提供两个选项、ItemsSource、DataContext 和复用表格/命令，骨架构建退出 0、0 警告/错误。本次整理期间发现 Width 已 260，保留工作区这项修改；未运行新代码或验收，SelectedItem 核心 TODO 仍未完成，不把单纯源码改动标为已掌握。上述运行步骤是当前任务建议，不是本轮执行证据。

相关任务卡与验收来源：[TASK-019](../tasks/TASK-019.md)、[TASK-020](../tasks/TASK-020.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-combobox.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft SelectedItem 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.selector.selecteditem?view=windowsdesktop-10.0)
- [Microsoft IsEditable 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox.iseditable?view=windowsdesktop-10.0)
