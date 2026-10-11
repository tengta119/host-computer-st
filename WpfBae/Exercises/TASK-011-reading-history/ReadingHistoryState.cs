using System.Collections.ObjectModel;
using WpfBae.Exercises.Task010;

namespace WpfBae.Exercises.Task011;

// 教练为本任务新建的数据类，名字由我们自己取，不是 WPF 自带的类型。
public class ReadingHistoryState
{
    // ObservableCollection 来自 .NET；Reading 复用 TASK-010 中你写的读数类。
    // new 创建一个空集合；字段 _readings 保存这个集合对象。
    private ObservableCollection<Reading> _readings = new ObservableCollection<Reading>();

    // Binding 按名称读取公开属性 Readings，取得上面那个集合。
    // 使用完整 getter：读取 _state.Readings 时，执行 return 并返回集合。
    // 没有 setter，不能给这个属性另换一个集合；仍然可以对原集合 Add/Clear。
    public ObservableCollection<Reading> Readings
    {
        get
        {
            return _readings;
        }
    }
}
