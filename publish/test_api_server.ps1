# 🔧 智慧养老系统 - 服务器API测试脚本

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "智慧养老系统 - API端点测试" -ForegroundColor Green  
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 设置服务器基础URL
$BaseUrl = "http://47.96.238.102:8080"
$ApiUrl = "$BaseUrl/api"

Write-Host "🎯 测试服务器: $BaseUrl" -ForegroundColor Yellow
Write-Host ""

# 测试Swagger文档
Write-Host "📚 测试Swagger文档..." -ForegroundColor Green
try {
    $SwaggerResponse = Invoke-WebRequest -Uri "$BaseUrl/swagger" -Method GET -TimeoutSec 10
    if ($SwaggerResponse.StatusCode -eq 200) {
        Write-Host "✅ Swagger文档访问成功" -ForegroundColor Green
        Write-Host "📋 访问地址: $BaseUrl/swagger" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ Swagger文档访问失败: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 测试所有API端点
$ApiTests = @(
    @{
        Name = "房间管理API"
        Endpoint = "/RoomManagement/rooms"
        Method = "GET"
        Description = "获取所有房间列表"
    },
    @{
        Name = "设备管理API"
        Endpoint = "/DeviceManagement/devices"
        Method = "GET" 
        Description = "获取所有设备列表"
    },
    @{
        Name = "健康监测API"
        Endpoint = "/HealthMonitoring/health-records"
        Method = "GET"
        Description = "获取健康记录"
    },
    @{
        Name = "电子围栏API"
        Endpoint = "/ElectronicFence/fences"
        Method = "GET"
        Description = "获取电子围栏列表"
    },
    @{
        Name = "IoT监控API"
        Endpoint = "/api/iot/monitoring/status"
        Method = "GET"
        Description = "获取IoT设备状态"
    }
)

Write-Host "🧪 开始API端点测试..." -ForegroundColor Green
Write-Host ""

foreach ($Test in $ApiTests) {
    Write-Host "测试: $($Test.Name)" -ForegroundColor Yellow
    Write-Host "描述: $($Test.Description)" -ForegroundColor White
    Write-Host "端点: $($Test.Endpoint)" -ForegroundColor Gray
    
    try {
        $TestUrl = "$ApiUrl$($Test.Endpoint)"
        $Response = Invoke-WebRequest -Uri $TestUrl -Method $Test.Method -TimeoutSec 10
        
        if ($Response.StatusCode -eq 200) {
            Write-Host "✅ 状态码: $($Response.StatusCode) - 成功" -ForegroundColor Green
        } elseif ($Response.StatusCode -eq 404) {
            Write-Host "⚠️  状态码: $($Response.StatusCode) - 端点未找到" -ForegroundColor Orange
        } else {
            Write-Host "ℹ️  状态码: $($Response.StatusCode)" -ForegroundColor Blue
        }
        
        # 显示响应内容长度
        $ContentLength = $Response.Content.Length
        Write-Host "📊 响应大小: $ContentLength 字节" -ForegroundColor Gray
        
    } catch {
        $ErrorMsg = $_.Exception.Message
        if ($ErrorMsg -like "*连接*" -or $ErrorMsg -like "*timeout*") {
            Write-Host "❌ 连接失败: 无法连接到服务器" -ForegroundColor Red
        } elseif ($ErrorMsg -like "*404*") {
            Write-Host "⚠️  端点未找到 (404)" -ForegroundColor Orange
        } else {
            Write-Host "❌ 请求失败: $ErrorMsg" -ForegroundColor Red
        }
    }
    
    Write-Host ""
}

# 服务器状态检查
Write-Host "🔍 服务器状态检查..." -ForegroundColor Green
try {
    $HealthCheck = Invoke-WebRequest -Uri "$BaseUrl/health" -Method GET -TimeoutSec 5 -ErrorAction SilentlyContinue
    if ($HealthCheck.StatusCode -eq 200) {
        Write-Host "✅ 服务器健康检查通过" -ForegroundColor Green
    }
} catch {
    Write-Host "ℹ️  健康检查端点未配置" -ForegroundColor Blue
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "测试完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 完整API文档: $BaseUrl/swagger" -ForegroundColor Cyan
Write-Host "🌐 基础API地址: $ApiUrl" -ForegroundColor Cyan
Write-Host ""

Read-Host "按任意键退出"
