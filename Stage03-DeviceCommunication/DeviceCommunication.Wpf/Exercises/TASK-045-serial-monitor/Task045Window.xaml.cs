using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DeviceCommunication.Wpf.Exercises.Task045
{
    public partial class Task045Window : Window
    {
        private readonly Task045SerialConnection _connection = new Task045SerialConnection();
        private readonly ObservableCollection<Task045Reading> _history = new ObservableCollection<Task045Reading>();
        private bool _isClosing = false;
        private bool _ready = false;
        private double _alarmThreshold = 25.5;

        public Task045Window()
        {
            InitializeComponent();
            HistoryGrid.ItemsSource = _history;
            _connection.ReadingReceived += SerialReadingReceived;
            _connection.ReceiveFailed += SerialReceiveFailed;
            _ready = true;
            RefreshPorts();
            RefreshConnectionControls();
            RefreshPresentation();
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
                // TOD 1：把选中的portName交给_connection.Open，再调用RefreshConnectionControls显示实际连接状态。
                _connection.Open(portName);
                RefreshConnectionControls();
                OperationText.Text = "已连接：" + portName;
            }
            catch (Exception exception)
            {
                OperationText.Text = "连接失败：" + exception.Message;
                RefreshConnectionControls();
            }
        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // TOD 2：调用_connection.Close，再刷新连接控件状态，保留已显示历史，连接内部会清理旧半帧。
                _connection.Close();
                RefreshConnectionControls();
                OperationText.Text = "断开连接";
            }
            catch (Exception exception)
            {
                OperationText.Text = "断开失败：" + exception.Message;
                RefreshConnectionControls();
            }
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // TOD 3：调用_connection.SendRequest，成功后显示已发送55 01，不直接复制另一套SerialPort对象。
                _connection.SendRequest();
                OperationText.Text = "发送请求。";
            }
            catch (Exception exception)
            {
                OperationText.Text = "发送失败：" + exception.Message;
            }
        }

        private void SerialReadingReceived(object? sender, Task045ReadingEventArgs e)
        {
            if (_isClosing || Dispatcher.HasShutdownStarted)
            {
                return;
            }
            // TOD 4：用窗口的Dispatcher.BeginInvoke安排ApplyReadingOnUi(e)，不要在这个接收线程直接改TextBlock或_history。
            Dispatcher.BeginInvoke(new Action(() => ApplyReadingOnUi(e)));
        }

        private void ApplyReadingOnUi(Task045ReadingEventArgs e)
        {
            // 教练提供关闭/重连后的旧通知保护，TOD 5只连接这次读数与已有显示辅助。
            if (_isClosing || !_connection.IsCurrentSession(e.SessionId))
            {
                return;
            }
            // TOD 5：把e.Reading追加到_history，再调用RefreshPresentation，让同一条读数进入表格、曲线与报警。
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
            ConnectButton.IsEnabled = !connected;
            DisconnectButton.IsEnabled = connected;
            SendButton.IsEnabled = connected;
            PortComboBox.IsEnabled = !connected;
            RefreshPortsButton.IsEnabled = !connected;
            if (connected)
            {
                ConnectionStatusText.Text = "已连接：" + _connection.PortName;
            }
            else
            {
                ConnectionStatusText.Text = "未连接";
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
                if (!_isClosing && _connection.IsCurrentSession(e.SessionId))
                {
                    OperationText.Text = "接收失败：" + e.Message;
                    _connection.Close();
                    RefreshConnectionControls();
                }
            }));
        }

        protected override void OnClosed(EventArgs e)
        {
            _isClosing = true;
            _connection.ReadingReceived -= SerialReadingReceived;
            _connection.ReceiveFailed -= SerialReceiveFailed;
            _connection.Dispose();
            base.OnClosed(e);
        }
    }
}
