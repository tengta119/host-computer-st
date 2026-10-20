# TASK-046 静态源码审查：2026-10-20

- 口径：沿用用户“只看源码，源码正确即可通过”的要求，本轮未构建、运行、测试、打开串口或执行诊断宿主。
- 结论：🟡 进行中，存在两处核心阻塞，不创建TASK-047。

## 源码发现

1. Task046Window.xaml.cs的BeginWaiting（89至98行）记录编号/时间、重启秒表和检查定时器，但没有把_waitingForResponse设为true，也没有刷新按钮和RequestStatusText；该字段初始false，其他可见赋值只有EndWaiting里的false，因此发送防重入永远不生效，CheckResponseTimeout总会在第一个条件返回，完整读数也不会进入结束等待分支。
2. 同文件126行使用if (_connection.BelongsToCurrentConnection(_waitingSessionId))后立即EndWaiting，方向与“原连接已结束才清等待”相反；第1处补好后，当前连接编号正常匹配时会在第一次定时检查就结束等待，无法走到3秒超时或当前连接失效检查，需要让不属于当前连接时才进入这个分支。

## DoD逐项结论

| DoD | 静态结论 | 依据与边界 |
| --- | --- | --- |
| 1：等待、完整回复、超时、防重复和重发。 | 未通过。 | 上述两处使等待标记与超时路径未连接，已有EndWaiting和接收后的调用不能弥补入口缺失；未声称实际观察过超时。 |
| 2：失败原因、释放旧连接与手动重连。 | 源码通过。 | Open失败撤掉端口，发送/接收异常转RecoverConnection，后者先结束等待再Close，catch追加关闭错误，finally按实际状态刷新按钮并保留原因，IsOpen为false的当前错误仍能被处理；实际设备断开/重连未运行验证。 |
| 3：主动断开、重连与窗口关闭清理。 | 源码通过。 | 主动断开调用统一恢复，Close增加编号/清缓冲/撤登记，UI对排队通知再查编号，OnClosed停表/撤Tick与接收事件/Dispose，重连前结束旧等待；旧状态保护是代码路径判断，未作为实际并发验证。 |

## 教练协助与后续动作

只把旧连接分支中的“连接超时”改为“原连接已结束，取消本次等待。”，重读源码确认文字已改，未代改上述两个核心问题；该分支不是按时长判断，原文案会混淆连接更换和响应超时。
先完成BeginWaiting中的true赋值、RefreshConnectionControls和RequestStatusText等待提示，再纠正126行条件方向；保留学员已经接好的EndWaiting、RecoverConnection与其他实现。
串口Q4仍待复习，教练给过示例与源码静态合理均不扩大为学员独立掌握或实际通信证据。
