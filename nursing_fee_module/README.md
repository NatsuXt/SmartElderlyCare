# 智慧养老系统 - 护理计划与费用管理模块

## 模块概述

本模块是智慧养老系统的核心业务模块，负责护理服务的执行与相关费用的自动结算。主要功能包括护理计划管理、费用明细计算、账单自动生成以及支付状态跟踪，为养老机构提供完整的护理服务与财务管理解决方案。

## 技术架构

- **开发语言**: C# .NET 8.0
- **数据库**: Oracle 18c
- **ORM**：Entity Framework Core
- **架构模式**：MVC (Model-View-Controller)
- **依赖注入**：ASP.NET Core内置DI容器
- **后台任务**：IHostedService

## 文件结构说明

```
api/
├── Models/                     # 数据实体类
│   ├── NursingPlan.cs                      # 护理计划实体
│   ├── FeeSettlement.cs                    # 费用结算实体
│   ├── ElderlyInfo.cs                      # 老人信息实体
│   ├── StaffInfo.cs                        # 员工信息实体
│   └── Dtos/                               # 数据传输对象
├── Controllers/                # API控制器
│   ├── NursingPlanController.cs            # 护理计划API
│   └── FeeSettlementController.cs          # 费用结算API
├── Services/                   # 业务逻辑服务
│   ├── FeeCalculationService.cs            # 费用计算服务
│   └── BillGenerationBackgroundService.cs  # 账单生成后台服务
├── Migrations/                 # 数据库迁移文件
├── AppDbContext.cs             # 数据库上下文
├── Program.cs                  # 应用入口与服务配置
└── appsettings.json            # 应用配置文件
```

## 数据库设计

### 核心表结构

| 表名              | 功能描述       | 主要字段                                                                 |
| ----------------- | -------------- | ------------------------------------------------------------------------ |
| **NursingPlan**   | 护理计划表     | plan_id, elderly_id, staff_id, plan_start_date, plan_end_date, care_type |
| **FeeSettlement** | 费用结算表     | settlement_id, elderly_id, total_amount, payment_status, billing_cycle   |
| **ElderlyInfo**   | 老人基本信息表 | elderly_id, name, age, health_status, emergency_contact                  |
| **StaffInfo**     | 员工信息表     | staff_id, name, position, department, contact_info                       |
| **FeeDetail**     | 费用明细表     | detail_id, settlement_id, fee_type, amount, description                  |

### 数据库连接信息

```json
用户名：NURSINGAPP
```

## 核心业务逻辑

### 费用自动计算与账单生成流程

1. **周期触发**: 每月底由 BillGenerationBackgroundService 自动启动账单生成程序

2. **费用汇总**: FeeCalculationService 汇总以下费用项:
   - 护理服务费用: 根据 NursingPlan 计算
   - 药品费用: 从 MedicalOrders 表获取已领用药品记录
   - 住宿费用: 从 RoomManagement 表计算住宿天数与费用
   - 活动费用: 从 ActivitySchedule 表获取收费活动记录

3. **账单生成**: 系统在 FeeSettlement 表中生成总账单记录，并将 payment_status 设为"待支付"


## API接口说明

### 1. 护理计划API( `NursingPlanController` )

| 接口             | 方法   | 描述                   | URL                   |
| ---------------- | ------ | ---------------------- | --------------------- |
| 获取所有护理计划 | GET    | 获取系统中所有护理计划 | /api/NursingPlan      |
| 获取单个护理计划 | GET    | 根据ID获取护理计划     | /api/NursingPlan/{id} |
| 创建护理计划     | POST   | 创建新的护理计划       | /api/NursingPlan      |
| 更新护理计划     | PUT    | 更新指定护理计划       | /api/NursingPlan/{id} |
| 删除护理计划     | DELETE | 删除指定护理计划       | /api/NursingPlan/{id} |

### 2. 费用结算API( `FeeSettlementController` )

| 接口             | 方法   | 描述                 | URL                     |
| ---------------- | ------ | -------------------- | ----------------------- |
| 获取所有结算记录 | GET    | 获取所有费用结算记录 | /api/FeeSettlement      |
| 获取单个结算记录 | GET    | 根据ID获取结算记录   | /api/FeeSettlement/{id} |
| 创建结算记录     | POST   | 创建新的结算记录     | /api/FeeSettlement      |
| 更新结算记录     | PUT    | 更新指定结算记录     | /api/FeeSettlement/{id} |
| 删除结算记录     | DELETE | 删除指定结算记录     | /api/FeeSettlement/{id} |

### 3. 老人信息API( `ElderlyInfoController` )

| 接口             | 方法   | 描述                   | URL                   |
| ---------------- | ------ | ---------------------- | --------------------- |
| 获取所有老人信息 | GET    | 获取系统中所有老人信息 | /api/ElderlyInfo      |
| 获取单个老人信息 | GET    | 根据ID获取老人信息     | /api/ElderlyInfo/{id} |
| 创建老人信息     | POST   | 创建新的老人信息       | /api/ElderlyInfo      |
| 更新老人信息     | PUT    | 更新指定老人信息       | /api/ElderlyInfo/{id} |
| 删除老人信息     | DELETE | 删除指定老人信息       | /api/ElderlyInfo/{id} |

### 4. 员工信息API( `StaffInfoController` )

| 接口             | 方法   | 描述                   | URL                 |
| ---------------- | ------ | ---------------------- | ------------------- |
| 获取所有员工信息 | GET    | 获取系统中所有员工信息 | /api/StaffInfo      |
| 获取单个员工信息 | GET    | 根据ID获取员工信息     | /api/StaffInfo/{id} |
| 创建员工信息     | POST   | 创建新的员工信息       | /api/StaffInfo      |
| 更新员工信息     | PUT    | 更新指定员工信息       | /api/StaffInfo/{id} |
| 删除员工信息     | DELETE | 删除指定员工信息       | /api/StaffInfo/{id} |

### 5. 费用计算API( `FeeCalculationController` )

| 接口             | 方法 | 描述                               | URL                                                                       |
| ---------------- | ---- | ---------------------------------- | ------------------------------------------------------------------------- |
| 计算老人费用     | GET  | 计算指定老人在指定时间段内的总费用 | /api/FeeCalculation/calculate/{elderlyId}?startDate={date}&endDate={date} |
| 生成所有老人账单 | POST | 为指定时间段内的所有老人生成账单   | /api/FeeCalculation/generate-bills?startDate={date}&endDate={date}        |

### 6. 账单生成API( `BillGenerationController` )

| 接口         | 方法 | 描述                 | URL                          |
| ------------ | ---- | -------------------- | ---------------------------- |
| 手动生成账单 | POST | 手动触发账单生成流程 | /api/BillGeneration/generate |


## 后台服务

- 账单生成服务：系统使用 BillGenerationBackgroundService 定期生成账单，默认每月执行一次，生成上个月的费用账单。
