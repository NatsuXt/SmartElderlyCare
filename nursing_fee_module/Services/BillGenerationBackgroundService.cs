using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace api.Services;

public class BillGenerationBackgroundService : BackgroundService
{
    private readonly FeeCalculationService _feeCalculationService;
    private readonly ILogger<BillGenerationBackgroundService> _logger;
    private readonly PeriodicTimer _timer;

    public BillGenerationBackgroundService(
        FeeCalculationService feeCalculationService,
        ILogger<BillGenerationBackgroundService> logger)
    {
        _feeCalculationService = feeCalculationService;
        _logger = logger;
        // 设置为每月执行一次（实际应用中可能需要更精确的调度）
        _timer = new PeriodicTimer(TimeSpan.FromDays(30));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bill generation background service started.");

        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                // 获取上个月的开始和结束日期
                var now = DateTime.Now;
                var lastMonth = now.AddMonths(-1);
                var startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                var endDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));

                _logger.LogInformation($"Generating bills for period {startDate} to {endDate}");

                // 生成账单
                int generatedCount = await _feeCalculationService.GenerateBillsForPeriod(startDate, endDate);

                _logger.LogInformation($"Successfully generated {generatedCount} bills.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating bills.");
            }
        }

        _logger.LogInformation("Bill generation background service stopped.");
    }
}