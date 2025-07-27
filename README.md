# 智慧养老系统 - 房间设备管理模块

## 模块概述

本模块是智慧养老系统的核心数据服务层，负责房间管理、设备监控和健康监测的数据处理与业务逻辑。采用C# .NET 8.0开发，基于Oracle 18c数据库，为上层应用提供完整的API接口。

## 技术架构

- **开发语言**: C# .NET 8.0
- **数据库**: Oracle 18c (47.96.238.102:1521/orcl)
- **架构模式**: 分层架构 + 依赖注入
- **连接方式**: Oracle.ManagedDataAccess.Core

## 文件结构说明

```
RoomDeviceManagement/
├── Models/                     # 数据实体类 (6个核心实体)
├── Interfaces/                 # 服务接口层 (3个主要接口)
├── Implementation/             # 服务实现层 (3个实现类)
├── Services/                   # 基础服务 (数据库连接)
├── Controllers/                # 控制器 (演示用)
├── SQL/                        # 数据库脚本 (建表+测试数据)
└── Program.cs                  # 演示程序入口
```

## 数据库设计

### 核心表结构

| 表名                   | 功能   | 主要字段             | 状态   |
| -------------------- | ---- | ---------------- | ---- |
| **RoomManagement**   | 房间管理 | 房间号、楼层、状态、类型、设施  | ✅ 完成 |
| **DeviceStatus**     | 设备状态 | 设备ID、房间、状态、电量、位置 | ✅ 完成 |
| **ElderlyInfo**      | 老人信息 | 姓名、年龄、健康状况、紧急联系人 | ✅ 完成 |
| **HealthMonitoring** | 健康监测 | 心率、血压、体温、血氧等生命体征 | ✅ 完成 |
| **ElectronicFence**  | 电子围栏 | 围栏配置、监控区域、时间规则   | ✅ 完成 |
| **FenceLog**         | 围栏日志 | 事件记录、报警处理、行为分析   | ✅ 完成 |

### 数据库连接信息

```
服务器: 47.96.238.102:1521/orcl
用户名: FIBRE
密码: FIBRE2025
```

## 接口服务说明

### 1. IRoomManagementService (房间管理)

```csharp
// 核心功能：房间信息的CRUD操作
GetAllRooms()                    // 获取所有房间
GetRoomById(int roomId)          // 获取指定房间
GetRoomsByStatus(string status)  // 按状态查询房间
GetRoomsByFloor(int floor)       // 按楼层查询房间
AddRoom(RoomManagement room)     // 添加房间
UpdateRoom(RoomManagement room)  // 更新房间
DeleteRoom(int roomId)           // 删除房间
GetAvailableRooms()             // 获取空闲房间
GetRoomStatistics()             // 房间统计信息
```

### 2. IDeviceStatusService (设备管理)

```csharp
// 核心功能：IoT设备状态监控与管理
GetAllDevices()                  // 获取所有设备
GetDeviceById(int deviceId)      // 获取指定设备
GetDevicesByRoom(int roomId)     // 按房间查询设备
GetDevicesByStatus(string status) // 按状态查询设备
AddDevice(DeviceStatus device)   // 添加设备
UpdateDevice(DeviceStatus device) // 更新设备
DeleteDevice(int deviceId)       // 删除设备
GetFaultyDevices()              // 获取故障设备
GetOfflineDevices()             // 获取离线设备
GetLowBatteryDevices()          // 获取低电量设备
UpdateDeviceOnlineStatus()       // 批量更新在线状态
GetDeviceStatistics()           // 设备统计信息
```

### 3. IHealthMonitoringService (健康监测)

```csharp
// 核心功能：健康数据采集与分析
GetAllHealthRecords()           // 获取所有健康记录
GetHealthRecordsByElderlyId()   // 按老人ID查询
GetHealthRecordsByDateRange()   // 按时间范围查询
AddHealthRecord()               // 添加健康记录
UpdateHealthRecord()            // 更新健康记录
DeleteHealthRecord()            // 删除健康记录
GetAbnormalHealthRecords()      // 获取异常记录
GetLatestHealthRecords()        // 获取最新记录
GetHealthStatistics()           // 健康统计分析
GetElderlyHealthSummary()       // 老人健康摘要
GetHealthTrends()               // 健康趋势分析
MarkAsProcessed()               // 标记为已处理
```

## 快速开始

### 1. 环境配置

```bash
# 安装.NET 8.0 SDK
# 确保Oracle 18c数据库可访问
# 克隆项目到本地
```

### 2. 初始化数据库

```sql
-- 按顺序执行SQL脚本
@SQL/Step2_CreateBaseTables.sql     -- 创建基础表
@SQL/Step3_CreateSequencesAndIndexes.sql -- 创建序列和索引
@SQL/Step4_CreateTriggers.sql       -- 创建触发器
@SQL/TestData_Fixed.sql             -- 插入测试数据
```

### 3. 运行项目

```bash
cd RoomDeviceManagement
dotnet build    # 编译项目
dotnet run      # 运行演示程序
```

### 4. 集成使用

```csharp
// 服务初始化
var dbService = new DatabaseService();
var roomService = new RoomManagementService(dbService);
var deviceService = new DeviceStatusService(dbService);
var healthService = new HealthMonitoringService(dbService);

// 业务调用示例
var rooms = roomService.GetAllRooms();
var devices = deviceService.GetFaultyDevices();
var healthData = healthService.GetAbnormalHealthRecords();
```

## 模块衔接指南

### 1. 依赖注入方式

```csharp
// 推荐在Startup.cs或Program.cs中注册服务
services.AddScoped<DatabaseService>();
services.AddScoped<IRoomManagementService, RoomManagementService>();
services.AddScoped<IDeviceStatusService, DeviceStatusService>();
services.AddScoped<IHealthMonitoringService, HealthMonitoringService>();
```

### 2. 错误处理

- 所有数据库操作都有异常处理
- 返回bool类型表示操作成功/失败
- 查询类方法返回null表示未找到数据

### 3. 数据状态说明

- **房间状态**: "空闲"、"已占用"、"维护中"、"清洁中"
- **设备状态**: "正常"、"故障"、"离线"、"维护"
- **健康状态**: "正常"、"异常"、"紧急"

### 4. 性能优化建议

- 使用分页查询处理大量数据
- 合理使用缓存减少数据库访问
- 定期清理历史日志数据

## 测试验证

运行演示程序验证功能：

```bash
dotnet run
```

程序会自动：

1. 测试数据库连接
2. 检查表结构完整性
3. 展示系统数据概览
4. 演示各接口功能
