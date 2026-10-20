using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpUdp.Exercises.Task049;
using TcpUdp.Exercises.Task051;
using TcpUdp.Exercises.Task052;

namespace TcpUdp
{
    // 教练提供的共享入口只负责选择运行模式，任务实现放在独立练习目录。
    public class Program
    {
        private static readonly CancellationTokenSource Shutdown = new CancellationTokenSource();

        public static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            Console.CancelKeyPress += HandleCancel;
            try
            {
                if (args.Length > 0 && args[0] == "--task052")
                {
                    return await Task052Entry.RunAsync(args, Shutdown.Token);
                }
                if (args.Length > 0 && args[0] == "--task051-simulator")
                {
                    int faultPort = 5000;
                    if (args.Length > 3 || (args.Length > 1 && (!Int32.TryParse(args[1], out faultPort) || faultPort < 1 || faultPort > 65535)))
                    {
                        Console.WriteLine("用法：--task051-simulator [端口] [normal|silent-temp|delay-temp|close-temp|silent-ping]。");
                        return 1;
                    }
                    string mode = args.Length == 3 ? args[2] : "normal";
                    string[] allowedModes = new string[] { "normal", "silent-temp", "delay-temp", "close-temp", "silent-ping" };
                    if (Array.IndexOf(allowedModes, mode) < 0)
                    {
                        Console.WriteLine("故障模式不合法，请使用normal、silent-temp、delay-temp、close-temp或silent-ping。");
                        return 1;
                    }
                    Task051Simulator simulator = new Task051Simulator();
                    await simulator.RunAsync(faultPort, mode, Shutdown.Token);
                    return 0;
                }
                if (args.Length == 1 && args[0] == "--observe")
                {
                    Task049BufferObservation.Run();
                    return 0;
                }
                if (args.Length == 0 || (args.Length == 1 && args[0] == "--help"))
                {
                    PrintHelp();
                    return 0;
                }
                if (args[0] != "--client" && args[0] != "--simulator")
                {
                    PrintHelp();
                    return 1;
                }

                int port = 5000;
                if (args.Length > 1 && (!Int32.TryParse(args[1], out port) || port < 1 || port > 65535))
                {
                    Console.WriteLine("端口请输入1至65535之间的整数。");
                    return 1;
                }
                bool splitResponses = args.Length == 3 && args[2] == "split";
                if (args.Length > 3 || (args.Length == 3 && (!splitResponses || args[0] != "--simulator")))
                {
                    PrintHelp();
                    return 1;
                }
                if (args[0] == "--simulator")
                {
                    Task049Simulator simulator = new Task049Simulator();
                    await simulator.RunAsync(port, splitResponses, Shutdown.Token);
                }
                else
                {
                    Task049ClientExercise exercise = new Task049ClientExercise();
                    await exercise.RunAsync("127.0.0.1", port, Shutdown.Token);
                }
                return 0;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("已停止本次运行。");
                return 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine("运行失败：" + exception.Message);
                return 1;
            }
            finally
            {
                Console.CancelKeyPress -= HandleCancel;
                Shutdown.Dispose();
            }
        }

        private static void HandleCancel(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Shutdown.Cancel();
        }

        private static void PrintHelp()
        {
            Task052Entry.PrintHelp();
            Console.WriteLine("TASK-049：控制台上位机与TCP设备模拟器。");
            Console.WriteLine("--simulator [端口] [split]：运行教练提供的模拟器，默认端口5000。");
            Console.WriteLine("--client [端口]：运行待完成的上位机，默认端口5000。");
            Console.WriteLine("--observe：观察学员缓冲实现的当前输出，不需要联网。");
            Console.WriteLine("请先阅读Exercises/TASK-049-tcp-roundtrip/RUN-GUIDE.md。");
            Console.WriteLine("TASK-051对端：--task051-simulator [端口] [normal|silent-temp|delay-temp|close-temp|silent-ping]。");
        }
    }
}
