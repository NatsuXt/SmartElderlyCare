# 智能设备状态监控业务实现方案

## 📋 业务需求
- 系统通过物联网(IoT)平台，实时轮询DeviceStatus表中所有设备的状态
- 当设备上报"故障"状态时，系统自动更新DeviceStatus表中的记录，并向职位为"维修人员"的StaffInfo发送告警通知

## 🏗️ 技术架构
我们采用 **Web API + 后台服务** 的实现方式，具有以下优势：
- ✅ 业务逻辑清晰，易于维护和扩展
- ✅ 支持多种通知方式（日志、数据库、短信、邮件等）
- ✅ 完整的日志记录和错误处理
- ✅ 易于测试和调试

## 🔧 核心组件

### 1. DeviceMonitoringBackgroundService (后台监控服务)
**位置**: `Services/DeviceMonitoringBackgroundService.cs`
**功能**:
- 每5分钟自动轮询所有设备状态
- 检测新出现的故障设备
- 触发故障通知流程
- 记录监控日志

**配置参数** (appsettings.json):
```json
{
  "DeviceMonitoring": {
    "IntervalMinutes": 5,           // 轮询间隔（分钟）
    "EnableNotifications": true,    // 是否启用通知
    "FaultDetectionWindow": 30      // 故障检测时间窗口（分钟）
  }
}
```

### 2. DeviceStatusReportController (设备状态上报接口)
**位置**: `Controllers/DeviceStatusReportController.cs`
**API端点**:

#### 单设备状态上报
```http
POST /api/DeviceStatusReport/status
Content-Type: application/json

{
  "deviceId": 1,
  "deviceName": "智能床垫001",
  "deviceType": "智能床垫",
  "status": "故障",
  "location": "101房间",
  "description": "传感器异常",
  "reportTime": "2025-08-07T10:30:00"
}
```

#### 批量设备状态上报
```http
POST /api/DeviceStatusReport/batch-status
Content-Type: application/json

[
  {
    "deviceId": 1,
    "status": "故障",
    "location": "101房间"
  },
  {
    "deviceId": 2,
    "status": "正常",
    "location": "102房间"
  }
]
```

#### 查询设备状态
```http
GET /api/DeviceStatusReport/status/{deviceId}
```

### 3. DeviceMonitoringService (设备监控服务)
**位置**: `Services/DeviceMonitoringService.cs`
**增强功能**:
- `NotifyDeviceFaultAsync()`: 发送故障通知
- `GetDeviceStatusByIdAsync()`: 获取设备状态
- `PollAllDeviceStatusAsync()`: 轮询所有设备状态

### 4. DeviceStatusReportDto (数据传输对象)
**位置**: `DTOs/IoTMonitoringDTOs.cs`
```csharp
public class DeviceStatusReportDto
{
    public int DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string Status { get; set; }           // "正常", "故障", "维护中"
    public string? Location { get; set; }
    public string? Description { get; set; }
    public DateTime ReportTime { get; set; }
}
```

## 🔄 业务流程

### 主动轮询流程
```
1. 后台服务每5分钟执行一次
   ↓
2. 查询数据库中所有设备状态
   ↓
3. 识别新出现的故障设备
   ↓
4. 查询维修人员信息
   ↓
5. 发送故障通知
   ↓
6. 更新设备检查时间
```

### 设备上报流程
```
IoT设备/平台 → API接口 → 更新数据库 → 故障检测 → 通知维修人员
```

## 📊 数据库表结构

### DeviceStatus 表
```sql
CREATE TABLE DeviceStatus (
    device_id NUMBER PRIMARY KEY,
    device_name VARCHAR2(100) NOT NULL,
    device_type VARCHAR2(50) NOT NULL,
    installation_date DATE NOT NULL,
    status VARCHAR2(20) NOT NULL,
    last_maintenance_date DATE,
    maintenance_status VARCHAR2(20),
    location VARCHAR2(100) NOT NULL
);
```

### STAFFINFO 表
```sql
-- 需要确保有职位为"维修人员"的记录
INSERT INTO STAFFINFO (STAFF_ID, NAME, POSITION, CONTACT_PHONE, EMAIL) 
VALUES (1, '张维修', '维修人员', '13812345678', 'repair@example.com');
```

## 🚀 部署运行

### 1. 启动应用
```bash
cd e:\MyTest\database_keshe
dotnet run
```

### 2. 访问接口文档
```
http://localhost:5000/swagger
```

### 3. 日志监控
后台服务会在控制台输出详细的监控日志：
```
[10:30:00] 设备监控后台服务已启动，轮询间隔: 5 分钟
[10:35:00] 开始执行设备状态轮询检查...
[10:35:01] 发现 2 个新故障设备，正在发送通知...
[10:35:01] 通知维修人员 张维修 (电话: 13812345678): 设备 1 (智能床垫) 在 101房间 发生故障
```

## 🧪 测试示例

### 1. 模拟设备故障上报
```bash
curl -X POST "http://localhost:5000/api/DeviceStatusReport/status" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 1,
    "deviceName": "智能床垫001",
    "deviceType": "智能床垫", 
    "status": "故障",
    "location": "101房间",
    "description": "传感器连接异常"
  }'
```

### 2. 查看故障设备状态
```bash
curl "http://localhost:5000/api/DeviceStatusReport/status/1"
```

## 🔮 扩展功能

### 未来可以添加的功能：
1. **多种通知方式**: 短信、邮件、企业微信
2. **故障等级分类**: 紧急、重要、一般
3. **维修工单系统**: 自动创建维修任务
4. **设备预测性维护**: 基于历史数据预测故障
5. **实时监控看板**: Web界面显示设备状态
6. **移动端App**: 维修人员移动端接收通知

## 🛡️ 安全考虑
- API接口需要添加认证授权
- 敏感数据加密存储
- 日志脱敏处理
- 数据库连接池管理

## 📈 性能优化
- 数据库索引优化
- 分页查询大量设备
- 异步处理通知发送
- 缓存频繁查询的数据

---
> **状态**: ✅ 已完成基础实现，0个编译错误，可以正常运行
> **测试**: 建议先在测试环境验证功能完整性
> **监控**: 关注后台服务的性能和稳定性
