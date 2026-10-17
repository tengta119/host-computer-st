# C# CSV历史导出

> [返回问答索引](./README.md)

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