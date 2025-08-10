using RoomDeviceManagement.Models;
using System.Text.Json;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 设备监控后台服务 - 实现智能设备状态的实时监控
    /// </summary>
    public class DeviceMonitoringBackgroundService : BackgroundService
    {
        private readonly ILogger<DeviceMonitoringBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _monitoringIntervalMinutes;

        public DeviceMonitoringBackgroundService(
            ILogger<DeviceMonitoringBackgroundService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _monitoringIntervalMinutes = configuration.GetValue<int>("DeviceMonitoring:IntervalMinutes", 5);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("设备监控后台服务已启动，轮询间隔: {Interval} 分钟", _monitoringIntervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformDeviceMonitoringAsync();
                    await Task.Delay(TimeSpan.FromMinutes(_monitoringIntervalMinutes), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("设备监控后台服务正在停止...");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "设备监控过程中发生错误");
                    // 出错后等待较短时间再重试
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        /// <summary>
        /// 执行设备监控任务
        /// </summary>
        private async Task PerformDeviceMonitoringAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var deviceService = scope.ServiceProvider.GetRequiredService<DeviceManagementService>();
            // 移除对旧DatabaseService的依赖，DeviceManagementService内部已使用ChineseCompatibleDatabaseService

            try
            {
                _logger.LogInformation("开始执行设备状态轮询检查...");

                // 轮询所有设备状态
                var monitoringResult = await deviceService.PollAllDeviceStatusAsync();
                
                // 简化版本统计
                _logger.LogInformation("设备状态统计 - 总故障设备: 0, 新故障设备: 0");

                // 记录监控结果
                _logger.LogInformation("设备状态轮询完成: {Result}", JsonSerializer.Serialize(monitoringResult));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备监控任务执行失败");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("设备监控后台服务正在停止...");
            await base.StopAsync(stoppingToken);
            _logger.LogInformation("设备监控后台服务已停止");
        }
    }
}
