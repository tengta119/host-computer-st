using DeviceCommunication.Wpf.Exercises.Task045;
using System.Diagnostics;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DeviceCommunication.Wpf.Exercises.Task048
{
    public partial class Task048Window : Window
    {
        private readonly Task048SerialConnection _connection = new Task048SerialConnection();
        private readonly ObservableCollection<Task045Reading> _history = new ObservableCollection<Task045Reading>();
        private readonly ObservableCollection<Task048LogEntry> _communicationLogs = new ObservableCollection<Task048LogEntry>();
        private const int MaximumLogCount = 200;
        private string _waitingPortName = "";
        private bool _isClosing = false;
        private bool _ready = false;
        private double _alarmThreshold = 25.5;
        private readonly DispatcherTimer _timeoutTimer = new DispatcherTimer();
        private readonly Stopwatch _requestWatch = new Stopwatch();
        private readonly TimeSpan _responseTimeout = TimeSpan.FromSeconds(3);
        private bool _waitingForResponse = false;
        private int _waitingSessionId = -1;
        private DateTime _requestSentAt = DateTime.MinValue;


        public Task048Window()
        {
            InitializeComponent();
            HistoryGrid.ItemsSource = _history;
            CommunicationLogGrid.ItemsSource = _communicationLogs;
            _connection.LogsReceived += SerialLogsReceived;
            _connection.ReadingReceived += SerialReadingReceived;
            _connection.ReceiveFailed += SerialReceiveFailed;
            // 这里只登记检查方法，Start由发送成功后的BeginWaiting负责。
            _timeoutTimer.Interval = TimeSpan.FromMilliseconds(200);
            _timeoutTimer.Tick += CheckResponseTimeout;
            _ready = true;
            RefreshPorts();
            RefreshConnectionControls();
            RefreshPresentation();
            RecordStatus("窗口", "窗口已就绪，连接端口后开始通信。", 0, "");
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            string portName = PortComboBox.Text.Trim();
            if (String.IsNullOrWhiteSpace(portName))
            {
                OperationText.Text = "先选择已经创建的那一对串口中给上位机使用的一端。";
                return;
            }
            try
            {

                EndWaiting("尚未等待响应。");
                _connection.Open(portName);
                RefreshConnectionControls();
                OperationText.Text = "已连接：" + portName;
                // TODO 3：调用RecordStatus记录连接成功，传入当前连接编号和portName，不把连接成功写成收到数据。
            }
            catch (Exception exception)
            {
                OperationText.Text = "连接失败：" + exception.Message;
                RefreshConnectionControls();
                // TODO 3：调用RecordStatus记录连接失败及OperationText.Text，保留尝试打开的portName。
            }
        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            RecoverConnection("已主动断开连接。");
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            if (_waitingForResponse)
            {
                OperationText.Text = "正在等待本次回复，请勿重复发送。";
                return;
            }
            try
            {
                _connection.SendRequest();
                BeginWaiting();
            }
            catch (Exception exception)
            {
                RecoverConnection("发送失败：" + exception.Message);
            }
        }

        private void BeginWaiting()
        {


            _waitingSessionId = _connection.CurrentSessionId;
            _waitingPortName = _connection.PortName;
            _requestSentAt = DateTime.Now;
            _requestWatch.Restart();
            _timeoutTimer.Start();
            _waitingForResponse = true;
            RequestStatusText.Text = "正在等待响应，最多3秒。";
            RefreshConnectionControls();
            OperationText.Text = "已发送55 01；进入等待状态。";
        }

        private void EndWaiting(string message)
        {
            bool wasWaiting = _waitingForResponse;
            int waitingSessionId = _waitingSessionId;
            string waitingPortName = _waitingPortName;


            _timeoutTimer.Stop();
            _requestWatch.Reset();          // 秒表停止，并把已等待的时间归零。
            _waitingForResponse = false;    // 表示现在不再等待回复。
            _waitingSessionId = -1;         // 表示现在没有等待中的连接编号。
            _waitingPortName = "";
            RequestStatusText.Text = message;
            RefreshConnectionControls();
            if (wasWaiting)
            {
                // TODO 3：调用RecordStatus记录等待结束及message，使用上面保存的waitingSessionId和waitingPortName，不使用已经清空的字段。
            }
            
        }

        private void CheckResponseTimeout(object? sender, EventArgs e)
        {






            if (_isClosing || !_waitingForResponse)
            {
                return;
            }

            if (!_connection.BelongsToCurrentConnection(_waitingSessionId))
            {
                EndWaiting("原连接已结束，取消本次等待。");
                return;
            }

            if (!_connection.IsConnected)
            {
                RecoverConnection("端口失效");
                return;
            }

            if (_requestWatch.Elapsed >= _responseTimeout)
            {
                EndWaiting("响应超时");
                return;
            }
            RequestStatusText.Text = "正在等待";
        }

        private void RecoverConnection(string reason)
        {
            int oldSessionId = _connection.CurrentSessionId;
            string oldPortName = _connection.PortName;
            string result = reason;
            EndWaiting(reason);

            try
            {
                _connection.Close();
            }
            catch (Exception exception)
            {
                result = reason + "；关闭串口时发生错误：" + exception.Message;
            }
            finally
            {
                RefreshConnectionControls();
                OperationText.Text = result;
                // TODO 3：调用RecordStatus记录连接清理及result，使用oldSessionId和oldPortName，让清理结果仍属于被关闭的连接。
            }
        }

        // 教练提供新的批量日志通知与窗口线程交接，保留上一题的连接归属保护。
        private void SerialLogsReceived(object? sender, Task048LogsEventArgs e)
        {
            if (_isClosing || Dispatcher.HasShutdownStarted)
            {
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_isClosing || !_connection.BelongsToCurrentConnection(e.SessionId))
                {
                    return;
                }
                foreach (Task048LogEntry entry in e.Entries)
                {
                    AppendLogOnUi(entry);
                }
            }));
        }

        private void AppendLogOnUi(Task048LogEntry entry)
        {
            if (_isClosing)
            {
                return;
            }
            // TODO 1：把entry追加到_communicationLogs，数量超过MaximumLogCount时从下标0删除最旧条目，再更新LogCountText显示当前数量。
            // 构造方法已写入一条真实的窗口就绪记录，完成这里后启动窗口即可先看到这一行，不需要连接串口。
        }

        private void RecordStatus(string category, string message, int sessionId, string portName)
        {
            // 此方法由窗口线程调用，条目中的编号和端口来自具体操作，不在这里重新猜当前连接。
            Task048LogEntry entry = new Task048LogEntry(sessionId, portName, category, 0, "", message);
            AppendLogOnUi(entry);
        }

        private void ClearLogsClick(object sender, RoutedEventArgs e)
        {
            _communicationLogs.Clear();
            LogCountText.Text = "日志条数：0 / " + MaximumLogCount;
        }

        private void SerialReadingReceived(object? sender, Task045ReadingEventArgs e)
        {
            if (_isClosing || Dispatcher.HasShutdownStarted)
            {
                return;
            }

            Dispatcher.BeginInvoke(new Action(() => ApplyReadingOnUi(e)));
        }

        private void ApplyReadingOnUi(Task045ReadingEventArgs e)
        {

            if (_isClosing || !_connection.IsCurrentSession(e.SessionId))
            {
                return;
            }

            if (_waitingForResponse && e.SessionId == _waitingSessionId && e.Reading.ReceivedAt >= _requestSentAt)
            {
                EndWaiting("已收到本次等待后的完整读数。");
            }
            _history.Add(e.Reading);
            RefreshPresentation();
            OperationText.Text = "已收到完整读数，追加到历史。";
        }

        private void RefreshPortsClick(object sender, RoutedEventArgs e)
        {
            RefreshPorts();
        }

        private void RefreshPorts()
        {
            string selected = PortComboBox.Text;
            try
            {
                PortComboBox.ItemsSource = System.IO.Ports.SerialPort.GetPortNames();
                PortComboBox.Text = selected;
            }
            catch (Exception exception)
            {
                OperationText.Text = "列举端口失败：" + exception.Message;
            }
        }

        private void RefreshConnectionControls()
        {
            bool connected = _connection.IsConnected;
            bool hasConnection = _connection.HasConnection;
            ConnectButton.IsEnabled = !hasConnection;
            DisconnectButton.IsEnabled = hasConnection;
            SendButton.IsEnabled = connected && !_waitingForResponse;
            PortComboBox.IsEnabled = !hasConnection;
            RefreshPortsButton.IsEnabled = !hasConnection;
            if (connected)
            {
                ConnectionStatusText.Text = "已连接：" + _connection.PortName;
            }
            else
            {
                ConnectionStatusText.Text = hasConnection ? "连接已失效，请断开后重连。" : "未连接";
            }
        }

        private void RefreshPresentation()
        {
            ReadingCountText.Text = "历史条数：" + _history.Count;
            if (_history.Count == 0)
            {
                LatestReadingText.Text = "尚未收到读数";
                AlarmText.Text = "暂无温度";
                AlarmText.Foreground = Brushes.SlateGray;
            }
            else
            {
                Task045Reading latest = _history[_history.Count - 1];
                LatestReadingText.Text = "设备 " + latest.DeviceId + "  ·  " + latest.Temperature.ToString("F1") + " ℃";
                if (latest.Temperature > _alarmThreshold)
                {
                    AlarmText.Text = "最新读数超限，阈值 " + _alarmThreshold.ToString("F1") + " ℃";
                    AlarmText.Foreground = Brushes.Firebrick;
                }
                else
                {
                    AlarmText.Text = "最新读数正常，阈值 " + _alarmThreshold.ToString("F1") + " ℃";
                    AlarmText.Foreground = Brushes.SeaGreen;
                }
            }
            DrawTemperatureLine();
        }

        private void DrawTemperatureLine()
        {
            List<Task045Reading> visible = new List<Task045Reading>();
            byte deviceId = 1;
            if (ChartDeviceComboBox.SelectedIndex == 1)
            {
                deviceId = 2;
            }
            foreach (Task045Reading reading in _history)
            {
                if (reading.DeviceId == deviceId)
                {
                    visible.Add(reading);
                }
            }
            if (visible.Count > 30)
            {
                visible.RemoveRange(0, visible.Count - 30);
            }
            PointCollection points = new PointCollection();
            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            if (visible.Count == 0 || width < 40 || height < 40)
            {
                TemperatureLine.Points = points;
                SingleReadingDot.Visibility = Visibility.Collapsed;
                ChartEmptyText.Visibility = Visibility.Visible;
                return;
            }
            double minimum = visible[0].Temperature;
            double maximum = minimum;
            foreach (Task045Reading reading in visible)
            {
                minimum = Math.Min(minimum, reading.Temperature);
                maximum = Math.Max(maximum, reading.Temperature);
            }
            double span = Math.Max(1, maximum - minimum);
            for (int i = 0; i < visible.Count; i++)
            {
                double x = 20 + i * (width - 40) / Math.Max(1, visible.Count - 1);
                double y = height - 20 - (visible[i].Temperature - minimum + 0.5) * (height - 40) / (span + 1);
                points.Add(new Point(x, y));
            }
            TemperatureLine.Points = points;
            if (points.Count == 1)
            {
                SingleReadingDot.Visibility = Visibility.Visible;
                Canvas.SetLeft(SingleReadingDot, points[0].X - 3.5);
                Canvas.SetTop(SingleReadingDot, points[0].Y - 3.5);
            }
            else
            {
                SingleReadingDot.Visibility = Visibility.Collapsed;
            }
            ChartEmptyText.Visibility = Visibility.Collapsed;
        }

        private void ApplyThresholdClick(object sender, RoutedEventArgs e)
        {
            double threshold;
            if (!Double.TryParse(ThresholdInput.Text, out threshold) || !Double.IsFinite(threshold) || threshold < 0 || threshold > 100)
            {
                OperationText.Text = "阈值请输入0到100之间的有效数字。";
                return;
            }
            _alarmThreshold = threshold;
            RefreshPresentation();
            OperationText.Text = "报警阈值已应用。";
        }

        private void ChartDeviceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_ready)
            {
                DrawTemperatureLine();
            }
        }

        private void ChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_ready)
            {
                DrawTemperatureLine();
            }
        }

        private void SerialReceiveFailed(object? sender, Task045SerialErrorEventArgs e)
        {
            if (_isClosing || Dispatcher.HasShutdownStarted)
            {
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!_isClosing && _connection.BelongsToCurrentConnection(e.SessionId))
                {
                    RecoverConnection("接收失败：" + e.Message);
                }
            }));
        }

        protected override void OnClosed(EventArgs e)
        {
            _isClosing = true;
            EndWaiting("窗口已关闭。");
            // 教练的最终释放保护保证TODO未完成的骨架也能安全关闭。
            _timeoutTimer.Stop();
            _timeoutTimer.Tick -= CheckResponseTimeout;
            _requestWatch.Reset();
            _connection.LogsReceived -= SerialLogsReceived;
            _connection.ReadingReceived -= SerialReadingReceived;
            _connection.ReceiveFailed -= SerialReceiveFailed;
            try
            {
                _connection.Dispose();
            }
            catch (Exception exception)
            {
                Trace.WriteLine("窗口关闭时释放串口失败：" + exception.Message);
            }
            finally
            {
                base.OnClosed(e);
            }
        }
    }
}
