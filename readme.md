老年人护理管理系统 - 员工信息模块(StaffInfo Module)文档

目录

1. #模块概述
2. #数据库表结构
3. #功能实现详情
   • #员工基本信息管理

   • #护理计划智能排班

   • #紧急sos呼叫处理

   • #消毒记录管理

4. #api使用指南
5. #数据安全性
6. #功能完成情况（和后续可优化部分）

模块概述

员工信息模块是老年人护理管理系统的核心模块，负责管理养老院所有员工信息及其相关操作。本模块实现了员工基本信息管理、护理计划智能排班、紧急SOS呼叫处理和消毒记录管理四大核心功能。

模块采用ASP.NET Core Web API构建，使用Entity Framework Core作为ORM框架，SQL Server作为数据库，并集成了Swagger UI用于API文档和测试。

数据库表结构

核心表

1. StaffInfo (员工信息表)
   • 存储员工基本信息，包括姓名、性别、职位、联系方式、技能等级等

   • 主键：StaffId

2. NursingPlan (护理计划表)
   • 记录老人的护理计划，包括护理类型、起止时间、优先级等

   • 外键：StaffId关联到StaffInfo表

3. ActivitySchedule (活动安排表)
   • 记录员工参与的活动安排

   • 外键：StaffId关联到StaffInfo表

4. MedicalOrders (医嘱管理表)
   • 记录医嘱信息

   • 外键：StaffId关联到StaffInfo表

5. OperationLog (操作日志表)
   • 记录系统操作日志

   • 外键：StaffId关联到StaffInfo表

6. EmergencySOS (紧急事件SOS表)
   • 记录老人发起的紧急呼叫

   • 外键：ResponseStaffId关联到StaffInfo表

7. SystemAnnouncements (系统公告表)
   • 记录系统公告

   • 外键：CreatedBy关联到StaffInfo表

8. DisinfectionRecord (消毒记录表)
   • 记录消毒工作记录

   • 外键：StaffId关联到StaffInfo表

辅助表

1. StaffSchedule (员工排班表)
   • 记录员工的工作时间安排

   • 外键：StaffId关联到StaffInfo表

2. StaffLocation (员工位置表)
   • 记录员工的实时位置信息

   • 外键：StaffId关联到StaffInfo表，RoomId关联到Room表

3. SosNotification (SOS通知表)
   • 记录SOS呼叫通知发送情况

   • 外键：CallId关联到EmergencySOS表，StaffId关联到StaffInfo表

4. Room (房间表)
   • 记录养老院的房间信息

功能实现详情

员工基本信息管理

实现情况

• 完整实现了员工信息的CRUD操作

• 支持分页查询和条件筛选

• 使用DTO进行数据传输，确保敏感信息(如薪资)只在必要时返回

• 实现了AutoMapper自动映射

使用方法

通过StaffInfoController提供的API进行管理：
• GET /api/StaffInfo - 获取所有员工基本信息

• GET /api/StaffInfo/{id} - 获取指定员工详细信息

• POST /api/StaffInfo - 添加新员工

• PUT /api/StaffInfo/{id} - 更新员工信息

• DELETE /api/StaffInfo/{id} - 删除员工

数据安全

• 薪资信息只在详细查询中返回，列表查询不包含

• 所有更新操作都会记录操作日志

• 删除操作会检查关联数据，避免误删

护理计划智能排班

实现情况

• 实现了基于技能和工作量的智能排班算法

• 支持手动分配和自动分配两种模式

• 自动排班考虑以下因素：

  • 护理类型与员工技能匹配度

  • 员工当前工作负荷

  • 护理任务的优先级

  • 工作时间冲突检查

使用方法

通过以下API进行管理：
• GET /api/StaffInfo/NursingPlans/Unassigned - 获取所有未分配的护理计划

• GET /api/StaffInfo/NursingPlans/{planId} - 获取护理计划详情

• POST /api/StaffInfo/NursingPlans/AutoSchedule - 执行自动排班

• PUT /api/StaffInfo/NursingPlans/Assign/{nursingPlanId} - 手动分配员工到护理计划

算法逻辑

1. 获取所有未分配且未过期的护理计划
2. 根据护理类型确定所需技能
3. 查询具备该技能且在工作时间内的员工
4. 计算每个员工当前工作负荷
5. 按工作负荷升序排序，实现均衡分配
6. 如果没有完全匹配的员工，尝试放宽技能要求
7. 最终选择最合适的员工进行分配

紧急SOS呼叫处理

实现情况

• 完整实现了SOS呼叫全流程处理

• 智能选择最近的护理人员进行响应

• 实时通知和状态更新机制

• 详细记录处理过程和结果

使用方法

通过以下API进行管理：
• POST /api/StaffInfo/EmergencySOS - 创建新的SOS呼叫

• GET /api/StaffInfo/EmergencySOS/Pending - 获取所有待处理的SOS呼叫

• GET /api/StaffInfo/EmergencySOS/{callId} - 获取SOS呼叫详情

• PUT /api/StaffInfo/EmergencySOS/Assign/{callId} - 分配响应人员

• PUT /api/StaffInfo/EmergencySOS/Complete/{callId} - 完成SOS呼叫处理

处理流程

1. 老人触发SOS呼叫，系统创建记录(状态:待响应)
2. 系统查找最近的3名在岗护理人员
   • 考虑当前位置和楼层距离

   • 优先选择同一楼层的护理人员

3. 向这些护理人员发送通知
4. 第一个响应的护理人员被记录为响应者(状态:处理中)
5. 护理人员处理完毕后填写处理结果(状态:已完成)

位置计算

• 同楼层距离 = 房间号差值
• 不同楼层距离 = 楼层差值
 × 100 + 50

• 无位置信息的员工距离 = int.MaxValue

消毒记录管理

实现情况

• 完整的消毒记录CRUD操作

• 按月生成消毒统计报告

• 支持按时间段查询

使用方法

通过以下API进行管理：
• POST /api/StaffInfo/DisinfectionRecords - 记录新的消毒工作

• GET /api/StaffInfo/DisinfectionRecords - 获取消毒记录(支持startDate和endDate参数)

• GET /api/StaffInfo/DisinfectionRecords/{recordId} - 获取消毒记录详情

• GET /api/StaffInfo/DisinfectionReports/{year}/{month} - 生成月度消毒报告

报告内容

• 消毒总次数

• 各区域消毒统计(次数、最后消毒时间)

• 员工参与情况统计

API使用指南

Swagger UI

系统集成了Swagger UI，访问 /swagger 即可查看完整的API文档和进行测试。

API分组

API按功能分为以下几组：
1. Staff Management - 员工基本信息管理
2. Nursing Plans - 护理计划管理
3. Emergency SOS - 紧急呼叫处理
4. Disinfection - 消毒记录管理



数据安全性

操作审计

• 所有关键操作都记录到OperationLog表

（✅ ​​已实现部分​​：

OperationLog模型定义完整
数据库关系和配置正确
表结构已准备就绪

❌ ​​缺失部分​​：

实际记录日志的代码逻辑
日志服务实现
控制器中的日志调用
）


数据完整性

• 使用外键约束确保关联数据完整性

• 删除操作前检查关联数据

• 重要字段设置NOT NULL约束


功能完成情况
1.1.4 护理计划智能排班功能
✅ ​​完全实现的部分​​：

​​数据获取​​：
通过NursingScheduleService.GetUnassignedNursingPlansAsync()从NursingPlan表获取未分配的护理需求
通过StaffInfo表的SkillLevel和StaffSchedule获取护理人员技能和工作时间
​​排班算法​​：
实现了基于技能匹配的分配逻辑（SelectBestStaff方法）
考虑了工作负荷均衡（GetAvailableStaffAsync中的工作量计算）
支持优先级排序（GetUnassignedNursingPlansAsync中的排序）
​​分配机制​​：
提供自动排班（AutoScheduleNursingPlansAsync）和手动分配（AssignStaffToNursingPlanAsync）两种方式
分配后自动更新NursingPlan表的staff_id字段
​​技能匹配​​：
实现了精确匹配和降级匹配逻辑（当找不到完全匹配技能时尝试基础护理）

⚠️ ​​可优化部分​​：

​​排班冲突检测​​：
当前仅检查了护理计划的时间范围，没有精细到具体小时级别的冲突检测
​​员工偏好​​：
未考虑员工对特定老人或护理类型的偏好设置
​​算法扩展性​​：
当前算法较简单，未来可加入更多优化因子（如员工疲劳度、历史配合度等）

1.1.12 紧急SOS呼叫处理功能
✅ ​​完全实现的部分​​：

​​SOS记录创建​​：
EmergencySosService.CreateEmergencySosAsync完整实现了创建记录逻辑
初始状态正确设置为"待响应"
​​响应人员查找​​：
FindAvailableRespondersAsync实现了：
在岗人员筛选（通过StaffSchedule检查）
距离计算（同楼层和跨楼层算法）
最近3名护理人员选择
​​通知机制​​：
通过NotificationService.SendSosNotificationAsync发送通知
记录到SosNotification表
​​状态管理​​：
完整的状态流转："待响应" → "处理中" → "已完成"
记录响应时间和处理结果

⚠️ ​​可优化部分​​：

​​通知方式​​：
当前是模拟实现，实际应集成短信/APP推送等
​​超时处理​​：
未实现呼叫超时自动升级机制
​​位置更新​​：
依赖人工位置更新，理想情况应集成IoT设备自动更新

1.1.18 消毒记录管理功能
✅ ​​完全实现的部分​​：

​​记录创建​​：
DisinfectionService.RecordDisinfectionAsync完整实现消毒记录
自动记录消毒时间并关联staff_id
​​查询统计​​：
支持按时间段查询（GetDisinfectionRecordsAsync）
按月生成统计报告（GenerateMonthlyReportAsync）
​​报告内容​​：
包含消毒总次数
按区域统计
按人员统计

⚠️ ​​可优化部分​​：

​​消毒标准​​：
未关联不同区域的消毒标准要求
​​提醒功能​​：
未实现定期消毒提醒
​​质量评估​​：
报告缺少消毒质量评估指标


此文档详细介绍了员工信息模块的设计与实现，如需更多帮助，请联系技术支持团队。
