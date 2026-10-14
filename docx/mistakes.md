# 典型错误与复查

> 仅记录实际观察到的错误。计划里的常见坑不是学员已犯的错误。

<!-- 发生错误后追加：情况与证据、根因、修复线索、复查点及状态。 -->

## TASK-022：历史区被纵向 StackPanel 包裹，未取得剩余高度

- 状态：✅ 已通关
- 2026-10-13 最终复查：学员已自主移除外层容器、调整直接子区域顺序并启用填满，17个实际场景全部通过，三个必需DoD已通过。窗口640→840时表格369.2267→569.2267，上下区稳定，恢复及最小尺寸/原业务正确；50个源码哈希未变，教练未代改。见[最新结果](./tasks/evidence/TASK-022/results.json)与[核对](./tasks/evidence/TASK-022/verification.json)。本条问题已修复，下方待复查/失败为对应历史。
- 2026-10-13 后续源码观察：学员已移除 ShowHistory，直接子区域改为选择/清空/历史，LastChildFill=True；本轮仅答疑，未重新运行验证，因此本条仍待复查。下方情况是修改前首次审查证据，未撤销既有业务能力。
- 情况与证据：2026-10-13 首次审查时窗口新增 ShowHistory StackPanel 包裹历史 Grid；主 DockPanel 直接子区域为 SelectionRegion、ShowHistory、ActionsRegion，LastChildFill=False。实际窗口高 640→840，表格仍 43.48；4 个布局场景未通过，13 个业务场景通过，见[首次结果](./tasks/evidence/TASK-022/first-review/results.json)及[当时源码快照](./tasks/evidence/TASK-022/first-review/Task022Window.xaml.txt)。
- 根因（确认的布局机制）：纵向 StackPanel 使内部历史 Grid 按内容高度测量；主 DockPanel 没有把剩余空间分给历史 Grid，移除表格自身固定 Height 不足以让它随窗口增长。未推断学员为何新增容器，未将此局部问题扩大为原 Binding/业务能力不足。
- 修复线索：先仅移除 ShowHistory 外层两标签，保留完整历史 Grid；随后让历史 Grid 成为主 DockPanel 最后一个直接子区域并启用 LastChildFill。详见[布局 Q2](./questions/wpf-layout.md)，源码由学员修正。
- 复查点：实际显示/拉高/恢复窗口，历史表格随高度增长且顶部/底部内容高度稳定；最小尺寸可读，旧行/原集合/命令连接继续正常。不要只改 LastChildFill 而把余下空间给当前最后的清空区。

## TASK-016：行内数值绑定路径与 Reading 属性名不对应

- 状态：✅ 已通关
- 情况与证据：2026-10-12 审查 [ReadingHistoryControl.xaml](../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml) 的第二个 TextBlock 绑定 FallbackValue；当前原编译窗口中每行对应 Reading.Value 为 26.5，但此绑定状态 PathError、显示空字符串。DeviceId/Unit 正常；见 [实际结果](./tasks/evidence/TASK-016/results.json)。
- 根因（可确认的机制）：Binding 路径在每行 Reading 上按属性名查找，Reading 没有 FallbackValue，导致查找失败。误写来自输入失误还是概念混淆未确定，不推断学员动机或将其已有绑定能力归零。
- 修复线索：对照 Reading 的公开属性核对数值路径；先让实际数值出现，再检查 ID 与数值间距。源码由学员修正。
- 复查点：数值控件 Binding 路径为 Value、状态 Active，实际文字为 26.5；三项分别从对应 Reading 读取，换 ID 保留旧行，间距可读，原业务行为保持。编译成功不代替实际行显示验证。
- 2026-10-12 同轮自主修复：学员改为 Value，重新构建与原编译窗口运行后绑定 Active、实际显示 26.5，各字段数据源均为对应 Reading，10 个业务场景通过。这里只标记数值路径错误已修复；任务仍进行中，DoD 2 的 ID/数值间距尚未通过，未把它算作修复后的可读性证据。修复前结果另存 results-before-value-fix.json，最新结果见原链接。
- 2026-10-12 最终复查：学员已补 ID 右边距，实际 ID/数值/单位显示正确，两段间距均为 12，10 个场景显示/业务全通过，TASK-016 三个 DoD 全通过。本条保持已修复，未新增认知错误，不将此前局部错误扩大为整体绑定能力不足。

## TASK-013：空白校验只显示提示，未结束方法

- 状态：✅ 已通关
- 情况与证据：2026-10-12 首次审查 [ReadTemperatureCommand.Execute](../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs) 的 `if (id.Length == 0)` 仅设置不合法提示，随后仍执行 Add 和成功提示。实际窗口输入空字符串使历史从 3 条变 4 条，纯空格使 4 条变 5 条，新增空 ID 读数，最终状态“读取成功”；见 [首次运行结果](./tasks/evidence/TASK-013/results-before-fix.json)。
- 根因：条件分支结束后方法仍按顺序执行后面的语句；提示只修改文字，不会阻止读取动作。
- 修复线索：无效输入分支提示后提前结束 Execute，使它不能继续进入成功路径。
- 复查点：空字符串及纯空格均保留原历史、最终显示不合法提示，不触发 Add；有效读取和清空恢复不受影响。
- 自主修复与验证：学员在审查期间补上 `return;`，教练未改源码；重新构建 0 警告/错误，10 个窗口场景全通过，两种空白均保留原 3 条历史，提示“该 Id 不合法”。见 [最终结果](./tasks/evidence/TASK-013/results.json)及[源码哈希核对](./tasks/evidence/TASK-013/verification.json)。本条表示这次错误已修复，不推定全部控制流概念已掌握。

## TASK-006：返回 Task 的方法未标 async，却直接使用 await

- 状态：✅ 已通关
- 情况与证据：2026-10-05 学员询问异步文件写入为何报错。[SaveAsync](../Base/Exercises/TASK-006-json-config/Task006Exercise.cs) 声明为 `private static Task SaveAsync(...)`，方法体使用 `await File.WriteAllTextAsync(path, configJson)` 并 `return writeAllTextAsync`；实际 `dotnet build Base/Base.csproj --no-restore` 退出码 1，报 CS4032 与 CS0103。
- 根因：混淆返回类型 Task 与允许方法体使用 await 的 async 修饰符；同时残留了返回未定义任务变量的语句。
- 修复线索：给 SaveAsync 添加 async，保留 Task 返回类型；async Task 没有结果值，移除带表达式的 return，自然结束即可。保存成功日志放在 await 写入之后。
- 复查点：学员自主修正后项目编译通过，保存成功时实际文件存在；能在后续实践区分 Task 返回类型、async 修饰符与 await 等待。尚未验证修复，不代改源码。
- 2026-10-05 后续复查：学员自主移除 SaveAsync 内部 await，定义写入 Task 变量后直接返回，调用方仍 await SaveAsync；实际构建成功（退出码 0，0 警告、0 错误）。CS4032 / CS0103 已修复，采用直接返回 Task 的合法方案，未强制改为 async；文件保存及完整应用证据仍待验证，本条复查状态保持 🟡 进行中。
- 2026-10-05 最终局部复查：学员随后自主改为 async Task SaveAsync 并 await 文件写入，调用方 await 后输出保存成功；实际 save 退出码 0，生成合法配置文件。原 async/返回变量问题已修复，本条 ✅ 已通关 仅指该错误复查；TASK-006 整体仍为 🟡 进行中，漏等待另记下条。

## TASK-006：调用 Task.Delay 后未等待，误以为外层 await 会等待它

- 状态：✅ 已通关
- 情况与证据：2026-10-05 审查 [ReadAsync](../Base/Exercises/TASK-006-json-config/Task006Exercise.cs) 第 99 行，`Task.Delay(config.ReadDelayMs)` 未 await；实际配置 3500 ms 时 load 进程 62 ms 结束，50 ms 对照 60 ms，重新构建报 CS4014。
- 根因：延迟任务被创建后未加入读取方法的完成流程；方法立即返回读数。调用方 await ReadAsync 不能自动等待方法内部没有跟踪的任务。
- 修复线索：在 ReadAsync 内等待配置决定的延迟任务，完成等待后才构造/返回读数。
- 复查点：学员自主修复后，以明显不同的等待参数观察真实读取耗时变化，CS4014 消失；仅打印配置值不算等待生效。
- 2026-10-05 最终复查：学员自主改为 await Task.Delay(config.ReadDelayMs)，CS4014 消失；3500 ms 配置运行 3565 ms，50 ms 对照 120 ms，设备 ID 与参数均输出正确。该漏等待错误已修复。

## TASK-006：两个配置依次写入同一文件，后一个覆盖前一个

- 状态：✅ 已通关
- 情况与证据：2026-10-05 save 分支两次调用 SaveAsync 使用同一 configPath；实际运行后合法 JSON 只保留设备 002、200 ms，设备 001、100 ms 被覆盖。
- 根因：WriteAllTextAsync 是整体写入文本，不能用连续写入同一路径表达“保存两条配置”。
- 修复线索：当前任务按单配置流程选一个样例保存即可；如果以后需要多配置，需明确集合或各自文件的存储设计，不在本次默认扩展范围。
- 复查点：当前流程的保存意图与实际文件内容一致。此问题不否定第一项 DoD 的单配置保存证据，但避免把运行成功误认为两条配置均被保留。
- 2026-10-05 最终复查：学员自主删除第二次保存，当前只保存设备 002、200 ms；实际生成内容与单配置意图一致，重复覆盖问题已修复。

## TASK-002：事件已触发，但设备 ID 未正确从设备传入

- 状态：✅ 已通关
- 情况与证据：2026-09-27 审查 [`Task002Exercise.cs`](../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)，`ProduceReading` 创建 `Reading` 时只赋 `Value` 与 `Unit`；处理器也只输出这两项。实际运行输出 `eReading: 5 ℃`、`eReading: 6 ℃`，无设备 ID。
- 根因：`Reading.DeviceId` 未从发布事件的设备传入读数对象，处理器无法从事件参数取得可核对的设备 ID。
- 修复线索：构造读数时利用 `SimulatedDevice` 已有的 `Id`；处理器从 `e.Reading` 输出相应属性。
- 复查点：两次事件输出均包含设备 `001` 的 ID、数值与单位；取消订阅后的第三次读数仍不产生处理器输出。
- 2026-09-27 第二次复查：处理器现已输出 `DeviceId`，两次读数均显示 `001`；但 `ProduceReading` 将 `DeviceId` 写死为 `"001"`，没有使用设备实例的 `Id`。当前样例掩盖了设备身份绑定错误；下一次复查需确认 ID 来源正确，换用其他设备 ID 时仍能如实传递。
- 2026-09-27 最终复查：`ProduceReading` 已改用设备实例的 `Id` 构造 `Reading.DeviceId`；运行输出包含两次设备 `001` 的读数，第三次在取消订阅后无处理器输出。此错误已修复。

## TASK-005：把触发条件当作取消信号源

- 状态：✅ 已通关
- 情况与证据：2026-10-02 提交说明中写“当 cancelEarly 为 true 时，cancelEarly 发出信号”，路径仅描述为 Cancel；[实际代码](../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs) 使用 `if (cancelEarly) { cts.Cancel(); }`，实现本身正确并已运行验证。
- 根因：说明中混淆了 bool 触发条件与 CancellationTokenSource 发请求的职责；信号源、令牌与响应操作的角色尚未区分清楚。
- 修复线索：对照条件分支，区分“何时取消”（cancelEarly）、“谁请求取消”（cts.Cancel）、“操作如何收到请求”（同一取消源的 Token 已传到 ReadAsync 里的 Task.Delay）。
- 复查点：在原实践提交中正确说明取消源与令牌传递路径；无需另写练习或单独口试。异常导致成功输出跳过及 catch 捕获的说明已正确，不列为错误。
- 2026-10-02：用户要求协助修正；教练已在[任务卡](./tasks/TASK-005.md)提供准确说明。文本已修正，但尚无学员自主应用角色区分的新证据，错误记录保持进行中；后续结合实践复核，不要求重复抄写或单独口试。
- 2026-10-05：TASK-005 已按用户明确要求完成任务级验收并通关；本条的 🟡 进行中 指概念复查状态。暂无角色区分自主理解的新证据，概念复查继续保留，不因关联任务通关自动标为已掌握或已修复。
- 2026-10-05 TASK-007 自主应用复查：学员代码用 cancelEarly 判断是否停止、cts.CancelAfter(100) 调度请求，cts.Token 经 AcquireAsync 和 ReadAsync 传给 Task.Delay；1000 ms 配置实际取消，无成功读数。角色区分已有自主代码及运行证据，本条错误复查标为 ✅ 已通关；TASK-007 整体仍进行中，取消异常处理另记下条，CancellationToken Q1 的其他复习内容不随之全部通关。

## TASK-007：调用方捕获取消并提示后再次抛出，导致未捕获异常退出

- 状态：✅ 已通关
- 情况与证据：2026-10-05 [Task007Exercise.cs](../Base/Exercises/TASK-007-config-acquisition/Task007Exercise.cs) 第 93～96 行，RunScenarioAsync 捕获 OperationCanceledException、提示后再次 throw。1000 ms 配置的 cancel 场景无成功读数，但输出 Unhandled exception，退出码 -532462766。
- 根因：捕获和打印不会消除后续 throw 的传播；RunAsync 与 Program 都没有后续捕获。中间循环需要向上传递取消以停止后续轮次，而负责结束本场景的调用方应处理预期取消，二者边界不同。
- 修复线索：保留采集循环向上传递异常，让 RunScenarioAsync 的预期取消分支处理后安全结束；不要把循环内的 throw 一并删除而继续循环。默认留给学员局部修复。
- 复查点：取消时有提示、无未捕获异常、无取消后成功读数、退出码 0；using 的作用域仍覆盖采集任务结束。
- 2026-10-05 同轮复查：用户自行移除 RunScenarioAsync catch 的 throw，保留采集循环的重抛；重新构建并执行 1000 ms 配置的 cancel，场景 110 ms、进程 171 ms，取消提示完整、无成功读数/未捕获异常、退出码 0。本条错误已修复，TASK-007 整体仍因短配置取消时机为 🟡 进行中。

## TASK-007：取消调度延迟写死，短等待配置下取消前已完成采集

- 状态：✅ 已通关
- 情况与证据：2026-10-05 调用方固定 CancelAfter(100)，而 ReadDelayMs 可配置为任意正数；实际改为 10 ms 后 cancel 输出三条成功读数、场景 45 ms 返回，没有取消提示，退出码 0。
- 根因：停止倒计时未随被取消操作的实际等待变化；“调度了取消”不保证请求发生在操作完成前。
- 修复线索：取消场景按加载的等待时间选择较短的停止延迟，确保正在等待时收到请求；正常场景仍不调度取消。
- 复查点：1000 ms 与较短正数配置的 cancel 均能在读取等待期间停止，无被取消读取及后续轮次的成功读数；正常模式仍完成三次。
- 2026-10-05 最终复查：学员自主改为 CancelAfter(config.ReadDelayMs)，1000 ms 两次、10 ms 六次、1 ms 六次均有取消提示、退出码 0，无取消后成功读数；正常仍三次完成。相等计时下允许请求前先完成一至两条读数，本任务 DoD 未要求必须取消第一轮。固定延迟失配问题已修复，本条 ✅ 已通关；相等延迟仍有竞争，不认定取消第一轮或精确计时已保证。
