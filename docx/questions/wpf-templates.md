# WPF 数据模板

> [返回问答索引](./README.md)

## Q1：DataTemplate 怎样把一条 Reading 显示成一行？里面的 Binding 读取谁？

- 日期：2026-10-12
- 来源：TASK-016 启动时的必要讲解。
- 状态：已掌握
- 证据边界：教练准备行模板结构和首步示例，骨架构建通过；尚无学员自主修改与实际窗口验证，不计已掌握。
- 2026-10-12 审查补充：学员自主添加三个 TextBlock，实际窗口 DeviceId/Unit 绑定 Active 且读取各自 Reading；数值路径 FallbackValue 不存在，PathError、文字为空。只有局部应用证据，整体仍待复习；不因窗口能打开将三字段显示计为掌握。启动时证据边界为此前历史。
- 2026-10-12 同轮复查：学员已自主改为 Value，三个字段实际绑定 Active、正确显示每条 Reading 的数据，旧记录保留；ID 与数值尚无间距。DoD 1/3 通过、2 未通过，问答仍待复习；上条数值失败是已修复历史。
- 2026-10-12 最终证据边界：学员自主补齐三个字段 Binding 并修正数值路径/ID 右边距，原编译窗口 10 个显示/业务场景全通过，字段分别读取对应 Reading，旧记录保留，间距可读。按提供的模板骨架中的字段绑定和逐条数据源应用范围标为已掌握；外层 DataTemplate/ItemTemplate 结构、窗口/控件定义由教练提供，不计独立模板设计或完整绑定引擎机制。上方“待复习/未运行/未通过”描述此前历史。

此前列表将已有 Reading 通过 ToString() 显示为整行字符串，难以分别控制 ID、数值和单位的间距。DataTemplate 是 WPF 提供的数据展示模板，可描述一条数据要用哪些控件显示。它不创建新读数，也不负责读取设备。

Reading 是已有自定义 C# 类，定义在 [Task010Window.xaml.cs](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)，公开属性为 DeviceId、Value、Unit。原 [ReadTemperatureCommand](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs) 每次有效读取都创建一条 Reading 并追加到原集合。例如读取 TEMP-001 后，第一条对象保存 TEMP-001、26.5、°C；换 TEMP-002 再读会追加另一条对象，旧对象的 ID 不变。

窗口后台用 `new ReadingViewModel()` 创建一个已有自定义类型的实例，将它赋给 WPF 的 DataContext 属性。输入框从这个对象的 DeviceId 读写当前输入；ListBox 的 ItemsSource 从其 Readings 属性取得集合。WPF 展示集合里的每一项时，用 ItemTemplate 描述的界面显示这条 Reading，模板内部默认从该条 Reading 取属性。因此同样的 `{Binding DeviceId}`：窗口输入框读写 ViewModel.DeviceId，第一行读第一条 Reading.DeviceId，第二行读第二条 Reading.DeviceId。无需在模板里再设置窗口 DataContext。

最小首步示例（外层已在骨架中提供，只改 TextBlock 的 Text）：

```xml
<ListBox.ItemTemplate>
    <DataTemplate>
        <StackPanel Orientation="Horizontal">
            <TextBlock Text="{Binding DeviceId, Mode=OneWay}" />
        </StackPanel>
    </DataTemplate>
</ListBox.ItemTemplate>
```

`ListBox.ItemTemplate` 是 XAML 给 ListBox 的 ItemTemplate 属性赋值的写法；内部 DataTemplate 就是该属性的值，不是第二个列表。StackPanel、TextBlock、Orientation、Text 都来自 WPF；DeviceId 是已有自定义类的属性名。Horizontal 让内部控件从左到右排列，OneWay 表示数据到显示控件的单向读取。本步没有再创建业务类或集合。

随后再放两个 TextBlock，分别显示 Value 和 Unit。Margin 是控件外部间距：`12` 为四边 12；`12,4` 为左右 12、上下 4；`0,0,12,0` 按左/上/右/下解释，只留右边 12。横向排列时，可为前两个文字留右侧空隙。沿用 Reading.ToString() 原代码即可，模板里的三项显示应各自取属性，不用拼接整行。

首步打开 [ReadingHistoryControl.xaml](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml)，只做 TODO 1，把“设备 ID 待绑定”改为示例绑定。关闭旧程序，在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`。读取 TEMP-001，再改为 TEMP-002 并读取；预期两行各自显示自己的 ID。继续改变输入不应改旧记录。设计预览不作为运行证据，随后才补其他字段与间距。

依据：[Microsoft WPF 数据模板概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/data-templating-overview)。本题只使用单个内联数据模板，不引入模板选择器或其他模板机制。

## Q2：编译通过，为什么数值没有显示？Margin 为什么要放在正确一侧？

- 日期：2026-10-12
- 来源：TASK-016 审查的实际显示失败。
- 状态：已掌握
- 同轮复查：学员已自主改为 Value，数值实际显示 26.5，路径问题已修复；当前剩余问题是 ID 与数值无间距，截图显示 TEMP-00126.5。先在第一个 TextBlock 添加右边距再实际运行，当前不将未完成间距应用计为已掌握。下文数值空白描述修复前现象，旧证据保存在 results-before-value-fix.json 与同后缀截图。
- 2026-10-12 最终证据边界：学员自主对应到 Reading.Value 并在 ID 控件添加右边距，原窗口实际字段绑定 Active、数值显示 26.5，两段文字间距均为 12，10 个场景显示/业务通过。按本次路径纠正与横向 Margin 应用范围标为已掌握，不扩展为全部编译/运行绑定机制。修复前间距证据另存 results-before-spacing-fix.json，当前 results.json 与截图为最终通过版；下文错误现象为历史。

现象：实际三条历史显示 TEMP-001/TEMP-001/TEMP-002 和 °C，数值全为空；对应 Reading 对象中 Value 均为 26.5。构建 0 警告/错误，因此不能把编译成功当作所有 Binding 有效的证据。

当前第二个 TextBlock 的写法是 `Text="{Binding FallbackValue, Mode=OneWay}"`。花括号中第一个名字是要从数据源查找的属性路径，这里等同 `Path=FallbackValue`；模板的数据源是本行 Reading。WPF 在运行时按该名字查找，但 Reading 只有 DeviceId、Value、Unit，没有 FallbackValue，因此本次绑定状态为 PathError，文字为空。OneWay 只决定读取方向，不能纠正错误的属性名。先对照 [Reading 定义](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs) 核对路径，改为其数值属性 Value 后实际运行观察。

横向 StackPanel 中，要让 ID 与后面的数值分开，可以在 ID 控件右边留空隙，或在数值控件左边留空隙。当前 ID 的 Margin 为 0；数值的 `Margin="0, 0 12 0"` 已实际解析为左 0、上 0、右 12、下 0，它隔开数值与单位，却不隔开 ID 与数值。单位右边距则位于单位之后。数值恢复后需再运行观察这两段文字的间距；本次没有代改数值，也不把未运行的修复结果记为通过。

先修一个可观察的问题：让数值正确出现，再调整行内间距并重新运行。证据：[每行属性路径、状态、文字和位置](../tasks/evidence/TASK-016/results.json)、[实际窗口截图](../tasks/evidence/TASK-016/history-three-readings.png)。路径语法依据：[WPF Binding 声明](https://learn.microsoft.com/dotnet/desktop/wpf/data/how-to-specify-the-direction-of-the-binding)。
