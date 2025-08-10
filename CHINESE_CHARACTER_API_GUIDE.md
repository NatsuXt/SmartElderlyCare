# 智慧养老系统 API 中文字符支持指南

## 概述
本系统已完全支持中文字符的创建、读取、更新、删除操作。所有API端点都能正确处理中文字符，确保数据的完整性和准确性。

## 核心修复内容

### 1. 数据库连接优化
- ✅ 连接字符串添加 `Unicode=True` 支持
- ✅ Oracle环境变量正确设置：
  - `NLS_LANG=SIMPLIFIED CHINESE_CHINA.AL32UTF8`
  - `ORA_NCHAR_LITERAL_REPLACE=TRUE`

### 2. 参数类型强化
- ✅ 所有字符串参数使用 `OracleDbType.NVarchar2`
- ✅ 明确指定参数大小，避免截断
- ✅ 非空值验证，防止null引用异常

### 3. JSON序列化配置
```csharp
options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
```

## API模块中文字符支持状态

### 🏠 房间管理模块 - 完全支持
| 端点 | 方法 | 中文字段 | 状态 |
|------|------|----------|------|
| `/api/RoomManagement/rooms` | GET | 房间类型、状态、床型 | ✅ |
| `/api/RoomManagement/rooms` | POST | 房间号、房间类型、状态、床型 | ✅ |
| `/api/RoomManagement/rooms/{id}` | GET | 全部字段 | ✅ |
| `/api/RoomManagement/rooms/{id}` | PUT | 全部字段 | ✅ |
| `/api/RoomManagement/rooms/{id}` | DELETE | - | ✅ |
| `/api/RoomManagement/statistics` | GET | 统计信息 | ✅ |

### 📱 设备管理模块 - 完全支持
| 端点 | 方法 | 中文字段 | 状态 |
|------|------|----------|------|
| `/api/DeviceManagement/devices` | GET | 设备名称、类型、位置、状态 | ✅ |
| `/api/DeviceManagement/devices` | POST | 设备名称、类型、位置、状态、描述 | ✅ |
| `/api/DeviceManagement/devices/{id}` | GET | 全部字段 | ✅ |
| `/api/DeviceManagement/devices/{id}` | PUT | 全部字段 | ✅ |
| `/api/DeviceManagement/devices/{id}` | DELETE | - | ✅ |
| `/api/DeviceManagement/status` | GET | 状态统计 | ✅ |

### 💓 健康监测模块 - 完全支持
| 端点 | 方法 | 中文字段 | 状态 |
|------|------|----------|------|
| `/api/HealthMonitoring/elderly` | GET | 姓名、联系方式、病史 | ✅ |
| `/api/HealthMonitoring/elderly` | POST | 姓名、联系方式、紧急联系人、病史 | ✅ |

### 🔒 电子围栏模块 - 完全支持
| 端点 | 方法 | 中文字段 | 状态 |
|------|------|----------|------|
| `/api/ElectronicFence/fences` | GET | 围栏名称、区域定义、状态 | ✅ |
| `/api/ElectronicFence/fences` | POST | 围栏名称、区域定义、状态、描述 | ✅ |

## 正确的API调用示例

### 创建房间（中文字符）
```http
POST /api/RoomManagement/rooms
Content-Type: application/json; charset=utf-8

{
    "roomNumber": "豪华套房-001",
    "roomType": "豪华套房",
    "capacity": 2,
    "status": "空闲",
    "rate": 388.50,
    "bedType": "双人大床",
    "floor": 3
}
```

### 创建设备（中文字符）
```http
POST /api/DeviceManagement/devices
Content-Type: application/json; charset=utf-8

{
    "deviceName": "智能血压监测仪",
    "deviceType": "医疗监测设备",
    "installationDate": "2025-08-10T00:00:00",
    "status": "正常运行",
    "location": "二楼护士站",
    "description": "专业医疗级血压监测设备",
    "lastMaintenanceDate": "2025-08-03T00:00:00"
}
```

### 创建老人信息（中文字符）
```http
POST /api/HealthMonitoring/elderly
Content-Type: application/json; charset=utf-8

{
    "name": "张三",
    "age": 75,
    "gender": "男",
    "contactInfo": "139****8888",
    "emergencyContact": "李女士 138****7777",
    "medicalHistory": "高血压、糖尿病等慢性疾病",
    "roomId": 1
}
```

## 测试工具

### 1. 完整API测试
```bash
dotnet run test-api-chinese
```
运行完整的中文字符兼容性测试套件。

### 2. 中文兼容服务测试
```bash
dotnet run test-chinese
```
测试底层中文兼容数据库服务。

### 3. 诊断工具
```bash
dotnet run diagnose
```
运行中文字符诊断工具。

## 重要提醒

### ✅ 必须遵循的原则
1. **Content-Type**: 始终使用 `application/json; charset=utf-8`
2. **DTO格式**: 严格按照定义的DTO格式传递数据
3. **字段验证**: 确保必填字段不为空
4. **字符长度**: 遵守字段长度限制（如房间号≤20字符）

### ❌ 常见错误
1. 使用错误的字段名（不符合DTO定义）
2. 缺少必填字段
3. Content-Type不正确
4. 字符编码问题

## 架构说明

### ChineseCompatibleDatabaseService
所有业务服务都使用此服务确保中文字符支持：
- `RoomManagementService`
- `DeviceManagementService` 
- `HealthMonitoringService`
- `ElectronicFenceService`

### 连接字符串配置
```csharp
private const string ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;Connection Timeout=30;Pooling=false;Unicode=True;";
```

### 参数配置示例
```csharp
var roomNumberParam = new OracleParameter(":roomNumber", OracleDbType.NVarchar2, 100) { Value = roomNumber };
var roomTypeParam = new OracleParameter(":roomType", OracleDbType.NVarchar2, 100) { Value = roomType };
```

## 维护说明

1. **新增API**: 确保使用 `ChineseCompatibleDatabaseService`
2. **参数处理**: 字符串参数使用 `OracleDbType.NVarchar2` 且指定大小
3. **测试**: 每次修改后运行 `dotnet run test-api-chinese` 验证
4. **监控**: 观察日志中的中文字符显示是否正常

---
*最后更新: 2025年8月10日*
*版本: 1.0 - 完整中文字符支持*
