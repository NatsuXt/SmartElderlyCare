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
    /// 获取公告列表。
    /// </summary>
    /// <param name="audience">受众类型，如"家属"、"员工"</param>
    /// <param name="status">公告状态，如"已发布"</param>
    /// <returns>公告列表</returns>
    [HttpGet]
    public IActionResult Get([FromQuery] string? audience, [FromQuery] string? status)
    {
        var anns = _service.GetAnnouncements(audience, status ?? "已发布");
        return Ok(anns);    // 返回请求成功，返回公告列表
    }

    /// <summary>
    /// 发布新公告。
    /// </summary>
    /// <param name="ann">公告实体，包含公告内容、类型、受众等</param>
    /// <returns>新发布的公告信息</returns>
    [HttpPost]
    public IActionResult Publish(SystemAnnouncements ann)
    {
        // 假设 staffId 从 token 或请求体获取，这里用 0 代替
        int staffId = 0;
        if (User.Identity?.IsAuthenticated == true && int.TryParse(User.Identity.Name, out int uid)) 
        //User.Identity.Name 默认表示"当前登录用户的主标识"。
            staffId = uid;
        var created = _service.Publish(ann, staffId);
        return CreatedAtAction(nameof(Get), new { id = created.announcement_id }, created);
    }

    /// <summary>
    /// 撤回（失效）指定公告。
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>无内容，操作成功返回204，失败返回404</returns>
    [HttpPut("{id}/deactivate")]
    public IActionResult Deactivate(int id)
    {
        int staffId = 0;
        if (User.Identity?.IsAuthenticated == true && int.TryParse(User.Identity.Name, out int uid))
            staffId = uid;
        if (!_service.Deactivate(id, staffId))
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// 给指定公告添加评论。
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <param name="comment">评论内容</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/comment")]
    public IActionResult AddComment(int id, [FromBody] CommentRequest request)
    {
        if (string.IsNullOrEmpty(request.Comment))
            return BadRequest("评论内容不能为空");
            
        if (!_service.AddComment(id, request.Comment))
            return NotFound($"公告ID {id} 不存在");
            
        return Ok(new { Message = "评论添加成功", AnnouncementId = id });
    }

    /// <summary>
    /// 标记公告为已读。
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}/read")]
    public IActionResult MarkAsRead(int id)
    {
        if (!_service.MarkAsRead(id))
            return NotFound();
        return Ok();
    }
}

// 请求模型类
public class CommentRequest
{
    public string Comment { get; set; } = string.Empty;
} 