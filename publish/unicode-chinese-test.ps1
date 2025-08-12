# Chinese Character API Test Script
# Tests Chinese character handling using Unicode escape sequences

Write-Host "Testing Chinese Character API..." -ForegroundColor Cyan

# Create Chinese text using Unicode escapes to avoid encoding issues
$chineseRoomType = [char]0x8C6A + [char]0x534E + [char]0x5957 + [char]0x623F  # 豪华套房
$chineseBedType = [char]0x53CC + [char]0x4EBA + [char]0x5E8A  # 双人床  
$chineseStatus = [char]0x53EF + [char]0x7528  # 可用
$chineseDesc = [char]0x8212 + [char]0x9002 + [char]0x623F + [char]0x95F4  # 舒适房间

# Prepare test data with Chinese characters
$roomData = @{
    roomNumber = "Room-$(Get-Date -Format 'HHmmss')"
    roomType = $chineseRoomType
    capacity = 2
    status = $chineseStatus
    rate = 288.50
    bedType = $chineseBedType
    floor = 3
    description = $chineseDesc
}

# Convert to JSON
$jsonData = $roomData | ConvertTo-Json -Depth 10
Write-Host "Sending JSON data:" -ForegroundColor Yellow
Write-Host $jsonData -ForegroundColor White

# Set headers with UTF-8 charset
$headers = @{
    "Content-Type" = "application/json; charset=utf-8"
    "Accept" = "application/json"
}

try {
    Write-Host "`nSending POST request..." -ForegroundColor Cyan
    $response = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomManagement/rooms" -Method POST -Body ([System.Text.Encoding]::UTF8.GetBytes($jsonData)) -Headers $headers -ContentType "application/json; charset=utf-8"
    
    Write-Host "Request successful!" -ForegroundColor Green
    Write-Host "Response data:" -ForegroundColor Yellow
    $response | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor White
    
    if ($response.success -and $response.data) {
        $roomId = $response.data.roomId
        Write-Host "`nVerifying data, reading room ID: $roomId" -ForegroundColor Cyan
        
        $getResponse = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomManagement/rooms/$roomId" -Method GET
        
        if ($getResponse.success) {
            Write-Host "Retrieved room information:" -ForegroundColor Yellow
            $roomInfo = $getResponse.data
            Write-Host "  Room Number: $($roomInfo.roomNumber)" -ForegroundColor White
            Write-Host "  Room Type: $($roomInfo.roomType)" -ForegroundColor White
            Write-Host "  Bed Type: $($roomInfo.bedType)" -ForegroundColor White
            Write-Host "  Status: $($roomInfo.status)" -ForegroundColor White
            Write-Host "  Description: $($roomInfo.description)" -ForegroundColor White
            
            # Check if Chinese characters are correctly stored and retrieved
            if ($roomInfo.roomType -eq $chineseRoomType -and $roomInfo.bedType -eq $chineseBedType -and $roomInfo.status -eq $chineseStatus) {
                Write-Host "`nChinese Character API Test SUCCESSFUL!" -ForegroundColor Green
                Write-Host "Chinese characters correctly transmitted and stored" -ForegroundColor Green
            } else {
                Write-Host "`nChinese characters may have issues" -ForegroundColor Red
                Write-Host "Expected: $chineseRoomType, $chineseBedType, $chineseStatus" -ForegroundColor Yellow
                Write-Host "Got: $($roomInfo.roomType), $($roomInfo.bedType), $($roomInfo.status)" -ForegroundColor Yellow
            }
        }
    }
    
} catch {
    Write-Host "Request failed:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Error response: $responseBody" -ForegroundColor Yellow
    }
}

Write-Host "`nTest completed!" -ForegroundColor Cyan
Write-Host "Note: Make sure server is running and listening on port 3003" -ForegroundColor Yellow
