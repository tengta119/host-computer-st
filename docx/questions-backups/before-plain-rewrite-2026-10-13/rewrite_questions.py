from pathlib import Path
import hashlib
import json
import re

ROOT = Path(__file__).resolve().parents[3]
BACKUP = Path(__file__).resolve().parent
QUESTIONS = ROOT / 'docx/questions'
BODIES = {}
EVIDENCE = {}

def add(name, body, evidence):
    BODIES[name + '.md'] = body.strip().split('\n===\n')
    EVIDENCE[name + '.md'] = evidence.strip()

add('csharp-properties', r'''
一条设备读数创建时，需要记住“设备 001、数值 35.6、单位 ℃”。以后查看这条旧读数，不能因为当前设备变了就把它改掉。[当前 Reading 定义](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)是本项目自定义的 C# 类，不是 .NET 自带设备类。

```csharp
Reading reading = new Reading
{
    DeviceId = "001",
    Value = 35.6,
    Unit = "℃"
};
double temperature = reading.Value;
```

`new Reading` 创建一条读数；花括号在创建时给这个对象的三个属性赋值，这种写法叫对象初始化器。`reading` 保存该对象的引用。最后一行从这条读数的 Value 取出 35.6。

类中的 `public double Value { get; init; }` 使用 C# 属性语法。`get` 允许上面最后一行读取 Value；`init` 允许在创建对象的初始化阶段设置它。创建结束后再写 `reading.Value = 40;` 会报编译错误。普通 `set` 则允许以后继续赋值。这里没有把字段重新赋值的实现写出来，编译器为这种自动属性生成存储。

`public string Unit { get; init; } = "";` 中的 `= ""` 给未填写的 Unit 一个空字符串；未赋值的 double Value 默认是 0。限制的是这些属性的后续赋值；若属性保存一个可修改的集合，init 不会连集合内部一起冻结。

可联系 Java 中构造时设值、以后只暴露 getter 的读数对象理解。这里主要看“创建时能写，创建后能读”，不需要为了复习把已完成模型重新写一遍。
===
先区分历史提问与当前文件：当时问题中的 Device 在另一个文件、尚未写 namespace；现在 Device 与 Reading 都定义在 [Task001Exercise.cs](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)，属于 `Base.Exercises.Task001`。旧的独立 Device.cs 链接已失效，不能再把它当成当前源码位置。

```csharp
using Base.Exercises.Task001;

Device device = new Device { Id = "001", Name = "温度传感器" };
```

这是跨命名空间使用 Device 的说明片段；`using` 放在文件开头，创建对象那行放在一个方法中。Device 是项目自定义类；`using Base.Exercises.Task001;` 让本文件可用短名字 Device。`new Device` 创建对象，初始化器设置 Id、Name，变量 device 保存它的引用。

另一个文件能使用这个类，是因为它属于同一项目的编译内容、访问权限允许，且代码能找到对应命名空间。C# 不靠“文件在同一个文件夹”建立类型引用；也不是运行时先打开 Device.cs 再读取类。SDK 项目默认把项目目录内的 .cs 源码纳入编译，另有排除配置时按配置处理。

如果两个类已经在同一命名空间，通常不用这行 using。若类没有写 namespace，它处在全局命名空间，当时可以直接用 Device。C# 允许在同一 .cs 文件放多个类；Java 的包/import 可帮助理解名字查找，但文件与类的规则不同。
===
先看实际赋值：按钮执行 `_state.ResultText = "读取完成";` 后，数据对象应保存这段文字；之后绑定读取 `_state.ResultText`，得到刚保存的内容。ReadingDisplayState 是本项目自定义的数据类，ResultText 是它的公开属性名。

下面是简化后的完整写法，只解释保存与读取；原类还保留变化通知：

```csharp
private string _resultText = "尚未读取";

public string ResultText
{
    get
    {
        return _resultText;
    }
    set
    {
        _resultText = value;
    }
}
```

`_resultText` 是真正保存字符串的字段；ResultText 是外部读写它的入口。读取 `string text = _state.ResultText;` 时执行 get，return 把字段当前值交给 text。

赋值 `_state.ResultText = "读取完成";` 时执行 set。C# 自动把等号右侧的“读取完成”交给 `value`，这里 value 的类型是 string。`_resultText = value` 才是实际保存动作；若写成空的 `set { }`，这次赋值不会自动保存到字段。

理解完整写法后，再看 `get => _resultText;`：它是 `get { return _resultText; }` 的简写。这个箭头在此简写属性读取，不是在执行 Lambda 回调。

可以类比普通 Java getter/setter：`GetResultText()` 返回字段，`SetResultText(string newText)` 保存传入文字。C# 属性把调用写成读取或赋值的形式，不要求把当前项目属性改成普通方法。后续 setter 中的 OnPropertyChanged 用于通知界面，先分清保存值与发通知两件事。
''', '''三个问答均保持待复习。TASK-001 模型创建/填充有实践证据，但按 ID 查询由教练实现；TASK-010 的通知调用与数据属性更新已验证，完整 get/set、value、new()/readonly 等来自骨架，不能随任务通关全部升级。2026-10-09“未学过完整属性语法”的反馈是教学起点，不记为学员错误。本轮只重写文档和核对源码。''')

add('csharp-dictionary', r'''
在 [TASK-001 查询代码](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)中，设备 003 存在但没有读数，999 连设备都不存在。要给这两种情况不同提示，所以分别用两本表：dictDevice 按 ID 保存设备对象，dictReading 按 ID 保存读数对象。

```csharp
Dictionary<string, Device> dictDevice = new Dictionary<string, Device>();
dictDevice.Add("001", device001);

Device device;
bool found = dictDevice.TryGetValue("001", out device);
```

Dictionary 是 .NET 的字典类；`<string, Device>` 表示用字符串作键、保存 Device 对象作值。Device 和 device001 来自当前练习，device001 指向已创建的设备。Add 把键 "001" 和这个对象关联起来。键必须唯一；重复 Add 会报错。

TryGetValue 是 .NET 的查询方法。它返回是否找到的 bool，同时把找到的对象交给 out 后的 device；找到时 found 为 true。查找失败则返回 false，Device 是引用类型，out 得到默认的 null，所以失败路径不能继续访问 device.Id。

`dictDevice["001"]` 可取已有对象，但读缺失的键会抛异常。`dictDevice["001"] = device001` 则可新增或替换该键对应的值。先用 TryGetValue 查询设备，设备存在后再查 dictReading，才能区分“无设备”和“设备无读数”。

这类似 Java Map 的按键关联；C# 的 TryGetValue 一次查询同时报告是否找到并取得值。遍历字典时每个 pair 的 Key 是 ID、Value 是对象；若一台设备要保留多条历史，可另设计保存读数列表的字典，本题不追加该实现。
===
当前 foreach 中，查询失败后会执行 continue，直接开始下一个 ID，不会走到下方输出 device.Id 的代码。这是“为什么后面能安全使用”的关键，除了变量可见，还必须确认查找成功。

先把声明分开看，下面等价于当前查询的主要流程：

```csharp
Device device;
if (!dictDevice.TryGetValue(deviceId, out device))
{
    Console.WriteLine("设备不存在");
    continue;
}
Console.WriteLine(device.Id);
```

片段位于 foreach 循环体内；deviceId 是本轮待查 ID。`out device` 让 TryGetValue 在返回前给 device 赋值。`!` 把“找到了”反过来，因此找不到才进入 if。continue 结束当前这一轮，能到最后一行就意味着已找到设备。

原代码的 `out var device` 将声明合并到查询位置。var 是 C# 的类型推断，编译器从字典的值类型推断这里是 Device，并非任意动态类型。这种 if 条件中的 out 声明在当前循环体后面的语句仍可见；若在 if 花括号内部另外声明局部变量，范围则不同。

去掉 continue 后，失败路径也可能来到输出行，此时 Device 的默认值为 null，读取其属性会失败。变量“可以访问”不等于“已经查询成功”。第二次查询 reading 使用同样的判断与提前跳过。
''', '''Q1/Q2 均保持待复习。TASK-001 的字典创建与填充是学员作品，查询、缺失设备和无读数的处理由教练提供；已有运行覆盖不能作为学员自主查询能力证据。本次没有修改或执行练习。''')

add('csharp-resources', r'''
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
''', '''Q1 保持待复习。2026-10-02 学员已在 RunScenarioAsync 使用 using var，并有正常/取消运行证据；Dispose 与 Cancel 以及作用域机制仍未有新的自主应用证据。本次示例仅讲解，未运行。''')

add('csharp-json', r'''
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
''', '''Q1 保持已掌握。2026-10-05 学员自主将 WriteIndented=true 的选项传给 Serialize 并完成写入，save 入口退出 0，实际合法 JSON 含设备 002、200 ms 和换行缩进。此前无法编译/尚未验证的描述是修复前历史，完整旧稿保留。本次没有重新执行保存。''')

add('csharp-lambda-linq', r'''
TASK-003 的四条样例中，目标是只留下“设备 001 且数值大于 30”的读数。001 的 10 不满足，001 的 70 满足；002 的两条即使大于 30，也不属于目标设备。

先把判断写成普通方法，下面是说明用的自定义方法，不要求替换已有作品：

```csharp
static bool IsTargetReading(Reading reading)
{
    return reading.DeviceId == "001" && reading.Value > 30;
}
```

Reading 是已有读数类；参数 reading 接到一条具体读数。`==` 判断设备 ID，`>` 不包含等于 30；`&&` 要求两个判断都为 true。方法把一个 bool 结果交回调用者。

再看同一判断的 Lambda 写法：

```csharp
Func<Reading, bool> predicate = reading =>
    reading.DeviceId == "001" && reading.Value > 30;
```

Func 是 .NET 提供的委托类型；这里规定接收 Reading、返回 bool。predicate 是保存判断方法的变量。箭头左边 reading 是参数名，右边是计算返回值的表达式；此时保存判断，之后调用 `predicate(oneReading)` 才判断那条读数。可联系 Java 的 `reading -> ...` 理解。

`readings.Where(predicate)` 使用 .NET LINQ 的 Where 方法，让每条读数接受这个判断，保留结果为 true 的项。它通常在 foreach 遍历时执行，而不是写出 Where 时就筛完整个列表；ToList 则会把查询结果执行并收集成列表。本题当前源码用 foreach 展示，见 [查询代码](../../Base/Exercises/TASK-003-reading-query/Task003Exercise.cs)。

`reading => false` 是永不保留任何读数的占位。既要数值满足，也要设备满足，不能只替换其中一个判断。
===
同一批数据既要列出符合条件的读数，又要算数量，两处必须用相同判断。“传给 CountMatching”是把判断方法交给它，不是先把某一条读数判断后的 true/false 交进去。

```csharp
Func<Reading, bool> predicate = reading =>
    reading.DeviceId == targetDeviceId && reading.Value > threshold;
int count = CountMatching(readings, predicate);
```

targetDeviceId、threshold、readings 均是练习中已有的变量。predicate 保存接收一条 Reading、返回 bool 的判断。CountMatching 是本项目自定义方法，第二个参数用同样的 Func<Reading, bool> 接收它。

该方法每次循环的核心代码是：

```csharp
if (predicate(reading))
{
    count++;
}
```

这里 reading 是本轮那条读数；`predicate(reading)` 调用保存的判断，得到 true 才把 count 加 1。传入方法时还没有逐条判断，遍历中带括号调用时才执行。这就是此处的委托调用。

可联系 Java 的 Predicate 参数和 `predicate.test(reading)` 理解。它与发布事件不同：此处一个普通方法明确调用传入的判断，返回值用来决定计数。已有样例最终应该只有设备 001 的 70，数量为 1；这是样例预期和历史验证结果，本轮没有重新运行。
''', '''Q1 保持待复习，Q2 保持已掌握。2026-09-27 TASK-003 学员自主实现双条件 Where、传递相同 Lambda 和逐条 predicate 调用，运行匹配数量 1。已有应用范围保留；延迟执行等机制没有因此全部掌握。''')

add('dotnet-projects', r'''
当时 TASK-001～008 都在 Base 控制台项目中，TASK-009 要实际打开窗口、显示输入框与按钮，所以需要一个配置为 WPF 的项目。这个判断已经落实：现在工作区使用的是 [WpfBae.csproj](../../WpfBae/WpfBae.csproj)，无需为每个后续 WPF 任务另建项目。

```xml
<OutputType>WinExe</OutputType>
<TargetFramework>net10.0-windows</TargetFramework>
<UseWPF>true</UseWPF>
```

这是当前 WpfBae.csproj 中的配置。OutputType 选择 Windows 程序形式，TargetFramework 选择 .NET 10 的 Windows 目标，UseWPF 启用 WPF 构建支持。csproj 是项目的构建配置文件，不是窗口界面。

[WpfBae.sln](../../WpfBae/WpfBae.sln) 是解决方案文件，用于组织项目，便于 Rider 打开和管理。一个解决方案可容纳多个项目；新增项目与新增解决方案是两个选择，不必绑在一起。

当时建议的 WpfPractice 是拟用名称；你最终创建的真实目录是 WpfBae。复习应以现在的文件名为准，不去创建第二套 WpfPractice，也不移动已完成作品。可以把项目类比为有独立构建配置的 Java 模块，把解决方案看成管理这些模块的集合，具体规则仍以 .NET 为准。
''', '''Q1 保持待复习。2026-10-07 的新建界面与建议保存在旧稿，TASK-009 后续已完成。此处更新当前路径和阅读方式，未新建项目、切换入口或运行程序。''')

add('learning-route', r'''
你当时问“TASK-008 做完，是否可以进入界面开发”。根据已验证的设备模型、事件、集合查询、异步读取、取消、JSON 和接口练习，已经可以开始 WPF；不需要先学完 C# 的所有语法再写一个窗口。

这个选择来自 [计划原文](../plan.md) 的顺序：第一阶段 C# 基础，第二阶段 WPF。已有 Java 后端背景可以帮助迁移部分经验，但不能据此默认你会 XAML、按钮、布局或桌面线程问题。

例如进入 WPF 后，真实窗口需要“按钮点下去做什么”和“读到的数据怎样显示”。于是属性、事件、异步继续在这些具体问题里补充，而不是为每个待复习名词另设考试。TASK-009 是当时的下一步；现在 TASK-009～019 已通关，TASK-020 进行中，进度以 [learning.md](../learning.md) 为准。

任务级通关确认的是该次要求和证据；完整属性、委托机制、线程和资源释放仍有复习内容，所以不能宣布 C# 或 WPF 阶段整体已掌握。后续通信、采集和设备控制按原计划与真实实践逐步推进，不在本次整理中另建任务。
''', '''Q1 保持待复习。2026-10-05 的答疑允许从集中 C# 入门进入 WPF，没有把第一阶段整体标通关。本次仅更新解释和当前进度指引，所有任务状态保持原记录。''')

add('csharp-task-async', r'''
TASK-004 先输出“开始读取”，过一段时间再得到一条 Reading。Task 用来让调用方跟踪这次操作是否结束；它是 .NET 提供的类，ReadAsync 是练习自定义的方法，pendingReading 是保存本次返回任务的变量。

```csharp
Task<Reading> pendingReading = ReadAsync(deviceId);
Reading reading = await pendingReading;
```

第一行调用读取方法，拿到表示本次读取完成状态的 Task<Reading>。尖括号里的 Reading 指明成功完成后会交出一条读数。第二行用 C# 的 await 等本次任务完成，得到 Reading，保存到 reading。Task 与最后取得的读数是两件东西。

读取方法里的 `await Task.Delay(8000)` 使用 .NET 的计时方法，约 8000 毫秒后才继续后面的代码。这是模拟等待，不是真读设备。若 Delay 的任务还未完成，await 暂停当前方法的后续部分，等待期间不靠一条线程一直阻塞着。

只写 `Task.Delay(8000)` 而丢弃返回任务，读取方法就可以继续 return，并不会自动等八秒；外层 await ReadAsync 也不会替内部补上丢掉的等待。Task.CompletedTask 已经完成，不能模拟耗时；Thread.Sleep 会阻塞当前线程。Task 可类比 Java CompletableFuture 的完成结果载体，不等于新建线程。
===
先看“返回一条读数”和“向调用方返回任务”分别发生在什么位置。下面按当前方法形式简化，温度和单位来自已有 Reading：

```csharp
static async Task<Reading> ReadAsync(string deviceId)
{
    await Task.Delay(8000);
    return new Reading { DeviceId = deviceId, Value = 100, Unit = "℃" };
}
```

`async Task<Reading>` 规定调用方拿到 Task<Reading>，而方法内部成功结束时用 return 提供 Reading。编译器为 async 方法生成完成任务所需代码，把这条 Reading 作为该任务的成功结果。调用方先得到任务，再通过 await 取得它的 Reading。

它不是把 Reading 当成 Task 强制转换，也不要求你在这里再手写一层 Task。去掉 async 后，这种直接 return Reading 就不符合普通 Task<Reading> 方法的返回类型了；普通方法可用 `return Task.FromResult(reading);` 明确返回已完成任务，FromResult 是 .NET 方法。

Java 普通方法常手写返回 CompletableFuture；此处 C# async 帮助管理返回任务及完成结果。复习时先对应三个具体名字：ReadAsync 的返回类型是 Task<Reading>，pendingReading 保存任务，`await pendingReading` 的结果是 Reading。
===
骨架的 `await Task.CompletedTask;` 不会模拟读取耗时：CompletedTask 是 .NET Task 类的静态属性，取得一个已经成功完成、没有读数结果的任务。

```csharp
await Task.CompletedTask;
Console.WriteLine("继续执行");
```

这段放在 async 方法中，await 发现任务已完成，就立即执行后面的输出，不会等待一秒，也不会因此必定切换线程。

不用 await 时，可以把任务保存到变量；普通返回 Task 的方法也可以写 `return Task.CompletedTask;`。但单独一行 `Task.CompletedTask;` 只是读取属性，不是 C# 允许的独立表达式语句，会报编译错误。

该属性适合作为“无需等待、已成功完成”的任务返回值。TASK-004 原先用它保留可构建骨架，后来已经改为 Delay；复习不需要把当前代码改回 CompletedTask。
===
这两处 await 等待的对象不同，也暂停不同的方法。以下是 TASK-004 的流程简图式代码，省去已有日志：

```csharp
Task<Reading> pendingReading = ReadAsync(deviceId);
Console.WriteLine("读取方法已调用，准备等待结果");
Reading reading = await pendingReading;
Console.WriteLine(reading.Value);
```

1. 第一行一调用 ReadAsync，它就开始执行。内部到达尚未完成的 `await Task.Delay(8000)`，读取方法的后续部分暂停，向外返回尚未完成的 Task<Reading>。
2. RunAsync 拿到这个任务，继续打印“准备等待结果”。到第三行 await pendingReading 时，若读取还没完成，RunAsync 的后续部分也暂停。
3. 计时结束，ReadAsync 从内部 await 后继续，构造并 return Reading；pendingReading 因此成功完成。
4. RunAsync 的 await 取得这条 Reading，再输出数值。入口等待的 RunAsync 最后完成。

计时从调用 Task.Delay 开始，不是从外面的 await pendingReading 开始，外层 await 不会再加一轮八秒。当前文件等待参数为 8000；旧记录里的 1000 是当时教学样例。

暂停不是把整台电脑暂停，也不是等待到外层 await 才启动方法。若等待对象已完成，await 不必暂停。历史输出中的 `System.Threading.Thread` 只是线程对象的默认文字，不能当作线程 ID。
===
方法签名里的 Task 表示“这个方法这次调用整体什么时候完成”。它不是自动指向方法内部某一行 Delay。

| 调用 | 返回的任务表示什么 | 成功后取得什么 |
| :--- | :--- | :--- |
| Task.Delay(8000) | 这次计时结束 | 无额外结果值 |
| ReadAsync(deviceId) | 这次模拟读取结束 | 一条 Reading |
| RunAsync() | 读取、取得结果、输出这一整段流程结束 | 无额外结果值 |

Task、Task<Reading> 是 .NET 类型；Delay 是 .NET 方法，ReadAsync/RunAsync 是本项目自定义方法。pendingReading 是变量名，保存 ReadAsync 返回的那一个任务。成功/失败/取消属于任务的完成结果，调用方可在 await 时观察它。

“每次调用拿到表示本次完成的任务”不保证每次都分配不同对象；某些已完成任务可以复用。这里理解它代表哪次操作即可，不把类型名 Task 理解成线程名称。
===
RunAsync 要等读取结果再输出，但不把一条 Reading 再返回给入口。它因此使用 `async Task`：async 允许方法内部写 await，Task 让入口能等待整段流程结束。

```csharp
public static async Task RunAsync()
{
    Reading reading = await ReadAsync("001");
    Console.WriteLine(reading.Value);
}
```

该说明片段位于原任务类中。`await RunAsync()` 等到这里的读取与输出完成，不取得额外读数值。普通 `async void` 方法无法供入口这样 await，所以不能仅因为“不返回读数”就改成 void。

### 追问：await File.WriteAllTextAsync(path, configJson) 为什么不能这样写？

这句调用本身合法：File 是 .NET 文件类，两个 string 参数分别是路径和待写文字。问题出在包含它的 SaveAsync 当时没有 async，以及末尾 return 使用了不存在的变量。

```csharp
static async Task SaveAsync(string path, string configJson)
{
    await File.WriteAllTextAsync(path, configJson);
}
```

这是用两个字符串简化的说明片段；原 SaveAsync 接收 DeviceConfig 并先序列化。写入任务成功后走到方法末尾，整个 SaveAsync 完成，不需要 `return task;`，也不应改成 Task<Task>。失败会通过任务在调用方 await 处被观察到。

### 追问：普通 Task 方法可以直接 return 写入任务吗？

可以。若里面不再使用 await，可写普通方法 `static Task SaveTextAsync(string path, string text) { return File.WriteAllTextAsync(path, text); }`。SaveTextAsync 是这里说明用的名字，它把 .NET 写入 API 的任务直接交给调用者。Async 后缀和 Task 返回类型不强制要求 async；使用 await 的普通方法则需相应 async 声明。

### 追问：async Task SaveAsync 末尾没有 return，为什么仍有返回任务？

返回任务由编译器生成的异步方法代码管理。调用者仍可写 `Task saving = SaveAsync(...); await saving;`；这里省略号仅表示原方法所需参数，不是可复制代码。方法走到末尾，表示成功完成，没有额外结果值。

普通 Task 方法由你显式 return Task；async Task 可自然结束或裸 `return;`；async Task<Reading> 成功路径要提供 Reading。这三个规则分别对应“交出任务”“完成但无读数值”“完成并提供读数值”。不要把“没有手写 return”解释为“没有返回给调用方的 Task”。
===
Task-004 的等待前后日志在历史运行中出现不同线程 ID，但并没有两段读取代码同时计算。先区分“方法暂停，之后继续”与“几条线程同时执行”。

`async`、`await` 本身不创建一个专属新线程。Task.Delay 依靠计时完成，等待八秒期间没有一条线程一直执行 ReadAsync 的空等待。Task 记录操作结果，线程执行某段代码，它们不是同一对象。

当前控制台没有 WPF 的 UI 调度环境，尚未完成的 await 后通常在线程池线程继续；历史实测 ID 从 2 变 5，只证明那次前后由不同线程先后执行，不能证明并行。在通常从 WPF UI 事件中开始的异步方法里，默认捕获 UI 上下文并安排回 UI 线程；已经完成的 await 也可能直接继续。不能保证每次 await 都换线程。

要看身份，用 .NET 的 `Environment.CurrentManagedThreadId` 输出整数；`Thread.CurrentThread` 的默认文字只会类似 System.Threading.Thread。Task.Run 可安排计算在线程池执行，是另一种用途；本题 Delay 没有因此变成并行计算。本次不新增线程实验或改已有日志。
''', '''Q1 保持已掌握，Q2～Q7 保持待复习。TASK-004 自主等待/返回读数有证据，结果输出与单位修正有教练协助；2026-09-29 线程 ID 2→5 为历史实测。2026-10-05 TASK-006 曾缺 await，3500 ms 配置整个进程仅 62 ms；自主修复后实测 3565 ms，50 ms 对照 120 ms，200 ms 为 265 ms，CS4014 消失。SaveAsync 当时有 CS4032/CS0103，普通 Task 转交写法曾编译通过，现在源码为 async Task 写入。以上是旧记录/源码核对，不是本轮重跑。''')

add('csharp-cancellation', r'''
TASK-005 开始一轮两秒模拟读取，调用方可能希望提前停下。真正发停止请求的是 `cts.Cancel()`；cancelEarly 只是“这轮要不要取消”的 bool 条件，不会自行发信号。

```csharp
CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken cancellationToken = cts.Token;
```

CancellationTokenSource 是 .NET 的取消源类，new 创建取消源，cts 保存它；CancellationToken 是 .NET 的令牌类型，cancellationToken 是本例变量名。Token 属性给出与这个取消源关联的令牌，把它传给读取方法，再传给内部等待。

```csharp
await Task.Delay(2000, cancellationToken);
```

这是 ReadAsync 内部真正接收令牌的等待。取消源请求取消后，该等待能观察请求、以取消结束；await 表现为取消异常，当前方法后面的 return Reading 不再执行。只在方法签名写 CancellationToken 却没传给等待或检查它，不会自动停止读取。

可以把 cts 看成停止开关，令牌是让操作观察这个开关状态的方式。CancellationToken.None 不会变成可取消令牌；它是不可取消的令牌。

### 追问：CancelAfter 做了什么，与 await/using 有什么关系？

`cts.CancelAfter(500)` 安排约 500 毫秒后请求取消，计时从调用开始；调用立即返回，本身不需要 await。cts.Cancel 则现在请求。源尚未取消时再次 CancelAfter 可重设计时；具体时刻受调度影响。

调用方之后仍要 await 采集任务，才能观察成功或取消，并让 using 的范围覆盖操作结束。Dispose 清理资源，不能代替 Cancel；两者分别解释见 [using 问答](./csharp-resources.md)。已完成的某条读数不会被后来取消撤回，取消也不会强杀线程或自动通知真实设备停机。
===
先把“创建、传入、请求、观察结果”串成一个最小例子。下面是 async 方法内的说明片段，不是要求重写已经通关的 TASK-005：

```csharp
using (CancellationTokenSource cts = new CancellationTokenSource())
{
    CancellationToken token = cts.Token;
    cts.CancelAfter(500);
    try
    {
        await Task.Delay(2000, token);
        Console.WriteLine("等待完成");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("等待被取消");
    }
}
```

cts 是取消源；token 接到它的令牌；CancelAfter 安排停止请求；Delay 的第二个参数让等待观察这个请求。通常等待尚未到两秒就收到取消，await 进入 catch，跳过“等待完成”。OperationCanceledException 是 .NET 的取消异常类，用于区分预期取消。

using 块在这段操作完成后调用取消源 Dispose，不在 await 暂停时释放。删除 CancelAfter 后，这个例子通常等待约两秒输出完成；两种时间都不是精确计时保证。

原 TASK-005 把 token 经 ReadAsync 传到 Delay，cancelEarly 为 true 时调用 Cancel；原 TASK-007 还经采集循环传递同一取消状态。终端场景调用方专门处理预期取消后可安全结束；若 catch 提示后又 throw，异常仍会离开入口，不能把“出现取消提示”当成正常退出。
===
就当前 TASK-005，调用方的取消 catch 是 `await pendingReading` 观察到读取任务取消后进入的。`cts.Cancel()` 发请求，不会把程序指针直接跳到下方 catch。

```csharp
Task<Reading> pendingReading = ReadAsync(deviceId, cts.Token);
cts.Cancel();
Reading reading = await pendingReading;
```

这三行是 try 块中的流程说明。第一行已启动 ReadAsync，它执行到内部尚未完成的 `await Task.Delay(2000, token)` 后暂停。第二行请求取消，内部等待无需等外面的第三行才响应。

内部 Delay 取消后，ReadAsync 的 await 观察到取消异常；当前 ReadAsync 没捕获它，因此不 return Reading，返回的读取任务也以取消完成。外层 await 再观察到取消，跳过成功输出，交给调用方的 OperationCanceledException 分支。

若第三行到达时任务已取消，会直接进入异常路径；若响应/传播尚未完成，会先暂停，再在任务完成取消后继续到 catch。不能保证 Cancel 返回的一瞬间 pendingReading 已取消，也不能把发请求时刻等同于 catch 执行时刻。

await pendingReading 不负责启动读取，也不发请求，它取得完成结果时把取消表现为异常。这个结论限定当前练习，不意味着 Cancel API 在所有其他回调情形下绝不会抛异常。
''', '''Q1/Q2 保持已掌握，Q3 保持待复习。2026-10-02 TASK-005 自主令牌传递、Cancel 和专门 catch 已运行，取消源角色说明曾由教练修正；2026-10-05 任务按用户要求通关，未据此升级 Q1。随后 TASK-007 学员自主实现取消条件/源/令牌/响应/调用方处理，1000 ms 两次、10 ms 六次、1 ms 六次共 14 次取消安全结束，Q1 才升级。请求前一条或两条已完成读数允许保留；不保证取消第一轮，不扩大为精确调度或真实设备停止。catch 重抛及调度过晚的修复前历史完整存于旧稿，本轮未运行。''')

add('csharp-events', r'''
在 TASK-002，设备执行 ProduceReading 产生一条读数后，监控处理方法要立即收到它。Reading 是读数数据；ReadingReceived 表示“刚产生新读数，请调用已登记的接收方法”。这就是此处的读数事件。

```csharp
SimulatedDevice device = new SimulatedDevice("001");
device.ReadingReceived += OnReadingReceived;
device.ProduceReading(5.0, "℃");
```

SimulatedDevice、ProduceReading、OnReadingReceived 是本项目自定义类/方法。new 创建设备 001，device 保存它。第二行把接收方法登记到这个设备的事件上，尚不调用；第三行让设备产生读数，设备内部发通知，才调用接收方法并传入读数。

它可类比 Java 设备对象登记监听器、数据到来调用回调。当前通知发生在程序内部，不是串口消息或硬件中断。外部代码可登记、取消登记，发通知由设备类内部负责。
===
当前自定义设备类需要公开一个通知入口，让监控侧登记“新读数到来后做什么”。类内这行就是声明：

```csharp
public event EventHandler<ReadingReceivedEventArgs>? ReadingReceived;
```

public/event 是 C# 关键字；ReadingReceived 是练习自定义事件名。EventHandler<T> 是 .NET 提供的委托类型，规定接收方法接收 sender 与一个事件参数、返回 void。这里 T 填入自定义 ReadingReceivedEventArgs，里面保存 Reading。问号表示可能没有接收方法，此时值可为 null。

先看配套接收方法的形状：`void OnReadingReceived(object? sender, ReadingReceivedEventArgs e)`。sender 接收发通知的设备，e 接收这次事件数据，返回 void。只有参数与返回要求符合，才能按此事件类型登记。

声明不会自动发送读数，也不创建事件参数。还要在 ProduceReading 中创建参数并调用当前登记的方法，具体用法见 Q4。事件参数类属于预备骨架，不把它的存在算成学员自主设计。
===
可以说设备发通知、订阅方收到数据，但实际动作是“设备调用已登记的处理方法，把数据作为参数传进去”。事件不是供以后主动取读数的存储区。

```csharp
private static void OnReadingReceived(object? sender, ReadingReceivedEventArgs e)
{
    Reading reading = e.Reading;
    Console.WriteLine(reading.Value);
}
```

这在已有任务类中是接收方法的简化说明。e 是本次接到的自定义参数对象；e.Reading 取得其中那条读数。变量 reading 保存它，再取 Value 输出。sender 是来源设备，读数保存在 e 的 Reading 属性中，两者用途不同。

设备触发时只调用当时已登记的方法。之后才登记的接收方不会自动补收旧通知；若要保留历史，需要另用集合。取消订阅后，后续通知不再调用被移除的处理方法。
===
先看当前任务的三步：登记、设备内部发布、移除同一接收方法。以下片段来自同一场景，放置位置不同，不能直接串成一个方法：

```csharp
device.ReadingReceived += OnReadingReceived;
device.ReadingReceived -= OnReadingReceived;
```

第一行在监控侧登记，第二行在不再监听时移除。它们都操作原 device 的 ReadingReceived，传入没有调用括号的方法名；`+=` 本身不会立即处理读数。

设备类内部发布时，先用已有 reading 创建参数，再以完整写法理解：

```csharp
ReadingReceivedEventArgs args = new ReadingReceivedEventArgs(reading);
EventHandler<ReadingReceivedEventArgs>? handler = ReadingReceived;
if (handler != null)
{
    handler(this, args);
}
```

args 包装这条 Reading；handler 保存当前接收方法，null 表示没有。最后一行调用登记的方法，this 是发通知的设备，args 交给接收方的 e。原源码 `ReadingReceived?.Invoke(this, args)` 是同类调用的空值判断简写，有接收方才调用。它不吞掉接收方法自己抛出的异常。

TASK-002 已用产生两条、移除、再产生第三条的流程验证：接收方法只输出前两条。复习时读 [真实文件](../../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)，无需重置已有 TODO 或再写整份答案。
===
TASK-002 已验证“一个设备、一个处理方法、取消订阅后不再收到”。后续要学的内容，应从实际问题引出，不能把已通关当作所有事件机制都学完。

例如同一温度既要显示又要检查报警，读取代码可以接收调用方传来的处理方法，Q6 就用这个例子讲委托。多个接收方法同时登记时，普通调用会依次同步执行；一个耗时处理会拖慢发通知的方法。长期保留设备对象时，还需在界面不再使用后取消相应订阅，避免对象被订阅关系持续引用。

将来真实设备数据若从后台线程来，WPF 界面更新要安排到界面线程。Dispatcher 是 WPF 用于把操作安排给界面线程的工具，到真实场景再展开。异步事件处理的异常边界同样待后续实践，不在本次整理里突然要求使用 async void。

当时后续顺序是 TASK-003 委托参数与 Lambda，再到 Task/async/await；这些任务现在已完成各自验收。尚未验证的生命周期、线程与持续设备数据问题仍继续复习，课程顺序以计划和最新进度为准。
===
普通方法 `ShowMessage("读取成功")` 会立即输出文字。若另一段代码需要“先保存要执行的处理方法，等读数到来再调用”，可以把方法引用保存到委托变量中。

先声明说明用的普通方法和委托类型，均放在同一个示例类内：

```csharp
void ShowMessage(string text)
{
    Console.WriteLine(text);
}
public delegate void MessageHandler(string text);
```

ShowMessage 与 MessageHandler 是例子自定义名字；Console.WriteLine 是 .NET 输出方法。delegate 是 C# 声明关键字，MessageHandler 规定能保存的方法“接收 string，返回 void”。它不是又声明了一个可直接输出的普通方法。

在同类另一个方法里：

```csharp
MessageHandler handler = new MessageHandler(ShowMessage);
handler("读取成功");
```

第一行创建委托，让 handler 保存 ShowMessage 的引用；没有调用 ShowMessage。第二行才调用保存的方法，把文字交给其 text 参数。变量保存的是可调用方法引用，不是输出结果或方法名字符串。

### 2026-10-11 追问：为什么不直接调用，委托只能指向一个方法有什么用？

若固定只调用一个方法，直接调用就够。委托的用处是让调用方决定读取后的动作，读取方法不写死显示或报警。下面是自定义说明方法：

```csharp
public delegate void ReadingHandler(double temperature);

void ReadTemperature(ReadingHandler afterReading)
{
    double temperature = 26.5;
    afterReading(temperature);
}
```

ReadingHandler 规定接收一个 double、返回 void；ReadTemperature 完成模拟读取后调用参数 afterReading 保存的方法。调用方可选下面两个普通方法，也放在同一个说明类中：

```csharp
void ShowTemperature(double temperature)
{
    Console.WriteLine("当前温度：" + temperature + " °C");
}
void CheckAlarm(double temperature)
{
    if (temperature > 25)
    {
        Console.WriteLine("温度报警");
    }
}
```

ShowTemperature 输出传入读数，CheckAlarm 在超过25时输出报警；两个名字由例子自定，不是框架 API。在同类另一个方法调用 `ReadTemperature(new ReadingHandler(ShowTemperature));` 就读取后显示，换成 CheckAlarm 就读取后检查报警，ReadTemperature 本身不用写死选哪个。

还可以组合，在同类另一个方法中：

```csharp
ReadingHandler readingHandler = new ReadingHandler(ShowTemperature);
readingHandler += new ReadingHandler(CheckAlarm);
readingHandler(26.5);
```

readingHandler 保存两个匹配的方法，调用时在正常完成的情况下先显示26.5、再输出温度报警。+= 登记第二个方法，不立即调用；最后一行才执行。委托并非只能引用一个方法。这与 Java 传入回调、监听器接收通知有相似用途，不用反射按名字查方法。片段仅作说明，本轮未运行。

### 2026-10-11 追问：EventHandler 既是委托又是事件吗？

回到命令类的 `public event EventHandler? CanExecuteChanged;`：EventHandler 是 .NET 提供的委托类型；event 是 C# 关键字；CanExecuteChanged 才是事件名。EventHandler 要求方法接收 object? sender、EventArgs e，返回 void。

类内部的 `EventHandler? handler = CanExecuteChanged;` 使用已有类型声明普通变量，并取得当前登记的方法。handler 是变量，事件是 CanExecuteChanged，它们身份不同。外部对这类事件用 +=/-= 登记或移除；不能自行调用事件、也不能用 = 替换全部订阅关系。

### 2026-10-11 追问：delegate 声明委托，为什么又有 EventHandler？

delegate 定义一个委托类型，例如上面的 MessageHandler。.NET 已经定义 EventHandler；你只是在用它声明事件或变量，不需要再在项目中定义同名类型。可以用“C# 语法定义类型 → 用这个类型创建/保存对象”理解两行的先后关系。

CanExecute 接收一个参数并返回 bool，不符合 EventHandler 的两个参数/void 要求，所以命令事件并不直接保存 CanExecute。

### 2026-10-11 追问：handler(...) 和 handler?.Invoke(...) 有什么区别？

先看当前骨架的完整调用：

```csharp
EventHandler? handler = CanExecuteChanged;
if (handler != null)
{
    handler(this, EventArgs.Empty);
}
```

`handler(...)` 与 `handler.Invoke(...)` 都调用委托保存的方法；Invoke 是委托的调用方法。直接调用时若 handler 为 null，会抛空引用异常。`handler?.Invoke(...)` 的 ?. 是 C# 空值条件访问：没有方法就跳过，有则调用，与这里局部变量的完整 if 写法对应。

EventArgs.Empty 是 .NET 的无额外数据参数。?. 只处理 handler 为 null 的情况，不能吞掉被调用方法自己的异常。当前例子的 this 指命令对象，详情见 [命令问答](./wpf-commands.md)。
''', '''Q1～Q4 保持已掌握，Q5/Q6 保持待复习。2026-09-27 TASK-002 学员自主声明、发布、登记、移除，当前设备 ID 随读数传入，实际前两次完整输出、移除后第三次无接收输出；参数类与入口由骨架提供。Q6 与各追问是 2026-10-11 的委托基础补充，没有独立示例运行证据。教练示例用途不足、术语混用属于教学改进，不撤销已有事件应用证据、不记为学员错题。''')

add('wpf-layout', r'''
先看 TASK-009 中“设备 ID 文字、输入框、读取按钮”这三个具体元素。它们从上到下出现，是因为被放在同一个 StackPanel 内，而不是靠文件里的名字自动找到位置。

```xml
<StackPanel>
    <TextBlock Text="设备 ID" />
    <TextBox Text="TEMP-001" />
    <Button Content="读取温度" />
</StackPanel>
```

XAML 是描述界面的标记语言。这段在窗口内部创建一个 StackPanel，并在其中按顺序放三个控件。它们都由 WPF 提供：TextBlock 显示固定文字，TextBox 是输入框，Button 是按钮。Text/Content 分别给这个文字控件、输入框、按钮指定显示内容。

StackPanel 默认从上往下摆放内部元素；设置 Orientation="Horizontal" 后改成从左往右。它只沿一个方向排，不自动在窗口边缘换行。StackPanel 是负责排列子控件的容器，Window 则是打开的整个窗口。

再看 Grid：它先划分格子，然后放控件，不按书写顺序自动堆起来。

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row="0" Text="设备 ID" />
    <TextBox Grid.Row="1" Text="TEMP-001" />
</Grid>
```

Grid.RowDefinitions 为这个 Grid 定义两行，每个 RowDefinition 描述一行，Auto 按内容需要分配高度。TextBlock 的 Grid.Row=0 放第一行，TextBox 的 1 放第二行，编号从 0 开始。若多个控件都放默认的第 0 行/第 0 列，就可能重叠。Grid 负责摆位置，不是 DataGrid 那种读数表格。

当前 [TASK-009 原文件](../../WpfBae/Exercises/TASK-009-window-events/Task009Window.xaml)使用 StackPanel，无需为了复习改成 Grid。原首步观察是把输入框 Margin 的 `0,8,0,12` 改为 `0,8,0,24`：四个数按左、上、右、下解释，只增加输入框下方外部空隙。Padding 则是按钮文字与按钮边缘的内部留白，作用位置不同。后来改 Title 是另一步，不能一次堆进首个动作。

历史观察应在实际 TASK-009 窗口核对；当前启动入口是 TASK-020，直接运行项目会打开当前任务，设计预览也不能代替原窗口运行证据。这里保留单处观察方法，不要求重做已完成任务。
''', '''Q1 保持待复习。2026-10-09 学员明确尚未理解 StackPanel/Grid，之前直接要求“调整布局”是教练漏讲；TASK-009 的按钮读取与输入保护后来已验证通关，但容器与布局机制不因此全部掌握。''')

add('wpf-binding', r'''
TASK-010 点击读取后，要让结果文字从“尚未读取”变成当前读数。结果先保存在数据对象 ResultText 中，再让结果控件从这里取值。先看最短连接：

```xml
<TextBlock Text="{Binding ResultText, Mode=OneWay}" />
```

TextBlock 是 WPF 文字控件。等号左边 Text 设置这块文字显示什么；右边 Binding 要求从数据来源对象读取 ResultText 属性。ResultText 是本项目自定义属性名，OneWay 表示从数据到文字控件。写成 `Text="ResultText"` 只显示这十个字母，不会读属性。

数据对象先这样创建与连接，下面是采用完整 new 的等价写法：

```csharp
private readonly ReadingDisplayState _state = new ReadingDisplayState();
```

ReadingDisplayState 是教练定义的类，保存结果/状态两段文字。new 创建对象，_state 保存它的引用。窗口构造中 `DataContext = _state;` 设置窗口的 WPF 数据来源属性；内部 TextBlock 没另设来源，就沿用这个对象。绑定首次读取 _state.ResultText，显示“尚未读取”。这条来源关系展开见 Q2。

之后按钮执行 `_state.ResultText = "读取完成";`，必须让绑定知道该属性变化。原 setter 的两个动作是：

```csharp
_resultText = value;
OnPropertyChanged("ResultText");
```

第一行保存此次赋值的文字，value 来自等号右侧；第二行调用项目自定义通知方法，报告 ResultText 变了。ReadingDisplayState 实现 .NET 的 INotifyPropertyChanged 接口，提供 PropertyChanged 事件；声明事件本身不会发通知。WPF 监听到通知后，再读公开 ResultText，更新 TextBlock.Text。

实际链是“按钮赋值 → setter 保存并发通知 → WPF 重读同一个对象的属性 → 结果文字更新”。OneWay 仍可持续更新，不表示只读初始一次。

原工具若出现 `nameof(ResultText)`，它得到属性名字符串 "ResultText"，不读取属性值。通知中的 ?.Invoke 是“有接收方法才调用”的简写，完整解释见 [事件 Q6](./csharp-events.md)。这些工具由骨架提供，不把每个语法都捆成首次 Binding 的前提。

不要另建一个数据对象后修改它，界面仍监听原 _state。也不要给结果控件 Text 直接赋值来刷新，普通赋值可能取代原绑定。当前 TASK-010 只绑定结果/状态输出，输入框仍由按钮代码直接取 Text。
===
先回答这个陌生名字：ReadingDisplayState 是教练新建的普通 C# 类，保存窗口的 ResultText 和 StatusText；不是 WPF 自带 API，也不是某个输入框。它的对象只保存显示文字与通知代码。

```csharp
private readonly ReadingDisplayState _state = new ReadingDisplayState();
```

ReadingDisplayState 是类名，new 创建一个该类的具体对象，字段 _state 保存引用。private 让字段供当前窗口类内部使用；readonly 限定初始化后不把字段换成另一个引用，仍可改这个对象的属性。原 `new()` 省略了左边已经确定的类型名，含义相同。

窗口构造里执行：

```csharp
DataContext = _state;
```

DataContext 是 WPF 窗口已有的属性，这里设置的是“这个窗口的绑定默认从哪个对象取值”。赋的是 _state 指向的对象，不是 ReadingDisplayState 类名、文件名或待显示文字；没有复制它，也没有创建第二份。

只有再配上文字控件的 `Text="{Binding ResultText}"`，该控件才按属性名从这个对象读取“尚未读取”。只设置 DataContext、但 Text 仍写死为“结果待显示”，界面就继续显示固定文字。

取值可以具体读成：“这个 TextBlock 沿用窗口 DataContext → 找到原 _state → 读它的 ResultText → 设置自己 Text”。后续属性变化如何刷新是 Q1 的下一层关系，先把来源讲清即可。
===
你看到的灰色 `(???)`、Path= 与下方空白预览，先分成三个地方看：磁盘 XAML、编辑器附加提示、真正运行的窗口。

当时磁盘保存的是 `Text="{Binding ResultText, Mode=OneWay}"`，并没有 (???)。Rider 会在 XAML 周围显示数据来源类型等内嵌提示；结合当时位置与源码，(???) 被解释为编辑器尚不能确定 DataContext 类型。这是对那次截图的判断，不是 WPF 异常，不能把灰色提示复制进代码。

下方设计预览也没呈现构造函数里设置的运行时数据，不能据此判定实际取值失败。不同设计器行为有差别，不把这个现象说成“设计器永远不执行 C#”。官方设置说明见文末。

当时实际打开 Task010Window 检查，结果控件已经取到“尚未读取”；真正的问题在第二块文字。状态控件也写了 ResultText，便显示相同文字。对应关系应是：

```xml
<TextBlock Text="{Binding ResultText, Mode=OneWay}" />
<TextBlock Text="{Binding StatusText, Mode=OneWay}" />
```

第一块取结果，第二块取状态。修复一个控件路径后，应观察实际原任务窗口的两段文字，不凭编译成功或设计预览作结论。这个路径问题后来已修复，当前不要照旧记录再宣称错误仍存在。
===
当 setter 调用 `OnPropertyChanged("ResultText")`，它要告诉 WPF“重新读取 ResultText”，而不是把新温度放进通知。携带这个属性名的参数对象来自 .NET：

```csharp
PropertyChangedEventArgs args = new PropertyChangedEventArgs("ResultText");
```

PropertyChangedEventArgs 是 System.ComponentModel 中的 .NET 类，不是教练新建类，不需要安装额外包。`using System.ComponentModel;` 让代码使用短类名，类似 Java import。new 创建一个参数对象，args 保存引用；它的 args.PropertyName 得到 "ResultText"。

自定义 OnPropertyChanged 接到 string propertyName 后，用同样方式 `new PropertyChangedEventArgs(propertyName)` 创建参数，并随 PropertyChanged 事件发出。WPF 接到名称，再从原数据对象读取 ResultText 的当前值。属性名与实际结果文字用途不同，通知不替你保存 _resultText。

ReadingDisplayState、OnPropertyChanged 是本项目自定义类/方法；PropertyChangedEventArgs、PropertyChangedEventHandler 和 INotifyPropertyChanged 则来自 .NET。这里先明确参数来自哪里、装什么，不要求重写整套通知工具或强制把合法字符串换成 nameof。
''', '''Q1 保持已掌握，Q2/Q3/Q4 保持待复习。2026-10-09 TASK-010 学员自主连接同一 DataContext、两个 OneWay Binding、调用原通知工具并修改数据属性，五种输入场景通过，局部应用计入 Q1；get/set、类结构、工具由骨架提供。Q3 初始检查中状态也绑定 ResultText，后来已自主修正为 StatusText；编辑器提示与设计预览机制不因此升级。旧临时检查路径、当时控件值及后续验收保存在原稿，本轮不重新运行。''')

add('wpf-collections', r'''
TASK-011 每点一次读取，历史列表应增加一行；清空则所有行消失。先找到实际保存记录的对象：ReadingHistoryState 是教练自定义类，它里面创建了一个 .NET 的 ObservableCollection<Reading>，保存多条 Reading。

```csharp
private ObservableCollection<Reading> _readings = new ObservableCollection<Reading>();

public ObservableCollection<Reading> Readings
{
    get
    {
        return _readings;
    }
}
```

new 创建空集合，_readings 保存引用；尖括号中的 Reading 是已有自定义读数类，规定集合中每项是一条读数。Readings 是自定义公开属性名，get 返回原集合，不创建另一份。没有 set 限制重新给属性赋集合，但仍可调用这个集合的 Add/Clear。

窗口的 _state 保存一个 ReadingHistoryState，DataContext 指向它。HistoryListBox 是窗口具体列表，ListBox 是 WPF 类；`ItemsSource="{Binding Readings, Mode=OneWay}"` 把这个列表的行数据来源连接到原 _state.Readings。

读取处理器创建 reading 后执行 `_state.Readings.Add(reading)`：集合保存这一条并发新增通知，已绑定的列表增加一行，用 Reading.ToString 的文字显示设备、数值和单位。Clear 移除所有项并发重置通知，原集合保留，之后还可 Add。不需要手工修改列表 Items 或反复重设 DataContext。

### 2026-10-11 追问：ObservableCollection 与 List 有什么区别？

两者都能保存多个元素，也都有 Add/Clear/Count。关键是绑定之后新增一条，列表是否收到通知。

```csharp
List<string> list = new List<string>();
list.Add("TEMP-001");
ObservableCollection<string> history = new ObservableCollection<string>();
history.Add("TEMP-001");
```

上面都保存了字符串。List 的 Add 不发 WPF 列表需要的集合变化通知；ObservableCollection 的 Add 会发 CollectionChanged。已存在的 List 数据可以通过 ItemsSource 显示，只是之后 List.Add 通常不会自动显示新行；ObservableCollection 会报告增删、替换、移动、重置。

集合通知不负责画屏幕，也不自动转发某条 Reading 内部 Value 的变化。列表必须先连接集合；如果以后要改旧读数的字段，还要考虑读数对象自己的属性通知，本次不追加这个实现。
===
两处都设置 DataContext，但变化的东西不同。TASK-010 改的是数据对象里一段结果文字；TASK-011 增删的是同一个历史集合中的记录。

```csharp
_state.ResultText = "读取完成";
```

这是 TASK-010 的情形：_state 指向 ReadingDisplayState，它的 ResultText 属性改值。绑定需要对象发 PropertyChanged 告诉它重读 ResultText；INotifyPropertyChanged 是 .NET 约定这种属性通知的接口。

TASK-011 则是：

```csharp
_state.Readings.Add(reading);
```

这里 _state 是另一个任务的 ReadingHistoryState。它的 Readings 仍返回原集合，属性指向没变；只是集合里多了一条。ObservableCollection 自己发 CollectionChanged，列表听集合通知即可，外层数据类当前无需为这次 Add 另发 Readings 属性通知。

如果将来把 Readings 换成新集合，外层对象要报告 Readings 属性变了，让列表去取新集合；新集合增删通知不能替它报告“换了集合”。若再增加持续变化的绑定 StatusText，也要报告文字属性变化。当前 TASK-011 直接设置状态控件 Text，不是绑定数据属性，所以与 TASK-010 的需求不同。

可以把 Readings 看成指向同一本清单：清单添一行，由清单报告；换一本清单，由保存清单引用的对象报告。已有记录内部属性变化又是第三种问题。DataContext 只指定来源，不会替这些变化自动发所有通知。
''', '''Q1 保持已掌握，Q2 保持待复习。2026-10-11 原窗口验证学员 Add/Clear、连续追加、换 ID、空白保护和恢复，集合与列表始终同一引用；集合创建/getter/绑定来自骨架。已有元素内部属性与更换集合机制不扩大掌握。2026-10-12 Q2 为比较两份源码的概念答疑，无新运行证据。''')

add('wpf-mvvm', r'''
TASK-013 希望读取命令拿到当前设备 ID，而不去访问窗口 TextBox。ReadingViewModel 是教练定义的普通 C# 类，保存当前 DeviceId、历史 Readings、提示 StatusText 和两个命令。

```csharp
private readonly ReadingViewModel _viewModel = new ReadingViewModel();
```

new 创建这个窗口使用的一个数据对象，_viewModel 保存引用。窗口执行 `DataContext = _viewModel`，使输入框、表格、按钮从同一个对象取属性。MVVM 是这种分工的名称：本例窗口 XAML 负责显示，Reading 保存一条读数，ReadingViewModel 提供显示所用数据与动作；这些类名都是项目自定义，DataContext 是 WPF 属性。

输入框连接如下：

```xml
Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
```

左边是这个 TextBox 的 Text，右边 DeviceId 是同一个 ReadingViewModel 的公开属性。TwoWay 让初始 TEMP-001 显示到输入框，也让用户改字后写回对象。UpdateSourceTrigger=PropertyChanged 使文字一变就写回；它是 WPF Binding 的设置，不是项目通知方法 OnPropertyChanged。

ReadingViewModel 构造函数用 `new ReadTemperatureCommand(this)` 把自己交给读取命令。命令把传入引用存到自己的 _viewModel 字段，因此 `string id = _viewModel.DeviceId;` 取得当前 ID。两个地方的字段分别属于窗口与命令，但指向同一个对象，不另建第二份。

### 2026-10-12 追问：在 TextBox 更改值，会自动更新 ViewModel.DeviceId 吗？

当前连接会。用户改成 TEMP-002，WPF 写入原对象的 DeviceId，效果相当于 `_viewModel.DeviceId = "TEMP-002"`，不要求再补这行。

```csharp
set
{
    _deviceId = value;
    OnPropertyChanged("DeviceId");
}
```

这是原 DeviceId 的写入部分。value 接到新字符串，第一行保存到字段，之后 getter 返回该字段即可取到 TEMP-002。第二行报告属性变化，作用与“取得输入框新文字”不同。写回不需要等点读取，读取按钮之后使用已保存 ID。XAML 的 TwoWay/PropertyChanged 不应加入字面星号。

### 2026-10-12 追问：文字已经在 TextBox 改过，为什么 setter 还要通知界面？

只看用户在这一个输入框输入、setter 原样保存，第一行已完成保存；通知不是取得新文字的动作。还要考虑另一个来源：程序直接执行 `_viewModel.DeviceId = "TEMP-003"`。输入框没有被用户编辑，需要对象报告变化，WPF 才重读 DeviceId 并把新 ID 显示过去。

若另一块文字也绑定 DeviceId，它也靠这个通知刷新。当前项目没增加这块文字，这是说明用途的例子。通知携带属性名 "DeviceId"，不携带新 ID，WPF 再读值。TwoWay 允许双向连接，不能替代对象变化通知。

### 2026-10-12 追问：StatusText 为什么不采用 TwoWay 和 UpdateSourceTrigger=PropertyChanged？

StatusText 显示在 TextBlock，用户不在这块提示里输入，只需对象到控件的 OneWay。命令执行 `_viewModel.StatusText = "读取成功"` 后，setter 保存、通知，WPF 重读提示并更新文字。

DeviceId 在可编辑 TextBox，需要把用户输入写回，所以用 TwoWay。UpdateSourceTrigger 另外决定“何时写回”：PropertyChanged 是文字变时，LostFocus 是离开输入框时，也是 TextBox.Text 的默认时机。失焦写回只更新数据对象，不保证已保存文件或发送设备。StatusText 的 OneWay 无写回方向，就无需这项时机设置。

旧 Reading 中的 DeviceId 是当时创建那条读数时保存的值。改变当前 ViewModel.DeviceId 不会自动把旧历史改掉，原读取命令继续创建新 Reading 并 Add 同一集合。
''', '''Q1 保持待复习。2026-10-12 TASK-013 的 10 个场景验证读取、Trim/空白 return、Reading 创建、Add 与提示，学员自主修复漏 return。输入绑定/完整属性/通知/对象连接由骨架提供，综合机制不按通关全部升级。教练未展开修改来源导致的追问保留为教学反馈，不记错题；漏 return 的实际实践错误保留原错题记录。''')

add('wpf-usercontrol', r'''
TASK-014 的窗口里，“读取历史”标题、列表和清空按钮被装进同一个区域。ReadingHistoryControl 是本项目自定义控件类，继承 WPF 的 UserControl；它放进窗口显示，不是又打开一个 Window。

`ReadingHistoryControl.xaml` 描述这块区域的元素，后台 .xaml.cs 定义配对的 C# 类。x:Class 指向完整类名，partial 是 C# 关键字，让手写与构建生成部分共同组成这个类。构造中的 InitializeComponent 是 WPF 构建生成的方法，用来创建 XAML 描述的元素。

窗口起始标签已有：

```xml
xmlns:history="clr-namespace:WpfBae.Exercises.Task014"
```

history 是教练选的前缀，右边指出控件的 C# 命名空间。之后窗口内部 `<history:ReadingHistoryControl />` 创建并放置这个类的一个控件；不是按文件路径读取一段文本。标题、列表、按钮由它自己的 XAML 提供。

窗口后台创建一份数据对象：

```csharp
private ReadingViewModel _viewModel = new ReadingViewModel();
```

ReadingViewModel 是已有自定义类，new 创建 TASK-014 窗口使用的对象，字段保存引用。`DataContext = _viewModel` 给窗口指定数据来源。历史控件没另设 DataContext，接到窗口后沿用这个对象；内部列表和清空按钮再取它的 Readings 与 ClearCommand。

这叫 DataContext 的属性值继承：沿用父元素设置的同一个对象。不是复制历史，也不是 ReadingHistoryControl 类继承 ReadingViewModel 类。若控件自己又 new 一份 ViewModel 并设为来源，就会显示另一份空集合，窗口读取可能成功而内部列表没有行。

原首个观察是在控件 StackPanel 内放 `<TextBlock Text="读取历史" />`，对应屏幕上这块区域的标题。当前内容已完成，复习直接读 [控件 XAML](../../WpfBae/Exercises/TASK-014-history-usercontrol/ReadingHistoryControl.xaml)，不要求重做。列表没写 x:Name 仍能 Binding 取属性；x:Name 用于命名控件，不是数据绑定必需条件。
''', '''Q1 保持待复习。2026-10-12 学员在骨架中组合标题/列表/按钮、应用 ItemsSource/Command 已通过 9 个真实窗口场景，窗口与内部控件同一个 DataContext/集合/命令。类定义、命名空间、窗口对象连接和属性继承机制由骨架提供，保留实践范围而不升级综合条目。''')

add('wpf-styles', r'''
ReadButton 和 ClearButton 是“读取温度”“清空历史”两个具体按钮，都由 WPF 的 Button 类创建。要让两者文字周围都有左右 24、上下 10 的留白，可以共用下面一组设置：

```xml
<Style x:Key="ActionButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="24,10" />
</Style>
```

Style 是 WPF 的设置对象，这段放在已有 Window.Resources 里，保存供窗口复用。`TargetType="Button"` 说“这组设置给 Button 按钮用”；Setter 指定给该按钮的 Padding 设置 24,10。Padding 是按钮文字与自身边缘的内部留白，两个数分别是左右、上下，不是与别的控件的外部间距。

x:Key 给这份资源取名 ActionButtonStyle，这个名字是教练自定，不是 WPF API。带这个键的 Style 放在资源里，不会仅靠名字自动连到两个按钮；按钮要引用它：

```xml
Style="{StaticResource ActionButtonStyle}"
```

左边设置这个 Button 的 Style 属性；右边 StaticResource 让 WPF 按键找到已定义的外观对象。StaticResource 是 WPF 的 XAML 写法，不是 C# static，也不读取 ViewModel 的命令。

若按钮上还直接写 Padding="12,6"，这个直接设置会压过 Style 中的 Padding Setter，留白仍用 12,6。把同一个属性移交给 Style 后，才会用共同值。按钮自己的 Margin 是外部位置间距，可继续保留，别因名字相似一并删除。

当前两个按钮引用同一份 Style，改共同 Padding Setter 就能一起调整。实际原窗口与独立副本的比较放在证据区，不插在语法解释中。窗口 ReadButton 使用 StaticResource，独立历史控件内 ClearButton 使用 DynamicResource，原因单独看 Q2，不在首次 Style 说明中混进查找时机。
===
当时 ClearButton 加 StaticResource 后，原窗口启动报“无法找到 ActionButtonStyle”。键名并没拼错，问题是它查找时还没连接到包含这份资源的窗口。

1. 窗口加载，创建 Window.Resources，窗口读取按钮可以找到 Style。
2. 窗口创建自定义 ReadingHistoryControl。
3. 控件构造里先执行自己的 InitializeComponent，加载它内部的 XAML。
4. 这时内部 ClearButton 解析 StaticResource，历史控件还没挂到窗口的父子结构，无法向上找到窗口资源，加载就失败了。

“最终显示在窗口内”不等于“构造时已经有父窗口”。类命名空间或文件夹放哪里，也不决定这份外观资源当时是否可见。XamlParseException 是加载 XAML 失败的外层异常，内层“找不到资源”才是本例具体线索；编码造成的乱码不改变这个原因。

当前历史控件使用：

```xml
Style="{DynamicResource ActionButtonStyle}"
```

左边仍给 ClearButton.Style 取值；右边保留按键查找的表达式，可以延后求值，并在连接父元素后重新找到窗口中的同一份 Style。DynamicResource 是 WPF 提供的写法，不是监视磁盘 XAML，也不是再创建一个业务对象。

当时单处修复只改 [历史控件 ClearButton 的资源引用](../../WpfBae/Exercises/TASK-015-button-style/ReadingHistoryControl.xaml)，保留窗口 Style 与命令绑定。窗口读取按钮仍可 StaticResource，不要求全部换掉。若另把共享样式放 Application.Resources，可以改变可见范围，但本题不搬资源或复制样式。

这次漏讲独立控件初始化时机来自教练指导，不是学员典型错误。后续原窗口已经成功打开，旧失败记录不能继续写成当前阻塞。
''', '''Q1 保持已掌握，Q2 保持待复习。2026-10-12 TASK-015 学员补齐 Setter、引用、移除直接属性，原窗口两个按钮同一 Style，9 个业务场景通过。原 Padding=24,10 时两按钮 98×37.24，教练独立副本改为32,14后为114×45.24；不计为学员自主运行时切换。资源异常曾在原编译窗口重现，内存诊断改 DynamicResource 验证同一资源；后续完整原窗口也已验收。Q2 的查找时机机制不随引用应用升级。''')

add('wpf-templates', r'''
读取 TEMP-001、TEMP-002 后，要分别控制每行的设备 ID、数值和单位。原 ToString 把它们拼成一段文字，不方便给三段文字分别留空隙。下面给每条 Reading 安排三个文字控件：

```xml
<ListBox.ItemTemplate>
    <DataTemplate>
        <StackPanel Orientation="Horizontal">
            <TextBlock Text="{Binding DeviceId}" Margin="0,0,12,0" />
            <TextBlock Text="{Binding Value}" Margin="0,0,12,0" />
            <TextBlock Text="{Binding Unit}" />
        </StackPanel>
    </DataTemplate>
</ListBox.ItemTemplate>
```

这段位于当前 ListBox 内。ListBox.ItemTemplate 是给这个列表的“每一项怎样显示”属性赋值；DataTemplate 是 WPF 的数据展示模板。它不创建新读数，不调用设备。内部 StackPanel 从左往右排列三个 WPF TextBlock。

模板显示第一条 Reading 时，`Binding DeviceId` 取第一条的设备 ID；显示第二条时取第二条的 ID。Value/Unit 同样取本行那条读数。不是每行都去取窗口 ReadingViewModel.DeviceId，也无需另设窗口 DataContext。

ItemsSource 先从窗口数据对象拿整份 Readings；ItemTemplate 再逐条展示这份集合里的 Reading。两个来源层次对应不同对象，所以窗口输入框里的同名 DeviceId 表示当前输入，模板中的 DeviceId 表示某条历史读数。

Reading 是已有自定义类，DeviceId/Value/Unit 是其公开属性。两个 Margin 给前两块文字右侧各留 12，第三块文字不需为了前面的间距再留右边空隙。原首步只连接设备 ID，一个效果看清后再补其他字段；现在已经完成，复习读 [真实模板](../../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml)即可。
===
当时数值全为空，Reading.Value 却是 26.5，而且构建通过。先看第二个 TextBlock 的旧错误：

```xml
Text="{Binding FallbackValue, Mode=OneWay}"
```

Binding 后的第一个名字表示从本行 Reading 查找哪个属性。这里是查找名为 FallbackValue 的属性；Reading 只有 DeviceId、Value、Unit，没有它，运行时绑定报告 PathError，文字就为空。

正确路径是 `Text="{Binding Value, Mode=OneWay}"`，显示这条读数的 Value。OneWay 只说取值方向，不能修正拼错的属性名。XAML 编译成功不保证运行时每个数据属性路径都有效。

数值修好后当时又看到 `TEMP-00126.5` 紧挨着。横向排列时，空隙应在“ID 的右边”或“数值的左边”：

```xml
<TextBlock Text="{Binding DeviceId}" Margin="0,0,12,0" />
```

四个数是左、上、右、下，此处在 ID 之后留 12。若只给数值右边留 12，它隔开的是数值与单位，不能隔开 ID 与数值。一个数表示四边相同；两个数分别是左右、上下；标准四数写法更容易看清是哪边。

原复查依次修一个现象：先让 Value 出现，再把 ID 右边距补齐。两次修复最终都已通过，复习不应把旧截图说成当前错误，也不要求覆盖现有正确代码。
''', '''Q1/Q2 保持已掌握，范围限模板骨架中的三字段绑定、逐条数据源、路径纠正与横向间距。2026-10-12 TASK-016 数值错误、随后缺间距均由学员自主修复，最终10个场景通过，各绑定 Active，两段间距12，旧行保留。外层模板与业务连接由骨架提供，完整绑定引擎不一并升级；修复前版本及证据在原稿/任务证据中保留。''')

add('wpf-controltemplates', r'''
TASK-017 想让“读取温度”“清空历史”两个按钮内部都有圆角边框和文字。ReadButton、ClearButton 是两个具体 Button；Button 是 WPF 的按钮类，不是某一个按钮对象的名字。

先看描述一个按钮内部的少量代码：

```xml
<ControlTemplate TargetType="Button">
    <Border CornerRadius="12" Background="#E8F0FE"
            BorderBrush="#4267A9" BorderThickness="1">
        <ContentPresenter />
    </Border>
</ControlTemplate>
```

`TargetType="Button"` 表示“这份内部布局给 Button 按钮用”。Border 是 WPF 边框元素，CornerRadius=12 使四角变圆，Background 填背景色，BorderBrush/BorderThickness 设置边框颜色与宽度。内部 ContentPresenter 让按钮文字出现在这块边框里，具体内容来源放到 Q2 单独讲。

这份 ControlTemplate 需要交给按钮的 Template 属性。原样式采用：

```xml
<Setter Property="Template">
    <Setter.Value>
        <!-- 此处放上面的 ControlTemplate -->
    </Setter.Value>
</Setter>
```

Setter 在共用 Style 中给 Button.Template 赋值；Setter.Value 是给这项设置的 Value 属性放一个模板对象，不是窗口上新增的控件。共享的是模板定义，每个具体按钮有自己的内部元素，因此不会共用一个文字框冒充两个按钮。原命令仍在外部 Button 上，模板不读取温度或清空历史。

原首步是在 [Task017Window.xaml](../../WpfBae/Exercises/TASK-017-button-controltemplate/Task017Window.xaml) 只把 Border.CornerRadius 从 0 改成 12，观察两个按钮变圆。当前正确代码已经完成，不重置；留白连接和禁用规则分别到 Q3/Q4 再读。

### 2026-10-13 补充：两个 TargetType="Button" 到底在说什么？

`Style TargetType="Button"` 说“这一组属性设置给 Button 按钮用”，其中 Padding Setter 设置两个按钮文字周围留白。`ControlTemplate TargetType="Button"` 说“这份内部布局给 Button 按钮用”，其中 Border 画外框、ContentPresenter 放文字。

一个设置按钮属性，一个描述按钮内部怎样画；都给当前这两个 Button 使用，所以都写 Button。先说明这一关系，不靠“应用对象类型”之类换词定义，也不在同一句塞入模板取值或条件规则。
===
本例所谓“内容”就是按钮上的“读取温度”四个字。Button.Content 指定这四个字；模板内部的 ContentPresenter 是 WPF 提供的显示元素，让这四个字出现在 Border 画的边框里面，不是第二个按钮。

读取按钮 `Content="读取温度"`，清空按钮 `Content="清空历史"`。模板若写死 `<ContentPresenter Content="读取温度" />`，两个按钮都会显示读取温度，所以不能把写死文字当作共享模板的最终写法。

可用显式连接说明这两个不同对象的同名属性：

```xml
<ContentPresenter Content="{TemplateBinding Content}" />
```

左边设置内部 ContentPresenter.Content；右边 TemplateBinding 从当前这个 Button.Content 取值。给读取按钮用就取读取温度，给清空按钮用就取清空历史；取值来源是包住它的按钮，不是窗口 ViewModel 或历史 Reading。

你后来观察“删掉这项仍有文字”是正确的。在当前 Button 的 ControlTemplate 内，这种 ContentPresenter 默认会取得 Button.Content，所以显式写法可选。ContentSource 默认值为 Content，支持这一默认连接；此处只交代默认来源，不要求你再使用新属性。

显式版本通常不产生外观变化；目的只是写清来源，不能强制当必需功能。原任务已把 TODO 2 改成可选，显示各自按钮文字的验收仍保留。也不能把这里特定的默认规则推到 Border.Padding；那一项需要实际连接，见 Q3。
===
先定位两个 Padding：上方 Style 的 Setter 给外部 Button.Padding 设置 24,10；内部 Border.Padding 才直接决定文字与画出的边框之间的留白。它们分别属于 Button 与 Border，名字相同不会自动连接。

```xml
<Setter Property="Padding" Value="24,10" />
```

这一行让使用共用 Style 的按钮得到左右24、上下10。若模板 Border 也固定写 Padding="24,10"，只是两边恰好同值；之后只改 Setter，内部固定值仍不跟随。

当前 Border 改为：

```xml
Padding="{TemplateBinding Padding}"
```

等号左边是 Border.Padding；右边读取当前按钮的 Button.Padding。TemplateBinding 是 WPF 在控件模板内部从外部这个按钮取属性的写法。于是 Style 先给 Button 值，Border 再取这个值，在文字周围留出空间。

刚建立连接时外观可能不变，因为原值相同；单独改变 Button.Padding 或共同 Setter，内部 Border 留白跟随才是可观察变化。历史审查已经做过这个诊断，不把教练临时改值算成学员独立完成新的实践。
===
希望清空按钮没记录时更淡，有记录时恢复正常。已有命令决定 Button.IsEnabled；模板只给这个状态画出区别，不负责执行清空。

先认识 Opacity：它是 WPF 元素的不透明度，1 正常不透明，0.4 更淡，0 完全透明。给 Border 设0.4，其背景、边框和内部文字一起变淡；变淡本身不禁止点击。

再看原模板规则：

```xml
<Trigger Property="IsEnabled" Value="False">
    <Setter TargetName="ButtonBorder" Property="Opacity" Value="0.4" />
</Trigger>
```

这段放在已有 ControlTemplate.Triggers 中，保存本模板的条件规则。外层 Trigger 检查“当前 Button.IsEnabled 是否 False”；满足时临时执行内部设置。Trigger 是 WPF 条件规则，不是按钮 Click 事件。

内层 Setter 设置的是模板中名为 ButtonBorder 的 Border.Opacity。ButtonBorder 是上方 Border 的 x:Name，来自教练自定义命名，不是 WPF 类型。条件属于 Button，变淡的对象是内部 Border，一行一行分开读。

Button 再变为可用时，条件不成立，这项临时设置撤销；当前没其他 Opacity 设置，便回到默认1。无需把所有 Border 固定设0.4，也无需新增启用 Trigger。原可观察流程是空历史淡且禁用 → 读取后恢复 → 清空后变淡 → 再读恢复，原业务命令保持。
''', '''Q1/Q2 保持待复习，Q3/Q4 保持已掌握。2026-10-12 TASK-017 原窗口11个显示/业务场景通过，学员自主 Padding 连接与禁用 Trigger 已验证；教练临时修改 Button.Padding 的内部取值诊断不扩大学员证据。初始清空 False/0.4，读后True/1，清空后False/0.4，再读True/1。显式 Content 连接误设必做、TargetType 抽象解释、TODO 提前引入新语法均为教学更正，不记学员错题、不撤销已验证应用范围。''')

add('wpf-commands', r'''
清空历史按钮应在没记录时禁用，有记录时可点。ClearHistoryCommand 是本项目自定义命令类，WPF 按钮把“能不能清空”和“真正清空”交给这个命令对象。

先看条件，来自原 ClearHistoryCommand 的简化等价写法：

```csharp
public bool CanExecute(object? parameter)
{
    return _state.Readings.Count > 0;
}
```

_state 保存原 ReadingCommandState 数据对象，Readings 是它的历史集合。Count>0 得到 true/false，WPF 用它更新按钮是否可用；object? parameter 是 .NET ICommand 接口要求的参数，本任务未提供额外命令参数，所以不使用它。CanExecute 本身不清空。

按钮正常点击执行 Execute，其中 `_state.Readings.Clear();` 清空同一集合，`_state.StatusText = "已清空";` 保存完成提示。这与判断条件是两个方法，不能发一次条件通知就当作用户点清空。

窗口创建一个 ReadingCommandState；它构造时用 `new ClearHistoryCommand(this)` 把自己交给命令。this 在这里指当前数据对象，命令字段保存它的引用。公开属性 ClearCommand 的完整 getter 返回原 _clearCommand，不每次创建新命令。

```xml
Command="{Binding ClearCommand, Mode=OneWay}"
```

左边设置具体清空 Button 的 Command 属性；右边从窗口 DataContext 指向的原数据对象读 ClearCommand，取得那个命令对象。Button.Command 是 WPF 属性，ICommand 是 .NET 接口，ClearCommand/ClearHistoryCommand 是项目自定属性/类名。

### 2026-10-12 理解确认：按钮绑定对象与集合变化后的条件检查链

实际来源是“Binding 读公开 ClearCommand → getter 返回 _clearCommand → 按钮持有这个命令”，不是 XAML 直接访问私有字段。初次连接时 WPF 就会检查条件，因此空集合的按钮不必等第一次集合变化才禁用。

新增记录后，骨架监听原集合 CollectionChanged，调用自定义 NotifyCanExecuteChanged；命令通知 WPF 再问 CanExecute，Count>0 后按钮可用。新增从1变2仍会发通知，但检查结果依旧true，通知不保证条件值改变。

### 2026-10-11 追问：public event EventHandler? CanExecuteChanged 有什么用？

它为这个命令公开一个事件，让按钮接收“条件可能变化，请重新检查”。event/public 是 C# 关键字，EventHandler 是 .NET 的委托类型，CanExecuteChanged 是 ICommand 规定的事件名，? 允许没有接收者。

事件声明只提供登记入口，不自动通知，也不是保存 bool。发通知的完整说明写法是：

```csharp
EventHandler? handler = CanExecuteChanged;
if (handler != null)
{
    handler(this, EventArgs.Empty);
}
```

这在命令内部取得已登记方法，检查存在后调用。这里 this 指发通知的命令对象，EventArgs.Empty 表示没额外数据。当前原 TASK-012 已用 ?.Invoke 简写，完整对应见 [事件 Q6](./csharp-events.md)。CollectionChanged 报集合增删；CanExecuteChanged 让按钮重查条件，两项通知职责不同。

读取 Count、遍历集合不会触发集合通知，改变旧 Reading 内部属性也不自动触发集合通知。实际 Add/Clear 才走本题已有的集合通知连接。

### 2026-10-11 追问：CanExecuteChanged 指向 CanExecute 吗，类似 Java 反射吗？

它登记的是 WPF 接收通知的方法。接收方法再通过 ICommand 调用 CanExecute；事件没有直接存 CanExecute，也不是按 Changed 名字找到对应方法。CanExecute 返回bool、接收一个参数，本身也不符合 EventHandler 的两个参数/void要求。

Java 监听器接通知、再调用检查方法的类比更合适。ICommand 已约定 CanExecute，WPF 可以按接口调用，不需要反射按字符串找它。

### 2026-10-11 追问：示例中的 command 是由事件参数 this 取得的吗？

说明代码 `ICommand command = _state.ClearCommand;` 的 command 来自数据属性；事件 `handler(this, EventArgs.Empty)` 的 this 被传给接收方法的 sender。这是两次不同的参数/属性动作，虽然可以指向同一命令，不能把属性读取说成来自sender。

按钮早已从 Command 绑定取得命令。官方 ButtonBase 接收通知时调用 UpdateCanExecute，通过按钮已持有的 Command 重查，不依赖 sender 来取命令。不同位置的 this 也不同：数据构造中是数据对象，命令通知方法中是命令对象。

### 2026-10-11 追问：当前已注册的处理方法具体是什么？

按本次核对的 WPF 官方源码，命令事件直接连接内部 CanExecuteChangedEventManager.HandlerSink 的 OnCanExecuteChanged，它再转发到按钮基类 ButtonBase.OnCanExecuteChanged。前者负责把通知转给按钮，后者再检查条件。它们是框架方法，不在学员源码中，也不是学员的 CanExecute。

这里说明源码里的实现路径，未在本轮运行程序枚举实际订阅列表；不扩展弱引用工具内部设计。正式依据附在文末，阅读主线只需先理解“接通知的方法收到后再检查条件”。

### 2026-10-11 追问：“WPF 把处理方法订阅到事件上”具体是什么意思？

先看一个自定义说明方法，放在能使用 _state 的同一示例类中：

```csharp
void OnCommandChanged(object? sender, EventArgs e)
{
    Console.WriteLine("收到命令变化通知");
}
```

在该类另一个方法里可登记它：

```csharp
ICommand command = _state.ClearCommand;
command.CanExecuteChanged += new EventHandler(OnCommandChanged);
```

new EventHandler 保存 OnCommandChanged 的引用，+= 把它登记到原命令的事件。登记时不执行；之后命令调用当前事件的处理方法，才会输出收到通知。这个方法只是说明订阅动作，不要求加到当前项目。

WPF 本身也是写好的 C# 类。按钮连接命令时，框架用相同的 += 动作登记内部转发方法，所以你的项目没手写这行也能收到通知。框架接收后的动作是更新按钮条件，不是这里的 Console 输出。

### 2026-10-11 追问：handler(this, EventArgs.Empty) 为什么传 this？

EventHandler 接收方法有 sender/e 两个参数。this 是正在发通知的 ClearHistoryCommand；传给第一项后，接收方法 sender 得到这个对象的引用，表示“通知来自谁”。不复制类源码，也不创建第二个命令。

多个命令可共用一个接收方法，用 sender 区分来源。标准约定传发送者，但接收者可以不用；本题按钮仍从已持有的 Command 检查条件。第二项 EventArgs.Empty 对应 e，没有true/false，条件要另问CanExecute。

“发布通知会调用 WPF 按钮接收方法”的复述主线有效；仍要补上它只重新判断，既不模拟点击，也不执行清空。所有通知步骤由既有骨架/框架提供，不要求在本次整理中新增练习。
''', '''Q1 保持已掌握，范围限原骨架上自主实现 Count>0、Execute.Clear 与提示。2026-10-11 TASK-012 的8个实际场景已验证禁用/启用/清空/恢复和空白保护；委托基础、接口事件声明、通知工具、框架登记与转发继续待复习。2026-10-11/12 的复述与源码核对保留为过程反馈，不扩展自主实践证据。旧稿各追问时“进行中/未运行”属于当时记录，任务最终通关状态保持。''')

add('wpf-charts', r'''
TASK-018 的蓝线叫 TemperatureLine，是 WPF 的 Polyline；ChartCanvas 是包住它的 WPF Canvas。两个名字来自教练。Polyline 根据收到的坐标按顺序连线，Canvas 提供放置空间，名字本身不把温度变成坐标。

Canvas 左上角是(0,0)，向右x增加，向下y增加。先看原占位方法中的一点：

```csharp
points.Add(new Point(280, 180));
```

Point 是 WPF 使用的坐标类型，new Point 创建x=280/y=180的坐标。points 是该方法创建的 PointCollection，Add 把坐标加入它。原首步只把180改为100，点就上移80，两端没有改；这是认识位置，不是完整温度计算。

希望温度越高点越上，需要给高温较小的y。本题固定y=240对应20度，y=40对应30度：10度覆盖200个界面单位，一度上移20。计算可读成：

```csharp
double y = 240 - (temperature - 20) * 20;
```

这是已完成转换关系的说明：temperature是本次那条温度，先减20看比底部高多少度，再乘20得到上移距离，从底部240减去。24度得到160，26.5度得到110，28度得到80。直接拿温度当y会让高温向下走，也不符合本题范围。

ConvertTemperatureToY 是自定义普通方法，接温度、返回y；DrawTemperatureLine 是自定义绘图方法，逐次取x/温度、创建Point加入集合，最后 `TemperatureLine.Points = points` 设置这条蓝线的坐标属性。Points 接坐标集合，不接温度数组。

当前原代码已完成，见 [TASK-018 后台](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml.cs)。旧“返回固定140/占位水平线”的说法是启动骨架历史，本次不改回去。
===
三个标签的中心与三个点的x相同，因为骨架手动算了位置，不是名字自动匹配。先看实际第一个标签：

```xml
<TextBlock x:Name="FirstReadingLabel" Canvas.Left="55" Canvas.Top="260"
           Width="90" TextAlignment="Center" />
```

TextBlock 是WPF文字控件，FirstReadingLabel 是教练命名。Canvas.Left=55 放它的左边到x=55；Width=90使右边到145，中心是55+90/2=100。TextAlignment=Center只让文字在这90宽的框内居中，不负责把整个框移到点下方。

| 标签 | 点的x | 文字框左边 | 中心计算 |
| :--- | ---: | ---: | :--- |
| 第一条 | 100 | 55 | 55+45=100 |
| 第二条 | 280 | 235 | 235+45=280 |
| 第三条 | 460 | 415 | 415+45=460 |

Canvas.Top=260 设置文字框上边的y，绘图区底部y=240，所以文字在下方。蓝线点的y随温度改变，标签y固定；“对应”指横向中心，不表示两者高度相同。

两者在同一Canvas内，蓝线无额外偏移，采用同一起点。若单独把第一点x改为120，标签中心仍100；要继续对齐，文字框左边要设为120-45=75。这个演算只是解释，不要求修改已完成作品，见 [原XAML](../../WpfBae/Exercises/TASK-018-temperature-polyline/Task018Window.xaml)。
''', '''Q1/Q2 均保持待复习。2026-10-13 TASK-018 原窗口三个坐标(100,160)/(280,110)/(460,140)已验证；教练独立28度副本中间y变80、上移30，两端不变、标签可读。学员自主补循环加点、Points赋值与调用，局部应用有效；转换计算/标签布局由既有实现提供，不计独立设计。原任务三项DoD通关，本轮未运行或扩大学员掌握范围。''')

add('wpf-datagrid', r'''
TASK-019 的 HistoryDataGrid 是一张具体表格，用WPF的DataGrid类创建。读取TEMP-001、再读取TEMP-002后，一条Reading占一行，设备/温度/单位分列；不是调用ToString把整行拼成一个格子。

先看第一列的现有写法：

```xml
<DataGridTextColumn Header="设备 ID" Binding="{Binding DeviceId}" Width="*" />
```

DataGridTextColumn 是WPF文字列。Header只指定顶部标题“设备ID”，不读取数据。Binding指定本列各行的字段：第一行读第一条Reading.DeviceId，第二行读第二条Reading.DeviceId。Width的星号让列分配剩余宽度。

再看表格取整份历史的连接：

```xml
ItemsSource="{Binding Readings, Mode=OneWay}"
```

左边是这张DataGrid的ItemsSource；右边从窗口DataContext指定的原ReadingViewModel读取Readings，取得历史集合。这让表格知道“有哪几条Reading”，上面列Binding再说明“这条Reading哪个属性放这列”。

温度列Header可写中文“温度”，Binding却要写Reading.Value；单位列取Unit。列名和属性名不是同一用途。换当前输入ID只改ViewModel.DeviceId，旧行仍读各自Reading.DeviceId，所以不会全变成新设备。

AutoGenerateColumns=False只用手写三列；DataGrid.Columns保存这张表的列，内部三个列标签不是三张表。IsReadOnly=True禁止单元格编辑，CanUserAddRows/CanUserDeleteRows=False禁止用户从表直接增删记录；不妨碍原命令Add/Clear集合。

原ObservableCollection的Add发新增通知，表格增加行，再按列取新Reading三个属性；Clear移除所有项并通知，表格清空，集合仍保留供下次读取。无需给表手工添加行或换集合。

旧首步只把第一列Header改为设备ID，观察标题变化。现在数据连接和三列已经完成，见 [原XAML](../../WpfBae/Exercises/TASK-019-reading-datagrid/Task019Window.xaml)，不继续把它称作“表格尚未连接”。列未写Mode的现有只读显示已验证，不为重写文档强迫改源码。
''', '''Q1保持已掌握，范围限提供骨架内Header、ItemsSource和三列字段Binding。2026-10-13原编译窗口11个实际场景通过，各单元格Active、读取各条Reading，旧行保留、同集合清空/恢复、空白保护、最小尺寸可读、BeginEdit为false；43个源码哈希不变。只读设置、窗口和原命令不计本次自主设计，编辑/排序机制不扩大。''')

add('wpf-combobox', r'''
TASK-020 顶部选择框展开后有TEMP-001、TEMP-002，点一项后收起显示它。ComboBox是WPF下拉框类，DeviceComboBox是教练给这个具体控件起的名字。Width决定这块选择框多宽。

原第一个动作是 [Task020Window.xaml](../../WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml) 的TODO1：只把Width从200改成260。整理期间重读源码，Width已经是260，保留这项修改，不再要求重复改写。在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，预期实际TASK-020窗口下拉框变宽；本次未核对运行结果，设计预览不能代替这个现象。

选项来自后台已准备的数组：

```csharp
private readonly string[] _deviceIds = new string[] { "TEMP-001", "TEMP-002" };
```

string[]是C#字符串数组写法，new创建含两个ID的数组，_deviceIds保存引用。private限定窗口类内部使用，readonly限定初始化后不换引用，不冻结数组内部。然后 `DeviceComboBox.ItemsSource = _deviceIds;` 把数组交给具体下拉框的WPF数据列表属性。它存“可选设备”，Readings存“已读温度”，用途不同。

本次选项都是字符串，选TEMP-002时，SelectedItem取得字符串"TEMP-002"。SelectedItem是这个下拉框的WPF属性，不是整个控件或整份历史。

后台new ReadingViewModel创建本窗口的一个已有自定义数据对象，_viewModel保存引用，DataContext指向它，DeviceId初始为TEMP-001。完成TODO2时给ComboBox添加：

```xml
SelectedItem="{Binding DeviceId, Mode=TwoWay}"
```

左边设置下拉框SelectedItem；右边从原数据对象读DeviceId。初始值TEMP-001在选项中，就选中它；TwoWay允许用户改选时把新字符串写回同一个对象。原DeviceId setter中的value接到新字符串，保存到_deviceId，原读取命令下次取它就用新ID。

原Reading保存当时的ID，表格各行取各自Reading属性，所以改选不改旧行。IsEditable=False允许选已有项，不能自由输入第三个ID；键盘可帮助选择已有项，仍不会新增任意字符串。它不决定读取按钮是否可用，按钮条件仍由原命令处理。

目前TODO2未完成，初始为空是占位；改选也不写回DeviceId，读数仍用原TEMP-001。宽度源码已修改，运行观察后再连接选中值，之后实际默认选择/两个ID读取/旧行/清空恢复按任务卡检查。本次整理不替学员补连接。
''', '''Q1保持待复习，TASK-020保持🟡进行中。2026-10-13启动时教练提供两个选项、ItemsSource、DataContext和复用表格/命令，骨架构建退出0、0警告/错误。本次整理期间发现Width已260，保留工作区这项修改；未运行新代码或验收，SelectedItem核心TODO仍未完成，不把单纯源码改动标为已掌握。上述运行步骤是当前任务建议，不是本轮执行证据。''')

def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest().upper()

def protected_check(records):
    changes = [r['Path'] for r in records if sha(Path(r['Path'])) != r['Hash']]
    if changes:
        raise RuntimeError('Protected files changed: ' + repr(changes))
    return len(records)

def metadata(text):
    return re.findall(r'^- 状态：(.+)$', text, re.M)

def readable_prose(text):
    in_code = False
    lines = []
    for line in text.splitlines():
        if line.startswith('```'):
            in_code = not in_code
        if not in_code and not line.startswith(('#', '```')):
            pieces = re.split(r'(`[^`]+`|\[[^\]]+\]\([^)]+\))', line)
            for i in range(0, len(pieces), 2):
                pieces[i] = re.sub(r'(?<=[\u4e00-\u9fff])(?=[A-Za-z0-9])', ' ', pieces[i])
                pieces[i] = re.sub(r'(?<=[A-Za-z0-9])(?=[\u4e00-\u9fff])', ' ', pieces[i])
            line = ''.join(pieces)
        lines.append(line)
    return '\n'.join(lines)

def local_links(text, folder):
    missing = []
    for label, url in re.findall(r'\[([^\]]*)\]\(([^)]+)\)', text):
        if re.match(r'^[a-z]+://', url) or url.startswith('#'):
            continue
        path = url.split('#')[0].strip('<>')
        if path and not (folder / path).resolve().exists():
            missing.append((label, url))
    return missing

def render():
    initial_records = json.loads((BACKUP / 'protected-hashes-before.json').read_text(encoding='utf-8-sig'))
    observed_records = [{'Path': r['Path'], 'Hash': sha(Path(r['Path']))} for r in initial_records]
    concurrent_changes = [r['Path'] for r, now in zip(initial_records, observed_records) if r['Hash'] != now['Hash']]
    expected_change = str(ROOT / 'WpfBae/Exercises/TASK-020-device-combobox/Task020Window.xaml')
    if any(p != expected_change for p in concurrent_changes):
        raise RuntimeError('Unreviewed workspace changes: ' + repr(concurrent_changes))
    (BACKUP / 'protected-hashes-at-render.json').write_text(json.dumps(observed_records, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    protected_count = protected_check(observed_records)
    question_records = json.loads((BACKUP / 'questions-hashes-before.json').read_text(encoding='utf-8-sig'))
    for record in question_records:
        if sha(BACKUP / Path(record['Path']).name) != record['Hash']:
            raise RuntimeError('Original backup changed: ' + record['Path'])
    originals = {Path(r['Path']).name: (BACKUP / Path(r['Path']).name).read_text(encoding='utf-8-sig') for r in question_records if Path(r['Path']).name != 'README.md'}
    if set(originals) != set(BODIES):
        raise RuntimeError('Category mismatch: ' + repr(set(originals) ^ set(BODIES)))
    rows = []
    outputs = {}
    for name, bodies in BODIES.items():
        original = originals[name]
        matches = list(re.finditer(r'^## Q\d+：.+$', original, re.M))
        if len(matches) != len(bodies):
            raise RuntimeError(f'{name}: {len(matches)} original questions, {len(bodies)} rewritten bodies')
        title = original.splitlines()[0]
        parts = [title, '> [返回问答索引](./README.md)\n> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。']
        for i, match in enumerate(matches):
            end = matches[i + 1].start() if i + 1 < len(matches) else len(original)
            old_section = original[match.end():end].lstrip('\n')
            first_paragraph = old_section.split('\n\n')[0]
            meta = '\n'.join(line for line in first_paragraph.splitlines() if re.match(r'^- (状态|日期|关联|来源)：', line))
            meta = meta.replace('[`Reading.cs`](../../Base/Exercises/TASK-001-device-model/Reading.cs)', '[当前 Reading 定义](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)')
            meta = meta.replace('[`Device.cs`](../../Base/Exercises/TASK-001-device-model/Device.cs)', '[当前 Device 定义](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)')
            parts.extend([match.group(), meta, readable_prose(bodies[i].strip())])
        related_tasks = sorted(set(re.findall(r'TASK-\d{3}', original)))
        task_links = '、'.join(f'[{task}](../tasks/{task}.md)' for task in related_tasks if (ROOT / f'docx/tasks/{task}.md').exists())
        parts.extend(['## 历史证据与掌握范围', readable_prose(EVIDENCE[name])])
        if task_links:
            parts.append('相关任务卡与验收来源：' + task_links + '。')
        parts.append(f'完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/{name})。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。')
        sources = []
        seen = set()
        for label, url in re.findall(r'\[([^\]]+)\]\((https?://[^)]+)\)', original):
            if url not in seen:
                seen.add(url)
                sources.append(f'- [{label}]({url})')
        extra_sources = {
            'csharp-properties.md': [('Microsoft：init 属性初始化', 'https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init')],
            'csharp-dictionary.md': [('Microsoft：TryGetValue 的返回值与 out 参数', 'https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.trygetvalue?view=net-10.0')],
            'csharp-lambda-linq.md': [('Microsoft：Where 与延迟查询', 'https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.where?view=net-10.0')],
        }
        for label, url in extra_sources.get(name, []):
            if url not in seen:
                seen.add(url)
                sources.append(f'- [{label}]({url})')
        if sources:
            parts.extend(['## 官方依据', '\n'.join(sources)])
        result = '\n\n'.join(p for p in parts if p) + '\n'
        if len(re.findall(r'^```', result, re.M)) % 2:
            raise RuntimeError(name + ': unclosed code fence')
        before_headings = [m.group() for m in matches]
        after_headings = re.findall(r'^## Q\d+：.+$', result, re.M)
        if before_headings != after_headings or metadata(original) != metadata(result):
            raise RuntimeError(name + ': headings or states changed')
        required_followups = re.findall(r'^### .*(?:追问|补充|理解确认)：.+$', original, re.M)
        absent = [h for h in required_followups if h not in result]
        if absent:
            raise RuntimeError(name + ': missing followup headings ' + repr(absent))
        missing = local_links(result, QUESTIONS)
        if missing:
            raise RuntimeError(name + ': broken links ' + repr(missing))
        outputs[name] = result
        rows.append({'file': name, 'question_count': len(matches), 'titles_preserved': True, 'states': metadata(result), 'followups_preserved': len(required_followups), 'local_links_valid': True, 'code_fences_balanced': True})
    old_index = (BACKUP / 'README.md').read_text(encoding='utf-8-sig')
    table_lines = [line for line in old_index.splitlines() if line.startswith('|')]
    for row in rows:
        table_row = next(line for line in table_lines if f'./{row["file"]}' in line)
        columns = [part.strip() for part in table_row.split('|')]
        review_count = sum(s.startswith('待复习') for s in row['states'])
        mastered_count = sum(s.startswith('已掌握') for s in row['states'])
        if [int(columns[-3]), int(columns[-2])] != [review_count, mastered_count]:
            raise RuntimeError(row['file'] + ': index statistics mismatch')
    index = '''# 问答库

> [返回进度](../learning.md) ｜ [当前教学规则](../../AGENTS.md)

这里保留你的原问题、Q 编号及追问。回答先说原练习里的具体对象和现象，再给少量代码、逐处解释含义，最后说明可观察结果；历史验收与掌握边界放到分类文末。

2026-10-13 已重构全部 22 个分类文件。统计沿用重构前记录；待复习不变成新考试，已掌握只指原文有证据的应用范围。源码、任务卡、DoD 和验收资料未改，本轮未运行教学示例或重新验收。

'''
    index += '\n'.join(table_lines)
    index += '''

## 阅读与运行说明

先打开与你的问题对应的分类，再读该 Q。同一个主题的追问放在该问题里；不必先读历史日志或名词表。代码中自定义类/变量与 .NET/WPF 类型会就地说明，完整普通写法先于简写。

旧任务的修改和界面现象是当时练习的说明，不要求重做已经通关的作品。当前 App.xaml 启动 TASK-020；直接运行 WpfBae 项目会打开当前任务，不能把它当成任意历史窗口的运行结果。需要复查旧任务时按对应任务卡读取真实入口，不在本次整理中切换入口。

未运行的代码效果均是说明或预期；编辑器提示、设计预览、项目编译与实际运行证据分别看待。没有因本次文字重写更改问答或任务状态。

## 原稿与历史记录

[重构备份说明](../questions-backups/before-plain-rewrite-2026-10-13/manifest.md)列出原稿与核对记录。原文件名、Q 标题和当前链接保留；旧问答索引中的逐次日志完整保存在[原索引](../questions-backups/before-plain-rewrite-2026-10-13/README.md)，不再放在当前阅读入口前面。

已失效的独立 Device.cs/Reading.cs 链接改为当前 Task001Exercise.cs。历史问答中 WpfPractice 是当时拟用名称，当前真实项目为 WpfBae；仅修正文档表述，没有创建第二套项目或搬移作品。
'''
    if local_links(index, QUESTIONS):
        raise RuntimeError('Index has broken local links')
    for name, result in outputs.items():
        (QUESTIONS / name).write_text(result, encoding='utf-8', newline='\n')
    (QUESTIONS / 'README.md').write_text(index, encoding='utf-8', newline='\n')
    protected_check(observed_records)
    verification = {'date': '2026-10-13', 'categories': len(rows), 'questions': sum(r['question_count'] for r in rows), 'protected_files_unchanged_during_document_write': protected_count, 'workspace_changes_observed_before_document_write': concurrent_changes, 'runtime_tests': 'not run; documentation-only', 'files': rows}
    (BACKUP / 'verification.json').write_text(json.dumps(verification, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({k:v for k,v in verification.items() if k != 'files'}, ensure_ascii=False))

if __name__ == '__main__':
    render()
