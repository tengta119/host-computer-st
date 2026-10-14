# TASK-020：从下拉框选设备，再让原命令读取它

> 2026-10-14 整理。先分清可选设备、当前选中设备和历史读数，再读SelectedItem连接。任务已于2026-10-13通关，本轮只整理文档。

## 1. 下拉框替代自由输入

窗口里的DeviceComboBox用WPF的ComboBox创建。展开后恰好两个选项：TEMP-001、TEMP-002。

启动时选中TEMP-001，读取一条；改选TEMP-002再读取，新行用002，旧行保留001。当前下拉框不允许输入第三个任意ID。

原读取/清空命令没有重写。改变的是“当前设备ID从哪里来”：从手动输入改为选择字符串。

## 2. _deviceIds 数组保存候选项

后台已有：

```csharp
private readonly string[] _deviceIds =
    new string[] { "TEMP-001", "TEMP-002" };
```

string[]是C#字符串数组，new创建它，_deviceIds是教练取的字段名。两项都是设备编号文字，不是新的Device对象。

private限制字段在类内部使用；readonly限制初始化/构造之后把字段换指另一数组，不冻结数组元素。

这份数组存“可以选择什么”，Readings集合存“已经读过什么”。它们的用途不同，选项不因读取次数增加而变多。

## 3. ItemsSource 把这两个字符串交给下拉框

构造函数中：

```csharp
InitializeComponent();
DeviceComboBox.ItemsSource = _deviceIds;
DataContext = _viewModel;
```

InitializeComponent创建真实下拉框。ItemsSource是ComboBox的WPF属性，接收可选项数组，展开时显示这两个值。

DataContext再给默认Binding指定原ReadingViewModel。前一句提供候选项，后一句指定读写业务属性的对象；它们不会因为都“提供数据”就成为同一个设置。

## 4. 先看当前完整的下拉框

```xml
<ComboBox x:Name="DeviceComboBox" Width="260"
          HorizontalAlignment="Left" IsEditable="False"
          SelectedItem="{Binding DeviceId}"
          Margin="0,8,0,12" />
```

Width=260控制这个控件宽度；HorizontalAlignment=Left使控件靠左；Margin在它上下留白。

IsEditable=False禁止自由编辑文字，仍允许展开选择已有项。它不决定读取按钮是否可用，也不禁止键盘选择已有项。

## 5. SelectedItem 取得的是当前选中字符串

本题选项为字符串，选TEMP-002后，DeviceComboBox.SelectedItem取得字符串"TEMP-002"。

它不是整个ComboBox控件，不是全部选项数组，也不是一条温度Reading。SelectedItem是WPF的选择属性，当前项是什么类型取决于ItemsSource中的项目。

以后如果ItemsSource装设备对象，选中值才会是那个对象；本题不提前引入设备类、SelectedValuePath或复杂选择设计。

## 6. SelectedItem 与 DeviceId，分别属于谁

当前XAML：

```xml
SelectedItem="{Binding DeviceId}"
```

左边属于下拉框，右边DeviceId属于窗口DataContext指向的ReadingViewModel。

连接建立后，原对象DeviceId初值TEMP-001在选项里，所以初始选中001。用户改选002，再把选中字符串写进同一个对象的DeviceId。

无需额外SelectedIndex来制造初始值，也不用在SelectionChanged处理器里再次做相同写回。

## 7. 先用显式TwoWay理解两条路

说明写法：

```xml
SelectedItem="{Binding DeviceId, Mode=TwoWay}"
```

TwoWay允许两条方向：

```text
开始时：ViewModel.DeviceId=TEMP-001 → 下拉框选中001
改选时：下拉框选中TEMP-002 → ViewModel.DeviceId=TEMP-002
```

原DeviceId setter收到新字符串作为value，存入_deviceId。读取命令随后取这个公开属性，就得到新的ID。

本题不要求制造未选中/null情形；固定两项正常选择与默认值才是当前范围。

## 8. 实际源码省略Mode，为什么效果相同

学员实际写的是Binding DeviceId，没有显式TwoWay。WPF使用SelectedItem属性的默认方向，而该属性默认支持双向。

历史运行记录明确：声明Mode=Default，属性元数据BindsTwoWayByDefault=True，选中变化实际写回DeviceId。元数据可以先理解成框架为这个属性规定的默认配置，不要求在项目里自行查询。

显式TwoWay便于阅读，当前省略写法也正确，不强制重做。这个默认规则属于SelectedItem，不能推定所有Binding都默认双向。

## 9. 读取按钮继续取得同一ViewModel的命令

```xml
Command="{Binding ReadCommand, Mode=OneWay}"
```

按钮从同一个对象取得原ReadCommand。命令Execute读取_viewModel.DeviceId，当前选择002就创建Reading.DeviceId=TEMP-002，再Add原历史集合。

选中写回与执行读取是分开的：只改选不点击，不新增历史，也不自动读取真实设备。

## 10. 改选后，为什么旧行不变

```text
当前ViewModel.DeviceId：改为TEMP-002
旧Reading.DeviceId：仍是当时保存的TEMP-001
```

表格各列读取每行Reading，当前选择绑定的是ViewModel。它们同名属性分属不同对象。

再次读取才新建一条002记录。不要让整张表的每行都绑定回下拉框当前选择，否则旧行会失去历史意义。

## 11. 清空与恢复不重建选项或对象

原ClearCommand清空Readings，状态为“已清空”，集合Count=0时清空按钮禁用。设备数组和当前DeviceId没有因此删除。

如果当前选择002，清空后再读仍使用002。空集合中改选也可以，当前没有把选择操作与清空命令混在一起。

每个窗口创建自己的ViewModel；当前窗口选择、表格与两个命令共用这一份，不在各区域另new。

## 12. 当前初始提示不是未完成证据

后台仍有“选中值尚未连接DeviceId”的旧骨架文字，注释也保留TODO。当前SelectedItem绑定已在源码存在并实际验证。

应根据真实属性连接和最新任务卡判断状态，不因教学占位文案撤销已有实践结果。读取/清空后的业务提示则由原命令正常更新。

## 13. 按不同问题检查不同位置

| 现象 | 先核对 |
| :--- | :--- |
| 展开没有两个选项 | ItemsSource是否交给_deviceIds |
| 能选002但读取仍001 | SelectedItem绑定、写回方向、是否同ViewModel |
| 初始不显示001 | DeviceId初值与实际选项是否对应 |
| 改选后旧行变了 | 表格是否读取各条Reading |
| 用户不能自由输入 | IsEditable=False是本题预期 |
| 清空后不能再读取 | 原集合/命令连接，不先改下拉框 |

## 文末查阅：文件与历史范围

实际文件：[窗口](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml)、[后台](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml.cs)、[原ViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[任务卡](../tasks/TASK-020.md)、[下拉框问答](../questions/wpf-combobox.md)。

当前入口TASK-025，复跑旧题需明确临时StartupUri并恢复，本轮未运行。可选小观察是只改变ComboBox.Width，重跑本题看控件变宽而选项/读取不变，再恢复；不同时改数据连接。

已读取 [13场景结果](../tasks/evidence/TASK-020/results.json)：全部passed、无运行异常，默认选择/实际展开/选择写回/旧行/清空恢复/最小尺寸通过。选择通过下拉框自动化走真实选择流程，读取/清空走按钮路径，不是直接改ViewModel模拟。核对见 [verification](../tasks/evidence/TASK-020/verification.json)，启动见 [launch](../tasks/evidence/TASK-020/launch.json)，显示见 [历史](../tasks/evidence/TASK-020/history-three-readings.png)、[最小尺寸](../tasks/evidence/TASK-020/minimum-size.png)。

审查工具曾使用不存在的AutomationPeer类导致工具构建失败，修正后通过，不计学员错误。自主范围限SelectedItem连接/默认选择与写回后读取；选项数组、ItemsSource、DataContext、表格和原命令来自骨架/旧作品，完整默认规则不一并升级。

官方查阅：[Microsoft SelectedItem](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.selector.selecteditem?view=windowsdesktop-10.0)。下一篇 [TASK-021](./TASK-021-wpf-grid-rows.md)让历史表格随窗口高度增长。
