using Base.Exercises.Task001;

namespace Base.Exercises.Task004;

public static class Task004Exercise
{
    public static async Task RunAsync()
    {
        const string deviceId = "001";
        Console.WriteLine($"开始读取设备 {deviceId} 当前线程为{Environment.CurrentManagedThreadId}");

        Task<Reading> pendingReading = ReadAsync(deviceId);
        Console.WriteLine($"读取方法已调用，准备等待结果，当前线程为{Environment.CurrentManagedThreadId}");
            
        Reading reading = await pendingReading;
        Console.WriteLine($"await pendingReading 得到：设备 {reading.DeviceId}，{reading.Value} {reading.Unit}，当前线程为{Environment.CurrentManagedThreadId}");
    }

    private static async Task<Reading> ReadAsync(string deviceId)
    {
        Console.WriteLine($"进入ReadAsync {DateTime.Now} 当前线程为{Environment.CurrentManagedThreadId}");
        await Task.Delay(8000);
        Console.WriteLine($"ReadAsync {DateTime.Now}");
        Console.WriteLine($"ReadAsync 当前线程为{Environment.CurrentManagedThreadId}");
        return new Reading { DeviceId = deviceId, Value = 100, Unit = "℃" };
    }
}
