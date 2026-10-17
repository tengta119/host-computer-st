# C# CSV历史导出

> [返回问答索引](./README.md)

## Q2：TASK-032 怎样选择保存位置，确认或取消以后分别做什么？

- 日期：2026-10-17。
- 状态：已掌握。
- 来源：TASK-032开启必要讲解，TASK-031的完整组行和固定格式读写已验证，保存窗口与结果判断尚无自主证据。

### 保存窗口给用户选择什么

本题点击“导出历史”后，希望出现Windows保存窗口，用户可以切换目录、输入文件名，并点击保存或取消。
保存窗口只负责取得选择，实际CSV仍由我们自己的文件写入代码保存。
Microsoft.Win32.SaveFileDialog是.NET为WPF提供的文件保存对话框类，完整名称已写在代码中，不需要安装新依赖。
本题后台里的CreateCsvSaveDialog是教练新建的普通辅助方法，它创建一个SaveFileDialog并返回这个对象，不会自己显示窗口。

```csharp
Microsoft.Win32.SaveFileDialog dialog = CreateCsvSaveDialog();
```

dialog是我们自行命名的局部变量，指向本次新建的保存窗口对象，后面用这个变量打开窗口和读取用户选中的路径。
辅助方法里的FileName给文件名输入框提供temperature-history.csv，Filter让文件类型显示CSV，DefaultExt和AddExtension在省略扩展名时补上csv。
Filter里的CSV文件 (*.csv)是显示给人看的说明，竖线后面的*.csv是匹配CSV文件的规则。
OverwritePrompt=true让选择已有文件时出现覆盖确认；这个确认不会替我们完成File.WriteAllText。
CreateTestFile=false避免保存窗口尝试创建测试文件，便于首步只观察路径。
这些属性设置由教练准备，不算学员自主实现保存窗口。

### 打开窗口以后接收确认或取消的结果

```csharp
bool? result = dialog.ShowDialog(this);
```

ShowDialog是.NET提供的方法，这一行显示dialog指向的保存窗口，用户完成操作并关闭它后，才会继续执行后面的语句。
this是C#表示当前对象的写法，这里指当前Task032Window监控窗口，传入它使保存窗口属于当前监控窗口。
result是示例自行命名的变量，接收本次选择结果。
bool表示true或false，bool?在这里表示还允许null，因此不能直接把result当成普通bool放入if。
只把result == true当成用户确认；其他结果统一按未确认处理，显示取消并return，不写任何文件。

```csharp
if (result == true)
{
    ExportStatusText.Text = "选择的路径：" + dialog.FileName;
}
else
{
    ExportStatusText.Text = "已取消导出。";
    return;
}
```

FileName是保存窗口提供的属性，确认后读取它可取得用户选中的完整文件路径；这里把路径写到结果文字控件，只是显示，不是写文件。
else是C#条件不成立时的分支，return结束这次ExportTemperatureHistory方法调用，防止后面继续写文件。
上面代码是本题首步路径观察，不要保留方法末尾的占位文字赋值，否则刚显示的结果会被覆盖。
运行WpfBae项目后添加一条，在停止状态点导出，选择一个新文件名并确认，结果应显示完整路径，但文件尚不存在；再点导出并取消，应显示已取消。

### 观察结果后再连接已有写入

BuildCsvText是教练从TASK-031提取的已验证组行方法，返回表头与全部原历史的CSV文字，固定小数格式保持。
WriteCsvToPath是教练从原写入代码提取的方法，它的string path参数接收完整目标路径，再创建父目录、覆盖写文件、捕获失败并显示实际结果。
确认路径观察通过后，只有确认分支才将dialog.FileName传给WriteCsvToPath，不再使用TASK-031的固定_exportPath。
不要在确认前写文件，也不要先调用写入再检查取消；打开窗口本身如果抛出异常，也要显示原因并继续使用。
当前停止/空历史保护已经沿用并放在创建保存窗口之前，不要求学员重写；本题还需实际验证覆盖确认、取消、不同目录及写入失败恢复。

### 当前证据与边界

骨架已实际显示并检查默认/最小布局、原集合与五条组行、停止/空历史保护、对话框配置和原exe关闭，尚未接通ShowDialog和写入连接。
没有把属性检查说成实际交互过保存窗口，确认、取消与覆盖操作仍待首步实践和后续验收。
Q2待复习，TASK-032进行中，三个DoD未勾选，TASK-031与Q1已有自主证据保留。
依据：[Microsoft SaveFileDialog文档](https://learn.microsoft.com/en-us/dotnet/api/microsoft.win32.savefiledialog?view=windowsdesktop-10.0)。

### 2026-10-17 TASK-032通关与掌握边界

Q2按ShowDialog接收bool?结果、result != true时返回、确认后将dialog.FileName传入已有写入方法的自主应用标已掌握，78项实际检查全通过，见[TASK-032审查](../tasks/TASK-032.md)。
真实保存窗口在两个目录导出、取消、拒绝覆盖与接受覆盖均运行验证；ShowDialog无参数重载本次可用，不要求仅为传this重提。
教练直接删除外层无条件成功文字以保留写入失败结果，另整理取消说明与一处错字，这三处不算学员自主实现。
WriteCsvToPath的return只结束写入方法，调用者ExportTemperatureHistory仍会继续执行，所以原外层成功赋值会覆盖失败文字；由负责写入的方法显示最终成功/失败结果即可。
保存窗口配置、原CSV组行与写入辅助方法为提供或复用，打开对话框异常未实现/模拟，不扩大为这些内容均已自主掌握。
当前25分类/59条、待复习32/已掌握27，Q1范围保持，前述Q2待复习/未接通为开启历史。
## Q1：TASK-031 怎样把完整温度历史写成CSV？

- 日期：2026-10-17。
- 状态：已掌握。
- 来源：TASK-031开启时的必要讲解，已有JSON文件操作与集合追加证据保留，CSV组行与完整历史导出尚无自主证据。

### 先确认按钮读取的是哪份数据

Task031Window是本题复用并新建的窗口类，里面的_temperatures保存全部原始double温度，HistoryDataGrid直接使用这个集合显示表格。
TemperatureLine.Points保存绘图坐标，最多三个点，点的Y是画布位置，不能作为导出的温度。
ExportHistoryButton是教练新建的WPF按钮，点击后进入ExportHistoryButton_Click，再调用ExportTemperatureHistory。
后两个名字都是教练自行命名的普通方法，不是.NET提供的API。
ExportStatusText是新增的WPF文字控件，设置它的Text属性会改变导出按钮下面的结果说明。

首步在ExportTemperatureHistory的TODO 1处，把末尾占位赋值替换为下面这一行。

```csharp
ExportStatusText.Text = "准备导出的历史条数：" + _temperatures.Count;
```

Count是.NET集合提供的属性，表示这份原集合此时保存了多少条记录；加号把数字接在前面的文字后面，再赋给这个文字控件。
运行项目，在停止状态点击五次“添加下一条”，再点击“导出历史”，应看到5；表格五条，图中最多三点。
这一步只观察条数，不创建文件，也不代表导出验收通过。

### CSV文件每一行保存什么

本题文件第一行是Sequence,TemperatureC，逗号把两个列名分开，分别表示序号与摄氏温度。
接着每条记录独占一行，例如前两条为下面的文字。

```csv
Sequence,TemperatureC
1,24
2,26.5
```

第一条原集合下标为0，但文件要显示序号1，因此序号来自原集合下标加1。
第二个字段直接来自_temperatures中相应的double值，不需要先换算绘图坐标，也不添加°C单位文字。
每条记录后需要换行，否则多条数据会挤在同一行。
Environment.NewLine是.NET提供的换行字符串，在Windows上用于让下一条从下一行开始；源代码中的换行本身不会自动加入字符串。

### 先把一行数字变成文字

例如temperature是本次从原集合取出的26.5，可以先用完整普通写法得到温度文字。

```csharp
double temperature = 26.5;
string temperatureText = temperature.ToString(System.Globalization.CultureInfo.InvariantCulture);
```

double和string是C#使用的数值与字符串类型，temperature与temperatureText是示例自定义变量名。
ToString是数值提供的方法，把数字转换成文字；System.Globalization.CultureInfo是.NET提供的格式设置类型，InvariantCulture提供固定的格式规则。
把这个规则传给ToString，使26.5始终写成带小数点的26.5，避免电脑区域设置把它写成26,5而多出一个CSV字段。
这里先使用完整名称，不需要额外引入StringBuilder；少量数据可以先用字符串拼接，逐步把表头和每行文字接起来。
遍历范围应覆盖_temperatures从下标0到Count减1，不能照抄绘图方法中只保留最新三条的起点。

### 文字准备好以后才写文件

_exportPath是教练提供的字段，保存本题CSV的完整路径，ExportPathText显示该路径。
普通Debug运行时，文件目标为WpfBae/bin/Debug/net10.0-windows/Exercises/TASK-031-history-csv/temperature-history.csv，以实际窗口显示为准。
配置也隔离到TASK-031-history-csv/alarm-threshold.json，TASK-030原配置不复制或改写；本题首次缺失配置时仍沿用27并提示。
沿用TASK-030已经实践的Directory.CreateDirectory和File.WriteAllText，先确保父目录存在，再覆盖写入本次全部CSV文字。
WriteAllText成功返回后才显示导出成功和真实路径，异常时显示失败原因并让窗口继续可用。
覆盖写入能让再次导出只保留当前历史，不能每次往旧文件末尾追加整份历史。
开始写文件以前先检查_samplingTimer.IsEnabled与_temperatures.Count，正在采集或空集合都显示原因并return，不能先清空或创建目标文件再拒绝。
导出只读取历史，不调用Clear，不改变阈值，也不保存配置。

### 2026-10-17追问：TODO 3要干什么，Sequence,TemperatureC是什么？

TODO 3先把准备写入文件的全部文字拼出来，TODO 4再把这些文字真正写入磁盘文件，两个步骤分别负责准备内容与保存内容。
Sequence与TemperatureC是教练给本题约定的两个列名，是普通文字，不是C#语法、变量或.NET API；Sequence表示记录序号，TemperatureC表示摄氏温度，其中C表示摄氏。
Sequence,TemperatureC这一整行是表头，与表格上方的列标题作用相同，中间逗号分开两列；后面的1,24表示第1条记录温度24度，2,26.5表示第2条记录温度26.5度。
五条样例的目标文字如下，第一行是列名，后面才是五条数据。

```csv
Sequence,TemperatureC
1,24
2,26.5
3,25
4,28
5,24
```

代码里把表头作为字符串时需要双引号，例如string csvText = "Sequence,TemperatureC" + Environment.NewLine;。
csvText是示例自行命名的字符串变量，双引号里的内容是要写出的文字，Environment.NewLine是.NET提供的换行文字，让后续记录从下一行开始。
循环时原集合下标0对应文件序号1，下标1对应序号2，后面依次类推；每条温度从_temperatures取原值，保持全部历史顺序。
本轮只要求先在TODO 3初始化表头字符串，并将末尾占位ExportStatusText.Text赋值临时替换为该字符串，运行项目添加一条后点导出，应显示表头，尚不创建文件；后续再追加数据行。
本次补充具体含义是对教练前述压缩表达的展开，不记学员错误，不修改学员代码，不把当前可见Count和两项保护写法视为已运行验证，TASK-031与三个DoD保持，Q1待复习、25分类/58条统计不变。
### 开启时证据与掌握边界

独立窗口、配置类、按钮连接、路径与四个TODO由教练准备，采集/报警/表格/图表/配置逻辑复用TASK-030最终作品。
实际骨架检查和原exe启动关闭通过，不代表CSV导出已完成；三个DoD保持未勾选，不增加自主掌握或典型错误。
相关路径、命令和结果见[TASK-031](../tasks/TASK-031.md)。

### 2026-10-17实际审查：26.5为什么导成26,5？

当前组行的Convert.ToString(_temperatures[i])使用运行线程的区域规则，de-DE规则下26.5转成文字26,5，所以本题第二条实际成为2,26,5，读成CSV时多出一列。
这不是原温度变了，而是把数字转成文件文字时选择了本地小数格式。
只需将当前温度转换表达式改为`_temperatures[i].ToString(System.Globalization.CultureInfo.InvariantCulture)`，把.NET固定格式规则传给该double值的ToString方法，使导出温度始终采用小数点。
不需要改变电脑设置、原温度集合或图表，也不需要重写已通过的文件写入。
84项实际检查83通过，默认区域五条完整历史/序号、空历史和采中拒绝并保留文件、覆盖旧行、两种写入失败与恢复、原配置与采集均有自主应用证据，保留这些局部能力；固定小数格式尚待修复，Q1整体仍待复习。
本轮源码未代改，不把示例已讲过视为自主使用过InvariantCulture，未使用的JSON选项不影响必需验收，成功旁路径显示已通过。
TASK-031仍🟡 进行中，DoD1未通过、2/3通过，详见[实际审查](../tasks/TASK-031.md)；25分类/58条、待复习33/已掌握25保持，上方尚未实现为开启历史。
### 2026-10-17最终复查与掌握边界

学员已自主修复温度格式表达式，84项实际窗口检查全通过，默认与de-DE区域均写出26.5，TASK-031三个DoD通关，见[最终结果](../tasks/evidence/TASK-031/review-final/results.json)。
Q1标已掌握，范围限现有骨架内读取完整集合、循环从1编号、表头/换行组行、固定温度小数格式、采集中和空历史保护、目录创建/覆盖写入/失败反馈，以及导出不修改原数据的自主应用。
本轮代码未代改；窗口、路径和按钮连接为教练提供，表格/图表/配置为复用，不扩大为独立设计整个监控界面或文件导出架构。
当前25分类/58条、待复习32/已掌握26，原Q1待复习和格式失败为历史；原C#/WPF其他待复习范围保持。
