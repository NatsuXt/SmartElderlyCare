using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ElderlyCareSystem.Models;
using System.Linq;

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
    /// 提交探视预约申请
    /// </summary>
    /// <remarks>
    /// **功能说明：** 访客通过移动端或Web端提交探视预约申请（视频探视或线下探视），系统创建待审核的预约记录。
    /// 
    /// **业务逻辑：**
    /// 1. 验证访客ID是否存在（关联VisitorLogin表）
    /// 2. 验证输入参数的完整性和格式
    /// 3. 创建预约记录，默认状态为"待批准"
    /// 4. 自动设置预约时间为当前时间
    /// 5. 返回创建的预约记录供后续跟踪
    /// 
    /// **输入要求：**
    /// - 负责人访客ID：必填，正整数，必须是已注册的访客
    /// - 实际访客姓名：必填，1-100字符，不需要与负责人ID对应
    /// - 与老人关系：必填，1-50字符（如：儿子、女儿、孙子等）
    /// - 探访原因：必填，详细说明探视目的
    /// - 访问类型：必填，只能是"视频探视"或"线下探视"
    /// - 老人ID：可选，支持两种模式：
    ///   * 指定老人：输入具体的老人ID（正整数）
    ///   * 全体老人：输入0，表示申请探视全体老人
    /// 
    /// **业务规则：**
    /// - 同一访客可以提交多个预约申请
    /// - 预约申请需要工作人员审核后才能生效
    /// - 全体老人探视：ElderlyId设为0，表示申请探视所有老人
    /// 
    /// **成功返回：** 201 Created + 预约记录详情
    /// **失败返回：** 400 Bad Request（参数错误/访客不存在） 或 500 Internal Server Error
    /// 
    /// **使用场景：**
    /// - 家属通过APP提交探视申请（指定老人）
    /// - 朋友通过Web端申请视频探视（指定老人）
    /// - 紧急情况下的远程探视需求
    /// - 访客申请探视全体老人（集体活动、公告传达等）
    /// 
    /// **示例请求1（视频探视指定老人）：**
    /// ```json
    /// {
    ///   "responsibleVisitorId": 1,
    ///   "visitorName": "张三",
    ///   "relationshipToElderly": "儿子",
    ///   "visitReason": "关心父亲身体状况，询问近期生活情况",
    ///   "visitType": "视频探视",
    ///   "elderlyId": 61
    /// }
    /// ```
    /// 
    /// **示例请求2（线下探视全体老人）：**
    /// ```json
    /// {
    ///   "responsibleVisitorId": 1,
    ///   "visitorName": "李四",
    ///   "relationshipToElderly": "志愿者",
    ///   "visitReason": "为全体老人进行健康讲座和问候",
    ///   "visitType": "线下探视",
    ///   "elderlyId": 0
    /// }
    /// ```
    /// 
    /// **示例请求3（视频探视指定老人）：**
    /// ```json
    /// {
    ///   "responsibleVisitorId": 1,
    ///   "visitorName": "王五",
    ///   "relationshipToElderly": "朋友",
    ///   "visitReason": "探望老朋友",
    ///   "visitType": "视频探视",
    ///   "elderlyId": 61
    /// }
    /// ```
    /// 
    /// **示例响应1（视频探视指定老人）：**
    /// ```json
    /// {
    ///   "registrationId": 1,
    ///   "visitorId": 1,
    ///   "elderlyId": 61,
    ///   "visitorName": "张三",
    ///   "visitTime": "2024-08-27T10:30:00",
    ///   "relationshipToElderly": "儿子", 
    ///   "visitReason": "关心父亲身体状况，询问近期生活情况 (探视对象: 指定老人ID 61)",
    ///   "visitType": "视频探视",
    ///   "approvalStatus": "待批准"
    /// }
    /// ```
    /// 
    /// **示例响应2（线下探视全体老人）：**
    /// ```json
    /// {
    ///   "registrationId": 2,
    ///   "visitorId": 1,
    ///   "elderlyId": 0,
    ///   "visitorName": "李四",
    ///   "visitTime": "2024-08-27T10:30:00",
    ///   "relationshipToElderly": "志愿者", 
    ///   "visitReason": "为全体老人进行健康讲座和问候 (探视对象: 全体老人)",
    ///   "visitType": "线下探视",
    ///   "approvalStatus": "待批准"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">视频探视预约申请对象</param>
    /// <returns>创建成功返回预约记录，失败返回错误信息</returns>
    /// <response code="201">预约申请提交成功，返回预约记录</response>
    /// <response code="400">请求参数错误或访客ID不存在</response>
    /// <response code="500">系统内部错误</response>
    [HttpPost("submit-video-visit")]
    public IActionResult SubmitVideoVisit([FromBody] VideoVisitRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 调用服务创建预约记录
            var result = _visitorService.SubmitVideoVisit(request);

            return CreatedAtAction(nameof(GetVisit), new { id = result.RegistrationId }, result);
        }
        catch (ArgumentException ex)
        {
            // 业务逻辑验证错误
            return BadRequest(new { 
                Message = "提交预约失败", 
                Error = ex.Message,
                Type = "ValidationError"
            });
        }
        catch (Exception)
        {
            // 系统错误
            return StatusCode(500, new { 
                Message = "系统内部错误，请稍后重试", 
                Error = "Internal Server Error",
                Type = "SystemError"
            });
        }
    }

    /// <summary>
    /// 批量提交探视预约申请
    /// </summary>
    /// <remarks>
    /// **功能说明：** 负责人通过一次请求为多个访客提交探视预约申请（视频探视或线下探视），系统为每个访客创建独立的预约记录。
    /// 
    /// **业务逻辑：**
    /// 1. 验证负责人访客ID是否存在（关联VisitorLogin表）
    /// 2. 验证每个访客的输入参数完整性和格式
    /// 3. 为每个访客创建独立的预约记录，默认状态为"待批准"
    /// 4. 自动设置预约时间为当前时间
    /// 5. 返回所有创建的预约记录ID列表
    /// 
    /// **输入要求：**
    /// - 负责人访客ID：必填，正整数，必须是已注册的访客
    /// - 访客列表：必填，至少包含一个访客信息
    ///   * 访客姓名：必填，1-100字符，不需要与负责人ID对应
    ///   * 与老人关系：必填，1-50字符
    ///   * 探访原因：必填，详细说明探视目的
    /// - 访问类型：必填，只能是"视频探视"或"线下探视"
    /// - 老人ID：可选，支持两种模式：
    ///   * 指定老人：输入具体的老人ID（正整数）
    ///   * 全体老人：输入0，表示申请探视全体老人
    /// 
    /// **业务规则：**
    /// - 负责人可以为多个不同的人提交预约申请
    /// - 每个访客都会得到独立的预约记录ID
    /// - 所有预约申请都需要工作人员审核后才能生效
    /// - 负责人可以查看自己提交的所有预约申请
    /// 
    /// **成功返回：** 201 Created + 预约记录ID列表
    /// **失败返回：** 400 Bad Request（参数错误/负责人不存在） 或 500 Internal Server Error
    /// 
    /// **使用场景：**
    /// - 家庭负责人为多个家庭成员批量预约探视
    /// - 团体负责人为团体成员批量申请探视
    /// - 活动组织者为参与者批量预约探视
    /// 
    /// **示例请求：**
    /// ```json
    /// {
    ///   "responsibleVisitorId": 1,
    ///   "visitors": [
    ///     {
    ///       "visitorName": "张三",
    ///       "relationshipToElderly": "儿子",
    ///       "visitReason": "关心父亲身体状况，询问近期生活情况"
    ///     },
    ///     {
    ///       "visitorName": "李四",
    ///       "relationshipToElderly": "女儿",
    ///       "visitReason": "看望母亲，汇报工作情况"
    ///     }
    ///   ],
    ///   "visitType": "视频探视",
    ///   "elderlyId": 61
    /// }
    /// ```
    /// 
    /// **示例响应：**
    /// ```json
    /// {
    ///   "message": "批量预约申请提交成功",
    ///   "registrationIds": [1, 2],
    ///   "totalCount": 2,
    ///   "responsibleVisitorId": 1
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">批量视频探视预约申请对象</param>
    /// <returns>创建成功返回预约记录ID列表，失败返回错误信息</returns>
    /// <response code="201">批量预约申请提交成功，返回预约记录ID列表</response>
    /// <response code="400">请求参数错误或负责人ID不存在</response>
    /// <response code="500">系统内部错误</response>
    [HttpPost("submit-bulk-video-visit")]
    public IActionResult SubmitBulkVideoVisit([FromBody] BulkVideoVisitRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 调用服务创建批量预约记录
            var results = _visitorService.SubmitBulkVideoVisit(request);

            return CreatedAtAction(nameof(GetVisitsByResponsible), 
                new { responsibleVisitorId = request.ResponsibleVisitorId }, 
                new { 
                    Message = "批量预约申请提交成功",
                    RegistrationIds = results.Select(r => r.RegistrationId).ToList(),
                    TotalCount = results.Count,
                    ResponsibleVisitorId = request.ResponsibleVisitorId
                });
        }
        catch (ArgumentException ex)
        {
            // 业务逻辑验证错误
            return BadRequest(new { 
                Message = "提交批量预约失败", 
                Error = ex.Message,
                Type = "ValidationError"
            });
        }
        catch (Exception)
        {
            // 系统错误
            return StatusCode(500, new { 
                Message = "系统内部错误，请稍后重试", 
                Error = "Internal Server Error",
                Type = "SystemError"
            });
        }
    }

    /// <summary>
    /// 审核视频探视预约申请
    /// </summary>
    /// <remarks>
    /// **功能说明：** 工作人员对访客提交的视频探视预约进行审核，决定批准或拒绝申请。
    /// 
    /// **业务逻辑：**
    /// 1. 根据预约ID查找对应的预约记录
    /// 2. 验证工作人员ID的有效性
    /// 3. 更新预约状态为"已批准"或"已拒绝"
    /// 4. 记录审核人员和审核时间
    /// 5. 创建操作日志记录审核过程
    /// 6. 返回审核结果给前端
    /// 
    /// **输入要求：**
    /// - 预约ID：路径参数，正整数，必须是存在的预约记录
    /// - 审核决定：请求体，true=批准，false=拒绝
    /// - 审核原因：请求体，可选，说明批准或拒绝的原因
    /// - 工作人员ID：请求体，必填，执行批准操作的工作人员，用于记录操作日志，此处没有设置判断是否在工作人员表中
    /// - 工作人员ID应该等于当前登录的工作人员的id
    /// 
    /// **业务规则：**
    /// - 每个预约只能审核一次
    /// - 审核后状态不可再次修改
    /// - 批准的预约可以进行后续的探视安排
    /// - 拒绝的预约需要通知访客并说明原因
    /// 
    /// **成功返回：** 200 OK + 审核结果
    /// **失败返回：** 404 Not Found（预约不存在） 或 400 Bad Request（审核失败）
    /// 
    /// **操作日志：**
    /// - 记录审核人员、时间、决定和原因
    /// - 用于后续的审计和跟踪
    /// 
    /// **示例请求：**
    /// ```
    /// PUT /api/VisitorRegistration/1/approve
    /// ```
    /// ```json
    /// {
    ///   "approve": true,
    ///   "reason": "访客身份验证通过，老人状态良好，可以安排视频探视",
    ///   "staffId": 2
    /// }
    /// ```
    /// 
    /// **示例响应（批准）：**
    /// ```json
    /// {
    ///   "message": "预约已批准",
    ///   "approvalStatus": "已批准",
    ///   "approvedBy": 2,
    ///   "timestamp": "2024-08-27T14:30:00"
    /// }
    /// ```
    /// 
    /// **示例响应（拒绝）：**
    /// ```json
    /// {
    ///   "message": "预约已拒绝",
    ///   "approvalStatus": "已拒绝", 
    ///   "approvedBy": 2,
    ///   "timestamp": "2024-08-27T14:30:00"
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">预约ID，正整数</param>
    /// <param name="request">审核请求对象，包含审核决定、原因和工作人员ID</param>
    /// <returns>审核成功返回结果信息，失败返回错误信息</returns>
    /// <response code="200">审核成功，返回审核结果</response>
    /// <response code="404">预约ID不存在</response>
    /// <response code="400">审核失败或参数错误</response>
    [HttpPut("{id}/approve")]
    public IActionResult ApproveVisit(int id, [FromBody] ApprovalRequest request)
    {
        // 从请求体获取staffId
        int staffId = request.StaffId;

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
    /// 工作人员一键批准某个负责人的所有待审核预约申请
    /// </summary>
    /// <remarks>
    /// **功能说明：** 工作人员可以一键批准指定负责人提交的所有待审核预约申请，提高审核效率。
    /// 
    /// **业务逻辑：**
    /// 1. 验证负责人访客ID是否存在
    /// 2. 查找该负责人所有状态为"待批准"的预约申请
    /// 3. 批量更新这些预约的状态为"已批准"
    /// 4. 记录操作日志
    /// 5. 返回批准的预约数量和详细信息
    /// 
    /// **使用场景：**
    /// - 信任的负责人批量提交预约后，工作人员可以一键批准
    /// - 减少逐个审核的工作量
    /// - 提高审核效率，特别适用于熟悉的访客
    /// 
    /// **输入要求：**
    /// - 负责人访客ID：路径参数，正整数，必须是存在的访客
    /// - 工作人员ID：请求体，必填，执行批准操作的工作人员，用于记录操作日志，此处没有设置判断是否在工作人员表中
    /// - 工作人员ID应该等于当前登录的工作人员的id
    /// 
    /// **业务规则：**
    /// - 只处理状态为"待批准"的预约申请
    /// - 已批准或已拒绝的预约不会被重复处理
    /// - 如果该负责人没有待审核的预约，返回相应提示
    /// 
    /// **成功返回：** 200 OK + 批准统计信息
    /// **失败返回：** 400 Bad Request（负责人不存在） 或 404 Not Found（无待审核预约）
    /// 
    /// **操作日志：**
    /// - 记录批量批准操作的工作人员、时间、影响的预约数量
    /// - 用于后续的审计和跟踪
    /// 
    /// **示例请求：**
    /// ```
    /// PUT /api/VisitorRegistration/responsible/1/approve-all
    /// ```
    /// ```json
    /// {
    ///   "staffId": 2
    /// }
    /// ```
    /// 
    /// **示例响应（成功）：**
    /// ```json
    /// {
    ///   "message": "成功批准 3 条预约申请",
    ///   "responsibleVisitorId": 1,
    ///   "approvedCount": 3,
    ///   "approvalStatus": "已批准",
    ///   "approvedBy": 2,
    ///   "timestamp": "2024-08-27T15:30:00"
    /// }
    /// ```
    /// 
    /// **示例响应（无待审核预约）：**
    /// ```json
    /// {
    ///   "message": "该负责人没有待审核的预约申请",
    ///   "responsibleVisitorId": 1,
    ///   "approvedCount": 0
    /// }
    /// ```
    /// </remarks>
    /// <param name="responsibleVisitorId">负责人访客ID，正整数</param>
    /// <param name="request">批量批准请求对象，包含工作人员ID</param>
    /// <returns>批准成功返回统计信息，失败返回错误信息</returns>
    /// <response code="200">批准成功，返回批准统计</response>
    /// <response code="400">负责人不存在或参数错误</response>
    /// <response code="404">该负责人没有待审核的预约</response>
    [HttpPut("responsible/{responsibleVisitorId}/approve-all")]
    public IActionResult ApproveAllVisitsByResponsiblePerson(int responsibleVisitorId, [FromBody] BulkApprovalRequest request)
    {
        int staffId = request.StaffId;

        try
        {
            int approvedCount = _visitorService.ApproveAllVisitsByResponsiblePerson(responsibleVisitorId);
            
            if (approvedCount == 0)
            {
                return NotFound(new { 
                    message = "该负责人没有待审核的预约申请",
                    responsibleVisitorId = responsibleVisitorId,
                    approvedCount = 0
                });
            }

            // 记录操作日志
            _logService.Create(new OperationLog
            {
                operation_description = $"工作人员批量批准预约：负责人ID {responsibleVisitorId}，批准数量 {approvedCount}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "批量审核预约",
                operation_status = "成功",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return Ok(new { 
                message = $"成功批准 {approvedCount} 条预约申请",
                responsibleVisitorId = responsibleVisitorId,
                approvedCount = approvedCount,
                approvalStatus = "已批准",
                approvedBy = staffId,
                timestamp = DateTime.Now
            });
        }
        catch (ArgumentException ex)
        {
            // 记录失败日志
            _logService.Create(new OperationLog
            {
                operation_description = $"工作人员批量批准预约失败：负责人ID {responsibleVisitorId}，错误：{ex.Message}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "批量审核预约",
                operation_status = "失败",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // 记录失败日志
            _logService.Create(new OperationLog
            {
                operation_description = $"工作人员批量批准预约系统错误：负责人ID {responsibleVisitorId}，错误：{ex.Message}",
                staff_id = staffId,
                operation_time = DateTime.Now,
                operation_type = "批量审核预约",
                operation_status = "失败",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);

            return StatusCode(500, new { message = "服务器内部错误", error = ex.Message });
        }
    }

    /// <summary>
    /// 获取预约记录详情
    /// </summary>
    /// <remarks>
    /// **功能说明：** 根据预约ID查询单个视频探视预约的详细信息。
    /// 
    /// **业务逻辑：**
    /// 1. 根据预约ID查找对应的预约记录
    /// 2. 如果记录存在，返回完整的预约信息
    /// 3. 如果记录不存在，返回404错误
    /// 
    /// **输入要求：**
    /// - 预约ID：路径参数，正整数，必填
    /// 
    /// **返回信息：**
    /// - 预约ID、访客信息、老人信息
    /// - 预约时间、探视类型、审批状态
    /// - 与老人关系、探视原因等详细信息
    /// 
    /// **成功返回：** 200 OK + 预约详情
    /// **失败返回：** 404 Not Found（预约不存在）
    /// 
    /// **使用场景：**
    /// - 访客查看自己的预约状态
    /// - 工作人员查看预约详细信息
    /// - 审核时查看预约申请内容
    /// - 探视安排时确认预约信息
    /// 
    /// **示例请求：**
    /// ```
    /// GET /api/VisitorRegistration/1
    /// ```
    /// 
    /// **示例响应：**
    /// ```json
    /// {
    ///   "registrationId": 1,
    ///   "visitorId": 1,
    ///   "elderlyId": 61,
    ///   "visitorName": "张三",
    ///   "visitTime": "2024-08-27T10:30:00",
    ///   "relationshipToElderly": "儿子",
    ///   "visitReason": "关心父亲身体状况，询问近期生活情况",
    ///   "visitType": "视频探视",
    ///   "approvalStatus": "已批准"
    /// }
    /// ```
    /// </remarks>
    /// <param name="id">预约ID，正整数</param>
    /// <returns>预约详情或错误信息</returns>
    /// <response code="200">查询成功，返回预约详情</response>
    /// <response code="404">预约ID不存在</response>
    [HttpGet("{id}")]
    public IActionResult GetVisit(int id)
    {
        var visits = _visitorService.GetVisits();
        var visit = visits.FirstOrDefault(v => v.RegistrationId == id);
        
        if (visit == null)
            return NotFound($"预约ID {id} 不存在");

        return Ok(visit);
    }

    /// <summary>
    /// 查询预约记录列表
    /// </summary>
    /// <remarks>
    /// **功能说明：** 根据条件查询视频探视预约记录列表，支持多种筛选条件。
    /// 
    /// **业务逻辑：**
    /// 1. 接收查询参数（访客ID、老人ID、审批状态）
    /// 2. 根据条件筛选预约记录
    /// 3. 记录查询操作到日志系统
    /// 4. 返回符合条件的预约列表
    /// 
    /// **输入要求：**
    /// - 访客ID：查询参数，可选，筛选特定访客的预约
    /// - 老人ID：查询参数，可选，筛选特定老人的预约
    /// - 审批状态：查询参数，可选，筛选特定状态的预约
    /// - 工作人员ID：查询参数，必填，用于记录操作日志
    /// 
    /// **筛选条件：**
    /// - 无参数：返回所有预约记录
    /// - 单一条件：按指定条件筛选
    /// - 多重条件：同时满足所有条件的记录
    /// 
    /// **审批状态值：**
    /// - "待批准"：刚提交的预约申请
    /// - "已批准"：审核通过的预约
    /// - "已拒绝"：审核被拒绝的预约
    /// 
    /// **成功返回：** 200 OK + 预约记录数组
    /// **失败返回：** 400 Bad Request 或 500 Internal Server Error
    /// 
    /// **使用场景：**
    /// - 工作人员查看所有待审核的预约
    /// - 访客查看自己的预约历史
    /// - 管理员查看特定老人的探视记录
    /// - 统计分析预约状态分布
    /// 
    /// **示例请求：**
    /// ```
    /// GET /api/VisitorRegistration?staffId=2                    # 查看所有预约
    /// GET /api/VisitorRegistration?visitorId=1&amp;staffId=2        # 查看特定访客的预约
    /// GET /api/VisitorRegistration?status=待批准&amp;staffId=2       # 查看待审核的预约
    /// GET /api/VisitorRegistration?elderlyId=61&amp;status=已批准&amp;staffId=2  # 多条件查询
    /// ```
    /// 
    /// **示例响应：**
    /// ```json
    /// [
    ///   {
    ///     "registrationId": 1,
    ///     "visitorId": 1,
    ///     "elderlyId": 61,
    ///     "visitorName": "张三",
    ///     "visitTime": "2024-08-27T10:30:00",
    ///     "relationshipToElderly": "儿子",
    ///     "visitReason": "关心父亲身体状况",
    ///     "visitType": "视频探视",
    ///     "approvalStatus": "已批准"
    ///   },
    ///   {
    ///     "registrationId": 2,
    ///     "visitorId": 2,
    ///     "elderlyId": 62,
    ///     "visitorName": "李四",
    ///     "visitTime": "2024-08-27T11:00:00",
    ///     "relationshipToElderly": "女儿",
    ///     "visitReason": "周末问候",
    ///     "visitType": "线下探视",
    ///     "approvalStatus": "待批准"
    ///   }
    /// ]
    /// ```
    /// </remarks>
    /// <param name="visitorId">访客ID筛选条件，可选</param>
    /// <param name="elderlyId">老人ID筛选条件，可选</param>
    /// <param name="status">审批状态筛选条件，可选（待批准/已批准/已拒绝）</param>
    /// <param name="staffId">工作人员ID，必填，用于操作日志记录</param>
    /// <returns>符合条件的预约记录列表</returns>
    /// <response code="200">查询成功，返回预约记录列表</response>
    /// <response code="400">参数错误</response>
    /// <response code="500">系统内部错误</response>
    [HttpGet]
    public IActionResult GetVisits([FromQuery] int? visitorId, [FromQuery] int? elderlyId, [FromQuery] string? status, [FromQuery] int staffId)
    {
        // staffId 从查询参数获取（GET 无请求体）

        var visits = _visitorService.GetVisits(visitorId, elderlyId, status);

        // 记录查询操作日志
        _logService.Create(new OperationLog
        {
            operation_description = $"查询视频探视预约记录：访客ID={visitorId}，老人ID={elderlyId}，状态={status}",
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
    /// 获取待审核的预约列表
    /// </summary>
    /// <remarks>
    /// **功能说明：** 工作人员查看所有状态为"待批准"的视频探视预约申请，用于审核管理。
    /// 
    /// **业务逻辑：**
    /// 1. 查询所有审批状态为"待批准"的预约记录
    /// 2. 按预约时间倒序排列（最新的在前）
    /// 3. 记录查询操作到日志系统
    /// 4. 返回完整的预约信息列表
    /// 
    /// **输入要求：**
    /// - 工作人员ID：查询参数，必填，用于操作日志记录
    /// 
    /// **返回信息：**
    /// - 预约ID、访客信息、老人信息
    /// - 预约时间、与老人关系、探视原因
    /// - 当前状态（均为"待批准"）
    /// 
    /// **业务用途：**
    /// - 工作人员集中处理审核任务
    /// - 查看待处理的预约申请数量
    /// - 优先处理紧急或重要的预约
    /// 
    /// **成功返回：** 200 OK + 待审核预约数组
    /// **失败返回：** 500 Internal Server Error
    /// 
    /// **使用场景：**
    /// - 每日工作开始时查看待审核列表
    /// - 定期检查是否有新的预约申请
    /// - 批量处理预约审核工作
    /// 
    /// **示例请求：**
    /// ```
    /// GET /api/VisitorRegistration/pending-approval?staffId=2
    /// ```
    /// 
    /// **示例响应：**
    /// ```json
    /// [
    ///   {
    ///     "registrationId": 3,
    ///     "visitorId": 1,
    ///     "elderlyId": 61,
    ///     "visitorName": "张三",
    ///     "visitTime": "2024-08-27T15:00:00",
    ///     "relationshipToElderly": "儿子",
    ///     "visitReason": "关心父亲身体状况 (探视对象: 指定老人ID 61)",
    ///     "visitType": "视频探视",
    ///     "approvalStatus": "待批准"
    ///   },
    ///   {
    ///     "registrationId": 4,
    ///     "visitorId": 2,
    ///     "elderlyId": 0,
    ///     "visitorName": "李四",
    ///     "visitTime": "2024-08-27T14:30:00",
    ///     "relationshipToElderly": "志愿者",
    ///     "visitReason": "为全体老人进行健康讲座 (探视对象: 全体老人)",
    ///     "visitType": "视频探视",
    ///     "approvalStatus": "待批准"
    ///   }
    /// ]
    /// ```
    /// 
    /// **注意事项：**
    /// - 返回的列表按时间倒序排列
    /// - 只包含状态为"待批准"的记录
    /// - 每次查询都会记录到操作日志
    /// </remarks>
    /// <param name="staffId">工作人员ID，必填，用于操作日志记录</param>
    /// <returns>待审核的预约记录列表</returns>
    /// <response code="200">查询成功，返回待审核预约列表</response>
    /// <response code="500">系统内部错误</response>
    [HttpGet("pending-approval")]
    public IActionResult GetPendingApprovals([FromQuery] int staffId)
    {
        // staffId 从查询参数获取（GET 无请求体）

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

    /// <summary>
    /// 负责人查看自己提交的所有预约申请
    /// </summary>
    /// <remarks>
    /// **功能说明：** 负责人查看自己作为提交者的所有视频探视预约申请记录。
    /// 
    /// **业务逻辑：**
    /// 1. 根据负责人访客ID查询其提交的所有预约记录
    /// 2. 按预约时间倒序排列（最新的在前）
    /// 3. 记录查询操作到日志系统
    /// 4. 返回完整的预约信息列表
    /// 
    /// **输入要求：**
    /// - 负责人访客ID：路径参数，必填，正整数
    /// - 工作人员ID：查询参数，可选，用于操作日志记录
    /// 
    /// **返回信息：**
    /// - 预约ID、实际访客信息、老人信息
    /// - 预约时间、与老人关系、探视原因
    /// - 当前审批状态
    /// 
    /// **业务用途：**
    /// - 负责人查看自己提交的所有预约申请
    /// - 跟踪预约申请的审核状态
    /// - 管理和确认预约信息
    /// 
    /// **成功返回：** 200 OK + 预约记录数组
    /// **失败返回：** 500 Internal Server Error
    /// 
    /// **使用场景：**
    /// - 负责人查看历史预约申请
    /// - 确认预约申请的审核进度
    /// - 管理团体或家庭成员的预约状态
    /// 
    /// **示例请求：**
    /// ```
    /// GET /api/VisitorRegistration/responsible/1?staffId=2
    /// ```
    /// 
    /// **示例响应：**
    /// ```json
    /// [
    ///   {
    ///     "registrationId": 1,
    ///     "visitorId": 1,
    ///     "elderlyId": 61,
    ///     "visitorName": "张三",
    ///     "visitTime": "2024-08-27T15:00:00",
    ///     "relationshipToElderly": "儿子",
    ///     "visitReason": "关心父亲身体状况，询问近期生活情况",
    ///     "visitType": "视频探视",
    ///     "approvalStatus": "已批准"
    ///   },
    ///   {
    ///     "registrationId": 2,
    ///     "visitorId": 1,
    ///     "elderlyId": 61,
    ///     "visitorName": "李四",
    ///     "visitTime": "2024-08-27T15:00:00",
    ///     "relationshipToElderly": "女儿",
    ///     "visitReason": "看望母亲，汇报工作情况",
    ///     "visitType": "视频探视",
    ///     "approvalStatus": "待批准"
    ///   }
    /// ]
    /// ```
    /// 
    /// **注意事项：**
    /// - 返回的列表按时间倒序排列
    /// - 只包含该负责人提交的预约记录
    /// - 包含所有状态的预约记录
    /// </remarks>
    /// <param name="responsibleVisitorId">负责人访客ID，必填</param>
    /// <param name="staffId">工作人员ID，可选，用于操作日志记录</param>
    /// <returns>负责人提交的所有预约记录列表</returns>
    /// <response code="200">查询成功，返回预约记录列表</response>
    /// <response code="500">系统内部错误</response>
    [HttpGet("responsible/{responsibleVisitorId}")]
    public IActionResult GetVisitsByResponsible(int responsibleVisitorId, [FromQuery] int? staffId)
    {
        var visits = _visitorService.GetVisits(visitorId: responsibleVisitorId);

        // 记录操作日志（如果提供了staffId）
        if (staffId.HasValue)
        {
            _logService.Create(new OperationLog
            {
                operation_description = $"负责人查看自己提交的预约记录：负责人ID={responsibleVisitorId}",
                staff_id = staffId.Value,
                operation_time = DateTime.Now,
                operation_type = "查询负责人预约",
                operation_status = "成功",
                affected_entity = "VisitorRegistration",
                ip_address = GetClientIpAddress(),
                device_type = GetDeviceType()
            }, out bool _);
        }

        return Ok(visits);
    }

    // 辅助方法
    // 已改为从请求体/查询参数获取staffId，不再依赖认证身份

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

/// <summary>
/// 预约审核请求模型
/// </summary>
public class ApprovalRequest
{
    /// <summary>
    /// 审核决定（true=批准，false=拒绝）
    /// </summary>
    /// <example>true</example>
    public bool Approve { get; set; }
    
    /// <summary>
    /// 审核原因（可选，说明批准或拒绝的理由）
    /// </summary>
    /// <example>访客身份验证通过，老人状态良好，可以安排视频探视</example>
    public string? Reason { get; set; }
    
    /// <summary>
    /// 执行审核的工作人员ID
    /// </summary>
    /// <example>2</example>
    public int StaffId { get; set; }
}

/// <summary>
/// 视频探视预约申请请求模型
/// </summary>
public class VideoVisitRequest
{
    /// <summary>
    /// 负责人访客ID（必须是已注册的访客）
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "负责人访客ID不能为空")]
    public int ResponsibleVisitorId { get; set; }

    /// <summary>
    /// 实际访客姓名（不需要与负责人ID对应）
    /// </summary>
    /// <example>张三</example>
    [Required(ErrorMessage = "访客姓名不能为空")]
    [StringLength(100, ErrorMessage = "访客姓名不能超过100个字符")]
    public string VisitorName { get; set; } = string.Empty;



    /// <summary>
    /// 与老人的关系
    /// </summary>
    /// <example>儿子</example>
    [Required(ErrorMessage = "与老人关系不能为空")]
    [StringLength(50, ErrorMessage = "与老人关系不能超过50个字符")]
    public string RelationshipToElderly { get; set; } = string.Empty;

    /// <summary>
    /// 探访原因（详细说明探视目的）
    /// </summary>
    /// <example>关心父亲身体状况，询问近期生活情况</example>
    [Required(ErrorMessage = "探访原因不能为空")]
    public string VisitReason { get; set; } = string.Empty;

    /// <summary>
    /// 访问类型（必须选择其中一种）
    /// </summary>
    /// <example>视频探视</example>
    [Required(ErrorMessage = "访问类型不能为空")]
    [RegularExpression("^(视频探视|线下探视)$", ErrorMessage = "访问类型只能是'视频探视'或'线下探视'")]
    public string VisitType { get; set; } = string.Empty;

    /// <summary>
    /// 老人ID（支持两种模式）
    /// 指定老人：输入具体的老人ID（如：61）
    /// 全体老人：输入0，表示申请探视全体老人
    /// </summary>
    /// <example>61</example>
    public int ElderlyId { get; set; }
}

/// <summary>
/// 批量视频探视预约申请请求模型
/// </summary>
public class BulkVideoVisitRequest
{
    /// <summary>
    /// 负责人访客ID（必须是已注册的访客）
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "负责人访客ID不能为空")]
    public int ResponsibleVisitorId { get; set; }

    /// <summary>
    /// 访客列表（每个访客的详细信息）
    /// </summary>
    [Required(ErrorMessage = "访客列表不能为空")]
    [MinLength(1, ErrorMessage = "至少需要一个访客")]
    public List<VisitorInfo> Visitors { get; set; } = new List<VisitorInfo>();

    /// <summary>
    /// 访问类型（必须选择其中一种）
    /// </summary>
    /// <example>视频探视</example>
    [Required(ErrorMessage = "访问类型不能为空")]
    [RegularExpression("^(视频探视|线下探视)$", ErrorMessage = "访问类型只能是'视频探视'或'线下探视'")]
    public string VisitType { get; set; } = string.Empty;

    /// <summary>
    /// 老人ID（支持两种模式）
    /// 指定老人：输入具体的老人ID（如：61）
    /// 全体老人：输入0，表示申请探视全体老人
    /// </summary>
    /// <example>61</example>
    public int ElderlyId { get; set; }
}

/// <summary>
/// 访客信息模型
/// </summary>
public class VisitorInfo
{
    /// <summary>
    /// 访客姓名
    /// </summary>
    /// <example>张三</example>
    [Required(ErrorMessage = "访客姓名不能为空")]
    [StringLength(100, ErrorMessage = "访客姓名不能超过100个字符")]
    public string VisitorName { get; set; } = string.Empty;



    /// <summary>
    /// 与老人的关系
    /// </summary>
    /// <example>儿子</example>
    [Required(ErrorMessage = "与老人关系不能为空")]
    [StringLength(50, ErrorMessage = "与老人关系不能超过50个字符")]
    public string RelationshipToElderly { get; set; } = string.Empty;

    /// <summary>
    /// 探访原因
    /// </summary>
    /// <example>关心父亲身体状况，询问近期生活情况</example>
    [Required(ErrorMessage = "探访原因不能为空")]
    public string VisitReason { get; set; } = string.Empty;
}

/// <summary>
/// 批量批准请求模型
/// </summary>
public class BulkApprovalRequest
{
    /// <summary>
    /// 执行批量批准操作的工作人员ID
    /// </summary>
    /// <example>2</example>
    [Required(ErrorMessage = "工作人员ID不能为空")]
    public int StaffId { get; set; }
} 