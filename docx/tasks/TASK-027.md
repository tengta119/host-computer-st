# TASK-027：用启动和停止按钮控制模拟温度采集

- 阶段：第二阶段：WPF（起步）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「三、第二阶段：WPF」的数据图表与设备监控系统示意中的实时曲线、启动/停止目标。
- 选择依据：TASK-026已验证完整表格与最新三点共用原温度集合，下一步把手动添加扩展为可启动和停止的周期模拟采集，启停后仍使用原集合；这一控制链尚缺实际实践证据。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

打开窗口时不自动采集，点“启动”后约每秒添加一条24/26.5/25/28循环样例，完整表格、计数和最新最多三点同步更新；点“停止”后历史保留且不再自动增加，再次启动从停止前的样例位置继续。

重复点击启动不得创建多份定时器或重复登记，停止可重复点击且不报错；保留手动添加和清空，清空只在停止状态下可操作，清空后计数/表格/图一起归零、样例位置恢复24，下次启动从24开始。

本题只做WPF界面线程上的短小模拟追加，不引入真实串口/TCP、后台线程、精确计时、图表库或完整采集框架，也不要求自行改写现有温度坐标与绘图算法。

## 当前最终结论（2026-10-14）

三个必需DoD全部通过，状态✅ 已通关；最终29项技术检查全部通过、0运行异常，构建0警告/错误，原exe正常启动关闭退出0，默认960×680与最小860×640可读。

用户明确要求两处状态文案由教练收尾，不再让学员反复修改；教练修正运行“采集中”/停止“已停止”，同步清理初始“启停待实现”占位和底部过期观察说明，其余学员实现保留。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 通过 | 初始不采集，自动24/26.5/25/28/24，原集合完整表格/Count/最新三点/原值标签一致，只读无新增行。 |
| 2 | 通过 | 停止至少2250毫秒不新增，继续26.5，重复启停安全、单次Tick追加、单一原定时器/一次登记，运行与停止文字正确。 |
| 3 | 通过 | 运行清空禁用且方法保护有效，停止正常按钮清空，重启24，临时入口隐藏，运行中关闭后停止，默认/最小布局正常。 |

真实命令：dotnet build WpfBae/WpfBae.csproj --nologo退出0、0警告/错误；dotnet run --project WpfBae/Exercises/TASK-027-start-stop-sampling/Verification/Recheck02.csproj -- docx/tasks/evidence/TASK-027/recheck-02退出0、29项全过；本任务launch.ps1启动原exe正常关闭退出0。

证据：[最终复查](./evidence/TASK-027/recheck-02/README.md)、[运行结果](./evidence/TASK-027/recheck-02/results.json)、[源码与DLL核对](./evidence/TASK-027/recheck-02/verification.json)、[原exe退出](./evidence/TASK-027/recheck-02/launch.json)，源码快照与截图保留；协助修改后的59项源码在本轮运行验证期间哈希不变，不能将协助修改说成全程未改源码。

学员自主范围限当前骨架内Tick登记/计次/单次追加、原定时器启停/重复保护、清空条件/方法保护、临时按钮隐藏；教练修正的状态文字不算自主掌握，周期通知Q1整体仍待复习，既有C#/图表/布局复习范围保留，不宣称阶段通关。

按计划监控界面的报警目标，只创建下一张[TASK-028：显示模拟温度超限提示](./TASK-028.md)，状态⚪ 未开始；未准备下一题源码或切换App.xaml，仍启动TASK-027。以下未通过与要求学员修改状态文字为历史，现已收尾。

## 第一次复查结论（2026-10-14，历史）

状态保持🟡 进行中，DoD1/3通过，DoD2仅状态文字相反未通过；实际29项技术检查27通过、2失败、0运行异常，59项原源码哈希不变，未代改学员实现，不生成下一任务。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 通过 | 自动五条24/26.5/25/28/24、原集合/完整表格/真实Count/最新三点标签一致，初始不采集，表格只读无新增行。 |
| 2 | 未通过 | 停止至少2250毫秒不新增、继续26.5、重复启停/单次Tick及临时按钮Collapsed通过；运行时文字写已取消，停止时文字写正在执行，与真实状态相反。 |
| 3 | 通过 | 运行清空禁用、停止恢复可点，正常按钮清空后重启24，方法运行保护有效，默认/最小布局可读，运行中关闭停止，原exe退出0。 |

当前只需修[UpdateSamplingControls](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Task027Window.xaml.cs)：if里定时器运行，应设置SamplingStateText.Text为“采集中”；else里停止，应写“已停止”，只改两处文字，清空按钮现有运行False/停止True保持。

修后重新运行，通过正式启动/停止按钮观察状态文字与自动新增/停止保持对应；此前已验证的核心采集与清空实现保留，说明见[周期通知Q1](../questions/wpf-timer.md)。

真实命令：dotnet build WpfBae/WpfBae.csproj --nologo退出0、0警告/错误；dotnet run --project WpfBae/Exercises/TASK-027-start-stop-sampling/Verification/Recheck01.csproj -- docx/tasks/evidence/TASK-027/recheck-01退出0表示检查执行完成，结果2项失败均指文字条件相反；本任务launch.ps1启动原exe正常关闭退出0。

证据：[复查说明](./evidence/TASK-027/recheck-01/README.md)、[29项结果](./evidence/TASK-027/recheck-01/results.json)、[源码与程序集核对](./evidence/TASK-027/recheck-01/verification.json)、[启动关闭](./evidence/TASK-027/recheck-01/launch.json)，源码快照及截图保留；以下首次审查的清空/临时按钮问题已修复，不覆盖最新结论。

## 首次审查结论（2026-10-14，历史）

状态保持🟡 进行中，DoD1通过，DoD2/3未通过；29项技术检查23通过、6失败，无运行异常，59项原源码哈希不变，未代改学员实现，不生成下一任务。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 通过 | 原项目构建0警告/错误，原编译窗口实际打开，初始不采集，自动取得24/26.5/25/28/24，同集合完整表格/Count/最新三点及原值标签一致，只读无新增行。 |
| 2 | 未通过 | 正式停止等待2250毫秒无新增，恢复得到26.5，重复启停安全且单次Tick追加正确；但临时观察入口仍可见，启动定时器后文字仍显示已取消，界面状态不同步。 |
| 3 | 未通过 | 默认960×680/最小860×640可读，运行中关闭停止且原exe退出0；但采集中清空可点并实际删除历史，停止后反而禁用，正常清空与恢复入口未通过。 |

第一个修复只看[后台UpdateSamplingControls](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Task027Window.xaml.cs)：运行分支应让ClearButton不可点，停止分支应让它可点，先重新运行观察“启动→清空变灰，停止→清空恢复”。

接着完成ClearButton_Click的运行状态保护，并在XAML隐藏临时ObserveButton；停止后的实际按钮清空与重新启动24必须重新验证，直接绕过禁用按钮调用方法只算诊断，不能代替正常界面操作通过。

机制说明已增量写入[周期通知Q1](../questions/wpf-timer.md)，同名IsEnabled分别控制定时器运行与清空按钮可用性；Q1保持待复习，保留已验证的Tick连接/单次追加/正式启停局部实践证据。

证据：[审查说明](./evidence/TASK-027/review-01/README.md)、[29项结果](./evidence/TASK-027/review-01/results.json)、[哈希与程序集核对](./evidence/TASK-027/review-01/verification.json)、[原exe启动关闭](./evidence/TASK-027/review-01/launch.json)；独立审查源码位于本任务Verification/TaskReview.cs.txt，工程TaskReview.csproj，无新增依赖。

真实运行命令：dotnet build WpfBae/WpfBae.csproj --nologo退出0、0警告/错误；dotnet run --project WpfBae/Exercises/TASK-027-start-stop-sampling/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-027/review-01退出0表示检查执行完毕，结果内6项业务检查未通过，不代表DoD全过；本任务launch.ps1启动原exe并正常关闭退出0。

教练审查工程初次Path名称冲突已补别名后重新运行，不记学员错误；下方核心留TODO/未验收为开启时历史，不覆盖当前已验证范围。

## 开启时可动手入口（2026-10-14，历史）

用户已明确开启，专属目录此前不存在，现已创建下方文件；原TASK-026作品保留，以下为当前入口，后面的仅规划说明是开启前历史。

- 专属目录：WpfBae/Exercises/TASK-027-start-stop-sampling/。
- [窗口](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Task027Window.xaml)：提供启动/停止/临时观察按钮、状态/通知次数文字、原表格/计数/绘图区，默认960×680、最小860×640。
- [后台](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Task027Window.xaml.cs)：提供一个定时器对象与一秒间隔、原集合连接/绘图、样例追加辅助方法、手动添加/清空和关闭收尾。
- [启动验证项目](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Verification/StartupReview.csproj)、[验证源码](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Verification/StartupReview.cs.txt)、[原exe启动验证脚本](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Verification/launch.ps1)：仅由教练用于验证骨架，不计学员核心实现。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml)的StartupUri切换为Exercises/TASK-027-start-stop-sampling/Task027Window.xaml。
- 共享项目例外：[WpfBae.csproj](../../WpfBae/WpfBae.csproj)只增加对本任务Verification生成的.cs文件的编译排除，独立验证输出移至docx/tasks/evidence/TASK-027/startup/，避免.NET递归编译导致重复程序集特性；没有新增依赖。

必要说明见[周期通知Q1](../questions/wpf-timer.md)，先解释定时器/时间长度、事件登记/启动、通知次数与温度历史，再动手，不把骨架中出现的框架API认定为已掌握。

| TODO | 实际位置与动作 | 实践 / DoD |
| :--- | :--- | :--- |
| 1a | 后台构造方法中只登记一次SamplingTimer_Tick到原定时器Tick。 | 实践1 / DoD2 |
| 1b | 后台SamplingTimer_Tick先只累加_tickCount并设置TickCountText.Text。 | 实践1 / DoD1的通知链准备 |
| 2a/2b | 正式启动/停止方法操作原定时器并调用UpdateSamplingControls，重复操作安全。 | 实践2/3 / DoD2 |
| 3 | SamplingTimer_Tick每次调用一次已提供的AppendNextSample。 | 实践2 / DoD1 |
| 4 | UpdateSamplingControls按定时器运行状态设置状态文字、清空可用性和启停按钮状态。 | 实践2/4 / DoD2/3 |
| 5 | ClearButton_Click开头加入运行状态保护，保留停止后原清空与样例复位。 | 实践4 / DoD3 |
| 收尾 | 正式启停完成后在XAML给ObserveButton设置Visibility="Collapsed"，移除临时观察入口的显示。 | 实践4 / DoD2/3的最终状态一致性 |

### 第一个可观察动作

只做后台TODO 1a和1b，代码与逐行说明见[周期通知Q1](../questions/wpf-timer.md)；此时不要求连接自动温度追加，也不要求一次写完正式启停状态。

在工作区根目录运行：

~~~powershell
dotnet run --project WpfBae/WpfBae.csproj
~~~

点击“观察通知（临时）”，预期通知次数约每秒从0增加为1、2、3；再次点击同一按钮停止，次数保持，完整历史仍为0且图为空，正式“启动”和“停止”方法目前留TODO所以尚无动作。

完成首步后可回复观察现象，之后再做TODO 2～5；完成全题时请求“检查TASK-027”，验收以实际至少五条、停止跨两个间隔、继续样例、重复启停、清空限制/恢复24与窗口退出证据为准。

### 教练骨架验证

实际执行dotnet build WpfBae/WpfBae.csproj --nologo，初次构建有_tickCount未读取的2个警告，教练补初始次数显示后最终构建退出0、0警告/错误。

实际执行dotnet run --project WpfBae/Exercises/TASK-027-start-stop-sampling/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-027/startup，退出0，原编译窗口Show成功，31项骨架检查通过；包括初始停止/单一集合、一秒间隔、手动添加/计数、默认与最小尺寸布局、停止清空、临时观察启停和运行中关闭停止。

独立验证工程首次引用路径不正确并产生MSBuild警告，教练修正引用与输出路径后验证成功；遗留的验证obj生成.cs曾被主项目递归编译而造成9个重复特性错误，教练增加专属排除后再次主项目构建0警告/错误，这些属于教练验证工程问题，不记学员错误。

实际通过本任务launch.ps1启动WpfBae.exe，窗口标题为TASK-027：启动和停止模拟采集，正常关闭退出0；结果见[骨架检查](./evidence/TASK-027/startup/results.json)、[原exe启动关闭](./evidence/TASK-027/startup/launch.json)、[默认截图](./evidence/TASK-027/startup/default.png)、[最小截图](./evidence/TASK-027/startup/minimum.png)。

这些检查只验证教练骨架，核心TODO尚未完成，三个DoD均未勾选，当前为🟡 进行中，不新增学员自主掌握证据或下一任务。

## 开启时的准备约定（规划历史）

本卡仅规划，未创建TASK-027源码或切换入口；用户明确开启时先核对当前任务、[TASK-026窗口](../../WpfBae/Exercises/TASK-026-history-and-chart/Task026Window.xaml)、[后台](../../WpfBae/Exercises/TASK-026-history-and-chart/Task026Window.xaml.cs)及[表格问答](../questions/wpf-datagrid.md)、[坐标问答](../questions/wpf-charts.md)、[事件问答](../questions/csharp-events.md)，再检查拟用目录 `WpfBae/Exercises/TASK-027-start-stop-sampling/`，不覆盖已有作品。

教练准备独立窗口、启停按钮、状态文字、沿用的表格/计数/绘图区与原集合，提取已有样例追加为普通辅助方法，并提供关闭停止和解除登记的收尾代码；原TASK-026保留，最小尺寸至少沿用已验证860×600，骨架布局问题由教练自行修复验证。

核心定时器连接、启动/停止调用、周期接收方法触发已有样例追加、启停状态与清空限制留TODO；开启后才登记实际文件、TODO与DoD映射、首个动作和真实运行结果，当前不链接拟建文件。

## 使用前必须先解释

1. 先指出本窗口里真实的定时器对象：DispatcherTimer是WPF提供的工具，Interval指定期望通知间隔，Tick是到时发出的事件；不是温度集合，也不是另一个后台采集线程。
2. 先用完整普通C#写法解释对象创建、字段保存、TimeSpan表示时间长度，再解释Interval赋给哪个对象，不能把这些API和自定义字段名视为已掌握。
3. 区分登记Tick接收方法与调用Start：前者告诉定时器通知谁，后者开始安排后续通知；Stop停止后续通知，不清空原历史，也不重置样例位置。
4. 用当前普通方法解释一次Tick如何追加一条原样例，再由原集合变化更新表格/Count/图，不为三个显示区域另建温度副本。
5. 说明通知在界面线程运行、繁忙时可能延后，本题仅添加模拟数值，不能由此推定已学会真实设备阻塞读取或精确采样。
6. 新增的状态文字控件、启动/停止按钮名字与样例辅助方法均由教练自定义，逐个说明具体对象和作用；一次只要求一个可观察改动。

## 实践与产出

1. 明确开启并完成必要说明后，先连接一次Tick到普通接收方法，临时只更新通知次数，运行点击骨架启动路径观察次数约每秒增加；此时尚未要求同时完成所有按钮状态和采集连接。
2. 接收方法每次调用已有样例追加方法，再完成启动/停止和采集状态，核对表格、计数、最新三点来自同一集合。
3. 连续运行至少取得五条，停止后等待至少两个约定间隔，确认计数不再增加；再次启动应接着下一条样例，重复点击启动不使单次Tick追加多条。
4. 采集中清空按钮禁用，停止后可清空；清空后重新启动恢复24，关闭窗口正常退出，默认与最小尺寸可读。

## 验收标准（DoD）

- [x] 构建并实际打开TASK-027，初始不采集；启动后定时追加到原温度集合，实际取得至少五条，表格完整历史/真实计数/最新三点和原值标签一致，表格只读无新增空行。
- [x] 实际停止后跨越至少两个约定间隔不新增，再启动从原样例位置继续；重复启动和重复停止安全，始终一份定时器及一次接收方法登记，界面启停状态显示正确。
- [x] 采集中清空禁用，停止后实际清空并重新启动恢复24，关闭后正常退出，0运行异常，默认与约定最小尺寸各区域可读。

## 状态与证据

- 2026-10-14最终复查：教练按用户要求修正两处状态文字与骨架过期说明后，实际29项检查全通过，三个DoD勾选，状态✅ 已通关；已记录协助边界，仅创建TASK-028未开始卡片，原入口保持。

- 2026-10-14第一次复查：DoD1/3通过，2仅运行/停止状态文字相反未通过，状态 🟡 进行中；29项检查27通过、59项原源码未改，清空和临时入口问题已修复，详见上方当前结论，前次证据保留。

- 2026-10-14隐藏按钮追问：已在周期通知Q1补原ObserveButton标签添加Visibility="Collapsed"的实际写法与效果，仅答疑/文档同步，未代改源码或重新验收，DoD与任务状态保持。

- 2026-10-14首次实际审查：DoD1通过并勾选，2/3未通过，状态 🟡 进行中；正常采集与停止/恢复/重复启停通过，清空条件反向与临时观察入口状态不同步待修复，详见上方当前结论。

- 2026-10-14：用户明确开启TASK-027，独立骨架与入口已准备，教练实际验证默认/最小布局及原exe正常退出；正式周期追加与启停等仍留TODO，状态 🟡 进行中，DoD均未勾选，首步及证据见上方当前入口。

- 2026-10-14：TASK-026最终复查三个DoD通过后，仅创建本卡，状态 ⚪ 未开始；未准备新源码或切换入口，App.xaml继续启动TASK-026，既有C#/WPF待复习范围保留，不宣称阶段通关。
