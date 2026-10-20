# 典型错误与复查

## TASK-048：等待结束日志重新读取当前连接，未用等待快照

- 2026-10-20最终源码复查通过：学员已将等待结束日志第三/四实参改为waitingSessionId/waitingPortName，message保留，核心归属问题已修复，三个DoD静态通过、TASK-048✅ 已通关；本轮未改源码或运行测试，以下待修结论为此前历史。

- 情况与证据：2026-10-20仅静态审查，EndWaiting第117/118行已保存等待编号/端口，第131行却把_connection.CurrentSessionId和_connection.PortName传给日志；旧等待清理分支下可能把旧记录标成新连接，见[审查记录](../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-048-serial-monitor-integration/Verification/static-review-2026-10-20.md)，未运行重现场景。
- 根因：可见调用没有使用已保存的waitingSessionId/waitingPortName；当前连接信息与原等待所属信息在旧等待分支下可能不同，不推测学员编写动机。
- 修复线索：日志第三、第四个实参使用已保存的两个局部值，不改原等待结束或串口关闭流程。
- 复查点：等待结束说明使用message，归属使用等待快照，清理结果继续使用oldSessionId/oldPortName；有效字节、200条上限和窗口线程保护保留，按用户偏好只查源码。
- 状态与边界：本条待修，TASK-048🟡 进行中，DoD1/2静态通过、3待修；两处文案由教练直接收尾，不计学员认知错题，Q6待复习，旧能力保持。


## TASK-047：检查了加和却漏掉原始值范围

- 2026-10-20最终源码复查通过：学员补int原始值合成与大于1000拒绝，0/1000保留，三个DoD静态通过、TASK-047✅ 已通关；未构建/运行/测试，教练只清一处旧提示，本条范围漏检已修复，以下待修状态为原审查历史。

- 情况与证据：2026-10-20静态审查，IsValidFrame在设备/命令和加和符合后直接返回true，缺少原始值范围判断；菜单8的AA 01 03 03 E9 9A加和正确但原始1001超出本题0至1000，源码会接受，见[静态记录](../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-047-frame-validation/Verification/static-review-2026-10-20.md)，未运行反例。
- 根因：可见方法里没有两字节数值合成与范围条件，已有加和判断不能替代该条件，不推测学员是否误解校验原理。
- 修复线索：返回true前合成frame[3] * 256 + frame[4]，拒绝大于1000；byte合成无负数，0和1000都是合法边界，原有字段与加和不用重写。
- 复查点：1001被拒绝、0/1000保留、坏帧不通知窗口或结束等待，坏帧后的好帧仍由已完成的恢复循环继续寻找；后续仍按用户偏好查源码，不擅自测试。
- 状态与边界：本条待修，TASK-047🟡 进行中，DoD2仅静态通过、1/3未通过，Q5待复习，已给局部参考不算独立掌握，既有实践能力保持。


## TASK-046：等待入口漏设状态，连接归属条件方向相反

- 2026-10-20最终源码复查通过：学员已补_waitingForResponse = true并将连接归属条件取反；教练只补按钮/等待提示刷新与旧占位文案，核心修复保留，三个DoD静态通过、TASK-046✅ 已通关，未构建/运行/测试，以下待修状态为此前审查历史。

- 情况与证据：2026-10-20仅静态审查，BeginWaiting未赋_waitingForResponse = true，定时检查首个条件会一直返回；126行在BelongsToCurrentConnection为true时结束等待，正常同一连接也会进入结束分支，见[静态记录](../Stage03-DeviceCommunication/DeviceCommunication.Wpf/Exercises/TASK-046-serial-recovery/Verification/static-review-2026-10-20.md)。
- 根因：可见代码只写结束时false、没有写开始时true，且“属于当前连接”被当成“应该结束旧等待”的条件；仅记录源码中的状态连接与条件方向，不推测学员编写动机。
- 修复线索：开始时设true并刷新按钮/等待提示，旧连接分支以“不属于当前连接”为条件；EndWaiting、RecoverConnection无须重写。
- 复查点：源码能从开始true进入定时检查，同一连接继续等到回复或3秒，不同连接才清旧等待，结束后恢复false与发送按钮；按用户偏好复查源码，不擅自运行测试。
- 状态与边界：本条待修，TASK-046仍🟡 进行中，DoD1未通过、2/3仅静态通过；教练表达修订和已给示例不记作学员错误，原实践能力保持。


> 仅记录实际观察到的错误。计划里的常见坑不是学员已犯的错误。

<!-- 发生错误后追加：情况与证据、根因、修复线索、复查点及状态。 -->

## TASK-044：用整数除法计算温度，再用F1补显示

- 状态：✅ 已通关。
- 2026-10-19按用户静态口径复查通过：学员已将原始值保存在int rawValue，再用double temperature = rawValue / 10.0，教练仅补原始值与温度显示；小数丢失问题从源码上已修正，未运行或测试，见[最终静态记录](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Verification/static-review-final-2026-10-19.md)，下方错误和待修线索为历史，不声称串口实际结果已验证。
- 情况与证据：2026-10-19按用户要求仅静态审查，PrintReading第168行用int value = (frame[3] * 256 + frame[4]) / 10，第169行对value用F1显示；原始256按C#整数除法得到25而不是25.6，且未单独输出原始值，见[静态记录](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-044-serial-roundtrip/Verification/static-review-2026-10-19.md)，未把推导当作实际运行。
- 根因：换算时两边都是整数，除法在格式化之前已丢掉小数，F1只改变显示形式；只记录可见表达式，不推测编写动机。
- 修复线索：int变量先保留两字节合成的原始值，double变量保存原始值除以10.0的温度，分别显示两者，核心修改由学员完成。
- 复查点：源码中原始值与温度分开、温度计算为浮点除法、F1用于温度；01 00应对应原始256和25.6℃，是否运行仍按用户届时要求执行，不安排本轮测试。
- 掌握边界：Open/Write/Read返回有效数量/Close+Clear的源码连接静态合理，但本轮未运行验证，不新增串口自主掌握，TASK-039至043已有能力保持，教练骨架或讲解反馈不归为学员错误。

## TASK-043：取帧放在循环外，取完后没有继续处理剩余缓冲

- 2026-10-19最终复查通过：学员自主把不足6等待判断移到取帧前、非AA删除之后，七组16个检查点和6组输入保护全部通过，场景六取A后清99、只留AA，补齐后输出B，所有零数量不新增帧，无死循环或重复输出；教练未改实现，见[最终证据](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-05-final-2026-10-19.json)，下方失败和修复线索均为历史。
- 2026-10-19第四版复查：七组正常退出0，16个检查点15符合/1不符合，DoD2/3通过、1未通过；唯一不符合是场景六取A后剩99 AA两项，Count < 6在取帧之后立即break使99未及时清理，见[第四版证据](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-04-2026-10-19.json)。
- 当前修复线索：把已有不足6判断整体移到取帧if之前、非AA删除分支之后，保证取完A后还能回到下一轮先删99，再只保留AA；当前半帧等待、完整帧输出、内部AA和零数量已通过，教练未代改实现，下方第三版卡住为历史。
- 2026-10-19第三版复查：取帧已移入while，完整A当次输出正确；当前缺少AA开头不足6的退出分支，AA 01既不删除也不取帧，循环4秒不返回后教练终止，见[第三版证据](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-03-2026-10-19.json)。
- 当前修复线索：在非AA删除分支之后、取帧判断之前补Count < 6时break，让半帧保留且本次方法能返回，下一次追加才能执行；DoD1无法完整验证、2/3未通过，三个均未勾，旧版半帧通过保留为历史，教练未代改实现。
- 2026-10-19同轮更新后复跑：学员新增break已消除死循环，七组正常退出0；但场景四首次取A后剩99和完整B共7字节，B延后到零数量，场景六第二段剩99 AA而非AA，最新根因是循环外只取一帧，取完不再回到循环，见[最新证据](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-02-2026-10-19.json)，下方卡住为旧版历史。
- 最新修复线索：把数量判断/取帧放进现有while，只在合法AA开头但不足6时break等待，完整帧取出后继续检查剩余字节；DoD2已通过、1/3未通过，教练未代改实现。
- 状态：✅ 已通关。
- 情况与证据：2026-10-19构建成功，场景一99 88清空且后续零数量安全；场景二收到99 88和完整A后没有输出，4秒仍未返回，由教练终止，见[首次审查证据](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-043-frame-resync/Verification/review-01-2026-10-19.json)。
- 根因：while内只处理非AA删除，遇AA时既不改变列表也不退出，Count > 0一直成立；等待/取帧代码放在while外，无法执行到，只记录可见代码结构与运行现象，不推测编写动机。
- 修复线索：遇AA后的等待和取帧放进现有while，少于6保留并停止，至少6取一帧后继续检查剩余内容，核心修改由学员完成。
- 复查点：场景二立即输出A/剩0，场景三先留AA 01再补齐A，帧间99清理、C内部AA原样保留、分段一致和零数量安全均通过，无死循环或重复输出。
- 掌握边界：短无效输入的非空保护、RemoveAt(0)与continue已有局部运行证据，TASK-042已有能力保持；此前教练TODO重复与用语难懂另记教学反馈，不归为学员错误，本轮未代改源码。

## TASK-042：完整帧循环遗漏恰好6字节的边界

- 2026-10-19最终复查：学员自主修正为Count >= 6，八项检查全部通过，12字节当次输出A/B并剩0，零数量后仍为2帧/剩0，2/3/1与8/4及输入保护均保持；本轮源码未改，见[最终证据](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/review-02-final.json)，下方失败为首次历史。
- 状态：✅ 已通关。
- 情况与证据：2026-10-19实际一次输入12字节A+B只输出A，缓冲剩6、帧数1；后续数量0调用才输出B，见[运行证据](../Stage03-DeviceCommunication/DeviceCommunication/Exercises/TASK-042-fixed-frame/Verification/review-2026-10-19.json)，DoD1通过、2/3未通过。
- 根因：取出第一帧后使用Count > 6作为重复处理条件，排除了恰好6这一整帧；只记录可见条件与运行结果，不推测编写动机。
- 修复线索：先将重复处理边界改为Count >= 6，核对每轮复制/移除使Count减少，少于6再停止等待，核心条件由学员修正。
- 复查点：12字节一次输入当场输出A/B、缓冲0；后续数量0不新增帧；2/3/1与8/4分段的内容、顺序、尾部保留及输入数组保护保持。
- 掌握边界：不足一帧保留、六字节复制、RemoveRange和打印连接已验证，记录中不撤销这些能力；没有观察到已输出帧重复，本轮未代改实现，完整重复取帧仍待复查。

## TASK-038：已写业务方法，但按钮、周期调用与显示通知未接完整

- 2026-10-18任务最终通关：264项功能已有全部通过证据，六组机械验收记录现按用户委托由教练整理，提交条件已满足，任务与本条状态同步；不新增学员测试报告能力，功能源码未代改，下方仅待记录为历史。

- 2026-10-18第二次审查：上述代码连接已由学员自主修复，264项功能全部通过，代码缺口已修复；任务仅因运行记录未提交保持进行中，不把空记录记为新的认知错误，见[结果](./tasks/evidence/TASK-038/review-02/results.json)，下方代码失败为历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-18实际按钮添加/清空无动作，Tick次数增加但历史不变，启停未发清空条件通知，输入未即时写回、有效阈值与反馈仍为占位文字；131项114通过/17失败，见[结果](./tasks/evidence/TASK-038/review-01/results.json)。
- 根因：Command路径误用了控件名AddButton/ClearButton而非VM属性AddCommand/ClearCommand，Tick漏调用已有追加方法，IsSampling漏条件重查通知，阈值显示绑定及CurrentThresholdText通知遗漏；只记录可见连接缺口，不推测编写动机。
- 修复线索：先改两个Command属性名，再接一次Tick追加与清空条件重查，最后接输入即时写回、同名显示绑定与派生显示通知，按原任务TODO顺序实际运行。
- 复查点：按钮添加1/清空0，真实自动追加、运行禁用与停止恢复，阈值输入和结果立即同步，当前设备完整CSV保持并提交实际运行记录。
- 掌握边界：设备选择/当前集合、独立位置、原命令方法保护/报警验证/配置恢复与CSV已有独立方法或真实操作证据，保留这些局部能力；不能用方法诊断替代未接通的按钮和定时器，教练表达与骨架辅助不计学员错误。
## TASK-037：把长结果与完整路径都放进横向StackPanel

- 2026-10-18最终复查：用户明确要求后，教练调整两处区域容器并补间距，264项全部通过，默认/最小完整路径可读，本条已修复；[最终结果](./tasks/evidence/TASK-037/review-final/results.json)，不计学员自主修复，原通过能力保留，下方失败为历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-18实际默认和最小窗口中ConfigStatusText、ConfigPathText、ExportPathText越过右边界，264项258通过/6失败，见[结果](./tasks/evidence/TASK-037/review-01/results.json)和[最小尺寸图](./tasks/evidence/TASK-037/review-01/minimum.png)。
- 根因：ThresholdArea和ExportArea把输入/按钮与不断增长的结果文字全部向右排，已有TextWrapping不会使父容器里的后续控件自动换到下一排，不推测学员编写动机。
- 修复线索：保留必要控件的内部横排，把反馈、状态和路径放到其下方的独立位置，外层竖直排列并保留TextWrapping及原绑定/名字。
- 复查点：实际保存阈值和导出后，默认与最小窗口完整显示结果和路径，按钮/图表/表格不受遮挡，表格仍随窗口伸展。
- 掌握边界：自主绑定、设备横排、根区域分行、曲线/表格分列及表格伸展均已验证，设备与采集等复用功能也通过，不撤销已有证据；教练此前“另建一行”表述另有更正，不归入本条错误。
## TASK-036：样例位置反复赋为1，选择框未连接禁用条件

- 2026-10-18最终复查：学员自主修复位置递增和IsEnabled绑定，235项全通过，两台独立样例循环与清空复位、采中禁用和停止恢复均正确，本条已修复；[最终结果](./tasks/evidence/TASK-036/review-final/results.json)，本轮未代改源码，下方失败为历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-18实际001五条为24/26.5/26.5/26.5/26.5，002第三条仍26.5，运行时CanSelectDevice为false但选择框仍可操作；235项228通过，见[结果](./tasks/evidence/TASK-036/review-01/results.json)。
- 根因：CurrentDevice.NextSampleIndex =+ 1每次赋正数1，没有累加旧值；DeviceComboBox缺IsEnabled绑定，现有条件和通知尚未连接到该控件，不推测学员误写动机。
- 修复线索：先将位置写成旧值加1再赋回，再给选择框IsEnabled绑定CanSelectDevice；保留现有设备存储/通知与运行方法保护。
- 复查点：两台各自从24开始，001五条完整循环、002第三条25，清空只复位当前设备，运行选择框禁用/停止恢复，原数据隔离与导出保持。
- 掌握边界：设备历史隔离、选中绑定/对象切换/通知、独立位置存储、运行入口保护均有局部通过证据，不撤销原能力，教练此前编号错误已另行修正，不计本条学员错误。
## TASK-035：成功应用直接改字段，遗漏显示通知与报警刷新

- 2026-10-18最终复查：已自主改为属性赋值并立即刷新报警，182项全通过，有效显示/通知/停止与采中判断均正确，本条已修复；[最终结果](./tasks/evidence/TASK-035/review-final/results.json)，本轮源码未改，下方失败为历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-18应用20/30/26.5时实际有效数值改变且可保存，但有效阈值显示未变；已有26.5应用26、运行中应用30等未立即重判报警，182项173通过，见[实际结果](./tasks/evidence/TASK-035/review-01/results.json)。
- 根因：ApplyThreshold直接赋_alarmThreshold绕过AlarmThreshold.set里的CurrentThresholdText通知，同时未调用已有RefreshDisplay；保存有效数值、通知显示与业务重判是不同动作，不推测学员误写动机。
- 修复线索：成功路径改为给AlarmThreshold属性赋候选值，随后调用RefreshDisplay(AlarmThreshold)，保留既有输入检查和成功反馈。
- 复查点：20/30/26.5应用后显示立即正确并通知，同一26.5应用26/27立即超限/正常，采中变化也即时反映，历史与保存保护保持。
- 掌握边界：输入TwoWay、命令连接、原合法性验证、配置恢复属性等已验证局部证据保留；TASK-029的旧刷新问题仍为历史已修复，本条是此次迁移路径的实际观察，不撤销旧任务能力，本轮源码未代改。
## TASK-034：把正在采集直接作为允许清空的条件

- 2026-10-17最终复查：清空条件已自主取反修复，105项实际检查全通过，采集中拒绝/停止恢复/实际清空复位正确，本条已修复；[最终结果](./tasks/evidence/TASK-034/review-final/results.json)，本轮源码未改，下方失败为历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-17实际命令按钮检查，ClearCommand.CanExecute直接返回IsSampling，停止时按钮禁用且Execute返回，运行时按钮可用；93项86通过，7失败均对应条件方向，见[实际结果](./tasks/evidence/TASK-034/review-01/results.json)。
- 根因：状态true表示正在采集，而本题清空允许条件在正在采集时应为false，当前返回状态本身而非停止条件；不推测学员误写动机。
- 修复线索：仅把CanExecute返回值改为IsSampling == false或!IsSampling，保留状态同步、通知与内部运行保护。
- 复查点：启动后清空不可执行/按钮禁用，停止后恢复，实际清空Count0/暂无温度/空图且下一条24，重复启停不重复追加。
- 掌握边界：追加命令、同对象连接、状态同步与条件通知已通过，ClearHistory内部运行保护仍正确，原TASK-012/027已验证能力不因此撤销，本轮未代改核心代码。
## TASK-031：CSV温度转换未固定小数点格式

- 2026-10-17最终复查：固定格式已自主修复，默认/de-DE均导出26.5，84项全通过，本条已修复；[最终结果](./tasks/evidence/TASK-031/review-final/results.json)，本轮源码未代改，下方失败为历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-17实际窗口在de-DE区域导出五条，第二条为2,26,5而非2,26.5，84项83通过，见[结果](./tasks/evidence/TASK-031/review-01/results.json)与[实际CSV](./tasks/evidence/TASK-031/review-01/de-DE.csv)。
- 根因：当前Convert.ToString(_temperatures[i])使用区域格式，小数逗号与本题CSV分隔逗号冲突，不推测学员误写动机。
- 修复线索：仅给温度ToString传入System.Globalization.CultureInfo.InvariantCulture，保留原集合、遍历、保护与写入。
- 复查点：默认和de-DE区域均为表头加五条、每条两列、26.5小数点不变，原保护/覆盖/异常恢复保持。
- 掌握边界：完整历史和序号/文件写入等已验证局部能力保留，固定格式待验证，教练本轮未代改代码。
## TASK-030：反序列化成功后没有检查对象与配置值

- 2026-10-17最终复查：null分支和候选值先验证后赋值均已自主修复，null/{}/35/19.9保留27且原文件不变，合法26可恢复，147项实际检查全通过；本轮未代改源码，见[最终结果](./tasks/evidence/TASK-030/review-final/results.json)，本条已修复，下方为历次问题记录。

- 2026-10-17再次复查：对象null保护已自主修复并通过，范围外有说明，但赋值先于检查，0/35/19.9仍留在有效字段中；当前修复线索更新为“条件检查配置对象的候选值，通过后才赋给_alarmThreshold”，return不回滚先前赋值。147项144通过、3失败，见[最新结果](./tasks/evidence/TASK-030/review-03/results.json)，保留既有局部能力，状态仍🟡 进行中，下方为首次缺失保护的历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-17独立实际窗口审查，配置null使LoadThresholdSettings读取Threshold时抛NullReferenceException，{}得到0，35/19.9成为有效阈值并显示加载完成，见[审查结果](./tasks/evidence/TASK-030/review-02/results.json)。
- 根因（当前代码控制关系）：Deserialize成功后直接读取属性并赋给_alarmThreshold，没有检查对象是否存在和数值是否符合20到30；格式合法、对象存在、业务数值合法是三次不同判断，不推测学员误写动机。
- 修复线索：在第一次访问Threshold前处理null并return，再校验Threshold，全部通过后才同步窗口；声明可空变量只是表达可能为空，不代替运行判断。
- 复查点：null/空对象/35/19.9均说明无效、保留27且原文件不变，随后合法26恢复，原保存和采集保持。
- 协助边界：教练只改读取失败提示的显示目标，原序列化/读写和同步实现保留，核心校验未代改。


## TASK-029：保存阈值后没有重新判断当前温度

- 2026-10-17最终复查：本条已自主修复，成功保存后调用UpdateTemperatureAlert；同一26.5切换26/27立即超限/正常，运行中也立即更新，98项实际检查全通过，见[最终结果](./tasks/evidence/TASK-029/review-final/results.json)，下方缺陷描述为首次历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-17实际窗口最新26.5，应用26后字段与有效阈值文字已是26，AlarmText仍温度正常；自动下一条到来后才按新阈值超限，见[审查结果](./tasks/evidence/TASK-029/review-02/results.json)。
- 根因（当前代码控制关系）：成功路径调用UpdateThresholdDisplay但未调用UpdateTemperatureAlert，两者分别更新阈值文字和温度判断；只确认调用缺失，不推测学员误写动机。
- 修复线索：在成功保存后调用已有UpdateTemperatureAlert，不追加或清空温度记录，不重写原采集逻辑。
- 复查点：同一条26.5应用26立即超限、应用27立即正常，历史内容和条数不变，采集中应用也立即更新，原非法输入保护继续保持。
- 协助边界：教练仅收尾反馈控件赋值与成功文案，本条核心调用未代改；NaN边界为此前未讲内容的必要补充，不另列典型错误。


## TASK-027：清空按钮可用条件与采集状态写反

- 状态：✅ 已通关。
- 2026-10-14第一次复查：本条清空条件错误已自主修复，运行禁用/停止可点、正常停止按钮清空/重启24及清空方法运行保护都通过，临时按钮已隐藏；此处通关仅指本条错误，TASK-027整体因状态文字相反仍🟡 进行中，见[复查证据](./tasks/evidence/TASK-027/recheck-01/README.md)，下方为首次失败历史。
- 情况与证据：2026-10-14实际窗口中，启动后ClearButton.IsEnabled=True且能清掉历史，停止后False使正常按钮入口无法清空；自动五条、停止等待与继续样例本身正确，见[首次审查](./tasks/evidence/TASK-027/review-01/README.md)。
- 根因（可确认的控制关系）：UpdateSamplingControls在定时器运行分支启用清空按钮、停止分支禁用，与“采集中禁止清空、停止后允许清空”的要求相反；这里只确认代码条件，不猜测学员的误写动机。
- 修复线索：先调整两个分支中的ClearButton.IsEnabled赋值，False表示清空按钮不能点，True表示可以点；随后在ClearButton_Click增加运行状态保护并隐藏完成观察用途的临时按钮，本轮未代改学员源码。
- 复查点：启动后清空变灰且历史保留，停止后清空恢复可点，实际清空使Count/表格/图归零，再启动首条24；临时入口不再造成定时器与状态文字不同步，DoD2/3仍待验证。

## TASK-027：状态文字与定时器实际运行相反

- 2026-10-14最终复查：用户要求此类状态文案由教练直接修正，教练已收尾，实际运行/停止文字正确、29项全部通过，TASK-027三个DoD通关；本条已修复但不计学员自主修正，避免再以文字收尾要求反复提交，见[最终证据](./tasks/evidence/TASK-027/recheck-02/README.md)，下方为前次失败历史。

- 状态：✅ 已通关。
- 情况与证据：2026-10-14第一次复查，定时器运行且五条历史持续新增时文字显示“定时任务以取消”，停止后文字显示“真正执行定时任务”；清空条件、运行保护和临时按钮隐藏已正确，见[复查结果](./tasks/evidence/TASK-027/recheck-01/results.json)。
- 根因（当前赋值关系）：UpdateSamplingControls中运行if分支设置停止文字，停止else分支设置运行文字；只确认提交代码，不推测为何交换文字。
- 修复线索：仅调整两个SamplingStateText.Text赋值，使运行时显示采集中、停止时显示已停止，保留已正确的清空按钮条件与定时器调用。
- 复查点：启动/停止文字分别与定时器运行状态一致，其他已通过DoD保持；本轮未代改源码。


## TASK-025：完整历史计数读取了循环样例下标

- 状态：✅ 已通关
- 2026-10-14 最终复查：学员已自主修正为读取_temperatures.Count，连续添加第四/五条计数4/5，Clear为0，再读为1；图表/标签/单点与历史保留均正常。9个实际状态/17项技术检查全通过，三个DoD通过；55项源码哈希未变，教练未代改。证据见[最新结果](./tasks/evidence/TASK-025/recheck-01/results.json)与[核对](./tasks/evidence/TASK-025/recheck-01/verification.json)。本条问题已修复，下方失败/待复查是首次审查历史。
- 情况与证据：2026-10-14实际按钮追加第四条后，原集合Count=4、界面“完整历史条数：0”；第五条Count=5、界面显示1。最新三点和标签正确，见[运行结果](./tasks/evidence/TASK-025/results.json)和[第四条图](./tasks/evidence/TASK-025/add-4.png)。
- 根因（可确认的数据用途）：当前计数文字读取_nextSampleIndex，该字段指定下一条循环样例位置，在四条样例末尾重置0；它不是温度集合中的元素数量。不推断误写动机，不否定已有集合/事件应用能力。
- 修复线索：仅让ReadingCountText.Text读取_temperatures.Count，保留样例循环和绘图代码；由学员修正，本轮未代改。
- 复查点：连续追加第四/五条显示4/5，集合历史完整保留，折线仍只画最新三条；Clear显示0且没有旧点/线/数据标签，再添加显示1和24度点。DoD1/2尚未通过，3已实际通过。

## TASK-022：历史区被纵向 StackPanel 包裹，未取得剩余高度

- 状态：✅ 已通关
- 2026-10-13 最终复查：学员已自主移除外层容器、调整直接子区域顺序并启用填满，17个实际场景全部通过，三个必需DoD已通过。窗口640→840时表格369.2267→569.2267，上下区稳定，恢复及最小尺寸/原业务正确；50个源码哈希未变，教练未代改。见[最新结果](./tasks/evidence/TASK-022/results.json)与[核对](./tasks/evidence/TASK-022/verification.json)。本条问题已修复，下方待复查/失败为对应历史。
- 2026-10-13 后续源码观察：学员已移除 ShowHistory，直接子区域改为选择/清空/历史，LastChildFill=True；本轮仅答疑，未重新运行验证，因此本条仍待复查。下方情况是修改前首次审查证据，未撤销既有业务能力。
- 情况与证据：2026-10-13 首次审查时窗口新增 ShowHistory StackPanel 包裹历史 Grid；主 DockPanel 直接子区域为 SelectionRegion、ShowHistory、ActionsRegion，LastChildFill=False。实际窗口高 640→840，表格仍 43.48；4 个布局场景未通过，13 个业务场景通过，见[首次结果](./tasks/evidence/TASK-022/first-review/results.json)及[当时源码快照](./tasks/evidence/TASK-022/first-review/Task022Window.xaml.txt)。
- 根因（确认的布局机制）：纵向 StackPanel 使内部历史 Grid 按内容高度测量；主 DockPanel 没有把剩余空间分给历史 Grid，移除表格自身固定 Height 不足以让它随窗口增长。未推断学员为何新增容器，未将此局部问题扩大为原 Binding/业务能力不足。
- 修复线索：先仅移除 ShowHistory 外层两标签，保留完整历史 Grid；随后让历史 Grid 成为主 DockPanel 最后一个直接子区域并启用 LastChildFill。详见[布局 Q2](./questions/wpf-layout.md)，源码由学员修正。
- 复查点：实际显示/拉高/恢复窗口，历史表格随高度增长且顶部/底部内容高度稳定；最小尺寸可读，旧行/原集合/命令连接继续正常。不要只改 LastChildFill 而把余下空间给当前最后的清空区。

## TASK-016：行内数值绑定路径与 Reading 属性名不对应

- 状态：✅ 已通关
- 情况与证据：2026-10-12 审查 [ReadingHistoryControl.xaml](../WpfBae/Exercises/TASK-016-reading-datatemplate/ReadingHistoryControl.xaml) 的第二个 TextBlock 绑定 FallbackValue；当前原编译窗口中每行对应 Reading.Value 为 26.5，但此绑定状态 PathError、显示空字符串。DeviceId/Unit 正常；见 [实际结果](./tasks/evidence/TASK-016/results.json)。
- 根因（可确认的机制）：Binding 路径在每行 Reading 上按属性名查找，Reading 没有 FallbackValue，导致查找失败。误写来自输入失误还是概念混淆未确定，不推断学员动机或将其已有绑定能力归零。
- 修复线索：对照 Reading 的公开属性核对数值路径；先让实际数值出现，再检查 ID 与数值间距。源码由学员修正。
- 复查点：数值控件 Binding 路径为 Value、状态 Active，实际文字为 26.5；三项分别从对应 Reading 读取，换 ID 保留旧行，间距可读，原业务行为保持。编译成功不代替实际行显示验证。
- 2026-10-12 同轮自主修复：学员改为 Value，重新构建与原编译窗口运行后绑定 Active、实际显示 26.5，各字段数据源均为对应 Reading，10 个业务场景通过。这里只标记数值路径错误已修复；任务仍进行中，DoD 2 的 ID/数值间距尚未通过，未把它算作修复后的可读性证据。修复前结果另存 results-before-value-fix.json，最新结果见原链接。
- 2026-10-12 最终复查：学员已补 ID 右边距，实际 ID/数值/单位显示正确，两段间距均为 12，10 个场景显示/业务全通过，TASK-016 三个 DoD 全通过。本条保持已修复，未新增认知错误，不将此前局部错误扩大为整体绑定能力不足。

## TASK-013：空白校验只显示提示，未结束方法

- 状态：✅ 已通关
- 情况与证据：2026-10-12 首次审查 [ReadTemperatureCommand.Execute](../WpfBae/Exercises/TASK-013-reading-viewmodel/ReadTemperatureCommand.cs) 的 `if (id.Length == 0)` 仅设置不合法提示，随后仍执行 Add 和成功提示。实际窗口输入空字符串使历史从 3 条变 4 条，纯空格使 4 条变 5 条，新增空 ID 读数，最终状态“读取成功”；见 [首次运行结果](./tasks/evidence/TASK-013/results-before-fix.json)。
- 根因：条件分支结束后方法仍按顺序执行后面的语句；提示只修改文字，不会阻止读取动作。
- 修复线索：无效输入分支提示后提前结束 Execute，使它不能继续进入成功路径。
- 复查点：空字符串及纯空格均保留原历史、最终显示不合法提示，不触发 Add；有效读取和清空恢复不受影响。
- 自主修复与验证：学员在审查期间补上 `return;`，教练未改源码；重新构建 0 警告/错误，10 个窗口场景全通过，两种空白均保留原 3 条历史，提示“该 Id 不合法”。见 [最终结果](./tasks/evidence/TASK-013/results.json)及[源码哈希核对](./tasks/evidence/TASK-013/verification.json)。本条表示这次错误已修复，不推定全部控制流概念已掌握。

## TASK-006：返回 Task 的方法未标 async，却直接使用 await

- 状态：✅ 已通关
- 情况与证据：2026-10-05 学员询问异步文件写入为何报错。[SaveAsync](../Base/Exercises/TASK-006-json-config/Task006Exercise.cs) 声明为 `private static Task SaveAsync(...)`，方法体使用 `await File.WriteAllTextAsync(path, configJson)` 并 `return writeAllTextAsync`；实际 `dotnet build Base/Base.csproj --no-restore` 退出码 1，报 CS4032 与 CS0103。
- 根因：混淆返回类型 Task 与允许方法体使用 await 的 async 修饰符；同时残留了返回未定义任务变量的语句。
- 修复线索：给 SaveAsync 添加 async，保留 Task 返回类型；async Task 没有结果值，移除带表达式的 return，自然结束即可。保存成功日志放在 await 写入之后。
- 复查点：学员自主修正后项目编译通过，保存成功时实际文件存在；能在后续实践区分 Task 返回类型、async 修饰符与 await 等待。尚未验证修复，不代改源码。
- 2026-10-05 后续复查：学员自主移除 SaveAsync 内部 await，定义写入 Task 变量后直接返回，调用方仍 await SaveAsync；实际构建成功（退出码 0，0 警告、0 错误）。CS4032 / CS0103 已修复，采用直接返回 Task 的合法方案，未强制改为 async；文件保存及完整应用证据仍待验证，本条复查状态保持 🟡 进行中。
- 2026-10-05 最终局部复查：学员随后自主改为 async Task SaveAsync 并 await 文件写入，调用方 await 后输出保存成功；实际 save 退出码 0，生成合法配置文件。原 async/返回变量问题已修复，本条 ✅ 已通关 仅指该错误复查；TASK-006 整体仍为 🟡 进行中，漏等待另记下条。

## TASK-006：调用 Task.Delay 后未等待，误以为外层 await 会等待它

- 状态：✅ 已通关
- 情况与证据：2026-10-05 审查 [ReadAsync](../Base/Exercises/TASK-006-json-config/Task006Exercise.cs) 第 99 行，`Task.Delay(config.ReadDelayMs)` 未 await；实际配置 3500 ms 时 load 进程 62 ms 结束，50 ms 对照 60 ms，重新构建报 CS4014。
- 根因：延迟任务被创建后未加入读取方法的完成流程；方法立即返回读数。调用方 await ReadAsync 不能自动等待方法内部没有跟踪的任务。
- 修复线索：在 ReadAsync 内等待配置决定的延迟任务，完成等待后才构造/返回读数。
- 复查点：学员自主修复后，以明显不同的等待参数观察真实读取耗时变化，CS4014 消失；仅打印配置值不算等待生效。
- 2026-10-05 最终复查：学员自主改为 await Task.Delay(config.ReadDelayMs)，CS4014 消失；3500 ms 配置运行 3565 ms，50 ms 对照 120 ms，设备 ID 与参数均输出正确。该漏等待错误已修复。

## TASK-006：两个配置依次写入同一文件，后一个覆盖前一个

- 状态：✅ 已通关
- 情况与证据：2026-10-05 save 分支两次调用 SaveAsync 使用同一 configPath；实际运行后合法 JSON 只保留设备 002、200 ms，设备 001、100 ms 被覆盖。
- 根因：WriteAllTextAsync 是整体写入文本，不能用连续写入同一路径表达“保存两条配置”。
- 修复线索：当前任务按单配置流程选一个样例保存即可；如果以后需要多配置，需明确集合或各自文件的存储设计，不在本次默认扩展范围。
- 复查点：当前流程的保存意图与实际文件内容一致。此问题不否定第一项 DoD 的单配置保存证据，但避免把运行成功误认为两条配置均被保留。
- 2026-10-05 最终复查：学员自主删除第二次保存，当前只保存设备 002、200 ms；实际生成内容与单配置意图一致，重复覆盖问题已修复。

## TASK-002：事件已触发，但设备 ID 未正确从设备传入

- 状态：✅ 已通关
- 情况与证据：2026-09-27 审查 [`Task002Exercise.cs`](../Base/Exercises/TASK-002-device-reading-event/Task002Exercise.cs)，`ProduceReading` 创建 `Reading` 时只赋 `Value` 与 `Unit`；处理器也只输出这两项。实际运行输出 `eReading: 5 ℃`、`eReading: 6 ℃`，无设备 ID。
- 根因：`Reading.DeviceId` 未从发布事件的设备传入读数对象，处理器无法从事件参数取得可核对的设备 ID。
- 修复线索：构造读数时利用 `SimulatedDevice` 已有的 `Id`；处理器从 `e.Reading` 输出相应属性。
- 复查点：两次事件输出均包含设备 `001` 的 ID、数值与单位；取消订阅后的第三次读数仍不产生处理器输出。
- 2026-09-27 第二次复查：处理器现已输出 `DeviceId`，两次读数均显示 `001`；但 `ProduceReading` 将 `DeviceId` 写死为 `"001"`，没有使用设备实例的 `Id`。当前样例掩盖了设备身份绑定错误；下一次复查需确认 ID 来源正确，换用其他设备 ID 时仍能如实传递。
- 2026-09-27 最终复查：`ProduceReading` 已改用设备实例的 `Id` 构造 `Reading.DeviceId`；运行输出包含两次设备 `001` 的读数，第三次在取消订阅后无处理器输出。此错误已修复。

## TASK-005：把触发条件当作取消信号源

- 状态：✅ 已通关
- 情况与证据：2026-10-02 提交说明中写“当 cancelEarly 为 true 时，cancelEarly 发出信号”，路径仅描述为 Cancel；[实际代码](../Base/Exercises/TASK-005-cancel-reading/Task005Exercise.cs) 使用 `if (cancelEarly) { cts.Cancel(); }`，实现本身正确并已运行验证。
- 根因：说明中混淆了 bool 触发条件与 CancellationTokenSource 发请求的职责；信号源、令牌与响应操作的角色尚未区分清楚。
- 修复线索：对照条件分支，区分“何时取消”（cancelEarly）、“谁请求取消”（cts.Cancel）、“操作如何收到请求”（同一取消源的 Token 已传到 ReadAsync 里的 Task.Delay）。
- 复查点：在原实践提交中正确说明取消源与令牌传递路径；无需另写练习或单独口试。异常导致成功输出跳过及 catch 捕获的说明已正确，不列为错误。
- 2026-10-02：用户要求协助修正；教练已在[任务卡](./tasks/TASK-005.md)提供准确说明。文本已修正，但尚无学员自主应用角色区分的新证据，错误记录保持进行中；后续结合实践复核，不要求重复抄写或单独口试。
- 2026-10-05：TASK-005 已按用户明确要求完成任务级验收并通关；本条的 🟡 进行中 指概念复查状态。暂无角色区分自主理解的新证据，概念复查继续保留，不因关联任务通关自动标为已掌握或已修复。
- 2026-10-05 TASK-007 自主应用复查：学员代码用 cancelEarly 判断是否停止、cts.CancelAfter(100) 调度请求，cts.Token 经 AcquireAsync 和 ReadAsync 传给 Task.Delay；1000 ms 配置实际取消，无成功读数。角色区分已有自主代码及运行证据，本条错误复查标为 ✅ 已通关；TASK-007 整体仍进行中，取消异常处理另记下条，CancellationToken Q1 的其他复习内容不随之全部通关。

## TASK-007：调用方捕获取消并提示后再次抛出，导致未捕获异常退出

- 状态：✅ 已通关
- 情况与证据：2026-10-05 [Task007Exercise.cs](../Base/Exercises/TASK-007-config-acquisition/Task007Exercise.cs) 第 93～96 行，RunScenarioAsync 捕获 OperationCanceledException、提示后再次 throw。1000 ms 配置的 cancel 场景无成功读数，但输出 Unhandled exception，退出码 -532462766。
- 根因：捕获和打印不会消除后续 throw 的传播；RunAsync 与 Program 都没有后续捕获。中间循环需要向上传递取消以停止后续轮次，而负责结束本场景的调用方应处理预期取消，二者边界不同。
- 修复线索：保留采集循环向上传递异常，让 RunScenarioAsync 的预期取消分支处理后安全结束；不要把循环内的 throw 一并删除而继续循环。默认留给学员局部修复。
- 复查点：取消时有提示、无未捕获异常、无取消后成功读数、退出码 0；using 的作用域仍覆盖采集任务结束。
- 2026-10-05 同轮复查：用户自行移除 RunScenarioAsync catch 的 throw，保留采集循环的重抛；重新构建并执行 1000 ms 配置的 cancel，场景 110 ms、进程 171 ms，取消提示完整、无成功读数/未捕获异常、退出码 0。本条错误已修复，TASK-007 整体仍因短配置取消时机为 🟡 进行中。

## TASK-007：取消调度延迟写死，短等待配置下取消前已完成采集

- 状态：✅ 已通关
- 情况与证据：2026-10-05 调用方固定 CancelAfter(100)，而 ReadDelayMs 可配置为任意正数；实际改为 10 ms 后 cancel 输出三条成功读数、场景 45 ms 返回，没有取消提示，退出码 0。
- 根因：停止倒计时未随被取消操作的实际等待变化；“调度了取消”不保证请求发生在操作完成前。
- 修复线索：取消场景按加载的等待时间选择较短的停止延迟，确保正在等待时收到请求；正常场景仍不调度取消。
- 复查点：1000 ms 与较短正数配置的 cancel 均能在读取等待期间停止，无被取消读取及后续轮次的成功读数；正常模式仍完成三次。
- 2026-10-05 最终复查：学员自主改为 CancelAfter(config.ReadDelayMs)，1000 ms 两次、10 ms 六次、1 ms 六次均有取消提示、退出码 0，无取消后成功读数；正常仍三次完成。相等计时下允许请求前先完成一至两条读数，本任务 DoD 未要求必须取消第一轮。固定延迟失配问题已修复，本条 ✅ 已通关；相等延迟仍有竞争，不认定取消第一轮或精确计时已保证。
