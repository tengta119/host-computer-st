using System.Windows;

namespace WpfBae.Exercises.Task013;

public partial class Task013Window : Window
{
    // ReadingViewModel 是教练新建的普通 C# 类，不是 WPF 自带类型。
    // new 创建一个对象；_viewModel 字段保存这个对象的引用。
    private ReadingViewModel _viewModel = new ReadingViewModel();

    public Task013Window()
    {
        InitializeComponent();
        // DataContext 是 WPF 提供的属性，内部 Binding 从此对象找公开属性。
        DataContext = _viewModel;
    }
}
