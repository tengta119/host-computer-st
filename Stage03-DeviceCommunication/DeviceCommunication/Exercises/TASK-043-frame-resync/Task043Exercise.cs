using System;
using System.Collections.Generic;

namespace DeviceCommunication.Exercises.Task043
{
    public class Task043Exercise
    {
        private List<byte> _receivedBytes = new List<byte>();
        private int _printedFrameCount = 0;

        public void Run()
        {
            Console.WriteLine("TASK-043：跳过无效开头并恢复取帧");

            byte[] noiseOnly = new byte[] { 0x99, 0x88 };
            byte[] prefixAndA = new byte[] { 0x99, 0x88, 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F };
            byte[] noiseAndPrefix = new byte[] { 0x77, 0xAA, 0x01 };
            byte[] restOfA = new byte[] { 0x03, 0x00, 0x64, 0x5F, 0xEE };
            byte[] aNoiseB = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0x99, 0xAA, 0x02, 0x03, 0x00, 0x65, 0x60 };
            byte[] frameC = new byte[] { 0xAA, 0x03, 0xAA, 0x00, 0x66, 0x61 };
            byte[] aFirst = new byte[] { 0xAA, 0x01 };
            byte[] aRestNoiseBHead = new byte[] { 0x03, 0x00, 0x64, 0x5F, 0x99, 0xAA };
            byte[] bRest = new byte[] { 0x02, 0x03, 0x00, 0x65, 0x60 };

            StartScenario("场景一：只有两个无效字节");
            ReceiveChunk(noiseOnly, 2, "收到99 88");
            ReceiveChunk(noiseOnly, 0, "本次有效数量0");

            StartScenario("场景二：无效前导后接完整A");
            ReceiveChunk(prefixAndA, 8, "收到99 88和完整A");
            ReceiveChunk(noiseOnly, 0, "本次有效数量0");

            StartScenario("场景三：无效前导后先留合法半帧，再补齐");
            ReceiveChunk(noiseAndPrefix, 3, "收到77 AA 01");
            ReceiveChunk(restOfA, 4, "收到A剩余4字节");
            ReceiveChunk(noiseOnly, 0, "本次有效数量0");

            StartScenario("场景四：两帧之间有无效字节");
            ReceiveChunk(aNoiseB, 13, "一次收到A、99和B");
            ReceiveChunk(noiseOnly, 0, "本次有效数量0");

            StartScenario("场景五：完整帧内部也有AA");
            ReceiveChunk(frameC, 6, "收到完整C");
            ReceiveChunk(noiseOnly, 0, "本次有效数量0");

            StartScenario("场景六：同样的A、99和B分段到达");
            ReceiveChunk(aFirst, 2, "收到A前2字节");
            ReceiveChunk(aRestNoiseBHead, 6, "收到A尾部、99和B的AA");
            ReceiveChunk(bRest, 5, "收到B剩余5字节");
            ReceiveChunk(noiseOnly, 0, "本次有效数量0");

            StartScenario("场景七：空缓冲收到零数量");
            ReceiveChunk(noiseOnly, 0, "没有新字节");

            // TODO 3：实现恢复处理后运行七组，核对短无效输入清空、合法半帧保留、内部AA原样输出与零数量安全。
        }

        private void ProcessBuffer()
        {
            // TODO 1：先判断 Count 是否为 0，为 0 时显示“缓冲为空”并返回，否则输出当前首字节的两位十六进制。

            // TODO 2a：继续实现时，用循环处理非空缓冲，每轮只判断当前下标 0 的字节，避免空列表读取下标 0。

            // TODO 2b：当前首字节不是 0xAA 时，只删除这一项，再回到循环检查新的首字节，短无效输入也要能清理。

            // TODO 2c：首字节为 0xAA 但不足 6 字节时，保留并停止本次处理；足够时调用 ExtractOneFrame，再继续检查剩余内容。
        }

        private void ExtractOneFrame()
        {
            // 这里复用 TASK-042 已验证的六字节复制、移除和打印，调用前由 ProcessBuffer 确认首字节为 AA 且数量至少为 6。
            byte[] frame = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                frame[i] = _receivedBytes[i];
            }
            _receivedBytes.RemoveRange(0, 6);
            PrintFrame(frame);
        }

        private void AppendReceivedBytes(byte[] data, int bytesRead)
        {
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
