using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfBae.Exercises.Task036
{
    // 教练提供的自定义数据类，保存本窗口唯一的温度集合和两段显示文字，不引用WPF控件。
    public class TemperatureMonitorViewModel : INotifyPropertyChanged
    {
        // 两台设备各自创建历史集合，当前监控对象在构造方法初始化为第一台。
        private readonly DeviceMonitorState[] _devices = new DeviceMonitorState[]
        {
            new DeviceMonitorState("TEMP-001"),
            new DeviceMonitorState("TEMP-002")
        };
        private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };
        private DeviceMonitorState _currentDevice;
        private string _selectedDeviceId = "TEMP-001";

        public string[] DeviceIds
        {
            get
            {
                return _deviceIds;
            }
        }

        public string SelectedDeviceId
        {
            get
            {
                return _selectedDeviceId;
            }
            set
            {
                // TODO 4a（实践2 / DoD 2）：正在采集时拒绝改变选择并返回，保护必须在给_selectedDeviceId赋值之前。
                if (_isSampling)
                {
                    return;
                }
                _selectedDeviceId = value;
                OnPropertyChanged("SelectedDeviceId");
                // TOD 2a（实践2 / DoD 1）：首步编号观察通过后，在这里调用SwitchToSelectedDevice。
                SwitchToSelectedDevice();
            }
        }

        public DeviceMonitorState CurrentDevice
        {
            get
            {
                return _currentDevice;
            }
        }

        public bool CanSelectDevice
        {
            get
            {
                // TODO 4b（实践2 / DoD 2）：根据IsSampling返回能否选择，停止允许，运行拒绝。
                if (IsSampling)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // 教练预留切换入口，首步只观察SelectedDeviceId，尚不改变_currentDevice。
        public void SwitchToSelectedDevice()
        {
            // TOD 2b（实践2 / DoD 1/2）：先做IsSampling保护，再从_devices找Id等于SelectedDeviceId的对象，找到后给_currentDevice赋该对象，不new或清空历史。
            if (_isSampling)
            {
                return;
            }

            for (int i = 0; i < _devices.Length; i++)
            {
                if (_devices[i].Id.Equals(SelectedDeviceId))
                {
                    _currentDevice = _devices[i];
                    break;
                }
            }
            // TOD 2c（实践2 / DoD 1/3）：切换后通知CurrentDevice和Temperatures属性变化，并调用RefreshDisplay(AlarmThreshold)，不要执行Add或Clear。
            OnPropertyChanged("CurrentDevice");
            OnPropertyChanged("Temperatures");
            RefreshDisplay(AlarmThreshold);
        }
        private string _readingCountText = "完整历史条数：0";
        private string _alarmText = "暂无温度";

        // 原窗口样例和位置迁到此对象，手动与定时追加都使用下面同一个方法。
        private readonly double[] _sampleTemperatures = new double[] { 24, 26.5, 25, 28 };
        // TODO 3c（实践2 / DoD 1/2）：完成3a和3b后删除本ViewModel的下一行共用字段，DeviceMonitorState里的同名字段保留。
        private bool _isSampling = false;
        private readonly AppendTemperatureCommand _addCommand;
        private readonly ClearTemperatureHistoryCommand _clearCommand;

        private string _thresholdInputText = "27";
        private double _alarmThreshold = 27;
        private string _thresholdFeedbackText = "请输入20到30之间的阈值，点击应用后生效。";
        private readonly ApplyThresholdCommand _applyThresholdCommand;

        // 新输入属性只保存文字，不自动解析、改变有效值或写文件。
        public string ThresholdInputText
        {
            get
            {
                return _thresholdInputText;
            }
            set
            {
                _thresholdInputText = value;
                OnPropertyChanged("ThresholdInputText");
            }
        }

        // 教练提供唯一有效数值存储，输入验证由本题ApplyThreshold负责。
        public double AlarmThreshold
        {
            get
            {
                return _alarmThreshold;
            }
            set
            {
                _alarmThreshold = value;
                OnPropertyChanged("AlarmThreshold");
                OnPropertyChanged("CurrentThresholdText");
            }
        }

        public string CurrentThresholdText
        {
            get
            {
                return "当前有效阈值：" + _alarmThreshold + " °C";
            }
        }

        public string ThresholdFeedbackText
        {
            get
            {
                return _thresholdFeedbackText;
            }
            set
            {
                _thresholdFeedbackText = value;
                OnPropertyChanged("ThresholdFeedbackText");
            }
        }

        public ApplyThresholdCommand ApplyThresholdCommand
        {
            get
            {
                return _applyThresholdCommand;
            }
        }

        public void ApplyThreshold()
        {
            // TASK-035已完成应用验证：参照旧窗口ApplyThresholdButton_Click，把输入读取改为ThresholdInputText，沿用Parse/catch、NaN及20至30验证。
            // 验证失败只更新ThresholdFeedbackText并return；成功才给AlarmThreshold赋候选值、调用RefreshDisplay(AlarmThreshold)，再显示成功，不修改历史。
            // 已完成：TASK-029步骤2：读取ThresholdInput.Text，用double.TryParse尝试转换，失败时设置ThresholdFeedbackText.Text后return，保留原阈值。
            string thresholdInputText = ThresholdInputText;
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
                ThresholdFeedbackText = "请输入20到30之间的数字。";
                return;
            }
            if (threshold < 20 || threshold > 30)
            {
                ThresholdFeedbackText  = "阈值温度只允许在20到30";
                return;
            }
            AlarmThreshold = threshold;
            RefreshDisplay(AlarmThreshold);

            ThresholdFeedbackText = "阈值已应用成功。";
        }

        public TemperatureMonitorViewModel()
        {
            // this是当前ViewModel，两个命令都保存这个对象，不创建另一份数据。
            _currentDevice = _devices[0];
            _applyThresholdCommand = new ApplyThresholdCommand(this);
            _addCommand = new AppendTemperatureCommand(this);
            _clearCommand = new ClearTemperatureHistoryCommand(this);
        }

        public AppendTemperatureCommand AddCommand
        {
            get
            {
                return _addCommand;
            }
        }

        public ClearTemperatureHistoryCommand ClearCommand
        {
            get
            {
                return _clearCommand;
            }
        }

        // 只记录窗口传来的运行状态，不创建或启动定时器。
        public bool IsSampling
        {
            get
            {
                return _isSampling;
            }
            set
            {
                _isSampling = value;
                // TASK-034已完成状态通知：调用已有_clearCommand.NotifyCanExecuteChanged，让按钮重新询问能否清空。
                _clearCommand.NotifyCanExecuteChanged();
                // TODO 4c（实践2 / DoD 2）：通知CanSelectDevice属性变化，让已绑定的选择框重新读取能否选择。
                OnPropertyChanged("CanSelectDevice");
            }
        }

        // 教练原样迁移已验证的样例追加算法，不是本题的新核心TODO。
        // TODO 3a（实践2 / DoD 1/2）：将本方法中的_nextSampleIndex替换为CurrentDevice.NextSampleIndex，使每台设备独立继续样例。
        public void AppendNextSample()
        {
            double temperature = _sampleTemperatures[CurrentDevice.NextSampleIndex];
            CurrentDevice.NextSampleIndex = CurrentDevice.NextSampleIndex + 1;
            if (CurrentDevice.NextSampleIndex == _sampleTemperatures.Length)
            {
                CurrentDevice.NextSampleIndex = 0;
            }
            Temperatures.Add(temperature);
        }

        // 教练迁移原清空与复位，保护判断使用窗口交来的IsSampling。
        public void ClearHistory()
        {
            if (IsSampling)
            {
                return;
            }
            // TODO 3b（实践2 / DoD 2）：改为只复位CurrentDevice.NextSampleIndex，不改变另一设备的位置。
            CurrentDevice.NextSampleIndex = 0;
            Temperatures.Clear();
        }

        public ObservableCollection<double> Temperatures
        {
            get
            {
                return _currentDevice.Temperatures;
            }
        }

        public string ReadingCountText
        {
            get
            {
                return _readingCountText;
            }
            set
            {
                _readingCountText = value;
                // TASK-033已完成通知（原步骤3a）：存好新文字后调用OnPropertyChanged，传入本属性的名称字符串。
                OnPropertyChanged(nameof(ReadingCountText));
            }
        }

        public string AlarmText
        {
            get
            {
                return _alarmText;
            }
            set
            {
                _alarmText = value;
                // TASK-033已完成通知（原步骤3b）：存好新文字后通知本属性变化，不传字段名或文字内容。
                OnPropertyChanged(nameof(AlarmText));
            }
        }

        // 教练预留普通方法，threshold接收窗口当前有效阈值，内部不操作任何TextBlock。
        public void RefreshDisplay(double threshold)
        {
            // TASK-033已完成状态更新（原步骤2）：用Temperatures.Count更新ReadingCountText，再处理空历史、最新原温度与threshold比较，把结果赋给AlarmText属性。
            // 空历史应先设置暂无温度再return，先更新条数以保证清空后能显示0。
            ReadingCountText = "完整历史条数：" + Temperatures.Count.ToString();
            if (Temperatures.Count == 0)
            {
                AlarmText = "暂无温度";
                return;
            } 
            double temperature = Temperatures[Temperatures.Count - 1];
            
            AlarmText = "最新温度" + temperature;
            
            if (temperature >= threshold)
            {
                AlarmText = "温度超限";
            }
            else
            {
                AlarmText = "温度正常";
            }
        }

        // INotifyPropertyChanged与这些事件参数来自.NET，下面是教练提供的通知基础代码。
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
