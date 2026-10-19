# TASK-041：保留分批收到的有效字节

- 阶段：第三阶段：设备通信（串口准备）。
- 状态：✅ 已通关
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
- 通关日期：2026-10-19。
- 计划来源：[学习计划](../plan.md)「四、第三阶段：设备通信」中保留前次尚未处理字节、把新到字节接在后面的接收缓冲。
- 选择依据：TASK-040已验证只处理数组中的有效位置，下一步需要让多次收到的有效部分按原顺序留下来；暂不判断包是否完整。
- 项目：复用[DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj)，不新增依赖。
- 难度/预计时间：待补充，不预设时长。

## 场景与目标

同一段样例分三次到达：第一次AA 01，第二次03 00 64，第三次5F。
每次的数组还可能有本次无效的位置，仍沿用TASK-040的有效数量约定，只把真正属于本次的数据追加到同一份持续保留的列表中。
第一次处理结束后AA 01要留下，第二次接在后面，最后才能观察到AA 01 03 00 64 5F。
这份持续保留收到字节的列表在本题用作接收缓冲区；当前只积累与观察，不把6当成通用包长，不解释字段或校验。

## 开始前先解释

TASK-040每次只显示当前数组，本题还要保存此前的字节，因此需要一个不会在每次处理时重新创建的列表。
List<byte>是.NET提供的可追加字节列表，和之前用过的List<T>属于同一种集合，只是这次里面每个元素都是byte。

```csharp
private List<byte> _receivedBytes = new List<byte>();
```

这行代码已放在Task041Exercise类里面、Run方法前面，_receivedBytes是教练自行命名的字段，指向这个练习对象保存的字节列表；名字开头的下划线只是命名习惯，不是.NET接口或特殊语法。
Program.cs里的new Task041Exercise()创建本次练习对象时，这个字段就指向一份新建的空列表，exercise.Run()在同一个对象上依次处理三批数据。
字段写在方法外面，因此三次AppendReceivedBytes调用访问的是同一份列表，前一次追加的内容可以留到下一次；如果每次追加都新建空列表，就会丢掉此前收到的字节。
Count读取当前已经保存多少字节，Add把一个字节放到列表末尾，Clear清掉当前已保存的全部字节，这三个成员均由.NET的List<byte>提供。
这些名字与数组的Length作用不同：这里观察的是列表当前保存了多少项，不是某次输入数组有多少位置。
AppendReceivedBytes是教练命名的普通追加方法，不是.NET或串口API，签名和三次调用已准备好，方法体里的有效追加留TODO 2。

```csharp
AppendReceivedBytes(firstChunk, 2);
```

firstChunk是教练命名的第一批数组，里面共有4个位置，这行把它和有效数量2传入追加方法，让方法体用data访问本次数组、用bytesRead取得本次数量，沿用TASK-040的参数关系。
要留下第一个字节时，对应操作是_receivedBytes.Add(data[0])，它读取本次数组下标0的字节，并把这个字节放到共享列表末尾；完整有效范围的遍历由学员在TODO 2完成，不把调用次数当成字节数量。
PrintBuffer也是教练新建的普通方法，负责显示当前累计结果；PrintBuffer("开始时")把文字“开始时”传给它的label参数，现成的Console.WriteLine(label)先输出检查点标题，Count和字节内容显示留TODO 1a/1b。
Task041Exercise类和方法签名均由教练准备，List<byte>与Console由.NET提供，private、new和方法参数写法属于C#语法，不把这些自定义名字当作学员理应认识的API。

## 实践顺序与产出

1. 在PrintBuffer的TODO 1a处先只输出共享列表的Count，观察“开始时”为0。
2. 完成一次有效范围内的追加，第一批后观察Count为2、内容为AA 01。
3. 继续处理第二、三批，保留此前内容并按顺序追加，依次观察Count为5和6。
4. 在已有内容后传入有效数量0，观察Count和内容不变；清空共享列表后再处理同样三批，确认重新得到2、5、6及相同顺序。

骨架已准备以下人工输入与三次追加调用，有效数据均从下标0开始，EE/FF仅作为本次无效位置的观察值。

| 批次 | 输入数组 | 本次有效数量 | 处理后应保留的字节 |
| :--- | :--- | :--- | :--- |
| 1 | AA 01 EE FF | 2 | AA 01 |
| 2 | 03 00 64 EE | 3 | AA 01 03 00 64 |
| 3 | 5F EE | 1 | AA 01 03 00 64 5F |

这些都是人工准备的数据，尚无实际串口读取；列表里包含完整样例也不表示已经按设备协议组包或解析成功。
本题只使用合法的有效数量，非法数量、并发和缓冲上限策略在后续实际需要时再展开。

## 已准备的目录与入口

用户于2026-10-19明确开启TASK-041，已实际创建独立骨架，Program.cs已连接本题，TASK-039/040原有成果保留。

| 位置 | 当前事实与用途 |
| :--- | :--- |
| 专属目录Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/ | 已创建，本题文件集中在此目录，未搬动已有作品。 |
| [Task041Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/Task041Exercise.cs) | 学员已完成有效追加、零数量与主动清空重来；教练仅补清空后及第二轮三批后的四处观察输出，全部DoD通过。 |
| [Program.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Program.cs) | 共享入口的必要例外，仅将当前练习类连接改为Task041Exercise，保留Hello, World!输出。 |

| 当前TODO | 对应实践与DoD | 完成后的可观察结果 |
| :--- | :--- | :--- |
| TODO 1a/1b：观察共享列表Count并输出当前保留内容。 | 实践1 / DoD1。 | 开始为空，追加后可以核对数量和每个字节。 |
| TODO 2：只把本次有效位置的字节追加到已有列表。 | 实践2、3 / DoD1、2。 | 依次保留2、5、6个字节，前次内容不丢失、无效尾部不混入。 |
| TODO 3a/3b：补充零数量、清空后重新接收的调用并观察。 | 实践4 / DoD3。 | 零数量不改变列表，清空后重来不残留旧数据。 |

首个动作只在PrintBuffer的TODO 1a处补下面这一行，暂不要求写完追加和所有对照。

```csharp
Console.WriteLine("当前已保留数量：" + _receivedBytes.Count);
```

这行读取本练习对象的共享列表Count，并用前题已用过的Console.WriteLine输出；没有在这里新建列表，也没有改变列表内容。
从工作区根目录D:\host-computer-st复用现有控制台运行方式，当前以下命令已运行TASK-041。

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj
```

开启时未实现骨架只输出原问候语、本题标题和四个检查点标题，首步完成而追加未实现时数量均为0。
当前实现已实际输出开始时0、三批后2/5/6、零数量后6、清空后0及第二轮三批后2/5/6，全部内容与顺序通过，四处收尾观察输出由教练补充。
必要说明见[字节问答Q3](../questions/csharp-bytes.md#q3task-041-怎样把分批收到的字节保留在同一个列表里)。

完成后说“检查TASK-041”即可，不要求额外抄写报告或口试。

## 验收标准（DoD）

- [x] 同一个字节列表处理三批人工输入后，Count依次为2、5、6，内容逐次与表格一致，每次都保留前面已收到的字节。
- [x] 追加依据本次有效数量，00正常保留，EE/FF等无效尾部不混入；不通过写死结果、每次清空或重建列表替代持续追加，输入数组内容保持。
- [x] 已有内容时传入数量0，列表数量和内容均不变；主动清空后重新处理三批，仍得到2、5、6及相同顺序，没有上次残留。

## 验收记录与掌握边界

### 2026-10-19最终复查

学员已自主补充数量0的追加调用和观察、_receivedBytes.Clear()、清空后再按2/3/1处理原三批，清空重来的核心逻辑已完成。
复查开始时实际构建与运行退出0，零数量后仍输出原6个字节；清空后和第二轮三批后缺观察输出，[本次协助前证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/Verification/review-02-before.json)保留提交源码、哈希及真实输出。
依据用户既有低影响收尾授权，教练仅增加四个PrintBuffer调用，分别观察清空后、重新接收第一批后、第二批后和第三批后；没有修改Clear、数组、有效数量、Add循环、零数量调用或其他学员逻辑。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-041-receive-buffer\Verification\review_task041_final.py`，退出0；[最终脚本](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/Verification/review_task041_final.py)与[完整证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/Verification/review-02-final.json)保存在任务独立目录，原首次审查脚本与结果继续保留。

| 验收项 | 本次真实证据 | 最终结论 |
| :--- | :--- | :--- |
| DoD1 | 真实入口第一轮数量2/5/6及各字节与预期一致，按同一列表持续追加。 | 通过。 |
| DoD2 | 有效范围追加保留00、排除EE/FF，诊断确认列表身份一致且输入数组不变。 | 通过。 |
| DoD3 | 真实入口零数量后仍为原6个字节，学员Clear后显示0，第二轮按2/3/1追加后显示2/5/6及相同顺序，没有上轮残留。 | 通过，四处观察输出由教练收尾。 |

最终真实项目和诊断项目均构建0警告/0错误、运行退出0，五项输出、状态与修改范围检查全部通过；差异校验确认本轮只加四个观察调用，Program.cs与项目文件哈希不变，临时诊断项目已自动清理。
三个DoD全部勾选，TASK-041为✅ 已通关；Q3按学员自主Count/内容遍历、有效Add、零数量调用、Clear和重新追加的范围标已掌握，类、共享字段和样例骨架、教练收尾观察不计新的自主设计。
没有观察到新的典型认知错误，不新增错题；按计划只创建下一张[TASK-042：按明确规则取出完整数据帧](./TASK-042.md)，状态⚪ 未开始，未准备源码或切换入口，第三阶段仍进行中。

### 2026-10-19首次审查（历史）

状态仍为🟡 进行中，DoD1/2通过，DoD3尚未完成，未创建TASK-042。
实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-041-receive-buffer\Verification\review_task041.py`，退出0；[审查脚本](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/Verification/review_task041.py)与[运行证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-041-receive-buffer/Verification/review-2026-10-19.json)保存在本题独立目录。
真实项目与临时诊断项目均构建0警告/0错误、运行退出0，三个已执行的输出/状态/源码保护检查全通过，不代表全部DoD通过。

| 验收项 | 证据与缺项 | 结论 |
| :--- | :--- | :--- |
| DoD1 | 原项目真实入口输出开始时0、三批后2/5/6，各检查点字节与表格逐项一致，第三批后为AA 01 03 00 64 5F。 | 通过。 |
| DoD2 | 源码使用i < bytesRead并Add(data[i])；诊断确认同一个列表对象持续保留，00有效、EE/FF不混入，三个输入数组内容均不变。 | 通过。 |
| DoD3 | 教练诊断调用零数量时列表不变，但学员Run里的TODO 3a/3b仍空着，没有零数量观察段，也没有主动清空、观察0、重新三批并观察2/5/6的调用，清空重来未完成验收。 | 未通过，待补验证调用。 |

本轮未修改学员实现、Program.cs或项目文件，三个输入文件的审查前后SHA-256一致；诊断临时项目已清理，教练调用不冒充学员已补充的源码或历史运行。
学员自主Count/内容遍历、按有效数量Add到已有列表并保留前次字节的局部实践已验证，予以保留；类、共享字段及样例调用由教练提供，Q3整体仍待复习，不把清空恢复或协议组包计为已掌握。
未观察到新的典型认知错误，本次只是TODO验证步骤尚未补齐，不新增错题或额外报告要求。

下一步先在Run的TODO 3a补一个小改动：

```csharp
AppendReceivedBytes(firstChunk, 0);
PrintBuffer("零数量后");
```

这两行把已有firstChunk数组和数量0传给现成追加方法，再显示同一列表，预期仍为6个字节且内容不变。
随后在TODO 3b主动调用已有列表的Clear并观察0，再复用前面三组追加与显示调用，预期重新得到2/5/6；补完后按任务入口运行并说“审查TASK-041”即可。

### 开启时的历史记录

- 2026-10-19已准备独立骨架、样例数组与调用、共享列表字段及入口，核心输出、有效追加、零数量/清空恢复仍待学员完成，三个DoD未勾选。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0，0警告/0错误。
- 实际执行`dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --no-build`，退出0，输出Hello, World!、本题标题及“开始时/第一批后/第二批后/第三批后”四个标题，尚无Count或字节输出。
- 这些结果仅验证骨架和入口可运行，不等于追加、累计显示或清空恢复已完成，三个DoD继续未勾选。
- 既有有效范围遍历能力保留，持续接收缓冲尚待实践，不标为已经掌握组包、串口或完整协议。
