# TASK-040：只处理接收数组中的有效字节

- 阶段：第三阶段：设备通信（串口准备）。
- 状态：✅ 已通关
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
- 通关日期：2026-10-19。
- 计划来源：[学习计划](../plan.md)「四、第三阶段：设备通信」中的数组容量与本次实际读取长度。
- 选择依据：TASK-039已验证字节数组、Length和按下标遍历；下一步需要区分数组里能放多少字节与本次真正收到多少字节，为后续读取和缓冲做准备。
- 项目：复用现有[DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj)，不新增依赖。
- 难度/预计时间：待补充，不预设时长。

## 场景与目标

现在人工准备一个有8个位置的数组，但用一个单独整数说明本次只有前6个位置属于新收到的数据。
最后两个位置放EE和FF用于观察是否错误地多输出，它们在本题被约定为本次无效位置，并不表示这两个数值在所有通信中都无效。
目标是按有效长度观察字节，暂不连接真实串口、不判断完整包，也不解释设备字段。

## 开始前先解释

以下数据已由教练写入练习文件的Run方法，数组和模拟数量属于骨架，输出与遍历留给学员。

```csharp
byte[] receiveBuffer = new byte[] { 0xAA, 0x01, 0x03, 0x00, 0x64, 0x5F, 0xEE, 0xFF };
int bytesRead = 6;
```

receiveBuffer是本题自行命名的字节数组变量，Length为8，表示数组共有8个位置。
bytesRead是本题自行命名的整数变量，人工模拟本次实际读取的数量6，表示本次只处理下标0～5。
数组的Length仍为8，不会因为本次有效数量是6而自动缩短；后续真实读取会给出实际数量，本题还没有调用SerialPort.Read。
以后把bytesRead改为3时，本次只处理下标0～2，数组后面的内容仍然存在。
数据里的00也可以是有效字节，不能用“遇到0就停止”或“跳过所有0”的方式替代有效数量。
本题约定有效数据从下标0开始，bytesRead的测试值在0～Length之间；其他起始位置和非法数量检查留待实际需要时再讲。

教练已准备普通的PrintReceivedBytes方法，名字由教练自行命名，用途是显示传进来的数组中本次有效的部分，并非.NET或串口提供的API。

```csharp
PrintReceivedBytes(receiveBuffer, bytesRead);
```

这行调用写在Run里面，第一个位置传入上面准备的8元素数组，第二个位置传入整数6，程序随后执行下面这个方法的花括号里的代码。

```csharp
private void PrintReceivedBytes(byte[] receiveBuffer, int bytesRead)
{
    // 输出和遍历在这里完成。
}
```

括号里的byte[] receiveBuffer和int bytesRead是C#方法参数写法，让方法体分别用receiveBuffer访问传来的同一个数组、用bytesRead取得本次传来的数量。
private是C#关键字，表示这个方法供Task040Exercise类内部使用，Run可以调用它；void表示它执行操作而不返回一个结果值。
Task040Exercise是教练新建的练习类，Program.cs用new Task040Exercise()创建本次练习对象，exercise指向它，exercise.Run()执行这份练习；这些入口连接已准备好，本次只改任务文件里的TODO。
本题“有效数量”只说明哪些位置属于本次数据，不表示这些字节已经组成完整包或通过协议校验。

## 实践顺序与产出

1. 在PrintReceivedBytes的TODO 1处先只输出receiveBuffer.Length和bytesRead，观察同一次运行同时看到8和6。
2. 在本题显示方法中按bytesRead控制遍历范围，逐行输出有效下标、十进制值和两位大写十六进制，核对六条数据。
3. 保持同一数组不变，把bytesRead改为3，再运行观察只输出AA、01、03对应的三条。
4. 把bytesRead改为0，观察本次没有字节行，再恢复6运行，观察原来的六条恢复。

本题不要求判断完整包，bytesRead为0也不在本题中表示端口断开或数据包结束。

## 已准备的目录与入口

用户于2026-10-19明确要求开启TASK-040，已实际创建独立骨架并将控制台入口切换到本题，TASK-039学员源码保留。

| 位置 | 当前事实与用途 |
| :--- | :--- |
| 专属目录Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-040-valid-bytes/ | 已创建，仅存放本题代码，不搬动TASK-039成果。 |
| [Task040Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-040-valid-bytes/Task040Exercise.cs) | 学员已完成长度/数量输出与按bytesRead遍历，教练仅把循环中的Write改为WriteLine，当前实际验收通过，原TODO注释保留。 |
| [Program.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Program.cs) | 共享入口的必要例外，只把当前练习类连接改为Task040Exercise，保留原Hello, World!输出。 |

| 当前TODO | 对应实践与DoD | 完成后的可观察结果 |
| :--- | :--- | :--- |
| TODO 1：输出数组长度和本次有效数量。 | 实践1 / DoD1。 | 同时看到8与6，清楚区分两个来源。 |
| TODO 2：按有效数量遍历并输出。 | 实践2 / DoD1。 | 下标0～5对应原始六字节，不输出下标6和7。 |
| TODO 3：保持数组不变，分别修改模拟有效数量并对照运行。 | 实践3、4 / DoD2、3。 | 数量3时只输出前三条，数量0时没有字节行，恢复6后输出六条。 |

首个动作只完成PrintReceivedBytes里的TODO 1，不一次要求写完遍历和所有对照。
这里的Console.WriteLine是前题已用过的.NET输出方法，下面两行分别读取数组位置总数和本次模拟数量，让输出能直接对应两个来源。

```csharp
Console.WriteLine("数组长度：" + receiveBuffer.Length);
Console.WriteLine("本次有效数量：" + bytesRead);
```

从工作区根目录D:\host-computer-st运行以下命令，当前入口已经是TASK-040。

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj
```

开启时未完成TODO的骨架只输出Hello, World!和任务标题；本次提交已输出8,6及有效字节行，实际结果见下方验收记录。
补充说明见[字节问答Q2](../questions/csharp-bytes.md#q2task-040-数组有8个位置为什么本次只处理前6个)。

完成后说“检查TASK-040”即可，不额外要求抄写运行报告或口试。

## 验收标准（DoD）

- [x] 同一8元素数组、bytesRead为6时，实际显示数组长度8与有效数量6，按有效数量遍历出下标0～5及AA、01、03、00、64、5F对应的十进制和两位十六进制，不输出后两位置。
- [x] 数组及其中元素保持不变，bytesRead为3时实际只显示前三个有效字节，遍历依据为有效数量，未通过清零、删元素或写死输出实现。
- [x] bytesRead为0时不输出任何字节行、不越界；恢复6后仍正确输出原始六字节，不沿用上次3或0的结果。

## 验收记录与掌握边界

### 2026-10-19实际审查与教练收尾

学员已自主实现receiveBuffer.Length与bytesRead输出，以及for条件i < bytesRead、下标/十进制/X2输出。
首次构建0警告/0错误、运行退出0，但循环内使用Console.Write，六条记录连在同一行，尚未满足DoD1逐行显示要求；[修正前证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-040-valid-bytes/Verification/review-2026-10-19-before.json)保存原始源码、哈希、命令和实际输出。
依据用户既有低影响收尾授权，教练只把该处Console.Write改为Console.WriteLine，没有修改循环条件、数组、模拟数量或其他学员实现，换行协助不计学员自主掌握。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-040-valid-bytes\Verification\review_task040.py`，退出0；[最终证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-040-valid-bytes/Verification/review-2026-10-19-final.json)保存真实入口与临时诊断项目的构建、运行、输出和源码保护结果，[审查脚本](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-040-valid-bytes/Verification/review_task040.py)保存在本题独立目录。

| 验收项 | 实际运行证据 | 最终结论 |
| :--- | :--- | :--- |
| DoD1 | 当前原项目显示8,6，随后六行依次为0,170,AA至5,95,5F，其中3,0,00保留，没有EE/FF。 | 通过，逐行显示由教练收尾。 |
| DoD2 | 临时诊断项目调用学员原方法，同一个练习对象和同一个8元素数组依次传入6、3、0、6，数量3时只有下标0～2三行，数组内容未变。 | 通过。 |
| DoD3 | 同一进程中数量0只显示8,0，没有字节行；随后恢复6输出六行，数组前后逐项相等，不残留前次数量。 | 通过。 |

最终原项目与诊断项目均构建0警告/0错误、运行退出0，五项输出与输入保护检查全通过；诊断入口由教练提供并调用原私有方法，没有改写学员源码来切换数量，也不冒充学员此前已运行这些历史场景。
Program.cs与项目文件哈希不变，源码差异校验确认只有Write→WriteLine这一处格式修改，临时项目已自动清理。
三个DoD全部勾选，TASK-040为✅ 已通关；字节Q2按学员自主输出两种数量、有效范围遍历与保留00的实践范围标已掌握，人工数量仍不等于真实串口读取。
未观察到新的典型认知错误，不新增错题；只创建下一张[TASK-041：保留分批收到的有效字节](./TASK-041.md)，状态⚪ 未开始，未准备新源码或切换入口。

### 开启时的历史记录

- 2026-10-19已创建Task040Exercise.cs并连接实际入口，教练提供样例、数量、方法签名和调用，核心输出、遍历与修改对照尚待学员完成，三个DoD均未勾选。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0，0警告/0错误。
- 实际执行`dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --no-build`，输出Hello, World!和TASK-040：只处理接收数组中的有效字节，未输出8、6或任何字节行，确认运行的是本题未实现骨架。
- 骨架构建和运行不等于DoD通过，TODO 1至3均待学员实践，不把教练提供的数量和数组计为自主掌握。
- TASK-039的字节格式与数组遍历实践继续保留，不能据此将有效读取长度、真实串口、缓冲组包或协议解析标为已掌握。
