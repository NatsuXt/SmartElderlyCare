#!/bin/bash#!/bin/bash#!/bin/bash#!/bin/bash



echo "============================================"

echo "  Smart Elderly Care System - Server v2.0"

echo "============================================"echo "============================================"# 智慧养老系统启动脚本 (Linux/macOS)

echo ""

echo "  Smart Elderly Care System - Server v2.0"

# Set environment variables

export ASPNETCORE_ENVIRONMENT=Productionecho "============================================"echo "启动智慧养老系统..."

export ASPNETCORE_URLS=http://0.0.0.0:3003

echo ""

echo "[INFO] Starting Smart Elderly Care System Server..."

echo "[INFO] Environment: $ASPNETCORE_ENVIRONMENT"echo ""echo "============================================"

echo "[INFO] Server Port: 3003"

echo "[INFO] API Documentation: http://localhost:3003/swagger"# Set environment variables

echo ""

export ASPNETCORE_ENVIRONMENT=Productionecho "   智慧养老系统 - 房间与设备管理模块 v2.0"

# Check if .NET Runtime is available

echo "[INFO] Checking .NET Runtime..."export ASPNETCORE_URLS=http://0.0.0.0:3003

if ! command -v dotnet &> /dev/null; then

    echo "[ERROR] .NET Runtime not found, please install .NET 8.0 or higher"# 设置环境变量echo "============================================"

    echo "[INFO] Download: https://dotnet.microsoft.com/download"

    exit 1echo "[INFO] Starting Smart Elderly Care System Server..."

fi

echo "[INFO] Environment: $ASPNETCORE_ENVIRONMENT"export ASPNETCORE_ENVIRONMENT=Productionecho

# Start the application

echo "[INFO] Starting application..."echo "[INFO] Server Port: 3003"

echo ""

dotnet RoomDeviceManagement.dllecho "[INFO] API Documentation: http://localhost:3003/swagger"export ASPNETCORE_URLS=http://0.0.0.0:3003



echo ""echo ""

echo "[INFO] Server stopped."
echo "[INFO] 启动智慧养老系统服务器..."

# Check if .NET Runtime is available

echo "[INFO] Checking .NET Runtime..."echo "环境: $ASPNETCORE_ENVIRONMENT"echo "[INFO] 服务端口: 3003"

if ! command -v dotnet &> /dev/null; then

    echo "[ERROR] .NET Runtime not found, please install .NET 8.0 or higher"echo "监听地址: $ASPNETCORE_URLS"echo "[INFO] API文档: http://localhost:3003/swagger"

    echo "[INFO] Download: https://dotnet.microsoft.com/download"

    exit 1echo ""echo

fi



# Start the application

echo "[INFO] Starting application..."# 检查.NET运行时echo "[INFO] 检查.NET运行时..."

echo ""

dotnet RoomDeviceManagement.dllif ! command -v dotnet &> /dev/null; thenif ! command -v dotnet &> /dev/null; then



echo ""    echo "错误: 未找到dotnet运行时，请先安装.NET 8.0 Runtime"    echo "[ERROR] 未找到.NET运行时，请先安装.NET 6.0或更高版本"

echo "[INFO] Server stopped."
    exit 1    echo "[INFO] 下载地址: https://dotnet.microsoft.com/download"

fi    exit 1

fi

echo "正在启动应用程序..."

dotnet RoomDeviceManagement.dllecho "[INFO] 启动应用程序..."
echo "[INFO] 按 Ctrl+C 停止服务器"
echo

dotnet RoomDeviceManagement.dll --urls "http://*:3003"

echo
echo "[INFO] 服务器已停止"
