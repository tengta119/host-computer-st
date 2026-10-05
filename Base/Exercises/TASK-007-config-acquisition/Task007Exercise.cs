using System.Diagnostics;
using System.Text.Json;
using Base.Exercises.Task001;
using Base.Exercises.Task006;

namespace Base.Exercises.Task007;

public static class Task007Exercise
{
    public static async Task RunAsync(string[] args)
    {
        string mode = args.Length == 0 ? "normal" : args[0].ToLowerInvariant();
        if (mode != "normal" && mode != "cancel")
        {
            Console.WriteLine("用法：dotnet run --project Base -- task007 normal 或 task007 cancel");
            return;
        }

        string configPath = Path.Combine(
            AppContext.BaseDirectory, "Exercises", "TASK-007-config-acquisition", "device-config.json");
        Console.WriteLine($"TASK-007 配置路径：{configPath}");

        DeviceConfig? config = await LoadAsync(configPath);
        if (config is null)
        {
            Console.WriteLine("未获得可用配置，本次流程结束。");
            return;
        }

        Console.WriteLine($"配置：设备 {config.DeviceId}，每次等待 {config.ReadDelayMs} ms");
        await RunScenarioAsync(config, cancelEarly: mode == "cancel");
    }

    private static async Task<DeviceConfig?> LoadAsync(string path)
    {
        // TODO 1：改为 async，await 读取文件，用 JsonSerializer 还原 DeviceConfig。
        // 缺失文件、损坏 JSON、反序列化为 null 时提示并返回 null；不得写回配置。
        // 检查 DeviceId 非空、ReadDelayMs > 0；可参考 TASK-006 自己的加载实现。
        DeviceConfig configRes = null;
        try
        {
            string? configJson = await File.ReadAllTextAsync(path);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            DeviceConfig config = JsonSerializer.Deserialize<DeviceConfig>(configJson, jsonSerializerOptions);
            if (config != null && config.ReadDelayMs > 0 && config.DeviceId.Length > 0)
            {
                configRes = config;
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
            Console.WriteLine($"{e.Message} 文件不存在");
            
        }
        catch (JsonException e)
        {
            Console.WriteLine($"{e.Message} 反序列化失败");
        }
        
        
        return configRes;
    }

    private static async Task RunScenarioAsync(DeviceConfig config, bool cancelEarly)
    {
        Console.WriteLine(cancelEarly ? "[取消场景] 开始" : "[正常场景] 开始");
        var timer = Stopwatch.StartNew();

        // TODO 4a：调用方创建并用 using 管理 CancellationTokenSource；
        // 用它的 Token 替换下方 None，并先启动采集、保存返回的 Task。
        // 仅 cancelEarly 时，在第一次读取的等待期间请求取消。
        // 可用 CancelAfter 调度停止请求，或短暂 await 后调用 Cancel；
        // 停止延迟必须小于 config.ReadDelayMs，正常场景不调度取消。
        // TODO 4b：await 采集 Task，专门捕获 OperationCanceledException，
        // 输出“采集已取消”；确保成功、取消两条路径都正确释放取消源。
        // TODO 5：结合代码注释或日志，标出取消条件、谁发出请求、
        // Token 经哪些方法传递，以及哪个等待操作响应它。
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken cancellationToken = cts.Token;
        Task task = AcquireAsync(config, cancellationToken);
        try
        {
            if (cancelEarly)
            {
                cts.CancelAfter(config.ReadDelayMs);
            }
            await task;
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine($"采集已取消 {e.Message}");
        }
        Console.WriteLine($"场景返回，耗时 {timer.ElapsedMilliseconds} ms（含流程开销）。");
    }

    private static async Task AcquireAsync(DeviceConfig config, CancellationToken cancellationToken)
    {
        // TODO 2：改为 async，循环三次；每轮 await ReadAsync(config, cancellationToken)。
        // 记录每轮开始与耗时；只在 await 成功返回后输出序号、设备 ID、数值、单位。
        // 取消异常应传回 RunScenarioAsync，不能捕获后继续输出成功或进入下一轮。
        for (int i = 0; i < 3; i++)
        {
            try
            {
                Reading reading = await ReadAsync(config, cancellationToken);
                Console.WriteLine($"reading,DeviceId: {reading.DeviceId} Value: {reading.Value} {reading.Unit}");
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"读数已取消 {e.Message}");
                throw;
            }
        }
        
        
    }

    private static async Task<Reading> ReadAsync(DeviceConfig config, CancellationToken cancellationToken)
    {
        // TODO 3：改为 async，await 由 config.ReadDelayMs 决定的 Task.Delay，
        // 把 cancellationToken 传给该等待；成功后构造并返回模拟 Reading。
        // DeviceId 来自 config；数值和单位自行设置，不输出被取消读取的成功结果。
        await Task.Delay(config.ReadDelayMs, cancellationToken);
        
        return new Reading { DeviceId = config.DeviceId, Value = 100, Unit = ".C" };
    }
}
