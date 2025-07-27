功能说明

1. 为了完成需求多建表Sosnotification，StaffLocation，Rooms，StaffSchedule
2. 员工基本信息管理
   GET api/StaffInfo - 获取所有员工信息
   GET api/StaffInfo/{id} - 获取特定员工信息
   POST api/StaffInfo - 添加新员工
   PUT api/StaffInfo/{id} - 更新员工信息
   DELETE api/StaffInfo/{id} - 删除员工
2. 护理计划智能排班 (1.1.4)
   GET api/StaffInfo/NursingPlans/Unassigned - 获取所有未分配的护理计划
   POST api/StaffInfo/NursingPlans/AutoSchedule - 自动排班所有未分配的护理计划
   PUT api/StaffInfo/NursingPlans/Assign/{nursingPlanId} - 手动分配员工到特定护理计划
3. 紧急SOS呼叫处理 (1.1.12)
   GET api/StaffInfo/EmergencySOS/Pending - 获取所有待处理的SOS呼叫
   POST api/StaffInfo/EmergencySOS - 创建新的SOS呼叫记录
   PUT api/StaffInfo/EmergencySOS/Assign/{callId} - 分配响应人员到SOS呼叫
   PUT api/StaffInfo/EmergencySOS/Complete/{callId} - 标记SOS呼叫为已完成
4. 消毒记录管理 (1.1.18)
   GET api/StaffInfo/DisinfectionRecords - 获取消毒记录(可选日期范围)
   POST api/StaffInfo/DisinfectionRecords - 记录新的消毒工作
   GET api/StaffInfo/DisinfectionReports/{year}/{month} - 生成月度消毒报告
5. 护理计划智能排班改动说明：
   ​​优先级排序​​：
   在获取未分配护理计划时，先按优先级排序，再按开始时间排序，确保重要和紧急的任务优先分配
   ​​智能员工选择​​：
   考虑员工当前工作负荷，避免过度分配
   实现工作负荷均衡，避免某些员工过度工作而其他员工闲置
   ​​技能匹配优化​​：
   精确匹配技能要求（如"急救护理"需要完全匹配）
   为特殊护理类型（如术后护理、康复护理）添加专门处理
   ​​后备策略​​：
   当没有完全匹配的员工时，尝试放宽技能要求（如急救护理可降级为基础护理）
   ​​员工选择算法​​：
   优先选择技能完全匹配的员工
   其次考虑技能部分匹配的员工
   最后考虑员工资历（入职时间）
   ​​时间段冲突检查​​：
   确保员工在同一时间段不会被分配多个任务
6. SOS主要改动点： ​​
   护理人员筛选​​：
   现在会检查护理人员的排班情况，只选择当前在岗的人员
   加入了位置信息，优先选择距离呼叫房间最近的护理人员
   使用简单的距离算法（同楼层按房间号差值，不同楼层按楼层差）
​   通知机制​​：
   只通知最近的3名护理人员，避免过多打扰
   记录通知发送情况，便于后续跟踪
   ​​响应处理​​：
   在AssignResponderToSosAsync中应该检查通知记录，确保只有被通知的护理人员可以响应
   ​​数据模型​​：
   添加了SosNotification实体来跟踪通知发送情况
   在StaffInfoDTO中添加了CurrentRoomId和Distance属性
7. 消毒部分按照文档做的
8.项目构建的时候是StaffInfo文件夹和StaffInfo.sln，在StaffInfo文件夹里面是branch里面的除了StaffInfo.sln的内容，并且bin文件下的Debug下的net8这样，不知道为啥git上合并过去了，obj就没事
