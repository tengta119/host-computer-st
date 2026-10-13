# .NET 项目与解决方案

> [返回问答索引](./README.md)
> 2026-10-13 按当前 AGENTS.md 重构。原问题编号与掌握状态保留；历史证据集中到文末。

## Q1：完成 TASK-009 是否需要新建项目？

- 状态：待复习
- 日期：2026-10-07
- 关联：[TASK-009](../tasks/TASK-009.md)、[Base.csproj](../../Base/Base.csproj)。

当时 TASK-001～008 都在 Base 控制台项目中，TASK-009 要实际打开窗口、显示输入框与按钮，所以需要一个配置为 WPF 的项目。这个判断已经落实：现在工作区使用的是 [WpfBae.csproj](../../WpfBae/WpfBae.csproj)，无需为每个后续 WPF 任务另建项目。

```xml
<OutputType>WinExe</OutputType>
<TargetFramework>net10.0-windows</TargetFramework>
<UseWPF>true</UseWPF>
```

这是当前 WpfBae.csproj 中的配置。OutputType 选择 Windows 程序形式，TargetFramework 选择 .NET 10 的 Windows 目标，UseWPF 启用 WPF 构建支持。csproj 是项目的构建配置文件，不是窗口界面。

[WpfBae.sln](../../WpfBae/WpfBae.sln) 是解决方案文件，用于组织项目，便于 Rider 打开和管理。一个解决方案可容纳多个项目；新增项目与新增解决方案是两个选择，不必绑在一起。

当时建议的 WpfPractice 是拟用名称；你最终创建的真实目录是 WpfBae。复习应以现在的文件名为准，不去创建第二套 WpfPractice，也不移动已完成作品。可以把项目类比为有独立构建配置的 Java 模块，把解决方案看成管理这些模块的集合，具体规则仍以 .NET 为准。

## 历史证据与掌握范围

Q1 保持待复习。2026-10-07 的新建界面与建议保存在旧稿，TASK-009 后续已完成。此处更新当前路径和阅读方式，未新建项目、切换入口或运行程序。

相关任务卡与验收来源：[TASK-009](../tasks/TASK-009.md)。

完整原问题、追问、教学更正与逐次复查记录见[重构前原稿](../questions-backups/before-plain-rewrite-2026-10-13/dotnet-projects.md)。该快照保留原文，里面的旧路径/当时状态可能已过时；当前任务状态以[学习进度](../learning.md)和对应任务卡为准。

## 官方依据

- [Rider 桌面应用入门](https://www.jetbrains.com/help/rider/Get_started_with_net_desktop_apps.html)
- [创建项目与解决方案](https://www.jetbrains.com/help/rider/Creating_and_Opening_Projects_and_Solutions.html)
