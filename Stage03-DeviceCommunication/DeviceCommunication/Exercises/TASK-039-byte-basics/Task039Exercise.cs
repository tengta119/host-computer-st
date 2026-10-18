using System;

namespace DeviceCommunication.Exercises.Task039
{
    public class Task039Exercise
    {
        public void Run()
        {
            Console.WriteLine("TASK-039：认识字节与十六进制数据");

            // TODO 1a：把 sample 的占位值 0 改为 0x64，再运行观察下面这一行输出由 0 变为 100。
            byte sample = 0;
            Console.WriteLine(sample);

            // TODO 1b：在这里用同一个 sample 的 ToString("X2") 输出两位大写十六进制，预期看到 64。

            // TODO 2a：在这里用完整的 new byte[] 写法创建 data，按顺序保存 AA 01 03 00 64 5F 六个字节。

            // TODO 2b：输出 data.Length，预期看到 6。

            // TODO 2c：用普通 for 循环遍历实际数组元素，每行输出下标、十进制值和 ToString("X2") 得到的文字。

            // TODO 3：先运行并核对原始六字节输出，再只把数组中第五个字节改为 0x65，重新运行对照下标 4 的变化。
        }
    }
}
