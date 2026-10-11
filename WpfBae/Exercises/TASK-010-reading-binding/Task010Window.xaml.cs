using System.Windows;

namespace WpfBae.Exercises.Task010;

public class Reading 
{
    public string DeviceId { get; init; } = "";
    public double Value { get; init; }
    public string Unit { get; init; } = "";

    public override string ToString() {
        return $"{DeviceId} {Value} {Unit} ";
    }
}

public partial class Task010Window : Window
{
    // 同一个对象既提供绑定的数据，也接收按钮处理器的修改。
    private readonly ReadingDisplayState _state = new();

    public Task010Window()
    {
        InitializeComponent();
        // TODO 1：把本窗口的 DataContext 设置为 _state。
        // 它告诉窗口内的 Binding：去这个对象上找 ResultText / StatusText。
        DataContext = _state;
    }

    private void ReadButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO 4a：沿用 TASK-009 已完成的输入读取和 Trim、空白检查。
        // 空白时设置 _state.StatusText 提示、清空 _state.ResultText，再 return。
        string id = DeviceIdTextBox.Text.Trim();
        if (string.IsNullOrEmpty(id))
        {
            _state.StatusText = "Id 为空或者异常";
            _state.ResultText = "";
            
            return;
        }
        // TODO 4b：有效输入时，把当前 ID、固定温度 26.5 和 °C 组成结果文字，
        // 保存到 _state.ResultText，并设置 _state.StatusText 为读取成功。
        // 也可以参考 TASK-009 的 Reading 类型构造读数。
        // 本任务不要给 ResultTextBlock.Text / StatusTextBlock.Text 直接赋值。
        Reading reading = new Reading{DeviceId = id, Value = 26.5, Unit = "°C"};
        _state.ResultText = reading.ToString();
        _state.StatusText = "读取成功";

        // 初始占位：点击暂时不改变任何值；完成 TODO 后由数据属性驱动显示。
    }
}
