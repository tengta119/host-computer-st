# C# using 与资源释放

## Q1：using 关键字有什么用？

- 状态：待复习
- 日期：2026-10-02
- 关联：[TASK-005](../tasks/TASK-005.md)；[CancellationToken 用法](./csharp-cancellation.md)
- 现象：学员看到教学示例 `using var cts = new CancellationTokenSource()` 后询问 using 的作用。
- 原因与机制：同一个关键字有不同用途。文件顶部的 `using Base.Exercises.Task001;` 引入命名空间中的类型，允许使用短类型名 `Reading`，类似 Java import。方法中的 `using var cts = ...;` 是资源管理声明，在离开声明所在作用域时自动调用 Dispose；using 块则在离开该块时释放。正常结束、return、异常退出均会触发清理，机制相当于 try/finally。通常用于实现 IDisposable 的对象，CancellationTokenSource 实现了此接口。
- 教学示例：

```csharp
static async Task DemoAsync()
{
    using var cts = new CancellationTokenSource();
    await Task.Delay(1000, cts.Token);
    // await 暂停期间仍在作用域中，不会因此释放 cts。
} // 离开作用域时调用 cts.Dispose()
```

- 类比：资源管理 using 类似 Java try-with-resources；Java 调用 close，C# 此处调用 Dispose。命名空间 using 才类似 import。
- 易错点：Dispose 是清理资源，不等于 Cancel；using 不负责发出取消请求，也不表示直接销毁对象或替代 GC 回收托管内存。`using var` 中的 var 只负责类型推断，不是 using 生效的条件，显式写 CancellationTokenSource 类型同样可以。
- 复查点：在自主完成 TASK-005 时正确管理取消源生命周期，并能区分引入命名空间、释放资源和发出取消信号；本轮未运行示例。
- 依据：[Microsoft Learn：using 声明/语句](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/using)、[using 指令](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive)、[CancellationTokenSource.Dispose](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.dispose)。
- 2026-10-02 实践复查：学员在 RunScenarioAsync 中自主使用 `using var cts` 管理取消源，两种场景运行完成；说明 Dispose 与 Cancel 及作用域的理解尚无提交证据，问答保持待复习。
