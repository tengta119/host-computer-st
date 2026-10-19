using System;

namespace DeviceCommunication.Exercises.Task040
{
    public class Task040Exercise
    {
        public void Run()
        {
            Console.WriteLine("TASK-040：只处理接收数组中的有效字节");

            byte[] receiveBuffer = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0xEE, 0xFF };

            // TOD 3：先完成下面的输出与遍历，再只把 bytesRead 按 6、3、0、6 依次设置并分别运行，保持数组不变。
            int bytesRead = 6;

            PrintReceivedBytes(receiveBuffer, bytesRead);
        }

        private void PrintReceivedBytes(byte[] receiveBuffer, int bytesRead)
        {
            // TOD 1：在这里分别输出 receiveBuffer.Length 和 bytesRead，先运行观察数组长度为 8、本次有效数量为 6。
            Console.WriteLine(receiveBuffer.Length + "," + bytesRead);
            // TOD 2：按本次有效数量遍历，每行输出下标、十进制值和两位大写十六进制，保留有效位置里的 00。
            for (int i = 0; i < bytesRead; i++)
            {
                Console.WriteLine(i + "," + receiveBuffer[i] + "," + receiveBuffer[i].ToString("X2"));
            }
        }
    }
}
