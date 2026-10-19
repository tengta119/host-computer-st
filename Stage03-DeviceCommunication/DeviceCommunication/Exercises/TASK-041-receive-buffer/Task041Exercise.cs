using System;
using System.Collections.Generic;

namespace DeviceCommunication.Exercises.Task041
{
    public class Task041Exercise
    {
        private List<byte> _receivedBytes = new List<byte>();

        public void Run()
        {
            Console.WriteLine("TASK-041：保留分批收到的有效字节");
            PrintBuffer("开始时");

            byte[] firstChunk = new byte[] { 0xAA, 0x01, 0xEE, 0xFF };
            byte[] secondChunk = new byte[] { 0x03, 0x00, 0x64, 0xEE };
            byte[] thirdChunk = new byte[] { 0x5F, 0xEE };

            AppendReceivedBytes(firstChunk, 2);
            PrintBuffer("第一批后");

            AppendReceivedBytes(secondChunk, 3);
            PrintBuffer("第二批后");

            AppendReceivedBytes(thirdChunk, 1);
            PrintBuffer("第三批后");

            // TOD 3a：用同一个 firstChunk 和有效数量 0 调用追加，再观察列表，预期数量仍为 6、内容不变。
            AppendReceivedBytes(firstChunk, 0);
            PrintBuffer("零数量后");
            // TOD 3b：主动清空 _receivedBytes 并观察数量 0，再按原顺序处理同样三批，观察数量重新变为 2、5、6。
            _receivedBytes.Clear();
            PrintBuffer("清空后");
            AppendReceivedBytes(thirdChunk, 0);
            AppendReceivedBytes(firstChunk, 2);
            PrintBuffer("重新接收第一批后");
            AppendReceivedBytes(secondChunk, 3);
            PrintBuffer("重新接收第二批后");
            AppendReceivedBytes(thirdChunk, 1);
            PrintBuffer("重新接收第三批后");
        }

        private void AppendReceivedBytes(byte[] data, int bytesRead)
        {
            // TOD 2：只遍历本次有效位置，把 data 中对应的字节逐个追加到已有的 _receivedBytes，保留前次内容和有效的 00。
            for (int i = 0; i < bytesRead; i++)
            {
                _receivedBytes.Add(data[i]);
            }
        }

        private void PrintBuffer(string label)
        {
            Console.WriteLine(label);

            // TOD 1a：在这里输出 _receivedBytes.Count，先运行观察“开始时”的数量为 0。
            Console.WriteLine(_receivedBytes.Count);
            // TOD 1b：按列表当前 Count 遍历 _receivedBytes，每行输出下标、十进制值和两位大写十六进制。
            for (int i = 0; i < _receivedBytes.Count; i++)
            {
                Console.WriteLine(i + "," + _receivedBytes[i] + "," + _receivedBytes[i].ToString("X2"));
            }
        }
    }
}
