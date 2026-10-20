# TASK-049：打通TCP设备模拟器与控制台上位机

- 阶段：第四阶段：TCP/UDP（TCP控制与消息边界）。
- 状态：🟡 进行中
- 创建与开启日期：2026-10-20，用户明确要求“把第三阶段标记为完成，并开启第四阶段”，本卡作为本阶段第一张任务实际开启。
- 计划来源：[第四阶段计划](../plan.md)的控制台TCP请求回复、模拟设备状态、按换行提取消息和关闭重连目标。
- 选择依据：已有有效数量、缓冲与连续取帧的实践可以迁移，当前先做一个完整TCP控制功能，不再拆字节或单个字段小题；WPF、自动重连、心跳、Socket对照与UDP留后续按实际需要推进。
- 前阶段收尾：第三阶段已按用户明确选择以已通过源码范围标为✅ 已通关，实际串口联调仍未验证，详见[阶段核对](../notes/serial-stage-source-check.md)。

## 已准备的项目与任务目录

已新建独立阶段目录Stage04-TcpUdp，使用与现有工程一致的.NET 10，不增加第三方依赖。
实际工程为[TcpUdp.csproj](../../Stage04-TcpUdp/TcpUdp/TcpUdp.csproj)，可打开[解决方案](../../Stage04-TcpUdp/TcpUdp.slnx)；新共享入口[Program.cs](../../Stage04-TcpUdp/TcpUdp/Program.cs)只选择模拟器、上位机与人工观察模式，未改动旧阶段入口。
专属目录为Stage04-TcpUdp/TcpUdp/Exercises/TASK-049-tcp-roundtrip/，本题源码、运行说明与准备验证均放在这里。

- [Task049ClientExercise.cs](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-049-tcp-roundtrip/Task049ClientExercise.cs)提供菜单、逐条请求、接收循环、错误反馈及关闭清理，连接和发送留TODO。
- [Task049LineBuffer.cs](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-049-tcp-roundtrip/Task049LineBuffer.cs)提供同一个字节列表和方法签名，有效追加及LF消息提取留TODO。
- [Task049Simulator.cs](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-049-tcp-roundtrip/Task049Simulator.cs)是教练提供的可运行C#设备对端，保存设备状态并按协议回复，不计学员自主TCP Server实现。
- [Task049BufferObservation.cs](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-049-tcp-roundtrip/Task049BufferObservation.cs)用人工字节观察同一解析器，不把人工输入当作真实TCP分段。
- [RUN-GUIDE.md](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-049-tcp-roundtrip/RUN-GUIDE.md)先解释真实对象和代码作用，再按步骤动手。

本次采用C#模拟器以直接复用当前.NET环境，Java模拟器仍是计划中的可选实现，不增加双语言重复开发要求。

## 四组TODO与首个动作

| TODO | 实际改动位置与效果 | 对应DoD |
| --- | --- | --- |
| 1 | Task049ClientExercise.OpenAsync创建并保存_client，等待ConnectAsync成功后保存_stream，只在成功后输出连接成功。 | 1、3 |
| 2 | SendRequestAsync将command加一个LF编码并发送，发送成功后再进入已有ReadReplyAsync等待完整回复。 | 1 |
| 3 | Task049LineBuffer.Append把本次count以内的有效字节追加到_bytes，保留此前半条。 | 2 |
| 4 | TryTakeMessage按第一个LF取出一条消息，解码并只移除已消费部分，未收齐则保留，拒绝空行及超长消息。 | 1、2 |

首个动作只完成TODO 1，然后分别运行模拟器与上位机，在上位机输入o；应看到客户端连接成功，模拟器出现接受连接的记录，此时尚不要求读取温度成功。
后续顺序为TODO 3和4后先用--observe核对同一个缓冲，再完成TODO 2，运行两端验证温度、启动/停止与设备状态。
每组TODO仍属于同一个完整功能任务，基础必要说明直接见运行说明，不设动手前口试。

## 本题协议和已有辅助边界

模拟器只监听127.0.0.1，默认TCP端口5000，端口被占用时两个终端统一改用同一个其他端口，例如5001。
双方使用不带BOM的UTF-8和单个LF字节0A作为消息结束，命令只接受可打印ASCII字符，不能发送CRLF；一条消息最多256字节，不含LF。
GET_TEMP回复35.6，START_DEVICE和STOP_DEVICE改变模拟器状态后回复OK，GET_STATUS回复RUNNING或STOPPED；设备停止后仍可查询温度，设备状态在模拟器进程内跨连接保留，重启模拟器初始为STOPPED。
PING/PONG已由对端提供但本题不要求实现心跳，未知命令回复ERR_UNKNOWN_COMMAND，空命令回复ERR_EMPTY_COMMAND，非法字节或超过256字节的命令关闭当前连接后继续监听。
客户端只等待一条请求，一条回复完整到达后才能执行下一条；--observe中的连续回复是解析器输入对照，不表示本题开启多条并发请求。
菜单、菜单编号到命令的映射、温度显示换算、读取循环、3秒请求保护和最终资源清理由教练提供，不计学员自行设计超时、取消或完整通信生命周期的证据。

## 运行与提交

每次修改后先退出自己运行的本题程序，再从D:\host-computer-st执行dotnet build .\Stage04-TcpUdp\TcpUdp.slnx --nologo，然后分别打开两个PowerShell终端，先运行模拟器，再运行上位机；两端使用--no-build启动同一份刚构建的代码。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --simulator
```

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --client
```

模拟器分段回复模式使用--simulator 5000 split，人工缓冲观察使用--observe；完整命令与预期见运行说明。
完成后提交源码并说“检查TASK-049”即可，继续尊重当前源码审查偏好；如只查源码，则明确保留未运行场景，不把教练模拟器准备验证算作学员通信结果，不要求重复抄录日志。

## 验收标准（DoD）

- [ ] 上位机的连接、UTF-8/LF发送与完整回复路径正确连接，能得到温度35.6、确认启动/停止并通过状态查询核对结果，不将写入成功等同执行成功。
- [ ] 只追加有效数量，半条保留、连续取消息、尾部半条和恰好256字节的边界处理正确，空消息/超过上限明确拒绝，消费一条后后续消息保留且不重复。
- [ ] 关闭后清空旧缓冲并可用新对象重连，失败后仍能继续操作，退出会清理连接；旧阶段成果保留，教练支持与学员核心实现及源码/运行证据分别记录。

## 2026-10-20 骨架准备证据

实际运行dotnet build Stage04-TcpUdp/TcpUdp.slnx --nologo，退出0，0警告/0错误。
实际运行python .\Stage04-TcpUdp\TcpUdp\Exercises\TASK-049-tcp-roundtrip\Verification\verify_preparation.py，退出0，25项教练准备检查全部通过，结果见[preparation-2026-10-20.json](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-049-tcp-roundtrip/Verification/preparation-2026-10-20.json)。
检查覆盖入口帮助、未实现时如实显示占位、参数拒绝、模拟器真实本机TCP命令往返、设备状态跨连接、分批/连续命令、长度边界、非法输入后重新连接及两次写入回复。
验证客户端是教练的Python检查代码，不是尚未完成的Task049ClientExercise；人工缓冲入口目前如实显示TODO未完成，没有完整消息输出，不计学员DoD通过。
验证脚本只终止自己启动的模拟器进程，没有停止用户进程；交互式Ctrl+C退出未作为运行证据，四组核心TODO保留，三个DoD均未勾选，不创建TASK-050。
