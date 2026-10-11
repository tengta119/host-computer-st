using System.Collections.Specialized;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using WpfBae.Exercises.Task012;

internal static class Program
{
    static readonly List<object> Results = new();
    static int exitCode;
    [STAThread]
    static int Main(string[] args)
    {
        Application app = new Application();
        Task012Window window = new Task012Window { ShowInTaskbar = false };
        window.Loaded += delegate {
            window.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => {
                try { Verify(window); }
                catch (Exception ex) { exitCode = 1; Results.Add(new { passed = false, error = ex.ToString() }); }
                finally {
                    System.IO.File.WriteAllText(args[0], JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
                    window.Close();
                }
            }));
        };
        app.Run(window);
        Console.WriteLine(JsonSerializer.Serialize(new { passed = exitCode == 0, cases = Results.Count, results = args[0] }));
        return exitCode;
    }
    static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Flush(Window window)
    {
        window.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        window.UpdateLayout();
    }
    static void Click(Button button, Window window)
    {
        Assert(button.IsEnabled, "Attempted to click disabled button: " + button.Name);
        ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
        IInvokeProvider provider = (IInvokeProvider)peer.GetPattern(PatternInterface.Invoke);
        provider.Invoke();
        Flush(window);
    }
    static IEnumerable<TextBlock> TextBlocks(DependencyObject node)
    {
        if (node is TextBlock text) yield return text;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(node); i++)
            foreach (TextBlock child in TextBlocks(VisualTreeHelper.GetChild(node, i))) yield return child;
    }
    static void Verify(Task012Window window)
    {
        Assert(window.IsVisible && new WindowInteropHelper(window).Handle != IntPtr.Zero, "Window not actually shown");
        TextBox input = (TextBox)window.FindName("DeviceIdTextBox");
        Button read = (Button)window.FindName("ReadButton");
        Button clear = (Button)window.FindName("ClearButton");
        ListBox list = (ListBox)window.FindName("HistoryListBox");
        TextBlock status = (TextBlock)window.FindName("StatusTextBlock");
        ReadingCommandState state = (ReadingCommandState)window.DataContext;
        object collection = state.Readings;
        object command = state.ClearCommand;
        object? windowState = typeof(Task012Window).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window);
        object? commandState = typeof(ClearHistoryCommand).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(command);
        Assert(ReferenceEquals(windowState, state) && ReferenceEquals(commandState, state), "Different state objects");
        Binding? commandBinding = BindingOperations.GetBinding(clear, Button.CommandProperty);
        Binding? listBinding = BindingOperations.GetBinding(list, ItemsControl.ItemsSourceProperty);
        Binding? statusBinding = BindingOperations.GetBinding(status, TextBlock.TextProperty);
        Assert(commandBinding?.Path.Path == "ClearCommand" && commandBinding.Mode == BindingMode.OneWay, "Wrong Command binding");
        Assert(listBinding?.Path.Path == "Readings" && listBinding.Mode == BindingMode.OneWay, "Wrong ItemsSource binding");
        Assert(statusBinding?.Path.Path == "StatusText" && statusBinding.Mode == BindingMode.OneWay, "Wrong status binding");
        List<string> collectionEvents = new();
        int commandEvents = 0;
        int statusEvents = 0;
        state.Readings.CollectionChanged += (sender, e) => {
            Assert(ReferenceEquals(sender, collection), "Wrong collection sender"); collectionEvents.Add(e.Action.ToString());
        };
        state.ClearCommand.CanExecuteChanged += (sender, e) => {
            Assert(ReferenceEquals(sender, command), "Wrong command sender"); commandEvents++;
        };
        state.PropertyChanged += (sender, e) => { if (e.PropertyName == "StatusText") statusEvents++; };
        void ResetEvents() { collectionEvents.Clear(); commandEvents = 0; statusEvents = 0; }
        void Snapshot(string name, string[] ids, bool enabled, string expectedStatus, string? expectedAction)
        {
            Flush(window);
            Assert(ReferenceEquals(state.Readings, collection) && ReferenceEquals(list.ItemsSource, collection), name + ": collection replaced or wrong ItemsSource");
            Assert(ReferenceEquals(clear.Command, command), name + ": command reference changed");
            Assert(state.Readings.Count == ids.Length && list.Items.Count == ids.Length, name + ": wrong row count");
            Assert(clear.IsEnabled == enabled && state.ClearCommand.CanExecute(null) == enabled, name + ": wrong enabled state");
            Assert(status.Text == expectedStatus && state.StatusText == expectedStatus, name + ": wrong status");
            List<string> renderedRows = new();
            for (int i = 0; i < ids.Length; i++)
            {
                var reading = state.Readings[i];
                Assert(reading.DeviceId == ids[i] && reading.Value == 26.5 && reading.Unit == "°C", name + ": wrong reading");
                Assert(ReferenceEquals(list.Items[i], reading), name + ": wrong row object");
                var container = (ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(i);
                Assert(container != null, name + ": missing rendered item");
                string rendered = string.Join("", TextBlocks(container!).Select(t => t.Text));
                Assert(rendered == reading.ToString(), name + ": wrong rendered text: " + rendered);
                renderedRows.Add(rendered);
            }
            Assert(collectionEvents.SequenceEqual(expectedAction == null ? Array.Empty<string>() : new[] { expectedAction }), name + ": wrong collection notifications");
            Assert(commandEvents == (expectedAction == null ? 0 : 1), name + ": wrong command notifications");
            Assert(statusEvents == (name == "initial" ? 0 : 1), name + ": wrong status notifications");
            Results.Add(new { name, passed = true, rows = ids.Length, renderedRows, clearEnabled = clear.IsEnabled, canExecute = state.ClearCommand.CanExecute(null), status = status.Text, collectionEvents = collectionEvents.ToArray(), commandEvents, statusEvents, sameState = true, sameCollection = true, commandBinding = commandBinding!.Path.Path });
        }
        Snapshot("initial", Array.Empty<string>(), false, state.StatusText, null);
        ResetEvents(); input.Text = "TEMP-001"; Click(read, window);
        Snapshot("first-read", new[] { "TEMP-001" }, true, "读取成功", "Add");
        ResetEvents(); input.Text = "TEMP-002"; Click(read, window);
        Snapshot("second-read-retains-first", new[] { "TEMP-001", "TEMP-002" }, true, "读取成功", "Add");
        ResetEvents(); input.Text = ""; Click(read, window);
        Snapshot("empty-input-preserves-history", new[] { "TEMP-001", "TEMP-002" }, true, "请输入设备 ID；本次没有新增记录。", null);
        ResetEvents(); input.Text = "   "; Click(read, window);
        Snapshot("whitespace-preserves-history", new[] { "TEMP-001", "TEMP-002" }, true, "请输入设备 ID；本次没有新增记录。", null);
        ResetEvents(); Click(clear, window);
        Snapshot("clear-via-button-invoke", Array.Empty<string>(), false, "已清空", "Reset");
        ResetEvents(); input.Text = "  TEMP-003  "; Click(read, window);
        Snapshot("read-after-clear-trims-input", new[] { "TEMP-003" }, true, "读取成功", "Add");
        ResetEvents(); Click(clear, window);
        Snapshot("clear-again-via-button-invoke", Array.Empty<string>(), false, "已清空", "Reset");
    }
}
