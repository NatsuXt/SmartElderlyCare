# 🚀 智慧养老系统 - 详细部署操作手册

## 📋 部署信息总览

| 项目 | 详情 |
|------|------|
| **服务器地址** | 47.96.238.102 |
| **登录用户** | Administrator |
| **登录密码** | DBdbdb2025@ |
| **应用端口** | 8080 (自动检测可用端口) |
| **数据库** | Oracle 19c (47.96.238.102:1521/orcl) |
| **部署包** | SmartElderlyCare_Deploy_FINAL.zip (4.28 MB) |
| **框架版本** | ASP.NET Core 8.0 |
| **部署时间** | 约5-10分钟 |

---

## 🎯 部署前检查清单

### ☑️ 必要条件确认
- [ ] 服务器可正常访问 (47.96.238.102)
- [ ] 具备管理员权限
- [ ] 网络连接稳定
- [ ] 部署包完整下载

### ☑️ 服务器环境
- [ ] Windows Server 操作系统
- [ ] .NET 8.0 Runtime (脚本会自动检查)
- [ ] Oracle客户端 (可选，已内置驱动)

---

## 🔧 详细部署步骤

### 步骤 1️⃣: 远程连接服务器

#### 使用Windows远程桌面连接:
1. **按快捷键** `Win + R`
2. **输入** `mstsc` 并按回车
3. **在远程桌面连接窗口中输入:**
   - 计算机: `47.96.238.102`
   - 用户名: `Administrator`
   - 密码: `DBdbdb2025@`
4. **点击** "连接" 按钮
5. **等待** 远程桌面连接成功

#### 连接成功标志:
- 看到服务器桌面
- 任务栏显示正常
- 可以正常操作

---

### 步骤 2️⃣: 创建部署文件夹

#### 在服务器桌面操作:
1. **右键点击** 桌面空白处
2. **选择** "新建" → "文件夹"
3. **输入文件夹名称:** `RoomManagement_Modules`
4. **按回车确认** 创建

#### 可选的其他位置:
```bash
C:\SmartElderlyCare\           # 系统盘根目录
D:\Applications\SmartCare\     # 其他盘符
C:\Users\Administrator\Desktop\RoomManagement_Modules\  # 完整路径
```

---

### 步骤 3️⃣: 上传部署包

#### 方法A: 直接复制粘贴 (推荐)
1. **在本地电脑** 找到 `SmartElderlyCare_Deploy_FINAL.zip`
2. **复制文件** (Ctrl+C)
3. **在远程桌面中** 进入 `RoomManagement_Modules` 文件夹
4. **粘贴文件** (Ctrl+V)
5. **等待上传完成** (约30秒-2分钟)

#### 方法B: 通过共享驱动器
1. **在远程桌面连接设置中** 启用"剪贴板"和"驱动器"共享
2. **重新连接** 远程桌面
3. **在服务器中访问** 本地驱动器
4. **复制部署包** 到目标文件夹

#### 上传完成确认:
- 文件大小显示: 4.28 MB
- 文件名: SmartElderlyCare_Deploy_FINAL.zip
- 无错误提示

---

### 步骤 4️⃣: 解压部署包

#### 解压操作:
1. **在服务器中** 进入 `RoomManagement_Modules` 文件夹
2. **右键点击** `SmartElderlyCare_Deploy_FINAL.zip`
3. **选择** "全部提取..." 或 "Extract All..."
4. **选择提取位置** (默认当前文件夹)
5. **点击** "提取" 或 "Extract"
6. **等待解压完成**

#### 解压后应包含以下文件:
```
📁 RoomManagement_Modules/
├── 📄 RoomDeviceManagement.dll         # 主程序文件
├── 📄 RoomDeviceManagement.exe         # 可执行文件
├── 📄 appsettings.json                 # 开发配置
├── 📄 appsettings.Production.json      # 生产配置
├── 📄 start_server_safe.bat           # 主启动脚本
├── 📄 setup_permissions.bat           # 权限设置
├── 📄 find_swagger.ps1                # 端口发现工具
├── 📄 network_check.bat               # 网络诊断
├── 📄 kill_port_5000.bat              # 端口清理
├── 📄 start_server_fixed.ps1          # PowerShell启动脚本
├── 📁 runtimes/                       # 运行时文件
└── 📄 各种.dll文件                     # 依赖库文件
```

---

### 步骤 5️⃣: 设置PowerShell执行权限

#### 🚨 重要: 此步骤必须最先执行!

1. **找到文件** `setup_permissions.bat`
2. **右键点击** 该文件
3. **选择** "以管理员身份运行"
4. **在弹出的窗口中** 看到:
   ```
   正在设置PowerShell执行策略...
   ✅ PowerShell执行策略设置成功
   现在可以运行PowerShell脚本了
   ```
5. **按任意键** 关闭窗口

#### 如果出现错误:
1. **手动打开PowerShell** (以管理员身份)
2. **输入命令:** 
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
   ```
3. **按回车执行**
4. **看到设置成功提示**

---

### 步骤 6️⃣: 启动智慧养老系统

#### 主要启动方法 (推荐):
1. **双击** `start_server_safe.bat` 文件
2. **观察控制台输出**，应该看到:
   ```
   ========================================
   智慧养老系统 - 安全启动脚本
   ========================================
   
   设置环境变量完成
   NLS_LANG=SIMPLIFIED CHINESE_CHINA.AL32UTF8
   ASPNETCORE_ENVIRONMENT=Production
   
   ✅ .NET Runtime 已安装
   ✅ 应用文件检查通过
   
   🚀 启动智慧养老系统...
   服务器IP: 47.96.238.102
   预期端口: 8080
   ```

3. **等待启动完成**，看到类似信息:
   ```
   Now listening on: http://[::]:8080
   Application started. Press Ctrl+C to shut down.
   ```

#### 备用启动方法:
1. **右键点击** `start_server_fixed.ps1`
2. **选择** "用PowerShell运行"
3. **观察彩色输出** 等待启动成功

#### 🔍 启动过程说明:
- **环境变量设置**: Oracle中文字符支持
- **运行时检查**: 验证.NET环境
- **端口冲突处理**: 自动选择可用端口
- **应用启动**: 启动API服务

---

### 步骤 7️⃣: 验证部署成功

#### 7.1 检查控制台输出
确认看到以下关键信息:
```
✅ 环境检查通过
🚀 应用启动成功
Now listening on: http://[::]:8080
Application started.
```

#### 7.2 访问Swagger API文档
1. **打开浏览器** (建议Chrome或Edge)
2. **输入地址:** `http://47.96.238.102:8080/swagger`
3. **应该看到:** 智慧养老系统API文档界面

#### 7.3 界面元素确认:
- 页面标题: "智慧养老系统 API v1"
- 看到5个主要API模块
- 界面布局正常，无乱码

#### 7.4 快速API测试:
1. **展开** "房间管理" 模块
2. **点击** GET `/api/RoomManagement/rooms`
3. **点击** "Try it out" 按钮
4. **点击** "Execute" 执行
5. **确认** 返回状态码200

---

### 步骤 8️⃣: 故障排除 (如需要)

#### 🔍 情况1: 无法访问Swagger文档

**解决方案:**
1. **运行端口发现工具:**
   - 双击 `find_swagger.ps1`
   - 查看输出中的实际端口
   - 使用发现的端口访问

2. **示例输出:**
   ```
   🎯 发现Swagger文档: http://localhost:8081/swagger
   🌍 外部访问地址: http://47.96.238.102:8081/swagger
   ```

#### 🔍 情况2: 端口被占用

**解决方案:**
1. **运行网络诊断:**
   - 双击 `network_check.bat`
   - 查看端口占用情况

2. **清理端口冲突:**
   - 双击 `kill_port_5000.bat`
   - 重新启动应用

#### 🔍 情况3: 权限错误

**解决方案:**
1. **确认已执行步骤5** (权限设置)
2. **以管理员身份重新运行** 启动脚本
3. **检查Windows防火墙** 设置

---

### 步骤 9️⃣: 完整功能测试

#### 9.1 API模块测试
在Swagger界面中测试以下模块:

**🏠 房间管理模块:**
- GET `/api/RoomManagement/rooms` - 获取房间列表
- POST `/api/RoomManagement/rooms` - 创建新房间
- PUT `/api/RoomManagement/rooms/{id}` - 更新房间信息
- DELETE `/api/RoomManagement/rooms/{id}` - 删除房间

**📱 设备管理模块:**
- GET `/api/DeviceManagement/devices` - 获取设备列表
- POST `/api/DeviceManagement/devices` - 添加新设备
- GET `/api/DeviceManagement/devices/{id}/status` - 获取设备状态

**💓 健康监测模块:**
- GET `/api/HealthMonitoring/health-records` - 获取健康记录
- POST `/api/HealthMonitoring/health-records` - 上报健康数据

**🔒 电子围栏模块:**
- GET `/api/ElectronicFence/fences` - 获取围栏列表
- POST `/api/ElectronicFence/check-boundary` - 边界检查

**🌐 IoT监控模块:**
- GET `/api/IoTMonitoring/status` - 获取设备状态
- POST `/api/IoTMonitoring/report-fault` - 故障上报

#### 9.2 数据库连接测试
1. **执行任意GET请求**
2. **确认返回数据** (可能为空，但不应报错)
3. **检查中文字符** 显示正常

---

### 步骤 🔟: 部署完成确认

#### ✅ 部署成功的标准:
- [ ] 应用服务正常启动，无错误
- [ ] Swagger文档可正常访问
- [ ] 至少一个API端点测试成功
- [ ] 数据库连接正常
- [ ] 中文字符显示无乱码
- [ ] 控制台无严重错误信息

#### 📝 记录重要信息:
```
✅ 部署成功信息记录
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📅 部署时间: _______________
🌐 访问地址: http://47.96.238.102:____/swagger
🎯 实际端口: ______ (如果不是8080)
👤 操作人员: _______________
📋 测试结果: _______________
```

---

## 🚨 常见问题及解决方案

### ❓ Q1: 启动时提示".NET Runtime未安装"
**💡 解决方案:**
1. 访问: https://dotnet.microsoft.com/download/dotnet/8.0
2. 下载".NET 8.0 Runtime"
3. 在服务器上安装
4. 重新启动应用

### ❓ Q2: 浏览器提示"无法访问此网站"
**💡 解决方案:**
1. 确认应用已启动且无错误
2. 检查Windows防火墙设置
3. 使用`find_swagger.ps1`找到实际端口
4. 尝试在服务器本地访问 http://localhost:8080/swagger

### ❓ Q3: API返回500内部服务器错误
**💡 解决方案:**
1. 检查控制台错误信息
2. 确认Oracle数据库服务运行正常
3. 验证数据库连接字符串
4. 检查Oracle客户端配置

### ❓ Q4: 中文字符显示为乱码
**💡 解决方案:**
1. 确认启动脚本已设置NLS_LANG环境变量
2. 重启应用服务
3. 检查数据库字符集配置
4. 验证Oracle客户端版本

### ❓ Q5: PowerShell脚本无法运行
**💡 解决方案:**
1. 确保已执行`setup_permissions.bat`
2. 手动设置PowerShell执行策略
3. 以管理员身份运行PowerShell
4. 使用批处理脚本作为替代

---

## 📞 部署支持

### 🆘 如需技术支持:
1. **保存完整的错误截图**
2. **记录详细的操作步骤**
3. **提供服务器环境信息**
4. **描述问题出现的具体场景**

### 📧 支持信息收集模板:
```
🐛 问题报告
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📅 发生时间: 
🔢 步骤编号: 
📝 具体操作: 
❌ 错误信息: 
🖼️ 错误截图: 
💻 系统环境: 
🌐 网络状态: 
```

---

## 🎉 恭喜部署成功!

### 🎯 您已成功部署智慧养老系统API服务！

#### 🔗 访问信息汇总:
- **Swagger API文档:** http://47.96.238.102:8080/swagger
- **API基础地址:** http://47.96.238.102:8080/api
- **应用状态:** 运行中
- **数据库连接:** 正常

#### 📋 下一步建议:
1. **通知团队成员** 进行功能测试
2. **配置前端应用** 连接API地址
3. **录入测试数据** 验证功能完整性
4. **建立监控机制** 确保服务稳定运行

#### 🏆 部署完成标志:
> ✅ **智慧养老系统API服务已成功部署到生产服务器！**
> 
> 🌟 **服务状态:** 运行正常
> 
> 🎯 **可访问地址:** http://47.96.238.102:8080/swagger

---

**📝 文档版本:** v1.0  
**📅 最后更新:** 2025年8月12日  
**🏷️ 文档状态:** 最新稳定版  

*感谢您使用智慧养老系统！*
********