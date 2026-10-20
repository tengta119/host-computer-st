using System;
using System.Collections.Generic;

namespace DeviceCommunication.Wpf.Exercises.Task048
{
    // 教练新建的日志数据类，一条对象对应日志表的一行，不是WPF内置类型。
    public class Task048LogEntry
    {
        public DateTime CreatedAt { get; }
        public int SessionId { get; }
        public string PortName { get; }
        public string Category { get; }
        public int ByteCount { get; }
        public string BytesHex { get; }
        public string Message { get; }

        public string SessionText
        {
            get
            {
                if (SessionId > 0)
                {
                    return SessionId.ToString();
                }
                return "";
            }
        }

        public string ByteCountText
        {
            get
            {
                if (Category == "发送" || Category == "接收" || Category == "拒绝候选" || Category == "有效帧")
                {
                    return ByteCount.ToString();
                }
                return "";
            }
        }

        public Task048LogEntry(int sessionId, string portName, string category, int byteCount, string bytesHex, string message)
        {
            CreatedAt = DateTime.Now;
            SessionId = sessionId;
            PortName = portName;
            Category = category;
            ByteCount = byteCount;
            BytesHex = bytesHex;
            Message = message;
        }
    }

    // 一次通知携带一批已经生成的条目，数组由教练复制，发送通知后不继续修改。
    public class Task048LogsEventArgs : EventArgs
    {
        public Task048LogEntry[] Entries { get; }
        public int SessionId { get; }

        public Task048LogsEventArgs(List<Task048LogEntry> entries, int sessionId)
        {
            Entries = entries.ToArray();
            SessionId = sessionId;
        }
    }
}
