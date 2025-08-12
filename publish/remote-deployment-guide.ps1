# 远程服务器部署脚本 - 智慧养老系统第三模块

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "远程服务器部署向导" -ForegroundColor Green
Write-Host "服务器: 47.96.238.102" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n📋 部署步骤说明:" -ForegroundColor Yellow
Write-Host "1. 将整个publish文件夹复制到远程服务器" -ForegroundColor White
Write-Host "2. 在远程服务器上配置防火墙" -ForegroundColor White
Write-Host "3. 在远程服务器上启动应用程序" -ForegroundColor White

Write-Host "`n🔧 手动部署步骤:" -ForegroundColor Yellow

Write-Host "`n第一步: 连接远程服务器" -ForegroundColor Cyan
Write-Host "  远程桌面连接: mstsc" -ForegroundColor White
Write-Host "  计算机: 47.96.238.102" -ForegroundColor White
Write-Host "  用户名: Administrator" -ForegroundColor White
Write-Host "  密码: DBdbdb2025@" -ForegroundColor White

Write-Host "`n第二步: 上传文件" -ForegroundColor Cyan
Write-Host "  方法1: 复制粘贴整个publish文件夹到远程桌面" -ForegroundColor White
Write-Host "  方法2: 使用共享文件夹" -ForegroundColor White
Write-Host "  目标位置: C:\SmartElderlyCare\" -ForegroundColor White

Write-Host "`n第三步: 在远程服务器上配置防火墙" -ForegroundColor Cyan
Write-Host "  以管理员身份运行PowerShell:" -ForegroundColor White
Write-Host "  New-NetFirewallRule -DisplayName `"SmartElderlyCare-Port3003`" -Direction Inbound -Protocol TCP -LocalPort 3003 -Action Allow" -ForegroundColor Green

Write-Host "`n第四步: 在远程服务器上启动应用" -ForegroundColor Cyan
Write-Host "  进入部署目录: cd C:\SmartElderlyCare\" -ForegroundColor White
Write-Host "  运行启动脚本: .\start_server_fixed.ps1" -ForegroundColor Green
Write-Host "  或直接运行: .\RoomDeviceManagement.exe --urls=`"http://*:3003`"" -ForegroundColor Green

Write-Host "`n第五步: 验证部署" -ForegroundColor Cyan
Write-Host "  本地测试: http://localhost:3003/swagger" -ForegroundColor White
Write-Host "  外部访问: http://47.96.238.102:3003/swagger" -ForegroundColor Green

Write-Host "`n🎯 一键创建防火墙规则脚本:" -ForegroundColor Yellow
$firewallScript = @"
# 在远程服务器上运行此命令 (需要管理员权限)
New-NetFirewallRule -DisplayName "SmartElderlyCare-Port3003" -Direction Inbound -Protocol TCP -LocalPort 3003 -Action Allow
Write-Host "防火墙规则已添加!" -ForegroundColor Green
"@

$firewallScript | Out-File -FilePath "setup-firewall.ps1" -Encoding UTF8
Write-Host "✅ 防火墙配置脚本已生成: setup-firewall.ps1" -ForegroundColor Green

Write-Host "`n🎯 远程服务器启动脚本检查:" -ForegroundColor Yellow
if (Test-Path "start_server_fixed.ps1") {
    Write-Host "✅ 启动脚本存在: start_server_fixed.ps1" -ForegroundColor Green
} else {
    Write-Host "❌ 启动脚本不存在!" -ForegroundColor Red
}

if (Test-Path "RoomDeviceManagement.exe") {
    Write-Host "✅ 应用程序文件存在: RoomDeviceManagement.exe" -ForegroundColor Green
} else {
    Write-Host "❌ 应用程序文件不存在!" -ForegroundColor Red
}

Write-Host "`n⚠️  重要提醒:" -ForegroundColor Yellow
Write-Host "  1. 确保远程服务器已安装 .NET 8.0 Runtime" -ForegroundColor White
Write-Host "  2. 确保Oracle数据库连接配置正确" -ForegroundColor White
Write-Host "  3. 启动应用后检查进程: Get-Process | Where-Object {`$_.Name -like '*Room*'}" -ForegroundColor White

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "部署向导完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
