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
            var deviceService = scope.ServiceProvider.GetRequiredService<IoTMonitoringService>();
            var databaseService = scope.ServiceProvider.GetRequiredService<DatabaseService>();

            try
            {
                _logger.LogInformation("开始执行设备状态轮询检查...");

                // 1. 轮询所有设备状态
                var monitoringResult = await deviceService.PollAllDeviceStatusAsync();
                
                // 2. 检查是否有新的故障设备
                await CheckForNewFaultDevicesAsync(databaseService, deviceService);

                // 3. 记录监控结果
                _logger.LogInformation("设备状态轮询完成: {Result}", JsonSerializer.Serialize(monitoringResult));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备监控任务执行失败");
                throw;
            }
        }

        /// <summary>
        /// 检查新出现的故障设备并发送通知
        /// </summary>
        private async Task CheckForNewFaultDevicesAsync(DatabaseService databaseService, IoTMonitoringService deviceService)
        {
            try
            {
                // 查询当前故障设备
                var currentFaultDevicesQuery = @"
                    SELECT device_id, device_name, device_type, status, 
                           installation_date, last_maintenance_date, location
                    FROM DeviceStatus 
                    WHERE status = '故障'";

                var currentFaultDevices = await databaseService.QueryAsync<DeviceStatus>(currentFaultDevicesQuery);

                // 查询最近30分钟内状态改变为故障的设备
                var recentFaultDevicesQuery = @"
                    SELECT device_id, device_name, device_type, status, 
                           installation_date, last_maintenance_date, location
                    FROM DeviceStatus 
                    WHERE status = '故障' 
                    AND (last_maintenance_date IS NULL OR last_maintenance_date <= :CheckTime)";

                var checkTime = DateTime.Now.AddMinutes(-30);
                var recentFaultDevices = await databaseService.QueryAsync<DeviceStatus>(
                    recentFaultDevicesQuery, 
                    new { CheckTime = checkTime });

                if (recentFaultDevices.Any())
                {
                    _logger.LogWarning("发现 {Count} 个新故障设备，正在发送通知...", recentFaultDevices.Count());
                    
                    foreach (var device in recentFaultDevices)
                    {
                        _logger.LogWarning($"检测到设备故障: {device.DeviceName} (ID: {device.DeviceId}) - 状态: {device.Status}");
                        
                        // 更新设备的最后检查时间，避免重复通知
                        await UpdateDeviceLastCheckTimeAsync(databaseService, device.DeviceId);
                    }
                }

                // 统计信息
                _logger.LogInformation("设备状态统计 - 总故障设备: {TotalFault}, 新故障设备: {NewFault}", 
                    currentFaultDevices.Count(), recentFaultDevices.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查故障设备时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 更新设备最后检查时间
        /// </summary>
        private async Task UpdateDeviceLastCheckTimeAsync(DatabaseService databaseService, int deviceId)
        {
            try
            {
                var updateQuery = @"
                    UPDATE DeviceStatus 
                    SET last_maintenance_date = :CurrentTime 
                    WHERE device_id = :DeviceId";

                await databaseService.ExecuteAsync(updateQuery, new 
                { 
                    CurrentTime = DateTime.Now, 
                    DeviceId = deviceId 
                });

                _logger.LogDebug("已更新设备 {DeviceId} 的最后检查时间", deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新设备 {DeviceId} 最后检查时间失败", deviceId);
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
