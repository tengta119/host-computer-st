using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfBae.Exercises.Task035
{
    // Task035Window 是教练新建的窗口类，Window 是 WPF 的窗口类。
    public partial class Task035Window : Window
    {
        // 此集合保存原始温度，表格、真实条数和绘图都使用它。
        // 教练创建本窗口唯一的数据对象，TemperatureMonitorViewModel是本题自定义类。
        private readonly TemperatureMonitorViewModel _viewModel = new TemperatureMonitorViewModel();
        // 兼容已有采集/绘图/导出代码的引用，在构造方法指向ViewModel中的原集合，不创建第二份集合。
        private readonly ObservableCollection<double> _temperatures;
        private readonly double[] _xPositions = new double[] { 100, 280, 460 };

        // 教练提供的当前有效阈值字段，输入框文字尚未应用时不会改变它。
        // 教练提供的兼容入口，不单独保存数值，旧配置/报警代码暂时通过它操作VM同一有效值。
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
        // TOD 5c（实践3 / DoD 2/3）：旧入口全部改为直接使用_viewModel.AlarmThreshold后删除这个兼容属性，不另建有效字段。

        // AppContext.BaseDirectory是运行程序所在目录，Path.Combine拼接目录与文件名，启动不创建或覆盖配置。
        private readonly string _settingsPath = Path.Combine(
            AppContext.BaseDirectory, "Exercises", "TASK-035-threshold-viewmodel", "alarm-threshold.json");

        // 导出按钮与接收方法沿用TASK-031，本题只新增保存位置选择。
        private void ExportHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ExportTemperatureHistory();
        }

        private void ExportTemperatureHistory()
        {
            // TASK-031已验证的两项保护保持在任何对话框或文件写入之前。
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

        // 教练提供窗口外观设置，不打开窗口、不写文件；返回本次新建的保存窗口对象。
        private Microsoft.Win32.SaveFileDialog CreateCsvSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "选择温度历史CSV保存位置";
            dialog.FileName = "temperature-history.csv";
            dialog.Filter = "CSV文件 (*.csv)|*.csv";
            dialog.DefaultExt = ".csv";
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;
            // 首步只选路径时也不创建测试文件，确认后是否写入由练习代码决定。
            dialog.CreateTestFile = false;
            return dialog;
        }

        // 原TASK-031已验证的组行代码由教练原样提取，不是本题的新TODO。
        private string BuildCsvText()
        {
            string csvText = "Sequence,TemperatureC" + Environment.NewLine;
            for (int i = 0; i < _temperatures.Count; i++)
            {
                csvText += Convert.ToString(i + 1) + "," + _temperatures[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + Environment.NewLine;
            }
            return csvText;
        }

        // 原写入代码提取为普通方法，path接收调用者传来的完整文件路径，当前骨架尚不调用它。
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
            ExportStatusText.Text = "导出成功：" + path;
        }
        // DispatcherTimer 来自 WPF，new 创建一个定时器，_samplingTimer 是教练自定义字段名。
        private readonly DispatcherTimer _samplingTimer = new DispatcherTimer();
        private int _tickCount = 0;

        public Task035Window()
        {
            _temperatures = _viewModel.Temperatures;
            InitializeComponent();
            // TASK-033已完成步骤1a（原实践1 / DoD 1）：将当前窗口DataContext设为已有_viewModel，不能再new另一份对象。
            DataContext = _viewModel;
            ConfigPathText.Text = "配置文件：" + _settingsPath;
            ExportPathText.Text = "CSV位置：尚未选择。";
            
            // TASK-033表格已绑定唯一ViewModel中的Temperatures。
            _temperatures.CollectionChanged += Temperatures_CollectionChanged;

            // TimeSpan 来自 .NET，new TimeSpan(0, 0, 1) 创建一秒的时间长度，赋给这个定时器的 Interval。
            _samplingTimer.Interval = new TimeSpan(0, 0, 1);
            TickCountText.Text = "通知次数：" + _tickCount;
            // （实践 1 / DoD 2）：在这里将 SamplingTimer_Tick 登记到 _samplingTimer.Tick，仅登记一次。
            _samplingTimer.Tick += SamplingTimer_Tick;
            DrawTemperatureLine();
            UpdateTemperatureAlert();
            LoadThresholdSettings();
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
            // TASK-034已完成状态同步：把_samplingTimer.IsEnabled赋给_viewModel.IsSampling，再移除本方法两处ClearButton.IsEnabled直接赋值，让命令决定按钮可用性。
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

        // 此普通辅助方法由教练从原添加按钮提取，AppendNextSample 是自定义名字，每次只追加一条样例。
        private void AppendNextSample()
        {
            // 教练迁移原算法到ViewModel，定时器沿用此入口，与追加命令共用同一份样例位置。
            _viewModel.AppendNextSample();
        }
        // TASK-034已完成旧追加Click移除：XAML改为Command后删除这个旧Click方法，避免保留重复入口。


        // TASK-034已完成旧清空Click移除：清空Command接通后删除这个旧Click方法，运行保护已由ViewModel.ClearHistory承接。


        private void Temperatures_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // TASK-033已移除Count直接赋值，继续通过属性通知刷新。
            DrawTemperatureLine();
            UpdateTemperatureAlert();

        }

        private void SaveThresholdButton_Click(object sender, RoutedEventArgs e)
        {
            SaveThresholdSettings();
        }

        // 教练命名的普通方法，保存逻辑由学员完成，首步仅观察当前有效字段。
        // TOD 5a（实践3 / DoD 2）：保存只读取_viewModel.AlarmThreshold，不从输入文字获取有效值。
        private void SaveThresholdSettings()
        {
            // TOD 1（实践1 / DoD 1）：先把_alarmThreshold显示到ConfigStatusText.Text，应用26后保存应看到26，未应用输入29后仍看到26，先不写文件。
            ConfigStatusText.Text = "准备保存的有效阈值：" + _viewModel.AlarmThreshold;
            // TOD 2（实践1 / DoD 1/2）：用AlarmThresholdSettings对象保存当前有效字段，序列化为JSON并写入_settingsPath，创建专属父目录，成功后才显示保存成功，写入失败显示原因且程序继续可用。
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

        // 构造方法已经调用此方法，加载不得先保存或覆盖原文件。
        private void LoadThresholdSettings()
        {
            // TOD 3（实践2 / DoD 2）：文件不存在时保留初始27并说明，直接return，不自动保存默认值。
            if (!File.Exists(_settingsPath))
            {
                ConfigStatusText.Text = "配置文件不存在";
                return;
            }
            // TOD 4（实践2 / DoD 2）：读取_settingsPath，反序列化为AlarmThresholdSettings，处理损坏JSON/null/非法阈值/读取失败，失败时保留27并说明，原文件不变。
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            AlarmThresholdSettings alarmThresholdSettings;
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
            // 沿用TASK-030已完成的加载同步（原步骤5）：合法加载后给_alarmThreshold赋值，同步ThresholdInput.Text，调用UpdateThresholdDisplay与UpdateTemperatureAlert，并显示加载结果。
            if (alarmThresholdSettings.Threshold < 20 || alarmThresholdSettings.Threshold > 30)
            {
                ConfigStatusText.Text = "配置中的报警阈值不合法。";
                return;
            }
            _viewModel.AlarmThreshold = alarmThresholdSettings.Threshold;
            // TODO 5b（实践3 / DoD 2/3）：验证配置候选值后，恢复到_viewModel.AlarmThreshold与ThresholdInputText，移除下面两处控件Text直接赋值。
            _viewModel.ThresholdInputText = Convert.ToString(alarmThresholdSettings.Threshold);
            UpdateTemperatureAlert();
            ConfigStatusText.Text = "配置加载完成";
        }

        // 教练提供的普通显示方法，只把已保存的数值写到文字控件，不解析输入，也不改变有效阈值。
        // TOD 4c（实践2/3 / DoD 1/2）：CurrentThresholdText改为绑定后删除此旧直接显示方法及其调用，避免覆盖绑定。
        // UpdateTemperatureAlert 是教练新建的普通方法，负责把原集合的最新温度写到 AlarmText.Text。
        // TASK-033已完成报警刷新转交，本题沿用。
        private void UpdateTemperatureAlert()
        {
            _viewModel.RefreshDisplay(_alarmThreshold);
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
