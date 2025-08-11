# 智慧养老系�?API 服务器配�?
| 获取所有房�?| GET | `/api/RoomManagement/rooms` | `http://localhost:5000/api/RoomManagement/rooms` |
| 获取房间详情 | GET | `/api/RoomManagement/rooms/{id}` | `http://localhost:5000/api/RoomManagement/rooms/1` |
| 创建房间 | POST | `/api/RoomManagement/rooms` | `http://localhost:5000/api/RoomManagement/rooms` |
| 更新房间 | PUT | `/api/RoomManagement/rooms/{id}` | `http://localhost:5000/api/RoomManagement/rooms/1` |
| 删除房间 | DELETE | `/api/RoomManagement/rooms/{id}` | `http://localhost:5000/api/RoomManagement/rooms/1` |
| 房间统计 | GET | `/api/RoomManagement/rooms/statistics` | `http://localhost:5000/api/RoomManagement/rooms/statistics` | 服务器地址配置

### 本地开发环�?
- **服务器地址**: localhost
- **API端口**: 5000
- **API基础URL**: http://localhost:5000
- **Swagger文档**: http://localhost:5000/swagger

### 数据库连�?
- **Oracle数据�?*: 47.96.238.102:1521/orcl
- **用户**: application_user
- **端口**: 1521 (数据库专用端�?

## 📋 API端点映射�?

### 🏠 房间管理模块 (RoomManagement)
| 功能 | 方法 | 端点 | 完整URL |
|------|------|------|---------|
| 获取所有房�?| GET | `/api/RoomManagement/rooms` | `http://localhost:5000/api/RoomManagement/rooms` |
| 获取房间详情 | GET | `/api/RoomManagement/rooms/{id}` | `http://localhost:5000/api/RoomManagement/rooms/1` |
| 创建房间 | POST | `/api/RoomManagement/rooms` | `http://localhost:5000/api/RoomManagement/rooms` |
| 更新房间 | PUT | `/api/RoomManagement/rooms/{id}` | `http://localhost:5000/api/RoomManagement/rooms/1` |
| 删除房间 | DELETE | `/api/RoomManagement/rooms/{id}` | `http://localhost:5000/api/RoomManagement/rooms/1` |
| 房间统计 | GET | `/api/RoomManagement/rooms/statistics` | `http://localhost:5000/api/RoomManagement/rooms/statistics` |

### 📱 设备管理模块 (DeviceManagement)
| 功能 | 方法 | 端点 | 完整URL |
|------|------|------|---------|
| 获取所有设�?| GET | `/api/DeviceManagement/devices` | `http://localhost:5000/api/DeviceManagement/devices` |
| 获取设备详情 | GET | `/api/DeviceManagement/{id}` | `http://localhost:5000/api/DeviceManagement/1` |
| 创建设备 | POST | `/api/DeviceManagement` | `http://localhost:5000/api/DeviceManagement` |
| 更新设备 | PUT | `/api/DeviceManagement/{id}` | `http://localhost:5000/api/DeviceManagement/1` |
| 删除设备 | DELETE | `/api/DeviceManagement/{id}` | `http://localhost:5000/api/DeviceManagement/1` |
| 设备状态轮�?| GET | `/api/DeviceManagement/poll-status` | `http://localhost:5000/api/DeviceManagement/poll-status` |
| 设备统计 | GET | `/api/DeviceManagement/statistics` | `http://localhost:5000/api/DeviceManagement/statistics` |

### 💓 健康监控模块 (HealthMonitoring)
| 功能 | 方法 | 端点 | 完整URL |
|------|------|------|---------|
| 健康数据上报 | POST | `/api/HealthMonitoring/report` | `http://localhost:5000/api/HealthMonitoring/report` |
| 获取老人最新健康数�?| GET | `/api/HealthMonitoring/elderly/{id}/latest` | `http://localhost:5000/api/HealthMonitoring/elderly/1/latest` |
| 获取老人健康历史 | GET | `/api/HealthMonitoring/elderly/{id}/history` | `http://localhost:5000/api/HealthMonitoring/elderly/1/history` |
| 健康统计 | GET | `/api/HealthMonitoring/statistics` | `http://localhost:5000/api/HealthMonitoring/statistics` |
| 健康异常警报 | GET | `/api/HealthMonitoring/alerts` | `http://localhost:5000/api/HealthMonitoring/alerts` |

### 🔒 电子围栏模块 (ElectronicFence)
| 功能 | 方法 | 端点 | 完整URL |
|------|------|------|---------|
| GPS位置上报 | POST | `/api/ElectronicFence/gps-report` | `http://localhost:5000/api/ElectronicFence/gps-report` |
| 围栏进出记录 | GET | `/api/ElectronicFence/logs` | `http://localhost:5000/api/ElectronicFence/logs` |
| 当前位置状�?| GET | `/api/ElectronicFence/current-status` | `http://localhost:5000/api/ElectronicFence/current-status` |
| 围栏配置 | GET | `/api/ElectronicFence/config` | `http://localhost:5000/api/ElectronicFence/config` |
| 创建围栏配置 | POST | `/api/ElectronicFence/config` | `http://localhost:5000/api/ElectronicFence/config` |
| 删除围栏配置 | DELETE | `/api/ElectronicFence/config/{id}` | `http://localhost:5000/api/ElectronicFence/config/1` |
| 老人位置轨迹 | GET | `/api/ElectronicFence/elderly/{id}/trajectory` | `http://localhost:5000/api/ElectronicFence/elderly/1/trajectory` |
| 围栏警报 | GET | `/api/ElectronicFence/alerts` | `http://localhost:5000/api/ElectronicFence/alerts` |
| 护理人员位置 | GET | `/api/ElectronicFence/staff-locations` | `http://localhost:5000/api/ElectronicFence/staff-locations` |
| 更新护理人员位置 | POST | `/api/ElectronicFence/staff-location` | `http://localhost:5000/api/ElectronicFence/staff-location` |
| 围栏检查测�?| POST | `/api/ElectronicFence/test-fence` | `http://localhost:5000/api/ElectronicFence/test-fence` |

## 🚀 部署命令

### 启动服务�?
```bash
cd /path/to/your/project
dotnet run
```

### 或者发布后启动
```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet RoomDeviceManagement.dll
```

## 🔧 前端配置示例

### JavaScript/TypeScript
```javascript
const API_CONFIG = {
    BASE_URL: 'http://localhost:5000/api',
    HEADERS: {
        'Content-Type': 'application/json; charset=utf-8',
        'Accept': 'application/json'
    }
};

// 示例API调用
async function getRooms() {
    const response = await fetch(`${API_CONFIG.BASE_URL}/RoomManagement/rooms`, {
        method: 'GET',
        headers: API_CONFIG.HEADERS
    });
    return await response.json();
}

async function createRoom(roomData) {
    const response = await fetch(`${API_CONFIG.BASE_URL}/RoomManagement/rooms`, {
        method: 'POST',
        headers: API_CONFIG.HEADERS,
        body: JSON.stringify(roomData)
    });
    return await response.json();
}
```

### React/Vue.js Axios配置
```javascript
import axios from 'axios';

const apiClient = axios.create({
    baseURL: 'http://localhost:5000/api',
    headers: {
        'Content-Type': 'application/json; charset=utf-8'
    }
});

// 示例使用
const roomService = {
    async getAllRooms() {
        return await apiClient.get('/RoomManagement/rooms');
    },
    
    async createRoom(roomData) {
        return await apiClient.post('/RoomManagement/rooms', roomData);
    }
};
```

## 📊 测试验证

### PowerShell测试命令
```powershell
# 测试房间API
$roomData = @{
    RoomNumber = "测试房间107"
    RoomType = "单人�?
    Capacity = 1
    Status = "空闲"
    Rate = 3500.00
    BedType = "单人�?
    Floor = 3
}
$body = $roomData | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/RoomManagement/rooms" -Method POST -Body $body -ContentType "application/json; charset=utf-8"
```

### cURL测试命令
```bash
# 获取所有房�?
curl -X GET "http://localhost:5000/api/RoomManagement/rooms" \
     -H "Accept: application/json"

# 创建新房�?
curl -X POST "http://localhost:5000/api/RoomManagement/rooms" \
     -H "Content-Type: application/json; charset=utf-8" \
     -d '{
       "RoomNumber": "测试房间108",
       "RoomType": "双人�?,
       "Capacity": 2,
       "Status": "空闲",
       "Rate": 4000.00,
       "BedType": "双人�?,
       "Floor": 2
     }'
```

## 🔒 安全配置

### CORS设置
系统已配置允许跨域访问：
```csharp
app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});
```

### 生产环境建议
1. **限制CORS来源**: �?`AllowAnyOrigin()` 改为具体的前端域�?
2. **HTTPS配置**: 配置SSL证书使用HTTPS
3. **防火�?*: 确保5000端口对外开�?
4. **监控**: 配置日志和监控系�?

## 📱 移动端配�?

### Android/iOS
```javascript
const API_BASE = 'http://localhost:5000/api';

// React Native示例
const apiCall = async (endpoint, method = 'GET', data = null) => {
    const config = {
        method,
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
        },
    };
    
    if (data) {
        config.body = JSON.stringify(data);
    }
    
    const response = await fetch(`${API_BASE}${endpoint}`, config);
    return await response.json();
};
```

现在您的API系统已经配置为在服务�?`47.96.238.102:5000` 上运行，Swagger文档也会正确显示服务器地址�?

