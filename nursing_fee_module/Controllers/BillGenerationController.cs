using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillGenerationController : ControllerBase
{
    private readonly FeeCalculationService _feeCalculationService;
    private readonly ILogger<BillGenerationController> _logger;

    public BillGenerationController(
        FeeCalculationService feeCalculationService,
        ILogger<BillGenerationController> logger)
    {
        _feeCalculationService = feeCalculationService;
        _logger = logger;
    }

    /// <summary>
    /// 手动触发账单生成
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>生成的账单数量</returns>
    [HttpPost("generate")]
    public async Task<ActionResult<int>> GenerateBills(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation($"手动触发账单生成: 开始日期 {startDate}, 结束日期 {endDate}");
        try
        {
            int generatedCount = await _feeCalculationService.GenerateBillsForPeriod(startDate, endDate);
            _logger.LogInformation($"成功生成 {generatedCount} 条账单");
            return Ok(generatedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成账单时发生错误");
            return StatusCode(500, "生成账单时发生错误: " + ex.Message);
        }
    }
}