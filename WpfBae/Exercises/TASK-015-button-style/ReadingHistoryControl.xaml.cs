using System.Windows.Controls;

namespace WpfBae.Exercises.Task015
{
    // 新任务命名空间中的自定义控件类，沿用 TASK-014 的界面结构。
    public partial class ReadingHistoryControl : UserControl
    {
        public ReadingHistoryControl()
        {
            InitializeComponent();
            // 沿用父窗口 DataContext；不新建业务对象或样式资源。
        }
    }
}
