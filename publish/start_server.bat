@echo off
REM 智慧养老系统 - 服务器启动脚本
REM 服务器: 47.96.238.102

echo ========================================
echo 智慧养老系统 - 服务器启动
echo ========================================
echo.

REM 设置Oracle环境变量
set NLS_LANG=SIMPLIFIED CHINESE_CHINA.AL32UTF8
set ORA_NCHAR_LITERAL_REPLACE=TRUE
set ASPNETCORE_ENVIRONMENT=Production

echo 设置Oracle字符编码环境...
echo NLS_LANG=%NLS_LANG%
echo ORA_NCHAR_LITERAL_REPLACE=%ORA_NCHAR_LITERAL_REPLACE%
echo ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT%
echo.

REM 使用当前目录作为应用目录（更灵活的部署方式）
echo 当前目录: %CD%
echo 提示: 可以将应用部署到任意目录，如桌面的 RoomManagement_Modules 文件夹
echo.

REM 检查.NET Runtime
echo 检查.NET Runtime...
dotnet --info
if errorlevel 1 (
    echo 错误: .NET 8.0 Runtime 未安装
    echo 请下载并安装: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)
echo.

REM 检查应用文件
if not exist "RoomDeviceManagement.dll" (
    echo 错误: 应用文件不存在
    echo 请确保已解压部署包到当前目录
    pause
    exit /b 1
)

echo 启动智慧养老系统...
echo 服务器地址: 47.96.238.102
echo 预期访问地址: http://47.96.238.102:8080/swagger
echo 注意: 如果8080端口被占用，应用可能会自动选择其他端口
echo 按 Ctrl+C 停止服务
echo.

REM 启动应用并显示详细输出
echo 正在启动应用...
dotnet RoomDeviceManagement.dll

echo.
echo 应用已停止
pause
