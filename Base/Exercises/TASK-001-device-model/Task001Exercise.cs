namespace Base.Exercises.Task001;

// TODO 1：根据设备场景完善属性，创建至少两个 Device 和各自的 Reading。
public class Device 
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
}

public class Reading 
{
    public string DeviceId { get; init; } = "";
    public double Value { get; init; }
    public string Unit { get; init; } = "";
}
public static class Task001Exercise 
{
    public static void Run() 
    {
        // TODO 2：用 List<Device> 和 List<Reading> 保存样例数据；
        // 再用 Dictionary<string, ...> 建立设备 ID 与读数的关联（类型自行选择）。
        // 教练示范：按设备 ID 查询，分别处理设备不存在和设备没有读数。
        var device001 =  new Device { Id = "001", Name = "温度传感器" };
        var device002 = new Device{Id = "002", Name = "光线传感器"};
        var device003 = new Device { Id = "003", Name = "湿度传感器" };
        var reading001 = new Reading { DeviceId = "001", Value = 5.0, Unit = "01"};
        var reading002 = new Reading { DeviceId = "002", Value = 6.0, Unit = "02"};
        
        List<Device> listsDevice = new List<Device>();
        listsDevice.Add(device001);
        listsDevice.Add(device002);
        listsDevice.Add(device003);
        List<Reading> listsReading = new List<Reading>();
        listsReading.Add(reading001);
        listsReading.Add(reading002);
        
        Dictionary<string, Reading> dictReading = new Dictionary<string, Reading>();
        Dictionary<string, Device> dictDevice = new Dictionary<string, Device>();
        dictDevice.Add(device001.Id, device001);
        dictDevice.Add(device002.Id, device002);
        dictDevice.Add(device003.Id, device003);
        dictReading.Add(reading001.DeviceId, reading001);
        dictReading.Add(reading002.DeviceId, reading002);

        foreach (var deviceId in new[] { "001", "002", "999", "003" })
        {
            if (!dictDevice.TryGetValue(deviceId, out var device))
            {
                Console.WriteLine($"设备 {deviceId} 不存在");
                continue;
            }

            if (!dictReading.TryGetValue(deviceId, out var reading))
            {
                Console.WriteLine($"设备 {device.Id}（{device.Name}）没有读数");
                continue;
            }

            Console.WriteLine($"设备 {device.Id}（{device.Name}）：{reading.Value} {reading.Unit}");
        }
    }
}
