using System.Windows;
using WpfBae.Exercises.Task010;

namespace WpfBae.Exercises.Task012;

public partial class Task012Window : Window
{
    // ReadingCommandState 是本任务的新数据类；此对象内有一个集合和一个清空命令。
    private ReadingCommandState _state = new ReadingCommandState();

    public Task012Window()
    {
        InitializeComponent();
        // WPF 的 DataContext 让窗口里的 Binding 从同一个 _state 对象读取属性。
        DataContext = _state;
    }

    private void ReadButton_Click(object sender, RoutedEventArgs e)
    {
        // 复用 TASK-011 已验证的读取过程，由骨架提供，保留旧任务源码。
        string id = DeviceIdTextBox.Text.Trim();
        if (string.IsNullOrEmpty(id))
        {
            _state.StatusText = "请输入设备 ID；本次没有新增记录。";
            return;
        }

        Reading reading = new Reading
        {
            DeviceId = id,
            Value = 26.5,
            Unit = "°C"
        };

        _state.Readings.Add(reading);
        _state.StatusText = "读取成功";
    }

    // 清空按钮通过 Command 执行，不在窗口中另写清空 Click 处理器。
}
