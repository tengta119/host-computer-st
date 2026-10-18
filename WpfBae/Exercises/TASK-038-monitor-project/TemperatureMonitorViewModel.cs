using System.Collections.ObjectModel;
using System.ComponentModel;
using WpfBae.Exercises.Task036;

namespace WpfBae.Exercises.Task038
{
    // 本题新建的监控数据类，不是TASK-036的VM；设备数据类DeviceMonitorState沿用旧实现。
    public class TemperatureMonitorViewModel : INotifyPropertyChanged
    {
        private readonly DeviceMonitorState[] _devices = new DeviceMonitorState[]
        {
            new DeviceMonitorState("TEMP-001"),
            new DeviceMonitorState("TEMP-002")
        };
        private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };
        private readonly double[] _sampleTemperatures = new double[] { 24, 26.5, 25, 28 };
        private DeviceMonitorState _currentDevice;
        private string _selectedDeviceId = "TEMP-001";
        private string _readingCountText = "完整历史条数：0";
        private string _alarmText = "暂无温度";
        private bool _isSampling = false;
        private string _thresholdInputText = "27";
        private double _alarmThreshold = 27;
        private string _thresholdFeedbackText = "阈值应用连接待完成。";
        private readonly AppendTemperatureCommand _addCommand;
        private readonly ClearTemperatureHistoryCommand _clearCommand;
        private readonly ApplyThresholdCommand _applyThresholdCommand;

        // 教练提供必要对象与普通属性，命令保存的是当前this，不创建另一份监控数据。
        public TemperatureMonitorViewModel()
        {
            _currentDevice = _devices[0];
            _addCommand = new AppendTemperatureCommand(this);
            _clearCommand = new ClearTemperatureHistoryCommand(this);
            _applyThresholdCommand = new ApplyThresholdCommand(this);
        }

        public string[] DeviceIds
        {
            get { return _deviceIds; }
        }

        public string SelectedDeviceId
        {
            get { return _selectedDeviceId; }
            set
            {
                // TODO 2a（实践1/DoD1/2）：赋值前拒绝采集中选择，赋值后通知SelectedDeviceId并调用SwitchToSelectedDevice。
                _selectedDeviceId = value;
            }
        }

        public DeviceMonitorState CurrentDevice
        {
            get { return _currentDevice; }
        }

        public ObservableCollection<double> Temperatures
        {
            get { return _currentDevice.Temperatures; }
        }

        public void SwitchToSelectedDevice()
        {
            // TODO 2b（实践1/DoD1/2）：运行时直接返回，按SelectedDeviceId查找_devices已有对象并改指_currentDevice，通知CurrentDevice与Temperatures并刷新显示，不new、追加或清空。
        }

        public string ReadingCountText
        {
            get { return _readingCountText; }
            set
            {
                _readingCountText = value;
                // TODO 3a（实践2/DoD1）：通知ReadingCountText，下面AlarmText的setter也通知其同名属性。
            }
        }

        public string AlarmText
        {
            get { return _alarmText; }
            set { _alarmText = value; }
        }

        public void RefreshDisplay(double threshold)
        {
            // TODO 3b（实践2/DoD1/3）：先更新完整Count，空历史设暂无温度再返回，否则用最新原温度和threshold比较，将正常或超限写到AlarmText属性。
        }

        public void AppendNextSample()
        {
            // TODO 3c（实践2/DoD1/2）：按CurrentDevice.NextSampleIndex取原样例，位置加一并在Length处回0，只向当前Temperatures追加一次。
        }

        public void ClearHistory()
        {
            // TODO 3d（实践2/DoD1/2）：运行时返回，停止时只复位当前设备的位置并清空当前Temperatures，不影响另一设备。
        }

        public AppendTemperatureCommand AddCommand
        {
            get { return _addCommand; }
        }

        public ClearTemperatureHistoryCommand ClearCommand
        {
            get { return _clearCommand; }
        }

        public ApplyThresholdCommand ApplyThresholdCommand
        {
            get { return _applyThresholdCommand; }
        }

        public bool IsSampling
        {
            get { return _isSampling; }
            set
            {
                _isSampling = value;
                // TODO 5a（实践2/DoD2）：存好状态后通知清空命令重查条件，并通知CanSelectDevice；下面CanSelectDevice按运行状态返回是否允许选择。
            }
        }

        public bool CanSelectDevice
        {
            get
            {
                // TODO 5a：停止允许、采集中拒绝，不能一直返回此占位值。
                return true;
            }
        }

        public string ThresholdInputText
        {
            get { return _thresholdInputText; }
            set
            {
                _thresholdInputText = value;
                // TODO 6a（实践3/DoD3）：输入文字变化通知本属性，AlarmThreshold的setter通知AlarmThreshold与CurrentThresholdText，反馈setter通知ThresholdFeedbackText。
            }
        }

        public double AlarmThreshold
        {
            get { return _alarmThreshold; }
            set { _alarmThreshold = value; }
        }

        public string CurrentThresholdText
        {
            get { return "当前有效阈值：" + AlarmThreshold + " °C"; }
        }

        public string ThresholdFeedbackText
        {
            get { return _thresholdFeedbackText; }
            set { _thresholdFeedbackText = value; }
        }

        public void ApplyThreshold()
        {
            // TODO 6b（实践3/DoD3）：解析ThresholdInputText并拒绝非数字/NaN及20至30以外数值，失败只反馈并返回，成功才赋AlarmThreshold、重算报警并反馈成功，不改历史。
        }

        // 教练提供.NET属性通知的基础代码，学员在对应setter和切换入口调用此方法。
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
