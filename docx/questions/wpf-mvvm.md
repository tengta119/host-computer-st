# WPF 窗口与 ViewModel

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：ViewModel 是什么，输入框的文字怎样进入读取命令？

- 日期：2026-10-12
- 状态：待复习
- 来源：TASK-013 启动的必要讲解；沿用 TASK-012 的对象与命令场景。

TASK-013 希望读取命令拿到当前设备 ID，而不去访问窗口 TextBox。ReadingViewModel 是教练定义的普通 C# 类，保存当前 DeviceId、历史 Readings、提示 StatusText 和两个命令。

```csharp
private readonly ReadingViewModel _viewModel = new ReadingViewModel();
```

new 创建这个窗口使用的一个数据对象，_viewModel 保存引用。窗口执行 `DataContext = _viewModel`，使输入框、表格、按钮从同一个对象取属性。MVVM 是这种分工的名称：本例窗口 XAML 负责显示，Reading 保存一条读数，ReadingViewModel 提供显示所用数据与动作；这些类名都是项目自定义，DataContext 是 WPF 属性。

输入框连接如下：

```xml
Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
```

左边是这个 TextBox 的 Text，右边 DeviceId 是同一个 ReadingViewModel 的公开属性。TwoWay 让初始 TEMP-001 显示到输入框，也让用户改字后写回对象。UpdateSourceTrigger=PropertyChanged 使文字一变就写回；它是 WPF Binding 的设置，不是项目通知方法 OnPropertyChanged。

ReadingViewModel 构造函数用 `new ReadTemperatureCommand(this)` 把自己交给读取命令。命令把传入引用存到自己的 _viewModel 字段，因此 `string id = _viewModel.DeviceId;` 取得当前 ID。两个地方的字段分别属于窗口与命令，但指向同一个对象，不另建第二份。

### 2026-10-12 追问：在 TextBox 更改值，会自动更新 ViewModel.DeviceId 吗？

当前连接会。用户改成 TEMP-002，WPF 写入原对象的 DeviceId，效果相当于 `_viewModel.DeviceId = "TEMP-002"`，不要求再补这行。

```csharp
set
{
    _deviceId = value;
    OnPropertyChanged("DeviceId");
}
```

这是原 DeviceId 的写入部分。value 接到新字符串，第一行保存到字段，之后 getter 返回该字段即可取到 TEMP-002。第二行报告属性变化，作用与“取得输入框新文字”不同。写回不需要等点读取，读取按钮之后使用已保存 ID。XAML 的 TwoWay/PropertyChanged 不应加入字面星号。

### 2026-10-12 追问：文字已经在 TextBox 改过，为什么 setter 还要通知界面？

只看用户在这一个输入框输入、setter 原样保存，第一行已完成保存；通知不是取得新文字的动作。还要考虑另一个来源：程序直接执行 `_viewModel.DeviceId = "TEMP-003"`。输入框没有被用户编辑，需要对象报告变化，WPF 才重读 DeviceId 并把新 ID 显示过去。

若另一块文字也绑定 DeviceId，它也靠这个通知刷新。当前项目没增加这块文字，这是说明用途的例子。通知携带属性名 "DeviceId"，不携带新 ID，WPF 再读值。TwoWay 允许双向连接，不能替代对象变化通知。

### 2026-10-12 追问：StatusText 为什么不采用 TwoWay 和 UpdateSourceTrigger=PropertyChanged？

StatusText 显示在 TextBlock，用户不在这块提示里输入，只需对象到控件的 OneWay。命令执行 `_viewModel.StatusText = "读取成功"` 后，setter 保存、通知，WPF 重读提示并更新文字。

DeviceId 在可编辑 TextBox，需要把用户输入写回，所以用 TwoWay。UpdateSourceTrigger 另外决定“何时写回”：PropertyChanged 是文字变时，LostFocus 是离开输入框时，也是 TextBox.Text 的默认时机。失焦写回只更新数据对象，不保证已保存文件或发送设备。StatusText 的 OneWay 无写回方向，就无需这项时机设置。

旧 Reading 中的 DeviceId 是当时创建那条读数时保存的值。改变当前 ViewModel.DeviceId 不会自动把旧历史改掉，原读取命令继续创建新 Reading 并 Add 同一集合。

## 历史证据与掌握范围

Q1 保持待复习。2026-10-12 TASK-013 的 10 个场景验证读取、Trim/空白 return、Reading 创建、Add 与提示，学员自主修复漏 return。输入绑定/完整属性/通知/对象连接由骨架提供，综合机制不按通关全部升级。教练未展开修改来源导致的追问保留为教学反馈，不记错题；漏 return 的实际实践错误保留原错题记录。

相关任务卡与验收来源：[TASK-012](../tasks/TASK-012.md)、[TASK-013](../tasks/TASK-013.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-mvvm.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-control-when-the-textbox-text-updates-the-source)
- [Microsoft 属性通知文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-implement-property-change-notification)
- [WPF 数据绑定概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)
