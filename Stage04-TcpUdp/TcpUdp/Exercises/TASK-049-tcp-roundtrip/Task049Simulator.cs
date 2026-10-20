using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpUdp.Exercises.Task049
{
    // 这是教练提供的 TCP 设备对端支持，不计为学员自主实现或掌握 TCP Server 的证据。
    public class Task049Simulator
    {
        private const int MaximumCommandBytes = 256;
        private readonly Encoding responseEncoding = new UTF8Encoding(false);
        private bool running;

        public async Task RunAsync(int port, bool splitResponses, CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                listener.Start();
                Console.WriteLine($"[模拟器] 正在监听 {listener.LocalEndpoint}，每条命令和回复以 LF 结束。");

                if (splitResponses)
                {
                    Console.WriteLine("[模拟器] 已启用回复分段观察：先写前 2 字节，等待 150 毫秒，再写其余字节。");
                    Console.WriteLine("[模拟器] 网络接收分段可能与上述写入分段不同，客户端仍必须以 LF 判断完整回复。");
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    using (TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken))
                    {
                        try
                        {
                            Console.WriteLine($"[模拟器] 接受连接：{client.Client.RemoteEndPoint}。");

                            using (NetworkStream stream = client.GetStream())
                            {
                                await ProcessConnectionAsync(stream, splitResponses, cancellationToken);
                            }
                        }
                        catch (IOException exception)
                        {
                            Console.WriteLine($"[模拟器] 当前连接发生 IO 错误：{exception.Message}");
                        }
                        catch (SocketException exception)
                        {
                            Console.WriteLine($"[模拟器] 当前连接发生 Socket 错误：{exception.Message}");
                        }
                        finally
                        {
                            Console.WriteLine("[模拟器] 关闭当前连接。");
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("[模拟器] 已收到停止请求。");
            }
            finally
            {
                listener.Stop();
                Console.WriteLine("[模拟器] 已停止监听并释放连接资源。");
            }
        }

        private async Task ProcessConnectionAsync(NetworkStream stream, bool splitResponses, CancellationToken cancellationToken)
        {
            byte[] receiveBuffer = new byte[1024];
            byte[] commandBuffer = new byte[MaximumCommandBytes];
            int commandByteCount = 0;

            while (true)
            {
                int receivedByteCount = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length, cancellationToken);

                if (receivedByteCount == 0)
                {
                    if (commandByteCount > 0)
                    {
                        Console.WriteLine("[模拟器] 对端已断开，丢弃尚未收到 LF 的命令片段。");
                    }
                    else
                    {
                        Console.WriteLine("[模拟器] 对端已断开。");
                    }

                    return;
                }

                for (int index = 0; index < receivedByteCount; index++)
                {
                    byte currentByte = receiveBuffer[index];

                    if (currentByte == 10)
                    {
                        string command = Encoding.ASCII.GetString(commandBuffer, 0, commandByteCount);
                        commandByteCount = 0;

                        if (command.Length == 0)
                        {
                            Console.WriteLine("[模拟器] 请求：<空行>。");
                        }
                        else
                        {
                            Console.WriteLine($"[模拟器] 请求：{command}");
                        }

                        string response = CreateResponse(command);
                        await SendResponseAsync(stream, response, splitResponses, cancellationToken);
                        continue;
                    }

                    // 除行尾 LF 外，命令只接受可打印 ASCII 字节，不接受 CR、其他控制字节或非 ASCII 字节。
                    if (currentByte < 32 || currentByte > 126)
                    {
                        Console.WriteLine($"[模拟器] 非法命令字节 0x{currentByte:X2}，关闭当前连接后继续监听。");
                        return;
                    }

                    if (commandByteCount >= MaximumCommandBytes)
                    {
                        Console.WriteLine($"[模拟器] 命令超过 {MaximumCommandBytes} 字节上限，关闭当前连接后继续监听。");
                        return;
                    }

                    commandBuffer[commandByteCount] = currentByte;
                    commandByteCount++;
                }
            }
        }

        private string CreateResponse(string command)
        {
            switch (command)
            {
                case "":
                    return "ERR_EMPTY_COMMAND";
                case "GET_TEMP":
                    return "35.6";
                case "START_DEVICE":
                    running = true;
                    return "OK";
                case "STOP_DEVICE":
                    running = false;
                    return "OK";
                case "GET_STATUS":
                    if (running)
                    {
                        return "RUNNING";
                    }

                    return "STOPPED";
                case "PING":
                    return "PONG";
                default:
                    return "ERR_UNKNOWN_COMMAND";
            }
        }

        private async Task SendResponseAsync(NetworkStream stream, string response, bool splitResponses, CancellationToken cancellationToken)
        {
            byte[] responseBytes = responseEncoding.GetBytes(response + "\n");

            if (splitResponses)
            {
                await stream.WriteAsync(responseBytes, 0, 2, cancellationToken);
                await Task.Delay(150, cancellationToken);
                await stream.WriteAsync(responseBytes, 2, responseBytes.Length - 2, cancellationToken);
            }
            else
            {
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
            }

            Console.WriteLine($"[模拟器] 回复：{response}");
        }
    }
}
