using DeviceCommunication.Exercises.Task047;
using DeviceCommunication.Exercises.Task044;

if (args.Length > 0 && args[0] == "--task047-list")
{
    Task047Simulator.PrintSamples();
    return;
}
if (args.Length > 0 && args[0] == "--task047-simulator")
{
    if (args.Length != 2)
    {
        Console.WriteLine("用法：--task047-simulator 对端端口名，例如COM2。");
        return;
    }
    Task047Simulator simulator = new Task047Simulator();
    simulator.Run(args[1]);
    return;
}

Console.WriteLine("Hello, World!");

Task044Exercise exercise = new Task044Exercise();
exercise.Run(args);
