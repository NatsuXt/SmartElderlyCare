# 智慧养老系统服务器部署指南

## 📋 部署信息
- **目标服务器**: 47.96.238.102
- **用户名**: Administrator  
- **密码**: DBdbdb2025@
- **建议端口**: 8080
- **部署包**: SmartElderlyCare_Deploy.zip (4.5MB)

## 🚀 部署步骤

### 第一步：连接服务器
1. 使用远程桌面连接到服务器
   ```
   地址: 47.96.238.102
   用户名: Administrator
   密码: DBdbdb2025@
   ```

### 第二步：准备服务器环境
1. **安装 .NET 8.0 Runtime**
   - 下载地址: https://dotnet.microsoft.com/download/dotnet/8.0
   - 选择 "ASP.NET Core Runtime 8.0.x" (Windows x64)
   - 安装完成后，在命令行运行 `dotnet --info` 验证

2. **创建应用目录**
   ```powershell
   New-Item -Path "C:\SmartElderlyCare" -ItemType Directory -Force
   ```

### 第三步：上传和部署应用
1. **上传部署包**
   - 将 `SmartElderlyCare_Deploy.zip` 上传到服务器的 `C:\temp\` 目录

2. **解压应用文件**
   ```powershell
   Expand-Archive -Path "C:\temp\SmartElderlyCare_Deploy.zip" -DestinationPath "C:\SmartElderlyCare" -Force
   ```

### 第四步：配置防火墙
1. **开放API端口**
   ```powershell
   New-NetFirewallRule -DisplayName "SmartElderlyCare API" -Direction Inbound -Protocol TCP -LocalPort 8080 -Action Allow
   ```

### 第五步：配置环境变量
1. **设置Oracle字符编码**
   ```powershell
   [Environment]::SetEnvironmentVariable("NLS_LANG", "SIMPLIFIED CHINESE_CHINA.AL32UTF8", "Machine")
   [Environment]::SetEnvironmentVariable("ORA_NCHAR_LITERAL_REPLACE", "TRUE", "Machine")
   ```

### 第六步：启动应用
1. **进入应用目录**
   ```powershell
   Set-Location "C:\SmartElderlyCare"
   ```

2. **启动应用**
   ```powershell
   dotnet RoomDeviceManagement.dll
   ```

### 第七步：验证部署
1. **本地测试**
   - 在服务器上访问: http://localhost:8080/swagger
   
2. **远程测试**
   - 在其他机器上访问: http://47.96.238.102:8080/swagger

## 🔧 配置文件说明

### appsettings.json 关键配置
```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=app123456;..."
  },
  "ServerConfig": {
    "BaseUrl": "http://47.96.238.102:8080",
    "SwaggerUrl": "http://47.96.238.102:8080/swagger",
    "ApiPort": 8080
  }
}
```

## 🛠️ 故障排除

### 常见问题

1. **端口被占用**
   ```powershell
   netstat -ano | findstr :8080
   ```
   如果端口被占用，可以更换其他端口如5000、5001等

2. **防火墙阻止**
   - 确保Windows防火墙允许端口8080
   - 检查网络防火墙设置

3. **Oracle连接失败**
   - 确认数据库服务器可访问
   - 验证用户名密码正确
   - 检查Oracle客户端配置

4. **.NET Runtime未安装**
   ```
   'dotnet' 不是内部或外部命令
   ```
   重新安装.NET 8.0 Runtime

### 服务管理

1. **创建Windows服务 (可选)**
   ```powershell
   sc create "SmartElderlyCare" binPath="C:\SmartElderlyCare\RoomDeviceManagement.exe" start=auto
   sc start SmartElderlyCare
   ```

2. **停止应用**
   ```powershell
   Get-Process -Name "RoomDeviceManagement" | Stop-Process -Force
   ```

## 📊 部署后检查清单

- [ ] 服务器可以访问
- [ ] .NET 8.0 Runtime已安装
- [ ] 应用文件已解压到正确位置
- [ ] 防火墙端口已开放
- [ ] 环境变量已设置
- [ ] 应用成功启动
- [ ] API接口响应正常
- [ ] Swagger文档可访问
- [ ] 数据库连接正常

## 🌐 访问地址

部署成功后，可以通过以下地址访问：

- **API文档**: http://47.96.238.102:8080/swagger
- **房间管理**: http://47.96.238.102:8080/api/RoomManagement/rooms
- **设备管理**: http://47.96.238.102:8080/api/DeviceManagement/devices
- **健康监测**: http://47.96.238.102:8080/api/HealthMonitoring/statistics

## 📞 技术支持

如遇到问题，请检查：
1. 服务器日志输出
2. Windows事件查看器
3. 防火墙设置
4. 网络连接状态

---
**部署包信息**: SmartElderlyCare_Deploy.zip (4.5MB)
**创建时间**: 2025年8月11日
**版本**: v1.0
