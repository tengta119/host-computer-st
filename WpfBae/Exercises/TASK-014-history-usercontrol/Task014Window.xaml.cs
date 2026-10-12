using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task014
{
    public partial class Task014Window : Window
    {
        // 复用 TASK-013 的类及学员已完成的命令；本窗口创建自己的一个实例。
        // _viewModel 保存该实例的引用，窗口与内部历史控件共用这个对象。
        private ReadingViewModel _viewModel = new ReadingViewModel();

        public Task014Window()
        {
            InitializeComponent();
            _viewModel.StatusText = "TASK-014：先完成控件内的历史标题（TODO 1）。";
            DataContext = _viewModel;
        }
    }
}
