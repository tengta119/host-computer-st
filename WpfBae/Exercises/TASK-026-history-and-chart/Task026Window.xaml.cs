using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfBae.Exercises.Task026
{
    // 教练新建的 TASK-026 窗口类；Window 来自 WPF，与 XAML 的 x:Class 配对。
    // 下方集合、按钮、计数、坐标和绘图逻辑沿用 TASK-025 已验证的实现。
    public partial class Task026Window : Window
    {
        // ObservableCollection<double> 来自 .NET，保存原始温度小数。
        // new 创建一个空集合，_temperatures 保存它；始终 Add/Clear 同一对象。
        private readonly ObservableCollection<double> _temperatures = new ObservableCollection<double>();
        private readonly double[] _sampleTemperatures = new double[] { 24, 26.5, 25, 28 };
        private readonly double[] _xPositions = new double[] { 100, 280, 460 };
        private int _nextSampleIndex = 0;

        public Task026Window()
        {
            InitializeComponent();

            // TODO 1（实践 1 / DoD 1）：把 HistoryDataGrid.ItemsSource 设置为 _temperatures。
            // ItemsSource 是 WPF 表格属性；_temperatures 指向下方按钮 Add/Clear 的原集合。
            // 只连接一次，不创建副本、不在按钮里给表格另添行。
            HistoryDataGrid.ItemsSource = _temperatures;

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

        // 沿用 TASK-025 的自定义接收方法；构造方法已经登记集合通知。
        // object? / NotifyCollectionChangedEventArgs 是此事件要求的参数类型。
        // sender 接收发通知的集合，e 接收本次变化说明；本题重画，无需分析 e。
        private void Temperatures_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ReadingCountText.Text =  "完整历史条数：" + _temperatures.Count;

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

            int startIndex = 0;
            if (_temperatures.Count > 3)
            {
                startIndex = _temperatures.Count - 3;
            }

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
