using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfBae.Exercises.Task029
{
    // Task029Window 是教练新建的窗口类，Window 是 WPF 的窗口类。
    public partial class Task029Window : Window
    {
        // 此集合保存原始温度，表格、真实条数和绘图都使用它。
        private readonly ObservableCollection<double> _temperatures = new ObservableCollection<double>();
        private readonly double[] _sampleTemperatures = new double[] { 24, 26.5, 25, 28 };
        private readonly double[] _xPositions = new double[] { 100, 280, 460 };
        private int _nextSampleIndex = 0;

        // 教练提供的当前有效阈值字段，输入框文字尚未应用时不会改变它。
        private double _alarmThreshold = 27;

        // DispatcherTimer 来自 WPF，new 创建一个定时器，_samplingTimer 是教练自定义字段名。
        private readonly DispatcherTimer _samplingTimer = new DispatcherTimer();
        private int _tickCount = 0;

        public Task029Window()
        {
            InitializeComponent();
            UpdateThresholdDisplay();
            HistoryDataGrid.ItemsSource = _temperatures;
            _temperatures.CollectionChanged += Temperatures_CollectionChanged;

            // TimeSpan 来自 .NET，new TimeSpan(0, 0, 1) 创建一秒的时间长度，赋给这个定时器的 Interval。
            _samplingTimer.Interval = new TimeSpan(0, 0, 1);
            TickCountText.Text = "通知次数：" + _tickCount;
            // （实践 1 / DoD 2）：在这里将 SamplingTimer_Tick 登记到 _samplingTimer.Tick，仅登记一次。
            _samplingTimer.Tick += SamplingTimer_Tick;
            DrawTemperatureLine();
            UpdateTemperatureAlert();
        }

        // 教练提供的临时观察入口，只为先观察通知，正式启停完成后在 XAML 将 ObserveButton.Visibility 设为 Collapsed。
        private void ObserveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_samplingTimer.IsEnabled)
            {
                _samplingTimer.Stop();
                ObserveButton.Content = "观察通知（临时）";
            }
            else
            {
                _samplingTimer.Start();
                ObserveButton.Content = "停止观察（临时）";
            }
        }

        // SamplingTimer_Tick 是教练命名的普通接收方法，sender 是通知来源，e 是本次事件参数。
        private void SamplingTimer_Tick(object? sender, EventArgs e)
        {
            // （实践 1 / DoD 1）：将 _tickCount 加一，再把次数写到 TickCountText.Text，首步只观察通知。
            _tickCount++;
            TickCountText.Text = "通知次数：" + _tickCount;
            // 已完成：原TASK-027步骤3（实践 2 / DoD 1）：通知次数观察成功后，每次通知调用一次 AppendNextSample()。
            AppendNextSample();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // （实践 2 / DoD 2）：已启动时直接返回，否则对原 _samplingTimer 调用 Start，再调用 UpdateSamplingControls()，不要在这里 new 或登记 Tick。
            if (_samplingTimer.IsEnabled)
            {
                return;
            }
            _samplingTimer.Start();
            UpdateSamplingControls();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // （实践 2 / DoD 2）：对原 _samplingTimer 调用 Stop，再调用 UpdateSamplingControls()，不清空历史或重置 _nextSampleIndex。
            _samplingTimer.Stop();
            UpdateSamplingControls();
        }

        private void UpdateSamplingControls()
        {
            // 已完成：原TASK-027步骤4（实践 2/4 / DoD 2/3）：根据 _samplingTimer.IsEnabled 设置 SamplingStateText.Text 和 ClearButton.IsEnabled，并让启动/停止按钮状态与启停一致。
            // IsEnabled 是 WPF 按钮属性，False 让按钮不能点击，True 让按钮可以点击。
            if (_samplingTimer.IsEnabled)
            {
                SamplingStateText.Text = "采集中";
                ClearButton.IsEnabled = false;
            }
            else
            {
                SamplingStateText.Text = "已停止";
                ClearButton.IsEnabled = true;
            }
        }

        // 此普通辅助方法由教练从原添加按钮提取，AppendNextSample 是自定义名字，每次只追加一条样例。
        private void AppendNextSample()
        {
            double temperature = _sampleTemperatures[_nextSampleIndex];
            _nextSampleIndex = _nextSampleIndex + 1;
            if (_nextSampleIndex == _sampleTemperatures.Length)
            {
                _nextSampleIndex = 0;
            }

            _temperatures.Add(temperature);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AppendNextSample();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // 已完成：原TASK-027步骤5（实践 4 / DoD 3）：先检查定时器是否正在运行，正在运行时直接返回，保留下方停止状态清空与样例复位。
            if (_samplingTimer.IsEnabled)
            {
                return;
            }
            _nextSampleIndex = 0;
            _temperatures.Clear();
        }

        private void Temperatures_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ReadingCountText.Text = "完整历史条数：" + _temperatures.Count;
            DrawTemperatureLine();
            UpdateTemperatureAlert();
        }

        // 应用按钮通过XAML的Click调用这里，方法名由教练命名，输入检查与保存由学员完成。
        private void ApplyThresholdButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO 2（实践2 / DoD 2）：读取ThresholdInput.Text，用double.TryParse尝试转换，失败时设置ThresholdFeedbackText.Text后return，保留原阈值。
            string thresholdInputText = ThresholdInput.Text;
            double threshold;
            try
            {
                threshold = double.Parse(thresholdInputText);
                if (double.IsNaN(threshold))
                {
                    throw new Exception("温度不合法");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ThresholdFeedbackText.Text = "请输入20到30之间的数字。";
                return;
            }
            // TODO 3（实践2 / DoD 2）：只接受20到30且包含端点的数值，范围不合法时说明原因并return，保留原阈值。
            if (threshold < 20 || threshold > 30)
            {
                ThresholdFeedbackText.Text = "阈值温度只允许在20到30";
                return;
            }
            // TODO 4（实践2 / DoD 1/2）：只有检查通过才给_alarmThreshold赋新值，调用UpdateThresholdDisplay()并显示应用成功。
            _alarmThreshold = threshold;
            UpdateThresholdDisplay();
            // TODO 5（实践3 / DoD 1/3）：成功保存后调用UpdateTemperatureAlert()，用已有最新温度立即刷新，不追加或清空历史。
            UpdateTemperatureAlert();
            ThresholdFeedbackText.Text = "阈值已应用成功。";
        }

        // 教练提供的普通显示方法，只把已保存的数值写到文字控件，不解析输入，也不改变有效阈值。
        private void UpdateThresholdDisplay()
        {
            CurrentThresholdText.Text = "当前有效阈值：" + _alarmThreshold + " °C";
        }
        // UpdateTemperatureAlert 是教练新建的普通方法，负责把原集合的最新温度写到 AlarmText.Text。
        private void UpdateTemperatureAlert()
        {
            //实践1 / DoD 1/2）：先判断 _temperatures.Count == 0 并 return，避免读取空集合的最后元素。
            if (_temperatures.Count == 0)
            {
                AlarmText.Text = "暂无温度";
                return;
            }
            // （实践1 / DoD 1）：读取下标 Count - 1 的原温度，先只把它显示到 AlarmText.Text，添加第一条后应看到24。
            var temperature = _temperatures[_temperatures.Count - 1];
            AlarmText.Text = "最新温度" + temperature;
            // TOD 1（实践1 / DoD 1/3）：只把下方比较中的27替换为_alarmThreshold，保留原正常/超限两个分支。
            if (temperature >= _alarmThreshold)
            {
                AlarmText.Text = "温度超限";
            }
            else
            {
                AlarmText.Text = "温度正常";
            }
            // 空集合文字复位已沿用TASK-028最终版本。
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

        // 关闭收尾由教练提供，Stop 停止后续通知，-= 解除同一接收方法的登记。
        protected override void OnClosed(EventArgs e)
        {
            _samplingTimer.Stop();
            _samplingTimer.Tick -= SamplingTimer_Tick;
            _temperatures.CollectionChanged -= Temperatures_CollectionChanged;
            base.OnClosed(e);
        }
    }
}
