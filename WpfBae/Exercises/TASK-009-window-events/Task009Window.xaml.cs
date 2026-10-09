using System.Windows;

namespace WpfBae.Exercises.Task009;

public class Reading 
{
    public string DeviceId { get; init; } = "";
    public double Value { get; init; }
    public string Unit { get; init; } = "";

    public override string ToString() {
        return $"{DeviceId} {Unit} {Value}";
    }
}

public partial class Task009Window : Window
{
    public Task009Window()
    {
        // 加载配套 XAML，并初始化其中具名的控件。
        InitializeComponent();
    }

    private void ReadButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO 2：读取 DeviceIdTextBox.Text 并 Trim()。
        // 如果 ID 为空或全是空白，在 StatusTextBlock.Text 显示提示，
        // 清除 ResultTextBlock.Text 中的旧结果，再提前 return。
        // 每次点击都读取当前输入，不要写死为 TEMP-001。
        string idStr = DeviceIdTextBox.Text.Trim();
        if (idStr.Length == 0)
        {
            StatusTextBlock.Text = "ID 为空或全是空白，请输入正确的ID";
            ResultTextBlock.Text = "";
            return;
        }
        // TODO 3：有效输入时，自行设置模拟温度（例如 26.5）及单位 °C，
        // 把当前设备 ID、温度和单位一起写入 ResultTextBlock.Text，
        // 在 StatusTextBlock.Text 显示本次读取成功。
        // 本任务可以直接使用局部变量，不需要通信库或异步等待。
        Reading reading = new Reading{DeviceId = idStr, Unit = "°C", Value = 26.5};
        ResultTextBlock.Text = reading.ToString();
        StatusTextBlock.Text = "读取成功";
        
        return;
    }
}
