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

## 历史证据与掌握范围

Q1/Q2 均保持待复习。2026-10-13 TASK-018 原窗口三个坐标(100,160)/(280,110)/(460,140)已验证；教练独立 28 度副本中间 y 变 80、上移 30，两端不变、标签可读。学员自主补循环加点、Points 赋值与调用，局部应用有效；转换计算/标签布局由既有实现提供，不计独立设计。原任务三项 DoD 通关，本轮未运行或扩大学员掌握范围。

相关任务卡与验收来源：[TASK-018](../tasks/TASK-018.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/wpf-charts.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft WPF 基本图形说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview)
- [Canvas 说明](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/canvas)
