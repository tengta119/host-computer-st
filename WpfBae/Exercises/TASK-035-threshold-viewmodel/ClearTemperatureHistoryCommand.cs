using System.Windows.Input;

namespace WpfBae.Exercises.Task035
{
    // 教练新建的清空命令类，窗口与此命令共用传入的原ViewModel。
    public class ClearTemperatureHistoryCommand : ICommand
    {
        private readonly TemperatureMonitorViewModel _viewModel;

        public ClearTemperatureHistoryCommand(TemperatureMonitorViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            // TASK-034已完成步骤2a（实践2 / DoD 2）：根据_viewModel.IsSampling返回能否清空，停止允许、采集中拒绝，不添加新的历史非空要求。
            return !_viewModel.IsSampling;
        }

        public void Execute(object? parameter)
        {
            // TASK-034已完成步骤2b（实践2 / DoD 1/2）：先检查CanExecute(parameter)，不允许就return，允许时调用原_viewModel.ClearHistory。
            if (!CanExecute(parameter))
            {
                return;
            }
            _viewModel.ClearHistory();
        }

        public event EventHandler? CanExecuteChanged;

        // 教练提供的普通通知方法，只要求按钮重查条件，不负责执行清空。
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
