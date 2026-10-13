# TASK-016：一条历史用三个 TextBlock 显示

> 2026-10-13 白话重写。先从“这一行对应哪条 Reading”讲起，再看模板写法和间距。当前数值路径与间距已经修复，历史失败不会当作当前状态。

## 1. 这次要把哪段文字拆开

过去列表靠 Reading.ToString 显示一段整行文字。现在希望 ID、数值、单位各自放到一个文字控件里，能单独控制间距。

比如一条 Reading 保存：

```text
DeviceId = TEMP-001
Value = 26.5
Unit = °C
```

这一行要显示成：

```text
[TEMP-001]  [26.5]  [°C]
 文字控件1  控件2   控件3
```

DataTemplate 是 WPF 提供的“怎样显示这一个数据项”的描述。它说明用哪几个控件展示 Reading，不负责创建读数或读取设备。

## 2. 数据还是原 Reading 和原集合

Reading 是 TASK-010 自定义类，公开属性为：

```csharp
public string DeviceId { get; init; } = "";
public double Value { get; init; }
public string Unit { get; init; } = "";
```

get 允许读取；init 允许初始化时赋值。普通外部代码不能在初始化后随意改这几个属性。

原读取命令每次有效执行都会：

```csharp
_viewModel.Readings.Add(new Reading
{
    DeviceId = id,
    Value = 26.5,
    Unit = "°C"
});
```

因此模板不用 new Reading。它显示的是已经加入集合的对象，ViewModel.Readings 的 getter 仍返回原集合。

## 3. ItemsSource 管哪些记录，ItemTemplate 管每条怎么显示

当前 ListBox 外层：

```xml
<ListBox x:Name="HistoryListBox"
         ItemsSource="{Binding Readings, Mode=OneWay}"
         Height="170">
    <!-- 内部设置 ItemTemplate -->
</ListBox>
```

ItemsSource 去 ViewModel 读取 Readings，取得要显示的历史集合。

ItemTemplate 是 ListBox 的另一个属性，接收一份 DataTemplate。它规定“集合里的每条 Reading，用什么内部界面显示”。

有模板但集合为空，列表仍没有历史行。模板没有替读取命令执行 Add。

## 4. ListBox.ItemTemplate 的点号，是给属性赋值

当前结构先看外层：

```xml
<ListBox.ItemTemplate>
    <DataTemplate>
        <StackPanel Orientation="Horizontal">
            <!-- 这里放三个文字控件 -->
        </StackPanel>
    </DataTemplate>
</ListBox.ItemTemplate>
```

ListBox.ItemTemplate 是 XAML 的属性元素写法：把里面的 DataTemplate 交给这个 ListBox 的 ItemTemplate 属性。

DataTemplate 是具体模板对象，StackPanel 描述每行的排列方式。这里没有创建第二个列表，也没有另写一个名为 ItemTemplate 的自定义类。

当前模板直接写在列表中，未放入资源字典，未定义 DataType/模板选择器。先用这一份内联模板即可。

## 5. 先看一个字段，再把另两个加进来

ID 字段：

```xml
<TextBlock Text="{Binding Path=DeviceId, Mode=OneWay}" />
```

左边 Text 是本行 TextBlock 的文字；右边读取这行 Reading.DeviceId。Path 指出属性名。

当前源码省去 Path=：

```xml
<TextBlock Text="{Binding DeviceId, Mode=OneWay}" Margin="0,0,12,0" />
```

含义相同。Value 字段读 Reading.Value，Unit 字段读 Reading.Unit。Binding 读取属性值，不把“DeviceId”这几个字当作固定显示文字。

## 6. 同样叫 DeviceId，输入框与历史行读取谁

假设先读 TEMP-001，再读 TEMP-002，然后只把输入改成 TEMP-EDIT-ONLY：

| 位置 | 读哪个对象 | 应显示的 ID |
| :--- | :--- | :--- |
| 输入框 | 当前 ViewModel.DeviceId | TEMP-EDIT-ONLY |
| 第一行文字 | 第一条 Reading.DeviceId | TEMP-001 |
| 第二行文字 | 第二条 Reading.DeviceId | TEMP-002 |

属性名相同，所属对象不同。输入框编辑的是用户此刻的 ID，历史行显示已经保存的 ID。

不要在模板里把数据来源强行改回窗口 ViewModel，否则每行可能都显示当前输入，失去历史意义。

## 7. WPF 展示集合项时，会把那条 Reading 交给行内绑定

窗口创建一份 ReadingViewModel 并设置 DataContext。列表与清空按钮使用这份对象。

列表显示集合里的某一项时，当前模板内部从那一条 Reading 取值。你不用手动为每条记录 new ViewModel 或给每个 TextBlock 设置数据对象。

```text
列表 ItemsSource：ViewModel.Readings → 集合
第一行内部绑定：第一条 Reading
第二行内部绑定：第二条 Reading
```

历史审查检查了每行 TextBlock.DataContext 与绑定 DataItem，都与对应 Reading 是同一对象。这是具体证据，不是凭窗口父级位置猜测所有绑定都读同一对象。

## 8. 一份模板，生成各行自己的文字控件

当前只写一次“横向 StackPanel + 三个 TextBlock”。WPF 用这个描述为已经显示的每行建立相应界面。

第一行和第二行的 TextBlock 不是同一个对象，否则无法分别显示 TEMP-001 和 TEMP-002。共用的是模板定义，不是共用一组会覆盖文字的控件。

读数集合保存 Reading，不保存 TextBlock。创建行内控件由 WPF 负责。长列表可能还有控件虚拟化，不把本题少量行的结果推广为所有行必须一次生成。

## 9. Horizontal 让这一行从左到右排

```xml
<StackPanel Orientation="Horizontal">
```

Orientation 是 WPF 的排列方向属性。Horizontal 表示里面三个文字控件从左往右排。

它改变的是“一条 Reading 的内部”，不是把所有历史记录放到一横行。外面的 ListBox 仍显示多条历史。

每行按自己的文字宽度排列；更长的 ID 会把该行数值往右推。固定 Margin 保证空隙，不自动让所有行变成对齐表格列。本题只要求三项可读间距。

## 10. Margin 的数字，先按四边读

Margin 是控件外部留白：

| 写法 | 左、上、右、下 |
| :--- | :--- |
| 12 | 12、12、12、12 |
| 12,4 | 12、4、12、4 |
| 0,0,12,0 | 0、0、12、0 |

当前 `0,0,12,0` 只在这个文字控件右侧留空。

底层是 WPF 的 Thickness，默认布局单位为 1/96 英寸；不是所有屏幕上必定对应 12 个物理像素。文字 Margin 与按钮 Padding 是不同位置：前者分开字段，后者留出按钮文字四周空间。

## 11. 间距要加在相邻字段之间

```text
[ID] -- ID 右留白 -- [数值] -- 数值右留白 -- [单位]
```

数值控件右边距隔开“数值与单位”，不能隔开前面的“ID 与数值”。

让 ID 与数值分开，可给 ID 右侧加留白，也可给数值左侧加留白。当前学员选择给第一个 TextBlock 加右 Margin=12。

单位自身的右 Margin 留在单位之后，不是前两项间距来源。两边都加留白时要合起来看，避免无意加成两份空隙。

## 12. 当前完整模板：保留源码实际写法

```xml
<ListBox.ItemTemplate>
    <DataTemplate>
        <StackPanel Orientation="Horizontal">
            <TextBlock Text="{Binding DeviceId, Mode=OneWay}"
                       Margin="0,0,12,0" />
            <TextBlock Text="{Binding Value, Mode=OneWay}"
                       Margin="0, 0 12 0"></TextBlock>
            <TextBlock Text="{Binding Unit, Mode=OneWay}"
                       Margin="0, 0 12 0"></TextBlock>
        </StackPanel>
    </DataTemplate>
</ListBox.ItemTemplate>
```

数值/单位的 Margin 混用逗号与空格，实际已被 WPF 解析为 0,0,12,0。它书写不统一，但不是本题编译/启动失败。

三个 TextBlock 分别读三个属性，不拼成整行字符串，不调用 Reading.ToString。原 ToString 保留在类中，不妨碍当前模板显示。

## 13. OneWay 能初次读值，不给数据类自动加通知

OneWay 让 Reading 的值进入文字控件，不从这个 TextBlock 把文字写回 Reading。

Value 实际是 double，当前运行中它被转换成文字 26.5；Unit 单独显示 °C。没有把 Value 属性改成字符串，也没有自定义 StringFormat 或 Converter。

文化设置可能影响数字显示格式，本题没有固定所有环境的格式规则。Reading 没有实现属性变化通知，get/init 也限制后续赋值；本题验收的是新记录显示，不要求运行时改同一记录的 Value 后自动刷新。

## 14. 原集合与命令继续控制追加、清空和提示

```text
ReadCommand 有效读取 → Add 原集合 → 列表生成该记录的显示
ClearCommand → Clear 原集合 → 列表为空
集合变化 → 清空命令条件重查 → 按钮启用/禁用
StatusText 属性通知 → 窗口提示更新
```

DataTemplate 描述显示，没有代替任何通知。ObservableCollection 通知成员增删，不会自动替每个 Reading 发布属性变化。

窗口仍用输入 TwoWay、命令 OneWay、提示 OneWay；两按钮仍取同一 ActionButtonStyle。读取按钮 StaticResource、内部清空 DynamicResource，加载时机沿用 TASK-015。模板内读 Reading，不会让模板外的清空按钮改读最后一条记录。

## 15. 历史数值空白：路径写成了不存在的属性

已修复的反例：

```xml
<TextBlock Text="{Binding FallbackValue, Mode=OneWay}" />
```

第一个名字是要找的数据属性，等价于 Path=FallbackValue。但 Reading 只有 DeviceId、Value、Unit，没有 FallbackValue。

历史结果中数据对象 Value=26.5，数值 Text 为空、绑定 PathError。所以不是采集没拿到数值，而是显示端读错属性。

改为 Binding Value 后，绑定 Active 并显示 26.5。OneWay 只管方向，不会猜出你本来想找哪个属性。

这里的“路径 FallbackValue”也不等于绑定的 `FallbackValue=后备文字` 配置；后者是读不到有效值时的后备显示。正确修复是对上 Reading.Value，不用后备文字掩盖路径问题。

## 16. 两次修复分开看，编译通过不代表行显示正确

| 历史阶段 | 数值 | ID 与数值的空隙 |
| :--- | :--- | :--- |
| Value 路径修复前 | 空文字，PathError | ID 没留右边距 |
| Value 已修复 | 26.5，Active | 仍紧贴，显示 TEMP-00126.5 |
| ID 右 Margin 已加 | 26.5，Active | 实测 12，分开可读 |

学员自主修复了两项，教练未代改。先让数字出现，再处理间距，才能区分问题。

构建成功说明项目能编译；窗口能开说明初始化完成；属性有 26.5 说明数据存在；还要检查 Text 的内容、绑定来源、实际间距。空历史没有行，不能因无显示错误就当作字段已经验证。

## 文末查阅：文件、坐标与历史依据

实际文件：[历史控件 XAML](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml)、[控件后台](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml.cs)、[窗口 XAML](../../WpfBae/Exercises/TASK-016-reading-datatemplate/Task016Window.xaml)、[窗口后台](../../WpfBae/Exercises/TASK-016-reading-datatemplate/Task016Window.xaml.cs)、[Reading 定义](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)、[任务卡](../tasks/TASK-016.md)。

可选小观察：只把 ID 的右 Margin 从 12 改为 20，运行本任务，预期第一段间距增大、数值与单位原间距不变，再恢复。当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-017，复跑 TASK-016 需明确临时入口并恢复，本轮没有运行。

2026-10-12 最终已通关：[最终结果](../tasks/evidence/TASK-016/results.json)、[核对](../tasks/evidence/TASK-016/verification.json)、[审查程序](../tasks/evidence/TASK-016/Review.cs)。10 个显示/业务场景通过：初始、首次、重复、换 ID、仅改输入、两种空白、清空、Trim 后恢复、再次清空；原对象/集合/命令/Style 保持。

历史对比：[路径修复前](../tasks/evidence/TASK-016/results-before-value-fix.json)、[间距修复前](../tasks/evidence/TASK-016/results-before-spacing-fix.json)。第一行 ID 左边为 27、宽约 58.8433；间距修复前数值左边 85.8433，正好接上 ID；修复后为 97.8433，多出 12。数值宽 24、单位左边 133.8433，第二段也为 12。这些是该次布局测量，不用来硬编码全部屏幕坐标。

图片：[缺数值](../tasks/evidence/TASK-016/history-three-readings-before-value-fix.png)、[文字紧贴](../tasks/evidence/TASK-016/history-three-readings-before-spacing-fix.png)、[最终三条](../tasks/evidence/TASK-016/history-three-readings.png)、[最终清空](../tasks/evidence/TASK-016/history-cleared.png)。均为历史窗口内容渲染，不是本轮新运行。

早期审查也退出 0，因为当时工具退出码只看运行异常；它不代表显示已通过。最终核对另检查显示/业务结果。先前整理对照历史 35 项哈希，34 项一致，只有已推进 TASK-017 的 App.xaml 不同。

Q1/Q2 只按骨架中的字段绑定/逐条数据源和路径修正/Margin 应用已掌握；外层模板/控件/业务由教练提供，不扩展为完整模板设计。旧 TODO 注释不是当前未完成证据，历史失败已经修复。原答疑见 [数据模板问答](../questions/wpf-templates.md)，框架查阅 [Microsoft 数据模板](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/data-templating-overview)。下一篇学习 [按钮里面怎样画出来](./TASK-017-wpf-controltemplate.md)。
