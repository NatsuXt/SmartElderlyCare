@echo off
echo 正在清理第三模块端口占用...

REM 清理端口3003
echo 清理端口3003 (第三模块)...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3003"') do (
    echo 结束进程: %%a
    taskkill /F /PID %%a >nul 2>&1
)

REM 清理端口3004
echo 清理端口3004...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3004"') do (
    echo 结束进程: %%a
    taskkill /F /PID %%a >nul 2>&1
)

REM 清理端口3005
echo 清理端口3005...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3005"') do (
    echo 结束进程: %%a
    taskkill /F /PID %%a >nul 2>&1
)

REM 清理端口5000（备用清理）
echo 清理端口5000...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5000"') do (
    echo 结束进程: %%a
    taskkill /F /PID %%a >nul 2>&1
)

echo 第三模块端口清理完成
pause
