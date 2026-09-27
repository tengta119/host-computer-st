using Base.Exercises.Task001;

namespace Base.Exercises.Task002;

public sealed class ReadingReceivedEventArgs : EventArgs
{
    public Reading Reading { get; }

    public ReadingReceivedEventArgs(Reading reading)
    {
        Reading = reading;
    }
}

public sealed class SimulatedDevice
{
    public string Id { get; }

    public SimulatedDevice(string id)
    {
        Id = id;
    }

    // TODO 1：声明一个携带 ReadingReceivedEventArgs 的读数事件。
    public event EventHandler<ReadingReceivedEventArgs>? ReadingReceived;

    public void ProduceReading(double value, string unit)
    {
        // TODO 1：用 Id、value、unit 创建 Reading，再向订阅者发布事件。
        var reading = new Reading {DeviceId = Id, Value = value, Unit = unit };
        var readingEventArgs = new ReadingReceivedEventArgs(reading);
        
        ReadingReceived?.Invoke(this, readingEventArgs);
    }
}

public static class Task002Exercise
{
    public static void Run()
    {
        var device = new SimulatedDevice("001");
        device.ReadingReceived += OnReadingReceived;
        // TODO 2：订阅读数事件，让 OnReadingReceived 处理通知。
        device.ProduceReading(5.0, "℃");
        device.ProduceReading(6.0, "℃");

        device.ReadingReceived -= OnReadingReceived;
        // TODO 3：取消同一个处理器的订阅，再产生一次读数并观察输出。
        device.ProduceReading(7.0, "℃");
    }

    private static void OnReadingReceived(object? sender, ReadingReceivedEventArgs e)
    {
        // TODO 2：输出 e.Reading 的设备 ID、数值和单位。
        var eReading = e.Reading;
        Console.WriteLine($"eReading: {eReading.DeviceId} {eReading.Value} {eReading.Unit}");
        
    }
}
