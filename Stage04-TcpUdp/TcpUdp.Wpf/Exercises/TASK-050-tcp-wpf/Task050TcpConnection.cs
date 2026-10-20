using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpUdp.Exercises.Task049;

namespace TcpUdp.Wpf.Exercises.Task050
{
    // 教练从TASK-049整理的无菜单连接辅助，只返回完整回复，不操作窗口。
    public class Task050TcpConnection
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private readonly Task049LineBuffer _buffer = new Task049LineBuffer();
        private readonly byte[] _readBuffer = new byte[64];

        public bool IsConnected
        {
            get { return _stream != null; }
        }

        public async Task ConnectAsync(string host, int port, CancellationToken cancellationToken)
        {
            Close();
            TcpClient client = new TcpClient();
            try
            {
                using (CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    timeout.CancelAfter(3000);
                    await client.ConnectAsync(host, port, timeout.Token);
                    timeout.Token.ThrowIfCancellationRequested();
                    _stream = client.GetStream();
                    _client = client;
                }
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        public async Task<string> RequestAsync(string command, CancellationToken cancellationToken)
        {
            if (_stream == null)
            {
                throw new IOException("当前没有TCP连接。");
            }
            NetworkStream stream = _stream;
            using (CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(3000);
                byte[] bytes = Encoding.UTF8.GetBytes(command + "\n");
                await stream.WriteAsync(bytes, 0, bytes.Length, timeout.Token);
                string reply;
                while (!_buffer.TryTakeMessage(out reply))
                {
                    int count = await stream.ReadAsync(_readBuffer, 0, _readBuffer.Length, timeout.Token);
                    timeout.Token.ThrowIfCancellationRequested();
                    if (count == 0)
                    {
                        throw new IOException("对端已关闭连接。");
                    }
                    _buffer.Append(_readBuffer, count);
                }
                timeout.Token.ThrowIfCancellationRequested();
                return reply;
            }
        }

        public void Close()
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
            _buffer.Clear();
        }
    }
}
