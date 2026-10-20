using DeviceCommunication.Wpf.Exercises.Task045;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace DeviceCommunication.Wpf.Exercises.Task048
{
    // 教练复用TASK-047连接、组包和恢复，新增日志先收集后在锁外发出，原有读数参数仍引用TASK-045。
    public class Task048SerialConnection : IDisposable
    {
        private readonly object _sync = new object();
        private readonly Task048Protocol _protocol = new Task048Protocol();
        private readonly List<byte> _receivedBytes = new List<byte>();
        private SerialPort? _port;
        private int _sessionId = 0;
        private bool _disposed = false;

        public event EventHandler<Task048LogsEventArgs>? LogsReceived;
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
                    throw new ObjectDisposedException("Task048SerialConnection");
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
            List<Task048LogEntry> logs = new List<Task048LogEntry>();
            int sessionId;
            lock (_sync)
            {
                if (_port == null || !_port.IsOpen)
                {
                    throw new InvalidOperationException("尚未连接串口。");
                }
                sessionId = _sessionId;
                byte[] request = new byte[] { 0x55, 0x01 };
                _port.Write(request, 0, request.Length);
                // Write成功之后才准备发送成功日志，失败直接交给窗口的错误收尾。
                AddByteLog(logs, sessionId, _port.PortName, "发送", request, request.Length, "请求已写入串口，仍需等待设备回复。");
            }
            PublishLogs(logs, sessionId);
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
            List<Task048LogEntry> logs = new List<Task048LogEntry>();
            string portName;
            string? failure = null;
            int sessionId;
            lock (_sync)
            {
                if (_port == null || !Object.ReferenceEquals(sender, _port))
                {
                    return;
                }
                sessionId = _sessionId;
                portName = _port.PortName;
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
                        AddByteLog(logs, sessionId, portName, "接收", data, bytesRead, "本次Read返回的字节，不代表已经收齐一帧。");
                        for (int i = 0; i < bytesRead; i++)
                        {
                            _receivedBytes.Add(data[i]);
                        }
                        ExtractReadings(readings, logs, sessionId, portName);
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

            if (failure != null)
            {
                logs.Add(new Task048LogEntry(sessionId, portName, "接收错误", 0, "", failure));
            }
            // 两种通知都在锁外发出，先交日志再交读数，窗口中的归属检查会丢弃旧连接的排队通知。
            PublishLogs(logs, sessionId);
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

        private void ExtractReadings(List<Task045Reading> readings, List<Task048LogEntry> logs, int sessionId, string portName)
        {
            int skippedNoise = 0;
            while (_receivedBytes.Count > 0)
            {
                if (_receivedBytes[0] != 0xAA)
                {
                    _receivedBytes.RemoveAt(0);
                    skippedNoise = skippedNoise + 1;
                    continue;
                }
                if (_receivedBytes.Count < 6)
                {
                    logs.Add(new Task048LogEntry(sessionId, portName, "等待半帧", 0, "", "缓冲保留" + _receivedBytes.Count + "字节，尚不足6字节，继续等待。"));
                    break;
                }
                byte[] frame = new byte[6];
                for (int i = 0; i < frame.Length; i++)
                {
                    frame[i] = _receivedBytes[i];
                }
                string rejection = _protocol.GetRejectionReason(frame);
                if (rejection != "")
                {
                    AddByteLog(logs, sessionId, portName, "拒绝候选", frame, frame.Length, rejection + "仅删除当前首字节，继续寻找后续候选。");
                    _receivedBytes.RemoveAt(0);
                    continue;
                }
                AddByteLog(logs, sessionId, portName, "有效帧", frame, frame.Length, "协议检查通过，准备生成一条设备读数。");
                byte deviceId = frame[1];
                int rawValue = frame[3] * 256 + frame[4];
                _receivedBytes.RemoveRange(0, 6);
                readings.Add(new Task045Reading(deviceId, rawValue));
            }
            if (skippedNoise > 0)
            {
                logs.Add(new Task048LogEntry(sessionId, portName, "跳过噪声", 0, "", "本次处理跳过" + skippedNoise + "个不以AA开头的字节。"));
            }
        }

        private void AddByteLog(List<Task048LogEntry> logs, int sessionId, string portName, string category, byte[] data, int count, string message)
        {
            if (count < 0 || count > data.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            // TOD 2：新建长度为count的byte数组，只复制data中下标0到count-1的字节，用FormatBytes得到文字，再把含count和文字的Task048LogEntry加入logs。
            // 例如data有256个位置但count是2，日志只能保存两个字节，不能遍历到data.Length。
            byte[] logBytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                logBytes[i] = data[i];
            }

            string formatBytes = FormatBytes(logBytes);
            Task048LogEntry entry = new Task048LogEntry(sessionId, portName, category, count, formatBytes, message);
            logs.Add(entry);
        }

        private string FormatBytes(byte[] bytes)
        {
            string text = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0)
                {
                    text = text + " ";
                }
                text = text + bytes[i].ToString("X2");
            }
            return text;
        }

        private void PublishLogs(List<Task048LogEntry> logs, int sessionId)
        {
            if (logs.Count == 0)
            {
                return;
            }
            EventHandler<Task048LogsEventArgs>? handler = LogsReceived;
            if (handler != null)
            {
                handler(this, new Task048LogsEventArgs(logs, sessionId));
            }
        }
    }
}
