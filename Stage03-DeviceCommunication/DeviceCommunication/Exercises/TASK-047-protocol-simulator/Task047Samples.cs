using System;
using System.Collections.Generic;

namespace DeviceCommunication.Exercises.Task047
{
    // 全部样例由教练提供，字段非法的帧仍重新计算末字节，避免只检查校验也误以为字段检查已完成。
    public class Task047Sample
    {
        public string Command { get; }
        public string Description { get; }
        public byte[] Data { get; }

        public Task047Sample(string command, string description, byte[] data)
        {
            Command = command;
            Description = description;
            Data = data;
        }
    }

    public class Task047Samples
    {
        public static List<Task047Sample> CreateAll()
        {
            byte[] a = CreateFrame(1, 3, 250);
            byte[] b = CreateFrame(2, 3, 260);
            byte[] badChecksum = CreateFrame(1, 3, 250);
            badChecksum[5] = (byte)(badChecksum[5] + 1);
            byte[] noise = new byte[2048];
            for (int i = 0; i < noise.Length; i++)
            {
                noise[i] = 0x99;
            }
            return new List<Task047Sample>
            {
                new Task047Sample("1", "完整A：设备1，25.0℃。", a),
                new Task047Sample("2", "A前2字节，只保留半帧。", new byte[] { 0xAA, 0x01 }),
                new Task047Sample("3", "A剩4字节，接在2之后补齐A。", new byte[] { a[2], a[3], a[4], a[5] }),
                new Task047Sample("4", "同批A+B，依次得到25.0℃和26.0℃。", Join(a, b)),
                new Task047Sample("5", "只改A的末字节，校验错误，应拒绝。", badChecksum),
                new Task047Sample("6", "设备03非法，但末字节正确，应拒绝。", CreateFrame(3, 3, 250)),
                new Task047Sample("7", "命令04非法，但末字节正确，应拒绝。", CreateFrame(1, 4, 250)),
                new Task047Sample("8", "原始值1001超范围，但末字节正确，应拒绝。", CreateFrame(1, 3, 1001)),
                new Task047Sample("9", "噪声99 88后接完整A，应恢复A。", Join(new byte[] { 0x99, 0x88 }, a)),
                new Task047Sample("10", "AA 99后紧接A，好帧头藏在首个坏候选内部，应恢复A。", Join(new byte[] { 0xAA, 0x99 }, a)),
                new Task047Sample("11", "原始值170含AA数据字节，应得到17.0℃。", CreateFrame(1, 3, 170)),
                new Task047Sample("12", "坏校验+A+B+噪声+A前2字节，应得A/B并留半帧，再用3补齐。", Join(badChecksum, a, b, new byte[] { 0x99, 0xAA, 0x01 })),
                new Task047Sample("13", "原始值0和1000的两个合法边界帧。", Join(CreateFrame(1, 3, 0), CreateFrame(2, 3, 1000))),
                new Task047Sample("14", "2048个99后接A，应清噪声并恢复A。", Join(noise, a))
            };
        }

        private static byte[] CreateFrame(byte deviceId, byte command, int rawValue)
        {
            byte[] frame = new byte[] { 0xAA, deviceId, command, (byte)(rawValue / 256), (byte)(rawValue % 256), 0 };
            int sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum = sum + frame[i];
            }
            frame[5] = (byte)(sum % 256);
            return frame;
        }

        private static byte[] Join(params byte[][] chunks)
        {
            List<byte> bytes = new List<byte>();
            foreach (byte[] chunk in chunks)
            {
                bytes.AddRange(chunk);
            }
            return bytes.ToArray();
        }
    }
}
