using System;
using System.Windows.Input;

namespace WpfBae.Exercises.Task012;

// ClearHistoryCommand 是教练自定义类；ICommand 来自 .NET 的 System.Windows.Input。
// : ICommand 表示这个类实现接口，提供接口要求的两个方法和一个事件。
public class ClearHistoryCommand : ICommand
{
    private ReadingCommandState _state;

    public ClearHistoryCommand(ReadingCommandState state)
    {
        // 参数 state 是创建命令时传来的数据对象；保存它，不再新建另一份历史。
        _state = state;
    }

    public bool CanExecute(object? parameter)
    {
        // WPF 调用此方法询问按钮是否可以执行：true 可用，false 禁用。
        // object? parameter 是接口规定的可空参数，本任务不传命令参数，不需要使用它。
        // TODO 1（首个动作）：用 _state.Readings.Count > 0 的判断替换下面的 false。
        // Count 是集合的记录数量；> 0 的结果本身就是 bool。
        if (_state.Readings.Count > 0)
        {
            return true;
        }
        return false;
    }

    public void Execute(object? parameter)
    {
        // WPF 在按钮可用且正常点击时调用这个方法。
        // 防止其他代码直接调用时越过条件；! 表示“不是 true”。此检查由骨架提供。
        if (!CanExecute(parameter))
        {
            return;
        }

        // TODO 2：调用 _state.Readings.Clear()，清空同一份历史集合。
        // 清空会发出集合变化通知；列表更新和按钮重新检查条件均已连接。
        _state.Readings.Clear();

        // TODO 3：把下面的占位提示改为“已清空”等完成提示。
        _state.StatusText = "已清空";
    }

    // ICommand 要求的事件；EventHandler 来自 .NET 的 System。
    // WPF 按钮会订阅它；事件的作用是通知按钮重新调用 CanExecute。
    public event EventHandler? CanExecuteChanged;

    // 这是教练自己命名的通知工具，不是 ICommand 要求的方法。已提供完整实现。
    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
