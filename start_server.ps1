# 智慧养老系统 - 服务器启动脚本 (PowerShell版本)
# 服务器: 47.96.238.102

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "智慧养老系统 - 服务器启动" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 设置Oracle环境变量
$env:NLS_LANG = "SIMPLIFIED CHINESE_CHINA.AL32UTF8"
$env:ORA_NCHAR_LITERAL_REPLACE = "TRUE"

Write-Host "设置Oracle字符编码环境..." -ForegroundColor Yellow
Write-Host "NLS_LANG: $env:NLS_LANG" -ForegroundColor White
Write-Host "ORA_NCHAR_LITERAL_REPLACE: $env:ORA_NCHAR_LITERAL_REPLACE" -ForegroundColor White
Write-Host ""

# 进入应用目录
$AppPath = "C:\SmartElderlyCare"
if (Test-Path $AppPath) {
    Set-Location $AppPath
    Write-Host "当前目录: $(Get-Location)" -ForegroundColor White
} else {
    Write-Host "错误: 应用目录不存在 - $AppPath" -ForegroundColor Red
    Write-Host "请确保已按照部署指南创建应用目录并解压文件" -ForegroundColor Yellow
    Read-Host "按任意键退出"
    exit 1
}
Write-Host ""

# 检查.NET Runtime
Write-Host "检查.NET Runtime..." -ForegroundColor Yellow
try {
    $dotnetInfo = dotnet --info 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ .NET Runtime 已安装" -ForegroundColor Green
    } else {
        throw "dotnet command failed"
    }
} catch {
    Write-Host "❌ 错误: .NET 8.0 Runtime 未安装" -ForegroundColor Red
    Write-Host "请下载并安装: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    Read-Host "按任意键退出"
    exit 1
}
Write-Host ""

# 检查应用文件
$AppFile = "RoomDeviceManagement.dll"
if (-not (Test-Path $AppFile)) {
    Write-Host "❌ 错误: 应用文件不存在 - $AppFile" -ForegroundColor Red
    Write-Host "请确保已解压部署包到当前目录" -ForegroundColor Yellow
    Read-Host "按任意键退出"
    exit 1
}

# 检查配置文件
$ConfigFile = "appsettings.json"
if (Test-Path $ConfigFile) {
    Write-Host "✅ 配置文件检查通过" -ForegroundColor Green
} else {
    Write-Host "⚠️  警告: 配置文件不存在，将使用默认配置" -ForegroundColor Orange
}
Write-Host ""

# 显示启动信息
Write-Host "🚀 启动智慧养老系统..." -ForegroundColor Green
Write-Host "📍 服务器地址: 47.96.238.102" -ForegroundColor Cyan
Write-Host "🌐 访问地址: http://47.96.238.102:8080/swagger" -ForegroundColor Cyan
Write-Host "📋 API文档: 启动后可通过上述地址查看完整API文档" -ForegroundColor White
Write-Host "⏹️  停止服务: 按 Ctrl+C" -ForegroundColor Yellow
Write-Host ""

# 启动应用
try {
    dotnet $AppFile
} catch {
    Write-Host "❌ 应用启动失败: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "按任意键退出"
    exit 1
}

Write-Host ""
Write-Host "应用已停止" -ForegroundColor Yellow
Read-Host "按任意键退出"
