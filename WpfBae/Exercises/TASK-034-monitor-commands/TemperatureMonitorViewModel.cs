using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfBae.Exercises.Task034
{
    // 教练提供的自定义数据类，保存本窗口唯一的温度集合和两段显示文字，不引用WPF控件。
    public class TemperatureMonitorViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<double> _temperatures = new ObservableCollection<double>();
        private string _readingCountText = "完整历史条数：0";
        private string _alarmText = "暂无温度";

        // 原窗口样例和位置迁到此对象，手动与定时追加都使用下面同一个方法。
        private readonly double[] _sampleTemperatures = new double[] { 24, 26.5, 25, 28 };
        private int _nextSampleIndex = 0;
        private bool _isSampling = false;
        private readonly AppendTemperatureCommand _addCommand;
        private readonly ClearTemperatureHistoryCommand _clearCommand;

        public TemperatureMonitorViewModel()
        {
            // this是当前ViewModel，两个命令都保存这个对象，不创建另一份数据。
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
                // TODO 3（实践2 / DoD 2）：调用已有_clearCommand.NotifyCanExecuteChanged，让按钮重新询问能否清空。
            }
        }

        // 教练原样迁移已验证的样例追加算法，不是本题的新核心TODO。
        public void AppendNextSample()
        {
            double temperature = _sampleTemperatures[_nextSampleIndex];
            _nextSampleIndex = _nextSampleIndex + 1;
            if (_nextSampleIndex == _sampleTemperatures.Length)
            {
                _nextSampleIndex = 0;
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
            _nextSampleIndex = 0;
            Temperatures.Clear();
        }

        public ObservableCollection<double> Temperatures
        {
            get
            {
                return _temperatures;
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
