using System;
using System.Windows.Input;

namespace WpfBae.Exercises.Task013;

// 复用 TASK-012 已验证的清空条件与动作，只将持有的数据类型改为 ReadingViewModel。
// 本任务不要求重新实现清空命令；原 TASK-012 文件保留。
public class ClearHistoryCommand : ICommand
{
    private ReadingViewModel _viewModel;

    public ClearHistoryCommand(ReadingViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public bool CanExecute(object? parameter)
    {
        return _viewModel.Readings.Count > 0;
    }

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        _viewModel.Readings.Clear();
        _viewModel.StatusText = "已清空";
    }

    public event EventHandler? CanExecuteChanged;

    // 普通自定义辅助方法：通知 WPF 重新调用 CanExecute，不执行清空。
    public void NotifyCanExecuteChanged()
    {
        EventHandler? handler = CanExecuteChanged;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
