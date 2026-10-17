using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfBae.Exercises.Task033
{
    // 教练提供的自定义数据类，保存本窗口唯一的温度集合和两段显示文字，不引用WPF控件。
    public class TemperatureMonitorViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<double> _temperatures = new ObservableCollection<double>();
        private string _readingCountText = "完整历史条数：0";
        private string _alarmText = "暂无温度";

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
                // TODO 3a（实践2 / DoD 2）：存好新文字后调用OnPropertyChanged，传入本属性的名称字符串。
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
                // TODO 3b（实践2 / DoD 2）：存好新文字后通知本属性变化，不传字段名或文字内容。
                OnPropertyChanged(nameof(AlarmText));
            }
        }

        // 教练预留普通方法，threshold接收窗口当前有效阈值，内部不操作任何TextBlock。
        public void RefreshDisplay(double threshold)
        {
            // TODO 2（实践2 / DoD 1/2）：用Temperatures.Count更新ReadingCountText，再处理空历史、最新原温度与threshold比较，把结果赋给AlarmText属性。
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
