using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task019;

public partial class Task019Window : Window
{
    // ReadingViewModel 是此前自定义的类，复用其已完成的读取/清空命令。
    // new 创建本窗口使用的一个对象；_viewModel 保存它的引用。
    private readonly ReadingViewModel _viewModel = new ReadingViewModel();

    public Task019Window()
    {
        InitializeComponent();
        // DataContext 是 WPF 属性；输入、按钮和之后的 ItemsSource 绑定从此对象取值。
        DataContext = _viewModel;
        // 只设置新窗口的初始提示，不修改原 ViewModel 文件或旧任务作品。
        _viewModel.StatusText = "先完成 TODO 1：把第一列表头改为设备 ID。表格数据连接尚未完成。";
    }
}
