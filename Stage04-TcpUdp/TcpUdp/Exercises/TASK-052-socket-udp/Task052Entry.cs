namespace TcpUdp.Exercises.Task052
{
    public static class Task052Entry
    {
        public static async Task<int> RunAsync(string[] args, CancellationToken token)
        {
            int port = 5000;
            if (args.Length < 2 || args.Length > 4 || (args.Length > 2 && (!Int32.TryParse(args[2], out port) || port < 1 || port > 65535)))
            {
                PrintHelp();
                return 1;
            }
            string mode = args[1];
            string option = args.Length == 4 ? args[3] : (mode == "server" ? "normal" : "GET_TEMP");
            if (mode == "server" && Array.IndexOf(Task052Simulator.Modes, option) >= 0)
            {
                await new Task052Simulator().RunAsync(port, option, token);
                return 0;
            }
            string[] tcpCommands = { "GET_TEMP", "GET_STATUS", "START_DEVICE", "STOP_DEVICE", "PING" };
            if (mode == "socket" && Array.IndexOf(tcpCommands, option) >= 0)
            {
                Console.WriteLine("[Socket完整回复] " + await new Task052SocketExercise().QueryAsync(port, option, token));
                return 0;
            }
            if (mode == "udp" && (option == "GET_TEMP" || option == "GET_STATUS"))
            {
                using (Task052UdpExercise exercise = new Task052UdpExercise())
                {
                    // 同一个UDP端口连续查询两次，便于观察重复/旧回复，超时后仍可进行下一次查询。
                    for (int round = 1; round <= 2; round++)
                    {
                        try { Console.WriteLine("[第" + round + "轮结果] " + await exercise.QueryAsync(port, option, token)); }
                        catch (TimeoutException e) { Console.WriteLine("[第" + round + "轮超时] " + e.Message); }
                    }
                }
                return 0;
            }
            PrintHelp();
            return 1;
        }

        public static void PrintHelp()
        {
            Console.WriteLine("TASK-052：--task052 server [端口] [normal|silent|duplicate|stale-first|foreign-first|wrong-id-first|malformed-first|close-tcp]");
            Console.WriteLine("TASK-052：--task052 socket [端口] [GET_TEMP|GET_STATUS|START_DEVICE|STOP_DEVICE|PING]");
            Console.WriteLine("TASK-052：--task052 udp [端口] [GET_TEMP|GET_STATUS]");
        }
    }
}
