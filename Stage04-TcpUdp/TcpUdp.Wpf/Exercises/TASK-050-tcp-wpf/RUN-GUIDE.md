# TASK-050：从控制台命令走到WPF监控窗口

2026-10-20本题已按静态源码口径✅ 已通关，下文保留开启时的操作讲解；四组核心连接已完成，教练仅收尾状态和旧占位文案，本轮未构建、运行或测试，不将初始准备检查视为当前完整窗口联调证据。

这次仍然和TASK-049的同一个模拟器通信，GET_TEMP、START_DEVICE、STOP_DEVICE和GET_STATUS的含义没有变化。
变化是由窗口按钮发起操作：收到完整回复后更新窗口，温度进入历史、曲线和报警，开始采集后再按约每秒一次查询温度。
先只接通一次连接，不需要把四组TODO同时写完。

## 首个动作：让连接按钮使用已有通信辅助

打开[Task050Window.xaml.cs](./Task050Window.xaml.cs)，找到ConnectCoreAsync，先将其中的占位await Task.CompletedTask替换为下面这一行。

```csharp
await _connection.ConnectAsync(host, port, token);
```

`_connection`是教练命名的字段，窗口创建时通过`new Task050TcpConnection()`创建一个通信辅助对象，并把它保存在这个字段里。
`Task050TcpConnection`是本题自定义的普通C#类，不是.NET内置API；它已经把TASK-049里创建TcpClient、等待连接和取得NetworkStream的代码整理成ConnectAsync方法，真实代码在[Task050TcpConnection.cs](./Task050TcpConnection.cs)。
因此这行代码的具体效果是：让当前窗口保存的通信对象，连接输入框指定的模拟器地址和端口。
`host`、`port`和`token`都是当前ConnectCoreAsync收到的参数，前两个来自窗口输入，`token`用于让断开或关窗能取消本次等待。
前面的`await`是C#语法，表示等这次连接完成后再继续；并不表示创建一条新线程。

你点“连接”后，WPF会调用教练登记的ConnectClick，它检查输入、安排等待保护，再调用这里的ConnectCoreAsync。
成功后的状态文字、日志和按钮刷新已由外层提供，连接失败也已有反馈，不需要你重复补这些收尾。
保留原方法签名和外围代码，只替换占位；没有替换时，窗口会如实提示TODO 1尚未接通，不会假装已经连接。

## 运行方式与这一小步的预期

两个PowerShell终端都从`D:\host-computer-st`运行。
先在终端A启动已经提供的TASK-049模拟器，并保持它运行。

```powershell
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj -- --simulator
```

如果控制台项目还没有构建输出，先退出自己之前运行的旧模拟器，再执行`dotnet build .\Stage04-TcpUdp\TcpUdp\TcpUdp.csproj --nologo`，然后重新运行上面的命令。
每次修改WPF源码后，先关闭自己运行的本题窗口，在终端B构建并启动新的WPF项目，不会改动旧串口窗口的入口。

```powershell
dotnet build .\Stage04-TcpUdp\TcpUdp.Wpf\TcpUdp.Wpf.csproj --nologo
dotnet run --no-build --project .\Stage04-TcpUdp\TcpUdp.Wpf\TcpUdp.Wpf.csproj
```

也可以打开[阶段解决方案](../../../TcpUdp.slnx)，选择TcpUdp.Wpf作为启动项目。
窗口默认地址为127.0.0.1、端口5000，点击“连接”后，窗口应显示已连接，模拟器终端应显示接受连接。
只有TODO 1完成时，点“读温度”仍会提示TODO 2a未接通请求，这是下一步尚未实现的正常现象。
点击“断开”再连接，应能继续使用；端口5000被其他程序占用时，可把模拟器参数改为`--simulator 5001`，并把窗口端口同步改为5001。
关闭窗口结束本题界面，模拟器终端使用Ctrl+C停止，保留其他无关程序。

## TODO 2：按钮的命令怎样得到完整回复

“读温度”按钮已经登记到ReadClick，它把GET_TEMP交给RunCommandAsync；启动、停止和查询状态按钮也都进入同一个RunCommandAsync。
这个自定义辅助先标记正在等待，让其他请求暂时不能进入，再调用你要补的SendCoreAsync。
因此TODO 2a只需调用通信对象现成的RequestAsync，等待它返回完整回复，再把回复返回给外层；继续使用本方法已有的command和token参数，替换占位await和return null。
RequestAsync内部已经完成UTF-8编码、附加一个LF、发送、按实际数量读取和复用TASK-049的换行解析，不需要在窗口里再创建TcpClient或再写一次字节循环。

当前窗口中，`string?`表示返回的字符串也允许为null，骨架用null表明还没有执行请求，不是设备回复了一条空消息。
正常实现应把RequestAsync得到的那条字符串传回来，例如GET_TEMP得到`35.6`，GET_STATUS得到`RUNNING`或`STOPPED`。
Task.CompletedTask仍只是占位，不会代替发送和接收，写好真实异步调用后移除即可。

对端回复回来后，已有ApplyReply先核对它是否与本次命令匹配，然后才进入显示处理。
TODO 2b位于DisplayDeviceState，参数state此时已经是确认后的RUNNING或STOPPED，你要把它分别显示为“运行”或“停止”，设置真实控件`DeviceStatusText.Text`，并把`OperationText.Text`改为确认后的操作提示。
DeviceStatusText和OperationText是XAML中教练命名的两个TextBlock控件，Text是WPF提供的文字属性；修改这里的Text会改变窗口当前显示的文字。
不能在刚点“启动远端设备”时就写成设备已启动，应该等OK到达后再显示确认，随后用“查询设备状态”核对模拟器保存的状态。

```text
点击启动远端设备 → START_DEVICE → 等OK → 显示已确认运行。
点击查询设备状态 → GET_STATUS → 等RUNNING或STOPPED → 显示查询结果。
```

## TODO 3：一条有效温度怎样带动表格、曲线和报警

GET_TEMP的完整回复会交给ApplyTemperature，教练已经提供小数解析和有限数值检查，局部变量temperature就是解析后的double温度，例如35.6。
接下来把这个数保存成一条记录，加入当前窗口的历史集合。

`Task050Reading`是本题新建的普通C#数据类，只有ReceivedAt和Temperature两个属性，真实定义见[Task050Reading.cs](./Task050Reading.cs)。
创建一个Task050Reading对象，把ReceivedAt设为DateTime.Now，把Temperature设为当前temperature，再把这个新对象加入`_history`，即可完成TODO 3的核心连接。
DateTime是.NET提供的时间类型，DateTime.Now取得当前时间，ReceivedAt是教练为这条记录自行命名的接收时间属性，不是WPF属性。
`_history`保存的是同一个ObservableCollection<Task050Reading>对象，窗口构造时已让HistoryGrid.ItemsSource指向它，表格按每条记录的ReceivedAt和Temperature读取两列。
教练也已登记集合变化后的RefreshPresentation，因此加入一条记录后，会更新条数、最新温度、最新最多30条折线与报警，不需要你再分别手动改四个区域。
完成后把当前“历史追加待完成”的占位提示改为实际成功提示，避免结果已经显示而说明还停留在TODO状态。

阈值默认35℃，模拟器固定返回35.6，所以收到有效温度后应触发报警；把阈值应用为40后应显示未超过阈值，不需要重新发送温度才刷新。
阈值输入、立即复算、清空、折线绘制和单点显示均由教练复用旧实践思路提供，不重复考察你已经做过的坐标和布局连接。
没有完整回复或回复不是合法温度时，不向_history加入记录；一次Read接到多少批字节，也不等于应加入多少条温度。

## TODO 4：采集启停控制本地查询，不是远端设备电源

“开始采集”会触发StartCollectionClick，你要在该方法的既有检查之后接好四个动作：将_collecting设为true、递增_collectionGeneration、调用_pollingTimer.Start，再刷新按钮和操作提示。
`_collecting`是教练命名的bool字段，表示窗口目前是否希望持续查询温度；它不会自己让设备启动，也不会自己启动定时器。
`_pollingTimer`是窗口创建并一直保留的同一个DispatcherTimer对象，DispatcherTimer是WPF提供的类，教练已设置Interval为一秒，并登记PollingTick作为到时处理方法。
Start启动这个定时器，PollingTick到时才会沿同一个RunCommandAsync查询GET_TEMP，Stop停止后续通知；时间间隔不是设备温度，也不保证系统忙时严格每秒一次。[Microsoft Learn：DispatcherTimer](https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?view=windowsdesktop-10.0)

`_collectionGeneration`是教练自行保存的一轮采集编号，每次开始或停止时改变它，已有保护代码据此识别“回复回来时，这轮采集是否已经结束”。
例如停止前的GET_TEMP尚未回复，停止后才收到35.6，就应该结束这次读取但不再把它加入已停止轮次的历史，不能让旧回复混到重新开始的一轮。
停止按钮已经连接StopCollectionSafely，它负责_collecting变为false、编号递增和定时器Stop，断开与关窗也会使用同一安全收尾，不需要你重复写三份停止代码。
已有_busy字段会在一次请求未结束时阻止其他按钮或定时查询再次发送；这套请求保护由教练提供，不把定时器的每次通知都强行变成一个重叠请求。

完成后点击“开始采集”，应约每秒追加一条有效温度，点击“停止采集”后条数停止增长，已有历史保留，再次开始可继续追加。
“停止远端设备”只发送STOP_DEVICE，本教学协议停止后仍可读取温度；它与停止本地采集、断开TCP连接是三个不同动作。

## 这里为什么可以在await后更新控件

本题的按钮事件和DispatcherTimer.Tick都从窗口线程进入，RunCommandAsync使用普通await等待通信，返回后继续沿窗口线程更新控件和_history，通信辅助自身不调用任何窗口控件。
因此这里沿着“按钮事件 → 等待完整字符串 → 更新窗口”的路径学习，不照搬串口DataReceived回调，也不需要给每行界面赋值额外包一层Task.Run。
这不是说所有await之后都一定在窗口线程；以后如果从后台任务或后台事件进入，再按那个调用位置安排Dispatcher更新。[Microsoft Learn：WPF线程模型](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/threading-model)
按钮的ConnectClick、ReadClick等是WPF事件处理方法，使用async void以匹配事件入口；可以被其他代码等待的ConnectCoreAsync、SendCoreAsync等仍返回Task，不把普通异步辅助方法一律改成async void。

## 运行结果与本次骨架检查分开看

当前四组TODO尚未完成，初始窗口没有真实历史，连接和发送占位不会虚报成功。
教练准备验证直接调用了通信辅助与模拟器，并给历史注入三条布局样例，所以截图中的温度和日志只用于检查显示，不表示你的按钮已经完成或任务已通过。
实际窗口Show、默认/最小尺寸、日志上限、阈值和关闭取消等准备结果见[验证记录](./Verification/startup-2026-10-20.json)，本题三个DoD仍未勾选。
完成实现后说“审查TASK-050”，继续按已约定的源码审查偏好记录结果，不要求额外抄录日志，实际运行和教练辅助范围仍分别注明。
