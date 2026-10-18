# TASK-039：认识字节与十六进制数据

- 阶段：第三阶段：设备通信（串口准备）。
- 状态：🟡 进行中
- 创建日期：2026-10-18。
- 开始日期：2026-10-18。
- 计划来源：[学习计划](../plan.md)「四、第三阶段：设备通信」中的字节、十六进制、字节数组及原始数据观察。
- 选择依据：WPF阶段实践已通关，但尚无字节表示和原始通信数据的自主实践证据，因此先在新控制台项目中看清每个字节和输出结果，再逐步学习有效读取长度、缓冲与解析。
- 项目：复用已创建的[DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj)，当前目标框架为net10.0。
- 难度/预计时间：待补充，不预设时长。

## 场景与目标

后续设备可能送来`AA 01 03 00 64 5F`，接收代码首先面对的是六个字节。
本题用人工准备的数据观察“同一个字节用十进制和十六进制分别显示是什么”，并按原顺序输出整个字节数组，暂不解释其中的设备编号、温度或校验含义。
本题在控制台项目完成，后续界面联动使用新建的[DeviceCommunication.Wpf.csproj](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/DeviceCommunication.Wpf.csproj)。

## 开始前先解释

当前骨架里的sample暂时是占位值0，下面的0x64是完成TODO 1后的示例，尚未替你写入练习文件。

```csharp
byte sample = 0x64;
Console.WriteLine(sample);
Console.WriteLine(sample.ToString("X2"));
```

`byte`是C#表示0～255整数的关键字，`sample`是示例自己命名的变量，`0x64`是C#按十六进制写出的数值，等于十进制100。
十六进制用0～9和A～F表示一位，A～F依次代表十进制10～15；0x64的右边一位是4，左边一位代表6个16，因此6×16＋4＝100。
`Console.WriteLine`由.NET提供，第二行把这个数默认显示为十进制，因此会看到`100`。
第三行调用这个数的`ToString`方法，把数值转换为文字；`"X2"`是.NET的数字格式约定，要求用大写十六进制并至少显示两位，不足时在前面补0，因此这里会看到`64`，而数值1会显示为`01`。
转换显示格式没有改变sample中保存的数值，十进制100和十六进制64是同一个数的两种写法。

再看一个只含两个字节的数组示例：

```csharp
byte[] data = new byte[] { 0xAA, 0x01 };
```

`byte[]`表示用于存放多个byte值的数组，`new byte[] { ... }`使用C#的完整写法创建数组，`data`是示例自己命名的数组变量。
`data[0]`取得第一个字节，`data[1]`取得第二个字节，下标从0开始；`data.Length`读取数组的元素总数，这个小例子中是2。
数组中保存的是字节值，屏幕上显示的空格和`AA`等文字用于方便人阅读，不要把文字`"AA"`直接当成字节值`0xAA`。
本题暂时让数组的所有元素都属于样例数据，数组容量与一次读取的有效长度如何区分，留到后续实际需要时再展开。

## 实践顺序与产出

1. 在已准备的练习文件中先只把`byte sample = 0;`改为`byte sample = 0x64;`，保留下一行默认输出，运行观察100，再补十六进制输出并观察64。
2. 用字节数组保存完整样例`AA 01 03 00 64 5F`，输出数组长度，再用普通循环逐个输出下标、十进制值和两位十六进制值，保持原有顺序。
3. 只把第五个字节从`0x64`改为`0x65`，重新运行，对照这一行的十进制输出由100变为101、十六进制由64变为65，数组长度与其余五个字节保持原样。

第二步完成后的预期对应关系如下，这张表用于核对输出，并非已经运行的结果：

| 数组下标 | 十进制值 | 两位十六进制显示 |
| :--- | :--- | :--- |
| 0 | 170 | AA |
| 1 | 1 | 01 |
| 2 | 3 | 03 |
| 3 | 0 | 00 |
| 4 | 100 | 64 |
| 5 | 95 | 5F |

## 已准备的目录与入口

用户于2026-10-18明确要求开始TASK-039，已实际准备下表中的独立骨架，核心实现留给学员。

| 项目或文件 | 当前事实与用途 |
| :--- | :--- |
| [DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj) | 已存在，复用当前控制台项目，本题不需要新增包。 |
| [Program.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Program.cs) | 保留原Hello, World!输出，增量创建Task039Exercise对象并调用Run方法；这是共享入口的必要例外。 |
| 专属目录`Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-039-byte-basics/` | 已实际创建，任务专属源码集中在此目录。 |
| [Task039Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-039-byte-basics/Task039Exercise.cs) | 已实际创建，包含普通类和Run方法、单字节占位值及TODO 1a至3，不添加第二份顶层语句。 |
| [新WPF项目](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/DeviceCommunication.Wpf.csproj) | 已存在，本题只在控制台观察数据，后续WPF实践使用此新项目。 |

Program.cs中的Task039Exercise是教练新建并命名的练习类，new Task039Exercise()创建本次练习对象，exercise指向这个对象，exercise.Run()执行该对象的练习方法。
using DeviceCommunication.Exercises.Task039让Program.cs可以直接写Task039Exercise这个类名，namespace里的这串名字由教练按项目和任务编号自行命名。
入口和默认输出由教练提供，不计学员自主实现；你本轮只需编辑任务专属文件中的TODO位置。

| 当前TODO | 对应实践与DoD | 完成后的可观察结果 |
| :--- | :--- | :--- |
| TODO 1a/1b：修改单字节占位值，再补十六进制输出。 | 实践1 / DoD1。 | 同一个0x64显示为100和64。 |
| TODO 2a/2b/2c：准备六字节数组，读取Length并按顺序输出各元素。 | 实践2 / DoD2。 | 长度为6，下标0～5及每个字节与上表一致。 |
| TODO 3：只修改第五个字节，再次运行对照。 | 实践3 / DoD3。 | 只有下标4的数据变为101和65，长度与其他字节不变。 |

首个动作只完成TODO 1a，把sample的0改为0x64，下一行Console.WriteLine(sample)已经准备好，不要求一次完成数组遍历。
从工作区根目录D:\host-computer-st运行：

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj
```

当前未修改占位值时，输出依次是原Hello, World!、本题标题和0；完成首步后的预期是最后一行变为100。
以后对照TODO 3时，保持单独的sample仍为0x64，只修改数组第五个元素，先核对原始版本，再观察修改版本，不要求另抄运行报告。
更详细的显示关系和后续数组提示见[字节入门Q1](../questions/csharp-bytes.md)。

提交源码后说“检查TASK-039”即可，教练按代码与实际运行核对，不要求另外抄写运行报告或完成口试。

## 验收标准（DoD）

- [ ] 单个字节0x64在实际运行中分别显示为十进制100和十六进制64，使用的是同一个字节值。
- [ ] 六字节样例用byte数组保存并按实际元素遍历，长度为6，下标和十进制/两位十六进制输出均与预期表一致。
- [ ] 将第五个字节改为0x65后，实际运行仅对应数据由100/64变为101/65，数组长度与其他五个字节保持原样。

## 验收记录与掌握边界

- 2026-10-18已准备任务专属源码和入口，教练准备的默认输出与占位值不计学员自主实践，三个DoD均未勾选。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0，0警告/0错误。
- 实际执行`dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --no-build`，退出0，依次输出Hello, World!、TASK-039：认识字节与十六进制数据、0，确认真实入口执行的是本题占位骨架。
- 上述验证只说明教练骨架可编译运行，没有替学员完成sample=0x64、十六进制显示、六字节遍历或修改前后的对照，三个DoD继续未勾选。
- 未使用SerialPort、真实串口、后台接收、组包或协议解析，后续逐步安排，不把本题样例当作完整设备协议。
- 此TASK-039是当前新建的字节入门题，先前已删除的同编号TCP题仍为历史，不恢复其目标、代码或验收结论。
