using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class OperationLogController : ControllerBase
{
    private readonly OperationLogService _service;

    public OperationLogController(OperationLogService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取所有操作日志。
    /// </summary>
    /// <returns>操作日志列表</returns>
    [HttpGet]
    public IActionResult GetAll()
    {
        var logs = _service.GetAll();
        return Ok(logs);
    }

    /// <summary>
    /// 根据ID获取单条操作日志。
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>操作日志详情</returns>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var log = _service.GetById(id);
        if (log == null)
            return NotFound();
        return Ok(log);
    }

    /// <summary>
    /// 创建一条新的操作日志。
    /// </summary>
    /// <param name="log">操作日志实体，包含操作描述、类型、影响对象等</param>
    /// <returns>新建的操作日志信息</returns>
    [HttpPost]
    public IActionResult Create(OperationLog log)
    {
        // 假设 staff_id 从 token 或请求体获取，这里用 0 代替
        int staffId = 0;
        if (User.Identity.IsAuthenticated && int.TryParse(User.Identity.Name, out int uid))
            staffId = uid;
        log.staff_id = staffId;
        try
        {
            bool isSensitive;
            var created = _service.Create(log, out isSensitive);
            if (isSensitive)
            {
                return CreatedAtAction(nameof(GetById), new { id = created.log_id }, new { log = created, warning = "敏感操作已记录，请注意！" });
            }
            return CreatedAtAction(nameof(GetById), new { id = created.log_id }, created);
        }
        catch (ArgumentException ex)
        {
            _service.LogException(staffId, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _service.LogException(staffId, ex.Message);
            return StatusCode(500, new { error = "服务器内部错误" });
        }
    }
}