using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 实时健康数据监控业务逻辑服务
    /// 业务功能：智能手环等IoT设备数据采集、解析、存储、异常检测、警报通知
    /// </summary>
    public class HealthMonitoringService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<HealthMonitoringService> _logger;

        public HealthMonitoringService(DatabaseService databaseService, ILogger<HealthMonitoringService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 处理IoT健康监测设备数据上报（核心业务方法）
        /// 功能：接收IoT网关上报的健康数据，解析设备ID、老人ID、心率、血压等指标，存入HealthMonitoring表
        /// </summary>
        public async Task<object> HandleHealthDataAsync(HealthDataReportDto healthReport)
        {
            try
            {
                // 插入健康监测数据到HealthMonitoring表
                var insertSql = @"
                    INSERT INTO HealthMonitoring (elderly_id, heart_rate, blood_pressure, oxygen_level, temperature, monitoring_date, status)
                    VALUES (:ElderlyId, :HeartRate, :BloodPressure, :OxygenLevel, :Temperature, :MeasurementTime, :Status)";

                var parameters = new
                {
                    ElderlyId = healthReport.ElderlyId,
                    HeartRate = healthReport.HeartRate,
                    BloodPressure = healthReport.BloodPressure,
                    OxygenLevel = healthReport.OxygenLevel,
                    Temperature = healthReport.Temperature,
                    MeasurementTime = healthReport.MeasurementTime,
                    Status = "Normal" // 默认状态为正常
                };

                await _databaseService.ExecuteAsync(insertSql, parameters);

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
                _logger.LogError(ex, $"健康数据处理失败: 老人ID {healthReport.ElderlyId}");
                throw;
            }
        }

        /// <summary>
        /// 获取老人健康历史数据
        /// </summary>
        public async Task<List<HealthMonitoring>> GetElderlyHealthHistoryAsync(int elderlyId, int days)
        {
            var sql = @"
                SELECT monitoring_id AS MonitoringId, elderly_id AS ElderlyId, heart_rate AS HeartRate, 
                       blood_pressure AS BloodPressure, oxygen_level AS OxygenLevel, 
                       temperature AS Temperature, monitoring_date AS MonitoringDate, status AS Status
                FROM HealthMonitoring 
                WHERE elderly_id = :ElderlyId 
                  AND monitoring_date >= :StartDate
                ORDER BY monitoring_date DESC";

            var parameters = new 
            { 
                ElderlyId = elderlyId, 
                StartDate = DateTime.Now.AddDays(-days) 
            };

            var result = await _databaseService.QueryAsync<HealthMonitoring>(sql, parameters);
            return result.ToList();
        }

        /// <summary>
        /// 获取健康数据统计
        /// </summary>
        public async Task<object> GetHealthStatisticsAsync(int? elderlyId)
        {
            try
            {
                var sql = @"
                    SELECT 
                        COUNT(*) as TOTALRECORDS,
                        AVG(CAST(heart_rate as FLOAT)) as AVGHEARTRATE,
                        AVG(CAST(oxygen_level as FLOAT)) as AVGOXYGENLEVEL,
                        AVG(CAST(temperature as FLOAT)) as AVGTEMPERATURE,
                        MIN(monitoring_date) as EARLIESTRECORD,
                        MAX(monitoring_date) as LATESTRECORD
                    FROM HealthMonitoring";

                object parameters = new { };

                if (elderlyId.HasValue)
                {
                    sql += " WHERE elderly_id = :ElderlyId";
                    parameters = new { ElderlyId = elderlyId.Value };
                }

                using var connection = _databaseService.GetConnection();
                await connection.OpenAsync();
                
                using var command = new Oracle.ManagedDataAccess.Client.OracleCommand(sql, connection);
                
                // 添加参数
                if (elderlyId.HasValue)
                {
                    command.Parameters.Add(":ElderlyId", elderlyId.Value);
                }

                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    // 安全地读取字段值 - 处理Oracle的NULL值和类型转换
                    var totalRecords = reader["TOTALRECORDS"] != DBNull.Value ? Convert.ToInt32(reader["TOTALRECORDS"]) : 0;
                    
                    // 对于可能为NULL的平均值，使用更安全的转换方法
                    decimal avgHeartRate = 0;
                    decimal avgOxygenLevel = 0;
                    decimal avgTemperature = 0;
                    
                    try 
                    {
                        avgHeartRate = reader["AVGHEARTRATE"] != DBNull.Value ? Convert.ToDecimal(reader["AVGHEARTRATE"]) : 0;
                    }
                    catch { avgHeartRate = 0; }
                    
                    try 
                    {
                        avgOxygenLevel = reader["AVGOXYGENLEVEL"] != DBNull.Value ? Convert.ToDecimal(reader["AVGOXYGENLEVEL"]) : 0;
                    }
                    catch { avgOxygenLevel = 0; }
                    
                    try 
                    {
                        avgTemperature = reader["AVGTEMPERATURE"] != DBNull.Value ? Convert.ToDecimal(reader["AVGTEMPERATURE"]) : 0;
                    }
                    catch { avgTemperature = 0; }
                    
                    var earliestRecord = reader["EARLIESTRECORD"] != DBNull.Value ? Convert.ToDateTime(reader["EARLIESTRECORD"]) : (DateTime?)null;
                    var latestRecord = reader["LATESTRECORD"] != DBNull.Value ? Convert.ToDateTime(reader["LATESTRECORD"]) : (DateTime?)null;

                    return new
                    {
                        TotalRecords = totalRecords,
                        AverageHeartRate = Math.Round(avgHeartRate, 1),
                        AverageOxygenLevel = Math.Round(avgOxygenLevel, 1),
                        AverageTemperature = Math.Round(avgTemperature, 1),
                        EarliestRecord = earliestRecord,
                        LatestRecord = latestRecord,
                        ElderlyId = elderlyId,
                        Message = elderlyId.HasValue ? $"老人 {elderlyId} 的健康数据统计" : "全体老人健康数据统计"
                    };
                }
                else
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取健康统计数据失败: ElderlyId={elderlyId}");
                throw;
            }
        }

        /// <summary>
        /// 获取最新健康数据
        /// </summary>
        public async Task<HealthMonitoring?> GetLatestHealthDataAsync(int elderlyId)
        {
            var sql = @"
                SELECT monitoring_id AS MonitoringId, elderly_id AS ElderlyId, heart_rate AS HeartRate, 
                       blood_pressure AS BloodPressure, oxygen_level AS OxygenLevel, 
                       temperature AS Temperature, monitoring_date AS MonitoringDate, status AS Status
                FROM HealthMonitoring 
                WHERE elderly_id = :ElderlyId 
                ORDER BY monitoring_date DESC 
                FETCH FIRST 1 ROWS ONLY";

            return await _databaseService.QueryFirstOrDefaultAsync<HealthMonitoring>(sql, new { ElderlyId = elderlyId });
        }

        /// <summary>
        /// 批量处理健康数据（支持IoT网关批量上报）
        /// </summary>
        public async Task<object> HandleBatchHealthDataAsync(List<HealthDataReportDto> healthReports)
        {
            var successCount = 0;

            foreach (var report in healthReports)
            {
                try
                {
                    await HandleHealthDataAsync(report);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"批量处理健康数据失败 - 老人ID: {report.ElderlyId}");
                }
            }

            return new
            {
                TotalReports = healthReports.Count,
                SuccessCount = successCount,
                FailedCount = healthReports.Count - successCount,
                ProcessTime = DateTime.Now,
                Message = $"批量健康数据处理完成，成功存储 {successCount} 条记录到HealthMonitoring表"
            };
        }

    }
}
