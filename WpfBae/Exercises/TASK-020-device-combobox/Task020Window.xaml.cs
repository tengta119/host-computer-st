using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task020;

public partial class Task020Window : Window
{
    // ReadingViewModel 是已有自定义类；new 创建本窗口使用的一个数据对象。
    // _viewModel 是教练起的字段名，保存这个对象的引用；其 DeviceId 初始为 TEMP-001。
    private readonly ReadingViewModel _viewModel = new ReadingViewModel();

    // string[] 是 C# 字符串数组写法，new string[] 创建含两个设备 ID 的数组。
    // _deviceIds 保存数组引用；这是可选设备列表，不是实际读数的 Readings 历史集合。
    private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };

    public Task020Window()
    {
        InitializeComponent();
        // DeviceComboBox 指向 XAML 中的那个具体下拉框。
        // ItemsSource 是 WPF 属性；把数组交给它，下拉框便有这两个可选项。
        DeviceComboBox.ItemsSource = _deviceIds;
        // DataContext 是 WPF 属性；选中值绑定、表格和两个命令使用同一个数据对象。
        DataContext = _viewModel;
        // 初始提示只用于本次骨架，不修改原 ViewModel 或旧任务文件。
        _viewModel.StatusText = "先完成 TODO 1：把下拉框宽度从 200 改为 260。选中值尚未连接 DeviceId。";
    }
}
