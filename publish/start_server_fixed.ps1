# 智慧养老系统 - 第三模块修复版启动脚本

try {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "智慧养老系统 - 第三模块启动" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""

    # 设置环境变量
    $env:NLS_LANG = "SIMPLIFIED CHINESE_CHINA.AL32UTF8"
    $env:ORA_NCHAR_LITERAL_REPLACE = "TRUE"
    $env:ASPNETCORE_ENVIRONMENT = "Production"

    Write-Host "设置环境变量完成" -ForegroundColor Green
    Write-Host "NLS_LANG: $env:NLS_LANG" -ForegroundColor White
    Write-Host "ASPNETCORE_ENVIRONMENT: $env:ASPNETCORE_ENVIRONMENT" -ForegroundColor White
    Write-Host ""

    # 检查.NET Runtime
    Write-Host "检查.NET Runtime..." -ForegroundColor Yellow
    $dotnetVersion = & dotnet --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ .NET Runtime 版本: $dotnetVersion" -ForegroundColor Green
    } else {
        Write-Host "❌ .NET Runtime 未安装" -ForegroundColor Red
        Write-Host "请下载安装: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
        Read-Host "按任意键退出"
        exit 1
    }
    Write-Host ""

    # 检查应用文件
    if (-not (Test-Path "RoomDeviceManagement.dll")) {
        Write-Host "❌ 应用文件不存在" -ForegroundColor Red
        Write-Host "请确保在正确的目录中运行此脚本" -ForegroundColor Yellow
        Read-Host "按任意键退出"
        exit 1
    }
    Write-Host "✅ 应用文件检查通过" -ForegroundColor Green
    Write-Host ""

    # 检查端口3003占用情况
    Write-Host "检查端口占用情况..." -ForegroundColor Yellow
    $port3003 = Get-NetTCPConnection -LocalPort 3003 -ErrorAction SilentlyContinue
    if ($port3003) {
        Write-Host "⚠️  端口3003被占用，尝试清理..." -ForegroundColor Orange
        $port3003 | ForEach-Object {
            try {
                Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue
                Write-Host "已清理进程: $($_.OwningProcess)" -ForegroundColor Yellow
            } catch {
                Write-Host "无法清理进程: $($_.OwningProcess)" -ForegroundColor Red
            }
        }
    }

    Write-Host ""
    Write-Host "🚀 启动智慧养老系统..." -ForegroundColor Green
    Write-Host "服务器IP: 47.96.238.102" -ForegroundColor Cyan
    Write-Host "第三模块端口: 3003" -ForegroundColor Cyan
    Write-Host "监听模式: 监听所有IP地址 (支持外部访问)" -ForegroundColor Green
    Write-Host "如果3003被占用，系统会自动选择其他可用端口" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "请注意控制台输出的实际访问地址！" -ForegroundColor Yellow
    Write-Host "按 Ctrl+C 停止服务" -ForegroundColor Yellow
    Write-Host ""

    # 尝试启动应用 - 监听所有IP地址
    try {
        & dotnet "RoomDeviceManagement.dll" --urls="http://*:3003"
    } catch {
        Write-Host ""
        Write-Host "❌ 端口3003启动失败，尝试其他端口..." -ForegroundColor Orange
        try {
            Write-Host "尝试端口3004..." -ForegroundColor Yellow
            & dotnet "RoomDeviceManagement.dll" --urls="http://*:3004"
        } catch {
            Write-Host "尝试端口3005..." -ForegroundColor Yellow
            & dotnet "RoomDeviceManagement.dll" --urls="http://*:3005"
        }
    }

} catch {
    Write-Host ""
    Write-Host "❌ 启动失败: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    Write-Host ""
    Write-Host "应用已停止" -ForegroundColor Yellow
    Read-Host "按任意键退出"
}
