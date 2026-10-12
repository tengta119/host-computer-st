using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using WpfBae.Exercises.Task010;

namespace WpfBae.Exercises.Task013;

// 自定义 ViewModel：在 TASK-012 数据对象的基础上增加设备 ID 和读取命令。
// Reading 复用 TASK-010 已有读数类；集合、完整属性和通知连接由教练提供。
public class ReadingViewModel : INotifyPropertyChanged
{
    private string _deviceId = "TEMP-001";
    private string _statusText = "先完成 TODO 1，观察命令取得的设备 ID。";
    private ObservableCollection<Reading> _readings = new ObservableCollection<Reading>();
    private ReadTemperatureCommand _readCommand;
    private ClearHistoryCommand _clearCommand;

    public ReadingViewModel()
    {
        // this 是正在创建的这一个 ReadingViewModel；两个命令都保存它的引用。
        _readCommand = new ReadTemperatureCommand(this);
        _clearCommand = new ClearHistoryCommand(this);
        // 沿用 TASK-012 的集合事件连接，通知清空按钮重查条件。
        _readings.CollectionChanged += Readings_CollectionChanged;
    }

    public string DeviceId
    {
        get
        {
            return _deviceId;
        }
        set
        {
            // 输入框写回的字符串成为 value；先存入字段，再通知监听者重读属性。
            _deviceId = value;
            OnPropertyChanged("DeviceId");
        }
    }

    public string StatusText
    {
        get
        {
            return _statusText;
        }
        set
        {
            _statusText = value;
            OnPropertyChanged("StatusText");
        }
    }

    public ObservableCollection<Reading> Readings
    {
        get
        {
            return _readings;
        }
    }

    // ReadCommand、ClearCommand 是自定义属性名；ICommand 是框架接口。
    // getter 每次返回原命令对象，不创建新对象或新集合。
    public ICommand ReadCommand
    {
        get
        {
            return _readCommand;
        }
    }

    public ICommand ClearCommand
    {
        get
        {
            return _clearCommand;
        }
    }

    private void Readings_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _clearCommand.NotifyCanExecuteChanged();
    }

    // 以下为骨架工具：接口、委托类型和参数类型均来自 System.ComponentModel。
    // ? 允许没有订阅者（null）；调用 handler 才发出通知，声明事件本身不通知。
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler? handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
