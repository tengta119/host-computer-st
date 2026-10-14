# TASK-025：点击追加温度后，条数、最新三点和标签怎样一起刷新

> 2026-10-14 整理。按“点按钮 → 集合变化 → 更新计数 → 重算最新三条 → 交给蓝线”逐步解释当前已完成代码。首次计数问题和最终通过证据在文末。

## 1. 先看连续点击会发生什么

刚打开窗口时，完整历史条数是 0，没有蓝线或蓝色单点。点“添加下一条”，依次追加 24、26.5、25、28 °C，再点又从 24 开始提供样例。

点到第四次，完整历史有 4 条，但图里只显示最新 3 条：第 2 条 26.5、第 3 条 25、第 4 条 28。第一条仍保存在集合中，只是不在这次图的三个位置里显示。

点“清空”，完整历史回到 0，蓝线、蓝点、原值标签消失；再次添加从 24 开始。背景范围框、刻度和说明文字还在，它们并不是要被清除的历史曲线。

这里的“刷新”是按钮驱动的，没有定时器、串口采集或后台线程。窗口使用普通 Click 事件，没有把本题按钮接成前面几题的 ICommand。

## 2. 先分清三份数值和一个下标

```csharp
private readonly ObservableCollection<double> _temperatures =
    new ObservableCollection<double>();
private readonly double[] _sampleTemperatures = new double[] { 24, 26.5, 25, 28 };
private readonly double[] _xPositions = new double[] { 100, 280, 460 };
private int _nextSampleIndex = 0;
```

_temperatures 保存这次运行中已经添加的全部温度，初始为空。ObservableCollection<T> 是 .NET 提供的集合，double 表示每项只是一条温度小数；这题没有用前面包含设备 ID、Value、Unit 的 Reading 对象。

_sampleTemperatures 是提供下一条样例的四项数组，它一直只有这四个样例，不等于完整历史。_xPositions 是图上的三个水平位置，也不是历史记录。

_nextSampleIndex 是自定义整数字段，用来表示下次从样例数组哪个位置取值。C# 数组下标从 0 开始，当前会循环使用 0、1、2、3。

readonly 让这些集合/数组字段在普通方法里不能改指向另一份对象；它不禁止对原集合 Add/Clear。保持同一个 _temperatures，能让已登记的集合通知持续作用在同一对象上。

## 3. 第一次添加，按钮实际做了什么

当前 AddButton_Click 的方法体：

```csharp
double temperature = _sampleTemperatures[_nextSampleIndex];
_nextSampleIndex = _nextSampleIndex + 1;
if (_nextSampleIndex == _sampleTemperatures.Length)
{
    _nextSampleIndex = 0;
}
_temperatures.Add(temperature);
```

第一次进入时下标为 0，先取到 24，保存在本次局部变量 temperature；下标随后增加为 1，表示下次准备取 26.5；最后 Add(24) 把这条温度加入完整历史。

到第四次，先取下标 3 的 28，再把下标增加到 4。样例数组 Length=4，有效下标只有 0～3，所以 if 把下次下标改回 0。

本次局部变量仍是已取出的 28，不会因为下标回到 0 就变成 24。第五次进入时才会再次取到 24。

这个方法只追加集合，没有直接改条数或蓝线。后面的统一通知处理负责那些显示更新。

## 4. XAML 的 Click 怎样接到这个普通方法

```xml
<Button x:Name="AddButton" Content="添加下一条" Click="AddButton_Click" />
<Button x:Name="ClearButton" Content="清空" Click="ClearButton_Click" />
```

这段仅摘出按钮的关键连接，当前 XAML 还包含 Padding、Margin 等显示设置。Button、Content、Click 是 WPF 的控件与属性/事件；AddButton_Click、ClearButton_Click 是项目自行命名的方法。

Click 后面的方法名告诉 WPF：按钮被点击时执行哪个方法。它与后面“集合变化时执行哪个方法”的登记，是两条不同连接。

按钮方法里的 object sender、RoutedEventArgs e 是点击事件要求的参数。当前方法不需要从按钮读取温度，直接从样例数组取值。

## 5. 集合 Add 之后，为什么有机会统一更新界面

_temperatures 是 ObservableCollection<double>。向它 Add 一项、Clear 全部等操作时，它会发出 CollectionChanged 事件，告诉已登记的处理方法“集合发生了变化”。

这不是说任意蓝线都会自动理解它。当前 Polyline 没有直接绑定这个温度集合，也不知道 26.5 应换成 y=110。我们仍要写代码接收变化、换算坐标、设置 Points。

想让新增和清空都走同一条更新路线，就把计数与重画集中到集合通知的处理方法里。这样两个按钮只管改变数据。

## 6. 用 += 登记收到通知时要执行的方法

构造方法的当前顺序：

```csharp
InitializeComponent();
_temperatures.CollectionChanged += Temperatures_CollectionChanged;
DrawTemperatureLine();
```

第一行先建立 XAML 中的控件。第二行给这份温度集合登记一个普通方法：以后它发 CollectionChanged 通知，就执行 Temperatures_CollectionChanged。

这里 += 是 C# 给事件增加处理方法的写法。右侧只有方法名，没有调用括号；登记时并不立即执行这个方法。该名字是项目自定义的，.NET 不会只凭名字自动发现它。

第三行有括号 DrawTemperatureLine()，这才是立即调用绘图方法，建立初始空图。初始“完整历史条数：0”由 XAML 设置；此次初始绘图不靠一次添加通知触发。

若从完整写法理解，下面是第二行的等价说明示例，二选一即可：

```csharp
_temperatures.CollectionChanged +=
    new NotifyCollectionChangedEventHandler(Temperatures_CollectionChanged);
```

NotifyCollectionChangedEventHandler 是 .NET 为这个事件提供的委托类型，用于描述可登记的方法参数形式。当前源码采用前面的普通简写，由 C# 完成对应转换；不要把两种写法都登记一次，否则同一处理方法会执行两次。

## 7. 通知方法为什么接收这两个参数

```csharp
private void Temperatures_CollectionChanged(
    object? sender, NotifyCollectionChangedEventArgs e)
{
    ReadingCountText.Text = "完整历史条数：" + _temperatures.Count;
    DrawTemperatureLine();
}
```

void 表示方法不返回数值。sender 接收发通知的对象，这里就是那份温度集合；object? 中的 ? 表示这个参数允许为空的标注，当前处理并不需要使用它。

NotifyCollectionChangedEventArgs 是 .NET 提供的变化说明类型，e 可以带来新增、清空等信息。本题每次都按集合当前内容完整重画，所以没有进一步分析 e。

ReadingCountText 是项目给条数 TextBlock 起的名字，Text 是它显示的字符串属性。先写入当前集合 Count，再调用 DrawTemperatureLine，这两行才是本题通知后的显示工作。

## 8. 完整历史条数必须读 Count

```csharp
ReadingCountText.Text = "完整历史条数：" + _temperatures.Count;
```

Count 是 .NET 集合的属性，表示现在实际保存了多少条。点四次就是 4，点五次就是 5；清空后就是 0。

不能用 _nextSampleIndex 代替。下表把实际两种数放在一起：

| 已添加次数 | 完整历史 Count | 下次样例下标 _nextSampleIndex |
| ---: | ---: | ---: |
| 0 | 0 | 0 |
| 1 | 1 | 1 |
| 2 | 2 | 2 |
| 3 | 3 | 3 |
| 4 | 4 | 0 |
| 5 | 5 | 1 |

前三次恰好一样，容易掩盖错误。第四次下标为了循环取样回到 0，但历史没有因此被清空。下标负责“下一条从哪取”，Count 负责“已经存了几条”。

首次检查正是在第四、五条发现计数显示 0、1；当前学员已改为 Count，最终复查通过。

## 9. 画最新三条之前，先算从历史哪里开始

```csharp
int startIndex = 0;
if (_temperatures.Count > 3)
{
    startIndex = _temperatures.Count - 3;
}
```

startIndex 是本次绘图的自定义局部变量，表示第一条要画的历史下标。0、1、2、3 条时，从下标 0 开始；4 条时从下标 1 开始；5 条时从下标 2 开始。

例如完整历史 [24,26.5,25,28] 有 4 条，Count-3=1，从下标 1 走到最后，正好取 26.5、25、28。

这里没有 Remove，也没有 Clear。第一条 24 仍保存在 _temperatures[0]，只是当前绘图跳过它。因此完整历史会继续增多，图中点数最多为 3。

## 10. 为什么历史下标 i 和图中位置 slot 要分开

有四条历史时，要取的历史下标是 1、2、3，但图上的三个位置数组只有下标 0、1、2。

如果直接用 _xPositions[i]，第一条可见点就会用到第二个水平位置；到 i=3 时还会超出三项数组的有效下标。

所以 i 负责在完整历史中找数据，slot 负责把这次选出的点从左到右放入三个位置。无论历史从下标几开始画，slot 总从 0 开始。

| 四条历史时 | i：历史下标 | 温度 | slot：图中位置下标 | x |
| :--- | ---: | ---: | ---: | ---: |
| 本次第一个点 | 1 | 26.5 | 0 | 100 |
| 本次第二个点 | 2 | 25 | 1 | 280 |
| 本次第三个点 | 3 | 28 | 2 | 460 |

## 11. 当前完整循环怎样连接数据、坐标与标签

```csharp
int slot = 0;
for (int i = startIndex; i < _temperatures.Count; i++)
{
    double temperature = _temperatures[i];
    double x = _xPositions[slot];
    double y = ConvertTemperatureToY(temperature);
    points.Add(new Point(x, y));
    ShowReadingLabel(slot, i, temperature);
    slot = slot + 1;
}
```

这是 DrawTemperatureLine 中的循环片段，points 已在循环前创建。for 从刚算出的 startIndex 开始，一直处理到当前完整历史最后一条。

每轮先取得原温度，再取当前图中位置的 x，把温度传入沿用 TASK-024 的换算方法，得到 y；new Point(x,y) 创建一个 WPF 坐标，再加到本次 points 集合。

ShowReadingLabel 是教练提供的自定义辅助方法，接收图中位置 slot、原历史下标 i 和原温度。处理完这一条，slot 增加 1，让下一条放到右边下一个位置。

当前计算保证最多选三条，所以 slot 只会用于 0、1、2。这里没有使用 LINQ 简写，普通 if 与 for 就能逐步看清每个数的用途。

## 12. 温度到 y 继续复用上一题

```csharp
double degreesAboveMinimum = temperature - 20;
double upwardDistance = degreesAboveMinimum * 20;
double y = 240 - upwardDistance;
return y;
```

这段是 ConvertTemperatureToY 方法体：20 度在底部 y=240，30 度在顶部 y=40，每升高 1 度向上 20 单位，所以从底部扣除上移距离。

当前 24、26.5、25、28 分别返回 160、110、140、80。数组和历史中仍保存原温度，不能用这些 y 覆盖原值。

TASK-025 没有引入新的自动缩放。绘图区和三个 x 固定，只是每次按最新历史重新选点。完整的三步计算见 [TASK-024](./TASK-024-wpf-temperature-coordinates.md)。

## 13. 标签保留原历史编号，而不是重新数成 1、2、3

辅助方法中先生成文字：

```csharp
string text = "第 " + (historyIndex + 1) + " 条：" + temperature + " °C";
```

historyIndex 是原完整历史的下标，从 0 开始；给人看的编号从 1 开始，所以加 1。temperature 是原温度，单位仍是 °C。

四条历史时，循环传入 i=1、2、3，三个标签应显示第 2、3、4 条。slot=0、1、2 只决定分别写 FirstReadingLabel、SecondReadingLabel、ThirdReadingLabel。

若用 slot+1 当历史编号，第四条以后图上始终写第 1、2、3 条，会让人误以为完整历史只有这三条或旧历史已被删掉。

## 14. 每次画图先清理旧标签，再建立本次坐标

DrawTemperatureLine 开头是：

```csharp
PointCollection points = new PointCollection();
ClearReadingLabels();
```

PointCollection 是 WPF 提供的坐标集合，points 是本次新建的局部变量。每次创建空集合，再加本次最多三个点，避免旧坐标被一直累加。

ClearReadingLabels 是提供的辅助方法，把三个标签 Text 都设成空字符串。然后循环只填写本次实际存在的标签。

这能处理从三点变成零点、再从一点开始的情况：没有数据的位置不会留着上一轮的“第 3 条”。清标签不等于清历史集合，只是先清这三个显示位置。

## 15. 空集合也必须交给蓝线，才能去掉旧图

```csharp
TemperatureLine.Points = points;
ShowSinglePoint(points);
```

循环结束后，TemperatureLine 指向 XAML 中那条 Polyline，Points 是它接收坐标集合的 WPF 属性。赋值后，蓝线使用这次新生成的点。

历史清空时 Count=0，循环一次也不执行，points 仍为空。仍然执行赋值，才会用空坐标集合替换原先三点。

如果在 Count=0 时直接 return，就可能让旧 Points 留在蓝线上，出现“条数 0，旧折线还在”的矛盾画面。

ShowSinglePoint 是另一段提供的显示工具，接下来专门处理只有一点时的可见性。

## 16. 一条温度为什么需要额外画一个蓝点

Polyline 连接相邻坐标形成线段。只有一个坐标时，没有两点可连接，不能指望像三点时那样看到一段蓝线。

当前 XAML 另外提供一个直径 8 的 Ellipse，名字为 SinglePointMarker。ShowSinglePoint 每次先隐藏它；仅当 points.Count==1 时显示，并把圆心对到该坐标。

```csharp
SinglePointMarker.Visibility = Visibility.Collapsed;
if (points.Count == 1)
{
    Point point = points[0];
    Canvas.SetLeft(SinglePointMarker, point.X - 4);
    Canvas.SetTop(SinglePointMarker, point.Y - 4);
    SinglePointMarker.Visibility = Visibility.Visible;
}
```

Canvas.SetLeft/SetTop 设置圆点左上角的位置；目标是圆心落在 point，所以各减去半径 4。第一条 24 的坐标为 (100,160)，圆点左上角应为 (96,156)。

Visibility.Collapsed 是 WPF 的隐藏状态，不是删除 Ellipse 对象；Visible 则显示。两点以上由蓝线表达，圆点隐藏；零点时两者都没有温度图形。

## 17. 清空按钮为什么同时重置取样下标

```csharp
_nextSampleIndex = 0;
_temperatures.Clear();
```

第一行让下次从样例数组首项 24 开始。第二行清空已经记录的全部历史，操作的还是同一个集合对象。

Clear 引发集合变化通知，处理方法写 Count=0、调用绘图；绘图清标签、设置空 Points、隐藏单点。因此清空按钮不必另写一份重复的清计数和清曲线代码。

对当前 ObservableCollection，即使已经为空，再调用 Clear 也会发出 Reset 通知，历史复查验证了这一情况。此时重画空图仍然得到同样结果。

重置下标和清历史解决的是两件事。若只清集合不重置下标，下次样例可能从上次剩余位置继续；若只重置下标不清集合，旧历史仍在。

## 18. 把四次、五次、清空后的实际数据展开

| 状态 | 完整历史 | 图中温度 | 图中坐标 | 标签编号 |
| :--- | :--- | :--- | :--- | :--- |
| 第一次添加 | 24 | 24 | (100,160)；显示单点 | 1 |
| 第二次添加 | 24,26.5 | 24,26.5 | (100,160)、(280,110) | 1,2 |
| 第三次添加 | 24,26.5,25 | 24,26.5,25 | (100,160)、(280,110)、(460,140) | 1,2,3 |
| 第四次添加 | 24,26.5,25,28 | 26.5,25,28 | (100,110)、(280,140)、(460,80) | 2,3,4 |
| 第五次添加 | 24,26.5,25,28,24 | 25,28,24 | (100,140)、(280,80)、(460,160) | 3,4,5 |
| 清空 | 空 | 空 | 空 Points，隐藏单点 | 全空 |
| 清空后再添 | 24 | 24 | (100,160)；显示单点 | 1 |

第四次之后，原第 2 条温度移动到图的左侧 x=100。当前三个 x 是“这次可见的三个位置”，并非固定代表历史第 1、2、3 条或真实时间戳。

这就是“保存完整历史、只画最新三条”的具体效果。Count 看完整历史，Points.Count 看本次图中点数，二者在第四条后不同。

## 19. 关闭窗口时 -= 做什么

```csharp
protected override void OnClosed(EventArgs e)
{
    _temperatures.CollectionChanged -= Temperatures_CollectionChanged;
    base.OnClosed(e);
}
```

OnClosed 是 WPF 窗口关闭时调用的方法。override 是 C# 重写语法，让这个窗口在原关闭处理过程中补充自己的动作；base.OnClosed(e) 保留 Window 原有处理。

-= 从同一份集合的事件中移除之前登记的同一个方法。这段由教练提供，用于配对结束通知连接，不把生命周期设计算成本题学员独立完成的核心。

不能仅凭这里有事件订阅，就断言当前程序一定发生内存泄漏。是否延长对象存活要看实际引用关系；本题只需认清 += 登记、-= 解除这一对具体动作。

## 20. 出问题时，按按钮到蓝线的顺序定位

点添加后集合 Count 不增加，先看 Click 是否接到方法、Add 是否执行；Count 增加但文字不变，检查集合事件 +=、处理方法里的 Text 赋值。

计数变化但折线不变，检查通知方法是否调用 DrawTemperatureLine，以及最后是否设置 TemperatureLine.Points；第四条报下标越界，检查 x 是否误用了历史下标 i；第四条计数归零，检查是否误用了循环取样下标。

清空后旧线还在，检查空集合时是否提前 return；有旧标签残留，检查每次绘图前是否 ClearReadingLabels；单条看不见，检查单点工具的可见性和坐标。

源码和窗口底部还保留启动阶段的 TODO 文字，不能只凭这些文字判断功能未完成。当前事件接线、Count、最新三条循环、Points 赋值与通知调用均已写入，最终复查通过。

## 文末查阅：实际文件、最新复查与掌握范围

实际文件：[按钮与绘图区](../../WpfBae/Exercises/TASK-025-live-temperature-line/Task025Window.xaml)、[集合、通知与绘图](../../WpfBae/Exercises/TASK-025-live-temperature-line/Task025Window.xaml.cs)、[任务卡](../tasks/TASK-025.md)、[坐标与折线问答 Q3](../questions/wpf-charts.md)。当前 [App.xaml](../../WpfBae/App.xaml) 指向本窗口，在工作区执行 `dotnet run --project WpfBae/WpfBae.csproj` 会按此入口打开；这是运行方式，本轮整理没有执行。

首次检查 17 项技术检查中 16 项通过，第四/五条计数错误为 0/1，实际集合是 4/5。学员修正为 Count 后，[最新 recheck-01 结果](../tasks/evidence/TASK-025/recheck-01/results.json) 的 9 个状态、17 项技术检查全部通过；[最终核对](../tasks/evidence/TASK-025/recheck-01/verification.json)保留源码与编译材料核对。[第四条历史图](../tasks/evidence/TASK-025/recheck-01/add-4.png)可以对照完整条数 4、图中编号 2/3/4。

九个状态包括初始、添加 1～5 次、清空、对空集合再次清空、恢复首条。集合通知累计对应 8 次按钮引起的操作，没有因为重复登记而倍增；同一完整历史集合、最新点数/坐标、标签、单点与关闭均有历史核对。

本题已通关。样例 Add/Clear、固定绘图区、转换、标签、单点和关闭工具由骨架提供；当前代码的事件连接、计数与最新三条绘图已通过运行核对，学员自主修正 Count 读取的证据明确。局部接线应用和正确运行不单独证明整段循环或完整图表均由学员自主编写；问答 Q3 继续待复习，未据此新增全部掌握结论。

TASK-026 目前只有未开始任务卡，尚未准备代码；本轮没有启动它。当前温度集合尚未同时接成完整历史 DataGrid，更没有完成真实设备连续采集。

官方查阅：[ObservableCollection 的集合变化事件](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1.collectionchanged?view=net-10.0)、[WPF 基础图形](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview)。
