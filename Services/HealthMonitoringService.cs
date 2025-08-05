using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 健康监测业务逻辑服务
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
        /// 处理健康数据上报
        /// </summary>
        public async Task<object> HandleHealthDataAsync(HealthDataReportDto healthReport)
        {
            // 插入健康监测数据
            var insertSql = @"
                INSERT INTO HealthMonitoring (elderly_id, heart_rate, blood_pressure, oxygen_level, temperature, monitoring_date)
                VALUES (:ElderlyId, :HeartRate, :BloodPressure, :OxygenLevel, :Temperature, :MeasurementTime)";

            var parameters = new
            {
                ElderlyId = healthReport.ElderlyId,
                HeartRate = healthReport.HeartRate,
                BloodPressure = healthReport.BloodPressure,
                OxygenLevel = healthReport.OxygenLevel,
                Temperature = healthReport.Temperature,
                MeasurementTime = healthReport.MeasurementTime
            };

            await _databaseService.ExecuteAsync(insertSql, parameters);

            // 检查健康指标是否异常
            var abnormalAlerts = await CheckHealthAbnormalitiesAsync(healthReport);

            if (abnormalAlerts.Any())
            {
                // 通知医护人员
                await NotifyMedicalStaffAsync(healthReport.ElderlyId, abnormalAlerts);
                
                _logger.LogWarning($"老人 {healthReport.ElderlyId} 健康指标异常: {string.Join(", ", abnormalAlerts)}");
            }

            return new
            {
                ElderlyId = healthReport.ElderlyId,
                MeasurementTime = healthReport.MeasurementTime,
                DataSaved = true,
                AbnormalAlerts = abnormalAlerts,
                AlertCount = abnormalAlerts.Count
            };
        }

        /// <summary>
        /// 获取老人健康历史数据
        /// </summary>
        public async Task<List<HealthMonitoring>> GetElderlyHealthHistoryAsync(int elderlyId, int days)
        {
            var sql = @"
                SELECT monitoring_id, elderly_id, heart_rate, blood_pressure, oxygen_level, temperature, monitoring_date
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
            var sql = @"
                SELECT 
                    COUNT(*) as TotalRecords,
                    AVG(CAST(heart_rate as FLOAT)) as AvgHeartRate,
                    AVG(CAST(oxygen_level as FLOAT)) as AvgOxygenLevel,
                    AVG(CAST(temperature as FLOAT)) as AvgTemperature,
                    MIN(monitoring_date) as EarliestRecord,
                    MAX(monitoring_date) as LatestRecord
                FROM HealthMonitoring";

            object parameters = new { };

            if (elderlyId.HasValue)
            {
                sql += " WHERE elderly_id = :ElderlyId";
                parameters = new { ElderlyId = elderlyId.Value };
            }

            var stats = await _databaseService.QueryFirstAsync<dynamic>(sql, parameters);

            return new
            {
                TotalRecords = stats.TotalRecords,
                AverageHeartRate = Math.Round((decimal)(stats.AvgHeartRate ?? 0), 1),
                AverageOxygenLevel = Math.Round((decimal)(stats.AvgOxygenLevel ?? 0), 1),
                AverageTemperature = Math.Round((decimal)(stats.AvgTemperature ?? 0), 1),
                EarliestRecord = stats.EarliestRecord,
                LatestRecord = stats.LatestRecord,
                ElderlyId = elderlyId
            };
        }

        /// <summary>
        /// 获取健康警报
        /// </summary>
        public async Task<List<object>> GetHealthAlertsAsync(int? elderlyId, bool activeOnly)
        {
            var sql = @"
                SELECT 
                    h.HealthId,
                    h.ElderlyId,
                    e.Name as ElderlyName,
                    h.HeartRate,
                    h.BloodPressure,
                    h.OxygenLevel,
                    h.Temperature,
                    h.MeasurementTime
                FROM HealthMonitoring h
                INNER JOIN ElderlyInfo e ON h.ElderlyId = e.ElderlyId
                WHERE (
                    (h.HeartRate IS NOT NULL AND (h.HeartRate < 60 OR h.HeartRate > 100)) OR
                    (h.OxygenLevel IS NOT NULL AND h.OxygenLevel < 95) OR
                    (h.Temperature IS NOT NULL AND (h.Temperature < 36.0 OR h.Temperature > 37.5))
                )";

            object parameters = new { };

            if (elderlyId.HasValue)
            {
                sql += " AND h.ElderlyId = :ElderlyId";
                parameters = new { ElderlyId = elderlyId.Value };
            }

            if (activeOnly)
            {
                sql += " AND h.MeasurementTime >= :RecentTime";
                parameters = elderlyId.HasValue 
                    ? new { ElderlyId = elderlyId.Value, RecentTime = DateTime.Now.AddHours(-24) }
                    : new { RecentTime = DateTime.Now.AddHours(-24) };
            }

            sql += " ORDER BY h.MeasurementTime DESC";

            var alerts = await _databaseService.QueryAsync<dynamic>(sql, parameters);
            
            return alerts.Select(a => new
            {
                HealthId = a.HealthId,
                ElderlyId = a.ElderlyId,
                ElderlyName = a.ElderlyName,
                HeartRate = a.HeartRate,
                BloodPressure = a.BloodPressure,
                OxygenLevel = a.OxygenLevel,
                Temperature = a.Temperature,
                MeasurementTime = a.MeasurementTime,
                AlertReasons = GetAlertReasons(a.HeartRate, a.OxygenLevel, a.Temperature)
            }).ToList<object>();
        }

        /// <summary>
        /// 获取最新健康数据
        /// </summary>
        public async Task<HealthMonitoring?> GetLatestHealthDataAsync(int elderlyId)
        {
            var sql = @"
                SELECT HealthId, ElderlyId, HeartRate, BloodPressure, OxygenLevel, Temperature, MeasurementTime
                FROM HealthMonitoring 
                WHERE ElderlyId = :ElderlyId 
                ORDER BY MeasurementTime DESC 
                ROWNUM <= 1";

            return await _databaseService.QueryFirstAsync<HealthMonitoring>(sql, new { ElderlyId = elderlyId });
        }

        /// <summary>
        /// 获取健康趋势分析
        /// </summary>
        public async Task<object> GetHealthTrendsAsync(int elderlyId, int days)
        {
            var sql = @"
                SELECT 
                    TRUNC(MeasurementTime) as MeasurementDate,
                    AVG(CAST(HeartRate as FLOAT)) as AvgHeartRate,
                    AVG(CAST(OxygenLevel as FLOAT)) as AvgOxygenLevel,
                    AVG(CAST(Temperature as FLOAT)) as AvgTemperature,
                    COUNT(*) as RecordCount
                FROM HealthMonitoring 
                WHERE ElderlyId = :ElderlyId 
                  AND MeasurementTime >= :StartDate
                GROUP BY TRUNC(MeasurementTime)
                ORDER BY MeasurementDate";

            var parameters = new 
            { 
                ElderlyId = elderlyId, 
                StartDate = DateTime.Now.AddDays(-days) 
            };

            var trends = await _databaseService.QueryAsync<dynamic>(sql, parameters);
            
            return new
            {
                ElderlyId = elderlyId,
                Days = days,
                TrendData = trends.Select(t => new
                {
                    Date = t.MeasurementDate,
                    AverageHeartRate = Math.Round((decimal)(t.AvgHeartRate ?? 0), 1),
                    AverageOxygenLevel = Math.Round((decimal)(t.AvgOxygenLevel ?? 0), 1),
                    AverageTemperature = Math.Round((decimal)(t.AvgTemperature ?? 0), 1),
                    RecordCount = t.RecordCount
                }).ToList()
            };
        }

        /// <summary>
        /// 批量处理健康数据
        /// </summary>
        public async Task<object> HandleBatchHealthDataAsync(List<HealthDataReportDto> healthReports)
        {
            var successCount = 0;
            var totalAlerts = new List<string>();

            foreach (var report in healthReports)
            {
                try
                {
                    var result = await HandleHealthDataAsync(report);
                    successCount++;
                    
                    // 收集警报信息
                    if (result is { } resultObj)
                    {
                        var props = resultObj.GetType().GetProperties();
                        var alertsProp = props.FirstOrDefault(p => p.Name == "AbnormalAlerts");
                        if (alertsProp?.GetValue(resultObj) is List<string> alerts)
                        {
                            totalAlerts.AddRange(alerts);
                        }
                    }
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
                TotalAlerts = totalAlerts.Count,
                AlertDetails = totalAlerts.Distinct().ToList()
            };
        }

        /// <summary>
        /// 检查健康指标异常
        /// </summary>
        private Task<List<string>> CheckHealthAbnormalitiesAsync(HealthDataReportDto healthReport)
        {
            var alerts = new List<string>();

            // 心率异常检查
            if (healthReport.HeartRate.HasValue)
            {
                if (healthReport.HeartRate < 60)
                    alerts.Add($"心率过低: {healthReport.HeartRate} BPM (正常范围: 60-100)");
                else if (healthReport.HeartRate > 100)
                    alerts.Add($"心率过高: {healthReport.HeartRate} BPM (正常范围: 60-100)");
            }

            // 血氧异常检查
            if (healthReport.OxygenLevel.HasValue && healthReport.OxygenLevel < 95)
            {
                alerts.Add($"血氧饱和度过低: {healthReport.OxygenLevel}% (正常范围: ≥95%)");
            }

            // 体温异常检查
            if (healthReport.Temperature.HasValue)
            {
                if (healthReport.Temperature < 36.0f)
                    alerts.Add($"体温过低: {healthReport.Temperature}°C (正常范围: 36.0-37.5°C)");
                else if (healthReport.Temperature > 37.5f)
                    alerts.Add($"体温过高: {healthReport.Temperature}°C (正常范围: 36.0-37.5°C)");
            }

            return Task.FromResult(alerts);
        }

        /// <summary>
        /// 通知医护人员
        /// </summary>
        private async Task NotifyMedicalStaffAsync(int elderlyId, List<string> alerts)
        {
            try
            {
                // 获取老人信息
                var elderlyInfo = await _databaseService.QueryFirstAsync<ElderlyInfo>(
                    "SELECT Name FROM ElderlyInfo WHERE ElderlyId = :ElderlyId", 
                    new { ElderlyId = elderlyId });

                // 获取医护人员信息
                var sql = @"
                    SELECT StaffId, Name, ContactPhone, Email 
                    FROM StaffInfo 
                    WHERE Position LIKE '%医生%' OR Position LIKE '%护士%' OR Position LIKE '%医护%'";

                var medicalStaff = await _databaseService.QueryAsync<StaffInfo>(sql);

                foreach (var staff in medicalStaff)
                {
                    _logger.LogInformation(
                        $"通知医护人员 {staff.Name} (电话: {staff.ContactPhone}): " +
                        $"老人 {elderlyInfo?.Name ?? elderlyId.ToString()} 健康异常 - {string.Join("; ", alerts)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通知医护人员失败");
            }
        }

        /// <summary>
        /// 获取警报原因
        /// </summary>
        private List<string> GetAlertReasons(int? heartRate, float? oxygenLevel, float? temperature)
        {
            var reasons = new List<string>();

            if (heartRate.HasValue)
            {
                if (heartRate < 60) reasons.Add("心率过低");
                else if (heartRate > 100) reasons.Add("心率过高");
            }

            if (oxygenLevel.HasValue && oxygenLevel < 95)
                reasons.Add("血氧过低");

            if (temperature.HasValue)
            {
                if (temperature < 36.0f) reasons.Add("体温过低");
                else if (temperature > 37.5f) reasons.Add("体温过高");
            }

            return reasons;
        }
    }
}
