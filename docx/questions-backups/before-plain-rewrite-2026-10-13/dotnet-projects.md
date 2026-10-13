# .NET 项目与解决方案

## Q1：完成 TASK-009 是否需要新建项目？

- 状态：待复习
- 日期：2026-10-07
- 关联：[TASK-009](../tasks/TASK-009.md)、[Base.csproj](../../Base/Base.csproj)。
- 现象：学员使用 Rider 的新建解决方案界面，已选择桌面分类、C#、net10.0 与 WPF Application，询问是否需要新建项目。
- 原因：当前工作区仅有 Base 控制台项目及 Base/Base.slnx；TASK-009 需要实际窗口、XAML 和按钮事件。按任务卡采用独立 WPF 项目，便于保留既有控制台练习。
- 机制：项目由 .csproj 描述源码和构建设置，解决方案由 .sln/.slnx 组织一个或多个项目；新建 WPF 项目不要求一定新建解决方案。截图中的新建解决方案方式可用于单独管理 WPF 练习，现有解决方案也可以容纳另一个项目。
- 创建建议：沿当前界面选择 WPF Application，语言 C#，目标框架 net10.0；解决方案名和项目名均用 WpfPractice，解决方案目录选 D:\host-computer-st，勾选“将解决方案和项目放在同一目录中”，保持“创建 Git 仓库”未勾选。检查界面预览，使生成项目位于 D:\host-computer-st\WpfPractice，避免默认 RiderProjects 位置或重复嵌套目录。
- 类比：解决方案类似组织多个模块的工作集合，项目类似有独立构建配置的模块；类比不意味着 .NET 与 Java 构建规则相同。
- 总结与复查点：为 TASK-009 新建独立 WPF 项目即可，项目与解决方案需要区分。上述路径为创建目标，当前尚未创建 WPF 文件、未启动任务或运行验收；待学员实际创建或明确要求开始后检查实际目录。
- 依据：[Rider 桌面应用入门](https://www.jetbrains.com/help/rider/Get_started_with_net_desktop_apps.html)、[创建项目与解决方案](https://www.jetbrains.com/help/rider/Creating_and_Opening_Projects_and_Solutions.html)。
