@echo off
echo ============================================
echo   Smart Elderly Care System - Room Management v2.0
echo ============================================
echo.

echo [INFO] Starting Smart Elderly Care System Server...
echo [INFO] Server Port: 3003
echo [INFO] API Documentation: http://localhost:3003/swagger
echo.

echo [INFO] Checking .NET Runtime...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET Runtime not found, please install .NET 6.0 or higher
    echo [INFO] Download: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [INFO] Starting application...
echo [INFO] Press Ctrl+C to stop server
echo.

dotnet RoomDeviceManagement.dll --urls "http://*:3003"

echo.
echo [INFO] Server stopped
pause
