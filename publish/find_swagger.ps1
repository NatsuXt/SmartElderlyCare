# 🔍 智慧养老系统 - 第三模块端口自动发现脚本

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "智慧养老系统 - 第三模块端口发现" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 获取服务器IP
try {
    $ServerIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -ne "127.0.0.1" -and $_.IPAddress -notlike "169.254.*"} | Select-Object -First 1).IPAddress
    Write-Host "🌐 服务器IP地址: $ServerIP" -ForegroundColor Yellow
} catch {
    $ServerIP = "47.96.238.102"  # 使用已知的服务器IP
    Write-Host "🌐 使用配置的服务器IP: $ServerIP" -ForegroundColor Yellow
}
Write-Host ""

# 检查常用端口 (包含各模块端口)
$CommonPorts = @(3003, 3004, 3005, 5000, 5001, 7000, 7001, 8080, 8081, 8082)
$ActivePorts = @()
$SwaggerPorts = @()

Write-Host "🔍 扫描活动端口 (重点检查第三模块端口3003)..." -ForegroundColor Green
foreach ($Port in $CommonPorts) {
    try {
        $Connection = Test-NetConnection -ComputerName "localhost" -Port $Port -InformationLevel Quiet -WarningAction SilentlyContinue
        if ($Connection) {
            $ActivePorts += $Port
            if ($Port -eq 3003) {
                Write-Host "✅ 端口 $Port 正在使用 (第三模块)" -ForegroundColor Green
            } else {
                Write-Host "✅ 端口 $Port 正在使用" -ForegroundColor Blue
            }
            
            # 尝试访问可能的Swagger端点
            try {
                $TestUrl = "http://localhost:$Port/swagger"
                $Response = Invoke-WebRequest -Uri $TestUrl -Method GET -TimeoutSec 3 -ErrorAction SilentlyContinue
                if ($Response.StatusCode -eq 200) {
                    $SwaggerPorts += $Port
                    if ($Port -eq 3003) {
                        Write-Host "🎯 发现第三模块Swagger文档: http://localhost:$Port/swagger" -ForegroundColor Cyan
                        Write-Host "🌍 外部访问地址: http://$ServerIP`:$Port/swagger" -ForegroundColor Green
                    } else {
                        Write-Host "🎯 发现Swagger文档: http://localhost:$Port/swagger" -ForegroundColor Blue
                        Write-Host "🌍 外部访问地址: http://$ServerIP`:$Port/swagger" -ForegroundColor Gray
                    }
                }
            } catch {
                # 尝试访问根路径
                try {
                    $RootUrl = "http://localhost:$Port/"
                    $RootResponse = Invoke-WebRequest -Uri $RootUrl -Method GET -TimeoutSec 3 -ErrorAction SilentlyContinue
                    if ($RootResponse.StatusCode -eq 200) {
                        Write-Host "🌐 发现Web服务: http://$ServerIP`:$Port/" -ForegroundColor Blue
                    }
                } catch {
                    # 静默处理
                }
            }
        }
    } catch {
        # 端口未使用，继续检查
    }
}

Write-Host ""

if ($ActivePorts.Count -eq 0) {
    Write-Host "❌ 未发现活动的Web服务端口" -ForegroundColor Red
    Write-Host "提示: 请确保应用已启动" -ForegroundColor Yellow
} else {
    Write-Host "📋 发现的活动端口: $($ActivePorts -join ', ')" -ForegroundColor White
}

if ($SwaggerPorts.Count -gt 0) {
    Write-Host ""
    Write-Host "🎯 可用的Swagger文档地址:" -ForegroundColor Green
    foreach ($Port in $SwaggerPorts) {
        if ($Port -eq 3003) {
            Write-Host "   http://$ServerIP`:$Port/swagger (第三模块)" -ForegroundColor Cyan
        } else {
            Write-Host "   http://$ServerIP`:$Port/swagger" -ForegroundColor Gray
        }
    }
} else {
    Write-Host ""
    Write-Host "⚠️  未发现Swagger文档端点" -ForegroundColor Orange
    Write-Host "🔧 手动检查端口:" -ForegroundColor Yellow
    foreach ($Port in $ActivePorts) {
        if ($Port -eq 3003) {
            Write-Host "   http://$ServerIP`:$Port/swagger (第三模块)" -ForegroundColor Cyan
        } else {
            Write-Host "   http://$ServerIP`:$Port/swagger" -ForegroundColor Gray
        }
        Write-Host "   http://$ServerIP`:$Port/api" -ForegroundColor Gray
    }
}

# 检查.NET进程
Write-Host ""
Write-Host "🔍 检查.NET进程..." -ForegroundColor Green
$DotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($DotnetProcesses) {
    Write-Host "✅ 发现 $($DotnetProcesses.Count) 个dotnet进程" -ForegroundColor Green
    $DotnetProcesses | ForEach-Object {
        Write-Host "   进程ID: $($_.Id), 内存: $([math]::Round($_.WorkingSet/1MB,1))MB" -ForegroundColor Gray
    }
} else {
    Write-Host "❌ 未发现dotnet进程" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

Read-Host "按任意键退出"
