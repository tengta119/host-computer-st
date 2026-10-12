using System.Windows.Controls;

namespace WpfBae.Exercises.Task014
{
    // ReadingHistoryControl 是本任务自定义的类名；UserControl 是 WPF 提供的基类。
    // partial 是 C# 关键字：此文件与 XAML 编译生成的部分共同组成同一个类。
    public partial class ReadingHistoryControl : UserControl
    {
        public ReadingHistoryControl()
        {
            // WPF 构建时生成此方法；执行后创建 XAML 中描述的内部界面。
            InitializeComponent();
            // 沿用父窗口的 DataContext；不在这里 new ReadingViewModel。
        }
    }
}
