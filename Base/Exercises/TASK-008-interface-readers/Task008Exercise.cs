using Base.Exercises.Task001;

namespace Base.Exercises.Task008;

public interface IDeviceReader
{
    // TODO 1：声明 string DeviceId { get; } 和 Task<Reading> ReadAsync(CancellationToken cancellationToken);
    // 接口成员描述调用约定，方法声明不写 async。
    string DeviceId { get; }
    Task<Reading> ReadAsync(CancellationToken cancellationToken);
}

public sealed class TemperatureReader : IDeviceReader
{
    public string DeviceId { get; } = string.Empty;
    private int ReadDelayMs { get; }

    public TemperatureReader(string deviceId, int readDelayMs)
    {
        // TODO 2a：把两个构造函数参数分别保存到当前实例的属性中。
        this.DeviceId = deviceId;
        this.ReadDelayMs = readDelayMs;
    }

    public async Task<Reading>  ReadAsync(CancellationToken cancellationToken)
    {
        // TODO 2b：把此方法改为 async，用 ReadDelayMs 和 cancellationToken
        // 调用并 await Task.Delay，再构造并返回温度读数 Reading。
        // 设备 ID 使用当前实例的 DeviceId，自行设置温度数值与单位。
        await Task.Delay(ReadDelayMs, cancellationToken);
        return new Reading{DeviceId = this.DeviceId, Value = 36, Unit = ".C"};
    }
}

public sealed class PressureReader : IDeviceReader
{
    public string DeviceId { get; } = string.Empty;
    private int ReadDelayMs { get; }

    public PressureReader(string deviceId, int readDelayMs)
    {
        // TODO 3a：把两个构造函数参数分别保存到当前实例的属性中。
        this.DeviceId = deviceId;
        this.ReadDelayMs = readDelayMs;
    }

    public async Task<Reading> ReadAsync(CancellationToken cancellationToken)
    {
        // TODO 3b：把此方法改为 async，用 ReadDelayMs 和 cancellationToken
        // 调用并 await Task.Delay，再构造并返回压力读数 Reading。
        // 设备 ID 使用当前实例的 DeviceId，数值与单位应能和温度读数区分。
        await Task.Delay(ReadDelayMs, cancellationToken);
        return new Reading{DeviceId = this.DeviceId, Value = 40, Unit = "kpa"};
    }
}

public static class Task008Exercise
{
    public static async Task RunAsync()
    {
        Console.WriteLine("TASK-008：用接口统一模拟设备读取");

        // TODO 4a：把一个 TemperatureReader 和一个 PressureReader 加入此集合。
        // 为每个实例提供设备 ID 与正数等待时间，例如 800 和 1200 ms。
        List<IDeviceReader> readers = new();
        readers.Add(new TemperatureReader("Temperature", 100));
        readers.Add(new PressureReader("Pressure", 100));

        // TODO 5：成功运行后，在这里修改一个实例的 ID 或替换一个读取对象，
        // 再次运行观察输出变化，保持 AcquireAsync 不变。
        await AcquireAsync(readers, CancellationToken.None);
    }

    private static async Task AcquireAsync(IEnumerable<IDeviceReader> readers, CancellationToken cancellationToken)
    {
        // TODO 4b：把此方法改为 async，遍历接口类型的读取对象。
        // 每次记录耗时并 await reader.ReadAsync(cancellationToken)，
        // 成功后输出 DeviceId、Value、Unit 和耗时毫秒数。
        // 两种实现使用同一段采集逻辑，不按具体类型写分支或强制转换。
        Console.WriteLine("TODO 1-4：请完成接口、设备读取实现和采集循环。");
        foreach (var reader in readers)
        {
            var reading = await reader.ReadAsync(cancellationToken);
            Console.WriteLine($"reading {reading.DeviceId} {reading.Value} {reading.Unit}");
        }
    }
}
