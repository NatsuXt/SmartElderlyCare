using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 健康监测服务 - 中文兼容版本
    /// 实时健康数据监控业务逻辑服务，支持智能手环等IoT设备数据采集、解析、存储、异常检测、警报通知
    /// </summary>
    public class HealthMonitoringService
    {
        private readonly ChineseCompatibleDatabaseService _chineseDbService;
        private readonly ILogger<HealthMonitoringService> _logger;

        public HealthMonitoringService(ChineseCompatibleDatabaseService chineseDbService, ILogger<HealthMonitoringService> logger)
        {
            _chineseDbService = chineseDbService;
            _logger = logger;
        }

        /// <summary>
        /// 处理IoT健康监测设备数据上报（核心业务方法） - 中文兼容
        /// 功能：接收IoT网关上报的健康数据，解析设备ID、老人ID、心率、血压等指标，存入HealthMonitoring表
        /// </summary>
        public async Task<object> HandleHealthDataAsync(HealthDataReportDto healthReport)
        {
            try
            {
                _logger.LogInformation($"🏥 处理健康数据上报 - 老人ID: {healthReport.ElderlyId}");

                // 使用中文兼容服务创建健康记录，处理nullable类型转换
                await _chineseDbService.CreateHealthRecordAsync(
                    healthReport.ElderlyId,
                    healthReport.HeartRate ?? 0, // 如果为null则默认为0
                    healthReport.BloodPressure ?? "未知", // 如果为null则默认为"未知"
                    (decimal)(healthReport.OxygenLevel ?? 0), // 转换为decimal
                    (decimal)(healthReport.Temperature ?? 0), // 转换为decimal
                    healthReport.MeasurementTime,
                    "正常" // 默认状态为正常
                );

                // 记录数据上报日志
                _logger.LogInformation($"📊 健康数据上报成功：老人 {healthReport.ElderlyId}, 心率: {healthReport.HeartRate}, 血压: {healthReport.BloodPressure}, 血氧: {healthReport.OxygenLevel}%, 体温: {healthReport.Temperature}°C");

                return new
                {
                    ElderlyId = healthReport.ElderlyId,
                    MeasurementTime = healthReport.MeasurementTime,
                    DataSaved = true,
                    ProcessTime = DateTime.Now,
                    Message = "健康数据已成功存储到HealthMonitoring表"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 健康数据处理失败: 老人ID {healthReport.ElderlyId}");
                throw;
            }
        }

        /// <summary>
        /// 获取老人健康历史数据 - 中文兼容
        /// </summary>
        public async Task<List<HealthMonitoring>> GetElderlyHealthHistoryAsync(int elderlyId, int days)
        {
            try
            {
                _logger.LogInformation($"📋 获取老人健康历史 - 老人ID: {elderlyId}, 天数: {days}");

                var startDate = DateTime.Now.AddDays(-days);
                var healthRecords = await _chineseDbService.GetHealthRecordsAsync(elderlyId, startDate);

                var result = healthRecords.Select(record => new HealthMonitoring
                {
                    MonitoringId = record.MonitoringId,
                    ElderlyId = record.ElderlyId,
                    HeartRate = record.HeartRate,
                    BloodPressure = record.BloodPressure,
                    OxygenLevel = (float)record.OxygenLevel, // 转换为float
                    Temperature = (float)record.Temperature, // 转换为float
                    MonitoringDate = record.MonitoringDate,
                    Status = record.Status
                }).ToList();

                _logger.LogInformation($"✅ 成功获取 {result.Count} 条健康历史记录");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取老人健康历史失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取健康数据统计 - 中文兼容
        /// </summary>
        public async Task<object> GetHealthStatisticsAsync(int? elderlyId)
        {
            try
            {
                _logger.LogInformation($"📈 获取健康统计数据 - 老人ID: {elderlyId}");

                var healthRecords = await _chineseDbService.GetHealthRecordsAsync(elderlyId);

                if (!healthRecords.Any())
                {
                    return new
                    {
                        TotalRecords = 0,
                        AverageHeartRate = 0m,
                        AverageOxygenLevel = 0m,
                        AverageTemperature = 0m,
                        EarliestRecord = (DateTime?)null,
                        LatestRecord = (DateTime?)null,
                        ElderlyId = elderlyId,
                        Message = "暂无健康数据"
                    };
                }

                var avgHeartRate = healthRecords.Average(h => h.HeartRate);
                var avgOxygenLevel = healthRecords.Average(h => h.OxygenLevel);
                var avgTemperature = healthRecords.Average(h => h.Temperature);
                var earliestRecord = healthRecords.Min(h => h.MonitoringDate);
                var latestRecord = healthRecords.Max(h => h.MonitoringDate);

                var result = new
                {
                    TotalRecords = healthRecords.Count,
                    AverageHeartRate = Math.Round(avgHeartRate, 1),
                    AverageOxygenLevel = Math.Round(avgOxygenLevel, 1),
                    AverageTemperature = Math.Round(avgTemperature, 1),
                    EarliestRecord = earliestRecord,
                    LatestRecord = latestRecord,
                    ElderlyId = elderlyId,
                    Message = elderlyId.HasValue ? $"老人 {elderlyId} 的健康数据统计" : "全体老人健康数据统计"
                };

                _logger.LogInformation($"✅ 成功获取健康统计数据 - 记录数: {healthRecords.Count}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取健康统计数据失败: ElderlyId={elderlyId}, {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取最新健康数据 - 中文兼容
        /// </summary>
        public async Task<HealthMonitoring?> GetLatestHealthDataAsync(int elderlyId)
        {
            try
            {
                _logger.LogInformation($"📊 获取最新健康数据 - 老人ID: {elderlyId}");

                var healthRecords = await _chineseDbService.GetHealthRecordsAsync(elderlyId);
                var latestRecord = healthRecords.FirstOrDefault();

                if (latestRecord == null)
                {
                    _logger.LogInformation($"📭 未找到老人健康数据 - 老人ID: {elderlyId}");
                    return null;
                }

                var result = new HealthMonitoring
                {
                    MonitoringId = latestRecord.MonitoringId,
                    ElderlyId = latestRecord.ElderlyId,
                    HeartRate = latestRecord.HeartRate,
                    BloodPressure = latestRecord.BloodPressure,
                    OxygenLevel = (float)latestRecord.OxygenLevel, // 转换为float
                    Temperature = (float)latestRecord.Temperature, // 转换为float
                    MonitoringDate = latestRecord.MonitoringDate,
                    Status = latestRecord.Status
                };

                _logger.LogInformation($"✅ 成功获取最新健康数据 - 老人ID: {elderlyId}, 状态: {result.Status}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取最新健康数据失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 批量处理健康数据（支持IoT网关批量上报） - 中文兼容
        /// </summary>
        public async Task<object> HandleBatchHealthDataAsync(List<HealthDataReportDto> healthReports)
        {
            try
            {
                _logger.LogInformation($"🔄 批量处理健康数据 - 记录数: {healthReports.Count}");

                var successCount = 0;
                var errorMessages = new List<string>();

                foreach (var report in healthReports)
                {
                    try
                    {
                        await _chineseDbService.CreateHealthRecordAsync(
                            report.ElderlyId,
                            report.HeartRate ?? 0, // 如果为null则默认为0
                            report.BloodPressure ?? "未知", // 如果为null则默认为"未知"
                            (decimal)(report.OxygenLevel ?? 0), // 转换为decimal
                            (decimal)(report.Temperature ?? 0), // 转换为decimal
                            report.MeasurementTime,
                            "正常"
                        );
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = $"老人ID {report.ElderlyId}: {ex.Message}";
                        errorMessages.Add(errorMsg);
                        _logger.LogError(ex, $"❌ 批量处理健康数据失败 - {errorMsg}");
                    }
                }

                var result = new
                {
                    TotalReports = healthReports.Count,
                    SuccessCount = successCount,
                    FailedCount = healthReports.Count - successCount,
                    ProcessTime = DateTime.Now,
                    Message = $"批量健康数据处理完成，成功存储 {successCount} 条记录到HealthMonitoring表",
                    Errors = errorMessages
                };

                _logger.LogInformation($"✅ 批量处理完成 - 成功: {successCount}, 失败: {healthReports.Count - successCount}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 批量处理健康数据失败: {ex.Message}");
                throw;
            }
        }

    }
}
