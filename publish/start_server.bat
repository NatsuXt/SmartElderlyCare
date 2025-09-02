@echo off
REM 智慧养老系统 - 房间入住管理模块启动脚本 (Windows版本)
REM 版本: v2.0
REM 构建日期: 2025年9月2日

echo === 🚀 智慧养老系统启动脚本 v2.0 ===
echo 功能模块: 房间入住管理 + 设备管理 + 健康监测 + 电子围栏
echo 构建时间: 2025年9月2日 10:45
echo.

REM 检查.NET环境
echo 🔍 检查.NET运行环境...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ 未找到.NET运行环境，请先安装.NET 8.0 Runtime
    pause
    exit /b 1
)

dotnet --version
echo ✅ .NET环境检查通过
echo.

REM 检查端口占用
echo 🔍 检查端口3003占用情况...
netstat -an | find "3003" | find "LISTENING" >nul 2>&1
if %errorlevel% equ 0 (
    echo ⚠️ 端口3003已被占用，请手动停止占用端口的进程
    echo 可以使用命令: netstat -ano | findstr :3003
    echo 然后使用: taskkill /PID <进程ID> /F
    pause
)

echo ✅ 端口3003可用
echo.

REM 启动应用
echo 🚀 启动智慧养老系统...
echo 📍 访问地址: http://47.96.238.102:3003/swagger
echo 🏨 房间入住管理: /api/RoomOccupancy/*
echo.

REM 设置环境变量
set ASPNETCORE_ENVIRONMENT=Production
set DOTNET_URLS=http://*:3003

REM 启动应用
dotnet RoomDeviceManagement.dll

echo.
echo 📋 应用已停止运行
pause
