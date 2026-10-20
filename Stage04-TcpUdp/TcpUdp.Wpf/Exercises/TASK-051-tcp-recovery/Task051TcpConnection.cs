using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpUdp.Exercises.Task049;

namespace TcpUdp.Wpf.Exercises.Task051
{
    // 教练新建的异常类，Operation保存这次超时正在等待什么，不是.NET内置的异常属性。
    public class Task051TimeoutException : TimeoutException
    {
        public string Operation { get; private set; }

        public Task051TimeoutException(string operation) : base(operation + "超过3秒。")
        {
            Operation = operation;
        }
    }

    // 复用TASK-050收发流程，教练只增加能区分等待位置的超时异常。
    public class Task051TcpConnection
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
            catch (OperationCanceledException)
            {
                client.Dispose();
                if (cancellationToken.IsCancellationRequested) throw;
                throw new Task051TimeoutException("连接");
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        public async Task<string> RequestAsync(string command, CancellationToken cancellationToken)
        {
            if (_stream == null) throw new IOException("当前没有TCP连接。");
            NetworkStream stream = _stream;
            using (CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(3000);
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(command + "\n");
                    await stream.WriteAsync(bytes, 0, bytes.Length, timeout.Token);
                    string reply;
                    while (!_buffer.TryTakeMessage(out reply))
                    {
                        int count = await stream.ReadAsync(_readBuffer, 0, _readBuffer.Length, timeout.Token);
                        timeout.Token.ThrowIfCancellationRequested();
                        if (count == 0) throw new IOException("对端主动结束发送，本次连接关闭。");
                        _buffer.Append(_readBuffer, count);
                    }
                    timeout.Token.ThrowIfCancellationRequested();
                    return reply;
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested) throw;
                    if (command == "PING") throw new Task051TimeoutException("心跳");
                    throw new Task051TimeoutException("业务请求");
                }
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
