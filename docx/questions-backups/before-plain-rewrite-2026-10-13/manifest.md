# 问答库重构前快照

- 日期：2026-10-13。
- 范围：`docx/questions/` 的22个分类文件和README.md，共23份原稿；逐字节复制，保留原问题、补充、更正、状态变化和历史记录。
- 当前阅读入口：[问答库](../../questions/README.md)。本目录是历史快照，不作为当前源码位置或任务状态来源。
- `questions-hashes-before.json` 记录复制前原稿SHA256，备份用于核对与恢复，不覆盖学员历史。
- `protected-hashes-before.json` 记录本轮前源码/项目、全部任务卡/证据、AGENTS.md、计划和错题本SHA256，用于确认整理没有修改这些内容。
- 整理期间TASK-020的XAML在工作区变为Width=260，重写未修改该源码，问答保留此观察且未新增验收。`protected-hashes-at-render.json`记录文档写入前已核对的现状，随后比较520个受保护文件，区分工作区并行修改与本次文档写入。
- `rewrite_questions.py` 是本次文档整理脚本，保留原Q标题和状态、核对追问覆盖并检查当前本地链接，不构建或运行练习。
- [verification.json](./verification.json)在实际整理与核对完成后生成，记录文件/问题数量、状态保持与源码保护检查。本轮没有新的运行验收或掌握证据。

原索引保存在[README.md](./README.md)。各分类原稿与当前分类同名；旧相对链接按原目录写，快照中的路径与当时状态可能已过时。
