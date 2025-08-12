# 智慧养老系统第三模块 - 完整部署包说明

## 部署包信息
- **项目名称**: 智慧养老系统第三模块 (RoomDeviceManagement)
- **版本**: ASP.NET Core 8.0
- **数据库**: Oracle 19c
- **模块端口**: 3003
- **网络模式**: 外部访问模式 (http://*:3003)
- **服务器**: 47.96.238.102

## 部署包内容清单

### 核心应用文件
```
RoomDeviceManagement.exe        - 主程序文件
RoomDeviceManagement.dll        - 应用程序库
RoomDeviceManagement.pdb        - 调试符号文件
RoomDeviceManagement.deps.json  - 依赖清单
RoomDeviceManagement.runtimeconfig.json - 运行时配置
```

### 配置文件
```
appsettings.json                 - 开发环境配置
appsettings.Production.json      - 生产环境配置 (端口3003)
```

### 依赖库文件
```
Oracle.ManagedDataAccess.dll     - Oracle数据库连接
Microsoft.OpenApi.dll            - OpenAPI支持
Swashbuckle.AspNetCore.*.dll     - Swagger API文档
System.Configuration.ConfigurationManager.dll - 配置管理
```

### 启动脚本 (推荐使用顺序)
```
1. start_server_safe.bat         - 安全启动脚本 (首选)
2. start_server_fixed.ps1        - PowerShell启动脚本 (推荐)
3. start_server.bat              - 基础启动脚本
```

### 测试脚本
```
safe-test.ps1                    - 一键安全测试（推荐使用）
unicode-chinese-test.ps1         - 中文字符API测试
simple-test.ps1                  - 基础API测试脚本
test_api_server.ps1              - 完整API端点测试
```

### 诊断工具
```
network_check.bat                - 网络连接检查
find_swagger.ps1                 - API文档定位工具
kill_port_5000.bat              - 端口清理工具 (支持3003端口)
```

### 前端集成资源
```
前端测试脚本_中文字符.js         - JavaScript测试示例
前端集成指南_中文字符支持.md      - 前端开发指南
```

### 文档资料
```
部署指南_第三模块.md             - 完整部署指南
API端点测试指南.md               - API测试文档
README_部署包说明.md             - 本说明文件
```

## 快速部署流程

### 第一步: 权限设置 (仅首次)
```powershell
# 以管理员身份运行
.\setup_permissions.bat
```

### 第二步: 网络检查
```powershell
.\network_check.bat
```

### 第三步: 启动服务
```powershell
# 推荐方式 - 安全启动
.\start_server_safe.bat

# 备用方式 - PowerShell启动  
.\start_server_fixed.ps1
```

### 第四步: 验证部署
```powershell
# 查找API文档
.\find_swagger.ps1

# 手动验证
# 浏览器访问: http://47.96.238.102:3003/swagger
```

## 重要配置说明

### 网络配置 (支持外部访问)
```
监听地址: http://*:3003 (所有IP地址)
外网访问: http://47.96.238.102:3003
内网访问: http://localhost:3003
API文档: http://47.96.238.102:3003/swagger
```

### 端口分配体系
```
模块一: 7000
模块二: 5000
模块三: 3003 ← 当前模块
备用端口: 3004, 3005 (自动切换)
```

### 数据库连接
```
服务器: 47.96.238.102:1521
服务名: XE
字符集: AL32UTF8 (支持中文)
连接池: 启用
```

## 故障排除指南

### 常见问题解决
1. **端口被占用**
   ```powershell
   .\kill_port_5000.bat
   ```

2. **PowerShell权限错误**
   ```powershell
   .\setup_permissions.bat
   ```

3. **网络连接问题**
   ```powershell
   .\network_check.bat
   ```

4. **找不到API文档**
   ```powershell
   .\find_swagger.ps1
   ```

### 日志检查
- 应用程序会在控制台输出详细日志
- 注意Oracle连接状态信息
- 端口绑定成功信息

## 前端集成支持

### API基础地址配置
```javascript
// 前端配置
const API_BASE_URL = 'http://47.96.238.102:3003';

// 示例调用
fetch(`${API_BASE_URL}/api/RoomManagement/rooms`)
  .then(response => response.json())
  .then(data => console.log(data));
```

### 跨域支持
- 系统已配置CORS支持
- 允许外部域名访问
- 支持所有HTTP方法

## 技术特性

### API模块功能
1. **房间管理** - 房间信息维护
2. **设备管理** - IoT设备监控
3. **健康监测** - 健康数据管理
4. **电子围栏** - 安全区域管理
5. **IoT监控** - 设备状态实时监控

### 架构特点
- RESTful API设计
- Swagger API文档自动生成
- Oracle数据库集成
- 中文字符完全支持
- 外部访问支持
- 自动端口冲突处理

## 支持信息
- **部署环境**: Windows Server
- **运行时**: .NET 8.0
- **数据库**: Oracle 19c
- **文档**: 完整的Swagger API文档
- **测试**: 包含完整的测试脚本

---
部署包生成时间: $(Get-Date)
版本标识: 第三模块-外部访问版
技术支持: 智慧养老系统开发团队
