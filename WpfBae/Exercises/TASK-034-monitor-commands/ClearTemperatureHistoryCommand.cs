using System.Windows.Input;

namespace WpfBae.Exercises.Task034
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
            // TODO 2a（实践2 / DoD 2）：根据_viewModel.IsSampling返回能否清空，停止允许、采集中拒绝，不添加新的历史非空要求。
            return false;
        }

        public void Execute(object? parameter)
        {
            // TODO 2b（实践2 / DoD 1/2）：先检查CanExecute(parameter)，不允许就return，允许时调用原_viewModel.ClearHistory。
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
