using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfBae.Exercises.Task025
{
    // 教练新建的窗口类；Window 来自 WPF，类名与 XAML 的 x:Class 配对。
    public partial class Task025Window : Window
    {
        // ObservableCollection<double> 来自 .NET，保存原始温度小数。
        // new 创建一个空集合，_temperatures 保存它；始终 Add/Clear 同一对象。
        private readonly ObservableCollection<double> _temperatures = new ObservableCollection<double>();
        private readonly double[] _sampleTemperatures = new double[] { 24, 26.5, 25, 28 };
        private readonly double[] _xPositions = new double[] { 100, 280, 460 };
        private int _nextSampleIndex = 0;

        public Task025Window()
        {
            InitializeComponent();

            // TODO 1a（实践 1 / DoD 1）：在此登记 Temperatures_CollectionChanged，
            // 让 _temperatures 的 CollectionChanged 通知调用下面的普通方法。
            // 方法名不带调用括号；登记本身不执行方法。
            _temperatures.CollectionChanged += Temperatures_CollectionChanged;

            DrawTemperatureLine();
        }

        // 以下两个按钮方法由教练提供：只操作集合，不直接更新条数或画线。
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            double temperature = _sampleTemperatures[_nextSampleIndex];
            _nextSampleIndex = _nextSampleIndex + 1;
            if (_nextSampleIndex == _sampleTemperatures.Length)
            {
                _nextSampleIndex = 0;
            }

            _temperatures.Add(temperature);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // 清空后从样例首条 24 度重新开始。
            _nextSampleIndex = 0;
            _temperatures.Clear();
        }

        // 自定义普通方法名，不是 .NET 自动发现的约定名字；必须先完成 TODO 1a。
        // object? / NotifyCollectionChangedEventArgs 是此事件要求的参数类型。
        // sender 接收发通知的集合，e 接收本次变化说明；本题重画，无需分析 e。
        private void Temperatures_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO 1b（实践 1 / DoD 1）：把 _temperatures.Count 写入
            // ReadingCountText.Text，格式为“完整历史条数：”加实际条数。
            ReadingCountText.Text =  "完整历史条数：" + _temperatures.Count;

            // TODO 4（实践 2、4 / DoD 1、3）：计数验证后，在这里调用
            // DrawTemperatureLine()，让新增和清空都走同一个重画方法。
            DrawTemperatureLine();
        }

        // 复用 TASK-024 当前计算，已提供，不是本题的新 TODO。
        private double ConvertTemperatureToY(double temperature)
        {
            double degreesAboveMinimum = temperature - 20;
            double upwardDistance = degreesAboveMinimum * 20;
            double y = 240 - upwardDistance;
            return y;
        }

        private void DrawTemperatureLine()
        {
            // PointCollection 来自 WPF，只保存坐标；每次重画创建一份空坐标集合。
            PointCollection points = new PointCollection();
            ClearReadingLabels();

            // TODO 2（实践 2、3 / DoD 2）：用普通 if 求最新最多三条的起始下标。
            // 从 int startIndex = 0; 开始；有四条时应从下标 1 开始，
            // 但不要删除原历史。声明和计算与下方循环一起补入。
            int startIndex = 0;
            if (_temperatures.Count > 3)
            {
                startIndex = _temperatures.Count - 3;
            }

            // TODO 3（实践 2、3、4 / DoD 2、3）：在此补普通 for 循环。
            // 从 startIndex 走到 _temperatures.Count 之前；另用 slot 表示槽位 0/1/2。
            // 依次取原温度、_xPositions[slot]、转换后的 y，创建 Point 加入 points。
            // 每轮调用骨架 ShowReadingLabel(slot, 历史下标, 原温度) 显示相同读数。
            // 循环后把 points 交给 TemperatureLine.Points；空集合时也要赋值，
            // 不要提前 return 而留下旧蓝线。startIndex 在完成此 TODO 时用于循环。
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

            TemperatureLine.Points = points;

            ShowSinglePoint(points);
        }

        // 教练提供的文字工具：每次先清空三个旧标签，避免不足三条时残留。
        private void ClearReadingLabels()
        {
            FirstReadingLabel.Text = "";
            SecondReadingLabel.Text = "";
            ThirdReadingLabel.Text = "";
        }

        // slot 是图中位置；historyIndex 是原集合下标；temperature 是原温度。
        // 四条历史时三个标签应是第 2 / 3 / 4 条，不能重编号成第 1 / 2 / 3 条。
        private void ShowReadingLabel(int slot, int historyIndex, double temperature)
        {
            string text = "第 " + (historyIndex + 1) + " 条：" + temperature + " °C";
            if (slot == 0)
            {
                FirstReadingLabel.Text = text;
            }
            else if (slot == 1)
            {
                SecondReadingLabel.Text = text;
            }
            else if (slot == 2)
            {
                ThirdReadingLabel.Text = text;
            }
        }

        // 教练提供的单点工具：圆点直径 8，左/上减 4 让中心对准实际坐标。
        private void ShowSinglePoint(PointCollection points)
        {
            SinglePointMarker.Visibility = Visibility.Collapsed;
            if (points.Count == 1)
            {
                Point point = points[0];
                Canvas.SetLeft(SinglePointMarker, point.X - 4);
                Canvas.SetTop(SinglePointMarker, point.Y - 4);
                SinglePointMarker.Visibility = Visibility.Visible;
            }
        }

        // 教练提供的关闭处理：-= 移除同一集合上的同一方法登记。
        // OnClosed 是 WPF 窗口关闭时调用的方法，override 是 C# 重写语法；
        // base.OnClosed(e) 保留 Window 原有的关闭处理。本题不要求自行设计生命周期。
        protected override void OnClosed(EventArgs e)
        {
            _temperatures.CollectionChanged -= Temperatures_CollectionChanged;
            base.OnClosed(e);
        }
    }
}
