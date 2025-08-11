# VS Code 远程连接服务器指南

## 🌐 连接方案概览

### 方案一：Remote Desktop + VS Code
**适用**: 全功能开发，服务器管理
**优点**: 完整的桌面环境，支持所有VS Code功能
**缺点**: 需要图形界面传输，网络要求较高

### 方案二：VS Code Remote Tunnels
**适用**: 代码编辑，轻量级开发
**优点**: 网络要求低，通过浏览器访问
**缺点**: 功能相对有限

### 方案三：SSH + PowerShell (如果启用SSH)
**适用**: 命令行操作，轻量级编辑
**优点**: 资源占用最少
**缺点**: 需要服务器启用SSH服务

## 🚀 推荐配置：Remote Desktop方案

### 第一步：连接远程桌面
```
服务器地址: 47.96.238.102
用户名: Administrator  
密码: DBdbdb2025@
端口: 3389 (默认)
```

### 第二步：在服务器上设置开发环境

#### 1. 安装 VS Code
下载地址: https://code.visualstudio.com/

#### 2. 安装必要的扩展
- C# Dev Kit
- .NET Extension Pack  
- Oracle Developer Tools (如需要)
- PowerShell
- Chinese (Simplified) Language Pack

#### 3. 配置项目
```powershell
# 克隆项目（如果需要）
cd C:\
git clone <your-repo-url> SmartElderlyCareSource

# 或者直接解压源码到服务器
```

## 🛠️ Remote Tunnels 配置 (替代方案)

如果您想尝试更现代的方案：

### 在服务器上配置
```powershell
# 下载VS Code CLI
Invoke-WebRequest -Uri "https://code.visualstudio.com/sha/download?build=stable&os=cli-win32-x64" -OutFile "vscode-cli.zip"

# 解压并安装
Expand-Archive -Path "vscode-cli.zip" -DestinationPath "C:\vscode-cli"

# 启动隧道
cd C:\vscode-cli
.\code.exe tunnel --accept-server-license-terms
```

### 在本地访问
- 通过浏览器访问: https://vscode.dev
- 连接到服务器隧道

## 🔧 SSH 方案配置 (高级)

如果您想启用SSH访问：

### 在服务器上启用SSH
```powershell
# 启用OpenSSH服务器功能
Add-WindowsCapability -Online -Name OpenSSH.Server

# 启动并设置为自动启动
Start-Service sshd
Set-Service -Name sshd -StartupType 'Automatic'

# 配置防火墙
New-NetFirewallRule -Name sshd -DisplayName 'OpenSSH SSH Server' -Enabled True -Direction Inbound -Protocol TCP -Action Allow -LocalPort 22
```

### VS Code SSH 配置
```
Host smart-elderly-server
    HostName 47.96.238.102
    User Administrator
    Port 22
```

## 🎯 推荐的开发工作流

### 方案一：完整开发环境
1. **RDP 连接到服务器**
2. **在服务器上用 VS Code 开发**
3. **直接在服务器上测试和部署**

### 方案二：混合开发
1. **本地开发和编码**
2. **使用部署脚本推送到服务器**
3. **RDP 连接进行服务器端调试**

## 📱 移动端访问

通过 VS Code Remote Tunnels，您甚至可以：
- 在平板电脑上通过浏览器编码
- 在手机上查看代码和日志
- 随时随地访问服务器环境

## 🛡️ 安全注意事项

1. **RDP 安全**
   ```powershell
   # 启用网络级身份验证
   Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp" -Name SecurityLayer -Value 1
   ```

2. **SSH 安全** (如果启用)
   - 使用密钥认证而非密码
   - 更改默认端口
   - 配置防火墙规则

## 🚀 快速开始

**立即可用的方案**:
1. 使用Windows远程桌面连接到 `47.96.238.102`
2. 在远程桌面中下载安装VS Code
3. 在服务器上直接开发您的智慧养老系统

**需要配置的方案**:
- Remote Tunnels: 需要在服务器上配置隧道
- SSH: 需要启用SSH服务器功能

您想使用哪种方案？我可以为您提供详细的配置步骤。
