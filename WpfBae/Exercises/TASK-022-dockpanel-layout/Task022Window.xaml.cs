using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task022;

public partial class Task022Window : Window
{
    // ReadingViewModel 是 TASK-013 已有的自定义数据类。
    // new 创建本窗口的一份对象；原读取/清空命令仍使用它的同一历史集合。
    private readonly ReadingViewModel _viewModel = new ReadingViewModel();
    private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };

    public Task022Window()
    {
        InitializeComponent();
        DeviceComboBox.ItemsSource = _deviceIds;
        // DataContext 是 WPF 属性；三个区域通过它取得本窗口的同一个数据对象。
        DataContext = _viewModel;
        _viewModel.StatusText = "布局练习：先把选择区 Dock 从 Bottom 改为 Top。";
    }
}
