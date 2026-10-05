# 典型错误与复查

> 仅记录实际观察到的错误。计划里的常见坑不是学员已犯的错误。

<!-- 发生错误后追加：情况与证据、根因、修复线索、复查点及状态。 -->

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

- 状态：🟡 进行中
- 情况与证据：2026-10-02 提交说明中写“当 cancelEarly 为 true 时，cancelEarly 发出信号”，路径仅描述为 Cancel；[实际代码](../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs) 使用 `if (cancelEarly) { cts.Cancel(); }`，实现本身正确并已运行验证。
- 根因：说明中混淆了 bool 触发条件与 CancellationTokenSource 发请求的职责；信号源、令牌与响应操作的角色尚未区分清楚。
- 修复线索：对照条件分支，区分“何时取消”（cancelEarly）、“谁请求取消”（cts.Cancel）、“操作如何收到请求”（同一取消源的 Token 已传到 ReadAsync 里的 Task.Delay）。
- 复查点：在原实践提交中正确说明取消源与令牌传递路径；无需另写练习或单独口试。异常导致成功输出跳过及 catch 捕获的说明已正确，不列为错误。
- 2026-10-02：用户要求协助修正；教练已在[任务卡](./tasks/TASK-005.md)提供准确说明。文本已修正，但尚无学员自主应用角色区分的新证据，错误记录保持进行中；后续结合实践复核，不要求重复抄写或单独口试。
- 2026-10-05：TASK-005 已按用户明确要求完成任务级验收并通关；本条的 🟡 进行中 指概念复查状态。暂无角色区分自主理解的新证据，概念复查继续保留，不因关联任务通关自动标为已掌握或已修复。
