@echo off
echo ========================================
echo 智慧养老系统 - 网络诊断工具
echo ========================================
echo.

echo 1. 检查当前网络配置...
ipconfig | findstr "IPv4"
echo.

echo 2. 检查端口占用情况...
echo 检查8080端口:
netstat -an | findstr ":8080"
echo.

echo 3. 检查可能的服务端口...
echo 检查.NET应用常用端口:
netstat -an | findstr ":5000"
netstat -an | findstr ":5001" 
netstat -an | findstr ":7000"
netstat -an | findstr ":7001"
echo.

echo 4. 检查进程...
echo 检查dotnet进程:
tasklist | findstr "dotnet"
echo.

echo ========================================
echo 诊断完成
echo ========================================
echo.
echo 提示: 如果应用启动后显示了不同的端口，
echo 请使用显示的端口访问Swagger文档
echo.

pause
