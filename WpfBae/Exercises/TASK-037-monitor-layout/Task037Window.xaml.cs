// 教练从TASK-036复用的后台连接，不是本题核心TODO；数据类、ViewModel与命令直接使用原有类型。
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

namespace WpfBae.Exercises.Task037
{
    public partial class Task037Window : Window
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
            AppContext.BaseDirectory, "Exercises", "TASK-037-monitor-layout", "alarm-threshold.json");
        private void ExportHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ExportTemperatureHistory();
        }

        private void ExportTemperatureHistory()
        {
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
            Microsoft.Win32.SaveFileDialog dialog = CreateCsvSaveDialog();
            bool? result = dialog.ShowDialog(this);
            if (result != true)
            {
                ExportStatusText.Text = "已取消导出。";
                return;
            }
            ExportStatusText.Text = "选择的路径：" + dialog.FileName;
            WriteCsvToPath(dialog.FileName);
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

        public Task037Window()
        {
            _temperatures = _viewModel.Temperatures;
            InitializeComponent();
            DataContext = _viewModel;
            ConfigPathText.Text = "配置文件：" + _settingsPath;
            ExportPathText.Text = "CSV位置：尚未选择。";
            _temperatures.CollectionChanged += Temperatures_CollectionChanged;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _samplingTimer.Interval = new TimeSpan(0, 0, 1);
            TickCountText.Text = "通知次数：" + _tickCount;
            _samplingTimer.Tick += SamplingTimer_Tick;
            DrawTemperatureLine();
            UpdateTemperatureAlert();
            LoadThresholdSettings();
        }
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
        private void SamplingTimer_Tick(object? sender, EventArgs e)
        {
            _tickCount++;
            TickCountText.Text = "通知次数：" + _tickCount;
            AppendNextSample();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_samplingTimer.IsEnabled)
            {
                return;
            }
            _samplingTimer.Start();
            UpdateSamplingControls();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _samplingTimer.Stop();
            UpdateSamplingControls();
        }

        private void UpdateSamplingControls()
        {
            _viewModel.IsSampling = _samplingTimer.IsEnabled;
            if (_samplingTimer.IsEnabled)
            {
                SamplingStateText.Text = "采集中";
            }
            else
            {
                SamplingStateText.Text = "已停止";
            }
        }
        private void AppendNextSample()
        {
            _viewModel.AppendNextSample();
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
        private void SaveThresholdSettings()
        {
            ConfigStatusText.Text = "准备保存的有效阈值：" + _viewModel.AlarmThreshold;
            AlarmThresholdSettings alarmThresholdSettings = new AlarmThresholdSettings{Threshold = _viewModel.AlarmThreshold};
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
            _viewModel.AlarmThreshold = alarmThresholdSettings.Threshold;
            _viewModel.ThresholdInputText = Convert.ToString(alarmThresholdSettings.Threshold);
            UpdateTemperatureAlert();
            ConfigStatusText.Text = "配置加载完成";
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
