using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VisitorRegistrationController : ControllerBase
{
    private readonly VisitorRegistrationService _visitorService;
    private readonly OperationLogService _logService;

    public VisitorRegistrationController(VisitorRegistrationService visitorService, OperationLogService logService)
    {
        _visitorService = visitorService;
        _logService = logService;
    }

    /// <summary>
    /// 家属提交远程视频探视预约申请。
    /// 业务逻辑 1.1.7 第1-2步：家属在移动端提交申请，系统创建记录。
    /// </summary>
    /// <param name="registration">预约信息</param>
    /// <returns>创建的预约记录</returns>
    [HttpPost("submit-video-visit")]
    public IActionResult SubmitVideoVisit([FromBody] VisitorRegistration registration)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 调用服务创建预约记录
            var result = _visitorService.SubmitVideoVisit(registration);

            // 记录操作日志
            var logResult = _logService.Create(new OperationLog
            {
                operation_description = $"家属提交视频探视预约：{registration.visitor_name} 预约探视老人ID {registration.elderly_id}",
                staff_id = registration.family_id, // 这里用家属ID，实际可能需要调整
                operation_time = DateTime.Now,
                operation_type = "提交预约",
                operation_status = "成功",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return CreatedAtAction(nameof(GetVisit), new { id = result.visitor_id }, result);
        }
        catch (ArgumentException ex)
        {
            // 业务逻辑验证错误
            _logService.Create(new OperationLog
            {
                operation_description = $"家属提交视频探视预约失败（参数错误）：{ex.Message}",
                staff_id = registration.family_id,
                operation_time = DateTime.Now,
                operation_type = "提交预约",
                operation_status = "失败",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return BadRequest(new { 
                Message = "提交预约失败", 
                Error = ex.Message,
                Type = "ValidationError"
            });
        }
        catch (Exception ex)
        {
            // 系统错误
            _logService.Create(new OperationLog
            {
                operation_description = $"家属提交视频探视预约失败（系统错误）：{ex.Message}",
                staff_id = registration.family_id,
                operation_time = DateTime.Now,
                operation_type = "提交预约",
                operation_status = "失败",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return StatusCode(500, new { 
                Message = "系统内部错误，请稍后重试", 
                Error = "Internal Server Error",
                Type = "SystemError"
            });
        }
    }

    /// <summary>
    /// 工作人员审核视频探视预约。
    /// 业务逻辑 1.1.7 第3步：工作人员审核预约并更新状态。
    /// </summary>
    /// <param name="id">预约ID</param>
    /// <param name="request">审核请求</param>
    /// <returns>审核结果</returns>
    [HttpPut("{id}/approve")]
    public IActionResult ApproveVisit(int id, [FromBody] ApprovalRequest request)
    {
        int staffId = GetCurrentStaffId();

        try
        {
            var success = _visitorService.ApproveVisit(id, request.Approve);
            if (!success)
                return NotFound($"预约ID {id} 不存在");

            // 记录操作日志
            var status = request.Approve ? "批准" : "拒绝";
            _logService.Create(new OperationLog
            {
                operation_description = $"工作人员{status}视频探视预约：预约ID {id}，原因：{request.Reason}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "审核预约",
                operation_status = "成功",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return Ok(new { 
                Message = $"预约已{status}", 
                ApprovalStatus = request.Approve ? "已批准" : "已拒绝",
                ApprovedBy = staffId,
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            // 记录失败日志
            _logService.Create(new OperationLog
            {
                operation_description = $"审核视频探视预约失败：{ex.Message}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "审核预约",
                operation_status = "失败",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return BadRequest($"审核失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取预约记录详情。
    /// </summary>
    /// <param name="id">预约ID</param>
    /// <returns>预约详情</returns>
    [HttpGet("{id}")]
    public IActionResult GetVisit(int id)
    {
        var visits = _visitorService.GetVisits();
        var visit = visits.FirstOrDefault(v => v.visitor_id == id);
        
        if (visit == null)
            return NotFound($"预约ID {id} 不存在");

        return Ok(visit);
    }

    /// <summary>
    /// 查询预约记录列表。
    /// </summary>
    /// <param name="familyId">家属ID筛选</param>
    /// <param name="elderlyId">老人ID筛选</param>
    /// <param name="status">状态筛选</param>
    /// <returns>预约记录列表</returns>
    [HttpGet]
    public IActionResult GetVisits([FromQuery] int? familyId, [FromQuery] int? elderlyId, [FromQuery] string? status)
    {
        int staffId = GetCurrentStaffId();

        var visits = _visitorService.GetVisits(familyId, elderlyId, status);

        // 记录查询操作日志
        _logService.Create(new OperationLog
        {
            operation_description = $"查询视频探视预约记录：家属ID={familyId}，老人ID={elderlyId}，状态={status}",
            staff_id = staffId,
            operation_time = DateTime.Now,
            operation_type = "查询预约",
            operation_status = "成功",
            affected_entity = "VisitorRegistration",
            ip_address = GetClientIpAddress(),
            device_type = GetDeviceType()
        }, out bool _);

        return Ok(visits);
    }

    /// <summary>
    /// 获取待审核的预约列表（工作人员使用）。
    /// </summary>
    /// <returns>待审核预约列表</returns>
    [HttpGet("pending-approval")]
    public IActionResult GetPendingApprovals()
    {
        int staffId = GetCurrentStaffId();

        var pendingVisits = _visitorService.GetVisits(status: "待批准");

        // 记录操作日志
        _logService.Create(new OperationLog
        {
            operation_description = "查询待审核的视频探视预约",
            staff_id = staffId,
            operation_time = DateTime.Now,
            operation_type = "查询待审核",
            operation_status = "成功",
            affected_entity = "VisitorRegistration",
            ip_address = GetClientIpAddress(),
            device_type = GetDeviceType()
        }, out bool _);

        return Ok(pendingVisits);
    }

    // 辅助方法
    private int GetCurrentStaffId()
    {
        if (User.Identity?.IsAuthenticated == true && int.TryParse(User.Identity.Name, out int uid))
            return uid;
        return 1; // 默认员工ID
    }

    private string GetClientIpAddress()
    {
        try
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            return !string.IsNullOrEmpty(remoteIp) ? remoteIp : "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
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

// 请求模型
public class ApprovalRequest
{
    public bool Approve { get; set; }
    public string? Reason { get; set; }
} 