# 智慧养老系统 - 部署验证测试脚本
# 版本: v2.0
# 用途: 验证系统部署是否成功

Write-Host "=== 🧪 智慧养老系统部署验证测试 ===" -ForegroundColor Green
Write-Host "版本: v2.0 - 房间入住管理功能完整版" -ForegroundColor Cyan
Write-Host "测试目标: http://47.96.238.102:3003" -ForegroundColor Yellow
Write-Host ""

# 测试函数
function Test-ApiEndpoint {
    param(
        [string]$Url,
        [string]$Description,
        [string]$Method = "GET"
    )
    
    try {
        $response = Invoke-RestMethod -Uri $Url -Method $Method -TimeoutSec 10
        Write-Host "✅ $Description" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ $Description - 错误: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

$baseUrl = "http://47.96.238.102:3003"
$successCount = 0
$totalTests = 0

Write-Host "🔍 开始测试..." -ForegroundColor Yellow
Write-Host ""

# 测试1: 系统健康检查
$totalTests++
if (Test-ApiEndpoint "$baseUrl/api/RoomOccupancy/test" "房间入住管理系统健康检查") {
    $successCount++
}

# 测试2: Swagger文档
$totalTests++
try {
    $swaggerResponse = Invoke-WebRequest -Uri "$baseUrl/swagger" -TimeoutSec 10
    if ($swaggerResponse.StatusCode -eq 200) {
        Write-Host "✅ Swagger API文档访问正常" -ForegroundColor Green
        $successCount++
    }
}
catch {
    Write-Host "❌ Swagger API文档访问失败" -ForegroundColor Red
}

# 测试3: 设备管理
$totalTests++
if (Test-ApiEndpoint "$baseUrl/api/DeviceManagement" "设备管理模块") {
    $successCount++
}

# 测试4: 房间管理
$totalTests++
if (Test-ApiEndpoint "$baseUrl/api/RoomManagement" "房间管理模块") {
    $successCount++
}

# 测试5: 账单记录查询
$totalTests++
if (Test-ApiEndpoint "$baseUrl/api/RoomOccupancy/billing/records?page=1&pageSize=5" "账单记录查询") {
    $successCount++
}

# 测试6: 入住记录查询
$totalTests++
if (Test-ApiEndpoint "$baseUrl/api/RoomOccupancy/all?page=1&pageSize=5" "入住记录查询") {
    $successCount++
}

Write-Host ""
Write-Host "=== 📊 测试结果汇总 ===" -ForegroundColor Blue
Write-Host "总测试数: $totalTests" -ForegroundColor White
Write-Host "成功测试: $successCount" -ForegroundColor Green
Write-Host "失败测试: $($totalTests - $successCount)" -ForegroundColor Red
Write-Host "成功率: $([math]::Round($successCount / $totalTests * 100, 2))%" -ForegroundColor Yellow

if ($successCount -eq $totalTests) {
    Write-Host ""
    Write-Host "🎉 部署验证完全成功！系统运行正常！" -ForegroundColor Green
    Write-Host "📍 API文档地址: $baseUrl/swagger" -ForegroundColor Cyan
    Write-Host "🏨 房间入住管理功能已就绪" -ForegroundColor Cyan
} elseif ($successCount -gt 0) {
    Write-Host ""
    Write-Host "⚠️  部分功能正常，请检查失败的模块" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "❌ 部署验证失败，请检查服务器状态和网络连接" -ForegroundColor Red
}

Write-Host ""
Write-Host "📋 主要功能模块:" -ForegroundColor Blue
Write-Host "   🏨 房间入住管理: /api/RoomOccupancy/* (12个端点)" -ForegroundColor White
Write-Host "   🏠 房间管理: /api/RoomManagement/* (6个端点)" -ForegroundColor White
Write-Host "   📱 设备管理: /api/DeviceManagement/* (6个端点)" -ForegroundColor White
Write-Host "   💓 健康监测: /api/HealthMonitoring/* (5个端点)" -ForegroundColor White
Write-Host "   🔒 电子围栏: /api/ElectronicFence/* (11个端点)" -ForegroundColor White
Write-Host "   🌐 IoT监控: /api/IoTMonitoring/* (5个端点)" -ForegroundColor White
