# TASK-006：用 JSON 文件保存和加载设备配置

- 阶段：第一阶段：C# 基础
- 状态：✅ 已通关
- 计划来源：[计划原文](../plan.md)「二、第一阶段：C# 基础」中 CancellationToken 之后的“文件 / JSON / 配置”；本任务是据此设计的练习，不是计划原文指定任务。
- 相关经验：可复用 [TASK-001](./TASK-001.md) 的模型与 [TASK-004](./TASK-004.md) 的模拟读取经验；计划原文未规定强制先修。
- 难度/预计时间：待补充（计划未给出）。
- 提前规划说明：2026-10-02 用户询问 TASK-006 应做什么，提前规划本张任务卡；创建时 TASK-005 仍为进行中。2026-10-05 [TASK-005](./TASK-005.md) 已按用户明确要求完成任务级验收，复用本卡作为下一任务；取消源角色的自主理解继续待复习。

## 场景与学习目标

上位机重启后，需要恢复上次保存的设备参数，而不是每次把设备 ID、读取等待时间写死在代码中。本任务把配置对象保存到 JSON 文件，再读取为对象，用读回的配置驱动一次模拟读取。

借此学习文件读写、JSON 序列化/反序列化，以及配置数据与程序逻辑的区别。选用现有 .NET 的 File API 与 System.Text.Json，不引入新的配置框架。

## 用户实践与产出

- 定义任务专属的 DeviceConfig，至少包含 DeviceId（设备 ID）与 ReadDelayMs（模拟等待毫秒数）。
- 将示例配置保存为可查看的 JSON 文件，打印文件的实际绝对路径；读取文件并还原配置对象，输出还原的字段。
- 使用加载后的配置决定模拟读取的设备 ID 与等待时间；修改 JSON 后再次加载，可以观察参数变化。
- 配置文件不存在时输出明确提示；JSON 格式损坏时输出明确错误提示，安全结束本次流程，保留原文件便于学员修复。
- 实际专属目录：`Base/Exercises/TASK-006-json-config/`；[Task006Exercise.cs](../../Base/Exercises/TASK-006-json-config/Task006Exercise.cs) 中学员已完成配置模型、保存、加载、字段输出、配置等待与两种异常处理。2026-10-05 最终复查三项必需 DoD 均通过。
- 复用 [Base.csproj](../../Base/Base.csproj)（无需改动或新增依赖）与 [Reading 模型](../../Base/Exercises/TASK-001-device-model/Task001Exercise.cs)。共享入口例外：[Program.cs](../../Base/Program.cs) 启动本任务；开始时曾添加 task005 选择分支，2026-10-05 审查时学员已改成直接调用 TASK-006，不再支持该参数选择。审查未修改源码。

## 骨架与首个动作

首个动作：打开 `Task006Exercise.cs`，完成 TODO 1a 的两个 public 属性，再完成 TODO 1b 的样例对象赋值。其余 TODO 按下表推进；所有占位都需由学员替换，骨架可运行不代表验收通过。

| TODO | 实践步骤 | 对应 DoD |
| :--- | :--- | :--- |
| 1a / 1b：配置属性与样例对象 | 步骤 1 | 第一项的配置数据基础 |
| 2：序列化并写入文件 | 步骤 2 | 第一项：真实 JSON 文件与绝对路径 |
| 3：读文件、反序列化、返回对象 | 步骤 3 / 4 | 第二项：还原配置，加载不写回 |
| 4：不存在、损坏、null 的提示与安全结束 | 步骤 5 | 第三项：异常场景、保留损坏文件 |
| 5a / 5b：输出配置，用配置决定读取参数 | 步骤 3 / 5 | 第二项：参数应用与手动修改验证 |

从工作区根目录 `D:\host-computer-st` 运行：

```powershell
dotnet run --project Base -- save
dotnet run --project Base -- load
```

`save` 只创建初始样例，入口在文件已存在时拒绝覆盖；`load` 独立加载，默认无参数也走此入口。完成 TODO 2 后，打开程序打印的绝对路径编辑 JSON，再运行 `load`。

运行时配置位于 `AppContext.BaseDirectory/Exercises/TASK-006-json-config/device-config.json`；当前默认 Debug 构建对应工作区相对路径 `Base/bin/Debug/net10.0/Exercises/TASK-006-json-config/device-config.json`。这是运行时生成的数据，不是源码目录中的文件；不要执行 clean 后期待它仍存在。

完成后分别观察正常保存/加载、手动修改后加载、将专属 JSON 临时改名后的缺失提示、把该 JSON 改为非法格式后的损坏提示；恢复文件后提交“检查 TASK-006”。只操作输出路径下的本任务文件。

## 建议实践步骤

1. 创建配置模型，准备一个有设备 ID 和正数等待时间的样例对象。
2. 将对象序列化为 JSON 并保存文件；区分“生成 JSON 字符串”和“写入磁盘”两个动作。
3. 从文件读取 JSON，再反序列化为 DeviceConfig；打印读回的字段，并用于模拟读取。
4. 将“创建样例配置”与“加载已有配置”分开：加载时不要先覆盖已有配置，否则看不到手动修改的效果。
5. 分别观察正常加载、文件不存在、JSON 格式损坏的结果；只对本任务专属练习文件进行操作，不覆盖其他任务或真实设备配置。

## 验收标准（DoD）

- [x] 练习可运行；配置对象被实际保存为合法 JSON，输出可定位的文件路径，文件内容含设备 ID 和等待时间。
- [x] 能从文件还原配置对象，并用加载的 DeviceId、ReadDelayMs 驱动模拟读取；修改 JSON 后重新加载，读回字段与实际读取参数一致，加载流程不会自动覆盖改动。
- [x] 文件不存在、JSON 格式损坏两种情形均有清晰提示并安全结束本次流程，损坏文件不会被静默覆盖。

## 可选巩固

- 在模拟读取或文件读写中传递取消令牌，结合自主实现说明“谁发请求、令牌传到哪里、操作怎样响应”，为相关问答与概念错题补充应用证据。此项不属于 TASK-006 必需验收；未实现不勾选为完成，也不自动把取消原理标为已掌握。

## 2026-10-05 最终复查结果

整体：✅ 已通关。学员自主修复延迟等待、缺失/损坏处理和重复覆盖；审查没有修改源码。

| 必需 DoD | 结果 | 实际证据 |
| :--- | :--- | :--- |
| 保存合法 JSON、输出实际路径 | 通过 | save 退出码 0，输出完整路径及保存成功；合法 JSON 含 DeviceId = 002、ReadDelayMs = 200，当前只保存一个样例。 |
| 加载配置、应用 ID 与等待时间、修改后不覆盖 | 通过 | 正常加载 200 ms 配置，整段进程 265 ms；改为 review-006 / 3500 ms 后输出一致，整段进程 3565 ms；review-fast / 50 ms 对照为 120 ms。ReadAsync 已 await 配置决定的延迟；加载前后修改文件的哈希一致。 |
| 缺失/损坏清晰提示、安全结束、保留损坏文件 | 通过 | 缺失文件显示“文件不存在”，损坏 JSON 显示“反序列化失败”；均显示“未获得可用配置，本次流程结束”、退出码 0，无成功读数。损坏文件内容保持原样。 |

- 实际执行 `dotnet build Base/Base.csproj --no-restore -t:Rebuild`：退出码 0，0 错误、3 个 CS8600 警告（第 82、90、91 行的可能 null 与非可空类型声明不一致）；CS4014 已消失。这三处声明可改为 DeviceConfig?，当前调用方已检查 null，本次必需场景无阻塞问题，警告不作为未列入 DoD 的额外通关门槛。
- 实际执行 `dotnet D:\host-computer-st\Base\bin\Debug\net10.0\Base.dll save` 与 `dotnet D:\host-computer-st\Base\bin\Debug\net10.0\Base.dll load`，通过 PowerShell 依次准备保存、正常加载、修改 3500 ms、50 ms 对照、缺失及损坏场景；各场景退出码均 0。保存 56 ms，缺失 50 ms，损坏 64 ms；耗时含进程启动开销，不作性能基准。
- 现有运行时配置先备份，检查在 try/finally 中进行，最后恢复原文件并确认 SHA256 与审查前一致，移除临时备份。未修改学员源码、TODO 或注释。
- 可选取消巩固未实现，不算完成；取消源角色及资源释放的待复习记录保持原状态。
- 依据计划第一阶段 Task/async/await、CancellationToken 和配置目标，以及前次漏等待与取消源角色待复习，下一步选择小型综合巩固：[TASK-007：用配置驱动可取消的连续采集](./TASK-007.md)。本轮只创建一张未开始任务卡；不启动代码骨架，不宣称第一阶段完成。

## 2026-10-05 首次审查结果（历史）

整体：🟡 进行中，必需 DoD 第一项通过，第二、三项未通过；不生成下一任务。

| 必需 DoD | 结果 | 可见证据与修复线索 |
| :--- | :--- | :--- |
| 保存合法 JSON、输出实际路径 | 通过 | `save` 退出码 0，输出绝对路径与“保存成功”；文件为带换行缩进的合法 JSON，含 DeviceId = 002、ReadDelayMs = 200。 |
| 加载、应用 ID 与等待时间、修改后不覆盖 | 未通过 | 加载保存样例成功，输出设备 002 和 200 ms；改为 review-006 / 3500 ms 后字段与设备 ID 正确，加载前后文件哈希一致。但整个进程仅 62 ms，50 ms 对照为 60 ms，未实际等待配置时间；ReadAsync 第 99 行 Task.Delay 未 await，重新构建也报 CS4014。给延迟任务添加正确的等待，保留配置参数来源，再复跑。 |
| 缺失/损坏时清晰提示、安全结束、保留文件 | 未通过 | 文件缺失抛未捕获 FileNotFoundException；损坏 JSON 抛未捕获 JsonException，两者退出码均 -532462766。损坏文件内容保留这一子项通过；仍需在 LoadAsync 按 TODO 4 处理这两种情形、提示并返回 null，复用调用方现有结束分支。 |

运行证据：

- 工作区根目录执行 `dotnet build Base/Base.csproj --no-restore -t:Rebuild`：退出码 0，0 错误、2 警告（第 89 行 CS8600，反序列化可能返回 null；第 99 行 CS4014，未等待延迟任务）。增量 build 先前报告 0 警告不代表重新编译也无警告。
- 分别执行 `dotnet D:\host-computer-st\Base\bin\Debug\net10.0\Base.dll save` 与 `dotnet D:\host-computer-st\Base\bin\Debug\net10.0\Base.dll load`；通过 PowerShell 顺序准备上述正常、修改、缺失、损坏场景并用 Stopwatch 记录整段进程耗时。正常保存 58 ms、正常加载 61 ms、修改 3500 ms 配置加载 62 ms、50 ms 对照 60 ms；此耗时包含启动开销，仅用于证明本次没有等待 3500 ms，不视为性能基准。
- 原运行时配置先复制到同目录唯一备份，所有场景在 try/finally 内执行，最终恢复原文件并确认 SHA256 与审查前一致，移除审查备份。学员源码未改动。
- 附加观察：两个样例连续写入同一 configPath，第二次覆盖第一次，实际文件仅剩设备 002；这不妨碍“保存一个合法配置”的第一项通过，但当前单配置流程应明确只保存哪个样例。无需为本任务擅自扩展成多设备配置。
- 先修复第 99 行未等待，再完成 TODO 4 的两种异常路径，并将第 89 行接收结果的类型与可能的 null 保持一致。修复后发送“检查 TASK-006”复验。

## 状态与证据

- 2026-10-02：读取课程计划与最新进度，确认下一知识点为文件 / JSON / 配置。TASK-005 功能项已通过，选择以小型设备配置场景推进，同时保留取消原理为待复习。本轮只提前创建这一张任务卡，未创建代码、未运行验收，状态为未开始。
- 2026-10-05：TASK-005 完成任务级验收后，复用本卡作为下一任务；不重复生成新 ID。状态仍为 ⚪ 未开始，未创建练习骨架或启动入口。
- 2026-10-05：用户明确要求“开启 task006”，已创建任务专属骨架并连接共享入口，状态更新为 🟡 进行中。核心实现留给学员，必需 DoD 均尚未验证。
- 2026-10-05 骨架验证（工作区根目录）：`dotnet build Base/Base.csproj --no-restore` 成功，0 警告、0 错误；`dotnet run --project Base --no-build -- save` 输出绝对配置路径与“TODO 2 尚未完成：未保存任何配置文件”；`dotnet run --project Base --no-build -- load` 输出同一路径、TODO 3/4 提示和“未获得可用配置，本次流程结束”。三条命令退出码均为 0；仅验证骨架与入口，未运行真实保存、加载、损坏文件等 DoD 验收。

- 2026-10-05 保存过程答疑：学员已添加配置属性、样例对象、WriteIndented 选项及序列化调用；实际执行 `dotnet build Base/Base.csproj --no-restore`，退出码 1，报 CS4032（SaveAsync 使用 await 但缺 async）与 CS0103（writeAllTextAsync 变量未定义）。已解释局部修复方式，未代改代码或运行保存，DoD 保持未勾选。另观察到两个样例连续写入同一 configPath，WriteAllTextAsync 会覆盖已有内容，需学员决定当前单配置流程只保存一个样例，或明确设计多配置方案；暂无实际覆盖运行证据。任务保持 🟡 进行中。

- 2026-10-05 SaveAsync 局部提交复查：学员自主使用 `Task writeAllTextAsync = File.WriteAllTextAsync(path, configJson); return writeAllTextAsync;`，保留普通 Task 方法且方法体无 await；调用方仍 await SaveAsync，合法。实际 `dotnet build Base/Base.csproj --no-restore` 退出码 0，0 警告、0 错误，原两处编译错误消失。本轮未运行 save/load 或代改代码；连续写同一路径的问题仍在，LoadAsync 新增 ReadAllTextAsync 后通过 Result 同步等待，尚未反序列化，后续应改为 await 取得文本。DoD 保持未勾选，任务保持 🟡 进行中。

## 参考

- [问答：async Task 的结束与任务返回（Q6）](../questions/csharp-task-async.md)（2026-10-05 通关后答疑；概念待复习，本轮未运行或修改源码，任务保持 ✅ 已通关）。
- [问答：WriteIndented 的作用与用法](../questions/csharp-json.md)（2026-10-05 答疑后经自主实现与运行验证，已掌握）。
- [Microsoft Learn：System.Text.Json 序列化](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to)
