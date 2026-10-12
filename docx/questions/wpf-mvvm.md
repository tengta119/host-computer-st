# WPF 窗口与 ViewModel

> [返回问答索引](./README.md)

## Q1：ViewModel 是什么，输入框的文字怎样进入读取命令？

- 日期：2026-10-12
- 状态：待复习
- 来源：TASK-013 启动的必要讲解；沿用 TASK-012 的对象与命令场景。

**现象与原因**：TASK-012 的读取 Click 方法位于窗口，直接取得 DeviceIdTextBox.Text；清空已经通过命令使用数据对象。希望输入和两个按钮统一使用界面数据，让读取动作不依赖控件。

**来源与身份**：MVVM 是代码组织方式。本任务 View 是 Task013Window 的 XAML 界面，Model 是复用的 Reading 读数类，ViewModel 是教练新建的普通 C# 类 ReadingViewModel。类名、DeviceId/ReadCommand 等属性名、命令类名及 OnPropertyChanged 工具名是自行命名的；DataContext、TextBox.Text、Button.Command、Binding 是 WPF 提供，ICommand/INotifyPropertyChanged 等是 .NET 接口。无需安装新框架。

**对象连接**：窗口 `new ReadingViewModel()` 创建实例，字段 `_viewModel` 保存它，DataContext 指向它。该对象构造时用 `new ReadTemperatureCommand(this)` 和 `new ClearHistoryCommand(this)` 创建两个命令；此处 this 指当前 ViewModel。命令保存传入引用，使用同一个对象及原历史集合。ReadCommand/ClearCommand 的完整 getter 返回原命令对象，按钮的 Binding 按属性名读取；不直接读取私有字段。

**输入机制**：`Text="{Binding DeviceId, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"` 表示双向连接：对象初始 DeviceId 显示到输入框；编辑输入框也将字符串写回 DeviceId。WPF 调用 setter 时，新字符串成为 value，`_deviceId = value` 保存它；getter 的 `return _deviceId` 返回当前文字。读取 Execute 取 `_viewModel.DeviceId` 即可，无需访问 TextBox。

UpdateSourceTrigger 是 WPF Binding 的更新时机设置。TextBox.Text 默认失去焦点才写回；本骨架的 PropertyChanged 表示文字变化时写回，依据 [Microsoft 文档](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-control-when-the-textbox-text-updates-the-source)。此设置不等于自定义 OnPropertyChanged 方法：前者决定控件何时写到对象；后者发布对象属性变化通知，让绑定重新读取公开属性。骨架提供完整 get/set 和通知工具，暂不要求自主重写。

**最小实践**：在 ReadTemperatureCommand.Execute 的 TODO 1 以两行替换占位状态赋值：

```csharp
string id = _viewModel.DeviceId;
_viewModel.StatusText = "命令取得的设备 ID：" + id;
```

实际运行后将输入改为 TEMP-002，再点击读取，预期提示显示当前 ID。赋值经过 StatusText setter 保存并通知，TextBlock 的绑定重新读取提示。此步骤只验证输入→对象→命令→提示，尚不追加历史；后续再完成 Trim、空白保护、Reading 构造、Add 和成功提示。

**证据边界**：启动时教练只验证骨架构建和原程序窗口启动，未验证学员的输入写回或读取功能，DoD 均未勾选。本条待复习；TASK-012 已验证应用范围保留，委托/事件及一般属性机制不因新骨架存在而认定掌握。
