using System.Windows.Input;

namespace WpfBae.Exercises.Task035
{
    // 教练新建的应用阈值命令，保存原ViewModel引用，不直接读取TextBox。
    public class ApplyThresholdCommand : ICommand
    {
        private readonly TemperatureMonitorViewModel _viewModel;

        public ApplyThresholdCommand(TemperatureMonitorViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            // 输入有效性在执行时说明原因，保留停止和采集中均可尝试应用的原行为。
            return true;
        }

        public void Execute(object? parameter)
        {
            // TODO 2a（实践2 / DoD 1）：调用已有_viewModel.ApplyThreshold一次，不读取控件或创建另一份VM。
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
