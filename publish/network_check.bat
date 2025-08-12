@echo off
chcp 65001 >nul
echo ========================================
echo 智慧养老系统 - 第三模块网络诊断工具
echo ========================================
echo.

echo 1. 检查当前网络配置...
ipconfig | findstr "IPv4"
echo.

echo 2. 检查端口占用情况...
echo 检查3003端口 (第三模块):
netstat -an | findstr ":3003"
echo 检查3004端口:
netstat -an | findstr ":3004"
echo 检查3005端口:
netstat -an | findstr ":3005"
echo 其他模块端口:
echo 检查5000端口 (第二模块):
netstat -an | findstr ":5000"
echo 检查7000端口 (第一模块):
netstat -an | findstr ":7000"
echo.

echo 3. 检查.NET应用常用端口...
netstat -an | findstr ":5000"
netstat -an | findstr ":5001" 
netstat -an | findstr ":7000"
netstat -an | findstr ":7001"
echo.

echo 4. 检查进程...
echo 检查dotnet进程:
tasklist | findstr "dotnet"
echo.

echo 5. 检查防火墙状态...
netsh advfirewall show allprofiles state
echo.

echo ========================================
echo 诊断完成
echo ========================================
echo.
echo 提示: 如果应用启动后显示了不同的端口，
echo 请使用显示的端口访问Swagger文档
echo 第三模块格式: http://47.96.238.102:[端口]/swagger
echo 推荐端口: 3003, 3004, 3005
echo.
echo 模块端口分配:
echo - 第一模块: 7000
echo - 第二模块: 5000
echo - 第三模块: 3003
echo.

pause
