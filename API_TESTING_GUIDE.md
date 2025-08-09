# 智慧养老系统API完整测试指南

## 📋 测试准备

### 环境要求
- VS Code 终端 (PowerShell)
- 服务运行在 http://localhost:5000
- .NET 8.0 项目正常启动

### 测试前检查
1. 确认服务正在运行：
```powershell
# 检查服务状态
Invoke-RestMethod -Uri "http://localhost:5000/swagger" -Method GET
```

2. 如果服务未启动，请运行：
```powershell
dotnet run
```

---

## 🏠 1. 房间管理API测试 (6个接口)

### 1.1 创建房间 (POST /api/RoomManagement/rooms)
**测试目的：** 创建新房间记录

**PowerShell命令：**
```powershell
$body = @{
    RoomNumber = "TEST102"
    RoomType = "标准间"
    Capacity = 2
    Status = "空闲"
    Rate = 200.00
    BedType = "单人床"
    Floor = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/RoomManagement/rooms" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回创建的房间信息，包含房间ID
**成功标志：** HTTP 201 Created + 房间数据JSON

---

### 1.2 获取房间列表 (GET /api/RoomManagement/rooms)
**测试目的：** 获取分页房间列表

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/RoomManagement/rooms?page=1&pageSize=10" -Method GET
```

**期望结果：** 返回房间列表数组
**成功标志：** HTTP 200 OK + 房间列表JSON

---

### 1.3 获取房间详情 (GET /api/RoomManagement/rooms/{id})
**测试目的：** 根据ID获取特定房间信息

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/RoomManagement/rooms/1" -Method GET
```

**期望结果：** 返回指定房间的详细信息
**成功标志：** HTTP 200 OK + 房间详情JSON

---

### 1.4 更新房间 (PUT /api/RoomManagement/rooms/{id})
**测试目的：** 更新房间信息

**PowerShell命令：**
```powershell
$body = @{
    RoomType = "豪华间"
    Status = "维护中"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/RoomManagement/rooms/1" -Method PUT -Body $body -ContentType "application/json"
```

**期望结果：** 返回更新后的房间信息
**成功标志：** HTTP 200 OK + 更新后的房间JSON

---

### 1.5 删除房间 (DELETE /api/RoomManagement/rooms/{id})
**测试目的：** 删除指定房间

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/RoomManagement/rooms/1" -Method DELETE
```

**期望结果：** 返回删除成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 1.6 房间统计 (GET /api/RoomManagement/rooms/statistics)
**测试目的：** 获取房间统计信息

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/RoomManagement/rooms/statistics" -Method GET
```

**期望结果：** 返回房间统计数据
**成功标志：** HTTP 200 OK + 统计数据JSON

---

## 📱 2. 设备管理API测试 (6个接口)

### 2.1 获取设备列表 (GET /api/DeviceManagement/devices)
**测试目的：** 获取分页设备列表

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/DeviceManagement/devices?page=1&pageSize=10" -Method GET
```

**期望结果：** 返回设备列表数组
**成功标志：** HTTP 200 OK + 设备列表JSON

---

### 2.2 获取设备详情 (GET /api/DeviceManagement/devices/{id})
**测试目的：** 根据ID获取特定设备信息

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/DeviceManagement/devices/1" -Method GET
```

**期望结果：** 返回指定设备的详细信息
**成功标志：** HTTP 200 OK + 设备详情JSON

---

### 2.3 创建设备 (POST /api/DeviceManagement/devices)
**测试目的：** 创建新设备记录

**PowerShell命令：**
```powershell
$body = @{
    DeviceName = "测试设备001"
    DeviceType = "智能传感器"
    Status = "正常"
    Location = "测试房间"
    InstallationDate = "2025-08-09T00:00:00"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/DeviceManagement/devices" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回创建的设备信息，包含设备ID
**成功标志：** HTTP 201 Created + 设备数据JSON

---

### 2.4 更新设备 (PUT /api/DeviceManagement/devices/{id})
**测试目的：** 更新设备信息

**PowerShell命令：**
```powershell
$body = @{
    Status = "维护中"
    Location = "新测试位置"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/DeviceManagement/devices/1" -Method PUT -Body $body -ContentType "application/json"
```

**期望结果：** 返回更新后的设备信息
**成功标志：** HTTP 200 OK + 更新后的设备JSON

---

### 2.5 删除设备 (DELETE /api/DeviceManagement/devices/{id})
**测试目的：** 删除指定设备

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/DeviceManagement/devices/1" -Method DELETE
```

**期望结果：** 返回删除成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 2.6 设备统计 (GET /api/DeviceManagement/devices/statistics)
**测试目的：** 获取设备统计信息

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/DeviceManagement/devices/statistics" -Method GET
```

**期望结果：** 返回设备统计数据
**成功标志：** HTTP 200 OK + 统计数据JSON

---

## 💊 3. 健康监测API测试 (5个接口)

### 3.1 健康数据上报 (POST /api/HealthMonitoring/data-report)
**测试目的：** 上报单条健康数据

**PowerShell命令：**
```powershell
$body = @{
    ElderlyId = 1
    HeartRate = 75
    BloodPressure = "120/80"
    OxygenLevel = 98.5
    Temperature = 36.5
    MeasurementTime = "2025-08-09T12:00:00"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/HealthMonitoring/data-report" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回上报成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 3.2 批量健康数据上报 (POST /api/HealthMonitoring/batch-report)
**测试目的：** 批量上报多条健康数据

**PowerShell命令：**
```powershell
$body = @(
    @{
        ElderlyId = 1
        HeartRate = 76
        Temperature = 36.6
    },
    @{
        ElderlyId = 2
        HeartRate = 72
        Temperature = 36.4
    }
) | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/HealthMonitoring/batch-report" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回批量上报成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 3.3 获取健康历史 (GET /api/HealthMonitoring/history/{elderlyId})
**测试目的：** 获取老人的健康历史记录

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/HealthMonitoring/history/1?days=7" -Method GET
```

**期望结果：** 返回健康历史记录列表
**成功标志：** HTTP 200 OK + 健康记录数组JSON

---

### 3.4 获取最新健康数据 (GET /api/HealthMonitoring/latest/{elderlyId})
**测试目的：** 获取老人的最新健康数据

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/HealthMonitoring/latest/1" -Method GET
```

**期望结果：** 返回最新健康数据
**成功标志：** HTTP 200 OK + 最新健康数据JSON

---

### 3.5 健康统计 (GET /api/HealthMonitoring/statistics)
**测试目的：** 获取健康数据统计

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/HealthMonitoring/statistics" -Method GET
```

**期望结果：** 返回健康统计数据
**成功标志：** HTTP 200 OK + 统计数据JSON

---

## 🗺️ 4. 电子围栏API测试 (11个接口)

### 4.1 GPS位置上报 (POST /api/ElectronicFence/gps-report)
**测试目的：** 上报GPS位置信息

**PowerShell命令：**
```powershell
$body = @{
    ElderlyId = 1
    Latitude = 39.9042
    Longitude = 116.4074
    LocationTime = "2025-08-09T12:00:00"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/gps-report" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回位置上报成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 4.2 获取围栏日志 (GET /api/ElectronicFence/logs)
**测试目的：** 获取围栏进出日志

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/logs?elderlyId=1" -Method GET
```

**期望结果：** 返回围栏日志列表
**成功标志：** HTTP 200 OK + 日志数组JSON

---

### 4.3 获取当前围栏状态 (GET /api/ElectronicFence/current-status)
**测试目的：** 获取当前围栏状态

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/current-status" -Method GET
```

**期望结果：** 返回当前围栏状态
**成功标志：** HTTP 200 OK + 状态数据JSON

---

### 4.4 获取围栏配置 (GET /api/ElectronicFence/config)
**测试目的：** 获取围栏配置信息

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/config" -Method GET
```

**期望结果：** 返回围栏配置列表
**成功标志：** HTTP 200 OK + 配置数组JSON

---

### 4.5 创建围栏配置 (POST /api/ElectronicFence/config)
**测试目的：** 创建新的围栏配置

**PowerShell命令：**
```powershell
$body = @{
    FenceName = "测试围栏001"
    AreaDefinition = "39.9042,116.4074;39.9052,116.4084;39.9032,116.4064"
    FenceType = "Polygon"
    Status = "Enabled"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/config" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回创建的围栏配置
**成功标志：** HTTP 201 Created + 围栏配置JSON

---

### 4.6 获取老人轨迹 (GET /api/ElectronicFence/elderly/{elderlyId}/trajectory)
**测试目的：** 获取老人的活动轨迹

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/elderly/1/trajectory?hours=24" -Method GET
```

**期望结果：** 返回老人轨迹数据
**成功标志：** HTTP 200 OK + 轨迹数组JSON

---

### 4.7 获取围栏警报 (GET /api/ElectronicFence/alerts)
**测试目的：** 获取围栏警报信息

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/alerts?activeOnly=true" -Method GET
```

**期望结果：** 返回警报列表
**成功标志：** HTTP 200 OK + 警报数组JSON

---

### 4.8 删除围栏配置 (DELETE /api/ElectronicFence/config/{fenceId})
**测试目的：** 删除指定围栏配置

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/config/1" -Method DELETE
```

**期望结果：** 返回删除成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 4.9 获取工作人员位置 (GET /api/ElectronicFence/staff-locations)
**测试目的：** 获取工作人员位置信息

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/staff-locations" -Method GET
```

**期望结果：** 返回工作人员位置列表
**成功标志：** HTTP 200 OK + 位置数组JSON

---

### 4.10 更新工作人员位置 (POST /api/ElectronicFence/staff-location)
**测试目的：** 更新工作人员位置

**PowerShell命令：**
```powershell
$body = @{
    StaffId = 1
    Floor = 2
    UpdateTime = "2025-08-09T12:00:00"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/staff-location" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回位置更新成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 4.11 测试围栏功能 (POST /api/ElectronicFence/test-fence)
**测试目的：** 测试围栏检查功能

**PowerShell命令：**
```powershell
$body = @{
    Latitude = 39.9042
    Longitude = 116.4074
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/ElectronicFence/test-fence" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回围栏测试结果
**成功标志：** HTTP 200 OK + 测试结果JSON

---

## 🌐 5. IoT监控API测试 (5个接口)

### 5.1 设备状态轮询 (POST /api/IoTMonitoring/devices/poll-status)
**测试目的：** 轮询设备状态

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/IoTMonitoring/devices/poll-status" -Method POST
```

**期望结果：** 返回设备状态轮询结果
**成功标志：** HTTP 200 OK + 轮询结果JSON

---

### 5.2 设备故障上报 (POST /api/IoTMonitoring/devices/fault-report)
**测试目的：** 上报设备故障

**PowerShell命令：**
```powershell
$body = @{
    DeviceId = 1
    DeviceType = "智能传感器"
    FaultStatus = "故障"
    FaultDescription = "设备连接超时"
    ReportTime = "2025-08-09T12:00:00"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/IoTMonitoring/devices/fault-report" -Method POST -Body $body -ContentType "application/json"
```

**期望结果：** 返回故障上报成功消息
**成功标志：** HTTP 200 OK + 成功消息

---

### 5.3 获取警报列表 (GET /api/IoTMonitoring/alerts)
**测试目的：** 获取当前活跃警报

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/IoTMonitoring/alerts" -Method GET
```

**期望结果：** 返回警报列表
**成功标志：** HTTP 200 OK + 警报数组JSON

---

### 5.4 获取设备状态 (GET /api/IoTMonitoring/devices/{deviceId}/status)
**测试目的：** 获取单个设备状态

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/IoTMonitoring/devices/1/status" -Method GET
```

**期望结果：** 返回设备状态信息
**成功标志：** HTTP 200 OK + 设备状态JSON

---

### 5.5 设备数据同步 (POST /api/IoTMonitoring/devices/sync)
**测试目的：** 同步设备数据

**PowerShell命令：**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/IoTMonitoring/devices/sync" -Method POST
```

**期望结果：** 返回同步结果
**成功标志：** HTTP 200 OK + 同步结果JSON

---

## 📊 测试结果记录表

请在测试每个接口后，在下表中记录结果：

| 模块 | 接口编号 | 接口名称 | 状态 | HTTP状态码 | 备注 |
|------|----------|----------|------|------------|------|
| 房间管理 | 1.1 | 创建房间 | ✅/❌ | | |
| 房间管理 | 1.2 | 获取房间列表 | ✅/❌ | | |
| 房间管理 | 1.3 | 获取房间详情 | ✅/❌ | | |
| 房间管理 | 1.4 | 更新房间 | ✅/❌ | | |
| 房间管理 | 1.5 | 删除房间 | ✅/❌ | | |
| 房间管理 | 1.6 | 房间统计 | ✅/❌ | | |
| 设备管理 | 2.1 | 获取设备列表 | ✅/❌ | | |
| 设备管理 | 2.2 | 获取设备详情 | ✅/❌ | | |
| 设备管理 | 2.3 | 创建设备 | ✅/❌ | | |
| 设备管理 | 2.4 | 更新设备 | ✅/❌ | | |
| 设备管理 | 2.5 | 删除设备 | ✅/❌ | | |
| 设备管理 | 2.6 | 设备统计 | ✅/❌ | | |
| 健康监测 | 3.1 | 健康数据上报 | ✅/❌ | | |
| 健康监测 | 3.2 | 批量上报 | ✅/❌ | | |
| 健康监测 | 3.3 | 获取健康历史 | ✅/❌ | | |
| 健康监测 | 3.4 | 获取最新数据 | ✅/❌ | | |
| 健康监测 | 3.5 | 健康统计 | ✅/❌ | | |
| 电子围栏 | 4.1 | GPS位置上报 | ✅/❌ | | |
| 电子围栏 | 4.2 | 围栏日志 | ✅/❌ | | |
| 电子围栏 | 4.3 | 当前状态 | ✅/❌ | | |
| 电子围栏 | 4.4 | 围栏配置 | ✅/❌ | | |
| 电子围栏 | 4.5 | 创建配置 | ✅/❌ | | |
| 电子围栏 | 4.6 | 老人轨迹 | ✅/❌ | | |
| 电子围栏 | 4.7 | 围栏警报 | ✅/❌ | | |
| 电子围栏 | 4.8 | 删除配置 | ✅/❌ | | |
| 电子围栏 | 4.9 | 工作人员位置 | ✅/❌ | | |
| 电子围栏 | 4.10 | 更新位置 | ✅/❌ | | |
| 电子围栏 | 4.11 | 测试围栏 | ✅/❌ | | |
| IoT监控 | 5.1 | 状态轮询 | ✅/❌ | | |
| IoT监控 | 5.2 | 故障上报 | ✅/❌ | | |
| IoT监控 | 5.3 | 获取警报 | ✅/❌ | | |
| IoT监控 | 5.4 | 设备状态 | ✅/❌ | | |
| IoT监控 | 5.5 | 数据同步 | ✅/❌ | | |

## 🔍 故障排查指南

### 常见错误及解决方案

**400 Bad Request**
- 检查JSON格式是否正确
- 检查必填字段是否提供
- 检查数据类型是否匹配

**404 Not Found**
- 检查URL路径是否正确
- 检查ID是否存在
- 确认服务是否正在运行

**500 Internal Server Error**
- 检查数据库连接
- 查看服务日志
- 检查数据库表结构

### 测试提示

1. **按顺序测试：** 先测试GET接口，再测试POST接口
2. **记录ID：** 创建操作成功后，记录返回的ID用于后续测试
3. **错误处理：** 遇到错误时，记录完整的错误信息
4. **数据清理：** 测试完成后可以删除测试数据

## 🎯 测试建议

### 测试流程建议

1. **第一轮：** 先测试所有GET接口（获取数据）
2. **第二轮：** 测试POST接口（创建数据）
3. **第三轮：** 测试PUT接口（更新数据）
4. **第四轮：** 测试DELETE接口（删除数据）

### 数据准备建议

在测试前，请确保数据库中有：
- 老人信息（elderly_id = 1, 2等）
- 工作人员信息（staff_id = 1, 2等）
- 一些基础设备数据

---

**测试完成后，请将结果表格和任何错误信息发送给我进行分析和问题排查！**
