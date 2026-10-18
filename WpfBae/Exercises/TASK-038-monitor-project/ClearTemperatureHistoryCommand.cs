using System.Windows.Input;

namespace WpfBae.Exercises.Task038
{
    public class ClearTemperatureHistoryCommand : ICommand
    {
        private readonly TemperatureMonitorViewModel _viewModel;

        public ClearTemperatureHistoryCommand(TemperatureMonitorViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            // TOD 4b（实践2/DoD2）：依据_viewModel.IsSampling返回能否清空，停止允许，采中拒绝，不增加非空条件。
            return !_viewModel.IsSampling;
        }

        public void Execute(object? parameter)
        {
            // TOD 4c（实践2/DoD1/2）：先检查CanExecute，不允许直接返回，允许时调用同一_viewModel.ClearHistory。
            if (CanExecute(parameter))
            {
                _viewModel.ClearHistory();
            }
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
