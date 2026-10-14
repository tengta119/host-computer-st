using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task023;

public partial class Task023Window : Window
{
    // ReadingViewModel 是 TASK-013 已有的自定义数据类，不是 WPF API。
    // new 创建本窗口的一份对象，_viewModel 保存它的引用。
    // 原读取/清空命令与表格继续连接它的同一个 Readings 集合。
    private readonly ReadingViewModel _viewModel = new ReadingViewModel();
    private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };

    public Task023Window()
    {
        // InitializeComponent 是 XAML 构建生成的方法，创建本窗口中的控件。
        InitializeComponent();
        DeviceComboBox.ItemsSource = _deviceIds;
        // DataContext 是 WPF 属性：左右两块从本窗口取得同一个数据对象。
        DataContext = _viewModel;
        _viewModel.StatusText = "列布局练习：先把左列宽度从 320 改为 360，观察右侧历史标题的位置。";
    }
}
