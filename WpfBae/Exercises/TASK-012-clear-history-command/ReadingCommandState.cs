using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using WpfBae.Exercises.Task010;

namespace WpfBae.Exercises.Task012;

// 教练新建的数据类：保存历史、状态文字和清空命令对象。
// 集合、getter、属性通知及命令连接由骨架提供，不是本次要求自主重写的部分。
public class ReadingCommandState : INotifyPropertyChanged
{
    private ObservableCollection<Reading> _readings = new ObservableCollection<Reading>();
    private ClearHistoryCommand _clearCommand;
    private string _statusText = "历史为空。先完成 TODO 1，观察清空按钮是否可用。";

    public ReadingCommandState()
    {
        // ClearHistoryCommand 是我们自己写的类；new 创建命令对象。
        // this 就是当前这个 ReadingCommandState 对象，传给命令以共用历史。
        _clearCommand = new ClearHistoryCommand(this);

        // CollectionChanged 是集合自带的事件；+= 订阅集合变化。
        // 新增或清空之后，下面的普通方法会通知按钮重新判断条件。
        _readings.CollectionChanged += Readings_CollectionChanged;
    }

    public ObservableCollection<Reading> Readings
    {
        get
        {
            return _readings;
        }
    }

    // ICommand 是 .NET 接口；实际返回的对象是上面创建的 ClearHistoryCommand。
    // ClearCommand 是我们自己取的属性名，供 XAML 中的 Binding 按名称读取。
    public ICommand ClearCommand
    {
        get
        {
            return _clearCommand;
        }
    }

    // 沿用 TASK-010 的属性通知做法，使用完整 get/set 写法。
    public string StatusText
    {
        get
        {
            return _statusText;
        }
        set
        {
            // 属性赋值右侧的字符串成为 value，例如 _state.StatusText = "读取成功"。
            _statusText = value;
            OnPropertyChanged("StatusText");
        }
    }

    private void Readings_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 参数类型来自 .NET 的 System.Collections.Specialized，不是自定义读数类。
        // 集合已变化；这里仅要求按钮重新问 CanExecute，不执行清空。
        _clearCommand.NotifyCanExecuteChanged();
    }

    // 以下为教练提供的状态文字通知工具；类型来自 System.ComponentModel。
    // ? 允许事件当前没有订阅者；此时它的值为 null。
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
