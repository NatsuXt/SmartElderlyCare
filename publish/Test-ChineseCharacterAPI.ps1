# 智慧养老系统第三模块 - PowerShell中文字符测试脚本
# 基于成功验证的API调用方法

param(
    [string]$ServerUrl = "http://47.96.238.102:3003"
)

# 确保PowerShell使用UTF-8编码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "🧪 智慧养老系统第三模块 - 中文字符API测试" -ForegroundColor Cyan
Write-Host "服务器地址: $ServerUrl" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Cyan

# 创建HTTP客户端的正确方法
function Invoke-ChineseApiRequest {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [hashtable]$Body = @{},
        [hashtable]$Headers = @{}
    )
    
    # 设置正确的头部以支持中文字符
    $defaultHeaders = @{
        "Content-Type" = "application/json; charset=utf-8"
        "Accept" = "application/json"
    }
    
    # 合并头部
    $allHeaders = $defaultHeaders + $Headers
    
    try {
        if ($Method -eq "POST" -or $Method -eq "PUT") {
            # 确保JSON正确序列化中文字符
            $jsonBody = $Body | ConvertTo-Json -Depth 10 -Compress:$false
            $utf8Body = [System.Text.Encoding]::UTF8.GetBytes($jsonBody)
            
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Body $utf8Body -Headers $allHeaders -ContentType "application/json; charset=utf-8"
        } else {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Headers $allHeaders
        }
        
        return $response
    } catch {
        Write-Host "❌ API请求失败: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "响应内容: $responseBody" -ForegroundColor Yellow
        }
        throw
    }
}

# 测试房间管理API - 中文字符支持
function Test-RoomManagementChinese {
    Write-Host "`n🏠 测试房间管理API中文字符支持" -ForegroundColor Yellow
    
    $timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
    $roomData = @{
        roomNumber = "豪华套房-$timestamp"
        roomType = "豪华套房"
        capacity = 2
        status = "空闲"
        rate = 288.50
        bedType = "双人大床"
        floor = 3
    }
    
    try {
        # 创建房间
        Write-Host "正在创建房间: $($roomData.roomNumber)" -ForegroundColor Cyan
        $createResponse = Invoke-ChineseApiRequest -Url "$ServerUrl/api/RoomManagement/rooms" -Method "POST" -Body $roomData
        
        if ($createResponse.success) {
            Write-Host "✅ 房间创建成功!" -ForegroundColor Green
            Write-Host "房间ID: $($createResponse.data.roomId)" -ForegroundColor Green
            
            # 立即读取验证
            $roomId = $createResponse.data.roomId
            Write-Host "正在验证中文字符..." -ForegroundColor Cyan
            $getResponse = Invoke-ChineseApiRequest -Url "$ServerUrl/api/RoomManagement/rooms/$roomId" -Method "GET"
            
            if ($getResponse.success) {
                $roomInfo = $getResponse.data
                Write-Host "📖 读取到的房间信息:" -ForegroundColor Cyan
                Write-Host "  房间号: $($roomInfo.roomNumber)" -ForegroundColor White
                Write-Host "  房间类型: $($roomInfo.roomType)" -ForegroundColor White
                Write-Host "  床型: $($roomInfo.bedType)" -ForegroundColor White
                Write-Host "  状态: $($roomInfo.status)" -ForegroundColor White
                
                # 验证中文字符
                if ($roomInfo.roomType -eq "豪华套房" -and $roomInfo.bedType -eq "双人大床" -and $roomInfo.status -eq "空闲") {
                    Write-Host "🎉 中文字符完美支持!" -ForegroundColor Green
                } else {
                    Write-Host "❌ 中文字符可能有问题" -ForegroundColor Red
                    Write-Host "期望: roomType='豪华套房', bedType='双人大床', status='空闲'" -ForegroundColor Yellow
                    Write-Host "实际: roomType='$($roomInfo.roomType)', bedType='$($roomInfo.bedType)', status='$($roomInfo.status)'" -ForegroundColor Yellow
                }
            }
        } else {
            Write-Host "❌ 房间创建失败: $($createResponse.message)" -ForegroundColor Red
        }
        
        return $createResponse
    } catch {
        Write-Host "❌ 房间管理测试失败: $($_.Exception.Message)" -ForegroundColor Red
        throw
    }
}

# 测试设备管理API - 中文字符支持
function Test-DeviceManagementChinese {
    Write-Host "`n📱 测试设备管理API中文字符支持" -ForegroundColor Yellow
    
    $timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
    $deviceData = @{
        deviceName = "智能血压监测仪-$timestamp"
        deviceType = "医疗监测设备"
        location = "二楼护士站"
        status = "正常运行"
        description = "专业医疗级血压监测设备，支持中文显示"
        installationDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
    }
    
    try {
        Write-Host "正在创建设备: $($deviceData.deviceName)" -ForegroundColor Cyan
        $createResponse = Invoke-ChineseApiRequest -Url "$ServerUrl/api/DeviceManagement/devices" -Method "POST" -Body $deviceData
        
        if ($createResponse.success) {
            Write-Host "✅ 设备创建成功!" -ForegroundColor Green
            Write-Host "设备ID: $($createResponse.data.deviceId)" -ForegroundColor Green
            
            # 读取验证
            $deviceId = $createResponse.data.deviceId
            $getResponse = Invoke-ChineseApiRequest -Url "$ServerUrl/api/DeviceManagement/devices/$deviceId" -Method "GET"
            
            if ($getResponse.success) {
                $deviceInfo = $getResponse.data
                Write-Host "📖 读取到的设备信息:" -ForegroundColor Cyan
                Write-Host "  设备名称: $($deviceInfo.deviceName)" -ForegroundColor White
                Write-Host "  设备类型: $($deviceInfo.deviceType)" -ForegroundColor White
                Write-Host "  位置: $($deviceInfo.location)" -ForegroundColor White
                Write-Host "  状态: $($deviceInfo.status)" -ForegroundColor White
                
                # 验证中文字符
                if ($deviceInfo.deviceType -eq "医疗监测设备" -and $deviceInfo.location -eq "二楼护士站" -and $deviceInfo.status -eq "正常运行") {
                    Write-Host "🎉 设备中文字符完美支持!" -ForegroundColor Green
                } else {
                    Write-Host "❌ 设备中文字符可能有问题" -ForegroundColor Red
                }
            }
        } else {
            Write-Host "❌ 设备创建失败: $($createResponse.message)" -ForegroundColor Red
        }
        
        return $createResponse
    } catch {
        Write-Host "❌ 设备管理测试失败: $($_.Exception.Message)" -ForegroundColor Red
        throw
    }
}

# 测试健康监测API - 中文字符支持
function Test-HealthMonitoringChinese {
    Write-Host "`n💓 测试健康监测API中文字符支持" -ForegroundColor Yellow
    
    $healthData = @{
        elderlyName = "张三"
        checkupType = "常规体检"
        healthStatus = "良好"
        notes = "血压正常，心率稳定，建议定期复查"
        checkupDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
    }
    
    try {
        Write-Host "正在创建健康记录: $($healthData.elderlyName)" -ForegroundColor Cyan
        $createResponse = Invoke-ChineseApiRequest -Url "$ServerUrl/api/HealthMonitoring/records" -Method "POST" -Body $healthData
        
        if ($createResponse.success) {
            Write-Host "✅ 健康记录创建成功!" -ForegroundColor Green
            Write-Host "记录ID: $($createResponse.data.recordId)" -ForegroundColor Green
        } else {
            Write-Host "❌ 健康记录创建失败: $($createResponse.message)" -ForegroundColor Red
        }
        
        return $createResponse
    } catch {
        Write-Host "❌ 健康监测测试失败: $($_.Exception.Message)" -ForegroundColor Red
        throw
    }
}

# 测试电子围栏API - 中文字符支持
function Test-ElectronicFenceChinese {
    Write-Host "`n🔒 测试电子围栏API中文字符支持" -ForegroundColor Yellow
    
    $timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
    $fenceData = @{
        fenceName = "医院安全区域-$timestamp"
        areaDefinition = "医院大楼及周边50米范围，包含门诊部、住院部、花园区域"
        isActive = $true
        status = "正常运行"
    }
    
    try {
        Write-Host "正在创建电子围栏: $($fenceData.fenceName)" -ForegroundColor Cyan
        $createResponse = Invoke-ChineseApiRequest -Url "$ServerUrl/api/ElectronicFence/fences" -Method "POST" -Body $fenceData
        
        if ($createResponse.success) {
            Write-Host "✅ 电子围栏创建成功!" -ForegroundColor Green
            Write-Host "围栏ID: $($createResponse.data.fenceId)" -ForegroundColor Green
        } else {
            Write-Host "❌ 电子围栏创建失败: $($createResponse.message)" -ForegroundColor Red
        }
        
        return $createResponse
    } catch {
        Write-Host "❌ 电子围栏测试失败: $($_.Exception.Message)" -ForegroundColor Red
        throw
    }
}

# 运行完整测试套件
function Start-CompleteChineseTest {
    Write-Host "`n🚀 开始完整中文字符测试套件" -ForegroundColor Cyan
    
    $testResults = @()
    
    try {
        # 测试房间管理
        $roomResult = Test-RoomManagementChinese
        $testResults += @{ Module = "房间管理"; Success = $roomResult.success; Message = $roomResult.message }
        Start-Sleep -Seconds 1
        
        # 测试设备管理
        $deviceResult = Test-DeviceManagementChinese
        $testResults += @{ Module = "设备管理"; Success = $deviceResult.success; Message = $deviceResult.message }
        Start-Sleep -Seconds 1
        
        # 测试健康监测
        $healthResult = Test-HealthMonitoringChinese
        $testResults += @{ Module = "健康监测"; Success = $healthResult.success; Message = $healthResult.message }
        Start-Sleep -Seconds 1
        
        # 测试电子围栏
        $fenceResult = Test-ElectronicFenceChinese
        $testResults += @{ Module = "电子围栏"; Success = $fenceResult.success; Message = $fenceResult.message }
        
        # 生成测试报告
        Write-Host "`n📊 测试结果报告" -ForegroundColor Cyan
        Write-Host "===============================================" -ForegroundColor Cyan
        
        foreach ($result in $testResults) {
            $status = if ($result.Success) { "✅ 成功" } else { "❌ 失败" }
            $color = if ($result.Success) { "Green" } else { "Red" }
            Write-Host "$($result.Module): $status - $($result.Message)" -ForegroundColor $color
        }
        
        $successCount = ($testResults | Where-Object { $_.Success }).Count
        $totalCount = $testResults.Count
        
        Write-Host "`n🎯 总体结果: $successCount/$totalCount 模块测试成功" -ForegroundColor Cyan
        
        if ($successCount -eq $totalCount) {
            Write-Host "🎉 所有中文字符测试通过！前端可以放心使用API！" -ForegroundColor Green
        } else {
            Write-Host "⚠️ 部分测试失败，请检查API服务器配置" -ForegroundColor Yellow
        }
        
    } catch {
        Write-Host "❌ 测试过程中出现错误: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# 检查服务器连接
function Test-ServerConnection {
    Write-Host "🔍 检查服务器连接..." -ForegroundColor Cyan
    
    try {
        $response = Invoke-RestMethod -Uri "$ServerUrl/swagger" -Method GET -TimeoutSec 10
        Write-Host "✅ 服务器连接正常" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "❌ 无法连接到服务器: $ServerUrl" -ForegroundColor Red
        Write-Host "请确保服务器正在运行并且端口3003可访问" -ForegroundColor Yellow
        return $false
    }
}

# 主执行流程
Write-Host "开始中文字符API测试..." -ForegroundColor Green

if (Test-ServerConnection) {
    Start-CompleteChineseTest
} else {
    Write-Host "请先启动服务器后再运行测试" -ForegroundColor Red
    exit 1
}

Write-Host "`n✨ 测试完成！" -ForegroundColor Green
