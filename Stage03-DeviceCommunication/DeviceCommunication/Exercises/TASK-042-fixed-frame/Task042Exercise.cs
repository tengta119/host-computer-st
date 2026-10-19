using System;
using System.Collections.Generic;

namespace DeviceCommunication.Exercises.Task042
{
    public class Task042Exercise
    {
        private List<byte> _receivedBytes = new List<byte>();
        private int _printedFrameCount = 0;

        public void Run()
        {
            Console.WriteLine("TASK-042：按明确规则取出完整数据帧");

            byte[] firstChunk = new byte[] { 0xAA, 0x01, 0xEE, 0xFF };
            byte[] secondChunk = new byte[] { 0x03, 0x00, 0x64, 0xEE };
            byte[] thirdChunk = new byte[] { 0x5F, 0xEE };
            byte[] frameB = new byte[] { 0xAA, 0x02, 0x03, 0x00, 0x65, 0x60 };
            byte[] bothFrames = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0xAA, 0x02, 0x03, 0x00, 0x65, 0x60 };
            byte[] oneAndPrefix = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0xAA, 0x02 };
            byte[] restOfB = new byte[] { 0x03, 0x00, 0x65, 0x60, 0xEE };

            StartScenario("场景一：帧A分2/3/1到达");
            ReceiveChunk(firstChunk, 2, "收到A前2字节");
            ReceiveChunk(secondChunk, 3, "又收到A的3字节");
            ReceiveChunk(thirdChunk, 1, "收到A最后1字节");

            StartScenario("场景二：一次收到帧A和帧B");
            ReceiveChunk(bothFrames, 12, "一次收到12字节");
            ReceiveChunk(firstChunk, 0, "本次有效数量0");

            StartScenario("场景三：先收到A和B前2字节，再补齐B");
            ReceiveChunk(oneAndPrefix, 8, "先收到8字节");
            ReceiveChunk(restOfB, 4, "再收到B剩余4字节");
            ReceiveChunk(firstChunk, 0, "本次有效数量0");

            StartScenario("场景四：同样的A和B分2/3/1/6到达");
            ReceiveChunk(firstChunk, 2, "收到A前2字节");
            ReceiveChunk(secondChunk, 3, "又收到A的3字节");
            ReceiveChunk(thirdChunk, 1, "收到A最后1字节");
            ReceiveChunk(frameB, 6, "收到完整帧B");
            ReceiveChunk(firstChunk, 0, "本次有效数量0");

            // TODO 3：核心提取完成后，实际运行并对照上述四组输出，确认整段与分段结果一致、零数量不重复输出帧。
        }

        private void ProcessBuffer()
        {
            // TOD 1：先用 if 判断 _receivedBytes.Count 是否小于 6，分别输出“继续等待”或“已经够一帧”，运行先观察第一段的 2 个字节。
            if (_receivedBytes.Count < 6)
            {
                Console.WriteLine("继续等待");
                return;
            }
            // TOD 2a：缓冲足够时，新建 6 个位置的 byte 数组，把缓冲最前面的 6 个字节按顺序复制进去。
            byte[] data = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                data[i] = _receivedBytes[i];
            }
            // TOD 2b：复制完成后，只移除缓冲开头已经取出的 6 个字节，并调用 PrintFrame 输出刚复制的数组，保留后面的字节。
            _receivedBytes.RemoveRange(0, 6);
            PrintFrame(data);
            // TOD 2c：用 while 重复检查剩余数量，每次取出一帧后继续处理，直到不足 6 个字节，避免同一帧重复输出。
            while (_receivedBytes.Count >= 6)
            {
                ProcessBuffer();
            }
        }

        private void AppendReceivedBytes(byte[] data, int bytesRead)
        {
            // 这里复用 TASK-041 已验证的有效数量追加，不计作本题新的自主实现。
            for (int i = 0; i < bytesRead; i++)
            {
                _receivedBytes.Add(data[i]);
            }
        }

        private void ReceiveChunk(byte[] data, int bytesRead, string label)
        {
            Console.WriteLine("输入：" + label);
            AppendReceivedBytes(data, bytesRead);
            ProcessBuffer();
            PrintBufferState();
        }

        private void StartScenario(string label)
        {
            // 只在不同测试组之间清空，每组内部持续保留同一个列表。
            _receivedBytes.Clear();
            _printedFrameCount = 0;
            Console.WriteLine();
            Console.WriteLine(label);
        }

        private void PrintFrame(byte[] frame)
        {
            _printedFrameCount = _printedFrameCount + 1;
            Console.Write("完整帧" + _printedFrameCount + "：");
            for (int i = 0; i < frame.Length; i++)
            {
                Console.Write(frame[i].ToString("X2") + " ");
            }
            Console.WriteLine();
        }

        private void PrintBufferState()
        {
            Console.WriteLine("缓冲剩余数量：" + _receivedBytes.Count);
            Console.Write("缓冲剩余字节：");
            for (int i = 0; i < _receivedBytes.Count; i++)
            {
                Console.Write(_receivedBytes[i].ToString("X2") + " ");
            }
            Console.WriteLine();
            Console.WriteLine("本组已输出帧数：" + _printedFrameCount);
        }
    }
}
