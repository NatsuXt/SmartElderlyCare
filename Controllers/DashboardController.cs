using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly OperationLogService _logService;
    private readonly AppDbContext _context;

    public DashboardController(OperationLogService logService, AppDbContext context)
    {
        _logService = logService;
        _context = context;
    }

    /// <summary>
    /// 记录老人健康趋势查询操作。
    /// 根据业务逻辑 1.1.21，所有数据查询和分析操作都应被记录在OperationLog表中。
    /// </summary>
    /// <param name="elderlyId">老人ID</param>
    /// <param name="start">开始时间</param>
    /// <param name="end">结束时间</param>
    /// <returns>操作记录结果</returns>
    [HttpPost("log-health-trend-query")]
    public IActionResult LogHealthTrendQuery([FromBody] DashboardQueryRequest request)
    {
        int staffId = GetCurrentStaffId();
        
        var operationLog = new OperationLog
        {
            operation_description = $"查询老人健康趋势：老人ID={request.ElderlyId}, 时间范围={request.StartDate:yyyy-MM-dd}至{request.EndDate:yyyy-MM-dd}",
            staff_id = staffId,
            operation_time = DateTime.Now,
            operation_type = "数据查询",
            operation_status = "成功",
            affected_entity = "HealthMonitoring",
            ip_address = GetClientIpAddress(),
            device_type = GetDeviceType()
        };

        var result = _logService.CreateSafe(operationLog, out var errorMessage);
        
        if (result == null)
            return BadRequest($"记录操作日志失败: {errorMessage}");
            
        return Ok(new { 
            Message = "健康趋势查询操作已记录", 
            LogId = result.log_id,
            Timestamp = result.operation_time 
        });
    }

    /// <summary>
    /// 记录费用构成分析操作。
    /// 根据业务逻辑 1.1.21，所有数据查询和分析操作都应被记录在OperationLog表中。
    /// </summary>
    /// <param name="request">查询请求参数</param>
    /// <returns>操作记录结果</returns>
    [HttpPost("log-fee-composition-query")]
    public IActionResult LogFeeCompositionQuery([FromBody] DashboardQueryRequest request)
    {
        int staffId = GetCurrentStaffId();
        
        var operationLog = new OperationLog
        {
            operation_description = $"查询费用构成分析：老人ID={request.ElderlyId}, 时间范围={request.StartDate:yyyy-MM-dd}至{request.EndDate:yyyy-MM-dd}",
            staff_id = staffId,
            operation_time = DateTime.Now,
            operation_type = "数据分析",
            operation_status = "成功",
            affected_entity = "FeeSettlement",
            ip_address = GetClientIpAddress(),
            device_type = GetDeviceType()
        };

        var result = _logService.CreateSafe(operationLog, out var errorMessage);
        
        if (result == null)
            return BadRequest($"记录操作日志失败: {errorMessage}");
            
        return Ok(new { 
            Message = "费用构成分析操作已记录", 
            LogId = result.log_id,
            Timestamp = result.operation_time 
        });
    }

    /// <summary>
    /// 记录通用的仪表板数据查询操作。
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>操作记录结果</returns>
    [HttpPost("log-dashboard-query")]
    public IActionResult LogDashboardQuery([FromBody] DashboardLogRequest request)
    {
        int staffId = GetCurrentStaffId();
        
        var operationLog = new OperationLog
        {
            operation_description = request.Description ?? "仪表板查询",
            staff_id = staffId,
            operation_time = DateTime.Now,
            operation_type = request.OperationType ?? "仪表板操作",
            operation_status = "成功",
            affected_entity = request.AffectedEntity ?? "Dashboard",
            ip_address = GetClientIpAddress(),
            device_type = GetDeviceType()
        };

        var result = _logService.CreateSafe(operationLog, out var errorMessage);
        
        if (result == null)
            return BadRequest($"记录操作日志失败: {errorMessage}");
            
        return Ok(new { 
            Message = "仪表板操作已记录", 
            LogId = result.log_id,
            Timestamp = result.operation_time 
        });
    }

    /// <summary>
    /// 获取老人健康趋势数据（既记录日志又返回实际数据）
    /// </summary>
    /// <param name="request">查询请求参数</param>
    /// <returns>健康趋势数据</returns>
    [HttpPost("health-trend-data")]
    public IActionResult GetHealthTrendData([FromBody] DashboardQueryRequest request)
    {
        int staffId = GetCurrentStaffId();
        
        try
        {
            // 1. 记录操作日志（保持现有逻辑）
            var operationLog = new OperationLog
            {
                operation_description = $"查询老人健康趋势数据：老人ID={request.ElderlyId}, 时间范围={request.StartDate:yyyy-MM-dd}至{request.EndDate:yyyy-MM-dd}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "健康数据查询",
                operation_status = "成功",
                affected_entity = "HealthMonitoring",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            };

            var logResult = _logService.Create(operationLog, out bool _);
            
            // 2. 查询实际健康数据
            var healthData = _context.HealthMonitorings
                .Where(h => h.elderly_id == request.ElderlyId 
                       && h.monitoring_date >= request.StartDate 
                       && h.monitoring_date <= request.EndDate)
                .OrderBy(h => h.monitoring_date)
                .Select(h => new {
                    Date = h.monitoring_date,
                    HeartRate = h.heart_rate,
                    BloodPressure = h.blood_pressure,
                    OxygenLevel = h.oxygen_level,
                    Temperature = h.temperature,
                    Status = h.status
                })
                .ToList();

            return Ok(new { 
                Message = "健康趋势数据获取成功",
                LogId = logResult?.log_id,
                Data = healthData,
                Count = healthData.Count,
                QueryInfo = new {
                    ElderlyId = request.ElderlyId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }
            });
        }
        catch (Exception ex)
        {
            // 记录失败日志
            _logService.Create(new OperationLog
            {
                operation_description = $"查询老人健康趋势数据失败：{ex.Message}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "健康数据查询",
                operation_status = "失败",
                affected_entity = "HealthMonitoring",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return StatusCode(500, new { 
                Message = "查询健康趋势数据失败", 
                Error = ex.Message 
            });
        }
    }

    /// <summary>
    /// 获取费用构成数据（既记录日志又返回实际数据）
    /// </summary>
    /// <param name="request">查询请求参数</param>
    /// <returns>费用构成数据</returns>
    [HttpPost("fee-composition-data")]
    public IActionResult GetFeeCompositionData([FromBody] DashboardQueryRequest request)
    {
        int staffId = GetCurrentStaffId();
        
        try
        {
            // 1. 记录操作日志（保持现有逻辑）
            var operationLog = new OperationLog
            {
                operation_description = $"查询费用构成数据：老人ID={request.ElderlyId}, 时间范围={request.StartDate:yyyy-MM-dd}至{request.EndDate:yyyy-MM-dd}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "费用数据分析",
                operation_status = "成功",
                affected_entity = "FeeSettlement",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            };

            var logResult = _logService.Create(operationLog, out bool _);
            
            // 2. 查询实际费用数据
            var feeData = _context.FeeSettlements
                .Where(f => f.elderly_id == request.ElderlyId 
                       && f.settlement_date >= request.StartDate 
                       && f.settlement_date <= request.EndDate)
                .GroupBy(f => f.payment_method)
                .Select(g => new {
                    PaymentMethod = g.Key,
                    TotalAmount = g.Sum(f => f.total_amount),
                    InsuranceAmount = g.Sum(f => f.insurance_amount),
                    PersonalPayment = g.Sum(f => f.personal_payment),
                    Count = g.Count()
                })
                .ToList();
                
            // 3. 计算汇总数据
            var summary = new {
                TotalAmount = feeData.Sum(f => f.TotalAmount),
                TotalInsuranceAmount = feeData.Sum(f => f.InsuranceAmount),
                TotalPersonalPayment = feeData.Sum(f => f.PersonalPayment),
                TotalRecords = feeData.Sum(f => f.Count)
            };

            return Ok(new {
                Message = "费用构成数据获取成功", 
                LogId = logResult?.log_id,
                Data = feeData,
                Summary = summary,
                QueryInfo = new {
                    ElderlyId = request.ElderlyId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }
            });
        }
        catch (Exception ex)
        {
            // 记录失败日志
            _logService.Create(new OperationLog
            {
                operation_description = $"查询费用构成数据失败：{ex.Message}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "费用数据分析",
                operation_status = "失败",
                affected_entity = "FeeSettlement",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return StatusCode(500, new { 
                Message = "查询费用构成数据失败", 
                Error = ex.Message 
            });
        }
    }

    /// <summary>
    /// 获取仪表板相关的操作日志。
    /// </summary>
    /// <param name="days">查询最近几天的记录，默认7天</param>
    /// <returns>操作日志列表</returns>
    [HttpGet("operation-logs")]
    public IActionResult GetDashboardOperationLogs([FromQuery] int days = 7)
    {
        var logs = _logService.GetLogs(
            days: days, 
            operationType: new[] { "数据查询", "数据分析", "仪表板操作", "健康数据查询", "费用数据分析" }
        );
        
        return Ok(logs);
    }

    // 辅助方法
    private int GetCurrentStaffId()
    {
        if (User.Identity?.IsAuthenticated == true && int.TryParse(User.Identity.Name, out int uid))
            return uid;
        return 1; // 默认员工ID，实际应用中应该处理未认证情况
    }

    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "未知";
    }

    private string GetDeviceType()
    {
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        if (userAgent.Contains("Mobile"))
            return "移动设备";
        else if (userAgent.Contains("Tablet"))
            return "平板设备";
        else
            return "桌面设备";
    }
}

// 请求模型类
public class DashboardQueryRequest
{
    public int ElderlyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class DashboardLogRequest
{
    public string? Description { get; set; }
    public string? OperationType { get; set; }
    public string? AffectedEntity { get; set; }
} 