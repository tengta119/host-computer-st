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
- 状态：待复习。
- 来源：TASK-041开启时的必要讲解；Q1字节表示与Q2有效范围遍历已验证，持续保留分批字节尚无学员自主证据。
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

运行后先看“开始时”下面的0；由于追加TODO 2还没有实现，其他三个检查点此时也都会显示0，这只是首步结果。
后续补完有效追加和内容输出，再按任务卡观察数量2/5/6和实际字节顺序，00仍要保留，EE/FF等本次无效位置仍要排除。
最后在TODO 3验证本次数量0不改变列表，_receivedBytes.Clear()主动移除已保存的全部字节后Count变0，再重复三批验证恢复；Clear也是.NET列表提供的方法，不会自动重新执行追加调用。

### 当前记录与掌握边界

- 样例、共享字段、类、方法签名和三批调用由教练提供，Count/内容输出、有效追加、零数量与清空重来仍待学员完成，三个DoD未勾选。
- 仅验证教练骨架可以构建运行，Q3待复习；Q1/Q2原已掌握范围保留，不把同一列表已存在视为学员已自主实现持续缓冲、组包或真实串口。
