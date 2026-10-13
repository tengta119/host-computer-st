# C# JSON 与配置文件

## Q1：JsonSerializerOptions.WriteIndented 有什么用，如何使用？

- 状态：已掌握
- 日期：2026-10-05
- 关联：[TASK-006](../tasks/TASK-006.md) 的 TODO 2。
- 现象：学员知道 JsonSerializer.Serialize 用于序列化，询问 WriteIndented 的作用和调用方式。
- 原因：默认生成紧凑的 JSON 文本，便于程序处理；手动查看和修改设备配置时，换行和缩进更便于阅读。
- 机制：JsonSerializerOptions 是序列化选项对象；将 WriteIndented 设置为 true，并把 options 作为 Serialize 的第二个参数传入，输出便带有换行与缩进。默认 false；此选项改变文本排版，不改变配置字段和值。创建 options 后若未传入本次调用，不会影响该调用的结果。
- 教学片段（config 表示待序列化对象；本轮未运行片段，也未修改学员源码）：

```csharp
var options = new JsonSerializerOptions
{
    WriteIndented = true
};
string json = JsonSerializer.Serialize(config, options);
Console.WriteLine(json);
```

- 假设 config 包含 DeviceId = "001"、ReadDelayMs = 1000，默认输出类似 `{"DeviceId":"001","ReadDelayMs":1000}`；开启选项后得到：

```json
{
  "DeviceId": "001",
  "ReadDelayMs": 1000
}
```

- 类比：类似编辑器格式化 JSON，为人类阅读整理排版；生成的 json 仍然是字符串。C# 的 `new JsonSerializerOptions { WriteIndented = true }` 是对象初始化器，相当于先创建对象再给属性赋值。
- 总结与易错点：Serialize 负责生成 JSON 文本，WriteIndented 控制该文本的排版；保存到磁盘还需要文件写入操作。它不会自动保存文件，也不要求加载时开启相同选项。
- 复查点：在自主完成 TODO 2 时将选项传给 Serialize，观察输出与保存文件的排版；尚无自主应用或验收证据，保持待复习。
- 依据：[Microsoft Learn：序列化及格式化 JSON](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to)。
- 2026-10-05 实践观察：学员已在 TASK-006 中创建 WriteIndented = true 的选项并传给 Serialize；但当前项目因 SaveAsync 的 async/return 错误无法编译，尚无实际文件及排版输出证据，保持待复习。相关答疑合并到 [Task/async 的 Q6](./csharp-task-async.md)，不重复建条目。
- 2026-10-05 审查应用证据：学员自主完成序列化选项与文件写入，实际执行编译后 Base.dll 的 save 入口退出码 0；生成的合法 JSON 包含设备 002、ReadDelayMs = 200，带换行缩进。自主应用已验证，Q1 标为已掌握；任务整体仍因等待与错误处理未通过而进行中。
