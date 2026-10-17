# TASK-030：用 JSON 保存并恢复报警阈值

- 阶段：第二阶段：WPF（起步，结合C#配置复习）。
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)的文件/JSON/配置、参数设置与设备监控界面目标。
- 选择依据：TASK-029已验证输入检查、当前有效阈值及立即刷新，但重启后仍回初始27；复用TASK-006配置经验，让一个已经生效的监控参数跨启动保留，不据此宣称WPF阶段完成。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

用户应用26后，点击“保存阈值”，把当前有效阈值写入本题专用JSON配置文件。
关闭并重新打开本题窗口后，加载这个数值，输入框与当前有效阈值同步为26，后续读数按26判断。
保存的是已成功应用的字段值，输入框里尚未应用或被拒绝的文字不应写进配置。
没有文件时使用27，文件损坏或保存内容不在20到30范围时给出可读说明并回退27，程序可继续使用；启动加载失败不覆盖原文件。
本题只保存这一个参数，不保存历史，不引入真实设备、数据库、新框架或复杂后台读写。

## 开启时准备约定

2026-10-17已按用户明确要求开启，独立骨架已准备，App.xaml切到TASK-030；下方仅建卡片为历史。
用户明确开启后读取最新源码、TASK-006记录与当前JSON问答，确认可安全使用的独立任务目录及专用配置路径，保留原作品。
教练提供独立窗口、保存按钮、配置结果显示区、最小自定义配置类、文件路径与加载/保存方法签名，核心JSON读写及合法性检查留TODO。
先分别解释配置类保存哪个值、序列化如何生成文件文字、反序列化如何还原对象，不因文件API曾出现就默认掌握全部机制。
首个动作只让保存按钮显示当前有效字段值，应用26后观察保存入口读取26，尚未应用的输入文字不影响它；随后再写文件。
实际文件路径、启动加载时机、每个TODO及DoD对应在开启时登记，不提前链接拟建文件。

## 实践与产出

1. 从当前有效字段保存单个阈值到本题JSON文件，保存成功/失败均给出可读说明。
2. 启动时加载并验证，成功后同步字段、输入文字和当前有效值显示，历史为空时仍暂无温度。
3. 缺失、损坏、非法配置及保存失败可恢复，保留原采集/应用逻辑。

## 验收标准（DoD）

- [x] 实际窗口应用26并保存后，JSON中保存26，关闭重开后输入框/有效阈值均26，读取26.5超限；输入框改为29但不应用时保存仍写26，拒绝非法输入后保存也不写入错误值。
- [x] 缺失文件使用27，损坏JSON和范围外配置给出说明并回退27且不改坏文件，空历史无异常；合法配置恢复后可正常读取，无法写入时显示失败而不是成功，程序继续可用。
- [x] 原有效输入/NaN拒绝/立即刷新、采集中应用、启停/清空保护、表格/Count/最新三点保持，默认与约定最小尺寸可读，两次实际启动关闭正常。

## 状态与证据

- 2026-10-17：TASK-029三个DoD通过后仅创建本卡，状态⚪ 未开始；未创建本题源码、配置文件或切换入口，未运行验收。
## 实际文件与配置路径

- 专属目录：WpfBae/Exercises/TASK-030-threshold-settings/。
- [Task030Window.xaml](../../WpfBae/Exercises/TASK-030-threshold-settings/Task030Window.xaml)：复用TASK-029最终界面，新增SaveThresholdButton、ConfigStatusText、ConfigPathText，默认960×880、最小860×840。
- [Task030Window.xaml.cs](../../WpfBae/Exercises/TASK-030-threshold-settings/Task030Window.xaml.cs)：复用原有效字段/应用验证/立即刷新/集合/表格/折线/启停，提供_settingsPath、保存入口和加载方法，构造方法末尾调用LoadThresholdSettings一次，核心JSON逻辑留TODO。
- [AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-030-threshold-settings/AlarmThresholdSettings.cs)：教练提供的最小配置类，公开double属性Threshold保存单个数值。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml)只切换StartupUri；[WpfBae.csproj](../../WpfBae/WpfBae.csproj)只排除本题Verification生成源码，防止重复编译，不新增包或框架。
- [骨架验证项目](../../WpfBae/Exercises/TASK-030-threshold-settings/Verification/StartupReview.csproj)和[验证源码](../../WpfBae/Exercises/TASK-030-threshold-settings/Verification/StartupReview.cs.txt)放在本题独立Verification内。
- 正常程序的运行时配置目标为AppContext.BaseDirectory/Exercises/TASK-030-threshold-settings/alarm-threshold.json，默认Debug对应WpfBae/bin/Debug/net10.0-windows/Exercises/TASK-030-threshold-settings/alarm-threshold.json。
- 默认Debug绝对目标：D:/host-computer-st/WpfBae/bin/Debug/net10.0-windows/Exercises/TASK-030-threshold-settings/alarm-threshold.json；这是运行后未来保存的数据路径，不是源码文件，本轮未创建配置文件。
- 窗口显示真实路径，验证工程的BaseDirectory在证据目录下，与原exe不同；只以实际运行窗口显示的路径定位本次文件，不操作TASK-006或其他配置。

## 首个可观察动作

打开Task030Window.xaml.cs，找到SaveThresholdSettings中的TODO 1，先将保存占位说明改为显示_alarmThreshold当前值。
ConfigStatusText是配置结果文字控件，设置Text可以显示“准备保存的有效阈值：”与字段值；这一步不写文件。
在工作区根目录执行dotnet run --project WpfBae/WpfBae.csproj，先输入26并点“应用阈值”，再点“保存阈值”，预期显示准备保存的有效阈值26。
把输入框改成29但不点应用，再点保存，预期仍显示26；这个结果说明保存入口使用已生效字段而不是尚未应用的输入文字。
先只做这一处修改，观察通过后再接JSON；完整必要说明见[JSON配置Q2](../questions/csharp-json.md#q2task-030-怎样把当前有效阈值保存到文件再在启动时恢复)。

## TODO与实践/DoD对应

| TODO | 对应 | 可观察结果 |
| :--- | :--- | :--- |
| 1：保存入口先显示当前有效字段。 | 实践1，DoD 1的起步。 | 应用26后显示26，输入未应用29仍显示26，尚不写文件。 |
| 2：创建配置对象、序列化、创建父目录并写文件，处理保存失败。 | 实践1，DoD 1/2。 | JSON保存26且可定位，失败显示失败，不假报成功。 |
| 3：加载时先处理文件不存在。 | 实践2，DoD 2。 | 缺失时保留初始27，说明原因并返回，不创建默认文件。 |
| 4：读文件、反序列化和合法性/异常保护。 | 实践2，DoD 2。 | 损坏/null/范围外配置不应用且保留原文件，合法配置随后可恢复。 |
| 5：合法加载后同步有效字段、输入框及两种显示。 | 实践2/3，DoD 1/3。 | 重启后恢复26，输入与当前值一致，空历史仍暂无温度，26.5超限。 |

不要在加载前调用保存，不把未应用的输入文字写入配置，不把新的配置类当作WPF自动绑定对象。
JSON格式合法不等于阈值有效，继续复用已讲的20到30与NaN判断；ToString、Deserialize和自定义类的对象关系已在Q2分步说明。
全部完成后发送“审查 TASK-030”，再按本卡三个DoD逐项验收。

## 2026-10-17 开启证据

- 开始时确认目标目录无作品，创建三个专属源码文件与验证入口，保留TASK-029全部原文件；配置读写核心留TODO，不新增自主掌握或错题。
- 教练自检修正了新副本里不应在集合变化时调用加载的连接，并补充“追加不重复加载”检查，最终只在构造方法调用一次；此为骨架修复，不作为学员步骤。
- 实际dotnet build WpfBae/WpfBae.csproj --nologo退出0、0警告/错误。
- 实际dotnet run --project WpfBae/Exercises/TASK-030-threshold-settings/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-030/startup退出0，最终50项实际窗口检查全部通过，包含真实路径、无配置写入、复用应用26/原集合/手动追加、停止清空、默认/最小布局边界与关闭收尾。
- 实际& WpfBae/Exercises/TASK-030-threshold-settings/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-030/startup/launch-final.json退出0，原exe显示TASK-030、正常关闭退出0。
- 最终证据：[骨架检查](./evidence/TASK-030/startup/results.json)、[原exe运行](./evidence/TASK-030/startup/launch-final.json)、[默认内容](./evidence/TASK-030/startup/default.png)、[最小内容](./evidence/TASK-030/startup/minimum.png)；早期launch.json为骨架自检修复前运行记录。
- 三个DoD均未勾选，状态🟡 进行中；保存/加载目前只显示占位说明，未创建真实配置文件，不创建TASK-031，不把骨架可运行当作核心验收通过。
## 2026-10-17 审查：🟡 进行中

| 必需项 | 结果 | 实际证据 |
| :--- | :--- | :--- |
| DoD 1：保存当前有效阈值并重启恢复。 | 通过。 | 应用26并保存为JSON，未应用输入29及拒绝abc后保存仍26，关闭旧窗口并新建原编译窗口后恢复26/输入文字/有效值显示，26.5超限，空历史暂无温度。 |
| DoD 2：缺失/损坏/非法配置与失败保护。 | 未通过。 | 缺失保留27、损坏JSON保留27和原文件、写入文件被锁时显示失败并继续可用、合法配置恢复均通过；但null造成窗口构造时NullReferenceException，{}成为0，35和19.9直接成为有效阈值且显示加载完成，没有合法性拒绝。 |
| DoD 3：原业务、布局与运行。 | 通过。 | 原阈值应用/NaN拒绝/立即刷新/自动采集、停止2200ms保持/清空重启、同集合表格/Count/最新三点、默认960×880与最小860×840布局边界通过，当前原用户配置下原exe两次正常启动关闭退出0。 |

保存/加载专项使用独立审查程序的输出目录，只在该目录创建配置与模拟文件锁，不覆盖或改写原用户配置；原exe两次启动前后用户配置哈希保持一致。
教练仅将读取失败提示从ConfigPathText.Text改为ConfigStatusText.Text，保留原实际路径显示；null与数值校验核心均未代改。
首次检查对加载有效值文字缺少空格产生一项格式误报，已修正为忽略空格、按实际数值验收，未要求学员改文字排版。
最新结果为146条记录：138项检查通过、7项检查失败及1条null异常详情，失败集中在空对象和配置值未验证两个问题，不是八处独立缺陷。
重新编译0错误、4条警告（两种可空警告CS8600/CS8602在WPF临时项目与主项目重复报告），与反序列化结果可能为空及访问其属性的实际问题一致。
复测期间三个本题源码哈希保持，审查DLL与原程序DLL一致，原用户配置不变。

### 先只修复null对象保护

打开Task030Window.xaml.cs的LoadThresholdSettings，在反序列化成功后、第一次访问alarmThresholdSettings.Threshold之前，先判断alarmThresholdSettings是否为null。
文件内容只有null时，它是合法JSON，Deserialize可能直接返回null而不抛异常；此时没有可供读取Threshold的配置对象。
所以catch不会因为返回null自动运行，而你现在访问Threshold的语句又在try/catch外，会在那里抛出NullReferenceException。
将接收变量声明为AlarmThresholdSettings?可表达它允许接到空值，但问号本身不做保护，仍要在if分支显示配置无效并return，保留窗口初始27和原文件。
先在原窗口显示的本题配置路径备份已有合法文件，临时用内容null检查；预期可正常打开、有效值27、提示配置无效，文件仍是null，随后恢复原合法文件。
审查已在隔离配置中复现，不要求改动其他任务配置或原用户文件才能理解。

### 随后补上配置数值检查

对象不为空以后，还要验证它的Threshold，确认20到30且合法后再赋给_alarmThreshold。
{}可以正常还原成对象，但未设置的double属性值是0；{"Threshold":35}和{"Threshold":19.9}也都是合法JSON，却不是本题合法阈值。
复用TASK-029已解释的范围与特殊值判断，失败先显示原因并return，不能先给_alarmThreshold或输入框赋非法值。
将对象存在与数值合法检查放在“给窗口字段/输入框赋值”之前，两个检查都通过才执行现有成功同步。
修复后复查null、{}、35、19.9都保留27和原文件，合法26随后能恢复，再发送“审查 TASK-030”。

实际命令：dotnet build WpfBae/WpfBae.csproj --nologo。
实际命令：dotnet run --project WpfBae/Exercises/TASK-030-threshold-settings/Verification/TaskReview02.csproj -- docx/tasks/evidence/TASK-030/review-02。
实际命令：& WpfBae/Exercises/TASK-030-threshold-settings/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-030/review-02/launch-1.json。
第二次以相同命令将OutputPath设为docx/tasks/evidence/TASK-030/review-02/launch-2.json。
证据：[初查结果](./evidence/TASK-030/review-01/results.json)、[最新结果](./evidence/TASK-030/review-02/results.json)、[源码/DLL/用户配置核对](./evidence/TASK-030/review-02/verification.json)、[原exe首次](./evidence/TASK-030/review-02/launch-1.json)、[原exe再次](./evidence/TASK-030/review-02/launch-2.json)、[最小窗口](./evidence/TASK-030/review-02/minimum.png)。
TASK-030继续🟡 进行中，DoD1/3已勾选、2未勾选，不创建TASK-031；JSON配置Q2整体继续待复习，保留保存/还原/缺失损坏/写入失败处理等已验证局部应用证据，不扩大为完整加载校验已掌握。
## 2026-10-17 再次审查：只剩非法配置赋值顺序

- 最新结论：TASK-030仍🟡 进行中，DoD1/3通过、2未通过。
- 已自主修复并实际通过：反序列化返回null时显示配置为空并return，窗口可打开、有效阈值27、原文件保持；范围外配置也已有错误说明。
- 剩余实际问题：先执行_alarmThreshold = alarmThresholdSettings.Threshold，再检查_alarmThreshold是否越界，虽然显示非法并return，字段已经被改成0/35/19.9，未保留默认27。
- 最新147项检查144通过、3项失败，失败为{}、35、19.9的有效字段未回退27；无未处理运行异常，原保存/合法恢复/缺失损坏/写入失败/原业务及布局保持。
- 本轮教练只把“_alarmThreshold不合法”改成“配置中的报警阈值不合法。”，避免界面暴露代码字段名；没有代改判断、赋值顺序或其他核心实现，不能将此文案整理计入学员自主掌握。
- 测试按“配置为空”和“不合法”等同义有效说明验收，没有要求特定错误文字；未因文案不同误判空值保护。

### 这次只调整一个关系：先检查候选值，再更新有效字段

Task030Window.xaml.cs第209行当前先把文件里的值写入_alarmThreshold，第210行才验证它。
条件应检查alarmThresholdSettings.Threshold是否越界，失败就说明并return。
原来的_alarmThreshold = alarmThresholdSettings.Threshold这行移到该if保护之后，只有通过检查才执行，再保留下方已有输入框/有效值/报警文字同步。
只移动赋值而仍用_alarmThreshold做条件也不正确，因为那时字段还是默认27，检查的不是读回值。
return只结束后续执行，不撤销已经执行的赋值，也不会自动把字段恢复成27。
按此顺序，35只留在配置对象中，会被拒绝；真正用于采集判断的_alarmThreshold保持27。
预期{}、35、19.9均显示配置无效且有效字段/输入框/显示为27，配置文件不变；合法26仍能恢复。

实际构建退出0、0错误、2条CS8600警告（同一可空接收声明在WPF临时项目和主项目重复报告），原先CS8602解引用警告已因学员空值保护消失。
接收变量可声明为AlarmThresholdSettings?以表达Deserialize可能返回null，现有空值判断仍需保留；这是可空声明整理，不把它与本次赋值顺序混为同一缺陷。
独立审查仍只修改自身输出目录配置，原用户配置前后哈希一致，原exe两次正常启动关闭退出0，三个本题源码验证期间哈希保持、原/审查DLL一致。

实际命令：dotnet build WpfBae/WpfBae.csproj --nologo。
实际命令：dotnet run --project WpfBae/Exercises/TASK-030-threshold-settings/Verification/TaskReview03.csproj -- docx/tasks/evidence/TASK-030/review-03。
两次实际运行：& WpfBae/Exercises/TASK-030-threshold-settings/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe，OutputPath分别为docx/tasks/evidence/TASK-030/review-03/launch-1.json和launch-2.json。
证据：[147项结果](./evidence/TASK-030/review-03/results.json)、[源码/DLL/原配置核对](./evidence/TASK-030/review-03/verification.json)、[原exe首次](./evidence/TASK-030/review-03/launch-1.json)、[原exe再次](./evidence/TASK-030/review-03/launch-2.json)。
不创建TASK-031，不撤销已验证的null保护/范围识别/JSON保存和恢复等局部能力，JSON配置Q2继续待复习，前文未检查null/没有错误提示等结论为首次历史。
## 2026-10-17 最终复查：✅ 已通关

| 必需项 | 最终结果 | 实际证据 |
| :--- | :--- | :--- |
| DoD 1：有效值保存和重启恢复。 | 通过。 | 有效26写入JSON，未应用29及拒绝abc后仍保存26，关闭后新窗口恢复字段/输入/显示26，26.5超限，空历史暂无温度。 |
| DoD 2：文件与配置合法性保护。 | 通过。 | 缺失/损坏/null/{}/35/19.9全部保留27，非法文件不变且有说明；合法26恢复；文件锁模拟写入失败时显示失败且窗口继续可用。 |
| DoD 3：原业务、布局和两次运行。 | 通过。 | 阈值/NaN/立即刷新、采集启停/清空、同集合表格/Count/最新三点保持，默认960×880与最小860×840布局通过，原exe两次正常启动关闭退出0。 |

学员已自主将范围条件改为检查alarmThresholdSettings.Threshold，并在通过后才赋_alarmThreshold，null保护保持；本轮没有修改学员源码。
最终147项实际检查全部通过，0未处理运行异常；普通非法输入、JSON损坏与文件锁产生的异常已由学员catch处理，不是程序崩溃。
构建0错误，余2条CS8600警告（同一条可空接收声明在WPF临时项目与主项目重复报告）；已实际验证null保护，声明是否标?不阻塞本卡DoD，不要求为此反复提交。
原用户配置文件前后哈希不变，异常文件与锁只在独立审查目录中构造，三个本题源码复查期间哈希一致，审查引用DLL与原程序DLL一致。
前轮教练协助只涉及读取失败提示显示目标与非法配置说明文案，核心读写、null判断及检查后赋值都为学员实现，不将文案协助计为自主掌握。

实际命令：dotnet build WpfBae/WpfBae.csproj --nologo。
实际命令：dotnet run --project WpfBae/Exercises/TASK-030-threshold-settings/Verification/TaskReviewFinal.csproj -- docx/tasks/evidence/TASK-030/review-final。
两次实际运行：& WpfBae/Exercises/TASK-030-threshold-settings/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe，OutputPath分别为docx/tasks/evidence/TASK-030/review-final/launch-1.json和launch-2.json。
证据：[147项结果](./evidence/TASK-030/review-final/results.json)、[源码/DLL/原用户配置核对](./evidence/TASK-030/review-final/verification.json)、[原exe首次](./evidence/TASK-030/review-final/launch-1.json)、[原exe再次](./evidence/TASK-030/review-final/launch-2.json)、[默认内容](./evidence/TASK-030/review-final/default.png)、[最小内容](./evidence/TASK-030/review-final/minimum.png)。
三个DoD全勾选，TASK-030✅ 已通关；[JSON配置Q2](../questions/csharp-json.md#q2task-030-怎样把当前有效阈值保存到文件再在启动时恢复)限本骨架中的配置对象、JSON读写、保护与成功同步自主应用标已掌握，不扩展为完整文件系统/异常体系/配置框架设计。
按计划文件/数据采集监控目标只创建下一张[TASK-031：导出完整温度历史为CSV](./TASK-031.md)（⚪ 未开始），没有准备源码或切换入口，WPF阶段仍进行中；上文失败和待修复结论为此前历史。