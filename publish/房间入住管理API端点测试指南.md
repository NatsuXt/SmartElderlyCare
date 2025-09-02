# 智慧养老系统 - 房间入住管理模块 API端点测试指南

## 系统信息
- **模块**: 房间入住管理模块 (RoomOccupancy)
- **端口**: 3003
- **访问地址**: http://localhost:3003 或 http://47.96.238.102:3003
- **API文档**: http://localhost:3003/swagger
- **模块路径**: /api/RoomOccupancy

## 🏠 API端点概览

### 基础功能
```
GET    /api/RoomOccupancy/test                           系统健康检查 ✅
```

### 入住记录管理
```
GET    /api/RoomOccupancy/elderly/{elderlyId}/occupancy-records    根据老人ID获取入住记录 ✅
GET    /api/RoomOccupancy/occupancy-records                        获取所有入住记录（分页） ✅
POST   /api/RoomOccupancy/check-in                                 办理入住登记 ✅
POST   /api/RoomOccupancy/check-out                                办理退房登记 ✅
```

### 账单管理
```
GET    /api/RoomOccupancy/billing/records                          获取所有账单记录（分页） ✅
GET    /api/RoomOccupancy/elderly/{elderlyId}/billing/records      根据老人ID获取账单记录 ✅
POST   /api/RoomOccupancy/billing/generate-all                     一键生成所有房间账单 ✅
POST   /api/RoomOccupancy/elderly/{elderlyId}/billing/generate     根据老人ID生成账单 ✅
```

### 支付管理
```
PUT    /api/RoomOccupancy/billing/{billingId}/payment              处理账单支付（全额） ✅
PUT    /api/RoomOccupancy/billing/{billingId}/partial-payment      处理部分支付 ✅
GET    /api/RoomOccupancy/billing/{billingId}/payment-history      获取支付历史 ✅
```

## 📋 详细测试指南

### 1. 系统健康检查

**测试目的**: 验证API服务是否正常运行

```powershell
# 基础连通性测试
$response = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/test" -Method GET
Write-Host "系统状态: $($response.message)"
Write-Host "服务版本: $($response.version)"
Write-Host "时间戳: $($response.timestamp)"
```

**预期响应**:
```json
{
  "message": "房间入住管理系统正常运行！",
  "timestamp": "2025-09-02T08:00:00",
  "version": "1.0.0"
}
```

### 2. 入住记录管理

#### 2.1 根据老人ID获取入住记录

**API**: `GET /api/RoomOccupancy/elderly/{elderlyId}/occupancy-records`

```powershell
# 设置UTF-8编码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$PSDefaultParameterValues['*:Encoding'] = 'utf8'

# 查询特定老人的入住记录
$elderlyId = 63  # 替换为实际的老人ID
$occupancyRecords = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/elderly/$elderlyId/occupancy-records" -Method GET

if ($occupancyRecords.success) {
    Write-Host "✅ 查询成功: $($occupancyRecords.message)" -ForegroundColor Green
    Write-Host "📊 入住记录数量: $($occupancyRecords.data.Count)" -ForegroundColor Cyan
    
    # 显示记录详情
    $occupancyRecords.data | ForEach-Object {
        Write-Host "  - 记录ID: $($_.occupancyId), 房间: $($_.roomNumber), 状态: $($_.status)" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ 查询失败: $($occupancyRecords.message)" -ForegroundColor Red
}
```

#### 2.2 获取所有入住记录（分页）

**API**: `GET /api/RoomOccupancy/occupancy-records`

```powershell
# 获取所有入住记录（支持分页和状态筛选）
$allOccupancy = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/occupancy-records?page=1&pageSize=10&status=入住" -Method GET

if ($allOccupancy.success) {
    Write-Host "✅ 查询成功: $($allOccupancy.message)" -ForegroundColor Green
    Write-Host "📊 记录数量: $($allOccupancy.data.Count)" -ForegroundColor Cyan
} else {
    Write-Host "❌ 查询失败: $($allOccupancy.message)" -ForegroundColor Red
}
```

**查询参数**:
- `page`: 页码（默认1）
- `pageSize`: 每页大小（默认20）
- `status`: 状态筛选（可选：入住、退房等）

### 3. 账单管理

#### 3.1 获取账单记录（分页）

**API**: `GET /api/RoomOccupancy/billing/records`

```powershell
# 获取所有账单记录
$billingRecords = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/billing/records?page=1&pageSize=10" -Method GET

if ($billingRecords.success) {
    Write-Host "✅ 账单查询成功" -ForegroundColor Green
    Write-Host "📊 总记录数: $($billingRecords.data.totalCount)" -ForegroundColor Cyan
    Write-Host "📋 当前页记录: $($billingRecords.data.items.Count)" -ForegroundColor Cyan
    
    # 显示账单摘要
    $billingRecords.data.items | Select-Object -First 3 | ForEach-Object {
        Write-Host "  账单ID: $($_.billingId) | 老人: $($_.elderlyName) | 总额: ¥$($_.totalAmount) | 状态: $($_.paymentStatus)" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ 账单查询失败: $($billingRecords.message)" -ForegroundColor Red
}
```

#### 3.2 根据老人ID获取账单记录

**API**: `GET /api/RoomOccupancy/elderly/{elderlyId}/billing/records`

```powershell
# 查询特定老人的账单记录
$elderlyId = 63
$elderlyBilling = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/elderly/$elderlyId/billing/records?page=1&pageSize=20" -Method GET

if ($elderlyBilling.success) {
    Write-Host "✅ 老人账单查询成功" -ForegroundColor Green
    Write-Host "📊 账单数量: $($elderlyBilling.data.totalCount)" -ForegroundColor Cyan
}
```

#### 3.3 生成账单

**API**: `POST /api/RoomOccupancy/elderly/{elderlyId}/billing/generate`

```powershell
# 为特定老人生成账单
$elderlyId = 63
$generateBillData = @{
    BillingStartDate = "2025-09-02T00:00:00"
    BillingEndDate = "2025-09-03T00:00:00"
} | ConvertTo-Json

$headers = @{
    'Content-Type' = 'application/json; charset=utf-8'
    'Accept' = 'application/json'
}

try {
    $generateResult = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/elderly/$elderlyId/billing/generate" -Method POST -Body ([System.Text.Encoding]::UTF8.GetBytes($generateBillData)) -Headers $headers
    
    if ($generateResult.success) {
        Write-Host "✅ 账单生成成功" -ForegroundColor Green
        Write-Host "📊 生成账单数量: $($generateResult.data.Count)" -ForegroundColor Cyan
        
        $generateResult.data | ForEach-Object {
            Write-Host "  新账单 - ID: $($_.billingId), 金额: ¥$($_.totalAmount), 天数: $($_.days)天" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ 账单生成失败: $($generateResult.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ 账单生成异常: $($_.Exception.Message)" -ForegroundColor Red
}
```

**生成所有房间账单**:
```powershell
# 一键生成所有房间账单
$generateAllData = @{
    BillingStartDate = "2025-09-02T00:00:00"
    BillingEndDate = "2025-09-03T00:00:00"
} | ConvertTo-Json

$generateAllResult = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/billing/generate-all" -Method POST -Body ([System.Text.Encoding]::UTF8.GetBytes($generateAllData)) -Headers $headers
```

### 4. 支付管理

#### 4.1 处理账单支付（全额支付）

**API**: `PUT /api/RoomOccupancy/billing/{billingId}/payment`

```powershell
# 全额支付测试
$billingId = 12  # 替换为实际的账单ID
$paymentData = @{
    PaymentAmount = 100.00
    PaymentDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    PaymentMethod = "现金"
    Remarks = "全额支付测试"
} | ConvertTo-Json

$headers = @{
    'Content-Type' = 'application/json; charset=utf-8'
    'Accept' = 'application/json'
}

try {
    $paymentResult = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/billing/$billingId/payment" -Method PUT -Body $paymentData -Headers $headers
    
    if ($paymentResult.success) {
        Write-Host "✅ 支付成功!" -ForegroundColor Green
        Write-Host "💰 支付金额: ¥$($paymentResult.data.paymentAmount)" -ForegroundColor Cyan
        Write-Host "📊 账单状态: $($paymentResult.data.paymentStatus)" -ForegroundColor Cyan
        Write-Host "💳 支付方式: $($paymentResult.data.paymentMethod)" -ForegroundColor Cyan
        Write-Host "🕒 支付时间: $($paymentResult.data.paymentDate)" -ForegroundColor Cyan
    } else {
        Write-Host "❌ 支付失败: $($paymentResult.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ 支付异常: $($_.Exception.Message)" -ForegroundColor Red
    # 常见错误：金额超出未支付金额、账单不存在、网络问题等
}
```

#### 4.2 处理部分支付

**API**: `PUT /api/RoomOccupancy/billing/{billingId}/partial-payment`

```powershell
# 部分支付测试
$billingId = 13
$partialPaymentData = @{
    PaymentAmount = 50.00
    PaymentDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    PaymentMethod = "银行卡"
    Remarks = "部分支付测试"
} | ConvertTo-Json

try {
    $partialResult = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/billing/$billingId/partial-payment" -Method PUT -Body $partialPaymentData -Headers $headers
    
    if ($partialResult.success) {
        Write-Host "✅ 部分支付成功!" -ForegroundColor Green
        Write-Host "💰 本次支付: ¥$($partialResult.data.paymentAmount)" -ForegroundColor Cyan
        Write-Host "💳 已支付总额: ¥$($partialResult.data.paidAmount)" -ForegroundColor Cyan
        Write-Host "💸 剩余未付: ¥$($partialResult.data.unpaidAmount)" -ForegroundColor Cyan
        Write-Host "📊 账单状态: $($partialResult.data.paymentStatus)" -ForegroundColor Cyan
    } else {
        Write-Host "❌ 部分支付失败: $($partialResult.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ 部分支付异常: $($_.Exception.Message)" -ForegroundColor Red
}
```

#### 4.3 获取支付历史

**API**: `GET /api/RoomOccupancy/billing/{billingId}/payment-history`

```powershell
# 查询支付历史
$billingId = 3
$paymentHistory = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/billing/$billingId/payment-history" -Method GET

if ($paymentHistory.success) {
    Write-Host "✅ 支付历史查询成功" -ForegroundColor Green
    Write-Host "📊 历史记录数量: $($paymentHistory.data.Count)" -ForegroundColor Cyan
    
    $paymentHistory.data | ForEach-Object {
        Write-Host "  📋 支付记录:" -ForegroundColor Yellow
        Write-Host "    - 支付金额: ¥$($_.PaidAmount)" -ForegroundColor White
        Write-Host "    - 支付状态: $($_.PaymentStatus)" -ForegroundColor White
        Write-Host "    - 支付时间: $($_.PaymentDate)" -ForegroundColor White
        Write-Host "    - 更新时间: $($_.UpdatedDate)" -ForegroundColor White
    }
} else {
    Write-Host "❌ 支付历史查询失败: $($paymentHistory.message)" -ForegroundColor Red
}
```

## 🧪 完整测试流程

### 测试脚本示例

```powershell
# 房间入住管理模块完整测试脚本
Write-Host "=== 🏠 房间入住管理模块完整测试 ===" -ForegroundColor Green

# 设置UTF-8编码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$PSDefaultParameterValues['*:Encoding'] = 'utf8'

# 1. 系统健康检查
Write-Host "`n1. 系统健康检查..." -ForegroundColor Yellow
$testResult = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/test" -Method GET
Write-Host "  ✅ $($testResult.message)" -ForegroundColor Green

# 2. 账单记录查询
Write-Host "`n2. 账单记录查询..." -ForegroundColor Yellow
$billing = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/billing/records?page=1&pageSize=5" -Method GET
Write-Host "  ✅ 查询到 $($billing.data.totalCount) 条账单记录" -ForegroundColor Green

# 3. 支付功能测试（如果有未支付账单）
Write-Host "`n3. 支付功能测试..." -ForegroundColor Yellow
$unpaidBills = $billing.data.items | Where-Object { $_.paymentStatus -eq "未支付" -and $_.unpaidAmount -gt 0 }
if ($unpaidBills.Count -gt 0) {
    $testBill = $unpaidBills[0]
    Write-Host "  🎯 测试账单: ID=$($testBill.billingId), 金额=¥$($testBill.unpaidAmount)" -ForegroundColor Cyan
    
    # 测试支付历史查询
    $history = Invoke-RestMethod -Uri "http://localhost:3003/api/RoomOccupancy/billing/$($testBill.billingId)/payment-history" -Method GET
    Write-Host "  ✅ 支付历史查询成功: $($history.data.Count) 条记录" -ForegroundColor Green
} else {
    Write-Host "  ⚠️ 没有找到可测试的未支付账单" -ForegroundColor Yellow
}

Write-Host "`n🎉 房间入住管理模块测试完成!" -ForegroundColor Green
```

## 📊 数据格式说明

### 账单记录 (BillingRecordDto)
```json
{
  "billingId": 12,
  "occupancyId": 2,
  "elderlyId": 61,
  "elderlyName": "孙磊",
  "roomId": 2,
  "roomNumber": "202B",
  "billingStartDate": "2025-09-02T00:00:00",
  "billingEndDate": "2025-09-02T00:00:00",
  "days": 1,
  "dailyRate": 2000,
  "totalAmount": 2000,
  "paymentStatus": "未支付",
  "paidAmount": 0,
  "unpaidAmount": 2000,
  "billingDate": "2025-09-02T01:26:57",
  "paymentDate": null,
  "remarks": null,
  "createdDate": "2025-09-02T01:26:57",
  "updatedDate": "2025-09-02T01:26:57"
}
```

### 支付请求 (BillingPaymentDto)
```json
{
  "paymentAmount": 100.00,
  "paymentDate": "2025-09-02T08:00:00",
  "paymentMethod": "现金",
  "remarks": "支付备注"
}
```

### 入住记录 (OccupancyRecordDto)
```json
{
  "occupancyId": 1,
  "roomId": 2,
  "elderlyId": 63,
  "checkInDate": "2025-09-01T14:00:00",
  "checkOutDate": null,
  "status": "入住",
  "bedNumber": "1",
  "remarks": "入住备注",
  "roomNumber": "202B",
  "elderlyName": "张三"
}
```

## ⚠️ 重要注意事项

### 支付功能使用注意
1. **金额验证**: 支付金额不能超过未支付金额
2. **支付方法**: 支持现金、银行卡、微信、支付宝、转账
3. **部分支付**: 支持多次部分支付直到全额支付
4. **支付状态**: 未支付 → 部分支付 → 已支付

### 错误处理
- **400 Bad Request**: 参数验证失败（如负数金额、超额支付）
- **404 Not Found**: 账单或记录不存在
- **500 Internal Server Error**: 服务器内部错误（数据库连接等）

### 中文字符支持
- 所有POST/PUT请求必须使用UTF-8编码
- 请求头需要包含: `Content-Type: application/json; charset=utf-8`
- PowerShell测试时需要设置正确的编码

## 🔧 故障排除

### 常见问题
1. **连接失败**: 确认服务器地址和端口号
2. **编码问题**: 确保使用UTF-8编码
3. **参数错误**: 检查JSON格式和必填字段
4. **业务逻辑错误**: 检查账单状态和支付金额

### 测试建议
1. 先执行健康检查确认服务正常
2. 使用已知存在的账单ID进行支付测试
3. 部分支付测试建议使用小金额
4. 定期查看服务器日志排查问题

---

**📍 测试服务器**: http://localhost:3003  
**📚 API文档**: http://localhost:3003/swagger  
**🏠 模块标识**: 房间入住管理模块  
**📅 更新时间**: 2025年9月2日
