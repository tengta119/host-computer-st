using System.Windows.Controls;

namespace WpfBae.Exercises.Task016
{
    // 本任务的自定义历史控件；XAML/后台连接由教练提供，核心字段绑定留作 TODO。
    public partial class ReadingHistoryControl : UserControl
    {
        public ReadingHistoryControl()
        {
            InitializeComponent();
            // 沿用父窗口 DataContext，不创建另一份 ViewModel 或历史集合。
        }
    }
}
