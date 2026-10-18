# C# 字节与十六进制

> [返回问答索引](./README.md)

## Q1：TASK-039 为什么写0x64却输出100，怎样看到64？

- 日期：2026-10-18。
- 状态：待复习。
- 来源：TASK-039开启时的必要讲解，尚无本题学员自主运行证据。
- 对应文件：[Task039Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-039-byte-basics/Task039Exercise.cs)。

### 先让一个具体字节出现在屏幕上

本题先观察设备样例AA 01 03 00 64 5F中的一个字节64，不急着猜它表示什么设备读数。
练习文件当前提供byte sample = 0和下一行默认输出，首步只把0改为0x64，运行后最后一行预期从0变成100。

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

- 本轮先讲单字节与显示格式，首个可观察动作仅修改TODO 1a，后续数组操作按任务卡逐步进行。
- 入口类、对象创建、Run方法与默认输出由教练提供，不计学员独立设计；byte、0x、X2与byte[]均未因讲解或骨架存在而标已掌握。
- 当前数组只保存人工样例，不包含串口读取、有效长度、接收缓冲、组包、协议解释或WPF联动的实践证据。
