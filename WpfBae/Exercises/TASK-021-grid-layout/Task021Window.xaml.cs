using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task021;

public partial class Task021Window : Window
{
    // ReadingViewModel 是 TASK-013 已有的自定义数据类。
    // new 创建本窗口的一份数据对象；读取/清空命令继续使用它的原历史集合。
    private readonly ReadingViewModel _viewModel = new ReadingViewModel();
    // 复用 TASK-020 的两个字符串选项，未引入新设备类或新业务逻辑。
    private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };

    public Task021Window()
    {
        InitializeComponent();
        DeviceComboBox.ItemsSource = _deviceIds;
        // DataContext 是 WPF 属性；三个区域继承同一个对象供原 Binding 使用。
        DataContext = _viewModel;
        _viewModel.StatusText = "布局练习：先只把第 0 行高度 160 改为 190。";
    }
}
