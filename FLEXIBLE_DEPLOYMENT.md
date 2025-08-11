# 🚀 智慧养老系统 - 快速部署指南

## 📂 灵活部署方式

### ✅ 您可以选择任意目录部署：

1. **桌面部署** (推荐给您)：
   ```
   %USERPROFILE%\Desktop\RoomManagement_Modules\
   ```

2. **系统目录部署**：
   ```
   C:\SmartElderlyCare\
   ```

3. **任意自定义目录**：
   ```
   D:\MyApps\SmartElderlyCare\
   ```

## 🎯 部署步骤

### 1. 创建文件夹
在服务器桌面创建文件夹: `RoomManagement_Modules`

### 2. 上传文件
将 `SmartElderlyCare_Deploy_Fixed.zip` 上传到该文件夹并解压

### 3. 启动服务
在文件夹中双击运行:
- `start_server.ps1` (PowerShell版本)
- `start_server.bat` (批处理版本)

### 4. 端口和访问地址检测
如果无法通过 http://47.96.238.102:8080/swagger 访问，请运行:
- `find_swagger.ps1` - 自动发现实际运行端口
- `network_check.bat` - 检查网络和端口状态
- `test_api_server.ps1` - 测试所有API端点

### 5. 验证部署
使用发现的实际端口访问Swagger文档

## ⚠️ 重要提示

- **启动脚本会自动检测当前目录**
- **无需修改任何配置文件**
- **支持在任意目录运行**

---
✅ 现在您可以安心在桌面创建 `RoomManagement_Modules` 文件夹部署了！
