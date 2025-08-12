# Safe Test Script for Smart Elderly Care System

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Smart Elderly Care System - Safe Test" -ForegroundColor Green  
Write-Host "============================================" -ForegroundColor Cyan

Write-Host "`nStarting server for testing..." -ForegroundColor Yellow

# Start server process
$process = Start-Process -FilePath ".\RoomDeviceManagement.exe" -ArgumentList "--urls=http://localhost:3003" -WindowStyle Hidden -PassThru

# Wait for server startup
Start-Sleep -Seconds 3

# Check process status
if ($process -and !$process.HasExited) {
    Write-Host "Server started successfully!" -ForegroundColor Green
    
    # Run Chinese character test
    Write-Host "`nRunning Chinese character API test..." -ForegroundColor Cyan
    try {
        .\unicode-chinese-test.ps1
        Write-Host "`nChinese character test completed!" -ForegroundColor Green
    } catch {
        Write-Host "`nChinese character test failed: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    # Safely stop server
    Write-Host "`nSafely stopping server..." -ForegroundColor Yellow
    try {
        if ($process -and !$process.HasExited) {
            $process.Kill()
            $process.WaitForExit(5000)
        }
        Write-Host "Server stopped safely" -ForegroundColor Green
    } catch {
        Write-Host "Server may have already exited" -ForegroundColor Yellow
    }
} else {
    Write-Host "Server failed to start" -ForegroundColor Red
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "  1. Port 3003 availability" -ForegroundColor White
    Write-Host "  2. .NET 8.0 Runtime installation" -ForegroundColor White
    Write-Host "  3. Configuration files" -ForegroundColor White
}

Write-Host "`n============================================" -ForegroundColor Cyan
Write-Host "Test completed!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
