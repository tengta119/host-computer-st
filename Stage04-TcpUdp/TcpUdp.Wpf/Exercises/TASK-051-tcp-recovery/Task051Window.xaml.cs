using TcpUdp.Wpf.Exercises.Task050;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TcpUdp.Wpf.Exercises.Task051
{
    public partial class Task051Window : Window
    {
        private readonly Task051TcpConnection _connection = new Task051TcpConnection();
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
        private readonly DispatcherTimer _heartbeatTimer = new DispatcherTimer();
        private readonly DispatcherTimer _reconnectTimer = new DispatcherTimer();
        private DateTime _lastActivity = DateTime.MinValue;
        private bool _wantConnected;
        private bool _recovering;
        private int _reconnectAttempts;
        private string _savedHost = "127.0.0.1";
        private int _savedPort = 5000;
        private const int MaximumReconnectAttempts = 5;
        private static readonly TimeSpan HeartbeatIdle = TimeSpan.FromSeconds(2);

        public Task051Window()
        {
            InitializeComponent();
            HistoryGrid.ItemsSource = _history;
            LogGrid.ItemsSource = _logs;
            _history.CollectionChanged += HistoryChanged;
            _pollingTimer.Interval = TimeSpan.FromSeconds(1);
            _pollingTimer.Tick += PollingTick;
            _heartbeatTimer.Interval = TimeSpan.FromMilliseconds(500);
            _heartbeatTimer.Tick += HeartbeatTick;
            _reconnectTimer.Interval = TimeSpan.FromSeconds(2);
            _reconnectTimer.Tick += ReconnectTick;
            _ready = true;
            RefreshControls();
            RefreshPresentation();
            RecordLog("窗口", "TASK-051已就绪，先观察故障后的清理与手动恢复。");
        }

        private async void ConnectClick(object sender, RoutedEventArgs e)
        {
            if (_closing || _busy) return;
            int port;
            string host = HostInput.Text.Trim();
            if (host.Length == 0 || !Int32.TryParse(PortInput.Text, out port) || port < 1 || port > 65535)
            {
                OperationText.Text = "请填写地址和1至65535之间的端口。";
                return;
            }
            _savedHost = host;
            _savedPort = port;
            _wantConnected = true;
            _reconnectAttempts = 0;
            _reconnectTimer.Stop();
            await AttemptConnectAsync(false);
        }

        // 教练保留单一连接入口，自动与手动尝试不会并行。
        private async Task AttemptConnectAsync(bool recovery)
        {
            if (_closing || _busy || !_wantConnected) return;
            ResetConnection("");
            _sessionSource = new CancellationTokenSource();
            int session = _sessionId;
            CancellationToken token = _sessionSource.Token;
            _busy = true;
            _recovering = recovery;
            RefreshControls();
            OperationText.Text = "正在连接：" + _savedHost + ":" + _savedPort;
            try
            {
                await ConnectCoreAsync(_savedHost, _savedPort, token);
                if (!IsCurrentSession(session)) return;
                ConnectionText.Text = "已连接：" + _savedHost + ":" + _savedPort;
                RecordLog("连接", ConnectionText.Text);
                await QueryStateAfterConnectAsync(token);
                if (!IsCurrentSession(session)) return;
                _lastActivity = DateTime.UtcNow;
                _heartbeatTimer.Start();
                _reconnectAttempts = 0;
                RecoveryText.Text = "连接已建立，采集保持停止。";
                OperationText.Text = "连接已建立，可查询设备状态；采集需手动开始。";
            }
            catch (Exception exception)
            {
                HandleFailure(session, exception);
            }
            finally
            {
                _busy = false;
                _recovering = false;
                RefreshControls();
            }
        }

        private async Task ConnectCoreAsync(string host, int port, CancellationToken token)
        {
            // TOD 1：await _connection.ConnectAsync(host, port, token)，移除下一行占位，成功显示和错误处理已由外层提供。
            await _connection.ConnectAsync(host, port, token);
        }

        private async Task<string?> SendCoreAsync(string command, CancellationToken token)
        {
            // TOD 2a：return await _connection.RequestAsync(command, token)，替换下面两行占位，不再直接操作Socket或重复解析字节。
            return await _connection.RequestAsync(command, token);
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
                _lastActivity = DateTime.UtcNow;
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
            if (command == "PING")
            {
                ValidateHeartbeatReply(reply);
                HeartbeatText.Text = "心跳正常：" + DateTime.Now.ToString("HH:mm:ss");
                OperationText.Text = "已收到匹配的PONG。";
                return;
            }
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
            // TOD 2b：把已由完整回复确认的state显示到DeviceStatusText，RUNNING显示运行、STOPPED显示停止，并更新OperationText。
            if (state == "RUNNING")
            {
                DeviceStatusText.Text = "运行";
            }
            else
            {
                DeviceStatusText.Text = "停止";
            }
            OperationText.Text = "设备已确认当前状态：" + DeviceStatusText.Text + "。";
        }

        private void ApplyTemperature(string reply)
        {
            double temperature;
            if (!Double.TryParse(reply, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out temperature) || !Double.IsFinite(temperature))
            {
                throw new InvalidDataException("温度回复不是约定的有限小数：" + reply);
            }
            // TOD 3：创建Task050Reading，将ReceivedAt设为DateTime.Now、Temperature设为temperature，再加入_history。
            // TOD 3：完成后把下一行改为实际读取成功提示，历史变化会自动刷新已有的表格、最新值、曲线和报警。
            Task050Reading reading = new Task050Reading{ReceivedAt = DateTime.Now, Temperature = temperature};
            _history.Add(reading);
            OperationText.Text = "温度已解析，历史追加完成";
        }

        private void StartCollectionClick(object sender, RoutedEventArgs e)
        {
            if (_closing || _busy || _collecting || !_connection.IsConnected)
            {
                return;
            }
            // TOD 4：将_collecting设为true，递增_collectionGeneration，启动_pollingTimer，调用RefreshControls并更新OperationText。
            _collecting = true;
            _collectionGeneration = _collectionGeneration + 1;
            _pollingTimer.Start();
            RefreshControls();
            OperationText.Text = "开始采集";
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
            _wantConnected = false;
            _reconnectTimer.Stop();
            _heartbeatTimer.Stop();
            _reconnectAttempts = 0;
            ResetConnection("已主动断开，不再自动重连。");
            RecoveryText.Text = "恢复流程已停止。";
        }

        private void ResetConnection(string reason)
        {
            _sessionId++;
            _heartbeatTimer.Stop();
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
                HeartbeatText.Text = "尚未确认心跳。";
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
            if (!IsCurrentSession(session)) return;
            string message = DescribeFailure(exception);
            ResetConnection(message);
            ScheduleReconnect();
        }

        private string DescribeFailure(Exception exception)
        {
            // TOD 1：区分Task051TimeoutException.Operation中的连接、业务请求、心跳，以及IOException/SocketException和InvalidDataException，返回明确原因。
            if (exception is Task051TimeoutException)
            {
                Task051TimeoutException timeout = (Task051TimeoutException)exception;
                return timeout.Operation + "等待超时，将结束旧连接。";
            }
            
            return "其它异常：" + exception.Message;
        }

        private void ScheduleReconnect()
        {
            if (_closing || !_wantConnected || AutoReconnectCheckBox.IsChecked != true)
            {
                if (!_closing) RecoveryText.Text = "自动重连未启动，可手动连接。";
                return;
            }
            if (_reconnectAttempts >= MaximumReconnectAttempts)
            {
                _wantConnected = false;
                _reconnectTimer.Stop();
                RecoveryText.Text = "已达到5次自动尝试上限，请手动连接。";
                RefreshControls();
                return;
            }
            // TOD 2：已有入口检查和次数上限通过后，设置下一次等待提示并启动_reconnectTimer，不直接递归调用连接方法。
            RecoveryText.Text = "2秒后进行第" + (_reconnectAttempts + 1) + "/" + MaximumReconnectAttempts + "次自动连接。";
            _reconnectTimer.Start();
        }

        private async void ReconnectTick(object? sender, EventArgs e)
        {
            _reconnectTimer.Stop();
            if (_closing || !_wantConnected || AutoReconnectCheckBox.IsChecked != true || _connection.IsConnected) return;
            if (_busy)
            {
                ScheduleReconnect();
                return;
            }
            if (_reconnectAttempts >= MaximumReconnectAttempts)
            {
                ScheduleReconnect();
                return;
            }
            _reconnectAttempts++;
            RecoveryText.Text = "正在进行第" + _reconnectAttempts + "/5次自动连接。";
            await AttemptConnectAsync(true);
        }

        private async Task QueryStateAfterConnectAsync(CancellationToken token)
        {
            // TOD 3：await _connection.RequestAsync("GET_STATUS", token)，记录完整回复并用ApplyReply("GET_STATUS", reply)显示真实状态，移除占位。
            // TOD 3：这里仍在连接步骤的忙状态内，不调用会被_busy拒绝的RunCommandAsync，不重发START_DEVICE/STOP_DEVICE或恢复采集。
            string requestAsync = await _connection.RequestAsync("GET_STATUS", token);
            token.ThrowIfCancellationRequested();
            RecordLog("完整回复", "GET_STATUS → " + requestAsync);
            ApplyReply("GET_STATUS", requestAsync);
        }

        private bool CanSendHeartbeat()
        {
            // TOD 4a：未采集且距离_lastActivity已至少HeartbeatIdle时返回true，否则返回false，真正发送仍走RunCommandAsync。
            TimeSpan idle = DateTime.UtcNow - _lastActivity;
            if (idle >= HeartbeatIdle && !_collecting)
            {
                return true;
            }
            
            return false;
        }

        private void ValidateHeartbeatReply(string reply)
        {
            // TOD 4b：仅reply为PONG时正常返回，否则抛出InvalidDataException，替换下面的未完成占位。
            if (reply.Equals("PONG"))
            {
                return;
            }
            throw new InvalidDataException("心跳检测失败");
        }

        private async void HeartbeatTick(object? sender, EventArgs e)
        {
            if (_closing || _busy || !_wantConnected || !_connection.IsConnected || !CanSendHeartbeat()) return;
            await RunCommandAsync("PING", false);
        }

        private async void HeartbeatClick(object sender, RoutedEventArgs e)
        {
            await RunCommandAsync("PING", false);
        }

        private void AutoReconnectChanged(object sender, RoutedEventArgs e)
        {
            if (!_ready || _closing) return;
            if (AutoReconnectCheckBox.IsChecked != true)
            {
                _reconnectTimer.Stop();
                if (_recovering)
                {
                    _wantConnected = false;
                    ResetConnection("已取消正在进行的自动连接。");
                }
                RecoveryText.Text = "自动重连已取消；已建立的正常连接保留。";
            }
            RefreshControls();
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
            DisconnectButton.IsEnabled = _sessionSource != null || _wantConnected || _reconnectTimer.IsEnabled;
            HeartbeatButton.IsEnabled = connected && !_busy;
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
            _wantConnected = false;
            _reconnectTimer.Stop();
            _heartbeatTimer.Stop();
            _reconnectTimer.Tick -= ReconnectTick;
            _heartbeatTimer.Tick -= HeartbeatTick;
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
