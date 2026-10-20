using System;

namespace TcpUdp.Wpf.Exercises.Task050
{
    // 本题自定义的一条温度记录，窗口表格读取这两个普通属性。
    public class Task050Reading
    {
        public DateTime ReceivedAt { get; set; }
        public double Temperature { get; set; }
    }

    // 本题自定义的日志记录，由教练提供格式与数量上限。
    public class Task050LogEntry
    {
        public DateTime Time { get; set; }
        public string Kind { get; set; } = "";
        public string Details { get; set; } = "";
    }
}
