# TASK-049静态源码审查

- 日期：2026-10-20。
- 范围：按用户已有偏好只读源码并修复教练骨架缺陷，本轮未构建、运行、测试或建立TCP连接。
- 结论：修复教练遗漏的协议异常捕获后，三个DoD按静态源码口径通过，任务✅ 已通关；实际完整TCP联调仍未验证。

## 逐项核对

| DoD | 结论 | 源码依据与边界 |
| --- | --- | --- |
| 1：连接、LF命令往返与状态确认。 | 通过（静态）。 | OpenAsync先清理旧连接，再创建_client、await ConnectAsync并取得_stream，成功提示在连接之后；SendRequestAsync按UTF-8编码command加LF，等待WriteAsync后才记录发送，继续等待ReadReplyAsync；菜单串行处理，PrintReply仅对匹配回复显示温度或控制成功，GET_STATUS核对RUNNING/STOPPED；模拟器保存实际状态，未运行两端验证。 |
| 2：有效追加、半条/连续/长度边界。 | 通过（静态）。 | Append只遍历count，不清空已有数据；TryTakeMessage按第一个LF判断当前消息长度，无LF且Count不超过256则等待，拒绝空行/超过256，复制完整消息解码后只RemoveRange到该LF，保留后续；观察辅助以while连续取消息；TODO 4为教练协助实现，历史18项局部检查单独保留，不算本轮新测试或学员独立设计。 |
| 3：关闭重连、错误后可继续、退出清理与归属。 | 通过（静态）。 | CloseConnection释放_stream/_client并清空缓冲；OpenAsync先关闭再创建新对象，失败与取消分支清理，RunAsync的finally覆盖退出；本轮补齐InvalidDataException捕获后，协议拒绝会关闭旧连接、显示原因并回到菜单；本轮未运行重连、取消或Ctrl+C场景。 |

## 教练骨架修复

原RunAsync只捕获OperationCanceledException、SocketException和IOException，而TryTakeMessage与PrintReply会抛出InvalidDataException。
InvalidDataException并不继承IOException，因此原代码遇到空消息、超长消息或回复不匹配时，会越过菜单内的捕获分支，进入Program的最外层错误处理并退出。[Microsoft Learn：InvalidDataException继承关系](https://learn.microsoft.com/en-us/dotnet/api/system.io.invaliddataexception?view=net-10.0)
本轮只新增一个catch (InvalidDataException exception)，调用既有CloseConnection并打印协议失败，之后回到菜单；这是教练提供的生命周期骨架问题，不作为学员错题或额外修复要求。
本轮保留学员OpenAsync、SendRequestAsync、Append的代码和注释，保留既有TryTakeMessage；未修改模拟器、入口、项目文件及旧阶段代码。
修复通过静态路径复核，未冒充编译或运行验证；审查前后哈希与限定变更核对见static-review-2026-10-20.json及static-review-before-2026-10-20.json。

## 能力范围和下一步

学员在已提供骨架中完成了创建TcpClient、等待连接并取得NetworkStream、UTF-8/LF命令编码与异步写入/读取连接、按count追加有效字节的源码应用，本轮仅确认这些实现的静态正确性。
教程中的连接参考、教练提供的读取循环、菜单/显示、3秒保护、资源清理和模拟器不计学员独立设计；TODO 4与本轮catch修复均为教练协助，不把任务通关扩大成自主掌握TCP Server、消息解析设计、超时或生命周期机制。
按计划下一步推进完整WPF监控接入，只创建TASK-050未开始卡片，不创建WPF工程、源码骨架或切换入口；第四阶段仍🟡 进行中，实际TCP联调和原待复习问答保持原证据边界。
