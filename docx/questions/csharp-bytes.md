# C# 字节与十六进制

> [返回问答索引](./README.md)

## Q1：TASK-039 为什么写0x64却输出100，怎样看到64？

- 日期：2026-10-18。
- 状态：已掌握。
- 来源：TASK-039开启讲解与2026-10-19实践审查，掌握仅限文末已验证范围。
- 对应文件：[Task039Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-039-byte-basics/Task039Exercise.cs)。

### 先让一个具体字节出现在屏幕上

本题先观察设备样例AA 01 03 00 64 5F中的一个字节64，不急着猜它表示什么设备读数。
开启时练习文件提供byte sample = 0和下一行默认输出，首步把0改为0x64即可从0看到100；学员现已完成这一修改和后续输出。

```csharp
byte sample = 0x64;
Console.WriteLine(sample);
```

byte是C#关键字，表示能保存0～255整数的一个字节，sample是我们自行命名的变量；这一行让sample保存数值100。
0x是C#中十六进制数值的开头标记，64两位按十六进制计算为6×16＋4，也就是十进制100；十六进制的一位从0数到9之后继续用A、B、C、D、E、F表示10～15。[Microsoft Learn：整数类型与数值写法](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types)
Console是.NET提供的控制台类，WriteLine是它的输出方法，Console.WriteLine(sample)读取sample的数值并默认显示成十进制100。
因此写入时用了0x，不代表以后所有输出都会自动使用十六进制。

### 再单独改变显示方式

完成首步后，再在TODO 1b处补这一行：

```csharp
Console.WriteLine(sample.ToString("X2"));
```

ToString是.NET提供的数值转文字方法，这里读取同一个sample并产生文字64，随后WriteLine把这段文字输出到屏幕。
"X2"是.NET规定的格式文字，X要求大写十六进制，2要求至少两位，数值1会显示01，数值0会显示00；这里的2不会把更大的数截断成两位。[Microsoft Learn：十六进制显示格式](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#hexadecimal-format-specifier-x)
sample里保存的数仍是100，100和64只是同一个数的两种显示方式，ToString没有修改sample。
本段代码中的"64"是两位显示文字，而0x64是一个字节的数值，后续做通信时要分清这两者。

### 需要观察多个字节时再使用数组

下面只演示两个元素的完整写法，六字节样例及循环由你在TODO 2完成。

```csharp
byte[] data = new byte[] { 0xAA, 0x01 };
```

byte[]是C#表示字节数组的写法，new byte[]创建本次使用的数组，花括号中按顺序放入两个字节，data是我们自行命名、指向这个数组的变量。
data[0]读取第一个字节170，data[1]读取第二个字节1，data.Length由.NET提供并返回这个数组的元素总数2。[Microsoft Learn：数组与下标](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays)
对完整样例使用普通for循环时，下标从0开始，每次增加1，条件使用小于data.Length，这样六个元素的下标就是0～5。
循环内的data[i]读取当前下标对应的字节，每行分别输出i、data[i]和data[i].ToString("X2")，不要把输出写死成六段文字。
TODO 3中的第五个字节对应下标4，保持前面的单字节sample不变，只把数组中原来的0x64改为0x65。
先运行原始数组，再运行修改后的数组，对照下标4从100/64变为101/65，其他五个元素和Length保持原样。

### 当前记录与掌握边界

- 2026-10-19实际审查通过：同一个sample输出100/64，数组原始样例与修改样例均长度6且按下标顺序输出，只有下标4从100/64变为101/65，证据见[任务卡](../tasks/TASK-039.md)。
- 学员自主应用已验证为本题byte与0x数值、ToString("X2")显示、完整byte[]写法、Length及for下标遍历和第五元素修改，Q1据此标已掌握；原始数组版本由教练隔离复现，不虚构学员历史运行。
- 入口类、对象创建、Run方法与默认输出由教练提供，不计学员独立设计；本次审查未修改学员源码。
- 当前数组只保存人工样例，不包含串口读取、有效长度、接收缓冲、组包、协议解释或WPF联动的实践证据。

## Q2：TASK-040 数组有8个位置，为什么本次只处理前6个？

- 日期：2026-10-19。
- 状态：已掌握。
- 来源：TASK-040开启讲解与2026-10-19实际审查，掌握仅限文末已验证范围。
- 对应文件：[Task040Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-040-valid-bytes/Task040Exercise.cs)。

### 先看这次准备好的具体数组

Run里准备了下面的数组和整数，这些名字由教练自行命名，bytesRead目前人工模拟本次读取数量。

```csharp
byte[] receiveBuffer = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0xEE, 0xFF };
int bytesRead = 6;
PrintReceivedBytes(receiveBuffer, bytesRead);
```

receiveBuffer.Length读取这个数组的位置总数8，bytesRead保存本次被约定为有效的数量6。
数组有8个位置，只代表它能放8个字节，不能据此认定本次8个位置都属于新收到的数据。
本题约定本次数据从下标0开始，因此数量6对应下标0～5，后面的EE和FF不在本次处理范围。
EE和FF只是特意准备的观察值，不是通信中一律无效的数值；同样，前面下标3的00属于本次数据，不能因为数值为0就跳过。

### 调用下面的显示方法时，两个数据怎样传进去

PrintReceivedBytes是教练新建的普通方法名，不是.NET提供的接收API。
调用括号里的第一个位置传入这个8元素数组，第二个位置传入当前数量6；程序接着进入PrintReceivedBytes的花括号。
这个方法的第一个参数byte[] receiveBuffer用来访问传来的同一个数组，第二个参数int bytesRead接到本次传来的整数6。
方法的完整签名以及private、void和练习入口对象的作用见[任务卡](../tasks/TASK-040.md)，入口和调用均已接好，本轮直接编辑方法体中的TODO即可。

### 首步只把两个数显示出来

在PrintReceivedBytes的TODO 1位置添加：

```csharp
Console.WriteLine("数组长度：" + receiveBuffer.Length);
Console.WriteLine("本次有效数量：" + bytesRead);
```

Console.WriteLine是前题已用过的.NET输出方法，这两行预期分别显示数组长度8和本次有效数量6。
这是开启时首步完成后的预期；学员本次已自主用逗号把两个数字输出为8,6，实际审查通过，不要求必须使用相同的说明文字。
后续只把Run里的bytesRead改为3时，数组Length仍为8，本次范围改为下标0～2；改为0时，本次没有任何字节行，数组也不需要被清零或缩短。
本题的有效数量只表示本次数据所在的位置范围，不等于完整数据包，也不说明校验或业务数据已经正确。

### 追问：为什么多条字节记录连在同一行

本次提交循环中的Console.Write输出一条记录后，不会自动移动到下一行，下一次循环就直接接着输出下标和字节，因此出现AA1,1,01等相连的文字。
Console.WriteLine在输出本条记录后追加换行，让下一次循环的输出从新一行开始；教练已按低影响收尾授权把这一处调用改为WriteLine，实际验证六条和三条数据各占一行，学员的i < bytesRead条件保持。

### 当前记录与掌握边界

- 2026-10-19真实入口和同对象/同数组的6→3→0→6诊断通过：长度始终8，有效行数依次6、3、0、6，00保留、EE/FF排除，数组内容不变，三个DoD通关，证据见[任务卡](../tasks/TASK-040.md)。
- Q2按学员自主实现的Length/bytesRead输出与有效数量遍历标已掌握；教练只收尾Write→WriteLine，换行修正不算学员自主掌握，类、数组样例、模拟数量与方法签名仍是骨架提供。
- TASK-039的Q1已掌握范围保留，不把人工数量与诊断调用视为学员已完成真实串口读取，也不增加组包、协议或后台接收能力。

## Q3：TASK-041 怎样把分批收到的字节保留在同一个列表里？

- 日期：2026-10-19。
- 状态：已掌握。
- 来源：TASK-041开启讲解与2026-10-19两次实践审查，掌握限文末已验证范围。
- 对应文件：[Task041Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/Task041Exercise.cs)。

### 第一次收到AA 01，第二次来数据时要留下什么

TASK-040只显示当前传入数组中的有效部分，方法结束后不会帮我们把这些字节另存起来。
TASK-041在Task041Exercise对象里准备了一个列表，第一次把AA 01放进去，第二次把03 00 64接在后面，第三次再放5F。
这份一直保留前面字节、又接收新字节的列表在本题用作接收缓冲区，目前只保存和观察，不判断数据包是否完整。

### 先找到教练准备的这份列表

```csharp
private List<byte> _receivedBytes = new List<byte>();
```

这行位于Task041Exercise类里面、Run方法前面，_receivedBytes是教练命名的字段，开头下划线只是名字的一部分。
List<byte>是.NET提供的可保存多个byte的列表，new List<byte>()建立一份空列表；Program.cs创建本次Task041Exercise对象时，这个字段就指向该对象要使用的列表。
Run和AppendReceivedBytes都在这个练习对象上执行，访问同一个_receivedBytes，因此上一次追加进去的字节可以留下来。
如果把新建空列表放进每次追加的方法体并替换原列表，后面一批就会丢掉前面一批，这不符合本题的持续保留目标。

### 本次数量和累计数量分别看哪里

```csharp
AppendReceivedBytes(firstChunk, 2);
```

firstChunk是教练准备的第一批数组，有4个位置，2只表示本次前两个位置有效；AppendReceivedBytes是教练命名的普通方法，参数关系沿用TASK-040。
方法体里的data指向本次传入数组，bytesRead取得本次有效数量；_receivedBytes是另一个用途的列表，用来留下前面和本次累计的字节。
_receivedBytes.Add(data[0])会读取本次数组第一个字节并放到列表末尾，Add由.NET列表提供；学员需要在TODO 2中按有效数量遍历并完成逐个追加。
_receivedBytes.Count读取列表当前已经保存多少个字节，第一次追加后应是2，第二次后应是5，第三次后应是6。
不要把bytesRead改成累计数去遍历当前数组，本次应该读几个位置和此前总共保留了几个字节是两个不同来源。

### 首步只输出空列表数量

PrintBuffer是教练准备的观察方法，label参数只用于输出“开始时”“第一批后”等检查点标题，不会改变列表。
在它的TODO 1a处添加：

```csharp
Console.WriteLine("当前已保留数量：" + _receivedBytes.Count);
```

开启时的首步是先看“开始时”下面的0，当时未实现追加，所以其他检查点也显示0；本次学员已实现有效追加，实际后三处为2/5/6。
后续补完有效追加和内容输出，再按任务卡观察数量2/5/6和实际字节顺序，00仍要保留，EE/FF等本次无效位置仍要排除。
最后在TODO 3验证本次数量0不改变列表，_receivedBytes.Clear()主动移除已保存的全部字节后Count变0，再重复三批验证恢复；Clear也是.NET列表提供的方法，不会自动重新执行追加调用。

### 当前记录与掌握边界

- 2026-10-19最终复查：学员已补零数量、Clear和重新三批调用，真实输出为初始0、第一轮2/5/6、零数量后6、清空后0、第二轮2/5/6，内容一致，三个DoD通关，证据见[任务卡](../tasks/TASK-041.md)。
- Q3按学员自主Count/内容输出、按bytesRead累计Add、零数量调用、Clear及重新追加的实践范围标已掌握；教练仅补四处清空后/重新三批后的PrintBuffer观察，不把这四处显示协助计为自主掌握。
- 同一列表身份、00保留、EE/FF排除及输入数组保护均已实际验证；首次缺验证调用为历史，未新增错题或机械报告要求。
- 样例、共享字段、类、方法签名和原三批调用由教练提供，Q1/Q2原范围保持，不扩大到缓冲生命周期独立设计、协议组包或真实串口。

## Q4：TASK-042 怎样判断收齐一帧，并把它从缓冲中取出来？

- 日期：2026-10-19。
- 状态：待复习。
- 来源：TASK-042开启必要讲解，Q3的有效累计与清空重来已验证，完整帧提取尚无学员自主实践证据。
- 对应文件：[Task042Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Task042Exercise.cs)。

### 先有明确规则，才知道该等待还是取出

本题新约定一帧固定6字节、开头为AA，后5字节原样保留，没有校验或业务字段解释；输入只包含满足这些规则的合法帧及其分段。
缓冲只有AA 01时Count为2，应留下来等待；再收到03 00 64后Count为5，仍等待；最后收到5F后Count为6，才够取一帧。
6来自本题明确的教学规则，不是因为一次输入刚好有6个字节，也不代表所有设备都固定发送6字节。

### 先把这一帧另存出来，再移除已处理部分

```csharp
byte[] frame = new byte[6];
frame[0] = _receivedBytes[0];
```

new byte[6]是C#新建6位置字节数组的写法，frame是自行命名的局部变量，这6个位置刚创建时都是0。
第二行把缓冲第一个字节复制到frame的第一个位置，缓冲内容仍在原处；完整六字节的复制由学员用循环完成。
教练准备的PrintFrame(frame)只负责打印这份数组和增加显示计数，不替你复制字节，也不替你移除缓冲。

```csharp
_receivedBytes.RemoveRange(0, 6);
```

RemoveRange由.NET列表提供，0表示从下标0开始，6表示移除6项；例如8字节缓冲中包含帧A加上AA 02，复制帧A并移除前6项后，缓冲应只剩AA 02。
这里不使用Clear清掉整个列表，因为后面的AA 02属于下一帧，仍需留给后续到达的数据。

### 一次有两帧时，每取完一帧还要再看剩余数量

12字节里有帧A和帧B，取完第一帧还剩6个字节，应继续取第二帧；8字节里只有一个完整帧加两个字节，取完一帧剩2就应停止。
所以需要C#的while重复检查当前Count是否仍至少为6，而不是只判断一次就结束；每轮移除6项后再检查，数量足够才继续。
[任务卡](../tasks/TASK-042.md)先用remaining=12的最小例子解释while的两次执行与结束，学员再把这个关系用到ProcessBuffer的缓冲提取上。
如果循环一直输出却不移除已处理部分，Count不会减少，就无法通过剩余数量结束本次处理，不能把它当成成功取出多个帧。

### 首步在真实方法里只观察一次判断

教练新建的ReceiveChunk先把本段有效字节追加到共享列表，再调用ProcessBuffer，随后由PrintBufferState输出剩余数量、内容和本组已输出帧数。
在ProcessBuffer的TODO 1位置按任务卡添加if/else：Count小于6时打印继续等待，否则打印已经够一帧；第一段后应看到继续等待和缓冲数量2。
if检查括号里的比较结果后只选择一组语句执行，while则在每一轮之前重新检查；本题先观察if结果，再继续完成重复取帧。
StartScenario只在场景组之间清空和重置显示计数，每组内部持续保留缓冲；这项场景准备由教练提供，不算学员新的自主实现。
当前教练实际运行的是ProcessBuffer尚未实现的骨架，四组输入和有效累计正确，完整帧数均为0；首步判断和后续取帧仍待学员完成，不把这些骨架结果记为DoD通过。

### 当前记录与掌握边界

- 本题只使用合法且边界对齐的教学帧，不处理噪声、错误开头、校验、业务字段或串口；这些没有被现有样例证明。
- 有效追加复用TASK-041的已验证代码，样例、显示计数和打印方法由教练提供；if判断、六字节复制/移除、while提取及分段对照留TODO，Q4待复习、三个DoD未勾选，Q1/Q2/Q3原范围保持。
