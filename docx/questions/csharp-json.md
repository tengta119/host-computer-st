# C# JSON 与配置文件

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：JsonSerializerOptions.WriteIndented 有什么用，如何使用？

- 状态：已掌握
- 日期：2026-10-05
- 关联：[TASK-006](../tasks/TASK-006.md) 的 TODO 2。

TASK-006 保存设备配置时，生成紧凑的一行 JSON 可以被程序读取，但人手工看 DeviceId、ReadDelayMs 不方便。WriteIndented 的效果是给 JSON 换行、缩进，字段和值保持原样。

在已有 SaveAsync 方法中可这样读这三行：

```csharp
JsonSerializerOptions options = new JsonSerializerOptions();
options.WriteIndented = true;
string json = JsonSerializer.Serialize(config, options);
```

JsonSerializerOptions 是 .NET 提供的选项类，new 创建选项对象，options 保存它的引用。第二行设置这个对象的 WriteIndented 属性。JsonSerializer.Serialize 是 .NET 提供的序列化方法；config 是传入 SaveAsync 的本项目 DeviceConfig 对象，options 是本次使用的选项。

Serialize 得到 string，保存到 json。若 config 的 DeviceId 为 002、ReadDelayMs 为 200，便得到类似：

```json
{
  "DeviceId": "002",
  "ReadDelayMs": 200
}
```

只创建 options 却没传给 Serialize，本次调用仍不会使用这项设置。设置为 false 时通常得到紧凑文本。它与编辑器格式化 JSON 的可读性效果相似。

原源码使用 `new JsonSerializerOptions { WriteIndented = true }`，是在创建对象后用初始化器设置同一属性。先看上面的分步写法，再看此简写更容易对应动作。

json 还没有自动变成磁盘文件。后面的 `await File.WriteAllTextAsync(path, json)` 才向指定路径写文件；WriteIndented 不负责保存，加载也不要求开启相同选项。当前真实代码见 [TASK-006](../../Base/Exercises/TASK-006-json-config/Task006Exercise.cs)。

## Q2：TASK-030 怎样把当前有效阈值保存到文件，再在启动时恢复？

- 日期：2026-10-17。
- 状态：已掌握。
- 来源：TASK-030开启必要讲解；TASK-006中JSON保存/加载已实践过，但本题启动恢复、单参数验证和控件同步尚无自主证据。

### 保存按钮应该读取哪一个值

Task030Window里的_alarmThreshold保存已经成功应用的数值，ThresholdInput.Text则是输入框里的文字。
先应用26，再把输入框改成29但不点应用，此时有效字段仍为26，保存入口应读取26。
SaveThresholdButton是教练命名的WPF按钮，Click已连接SaveThresholdButton_Click，后者调用教练新建的普通方法SaveThresholdSettings。
ConfigStatusText是教练命名的WPF TextBlock，设置它的Text属性只改变配置结果这一处文字。

首步只在SaveThresholdSettings中用_alarmThreshold替换占位说明，例如下面一行不写入文件。

    ConfigStatusText.Text = "准备保存的有效阈值：" + _alarmThreshold;

运行后先应用26，再点保存，预期显示26；把输入框改为29但不应用，再点保存仍显示26。
这个观察通过后，再将显示入口接到实际JSON保存。

### 配置对象、JSON文字与磁盘文件

AlarmThresholdSettings是教练新建的配置类，只保存一个名为Threshold的double属性，不是.NET内置类型。
它在本题AlarmThresholdSettings.cs里定义，普通get/set用于保存和读取这一个数值，界面与采集历史不在其中。

    AlarmThresholdSettings settings = new AlarmThresholdSettings();
    settings.Threshold = _alarmThreshold;

第一行通过C#的new创建一个具体配置对象，settings变量指向它；第二行把窗口已经生效的阈值复制到这个对象的Threshold属性。
这不会让对象与字段自动持续同步，所以每次保存应从当时有效字段取值。
例如字段为26，这份对象就保存26，接下来才把对象转换为JSON文字。

    string json = JsonSerializer.Serialize(settings);

JsonSerializer是.NET的System.Text.Json提供的类型，Serialize方法读取对象的公开属性，生成字符串，json接到结果。
示例结果是{"Threshold":26}，其中Threshold来自自定义属性名，26来自该对象当前保存的值。
你已在TASK-006验证过WriteIndented=true的格式选项，本题可继续用它让文件便于阅读，但缩进不负责写文件。

    File.WriteAllText(_settingsPath, json);

File是.NET的System.IO提供的文件操作类型，WriteAllText把json写入_settingsPath指向的文件，成功返回后才能显示保存成功。
如果目标文件已有内容，它会覆盖该文件，因此只操作本题明确的配置路径；加载时不得先执行保存。
本题数据很小，使用普通同步读写即可，不要求改造原采集为后台线程。

### 文件实际在哪里

_settingsPath是教练提供的窗口字段，Path.Combine把目录片段和文件名组成完整路径，AppContext.BaseDirectory指运行程序所在目录，不是源代码目录。
从工作区根目录运行WpfBae项目时，默认Debug路径为WpfBae/bin/Debug/net10.0-windows/Exercises/TASK-030-threshold-settings/alarm-threshold.json。
窗口ConfigPathText显示当前运行实际使用的绝对路径，以窗口为准；审查项目使用自己的输出目录，所以其路径不同。
保存前需创建该路径的父目录，Directory.CreateDirectory负责确保目录存在，Path.GetDirectoryName可以从完整文件路径取得父目录。
Directory和Path来自.NET的System.IO，GetDirectoryName取得文件上一级目录，CreateDirectory确保目录存在，已有目录时不会清空它。
GetDirectoryName的返回值可能为null，先检查它不为空再交给CreateDirectory，使用普通if判断即可；目录创建也可能失败，和写文件一样要有失败说明。

### 启动时把文件读回配置对象

LoadThresholdSettings是教练提供的普通方法，构造方法在控件和原集合/定时器连接完成后调用它一次，新增读数不会重新加载。
字段和输入框初始都是27，所以缺失文件或加载失败时可以保留这份默认值并显示原因，不创建或覆盖文件。

    string json = File.ReadAllText(_settingsPath);
    AlarmThresholdSettings? settings = JsonSerializer.Deserialize<AlarmThresholdSettings>(json);

ReadAllText读出整个文件的字符串，不修改原文件。
Deserialize是.NET的JSON还原方法，尖括号中的AlarmThresholdSettings告诉它要还原成本题这个配置类；json是输入文字，settings接到还原出的对象。
问号表示settings可能为空，因为文件内容为JSON的null时，方法可以返回null；必须先判断，再访问settings.Threshold。
损坏JSON会抛出异常，应显示加载失败并保留默认27，不能启动时用默认配置覆盖坏文件。
合法JSON也要检查数值，例如{}会使未设置的Threshold为0，{"Threshold":35}虽格式合法但超出本题范围，均不能直接应用。
只有对象存在、Threshold符合20到30且非NaN等本题合法性要求时才接受。

### 恢复数值以后还要同步界面

成功读取后，把settings.Threshold赋给_alarmThreshold，把这个数值的ToString()结果赋给ThresholdInput.Text。
ToString()是数值提供的方法，在这里把26这样的数字变成输入框需要的文字。
再调用已有UpdateThresholdDisplay显示有效字段，调用UpdateTemperatureAlert重判已有读数；初始历史为空时它会保留暂无温度。
应用阈值、保存文件、启动加载是三个具体入口，本题不要求每次应用自动保存，也不要求把温度历史写入配置。
保存异常要显示失败并继续可用；加载失败要保留默认值和原文件，两处错误都显示在ConfigStatusText，不覆盖温度报警提示。

### 2026-10-17 TASK-030审查补充：读回对象以后还不能直接应用

已自主通过：用当前有效字段创建配置对象/WriteIndented序列化/创建父目录/写文件、未应用或拒绝输入仍保存旧有效值、关闭后新窗口恢复26与界面/原温度判断、缺失/损坏JSON及写入失败时保留可用状态。
未通过：JSON为null时Deserialize返回空值，直接访问Threshold使窗口构造失败；{}默认Threshold为0，35/19.9会直接生效，缺少独立的配置合法性检查。
Deserialize能读懂文件文字，不保证返回对象存在，也不保证属性值符合温度范围；先判断对象不为null，再判断其Threshold合法，最后才赋给窗口字段和输入框。
AlarmThresholdSettings?中的问号表示变量允许为空，只向编译器表达这种可能性，不会替你执行判断，也不会自动进入catch。
JSON的null是一个合法内容，方法可以正常返回null；你当前在try块之后访问属性，因此此处的NullReferenceException也不受前面的catch保护。
先在成功还原后、第一次访问Threshold之前做空值分支显示说明并return，再接数值校验；{}的0与35等格式合法值同样要拒绝。
教练仅收尾读取失败提示写入错误控件的问题，null和范围核心未代改；Q2仍待复习，TASK-030 DoD1/3通过、2未通过，原用户配置未动，证据见[任务卡](../tasks/TASK-030.md)。
当前24分类/57条、待复习33/已掌握24不变，保留前述局部自主实践，不因尚未通关全部归零。
### 2026-10-17 再次审查：return不会撤销前面的赋值

学员已自主增加null保护，null配置正常打开且保留27，范围外也能显示错误；这些局部应用已通过。
现在剩余问题是先给_alarmThreshold赋文件值，再检查是否越界，所以35虽被提示非法，窗口真正用于比较的字段已经是35。
alarmThresholdSettings.Threshold是这次从文件读到的候选值，_alarmThreshold是当前已生效值，二者承担不同用途。
先在条件中验证候选值，不合法就返回；只有合法，才把候选值写入有效字段，再同步输入框和文字。
return结束这次方法后续语句，不会回滚刚才执行过的赋值。
本轮147项检查144通过、3失败均为这个顺序造成，DoD1/3保持通过、2仍未通过，问答状态与24分类/57条、待复习33/已掌握24统计保持。
教练只整理一处错误说明中的字段名，核心顺序未代改，详见[TASK-030](../tasks/TASK-030.md)。
### 2026-10-17 TASK-030最终复查与掌握边界

Q2当前标已掌握，范围限现有骨架中从有效字段创建配置对象、序列化/写文件、读取/反序列化、缺失和损坏处理、null和数值保护、先验证候选值后赋有效字段、成功同步输入和显示的自主应用。
学员已自主修复赋值顺序，147项实际窗口检查全通过，原用户配置不变，本轮源码未代改，三个DoD通关，见[TASK-030](../tasks/TASK-030.md)。
先前错误提示目标和一处说明文字由教练收尾，类/路径/方法签名及窗口业务为提供或复用，不扩大为独立配置架构设计。
可空接收声明仍有CS8600警告，null分支已实际验证；不把未消除的声明警告说成没有风险，也不因它撤销已证实的运行保护和本卡验收。
当前24分类/57条，待复习32/已掌握25；本节之前各次失败、进行中和待复习为历史。
### 开启时证据与掌握边界

独立骨架提供了类、路径和方法签名，核心对象使用/JSON读写/错误保护/加载同步留五个TODO，不计学员自主掌握。
实际构建0警告/错误、50项骨架检查及原exe启动关闭通过，详见[TASK-030](../tasks/TASK-030.md)；三个DoD均待实践。
Q1已有WriteIndented自主证据保持，Q2待复习；当前24分类/57问答，待复习33/已掌握24。
## 历史证据与掌握范围

Q1 保持已掌握。2026-10-05 学员自主将 WriteIndented=true 的选项传给 Serialize 并完成写入，save 入口退出 0，实际合法 JSON 含设备 002、200 ms 和换行缩进。此前无法编译/尚未验证的描述是修复前历史，完整旧稿保留。本次没有重新执行保存。

相关任务卡与验收来源：[TASK-006](../tasks/TASK-006.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/csharp-json.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Microsoft Learn：序列化及格式化 JSON](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to)
