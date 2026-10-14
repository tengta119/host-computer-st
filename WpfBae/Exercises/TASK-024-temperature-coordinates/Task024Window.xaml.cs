using System.Windows;
using System.Windows.Media;

namespace WpfBae.Exercises.Task024
{
    // 教练新建的窗口类，与同名 XAML 配对；Window 是 WPF 的窗口类。
    public partial class Task024Window : Window
    {
        // 以下数组、循环、标签连接均复用 TASK-018，不作为本题自主设计成果。
        // double[] 是 C# 数组写法；_temperatures 保存温度，_xPositions 保存 x。
        // TODO 3（实践 4 / DoD 3）：完成最终换算并保留原运行证据后，
        // 仅把中间 26.5 改成 28，重新运行比较。两个端点和横坐标不改。
        private readonly double[] _temperatures = new double[] { 24, 28, 25 };
        private readonly double[] _xPositions = new double[] { 100, 280, 460 };

        public Task024Window()
        {
            InitializeComponent();
            ShowReadingLabels();
            DrawTemperatureLine();
            ShowConversionResults();
        }

        // ConvertTemperatureToY 是自定义普通 C# 方法，不是 WPF API。
        // temperature 接收调用时传入的一条温度；double 表示返回一个小数数值。
        private double ConvertTemperatureToY(double temperature)
        {
            // 当前温度比最低温度 20 °C 高出多少度。
            double degreesAboveMinimum = temperature - 20;

            // 10 度覆盖 200 个界面单位，每升高 1 度向上移动 20。
            double upwardDistance = degreesAboveMinimum * 20;

            // 从底部 y=240 扣除上移距离，得到这个温度对应的 y。
            double y = 240 - upwardDistance;
            return y;
        }

        // 教练复用的绘图连接：取温度 -> 调用转换 -> 建坐标 -> 交给蓝线。
        private void DrawTemperatureLine()
        {
            // PointCollection 是 WPF 的坐标集合；points 是本次新建集合的变量。
            PointCollection points = new PointCollection();
            for (int i = 0; i < _temperatures.Length; i++)
            {
                double x = _xPositions[i];
                // 数组本轮的温度传给 temperature，方法返回的数值存进 y。
                double y = ConvertTemperatureToY(_temperatures[i]);
                // Point 是 WPF 的坐标；new Point(x, y) 创建本轮的点。
                points.Add(new Point(x, y));
            }

            // TemperatureLine 指向 XAML 蓝线；Points 是它接收坐标集合的 WPF 属性。
            TemperatureLine.Points = points;
        }

        // 教练提供：标签始终显示原始温度，不显示转换后的 y。
        private void ShowReadingLabels()
        {
            FirstReadingLabel.Text = "第 1 次：" + _temperatures[0] + " °C";
            SecondReadingLabel.Text = "第 2 次：" + _temperatures[1] + " °C";
            ThirdReadingLabel.Text = "第 3 次：" + _temperatures[2] + " °C";
        }

        // 教练提供的观察工具；方法名与 ConversionResults 控件名均为自行命名。
        // 每个箭头后面都是同一转换方法的实际返回值，没有预填期望答案。
        private void ShowConversionResults()
        {
            ConversionResults.Text =
                "样例：" + _temperatures[0] + " → " + ConvertTemperatureToY(_temperatures[0])
                + "；" + _temperatures[1] + " → " + ConvertTemperatureToY(_temperatures[1])
                + "；" + _temperatures[2] + " → " + ConvertTemperatureToY(_temperatures[2])
                + "\n边界与中点：20 → " + ConvertTemperatureToY(20)
                + "；25 → " + ConvertTemperatureToY(25)
                + "；30 → " + ConvertTemperatureToY(30);
        }
    }
}
