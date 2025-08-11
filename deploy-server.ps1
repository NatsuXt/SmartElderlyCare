# 智慧养老系统 - 服务器部署脚本
# 目标服务器: 47.96.238.102
# 用户: Administrator
# 日期: 2025年8月11日

param(
    [string]$ServerIP = "47.96.238.102",
    [string]$Username = "Administrator", 
    [string]$Password = "DBdbdb2025@",
    [int]$Port = 8080
)

Write-Host "🚀 开始部署智慧养老系统到服务器..." -ForegroundColor Green
Write-Host "📍 目标服务器: $ServerIP" -ForegroundColor Yellow
Write-Host "👤 用户名: $Username" -ForegroundColor Yellow
Write-Host "🔌 端口: $Port" -ForegroundColor Yellow

# 1. 创建远程会话凭据
$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force
$Credential = New-Object System.Management.Automation.PSCredential($Username, $SecurePassword)

Write-Host "`n📦 准备部署文件..." -ForegroundColor Cyan

# 2. 检查本地发布文件
$PublishPath = ".\publish"
if (-not (Test-Path $PublishPath)) {
    Write-Host "❌ 发布文件夹不存在，请先运行: dotnet publish -c Release -o publish" -ForegroundColor Red
    exit 1
}

# 3. 创建部署包
$DeployPackage = "SmartElderlyCare_Deploy.zip"
if (Test-Path $DeployPackage) {
    Remove-Item $DeployPackage -Force
}

Write-Host "📁 创建部署压缩包..." -ForegroundColor Cyan
Compress-Archive -Path "$PublishPath\*" -DestinationPath $DeployPackage -Force

# 4. 创建远程部署脚本
$RemoteScript = @"
# 远程服务器部署脚本
Write-Host "🖥️  开始在服务器上部署智慧养老系统..." -ForegroundColor Green

# 创建应用目录
`$AppPath = "C:\SmartElderlyCare"
if (-not (Test-Path `$AppPath)) {
    New-Item -Path `$AppPath -ItemType Directory -Force
    Write-Host "📁 创建应用目录: `$AppPath" -ForegroundColor Yellow
}

# 停止现有服务（如果存在）
Write-Host "🛑 检查并停止现有服务..." -ForegroundColor Cyan
Get-Process -Name "RoomDeviceManagement" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {`$_.MainModule.FileName -like "*SmartElderlyCare*"} | Stop-Process -Force

# 解压应用文件
Write-Host "📦 解压应用文件到 `$AppPath..." -ForegroundColor Cyan
Expand-Archive -Path "C:\temp\SmartElderlyCare_Deploy.zip" -DestinationPath `$AppPath -Force

# 配置防火墙
Write-Host "🔥 配置防火墙规则..." -ForegroundColor Cyan
New-NetFirewallRule -DisplayName "SmartElderlyCare API" -Direction Inbound -Protocol TCP -LocalPort $Port -Action Allow -ErrorAction SilentlyContinue

# 修改配置文件中的服务器地址
Write-Host "⚙️  更新配置文件..." -ForegroundColor Cyan
`$ConfigFile = "`$AppPath\appsettings.json"
if (Test-Path `$ConfigFile) {
    `$Config = Get-Content `$ConfigFile -Raw | ConvertFrom-Json
    `$Config.ServerConfig.BaseUrl = "http://$ServerIP`:$Port"
    `$Config.ServerConfig.SwaggerUrl = "http://$ServerIP`:$Port/swagger"
    `$Config.ServerConfig.ApiPort = $Port
    `$Config | ConvertTo-Json -Depth 10 | Set-Content `$ConfigFile -Encoding UTF8
    Write-Host "✅ 配置文件更新完成" -ForegroundColor Green
}

# 启动应用
Write-Host "🚀 启动智慧养老系统..." -ForegroundColor Green
Set-Location `$AppPath

# 设置Oracle环境变量
`$env:NLS_LANG = "SIMPLIFIED CHINESE_CHINA.AL32UTF8"
`$env:ORA_NCHAR_LITERAL_REPLACE = "TRUE"

# 启动应用（后台运行）
Start-Process -FilePath "dotnet" -ArgumentList "RoomDeviceManagement.dll" -WorkingDirectory `$AppPath -WindowStyle Hidden

Write-Host "⏳ 等待应用启动..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# 测试API是否正常运行
try {
    `$TestUrl = "http://localhost:$Port/api/RoomManagement/rooms"
    `$Response = Invoke-WebRequest -Uri `$TestUrl -UseBasicParsing -TimeoutSec 10
    if (`$Response.StatusCode -eq 200) {
        Write-Host "✅ API服务启动成功！" -ForegroundColor Green
        Write-Host "🌐 访问地址: http://$ServerIP`:$Port/swagger" -ForegroundColor Cyan
    } else {
        Write-Host "⚠️  API响应异常，状态码: `$(`$Response.StatusCode)" -ForegroundColor Orange
    }
} catch {
    Write-Host "❌ API测试失败: `$(`$_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 部署脚本执行完成！" -ForegroundColor Green
Write-Host "📋 部署信息:" -ForegroundColor Yellow
Write-Host "   - 应用路径: `$AppPath" -ForegroundColor White
Write-Host "   - API地址: http://$ServerIP`:$Port" -ForegroundColor White  
Write-Host "   - Swagger: http://$ServerIP`:$Port/swagger" -ForegroundColor White
Write-Host "   - 防火墙: 端口 $Port 已开放" -ForegroundColor White
"@

# 5. 保存远程脚本到文件
$RemoteScript | Out-File -FilePath "deploy-remote.ps1" -Encoding UTF8

Write-Host "`n🔐 尝试连接远程服务器..." -ForegroundColor Cyan

try {
    # 6. 建立远程会话
    $Session = New-PSSession -ComputerName $ServerIP -Credential $Credential -ErrorAction Stop
    Write-Host "✅ 成功连接到服务器 $ServerIP" -ForegroundColor Green
    
    # 7. 创建临时目录并上传文件
    Write-Host "📤 上传部署文件..." -ForegroundColor Cyan
    Invoke-Command -Session $Session -ScriptBlock {
        if (-not (Test-Path "C:\temp")) {
            New-Item -Path "C:\temp" -ItemType Directory -Force
        }
    }
    
    # 上传部署包
    Copy-Item -Path $DeployPackage -Destination "C:\temp\" -ToSession $Session -Force
    Write-Host "✅ 部署包上传完成" -ForegroundColor Green
    
    # 8. 执行远程部署脚本
    Write-Host "🚀 执行远程部署..." -ForegroundColor Cyan
    Invoke-Command -Session $Session -ScriptBlock ([ScriptBlock]::Create($RemoteScript))
    
    # 9. 清理远程会话
    Remove-PSSession -Session $Session
    Write-Host "✅ 远程会话已关闭" -ForegroundColor Green
    
} catch {
    Write-Host "❌ 远程连接失败: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`n🔧 可能的解决方案:" -ForegroundColor Yellow
    Write-Host "1. 检查服务器IP地址是否正确" -ForegroundColor White
    Write-Host "2. 确认用户名和密码是否正确" -ForegroundColor White
    Write-Host "3. 确保服务器开启了PowerShell远程管理" -ForegroundColor White
    Write-Host "4. 检查网络连接和防火墙设置" -ForegroundColor White
    Write-Host "`n💡 手动部署选项:" -ForegroundColor Cyan
    Write-Host "1. 使用RDP远程桌面连接到服务器" -ForegroundColor White
    Write-Host "2. 将 $DeployPackage 文件上传到服务器" -ForegroundColor White
    Write-Host "3. 在服务器上运行 deploy-remote.ps1 脚本" -ForegroundColor White
}

# 10. 清理本地临时文件
Write-Host "`n🧹 清理临时文件..." -ForegroundColor Cyan
if (Test-Path $DeployPackage) {
    # Remove-Item $DeployPackage -Force
    Write-Host "📦 部署包保留: $DeployPackage" -ForegroundColor Yellow
}

Write-Host "`n🎉 部署脚本执行完成！" -ForegroundColor Green
Write-Host "🌐 如果部署成功，请访问: http://$ServerIP`:$Port/swagger" -ForegroundColor Cyan
