using System;
using System.Text;

namespace TcpUdp.Exercises.Task049
{
    // 人工输入用于观察同一解析器，不把这些输入当作实际TCP到达次数。
    public static class Task049BufferObservation
    {
        public static void Run()
        {
            Task049LineBuffer buffer = new Task049LineBuffer();
            Console.WriteLine("当前为骨架观察，未实现的TODO不会被报告为通过。");
            byte[] first = new byte[] { 0x33, 0x35, 0xEE, 0xFF };
            buffer.Append(first, 2);
            Console.WriteLine("只追加前2个有效字节后，期望剩余2，当前：" + buffer.Count);
            PrintAvailable(buffer);
            byte[] second = Encoding.UTF8.GetBytes(".6\nOK\nST");
            buffer.Append(second, second.Length);
            Console.WriteLine("现在应依次取出35.6和OK，并保留ST两个字节。");
            PrintAvailable(buffer);
            Console.WriteLine("期望剩余2，当前：" + buffer.Count);
            byte[] third = Encoding.UTF8.GetBytes("OPPED\n");
            buffer.Append(third, third.Length);
            Console.WriteLine("现在应只取出STOPPED。");
            PrintAvailable(buffer);
            Console.WriteLine("期望剩余0，当前：" + buffer.Count);
            buffer.Clear();
        }

        private static void PrintAvailable(Task049LineBuffer buffer)
        {
            string message;
            while (buffer.TryTakeMessage(out message))
            {
                Console.WriteLine("完整消息：" + message);
            }
        }
    }
}
