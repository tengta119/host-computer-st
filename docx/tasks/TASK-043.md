# TASK-043：跳过无效开头并恢复取帧

- 阶段：第三阶段：设备通信（串口准备）。
- 状态：✅ 已通关
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
- 通关日期：2026-10-19。
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
2026-10-19根据反馈改写TODO 2a：99 88 AA 01先读到99，删掉99后列表成为88 AA 01，再读[0]就是88，再删掉88后读[0]就是AA；AA 01只有2个字节，所以留下等待。
如果原本只有99 88，删完两项就没有第一个字节了，应停止读取；这里的“每次检查0”是每次看现在的第一个，而不是固定反复看原来的99。
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
| TODO 2：在同一个循环里检查第一个字节，非AA则丢弃，遇到AA则等待或取帧。 | 实践2、3 / DoD1、2。 | 前导字节清掉，半帧留下，连续合法帧全部输出。 |
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

开启时ProcessBuffer为空，骨架只显示输入、累计缓冲和帧数0；当前学员已将不足6等待判断移到取帧前、非AA删除之后，七组16个检查点与输入保护检查全部通过，详见下方最终复查。
完成首步后，场景一应出现“当前首字节：99”，场景七空缓冲应出现“缓冲为空”；这一步不会删除99或产生完整帧，后续再完成TODO 2，不能把首字节已显示当作恢复取帧通过。
必要讲解见[字节问答Q5](../questions/csharp-bytes.md#q5task-043-怎样跳过无效开头又保留合法半帧)。

完成后说“检查TASK-043”即可，不要求额外抄写运行报告或口试。

## 验收标准（DoD）

- [x] 非AA前导字节及两帧之间的非AA字节被逐个清理，后续完整A/B及时且按顺序各输出一次，缓冲剩余内容正确，输入数组保持。
- [x] 不足6的纯无效输入99 88也能清空；77 AA 01先只留下AA 01，补齐后正确输出A，不清掉合法半帧。
- [x] 空缓冲及数量0不越界、不新增帧，完整C中的内部AA原样保留；不同分段方式下合法帧结果一致，不死循环或重复输出。

## 验收记录与掌握边界

### 2026-10-19最终复查：三个必需项全部通过

学员自主将Count < 6时break的判断移到非AA删除分支之后、取帧if之前，非AA删除、合法半帧等待和完整帧消费都在同一个while内完成；本轮教练未修改学员源码、入口或注释。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-043-frame-resync\Verification\review_runtime.py review-05-final-2026-10-19.json`，脚本中构建0警告/0错误，原入口七组运行正常退出0，16个缓冲/累计帧数检查点全部符合，完整帧序列全部正确，无超时或重复输出。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-043-frame-resync\Verification\verify_inputs.py`，教练在临时目录编译诊断程序并调用同一个已构建Task043Exercise的ReceiveChunk，6组实际调用全部通过输入数组不变、共享列表未替换、剩余缓冲/帧数正确的检查，有效数量外EE被排除，零数量后可继续处理新输入；临时诊断未替换学习项目入口，不计学员自主编写测试的能力。
完整命令、输出、源码快照及哈希见[最终七组证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-05-final-2026-10-19.json)与[输入保护证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/input-preservation-final-2026-10-19.json)，源码、入口及原DLL在诊断前后均未改变。

| 必需项 | 最终结论 | 真实结果 |
| :--- | :--- | :--- |
| DoD1：前导/帧间清理、完整帧及时输出、剩余正确、输入保持。 | 通过。 | 场景四当次输出A/B、剩0；场景六第二段先取A、清掉99，只剩AA、数量1、帧数1，补齐后B输出且剩0；6组输入保护诊断全过。 |
| DoD2：短无效输入清空与合法半帧保留。 | 通过。 | 99 88不足6也清空；77 AA 01先清77留下AA 01，补4字节后A只输出一次、剩0。 |
| DoD3：空/零数量、内部AA、分段一致、无死循环或重复。 | 通过。 | 七组运行结束，所有零数量不新增帧；C含内部AA仍完整原样输出，整段与分段的A/B结果一致，空列表不越界。 |

Q5按学员实际应用的非空检查、RemoveAt逐项清理、continue重新检查、合法半帧break等待和取帧后继续处理范围标为已掌握；样例、观察方法、类/字段骨架及ExtractOneFrame由教练提供，不扩大为独立设计解析器、校验、真实串口或新WPF联动已经完成。
TASK-043状态同步✅ 已通关，相关循环错题复查通过；依据计划下一步明确字段含义和换算，只创建[TASK-044：按明确规则解析完整帧中的读数](./TASK-044.md)一张⚪ 未开始卡片，不准备源码或切换入口，第三阶段仍🟡 进行中。

### 2026-10-19第四版复查：等待判断放在取帧后，残留短无效开头

实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-043-frame-resync\Verification\review_runtime.py review-04-2026-10-19.json`，脚本中构建0警告/0错误，七组正常退出0、没有超时，16个检查点15符合、1不符合，最终完整帧序列全部正确。
完整输出、源码快照和哈希见[第四版复查证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-04-2026-10-19.json)，本轮源码与入口前后哈希一致，教练未代改实现。

| 必需项 | 最新结论 | 真实结果 |
| :--- | :--- | :--- |
| DoD1：前导/帧间清理并及时恢复A/B，输入保持。 | 未通过。 | 场景四当次输出A/B、缓冲0已通过；场景六第二段后残留99 AA两项，应清掉99只留AA一项，输入数组保持尚未作独立运行诊断。 |
| DoD2：短无效输入清空与合法半帧保留。 | 通过。 | 场景一99 88清空；场景三先保留AA 01，下一段补齐后输出A一次、剩0。 |
| DoD3：空/零数量、内部AA、分段一致且不死循环。 | 通过。 | 七组正常结束、零数量不新增帧、C原样输出，场景四/六最终都是A/B各一次且无重复，空缓冲安全；场景六中间残留错误归DoD1，不重复扩大为最终帧结果不一致。 |

唯一不符合的检查点是场景六第二段：取走A的6个字节后剩99 AA，Count此时为2，写在ExtractOneFrame之后的Count < 6马上触发break，循环没有再检查最前面的99，所以99没有及时删除。
当前只需移动已有代码，不另加一套循环：把ProcessBuffer中第79至82行的Count < 6判断整体移到第75行取帧if之前，仍位于非AA删除分支之后。
这样取完A后会回到循环开头，先删掉99，再检查剩下AA只有1字节，才停止并保留；不能仅根据取帧后剩余数量少于6就立刻等待，因为剩余开头也可能是要丢弃的99。
修改后先跑场景六，第二段结束应只剩AA、数量1、帧数1，第三段结束输出B、剩0、帧数2；随后核对全部七组，预期尚未当作修复后的实际结果。
任务仍🟡 进行中，DoD2/3勾选、1未勾选，Q5整体待复习，既有能力保留，不创建下一题。

### 2026-10-19第三版复查：合法半帧缺少停止等待

学员已把数量判断和ExtractOneFrame放进while，场景二完整A能够当次输出一次并清空缓冲，取帧位置已修正；当前缺少AA开头但不足6字节时退出循环的动作。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-043-frame-resync\Verification\review_runtime.py review-03-2026-10-19.json`，脚本中构建0警告/0错误；场景一、二共4个已返回的检查点全部符合，场景三输入77 AA 01后卡住4秒，由教练终止进程，不记为正常退出。
其余12个检查点没有返回数据，表示被阻塞而无法验证，不代表独立运行出了12处错误；完整输出、源码快照和哈希见[第三版复查证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-03-2026-10-19.json)，源码与入口在本次检查前后保持不变，教练未代改实现。

| 必需项 | 最新结论 | 真实结果 |
| :--- | :--- | :--- |
| DoD1：前导/帧间清理并及时恢复A/B，输入保持。 | 无法完整验证。 | 场景二前导清理+A当次输出正确；场景四/六未执行到，帧间B、分段剩余及输入数组保持未完成本版运行验证。 |
| DoD2：短无效输入清空与合法半帧保留。 | 未通过。 | 99 88清空通过；场景三77删除后剩AA 01，方法不返回，后续4字节的调用无法执行。 |
| DoD3：空/零数量、内部AA、分段一致且不死循环。 | 未通过。 | 场景一、二后的零数量安全；场景三发生死循环，内部AA及后续分段场景未执行到。 |

AA 01的第一个字节是AA，所以不会走删除分支；Count为2，所以也不会走Count >= 6的取帧分支，列表没有变化，while会一直检查同样的两个字节。
本次只补一个小改动：在while内部、删除非AA的if结束之后、现有Count >= 6取帧判断之前，增加下面的不足6退出判断。

```csharp
if (_receivedBytes.Count < 6)
{
    break;
}
```

这段放在非AA删除分支之后，是因为前面的continue已经让无效字节回到下一轮，此时能走到这里的开头就是AA；break只退出当前while，列表中的AA 01不会被删除。
退出ProcessBuffer后ReceiveChunk才能继续显示剩余AA 01并返回Run，下一次ReceiveChunk才能追加剩余4字节，再次进入ProcessBuffer取完整A。
运行方式沿用上面的dotnet run命令，首个预期是场景三先显示缓冲2和AA 01，随后补齐输出A、缓冲0，然后继续跑完其余场景；这些是修复后待验证的结果，未代写到源码。
第二版曾验证的半帧等待和C原样保留证据继续保留为历史，本版DoD2因新的半帧卡住而取消勾选，不撤销TASK-042或其他已验证能力；三个DoD均未勾选，TASK-043仍🟡 进行中，Q5待复习，不创建下一题。

### 2026-10-19审查期间源码更新后的复跑：只取一帧就返回

首次运行之后、整理记录时检测到学员新增break，已按新源码重新构建和运行，首次死循环记录保留为历史，不用于描述当前代码。
实际执行[复跑脚本](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review_runtime.py)，命令为`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-043-frame-resync\Verification\review_runtime.py review-02-2026-10-19.json`；脚本中的实际构建0警告/0错误，七组运行正常退出0、未超时，16个缓冲/累计帧数检查点14符合、2不符合，完整帧最终顺序正确。
完整输出、源码快照和运行前后哈希见[更新后复跑证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-02-2026-10-19.json)，复跑期间源码与入口保持不变，教练未修改学员实现。

| 必需项 | 最新结论 | 真实结果 |
| :--- | :--- | :--- |
| DoD1：前导/帧间清理并及时恢复A/B，输入保持。 | 未通过。 | 场景二前导+A正确；场景四一次A、99、B只输出A，剩99和完整B共7字节，B延后到零数量调用；输入数组保持未作独立运行诊断。 |
| DoD2：短无效输入清空与合法半帧保留。 | 通过。 | 99 88清空；77 AA 01先只留下AA 01，随后有效4字节补齐并输出A，缓冲0。 |
| DoD3：空/零数量、内部AA、分段一致且不死循环。 | 未通过。 | 空缓冲安全、C原样、最终帧顺序正确且无死循环；但场景四零数量调用把帧数从1增至2，说明上一调用未处理完；场景六第二段后残留99 AA两项，预期应清掉99只留AA。 |

本次两个不符合的检查点分别是场景四首次输入后剩7/帧数1（预期剩0/帧数2），以及场景六第二段后剩99 AA两项（预期只剩AA一项）。
当前break会在遇到任何AA时离开while，然后循环外的ExtractOneFrame只执行一次，取完A后不会回到while清理99和检查B。
首个修改仍在ProcessBuffer：把遇AA后的数量判断和ExtractOneFrame放到现有while里，只有AA开头但不足6时才break等待，取完一帧后继续下一轮检查，不要遇AA就直接break。
改完先运行场景四，应在第一次输入后输出A/B两帧并剩0，紧接数量0时帧数保持2，再检查场景六第二段后只剩AA。
非AA逐项清理、合法半帧保留、内部AA原样和空缓冲安全已有运行证据，保留这些局部能力；Q5整体仍待复习，任务仍🟡 进行中，DoD1/3未勾选，不创建下一题。

### 2026-10-19首次审查：遇到AA后循环无法继续

实际构建退出0，0警告/0错误；运行已构建的DeviceCommunication.dll，场景一清空99 88，缓冲0、帧数0，随后零数量仍为0且安全返回。
场景二输入99 88和完整A后没有输出完整帧，4秒仍未返回，由教练终止进程，不能记为正常运行退出0；场景三至七没有执行到。
证据见[首次审查结果](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-01-2026-10-19.json)，源码与入口运行前后SHA-256一致，本轮未代改实现。

| 必需项 | 结论 | 已有证据与缺口 |
| :--- | :--- | :--- |
| DoD1：清理前导/帧间字节并恢复完整帧，输入保持。 | 未通过。 | 场景二遇AA卡住，完整A未输出；帧间B与输入数组保持本轮无法运行验证。 |
| DoD2：短无效输入清空与合法半帧保留。 | 未通过。 | 99 88清空已通过；AA半帧分支缺少停止等待，场景三未执行到。 |
| DoD3：零数量、内部AA、分段一致且无死循环。 | 未通过。 | 场景一清空后零数量安全；场景二已发生死循环，内部AA与不同分段未执行到。 |

卡住的位置是ProcessBuffer的while：删掉99、88后，列表开头成为AA，`_receivedBytes[0] != 0xAA`为false，循环里没有删除、取帧或退出动作，Count始终为6，所以一直重新判断同一个AA。
后面的ExtractOneFrame位于while结束的大括号外，必须先退出while才能执行，而当前列表以AA开头时不会退出。
首个修改是把遇到AA后的处理放进现有while内部，与非AA删除使用同一个循环：少于6字节时停止这次循环并保留列表，至少6字节时调用ExtractOneFrame，然后继续检查剩余内容。
break在这里表示退出当前while，它不会删除保留的半帧；调用ExtractOneFrame则会取走6个字节，使下一次循环检查新的剩余内容。
修改后先运行场景二，应立即输出A一次、缓冲0，再检查场景三先保留AA 01、补齐后输出A，以及其余场景；这是预期，尚未作为修复后的运行结果。
已验证的非空保护、逐个RemoveAt(0)及continue清理短无效输入予以保留，Q5整体仍待复习，TASK-042的既有能力不受影响；三个DoD保持未勾选，任务仍🟡 进行中，不创建下一题。

### 开启与讲解修正的历史记录

- 2026-10-19拆分修正：学员指出TODO 2a与2b重复，教练此前把同一段循环拆得过碎，已将原2a/2b/2c合并为TODO 2，在同一个循环里说明非AA丢弃、遇到AA等待或取帧，不要求分别写两套循环；仅修改教练注释与文档，保留学员代码，状态和DoD不变，不记学员错题。
- 2026-10-19讲解反馈：学员指出教练TODO 2a用语难懂，已将该注释改成“列表里还有字节时读第一个，删除后重新检查，空了停止”，并以99→88→AA的列表变化展开；只修改教练注释与说明，保留学员已有空判断代码，未运行验收、未勾选DoD，状态保持🟡 进行中，不记新错题。
- 2026-10-19已准备独立骨架、七组输入、单帧提取辅助与实际入口，新增非空判断/首字节检查/跳过/等待仍留给学员，三个DoD未勾选。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0，0警告/0错误。
- 实际执行`dotnet run --project D:\host-computer-st\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --no-build`，退出0，七组共16次输入的缓冲数量与内容全部符合未实现处理时的累计预期，帧数均为0。
- [骨架运行证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/startup-2026-10-19.json)保存真实命令、输出及四项准备检查，全部通过；仅验证样例、调用和入口，首字节判断/无效跳过/等待尚未由学员完成，不计DoD通过。
- TASK-042在合法对齐输入下的完整帧处理继续保留，不推定已经掌握噪声恢复、校验、字段解析或真实串口。
