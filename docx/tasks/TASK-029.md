# TASK-029：设置温度报警阈值

- 阶段：第二阶段：WPF（起步）。
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)的参数设置、报警和第二阶段设备监控界面目标。
- 选择依据：TASK-028已验证最新原温度读取与正常/超限切换，下一步让输入的阈值影响当前提示；不据此宣称WPF阶段完成。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

当前27写在判断代码中，本题提供阈值输入框与“应用阈值”按钮，初始有效阈值仍为27 °C。
接受20到30之间且包含两个端点的数字，允许26.5等小数，这只是模拟练习范围。
应用有效阈值后立即根据原集合最新温度刷新提示，不追加读数；空历史保持暂无温度。
空白、非数字或范围外输入显示原因，保留上一个有效阈值与原历史，不中断采集。

## 开启时准备约定

2026-10-17已按用户明确要求开启，独立窗口已准备，App.xaml已切到TASK-029；下方仅建卡片为历史。
用户明确开启后读取最新源码与记录，再准备独立任务窗口，保留原作品。
教练提供输入框、应用按钮和当前有效阈值/输入结果显示区，复用集合、提示、启停、绘图，阈值保存/输入检查/刷新留TODO。
先解释TextBox.Text是输入文字、阈值字段保存数值，再解释.NET的double.TryParse如何尝试转换及C#的out如何接收结果，不默认学员已掌握。
首个动作只把原判断中的27改为读取一个初始值27的数值字段，运行确认原样例提示保持，再连接输入。

## 实践与产出

1. 保存当前有效阈值，并让最新温度判断读取它。
2. 点击应用时验证输入，合法才保存并显示当前阈值，失败保留原值。
3. 应用后立即刷新原提示，在停止和采集中检查原数据链保持。

## 验收标准（DoD）

- [x] 构建并实际打开窗口，初始27且原样例提示保持；最新26.5时应用26立即超限、应用27立即正常，历史内容和条数不变。
- [x] 20、30、26.5可应用，空白、非数字、19.9、30.1提示错误并保留旧阈值，随后合法输入可恢复；最新值恰等于阈值时超限，空历史应用后仍显示暂无温度。
- [x] 自动采集期间应用后使用新阈值，原启停、清空限制、停止保留、清空恢复、表格/计数/最新三点保持，默认和约定最小尺寸可读，正常关闭无异常。

## 状态与证据

- 2026-10-17：TASK-028三个DoD通过后仅创建本卡，状态⚪ 未开始，未准备源码、切换入口或运行验收。
## 实际文件与共享入口

- 专属目录：WpfBae/Exercises/TASK-029-alarm-threshold/。
- [Task029Window.xaml](../../WpfBae/Exercises/TASK-029-alarm-threshold/Task029Window.xaml)：沿用TASK-028窗口，新增ThresholdInput、ApplyThresholdButton、CurrentThresholdText、ThresholdFeedbackText，默认960×800、最小860×760。
- [Task029Window.xaml.cs](../../WpfBae/Exercises/TASK-029-alarm-threshold/Task029Window.xaml.cs)：保留原集合/表格/绘图/启停/清空保护/提示，新增初始为27的_alarmThreshold字段、UpdateThresholdDisplay显示方法和应用按钮空骨架；五个TODO为学员实践。
- [App.xaml](../../WpfBae/App.xaml)：共享入口例外，仅切换StartupUri到Task029Window。
- [WpfBae.csproj](../../WpfBae/WpfBae.csproj)：仅增加本题Verification生成源码的编译排除，防止主项目重复编译，不增加依赖。
- [骨架验证项目](../../WpfBae/Exercises/TASK-029-alarm-threshold/Verification/StartupReview.csproj)及[源码](../../WpfBae/Exercises/TASK-029-alarm-threshold/Verification/StartupReview.cs.txt)位于本题独立Verification目录。
- 原TASK-028文件未改，新副本复用其最终版本；旧提示TODO注释在新副本中整理为复用说明，不把已有实现重置为待做。

## 讲解与首个操作

先读[阈值输入Q2](../questions/wpf-alerts.md#q2task-029-怎样让输入的报警阈值生效)，里面按真实输入框、字段、转换返回值/out结果、有效范围与两个显示方法展开必要基础。
TextBox.Text提供文字，_alarmThreshold保存当前有效数字，UpdateThresholdDisplay只更新阈值文字，UpdateTemperatureAlert才会重新判断当前温度。

现在只做TODO 1：打开Task029Window.xaml.cs的UpdateTemperatureAlert，找到temperature >= 27，把右边27替换成_alarmThreshold，保留原来的正常/超限分支。
字段已在窗口类中提供，初始值27，不需要再声明第二个同名变量，也先不改应用按钮。
在工作区根目录运行dotnet run --project WpfBae/WpfBae.csproj，关闭旧窗口后再重新运行。
先不启动自动采集，连续点击“添加下一条”五次，预期24/26.5/25正常、28超限、下一条24正常，当前有效阈值仍显示27。
本步确认比较已从读取固定数字改为读取字段，界面结果与原来一致是预期；它不证明输入框已连接成功。

## TODO与实践/DoD对应

| TODO | 对应 | 可观察结果 |
| :--- | :--- | :--- |
| 1：原比较改用字段。 | 实践1，DoD 1/3。 | 初始27时原五条样例提示保持。 |
| 2：读取Text并TryParse，失败说明后返回。 | 实践2，DoD 2。 | 空白或abc提示输入错误，旧有效阈值不变。 |
| 3：只接受20到30且含端点。 | 实践2，DoD 2。 | 20/30/26.5合法，19.9/30.1拒绝且旧值保持。 |
| 4：成功才保存字段并更新当前阈值和成功说明。 | 实践2，DoD 1/2。 | 输入文字和已生效数字分别可见，错误后合法输入能恢复，末尾占位说明被移除或替换。 |
| 5：保存后调用原提示方法，不改变历史。 | 实践3，DoD 1/3。 | 最新26.5时应用26立即超限，应用27立即正常，条数不变；空历史仍暂无温度，自动采集使用新阈值。 |

先完成TODO 1并运行观察，再继续下一步；全部完成后发送“审查 TASK-029”，按本卡三个DoD验收。

## 2026-10-17 开启证据

- 新建前确认目标目录不存在，保留全部原作品，未准备TASK-030，不扩大已掌握范围或新增错题。
- 实际执行dotnet build WpfBae/WpfBae.csproj --nologo，退出0、0警告/错误。
- 实际执行dotnet run --project WpfBae/Exercises/TASK-029-alarm-threshold/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-029/startup，退出0，39项实际窗口骨架检查全部通过，含初始27、应用占位不改历史/阈值、手动追加/Count/同集合、停止清空、默认及最小布局边界、关闭收尾。
- 实际执行& WpfBae/Exercises/TASK-029-alarm-threshold/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-029/startup/launch.json，原exe标题TASK-029，正常关闭退出0。
- 证据：[骨架检查](./evidence/TASK-029/startup/results.json)、[原exe运行](./evidence/TASK-029/startup/launch.json)、[默认内容](./evidence/TASK-029/startup/default.png)、[最小内容](./evidence/TASK-029/startup/minimum.png)。
- 三个核心DoD均未勾选，状态🟡 进行中；骨架中的应用按钮仅显示占位说明，当前不保存输入，不把骨架运行当作本题验收。
## 2026-10-17 审查结果：🟡 进行中

| 必需项 | 结果 | 实际现象与依据 |
| :--- | :--- | :--- |
| DoD 1：保存后立即用当前读数刷新。 | 未通过。 | 初始27及原五条样例正确，字段/有效值文字保存正确且历史不变；最新26.5时应用26，仍显示温度正常，缺少成功路径调用UpdateTemperatureAlert。 |
| DoD 2：合法/非法输入、相等边界及恢复。 | 未通过。 | 20/30/26.5正确保存，空白/abc/19.9/30.1拒绝且保留旧有效阈值与历史；最新值等于新阈值时未立即超限，NaN可绕过原范围条件并被保存，违反仅接受20到30有效数字。 |
| DoD 3：后续自动采集和原业务/布局/关闭。 | 通过。 | 自动下一条使用新阈值，运行清空禁用、重复启停、停止2200ms保留、清空及重启24、表格/Count/最新三点、默认960×800/最小860×760布局边界和原exe关闭退出0通过。 |

原提交实际98项检查79通过、19失败，无未处理运行异常；输入解析失败时Console输出的FormatException已由学员catch处理，不是程序崩溃。
教练按既有低影响状态文案直接修正规则，仅改三处赋值：解析失败与范围失败改写ThresholdFeedbackText，成功文字替换末尾占位说明。
未改学员的Parse/catch、范围条件、有效字段赋值、比较或提示刷新逻辑，这些核心问题仍由学员修改。
协助后完整复跑98项，92通过、6失败，其中5项为未立即刷新在不同场景的表现，1项为NaN未拒绝；并不是六个独立缺陷。
构建0警告/错误；原exe实际打开TASK-029并正常关闭退出0；复测期间本题后台源码哈希不变，审查与原程序DLL一致。

### 首个修复：成功保存后重新判断

只改ApplyThresholdButton_Click的成功路径，在UpdateThresholdDisplay之后调用已有UpdateTemperatureAlert。
_alarmThreshold保存的是数值，UpdateThresholdDisplay只设置当前有效阈值的文字；AlarmText由另一个方法UpdateTemperatureAlert负责，字段赋值不会自动调用它。
运行后手动添加24和26.5，应用26应立即超限，再应用27应立即正常，条数一直为2。
这是本轮先做的一个小改动，不需要重写整个按钮方法，也不要求为已经处理普通非法输入的Parse/catch强行改用TryParse。

### 随后修复：只允许范围内的数字

.NET的double还能表示一个特殊值NaN，意思是“不是一个数”，double.Parse能接受字符串NaN；它不属于本题合法20到30数字。
NaN与20/30做小于或大于比较时都为false，所以当前threshold < 20 || threshold > 30不会进入拒绝分支。
可以先明确拒绝double.IsNaN(threshold)，或者改成“同时满足>=20且<=30才接受”的合法条件，其他情况返回；double.IsNaN是.NET提供的判断方法。
这是本轮补充此前未展开的double特殊值基础，不把教练未讲过该边界单独归为学员典型错误，也不要求实现任务范围外的数值框架。
复测NaN保留旧有效阈值并显示输入错误，再检查20/30/26.5和普通非法输入仍正常，随后发送“审查 TASK-029”。

实际命令：dotnet build WpfBae/WpfBae.csproj --nologo。
实际命令：dotnet run --project WpfBae/Exercises/TASK-029-alarm-threshold/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-029/review-02。
实际命令：& WpfBae/Exercises/TASK-029-alarm-threshold/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-029/review-02/launch.json。
证据：[原提交结果](./evidence/TASK-029/review-01/results.json)、[协助后结果](./evidence/TASK-029/review-02/results.json)、[原exe运行](./evidence/TASK-029/review-02/launch.json)、[核对与协助范围](./evidence/TASK-029/review-02/verification.json)、[最小窗口](./evidence/TASK-029/review-02/minimum.png)。
本题仍🟡 进行中，DoD 1/2未勾选、3已勾选，不创建TASK-030；学员已验证的有效字段连接、Parse/catch普通错误保护、有效值保存与后续采集局部能力保留，Q2整体继续待复习，不声称掌握TryParse/out。
## 2026-10-17 最终复查：✅ 已通关

| 必需项 | 最终结果 | 实际证据 |
| :--- | :--- | :--- |
| DoD 1：应用后立即刷新且保留历史。 | 通过。 | 最新26.5应用26立即超限、应用27立即正常；20/30/26.5保存与显示正确，恰等阈值立即超限，历史内容/条数不变。 |
| DoD 2：非法输入与恢复。 | 通过。 | 空白/abc/19.9/30.1拒绝且保留有效阈值和历史，合法输入恢复，NaN识别后进入现有catch并返回、不保存，空历史应用仍暂无温度。 |
| DoD 3：采集/原业务/布局/运行。 | 通过。 | 采集中应用立即刷新且后续读数使用新阈值，停止等待2200ms保持、停止清空/重启24、同集合表格/Count/最新三点、默认960×800/最小860×760布局和原exe关闭退出0通过。 |

学员已自主补充double.IsNaN检查并抛出异常交现有catch处理，以及成功路径调用UpdateTemperatureAlert；本轮未修改学员源码。
原先教练协助的两个错误反馈目标和成功文字仍按协助范围保留，不算学员自主实现。
构建退出0、0警告/错误，原编译窗口98项实际检查全部通过、0未处理运行异常；Console显示的普通格式异常与主动抛出的“温度不合法”均已被catch处理，不是程序崩溃。
本题XAML及后台源码在复查前后哈希一致，审查引用DLL与原exe加载DLL一致。

实际命令：dotnet build WpfBae/WpfBae.csproj --nologo。
实际命令：dotnet run --project WpfBae/Exercises/TASK-029-alarm-threshold/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-029/review-final。
实际命令：& WpfBae/Exercises/TASK-029-alarm-threshold/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-029/review-final/launch.json。
证据：[98项结果](./evidence/TASK-029/review-final/results.json)、[源码/DLL核对](./evidence/TASK-029/review-final/verification.json)、[原exe运行](./evidence/TASK-029/review-final/launch.json)、[默认内容](./evidence/TASK-029/review-final/default.png)、[最小内容](./evidence/TASK-029/review-final/minimum.png)。
三个DoD全勾选，TASK-029✅ 已通关；[阈值输入Q2](../questions/wpf-alerts.md#q2task-029-怎样让输入的报警阈值生效)限本骨架中的字段/Parse-catch/范围/IsNaN/保存与立即刷新自主应用标已掌握，不包含TryParse/out、独立界面设计及提供的显示方法实现。
只创建下一张[TASK-030：用JSON保存并恢复报警阈值](./TASK-030.md)（⚪ 未开始），依据计划配置与参数设置目标，未准备源码或切换入口，WPF阶段仍进行中；上文未通过结论为历史。