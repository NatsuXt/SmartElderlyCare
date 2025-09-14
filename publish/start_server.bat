@echo off
chcp 65001 >nul
echo ============================================
echo   Smart Elderly Care System - Server v2.0
echo ============================================
echo.

REM Set environment variables
set ASPNETCORE_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://0.0.0.0:3003

echo [INFO] Starting Smart Elderly Care System Server...
echo [INFO] Environment: %ASPNETCORE_ENVIRONMENT%
echo [INFO] Server Port: 3003
echo [INFO] API Documentation: http://localhost:3003/swagger
echo.

REM Check if .NET Runtime is available
echo [INFO] Checking .NET Runtime...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET Runtime not found, please install .NET 8.0 or higher
    echo [INFO] Download: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Start the application
echo [INFO] Starting application...
echo.
RoomDeviceManagement.exe

echo.
echo [INFO] Server stopped. Press any key to exit...
pause >nul
