# WPF 周期通知与模拟采集

> [返回问答索引](./README.md)

## Q1：TASK-027 怎样先观察每秒通知，再接上温度采集？

- 日期：2026-10-14。
- 状态：待复习。
- 来源：TASK-027 开启时的必要讲解，定时器尚无学员自主应用证据。
- 当前文件：[窗口后台](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Task027Window.xaml.cs)、[窗口界面](../../WpfBae/Exercises/TASK-027-start-stop-sampling/Task027Window.xaml)。

窗口已经能手动添加24、26.5、25、28度，右边表格显示完整历史，左边显示最新最多三条；本题希望不用每次点添加，也能约每秒增加一条，并能停止后保留历史。

### 先找到会发通知的对象

后台已有教练提供的两处代码，分别放在窗口字段区和构造方法里。

```csharp
private readonly DispatcherTimer _samplingTimer = new DispatcherTimer();
```

DispatcherTimer是WPF提供的定时器类，new是C#创建对象的语法；右边创建一个定时器，教练命名的字段_samplingTimer保存它，窗口后续一直操作这一个对象。

private表示这个字段供当前窗口类内部使用，readonly限制字段改指另一个定时器，仍允许设置原对象的属性、调用原对象的方法。

```csharp
_samplingTimer.Interval = new TimeSpan(0, 0, 1);
```

TimeSpan是.NET提供的时间长度类型，这个三个整数的写法依次表示小时、分钟、秒，所以右侧表示一秒；Interval是WPF定时器提供的属性，这一行把这个定时器的期望通知间隔设为一秒。

这里保存的是时间长度，不是某个具体时刻，也不是温度值；设置Interval本身不会开始通知。

### 告诉定时器到时调用谁

Tick是这个定时器提供的事件，到时会调用已经登记的接收方法；SamplingTimer_Tick是教练自行命名的普通C#方法，名字本身不会让定时器找到它。

```csharp
private void SamplingTimer_Tick(object? sender, EventArgs e)
{
    // 首步在这里累加通知次数并显示。
}
```

void表示这个方法不返回结果，sender在通知时接收发通知的定时器，object?表示允许为空；EventArgs是.NET提供的事件参数类型，e接收本次事件参数，本题不需要从中读取温度。

在构造方法的TODO 1a登记一次：

```csharp
_samplingTimer.Tick += SamplingTimer_Tick;
```

左侧是这一个定时器的Tick事件，右侧是这一个窗口的接收方法；+=是C#登记方法的写法，方法名后没有括号，所以这一行登记但不立即执行方法，与已经练习过的设备事件登记作用相同。

必须只在构造方法登记一次，不能每次点启动又登记一次，否则到时会重复调用同一个方法。

### 首步只看通知次数

_tickCount是教练提供的整数计数，表示窗口总共收到多少次Tick；TickCountText是XAML里教练命名的WPF TextBlock文字控件，Text是它要显示的文字属性。

在接收方法的TODO 1b填写：

```csharp
_tickCount = _tickCount + 1;
TickCountText.Text = "通知次数：" + _tickCount;
```

第一行读取原次数，加一后存回同一个字段；第二行把文字与这个数拼接，设置通知次数控件的Text，所以运行时会看到1、2、3依次增加。

运行项目后，点击“观察通知（临时）”，预期约每秒增加一次次数；再次点击这个按钮停止观察，次数应保持，此时完整历史仍是0，图为空，因为首步尚未连接温度追加。

ObserveButton是教练为首步新增的临时按钮，ObserveButton_Click是已提供的按钮接收方法；它对同一个定时器调用Start或Stop，按钮文字也随观察状态改变。

Start是WPF提供的启动方法，安排后续到时通知，不能替代Tick登记；Stop是WPF提供的停止方法，停止后续通知，不会删除历史，也不会改变下一条样例位置。

当前“启动”和“停止”按钮已经连到StartButton_Click和StopButton_Click，但这两个方法仍留TODO；首步使用临时观察按钮，不能把正式按钮尚未动作当成界面入口出错。

### 次数可见后，再把一次通知接到一条温度

AppendNextSample是教练把原添加按钮内容提取出来的普通方法，不是WPF API；每次执行它取到下一条样例、移动样例位置，并对原_temperatures执行一次Add。

```csharp
AppendNextSample();
```

后续TODO 3把这一次普通调用放进SamplingTimer_Tick；圆括号表示立即执行方法，到时通知一次便追加一条，不能再额外给表格、计数或折线各建一份温度集合。

HistoryDataGrid.ItemsSource已由骨架指向原_temperatures，集合原有的CollectionChanged接收方法也已连接；Add会让表格更新，再由该接收方法读取真实Count和重画最新三点，这条显示链沿用已验证的任务实现。

通知次数与完整历史条数不同，手动添加不会增加通知次数，清空历史也不要求重置通知次数；ReadingCountText始终读取原温度集合的Count。

### 正式启停与清空随后完成

TODO 2a/2b分别在正式启动/停止方法中操作原定时器，启动前可用_samplingTimer.IsEnabled判断它是否已经运行，避免重复启动路径；这里IsEnabled是WPF定时器的运行状态属性。

UpdateSamplingControls是教练预留的普通辅助方法，供两条正式按钮路径在操作后刷新界面；SamplingStateText是状态TextBlock，用它的Text显示“采集中”或“已停止”。

TODO 4还需设置ClearButton.IsEnabled；这里ClearButton是窗口里的清空按钮，这个按钮的IsEnabled控制能否点击，与定时器同名属性各自作用于不同对象。

当定时器运行时，清空按钮应禁用；停止时清空按钮应可用，启动/停止按钮状态也应一致，不把界面文字当作定时器实际状态。

TODO 5在清空方法开头增加运行状态保护；正在采集时直接返回，停止后才沿用下方已有Clear和样例位置复位，所以下次添加或采集从24开始。

正式启停完成后，把临时ObserveButton的Visibility设为Collapsed，让它不显示也不占布局位置；这项收尾避免临时观察路径与正式界面状态混用。

OnClosed关闭处理由教练提供，窗口关闭时停止定时器，并以-=解除Tick和集合变化的同一接收方法登记；本题无需学员重新设计关闭收尾。

### 通知在哪执行

这个定时器在窗口的界面线程上发Tick，当前接收方法只短暂追加模拟数值，可直接更新当前控件；界面繁忙时通知可能延后，一秒是期望间隔，不能视为精确采样保证。

本题没有真实设备阻塞读取，不在接收方法里Thread.Sleep，也不引入后台线程或真实通信；真实采集线程和界面更新的连接将在需要时展开。

依据：[Microsoft DispatcherTimer 文档](https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?view=windowsdesktop-10.0)。

### 2026-10-17 TASK-031追问：_samplingTimer.IsEnabled是什么意思？

_samplingTimer是当前窗口中保存的那一个WPF DispatcherTimer定时器，由教练自行命名；IsEnabled是WPF给这个定时器提供的属性。
_samplingTimer.IsEnabled读取“这个定时器是否处于启动状态”，结果是C#的bool值true或false。
调用_samplingTimer.Start()后它为true，定时器安排后续Tick；调用_samplingTimer.Stop()后它为false，停止后续通知。
点号表示访问左边对象的成员，这里访问的是定时器的IsEnabled属性；读取这个属性本身不会启动或停止定时器。

```csharp
if (_samplingTimer.IsEnabled)
{
    ExportStatusText.Text = "正在采集，请先停止再导出。";
    return;
}
```

if是C#的条件判断，属性为true时执行大括号里的语句，为false时跳过这一块，继续后面的代码。
上例在正在采集时显示原因，return结束这一次ExportTemperatureHistory调用，避免继续执行后续导出；return本身不会停止采集。
_samplingTimer.IsEnabled的意思等同于这里写_samplingTimer.IsEnabled == true，可以直接把bool值放入if。
ClearButton.IsEnabled虽然同名，但左边是按钮，表示按钮能否点击；这里左边是定时器，表示它是否启动，需看具体对象。
预期观察：点击启动后点导出，显示先停止；点击停止后再点导出，跳过这个分支，但仍需另行处理空历史和实际写入。
本次仅根据当前源码答疑与补充记录，未修改学员代码或运行验收，TASK-031仍🟡 进行中，DoD与掌握范围不变，Q1仍待复习。
### 2026-10-18 TASK-038审查：通知次数在增加，为什么温度没有增加？

当前SamplingTimer_Tick只执行_tickCount++和更新TickCountText，实际运行已经收到通知，但方法中没有调用_viewModel.AppendNextSample()，所以历史不会自动增加。
在该接收方法里调用同一VM的追加入口一次，才能把一次通知接成一条温度；原定时器与Tick登记已正确，无需重新登记或创建另一个对象。
本轮真实启停、状态同步、停止后不变与关闭收尾已验证，自动追加没有通过，TASK-038保持进行中；原TASK-027已验证的局部能力保留，Q1整体仍待复习，未代写此次核心调用。
### 2026-10-18 TASK-038第二次审查：Tick已接通原追加入口

学员已在SamplingTimer_Tick内调用同一VM.AppendNextSample一次，真实定时采集、手动混合顺序、重复启停不重复追加、停止后保留历史、运行保护和关闭停止均通过。
264项功能全部通过，保留这些自主连接证据；定时器对象/间隔/安全收尾由骨架提供，Q1原待复习范围不因局部运行通过全部升级，TASK-038仅待运行记录，见[结果](../tasks/evidence/TASK-038/review-02/results.json)。

### 当前证据与掌握边界

2026-10-14最终复查：用户要求教练直接收尾状态文案，教练已修正运行/停止文字及骨架过期提示，实际29项全通过、三个DoD通关；学员自主Tick连接/计次/单次追加、原定时器启停与保护、清空条件/运行保护、临时按钮隐藏的证据保留，协助状态文字不计自主掌握，Q1整体仍待复习。最终证据见[复查](../tasks/evidence/TASK-027/recheck-02/README.md)，下方文字相反与未通过为历史，不要求恢复错误状态；状态文案等低影响收尾今后由教练主动修正验证，避免学员反复提交。

2026-10-14第一次复查：运行清空禁用/停止清空恢复、正常按钮清空/重启24、清空方法运行保护和ObserveButton隐藏已自主修复并实际通过，自动五条、停止等待2250毫秒/继续26.5/重复启停/同集合显示/默认与最小布局仍通过；29项检查27通过、0异常、59项原源码未改，DoD1/3通过，2只剩状态文字相反，见[复查证据](../tasks/evidence/TASK-027/recheck-01/README.md)。

UpdateSamplingControls里if的_samplingTimer.IsEnabled为True表示这个定时器正在运行，但该分支当前文字是“定时任务以取消”；else表示没有运行，文字却是“真正执行定时任务”，所以运行和停止的文字赋值也被换了位置。

这次只修SamplingStateText.Text的两个赋值，让运行分支写“采集中”、停止分支写“已停止”；ClearButton.IsEnabled的运行False/停止True已经正确，不需要再交换整个分支，界面文字不会自行读取定时器状态。

Q1整体继续待复习，新增已验证的清空控制/运行保护和隐藏按钮局部应用范围，保留此前事件/追加能力；下方清空条件相反与临时入口仍可见是首次审查历史，不要求恢复。

2026-10-14首次实际审查：已自主连接Tick、累加次数、每次调用AppendNextSample、在正式按钮操作原定时器；自动取得五条24/26.5/25/28/24、停止至少2250毫秒不新增、再启动得到26.5、重复启停单次追加、同集合显示均通过，这些局部应用证据保留。

### 审查发现：为什么采集中能清空，停止后却不能清空？

实际启动后ClearButton可点，点击会把原历史删掉；实际停止后ClearButton变灰，正常点击入口不能清空。

原因在UpdateSamplingControls：定时器运行的if分支给ClearButton.IsEnabled赋True，停止的else分支反而赋False，按钮可用条件与任务要求相反。

这里同名的两个属性要落到具体对象：_samplingTimer.IsEnabled回答“这个定时器是否运行”，ClearButton.IsEnabled设置“这个清空按钮是否能点”；True对按钮表示可以点击，False表示禁用，不表示按钮是否正在执行清空。

第一个修复只调整两个分支对清空按钮的赋值，运行中禁用，停止后启用；实际运行点击启动时清空应变灰，点击停止时应恢复可点，先确认这个可观察变化。

随后再完成ClearButton_Click开头的运行状态保护，正在运行就返回，停止才执行已有的样例位置复位和Clear；按钮禁用控制正常入口，方法保护限制清空方法本身，两个动作各有作用。

临时ObserveButton仍显示，它直接Start/Stop但没有调用UpdateSamplingControls，所以定时器已运行而状态文字仍写“定时任务以取消”；正式启停接通后按任务约定将临时按钮Visibility设为Collapsed，隐藏同时移除占位，避免保留这个绕过正式状态更新的入口。

### 追问：如何隐藏临时观察按钮？

2026-10-14学员询问具体隐藏写法；原说明只要求设置Visibility，没有展示当前标签的实际改法，本次补充，不记学员错误。

在Task027Window.xaml的ObserveButton标签内添加Visibility="Collapsed"，保留原名字、文字和Click登记：

```xml
<Button x:Name="ObserveButton" Content="观察通知（临时）" Padding="12,6" Margin="12,0,12,0" Click="ObserveButton_Click" Visibility="Collapsed" />
```

ObserveButton是教练命名的那个临时按钮，Visibility是WPF提供的显示属性，Collapsed表示这个按钮不显示也不占布局位置，所以右边的“添加下一条”会向左靠拢。

如果使用Hidden，按钮也不显示，但会保留它原来占用的空位；本题选择Collapsed，避免留下临时按钮的空白位置。

保存后在工作区根目录执行dotnet run --project WpfBae/WpfBae.csproj，预期看不到“观察通知（临时）”，正式启动/停止、添加和清空仍显示；这里说明的是预期现象，本轮未代改源码或重新运行验收，TASK-027状态和DoD保持。

当前DoD1通过、DoD2/3未通过，Q1整体保持待复习；29项检查23通过、0运行异常，59项原源码未改，不扩展为后台通信、精确计时或完整独立采集设计，证据见[首次审查](../tasks/evidence/TASK-027/review-01/README.md)。

下方是开启时教练骨架验证历史，原“核心仍TODO”不能覆盖上方最新局部实践证据。

2026-10-14教练已实际显示骨架窗口，默认960×680和最小860×640布局检查通过，手动添加/停止状态清空、临时观察启停和关闭停止通过；原exe启动TASK-027正常关闭退出0，最终主项目构建0警告/错误。

Tick登记、次数累加、自动追加、正式启停状态与采集时清空限制仍为TODO，当前结果不算任务DoD通过，Q1保持待复习，不扩大此前已掌握范围。
