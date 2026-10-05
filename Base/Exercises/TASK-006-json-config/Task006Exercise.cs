using System.Text.Json;
using Base.Exercises.Task001;

namespace Base.Exercises.Task006;

public class DeviceConfig
{
    // TODO 1a：定义 public 属性 DeviceId（string）与 ReadDelayMs（int）。
    // 使用可读写的属性，供 JSON 序列化和反序列化使用。
    public string DeviceId { get; set; } = "";
    public int ReadDelayMs { get; set; } = 0;
}

public static class Task006Exercise
{
    public static async Task RunAsync(string[] args)
    {
        string configPath = Path.Combine(
            AppContext.BaseDirectory, "Exercises", "TASK-006-json-config", "device-config.json");
        Console.WriteLine($"TASK-006 配置路径：{configPath}");
        string mode = args.Length == 0 ? "load" : args[0].ToLowerInvariant();

        if (mode == "save")
        {
            // 入口保护：已有文件由学员手动编辑；此命令只创建初始样例。
            if (File.Exists(configPath))
            {
                Console.WriteLine("配置文件已存在，未覆盖。请编辑原文件后运行 load。");
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
            // TODO 1b：填充样例配置，例如设备 001、等待 1000 ms。
            var deviceConfig002 = new DeviceConfig{DeviceId = "002", ReadDelayMs = 200};
            
            await SaveAsync(configPath, deviceConfig002);
            Console.WriteLine("保存成功");
            return;
        }

        if (mode != "load")
        {
            Console.WriteLine("用法：dotnet run --project Base -- save 或 load");
            return;
        }

        DeviceConfig? config = await LoadAsync(configPath);
        if (config is null)
        {
            Console.WriteLine("未获得可用配置，本次流程结束。");
            return;
        }

        // TODO 5a：打印加载的 DeviceId、ReadDelayMs，再观察读取前后的日志。
        Reading reading = await ReadAsync(config);
        Console.WriteLine($"模拟读数：设备 {reading.DeviceId}，{reading.Value} {reading.Unit} config {config.DeviceId} {config.ReadDelayMs}ms");
    }

    private static async Task SaveAsync(string path, DeviceConfig config)
    {
        // TODO 2：JsonSerializer.Serialize 将 config 转为 JSON 字符串；
        // 用 File.WriteAllTextAsync 写入 path（修改本方法为 async 并 await）。
        // 可用 JsonSerializerOptions 的 WriteIndented 使文件容易阅读。
        // 只有写入成功后才输出保存成功；替换下方占位输出与返回值。
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string configJson = JsonSerializer.Serialize(config, jsonSerializerOptions);
        await File.WriteAllTextAsync(path, configJson);
        
    }

    private static async Task<DeviceConfig?> LoadAsync(string path)
    {
        // TODO 3：File.ReadAllTextAsync 读取 JSON 字符串，
        // JsonSerializer.Deserialize<DeviceConfig> 还原对象并返回。
        // 修改本方法为 async；不要在加载流程中调用 SaveAsync。
        // TODO 4：文件不存在时给出明确提示；捕获 JsonException 提示格式损坏。
        // 这两种情况都返回 null，保留原文件。反序列化结果为 null 也应提示。
        // 可额外检查空设备 ID 和非正数等待时间，避免无效参数参与读取。
        DeviceConfig configRes = null;
        try
        {
            string? configJson = await File.ReadAllTextAsync(path);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            DeviceConfig config = JsonSerializer.Deserialize<DeviceConfig>(configJson, jsonSerializerOptions);
            configRes = config;
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

    private static async Task<Reading> ReadAsync(DeviceConfig config)
    {
        // TODO 5b：改为 async，用加载的 ReadDelayMs 进行 Task.Delay；
        // 构造 Reading，DeviceId 来自 config，数值与单位可沿用模拟样例。
        // 替换占位返回值，不能把设备 ID 与等待时间写死在此处。
        await Task.Delay(config.ReadDelayMs);
        Reading reading = new Reading { DeviceId = config.DeviceId, Value = 100, Unit = ".C" };
        
        return reading;
    }
}
