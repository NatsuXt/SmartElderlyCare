using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class SystemAnnouncementController : ControllerBase
{
    private readonly SystemAnnouncementService _service;

    public SystemAnnouncementController(SystemAnnouncementService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取公告列表
    /// </summary>
    /// <remarks>
    /// 获取系统公告列表，支持按受众类型和状态进行筛选。
    /// 
    /// **受众类型说明：**
    /// - "家属"：针对老人家属的公告
    /// - "员工"：针对养老院员工的公告  
    /// - "全体"：针对所有人的公告
    /// - 不传入audience参数：获取所有公告
    /// 
    /// **状态说明：**
    /// - "已发布"：正常显示的公告（默认值）
    /// - "已撤回"：已撤回的公告
    /// 
    /// **注意：** 查询特定受众时，会自动包含受众为"全体"的公告
    /// </remarks>
    /// <param name="audience">受众类型筛选，可选值：家属、员工、全体。不传入则查询所有受众</param>
    /// <param name="status">公告状态筛选，可选值：已发布、已撤回。默认为"已发布"</param>
    /// <returns>
    /// 返回公告列表，按发布日期降序排列
    /// </returns>
    /// <response code="200">成功返回公告列表</response>
    /// <response code="400">请求参数错误</response>
    [HttpGet]
    public IActionResult Get([FromQuery] string? audience, [FromQuery] string? status)
    {
        var anns = _service.GetAnnouncements(audience, status ?? "已发布");
        return Ok(anns);    // 返回请求成功，返回公告列表
    }

    /// <summary>
    /// 发布新公告
    /// </summary>
    /// <remarks>
    /// 创建并发布一条新的系统公告。
    /// 
    /// **请求示例：**
    /// ```json
    /// {
    ///   "content": "请注意：明天上午9点将进行消防演练，请各位配合。",
    ///   "type": "紧急通知",
    ///   "audience": "全体",
    ///   "staffId": 1
    /// }
    /// ```
    /// 
    /// **字段说明：**
    /// - content: 公告内容，支持长文本
    /// - type: 公告类型，如"紧急通知"、"常规通知"、"活动安排"等
    /// - audience: 受众类型，必须是"家属"、"员工"或"全体"之一
    /// - staffId: 发布人的员工ID
    /// 
    /// **自动设置的字段：**
    /// - announcement_date: 系统当前时间
    /// - status: 自动设为"已发布"
    /// - comments: 初始化为空字符串
    /// </remarks>
    /// <param name="request">公告发布请求，包含公告内容、类型、受众和发布人信息</param>
    /// <returns>
    /// 返回新发布的公告完整信息，包含自动生成的ID和发布时间
    /// </returns>
    /// <response code="201">公告发布成功，返回公告详情</response>
    /// <response code="400">请求数据无效，如受众类型不正确</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost]
    public IActionResult Publish([FromBody] AnnouncementRequest request)
    {
        // 从请求体中获取staffId
        int staffId = request.StaffId;
        
        // 创建公告实体
        var ann = new SystemAnnouncements
        {
            announcement_content = request.Content,
            announcement_type = request.Type,
            audience = request.Audience,
            status = "已发布"
        };
        
        var created = _service.Publish(ann, staffId);
        return CreatedAtAction(nameof(Get), new { id = created.announcement_id }, created);
    }

    /// <summary>
    /// 撤回指定公告
    /// </summary>
    /// <remarks>
    /// 撤回已发布的公告，将公告状态更改为"已撤回"。
    /// 
    /// **请求示例：**
    /// ```json
    /// PUT /api/SystemAnnouncement/123/deactivate
    /// {
    ///   "staffId": 1
    /// }
    /// ```
    /// 
    /// **操作说明：**
    /// - 只能撤回状态为"已发布"的公告
    /// - 撤回后的公告不会在正常查询中显示
    /// - 操作会记录到操作日志中，记录执行撤回操作的员工ID
    /// - 撤回操作不可逆
    /// 
    /// **字段说明：**
    /// - staffId: 执行撤回操作的员工ID，用于操作日志记录
    /// 
    /// **权限要求：**
    /// - 建议只允许发布者本人或管理员执行撤回操作
    /// </remarks>
    /// <param name="id">要撤回的公告ID</param>
    /// <param name="request">撤回请求，包含执行操作的员工ID</param>
    /// <returns>
    /// 无返回内容，通过HTTP状态码表示操作结果
    /// </returns>
    /// <response code="204">公告撤回成功，无返回内容</response>
    /// <response code="400">请求数据无效，如员工ID为空</response>
    /// <response code="404">公告不存在或已被撤回</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id}/deactivate")]
    public IActionResult Deactivate(int id, [FromBody] DeactivateRequest request)
    {
        if (request.StaffId <= 0)
            return BadRequest("员工ID必须大于0");
            
        if (!_service.Deactivate(id, request.StaffId))
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// 为公告添加评论
    /// </summary>
    /// <remarks>
    /// 为指定的公告添加评论，支持家属、员工等不同身份的用户评论。
    /// 
    /// **请求示例：**
    /// ```json
    /// {
    ///   "comment": "谢谢提醒，我们会准时参加活动。",
    ///   "commenterId": 123,
    ///   "commenterType": "家属"
    /// }
    /// ```
    /// 
    /// **字段说明：**
    /// - comment: 评论内容，不能为空
    /// - commenterId: 评论者的ID（家属ID或员工ID）
    /// - commenterType: 评论者身份，可选值："家属"、"员工"、"老人"
    /// 
    /// **业务规则：**
    /// - 评论内容不能为空
    /// - 评论者身份必须指定
    /// - 只能对存在的公告添加评论
    /// - 评论会追加到公告的comments字段中
    /// 
    /// **使用场景：**
    /// - 家属对公告的反馈和确认
    /// - 员工之间的协作沟通
    /// - 老人对活动安排的回应
    /// </remarks>
    /// <param name="id">公告ID</param>
    /// <param name="request">评论请求，包含评论内容、评论者ID和身份类型</param>
    /// <returns>
    /// 返回操作结果和公告ID
    /// </returns>
    /// <response code="200">评论添加成功</response>
    /// <response code="400">请求数据无效，如评论内容为空或身份类型错误</response>
    /// <response code="404">指定的公告不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("{id}/comment")]
    public IActionResult AddComment(int id, [FromBody] CommentRequest request)
    {
        if (string.IsNullOrEmpty(request.Comment))
            return BadRequest("评论内容不能为空");
            
        if (string.IsNullOrEmpty(request.CommenterType))
            return BadRequest("评论者身份不能为空");
            
        if (!_service.AddComment(id, request.Comment, request.CommenterId, request.CommenterType))
            return NotFound($"公告ID {id} 不存在");
            
        return Ok(new { Message = "评论添加成功", AnnouncementId = id });
    }


}

/// <summary>
/// 公告评论请求模型
/// </summary>
public class CommentRequest
{
    /// <summary>
    /// 评论内容
    /// </summary>
    /// <example>谢谢提醒，我们会准时参加活动。</example>
    public string Comment { get; set; } = string.Empty;
    
    /// <summary>
    /// 评论者ID（对应家属ID、员工ID或老人ID）
    /// </summary>
    /// <example>123</example>
    public int CommenterId { get; set; }
    
    /// <summary>
    /// 评论者身份类型
    /// </summary>
    /// <example>家属</example>
    public string CommenterType { get; set; } = string.Empty; // 家属/员工/老人
}

/// <summary>
/// 公告发布请求模型
/// </summary>
public class AnnouncementRequest
{
    /// <summary>
    /// 公告内容
    /// </summary>
    /// <example>请注意：明天上午9点将进行消防演练，请各位配合。</example>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 公告类型
    /// </summary>
    /// <example>紧急通知</example>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// 受众类型，必须是："家属"、"员工"、"全体"之一
    /// </summary>
    /// <example>全体</example>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// 发布人员工ID
    /// </summary>
    /// <example>1</example>
    public int StaffId { get; set; }
}

/// <summary>
/// 公告撤回请求模型
/// </summary>
public class DeactivateRequest
{
    /// <summary>
    /// 执行撤回操作的员工ID
    /// </summary>
    /// <example>1</example>
    public int StaffId { get; set; }
} 