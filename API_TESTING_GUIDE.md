# 智能设备状态监控业务测试脚本

## 1. 设备状态上报 API 测试

### 测试单设备状态上报（正常状态）
```bash
curl -X POST "http://localhost:5000/api/DeviceStatusReport/status" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 1,
    "deviceName": "智能床垫001",
    "deviceType": "智能床垫",
    "status": "正常",
    "location": "101房间",
    "description": "设备运行正常"
  }'
```

### 测试单设备故障上报（故障状态）
```bash
curl -X POST "http://localhost:5000/api/DeviceStatusReport/status" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 2,
    "deviceName": "门锁002",
    "deviceType": "智能门锁",
    "status": "故障",
    "location": "102房间",
    "description": "门锁传感器失灵，无法正常开启"
  }'
```

### 测试批量设备状态上报
```bash
curl -X POST "http://localhost:5000/api/DeviceStatusReport/batch-status" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "deviceId": 3,
      "deviceName": "心率监测仪003",
      "deviceType": "心率监测仪",
      "status": "正常",
      "location": "103房间"
    },
    {
      "deviceId": 4,
      "deviceName": "智能床垫004",
      "deviceType": "智能床垫",
      "status": "故障",
      "location": "104房间",
      "description": "压力传感器数据异常"
    }
  ]'
```

### 查询设备状态
```bash
curl "http://localhost:5000/api/DeviceStatusReport/status/1"
curl "http://localhost:5000/api/DeviceStatusReport/status/2"
```

## 2. 设备监控 API 测试

### 轮询所有设备状态
```bash
curl "http://localhost:5000/api/DeviceMonitoring/poll-status"
```

### 处理设备故障上报
```bash
curl -X POST "http://localhost:5000/api/DeviceMonitoring/fault-report" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 5,
    "deviceType": "智能水表",
    "faultStatus": "故障",
    "faultDescription": "流量传感器读数异常",
    "reportTime": "2025-08-07T10:30:00"
  }'
```

### 同步所有设备状态
```bash
curl -X POST "http://localhost:5000/api/DeviceMonitoring/sync-status"
```

## 3. IoT监控 API 测试

### 获取设备状态详情
```bash
curl "http://localhost:5000/api/IoTMonitoring/device-status/1"
```

### 获取老人健康历史
```bash
curl "http://localhost:5000/api/IoTMonitoring/elderly-health/1?days=7"
```

### 获取围栏日志
```bash
curl "http://localhost:5000/api/IoTMonitoring/fence-logs?elderlyId=1&hours=24"
```

## 4. 预期业务流程测试

### 完整故障处理流程测试
1. 设备上报故障状态
2. 系统自动检测故障
3. 查询维修人员信息
4. 发送故障通知
5. 记录故障日志

```bash
# 1. 上报故障设备
curl -X POST "http://localhost:5000/api/DeviceStatusReport/status" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": 99,
    "deviceName": "测试故障设备",
    "deviceType": "智能监测仪",
    "status": "故障",
    "location": "测试房间",
    "description": "模拟设备故障测试"
  }'

# 2. 查看故障设备状态
curl "http://localhost:5000/api/DeviceStatusReport/status/99"

# 3. 手动触发设备监控轮询
curl "http://localhost:5000/api/DeviceMonitoring/poll-status"
```

## 5. 测试预期结果

### 成功指标：
- ✅ API接口正常响应（HTTP 200）
- ✅ 故障设备状态正确更新到数据库
- ✅ 后台服务检测到故障设备
- ✅ 维修人员收到通知（在日志中显示）
- ✅ 设备状态查询返回正确信息

### 监控日志：
观察控制台输出，应该看到类似的日志：
```
info: 设备监控后台服务已启动，轮询间隔: 5 分钟
info: 开始执行设备状态轮询检查...
info: 发现 1 个新故障设备，正在发送通知...
info: 通知维修人员 张维修 (电话: 13812345678): 设备 99 (智能监测仪) 在 测试房间 发生故障
```

## 6. 故障排查

如果API调用失败，检查以下方面：

### 6.1 数据库连接问题
- 检查Oracle数据库是否可访问
- 验证用户名密码：application_user/20252025
- 确认必要的表是否存在

### 6.2 服务状态检查
- 确认应用已启动在 http://localhost:5000
- 检查后台服务是否正常运行
- 观察控制台日志错误信息

### 6.3 数据准备
- 确保STAFFINFO表中有维修人员数据
- 检查DeviceStatus表结构是否正确
- 验证表权限设置

## 7. 业务验证要点

本次测试主要验证以下业务需求：

1. ✅ **实时轮询**: 系统每5分钟自动轮询DeviceStatus表
2. ✅ **故障检测**: 当设备上报"故障"状态时能够被系统检测到
3. ✅ **自动更新**: 设备状态自动更新到DeviceStatus表
4. ✅ **通知机制**: 向职位为"维修人员"的员工发送故障通知
5. ✅ **API接口**: 提供完整的设备状态上报和查询接口
6. ✅ **后台服务**: 后台服务持续监控设备状态变化
