# -*- coding: utf-8 -*-
# 智慧养老系统中文字符API测试脚本

Write-Host "正在测试中文字符API..." -ForegroundColor Cyan

# 准备中文测试数据
$roomData = @{
    roomNumber = "房间-$(Get-Date -Format 'HHmmss')"
    roomType = "豪华套房"
    capacity = 2
    status = "可用"
    rate = 288.50
    bedType = "双人床"
    floor = 3
    description = "舒适房间"
}

# 转换为JSON
$jsonData = $roomData | ConvertTo-Json -Depth 10
Write-Host "发送的JSON数据:" -ForegroundColor Yellow
Write-Host $jsonData -ForegroundColor White

# 设置请求头
$headers = @{
    "Content-Type" = "application/json; charset=utf-8"
    "Accept" = "application/json"
}

try {
    Write-Host "`n正在发送POST请求..." -ForegroundColor Cyan
    $response = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomManagement/rooms" -Method POST -Body ([System.Text.Encoding]::UTF8.GetBytes($jsonData)) -Headers $headers -ContentType "application/json; charset=utf-8"
    
    Write-Host "请求成功！" -ForegroundColor Green
    Write-Host "响应数据:" -ForegroundColor Yellow
    $response | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor White
    
    if ($response.success -and $response.data) {
        $roomId = $response.data.roomId
        Write-Host "`n正在验证数据，读取房间ID: $roomId" -ForegroundColor Cyan
        
        $getResponse = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomManagement/rooms/$roomId" -Method GET
        
        if ($getResponse.success) {
            Write-Host "获取到的房间信息:" -ForegroundColor Yellow
            $roomInfo = $getResponse.data
            Write-Host "  房间号: $($roomInfo.roomNumber)" -ForegroundColor White
            Write-Host "  房间类型: $($roomInfo.roomType)" -ForegroundColor White
            Write-Host "  床型: $($roomInfo.bedType)" -ForegroundColor White
            Write-Host "  状态: $($roomInfo.status)" -ForegroundColor White
            
            if ($roomInfo.roomType -eq "豪华套房") {
                Write-Host "`n✅ 中文字符API测试成功！" -ForegroundColor Green
            } else {
                Write-Host "`n❌ 中文字符有问题" -ForegroundColor Red
            }
        }
    }
    
} catch {
    Write-Host "请求失败:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host "`n测试完成！" -ForegroundColor Cyan
