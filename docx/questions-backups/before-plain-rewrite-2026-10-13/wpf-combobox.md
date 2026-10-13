# WPF 设备下拉框

> [返回问答索引](./README.md) ｜ [TASK-020](../tasks/TASK-020.md)

## Q1：下拉框的两个设备从哪里来，怎样把选中的设备交给读取命令？

- 日期：2026-10-13
- 来源：TASK-020 启动的必要讲解。
- 状态：待复习

### 先认出下拉框

窗口顶部的设备选择框，点右边箭头后展开 TEMP-001、TEMP-002；点一个选项后收起并显示它。ComboBox 是 WPF 提供的下拉框类；DeviceComboBox 是教练给这个具体控件起的名字。Width 设置它的宽度。

首步打开 [Task020Window.xaml](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml) 的 TODO 1，只把 `Width="200"` 改成 `Width="260"`。关闭旧程序，在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，预期实际窗口里同一个下拉框变宽，选项数量不变。尚未连接选中值时初始为空是骨架占位；这一步不同时要求完成读取连接。

### 两个选项来自字符串数组

[Task020Window.xaml.cs](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml.cs) 已准备：

```csharp
private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };
```

`string` 是 C# 字符串类型；`string[]` 表示保存字符串的数组。`new string[]` 创建一个数组，花括号中给出两个设备 ID。`_deviceIds` 是教练起的字段名，保存数组引用。`private` 限定字段在窗口类内部使用；`readonly` 表示初始化后不能把字段重新赋成另一个数组，不代表内部元素绝对不能改。本次固定使用已给出的两个 ID，无需改数组。

`InitializeComponent()` 先按 XAML 创建控件，之后才有实际的 DeviceComboBox 可用。接着执行：

```csharp
DeviceComboBox.ItemsSource = _deviceIds;
```

左边是窗口中那个下拉框的 ItemsSource 属性；右边是含两个 ID 的数组。把数组交给这项 WPF 属性，下拉框便从数组取得可选项。它存“可以选择哪些设备”；原 Readings 集合存“已经读过哪些温度”，两者用途不同。

### 当前选中的字符串怎样交给数据对象

本次选项是字符串，因此选 TEMP-002 时，DeviceComboBox.SelectedItem 取得字符串 `"TEMP-002"`。SelectedItem 是 WPF 提供的属性，表示当前选中的项，本例中不是整个下拉框，也不是历史集合。选中后收起下拉框会显示该字符串。[Microsoft SelectedItem 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.selector.selecteditem?view=windowsdesktop-10.0)说明它表示当前选择的项。

后台还复用已有自定义类 ReadingViewModel：

```csharp
private readonly ReadingViewModel _viewModel = new ReadingViewModel();
```

`new ReadingViewModel()` 创建本窗口使用的一个数据对象，`_viewModel` 保存它的引用。对象的 DeviceId 初始为 TEMP-001。`DataContext = _viewModel` 设置窗口的 WPF 数据来源属性；窗口里的绑定从这个对象取得 DeviceId、Readings 和两个命令。

TODO 2 在 ComboBox 开始标签添加：

```xml
SelectedItem="{Binding DeviceId, Mode=TwoWay}"
```

左边设置这个下拉框的 SelectedItem；右边的 Binding 从窗口 DataContext 指向的原数据对象读取 DeviceId。初始 DeviceId 是 TEMP-001，WPF 就在两个字符串选项中选中 TEMP-001。TwoWay 允许反方向写回：用户改选 TEMP-002 后，WPF 将新字符串写到同一个对象的 DeviceId。

原 DeviceId 的完整属性实现中：

```csharp
set
{
    _deviceId = value;
    OnPropertyChanged("DeviceId");
}
```

`set` 是 C# 属性写入部分，写入的新字符串成为 `value`。第一行把 TEMP-002 保存到原字段 `_deviceId`；第二行调用既有自定义通知方法，让监听这个属性的绑定重新取值。通知工具沿用原实现，本次无需重写。原读取命令取 `_viewModel.DeviceId` 时得到现在保存的 TEMP-002，再创建一条新的 Reading 加到原 Readings 中。

旧 Reading 已保存当时的设备 ID，表格设备列取的是各行 Reading.DeviceId。因此改选当前设备不会把旧行改成 TEMP-002；不要把当前设备属性当作每行的数据源。

### 只能选择已有设备

骨架的 `IsEditable="False"` 允许展开选择 TEMP-001、TEMP-002，不能自由输入第三个 ID。键盘可能帮助选择已有项，但不会新增任意设备字符串。IsEditable 是下拉框的 WPF 属性，控制文字编辑；读取/清空按钮是否可用仍由原命令决定。[Microsoft IsEditable 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox.iseditable?view=windowsdesktop-10.0)说明此设置的选择与编辑效果。

连接完成后实际运行：默认 TEMP-001，读一次；改选 TEMP-002 后再读，看到两行分别记录两个 ID，温度/单位为 26.5/°C。随后连续切换读取、实际清空并在当前选择下再次读取。设计预览不能代替这些运行结论。

### 证据与掌握边界

启动骨架提供数组、ItemsSource、DataContext、原表格和两个命令；核心 SelectedItem 绑定由学员完成。实际 `dotnet build WpfBae/WpfBae.csproj --nologo` 退出 0、0 警告/错误，未运行窗口或验收。Width 观察不单独证明选中值写回。尚无本次自主实现证据，状态待复习；TASK-019 已验证应用范围保留，不扩大到设备对象选项、真实通信或完整 WPF 机制。
