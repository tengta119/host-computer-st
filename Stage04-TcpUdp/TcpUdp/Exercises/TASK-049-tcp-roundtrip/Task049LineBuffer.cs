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
            // TOD 3：把data中前count个有效字节追加到_bytes，不使用整个数组长度，也不先清空旧半条消息。
            for (int i = 0; i < count; i++)
            {
                _bytes.Add(data[i]);
            }
        }

        public bool TryTakeMessage(out string message)
        {
            message = String.Empty;
            // TOD 4已按用户要求由教练补齐，每次只取第一个LF之前的一条消息。
            int lineFeedIndex = _bytes.IndexOf(0x0A);
            if (lineFeedIndex == -1)
            {
                if (_bytes.Count > MaxMessageLength)
                {
                    throw new InvalidDataException("消息超过256字节且尚未收到LF换行。");
                }
                return false;
            }

            if (lineFeedIndex == 0)
            {
                throw new InvalidDataException("收到空消息。");
            }
            if (lineFeedIndex > MaxMessageLength)
            {
                throw new InvalidDataException("消息超过256字节上限。");
            }

            byte[] messageBytes = new byte[lineFeedIndex];
            for (int i = 0; i < lineFeedIndex; i++)
            {
                messageBytes[i] = _bytes[i];
            }
            message = Encoding.UTF8.GetString(messageBytes);
            // 多移除的一个字节是当前消息的LF，后面的消息或半条继续保留。
            _bytes.RemoveRange(0, lineFeedIndex + 1);
            return true;
        }

        public void Clear()
        {
            _bytes.Clear();
        }
    }
}
