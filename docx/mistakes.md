# 典型错误与复查

> 仅记录实际观察到的错误。计划里的常见坑不是学员已犯的错误。

<!-- 发生错误后追加：情况与证据、根因、修复线索、复查点及状态。 -->

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
