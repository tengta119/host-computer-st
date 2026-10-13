# C# using 与资源释放

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：using 关键字有什么用？

- 状态：待复习
- 日期：2026-10-02
- 关联：[TASK-005](../tasks/TASK-005.md)；[CancellationToken 用法](./csharp-cancellation.md)

你问的是 TASK-005 方法中的 `using var cts = new CancellationTokenSource();`：本次读取结束后，取消源不再使用，应释放它占用的资源。它与文件顶部用于找类型的 using 作用不同。

先看完整类型、明确代码块的写法，这是讲解片段，放在 async 方法中：

```csharp
using (CancellationTokenSource cts = new CancellationTokenSource())
{
    await Task.Delay(1000, cts.Token);
}
```

CancellationTokenSource 是 .NET 提供的取消源类；new 创建对象，cts 保存它。离开 using 的花括号时，C# 自动调用这个对象的 Dispose 方法清理资源。正常结束、return 或异常离开都会执行这项清理。对象实现的 IDisposable 接口提供 Dispose 约定。

`await` 暂停这个方法时，代码还没离开 using 块，cts 仍需供等待使用，不会因为暂停就提前释放。应让这个块覆盖实际读取的结束，包括调用方 await。

原代码简写为：

```csharp
using var cts = new CancellationTokenSource();
```

var 让编译器推断 cts 是 CancellationTokenSource。using 声明没有自己的花括号，清理发生在离开它所在的代码作用域时。它可类比 Java try-with-resources；此处调用 Dispose，Java 通常调用 close。

另一个场景是文件顶部 `using Base.Exercises.Task001;`：它只让本文件用短类名 Reading，类似 import，不创建对象，也不释放任何资源。

cts.Dispose 是清理取消源资源，cts.Cancel 是发取消请求。using 不会替你按停止按钮，也不等同于 GC 回收托管内存。按调用和作用域区分这两个动作即可。

## 历史证据与掌握范围

Q1 保持待复习。2026-10-02 学员已在 RunScenarioAsync 使用 using var，并有正常/取消运行证据；Dispose 与 Cancel 以及作用域机制仍未有新的自主应用证据。本次示例仅讲解，未运行。

相关任务卡与验收来源：[TASK-005](../tasks/TASK-005.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-resources.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft Learn：using 声明/语句](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/using)
- [using 指令](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-directive)
- [CancellationTokenSource.Dispose](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource.dispose)
