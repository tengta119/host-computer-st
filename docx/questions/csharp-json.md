# C# JSON 与配置文件

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：JsonSerializerOptions.WriteIndented 有什么用，如何使用？

- 状态：已掌握
- 日期：2026-10-05
- 关联：[TASK-006](../tasks/TASK-006.md) 的 TODO 2。

TASK-006 保存设备配置时，生成紧凑的一行 JSON 可以被程序读取，但人手工看 DeviceId、ReadDelayMs 不方便。WriteIndented 的效果是给 JSON 换行、缩进，字段和值保持原样。

在已有 SaveAsync 方法中可这样读这三行：

```csharp
JsonSerializerOptions options = new JsonSerializerOptions();
options.WriteIndented = true;
string json = JsonSerializer.Serialize(config, options);
```

JsonSerializerOptions 是 .NET 提供的选项类，new 创建选项对象，options 保存它的引用。第二行设置这个对象的 WriteIndented 属性。JsonSerializer.Serialize 是 .NET 提供的序列化方法；config 是传入 SaveAsync 的本项目 DeviceConfig 对象，options 是本次使用的选项。

Serialize 得到 string，保存到 json。若 config 的 DeviceId 为 002、ReadDelayMs 为 200，便得到类似：

```json
{
  "DeviceId": "002",
  "ReadDelayMs": 200
}
```

只创建 options 却没传给 Serialize，本次调用仍不会使用这项设置。设置为 false 时通常得到紧凑文本。它与编辑器格式化 JSON 的可读性效果相似。

原源码使用 `new JsonSerializerOptions { WriteIndented = true }`，是在创建对象后用初始化器设置同一属性。先看上面的分步写法，再看此简写更容易对应动作。

json 还没有自动变成磁盘文件。后面的 `await File.WriteAllTextAsync(path, json)` 才向指定路径写文件；WriteIndented 不负责保存，加载也不要求开启相同选项。当前真实代码见 [TASK-006](../../Base/Exercises/TASK-006-json-config/Task006Exercise.cs)。

## 历史证据与掌握范围

Q1 保持已掌握。2026-10-05 学员自主将 WriteIndented=true 的选项传给 Serialize 并完成写入，save 入口退出 0，实际合法 JSON 含设备 002、200 ms 和换行缩进。此前无法编译/尚未验证的描述是修复前历史，完整旧稿保留。本次没有重新执行保存。

相关任务卡与验收来源：[TASK-006](../tasks/TASK-006.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-json.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft Learn：序列化及格式化 JSON](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to)
