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
- 状态：待复习。
- 来源：TASK-040开启时的必要讲解，TASK-039的byte[]、Length和下标遍历已验证，有效数量尚无学员自主实践证据。
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
这是首步完成后的预期；教练实际运行的未实现骨架当前只显示原问候语和任务标题，不把预期记为学员已运行。
后续只把Run里的bytesRead改为3时，数组Length仍为8，本次范围改为下标0～2；改为0时，本次没有任何字节行，数组也不需要被清零或缩短。
本题的有效数量只表示本次数据所在的位置范围，不等于完整数据包，也不说明校验或业务数据已经正确。

### 当前记录与掌握边界

- 样例数组、模拟数量、类、方法签名与调用由教练准备，输出和按有效数量遍历仍待学员完成，三个DoD未勾选。
- TASK-039的Q1已掌握状态与已验证范围保留；本题Q2待复习，不把人工数量视为已经调用真实串口读取，也不增加组包、协议或后台接收能力。
