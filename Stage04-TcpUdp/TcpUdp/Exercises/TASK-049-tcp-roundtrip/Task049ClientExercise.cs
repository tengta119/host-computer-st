using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpUdp.Exercises.Task049
{
    public class Task049ClientExercise
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private readonly byte[] _readBuffer = new byte[64];
        private readonly Task049LineBuffer _lineBuffer = new Task049LineBuffer();

        public async Task RunAsync(string host, int port, CancellationToken cancellationToken)
        {
            Console.WriteLine("TASK-049上位机：" + host + ":" + port);
            Console.WriteLine("o 连接；1 读温度；2 启动设备；3 停止设备；4 查状态；c 断开；q 退出。");
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.Write("操作：");
                    string? operation = await Console.In.ReadLineAsync(cancellationToken);
                    if (operation == null || operation == "q")
                    {
                        break;
                    }
                    try
                    {
                        if (operation == "o")
                        {
                            await OpenAsync(host, port, cancellationToken);
                        }
                        else if (operation == "c")
                        {
                            CloseConnection();
                        }
                        else
                        {
                            string command = GetDeviceCommand(operation);
                            if (command.Length == 0)
                            {
                                Console.WriteLine("请输入菜单中的操作。");
                                continue;
                            }
                            using (CancellationTokenSource request = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                            {
                                // 教练提供3秒保护，避免TODO遗漏后一直卡住，本题不据此认定学员掌握网络超时设计。
                                request.CancelAfter(3000);
                                string? reply = await SendRequestAsync(command, request.Token);
                                if (reply != null)
                                {
                                    PrintReply(command, reply);
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        CloseConnection();
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        Console.WriteLine("请求超过3秒，旧连接已关闭，请重新连接后操作。");
                    }
                    catch (SocketException exception)
                    {
                        CloseConnection();
                        Console.WriteLine("连接或收发失败：" + exception.Message);
                    }
                    catch (IOException exception)
                    {
                        CloseConnection();
                        Console.WriteLine("接收或协议失败：" + exception.Message);
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        private async Task OpenAsync(string host, int port, CancellationToken cancellationToken)
        {
            CloseConnection();
            // TODO 1：把new TcpClient()创建的对象保存到_client，await它的ConnectAsync(host, port, cancellationToken)，再把GetStream()结果保存到_stream。
            // TODO 1：只有上述操作成功后才输出连接成功，当前这两行只是可编译的占位，完成时移除。
            await Task.CompletedTask;
            Console.WriteLine("TODO 1尚未连接模拟器。");
        }

        private async Task<string?> SendRequestAsync(string command, CancellationToken cancellationToken)
        {
            if (_stream == null)
            {
                Console.WriteLine("请先连接模拟器。");
                return null;
            }
            // TODO 2：用Encoding.UTF8.GetBytes把command加一个LF换行变成byte[]，await _stream.WriteAsync发送完整数组，再return await ReadReplyAsync(cancellationToken)。
            // TODO 2：发送日志放在WriteAsync成功之后，删除下面两行占位，不把写入成功显示成设备控制成功。
            await Task.CompletedTask;
            Console.WriteLine("TODO 2尚未发送请求。");
            return null;
        }

        // 教练提供单一路径的读取循环，新的消息边界判断由学员在Task049LineBuffer内完成。
        private async Task<string> ReadReplyAsync(CancellationToken cancellationToken)
        {
            if (_stream == null)
            {
                throw new IOException("当前没有网络连接。");
            }
            string reply;
            while (!_lineBuffer.TryTakeMessage(out reply))
            {
                int count = await _stream.ReadAsync(_readBuffer, 0, _readBuffer.Length, cancellationToken);
                if (count == 0)
                {
                    throw new IOException("对端已经正常关闭发送，本次连接结束。");
                }
                Console.WriteLine("本次实际接收" + count + "字节：" + BitConverter.ToString(_readBuffer, 0, count));
                _lineBuffer.Append(_readBuffer, count);
            }
            Console.WriteLine("完整回复：" + reply);
            return reply;
        }

        private void CloseConnection()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
            _lineBuffer.Clear();
        }

        private static string GetDeviceCommand(string operation)
        {
            switch (operation)
            {
                case "1": return "GET_TEMP";
                case "2": return "START_DEVICE";
                case "3": return "STOP_DEVICE";
                case "4": return "GET_STATUS";
                default: return String.Empty;
            }
        }

        private static void PrintReply(string command, string reply)
        {
            if (command == "GET_TEMP")
            {
                double temperature;
                if (!Double.TryParse(reply, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out temperature) || !Double.IsFinite(temperature))
                {
                    throw new InvalidDataException("温度回复不是约定的小数。");
                }
                Console.WriteLine("温度：" + temperature.ToString("F1", CultureInfo.InvariantCulture) + "℃");
            }
            else if (command == "GET_STATUS" && (reply == "RUNNING" || reply == "STOPPED"))
            {
                Console.WriteLine("模拟器设备状态：" + reply);
            }
            else if ((command == "START_DEVICE" || command == "STOP_DEVICE") && reply == "OK")
            {
                Console.WriteLine("模拟器已确认执行：" + command);
            }
            else
            {
                throw new InvalidDataException("本次回复与请求不匹配：" + reply);
            }
        }
    }
}
