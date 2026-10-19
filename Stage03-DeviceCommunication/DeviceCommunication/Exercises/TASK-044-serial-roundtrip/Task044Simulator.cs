using System;
using System.IO.Ports;

namespace DeviceCommunication.Exercises.Task044
{
    // 这是教练提供的对端工具，不作为学员新增实现，必须打开与上位机不同的另一端串口。
    public class Task044Simulator
    {
        public void Run(string portName)
        {
            SerialPort port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
            port.Handshake = Handshake.None;
            port.ReadTimeout = 500;
            port.WriteTimeout = 1000;
            try
            {
                port.Open();
                Console.WriteLine("模拟设备已打开：" + portName);
                Console.WriteLine("1 发25.0℃完整帧；2 发A前2字节；3 发A剩4字节；4 发A和B；5 发四组读数。");
                Console.WriteLine("6 发99 88加A前2字节；7 发A加99加B帧头；8 发B剩5字节；r 查看请求；q 退出。");
                while (true)
                {
                    Console.Write("模拟设备操作：");
                    string? command = Console.ReadLine();
                    if (command == null || command == "q")
                    {
                        break;
                    }
                    byte[] data;
                    if (command == "1")
                    {
                        data = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0xFA, 0x5F };
                    }
                    else if (command == "2")
                    {
                        data = new byte[] { 0xAA, 0x01 };
                    }
                    else if (command == "3")
                    {
                        data = new byte[] { 0x03, 0x00, 0xFA, 0x5F };
                    }
                    else if (command == "4")
                    {
                        data = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0xFA, 0x5F, 0xAA, 0x02, 0x03, 0x01, 0x04, 0x60 };
                    }
                    else if (command == "5")
                    {
                        data = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0xFA, 0x5F, 0xAA, 0x02, 0x03, 0x01, 0x04, 0x60,
                            0xAA, 0x01, 0x03, 0x01, 0x00, 0x5F, 0xAA, 0x01, 0x03, 0x00, 0x00, 0x5F };
                    }
                    else if (command == "6")
                    {
                        data = new byte[] { 0x99, 0x88, 0xAA, 0x01 };
                    }
                    else if (command == "7")
                    {
                        data = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0xFA, 0x5F, 0x99, 0xAA };
                    }
                    else if (command == "8")
                    {
                        data = new byte[] { 0x02, 0x03, 0x01, 0x04, 0x60 };
                    }
                    else if (command == "r")
                    {
                        if (port.BytesToRead == 0)
                        {
                            Console.WriteLine("当前没有收到请求。");
                        }
                        else
                        {
                            byte[] request = new byte[256];
                            int count = port.Read(request, 0, request.Length);
                            PrintBytes("收到请求：", request, count);
                        }
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("请输入菜单中的操作。");
                        continue;
                    }
                    port.Write(data, 0, data.Length);
                    PrintBytes("模拟设备已发送：", data, data.Length);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("模拟设备失败：" + exception.Message);
            }
            finally
            {
                port.Dispose();
            }
        }

        private void PrintBytes(string label, byte[] data, int count)
        {
            Console.Write(label);
            for (int i = 0; i < count; i++)
            {
                Console.Write(data[i].ToString("X2") + " ");
            }
            Console.WriteLine();
        }
    }
}
