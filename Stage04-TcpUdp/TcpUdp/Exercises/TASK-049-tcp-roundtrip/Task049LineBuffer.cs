using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TcpUdp.Exercises.Task049
{
    // 这是本题自定义的消息缓冲类，保存尚未按LF换行取出的字节。
    public class Task049LineBuffer
    {
        private readonly List<byte> _bytes = new List<byte>();
        public const int MaxMessageLength = 256;

        public int Count
        {
            get { return _bytes.Count; }
        }

        public void Append(byte[] data, int count)
        {
            if (count < 0 || count > data.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            // TODO 3：把data中前count个有效字节追加到_bytes，不使用整个数组长度，也不先清空旧半条消息。
        }

        public bool TryTakeMessage(out string message)
        {
            message = String.Empty;
            // TODO 4：找到第一个0x0A，未找到时保留半条并返回false，超过256字节仍无换行则抛出InvalidDataException。
            // TODO 4：找到后拒绝空行和超过256字节的消息，复制换行之前的字节并用Encoding.UTF8.GetString解码到message。
            // TODO 4：只移除这条消息及紧随它的一个LF，保留后面的数据并返回true，由调用方继续提取。
            return false;
        }

        public void Clear()
        {
            _bytes.Clear();
        }
    }
}
