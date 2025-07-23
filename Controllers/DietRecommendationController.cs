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

    [HttpPost("generate/{elderlyId}")]
    public async Task<IActionResult> Generate(int elderlyId)
    {
        var result = await _service.GenerateDietAdviceAsync(elderlyId);
        return Ok(result);
    }

    [HttpGet("{elderlyId}")]
    public async Task<IActionResult> GetAll(int elderlyId)
    {
        var list = await _service.GetRecommendationsByElderlyAsync(elderlyId);
        return Ok(list);
    }

    [HttpPut("{recommendationId}/status")]
    public async Task<IActionResult> UpdateStatus(int recommendationId)
    {
        string status = "已执行";  // 固定状态值

        var success = await _service.UpdateExecutionStatusAsync(recommendationId, status);
        if (!success) return NotFound();
        return Ok("更新成功");
    }

}
