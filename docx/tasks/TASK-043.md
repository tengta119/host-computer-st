# TASK-043：跳过无效开头并恢复取帧

- 阶段：第三阶段：设备通信（串口准备）。
- 状态：🟡 进行中
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
- 计划来源：[学习计划](../plan.md)「四、第三阶段：设备通信」中遇到无效开头后清理无效数据、继续寻找后续完整帧。
- 选择依据：TASK-042已验证边界对齐的合法帧及其分段提取，接下来处理合法帧前面或两帧之间出现非AA字节的情况。
- 项目：复用[DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj)，不新增依赖。
- 难度/预计时间：待补充，不预设时长。

## 场景与本题明确规则

此前缓冲从AA开始，本题人工输入可能先出现99 88，然后才是完整帧AA 01 03 00 64 5F。
99、88在这里不符合一帧开头必须为AA的约定，应该逐个丢弃，再处理后面的合法帧，不能把前6个字节直接当成一帧。

本题延续TASK-042的教学规则，并明确新增的跳过规则：

| 对象或情况 | 本题约定 |
| :--- | :--- |
| 合法帧 | 固定6字节，第一个为AA，其余5个原样保留，无长度字段或校验。 |
| 注入的无效字节 | 只出现在帧之前或两帧之间，取值不是AA。 |
| 当前第一字节不是AA | 丢弃这一字节，再检查新的第一字节。 |
| 当前第一字节是AA但不足6字节 | 保留全部剩余部分，等待后续输入。 |
| 当前第一字节是AA且至少6字节 | 按已验证规则取出6字节，再继续检查剩余缓冲。 |
| 缓冲为空 | 停止处理，不读取下标0。 |

本题没有校验，不能用这些规则判断一个假AA后面的数据是否来自真实设备；本题只注入非AA的前导/帧间无效字节，假开头、坏字段或校验失败在后续明确协议后学习。

## 开始前先解释

_receivedBytes[0]读取当前列表第一个字节，只有Count大于0时这个位置才存在。
比较_receivedBytes[0] != 0xAA，是在问当前第一个字节是否不等于AA，!=是C#不等于的比较写法。
例如列表为99 88 AA 01时，先移除99，88变为下标0，再移除88，AA才成为新的下标0；此时只剩AA 01，应继续保留，不能因为不足6就把它清掉。

```csharp
_receivedBytes.RemoveAt(0);
```

RemoveAt是.NET列表的方法，0表示只删除当前下标0这一项，后面的字节向前移动；它与已用过的RemoveRange(0, 6)删除完整一帧的用途不同。
当无效字节总共只有99 88两个、还不足6时，也应能清掉它们，因此不能只在Count >= 6时才检查无效开头。
一旦开始按合法帧复制6个字节，里面即使还有AA，也属于本帧内容，不能遇到AA就重新切开。
教练已把TASK-042验证过的六字节复制、RemoveRange移除和PrintFrame打印放在ExtractOneFrame方法里，名字由教练自行命名，并非.NET提供的API。
ProcessBuffer是本题要实现的普通处理方法，需要先确认当前首字节为AA且Count至少为6，再调用ExtractOneFrame()取出一帧，随后继续检查剩余内容；这个辅助方法本身不负责跳过无效字节或判断是否收齐。
循环本次应围绕“列表还有字节”展开，而不只围绕“够6字节”展开，因为99 88两个字节也需要清掉，AA 01两个字节则要留下。

本题可以用C#的continue在丢弃一个无效字节后回到while条件，再判断列表是否还非空；它会跳过本轮后续语句，所以这一轮不会继续拿已经变化的列表按完整帧处理。
break用于退出当前while循环，例如首字节已经是AA但不足6时，停止继续取帧并保留这个半帧；它不清空列表。
return用于结束这次ProcessBuffer方法调用并回到ReceiveChunk，ReceiveChunk后面的PrintBufferState仍会执行；它不结束整个程序或禁止下一段数据再次调用ProcessBuffer。
这些控制方式的具体选择留给学员，不要求使用未解释的简写；核心是每次删除或取出后重新检查，遇到合法半帧就等待。

## 实践顺序与产出

1. 在ProcessBuffer的TODO 1处先只观察缓冲是否为空以及第一个字节，在99 88的输入后看到首字节99。
2. 增加无效开头处理，每次只丢一个非AA字节，再重新检查，直到为空或遇到AA。
3. 遇到AA后复用TASK-042的等待和完整帧消费，让合法半帧留下、完整帧及时取完。
4. 对照前导无效字节、两帧之间无效字节、只剩无效字节及分段到达，验证后续合法帧可以恢复输出。

使用以下合法教学帧和人工输入：

```text
帧A：AA 01 03 00 64 5F
帧B：AA 02 03 00 65 60
帧C：AA 03 AA 00 66 61
```

帧C第三个字节的AA属于帧内部普通数据，用来核对不会被错误切开，不代表新增设备编号或温度字段。

| 输入场景 | 应观察到的结果 |
| :--- | :--- |
| 99 88加完整A | 只输出A一次，缓冲为空。 |
| 只输入99 88 | 没有完整帧，缓冲为空，不等待凑够6个无效字节。 |
| 先输入77 AA 01，再输入03 00 64 5F | 先只保留AA 01，补齐后输出A一次。 |
| 一次输入A、99、B | 按顺序输出A/B各一次，缓冲为空。 |
| 输入完整C | 原样输出完整6字节C，内部AA不触发重新切分。 |
| 已处理完成后输入数量0 | 不输出新帧，不越界读取空列表。 |

每组从主动清空后的缓冲开始，组内持续保留同一个列表，有效数量处理和输入数组保护沿用已验证范围。

## 已准备的目录与入口

用户于2026-10-19明确开启TASK-043，已创建独立骨架并切换Program.cs，旧TASK-039至042成果保留。

| 位置 | 当前事实与用途 |
| :--- | :--- |
| 专属目录Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/ | 已创建，任务代码集中在此目录，未搬动旧作品。 |
| [Task043Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Task043Exercise.cs) | 已准备共享缓冲、已验证的有效追加和单帧辅助、七组样例与观察，ProcessBuffer新增判断/跳过/等待留TODO。 |
| [Program.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Program.cs) | 共享入口必要例外，仅将当前练习类连接改为Task043Exercise，保留原Hello, World!输出。 |

Task043Exercise是教练新建的练习类，Program.cs用new Task043Exercise()创建本次对象，由exercise.Run()依次执行七组输入；共享列表_receivedBytes仍由同一个练习对象保存。
ReceiveChunk先把本段有效字节追加到共享列表，再调用ProcessBuffer，最后显示缓冲和本组帧数，关系沿用TASK-042。
StartScenario只在不同场景之间清空与重置计数，每组内部保留当前缓冲；七组样例和观察调用由教练提供，不计学员新增自主实现。

| 当前TODO | 对应实践与DoD | 完成后的可观察结果 |
| :--- | :--- | :--- |
| TODO 1：在安全的非空条件下观察首字节。 | 实践1 / DoD1、3。 | 首字节99可见，空列表不访问下标0。 |
| TODO 2a/2b/2c：非空循环、逐个丢弃无效开头、保留并处理合法帧。 | 实践2、3 / DoD1、2。 | 前导字节清掉，半帧留下，连续合法帧全部输出。 |
| TODO 3：对照只剩无效字节、内部AA和零数量。 | 实践4 / DoD2、3。 | 不足6的无效内容可清理，帧内部AA原样保留，空缓冲安全。 |

首个动作只在ProcessBuffer的TODO 1处添加以下代码，观察空与非空两种情况，不一次要求完成整个恢复循环。

```csharp
if (_receivedBytes.Count == 0)
{
    Console.WriteLine("缓冲为空");
    return;
}
Console.WriteLine("当前首字节：" + _receivedBytes[0].ToString("X2"));
```

==是C#相等比较，Count为0时执行第一组语句并通过return结束本次方法，后面的下标0读取因此不会发生；非空时读取实际第一个字节并按已学的X2显示。
从工作区根目录D:\host-computer-st复用控制台运行方式，当前以下命令已运行TASK-043。

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj
```

当前ProcessBuffer仍为空，骨架只显示输入、累计缓冲和帧数0；完成首步后，场景一应出现“当前首字节：99”，场景七空缓冲应出现“缓冲为空”。
这一步不会删除99或产生完整帧，后续再完成TODO 2a/2b/2c，不能把首字节已显示当作恢复取帧通过。
必要讲解见[字节问答Q5](../questions/csharp-bytes.md#q5task-043-怎样跳过无效开头又保留合法半帧)。

完成后说“检查TASK-043”即可，不要求额外抄写运行报告或口试。

## 验收标准（DoD）

- [ ] 非AA前导字节及两帧之间的非AA字节被逐个清理，后续完整A/B及时且按顺序各输出一次，缓冲剩余内容正确，输入数组保持。
- [ ] 不足6的纯无效输入99 88也能清空；77 AA 01先只留下AA 01，补齐后正确输出A，不清掉合法半帧。
- [ ] 空缓冲及数量0不越界、不新增帧，完整C中的内部AA原样保留；不同分段方式下合法帧结果一致，不死循环或重复输出。

## 验收记录与掌握边界

- 2026-10-19已准备独立骨架、七组输入、单帧提取辅助与实际入口，新增非空判断/首字节检查/跳过/等待仍留给学员，三个DoD未勾选。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0，0警告/0错误。
- 实际执行`dotnet run --project D:\host-computer-st\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --no-build`，退出0，七组共16次输入的缓冲数量与内容全部符合未实现处理时的累计预期，帧数均为0。
- [骨架运行证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/startup-2026-10-19.json)保存真实命令、输出及四项准备检查，全部通过；仅验证样例、调用和入口，首字节判断/无效跳过/等待尚未由学员完成，不计DoD通过。
- TASK-042在合法对齐输入下的完整帧处理继续保留，不推定已经掌握噪声恢复、校验、字段解析或真实串口。
