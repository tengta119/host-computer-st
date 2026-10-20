# TASK-052：完成Socket收发对照与UDP设备查询

- 阶段：第四阶段：TCP/UDP（Socket对照与UDP查询）。
- 状态：🟡 进行中
- 创建日期：2026-10-20。
- 开启日期：2026-10-20，用户明确要求“开启task52”。
- 计划来源：[第四阶段计划](../plan.md)的Socket收发对照、UDP独立查询及综合核对目标。
- 选择依据：[TASK-051](./TASK-051.md)已按静态源码口径通关，TCP控制、WPF监控和恢复路径已有实现，下一步覆盖计划尚余的Socket对象与收发数量、UDP来源和查询编号匹配，不重复拆已完成的按钮或历史小题。

## 要完成的一个完整功能

在同一设备模拟场景下，用最小Socket客户端对照已有TCP命令往返，再增加UDP单播温度或状态查询，并核对客户端记录与模拟端实际状态。
Socket部分只做最小连接、完整发送、按实际数量接收和关闭对照，不要求将既有WPF系统整体改写；复用已有TCP消息边界规则，发送不足时继续发送剩余字节，接收为零时识别连接结束。
UDP查询使用独立数据报，约定请求编号、回复格式、长度上限与截止时间，只接受指定来源且编号匹配的回复，同一查询最多完成一次，不把旧回复当作新结果。
模拟端提供正常、不回复、重复及迟到旧回复等可控场景，TCP继续承担既有控制命令，不增加UDP启动/停止控制或可靠传输框架。

## 开启时准备与讲解

仅在用户明确要求开始TASK-052后准备骨架，先核对现有阶段项目、入口和模拟器，任务专属文件放独立目录，保留TASK-049至051和旧阶段作品，届时记录真实路径。
先从现有TcpClient所代表的连接，说明.NET Socket对象怎样负责连接和字节收发，再逐处解释发送返回数量与剩余字节；不能把已有TCP辅助当作已自主掌握Socket接口。
UDP从一次查询发送给哪个地址/端口、收到的数据报来自哪里展开，解释UdpClient、查询编号及接收结果的用途后，再要求连接实现；不把TCP连续缓冲规则直接套给UDP数据报。
教练准备必要模拟端、故障输入与运行入口，学员完成关键发送数量处理、来源/编号/长度校验和完成/超时连接，不引入新网络框架。
开始时明确协议细节、取消与等待规则、可执行步骤和每个TODO对应的DoD；首次只完成一个能观察到回复的Socket往返，再推进UDP，避免一次堆叠所有新API。

## 验收标准（DoD）

- [ ] 最小Socket TCP客户端将连接、发送、接收和关闭对应到具体对象，按返回数量推进完整发送及有效接收，正确处理LF分段与对端关闭，并与已有TcpClient流程对照。
- [ ] UDP单播查询按约定编码/长度/来源/查询编号接受有效回复，一条查询只完成一次，无回复能按期限结束，重复、错误来源或编号及迟到旧回复不污染后续查询，取消后不继续等待或显示旧结果。
- [ ] 综合核对既有TCP控制/WPF状态、恢复路径与新增查询的记录和设备状态，分别注明静态源码、实际运行和教练辅助证据，说明TCP字节流与UDP数据报在本题代码中的边界处理区别，未验证部分如实保留。

## 当前证据

当前已实际开启并准备独立源码、双协议模拟端、命令行入口和运行说明，五组核心TODO保留学员实现，三个DoD均未勾选；初次仅建卡的说明保留为历史依据。
TASK-051的静态通关和教练历史支持检查不直接计作本题能力或阶段综合运行证据，第四阶段仍🟡 进行中，阶段完成时还需按计划核对全部目标与证据范围。

## 2026-10-20 实际开启与文件范围

本题专属目录为Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/，复用现有控制台项目，不新建框架依赖或搬移旧作品。
已创建[Socket练习](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/Task052SocketExercise.cs)、[UDP练习](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/Task052UdpExercise.cs)、[双协议模拟器](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/Task052Simulator.cs)、[任务入口](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/Task052Entry.cs)和[运行说明](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/RUN-GUIDE.md)。
共享Program.cs仅增量接入--task052及帮助，TcpUdp.csproj仅排除本题Verification下可能的.cs诊断源码；WPF入口和TASK-049至051代码未改，复用既有Task049LineBuffer。
教练提供Socket对象创建/释放、3秒总截止、有效接收数量传入既有LF缓冲、UDP2秒总截止、单请求互斥、每轮GUID编号、一次完成与取消/释放入口，不能把这些骨架算作学员自主掌握。
模拟器同时监听同一个数字端口的TCP与UDP，二者读取同一个设备运行状态，TCP回复故意分两次写出；UDP提供不回复、重复、暂存旧回复、错误来源、错误编号、非法内容等可控模式，另有TCP主动关闭模式。

## TODO与具体实践对应

| TODO | 实际文件与方法 | 核心动作 | 对应DoD |
| --- | --- | --- | --- |
| 1 | Task052SocketExercise.ConnectCoreAsync | 等待socket.ConnectAsync，移除未完成异常。 | 1 |
| 2 | Task052SocketExercise.SendAllAsync | 每次最多提供3字节，按SendAsync实际返回量推进offset，发完才结束，零/负数量作为失败。 | 1 |
| 3 | Task052SocketExercise.ReceiveCoreAsync | 返回ReceiveAsync实际读取数量，外层复用已有有效数量追加、EOF检查和LF提取。 | 1 |
| 4 | Task052UdpExercise.SendQueryAsync | 用UdpClient把完整查询发送为一条数据报，保留当前编号与取消token。 | 2 |
| 5 | Task052UdpExercise.TryAcceptDatagram | 校验来源地址/端口、字节范围和长度、四段格式、当前编号/命令与合法值，成功才赋reply并返回true。 | 2 |

DoD3在这两条链路完成后综合核对已有TCP控制、WPF显示、恢复路径和新增UDP查询，源码和实际观察分别记录，不额外要求重复抄录已有日志。
TCP沿用ASCII命令加LF、UTF-8兼容回复及256字节消息上限，Socket练习每次命令使用独立连接，一次连接/发送/完整接收共用3秒总截止；本题每次最多提供3字节是人为分块，不当作内核发生部分发送的证据。
UDP请求为Q|32位GUID编号|GET_TEMP或GET_STATUS，回复为R|同编号|同命令|值，不加LF，每条1至128可打印ASCII字节，来源地址和端口必须匹配；温度只接受0至100有限小数、状态只接受RUNNING或STOPPED。
UDP每轮共用2秒截止，忽略无效报文不重置截止，同一UdpClient连续查询两轮但编号不复用；stale-first第一轮暂存回复使其超时，第二轮先发旧编号再发当前编号，教练用确定性暂存验证旧回复过滤，不宣称覆盖全部真实网络故障。

## 首个动作与运行方式

现在只修改Task052SocketExercise.cs的ConnectCoreAsync：签名改为async Task，等待socket.ConnectAsync(endpoint, token)，移除Task.FromException占位；实际代码、对象身份、token和预期现象已在运行说明逐处讲解。
从根目录执行dotnet build .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj --nologo，终端一运行dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --task052 server 5000 normal，终端二运行同一项目前缀并传--task052 socket 5000 GET_TEMP。
仅完成TODO 1后应从连接未完成提示推进到TODO 2发送未完成提示；TODO 1至3完成后才应看到35.6，不把第一步推进等同完整往返成功。
UDP阶段命令为--task052 udp 5000 GET_STATUS，待TODO 4/5完成后再按运行说明切换对端故障模式；Ctrl+C停止自己这轮程序，不终止用户原有进程。

## 教练准备验证与边界

实际执行python .\Stage04-TcpUdp\TcpUdp\Exercises\TASK-052-socket-udp\Verification\verify_preparation.py，最终退出0，临时输出构建0警告/0错误，38项准备检查及6项骨架占位/预取消检查通过，193个受保护旧源码哈希不变，详细命令和构建输出见[preparation-2026-10-20.json](../../Stage04-TcpUdp/TcpUdp/Exercises/TASK-052-socket-udp/Verification/preparation-2026-10-20.json)。
检查直接使用教练测试端验证新模拟器TCP分段命令/连续命令、TCP控制后UDP读到同一RUNNING状态、各故障模式的预定报文、TCP EOF、八个临时服务的取消退出、命令行帮助/非法参数/占位错误以及旧作品保护，不验证学员尚未实现的筛选和收发逻辑。
首次准备脚本误把有意增量修改的共享csproj纳入不变基线，源保护检查失败，修正为明确排除Program.cs及csproj；随后一次临时服务连接就绪检查失败，修正测试端口选择为同时预检TCP与UDP可绑定后复跑通过，未把失败尝试记作验收通过或学员问题。
本轮只运行自己创建的临时服务和测试进程，没有启动/替换WPF窗口或杀用户程序；完整学员Socket/UDP链路与WPF综合运行均未验证，三个DoD未勾选，不创建TASK-053，问答状态/统计与已掌握范围保持。
