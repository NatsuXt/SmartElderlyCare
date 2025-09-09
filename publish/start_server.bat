@echo off
chcp 65001 >nul
echo ============================================
echo    智慧养老系统 - 房间与设备管理模块 v2.0
echo ============================================
echo.

echo [INFO] 启动智慧养老系统服务器...
echo [INFO] 服务端口: 3003
echo [INFO] API文档: http://localhost:3003/swagger
echo.

echo [INFO] 检查.NET运行时...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] 未找到.NET运行时，请先安装.NET 6.0或更高版本
    echo [INFO] 下载地址: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [INFO] 启动应用程序...
echo [INFO] 按 Ctrl+C 停止服务器
echo.

dotnet RoomDeviceManagement.dll --urls "http://*:3003"

echo.
echo [INFO] 服务器已停止
pause
