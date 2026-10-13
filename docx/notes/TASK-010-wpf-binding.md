# TASK-010：改数据对象里的文字，让窗口跟着更新

> 2026-10-13 白话重写。先沿着“文字保存在哪里、窗口去哪里读、变化后怎么通知”阅读。当前代码与说明示例分别标注，运行和历史证据在文末。

## 1. 这次要改变哪件事

TASK-009 的按钮直接写 `ResultTextBlock.Text`。这次先把结果保存到一个普通 C# 对象里，结果控件从这个对象读文字。

操作员看到的过程仍是：输入 ID → 点击读取 → 显示模拟温度。但代码里改成：

```csharp
_state.ResultText = reading.ToString();
_state.StatusText = "读取成功";
```

_state 是窗口持有的数据对象。ResultText 和 StatusText 是它的两个公开属性。这里改的是数据，不是两个 TextBlock 的 Text。

窗口怎样知道这些数据？先从 _state 的创建开始。

## 2. ReadingDisplayState 是谁写的，保存什么

ReadingDisplayState 是教练为这次任务写的类。它保存“结果文字”和“状态文字”，不保存 TextBlock 控件。

窗口中有：

```csharp
private readonly ReadingDisplayState _state = new();
```

先展开短写法：

```csharp
private readonly ReadingDisplayState _state =
    new ReadingDisplayState();
```

new 创建一个 ReadingDisplayState 对象，_state 字段记住这个对象，按钮和窗口后续都用它。短写 new() 可以从左侧类型知道要创建什么。

readonly 限制字段在初始化或构造后换指向另一个对象；它不禁止修改当前对象的 ResultText。ReadingDisplayState 类上的 sealed 表示这个类不允许再被继承，并不禁止修改属性。

Reading 是另一种自定义类，保存单次读数的 ID/Value/Unit；ReadingDisplayState 保存整个结果显示用的文字。不要把两个名字都理解成温度控件。

## 3. 字段存值，公开属性让外部读写

先看 ResultText 的完整普通写法。原代码 getter 使用箭头，下面只把它展开：

```csharp
private string _resultText = "尚未读取";

public string ResultText
{
    get
    {
        return _resultText;
    }
    set
    {
        _resultText = value;
        OnPropertyChanged("ResultText");
    }
}
```

_resultText 是 private 字段，真正保存文字。ResultText 是 public 属性，按钮代码和 WPF 可以通过它读写。

读取 `_state.ResultText`，执行 get，返回当前字段值。

执行 `_state.ResultText = "读取成功"`，进入 set；右边“读取成功”成为 value。第一句保存文字，第二句通知外部这项文字发生了变化。先把保存看明白，下一节再解释通知。

## 4. value 不用自己声明，它来自赋值右边

假设执行：

```csharp
_state.ResultText = "TEMP-001 26.5 °C";
```

此时 setter 中：

```csharp
_resultText = value;
```

相当于把那段文字存入 _resultText。value 是 C# 在 setter 中提供的名字，不是自定义成员，也不是 Reading.Value。

用普通方法作说明，可以想成：

```csharp
public void SetResultText(string newText)
{
    _resultText = newText;
}
```

这只是帮助理解参数从哪里来，项目实际使用属性。只有空的 set 代码块不会自动把文字存入字段。

原 getter `get => _resultText;` 就是 `get { return _resultText; }` 的简写。先读完整写法，再把箭头看作省去代码块。

## 5. DataContext：告诉窗口里的绑定去哪个对象找文字

窗口构造函数已有：

```csharp
InitializeComponent();
DataContext = _state;
```

InitializeComponent 创建配套 XAML 的控件。随后给窗口的 DataContext 属性放入 _state。

DataContext 是 WPF 提供的属性。这里可以读成：“窗口里面需要读数据的 Binding，默认从这个 _state 对象找。”

它没有创建新的 ReadingDisplayState，也没有把所有文字立刻拼成一句。_state 已经存在，这句只是交代数据来源。

当前两个显示控件没有另设自己的数据来源，所以沿父级取得窗口设置的同一个 _state。按钮也修改这个对象，窗口才看得见那些变化。

## 6. Binding ResultText：具体从哪读，放到哪

结果控件当前写：

```xml
<TextBlock x:Name="ResultTextBlock"
           Text="{Binding ResultText, Mode=OneWay}"
           TextWrapping="Wrap" />
```

逐侧读：

左边 Text 属于屏幕上的 ResultTextBlock；右边 Binding ResultText 表示读取 _state.ResultText；读到的文字放进这个 TextBlock 的 Text。

完整路径写法是：

```xml
Text="{Binding Path=ResultText, Mode=OneWay}"
```

省去 Path= 的当前写法含义相同。ResultText 是 ReadingDisplayState 的公开属性名；ResultTextBlock 是控件名，这两个名字负责不同事情。

状态控件同理：

```xml
Text="{Binding StatusText, Mode=OneWay}"
```

它应该读 _state.StatusText。若误写 ResultText，状态处也会出现结果文字。当前源码已修正这项历史问题。

## 7. OneWay 表示数据到显示，不是只显示一次

Mode=OneWay 可以读成：“从 _state 的属性，把文字送到 TextBlock。”

第一次建立连接时，结果显示“尚未读取”，状态显示“请输入设备 ID，然后读取温度。”，它们来自数据对象字段的初值。

以后只要这个对象正确通知属性变化，WPF 会再读新文字更新显示。OneWay 规定方向；它不会让普通 C# 字段自己自动发通知。

本题输入框尚未绑定到数据对象。按钮仍从 `DeviceIdTextBox.Text` 取得当前输入，结果/状态才用 Binding。

## 8. 数据已经改了，WPF 怎样得知要重读

当前数据类声明：

```csharp
public sealed class ReadingDisplayState : INotifyPropertyChanged
```

INotifyPropertyChanged 是 .NET 提供的接口，名称来自 System.ComponentModel。可以先把它理解成“这个类提供属性变化通知”。

它要求类提供 PropertyChanged 事件。WPF 建立绑定后会监听相应通知。通知中说 ResultText 变了，WPF 就再次读 ResultText 的 getter。

只写接口名称不够。当前 setter 还实际调用了：

```csharp
OnPropertyChanged("ResultText");
```

OnPropertyChanged 是教练自己取的辅助方法名，用来发通知；它不是接口强制要求的固定方法名。

## 9. PropertyChanged 事件与通知参数分别是什么

当前事件：

```csharp
public event PropertyChangedEventHandler? PropertyChanged;
```

PropertyChangedEventHandler 是 .NET 提供的委托类型。先把委托理解成：规定监听方法应接收哪些参数，方便程序保存并调用这些方法。

event 把 PropertyChanged 声明为事件，让外部通过订阅得到通知。后面的问号表示允许当前没有监听者，值可以是 null。

通知带的参数来自：

```csharp
new PropertyChangedEventArgs("ResultText")
```

PropertyChangedEventArgs 是 .NET 提供的类，不是教练自定义读数类。它记录发生变化的属性名 ResultText，名字在 PropertyName 中。

这个参数不装新温度文字。WPF 得到属性名后，再读 _state.ResultText，才得到新的文字。

## 10. 先看普通发通知写法，再读 ?.Invoke

原辅助方法的核心一行：

```csharp
PropertyChanged?.Invoke(
    this,
    new PropertyChangedEventArgs(propertyName));
```

可以先按下面的展开写法理解：

```csharp
PropertyChangedEventHandler? handler = PropertyChanged;

if (handler != null)
{
    handler(this, new PropertyChangedEventArgs(propertyName));
}
```

handler 保存当前监听方法；不为 null 时才调用。this 是正在发通知的这个 ReadingDisplayState 对象，第二个参数告诉监听方哪项属性变了。

?.Invoke 将“有监听者才调用”缩成一行。声明事件和写这个辅助方法都不会自己通知；setter 调用辅助方法时才发出通知。

这里的 using System.ComponentModel 允许用这些框架类型的短名称，不是在创建对象，也不是在注册 WPF。

## 11. 属性名字符串与 nameof 怎样对应

当前作品写：

```csharp
OnPropertyChanged("ResultText");
```

这是合法的。也可写：

```csharp
OnPropertyChanged(nameof(ResultText));
```

nameof(ResultText) 在编译时得到属性名字符串“ResultText”，便于重命名检查。它不会读取 _resultText 的内容，不是“把当前温度传给通知”。

本题不要求为了换成 nameof 而重写已正确运行的字符串版本；关键是通知名对应实际改变的属性。

## 12. 为什么必须先保存文字，再通知

当前正确顺序：

```csharp
_resultText = value;
OnPropertyChanged("ResultText");
```

WPF 收到通知会重读 getter。先保存，它读到新值；若先通知再保存，当场重读可能仍拿到旧值。

两个属性各自通知自己的名字。改 ResultText 不会自动告诉 WPF StatusText 也变了。

当前 setter 没有“新旧相同就跳过”的判断。即使又赋同样文字，也会调用通知。这是作品的实际行为，不应凭空说已经去重。

## 13. 按钮中的有效输入分支，怎样触发显示

当前有效分支：

```csharp
Reading reading = new Reading
{
    DeviceId = id,
    Value = 26.5,
    Unit = "°C"
};

_state.ResultText = reading.ToString();
_state.StatusText = "读取成功";
```

第一步创建单次 Reading。其 ToString 为“ID 数值 单位”，原实现还留一个尾部空格。

第二步把结果字符串赋给 _state.ResultText，setter 保存并通知，结果控件重读。第三步赋 StatusText，同样更新提示。

```text
点击 → 创建 Reading → 结果字符串
赋给同一个 _state → setter 保存 → 通知属性名
WPF 重读 getter → TextBlock 显示新文字
```

如果按钮另建一个 ReadingDisplayState 并只改那个新对象，窗口仍连接旧 _state，就不会看到这次修改。先确认对象相同，再排查名字和通知。

## 14. 无效输入分支仍然清结果，提示保存到数据对象

当前分支：

```csharp
string id = DeviceIdTextBox.Text.Trim();

if (string.IsNullOrEmpty(id))
{
    _state.StatusText = "Id 为空或者异常";
    _state.ResultText = "";
    return;
}
```

Trim 处理两端空白；IsNullOrEmpty 判断 null 或空字符串。当前 Text 经过 Trim 后，纯空格也变为空。

这里先改提示再清结果，所以通知顺序与有效分支不同。两项都会发出通知；最终界面是结果为空、提示输入不合法。

return 阻止后面的成功逻辑。这里没有直接改 TextBlock.Text，输入框文字也没有被 Trim 自动回写。

## 15. 已绑定的显示控件，继续让数据对象驱动

反例说明：

```csharp
ResultTextBlock.Text = "绕过数据对象的文字";
```

直接给已绑定目标属性赋值，可能替换原绑定，之后数据对象再发通知也不能按原连接更新。这次任务应改 _state.ResultText。

历史审查工具曾用保留属性来源的方式调整控件值；那是审查操作，不是要求业务代码用控件赋值代替绑定。

界面不更新时，按顺序看：DataContext 是否是 _state，Path 是否拼对，setter 是否保存值和通知，按钮是否改同一个对象，目标 Binding 是否还在。

## 16. 编辑器提示、预览与运行要分开看

Rider 的灰色 Path= 或 (???) 可以是编辑器内嵌提示，不一定出现在 XAML 文件中。先看真实源码，再判断是什么问题。

设计器可能没有运行窗口构造过程，预览空白不直接证明运行绑定失败。反过来，构建成功也不证明运行时的属性名一定正确。

本题曾把状态控件接到 ResultText；属性存在，所以它能显示，只是显示错了地方。检查要看“对象、属性和屏幕位置”是否对应，而不只看有没有文字。

## 文末查阅：文件、观察和历史范围

实际文件：[数据类](../../WpfBae/Exercises/TASK-010-reading-binding/ReadingDisplayState.cs)、[窗口 XAML](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml)、[窗口 C# 与 Reading](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)、[任务卡](../tasks/TASK-010.md)。

复习可先只改数据类 _resultText 初值，运行本任务，观察初始结果文字是否跟随，再恢复；这个观察不需要同时改通知或按钮。当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-017，默认运行命令现在启动 TASK-017；复跑旧题须明确临时入口并恢复。本轮没有修改入口或运行。

任务卡记录 2026-10-09 已通关：原应用启动、同一对象、两项 OneWay、五种输入/恢复场景及两项通知通过；原 TASK-010 的结果 JSON 当前缺失，结论引用任务卡。三个任务源码曾在整理时与卡片保留哈希核对一致，不把当时 App.xaml 的哈希强加给现入口。

学员自主应用的是 DataContext、两个 Binding、setter 中的正确通知调用和按钮修改属性。完整类、通知发布工具等由骨架提供，属性/事件机制继续按原记录复习，阅读不新增掌握证据。

更详细的原答疑见 [绑定问答](../questions/wpf-binding.md)、[C# 属性问答](../questions/csharp-properties.md)。框架机制可查 [Microsoft 数据绑定](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)和 [INotifyPropertyChanged](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=net-10.0)。下一篇学习 [保留多条历史](./TASK-011-wpf-reading-history.md)。
