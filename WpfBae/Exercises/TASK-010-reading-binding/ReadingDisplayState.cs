using System.ComponentModel;

namespace WpfBae.Exercises.Task010;

// 这个对象保存“窗口要显示的数据”，不持有 TextBlock 等控件。
public sealed class ReadingDisplayState : INotifyPropertyChanged
{
    // 字段保存实际值，公开属性供 Binding 按名称读取。
    private string _resultText = "尚未读取";
    private string _statusText = "请输入设备 ID，然后读取温度。";

    public string ResultText
    {
        get => _resultText;
        set
        {
            _resultText = value;
            // TODO 3a：赋值后调用 OnPropertyChanged，通知 ResultText 已变化。
            // 参数用 nameof(ResultText)，它会得到属性名字符串，避免手写拼错。
            OnPropertyChanged("ResultText");
        }
    }

    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            // TODO 3b：仿照 ResultText，通知 StatusText 已变化。
            OnPropertyChanged("StatusText");
        }
    }

    // 接口要求的事件：WPF 会在绑定建立时监听这个事件。
    // ? 表示当前可以没有订阅者，刚创建数据对象时这是正常情况。
    public event PropertyChangedEventHandler? PropertyChanged;

    // 已提供通知工具；TODO 3 的关键是让两个 setter 在赋值后调用它。
    private void OnPropertyChanged(string propertyName)
    {
        // this 是发出通知的数据对象；参数携带发生变化的属性名。
        // ?.Invoke 表示有订阅者才调用，避免没有订阅者时抛异常。
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
