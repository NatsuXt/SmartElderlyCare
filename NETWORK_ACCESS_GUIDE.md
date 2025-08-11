# 外网访问配置指南

## 🌐 如何让API支持外网访问

### 方法1: 本地网络访问
如果在同一局域网内访问：

1. **找到本机IP地址**:
   ```cmd
   ipconfig
   ```
   找到类似 `192.168.x.x` 的地址

2. **使用本机IP访问**:
   ```
   http://你的本机IP:5000/swagger
   例如: http://192.168.1.100:5000/swagger
   ```

3. **确保防火墙允许5000端口**

### 方法2: 部署到真实服务器
如果要部署到47.96.238.102服务器：

1. **上传代码到服务器**
2. **在服务器上运行**:
   ```bash
   cd /path/to/your/project
   dotnet run
   ```
3. **访问**: `http://47.96.238.102:5000/swagger`

### 方法3: 使用内网穿透工具
使用ngrok等工具将本地服务暴露到公网：

```bash
# 安装ngrok后
ngrok http 5000
```

## 🔧 当前最佳解决方案

**立即可用的访问地址**:
- 📋 **Swagger文档**: http://localhost:5000/swagger
- 🏠 **房间API**: http://localhost:5000/api/RoomManagement/rooms  
- 📱 **设备API**: http://localhost:5000/api/DeviceManagement/devices
- 💓 **健康监控**: http://localhost:5000/api/HealthMonitoring/statistics
- 🔒 **电子围栏**: http://localhost:5000/api/ElectronicFence/logs

## 📱 前端开发配置

```javascript
// 本地开发
const API_BASE_URL = 'http://localhost:5000/api';

// 生产环境（部署到服务器后）
const API_BASE_URL = 'http://47.96.238.102:5000/api';
```

## 🔄 架构说明

```
[您的电脑 - 本地开发]
├── API服务器: localhost:5000
├── 数据库连接: 47.96.238.102:1521 (远程Oracle)
└── 访问地址: http://localhost:5000/swagger

[部署到服务器后]
├── API服务器: 47.96.238.102:5000
├── 数据库服务器: 47.96.238.102:1521
└── 访问地址: http://47.96.238.102:5000/swagger
```
