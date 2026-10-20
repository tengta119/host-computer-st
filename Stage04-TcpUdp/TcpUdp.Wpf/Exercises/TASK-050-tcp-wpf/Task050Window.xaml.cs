using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TcpUdp.Wpf.Exercises.Task050
{
    public partial class Task050Window : Window
    {
        private readonly Task050TcpConnection _connection = new Task050TcpConnection();
        private readonly ObservableCollection<Task050Reading> _history = new ObservableCollection<Task050Reading>();
        private readonly ObservableCollection<Task050LogEntry> _logs = new ObservableCollection<Task050LogEntry>();
        private readonly DispatcherTimer _pollingTimer = new DispatcherTimer();
        private CancellationTokenSource? _sessionSource;
        private int _sessionId;
        private int _collectionGeneration;
        private bool _busy;
        private bool _collecting;
        private bool _closing;
        private bool _ready;
        private double _threshold = 35;

        public Task050Window()
        {
            InitializeComponent();
            HistoryGrid.ItemsSource = _history;
            LogGrid.ItemsSource = _logs;
            _history.CollectionChanged += HistoryChanged;
            _pollingTimer.Interval = TimeSpan.FromSeconds(1);
            _pollingTimer.Tick += PollingTick;
            _ready = true;
            RefreshControls();
            RefreshPresentation();
            RecordLog("窗口", "窗口已就绪，先完成TODO 1连接模拟器。");
        }

        private async void ConnectClick(object sender, RoutedEventArgs e)
        {
            if (_closing || _busy)
            {
                return;
            }
            int port;
            string host = HostInput.Text.Trim();
            if (host.Length == 0 || !Int32.TryParse(PortInput.Text, out port) || port < 1 || port > 65535)
            {
                OperationText.Text = "请填写地址和1至65535之间的端口。";
                return;
            }
            ResetConnection("");
            _sessionSource = new CancellationTokenSource();
            int session = _sessionId;
            CancellationToken token = _sessionSource.Token;
            _busy = true;
            RefreshControls();
            OperationText.Text = "正在连接：" + host + ":" + port;
            try
            {
                await ConnectCoreAsync(host, port, token);
                if (!IsCurrentSession(session))
                {
                    return;
                }
                if (!_connection.IsConnected)
                {
                    OperationText.Text = "TODO 1尚未接通连接，不把占位当作连接成功。";
                    return;
                }
                ConnectionText.Text = "已连接：" + host + ":" + port;
                OperationText.Text = "连接成功，可以读取温度或查询设备状态。";
                RecordLog("连接", ConnectionText.Text);
            }
            catch (Exception exception)
            {
                HandleFailure(session, exception);
            }
            finally
            {
                _busy = false;
                RefreshControls();
            }
        }

        private async Task ConnectCoreAsync(string host, int port, CancellationToken token)
        {
            // TODO 1：await _connection.ConnectAsync(host, port, token)，移除下一行占位，成功显示和错误处理已由外层提供。
            await Task.CompletedTask;
        }

        private async Task<string?> SendCoreAsync(string command, CancellationToken token)
        {
            // TODO 2a：return await _connection.RequestAsync(command, token)，替换下面两行占位，不再直接操作Socket或重复解析字节。
            await Task.CompletedTask;
            return null;
        }

        private async void ReadClick(object sender, RoutedEventArgs e)
        {
            await RunCommandAsync("GET_TEMP", false);
        }

        private async void StartDeviceClick(object sender, RoutedEventArgs e)
        {
            await RunCommandAsync("START_DEVICE", false);
        }

        private async void StopDeviceClick(object sender, RoutedEventArgs e)
        {
            await RunCommandAsync("STOP_DEVICE", false);
        }

        private async void StatusClick(object sender, RoutedEventArgs e)
        {
            await RunCommandAsync("GET_STATUS", false);
        }

        // 教练提供串行请求和旧结果保护，所有按钮与定时查询都经过这一个入口。
        private async Task RunCommandAsync(string command, bool fromPolling)
        {
            if (_closing || _busy || !_connection.IsConnected || _sessionSource == null)
            {
                return;
            }
            int session = _sessionId;
            int generation = _collectionGeneration;
            CancellationToken token = _sessionSource.Token;
            _busy = true;
            RefreshControls();
            RecordLog("请求", "准备请求：" + command);
            OperationText.Text = "等待" + command + "的完整回复。";
            try
            {
                string? reply = await SendCoreAsync(command, token);
                if (!IsCurrentSession(session))
                {
                    return;
                }
                if (reply == null)
                {
                    OperationText.Text = "TODO 2a尚未接通请求，没有发送命令。";
                    return;
                }
                RecordLog("完整回复", command + " → " + reply);
                if (fromPolling && (!_collecting || generation != _collectionGeneration))
                {
                    RecordLog("采集", "停止或重新开始前发出的查询已结束，忽略其温度结果。");
                    return;
                }
                ApplyReply(command, reply);
            }
            catch (Exception exception)
            {
                HandleFailure(session, exception);
            }
            finally
            {
                _busy = false;
                RefreshControls();
            }
        }

        private void ApplyReply(string command, string reply)
        {
            if (command == "GET_TEMP")
            {
                ApplyTemperature(reply);
                return;
            }
            string state;
            if (command == "GET_STATUS")
            {
                if (reply != "RUNNING" && reply != "STOPPED")
                {
                    throw new InvalidDataException("设备状态回复不合法：" + reply);
                }
                state = reply;
            }
            else
            {
                if (reply != "OK")
                {
                    throw new InvalidDataException("设备未确认本次控制：" + reply);
                }
                if (command == "START_DEVICE")
                {
                    state = "RUNNING";
                }
                else
                {
                    state = "STOPPED";
                }
            }
            DisplayDeviceState(state);
        }

        private void DisplayDeviceState(string state)
        {
            // TODO 2b：把已由完整回复确认的state显示到DeviceStatusText，RUNNING显示运行、STOPPED显示停止，并更新OperationText。
            OperationText.Text = "收到有效设备确认，状态显示待完成（TODO 2b）。";
        }

        private void ApplyTemperature(string reply)
        {
            double temperature;
            if (!Double.TryParse(reply, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out temperature) || !Double.IsFinite(temperature))
            {
                throw new InvalidDataException("温度回复不是约定的有限小数：" + reply);
            }
            // TODO 3：创建Task050Reading，将ReceivedAt设为DateTime.Now、Temperature设为temperature，再加入_history。
            // TODO 3：完成后把下一行改为实际读取成功提示，历史变化会自动刷新已有的表格、最新值、曲线和报警。
            OperationText.Text = "温度已解析，历史追加待完成（TODO 3）。";
        }

        private void StartCollectionClick(object sender, RoutedEventArgs e)
        {
            if (_closing || _busy || _collecting || !_connection.IsConnected)
            {
                return;
            }
            // TODO 4：将_collecting设为true，递增_collectionGeneration，启动_pollingTimer，调用RefreshControls并更新OperationText。
            OperationText.Text = "TODO 4尚未开始采集。";
        }

        private void StopCollectionClick(object sender, RoutedEventArgs e)
        {
            StopCollectionSafely();
            OperationText.Text = "已停止采集，本轮未完成的采集结果不会再加入历史。";
            RecordLog("采集", OperationText.Text);
            RefreshControls();
        }

        private async void PollingTick(object? sender, EventArgs e)
        {
            if (_collecting && !_closing && !_busy && _connection.IsConnected)
            {
                await RunCommandAsync("GET_TEMP", true);
            }
        }

        private void StopCollectionSafely()
        {
            _collecting = false;
            _collectionGeneration++;
            _pollingTimer.Stop();
        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            ResetConnection("已主动断开连接。");
        }

        private void ResetConnection(string reason)
        {
            _sessionId++;
            StopCollectionSafely();
            if (_sessionSource != null)
            {
                _sessionSource.Cancel();
                _sessionSource.Dispose();
                _sessionSource = null;
            }
            _connection.Close();
            if (!_closing)
            {
                ConnectionText.Text = "未连接";
                DeviceStatusText.Text = "未知（连接后查询）";
                if (reason.Length > 0)
                {
                    OperationText.Text = reason;
                    RecordLog("连接", reason);
                }
                RefreshControls();
            }
        }

        private bool IsCurrentSession(int session)
        {
            return !_closing && session == _sessionId && _sessionSource != null && !_sessionSource.IsCancellationRequested;
        }

        private void HandleFailure(int session, Exception exception)
        {
            if (!IsCurrentSession(session))
            {
                return;
            }
            string message = "通信失败：" + exception.Message;
            if (exception is OperationCanceledException)
            {
                message = "本次连接或请求超过3秒，旧连接已关闭，请手动重新连接。";
            }
            ResetConnection(message);
        }

        private void RefreshControls()
        {
            if (!_ready || _closing)
            {
                return;
            }
            bool connected = _connection.IsConnected;
            HostInput.IsEnabled = !connected && !_busy;
            PortInput.IsEnabled = !connected && !_busy;
            ConnectButton.IsEnabled = !connected && !_busy;
            DisconnectButton.IsEnabled = _sessionSource != null;
            ReadButton.IsEnabled = connected && !_busy;
            StartDeviceButton.IsEnabled = connected && !_busy;
            StopDeviceButton.IsEnabled = connected && !_busy;
            StatusButton.IsEnabled = connected && !_busy;
            StartCollectionButton.IsEnabled = connected && !_busy && !_collecting;
            StopCollectionButton.IsEnabled = _collecting;
            ClearButton.IsEnabled = !_busy && !_collecting;
            CollectionText.Text = _collecting ? "采集中（约每秒查询）" : "采集已停止";
        }

        private void RecordLog(string kind, string details)
        {
            if (_closing)
            {
                return;
            }
            Task050LogEntry entry = new Task050LogEntry();
            entry.Time = DateTime.Now;
            entry.Kind = kind;
            entry.Details = details;
            _logs.Add(entry);
            while (_logs.Count > 200)
            {
                _logs.RemoveAt(0);
            }
        }

        private void HistoryChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshPresentation();
        }

        private void RefreshPresentation()
        {
            if (!_ready || _closing)
            {
                return;
            }
            HistoryCountText.Text = "有效温度历史：" + _history.Count + "条";
            if (_history.Count == 0)
            {
                LatestText.Text = "最新温度：—";
                AlarmText.Text = "暂无温度，尚未判断报警。";
                AlarmText.Foreground = Brushes.SlateGray;
            }
            else
            {
                double value = _history[_history.Count - 1].Temperature;
                LatestText.Text = "最新温度：" + value.ToString("F1", CultureInfo.InvariantCulture) + " ℃";
                bool alarm = value > _threshold;
                AlarmText.Text = alarm ? "温度超过阈值，报警。" : "温度未超过阈值。";
                AlarmText.Foreground = alarm ? Brushes.Firebrick : Brushes.SeaGreen;
            }
            Task050Chart.Draw(ChartCanvas, _history, _threshold);
        }

        private void ApplyThresholdClick(object sender, RoutedEventArgs e)
        {
            double threshold;
            if (!Double.TryParse(ThresholdInput.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out threshold) || !Double.IsFinite(threshold) || threshold < 0 || threshold > 100)
            {
                OperationText.Text = "阈值请输入0至100之间的小数，使用点号作为小数点。";
                return;
            }
            _threshold = threshold;
            RefreshPresentation();
            OperationText.Text = "已应用报警阈值。";
        }

        private void ClearHistoryClick(object sender, RoutedEventArgs e)
        {
            if (!_busy && !_collecting)
            {
                _history.Clear();
            }
        }

        private void ChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshPresentation();
        }

        protected override void OnClosed(EventArgs e)
        {
            _closing = true;
            try
            {
                ResetConnection("");
                _pollingTimer.Tick -= PollingTick;
                _history.CollectionChanged -= HistoryChanged;
            }
            finally
            {
                base.OnClosed(e);
            }
        }
    }
}
