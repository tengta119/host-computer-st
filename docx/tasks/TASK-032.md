# TASK-032：选择CSV保存位置

- 阶段：第二阶段：WPF（起步，结合文件操作与监控结果导出）。
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)的WPF界面、文件处理与数据采集/监控目标；保存位置选择是据此设计的练习，不是原文逐项列出的要求。
- 选择依据：TASK-031已验证完整CSV组行、固定小数格式与写入保护，继续沿用这些能力，只增加由用户选择文件位置和取消操作的处理。
- 难度/预计时间：待补充，计划未给出。

## 场景与目标

停止采集后点击导出，由用户选择CSV文件的位置和名称，确认后再写入该文件，成功显示本次真实路径。
用户取消选择时说明已取消，不创建或改写文件，历史、阈值和配置保持。
采集中或历史为空时直接显示原因，不弹出保存窗口，也不写文件。

## 开启时准备约定

目前只创建本卡，不创建源码、不切换App.xaml，入口保留TASK-031。
用户明确开启后先读TASK-031最终源码、相关问答和既有数据路径，再准备独立任务目录，保留原作品。
先用真实保存窗口说明文件名、目录、确认和取消的效果，再介绍Microsoft.Win32.SaveFileDialog是.NET提供的文件保存对话框类。
ShowDialog的返回结果与FileName的用途在使用前分别解释，不默认掌握bool?、对象初始化简写或新API。
首步只弹出保存窗口并显示确认得到的路径或取消说明，不写文件；教练准备安全入口，核心选择判断和写入连接留TODO。
沿用TASK-031全部历史和固定小数点的实现，不新增文件格式或第三方依赖，不在取消前提前创建或清空目标文件。

## 实践与产出

1. 保留采集中和空历史保护，先观察保存窗口确认/取消的结果。
2. 确认后把原CSV文字写入用户选择的路径，处理写入失败与再次尝试。
3. 实际验证不同目录、取消、覆盖确认和原功能保持。

## 验收标准（DoD）

- [x] 停止状态导出五条24/26.5/25/28/24到用户指定的两个不同目录与文件名，内容均为固定表头加五条，序号1至5、温度使用小数点，界面显示本次真实路径，文件类型选择明确为CSV。
- [x] 取消选择不创建或改写文件；空历史/采集中不弹出保存窗口且有原因；选择已有文件时有覆盖确认，拒绝覆盖不改原文件；模拟写入失败有说明且窗口继续可用，恢复后可重新选择并成功导出。
- [x] 导出和取消均不改变历史、阈值或配置，原启停/运行清空保护/报警即时判断/配置保存恢复/表格完整Count与图中最新三点保持，默认及约定最小尺寸可读，实际窗口正常关闭。

## 状态与证据

- 2026-10-17：TASK-031三个DoD最终通过后仅创建本卡，未准备源码、导出文件或切换入口，未开始或验收本题。

## 2026-10-17明确开启：实际骨架与首步

当前状态🟡 进行中，上方“仅建卡片/未准备源码/入口TASK-031”为开启前历史，现入口已切到Task032Window。

- 独立目录：`WpfBae/Exercises/TASK-032-save-csv-dialog/`。
- 实际窗口：[Task032Window.xaml](../../WpfBae/Exercises/TASK-032-save-csv-dialog/Task032Window.xaml)。
- 本题代码：[Task032Window.xaml.cs](../../WpfBae/Exercises/TASK-032-save-csv-dialog/Task032Window.xaml.cs)的ExportTemperatureHistory方法。
- 复用配置类：[AlarmThresholdSettings.cs](../../WpfBae/Exercises/TASK-032-save-csv-dialog/AlarmThresholdSettings.cs)，使用独立命名空间和`AppContext.BaseDirectory/Exercises/TASK-032-save-csv-dialog/alarm-threshold.json`，不复制或改写TASK-031原用户配置。
- 教练提供：保存窗口配置CreateCsvSaveDialog，已验证的空历史/采集中保护，原组行提取为BuildCsvText，原写入提取为WriteCsvToPath(string path)，实际路径与成功文字更新由写入方法负责；选择判断与调用连接留TODO。
- 共享例外：[App.xaml](../../WpfBae/App.xaml)仅切换StartupUri，[项目文件](../../WpfBae/WpfBae.csproj)仅增加本题Verification编译排除。
- 验证入口：[StartupReview.csproj](../../WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/StartupReview.csproj)、[StartupReview.cs.txt](../../WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/StartupReview.cs.txt)、[launch.ps1](../../WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/launch.ps1)。
- 本题不再使用固定CSV输出路径，后续只在用户确认后写入所选位置，骨架点击导出不会写CSV。

| TODO | 实践对应 | DoD对应 | 内容 |
| :--- | :--- | :--- | :--- |
| 1 | 实践1 | 1/2的入口准备 | 调用dialog.ShowDialog(this)，用bool?接收结果，首步不写文件。 |
| 2 | 实践1 | 2 | 确认时显示dialog.FileName，未确认时显示取消并return，避免末尾占位赋值覆盖结果。 |
| 3 | 实践2 | 1/2/3 | 路径观察通过后，只在确认分支把所选路径传给已有WriteCsvToPath。 |
| 4 | 实践2/3 | 2 | 处理打开保存窗口的异常，验证覆盖确认/取消与写入失败恢复。 |

首个可观察动作是只接通TODO 1/2的保存窗口和结果显示，不调用写入方法；在已有dialog变量之后接上ShowDialog与确认/取消分支，并移除末尾占位文字赋值。
SaveFileDialog是.NET提供的文件保存对话框类，dialog是本次创建的对象；ShowDialog(this)显示它并等待用户关闭，this指当前Task032Window。
bool?允许true/false/null，只将result == true视为确认；FileName在确认后提供所选完整路径，取消分支return结束本次导出。
先读[CSV问答Q2](../questions/csharp-csv.md)，其中给出普通完整写法、每行具体含义、界面效果及后续连接顺序。
从工作区根目录运行`dotnet run --project WpfBae/WpfBae.csproj`，添加一条后在停止状态点导出，选择新文件名确认应显示完整路径且尚不创建文件，再点导出并取消应显示已取消。
这条dotnet run命令是学员运行方式，本轮未执行它；本轮实际运行下方验证项目和原exe启动脚本。

### 实际骨架验证

- `dotnet build WpfBae/WpfBae.csproj --nologo`退出0、0错误、6条CS8600报告，TASK-030/031与本题复用的加载声明在临时/正式编译各报告一次，未改原学员实现消除此历史警告。
- `dotnet run --project WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/StartupReview.csproj -- docx/tasks/evidence/TASK-032/startup`最终退出0，66项骨架检查全通过；[结果](./evidence/TASK-032/startup/results.json)。
- 首轮验证中对DefaultExt回读格式的断言过严，WPF回读会规范化掉开头点号，已修正验证代码再执行，非学员缺陷；[首轮结果](./evidence/TASK-032/startup/results-initial.json)保留。
- 实际窗口检查涵盖原集合/五条完整组行与最新三点、配置初始与阈值应用、空历史和运行保护、保存窗口默认名/CSV筛选/补扩展名/覆盖提示/不创建测试文件属性、占位不写文件、默认960×960和最小860×920控件边界及清空/关闭。
- 实际执行`& WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-032/startup/launch.json`退出0，原exe标题TASK-032，启动后正常关闭退出0；[启动记录](./evidence/TASK-032/startup/launch.json)。
- 已查看[最小尺寸实际图](./evidence/TASK-032/startup/minimum.png)，新增说明和未选择路径文字可读，图表/表格无裁切边界失败；[默认图](./evidence/TASK-032/startup/default.png)保留。
- ShowDialog仍为TODO，未实际操作文件保存窗口的确认/取消/覆盖，不能把配置属性检查当成交互验收；三个DoD保持未勾选，无新增自主掌握或典型错误，下一任务不创建。
## 2026-10-17实际审查通关

当前状态✅ 已通关，三个DoD全部通过，78项实际检查全通过；上方TODO未实现和进行中为开启时历史。

| DoD | 结论 | 实际证据 |
| :--- | :--- | :--- |
| 1 | 通过 | 真实保存窗口输入两个不同目录/名称，五条完整原历史逐行正确，CSV类型明确，所选真实路径显示；de-DE小数格式及省略扩展名自动补csv通过。 |
| 2 | 通过 | 实际点击取消保留文件和数据；真实覆盖窗口拒绝后取消保留原文，接受覆盖得到完整CSV；空历史/采集中直接返回且无对话框；实际写入方法模拟独占文件和目标目录均显示失败，解除后重新走保存窗口选择路径成功。 |
| 3 | 通过 | 历史/阈值/配置字节保留、真实定时追加与停止等待、运行清空禁用和方法保护、阈值立即报警/恢复、重开恢复已保存26、完整表格/Count/图表、默认与最小布局及运行/停止关闭收尾通过。 |

### 教练协助范围与剩余边界

学员已自主完成ShowDialog获取结果、result != true时返回、确认后传dialog.FileName给WriteCsvToPath，核心选择判断与写入连接保留。
教练按既有用户授权直接收尾三处结果文字：删除调用WriteCsvToPath后无条件赋“保存成功”的语句，避免覆盖写入方法的失败说明；将取消提示改为“已取消导出”；修正“保存的位置位”为“选择的路径”。
这些修改仅为文案和保留实际结果的收尾，不算学员自主实现；不要求学员为同类状态赋值反复提交。
ShowDialog使用无参数重载在本次真实交互中可用，不以未传this判定失败；保存窗口配置/BuildCsvText/WriteCsvToPath由教练提供或提取，不计新自主掌握。
打开保存窗口本身的异常分支未实现或实际模拟，本卡必需DoD中的写入失败已覆盖；没有将未运行的打开异常处理说成已验证能力，不新增该项为通关门槛。

### 实际命令与结果

- 实际执行`dotnet build WpfBae/WpfBae.csproj --nologo`：初次增量输出0警告/错误，文案收尾后构建退出0、0错误、6条既有CS8600报告，未消除历史加载声明警告。
- 真实保存窗口先用本题验证项目探测控件，记录[窗口控件](./evidence/TASK-032/review-01/dialog-controls.txt)；UI Automation探测未拿到保存窗口，改用Win32窗口/控件消息完成实际确认和取消操作，未改用户系统设置。
- 实际执行`dotnet run --project WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/TaskReview.csproj -- docx/tasks/evidence/TASK-032/review-01`，最终退出0，78项全通过；[结果](./evidence/TASK-032/review-01/results.json)，项目编译[DialogReview.cs.txt](../../WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/DialogReview.cs.txt)。
- 两段预期文件异常由WriteCsvToPath内部catch捕获后输出，反射调用验证了写入方法实际失败说明；失败恢复再经过真实保存窗口，不把失败注入说成通过对话框选中了不可写路径。
- 真实选择输出：[第一目录CSV](./evidence/TASK-032/review-01/chosen/A/first.csv)、[第二目录CSV](./evidence/TASK-032/review-01/chosen/B/second.csv)、[de-DE CSV](./evidence/TASK-032/review-01/chosen/B/german.csv)和[恢复CSV](./evidence/TASK-032/review-01/chosen/A/recovered.csv)，第二目录文件后续恢复验证为当前两条，五条原始逐行验证保留在结果中。
- 实际执行`& WpfBae/Exercises/TASK-032-save-csv-dialog/Verification/launch.ps1 -Executable WpfBae/bin/Debug/net10.0-windows/WpfBae.exe -OutputPath docx/tasks/evidence/TASK-032/review-01/launch.json`退出0，原exe标题TASK-032，正常启动关闭退出0；[启动记录](./evidence/TASK-032/review-01/launch.json)。
- 已实际查看[最小布局图](./evidence/TASK-032/review-01/minimum.png)，真实导出结果/路径与表格图表可读，默认960×960/最小860×920边界检查通过；[默认图](./evidence/TASK-032/review-01/default.png)保留。
- 审查配置和CSV使用本轮独立输出位置，未改原用户配置或既有导出文件；本轮确有上述文案收尾，不声明学员源码完全未改。
- [CSV Q2](../questions/csharp-csv.md)按ShowDialog结果接收、确认/取消分支与选择路径传入已有写入方法的自主范围标已掌握，其他提供或未验证内容不扩大。
- 依据计划MVVM与独立监控界面目标，仅创建[TASK-033：把温度历史与报警显示移到ViewModel](./TASK-033.md)（⚪ 未开始），复用已有功能整理数据与绑定职责，未准备源码或切换入口，原阶段仍进行中。