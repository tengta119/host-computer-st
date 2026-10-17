# WPF 温度报警提示

> [返回问答索引](./README.md)

## Q1：TASK-028 为什么清空历史后仍可能显示温度超限？

- 日期：2026-10-17。
- 状态：待复习。
- 来源：TASK-028实际提交审查与状态文字协助收尾。

点击清空后，表格使用的_temperatures已经没有温度，但AlarmText是另一个具体文字控件，它的Text不会因为集合变空自动恢复。
骨架会调用UpdateTemperatureAlert，不过空集合分支若直接return，就没有任何一行修改原来的“温度超限”。

    if (_temperatures.Count == 0)
    {
        AlarmText.Text = "暂无温度";
        return;
    }

_temperatures是原温度集合，Count为0表示里面没有任何温度。
AlarmText是教练给窗口中TextBlock取的名字，Text是WPF文字控件提供的属性，这行赋值会把当前文字换成“暂无温度”。
return是C#语法，会立即结束这次方法执行，所以需要显示的文字必须在return之前设置，后面也不会再尝试读取下标Count-1。
集合非空时，Count-1才是最新一条下标，取得的原温度用于与27比较；图中y是画面位置，不能用它判断温度是否超限。

实际可观察结果：添加到28时超限，停止后保留，清空后表格和图归零、文字显示暂无温度，再添加24显示正常。

## Q2：TASK-029 怎样让输入的报警阈值生效？

- 日期：2026-10-17。
- 状态：已掌握。
- 来源：TASK-029开启必要讲解，TryParse与out没有自主应用证据，不能因字典问答讲过out就默认已掌握。

### 输入框文字与有效数值是两处内容

窗口的ThresholdInput是教练命名的WPF TextBox，用户输入的“26.5”保存在它的Text属性中，是字符串。
_alarmThreshold是教练提供的double字段，初始保存数字27，它属于本次Task029Window窗口；不是WPF的内置属性，也不是一个新框架对象。
窗口由App.xaml的StartupUri打开，构造方法调用InitializeComponent创建XAML中的控件，再调用UpdateThresholdDisplay显示字段中的有效数值。

    private double _alarmThreshold = 27;

private表示这个字段供当前窗口类内部使用，double表示它保存可带小数的数值，等号给它初始值27。
输入框改成26并不自动改变这个字段，因此“输入中内容”和“当前有效阈值”可以不同；只有点击应用且检查通过，才保存新值。

首步只到Task029Window.xaml.cs的UpdateTemperatureAlert中，把原比较里的27换成_alarmThreshold。
原来的temperature仍来自同一温度集合最新一条，比较右边改为读取字段，正常/超限两个分支原样保留。
初始字段也是27，所以重新运行后手动24、26.5、25仍正常，28超限，下一条24恢复正常；此时应用按钮尚是占位。

### 把输入文字尝试转换成数值

第二步才进入ApplyThresholdButton_Click，它是教练命名的普通事件处理方法；XAML中应用按钮的Click已连接它，点击时WPF调用这个方法。
先看一段独立的小例子，不是整份应用按钮实现。

    string text = "26.5";
    double candidate;
    bool parsed = double.TryParse(text, out candidate);

string、double、bool、out是C#语法的一部分，text、candidate、parsed是这里自行命名的局部变量。
double.TryParse是.NET提供的数值转换方法，不需要安装包；它接收text中的字符串，并尝试得到double数值。
返回的bool赋给parsed，表示转换是否成功；out candidate让这个方法把得到的数字写入candidate，这个调用结束后candidate已经被赋值。
因此这里有两份结果：parsed回答“转换成功了吗”，candidate保存“转换后的数字是多少”。
输入为26.5时成功，parsed为true、candidate为26.5；输入为abc或空白时失败，parsed为false。
实际按钮里text要来自ThresholdInput.Text，不能一直使用示例字符串。
失败时先通过ThresholdFeedbackText.Text告诉用户“请输入数字”，再return结束本次应用，不能把失败得到的值写入_alarmThreshold。

### 数字转换成功之后还要检查范围

能转换成数字只代表形式可读，例如19.9也能转换，但它不在本题20到30的范围内。
用candidate >= 20和candidate <= 30分别判断两端，C#的&&表示两个条件都满足才接受，等号保留20和30两个端点。
不满足范围时显示原因并return，原_alarmThreshold和CurrentThresholdText都应保持；这里只是拒绝本次参数输入，不清空温度历史，也不停止采集。

### 保存以后让两处文字重新显示

只有检查都通过，才把candidate赋给_alarmThreshold。
UpdateThresholdDisplay是教练提供的普通显示方法，读取_alarmThreshold并更新CurrentThresholdText.Text；它不负责转换或保存。
ThresholdFeedbackText是显示本次成功/失败说明的WPF TextBlock，成功时需要把先前的失败文字更新掉，并移除或替换骨架末尾的占位说明。

最后调用已有UpdateTemperatureAlert，让它用原集合的最新温度和新阈值重新比较。
例如表格最后一条已经是26.5，当前阈值27时正常，应用26后立即超限，再应用27立即正常，历史条数不变。
只调用UpdateThresholdDisplay会更新阈值文字，却不会自动执行另一个提示方法；这两个普通方法由调用顺序连接。
空集合时已有UpdateTemperatureAlert会显示暂无温度并返回，本题继续复用这段处理。

### 2026-10-17 TASK-029实际审查补充

提交使用double.Parse配合try/catch，普通空白/abc能拒绝并保留原阈值和历史；这是可工作的替代方式，不因没用TryParse判定这些场景失败，也不据此认定已经掌握TryParse/out。
有效输入保存字段和更新阈值文字已自主验证，后续自动采集确实读取新值；但应用时没有调用UpdateTemperatureAlert，最新26.5应用26后仍显示正常。
字段赋值不自动执行另一个普通方法，先在成功保存并显示阈值后调用UpdateTemperatureAlert，手动观察26/27切换即时影响同一条26.5。
double.Parse还可接受特殊值NaN，表示“不是一个数”，与上下限比较<或>都不会为true，所以仅凭threshold < 20 || threshold > 30会漏过它。
.NET的double.IsNaN可直接判断这个值，或者只接受同时满足>=20和<=30的数值，其余拒绝；这是此前未展开的基础补充，不把它单独记为学员典型错误。
教练只协助错误反馈的两个显示目标与成功占位文字，核心刷新和范围判断仍留给学员修复；协助后98项检查92通过，DoD1/2未通过、3通过，任务进行中。
本Q2继续待复习，已验证局部应用保留，问答24分类/56条、待复习33/已掌握23不变，证据见[TASK-029](../tasks/TASK-029.md)。
### 追问：输入NaN为什么没有进入catch？

- 2026-10-17：用户明确追问double.Parse输入NaN是否应进入catch，本轮补充“特殊值可被成功解析”与“解析抛异常”的区别，不推定为学员已掌握，也不新增错题。

用户当前代码中threshold = double.Parse(thresholdInputText)只有在Parse抛出异常时才进入catch，catch不会因为结果不适合当温度阈值就自动执行。
字符串abc没有可识别的double表示，Parse会抛FormatException；字符串NaN却是.NET允许的特殊表示，Parse正常返回double.NaN，所以继续执行try后面的代码。
NaN全称Not a Number，表示非数值的浮点特殊状态；它是double能保存的一个值，名字“不是数字”不等于“解析格式错误”。

    double threshold = double.Parse("NaN");
    bool special = double.IsNaN(threshold);

threshold是示例局部变量，Parse来自.NET并成功返回特殊值，double.IsNaN也是.NET提供的方法，判断传入值是否为NaN，special在此为true。
当前范围条件中的threshold < 20和threshold > 30都为false，因此还需要明确拒绝NaN或仅接受>=20且<=30都满足的值。
改为TryParse也不会自动拒绝NaN；实际.NET 10验证TryParse返回true，同时out结果仍为NaN，仍须检查结果是否符合任务范围。

实际使用SDK 10.0.401执行dotnet run --project WpfBae/Exercises/TASK-029-alarm-threshold/Verification/ParseProbe.csproj -p:NuGetAudit=false，退出0，26.5成功、abc进入FormatException分支、NaN成功且IsNaN=True、两个越界比较均False、TryParse NaN成功。
验证源码[ParseProbe.cs](../../WpfBae/Exercises/TASK-029-alarm-threshold/Verification/ParseProbe.cs)位于本题独立目录，仅作概念验证；未修改学员窗口源码或重新验收DoD。
依据：[微软Double.Parse说明](https://learn.microsoft.com/en-us/dotnet/api/system.double.parse?view=net-10.0)、[微软Double.IsNaN说明](https://learn.microsoft.com/en-us/dotnet/api/system.double.isnan?view=net-10.0)。
TASK-029保持🟡 进行中，DoD1/2未通过、3通过，Q2继续待复习，问答数量与掌握统计不变。
### 2026-10-17 TASK-029最终复查与掌握范围

当前Q2状态为已掌握，范围限本骨架里的阈值字段连接、Parse/catch普通非法输入保护、20到30范围检查、IsNaN拒绝、有效字段保存、调用显示方法和立即重判原温度。
学员已自主在Parse后识别NaN并主动抛异常交catch处理，成功保存后调用UpdateTemperatureAlert，98项实际窗口检查全部通过；本轮未代改源码。
错误反馈的显示目标和成功文字由教练前轮收尾，窗口/字段骨架/显示方法与已有业务为提供或复用，不计新自主设计。
TryParse/out只有讲解及教练小程序验证，未因本题通关一并计入掌握；完整浮点规则、异常类型设计也不扩大认定。
三个DoD均已通关，证据见[TASK-029最终复查](../tasks/TASK-029.md)，旧进行中/失败/待复习结论均为各次历史。
当前24分类/56问答、待复习32/已掌握24，Q1仍待复习。
### 开启时证据与练习顺序

先做TODO 1并实际观察原样例，再依次做转换、范围、保存、立即刷新，每一步对照任务卡中的观察结果。
39项实际骨架检查、默认960×800与最小860×760布局边界及原exe启动关闭已通过，核心TODO均待实践；本节为教练讲解，不新增自主掌握。
新增本条后，问答总计24分类/56条，待复习33、已掌握23。
## 实践证据与掌握边界

2026-10-17实际窗口最终104项检查通过，三个DoD通关，详见[TASK-028](../tasks/TASK-028.md)。
学员自主范围：空集合先返回、Count-1读取最新原温度、>=27阈值及else恢复正常，手动/自动和27/26.99边界已实际验证。
空分支文字赋值由教练按既有低影响收尾要求补充，不算自主实现；初始化、集合事件连接及绘图是复用骨架，整体Q1继续待复习。