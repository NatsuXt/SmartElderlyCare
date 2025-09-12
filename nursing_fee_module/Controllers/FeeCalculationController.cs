using Microsoft.AspNetCore.Mvc;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeeCalculationController : ControllerBase
{
    private readonly FeeCalculationService _feeCalculationService;

    public FeeCalculationController(FeeCalculationService feeCalculationService)
    {
        _feeCalculationService = feeCalculationService;
    }

    /// <summary>
    /// 计算指定老人在指定时间段内的总费用
    /// </summary>
    /// <param name="elderlyId">老人ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>总费用</returns>
    [HttpGet("calculate/{elderlyId}")]
    public async Task<ActionResult<decimal>> CalculateTotalFee(int elderlyId, DateTime startDate, DateTime endDate)
    {
        var totalFee = await _feeCalculationService.CalculateTotalFee(elderlyId, startDate, endDate);
        return Ok(totalFee);
    }

    /// <summary>
    /// 为指定时间段内的所有老人生成账单
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>生成的账单数量</returns>
    [HttpPost("generate-bills")]
    public async Task<ActionResult<int>> GenerateBills(DateTime startDate, DateTime endDate)
    {
        var billCount = await _feeCalculationService.GenerateBillsForPeriod(startDate, endDate);
        return Ok(billCount);
    }
}