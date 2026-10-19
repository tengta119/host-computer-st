# TASK-044：打通串口收发并解析设备读数

- 阶段：第三阶段：设备通信（串口实践）。
- 状态：✅ 已通关
- 创建日期：2026-10-19。
- 开始日期：2026-10-19。
- 通关日期：2026-10-19。
- 本任务最终验收口径：按用户“源码没问题就通过”的明确要求，以静态源码审查通关，未构建、运行或测试，不声称实际端口往返已验证。
- 调整日期：2026-10-19，用户要求提高任务粒度，在串口实践中解决解析问题。
- 计划来源：[学习计划](../plan.md)「四、第三阶段：设备通信」中的端口打开、发送、接收、组包解析与关闭重开。
- 选择依据：TASK-039至043已经验证字节、有效数量和缓冲恢复，不再单独安排字段读取小题，直接在串口收发流程里使用这些能力，必要概念随使用讲解。
- 项目：继续使用[DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj)，串口依赖在明确开启并核对环境后再做最小增量连接。
- 难度/预计时间：待补充，不预设时长。

## 当前已开启，先准备本机端口对

用户于2026-10-19明确开启TASK-044，并确认“没有实体设备，用本机虚拟串口对”；已实际准备独立源码、模拟设备和运行说明，Program.cs已切换本题，旧TASK-039至043作品保留。
原独立字段练习卡完整保存在[调整前记录](./history/TASK-044-before-serial-2026-10-19.txt)，其内容作为历史，不再作为当前任务或新增前置关卡。
实际专属目录为Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/，学员已补打开/发送/读取/关闭并修正温度浮点换算，教练仅补原始值输出和关闭文案，最终静态审查通过，缓冲算法与模拟设备的提供边界保持。

## 这次做出什么

完成一个能打开端口、发送字节、接收对端数据、处理完整帧并显示设备编号/读数、关闭后再次打开的控制台串口程序。
一张任务卡内分几次运行观察，按完整收发功能验收，不再把一个if、字段读取或乘除换算拆成独立任务；实际需要的新API和规则仍先解释用途，再动手。
每一步围绕一个运行结果推进，遇到半帧、字节顺序或有效数量问题就在这条收发链路里处理，已有缓冲算法由教练复用并核对，不要求重写已通关题目。

## 开启时确认对端，再决定具体端口

开启时通过系统串口列举、设备信息和SERIALCOMM登记只看到蓝牙COM5、COM6、COM7，未观察到互连端口对；之后用户截图已显示HHD工具和COM1/COM2创建配置页，这些是各自时点的证据，不把开启时检查当作本轮环境现状。
用户已选无实体设备的本机虚拟串口方案，已说明拟上位机COM1、模拟端COM2；本轮仅静态审查，没有确认创建结果或检查端口，具体操作见[运行说明](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/RUN-GUIDE.md)。
有真实设备时先核对接口、参数和协议文档，按实际协议收发；没有合适设备时用虚拟串口对和可控对端完成真实COM端口收发，工具选择和安装按届时条件处理。
以下六字节规则供可控教学对端使用；若采用真实设备且协议不同，开启时依据其文档调整字段和输入样例，不把教学字节直接当作真实设备命令。
打开、发送和接收的新方法在实际代码中分别解释，不一次堆叠线程、事件和界面绑定；本题先控制台打通，WPF线程更新和完整异常恢复继续按计划后续学习。

## 可控教学对端的读数约定

以下是本题新明确的教学约定，不是猜测真实设备协议，也不追认TASK-039至043里的样例已经含有这些业务含义。

| 数组位置 | 本题含义 |
| :--- | :--- |
| frame[0] | 固定AA，表示这一帧的开头。 |
| frame[1] | 设备编号，按数值显示，例如01表示设备1，02表示设备2。 |
| frame[2] | 本题样例固定03，不作为长度字段，不新增命令处理。 |
| frame[3] | 原始数值中较大的一部分，先发送这个字节。 |
| frame[4] | 原始数值中较小的一部分，后发送这个字节。 |
| frame[5] | 本题暂不解释，原样保留，不当作已实现的校验字段。 |

原始数值按frame[3]乘256再加frame[4]得到，范围为0至65535；本题约定每10个数值单位表示1摄氏度，所以温度等于原始数值除以10.0。
例如00 FA对应0乘256加250等于250，温度为25.0℃；01 04对应1乘256加4等于260，温度为26.0℃。
“先放较大部分”描述本题两个字节的先后顺序，开启时再结合这两个具体样例解释通常所说的高字节、低字节和大端顺序，不要求先学位运算或新的转换API。
使用10.0是为了保留小数，例如01 00得到256后应为25.6℃，不能按整数相除只得到25。

## 开启后的实践顺序

1. 确认可用对端和参数，准备串口骨架，首个运行结果是成功打开选定端口并能关闭，失败时看得到原因并能重新尝试。
2. 从程序发送约定字节，让对端核对实际收到的内容，再由对端发送数据，让程序打印本次实际收到的有效字节和数量。
3. 把收到的有效字节交给已验证的缓冲处理，现场用完整帧、分段帧和连续帧观察结果，不要求接收回调的次数等于发送次数。
4. 取到完整帧后读取设备编号、合并两个数值字节并换算温度，相关字段和计算随此步骤讲解，核对四组读数。
5. 关闭后重新打开，再收发一次，确认旧半帧不混入新连接、没有重复输出，并核对一次打开失败后重试。

| 完整输入帧 | 设备编号 | 原始数值 | 温度 |
| :--- | :---: | :---: | :---: |
| AA 01 03 00 FA 5F | 1 | 250 | 25.0℃ |
| AA 02 03 01 04 60 | 2 | 260 | 26.0℃ |
| AA 01 03 01 00 5F | 1 | 256 | 25.6℃ |
| AA 01 03 00 00 5F | 1 | 0 | 0.0℃ |

可控教学对端按上述规则发送完整帧、分段帧和连续帧，也可复用TASK-043的非AA前导/帧间输入核对恢复；不新增负数、可变长度或校验要求，不把完整回调线程与WPF同步混入本次验收。
短小的人工输入对照可作为现场排查手段，不再单独建任务；程序需实际通过串口接收并走到读数输出，只有直接调用解析方法的结果不能代替收发验收。

## 验收标准（DoD）

2026-10-19用户明确将本任务验收调整为只检查源码、源码无问题即通关，以下勾选表示静态审查满足，不表示实际串口运行通过；原运行验收卡保存在[调整前完整记录](./history/TASK-044-before-static-acceptance-2026-10-19.txt)。

- [x] 源码中的端口选择/配置、打开、原始字节Write、Read实际数量传递和关闭相互连接正确，模拟端入口与数据规则对应，未把数组容量或显示文本当作有效接收字节。
- [x] 复用缓冲的非AA清理、合法半帧保留和连续取帧代码顺序正确，字段按约定读取，int原始值与double温度分开且除以10.0，输出编号/原始值/一位小数温度，输入数组未被解析代码改写。
- [x] 源码包含关闭后清理旧半帧、再次打开、操作异常反馈后继续菜单及退出Dispose路径，本题没有重复事件订阅，未发现任务范围内其他阻塞源码问题；本项不等同于已验证断线/超时或实际重连。

## 当前证据与边界

- 学员已补五处实现并自主修正整数除法，最终按用户授权的静态口径通过，三个静态DoD已勾选，状态✅ 已通关；教练仅补两处显示收尾，未构建/运行/测试，实际串口往返和环境状态仍无本轮运行证据。
- TASK-043的组包恢复能力继续保留，不推定已经掌握双字节数值换算、温度字段、校验、串口或新WPF联动。
- 本题开启时先解释必要的新规则，再准备实际骨架和第一个可观察动作，不设动手前口试。
- 教学方式改为围绕完整通信功能推进，必要基础在使用处补充，不再将已验证能力归零或反复拆成独立小题；用户节奏反馈不记为学员错题。

## 已准备的文件、TODO与运行方式

| 实际文件 | 已准备的内容与边界 |
| :--- | :--- |
| [Task044Exercise.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Task044Exercise.cs) | 学员的上位机菜单和端口配置已准备，串口调用与读数计算留五个TODO，已接好旧缓冲逻辑。 |
| [Task044Simulator.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Task044Simulator.cs) | 教练提供的完整对端工具，读请求和手动发整帧/分段/连续帧，不计学员新增实现。 |
| [RUN-GUIDE.md](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/RUN-GUIDE.md) | 端口准备、对象身份、API用途、两个终端的命令、第一处改动和完整联调顺序，运行前先按此核对实际端口。 |
| [Program.cs](../../Stage03-DeviceCommunication/DeviceCommunication/Program.cs) | 共享入口必要例外，保留Hello, World!，创建Task044Exercise并传入args选择列举/上位机/模拟器模式。 |
| [DeviceCommunication.csproj](../../Stage03-DeviceCommunication/DeviceCommunication/DeviceCommunication.csproj) | 共享配置必要例外，增量加入Microsoft提供的System.IO.Ports 10.0.0，未引入新框架。 |

| TODO | 对应实践/DoD | 运行后观察什么 |
| :--- | :--- | :--- |
| 1：OpenPort打开选定端口并显示状态。 | 实践1 / DoD1、3。 | 已创建的实际端口可打开，错误可见，成功IsOpen为True。 |
| 2：SendRequest把55 01原始数组写出。 | 实践2 / DoD1。 | 模拟端r读到55 01，不是文本字符。 |
| 3：ReceiveAvailable读取并把实际数量传给HandleReceivedBytes。 | 实践2、3 / DoD1、2。 | 只追加有效字节，整帧取出，半帧留下，不重写缓冲算法。 |
| 4：ClosePort关闭并清空旧半帧。 | 实践5 / DoD3。 | c后o可重新打开，旧半帧不混入本次完整帧。 |
| 5：PrintReading读取设备编号、原始值并换算温度。 | 实践4 / DoD2。 | 四组编号/整数/一位小数温度正确，有效00保留。 |

第一步是准备互连虚拟串口对并核对实际端口名，首个代码动作在OpenPort的TODO 1调用_port.Open()并打印_port.IsOpen；SerialPort是.NET类，_port是教练命名的操作对象，设置PortName只选择端口，Open才真正打开，不创建Windows串口。
先运行下面的实际列举命令，再按运行说明用两个终端各开一端：

```powershell
dotnet run --project .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj -- --ports
```

本题为手动r读取的同步控制台练习，打开、发送、接收和关闭均由同一菜单执行，后台事件和WPF线程联动在后续实际需要时学习；模拟端手动发送，不虚构收到请求就会自动回复。
2026-10-19用户进一步明确“源码没问题就通过”，本任务已按静态口径验收并通关，不构建、运行或测试，不将源码通过等同于实际通信验证；后续验收继续按用户届时要求执行。

## 2026-10-19最终静态复查通关

PrintReading已由学员改为int rawValue = frame[3] * 256 + frame[4]和double temperature = rawValue / 10.0，小数丢失问题已修正；其余Open、Write、Read返回值传递、缓冲处理、Close后Clear与菜单重试路径静态未发现阻塞问题。
教练按既有收尾授权只修改两行输出：补原始值、温度单位和标识，将“以关闭”改为“已关闭”，保留学员核心实现，不把显示协助计作自主掌握。
修改后仅重读源码确认，未构建、运行、打开端口或执行测试；详细依据见[最终静态审查](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Verification/static-review-final-2026-10-19.md)，原失败审查和开启骨架证据保留为历史。
本任务三个静态DoD通过、状态✅ 已通关，整数除法错题标为源码修正完成；串口Q1/Q2仍待复习，不把静态验收变成真实收发已经自主验证的能力。
按计划只创建下一张[TASK-045：把串口读数接入WPF监控界面](./TASK-045.md)⚪ 未开始卡，未创建WPF骨架或切换入口，第三阶段仍🟡 进行中。

## 2026-10-19静态源码审查：温度换算需保留小数

本次仅阅读源文件、入口与项目配置，未构建、运行、测试或修改源码；完整范围和结论见[静态审查记录](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Verification/static-review-2026-10-19.md)。
Open/Write/Read实际数量传递/Close后Clear与旧缓冲的连接未发现明显源码问题；PrintReading第168行把两个字节合成的整数除以整数10并存为int，按C#规则会截掉小数，例如256对应25而非25.6，后续F1不能恢复已丢失的小数，同时缺少原始整数单独输出。
首个修改只在PrintReading：先保留int原始值，再用double温度保存原始值除以10.0的结果，分别显示设备编号、原始值与一位小数温度；此处未代改，未把语义推导写成实际运行结果。
DoD1/3仅能确认源码连接、运行部分无法验证，DoD2因上述换算错误未通过，三个均不勾选，任务保持🟡 进行中；Q2继续待复习，既有能力保持，不创建下一题。

## 2026-10-19骨架实际检查

- 实际执行`dotnet add .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj package System.IO.Ports --version 10.0.0`，退出0，依赖已加入并还原。
- 实际执行`dotnet build .\Stage03-DeviceCommunication\DeviceCommunication\DeviceCommunication.csproj --nologo`，退出0、0警告/0错误。
- 实际执行`python .\Stage03-DeviceCommunication\DeviceCommunication\Exercises\TASK-044-serial-roundtrip\Verification\verify_startup.py`，退出0，六项骨架检查全部通过：列举端口、未选端口安全退出、菜单保留真实TODO状态、模拟端不存在端口时报错、教练复用缓冲六组诊断、旧五个任务源码哈希不变。
- [实际骨架证据](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Verification/startup-2026-10-19.json)含命令、输出与诊断源码，[旧源码基线](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Verification/baseline-2026-10-19.json)保留检查结果；这些只验证准备工作，不等于TODO完成或DoD通过，虚拟端口对尚未创建，串口往返未验证。
