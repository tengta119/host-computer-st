# TASK-031：导出完整温度历史为 CSV

- 阶段：第二阶段：WPF（起步，结合C#文件与集合复习）。
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)的文件处理、数据采集/监控与设备监控界面目标；CSV导出是据此设计的练习，不是计划原文逐项指定的内容。
- 选择依据：TASK-030已验证单个有效参数的JSON保存恢复，下一步把监控表格的全部历史作为结果文件导出，巩固同一集合、序号及文件写入，保留图表只显示最新三点的差异。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

用户停止采集后点击“导出历史”，将当前全部温度记录写到本题专用CSV文件。
CSV是按行保存表格的文字文件，本题表头固定为Sequence,TemperatureC，每条记录一行，序号从1开始，温度使用小数点。
导出的是原温度集合全部记录，顺序与历史表格一致，不只导出图中最新三点。
没有历史或仍在采集时不执行导出，显示原因，不改动已有导出文件。
写入失败给出可读说明并继续使用，成功显示真实路径；导出不清空历史或修改当前阈值。

## 开启时准备约定

仅创建任务卡，未准备TASK-031源码或切换入口，App.xaml保留TASK-030。
明确开启后读取最新源码、文件与集合相关问答，准备独立任务目录和任务专属导出路径，保留原作品。
教练提供导出按钮、结果文字与路径，复用阈值保存/加载和采集/表格/图表，核心保护、组行与写入留TODO。
先解释CSV表头、每行字段与换行，明确序号来自原集合下标加1，温度来自原double值，不能取图中y。
若使用StringBuilder或CultureInfo.InvariantCulture，先从具体“把几行文字合起来”和“导出的26.5始终使用小数点”解释，再给普通完整写法，不默认学员已经掌握。
首个动作只让导出入口显示原集合Count，添加五条后显示5，确认不是图中3个点，再做文件。

## 实践与产出

1. 检查停止状态且集合非空，不满足时说明原因并保留原文件。
2. 生成表头和全部历史行，从1编号，写入独立任务CSV文件。
3. 对照实际原集合与文件验证顺序/条数/小数，覆盖正常导出、失败保护及原功能保持。

## 验收标准（DoD）

- [x] 停止状态导出24/26.5/25/28/24，文件为固定表头加5条记录，序号1到5、值与完整原集合逐条一致，26.5保持小数点，显示真实路径。
- [x] 空历史或采集中点击导出均有说明且不改已有文件；正常再次导出反映当前历史，没有多余旧行；模拟写入失败显示失败，程序继续可用，恢复后可成功导出。
- [x] 导出不改变历史/有效阈值/配置，原启停、清空保护、报警/参数立即生效、配置保存恢复、表格/Count/最新三点保持，默认和约定最小尺寸可读，实际窗口正常关闭。

## 状态与证据

- 2026-10-17：TASK-030三个DoD通过后仅创建本卡，状态⚪ 未开始；未创建本题源码、CSV文件或切换入口，未验收。
## 2026-10-17明确开启：实际骨架与首步

当前状态🟡 进行中，上方“仅创建任务卡、未准备源码、入口TASK-030”为开启前约定与历史，现入口已切到TASK-031。

- 专属目录：`WpfBae/Exercises/TASK-031-history-csv/`。
- 实际窗口：[Task031Window.xaml](../../WpfBae/Exercises/TASK-031-history-csv/Task031Window.xaml)。
- 核心练习：[Task031Window.xaml.cs](../../WpfBae/Exercises/TASK-031-history-csv/Task031Window.xaml.cs)的`ExportTemperatureHistory`。
- 复用配置类：[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-031-history-csv/AlarmThresholdSettings.cs)，使用本题命名空间，保留原TASK-030作品。
- 共享入口例外：[App.xaml](../../WpfBae/App.xaml)仅切换StartupUri；[项目文件](../../WpfBae/WpfBae.csproj)仅增加本题Verification目录编译排除。
- 实际新增验证入口：[StartupReview.csproj](../../WpfBae/Exercises/TASK-031-history-csv/Verification/StartupReview.csproj)、[StartupReview.cs.txt](../../WpfBae/Exercises/TASK-031-history-csv/Verification/StartupReview.cs.txt)、[launch.ps1](../../WpfBae/Exercises/TASK-031-history-csv/Verification/launch.ps1)。
- CSV目标：`AppContext.BaseDirectory/Exercises/TASK-031-history-csv/temperature-history.csv`；普通Debug运行对应`WpfBae/bin/Debug/net10.0-windows/Exercises/TASK-031-history-csv/temperature-history.csv`，窗口显示实际绝对路径。
- 配置目标：同一任务输出目录下的`alarm-threshold.json`，不复制或改写TASK-030原用户配置，缺失时沿用原27默认值。
- 教练准备：导出按钮、结果和真实路径显示、独立窗口及配置类；沿用原采集/阈值/配置/图表/表格实现，核心导出未代做。

| TODO | 实践对应 | DoD对应 | 要完成的内容 |
| :--- | :--- | :--- | :--- |
| 1 | 实践1的首步观察 | 1的数据来源准备 | 在导出入口显示原_temperatures.Count，五条应显示5，先不写文件。 |
| 2 | 实践1的保护 | 2 | 正在采集或空历史说明原因并返回，不改已有CSV。 |
| 3 | 实践2的组行 | 1 | 固定表头、全部原集合、下标加1、小数点温度与逐行换行。 |
| 4 | 实践2/3的写入与验证 | 1/2/3 | 创建父目录、覆盖写入、成功路径、异常说明、不改变原业务数据。 |

第一步只修改TODO 1，把方法末尾占位赋值改为`ExportStatusText.Text = "准备导出的历史条数：" + _temperatures.Count;`。
ExportStatusText是教练命名的WPF文字控件，Text是它显示的文字；_temperatures是原温度集合，Count是这份集合的条数。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，在停止状态点击五次“添加下一条”，再点“导出历史”，预期显示5，表格五条而折线最多三点。
完成后可以反馈这一步的运行现象，或实现后请求“检查TASK-031”；本轮尚未执行这条供学员使用的dotnet run命令。
CSV示例、字段/换行/小数与写入关系见[CSV问答Q1](../questions/csharp-csv.md)，新增为待复习。

### 实际验证记录

- 已执行`dotnet build WpfBae/WpfBae.csproj --nologo`，退出0、0错误、4条CS8600警告（TASK-030和本题复用加载声明各在临时编译/正式编译报告一次），未更改原实现以消除历史警告。
- 已执行`dotnet run --project WpfBae/Exercises/TASK-031-history-csv/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-031/startup`，退出0，59项实际窗口骨架检查全通过，包含同一集合/添加五条而图中三点/导出占位不写文件/原阈值应用/清空与关闭/默认960×960和最小860×920边界；[检查结果](./evidence/TASK-031/startup/results.json)。
- 已执行`& WpfBae/Exercises/TASK-031-history-csv/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-031/startup/launch.json`，实际原exe标题TASK-031、启动后正常关闭、退出0；[启动记录](./evidence/TASK-031/startup/launch.json)。
- 已查看实际渲染[最小尺寸图](./evidence/TASK-031/startup/minimum.png)，新增路径可换行、导出按钮/表格/图表和底部说明布局检查通过；[默认尺寸图](./evidence/TASK-031/startup/default.png)同时保留。
- 以上仅为教练骨架可用性证据，导出尚未实现，三个DoD未勾选，不新增学员自主掌握、错题或下一张任务卡。
## 2026-10-17首次实际审查

状态🟡 进行中；DoD 1未通过、2/3通过，不生成下一任务，上方“尚未实现/DoD均未勾选”为开启时历史。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 未通过 | 默认区域导出固定表头和5条、序号1到5、全部原值均正确，成功文字和界面真实路径可见；de-DE区域将第二条导成2,26,5，不满足温度始终用小数点。 |
| 2 | 通过 | 无文件空历史不创建文件，有文件空历史/采集中拒绝且字节不变；五条后清空再导一条仅剩两行；独占文件与目标路径为目录均显示失败，解除后可成功导出，窗口继续可用。 |
| 3 | 通过 | 历史和阈值不被导出修改，独立配置字节不变，真实定时采集/停止等待/运行清空禁用及方法保护/参数即时判断/保存与新窗口恢复/表格全部行/完整Count/最新三点正确，默认960×960和最小860×920边界及实际图检查通过，停止及运行关闭均停止定时器，原exe正常关闭退出0。 |

当前唯一阻塞位于[Task031Window.xaml.cs](../../WpfBae/Exercises/TASK-031-history-csv/Task031Window.xaml.cs)的组行循环，Convert.ToString(_temperatures[i])按当前区域设置把数字转成文字，de-DE下26.5得到26,5。
逗号在本文件中用于分列，所以2,26,5变成三个字段，不再是两列记录。
首个修复仅把温度转换表达式改为`_temperatures[i].ToString(System.Globalization.CultureInfo.InvariantCulture)`，其余循环/保护/写入保留；InvariantCulture是.NET提供的固定格式规则，传给double.ToString后26.5始终用小数点，详见[CSV Q1](../questions/csharp-csv.md)。
当前未使用的JsonSerializerOptions不影响本次结果，可自行整理，不作为新增必需项；成功结果旁已显示真实CSV路径，不要求为此重复修改文字。
本轮未代改学员源码，未把失败归因为集合或文件写入能力不足，Q1整体待复习，已通过局部范围保留。

实际命令与结果：

- `dotnet build WpfBae/WpfBae.csproj --nologo`退出0，本次增量构建输出0警告/错误，不代表此前可空声明警告已修复。
- `dotnet run --project WpfBae/Exercises/TASK-031-history-csv/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-031/review-01`退出1，共84项检查，83通过、1失败；两段预期的文件占用/拒绝访问异常由学员catch捕获后输出，未发生未处理异常；[实际结果](./evidence/TASK-031/review-01/results.json)、[默认区域CSV](./evidence/TASK-031/review-01/default-culture.csv)、[de-DE区域CSV](./evidence/TASK-031/review-01/de-DE.csv)。
- `& WpfBae/Exercises/TASK-031-history-csv/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-031/review-01/launch.json`退出0，原exe显示TASK-031并正常关闭退出0；[启动记录](./evidence/TASK-031/review-01/launch.json)。
- 审查前保存源文件及既有配置/CSV哈希，审查后核对73项全部未变；[核对结果](./evidence/TASK-031/review-01/verification.json)、[基线](./evidence/TASK-031/review-01/before.json)，审查写入只发生于本轮独立输出目录。
- 已实际查看[最小布局图](./evidence/TASK-031/review-01/minimum.png)，表格五条与图中第3/4/5条可读，路径可换行；[默认图](./evidence/TASK-031/review-01/default.png)保留。
## 2026-10-17最终复查通关

学员已自主将温度转换改为带System.Globalization.CultureInfo.InvariantCulture的ToString；默认与de-DE区域均导出2,26.5，84项实际检查全部通过，三个DoD均已勾选，状态✅ 已通关，上方失败与进行中为修复前历史。

- DoD 1通过：固定表头、五条完整原历史、序号1至5与小数点、成功提示旁真实路径均已验证；[默认CSV](./evidence/TASK-031/review-final/default-culture.csv)、[de-DE CSV](./evidence/TASK-031/review-final/de-DE.csv)。
- DoD 2通过：空历史和采集中拒绝并保留原CSV，重新导出仅保留当前行数，文件独占和目标为目录均提示失败，解除后成功导出，程序继续可用。
- DoD 3通过：原集合/阈值/配置保留、真实自动采集与停止等待、清空保护、参数即时判断、保存恢复、完整表格与Count/最新三点、默认与最小布局、关闭收尾通过。
- 实际执行`dotnet build WpfBae/WpfBae.csproj --nologo`退出0、0错误、4条CS8600报告（TASK-030和本题复用声明在临时/正式编译各报告一次）；这些原声明警告未消除，不影响本次已验证分支。
- 实际执行`dotnet run --project WpfBae/Exercises/TASK-031-history-csv/Verification/TaskReviewFinal.csproj -- docx/tasks/evidence/TASK-031/review-final`退出0，84项全通过；[结果](./evidence/TASK-031/review-final/results.json)，两段文件异常是模拟失败时被原catch捕获的预期日志。
- 实际执行`& WpfBae/Exercises/TASK-031-history-csv/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-031/review-final/launch.json`退出0，原exe启动TASK-031并正常关闭退出0；[启动记录](./evidence/TASK-031/review-final/launch.json)。
- 审查前后73项源码和既有配置/CSV哈希未变，原DLL与审查加载DLL一致，本轮未代改学员源码或原用户数据；[核对](./evidence/TASK-031/review-final/verification.json)、[基线](./evidence/TASK-031/review-final/before.json)。
- 已查看[实际最小布局图](./evidence/TASK-031/review-final/minimum.png)，五行表格、最新三条标签及路径可读；[默认图](./evidence/TASK-031/review-final/default.png)保留，窗口尺寸沿用960×960与最小860×920。
- [CSV Q1](../questions/csharp-csv.md)按现有骨架内自主遍历全部原集合/序号与换行/固定温度小数格式/保护/覆盖写入与失败反馈范围标已掌握，窗口、路径、表格、图表和配置复用不算新自主设计。
- 下一步依据计划文件与监控界面目标，只创建[TASK-032：选择CSV保存位置](./TASK-032.md)（⚪ 未开始），复用已验证导出能力增加保存位置选择和取消处理，未准备源码或切换入口；原C#/WPF待复习范围与阶段进行中状态保留。