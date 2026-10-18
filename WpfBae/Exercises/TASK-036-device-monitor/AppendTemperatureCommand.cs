using System.Windows.Input;

namespace WpfBae.Exercises.Task036
{
    // 教练新建的追加命令类，ICommand是.NET提供给WPF按钮使用的接口。
    public class AppendTemperatureCommand : ICommand
    {
        private readonly TemperatureMonitorViewModel _viewModel;

        public AppendTemperatureCommand(TemperatureMonitorViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            // 原手动追加允许在停止或采集中使用，本题保持这个行为。
            return true;
        }

        public void Execute(object? parameter)
        {
            // TASK-034已完成步骤1a（实践1 / DoD 1）：调用已有_viewModel.AppendNextSample一次，不另建集合或ViewModel。
            _viewModel.AppendNextSample();
        }

        public event EventHandler? CanExecuteChanged;

        public void NotifyCanExecuteChanged()
        {
            EventHandler? handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
