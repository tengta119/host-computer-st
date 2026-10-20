using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpUdp.Exercises.Task052
{
    // 教练创建的UDP查询类，同一对象保留同一个本地端口，以便观察上一查询留下的重复和旧回复。
    public class Task052UdpExercise : IDisposable
    {
        private readonly UdpClient _udp = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);
        public const int MaximumDatagramBytes = 128;

        public async Task<string> QueryAsync(int port, string command, CancellationToken token)
        {
            if (command != "GET_TEMP" && command != "GET_STATUS") throw new ArgumentException("UDP仅用于查询。");
            await _gate.WaitAsync(token);
            try
            {
                IPEndPoint expected = new IPEndPoint(IPAddress.Loopback, port);
                string requestId = Guid.NewGuid().ToString("N");
                byte[] request = Encoding.ASCII.GetBytes("Q|" + requestId + "|" + command);
                using (CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    deadline.CancelAfter(2000);
                    try
                    {
                        Console.WriteLine("[UDP请求] " + expected + " " + Encoding.ASCII.GetString(request));
                        await SendQueryAsync(_udp, request, expected, deadline.Token);
                        while (true)
                        {
                            UdpReceiveResult datagram = await _udp.ReceiveAsync(deadline.Token);
                            deadline.Token.ThrowIfCancellationRequested();
                            string reply;
                            if (TryAcceptDatagram(datagram, expected, requestId, command, out reply))
                            {
                                Console.WriteLine("[UDP接受] " + requestId + " " + reply);
                                return reply;
                            }
                            Console.WriteLine("[UDP忽略] 来源=" + datagram.RemoteEndPoint + " 字节数=" + datagram.Buffer.Length);
                        }
                    }
                    catch (OperationCanceledException) when (!token.IsCancellationRequested)
                    {
                        throw new TimeoutException("UDP查询超过2秒，已结束本编号等待。");
                    }
                }
            }
            finally { _gate.Release(); }
        }

        public Task SendQueryAsync(UdpClient udp, byte[] request, IPEndPoint expected, CancellationToken token)
        {
            // TODO 4：改成async Task方法，await udp.SendAsync(request.AsMemory(), expected, token)，一条request只发一个数据报。
            return Task.FromException(new NotImplementedException("TODO 4：尚未发送UDP查询。"));
        }

        public bool TryAcceptDatagram(UdpReceiveResult datagram, IPEndPoint expected, string requestId, string command, out string reply)
        {
            reply = String.Empty;
            // TODO 5：检查来源地址和端口、1至128字节、可打印ASCII及R|编号|命令|值四段，编号/命令必须匹配。
            // TODO 5：GET_TEMP只接受InvariantCulture可解析的0至100有限小数，GET_STATUS只接受RUNNING或STOPPED，成功将第四段赋给reply并返回true。
            return false;
        }

        // 只在本轮查询已结束后释放；运行入口在所有await完成后调用，不与接收同时Dispose。
        public void Dispose() { _udp.Dispose(); _gate.Dispose(); }
    }
}
