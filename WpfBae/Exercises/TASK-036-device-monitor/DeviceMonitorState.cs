using System.Collections.ObjectModel;

namespace WpfBae.Exercises.Task036
{
    // 教练新建的设备数据类，每个具体对象分别保存自己的编号、历史和下一条样例位置。
    public class DeviceMonitorState
    {
        private readonly string _id;
        private readonly ObservableCollection<double> _temperatures = new ObservableCollection<double>();
        private int _nextSampleIndex = 0;

        public DeviceMonitorState(string id)
        {
            _id = id;
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public ObservableCollection<double> Temperatures
        {
            get
            {
                return _temperatures;
            }
        }

        public int NextSampleIndex
        {
            get
            {
                return _nextSampleIndex;
            }
            set
            {
                _nextSampleIndex = value;
            }
        }
    }
}
