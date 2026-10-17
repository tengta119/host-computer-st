# TASK-028：显示模拟温度超限提示

- 阶段：第二阶段：WPF（起步）。
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)的上位机“参数设置 / 曲线 / 报警”与第二阶段设备监控界面目标。
- 选择依据：TASK-027已验证可启停的周期模拟采集、同集合历史/图表和清空恢复，下一步以最新温度驱动一个可观察的报警提示，巩固数据到界面状态的连接；不据此宣称WPF阶段完成。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

沿用24、26.5、25、28的样例及最新温度，练习约定27 °C为提示阈值，这只是模拟练习数值，不代表真实设备安全限值。

没有历史时显示“暂无温度”；最新温度小于27时显示“温度正常”，达到或超过27时显示“温度超限”；后续24到来恢复正常，停止保留最新提示，停止清空后恢复暂无温度。

本题只做文字提示，保留已有启停、表格与图表，不增加真实设备通信、后台线程、声音、报警记录或新框架。

## 开启时准备约定

2026-10-17 已按用户要求开启，创建独立任务目录并切换 App.xaml 到 Task028Window，原 TASK-027 文件保留；下方建卡记录为历史。

教练准备独立窗口及报警TextBlock、复用已验证集合/追加/绘图/启停，先解释最新一条温度是谁、Count与最后下标的对应、这个TextBlock显示什么，再预留判断/显示和清空复位TODO。

首个动作只连接非空历史的最新温度到提示文字，手动添加24后观察提示显示24，随后再完成阈值判断；具体真实路径与运行入口在开启时登记，不提前链接拟建文件。

## 实践与产出

1. 找到原集合中的最新一条并显示，集合为空时先处理，不能读取不存在的最后元素。
2. 根据27的练习阈值判断正常/超限；提示读取原温度，不读取换算后的y或图中槽位。
3. 实际观察24/26.5/25正常、28超限、下一条24恢复，停止保留，清空恢复暂无温度。

## 验收标准（DoD）

- [x] 构建并实际打开TASK-028，初始显示暂无温度，手动样例24/26.5/25/28/24依次显示正常/正常/正常/超限/正常，提示依据同一集合的最新原温度。
- [x] 自动采集时提示随最新温度变化，停止后历史与提示保持，停止清空后恢复暂无温度，重新启动从24开始且显示正常，空集合无异常。
- [x] 原启停、清空限制、完整表格/Count/最新三点保持正确，窗口默认与约定最小尺寸可读，实际运行关闭正常。

## 状态与证据

- 2026-10-14：TASK-027三个DoD完成后仅创建本卡，状态⚪ 未开始；未准备源码、未切换入口、未验收，本题不会自动开始。

## 实际文件与运行入口

- 专属目录：`WpfBae/Exercises/TASK-028-temperature-alert/`。
- [Task028Window.xaml](../../WpfBae/Exercises/TASK-028-temperature-alert/Task028Window.xaml)：沿用已完成窗口，新增名为 AlarmText 的 TextBlock，初始显示“暂无温度”。
- [Task028Window.xaml.cs](../../WpfBae/Exercises/TASK-028-temperature-alert/Task028Window.xaml.cs)：复用 TASK-027 的集合、追加、启停、清空保护和绘图，只在 UpdateTemperatureAlert 留本题四个 TODO。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml) 改为启动 TASK-028；[WpfBae.csproj](../../WpfBae/WpfBae.csproj) 只新增排除本题验证目录的编译规则，防止生成文件重复编译。
- [骨架验证源码](../../WpfBae/Exercises/TASK-028-temperature-alert/Verification/StartupReview.cs.txt)与项目放在本题 Verification 内，验证产物位于 evidence/TASK-028/startup。
- 在工作区根目录运行 `dotnet run --project WpfBae/WpfBae.csproj`，关闭旧窗口后再重新运行修改后的程序。

## 先看真实对象，再做第一个改动

窗口右侧表格的温度来自 `_temperatures`，它是已有的原温度集合，元素是 double 数字。
本题新增的 AlarmText 是教练给那个 TextBlock 起的名字，TextBlock 是 WPF 的文字控件，给它的 Text 属性赋值就会改变窗口上的文字。
UpdateTemperatureAlert 是教练新建的普通 C# 方法，不是 WPF 自带 API；骨架已经在初始化和集合改变后调用它，因此手动追加、自动追加、清空都会走到这里，不需要再连接事件。

先区分条数与下标：添加四条后 Count 是 4，下标依次为 0、1、2、3，最后一条的下标就是 Count - 1。
数组和集合的方括号读取写法相似，但这里必须读 `_temperatures`，不能读循环样例数组，也不能读取图中的 y 坐标。

例如，集合已经有三条时，下面代码读取下标 2 的原温度，并把数字写到界面文字中。

```csharp
double temperature = _temperatures[2];
AlarmText.Text = "最新温度：" + temperature + " °C";
```

double 是 C# 的数值类型关键字，temperature 是这段方法里自行命名的局部变量，右边读取的数字保存到这个变量中。
第二行把文字和刚才的数字组合后赋给 AlarmText.Text，影响的是温度提示这一处文字。
示例固定下标 2 仅适用于已有三条的情况，实际 TODO 2 要随条数读取最后一条。

现在只做 TODO 1 和 TODO 2，先不做阈值判断。
进入 UpdateTemperatureAlert 后先检查 Count 是否为 0，空集合时用 return 结束当前方法，否则 Count - 1 会是 -1，读取它会出错。
然后用 Count - 1 读取最新温度，并按上面的文字赋值方式显示它。
运行后先别点启动，点击一次“添加下一条”，预期表格显示 24，提示变为“最新温度：24 °C”；再点一次，预期提示显示 26.5，表格仍保留两条。
当前方法还是空骨架，所以没有实现 TODO 前，即使表格出现 24，提示仍会保持“暂无温度”。

## 后续步骤与 TODO 对应

| TODO | 对应实践与 DoD | 完成后观察 |
| :--- | :--- | :--- |
| 1：空集合先返回。 | 实践 1，DoD 1/2。 | 启动时和空集合清空时都不会读取不存在的元素。 |
| 2：读取最后一条并显示。 | 实践 1，DoD 1。 | 手动添加后提示随 24、26.5 改变。 |
| 3：根据原温度判断并显示。 | 实践 2，DoD 1/2。 | 用 C# 的 >= 比较最新原温度和 27，达到或超过显示“温度超限”，其余显示“温度正常”，28 后的 24 也要更新回来。 |
| 4：在空集合分支复位文字。 | 实践 3，DoD 2。 | 在 TODO 1 的 return 前把提示设回“暂无温度”，停止后清空即可观察。 |

完成第一步后可直接描述实际现象；全部完成后发送“检查 TASK-028”，再按本卡三个 DoD 逐项验收。
停止不改变集合，所以保留最新提示；本题不需要在停止按钮里额外重置提示。
27 °C 只是本题的练习阈值。

## 2026-10-17 开启与骨架验证

- 新建前确认目标目录不存在，原 TASK-027 作品未修改，旧完成注释在新副本内整理为复用说明；本题判断核心留 TODO，不计新增自主掌握。
- 实际执行 `dotnet build WpfBae/WpfBae.csproj --nologo`，退出 0，0 警告、0 错误。
- 实际执行 `dotnet run --project WpfBae/Exercises/TASK-028-temperature-alert/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-028/startup`，退出 0，29 项骨架检查通过，包含实际窗口显示、初始提示、同一原集合、手动追加与计数、停止清空、关闭收尾、默认 960×720 与最小 860×680 的布局边界；这不是本题核心 DoD 验收。
- 实际执行 `& WpfBae/Exercises/TASK-028-temperature-alert/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-028/startup/launch.json`，原 exe 窗口标题为 TASK-028，正常关闭退出 0。
- 证据：[骨架检查](./evidence/TASK-028/startup/results.json)、[原程序启动关闭](./evidence/TASK-028/startup/launch.json)、[默认窗口内容](./evidence/TASK-028/startup/default.png)、[最小窗口内容](./evidence/TASK-028/startup/minimum.png)。
- 三个 DoD 均未勾选，状态 🟡 进行中；本轮不增加问答条数、掌握条目或错题，不创建下一张任务卡。
## 2026-10-17 最终审查：✅ 已通关

| 必需验收项 | 结果 | 实际证据 |
| :--- | :--- | :--- |
| DoD 1：手动样例与最新原温度。 | 通过。 | 手动24/26.5/25/28/24依次正常/正常/正常/超限/正常，同一集合27超限、26.99正常，最新原温度读取正确。 |
| DoD 2：自动变化、停止、清空重启。 | 协助文字收尾后通过。 | 自动五条逐条检查，停止等待2200ms保持历史与提示，继续26.5，停止清空后暂无温度，重启24正常，正常/超限后清空及重复空清空无异常。 |
| DoD 3：原业务、布局和关闭。 | 通过。 | 同一集合表格/Count/最新三点、重复启停、运行清空禁用及方法保护、只读/隐藏临时按钮、默认960×720与最小860×680布局通过，运行中关闭停止定时器，原exe退出0。 |

首次审查104项中100通过，4项失败均为清空后提示残留；学员已正确实现Count为空先返回、Count-1取最新原温度、>=27判断及else恢复正常。
教练按已有“状态文案低影响收尾直接修正”授权，仅在空集合分支return之前补一行AlarmText.Text赋值为“暂无温度”，未修改学员最新下标、阈值判断和原启停核心实现。
这行复位不计学员自主掌握，未凭一次遗漏推定典型认知错误，不新增错题。

最终重新构建0警告/错误，完整实际窗口审查104项全部通过、0运行异常；原exe实际打开TASK-028并正常关闭退出0。
最终审查前后本题后台源码哈希一致，审查加载DLL与原程序DLL一致，实际显示原编译窗口并执行按钮事件/等待定时通知，不使用设计预览代替运行。
实际命令：dotnet build WpfBae/WpfBae.csproj --nologo。
实际命令：dotnet run --project WpfBae/Exercises/TASK-028-temperature-alert/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-028/review-final。
实际命令：& WpfBae/Exercises/TASK-028-temperature-alert/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-028/review-final/launch.json。
证据：[首次结果](./evidence/TASK-028/review-01/results.json)、[最终104项结果](./evidence/TASK-028/review-final/results.json)、[原exe运行](./evidence/TASK-028/review-final/launch.json)、[源码与DLL核对](./evidence/TASK-028/review-final/verification.json)、[超限画面](./evidence/TASK-028/review-final/over-limit.png)、[最小窗口](./evidence/TASK-028/review-final/minimum.png)。
三个DoD已勾选，下一张仅创建[TASK-029：设置温度报警阈值](./TASK-029.md)（⚪ 未开始），依据计划参数设置/报警目标推进，未准备下一题源码或切换入口，WPF阶段仍进行中。
上文空骨架和未验收描述为开启时说明，最新状态以本节为准。