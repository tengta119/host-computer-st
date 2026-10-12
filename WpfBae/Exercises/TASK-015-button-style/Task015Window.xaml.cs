using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task015
{
    // 本题自定义窗口类；Window 是 WPF 提供的基类。
    public partial class Task015Window : Window
    {
        // 复用已有自定义类型，创建本窗口使用的一个实例；原任务的代码不变。
        private ReadingViewModel _viewModel = new ReadingViewModel();

        public Task015Window()
        {
            InitializeComponent();
            _viewModel.StatusText = "TASK-015：先完成 TODO 1，观察读取按钮的内边距。";
            // 数据和命令来自此对象；Style 则来自 XAML 的 Window.Resources。
            DataContext = _viewModel;
        }
    }
}
