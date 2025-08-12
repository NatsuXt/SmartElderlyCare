# 智慧养老系统第三模块 - API端点测试指南

## 系统信息
- **模块**: 第三模块 (RoomDeviceManagement)
- **端口**: 3003
- **访问地址**: http://47.96.238.102:3003
- **API文档**: http://47.96.238.102:3003/swagger

## API端点列表

### 1. 房间管理模块 (/api/RoomManagement)
```
GET    /api/RoomManagement/rooms              获取所有房间信息
GET    /api/RoomManagement/room/{id}          获取特定房间信息
POST   /api/RoomManagement/room               创建新房间
PUT    /api/RoomManagement/room/{id}          更新房间信息
DELETE /api/RoomManagement/room/{id}          删除房间
```

### 2. 设备管理模块 (/api/DeviceManagement)
```
GET    /api/DeviceManagement/devices          获取所有设备
GET    /api/DeviceManagement/device/{id}      获取特定设备
POST   /api/DeviceManagement/device           添加设备
PUT    /api/DeviceManagement/device/{id}      更新设备信息
DELETE /api/DeviceManagement/device/{id}      删除设备
GET    /api/DeviceManagement/device/{id}/status  获取设备状态
```

### 3. 健康监测模块 (/api/HealthMonitoring)
```
GET    /api/HealthMonitoring/records          获取健康记录
GET    /api/HealthMonitoring/record/{id}      获取特定健康记录
POST   /api/HealthMonitoring/record           创建健康记录
PUT    /api/HealthMonitoring/record/{id}      更新健康记录
DELETE /api/HealthMonitoring/record/{id}      删除健康记录
```

### 4. 电子围栏模块 (/api/ElectronicFence)
```
GET    /api/ElectronicFence/fences           获取所有围栏
GET    /api/ElectronicFence/fence/{id}       获取特定围栏
POST   /api/ElectronicFence/fence            创建围栏
PUT    /api/ElectronicFence/fence/{id}       更新围栏
DELETE /api/ElectronicFence/fence/{id}       删除围栏
GET    /api/ElectronicFence/logs             获取围栏日志
```

### 5. IoT监控模块 (/api/IoTMonitoring)
```
GET    /api/IoTMonitoring/devices            获取IoT设备列表
GET    /api/IoTMonitoring/device/{id}/status 获取设备实时状态
POST   /api/IoTMonitoring/device/{id}/command 发送设备控制命令
GET    /api/IoTMonitoring/alerts             获取报警信息
```

## 快速测试命令

### PowerShell测试脚本
```powershell
# 测试服务器状态
Invoke-RestMethod -Uri "http://47.96.238.102:3003/api/RoomManagement/rooms" -Method GET

# 测试设备状态
Invoke-RestMethod -Uri "http://47.96.238.102:3003/api/DeviceManagement/devices" -Method GET

# 测试健康监测
Invoke-RestMethod -Uri "http://47.96.238.102:3003/api/HealthMonitoring/records" -Method GET
```

### Curl测试命令
```bash
# 测试房间管理
curl -X GET "http://47.96.238.102:3003/api/RoomManagement/rooms"

# 测试设备管理
curl -X GET "http://47.96.238.102:3003/api/DeviceManagement/devices"

# 测试健康监测
curl -X GET "http://47.96.238.102:3003/api/HealthMonitoring/records"
```

## 前端集成示例

### JavaScript/Axios
```javascript
const API_BASE = 'http://47.96.238.102:3003';

// 获取房间列表
axios.get(`${API_BASE}/api/RoomManagement/rooms`)
  .then(response => console.log(response.data))
  .catch(error => console.error(error));

// 获取设备状态
axios.get(`${API_BASE}/api/DeviceManagement/devices`)
  .then(response => console.log(response.data))
  .catch(error => console.error(error));
```

### Python/Requests
```python
import requests

API_BASE = 'http://47.96.238.102:3003'

# 测试房间管理API
response = requests.get(f'{API_BASE}/api/RoomManagement/rooms')
print(response.json())

# 测试设备管理API
response = requests.get(f'{API_BASE}/api/DeviceManagement/devices')
print(response.json())
```

## 数据格式示例

### 房间信息 (Room)
```json
{
  "roomId": "R001",
  "roomName": "101号房间",
  "roomType": "单人间",
  "location": "一楼东侧",
  "capacity": 1,
  "currentOccupancy": 1,
  "status": "已入住"
}
```

### 设备信息 (Device)
```json
{
  "deviceId": "D001",
  "deviceName": "智能床垫",
  "deviceType": "监测设备",
  "roomId": "R001",
  "status": "在线",
  "lastUpdateTime": "2024-01-15T10:30:00"
}
```

### 健康记录 (HealthRecord)
```json
{
  "recordId": "H001",
  "elderlyId": "E001",
  "heartRate": 75,
  "bloodPressure": "120/80",
  "temperature": 36.5,
  "recordTime": "2024-01-15T10:30:00"
}
```

## 状态码说明
- **200**: 请求成功
- **201**: 创建成功
- **400**: 请求参数错误
- **404**: 资源不存在
- **500**: 服务器内部错误

## 注意事项
1. 所有时间格式使用 ISO 8601 标准
2. 支持中文字符编码 (UTF-8)
3. 大部分端点支持分页查询
4. 所有POST/PUT请求需要JSON格式数据
5. 支持跨域访问 (CORS已配置)

---
测试服务器: http://47.96.238.102:3003
API文档: http://47.96.238.102:3003/swagger
模块标识: 第三模块
