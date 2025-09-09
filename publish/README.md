# 智慧养老系统 - 房间与设备管理模块

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Oracle](https://img.shields.io/badge/Oracle-18c-red.svg)](https://www.oracle.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 项目概述

智慧养老系统是一个基于 ASP.NET Core 8.0 和 Oracle 数据库的综合性养老院管理平台，提供房间管理、设备监控、健康监测、电子围栏和IoT设备管理等核心功能。

## 技术架构

- **后端框架**: ASP.NET Core 8.0 Web API
- **数据库**: Oracle 18c (47.96.238.102:1521/orcl)
- **API服务**: http://localhost:5000
- **API文档**: http://localhost:5000/swagger
- **ORM**: Oracle.ManagedDataAccess
- **API文档**: Swagger/OpenAPI
- **架构模式**: MVC + Service Layer + Repository Pattern

## 核心功能模块

### 1. 🏠 房间管理模块
- 房间信息CRUD操作
- 房间容量和状态管理
- 房间占用率统计
- 支持分页查询和搜索

### 2. 📱 设备管理模块
- 智能设备注册和配置
- 设备状态实时监控
- 设备维护记录管理
- 设备故障检测和警报

### 3. 💓 健康监测模块
- IoT健康设备数据采集
- 心率、血压、血氧、体温监测
- 健康异常检测和警报
- 健康数据统计和趋势分析

### 4. 🔒 电子围栏模块
- GPS位置实时追踪
- 安全区域配置管理
- 越界警报和通知
- 老人活动轨迹记录

### 5. 🌐 IoT监控模块
- 设备状态轮询检测
- 故障设备自动发现
- 实时警报管理
- 设备同步和数据上报

## API接口文档

### 设备管理API (`/api/DeviceManagement`)

| 方法 | 端点 | 描述 | 参数 |
|------|------|------|------|
| GET | `/devices` | 获取设备列表 | page, pageSize, search, sortBy, sortDesc |
| GET | `/{id}` | 获取设备详情 | id |
| POST | `/` | 创建新设备 | DeviceCreateDto |
| PUT | `/{id}` | 更新设备信息 | id, DeviceUpdateDto |
| DELETE | `/{id}` | 删除设备 | id |
| GET | `/statistics` | 获取设备统计 | - |

### 房间管理API (`/api/RoomManagement`)

| 方法 | 端点 | 描述 | 参数 |
|------|------|------|------|
| GET | `/` | 获取房间列表 | page, pageSize, search, sortBy, sortDesc |
| GET | `/{id}` | 获取房间详情 | id |
| POST | `/` | 创建新房间 | RoomCreateDto |
| PUT | `/{id}` | 更新房间信息 | id, RoomUpdateDto |
| DELETE | `/{id}` | 删除房间 | id |
| GET | `/statistics` | 获取房间统计 | - |

### 健康监测API (`/api/health-monitoring`)

| 方法 | 端点 | 描述 | 参数 |
|------|------|------|------|
| POST | `/data-report` | IoT健康数据上报 | HealthDataReportDto |
| POST | `/batch-data-report` | 批量健康数据上报 | List&lt;HealthDataReportDto&gt; |
| GET | `/elderly/{elderlyId}/history` | 获取健康历史数据 | elderlyId, days |
| GET | `/statistics` | 获取健康数据统计 | elderlyId (可选) |
| GET | `/elderly/{elderlyId}/latest` | 获取最新健康数据 | elderlyId |

### 电子围栏API (`/api/ElectronicFence`)

| 方法 | 端点 | 描述 | 参数 |
|------|------|------|------|
| POST | `/gps-report` | GPS位置上报 | GpsLocationReportDto |
| GET | `/logs` | 获取围栏日志 | elderlyId, startDate, endDate |
| GET | `/current-status` | 获取当前围栏状态 | - |
| GET | `/config` | 获取围栏配置 | - |
| GET | `/elderly/{elderlyId}/trajectory` | 获取老人轨迹 | elderlyId, hours |
| GET | `/alerts` | 获取围栏警报 | activeOnly |
| POST | `/config` | 创建围栏配置 | ElectronicFenceCreateDto |
| DELETE | `/config/{fenceId}` | 删除围栏配置 | fenceId |
| GET | `/staff-locations` | 获取工作人员位置 | - |
| POST | `/staff-location` | 上报工作人员位置 | StaffLocationUpdateDto |
| POST | `/test-fence` | 测试围栏功能 | FenceTestDto |

### IoT监控API (`/api/IoTMonitoring`)

| 方法 | 端点 | 描述 | 参数 |
|------|------|------|------|
| GET | `/devices/poll-status` | 轮询设备状态 | - |
| POST | `/devices/fault-report` | 设备故障上报 | DeviceFaultReportDto |
| GET | `/alerts` | 获取活跃警报 | - |
| GET | `/devices/{deviceId}/status` | 获取单个设备状态 | deviceId |
| POST | `/devices/sync` | 同步设备数据 | DeviceSyncRequestDto |

## 数据库结构

### 核心数据表

- **DeviceStatus** - 设备状态表
- **RoomManagement** - 房间管理表
- **HealthMonitoring** - 健康监测表
- **ElectronicFence** - 电子围栏配置表
- **FenceLog** - 围栏日志表
- **ElderlyInfo** - 老人信息表
- **StaffInfo** - 工作人员信息表
- **StaffLocation** - 工作人员位置表

## 快速开始

### 环境要求

- .NET 8.0 SDK
- Oracle Client 18c+
- Visual Studio 2022 或 VS Code

### 安装部署

1. **克隆项目**
```bash
git clone https://github.com/NatsuXt/SmartElderlyCare.git
cd SmartElderlyCare
```

2. **配置数据库连接**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=your_password;"
  }
}
```

3. **安装依赖包**
```bash
dotnet restore
```

4. **运行项目**
```bash
dotnet run
```

5. **访问API文档**
```
http://47.96.238.102:5000/swagger
```

### 数据库测试

```bash
# 测试数据库连接
dotnet run --test-db

# 调试数据库连接
dotnet run --debug-db
```

## 开发指南

### 项目结构

```
RoomDeviceManagement/
├── Controllers/          # API控制器
├── Services/             # 业务逻辑服务
├── Models/              # 数据模型
├── DTOs/                # 数据传输对象
├── Interfaces/          # 接口定义
├── Program.cs           # 应用程序入口
└── appsettings.json     # 配置文件
```

### 开发规范

1. **命名规范**: 使用PascalCase命名控制器、服务和模型
2. **API设计**: 遵循RESTful API设计规范
3. **错误处理**: 统一使用ApiResponse包装返回结果
4. **日志记录**: 使用ILogger记录关键操作和异常
5. **参数验证**: 使用Data Annotations进行输入验证

### 后台服务

系统包含设备监控后台服务，每5分钟自动轮询检查设备状态：

```csharp
services.AddHostedService<DeviceMonitoringBackgroundService>();
```

## API使用示例

### 获取设备列表

```bash
curl -X GET "http://localhost:5000/api/DeviceManagement/devices?page=1&pageSize=10" \
-H "accept: application/json"
```

### 上报健康数据

```bash
curl -X POST "http://localhost:5000/api/HealthMonitoring/report" \
-H "Content-Type: application/json" \
-d '{
  "elderlyId": 1,
  "heartRate": "75",
  "bloodPressure": "120/80",
  "oxygenLevel": "98",
  "temperature": 36.5,
  "measurementTime": "2025-08-07T22:30:00"
}'
```

### GPS位置上报

```bash
curl -X POST "http://localhost:5000/api/ElectronicFence/gps-report" \
-H "Content-Type: application/json" \
-d '{
  "elderlyId": 1,
  "latitude": 39.9042,
  "longitude": 116.4074,
  "timestamp": "2025-08-07T22:30:00",
  "accuracy": 5.0
}'
```

## 监控与维护

### 系统监控

- **设备状态监控**: 自动检测设备故障和离线状态
- **健康数据监控**: 实时监测老人健康指标异常
- **围栏状态监控**: 监控老人位置和越界情况
- **系统日志监控**: 记录关键操作和异常信息

### 性能优化

- 数据库连接池管理
- 分页查询优化
- 后台任务异步处理
- 缓存策略应用

## 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 许可证

该项目基于 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 联系方式

- 项目作者: NatsuXt
- 项目链接: [https://github.com/NatsuXt/SmartElderlyCare](https://github.com/NatsuXt/SmartElderlyCare)
- 技术支持: 请在GitHub Issues中提出问题

---

**智慧养老系统** - 让科技守护每一位老人的健康与安全 ❤️

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
