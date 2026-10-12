using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfBae.Exercises.Task013;
using WpfBae.Exercises.Task014;

internal static class Review
{
    private static readonly List<object> Results = new List<object>();
    private static readonly List<string> Failures = new List<string>();
    private static string OutputDirectory = "";

    [STAThread]
    private static int Main(string[] args)
    {
        OutputDirectory = Path.GetFullPath(args[0]);
        Directory.CreateDirectory(OutputDirectory);
        Task014Window? window = null;
        try
        {
            Application app = new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            window = new Task014Window();
            window.Show();
            Pump(window);

            ReadingHistoryControl history = (ReadingHistoryControl)window.FindName("HistoryControl");
            ListBox list = Descendants(history).OfType<ListBox>().Single();
            Button clear = (Button)history.FindName("ClearButton");
            Button read = (Button)window.FindName("ReadButton");
            TextBox input = (TextBox)window.FindName("DeviceIdTextBox");
            TextBlock status = (TextBlock)window.FindName("StatusTextBlock");
            TextBlock heading = Descendants(history).OfType<TextBlock>().Single(x => x.Text == "读取历史");
            ReadingViewModel vm = (ReadingViewModel)window.DataContext;
            object originalReadings = vm.Readings;
            object originalReadCommand = read.Command;
            object originalClearCommand = clear.Command;
            int collectionChanges = 0;
            vm.Readings.CollectionChanged += (sender, e) => collectionChanges++;

            void CheckConnections()
            {
                Require(window.IsVisible && window.IsLoaded, "窗口已经显示并加载");
                Require(ReferenceEquals(history.DataContext, vm), "历史控件继承同一 ViewModel");
                Require(ReferenceEquals(list.DataContext, vm) && ReferenceEquals(clear.DataContext, vm), "内部列表与按钮共用 ViewModel");
                Require(ReferenceEquals(vm.Readings, originalReadings) && ReferenceEquals(list.ItemsSource, originalReadings), "原历史集合和 ItemsSource 未替换");
                Require(ReferenceEquals(read.Command, vm.ReadCommand) && ReferenceEquals(clear.Command, vm.ClearCommand), "按钮命令与 ViewModel 命令一致");
                Require(ReferenceEquals(read.Command, originalReadCommand) && ReferenceEquals(clear.Command, originalClearCommand), "命令对象未替换");
                CheckBinding(list, ItemsControl.ItemsSourceProperty, "Readings", BindingMode.OneWay);
                CheckBinding(clear, Button.CommandProperty, "ClearCommand", BindingMode.OneWay);
                CheckBinding(read, Button.CommandProperty, "ReadCommand", BindingMode.OneWay);
                CheckBinding(input, TextBox.TextProperty, "DeviceId", BindingMode.TwoWay, UpdateSourceTrigger.PropertyChanged);
                CheckBinding(status, TextBlock.TextProperty, "StatusText", BindingMode.OneWay);
                Require(list.Items.Count == vm.Readings.Count, "列表数量等于集合数量");
                Require(status.Text == vm.StatusText, "状态提示经绑定同步");
                Require(clear.IsEnabled == (vm.Readings.Count > 0), "清空按钮条件状态正确");
                Require(read.IsEnabled, "读取按钮可用");
                foreach (var element in new FrameworkElement[] { heading, list, clear, input, read, status })
                {
                    Point point = element.TranslatePoint(new Point(0, 0), window);
                    Require(element.IsVisible && element.ActualHeight > 0 && element.ActualWidth > 0, "控件有可见布局尺寸");
                    Require(point.Y >= 0 && point.Y + element.ActualHeight <= window.ActualHeight && point.X >= 0 && point.X + element.ActualWidth <= window.ActualWidth, "控件在窗口范围内");
                }
            }

            void Record(string scenario, int expectedCount, string expectedStatus, params string[] expectedIds)
            {
                CheckConnections();
                Require(vm.Readings.Count == expectedCount, scenario + "：历史数量正确");
                Require(status.Text == expectedStatus, scenario + "：最终提示正确");
                Require(vm.Readings.Select(x => x.DeviceId).SequenceEqual(expectedIds), scenario + "：当前 ID 与旧记录正确");
                Require(vm.Readings.All(x => x.Value == 26.5 && x.Unit == "°C"), scenario + "：温度与单位正确");
                Require(Descendants(list).OfType<TextBlock>().Select(x => x.Text).Where(x => !string.IsNullOrEmpty(x)).SequenceEqual(vm.Readings.Select(x => x.ToString())), scenario + "：实际列表行文字正确");
                Results.Add(new { scenario, passed = true, windowTitle = window.Title, input = input.Text, viewModelDeviceId = vm.DeviceId, count = list.Items.Count, ids = vm.Readings.Select(x => x.DeviceId).ToArray(), displayedRows = Descendants(list).OfType<TextBlock>().Select(x => x.Text).ToArray(), status = status.Text, clearEnabled = clear.IsEnabled, collectionChanges, sameViewModel = ReferenceEquals(history.DataContext, vm), sameCollection = ReferenceEquals(list.ItemsSource, originalReadings) });
            }

            void ReadId(string id)
            {
                // 保留输入绑定，先经过 TextBox 写回，再经按钮自动化触发 WPF 命令路径。
                input.SetCurrentValue(TextBox.TextProperty, id);
                Pump(window);
                Require(vm.DeviceId == id, "按钮执行前输入已经写回对象");
                Invoke(read);
                Pump(window);
            }

            Record("初始空历史，清空禁用", 0, "TASK-014：先完成控件内的历史标题（TODO 1）。");
            ReadId("TEMP-001");
            Record("首次读取，清空启用", 1, "读取成功", "TEMP-001");
            object firstReading = vm.Readings[0];
            ReadId("TEMP-001");
            Record("同一 ID 连续读取保留旧记录", 2, "读取成功", "TEMP-001", "TEMP-001");
            ReadId("TEMP-002");
            Record("修改 ID 后追加当前读数", 3, "读取成功", "TEMP-001", "TEMP-001", "TEMP-002");
            Require(ReferenceEquals(firstReading, vm.Readings[0]), "原记录对象保留");
            Snapshot(window, "history-three-readings.png");

            int beforeInvalid = collectionChanges;
            ReadId("");
            Record("空字符串提示且保持历史", 3, "该 Id 不合法", "TEMP-001", "TEMP-001", "TEMP-002");
            Require(collectionChanges == beforeInvalid, "空字符串未修改集合");
            ReadId("   ");
            Record("纯空格提示且保持历史", 3, "该 Id 不合法", "TEMP-001", "TEMP-001", "TEMP-002");
            Require(collectionChanges == beforeInvalid, "纯空格未修改集合");
            Invoke(clear);
            Pump(window);
            Record("实际点击控件内清空，列表为空并禁用", 0, "已清空");
            Snapshot(window, "history-cleared.png");
            ReadId("  TEMP-003  ");
            Record("清空后再次读取，Trim 后恢复", 1, "读取成功", "TEMP-003");
            Invoke(clear);
            Pump(window);
            Record("再次清空，条件恢复禁用", 0, "已清空");
            window.Close();
            Pump(window);
            app.Shutdown();
        }
        catch (Exception error)
        {
            Failures.Add(error.ToString());
            if (window != null) window.Close();
        }
        File.WriteAllText(Path.Combine(OutputDirectory, "results.json"), JsonSerializer.Serialize(new { reviewedTask = "TASK-014", windowActuallyShown = true, interaction = "TextBox.SetCurrentValue + ButtonAutomationPeer/IInvokeProvider.Invoke", scenarios = Results, failures = Failures, passed = Failures.Count == 0 && Results.Count == 9 }, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"TASK-014: {Results.Count}/9 场景通过；失败 {Failures.Count} 项。");
        foreach (string failure in Failures) Console.WriteLine(failure);
        return Failures.Count == 0 && Results.Count == 9 ? 0 : 1;
    }

    private static void Require(bool condition, string explanation)
    {
        if (!condition) throw new InvalidOperationException(explanation);
    }

    private static void CheckBinding(DependencyObject target, DependencyProperty property, string path, BindingMode mode, UpdateSourceTrigger? trigger = null)
    {
        BindingExpression? expression = BindingOperations.GetBindingExpression(target, property);
        Require(expression != null && expression.Status == BindingStatus.Active, path + " 的绑定有效");
        Require(expression!.ParentBinding.Path.Path == path && expression.ParentBinding.Mode == mode, path + " 的路径与方向正确");
        if (trigger != null) Require(expression.ParentBinding.UpdateSourceTrigger == trigger, "输入写回时机正确");
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            yield return child;
            foreach (DependencyObject descendant in Descendants(child)) yield return descendant;
        }
    }

    private static void Invoke(Button button)
    {
        Require(button.IsEnabled, "自动化调用的按钮当前可用");
        ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
        IInvokeProvider provider = (IInvokeProvider)peer.GetPattern(PatternInterface.Invoke);
        provider.Invoke();
    }

    private static void Pump(Window window)
    {
        DispatcherFrame frame = new DispatcherFrame();
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        timer.Interval = TimeSpan.FromMilliseconds(100);
        timer.Tick += (sender, e) => { timer.Stop(); frame.Continue = false; };
        timer.Start();
        Dispatcher.PushFrame(frame);
        window.UpdateLayout();
    }

    private static void Snapshot(Window window, string filename)
    {
        FrameworkElement content = (FrameworkElement)window.Content;
        RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Math.Ceiling(content.ActualWidth), (int)Math.Ceiling(content.ActualHeight), 96, 96, PixelFormats.Pbgra32);
        // 内容区域自身背景透明；导出时补窗口的白色底，避免预览器黑底遮住文字。
        DrawingVisual background = new DrawingVisual();
        using (DrawingContext drawing = background.RenderOpen())
        {
            drawing.DrawRectangle(Brushes.White, null, new Rect(0, 0, content.ActualWidth, content.ActualHeight));
        }
        bitmap.Render(background);
        bitmap.Render(content);
        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using FileStream stream = File.Create(Path.Combine(OutputDirectory, filename));
        encoder.Save(stream);
    }
}
