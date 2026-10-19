using DeviceCommunication.Wpf.Exercises.Task045;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace DeviceCommunication.Wpf.Exercises.Task046
{
    // 教练复用TASK-045串口和解析辅助，读数及事件参数仍使用TASK-045的数据类。
    public class Task046SerialConnection : IDisposable
    {
        private readonly object _sync = new object();
        private readonly List<byte> _receivedBytes = new List<byte>();
        private SerialPort? _port;
        private int _sessionId = 0;
        private bool _disposed = false;

        public event EventHandler<Task045ReadingEventArgs>? ReadingReceived;
        public event EventHandler<Task045SerialErrorEventArgs>? ReceiveFailed;

        public bool IsConnected
        {
            get
            {
                lock (_sync)
                {
                    return _port != null && _port.IsOpen;
                }
            }
        }

        // 连接编号只用于区分打开前后的连接，不代表请求编号。
        public int CurrentSessionId
        {
            get { lock (_sync) { return _sessionId; } }
        }

        public bool HasConnection
        {
            get { lock (_sync) { return _port != null; } }
        }

        // 错误通知也要处理已经失效的当前端口，因此这里不要求IsOpen为true。
        public bool BelongsToCurrentConnection(int sessionId)
        {
            lock (_sync)
            {
                return sessionId == _sessionId && _port != null;
            }
        }

        public string PortName
        {
            get
            {
                lock (_sync)
                {
                    if (_port == null)
                    {
                        return "";
                    }
                    return _port.PortName;
                }
            }
        }

        public void Open(string portName)
        {
            lock (_sync)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("Task046SerialConnection");
                }
                if (_port != null)
                {
                    throw new InvalidOperationException("请先断开当前端口。");
                }
                SerialPort port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                port.Handshake = Handshake.None;
                port.ReadTimeout = 200;
                port.WriteTimeout = 1000;
                _receivedBytes.Clear();
                _sessionId = _sessionId + 1;
                _port = port;
                port.DataReceived += PortDataReceived;
                try
                {
                    port.Open();
                }
                catch
                {
                    _port = null;
                    port.DataReceived -= PortDataReceived;
                    port.Dispose();
                    throw;
                }
            }
        }

        public void SendRequest()
        {
            lock (_sync)
            {
                if (_port == null || !_port.IsOpen)
                {
                    throw new InvalidOperationException("尚未连接串口。");
                }
                byte[] request = new byte[] { 0x55, 0x01 };
                _port.Write(request, 0, request.Length);
            }
        }

        public void Close()
        {
            SerialPort? oldPort;
            lock (_sync)
            {
                _sessionId = _sessionId + 1;
                oldPort = _port;
                _port = null;
                _receivedBytes.Clear();
                if (oldPort != null)
                {
                    oldPort.DataReceived -= PortDataReceived;
                }
            }
            // 离开lock后释放端口，避免关闭等待接收结束时仍挡住接收代码所需的锁。
            if (oldPort != null)
            {
                oldPort.Dispose();
            }
        }

        public bool IsCurrentSession(int sessionId)
        {
            lock (_sync)
            {
                return sessionId == _sessionId && _port != null && _port.IsOpen;
            }
        }

        public void Dispose()
        {
            lock (_sync)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;
            }
            Close();
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs arguments)
        {
            List<Task045Reading> readings = new List<Task045Reading>();
            string? failure = null;
            int sessionId;
            lock (_sync)
            {
                if (_port == null || !Object.ReferenceEquals(sender, _port))
                {
                    return;
                }
                sessionId = _sessionId;
                try
                {
                    if (!_port.IsOpen)
                    {
                        throw new InvalidOperationException("当前串口已经关闭或失效。");
                    }
                    byte[] data = new byte[256];
                    while (_port.BytesToRead > 0)
                    {
                        int count = Math.Min(data.Length, _port.BytesToRead);
                        int bytesRead = _port.Read(data, 0, count);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        for (int i = 0; i < bytesRead; i++)
                        {
                            _receivedBytes.Add(data[i]);
                        }
                        ExtractReadings(readings);
                    }
                }
                catch (TimeoutException)
                {
                    // 当前一小次Read超时不丢掉已有半帧，后续通知仍可继续接收。
                }
                catch (Exception exception)
                {
                    failure = exception.Message;
                }
            }

            // 发出通知时已经离开锁，窗口只负责安排界面更新，不能在接收线程直接改控件。
            foreach (Task045Reading reading in readings)
            {
                EventHandler<Task045ReadingEventArgs>? handler = ReadingReceived;
                if (handler != null)
                {
                    handler(this, new Task045ReadingEventArgs(reading, sessionId));
                }
            }
            if (failure != null)
            {
                EventHandler<Task045SerialErrorEventArgs>? handler = ReceiveFailed;
                if (handler != null)
                {
                    handler(this, new Task045SerialErrorEventArgs(failure, sessionId));
                }
            }
        }

        private void ExtractReadings(List<Task045Reading> readings)
        {
            while (_receivedBytes.Count > 0)
            {
                if (_receivedBytes[0] != 0xAA)
                {
                    _receivedBytes.RemoveAt(0);
                    continue;
                }
                if (_receivedBytes.Count < 6)
                {
                    break;
                }
                byte deviceId = _receivedBytes[1];
                int rawValue = _receivedBytes[3] * 256 + _receivedBytes[4];
                _receivedBytes.RemoveRange(0, 6);
                readings.Add(new Task045Reading(deviceId, rawValue));
            }
        }
    }
}
