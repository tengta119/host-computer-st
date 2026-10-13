using System.Windows;
using System.Windows.Media;

namespace WpfBae.Exercises.Task018
{
    // Task018Window 是教练新建的窗口类，配合旁边同名 XAML 使用。
    // Point 来自 System.Windows；PointCollection 来自 System.Windows.Media。
    public partial class Task018Window : Window
    {
        // double[] 是 C# 数组写法；以下字段名与固定数据均由教练提供。
        // _temperatures 保存原始温度；_xPositions 保存三个点的横向位置。
        private readonly double[] _temperatures = new double[] { 24, 26.5, 25 };
        private readonly double[] _xPositions = new double[] { 100, 280, 460 };

        public Task018Window()
        {
            InitializeComponent();
            ShowReadingLabels();
            DrawTemperatureLine();
            // TODO 3（最后连接 / 实践 2 / DoD 1、2）：
            // 完成下方转换与绘图后，把上面 ShowPlaceholderLine() 这一处调用
            // 替换为 DrawTemperatureLine()，不要两种绘图方法都调用。
        }

        // 普通自定义方法，只有被调用才会运行；当前在窗口构造时调用一次。
        private void ShowPlaceholderLine()
        {
            PointCollection points = new PointCollection();
            // new Point(x, y) 创建一个坐标；Add 按先后顺序把坐标放进集合。
            points.Add(new Point(100, 180));
            // TODO 1（首个动作 / 实践 1 / DoD 1 的坐标方向准备）：
            // 只把下一行第二个数 180 改成 100，保存、关闭旧窗口后重新运行。
            // 中间点应向上移动，两个端点不变；这是占位坐标，还不是温度换算。
            points.Add(new Point(280, 100));
            points.Add(new Point(460, 180));
            // TemperatureLine 指向 XAML 中那条蓝线；Points 是它的 WPF 属性。
            // 把这份坐标集合交给蓝线，蓝线就按顺序连接三个坐标。
            TemperatureLine.Points = points;
        }

        // 自定义辅助方法：输入一条温度，返回这个点在 Canvas 中的 y。
        private double ConvertTemperatureToY(double temperature)
        {
            // 比最低温度 20 °C 高出多少度。
            double degreesAboveMinimum = temperature - 20;
            // 每升高 1 °C，点向上移动 20 个界面单位。
            double upwardDistance = degreesAboveMinimum * 20;
            // 从底部 y=240 向上移动，y 要减小。
            return 240 - upwardDistance;
        }

        private void DrawTemperatureLine()
        {
            ExerciseHint.Text = "当前调用温度绘图方法：请核对三个点是否对应温度，再做中间温度变化比较。";
            PointCollection points = new PointCollection();
            // i 是 C# 循环局部变量，依次为 0、1、2；数组下标从 0 开始。
            for (int i = 0; i < _temperatures.Length; i++)
            {
                double x = _xPositions[i];
                double y = ConvertTemperatureToY(_temperatures[i]);
                // TODO 3（实践 2 / DoD 1、2）：把本轮 x、y 创建成 Point，
                // 加入上方 points 集合。可参照占位方法中单个 Point 的写法。
                points.Add(new Point(x, y));
            }

            // TODO 3（实践 2 / DoD 1、2）：把 points 赋给同一条蓝线的 Points。
            // 完成后再替换构造方法中的绘图调用。
            TemperatureLine.Points = points;
        }

        // 教练提供的文字显示方法；+ 拼接字符串，让标签跟随原始数组。
        private void ShowReadingLabels()
        {
            FirstReadingLabel.Text = "第 1 次：" + _temperatures[0] + " °C";
            SecondReadingLabel.Text = "第 2 次：" + _temperatures[1] + " °C";
            ThirdReadingLabel.Text = "第 3 次：" + _temperatures[2] + " °C";
            // TODO 4（实践 3 / DoD 3）：完成温度绘图后，仅将数组中间的 26.5
            // 改成 28，重新运行，比较中间点和标签；两个端点应保持原位置。
            // 保留修改前后运行证据；完成比较后可恢复为 26.5。
        }
    }
}
