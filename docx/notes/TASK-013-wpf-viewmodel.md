# TASK-013：输入写进数据对象，读取命令再使用它

> 2026-10-13 白话重写。先沿着输入文字到读取命令的路走，之后再看集合与通知。当前实现已经修复空白分支并通关，历史情况在文末区分。

## 1. 读取代码这次搬到了哪里

TASK-012 读取时，窗口方法直接读 DeviceIdTextBox.Text。TASK-013 把当前设备 ID 保存到 ReadingViewModel，读取动作放在 ReadTemperatureCommand。

窗口不再自己写读取 Click。它提供界面，再让输入框和按钮连接同一个数据对象。

```text
用户编辑输入框 → ReadingViewModel.DeviceId
用户点击读取 → ReadTemperatureCommand.Execute
命令读 DeviceId → 新 Reading → Add 历史 → 更新提示
```

ReadingViewModel 和 ReadTemperatureCommand 都是教练自定义类名。它们不是 WPF 自带的设备通信类。

## 2. ViewModel 在本题具体保存什么

ReadingViewModel 可以先理解成“窗口要用的数据和动作的提供者”。它有：

- DeviceId：用户现在输入的 ID。
- StatusText：当前提示。
- Readings：历史集合。
- ReadCommand、ClearCommand：交给两个按钮的命令对象。

MVVM 是这种组织方式的名称。本题界面是 XAML 窗口，Reading 是单条读数数据，ReadingViewModel 提供界面数据和命令。它不是新安装的框架，也不要求此刻再增加服务层或依赖注入。

## 3. 窗口先创建一份数据对象，再让绑定使用它

窗口后台当前只有必要初始化：

```csharp
private ReadingViewModel _viewModel = new ReadingViewModel();

public Task013Window()
{
    InitializeComponent();
    DataContext = _viewModel;
}
```

new 创建一份 ReadingViewModel，_viewModel 保存它。InitializeComponent 创建本窗口控件；DataContext 指定窗口里的默认 Binding 从这份对象找公开属性。

因此按钮、输入、列表和状态不应各自另建一个 ReadingViewModel。读写同一对象，输入才能被命令读到，新增历史才能被列表显示。

## 4. 两个命令为什么能操作同一对象

ReadingViewModel 的构造函数：

```csharp
_readCommand = new ReadTemperatureCommand(this);
_clearCommand = new ClearHistoryCommand(this);
_readings.CollectionChanged += Readings_CollectionChanged;
```

先只看前两句。this 是当前这个 ReadingViewModel，把自己分别传给读取命令和清空命令。

读取命令保存参数：

```csharp
public ReadTemperatureCommand(ReadingViewModel viewModel)
{
    _viewModel = viewModel;
}
```

命令内部的 _viewModel 指向窗口使用的那份对象，没有创建新历史。清空命令也保存同一个对象。

第三句订阅集合变化，稍后解释它怎样让清空按钮重查条件。

## 5. getter 把原集合和原命令交给界面

例如：

```csharp
public ICommand ReadCommand
{
    get
    {
        return _readCommand;
    }
}
```

ReadCommand 是自定义属性名；ICommand 是 .NET 接口。getter 返回构造时创建的原 ReadTemperatureCommand。

ClearCommand 同样返回原清空命令。Readings 同样返回原 ObservableCollection<Reading>，不是每次调用都 new 一个集合。

WPF 按公开属性读取对象；字段 _readCommand 是类内部的保存位置，不作为 XAML 的绑定路径。

## 6. 输入框怎样读写 DeviceId

当前 XAML：

```xml
<TextBox x:Name="DeviceIdTextBox"
         Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

左边 Text 是输入框的文字。右边 DeviceId 是 _viewModel 的公开属性。

TwoWay 表示两条路：

```text
创建窗口时：对象 DeviceId → 输入框 Text
用户编辑时：输入框 Text → 对象 DeviceId
```

初始字段为 TEMP-001，所以输入框先显示 TEMP-001。用户改为 TEMP-002，WPF 将新文字赋给 DeviceId。

这里已经有写回连接，因此读取命令可以只看 DeviceId，不用知道窗口有一个叫 DeviceIdTextBox 的控件。

## 7. DeviceId setter 怎样收到输入文字

当前普通完整写法：

```csharp
private string _deviceId = "TEMP-001";

public string DeviceId
{
    get
    {
        return _deviceId;
    }
    set
    {
        _deviceId = value;
        OnPropertyChanged("DeviceId");
    }
}
```

WPF 把输入框的新文字赋给属性，set 中的 value 就是那段文字。第一句存入字段；getter 后续能读出它。

第二句通知对象属性变化，让其他读取这项属性的绑定也有机会重读。这次只有一个输入框在用，也不能把“用户已经看到自己输入了”当作所有监听方都知道数据已变化。

当前 setter 没有新旧相等检查，重复赋值仍会调用通知。

## 8. UpdateSourceTrigger 只说明何时写回

UpdateSourceTrigger=PropertyChanged 可以读成：“输入框 Text 一变，就把文字写回 DeviceId。”

若不显式这样设，TextBox.Text 的默认写回时机通常是失去焦点。当前骨架明确采用文字变化时写回，让命令取得最新已写入对象的文字。

这项配置没有发出你自己定义的 PropertyChanged 事件。它控制的是“输入框到数据对象”的时机。

对象 setter 中的 OnPropertyChanged 则通知“数据对象到读取它的绑定”。两个名字相似，负责的方向不同。

## 9. 按钮怎样拿到 ReadCommand

当前按钮：

```xml
<Button Content="读取温度"
        Command="{Binding ReadCommand, Mode=OneWay}" />
```

WPF 从 _viewModel.ReadCommand getter 取得命令，放进 Button.Command。ReadTemperatureCommand 的 CanExecute 当前固定返回 true，所以读取按钮始终可用。

空白输入也能点击读取；之后 Execute 会给提示并提前结束。别把“按钮可用”理解成“输入已经合法”。

当前没有读取 Click，也没有通过 Binding 自动执行命令。按钮正常执行动作时，WPF 才调用命令 Execute。

## 10. Execute 的第一步：读对象里的当前 ID

当前代码：

```csharp
string id = _viewModel.DeviceId;
_viewModel.StatusText = "命令取得的设备 ID：" + id;
```

id 是这次执行的局部变量，取得 DeviceId 的当前字符串。第二句是任务练习留下的中间提示，用来观察输入连接。

方法之后还会设置“不合法”或“读取成功”，因此这行中间文字不一定作为最终屏幕文字稳定显示。不要只截取一个赋值就判断整个方法最后显示什么。

## 11. Trim 的局部 id 与输入属性分开看

紧接着：

```csharp
id = id.Trim();
```

例如对象 DeviceId 是两侧带空格的 `  TEMP-003  `，这一句让局部 id 得到 TEMP-003。

原代码没有 `_viewModel.DeviceId = id`，所以它没有把修剪后的字符串回写输入属性。新历史使用局部 id，输入框仍可能保留原空格。

id 和 DeviceId 起初文字相同，之后也不是一个会自动同步的存储位置。Trim 返回字符串，不会在所有引用位置原地改输入。

## 12. 空白分支必须 return，提示不会代替结束方法

当前已修复代码：

```csharp
if (id.Length == 0)
{
    _viewModel.StatusText = "该 Id 不合法";
    return;
}
```

空字符串与纯空格经过 Trim 后长度为 0。设置提示后，return 结束本次 Execute，因此不能再创建/追加记录。

历史中曾漏掉 return：无效分支结束后仍往下执行 Add，又把提示设成“读取成功”。学员后来自主补回 return 并复查通过。

if 的花括号结束，只是结束那个分支；return 才结束整个方法。这点在当前真实错误中有证据，不将它扩大成学员不懂所有条件判断。

## 13. 有效分支创建新记录并追加原集合

当前有效路径：

```csharp
_viewModel.Readings.Add(new Reading
{
    DeviceId = id,
    Value = 26.5,
    Unit = "°C"
});

_viewModel.StatusText = "读取成功";
```

new Reading 创建这次记录，对象初始化器填入当前局部 id 和模拟值。Reading 的 get/init 属性允许初始化时赋值，之后普通外部赋值受限制。

Readings getter 返回原集合，Add 在末尾追加，新记录不会覆盖旧条目。重复 ID 仍是新的一次读取，历史不会去重。

ObservableCollection 发新增通知，ListBox 增加行；StatusText setter 保存并通知，状态 TextBlock 更新。命令没有直接访问 TextBox/TextBlock/ListBox。

## 14. 当前输入改变，为什么旧历史不变

假设读过 TEMP-001，再把输入改为 TEMP-EDIT-ONLY，还没点击读取：

```text
_viewModel.DeviceId：TEMP-EDIT-ONLY
历史第一条 Reading.DeviceId：TEMP-001
```

两个 DeviceId 属于不同对象。输入绑定只写 ViewModel，原代码没有回头修改那条 Reading。

点击读取后再创建一个新 Reading 保存新的 ID，这才是新增历史。已保存读数与用户当前输入分别存在，不能把一条历史改造成“始终显示当前输入”。

## 15. 提示和历史各自从公开属性取得

当前窗口有：

```xml
<ListBox ItemsSource="{Binding Readings, Mode=OneWay}" Height="170" />
<TextBlock Text="{Binding StatusText, Mode=OneWay}" />
```

列表从 _viewModel.Readings 取得集合；状态控件从 _viewModel.StatusText 取得文字。

StatusText 使用完整 get/set、字段保存和 OnPropertyChanged。通知工具当前使用普通非空判断：

```csharp
PropertyChangedEventHandler? handler = PropertyChanged;
if (handler != null)
{
    handler(this, new PropertyChangedEventArgs(propertyName));
}
```

this 是 ViewModel，参数装属性名，WPF 再读对应 getter。类型/事件/?.Invoke 的基础见 [TASK-010](./TASK-010-wpf-binding.md)，这里沿用工具而不再引入一套通知方式。

## 16. 清空命令和集合变化继续连在一起

清空命令条件：

```csharp
return _viewModel.Readings.Count > 0;
```

Execute 先检查条件，再 Clear 原集合，最后提示“已清空”。

ViewModel 订阅集合后，在 Readings_CollectionChanged 中调用清空命令 NotifyCanExecuteChanged。Add 或 Clear 时，列表更新，清空按钮也重新判断 Count。

```text
Add → 列表增加 → 清空条件重查为 true
Clear → 列表为空 → 清空条件重查为 false
```

PropertyChanged 管对象属性文字；CollectionChanged 管集合项目；CanExecuteChanged 请求条件重查。输入框写回时机是另一项设置，不把这几项都简称为一个“通知”。

## 17. 一次读取最后应该看到什么

| 操作 | 历史 | 最终提示 |
| :--- | :--- | :--- |
| 初始 | 0 条，清空禁用 | ViewModel 的初始提示 |
| TEMP-001 连读两次 | 两条 TEMP-001 | 读取成功 |
| TEMP-002 再读 | 前两条保留，共三条 | 读取成功 |
| 只改输入 | 旧三条不变 | 原提示保持 |
| 空字符串/纯空格后读 | 旧三条不变 | 该 Id 不合法 |
| 点击清空 | 0 条，清空禁用 | 已清空 |
| 两侧空格 TEMP-003 再读 | 一条 TEMP-003，清空启用 | 读取成功 |

如果输入没进入命令，先核对 TextBox 的 DeviceId/TwoWay/写回时机和数据对象是否相同；如果空白仍新增，先看 return；如果列表不更新，先看是否 Add 原集合。

## 文末查阅：文件、观察和历史范围

实际文件：[ViewModel](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadingViewModel.cs)、[读取命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs)、[清空命令](../../WpfBae/Exercises/TASK-013-reading-viewmodel/ClearHistoryCommand.cs)、[窗口 XAML](../../WpfBae/Exercises/TASK-013-reading-viewmodel/Task013Window.xaml)、[窗口后台](../../WpfBae/Exercises/TASK-013-reading-viewmodel/Task013Window.xaml.cs)、[任务卡](../tasks/TASK-013.md)。

最小复习观察：读一条，再只改输入而不点击，确认旧行不变。当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-017，默认运行命令不自动选择 TASK-013，复跑旧题需明确临时入口并恢复。本轮未改入口或运行。

任务卡记录 2026-10-12 自主修复 return 后已通关，构建/原应用启动、10 个实际窗口场景、同对象/集合/命令与原绑定通过。首次空白错误及修复见 [错题本](../mistakes.md)。原 evidence/TASK-013 目录当前不存在，历史结果据卡片/错题记录，不声称本轮读到原 JSON。

掌握范围限在提供连接上的读取逻辑：当前 ID、Trim、空白返回、Reading/Add 和提示。ViewModel 类、完整属性、命令构造、TwoWay 与通知工具由骨架提供，ViewModel 问答整体机制仍待复习，不推定完整 MVVM 设计能力。

答疑见 [ViewModel 问答](../questions/wpf-mvvm.md)；输入方向/写回机制可查 [Microsoft 数据绑定](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)。下一篇把 [历史区域放进 UserControl](./TASK-014-wpf-usercontrol.md)。
