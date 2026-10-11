using System.Windows;
using WpfBae.Exercises.Task010;

namespace WpfBae.Exercises.Task011;

public partial class Task011Window : Window
{
    // ReadingHistoryState 是本任务自定义类；new 创建它，_state 保存这个对象。
    // 这个对象内部创建了一个历史集合，窗口和按钮共同使用它。
    private ReadingHistoryState _state = new ReadingHistoryState();

    public Task011Window()
    {
        InitializeComponent();
        // DataContext 是 WPF 提供的属性：窗口内的 Binding 从 _state 找数据。
        // ListBox 的 Binding Readings 取得 _state.Readings，连接只设置这一次。
        DataContext = _state;
    }

    private void ReadButton_Click(object sender, RoutedEventArgs e)
    {
        // 以下输入校验与读数构造复用 TASK-009/010 已验证的做法，由骨架提供。
        string id = DeviceIdTextBox.Text.Trim();
        if (string.IsNullOrEmpty(id))
        {
            StatusTextBlock.Text = "请输入设备 ID；本次没有新增记录。";
            return;
        }

        Reading reading = new Reading
        {
            DeviceId = id,
            Value = 26.5,
            Unit = "°C"
        };

        // TODO 1（首个动作）：调用 _state.Readings 的 Add 方法，把 reading 加进去。
        // Add 的参数就是刚创建的读数对象；每次调用在原集合末尾追加一条。
        // 只改集合，ListBox 会通过集合变化通知自动增加一行。
        _state.Readings.Add(reading);

        // TODO 2：新增完成后，把下面的占位提示改为读取成功的提示。
        StatusTextBlock.Text = "读取成功";
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO 3：调用 _state.Readings 的 Clear 方法，清空原集合。
        // Clear 不需要参数；不要新建另一份集合，不要直接改 HistoryListBox.Items。
        _state.Readings.Clear();
        // TODO 4：清空完成后，把下面的占位提示改为已清空的提示。
        StatusTextBlock.Text = "已清空";
    }
}
