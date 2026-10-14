# TASK-018：把三次温度画成一条折线

> 2026-10-14 整理。先看温度、位置和蓝线三个具体对象，再读换算与循环。任务已于 2026-10-13 通关；代码解释与历史验收分开，本轮未运行程序。

## 1. 这张图表示什么

窗口显示三次模拟温度：24、26.5、25 °C。从左往右表示第 1、2、3 次读取，点越高表示温度越高。

蓝线叫 TemperatureLine，是 WPF 的 Polyline。灰色背景所在的容器叫 ChartCanvas，是 WPF 的 Canvas。这两个名字是教练取的，不会仅凭名字自动读取温度。

程序先保存温度，再算它们在图中的位置，最后让蓝线连接这些位置。这一题没有读取真实设备、定时采集或追加按钮。

## 2. Canvas 的位置从左上开始

当前画布：

```xml
<Canvas x:Name="ChartCanvas" Width="560" Height="300"
        HorizontalAlignment="Left" Background="#F8FAFC">
    <!-- 背景、蓝线、刻度和标签 -->
</Canvas>
```

Canvas 是摆放内部元素的容器。左上角为 (0,0)，x 向右增大，y 向下增大。

Point(280,100) 比 Point(280,180) 上移 80，因为它们横向位置相同，后者距上边更远。这里用 WPF 的布局单位，不能无条件等同于显示器物理像素。

## 3. 灰色矩形规定本题画温度的范围

```xml
<Rectangle Canvas.Left="100" Canvas.Top="40"
           Width="360" Height="200"
           Fill="#EEF2F6" Stroke="#94A3B8" StrokeThickness="1" />
```

Rectangle 是 WPF 的矩形图形。它的左边为 x=100，上边为 y=40；宽 360，所以右边为 460；高 200，所以底边为 240。

Canvas.Left/Top 写在矩形上，由外面的 Canvas 读取并定位它。这种给父布局使用、写在子元素上的设置叫附加属性。

Fill 填背景，Stroke 画外框，StrokeThickness 指线宽。这个矩形只是图的背景，不保存温度，也不自动限制所有点只能在里面。

## 4. Polyline.Points 接收的是坐标

```xml
<Polyline x:Name="TemperatureLine"
          Stroke="#2563EB" StrokeThickness="3" />
```

Polyline 按 Points 中的先后顺序连线。Stroke 给当前线设置蓝色，StrokeThickness=3 设置线宽。

三点通常形成两段线。它要的是 (x,y) 坐标，不能直接把 24、26.5、25 的温度数组当作 Points。

当前蓝线没有额外设置 Canvas.Left/Top，因此后面的坐标直接按画布位置理解；若另给整条线位移，点会随它一起偏移。

## 5. 原始温度与横坐标分别存在哪里

当前两个 C# 数组：

```csharp
private readonly double[] _temperatures = new double[] { 24, 26.5, 25 };
private readonly double[] _xPositions = new double[] { 100, 280, 460 };
```

double 是可带小数的数值类型，double[] 是数组写法。new 创建数组，字段记住它。

第一组存温度，第二组存读取顺序的位置。下标从 0 开始：第 0 项对应第一次，第 1 项对应第二次，第 2 项对应第三次。

readonly 限制字段在初始化/构造之后换指向另一个数组，不冻结数组元素。当前任务通过修改源码中的中间数据并重新运行来比较，没有运行中编辑温度的界面。

## 6. 为什么不能直接把温度当作 y

本题 y=240 表示最低的 20 °C，y=40 表示最高的 30 °C。温度升高时，要让点更靠上，所以 y 必须减小。

若直接用 temperature 当 y，26.5 会比 24 更靠下；数值也不对应约定的灰色绘图区。

先算 24 °C：它比 20 高 4 度。10 度铺在 200 高的区域里，每度上移 20；4 度上移 80。底部 240 减 80，得到 y=160。

## 7. 普通方法怎样接温度、返回 y

当前方法：

```csharp
private double ConvertTemperatureToY(double temperature)
{
    double degreesAboveMinimum = temperature - 20;
    double upwardDistance = degreesAboveMinimum * 20;
    return 240 - upwardDistance;
}
```

ConvertTemperatureToY 是自定义方法名，不是 WPF API。参数 temperature 收到调用时给它的一个温度；方法前的 double 表示返回一个数值。

第一句算“比最低温度高多少度”，第二句算“这几度应上移多远”，return 把最终位置交回调用处。

| 传入温度 | 比 20 高多少 | 上移距离 | 返回 y |
| :--- | :--- | :--- | :--- |
| 20 | 0 | 0 | 240 |
| 24 | 4 | 80 | 160 |
| 25 | 5 | 100 | 140 |
| 26.5 | 6.5 | 130 | 110 |
| 28 | 8 | 160 | 80 |
| 30 | 10 | 200 | 40 |

这一方法当前没有截断超范围温度；样例都在 20～30 内。本题不要求动态范围或自动缩放。

## 8. Point 与 PointCollection 装的是位置

```csharp
PointCollection points = new PointCollection();
points.Add(new Point(100, 160));
```

Point 来自 System.Windows，表示一个坐标；PointCollection 来自 System.Windows.Media，装多个坐标。points 是当前方法的局部变量。

new Point(100,160) 建立一个点，Add 放入坐标集合。这份集合与 _temperatures 的用途不同：一个存画在哪里，一个存原温度是多少。

using 让 C# 可以用这些短类型名，它本身不画线。

## 9. for 循环把相同下标的温度和 x 配起来

当前绘图方法的关键部分：

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

int i=0 从第一项开始；i<Length 表示还有项目时继续；i++ 每轮增加 1。这次 i 依次为 0、1、2。

第一轮取 x=100、温度24，调用方法得到160，加入 (100,160)；第二轮加入 (280,110)；第三轮加入 (460,140)。两个数组在本题都恰有三项，按同一下标配对。

循环结束后才把整份 points 交给蓝线的 Points，蓝线按这个顺序连接。创建局部集合与把集合交给控件是两个动作。

## 10. 方法要被调用，窗口才会画新线

当前构造函数：

```csharp
InitializeComponent();
ShowReadingLabels();
DrawTemperatureLine();
```

InitializeComponent 创建 XAML 控件；ShowReadingLabels 填标签；DrawTemperatureLine 计算并设置蓝线。

三个名字中的后两个是自定义普通方法，写好方法体不会自动执行。当前构造时调用一次，之后没有集合事件或按钮让它自动重画。

旧 ShowPlaceholderLine 仍留在文件里，但构造函数已经不调用它。它里面的中间点是 (280,100)，属于当时单点观察，不代表当前实际温度折线用那个点。

## 11. 三个标签显示原温度，不显示 y

```csharp
SecondReadingLabel.Text = "第 2 次：" + _temperatures[1] + " °C";
```

标签读取原数组的中间温度，得到 26.5；它不会显示换算后的 110。

蓝线和标签分别设置。它们看起来对应，是因为同一次读取的温度和位置在代码里配好，不是控件名字自动匹配。

## 12. 标签为什么在点的下方

当前中间标签：

```xml
<TextBlock x:Name="SecondReadingLabel"
           Canvas.Left="235" Canvas.Top="260"
           Width="90" TextAlignment="Center" />
```

按声明宽度计算，标签中心 x=235+90/2=280，正好对着中间点。另两项 Left=55/415，理论中心为100/460。

Top=260 让标签位于绘图区底边240下方；点的 y 随温度变，标签的 y 不变。TextAlignment=Center 只让文字在标签内部居中，不负责移动整个标签。

## 13. 中间温度升到 28 时，哪几项会变

数组从24、26.5、25改为24、28、25并重新加载窗口后：

```text
第一点：(100,160) 保持
中间点：(280,110) → (280,80)，上移30
第三点：(460,140) 保持
中间标签：26.5 °C → 28 °C
```

横坐标数组没改，两个端点温度没改，所以它们的位置保持。需要同一转换方法使用输入参数，不能按“第几个样例”写死 y。

## 14. 按现象排查

| 现象 | 先核对 |
| :--- | :--- |
| 三个点总在同一高度 | 转换方法是否仍固定返回一个数 |
| 改温度却看不到变化 | 是否运行了新版本、是否实际调用绘图方法 |
| 越热越往下 | 是否直接把温度当 y，或上移时用了加法 |
| 标签变了，线不变 | Points 是否由温度转换后重新赋值 |
| 点与标签横向错开 | 标签 Left/Width 与点的 x、蓝线是否额外偏移 |

## 文末查阅：真实文件与历史范围

实际文件：[窗口](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml)、[后台](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml.cs)、[任务卡](../tasks/TASK-018.md)、[坐标问答 Q1/Q2](../questions/wpf-charts.md)。

当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-025。执行 `dotnet run --project WpfBae/WpfBae.csproj` 现在打开 TASK-025；复看旧题需明确临时入口并在观察后恢复，本轮没有切换或运行。

可选的小观察是仅把中间原温度改为28，按本题入口重跑，观察点与标签，再恢复；它是说明步骤，不是本轮新验证。

已读取本地 [原26.5结果](../tasks/evidence/TASK-018/original-26_5.json)与 [28度副本结果](../tasks/evidence/TASK-018/variant-28.json)：各自 passed=true，原点为 (100,160)/(280,110)/(460,140)，副本中间y为80。证据 [核对](../tasks/evidence/TASK-018/verification.json)、[原图](../tasks/evidence/TASK-018/original-26_5.png)、[副本图](../tasks/evidence/TASK-018/variant-28.png)保留。

这里28度副本由教练准备并运行，与 TASK-024 后来学员自主改28的证据不同。自主范围限循环加点、Points赋值和接通调用；转换/标签布局不因代码存在就算自主设计，Q1/Q2继续待复习。历史工具曾将标签中心误差定得过严，修正容差后通过，不记学员错误。

框架查阅：[Microsoft 基本图形](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview)。后续 [TASK-024](./TASK-024-wpf-temperature-coordinates.md)继续展开参数换算，[TASK-025](./TASK-025-wpf-live-temperature-line.md)学习追加后重画。
