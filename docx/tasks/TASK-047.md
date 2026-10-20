# TASK-047：检查串口数据帧并从坏帧中恢复

- 阶段：第三阶段：设备通信（协议检查与恢复）。
- 状态：✅ 已通关
- 创建日期：2026-10-20。
- 开启日期：2026-10-20，用户明确要求“开启task047”。
- 计划来源：[计划](../plan.md)第三阶段第4节的字段有效性、校验与无效数据恢复，以及只有有效读数才进入历史的目标。
- 选择依据：TASK-046等待/超时/清理已按源码口径通关，现有解析仍把AA开头的任意六字节当读数，尚无命令、设备范围、测量范围或校验的完整检查，因此把这些问题放进一张完整收发实践，不拆成多个语法小题。
- 项目：复用[WPF项目](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/DeviceCommunication.Wpf.csproj)与[TASK-046成果](./TASK-046.md)，开启后按现有约定准备独立任务目录，不改旧任务的教学协议与源码。
- 难度/预计时间：待补充，不预设时长。

## 实际骨架与复用范围

此前仅建卡安排已由本次明确开启覆盖，WPF专属目录为Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/，控制台模拟端专属目录为Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-047-protocol-simulator/。
新增[窗口源码](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Task047Window.xaml.cs)、[布局](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Task047Window.xaml)、[串口辅助](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Task047SerialConnection.cs)、[协议判断](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Task047Protocol.cs)和[运行说明](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/RUN-GUIDE.md)。
配套[模拟端](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-047-protocol-simulator/Task047Simulator.cs)和[14组菜单样例](../../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-047-protocol-simulator/Task047Samples.cs)由教练提供，字段非法样例保留正确加和字节，包含半帧、坏候选内部好帧头、内部AA、边界与长噪声。
窗口、等待/超时、清理和显示从TASK-046复制后复用，仍引用TASK-045读数与事件参数，不修改原TASK-044至046源码；共享App.xaml切换Task047Window，控制台Program.cs只增加--task047-simulator与--task047-list入口，原默认TASK-044及旧--simulator模式保留，两个项目文件与依赖不改。
教练已连接合法帧消费与读数通知，两处核心逻辑留TODO；IsValidFrame暂时返回false，坏候选分支暂Clear/break以安全退出，因此骨架不会接受任何完整读数，不能当作任务完成。

## TODO对应与首个动作

| TODO | 实际位置与动作 | DoD |
| --- | --- | --- |
| 1 | Task047Protocol.IsValidFrame检查帧头、设备、命令、原始范围与前五字节加和取余，全部满足才返回true。 | 1、3 |
| 2 | Task047SerialConnection.ExtractReadings拒绝候选后只删缓冲首字节并继续循环，替换Clear/break占位，保留后面的好帧。 | 2、3 |

首个动作只完成Task047Protocol.cs的TODO 1，再运行两端、在模拟端输入1，预期历史新增设备1、25.0℃；5至8各自不应新增，TODO 2稍后用菜单10和12检查恢复。
当前返回false的占位会让所有候选被拒绝并保持等待直到3秒超时，不据此认定串口没收到；TODO 1完成后单独好帧可显示，TODO 2未完成时坏数据后的好帧仍可能丢失。
具体对象身份、%取余、424除以256余168、六字节候选和AA 99后接好帧的逐步变化先见运行说明，不把名词和TODO注释当作已掌握。
运行命令与菜单见运行说明，沿用原虚拟串口对但改用本题专用模拟端；旧控制台仍占用exe或端口时先在旧模拟端输入q并退出，再运行本题。


## 真实问题与本题规则

现有解析只认AA开头和6字节长度，传输中某个字节变了也可能变成一条看起来正常的温度，窗口需要在加入历史前检查它是否符合协议。
本题明确采用一份新的六字节教学协议，下面规则仅用于TASK-047，不把它说成真实设备标准或原TASK-044协议；旧模拟端原样例的末字节不保证满足新规则，开启时教练提供配套模拟端和有效/无效样例，不让学员猜末字节。

| 字节位置 | 本题约定 |
| --- | --- |
| 0 | 固定AA帧头。 |
| 1 | 设备编号，只接受01或02。 |
| 2 | 固定03，表示本题的温度读数。 |
| 3和4 | 高字节在前的原始值，原始值 = 第3字节乘256加第4字节，只接受0至1000，对应0至100.0℃。 |
| 5 | 前五字节相加后除以256的余数，用于本题的简单加和校验。 |

例如AA 01 03 00 FA A8的前五字节相加为424，除以256余168，即十六进制A8，因此表示设备1、原始250、25.0℃的有效帧。
AA 02 03 01 04 B4表示设备2、原始260、26.0℃；AA 01 03 00 AA 58中的AA是数值的一部分，不可一见内部AA就重开一帧。
这里的加和校验只是教学规则，不等于CRC；先解释“发送端算一份，接收端再算一份并比较”的实际用途，再结合样例讲取余，说明简单加和并不能发现所有错误，不要求背新术语。

## 要完成的一个功能

串口收到完整候选帧后先检查字段和末字节，只有检查通过的读数才能发给窗口、结束请求等待并进入历史、曲线和报警。
坏帧不能伪装成一条响应，也不能让接收永远卡住；拒绝当前候选后继续寻找后面的合法开头，不能把后面可能属于好帧的字节随意整批删掉。
不足6字节继续保留，一次多个候选继续检查，整包或分段输入应产生相同的有效读数序列；处理后留下的未完成候选有明确长度边界，避免无效数据持续积累。
请求仍是55 01，等待上限与TASK-046保持，坏帧不结束等待；之后收到有效完整帧才结束等待，没有有效回复仍能超时，重连继续清旧缓冲。

## 开启时教练应先准备

1. 为有效候选检查、消费/恢复循环和读数输出准备独立骨架，复用原窗口、连接生命周期和已完成显示，不把新规则写进旧TASK-046。
2. 提供符合新末字节规则的模拟端，以及正确帧、错误校验、错误设备/命令/范围、噪声后好帧、坏候选内出现后续好帧开头、数值含AA和分段/连续帧样例。
3. 字段非法样例应使用重新算好的末字节，以便分别检查字段规则与校验规则，不能所有样例都只因校验错误被拒绝。
4. 先解释候选帧、字段检查、加和取余与拒绝后怎样继续找帧的具体代码位置，再给首个可观察动作；不新增通信框架，不提前要求CRC或完整Modbus。

## 验收标准（DoD）

- [x] 按本题明确规则检查设备、命令、原始范围和校验，只有合法完整帧生成读数，正确高低字节与温度换算保持，错误帧不会加入历史或结束请求等待。
- [x] 半帧保留、连续帧持续处理，噪声/错误候选之后能继续找到好帧，合法帧内部AA保留为数据，相同数据分段方式不改变有效读数顺序或产生重复，缓冲不会随持续无效数据无限增长。
- [x] 新规则与现有串口收发、WPF历史/曲线/报警、3秒超时和断开重开保持连接，配套模拟端与样例一致，旧TASK-044至046作品保留，关闭后不再追加旧通知。

## 当前证据与验收口径

2026-10-20实际执行python .\Stage03-DeviceCommunication\DeviceCommunication.Wpf\Exercises\TASK-047-frame-validation\Verification\verify_startup.py，WPF构建0警告/0错误，临时窗口宿主Show/最小尺寸渲染/关闭退出0；[准备记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/startup-2026-10-20.json)与[已查看的布局](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/startup-minimum-2026-10-20.png)已保存。
控制台初次普通构建因既有DeviceCommunication进程34092占用exe而失败，未结束用户进程；随后dotnet build项目并使用临时--output目录成功，0警告/0错误，dotnet临时DeviceCommunication.dll --task047-list退出0，[原失败记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/simulator-preparation-2026-10-20.json)和[最终记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/simulator-preparation-final-2026-10-20.json)均保留。
教练只核对14组配套字节的10项准备条件，全部符合，[样例记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/sample-preparation-2026-10-20.json)保存真实结果及旧文件哈希不变的数量；未打开串口、发送样例或运行学员协议/恢复实现，两个TODO尚未完成，三个DoD未勾选，第三阶段仍进行中，不创建下一题。
后续审查沿用用户“只看源码，源码正确即可通过”的偏好，按可见代码逐项验收，不伪造实际串口联调或坏帧实验结果；阶段计划中的实际联调证据仍单独保留边界。


## 2026-10-20 静态审查：仍🟡 进行中

按用户偏好仅查源码，未构建/运行/测试或修改源码；[审查记录](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/static-review-2026-10-20.md)与[源码哈希](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/static-review-2026-10-20.json)已保存。
TODO 2拒绝后RemoveAt(0)/continue及已有半帧/连续帧流程源码通过，DoD2勾选；TODO 1已有长度、帧头、设备、命令和加和，缺少原始值0至1000检查，导致菜单8的1001也会被接受，所以DoD1和3未通过，只有这一处独立阻塞。
首步在IsValidFrame返回true之前合成frame[3] * 256 + frame[4]，拒绝超过1000的原始值，0和1000仍合法，不必重写已正确条件和循环；保留学员实现，Q5待复习与27分类/75条统计不变，不创建下一题。
以上为静态路径推导，不声称实际输出过100.1℃，开题准备中“两个TODO未完成、DoD未勾选”为当时历史，当前以此审查为准。


## 2026-10-20 最终静态复查：✅ 已通关

学员已在IsValidFrame中合成int原始值并拒绝大于1000，0和1000仍合法，字段/加和及坏帧恢复保持正确，三个DoD按静态源码口径通过、勾选已同步。
[最终审查](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/static-review-final-2026-10-20.md)与[协助前后源码哈希](../../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/static-review-final-2026-10-20.json)已保存，教练只清理一处XAML旧骨架提示，未改核心代码；旧失败审查保留，之前待修结论为历史。
本轮未构建、运行、测试或打开串口，不声称真实坏帧或串口联调已验证，Q5继续待复习，参考代码不计独立掌握，问答统计不变。
按计划仅创建[TASK-048：整合串口监控与通信日志](./TASK-048.md)⚪ 未开始卡，未准备新源码或切换入口，WPF仍为TASK-047，第三阶段仍🟡 进行中。
