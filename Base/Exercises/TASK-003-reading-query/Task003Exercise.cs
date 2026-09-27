using Base.Exercises.Task001;

namespace Base.Exercises.Task003;

public static class Task003Exercise
{
    public static void Run()
    {
        const string targetDeviceId = "001";
        const double threshold = 30.0;

        // TODO 1：加入至少两台设备、合计至少四条 Reading；
        // 同一单位下，让目标设备既有高于阈值的读数，也有不高于阈值的读数。
        var readings = new List<Reading>();

        // TODO 2：用 Lambda 和 LINQ 同时筛选目标设备及大于阈值的读数。
        // 将下方占位结果替换为真正的查询结果。
        IEnumerable<Reading> matchingReadings = Array.Empty<Reading>();

        Console.WriteLine($"设备 {targetDeviceId} 中高于 {threshold} 的读数：");
        foreach (var reading in matchingReadings)
        {
            Console.WriteLine($"{reading.DeviceId}: {reading.Value} {reading.Unit}");
        }

        // TODO 3：将表示相同条件的 Lambda 传给 CountMatching，
        // 再实现 CountMatching 中的委托调用，并核对计数与上方结果。
        var count = CountMatching(readings, reading => false);
        Console.WriteLine($"匹配数量：{count}");
    }

    private static int CountMatching(IEnumerable<Reading> readings, Func<Reading, bool> predicate)
    {
        // TODO 3：逐条调用 predicate；只有返回 true 时才计入结果。
        return 0;
    }
}
