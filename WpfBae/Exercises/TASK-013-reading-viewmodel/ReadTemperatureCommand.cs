using System;
using System.Windows.Input;
using WpfBae.Exercises.Task010;

namespace WpfBae.Exercises.Task013;

// 教练自定义的读取命令类；ICommand 来自 .NET，供 WPF 按钮使用。
public class ReadTemperatureCommand : ICommand
{
    private ReadingViewModel _viewModel;

    public ReadTemperatureCommand(ReadingViewModel viewModel)
    {
        // 保存窗口 DataContext 指向的同一个对象，不在这里 new 另一份。
        _viewModel = viewModel;
    }

    public bool CanExecute(object? parameter)
    {
        // 读取按钮始终可用；空白校验放在 Execute 中，保留原界面行为。
        return true;
    }

    public void Execute(object? parameter)
    {
        // TODO 1（首个动作）：在这里读取 _viewModel.DeviceId，保存为局部 string id。
        // 把下一行改为 _viewModel.StatusText = "命令取得的设备 ID：" + id;
        // 先只做这一步：改变输入并点击读取，观察提示是否跟随当前输入。
        string id = _viewModel.DeviceId;
        _viewModel.StatusText = "命令取得的设备 ID：" + id;
        

        // TODO 2：看清输入连接后，取得 ID 时调用 Trim()；检查空字符串/纯空格。
        // 无效输入时设置 StatusText 提示并 return，不改变原 Readings。
        id = id.Trim();
        if (id.Length == 0)
        {
            _viewModel.StatusText = "该 Id 不合法";
            return;
        }
 
        // TODO 3：有效输入时创建已有 Reading 对象，使用当前 id、26.5、°C，
        // 调用 _viewModel.Readings.Add(...) 追加到原集合；不要新建集合替换历史。
        _viewModel.Readings.Add(new Reading{DeviceId = id, Value = 26.5, Unit = "°C"});
        
        // TODO 4：追加成功后将 StatusText 设置为“读取成功”等成功提示。
        // 不访问 TextBox/TextBlock/ListBox；界面从数据对象和集合自动更新。
        _viewModel.StatusText = "读取成功";
    }

    // 接口要求此事件。条件固定为 true，当前无需发通知；完整工具由骨架提供。
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
