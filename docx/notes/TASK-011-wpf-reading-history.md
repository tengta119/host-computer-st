# TASK-011：每次读取加一条历史，清空后还能继续读

> 2026-10-13 白话重写。先看记录放在哪里，再看列表怎样连接它。原任务已经通关；本篇解释当前代码，历史依据和运行入口放在文末。

## 1. 从一条结果，变成保留多条历史

这次读取 TEMP-001 后，列表出现一行；再读一次，第一行保留，下面增加第二行；改 TEMP-002 再读，得到第三行。

程序不能只有一个“结果文字”，还需要一份集合保存这些 Reading 对象。ListBox 是 WPF 提供的列表控件，负责把集合里的记录显示出来。

当前三个动作：

```text
有效读取 → 往原集合追加 Reading → 列表多一行
无效输入 → 给提示 → 旧历史保持
点击清空 → 清掉原集合内容 → 列表为空
```

这一题状态提示仍直接写 TextBlock.Text，列表才使用集合绑定。按当前任务看，不必把所有地方同时改成绑定。

## 2. ReadingHistoryState 是装历史集合的自定义对象

教练新建了 ReadingHistoryState 类。它只有一个任务：创建并提供历史集合。

```csharp
public class ReadingHistoryState
{
    private ObservableCollection<Reading> _readings =
        new ObservableCollection<Reading>();

    public ObservableCollection<Reading> Readings
    {
        get
        {
            return _readings;
        }
    }
}
```

ReadingHistoryState、_readings、Readings 都是我们取的名字。ObservableCollection 是 .NET 提供的集合类，Reading 是 TASK-010 已有的读数类。

new ObservableCollection<Reading>() 创建一份空集合；字段 _readings 保存它；公开属性 Readings 把它交给窗口和按钮使用。

## 3. 尖括号里的 Reading，说明集合装什么

```csharp
ObservableCollection<Reading>
```

可以读成：“装 Reading 对象的 ObservableCollection。”

Reading 每个对象有 DeviceId、Value、Unit。集合存的是这些对象，而不是屏幕上的 TextBlock。

用 Java 集合经验作有限对照：尖括号同样指出元素类型。这次新的地方是集合增删时有内置通知，后面再看它怎样帮助 WPF 更新。

所需 using：

```csharp
using System.Collections.ObjectModel;
using WpfBae.Exercises.Task010;
```

第一项让 C# 找到集合类型的短名称；第二项让本题使用 Task010 的 Reading，不用复制一个同名类。

## 4. _state、集合和记录，是三个不同层次

窗口创建：

```csharp
private ReadingHistoryState _state = new ReadingHistoryState();
```

这会创建一个 ReadingHistoryState；它内部再创建自己的集合。有效读取时才创建 Reading 记录。

```text
窗口字段 _state
  → 一个 ReadingHistoryState 对象
      → 它的历史集合
          → 第一条 Reading
          → 第二条 Reading
```

改变集合内容，不需要重新创建 _state。追加记录，也不需要重新创建集合。

每次读取产生的新 Reading 保存本次 ID；改输入不等于回头修改第一条 Reading。

## 5. getter 返回原集合，所以没有 setter 也能 Add

这句：

```csharp
return _readings;
```

把已经存在的集合交出去，没有 new，没有复制其中记录。

因此：

```csharp
_state.Readings.Add(reading);
```

可分开理解成下面的说明写法：

```csharp
ObservableCollection<Reading> history = _state.Readings;
history.Add(reading);
```

history 与 _readings 指向同一份集合。

Readings 只有 get，外部不能用 `_state.Readings = 另一份集合` 换掉属性值；仍能对取出的集合调用 Add/Clear。“不能换集合”与“不能改集合内容”不是一件事。

## 6. DataContext 指定数据对象，ItemsSource 指定记录集合

窗口构造函数已有：

```csharp
InitializeComponent();
DataContext = _state;
```

它告诉窗口里的默认 Binding 去 _state 找公开属性。列表写：

```xml
<ListBox x:Name="HistoryListBox"
         ItemsSource="{Binding Readings, Mode=OneWay}"
         Height="170" />
```

左侧 ItemsSource 是 ListBox 的属性，决定这张列表从哪个集合取得项目。右侧 Binding Readings 去 _state 读取 Readings 的 getter，得到原集合。

OneWay 表示集合来源从对象交给列表，不把列表的属性反写回 _state。这里绑定的是整个集合来源，不是某一条 Reading.Value。

Height=170 给列表一个显示区域；HistoryListBox 是具体控件名。名字方便 C# 或审查找到控件，Binding 中的 Readings 则是数据属性名。

## 7. ObservableCollection 比 List 多了什么

List<Reading> 与 ObservableCollection<Reading> 都能保存记录、Add、Clear、取得 Count。

区别发生在列表已经连接后。ObservableCollection 在内容增删时会告诉监听方“集合变化了”；普通 List 没有同样的内置集合变化事件，所以对原 List 调 Add，不能单靠这次连接保证 WPF 实时增加行。

当前 WPF 列表监听集合，学员只需调用 Add/Clear，不必自己写“刷新列表”的方法。

这一通知只负责成员的增加、移除、清空等变化。它不会自动替集合中每条 Reading 发布 Value 属性变化。

## 8. CollectionChanged 的通知说了什么

CollectionChanged 是集合提供的事件。Add 一条时，通知可以说明新增了哪项；Clear 全部时，发出 Reset，表示集合项目需要重新看。

NotifyCollectionChangedEventArgs 是 .NET 的参数类，来自 System.Collections.Specialized。它可以携带操作类别、新旧项目等信息。这里不要求在业务代码中自己创建这些参数，集合会处理。

说明观察写法：

```csharp
history.CollectionChanged += HistoryChanged;
```

表示有人开始监听 history 的集合变化。当前窗口没有为了更新列表手动写这行；WPF 列表建立连接时负责监听。后来的命令任务还会利用这个事件，通知按钮重查条件。

## 9. 一次有效读取，怎样进入历史

当前按钮先读输入、校验，随后创建 Reading：

```csharp
Reading reading = new Reading
{
    DeviceId = id,
    Value = 26.5,
    Unit = "°C"
};

_state.Readings.Add(reading);
StatusTextBlock.Text = "读取成功";
```

new Reading 创建本次记录。id 来自这一次输入，26.5/°C 是模拟值。

Add 的参数就是这个对象。原集合末尾多一项，发出新增通知，ListBox 得到新项目并显示新行。

这里没有给 ItemsSource 重新赋值，也没有操作 HistoryListBox.Items.Add。数据集合是唯一追加入口，窗口显示跟着它走。

## 10. 为什么重复 ID 仍然出现两行

原代码没有按 ID 查找并覆盖旧记录。每次有效点击都是：

```text
创建一条新 Reading → Add 到末尾
```

所以：

| 点击顺序 | 集合里的记录 |
| :--- | :--- |
| TEMP-001 第一次 | 一条 TEMP-001 |
| TEMP-001 第二次 | 两条 TEMP-001 |
| TEMP-002 一次 | 前两条保留，再加 TEMP-002 |

Count 是集合成员数，不是不同设备 ID 的数量。两次同 ID 仍算两次读数。

## 11. ListBox 为什么能显示 ID、数值和单位

当前列表没有 DataTemplate。WPF 按对象的字符串表示来显示它；Reading 重写了 ToString：

```csharp
public override string ToString()
{
    return $"{DeviceId} {Value} {Unit} ";
}
```

因此每行显示当前对象的 ID、26.5、°C。末尾空格是原方法里的字符，本篇如实保留。

空集合时没有行，是正常初始状态，不表示绑定失败。若集合有记录但只显示类名，应检查读数类是否提供合适的字符串表示。

TASK-016 后来用三个 TextBlock 分开显示字段；那是另一种显示方式，这次无需提前引入模板。

## 12. 空白输入只提示，保留已有历史

当前分支：

```csharp
string id = DeviceIdTextBox.Text.Trim();

if (string.IsNullOrEmpty(id))
{
    StatusTextBlock.Text = "请输入设备 ID；本次没有新增记录。";
    return;
}
```

return 使执行停在创建/追加记录之前。这次没有 Add，也没有 Clear，所以旧历史保持。

这与 TASK-009/010 的“清掉单条旧结果”不同。现在历史代表之前已经发生的读取，无效的新输入不该删除它。

Trim 处理局部 id，没给输入框 Text 回写。纯空格变为空后也走这个分支。

## 13. Clear 清内容，原集合还在

当前清空方法：

```csharp
private void ClearButton_Click(object sender, RoutedEventArgs e)
{
    _state.Readings.Clear();
    StatusTextBlock.Text = "已清空";
}
```

Clear 不需要参数，把集合中的项目移除。它没有将 _state 设为 null，也没有创建一个新集合。

列表收到清空通知后变为空。再次读取仍 Add 到原集合，所以能恢复显示。

当前清空按钮还是 Click 事件按钮，空历史时也没有本题代码规定的命令禁用条件；TASK-012 才加入这一层。

## 14. 为什么提示没做 PropertyChanged 也能变

当前成功或清空时直接写：

```csharp
StatusTextBlock.Text = "已清空";
```

左侧就是显示控件，因此这次不需要先改状态对象再发属性通知。

ReadingHistoryState 也没有 INotifyPropertyChanged。它一直返回同一集合，列表内容增删由 ObservableCollection 自己通知。

如果以后把 Readings 属性改成另一份集合，或者修改已有可变记录的 Value，那是其他变化，需要相应通知设计。当前成功的 Add/Clear 不证明那些情况也已支持。

## 15. 三种变化分清，排查会更直接

| 你做的事 | 哪层变化 | 当前用什么处理 |
| :--- | :--- | :--- |
| Add/Clear 原集合 | 集合成员 | CollectionChanged |
| 提示文字改变 | 显示控件 Text | 直接赋值 |
| 修改一条 Reading 的属性 | 数据项内部 | 本题未实现/未要求运行更新 |

不要为了列表更新同时调用集合 Add 与 ListBox.Items.Add。ItemsSource 已提供集合后，直接操纵 Items 还可能受到 WPF 限制；会把数据和显示混成两套路径。

若集合 Count 变了而行没变，先核对 ItemsSource 是否指向那个集合、是否用了可通知集合；若换 ID 时旧行没保留，检查是否误用了 Clear 或替换集合。

## 文末查阅：文件、观察和历史范围

真实文件：[数据类](../../WpfBae/Exercises/TASK-011-reading-history/ReadingHistoryState.cs)、[窗口 XAML](../../WpfBae/Exercises/TASK-011-reading-history/Task011Window.xaml)、[读取/清空方法](../../WpfBae/Exercises/TASK-011-reading-history/Task011Window.xaml.cs)、[Reading 来源](../../WpfBae/Exercises/TASK-010-reading-binding/Task010Window.xaml.cs)、[任务卡](../tasks/TASK-011.md)。

复习可以只观察“同 ID 连点两次是否两行”，再单独观察 Clear 后再读。不需为了观察增加新类。当前 [App.xaml](../../WpfBae/App.xaml) 指向 TASK-017，默认运行命令不会自动打开 TASK-011；复跑旧题须明确临时入口并恢复。本轮没有运行。

任务卡记录 2026-10-11 已通关：构建/原应用启动、同对象同集合、追加/换 ID、空白保留、清空恢复通过。历史审查是实际窗口的 Click 路由事件路径。原 evidence/TASK-011 JSON 当前缺失，因此历史结论引用卡片，未声称本轮重跑或当前文件匹配当时缺失的哈希明细。

自主应用证据限原骨架中的 Add/Clear 与提示；数据类、getter、绑定和输入校验由骨架提供。集合项目通知与元素属性变化仍应分别理解。

答疑见 [集合问答](../questions/wpf-collections.md)；框架定义见 [ObservableCollection](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1?view=net-10.0)。下一篇学习 [空历史时禁用清空按钮](./TASK-012-wpf-commands.md)。
