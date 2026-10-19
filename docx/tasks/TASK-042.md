# TASK-042：按明确规则取出完整数据帧

- 阶段：第三阶段：设备通信（串口准备）。
- 状态：🟡 进行中
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
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
| [Task042Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Task042Exercise.cs) | 已提供共享缓冲、四组样例及有效追加、输入连接、观察/打印辅助，ProcessBuffer的判断和完整取帧实现留TODO。 |
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

当前ProcessBuffer尚未实现，运行只显示各输入后的累计缓冲和帧数0；首步完成后，场景一第一个输入应先显示继续等待，再看到缓冲剩余数量2和AA 01。
此时后续段仍会累积到5和6，所有组也仍没有完整帧输出，这是判断观察阶段，不能据此认为组包已完成；继续完成TODO 2后才应消费完整部分并保留尾部。
必要讲解见[字节问答Q4](../questions/csharp-bytes.md#q4task-042-怎样判断收齐一帧并把它从缓冲中取出来)。

完成后说“检查TASK-042”即可，不额外要求抄写运行报告或口试。

## 验收标准（DoD）

- [ ] 帧A按2/3/1分段输入，累计不足6字节时全部保留且不输出完整帧，收齐时恰好输出完整帧A一次并从缓冲移除。
- [ ] 一次收到A+B时依次输出两帧且缓冲为空；一次收到A加B前2字节时只输出A、保留AA 02，补齐后输出B，输入数组保持原样。
- [ ] 同一A+B数据整段输入和不同分段输入时，输出的帧数量、字节和顺序完全一致，每帧仅输出一次；数量0不会再次输出已处理的帧。

## 验收记录与掌握边界

- 2026-10-19已准备独立源码、四组输入与实际入口，复用已验证的有效追加，判断、复制/移除/反复提取与对照留给学员，三个DoD未勾选。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0，0警告/0错误。
- 实际执行`dotnet run --project D:\host-computer-st\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --no-build`，退出0；四组共13次输入的缓冲数量与内容逐项一致，当前提取未实现，已输出帧数全部为0。
- [骨架运行证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/startup-2026-10-19.json)保存实际运行命令、输出和四项准备检查，均通过；只说明骨架样例与连接正确，不算判断/复制/移除/重复提取已经完成。
- 本题尚无协议组包实践证据，既有有效范围遍历与分批保留能力继续保留，不扩大为校验、字段解析、串口或新WPF联动已经完成。
