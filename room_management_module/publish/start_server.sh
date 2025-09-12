#!/bin/bash

# 智慧养老系统 - 房间入住管理模块启动脚本
# 版本: v2.0
# 构建日期: 2025年9月2日

echo "=== 🚀 智慧养老系统启动脚本 v2.0 ==="
echo "功能模块: 房间入住管理 + 设备管理 + 健康监测 + 电子围栏"
echo "构建时间: 2025年9月2日 10:45"
echo ""

# 检查.NET环境
echo "🔍 检查.NET运行环境..."
if command -v dotnet &> /dev/null; then
    dotnet --version
    echo "✅ .NET环境检查通过"
else
    echo "❌ 未找到.NET运行环境，请先安装.NET 8.0 Runtime"
    exit 1
fi

echo ""

# 检查端口占用
echo "🔍 检查端口3003占用情况..."
if lsof -Pi :3003 -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo "⚠️  端口3003已被占用，尝试停止现有进程..."
    sudo kill -9 $(lsof -Pi :3003 -sTCP:LISTEN -t) 2>/dev/null || true
    sleep 2
fi

echo "✅ 端口3003可用"
echo ""

# 启动应用
echo "🚀 启动智慧养老系统..."
echo "📍 访问地址: http://47.96.238.102:3003/swagger"
echo "🏨 房间入住管理: /api/RoomOccupancy/*"
echo ""

# 设置环境变量
export ASPNETCORE_ENVIRONMENT=Production
export DOTNET_URLS="http://*:3003"

# 启动应用
dotnet RoomDeviceManagement.dll

echo ""
echo "📋 如需后台运行，请使用:"
echo "nohup dotnet RoomDeviceManagement.dll > app.log 2>&1 &"
