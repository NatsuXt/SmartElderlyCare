using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ElderlyCareSystem.Services;
[ApiController]
[Route("api/[controller]")]
public class DietRecommendationController : ControllerBase
{
    private readonly DietRecommendationService _service;

    public DietRecommendationController(DietRecommendationService service)
    {
        _service = service;
    }
    /// <summary>
    /// 获取老人的健康建议
    /// </summary>
    /// <param name="elderlyId">老人的id</param>
    /// <returns>返回饮食建议</returns>
    [HttpPost("generate/{elderlyId}")]
    public async Task<IActionResult> Generate(int elderlyId)
    {
        var result = await _service.GenerateDietAdviceAsync(elderlyId);
        return Ok(result);
    }
    /// <summary>
    /// 读取老人的diet信息
    /// </summary>
    /// <param name="elderlyId">老人的id</param>
    /// <returns>返回DietRecommendation表格</returns>
    [HttpGet("{elderlyId}")]
    public async Task<IActionResult> GetAll(int elderlyId)
    {
        var list = await _service.GetRecommendationsByElderlyAsync(elderlyId);
        return Ok(list);
    }
    /// <summary>
    /// 查看并确认是否设置饮食计划为“已执行”
    /// </summary>
    /// <param name="recommendationId">饮食建议的唯一ID</param>
    /// <param name="confirm">是否确认设置为“已执行”</param>
    /// <returns></returns>
    [HttpPut("{recommendationId}/status")]
    public async Task<IActionResult> UpdateStatus(int recommendationId, [FromQuery] bool confirm = false)
    {
        var recommendation = await _service.GetRecommendationByIdAsync(recommendationId);
        if (recommendation == null)
            return NotFound("未找到该饮食建议");

        if (!confirm)
        {
            // 返回当前建议的状态，并提示是否设置为“已执行”
            return Ok(new
            {
                Message = "当前状态：" + recommendation.ExecutionStatus + "，确认设置为“已执行”请加上参数 ?confirm=true",
                CurrentStatus = recommendation.ExecutionStatus,
                Recommendation = recommendation
            });
        }

        // 用户已确认，则执行更新
        var success = await _service.UpdateExecutionStatusAsync(recommendationId, "已执行");
        if (!success) return StatusCode(500, "状态更新失败");

        return Ok("已成功设置为“已执行”");
    }


}
