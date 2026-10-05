# TASK-008：用接口统一模拟设备读取

- 阶段：第一阶段：C# 基础（面向对象巩固）
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「二、第一阶段：C# 基础」中的面向对象及 Java/C# interface 对照；具体场景为按实践证据设计的练习，原文未指定此任务。
- 选择依据：TASK-007 配置、顺序异步读取及取消验收已通过；现有练习以数据类和静态读取方法为主，尚未见接口与通过接口调用不同实现的自主证据。先补充该项 C# 面向对象实践，再评估第一阶段覆盖，不仅依据已创建任务全部完成判断阶段通关。
- 难度/预计时间：待补充（计划未给出）。

## 场景与目标

监控程序读取不同设备时，希望采集流程使用统一的读取约定。用两个模拟设备实现同一接口，调用方通过接口读取，不按具体类型分别写采集分支。可适度对照 Java 的 interface、实现类和接口类型集合。

沿用现有 .NET 控制台项目及 [Reading 类型](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)，参考 [TASK-007 的异步等待和令牌传递](../../Base/Exercises/TASK-007-config-acquisition/Task007Exercise.cs)。不接真实设备，不引入依赖注入框架或通信库。

## 用户实践与产出

1. 定义 `IDeviceReader` 接口，提供可读取的设备 ID 和 `Task<Reading> ReadAsync(CancellationToken cancellationToken)` 约定。
2. 用两个模拟实现（例如温度与压力设备）实现接口。通过构造函数提供设备 ID 与模拟等待时间，各自产生可区分的数值/单位；返回读数的设备 ID 来自当前实例。等待支持传入的 Token。
3. 创建 `List<IDeviceReader>` 保存两个对象，用同一遍历逻辑逐次 await 读取，输出设备 ID、数值和单位；采集逻辑通过接口调用，无按具体设备类型区分的 if/switch 或强制转换。
4. 修改一个实例的设备 ID 或更换实现对象，再运行；观察输出对应变化，共用采集方法不需要跟着修改。结合代码或日志观察“接口约定相同，实际运行实现不同”，不设单独口试。

实际专属目录：`Base/Exercises/TASK-008-interface-readers/`。2026-10-05 用户要求“开启task008”后，确认目录不存在，再创建可编译骨架：

- 新建 [Task008Exercise.cs](../../Base/Exercises/TASK-008-interface-readers/Task008Exercise.cs)：接口声明位置、两个实现类、构造函数与读取方法签名、接口集合和共用采集入口；核心实现留作 TODO。
- 复用 [Reading](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)，参考 TASK-007 的等待和令牌传递；已有任务源码及注释未改动。
- 共享入口例外：[Program.cs](../../Base/Program.cs) 仅增加 `task008` 参数分支；TASK-007 与原 TASK-006 路由保留。项目默认递归编译新文件，无须修改项目文件或新增依赖。

## 骨架、首个动作与运行方式

首个动作：在 `Task008Exercise.cs` 的 TODO 1 声明接口成员 `string DeviceId { get; }` 与 `Task<Reading> ReadAsync(CancellationToken cancellationToken);`。两个类已有对应的公开属性和方法签名，接着完成构造函数赋值和实际读取。

| TODO | 实践步骤 | 对应 DoD |
| :--- | :--- | :--- |
| 1：在接口中声明设备 ID 与异步读取约定 | 步骤 1 | 第一项的真实接口及实现关系 |
| 2a / 3a：将构造函数参数保存到实例属性 | 步骤 2 | 第一项的实例 ID 与等待参数初始化 |
| 2b / 3b：异步等待、传递 Token、返回绑定当前实例的不同读数 | 步骤 2 | 第一项的实例关联；第二项的两种读数；第三项的等待生效 |
| 4a：把两个实例放入 List<IDeviceReader> | 步骤 3 | 第二项的接口类型集合 |
| 4b：共用遍历逐次 await 并输出完整读数与耗时 | 步骤 3 | 第二项的统一采集；第三项的实际实现与等待证据 |
| 5：修改实例 ID 或替换对象，保持采集方法不变并复跑 | 步骤 4 | 第三项的多态调用及修改对照 |

从工作区根目录 `D:\host-computer-st` 运行：

```powershell
dotnet run --project Base -- task008
```

建议顺序：TODO 1 → 2a/2b → 3a/3b → 4a/4b → 5。两次正常运行保留设备 ID、数值、单位及耗时日志，完成后发送“检查 TASK-008”。初始骨架只输出待实现提示，不读取设备；未实现的读取方法返回失败 Task，以免占位读数被误认为成果。骨架可运行不代表任何 DoD 通过。

### 必要概念与常见坑

- 实际场景：采集流程只需要知道设备“能读取”，温度和压力的数值、单位由各自实现决定。接口类型变量调用 ReadAsync 时，会执行该变量当前指向对象的实现。
- 对照 Java：`class TemperatureReader : IDeviceReader` 对应实现接口；`List<IDeviceReader>` 可以保存不同实现对象。C# 的接口还可以约定属性，例如只读的 DeviceId。
- 构造函数参数用于初始化当前对象；只读自动属性可以在该类构造函数中赋值。返回 Reading 时使用当前对象的 DeviceId，避免写死或混用另一个设备的 ID。
- 接口声明返回 `Task<Reading>`，不写 `async`；实现方法使用 `async` 和 `await` 后，成功路径直接返回 Reading，由编译器处理 Task 包装。把方法改为 async 时，应移除原来的失败 Task / CompletedTask 占位。
- 共用循环中逐次 await 后再输出；不为温度和压力分别写分支。等待使用实例的 ReadDelayMs，并传入 cancellationToken；不要固定所有实例的等待或调用 Delay 后漏掉 await。

## 验收标准（DoD）

- [x] 项目可编译，真实存在 IDeviceReader 与两个实现类，实例的 ID 与等待参数由对象初始化提供，读取结果绑定当前实例。
- [x] 两个对象通过接口类型集合和同一采集逻辑被 await 调用，输出两种可区分且完整的读数；没有按具体类型重复采集分支。
- [x] 修改实例 ID 或替换实现后运行输出对应变化，接口采集逻辑无须修改；代码/日志体现接口调用实际执行不同实现，等待参数真实生效。

## 可选巩固

- 复用已学的调用方取消源与专门取消处理，验证接口实现仍响应 Token；若要固定取消第一轮，将停止请求安排在模拟等待完成前。可选项不影响必需验收。
- 在本任务中继续区分构造函数赋值、只读属性与 init/set；通过实践补充属性问答证据。

## 状态与证据

### 2026-10-05 最终复查：✅ 已通关

学员自主将压力读数的 Unit 从 `.C` 改为 `kpa`，教练未修改源码。三项必需 DoD 均有证据：

| 必需 DoD | 结果 | 证据 |
| :--- | :--- | :--- |
| 编译、接口和两个实现、构造参数及实例绑定 | 通过 | 本轮构建退出码 0、0 错误，TASK-008 无新增警告；接口、构造函数与当前实例的 ID/等待属性保留正确实现。 |
| 接口集合、共用 await 采集、两种完整读数 | 通过 | 本轮实际输出 `reading Temperature 36 .C`、`reading Pressure 40 kpa`；两类读数已可区分，ID/数值/单位均输出。List<IDeviceReader> 中的两个对象仍经同一 foreach 逐次 await 调用，无按具体类型划分的分支或转换。 |
| 修改实例参数、共用逻辑不变、等待生效 | 通过 | 沿用同日首次审查的真实对照：100/100 ms 共 229 ms，800/1200 ms 共 2022 ms，20/30 ms 共 62 ms，输出 ID 随各场景实例变化。本次核对构造函数、等待与共用采集逻辑未变，仅压力单位修正，不重复运行已验证的参数对照。 |

实际命令与结果：

- `dotnet build Base/Base.csproj --no-restore`：退出码 0、0 错误、5 个来自 TASK-006/007 的原有 CS8600 警告。
- `dotnet D:\host-computer-st\Base\bin\Debug\net10.0\Base.dll task008`：退出码 0，输出上述两条读数，进程耗时 319 ms（含启动与流程开销）。源码 SHA256 为 `DE82ED191FD5148A007FE4AF63E1CFDF659805F21F1839A4803EF9B1BCEB9E24`。
- 可选整理：压力单位标准写法为 `kPa`，温度可用 `°C` 或 `℃`；第 80 行仍有旧占位提示。当前没有单位解析逻辑，大小写与显示排版不追加为新的通关门槛。
- 可选取消场景未运行，属性、线程、using 等问答保留原状态，不扩大掌握范围。
- 下一步：[TASK-009：用 WPF 窗口与按钮事件显示设备读数](./TASK-009.md)。计划依次进入 WPF 的 Window、基础控件、布局、XAML 与 Event；现有 C# 实践已覆盖接口、多态、异步、事件及配置，可在界面中继续应用。只创建这一张任务卡，状态 ⚪ 未开始，未创建 WPF 项目或启动任务。第一阶段仍有概念复习项，不宣称整阶段已通关。

### 2026-10-05 首次审查：🟡 进行中

| 必需 DoD | 结果 | 可见证据 |
| :--- | :--- | :--- |
| 编译、接口和两个实现、构造参数及实例绑定 | 通过 | 接口声明 DeviceId 与 ReadAsync；两类构造函数分别保存 ID 与等待参数，ReadAsync 返回当前实例 ID。实际构建退出码 0、0 错误、5 个 TASK-006/007 原有 CS8600 警告，TASK-008 无新增警告。 |
| 接口集合、共用 await 采集、两种完整读数 | 未通过 | List<IDeviceReader> 保存两个对象，共用 foreach 逐次 await，无具体类型分支或转换。实际输出 `reading Temperature 36 .C` 与 `reading Pressure 40 .C`；压力实现第 53 行沿用了温度单位，读数的单位与设备类型不符。 |
| 修改实例后的输出变化、采集逻辑不变、等待生效 | 通过 | 临时验证程序通过反射调用学员原有私有 AcquireAsync，依次提供不同 ID 和等待时间的两类实例。100/100 ms 共 229 ms；800/1200 ms 共 2022 ms；20/30 ms 共 62 ms。各场景输出分别对应 Temperature/Pressure、review-temp-slow/review-pressure-slow、review-temp-fast/review-pressure-fast，数值保持各实现的 36/40；共用采集源码未改动。 |

实际运行与保护：

- 工作区根目录执行 `dotnet build Base/Base.csproj --no-restore`，退出码 0、0 错误、5 个原有警告。
- 执行 `dotnet D:\host-computer-st\Base\bin\Debug\net10.0\Base.dll task008`，输出上述两条原始读数，退出码 0，进程耗时 348 ms（含启动开销）。
- 执行 `dotnet run --project C:\Users\lbwxxc\AppData\Local\Temp\TASK-008-review-84120a469fda43a695ef61c3ffdbab13\Review.csproj`，退出码 0。临时程序只引用本次构建的 Base.dll，改变调用方实例参数，并调用同一个原有 AcquireAsync；没有修改或代写学员源码。上述耗时为实际观测值，不作精确计时保证。
- 运行前后 Task008Exercise.cs 的 SHA256 均为 `1AF56C38FE4861CBC8F15009CFC604B176E6CDA234BF510BD3E2E96ADF23F596`。
- 可选取消场景未运行，不勾选、不影响必需验收。属性 init/set 等概念问答维持原状态，未因构造函数赋值正确就认定全部属性概念已掌握。

首个修复动作：检查 [Task008Exercise.cs](../../Base/Exercises/TASK-008-interface-readers/Task008Exercise.cs) 第 53 行，将压力读数的 Unit 改为压力单位，例如 kPa；温度单位也可整理为 °C 或 ℃。随后运行 `dotnet run --project Base -- task008`，确认两种读数的 ID、数值、单位均合理，再发送“检查 TASK-008”。第 80 行“请完成接口……”占位提示已过时，可删除或改为采集开始提示，属可选整理；未逐次输出耗时不单独阻塞，本轮已用实际对照验证等待。尚有必需 DoD 未通过，保持进行中，不生成下一任务。

### 启动与修正记录

- 2026-10-05 注释排版修正：将 TODO 1 的两个接口成员声明合并到同一行说明，保留学员已添加的接口成员及其他代码。本轮仅调整注释，未运行或验收，任务保持 🟡 进行中。
- 2026-10-05：TASK-007 三项必需 DoD 验证通过后，根据计划与源码覆盖情况创建这一张递增 ID 任务卡。状态 ⚪ 未开始，未创建骨架、未运行任务或验收；等待用户明确开始。
- 2026-10-05：用户要求“开启task008”，已检查现有任务、源码树、项目及入口，确认目标目录不存在后创建专属骨架并增量连接 Program.cs。状态 🟡 进行中，首个动作是 TODO 1 接口成员声明，全部必需 DoD 保持未勾选；未新增后续任务。
- 2026-10-05 骨架验证（工作区根目录）：实际执行 `dotnet build Base/Base.csproj --no-restore`，退出码 0、0 错误、5 个来自 TASK-006/007 的原有 CS8600 警告，TASK-008 无新增警告。实际执行 `dotnet run --project Base --no-build -- task008`，输出 `TASK-008: interface readers` 与 TODO 1-4 待实现提示，退出码 0；确认入口可运行，未执行设备读取或验收。实际执行 `git diff --check`，无差异格式错误，仅 Git 行尾转换提醒。学习进度、任务索引与导航已同步启动状态。
- 2026-10-05 语言修正：按用户反馈，将 TASK-008 骨架的全部说明注释、TODO 说明、占位异常消息和控制台提示改为中文，保留代码标识符与核心 TODO。实际执行 `dotnet run --project Base --no-restore -- task008`，编译并运行成功、退出码 0，输出“TASK-008：用接口统一模拟设备读取”和“TODO 1-4：请完成接口、设备读取实现和采集循环。”；仍有原 TASK-006/007 的 5 个 CS8600 警告，无新增警告。中文学习偏好已记入学习进度，任务保持 🟡 进行中，DoD 未勾选。
