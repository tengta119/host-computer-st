using Base.Exercises.Task001;

namespace Base.Exercises.Task003;

public static class Task003Exercise
{
    public static void Run()
    {
        const string targetDeviceId = "001";
        const string seDeviceId = "002";
        const double threshold = 30.0;

        // TODO 1：加入至少两台设备、合计至少四条 Reading；
        // 同一单位下，让目标设备既有高于阈值的读数，也有不高于阈值的读数。
        var readings = new List<Reading>();
        var rd01 = new Reading{ DeviceId = targetDeviceId, Value = 10, Unit = "km" };
        var rd02 = new Reading{ DeviceId = targetDeviceId, Value = 70, Unit = "km" };
        var rd03 = new Reading{ DeviceId = seDeviceId, Value = 40, Unit = "km" };
        var rd04 = new Reading{ DeviceId = seDeviceId, Value = 60, Unit = "km" };
        readings.Add(rd01);
        readings.Add(rd02);
        readings.Add(rd03);
        readings.Add(rd04);
        // TODO 2：用 Lambda 和 LINQ 同时筛选目标设备及大于阈值的读数。
        // 将下方占位结果替换为真正的查询结果。
        IEnumerable<Reading> matchingReadings = readings.Where(value => value.Value > threshold && value.DeviceId == targetDeviceId);

        Console.WriteLine($"设备 {targetDeviceId} 中高于 {threshold} 的读数：");
        foreach (var reading in matchingReadings)
        {
            Console.WriteLine($"{reading.DeviceId}: {reading.Value} {reading.Unit}");
        }

        // TODO 3：将表示相同条件的 Lambda 传给 CountMatching，
        // 再实现 CountMatching 中的委托调用，并核对计数与上方结果。
        var count = CountMatching(readings, value => value.Value > threshold && value.DeviceId == targetDeviceId);
        Console.WriteLine($"匹配数量：{count}");
    }

    private static int CountMatching(IEnumerable<Reading> readings, Func<Reading, bool> predicate)
    {
        var count = 0;
        foreach (var reading in readings)
        {
            if (predicate(reading))
            {
                count++;
            }
        }
        return count;
    }
}
