using System.Windows.Input;

namespace WpfBae.Exercises.Task038
{
    public class ApplyThresholdCommand : ICommand
    {
        private readonly TemperatureMonitorViewModel _viewModel;

        public ApplyThresholdCommand(TemperatureMonitorViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            // TOD 6b（实践3/DoD3）：VM.ApplyThreshold完成后，在这里调用同一_viewModel.ApplyThreshold一次。
            _viewModel.ApplyThreshold();
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
