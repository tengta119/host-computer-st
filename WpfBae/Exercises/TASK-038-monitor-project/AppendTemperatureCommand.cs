using System.Windows.Input;

namespace WpfBae.Exercises.Task038
{
    // 本题追加命令只保存当前新VM，接口和通知基础由教练预留。
    public class AppendTemperatureCommand : ICommand
    {
        private readonly TemperatureMonitorViewModel _viewModel;

        public AppendTemperatureCommand(TemperatureMonitorViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            // TOD 4a（实践2/DoD1/2）：只调用当前_viewModel的AppendNextSample一次。
            _viewModel.AppendNextSample();
        }

        public event EventHandler? CanExecuteChanged;

        public void NotifyCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
