# WPF 坐标与折线

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：TASK-018 的中间点怎么移动，温度为什么不能直接当 y？

- 日期：2026-10-13
- 状态：待复习
- 来源：TASK-018 启动必要讲解。已有窗口实践证据，Canvas/Polyline 尚无自主应用证据。

TASK-018 的蓝线叫 TemperatureLine，是 WPF 的 Polyline；ChartCanvas 是包住它的 WPF Canvas。两个名字来自教练。Polyline 根据收到的坐标按顺序连线，Canvas 提供放置空间，名字本身不把温度变成坐标。

Canvas 左上角是(0,0)，向右 x 增加，向下 y 增加。先看原占位方法中的一点：

```csharp
points.Add(new Point(280, 180));
```

Point 是 WPF 使用的坐标类型，new Point 创建 x=280/y=180 的坐标。points 是该方法创建的 PointCollection，Add 把坐标加入它。原首步只把 180 改为 100，点就上移 80，两端没有改；这是认识位置，不是完整温度计算。

希望温度越高点越上，需要给高温较小的 y。本题固定 y=240 对应 20 度，y=40 对应 30 度：10 度覆盖 200 个界面单位，一度上移 20。计算可读成：

```csharp
double y = 240 - (temperature - 20) * 20;
```

这是已完成转换关系的说明：temperature 是本次那条温度，先减 20 看比底部高多少度，再乘 20 得到上移距离，从底部 240 减去。24 度得到 160，26.5 度得到 110，28 度得到 80。直接拿温度当 y 会让高温向下走，也不符合本题范围。

ConvertTemperatureToY 是自定义普通方法，接温度、返回 y；DrawTemperatureLine 是自定义绘图方法，逐次取 x/温度、创建 Point 加入集合，最后 `TemperatureLine.Points = points` 设置这条蓝线的坐标属性。Points 接坐标集合，不接温度数组。

当前原代码已完成，见 [TASK-018 后台](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml.cs)。旧“返回固定 140/占位水平线”的说法是启动骨架历史，本次不改回去。

### TASK-024：先观察传入温度，再逐步算出 y

2026-10-14 启动补充，沿用 Q1，状态继续待复习。本题新增[后台](../../WpfBae/Exercises/TASK-024-temperature-coordinates/Task024Window.xaml.cs)与[窗口](../../WpfBae/Exercises/TASK-024-temperature-coordinates/Task024Window.xaml)，旧 TASK-018 作品未修改。

新窗口里的 TemperatureLine 仍是那条蓝线。它的中间点要表示26.5度：底部y=240代表20度，顶部y=40代表30度，所以升高10度需要上移200个界面单位。拆开看这一条温度：

1. 26.5减20，得到高出的6.5度。
2. 每度对应200除10，即20个界面单位；6.5度对应上移130。
3. 从底部240扣去130，最终y为110。Canvas的y向下增加，上移会让y减小。

先只做第一件事。在 ConvertTemperatureToY 里把占位返回改为：

```csharp
return temperature - 20;
```

`return` 是 C# 的返回语句；`temperature` 是这个普通方法的参数名字，接收调用时给它的温度。方法名 ConvertTemperatureToY 是教练自行命名，不是 WPF 功能。声明中的 `double` 表示返回一个可带小数的数值。

绘图循环里已有这一行，由骨架提供：

```csharp
double y = ConvertTemperatureToY(_temperatures[i]);
```

`_temperatures` 是保存24、26.5、25的原始数组；`i` 依次为0、1、2，第一轮取到24。调用把24交给参数temperature，上面的return返回4，左侧局部变量y收到4。后两轮同样收到6.5和5。方法现在名字里有Y，但计算尚未完成，返回的是温差；名字不会自动做转换。

窗口下方的 ConversionResults 是教练命名的 WPF TextBlock。ShowConversionResults 是教练新增的普通方法，分别调用同一转换方法，再把结果写入它的Text属性。首步重新运行后，样例那一行应显示24→4、26.5→6.5、25→5；边界那一行应显示20→0、25→5、30→10。当前固定占位会全部显示140；这些预期值是供学员核对的目标，本轮未运行窗口验证。

这一步只看返回值。后续再用普通局部变量逐步计算上移距离、从底部扣除距离并返回最终y。骨架中的 `points.Add(new Point(x, y))` 把当前x/y建成坐标并加入PointCollection，`TemperatureLine.Points = points` 把整份坐标集合交给蓝线；调用计算方法、保存返回值、设置蓝线坐标是三个动作。下方原温度标签仍直接读取数组，不使用y。数组/绘图循环/标签连接由教练复用，不计本题新自主设计。

首步操作与运行方式见[任务卡](../tasks/TASK-024.md)。本轮只构建成功，未运行或验收；TASK-018已验证的连接应用范围保留，温度换算与自主28度比较仍待实践。

## Q2：蓝线的三个点为什么正好对应下方三个 TextBlock？

- 日期：2026-10-13
- 状态：待复习
- 来源：学员询问 Polyline 与三个文字标签的位置对应关系；本轮静态核对当前 XAML/后台源码，未运行或验收。

三个标签的中心与三个点的 x 相同，因为骨架手动算了位置，不是名字自动匹配。先看实际第一个标签：

```xml
<TextBlock x:Name="FirstReadingLabel" Canvas.Left="55" Canvas.Top="260"
           Width="90" TextAlignment="Center" />
```

TextBlock 是 WPF 文字控件，FirstReadingLabel 是教练命名。Canvas.Left=55 放它的左边到 x=55；Width=90 使右边到 145，中心是 55+90/2=100。TextAlignment=Center 只让文字在这 90 宽的框内居中，不负责把整个框移到点下方。

| 标签 | 点的 x | 文字框左边 | 中心计算 |
| :--- | ---: | ---: | :--- |
| 第一条 | 100 | 55 | 55+45=100 |
| 第二条 | 280 | 235 | 235+45=280 |
| 第三条 | 460 | 415 | 415+45=460 |

Canvas.Top=260 设置文字框上边的 y，绘图区底部 y=240，所以文字在下方。蓝线点的 y 随温度改变，标签 y 固定；“对应”指横向中心，不表示两者高度相同。

两者在同一 Canvas 内，蓝线无额外偏移，采用同一起点。若单独把第一点 x 改为 120，标签中心仍 100；要继续对齐，文字框左边要设为 120-45=75。这个演算只是解释，不要求修改已完成作品，见 [原XAML](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml)。

## Q3：TASK-025点击追加后为什么蓝线不会自动变化，怎样先接通集合通知？

- 日期：2026-10-14
- 状态：待复习
- 来源：TASK-025启动前的必要讲解；已有Add/Clear更新绑定列表的证据，手工连接集合通知与折线尚待实践。
- 当前代码：[后台](../../WpfBae/Exercises/TASK-025-live-temperature-line/Task025Window.xaml.cs)、[窗口](../../WpfBae/Exercises/TASK-025-live-temperature-line/Task025Window.xaml)。

### 先找到保存温度的对象

点“添加下一条”后，AddButton_Click先取到24度，再执行 `_temperatures.Add(temperature)`。这个按钮方法由教练提供；下一次取26.5，然后25、28，接着循环。清空按钮调用同一集合的Clear，并把样例顺序恢复到首条24。

```csharp
private readonly ObservableCollection<double> _temperatures = new ObservableCollection<double>();
```

ObservableCollection来自.NET，不是教练自建类；double是C#的小数数值类型，尖括号这里规定每项是一条小数温度。new创建空集合，_temperatures是教练命名的字段，保存这一个集合的引用。readonly限制字段改指另一个集合，仍允许向原集合Add或Clear。

窗口里的TemperatureLine是WPF的Polyline，保存它所收到的坐标。温度集合保存24这样的原温度，蓝线的Points接收(100,160)这样的坐标；它们是两份不同用途的集合。温度新增后，不会自行完成换算并交给蓝线。

### 谁通知谁：先只更新条数

ObservableCollection的Add/Clear会发CollectionChanged通知。这个事件由.NET提供；窗口要先登记一个接收方法。先看骨架里这个普通方法：

```csharp
private void Temperatures_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
{
    // TODO 1b：设置条数文字。
}
```

Temperatures_CollectionChanged是教练自行命名的C#方法，不是框架按名字自动找到的方法。void表示不返回结果。sender在这次调用里收到发通知的温度集合；object?允许它为空，?是C#的可空标注。e收到本次集合变化的说明，NotifyCollectionChangedEventArgs来自.NET。本题重读集合即可，不要求先分析e。

这个事件要求接收方法具有上述两个参数、返回void，这个要求由.NET已有的NotifyCollectionChangedEventHandler委托类型规定。委托在这里保存待调用的方法引用；CollectionChanged才是事件名，二者身份不同。无需自行声明新delegate或event。[微软事件定义](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1.collectionchanged?view=net-10.0)、[接收方法参数要求](https://learn.microsoft.com/en-us/dotnet/api/system.collections.specialized.notifycollectionchangedeventhandler?view=net-10.0)。

在构造方法里，InitializeComponent已经创建了XAML中的控件；之后的TODO 1a填写：

```csharp
_temperatures.CollectionChanged += Temperatures_CollectionChanged;
```

左侧是“这一个温度集合的变化事件”，右侧是“这一个窗口的接收方法”。+=是C#登记写法；右侧不带调用括号，此时没有执行接收方法。后续Add完成集合新增并发通知，才会调用它。可以类比Java给当前对象登记监听器；本题通知在按钮操作时发生，不另起后台线程。

需要把保存的方法引用看得更明确时，同一登记也可以写成：

```csharp
_temperatures.CollectionChanged += new NotifyCollectionChangedEventHandler(Temperatures_CollectionChanged);
```

new创建一个引用该接收方法的委托，+=将其登记到事件上。两种写法选一种，不能一起写，否则会登记两次。首步使用前面的普通方法名写法即可，不需要Lambda或Invoke。

然后在接收方法的TODO 1b填写：

```csharp
ReadingCountText.Text = "完整历史条数：" + _temperatures.Count;
```

ReadingCountText指向XAML里教练命名的TextBlock文字控件；Text是WPF提供的文字属性。Count是集合当前条数，字符串加数值拼成“完整历史条数：1”。集合新增后调用这里，窗口便显示新条数；Clear后这里读到0。Count统计完整集合，不是图里画了几个点。

运行项目，追加两次再清空，预期计数0→1→2→0。首步图为空，因为绘图还留TODO。若计数不变，先核对登记是否放在InitializeComponent之后、方法名是否一致、文字赋值是否在接收方法里。实际运行方式见[任务卡](../tasks/TASK-025.md)；此处是预期说明，本轮未运行验证。

### 计数接通后，再从完整历史挑最新三条

四条历史为24、26.5、25、28，下标分别0、1、2、3。图只有三个横向位置，所以读下标1、2、3，显示26.5、25、28；原集合仍保留四条，不能为了画三条去删除24。

| 原历史下标 | 原温度 | 图中槽位slot | x | 换算后的y |
| :--- | ---: | ---: | ---: | ---: |
| 1 | 26.5 | 0 | 100 | 110 |
| 2 | 25 | 1 | 280 | 140 |
| 3 | 28 | 2 | 460 | 80 |

startIndex是学员要在绘图方法里声明的局部变量，表示从原集合哪一条开始读：最多三条时从0开始；超过三条时，从Count减3开始。用普通if拆开这两种情况。这里Count=4，开始下标为1。

普通for每轮读取一个历史下标；另一个局部变量slot从0开始，依次选择_xPositions中的100、280、460。历史下标用来取温度，slot用来选横向位置，两者在第四条时已经不同。不要直接拿历史下标去读只有三个元素的_xPositions。

points是在DrawTemperatureLine里创建的WPF PointCollection，专门装本次图的坐标。每轮把原温度交给已经提供的ConvertTemperatureToY，返回数值保存为y；用x/y创建WPF Point，再加进points。随后调用教练提供的ShowReadingLabel(slot, 历史下标, 原温度)，让对应标签显示同一条历史的编号和温度，不显示y。标签位置和单点圆点工具已提供，不计自主设计。

循环完后，把整份points赋给TemperatureLine.Points，蓝线才使用新坐标。零条时循环不执行，但仍须交给蓝线这份空坐标集合；提前return会留下上一次的线。骨架先清旧标签，并用ShowSinglePoint隐藏旧圆点或显示新单点，所以不需要自行修改Canvas的全部子对象；背景/刻度仍保留。

最后才在集合变化方法的TODO 4调用DrawTemperatureLine()。这是C#普通方法调用，带括号会执行绘图。新增与清空都通过集合变化走同一个方法，不需要在每个按钮各补一次绘图。

### 已提供的关闭处理

后台的OnClosed由教练提供，WPF在窗口关闭时调用它。override是C#重写写法，让当前窗口补充关闭动作；base.OnClosed(e)保留WPF原来的关闭处理。中间的 `_temperatures.CollectionChanged -= Temperatures_CollectionChanged;` 用-=移除原集合上同一接收方法的登记。这里解除窗口已不需要的通知关系，不会清空历史，也不是绘图步骤；本题不要求自行设计生命周期。

### 审查发现：第四次添加后计数为什么回到0？

2026-10-14实际窗口中，第四条最新三点/标签正确，完整历史集合Count是4，但文字显示0。当前文字拼接_nextSampleIndex；这个字段指向四个循环样例中下一项的位置，添加第四条之前就从4重置为0。它描述下一次取哪条样例，不描述已保存多少条历史。

_temperatures.Count读取原温度集合中实际元素个数，四次Add且未Clear时就是4。计数文字应读取这个Count，样例下标的循环仍保留；绘图取最新三条也不删除旧历史。因此要分开看“样例位置”“完整历史条数”“图中最多三个位置”。修复只改计数读取来源，重新运行连续追加四次，文字应1、2、3、4，第四次图仍为26.5/25/28。本轮没有代改源码，Q3保持待复习，证据见[审查结果](../tasks/evidence/TASK-025/results.json)。

## 历史证据与掌握范围

2026-10-14 TASK-025最终复查：学员自主修正计数读取集合Count后，9个实际状态/17项技术检查全部通过，三个DoD通关。新增自主验证范围限本骨架内真实历史计数读取和集合事件处理方法调用绘图；完整绘图循环/坐标机制与独立设计不自动算掌握，Q3整体仍待复习。第四/五条显示4/5，最新三点和标签正确，Clear与恢复24度通过，审查未代改源码。最新证据见[复查](../tasks/evidence/TASK-025/recheck-01/results.json)，下方计数失败为首次历史，不要求恢复错误字段。

2026-10-14 TASK-025审查：最新三条/标签、单点/连线、原集合/事件刷新、清空恢复实际通过，计数一致性失败；DoD1/2未通过、3通过。只确认当前运行行为，不扩大学员独立编写/完整机制的掌握范围，Q3仍待复习。详见任务卡，以下未运行为启动历史。

2026-10-14 TASK-025启动：Q3新增为待复习；独立窗口/同集合/按钮/转换/标签与单点/关闭处理由教练提供，集合通知接线和重建折线留TODO。实际构建0警告/错误，未运行窗口或验收；不因新讲解、新骨架扩大既有掌握范围。

2026-10-14 TASK-024三项DoD已通过。原26.5与最新28两个版本均实际Show/加载，各12项技术检查通过；三点由(100,160)/(280,110)/(460,140)变为(100,160)/(280,80)/(460,140)，标签相应变化，两端/x保持。学员仅修改原始中间温度，并确认已运行观察。新增自主验证范围限本骨架内修改原始数据、比较点与标签变化；现有计算运行正确不单独证明自主编写或完整理解，Q1/Q2整体仍待复习。当前源码保留28，启动步骤中的固定140/中间结果是历史，不要求恢复。证据见[TASK-024审查](../tasks/TASK-024.md)与[前后对照](../tasks/evidence/TASK-024/comparison.json)。

Q1/Q2 均保持待复习。2026-10-13 TASK-018 原窗口三个坐标(100,160)/(280,110)/(460,140)已验证；教练独立 28 度副本中间 y 变 80、上移 30，两端不变、标签可读。学员自主补循环加点、Points 赋值与调用，局部应用有效；转换计算/标签布局由既有实现提供，不计独立设计。原任务三项 DoD 通关，本轮未运行或扩大学员掌握范围。

相关任务卡与验收来源：[TASK-018](../tasks/TASK-018.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-charts.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft WPF 基本图形说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview)
- [Canvas 说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/canvas)
