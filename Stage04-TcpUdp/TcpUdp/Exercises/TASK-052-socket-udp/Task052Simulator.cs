using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpUdp.Exercises.Task049;

namespace TcpUdp.Exercises.Task052
{
    // 教练模拟端，TCP与UDP读取同一个运行状态，关键查询实现留在两个Exercise类中。
    public class Task052Simulator
    {
        private volatile bool _running;
        public static readonly string[] Modes = { "normal", "silent", "duplicate", "stale-first", "foreign-first", "wrong-id-first", "malformed-first", "close-tcp" };

        public async Task RunAsync(int port, string mode, CancellationToken token)
        {
            using (CancellationTokenSource stop = CancellationTokenSource.CreateLinkedTokenSource(token))
            using (UdpClient udp = new UdpClient(new IPEndPoint(IPAddress.Loopback, port)))
            {
                TcpListener listener = new TcpListener(IPAddress.Loopback, port);
                List<Task> clients = new List<Task>();
                Task udpLoop = Task.CompletedTask;
                try
                {
                    listener.Start();
                    Console.WriteLine("[TASK-052就绪] TCP/UDP 127.0.0.1:" + port + " 模式=" + mode);
                    udpLoop = RunUdpAsync(udp, mode, stop.Token);
                    Task acceptLoop = AcceptAsync(listener, mode, clients, stop.Token);
                    await Task.WhenAny(udpLoop, acceptLoop);
                    stop.Cancel();
                    await Task.WhenAll(udpLoop, acceptLoop);
                }
                catch (OperationCanceledException) when (stop.IsCancellationRequested) { }
                finally
                {
                    stop.Cancel();
                    listener.Stop();
                    try { await udpLoop; } catch (OperationCanceledException) when (stop.IsCancellationRequested) { }
                    await Task.WhenAll(clients);
                    Console.WriteLine("[TASK-052停止] 已结束TCP会话与UDP接收。");
                }
            }
        }

        private async Task AcceptAsync(TcpListener listener, string mode, List<Task> clients, CancellationToken token)
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(token);
                clients.RemoveAll(task => task.IsCompletedSuccessfully);
                clients.Add(ServeTcpAsync(client, mode, token));
            }
        }

        private async Task ServeTcpAsync(TcpClient client, string mode, CancellationToken token)
        {
            using (client)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    Task049LineBuffer buffer = new Task049LineBuffer();
                    byte[] received = new byte[64];
                    while (true)
                    {
                        int count = await stream.ReadAsync(received, token);
                        if (count == 0) return;
                        buffer.Append(received, count);
                        string command;
                        while (buffer.TryTakeMessage(out command))
                        {
                            if (mode == "close-tcp") return;
                            string reply = CreateReply(command);
                            byte[] bytes = Encoding.ASCII.GetBytes(reply + "\n");
                            await stream.WriteAsync(bytes.AsMemory(0, 2), token);
                            await Task.Delay(60, token);
                            await stream.WriteAsync(bytes.AsMemory(2), token);
                            Console.WriteLine("[TCP] " + command + " → " + reply);
                        }
                    }
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested) { }
                catch (IOException e) { Console.WriteLine("[TCP结束] " + e.Message); }
                catch (SocketException e) { Console.WriteLine("[TCP结束] " + e.Message); }
            }
        }

        private string CreateReply(string command)
        {
            if (command == "START_DEVICE") { _running = true; return "OK"; }
            if (command == "STOP_DEVICE") { _running = false; return "OK"; }
            if (command == "GET_STATUS") return _running ? "RUNNING" : "STOPPED";
            if (command == "GET_TEMP") return "35.6";
            if (command == "PING") return "PONG";
            return "ERR_UNKNOWN_COMMAND";
        }

        private async Task RunUdpAsync(UdpClient udp, string mode, CancellationToken token)
        {
            Dictionary<string, string> held = new Dictionary<string, string>();
            while (true)
            {
                UdpReceiveResult request = await udp.ReceiveAsync(token);
                if (request.Buffer.Length == 0 || request.Buffer.Length > 128 || request.Buffer.Any(b => b < 32 || b > 126)) continue;
                string[] parts = Encoding.ASCII.GetString(request.Buffer).Split('|');
                Guid id;
                if (parts.Length != 3 || parts[0] != "Q" || !Guid.TryParseExact(parts[1], "N", out id) || (parts[2] != "GET_TEMP" && parts[2] != "GET_STATUS")) continue;
                string reply = "R|" + parts[1] + "|" + parts[2] + "|" + CreateReply(parts[2]);
                Console.WriteLine("[UDP] " + request.RemoteEndPoint + " " + String.Join('|', parts) + " → " + reply);
                if (mode == "silent") continue;
                if (mode == "stale-first")
                {
                    string key = request.RemoteEndPoint.ToString();
                    string? old;
                    if (!held.Remove(key, out old))
                    {
                        held[key] = reply;
                        continue;
                    }
                    await SendDatagramAsync(udp, old, request.RemoteEndPoint, token);
                }
                if (mode == "foreign-first")
                {
                    using (UdpClient other = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0)))
                        await SendDatagramAsync(other, reply, request.RemoteEndPoint, token);
                }
                if (mode == "wrong-id-first") await SendDatagramAsync(udp, "R|" + Guid.NewGuid().ToString("N") + "|" + parts[2] + "|35.6", request.RemoteEndPoint, token);
                if (mode == "malformed-first")
                {
                    await SendDatagramAsync(udp, new string('X', 129), request.RemoteEndPoint, token);
                    await SendDatagramAsync(udp, "R|" + parts[1] + "|" + parts[2] + "|INVALID", request.RemoteEndPoint, token);
                }
                await SendDatagramAsync(udp, reply, request.RemoteEndPoint, token);
                if (mode == "duplicate") await SendDatagramAsync(udp, reply, request.RemoteEndPoint, token);
            }
        }

        private static async Task SendDatagramAsync(UdpClient udp, string text, IPEndPoint target, CancellationToken token)
        {
            await udp.SendAsync(Encoding.ASCII.GetBytes(text).AsMemory(), target, token);
        }
    }
}
