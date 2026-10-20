using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpUdp.Exercises.Task049;

namespace TcpUdp.Exercises.Task052
{
    // 教练创建的练习类，每次请求独占一个Socket，结束或失败时由using释放。
    public class Task052SocketExercise
    {
        public async Task<string> QueryAsync(int port, string command, CancellationToken token)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                deadline.CancelAfter(3000);
                try
                {
                    await ConnectCoreAsync(socket, new IPEndPoint(IPAddress.Loopback, port), deadline.Token);
                    byte[] request = Encoding.ASCII.GetBytes(command + "\n");
                    await SendAllAsync(socket, request, deadline.Token);
                    Task049LineBuffer buffer = new Task049LineBuffer();
                    byte[] received = new byte[3];
                    string reply;
                    while (!buffer.TryTakeMessage(out reply))
                    {
                        int count = await ReceiveCoreAsync(socket, received, deadline.Token);
                        if (count == 0) throw new IOException("对端已关闭，尚未收到完整回复。");
                        buffer.Append(received, count);
                    }
                    deadline.Token.ThrowIfCancellationRequested();
                    return reply;
                }
                catch (OperationCanceledException) when (!token.IsCancellationRequested)
                {
                    throw new TimeoutException("Socket往返超过3秒，本次连接已释放。");
                }
            }
        }

        public async Task ConnectCoreAsync(Socket socket, IPEndPoint endpoint, CancellationToken token)
        {
            // TOD 1：改成async Task方法，await socket.ConnectAsync(endpoint, token)，移除未完成异常。
            await socket.ConnectAsync(endpoint, token);
        }

        public async Task SendAllAsync(Socket socket, byte[] request, CancellationToken token)
        {
            // TOD 2：改成async Task方法，从offset=0开始循环，每次最多提供3字节给SendAsync，根据实际返回数量推进offset，直到发完。
            // TOD 2：本题用Math.Min(3, request.Length - offset)限制每次提供的长度，返回数量小于等于0时抛IOException，不从头重发。
            int offset = 0;
            while (offset < request.Length)
            {
                int count = Math.Min(3, request.Length - offset);
                int sent = await socket.SendAsync(request.AsMemory(offset, count), SocketFlags.None, token);
                if (sent <= 0) throw new IOException("Socket未能继续发送剩余命令。");
                offset += sent;
            }
        }

        public async Task<int> ReceiveCoreAsync(Socket socket, byte[] received, CancellationToken token)
        {
            // TOD 3：改成async Task<int>方法，return await socket.ReceiveAsync(received.AsMemory(), SocketFlags.None, token)，外层只追加实际count。
            return await socket.ReceiveAsync(received.AsMemory(), SocketFlags.None, token);
        }
    }
}
