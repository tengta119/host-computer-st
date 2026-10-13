# TASK-009：把输入框里的设备 ID 显示成一条温度读数

> 2026-10-13 白话重写。阅读时先看正文中的代码与现象，文末再看文件、运行入口和历史记录。代码来自当前作品；注明“说明示例”的片段只用于解释。

## 1. 先看这个窗口要做什么

窗口里有一个设备 ID 输入框、一个“读取温度”按钮，以及两处文字：一处显示读数，一处显示操作提示。

你输入 TEMP-001，点击按钮，程序就显示 `TEMP-001 °C 26.5` 和“读取成功”。改成 TEMP-002 再点，显示的 ID 也应该换成 TEMP-002。温度 26.5 是这次练习写在代码里的模拟值。

这一篇先沿着一次点击往下看：输入框保存文字，按钮叫一个 C# 方法来处理，方法检查输入，再把结果放进显示控件。

## 2. XAML 中的标签，对应屏幕上的控件

当前输入框：

```xml
<TextBox x:Name="DeviceIdTextBox" Text="TEMP-001" Margin="0,8,0,12" />
```

TextBox 是 WPF 提供的输入框类。写这个标签，就是要求 WPF 创建一个输入框。`Text="TEMP-001"` 是它启动时的文字。

DeviceIdTextBox 是我们给这个具体输入框取的名字。C# 里写 `DeviceIdTextBox.Text`，就能取得用户此刻输入的文字。这个名字由 `x:Name` 指定，不是 WPF 固定要求的名称。

TextBlock 是另一种控件，用来显示文字：

```xml
<TextBlock x:Name="ResultTextBlock"
           Text="尚未读取"
           TextWrapping="Wrap" />
```

ResultTextBlock 是结果显示控件的名字。TextWrapping=Wrap 表示宽度不够时文字可以换行；它不会自动缩小字号，也不保证任意长文本都适合当前窗口。

Button 是按钮类。它的 Content 决定按钮上写什么，输入框和 TextBlock 用的则是 Text 属性。

## 3. Window 和 StackPanel 各管哪一层

Window 是整个窗口。当前设置：

```xml
Title="TASK-009：设备读数练习"
Height="360" Width="560"
MinHeight="300" MinWidth="360"
```

Title 是标题栏文字；Width/Height 是初始大小；MinWidth/MinHeight 限制用户缩小窗口的下限。这里使用 WPF 的布局单位，不应直接当作任意屏幕上的物理像素。

Window 内放一个 StackPanel：

```xml
<StackPanel Margin="20">
    <TextBlock Text="设备温度监控" />
    <TextBox ... />
    <Button ... />
    <TextBlock Text="读取结果" />
    <TextBlock ... />
    <TextBlock ... />
</StackPanel>
```

上方省略号仅表示省去属性，不能原样粘贴运行。这个 StackPanel 默认把直接放在里面的控件从上到下排。输入框写在按钮前面，运行时就在按钮上面。

Window 直接装着一个 StackPanel；StackPanel 再装多个控件。标签的嵌套告诉 WPF 哪个控件放在谁里面。

## 4. 留白要先看属于哪个控件

输入框的 `Margin="0,8,0,12"` 按左、上、右、下读：左 0、上 8、右 0、下 12。最后的 12 留在输入框下面，所以会隔开它与按钮。

外层 StackPanel 的 Margin=20 在四边都留 20，让整块内容与窗口边缘分开。

按钮的 `Padding="12,6"` 是内部留白：文字左右各 12，上下各 6。Margin 管控件外面的空间，Padding 管里面的空间。

按钮还有 `HorizontalAlignment="Left"`：让按钮本身在父布局给它的横向空间内靠左。它不等于按钮文字的文本对齐。

这些属性分属不同对象。调整输入框的下 Margin，应观察“输入框与按钮之间”；调整按钮 Padding，应观察“文字与按钮边缘之间”。

## 5. Grid 可以分行列，当前作品用的是 StackPanel

如果以后希望标签和输入框左右排、几块区域按行列分配空间，可以使用 WPF 提供的 Grid。

说明示例：

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <TextBlock Grid.Row="0" Text="设备温度监控" />
    <TextBlock Grid.Row="1" Text="这里放结果" />
</Grid>
```

第一行 Auto 表示按内容所需高度安排；第二行的星号表示分配剩余空间。Grid.Row=0 指第一行，编号从 0 开始。

这是帮助区分两种布局的例子。TASK-009 当前源码没有改成 Grid，历史通过也不能证明你已经独立应用过 Grid。

## 6. 点按钮后，WPF 去调用哪个方法

当前按钮写着：

```xml
<Button Content="读取温度"
        Click="ReadButton_Click"
        HorizontalAlignment="Left"
        Padding="12,6" />
```

Click 是 WPF 按钮提供的点击事件。ReadButton_Click 是我们写在窗口 C# 文件里的方法名。这里的意思是：“按钮发生点击时，调用这个窗口的 ReadButton_Click 方法。”

对应方法的开头：

```csharp
private void ReadButton_Click(object sender, RoutedEventArgs e)
{
    // 读取输入并更新显示
}
```

sender 表示发出事件的对象；RoutedEventArgs 是 WPF 提供的事件参数类，e 是这次传入的参数。这次方法不需要使用它们，但方法签名仍要与事件要求相符。

XAML 编译生成的连接代码会完成事件订阅，可以联系之前的 `+=` 订阅理解。你不需要同时再写一次订阅，否则一次点击可能重复处理。当前练习只需要知道这两个同名位置怎样对应。

## 7. XAML 和 .xaml.cs 为什么能操作同一个窗口

XAML 开头的：

```xml
x:Class="WpfBae.Exercises.Task009.Task009Window"
```

说明这份界面属于 Task009Window 类。C# 文件中也有相同命名空间和类名：

```csharp
public partial class Task009Window : Window
{
    public Task009Window()
    {
        InitializeComponent();
    }
}
```

`: Window` 表示这个自定义类继承 WPF 的 Window，能作为窗口使用。partial 是 C# 语法，允许同一个类的代码分放在不同部分；你写的后台与 WPF 生成的部分一起组成这个类。

构造函数 Task009Window 在创建窗口对象时执行。InitializeComponent 是构建过程中生成的方法，用来加载配套 XAML、创建控件并连接名称和事件。执行后，DeviceIdTextBox 就指向这份窗口里实际的输入框。

XAML 的 xmlns 声明帮助解释标签来自哪里：默认的 WPF 声明让 TextBox 等标签找到 WPF 类型；xmlns:x 引入 x:Class、x:Name 等 XAML 写法。它们是标识，不要求程序打开这些网址下载控件。

## 8. 每次点击都读此刻的输入

当前第一句：

```csharp
string idStr = DeviceIdTextBox.Text.Trim();
```

先取得输入框 Text，再调用字符串的 Trim，去掉两端空白，最后把处理后的文字放进局部变量 idStr。

例如输入 `  TEMP-003  `，idStr 得到 TEMP-003。Trim 返回处理后的字符串；这一句没有把输入框 Text 重新赋值，所以屏幕上的原输入不一定同时被修剪。

这句在点击方法里面。用户每点一次，方法都重新读取输入，而不是只在窗口创建时读一次。

## 9. 输入不合法时，要结束这次方法

当前分支：

```csharp
if (idStr.Length == 0)
{
    StatusTextBlock.Text = "ID 为空或全是空白，请输入正确的ID";
    ResultTextBlock.Text = "";
    return;
}
```

空字符串或纯空格经过 Trim 后长度为 0。第一句给提示；第二句清掉旧结果，避免用户误把上次读数当成本次结果；return 结束整个点击方法。

只写提示不会阻止后面的成功代码。没有 return，程序仍可能继续生成读数并把提示覆盖为“读取成功”。

这是 TASK-009 的行为：无效输入清旧结果。后面的历史列表任务会保留旧历史，应分别按对应任务读。

## 10. Reading 是我们自己写的读数类

当前 Reading 保存三个值：

```csharp
public class Reading
{
    public string DeviceId { get; init; } = "";
    public double Value { get; init; }
    public string Unit { get; init; } = "";
}
```

Reading 是自定义类，不是 WPF 的设备读取 API。DeviceId 保存设备编号文字，Value 保存 double 数值，Unit 保存单位文字。public 允许外部代码访问这些属性。

先用普通写法理解“读取属性”。说明示例：

```csharp
private string _deviceId = "";

public string DeviceId
{
    get
    {
        return _deviceId;
    }
    set
    {
        _deviceId = value;
    }
}
```

读取 reading.DeviceId 时执行 get；给它赋值时执行 set，赋值右边的文字成为 value，字段保存实际值。自动属性省掉了显式字段。

原类用的是 init，而非示例的 set：可以在创建/初始化对象时赋值，初始化后不能在外部随意再赋值。默认的空字符串让未填的文字属性有初值。init 的限制不表示所有类因此都“深度不可变”。

## 11. new Reading 与花括号分别做什么

当前有效分支创建读数：

```csharp
Reading reading = new Reading
{
    DeviceId = idStr,
    Unit = "°C",
    Value = 26.5
};
```

new Reading 创建一个读数对象。reading 是这次方法里的变量，保存这个对象的引用。花括号是 C# 对象初始化器：把当前 ID、单位和模拟温度放进新对象。

这里创建的是数据对象，没有创建一个新窗口，也没有调用真实设备。每次有效点击都会创建本次自己的读数对象。

## 12. ToString 把这条读数变成文字

当前 Reading 还有：

```csharp
public override string ToString()
{
    return $"{DeviceId} {Unit} {Value}";
}
```

ToString 是对象已有的方法名称；override 表示在 Reading 中提供自己的实现。调用 reading.ToString() 时，执行这段方法。

美元符号加字符串中的花括号是 C# 字符串插值，把当前对象三个属性的值放进去。TASK-009 顺序为“ID、单位、数值”，所以得到 `TEMP-001 °C 26.5`。

TASK-010 使用另一个命名空间中的 Reading，顺序为“ID、数值、单位”。同名类不代表同一份定义，读输出时要看本任务实际引用哪个类。

## 13. 把字符串交给控件，屏幕才有结果

当前最后两项操作：

```csharp
ResultTextBlock.Text = reading.ToString();
StatusTextBlock.Text = "读取成功";
```

左侧分别是结果控件和状态控件的 Text。给它们赋值后，窗口显示相应文字。Console.WriteLine 是往控制台写，不会替这两个控件更新文字。

当前方法末尾还有 return，表示结束方法；放在末尾合法。成功提示后如果又写旧占位提示，后面的赋值会覆盖前面的文字，应按执行顺序读。

整条过程是：

```text
点击 → 读取当前 Text → Trim → 检查空白
有效 → 创建 Reading → ToString → 设置结果和提示
无效 → 提示、清旧结果、return
```

## 14. 用几种输入对照代码

| 操作 | 当前代码应产生的结果 |
| :--- | :--- |
| 输入 TEMP-001 并读取 | TEMP-001 °C 26.5，读取成功 |
| 改 TEMP-002 并读取 | 结果使用 TEMP-002 |
| 输入空字符串或纯空格 | 结果清空，提示 ID 为空或全是空白 |
| 再读两侧空格 TEMP-003 | 结果使用修剪后的 TEMP-003 |

如果按钮没有反应，先核对运行的窗口与 Click 方法名；如果总是显示旧 ID，核对读取输入的语句是否在每次点击的方法里；如果无效输入最后仍成功，核对 return 和后面的赋值。

## 文末查阅：文件、运行与证据

实际文件：[窗口 XAML](../../WpfBae/Exercises/TASK-009-window-events/Task009Window.xaml)、[窗口 C# 与 Reading](../../WpfBae/Exercises/TASK-009-window-events/Task009Window.xaml.cs)、[项目文件](../../WpfBae/WpfBae.csproj)、[App.xaml](../../WpfBae/App.xaml)。

项目使用 net10.0-windows、UseWPF=true；WinExe 指定桌面应用输出，Nullable 开启可空检查，ImplicitUsings 提供部分常用 using。using System.Windows 让 C# 能用短名称写 Window/RoutedEventArgs；它没有自动创建窗口。解决方案用于组织项目，实际编译配置在项目文件中。

当前 App.xaml 的 StartupUri 指向 TASK-017。执行 `dotnet run --project WpfBae/WpfBae.csproj` 现在打开的是 TASK-017。以后复看 TASK-009，需要明确临时指向本任务 XAML，并在观察后恢复入口；本轮没有切换、构建或运行。

一个可选观察：只改输入框下方 Margin 的 12 为 24，重跑 TASK-009，预期输入框与按钮之间更宽，再恢复。代码修改、设计预览、实际运行和验收是不同事实。

[任务卡](../tasks/TASK-009.md)记录 2026-10-09 已通关：构建、原窗口启动、按钮自动化调用、不同 ID/空白恢复、默认与最小尺寸通过。原 evidence/TASK-009 的 JSON/图片当前缺失，历史结论据任务卡保留。未记录的长 ID、手工鼠标或键盘操作不扩大为已验证。

自主验证范围是当前输入校验、创建读数和显示逻辑；布局、事件连接等骨架由教练提供，Grid/布局机制等继续按学习记录复习。旧 TODO 注释不单独表示功能未完成。

需要查原定义时，可读 [WPF 布局问答](../questions/wpf-layout.md)及 [Microsoft XAML 概览](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/xaml/)。下一篇是 [TASK-010：让数据属性驱动文字](./TASK-010-wpf-binding.md)。
