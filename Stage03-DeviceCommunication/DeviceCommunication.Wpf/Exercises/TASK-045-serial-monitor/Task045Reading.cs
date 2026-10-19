using System;

namespace DeviceCommunication.Wpf.Exercises.Task045
{
    // 教练准备的读数对象，来自完整教学帧，不由界面定时生成。
    public class Task045Reading
    {
        public DateTime ReceivedAt { get; }
        public byte DeviceId { get; }
        public int RawValue { get; }
        public double Temperature { get; }

        public Task045Reading(byte deviceId, int rawValue)
        {
            ReceivedAt = DateTime.Now;
            DeviceId = deviceId;
            RawValue = rawValue;
            Temperature = rawValue / 10.0;
        }
    }

    // Reading保存这次完整读数，SessionId用于丢弃已经断开的旧连接通知，均由教练准备。
    public class Task045ReadingEventArgs : EventArgs
    {
        public Task045Reading Reading { get; }
        public int SessionId { get; }

        public Task045ReadingEventArgs(Task045Reading reading, int sessionId)
        {
            Reading = reading;
            SessionId = sessionId;
        }
    }

    public class Task045SerialErrorEventArgs : EventArgs
    {
        public string Message { get; }
        public int SessionId { get; }

        public Task045SerialErrorEventArgs(string message, int sessionId)
        {
            Message = message;
            SessionId = sessionId;
        }
    }
}
