using Base.Exercises.Task001;

namespace Base.Exercises.Task005;

public static class Task005Exercise
{
    public static async Task RunAsync()
    {
        Console.WriteLine("TASK-005 骨架：取消逻辑尚未实现。");
        await RunScenarioAsync("正常读取", cancelEarly: false);
        await RunScenarioAsync("提前取消", cancelEarly: true);
    }

    private static async Task RunScenarioAsync(string scenario, bool cancelEarly)
    {
        const string deviceId = "001";
        Console.WriteLine($"[{scenario}] 开始读取设备 {deviceId}");

        // TODO 2：创建并释放 CancellationTokenSource，用它的 Token 替换下方占位令牌。
        // cancelEarly 为 true 时，在读取完成前发出取消信号（例如 500 ms 后）。
        // 正常情形不要发出取消信号；ReadAsync 的模拟等待为 2000 ms。
        using var cts = new CancellationTokenSource();
        CancellationToken cancellationToken = cts.Token;
        
        // TODO 3：await 正常完成后输出 reading 的设备 ID、数值、单位；
        // 在调用方捕获 OperationCanceledException 并输出取消结果。
        // 取消路径不能输出成功读数；结合两种输出说明信号的传递路径。
        try
        {
            Task<Reading> pendingReading = ReadAsync(deviceId, cancellationToken);
            if (cancelEarly)
            {
                cts.Cancel();
            }

            var reading = await pendingReading;
            Console.WriteLine($"reading {reading.DeviceId} {reading.Value} {reading.Unit}");
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine($"读取已取消 {e.Message}");
        }
        Console.WriteLine($"[{scenario}] 占位：方法已返回，结果处理待完成。");
    }

    private static async Task<Reading> ReadAsync(string deviceId, CancellationToken cancellationToken)
    {
        // TODO 1：让异步等待接收 cancellationToken，以响应取消。
        await Task.Delay(2000, cancellationToken);
        return new Reading { DeviceId = deviceId, Value = 100, Unit = "℃" };
    }
}
