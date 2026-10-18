// 本题的新窗口；绘图、集合换接、文件辅助和关闭安全收尾由教练复用，TODO处连接留给学员。
using WpfBae.Exercises.Task036;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfBae.Exercises.Task038
{
    public partial class Task038Window : Window
    {
        private readonly TemperatureMonitorViewModel _viewModel = new TemperatureMonitorViewModel();
        private ObservableCollection<double> _temperatures;
        private readonly double[] _xPositions = new double[] { 100, 280, 460 };
        private double _alarmThreshold
        {
            get
            {
                return _viewModel.AlarmThreshold;
            }
            set
            {
                _viewModel.AlarmThreshold = value;
            }
        }
        private readonly string _settingsPath = Path.Combine(
            AppContext.BaseDirectory, "Exercises", "TASK-038-monitor-project", "alarm-threshold.json");
        private void ExportHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ExportTemperatureHistory();
        }

        private void ExportTemperatureHistory()
        {
            // TOD 7c（实践3/DoD2/3）：先拒绝采集或空历史，再显示CreateCsvSaveDialog，只有确认才将FileName交给WriteCsvToPath，取消保留已有文件并说明。
            if (_samplingTimer.IsEnabled)
            {
                ExportStatusText.Text = "正在采集，请先停止再导出。";
                return;
            }
            if (_temperatures.Count == 0)
            {
                ExportStatusText.Text = "数据为空";
                return;
            }

            // dialog是本次点击新建的保存窗口，CreateCsvSaveDialog是教练提供的普通辅助方法。
            Microsoft.Win32.SaveFileDialog dialog = CreateCsvSaveDialog();
            // 沿用TASK-032已完成步骤1（原实践1 / DoD 1/2准备）：调用dialog.ShowDialog(this)，用bool?变量接结果，此步先不写文件。
            bool? result = dialog.ShowDialog();
            // 沿用TASK-032已完成步骤2（原实践1 / DoD 2）：只有结果等于true才读取dialog.FileName，其他结果显示已取消并return；确认时先显示路径。
            if (result != true)
            {
                ExportStatusText.Text = "已取消导出。";
                return;
            }
            ExportStatusText.Text = "选择的路径：" + dialog.FileName;
            // 沿用TASK-032已完成步骤3（原实践2 / DoD 1/2/3）：路径观察通过后，把确认的dialog.FileName传给WriteCsvToPath，取消路径不得调用它。
            WriteCsvToPath(dialog.FileName);
            // TASK-032原覆盖确认/取消和写入失败已验证，打开窗口异常未实现的边界保持。
            // 写入方法负责显示成功或失败，这里保留它给出的实际结果。
        }
        private Microsoft.Win32.SaveFileDialog CreateCsvSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "选择温度历史CSV保存位置";
            dialog.FileName = "temperature-history.csv";
            dialog.Filter = "CSV文件 (*.csv)|*.csv";
            dialog.DefaultExt = ".csv";
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;
            dialog.CreateTestFile = false;
            return dialog;
        }
        private string BuildCsvText()
        {
            string csvText = "Sequence,TemperatureC" + Environment.NewLine;
            for (int i = 0; i < _temperatures.Count; i++)
            {
                csvText += Convert.ToString(i + 1) + "," + _temperatures[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine;
            }
            return csvText;
        }
        private void WriteCsvToPath(string path)
        {
            string csvText = BuildCsvText();
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, csvText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ExportStatusText.Text = "导出失败：" + e.Message;
                return;
            }
            ExportPathText.Text = "本次CSV文件：" + path;
            ExportStatusText.Text = _viewModel.CurrentDevice.Id + " 导出成功：" + path;
        }
        private readonly DispatcherTimer _samplingTimer = new DispatcherTimer();
        private int _tickCount = 0;

        public Task038Window()
        {
            _temperatures = _viewModel.Temperatures;
            InitializeComponent();
            // TOD 1（实践1/DoD1准备）：让当前窗口的DataContext指向上方已有_viewModel，不创建第二份对象。
            DataContext = _viewModel;
            ConfigPathText.Text = "配置文件：" + _settingsPath;
            ExportPathText.Text = "CSV位置：尚未选择。";
            _temperatures.CollectionChanged += Temperatures_CollectionChanged;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _samplingTimer.Interval = new TimeSpan(0, 0, 1);
            TickCountText.Text = "通知次数：" + _tickCount;
            // TOD 5d（实践2/DoD2）：在构造方法将SamplingTimer_Tick登记到原_samplingTimer.Tick一次。
            _samplingTimer.Tick += SamplingTimer_Tick;
            DrawTemperatureLine();
            UpdateTemperatureAlert();
            LoadThresholdSettings();
        }
        private void SamplingTimer_Tick(object? sender, EventArgs e)
        {
            // TOD 5c（实践2/DoD2）：累加_tickCount并更新TickCountText，然后只调用一次_viewModel.AppendNextSample。
            _tickCount++;
            TickCountText.Text = "通知次数：" + _tickCount;
            _viewModel.AppendNextSample();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // TOD 5e（实践2/DoD2）：已运行直接返回，否则启动原_samplingTimer并调用UpdateSamplingControls，不new或重复登记Tick。
            if (_samplingTimer.IsEnabled)
            {
                return;
            }
            _samplingTimer.Start();
            UpdateSamplingControls();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // TOD 5f（实践2/DoD2）：停止原_samplingTimer并调用UpdateSamplingControls，保留历史和样例位置。
            _samplingTimer.Stop();
            UpdateSamplingControls();
        }

        private void UpdateSamplingControls()
        {
            // TOD 5b（实践2/DoD2）：把_samplingTimer.IsEnabled交给_viewModel.IsSampling，并把已停止/采集中写到SamplingStateText。
            _viewModel.IsSampling = _samplingTimer.IsEnabled;
            // 已完成：原TASK-027步骤4（实践 2/4 / DoD 2/3）：根据 _samplingTimer.IsEnabled 设置 SamplingStateText.Text 和 ClearButton.IsEnabled，并让启动/停止按钮状态与启停一致。
            // IsEnabled 是 WPF 按钮属性，False 让按钮不能点击，True 让按钮可以点击。
            if (_samplingTimer.IsEnabled)
            {
                SamplingStateText.Text = "采集中";
            }
            else
            {
                SamplingStateText.Text = "已停止";
            }
        }
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Temperatures")
            {
                _temperatures.CollectionChanged -= Temperatures_CollectionChanged;
                _temperatures = _viewModel.Temperatures;
                _temperatures.CollectionChanged += Temperatures_CollectionChanged;
                DrawTemperatureLine();
                UpdateTemperatureAlert();
            }
        }
        private void Temperatures_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            DrawTemperatureLine();
            UpdateTemperatureAlert();

        }

        private void SaveThresholdButton_Click(object sender, RoutedEventArgs e)
        {
            SaveThresholdSettings();
        }
        // 教练提取文件写入方法，下面两个连接由学员完成；threshold是已读取并验证的候选数值。
        private void SaveThresholdSettings()
        {
            // TOD 7a（实践3/DoD3）：把_viewModel.AlarmThreshold传给WriteThresholdToFile，不从未应用输入读取。
            WriteThresholdToFile(_viewModel.AlarmThreshold);
        }
        private void ApplyLoadedThreshold(double threshold)
        {
            // TOD 7b（实践3/DoD3）：threshold是文件已读出的合法阈值，例如26；把它设为当前有效阈值和输入框文字，再刷新报警并显示配置加载完成，本步不写文件。
            // 把报警实际使用的阈值改成26。
            _viewModel.AlarmThreshold = threshold;
            // 让输入框也显示26。
            _viewModel.ThresholdInputText = Convert.ToString(threshold);
            // 用恢复后的阈值重新判断当前温度是否超限。
            UpdateTemperatureAlert();
            // 在界面上显示加载结果。
            ConfigStatusText.Text = "配置加载完成。";
        }
        private void WriteThresholdToFile(double threshold)
        {
            ConfigStatusText.Text = "准备保存的有效阈值：" + threshold;
            AlarmThresholdSettings alarmThresholdSettings = new AlarmThresholdSettings{Threshold = threshold};
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
                string alarmThreshJson = JsonSerializer.Serialize(alarmThresholdSettings, jsonSerializerOptions);
                File.WriteAllText(_settingsPath, alarmThreshJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ConfigStatusText.Text = "保存失败" + e.Message;    
                return;
            }
            ConfigStatusText.Text = "保存成功" + _settingsPath;
        }
        private void LoadThresholdSettings()
        {
            if (!File.Exists(_settingsPath))
            {
                ConfigStatusText.Text = "配置文件不存在";
                return;
            }
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            AlarmThresholdSettings? alarmThresholdSettings;
            try
            {
                string? alarmThreshJson = File.ReadAllText(_settingsPath);
                alarmThresholdSettings = JsonSerializer.Deserialize<AlarmThresholdSettings>(alarmThreshJson, jsonSerializerOptions);
                if (alarmThresholdSettings == null)
                {
                    ConfigStatusText.Text = "配置为空";
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ConfigStatusText.Text = "读取文件失败" + e.Message;
                return;
            }
            if (alarmThresholdSettings.Threshold < 20 || alarmThresholdSettings.Threshold > 30)
            {
                ConfigStatusText.Text = "配置中的报警阈值不合法。";
                return;
            }
            ApplyLoadedThreshold(alarmThresholdSettings.Threshold);
        }
        private void UpdateTemperatureAlert()
        {
            _viewModel.RefreshDisplay(_alarmThreshold);
        }
        private double ConvertTemperatureToY(double temperature)
        {
            double degreesAboveMinimum = temperature - 20;
            double upwardDistance = degreesAboveMinimum * 20;
            double y = 240 - upwardDistance;
            return y;
        }

        private void DrawTemperatureLine()
        {
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
        private void ClearReadingLabels()
        {
            FirstReadingLabel.Text = "";
            SecondReadingLabel.Text = "";
            ThirdReadingLabel.Text = "";
        }
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
        protected override void OnClosed(EventArgs e)
        {
            _samplingTimer.Stop();
            _samplingTimer.Tick -= SamplingTimer_Tick;
            _temperatures.CollectionChanged -= Temperatures_CollectionChanged;
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            base.OnClosed(e);
        }
    }
}
