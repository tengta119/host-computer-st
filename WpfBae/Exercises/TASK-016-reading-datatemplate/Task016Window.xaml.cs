using System.Windows;
using WpfBae.Exercises.Task013;

namespace WpfBae.Exercises.Task016
{
    // 教练新建的任务窗口；业务数据与命令复用 TASK-013 的自定义类型。
    public partial class Task016Window : Window
    {
        private ReadingViewModel _viewModel = new ReadingViewModel();

        public Task016Window()
        {
            InitializeComponent();
            _viewModel.StatusText = "TASK-016：先完成 TODO 1，让每条历史显示自己的设备 ID。";
            // 窗口输入、列表集合与命令来自同一对象；模板内数据源是每条 Reading。
            DataContext = _viewModel;
        }
    }
}
