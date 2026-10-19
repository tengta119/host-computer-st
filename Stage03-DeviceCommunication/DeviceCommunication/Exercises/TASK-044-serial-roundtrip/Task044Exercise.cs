using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace DeviceCommunication.Exercises.Task044
{
    public class Task044Exercise
    {
        private SerialPort _port = new SerialPort();
        private List<byte> _receivedBytes = new List<byte>();
        private int _printedFrameCount = 0;

        public void Run(string[] arguments)
        {
            Console.WriteLine("TASK-044：打通串口收发并解析设备读数");
            PrintPorts();
            if (arguments.Length == 1 && arguments[0] == "--ports")
            {
                return;
            }
            if (arguments.Length == 2 && arguments[0] == "--simulator")
            {
                Task044Simulator simulator = new Task044Simulator();
                simulator.Run(arguments[1]);
                return;
            }

            string? portName;
            if (arguments.Length == 2 && arguments[0] == "--port")
            {
                portName = arguments[1];
            }
            else if (arguments.Length == 0)
            {
                Console.Write("输入你的程序要用的端口名，没有准备好就直接回车退出：");
                portName = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("用法：--ports，或 --port COM号，或 --simulator 另一端COM号。");
                return;
            }
            if (String.IsNullOrWhiteSpace(portName))
            {
                Console.WriteLine("尚未选择串口，本次没有打开任何端口。");
                return;
            }

            _port.PortName = portName.Trim();
            _port.BaudRate = 115200;
            _port.DataBits = 8;
            _port.Parity = Parity.None;
            _port.StopBits = StopBits.One;
            _port.Handshake = Handshake.None;
            _port.ReadTimeout = 500;
            _port.WriteTimeout = 1000;

            Console.WriteLine("当前选择：" + _port.PortName + "，115200 / 8 / 无校验 / 1停止位。");
            Console.WriteLine("o 打开；s 发送请求；r 读取当前收到的数据；c 关闭；q 退出。");
            try
            {
                while (true)
                {
                    Console.Write("操作：");
                    string? command = Console.ReadLine();
                    if (command == null || command == "q")
                    {
                        break;
                    }
                    try
                    {
                        if (command == "o")
                        {
                            OpenPort();
                        }
                        else if (command == "s")
                        {
                            SendRequest();
                        }
                        else if (command == "r")
                        {
                            ReceiveAvailable();
                        }
                        else if (command == "c")
                        {
                            ClosePort();
                        }
                        else
                        {
                            Console.WriteLine("请输入 o、s、r、c 或 q。");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("本次操作失败：" + exception.Message);
                        Console.WriteLine("可以继续操作；端口被占用时先关闭占用它的程序。");
                    }
                }
            }
            finally
            {
                // 教练提供退出时的最后清理，避免练习中途退出仍占用串口，不代替 TOD 4 的主动关闭验证。
                _port.Dispose();
                _receivedBytes.Clear();
            }
        }

        private void OpenPort()
        {
            if (_port.IsOpen)
            {
                Console.WriteLine("这个端口已经打开。");
                return;
            }
            // TOD 1：调用 _port.Open() 打开已选择的端口，成功后打印端口名和 _port.IsOpen，失败交给菜单的 catch 显示。
            _port.Open();
            Console.WriteLine("已打开：" + _port.PortName + "，状态：" + _port.IsOpen);
        }

        private void SendRequest()
        {
            if (!_port.IsOpen)
            {
                Console.WriteLine("请先输入 o 打开串口。");
                return;
            }
            byte[] request = new byte[] { 0x55, 0x01 };
            // TOD 2：调用 _port.Write(request, 0, request.Length) 发送这两个原始字节，然后调用 PrintBytes 显示已发送内容。
            _port.Write(request, 0, request.Length);
            PrintBytes("已发送：", request, request.Length);
        }

        private void ReceiveAvailable()
        {
            if (!_port.IsOpen)
            {
                Console.WriteLine("请先输入 o 打开串口。");
                return;
            }
            if (_port.BytesToRead == 0)
            {
                Console.WriteLine("串口当前没有新字节，缓冲保留数量：" + _receivedBytes.Count);
                return;
            }
            byte[] data = new byte[256];
            // TOD 3：用 _port.Read(data, 0, data.Length) 的返回值保存本次有效数量，再调用 HandleReceivedBytes(data, 有效数量)。
            int bytesRead = _port.Read(data, 0, data.Length);
            HandleReceivedBytes(data, bytesRead);
        }

        private void ClosePort()
        {
            if (!_port.IsOpen)
            {
                Console.WriteLine("端口当前未打开。");
                return;
            }
            // TOD 4：调用 _port.Close() 主动关闭，再清空 _receivedBytes，打印关闭状态，重新 o 打开后核对不会混入旧半帧。
            _port.Close();
            _receivedBytes.Clear();
            Console.WriteLine(_port.PortName + "当前串口已关闭");
        }

        private void PrintReading(byte[] frame)
        {
            // TOD 5：读取 frame[1] 为设备编号，用 frame[3] * 256 + frame[4] 得到原始值，再除以 10.0，打印编号、原始值和一位小数温度。
            byte id = frame[1];
            int rawValue = frame[3] * 256 + frame[4];
            double temperature = rawValue / 10.0;
            Console.WriteLine("设备Id:" + id + ", 原始值:" + rawValue + ", 温度:" + temperature.ToString("F1") + "℃");
        }

        private void HandleReceivedBytes(byte[] data, int bytesRead)
        {
            PrintBytes("本次实际收到：", data, bytesRead);
            Console.WriteLine("本次有效数量：" + bytesRead);
            for (int i = 0; i < bytesRead; i++)
            {
                _receivedBytes.Add(data[i]);
            }
            ProcessBuffer();
            byte[] remaining = _receivedBytes.ToArray();
            PrintBytes("缓冲剩余：", remaining, remaining.Length);
            Console.WriteLine("剩余数量：" + _receivedBytes.Count + "，已取帧数：" + _printedFrameCount);
        }

        private void ProcessBuffer()
        {
            // 教练复用 TASK-043 已验证的处理顺序，不要求重新实现缓冲算法。
            while (_receivedBytes.Count > 0)
            {
                if (_receivedBytes[0] != 0xAA)
                {
                    _receivedBytes.RemoveAt(0);
                    continue;
                }
                if (_receivedBytes.Count < 6)
                {
                    break;
                }
                ExtractOneFrame();
            }
        }

        private void ExtractOneFrame()
        {
            byte[] frame = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                frame[i] = _receivedBytes[i];
            }
            _receivedBytes.RemoveRange(0, 6);
            _printedFrameCount = _printedFrameCount + 1;
            PrintBytes("完整帧" + _printedFrameCount + "：", frame, frame.Length);
            PrintReading(frame);
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

        private void PrintPorts()
        {
            string[] names = SerialPort.GetPortNames();
            Console.WriteLine("系统当前串口：" + String.Join("、", names));
            Console.WriteLine("端口列出来不代表互相连接，请使用已创建的虚拟串口对，两个程序各选一端。");
        }
    }
}
