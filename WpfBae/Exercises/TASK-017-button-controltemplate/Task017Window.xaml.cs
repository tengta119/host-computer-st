using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task017
{
    // 教练新建的任务窗口，复用原 ReadingViewModel 和两个命令的实现。
    public partial class Task017Window : Window
    {
        private ReadingViewModel _viewModel = new ReadingViewModel();

        public Task017Window()
        {
            InitializeComponent();
            _viewModel.StatusText = "TASK-017：先只改 TODO 1 的圆角，观察两个按钮。";
            // new 创建同一个自定义 ViewModel 实例；窗口与历史控件沿用它作为数据源。
            DataContext = _viewModel;
        }
    }
}
