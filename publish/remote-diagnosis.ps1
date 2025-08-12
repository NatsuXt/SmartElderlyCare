# Remote Server Diagnosis Script for Smart Elderly Care System

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Remote Server Deployment Diagnosis" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

$ServerIP = "47.96.238.102"

Write-Host "`n1. Checking server connectivity..." -ForegroundColor Yellow

# Check RDP connection
Write-Host "Checking RDP connection (port 3389)..." -ForegroundColor Cyan
$rdpTest = Test-NetConnection -ComputerName $ServerIP -Port 3389 -WarningAction SilentlyContinue
if ($rdpTest.TcpTestSucceeded) {
    Write-Host "RDP connection: SUCCESS" -ForegroundColor Green
} else {
    Write-Host "RDP connection: FAILED" -ForegroundColor Red
}

# Check target port 3003
Write-Host "`nChecking target port 3003..." -ForegroundColor Cyan
$httpTest = Test-NetConnection -ComputerName $ServerIP -Port 3003 -WarningAction SilentlyContinue
if ($httpTest.TcpTestSucceeded) {
    Write-Host "Port 3003: ACCESSIBLE" -ForegroundColor Green
} else {
    Write-Host "Port 3003: NOT ACCESSIBLE - This is the problem!" -ForegroundColor Red
}

# Check common web ports
Write-Host "`nChecking other common ports..." -ForegroundColor Cyan
$ports = @(80, 443, 8080, 5000)
foreach ($port in $ports) {
    $test = Test-NetConnection -ComputerName $ServerIP -Port $port -WarningAction SilentlyContinue
    if ($test.TcpTestSucceeded) {
        Write-Host "Port $port : ACCESSIBLE" -ForegroundColor Green
    } else {
        Write-Host "Port $port : NOT ACCESSIBLE" -ForegroundColor Red
    }
}

# Try HTTP request
Write-Host "`n2. Testing HTTP requests..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://$ServerIP:3003" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "HTTP request: SUCCESS, Status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "HTTP request: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Try Swagger page
Write-Host "`nTesting Swagger page..." -ForegroundColor Cyan
try {
    $swaggerResponse = Invoke-WebRequest -Uri "http://$ServerIP:3003/swagger" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "Swagger page: ACCESSIBLE, Status: $($swaggerResponse.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "Swagger page: NOT ACCESSIBLE - $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Diagnosis Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`nPossible Issues and Solutions:" -ForegroundColor Yellow
Write-Host "1. Application not started on remote server" -ForegroundColor White
Write-Host "2. Firewall blocking port 3003" -ForegroundColor White
Write-Host "3. Application configuration error" -ForegroundColor White
Write-Host "4. .NET Runtime not installed" -ForegroundColor White

Write-Host "`nNext Steps:" -ForegroundColor Yellow
Write-Host "1. Connect to server via Remote Desktop" -ForegroundColor White
Write-Host "2. Check if application is running" -ForegroundColor White
Write-Host "3. Configure firewall rules" -ForegroundColor White
Write-Host "4. Check application logs" -ForegroundColor White
