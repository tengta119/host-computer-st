# TASK-024：26.5 度为什么画在 y=110，28 度为什么画在 y=80

> 2026-10-14 整理。先逐步算一条真实温度，再把计算与原数组、绘图和标签连接。当前源码的中间温度已经是 28；26.5 是保留在历史证据中的修改前版本。

## 1. 先指向图里的中间点

图中有三次温度，横坐标固定在 100、280、460。原来是 24、26.5、25 °C；当前中间一条已改为 28 °C。

温度变高，中间点应该上移；两端温度没有变化，所以两端位置应保持。横坐标也没有改，不能为了让中间点上移而改 x。

这道题重点是：把一条原始温度换成图里的纵坐标 y。同一个方法接收 24，就算 24 的位置；接收 28，就算 28 的位置，不能总返回一个固定数。

## 2. 图的上下边界是计算的依据

当前范围有两条确定关系：20 °C 对应下边 y=240，30 °C 对应上边 y=40。

Canvas 是 WPF 的绘图容器，原点在它左上角，x 向右增大，y 向下增大。因此“点向上移动”对应 y 变小。

从 y=240 到 y=40 相差 200 个 WPF 长度单位，对应温度从 20 到 30 相差 10 度。每升高 1 度，点就向上移动 200÷10=20 个单位。

这些长度是 WPF 的界面单位，不必等同屏幕物理像素。计算依据来自当前固定绘图区，而不是摄氏度和像素之间天然存在的比例。

## 3. 第一小步：26.5 比最低温度高几度

```csharp
double degreesAboveMinimum = temperature - 20;
```

temperature 是本次传进来的温度。传入 26.5 时，这一步得到 6.5，意思是“比 20 度高 6.5 度”。

double 是 C# 的小数数值类型；degreesAboveMinimum 是自定义局部变量名，用于保存当前方法这次计算的温差。

6.5 还不是画图要用的 y。原实践曾先返回温差，让学员观察不同参数产生不同结果；那是理解参数的中间步骤，不能把这一阶段当成最终坐标。

## 4. 第二小步：6.5 度对应多大的上移距离

```csharp
double upwardDistance = degreesAboveMinimum * 20;
```

每度对应 20 个界面单位，6.5×20=130。因此点应该从下边界向上移动 130。

upwardDistance 是另一个自定义局部变量名，保存距离 130。它也还不是最终 y：这里算的是“从底部走多远”，尚未算“在 Canvas 中位于哪里”。

如果把温差直接当坐标，或者把距离直接当坐标，就跳过了下一步从底部定位的过程。

## 5. 第三小步：从底部 y=240 扣除上移距离

```csharp
double y = 240 - upwardDistance;
return y;
```

从下边界 240 向上走 130，最终 y=110。这里用减法，是因为 Canvas 的 y 向下增加；向上走，y 应减小。

return 是 C# 返回语句，把这次算出的 110 交回调用这个方法的地方。它不会自己修改蓝线，也不会自动修改温度数组。

三步完整地对应：温差 6.5 度 → 上移 130 单位 → 最终纵坐标 110。把三个数分别认清，比背一行合并公式更容易检查错误。

## 6. 把三步放进一个普通方法

当前源码的完整方法是：

```csharp
private double ConvertTemperatureToY(double temperature)
{
    double degreesAboveMinimum = temperature - 20;
    double upwardDistance = degreesAboveMinimum * 20;
    double y = 240 - upwardDistance;
    return y;
}
```

ConvertTemperatureToY 是项目自定义的方法名，不是 WPF 提供的接口。private 表示它供这个窗口类内部使用；方法名前的 double 表示它会返回一个小数。

括号里的 double temperature 声明一个参数：调用者给一个数，temperature 在本次调用中接收这个数。Java 普通方法也有“传参、执行方法体、取得返回值”的过程，这里可以按同一思路理解。

例如下面这段说明示例：

```csharp
double y = ConvertTemperatureToY(28);
```

先把 28 传进去，方法算出 80 并返回，再把 80 赋给调用处的 y。方法内部的 y 和调用处的 y 是各自声明的局部变量，名字相同不意味着它们是同一个变量。

## 7. 用边界和中点检查方向、比例和起点

| 传入温度 | 比 20 高几度 | 向上移动距离 | 最终 y |
| :--- | ---: | ---: | ---: |
| 20 | 0 | 0 | 240 |
| 24 | 4 | 80 | 160 |
| 25 | 5 | 100 | 140 |
| 26.5 | 6.5 | 130 | 110 |
| 28 | 8 | 160 | 80 |
| 30 | 10 | 200 | 40 |

20 落在下边，30 落在上边，25 落在中间 y=140。这里“中间”是 (240+40)÷2，不是 200÷2。

26.5 到 28 升高 1.5 度，应上移 1.5×20=30，y 从 110 减到 80。这一变化同时检查了方向与比例。

原历史还直接调用方法检查 20.25→235、29.75→45，表明这份实现使用传入小数进行计算，并非只为三条样例预填答案。

## 8. 原始温度、固定 x 和绘图坐标分别保存什么

```csharp
private readonly double[] _temperatures = new double[] { 24, 28, 25 };
private readonly double[] _xPositions = new double[] { 100, 280, 460 };
```

double[] 是保存多个小数的数组。_temperatures 保存原始温度，当前第二项为 28；_xPositions 保存三个水平位置。

new double[] 创建数组；花括号给出初始内容；下标从 0 开始，所以第二条温度写作 _temperatures[1]。

readonly 限制这些字段在正常方法中改指向另一份数组，不表示数组内容永远不能改。此次实际做的是改源码中的初始数值、重新编译运行，并不是运行时修改 readonly 字段指向。

两个数组在当前绘图循环里按同一下标配对。温度不会被 y 覆盖，换算得到的坐标放入另一份 PointCollection。

## 9. 循环怎样把一条温度变成一个坐标点

当前 DrawTemperatureLine 中的核心片段：

```csharp
PointCollection points = new PointCollection();
for (int i = 0; i < _temperatures.Length; i++)
{
    double x = _xPositions[i];
    double y = ConvertTemperatureToY(_temperatures[i]);
    points.Add(new Point(x, y));
}
TemperatureLine.Points = points;
```

for 是 C# 循环，i 从 0 开始，在小于数组 Length 时继续，每轮结束 i 增加 1。当前三轮依次处理下标 0、1、2。

第一轮取 x=100、温度 24，方法返回 y=160；new Point(100,160) 创建一个 WPF 坐标点，再加入 points。第二轮得到 (280,80)，第三轮得到 (460,140)。

Point 是 WPF 提供的坐标类型，PointCollection 是保存这些点的集合。最后 TemperatureLine.Points=points 才把三点交给 XAML 中那条 Polyline 蓝线，由蓝线按加入顺序连接。

方法计算、创建坐标、加入集合、交给蓝线是四个动作。仅算出 y，却没有把 points 交回蓝线，屏幕不会因此更新成新折线。

## 10. 标签显示原温度，下面的观察文字显示计算结果

```csharp
SecondReadingLabel.Text = "第 2 次：" + _temperatures[1] + " °C";
```

SecondReadingLabel 是 XAML 中第二个标签的自定义名字，Text 是 WPF 的文字属性。它读原始温度数组，因此当前显示“第 2 次：28 °C”，不是“80 °C”。80 是绘图坐标，不是温度。

另一个 ConversionResults TextBlock 用于展示“28 → 80”这类关系。后台 ShowConversionResults 会实际调用同一个 ConvertTemperatureToY，拼接返回值；没有把预期答案写死成观察文字。

字符串里的 \n 是 C# 换行写法，让样例结果和边界/中点分开显示。箭头左边是原温度，右边才是方法返回的 y。

蓝线位置、原值标签和计算结果来自同一组输入，但各自显示的内容不同。检查时要同时看它们，避免只把标签改成 28、线还停在旧位置。

## 11. 为什么三个显示步骤在构造方法中依次调用

```csharp
InitializeComponent();
ShowReadingLabels();
DrawTemperatureLine();
ShowConversionResults();
```

InitializeComponent 是 WPF 根据 XAML 生成的初始化连接，先建立蓝线和文字控件，后面的代码才能访问它们。

后面三个是项目自定义普通方法，分别设置原值标签、计算并交给蓝线、显示换算结果。它们在窗口创建时执行，当前数组在这之前已准备好。

这题仍是启动后展示三条固定样例，尚未把集合变化接到重画。想点按钮不断追加并刷新，是下一篇 TASK-025 的内容。

## 12. 只改中间温度时，什么应该变、什么应保持

修改前保存的三点为 (100,160)、(280,110)、(460,140)，标签为 24、26.5、25 °C。

只把数组第二项从 26.5 改为 28，最新三点为 (100,160)、(280,80)、(460,140)，标签为 24、28、25 °C。中间点向上 30，左右两段线的斜率随之变化。

左右端点、三个 x 和绘图区范围都保持；不能因为只改一个温度，就手工移动折线坐标或改刻度来配合。当前代码通过同一计算方法自然得到变化。

TASK-018 的 28 度对比曾由教练执行；TASK-024 这次有学员自主修改中间原始温度并确认实际观察的证据，两者不能混作同一次自主实践。

## 13. 常见错位从哪一小步查

高温点反而更低，检查最后是否把上移距离加到了底部；点整体偏一段，检查底部起点是不是 240；上下距离比例不对，检查每度是不是 20，而不是直接乘 200。

三点全在同一高度，检查方法是否仍固定返回 140；标签显示 80 °C，检查是否把返回坐标当成原温度；中间标签变了、线没变，检查是否真正使用最新编译窗口，以及蓝线 Points 是否来自同一数组计算。

本题只讨论 20～30 °C 的固定范围。方法对超范围数值仍会按相同算式返回范围外的 y；当前没有加入裁剪、自动缩放或异常提示，不能把这些功能当成已经完成。

当前提示与部分 TODO 还保留原首步安排，不代表最终实现仍未完成。现在方法返回的是最终 y，源码中间温度为 28。本轮没有改源码或重跑；现有 App.xaml 启动 TASK-025。

## 文末查阅：真实文件、前后证据与掌握范围

实际文件：[绘图区与观察文字](../../WpfBae/Exercises/TASK-024-temperature-coordinates/Task024Window.xaml)、[换算与绘图后台](../../WpfBae/Exercises/TASK-024-temperature-coordinates/Task024Window.xaml.cs)、[任务卡](../tasks/TASK-024.md)、[坐标与折线问答](../questions/wpf-charts.md)。

本题已通关。历史[原 26.5 版本](../tasks/evidence/TASK-024/results.json)和[修改为 28 的版本](../tasks/evidence/TASK-024/changed-28.json)各 12 项技术检查通过；[前后对比](../tasks/evidence/TASK-024/comparison.json)记录中间 y=110→80、上移 30、端点与 x 不变，以及学员实际观察确认。

计算结果正确、实际传参通过检查，与“学员独立编写完整换算方法”是不同证据。本题明确增加的自主验证是：在现有骨架中修改中间原始温度，实际比较显示变化。数组、绘图循环、标签与观察工具复用教练提供的结构；坐标问答 Q1/Q2 整体继续待复习，不因笔记整理升级。

固定 Canvas、Point、Polyline 的补充解释见 [TASK-018](./TASK-018-wpf-temperature-polyline.md)；新增与清空后重画见 [TASK-025](./TASK-025-wpf-live-temperature-line.md)。官方查阅：[WPF 基础图形](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview)。
