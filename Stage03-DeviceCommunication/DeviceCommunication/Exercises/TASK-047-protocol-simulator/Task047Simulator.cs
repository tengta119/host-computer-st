using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace DeviceCommunication.Exercises.Task047
{
    // 这是教练提供的配套模拟端，沿用虚拟串口对，不自动回复请求，也不属于学员TODO。
    public class Task047Simulator
    {
        public static void PrintSamples()
        {
            foreach (Task047Sample sample in Task047Samples.CreateAll())
            {
                Console.WriteLine(sample.Command + "：" + sample.Description);
                PrintBytes("字节：", sample.Data);
            }
        }

        public void Run(string portName)
        {
            List<Task047Sample> samples = Task047Samples.CreateAll();
            SerialPort port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
            port.Handshake = Handshake.None;
            port.ReadTimeout = 500;
            port.WriteTimeout = 1000;
            try
            {
                port.Open();
                Console.WriteLine("TASK-047模拟端已打开：" + portName);
                foreach (Task047Sample sample in samples)
                {
                    Console.WriteLine(sample.Command + "：" + sample.Description);
                }
                Console.WriteLine("r读取请求，q退出；菜单操作只负责发字节，不自动回复。");
                while (true)
                {
                    Console.Write("TASK-047操作：");
                    string? command = Console.ReadLine();
                    if (command == null || command == "q")
                    {
                        break;
                    }
                    if (command == "r")
                    {
                        if (port.BytesToRead == 0)
                        {
                            Console.WriteLine("当前没有请求。");
                        }
                        else
                        {
                            byte[] request = new byte[256];
                            int count = port.Read(request, 0, Math.Min(request.Length, port.BytesToRead));
                            byte[] received = new byte[count];
                            Array.Copy(request, received, count);
                            PrintBytes("收到请求：", received);
                        }
                        continue;
                    }
                    Task047Sample? selected = null;
                    foreach (Task047Sample sample in samples)
                    {
                        if (sample.Command == command)
                        {
                            selected = sample;
                            break;
                        }
                    }
                    if (selected == null)
                    {
                        Console.WriteLine("请输入菜单中的编号、r或q。");
                        continue;
                    }
                    port.Write(selected.Data, 0, selected.Data.Length);
                    PrintBytes("发送：", selected.Data);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("TASK-047模拟端失败：" + exception.Message);
            }
            finally
            {
                port.Dispose();
            }
        }

        private static void PrintBytes(string label, byte[] data)
        {
            Console.Write(label);
            foreach (byte value in data)
            {
                Console.Write(value.ToString("X2") + " ");
            }
            Console.WriteLine();
        }
    }
}
