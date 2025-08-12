# Firewall Configuration Script - Run on remote server (requires admin privileges)

Write-Host "Configuring firewall for Smart Elderly Care System..." -ForegroundColor Yellow

# Add firewall rule for port 3003
try {
    New-NetFirewallRule -DisplayName "SmartElderlyCare-Port3003" -Direction Inbound -Protocol TCP -LocalPort 3003 -Action Allow -ErrorAction Stop
    Write-Host "Firewall rule added successfully!" -ForegroundColor Green
} catch {
    Write-Host "Failed to add firewall rule: $($_.Exception.Message)" -ForegroundColor Red
}

# Verify the rule was created
$rule = Get-NetFirewallRule -DisplayName "SmartElderlyCare-Port3003" -ErrorAction SilentlyContinue
if ($rule) {
    Write-Host "Firewall rule verification: SUCCESS" -ForegroundColor Green
    Write-Host "Rule details:" -ForegroundColor Cyan
    $rule | Select-Object DisplayName, Direction, Action, Enabled | Format-Table
} else {
    Write-Host "Firewall rule verification: FAILED" -ForegroundColor Red
}

Write-Host "Firewall configuration completed!" -ForegroundColor Greenㄨ繙绋嬫湇鍔″櫒涓婅繍琛屾鍛戒护 (闇€瑕佺鐞嗗憳鏉冮檺)
New-NetFirewallRule -DisplayName "SmartElderlyCare-Port3003" -Direction Inbound -Protocol TCP -LocalPort 3003 -Action Allow
Write-Host "闃茬伀澧欒鍒欏凡娣诲姞!" -ForegroundColor Green
