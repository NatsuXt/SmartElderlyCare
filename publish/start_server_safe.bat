@echo off
chcp 65001 >nul
echo ========================================
echo 智慧养老系统 - 第三模块安全启动脚本
echo ========================================
echo.

REM 设置环境变量
set NLS_LANG=SIMPLIFIED CHINESE_CHINA.AL32UTF8
set ORA_NCHAR_LITERAL_REPLACE=TRUE
set ASPNETCORE_ENVIRONMENT=Production

echo 设置环境变量完成
echo NLS_LANG=%NLS_LANG%
echo ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT%
echo.

REM 检查.NET Runtime
echo 检查.NET Runtime...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ 错误: .NET Runtime 未安装
    echo 请下载安装: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)
echo ✅ .NET Runtime 已安装
echo.

REM 检查应用文件
if not exist "RoomDeviceManagement.dll" (
    echo ❌ 错误: 应用文件不存在
    echo 请确保在正确的目录中运行此脚本
    pause
    exit /b 1
)
echo ✅ 应用文件检查通过
echo.

REM 检查并清理端口3003（如果被占用）
echo 检查端口占用情况...
netstat -an | findstr ":3003" >nul 2>&1
if not errorlevel 1 (
    echo ⚠️  端口3003被占用，尝试清理...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3003"') do (
        echo 结束进程: %%a
        taskkill /F /PID %%a >nul 2>&1
    )
)

echo.
echo 🚀 启动智慧养老系统...
echo 服务器IP: 47.96.238.102
echo 第三模块端口: 3003
echo 监听模式: 监听所有IP地址 (支持外部访问)
echo 如果3003被占用，系统会自动选择其他可用端口
echo.
echo 请注意控制台输出的实际访问地址！
echo 按 Ctrl+C 停止服务
echo.

REM 启动应用 - 监听所有IP地址
dotnet RoomDeviceManagement.dll --urls="http://*:3003"
if errorlevel 1 (
    echo.
    echo ❌ 应用启动失败，尝试使用其他端口...
    echo 尝试端口3004...
    dotnet RoomDeviceManagement.dll --urls="http://*:3004"
    if errorlevel 1 (
        echo 尝试端口3005...
        dotnet RoomDeviceManagement.dll --urls="http://*:3005"
    )
)

echo.
echo 应用已停止
pause
