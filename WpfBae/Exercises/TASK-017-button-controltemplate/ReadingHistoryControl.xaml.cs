using System.Windows.Controls;

namespace WpfBae.Exercises.Task017
{
    // 教练提供的任务专属历史控件，沿用 TASK-016 的界面与父窗口数据源。
    public partial class ReadingHistoryControl : UserControl
    {
        public ReadingHistoryControl()
        {
            InitializeComponent();
            // 不另建 ViewModel、集合或命令。
        }
    }
}
