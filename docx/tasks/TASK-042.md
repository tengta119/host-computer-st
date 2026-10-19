# TASK-042：按明确规则取出完整数据帧

- 阶段：第三阶段：设备通信（串口准备）。
- 状态：✅ 已通关
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
- 通关日期：2026-10-19。
- 计划来源：[学习计划](../plan.md)「四、第三阶段：设备通信」中的明确协议后组包、半包保留、连续多包逐个取出。
- 选择依据：TASK-041已验证分批有效字节可以保留在同一列表中，接下来需要判断是否已收齐、取出完整部分并保留后面尚未收齐的部分。
- 项目：复用[DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj)，不新增依赖。
- 难度/预计时间：待补充，不预设时长。

## 场景与本题教学规则

缓冲里已有AA 01时继续保留，后面收到03 00 64 5F后，按照本题新明确的规则取出一个完整的6字节数据帧。
“帧”在这里指按本题规则划分出的一整段通信数据，本题只取完整字节，不解释业务数值。
以下规则是为TASK-042新定义的教学约定，不是设备说明书，也不追认此前样例为某个真实协议。

| 规则 | 本题约定 |
| :--- | :--- |
| 一帧从哪里开始 | 第一个字节为AA。 |
| 一帧多少字节 | 固定6字节，包括开头的AA。 |
| 后面5字节怎样处理 | 原样保留，暂不赋予设备编号、温度或其他业务含义。 |
| 长度字段或校验字段 | 本题均不设置，最后的5F或60只是普通字节。 |
| 输入范围 | 仅由符合以上规则的数据帧连接或分段组成，开始和后续帧边界均对齐；噪声、坏帧与重新寻找开头的规则留到后续任务。 |

准备两份不同的合法教学帧供对照：

```text
帧A：AA 01 03 00 64 5F
帧B：AA 02 03 00 65 60
```

本题只因明确约定每帧6字节才按6取出，不能把这个长度推广到所有设备。
现有TASK-041样例仍保留为字节累计练习，不改写其历史验收含义。

## 开始前先解释

缓冲只有2或5个字节时，还不够组成一帧，应继续保留并等待后续输入。
缓冲有6个或更多字节时，可以先复制最前面的6个字节作为一帧；复制完成后再从缓冲中移除这6个，剩余字节继续保留。
例如缓冲有帧A加帧B前两个字节，共8字节，取出帧A后应只剩AA 02，而不是把整个缓冲清空。

```csharp
_receivedBytes.RemoveRange(0, 6);
```

RemoveRange是.NET列表提供的方法，这行从列表下标0开始移除6项，后面的剩余字节自动排到前面，Count相应减少。
教练已提供PrintFrame方法打印传入的完整帧，并记录本组已输出帧数；取帧时需要先准备另一个数组保存这6个字节，之后再移除缓冲里的原位置。

```csharp
byte[] frame = new byte[6];
frame[0] = _receivedBytes[0];
```

byte[]是前题用过的C#字节数组写法，new byte[6]新建6个位置，初始值都是0，frame是本题自行命名的局部变量。
第二行只把缓冲第一个字节复制到新数组第一个位置，不会从缓冲删除这个字节；其余位置也需要按原顺序复制，完整循环留给学员。
PrintFrame(frame)会打印这份复制出来的数组，不负责替你复制或移除缓冲。
每取出一帧后还要再检查剩余Count，如果仍至少为6就继续处理，否则一次收到两帧时只会处理第一帧。
while是C#重复执行代码的语法，下面先用普通整数演示它每次执行前都会重新判断条件。

```csharp
int remaining = 12;
while (remaining >= 6)
{
    Console.WriteLine(remaining);
    remaining = remaining - 6;
}
```

remaining是演示里自行命名的整数变量，第一次为12，条件12 >= 6成立，输出12并减为6；第二次条件6 >= 6成立，输出6并减为0；第三次条件0 >= 6不成立，循环结束。
本题需要反复检查的是_receivedBytes.Count，移除一帧后Count会减少6，再检查就能决定继续取下一帧还是停下来等待，不能在数量没有减少时一直重复打印同一帧。
PrintFrame、ProcessBuffer、ReceiveChunk、PrintBufferState和StartScenario都是教练命名的普通方法，List<byte>、RemoveRange和Console由.NET提供，while、if和new byte[6]属于C#语法，不把这些自定义方法名当成学员理应知道的API。

## 实践顺序与产出

1. 在ProcessBuffer的TODO 1处先只判断当前数量是否小于6，并显示等待或够一帧，在第一段只有2个字节时看到继续等待，后面的PrintBufferState已提供当前数量显示。
2. 依次输入帧A的2、3、1个字节，前两次不输出完整帧，第三次恰好输出帧A一次，缓冲剩0。
3. 一次输入帧A加帧B共12字节，按顺序输出两帧，缓冲剩0。
4. 输入帧A加帧B前2字节共8字节，先输出帧A并保留AA 02，再输入帧B剩余4字节，输出帧B一次。
5. 对照整段、分段和连续输入的结果，确认完整帧不会重复出现，本次数量0不会再次输出已取出的帧。

每组对照从同一个练习对象的已清空缓冲开始，使用已掌握的有效数量追加；每组内持续使用同一列表，不能靠清空丢掉后面的半帧。

## 已准备的目录与入口

用户于2026-10-19明确开启TASK-042，已创建独立骨架并将Program.cs连接本题，旧039/040/041成果保留。

| 位置 | 当前事实与用途 |
| :--- | :--- |
| 专属目录Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/ | 已创建，本题源码集中在此目录，未搬动旧作品。 |
| [Task042Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Task042Exercise.cs) | 学员已自主把重复处理条件修正为Count >= 6，四组实际运行与全部DoD通过，方法自调用结构保留，本轮源码未改。 |
| [Program.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Program.cs) | 共享入口的必要例外，仅将当前练习类改为Task042Exercise，保留Hello, World!输出。 |

Program.cs用new Task042Exercise()创建本次练习对象，exercise指向它，exercise.Run()顺序运行四组场景；_receivedBytes字段由该对象保存，与TASK-041的同一列表关系一致。
ReceiveChunk在每段数据到达时先调用已验证的AppendReceivedBytes，再调用待实现的ProcessBuffer，最后由PrintBufferState显示缓冲剩余数量、内容和本组帧数。
StartScenario只在组与组之间清空并重置显示计数，每组内部保留未取出的字节；_printedFrameCount和PrintFrame的计数/打印由教练提供，不计新的学员自主实现。

| 当前TODO | 对应实践与DoD | 完成后的可观察结果 |
| :--- | :--- | :--- |
| TODO 1：观察是否已经够一帧。 | 实践1、2 / DoD1。 | 2或5字节时等待，6字节时可以取出。 |
| TODO 2a/2b/2c：复制并移除一帧，重复处理全部完整帧，保留尾部不足部分。 | 实践2、3、4 / DoD1、2。 | 单帧与连续两帧正确输出，8字节输入后只留后两字节。 |
| TODO 3：对照不同分段与零数量输入。 | 实践5 / DoD3。 | 同样原始字节产生相同顺序的帧，已消费的字节不重复输出。 |

首个动作只在ProcessBuffer的TODO 1处添加以下判断，不一次要求完成整个提取循环。

```csharp
if (_receivedBytes.Count < 6)
{
    Console.WriteLine("继续等待");
}
else
{
    Console.WriteLine("已经够一帧");
}
```

if读取括号里的比较结果，当前Count小于6时执行第一组输出，否则执行else里的输出，这一步仅观察，不改变缓冲或生成完整帧。
从工作区根目录D:\host-computer-st复用现有控制台运行方式，当前以下命令已运行TASK-042。

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj
```

开启时ProcessBuffer未实现、首次审查存在恰好6的边界遗漏；当前学员已自主修正为>=6，连续两帧当次取完、零数量不新增帧，全部验收通过，见最终复查。
必要讲解见[字节问答Q4](../questions/csharp-bytes.md#q4task-042-怎样判断收齐一帧并把它从缓冲中取出来)。

完成后说“检查TASK-042”即可，不额外要求抄写运行报告或口试。

## 验收标准（DoD）

- [x] 帧A按2/3/1分段输入，累计不足6字节时全部保留且不输出完整帧，收齐时恰好输出完整帧A一次并从缓冲移除。
- [x] 一次收到A+B时依次输出两帧且缓冲为空；一次收到A加B前2字节时只输出A、保留AA 02，补齐后输出B，输入数组保持原样。
- [x] 同一A+B数据整段输入和不同分段输入时，输出的帧数量、字节和顺序完全一致，每帧仅输出一次；数量0不会再次输出已处理的帧。

## 验收记录与掌握边界

### 2026-10-19最终复查

学员已自主将while条件从Count > 6改为Count >= 6，本轮仅运行验收与同步记录，没有修改学员源码、入口或项目文件。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-042-fixed-frame\Verification\review_task042_final.py`，退出0；[最终脚本](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/review_task042_final.py)与[完整运行证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/review-02-final.json)保存本轮四次实际命令、输出、逐输入状态和哈希，首次失败证据保留。

| 验收项 | 本次证据 | 结论 |
| :--- | :--- | :--- |
| DoD1 | A分2/3/1到达时前两段不输出，收齐后恰好输出A一次、缓冲0。 | 通过。 |
| DoD2 | 12字节A+B输入当次输出两帧、缓冲0；8/4输入先输出A并留AA 02，补齐后输出B，原输入数组不变。 | 通过。 |
| DoD3 | A+B整段、8/4、2/3/1/6分段结果均为A/B各一次且顺序一致，后续零数量均不新增或补输出帧。 | 通过。 |

两个项目均构建0警告/0错误、运行退出0，八项检查全部通过；诊断状态由此前12字节后剩6/帧数1，变为剩0/帧数2，零数量后仍为剩0/帧数2。
三个输入文件审查前后SHA-256一致，同一列表身份、数组保护和有效数据范围保持；临时诊断项目已自动清理。
三个DoD全部勾选，TASK-042为✅ 已通关；Q4按本题不足一帧等待、逐项复制、RemoveRange消费、相等边界与分段一致的自主应用范围标已掌握，打印/计数及样例骨架仍不计自主设计。
方法自调用的现有结构没有在本轮改写，先前结构建议不作为新增验收门槛，不将本题四组结果扩大为任意噪声或其他协议的可靠性证据。
错题本对应边界条目标记已复查通过；按计划只创建下一张[TASK-043：跳过无效开头并恢复取帧](./TASK-043.md)，状态⚪ 未开始，未准备源码或切换入口，第三阶段仍进行中。

### 2026-10-19首次审查（历史）

状态仍为🟡 进行中，DoD1通过、DoD2/3未通过，不创建TASK-043。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-042-fixed-frame\Verification\review_task042.py`，审查脚本退出0，表示已完成检查，不表示功能全部通过；[脚本](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/review_task042.py)与[运行证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/review-2026-10-19.json)保存在本题独立目录。
真实入口与临时诊断项目均构建0警告/0错误、运行退出0；八项输出/状态/保护检查六项通过、两项未通过，三个输入文件哈希不变，本轮未修改学员源码、入口或项目文件。

| 验收项 | 实际证据 | 结论 |
| :--- | :--- | :--- |
| DoD1 | 帧A分2/3/1输入时，前两段留下AA 01及AA 01 03 00 64，不输出帧；第三段输出完整A一次、缓冲剩0。 | 通过。 |
| DoD2 | A加B前2字节的8/4输入正确保留AA 02并补齐B，输入数组不变；但一次输入12字节A+B后仅输出A，缓冲剩完整B的6字节、帧数1。 | 未通过，连续帧未在本次全部取出。 |
| DoD3 | A+B按2/3/1/6分段时收齐即输出两帧；整段12字节时只输出一帧，直到后续数量0调用才补输出B，同样有效数据输入完成时结果不一致。 | 未通过。 |

没有发现已经输出的A/B再次重复输出，四组最终都能得到各自预期的帧内容；当前问题是第二帧被延后到零数量调用处理，不能把它写成重复帧错误。
原因可直接对应源码：ProcessBuffer先复制、移除和打印第一帧，12字节因此剩6；后面的while使用Count > 6，6 > 6为false，完整B被留下，下一次调用才从方法开头继续处理。
首个修复只修改这条边界条件：

```csharp
while (_receivedBytes.Count >= 6)
```

本题6个字节已经是一整帧，因此剩余数量恰好6也应继续处理；少于6时才等待，修改后再实际运行四组，预期12字节一次输入当场输出A/B、缓冲0，后续零数量不新增帧。
这是待学员修改与验证的核心条件，本轮未代改，也没有把预期记为修复后的运行结果。
当前while内部再次调用ProcessBuffer属于同一个方法再次进入自己；把复制/移除/打印组织在while的循环体内可以更直接地表达每轮取一帧，这是结构建议，不新增本次DoD门槛，先完成恰好6的边界修复。
不足6返回、new byte[6]逐项复制、RemoveRange只移除已取部分、PrintFrame连接与尾部保留的局部自主能力已经验证，予以保留；Q4整体待复习，原Q1/Q2/Q3不受影响。
已将本次实际边界错误增量记入错题本，不推测学员编写动机，不要求机械运行报告。

### 开启时的历史记录

- 2026-10-19已准备独立源码、四组输入与实际入口，复用已验证的有效追加，判断、复制/移除/反复提取与对照留给学员，三个DoD未勾选。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0，0警告/0错误。
- 实际执行`dotnet run --project D:\host-computer-st\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --no-build`，退出0；四组共13次输入的缓冲数量与内容逐项一致，当前提取未实现，已输出帧数全部为0。
- [骨架运行证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/startup-2026-10-19.json)保存实际运行命令、输出和四项准备检查，均通过；只说明骨架样例与连接正确，不算判断/复制/移除/重复提取已经完成。
- 本题尚无协议组包实践证据，既有有效范围遍历与分批保留能力继续保留，不扩大为校验、字段解析、串口或新WPF联动已经完成。
