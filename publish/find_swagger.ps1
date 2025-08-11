# 🔍 智慧养老系统 - 端口自动发现脚本

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "智慧养老系统 - 端口自动发现" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 获取服务器IP
$ServerIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -ne "127.0.0.1"} | Select-Object -First 1).IPAddress
Write-Host "🌐 服务器IP地址: $ServerIP" -ForegroundColor Yellow
Write-Host ""

# 检查常用端口
$CommonPorts = @(5000, 5001, 7000, 7001, 8080, 8081, 8082)
$ActivePorts = @()

Write-Host "🔍 扫描活动端口..." -ForegroundColor Green
foreach ($Port in $CommonPorts) {
    try {
        $Connection = Test-NetConnection -ComputerName "localhost" -Port $Port -InformationLevel Quiet
        if ($Connection) {
            $ActivePorts += $Port
            Write-Host "✅ 端口 $Port 正在使用" -ForegroundColor Green
            
            # 尝试访问可能的Swagger端点
            try {
                $TestUrl = "http://localhost:$Port/swagger"
                $Response = Invoke-WebRequest -Uri $TestUrl -Method GET -TimeoutSec 3 -ErrorAction SilentlyContinue
                if ($Response.StatusCode -eq 200) {
                    Write-Host "🎯 发现Swagger文档: http://localhost:$Port/swagger" -ForegroundColor Cyan
                    Write-Host "🌍 外部访问地址: http://$ServerIP:$Port/swagger" -ForegroundColor Cyan
                }
            } catch {
                # 静默处理，只记录活动端口
            }
        }
    } catch {
        # 端口未使用，继续检查
    }
}

if ($ActivePorts.Count -eq 0) {
    Write-Host "❌ 未发现活动的Web服务端口" -ForegroundColor Red
    Write-Host "提示: 请确保应用已启动" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "📋 发现的活动端口: $($ActivePorts -join ', ')" -ForegroundColor White
}

Write-Host ""
Write-Host "🔧 手动测试端口:" -ForegroundColor Yellow
foreach ($Port in $ActivePorts) {
    Write-Host "   http://$ServerIP:$Port/swagger" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

Read-Host "按任意键退出"
