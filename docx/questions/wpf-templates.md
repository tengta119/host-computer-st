# WPF 数据模板

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：DataTemplate 怎样把一条 Reading 显示成一行？里面的 Binding 读取谁？

- 日期：2026-10-12
- 来源：TASK-016 启动时的必要讲解。
- 状态：已掌握

读取 TEMP-001、TEMP-002 后，要分别控制每行的设备 ID、数值和单位。原 ToString 把它们拼成一段文字，不方便给三段文字分别留空隙。下面给每条 Reading 安排三个文字控件：

```xml
<ListBox.ItemTemplate>
    <DataTemplate>
        <StackPanel Orientation="Horizontal">
            <TextBlock Text="{Binding DeviceId}" Margin="0,0,12,0" />
            <TextBlock Text="{Binding Value}" Margin="0,0,12,0" />
            <TextBlock Text="{Binding Unit}" />
        </StackPanel>
    </DataTemplate>
</ListBox.ItemTemplate>
```

这段位于当前 ListBox 内。ListBox.ItemTemplate 是给这个列表的“每一项怎样显示”属性赋值；DataTemplate 是 WPF 的数据展示模板。它不创建新读数，不调用设备。内部 StackPanel 从左往右排列三个 WPF TextBlock。

模板显示第一条 Reading 时，`Binding DeviceId` 取第一条的设备 ID；显示第二条时取第二条的 ID。Value/Unit 同样取本行那条读数。不是每行都去取窗口 ReadingViewModel.DeviceId，也无需另设窗口 DataContext。

ItemsSource 先从窗口数据对象拿整份 Readings；ItemTemplate 再逐条展示这份集合里的 Reading。两个来源层次对应不同对象，所以窗口输入框里的同名 DeviceId 表示当前输入，模板中的 DeviceId 表示某条历史读数。

Reading 是已有自定义类，DeviceId/Value/Unit 是其公开属性。两个 Margin 给前两块文字右侧各留 12，第三块文字不需为了前面的间距再留右边空隙。原首步只连接设备 ID，一个效果看清后再补其他字段；现在已经完成，复习读 [真实模板](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml)即可。

## Q2：编译通过，为什么数值没有显示？Margin 为什么要放在正确一侧？

- 日期：2026-10-12
- 来源：TASK-016 审查的实际显示失败。
- 状态：已掌握

当时数值全为空，Reading.Value 却是 26.5，而且构建通过。先看第二个 TextBlock 的旧错误：

```xml
Text="{Binding FallbackValue, Mode=OneWay}"
```

Binding 后的第一个名字表示从本行 Reading 查找哪个属性。这里是查找名为 FallbackValue 的属性；Reading 只有 DeviceId、Value、Unit，没有它，运行时绑定报告 PathError，文字就为空。

正确路径是 `Text="{Binding Value, Mode=OneWay}"`，显示这条读数的 Value。OneWay 只说取值方向，不能修正拼错的属性名。XAML 编译成功不保证运行时每个数据属性路径都有效。

数值修好后当时又看到 `TEMP-00126.5` 紧挨着。横向排列时，空隙应在“ID 的右边”或“数值的左边”：

```xml
<TextBlock Text="{Binding DeviceId}" Margin="0,0,12,0" />
```

四个数是左、上、右、下，此处在 ID 之后留 12。若只给数值右边留 12，它隔开的是数值与单位，不能隔开 ID 与数值。一个数表示四边相同；两个数分别是左右、上下；标准四数写法更容易看清是哪边。

原复查依次修一个现象：先让 Value 出现，再把 ID 右边距补齐。两次修复最终都已通过，复习不应把旧截图说成当前错误，也不要求覆盖现有正确代码。

## 历史证据与掌握范围

Q1/Q2 保持已掌握，范围限模板骨架中的三字段绑定、逐条数据源、路径纠正与横向间距。2026-10-12 TASK-016 数值错误、随后缺间距均由学员自主修复，最终 10 个场景通过，各绑定 Active，两段间距 12，旧行保留。外层模板与业务连接由骨架提供，完整绑定引擎不一并升级；修复前版本及证据在原稿/任务证据中保留。

相关任务卡与验收来源：[TASK-010](../tasks/TASK-010.md)、[TASK-013](../tasks/TASK-013.md)、[TASK-016](../tasks/TASK-016.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-templates.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft WPF 数据模板概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/data-templating-overview)
- [WPF Binding 声明](https://learn.microsoft.com/dotnet/desktop/wpf/data/how-to-specify-the-direction-of-the-binding)
