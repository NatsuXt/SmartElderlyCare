using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;
using System.Text.Json;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// IoT 监控服务
    /// </summary>
    public class IoTMonitoringService
    {
        private readonly DatabaseService _dbService;
        private readonly ILogger<IoTMonitoringService> _logger;

        public IoTMonitoringService(DatabaseService dbService, ILogger<IoTMonitoringService> logger)
        {
            _dbService = dbService;
            _logger = logger;
        }

        /// <summary>
        /// 轮询所有设备状态
        /// </summary>
        public async Task<object> PollAllDeviceStatusAsync()
        {
            try
            {
                string sql = @"
                    SELECT device_id, device_name, device_type, status, 
                           installation_date, last_maintenance_date, location
                    FROM DeviceStatus 
                    ORDER BY device_id";

                var devices = await _dbService.QueryAsync<DeviceStatus>(sql);
                
                var faultDevices = devices.Where(d => d.Status == "故障").ToList();
                
                // 如果有故障设备，发送通知给维修人员
                if (faultDevices.Any())
                {
                    await NotifyMaintenanceStaffAsync(faultDevices);
                }

                return new
                {
                    TotalDevices = devices.Count(),
                    FaultDevices = faultDevices.Count,
                    OnlineDevices = devices.Count(d => d.Status == "正常"),
                    Devices = devices,
                    LastPolled = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备状态轮询失败");
                throw;
            }
        }

        /// <summary>
        /// 处理设备故障上报
        /// </summary>
        public async Task<object> HandleDeviceFaultAsync(DeviceFaultReportDto faultReport)
        {
            try
            {
                // 更新设备状态
                string updateSql = @"
                    UPDATE DeviceStatus 
                    SET status = @FaultStatus, 
                        last_maintenance_date = @ReportTime
                    WHERE device_id = @DeviceId";

                var updateParams = new
                {
                    FaultStatus = faultReport.FaultStatus,
                    ReportTime = faultReport.ReportTime,
                    DeviceId = faultReport.DeviceId
                };

                int rowsAffected = await _dbService.ExecuteAsync(updateSql, updateParams);

                if (rowsAffected > 0)
                {
                    // 获取设备信息
                    string deviceSql = "SELECT * FROM DeviceStatus WHERE device_id = @DeviceId";
                    var device = await _dbService.QueryFirstAsync<DeviceStatus>(deviceSql, new { DeviceId = faultReport.DeviceId });

                    // 发送故障通知给维修人员
                    await NotifyMaintenanceStaffAsync(new List<DeviceStatus> { device });

                    _logger.LogInformation($"设备 {faultReport.DeviceId} 故障状态已更新: {faultReport.FaultStatus}");

                    return new
                    {
                        DeviceId = faultReport.DeviceId,
                        UpdatedStatus = faultReport.FaultStatus,
                        NotificationSent = true,
                        ProcessedAt = DateTime.Now
                    };
                }
                else
                {
                    throw new InvalidOperationException($"设备 {faultReport.DeviceId} 不存在");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理设备故障上报失败: DeviceId={faultReport.DeviceId}");
                throw;
            }
        }

        /// <summary>
        /// 处理GPS位置上报
        /// </summary>
        public async Task<object> HandleGpsLocationAsync(GpsLocationReportDto gpsReport)
        {
            try
            {
                // 检查是否超出电子围栏
                bool isOutOfFence = await CheckElectronicFenceAsync(gpsReport.Latitude, gpsReport.Longitude);

                if (isOutOfFence)
                {
                    // 在FenceLog表中创建记录
                    string insertLogSql = @"
                        INSERT INTO FenceLog (elderly_id, fence_id, entry_time)
                        VALUES (@ElderlyId, 1, @LocationTime)";

                    var logParams = new
                    {
                        ElderlyId = gpsReport.ElderlyId,
                        LocationTime = gpsReport.LocationTime
                    };

                    await _dbService.ExecuteAsync(insertLogSql, logParams);

                    // 通知最近的护理人员
                    await NotifyNearestNurseAsync(gpsReport.ElderlyId, gpsReport.Latitude, gpsReport.Longitude);

                    _logger.LogWarning($"老人 {gpsReport.ElderlyId} 超出电子围栏范围");
                }

                return new
                {
                    ElderlyId = gpsReport.ElderlyId,
                    Latitude = gpsReport.Latitude,
                    Longitude = gpsReport.Longitude,
                    IsOutOfFence = isOutOfFence,
                    AlertTriggered = isOutOfFence,
                    ProcessedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理GPS位置上报失败: ElderlyId={gpsReport.ElderlyId}");
                throw;
            }
        }

        /// <summary>
        /// 处理健康数据上报
        /// </summary>
        public async Task<object> HandleHealthDataAsync(HealthDataReportDto healthReport)
        {
            try
            {
                // 确定健康状态
                string healthStatus = DetermineHealthStatus(healthReport);

                // 插入健康监测记录
                string insertSql = @"
                    INSERT INTO HealthMonitoring 
                    (elderly_id, monitoring_date, heart_rate, blood_pressure, 
                     oxygen_level, temperature, status)
                    VALUES 
                    (@ElderlyId, @MeasurementTime, @HeartRate, @BloodPressure, 
                     @OxygenLevel, @Temperature, @Status)";

                var healthParams = new
                {
                    ElderlyId = healthReport.ElderlyId,
                    MeasurementTime = healthReport.MeasurementTime,
                    HeartRate = healthReport.HeartRate,
                    BloodPressure = healthReport.BloodPressure,
                    OxygenLevel = healthReport.OxygenLevel,
                    Temperature = healthReport.Temperature,
                    Status = healthStatus
                };

                await _dbService.ExecuteAsync(insertSql, healthParams);

                // 如果健康状态异常，发送警报
                if (healthStatus == "异常")
                {
                    await SendHealthAlertAsync(healthReport.ElderlyId, healthReport);
                }

                _logger.LogInformation($"老人 {healthReport.ElderlyId} 健康数据已记录，状态: {healthStatus}");

                return new
                {
                    ElderlyId = healthReport.ElderlyId,
                    HealthStatus = healthStatus,
                    AlertSent = healthStatus == "异常",
                    RecordedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理健康数据上报失败: ElderlyId={healthReport.ElderlyId}");
                throw;
            }
        }

        /// <summary>
        /// 获取活跃警报列表
        /// </summary>
        public async Task<List<object>> GetActiveAlertsAsync()
        {
            try
            {
                var alerts = new List<object>();

                // 故障设备警报
                string faultDevicesSql = @"
                    SELECT device_id, device_name, device_type, status, location
                    FROM DeviceStatus 
                    WHERE status IN ('故障', '异常', 'ERROR')";
                var faultDevices = await _dbService.QueryAsync<DeviceStatus>(faultDevicesSql);

                foreach (var device in faultDevices)
                {
                    alerts.Add(new
                    {
                        Type = "设备故障",
                        Title = $"设备故障警报",
                        Message = $"{device.DeviceType} {device.DeviceName} 在 {device.Location} 发生故障",
                        DeviceId = device.DeviceId,
                        Severity = "高",
                        Time = DateTime.Now
                    });
                }

                // 健康异常警报
                string healthAlertsSql = @"
                    SELECT elderly_id, heart_rate, blood_pressure, oxygen_level, temperature, monitoring_date, status
                    FROM HealthMonitoring 
                    WHERE status IN ('Abnormal', 'Critical', '异常', '危险')
                    AND monitoring_date >= :RecentTime";
                var healthAlerts = await _dbService.QueryAsync<dynamic>(healthAlertsSql, new { RecentTime = DateTime.Now.AddHours(-2) });

                foreach (var health in healthAlerts)
                {
                    alerts.Add(new
                    {
                        Type = "健康异常",
                        Title = $"健康监测警报",
                        Message = $"老人{health.elderly_id}出现{health.status}状况，请及时关注",
                        ElderlyId = health.elderly_id,
                        Severity = health.status == "Critical" || health.status == "危险" ? "紧急" : "警告",
                        Time = health.monitoring_date
                    });
                }

                // 围栏异常警报（简化版，不依赖ElderlyInfo表）
                string fenceAlertsSql = @"
                    SELECT fl.elderly_id, fl.entry_time, fl.event_type
                    FROM FenceLog fl
                    WHERE fl.exit_time IS NULL 
                    AND fl.entry_time >= :RecentTime";
                var fenceAlerts = await _dbService.QueryAsync<dynamic>(fenceAlertsSql, new { RecentTime = DateTime.Now.AddHours(-1) });

                foreach (var alert in fenceAlerts)
                {
                    alerts.Add(new
                    {
                        Type = "围栏异常",
                        Title = "电子围栏警报",
                        Message = $"老人{alert.elderly_id}超出安全区域",
                        ElderlyId = alert.elderly_id,
                        Severity = "紧急",
                        Time = alert.entry_time
                    });
                }

                return alerts.OrderByDescending(a => ((dynamic)a).Time).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取活跃警报失败");
                return new List<object>();
            }
        }

        // 私有辅助方法

        /// <summary>
        /// 通知维修人员
        /// </summary>
        private async Task NotifyMaintenanceStaffAsync(List<DeviceStatus> faultDevices)
        {
            try
            {
                string staffSql = @"
                    SELECT STAFF_ID, NAME, CONTACT_PHONE, EMAIL 
                    FROM STAFFINFO 
                    WHERE POSITION = '维修人员'";
                
                var maintenanceStaff = await _dbService.QueryAsync<STAFFINFO>(staffSql);

                foreach (var staff in maintenanceStaff)
                {
                    foreach (var device in faultDevices)
                    {
                        // 这里可以集成实际的通知服务（短信、邮件、推送等）
                        _logger.LogInformation($"通知维修人员 {staff.NAME}({staff.CONTACT_PHONE}): 设备 {device.DeviceName} 故障");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通知维修人员失败");
            }
        }

        /// <summary>
        /// 检查是否超出电子围栏
        /// </summary>
        private async Task<bool> CheckElectronicFenceAsync(double latitude, double longitude)
        {
            try
            {
                string fenceSql = "SELECT area_definition FROM ElectronicFence WHERE fence_id = 1";
                var fence = await _dbService.QueryFirstAsync<ElectronicFence>(fenceSql);

                if (fence != null)
                {
                    // 这里简化处理，实际应该解析 area_definition 的坐标范围
                    // 假设 area_definition 格式为 JSON: {"minLat":30.0, "maxLat":31.0, "minLng":120.0, "maxLng":121.0}
                    
                    // 简化演示：如果经纬度超出一个固定范围就认为超出围栏
                    bool outOfRange = latitude < 30.0 || latitude > 31.0 || longitude < 120.0 || longitude > 121.0;
                    return outOfRange;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查电子围栏失败");
                return false;
            }
        }

        /// <summary>
        /// 通知最近的护理人员
        /// </summary>
        private async Task NotifyNearestNurseAsync(int elderlyId, double latitude, double longitude)
        {
            try
            {
                string nurseSql = @"
                    SELECT STAFF_ID, NAME, CONTACT_PHONE, EMAIL 
                    FROM STAFFINFO 
                    WHERE POSITION LIKE '%护理%' 
                    LIMIT 1";
                
                var nurse = await _dbService.QueryFirstAsync<STAFFINFO>(nurseSql);
                
                if (nurse != null)
                {
                    _logger.LogInformation($"通知护理人员 {nurse.NAME}({nurse.CONTACT_PHONE}): 老人 {elderlyId} 超出安全区域");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通知护理人员失败");
            }
        }

        /// <summary>
        /// 判断健康状态
        /// </summary>
        private string DetermineHealthStatus(HealthDataReportDto healthData)
        {
            // 简化的健康状态判断逻辑
            if (healthData.HeartRate.HasValue && (healthData.HeartRate < 60 || healthData.HeartRate > 100))
                return "异常";
            
            if (healthData.Temperature.HasValue && (healthData.Temperature < 36.0 || healthData.Temperature > 37.5))
                return "异常";
            
            if (healthData.OxygenLevel.HasValue && healthData.OxygenLevel < 95.0)
                return "异常";

            return "正常";
        }

        /// <summary>
        /// 发送健康警报
        /// </summary>
        private async Task SendHealthAlertAsync(int elderlyId, HealthDataReportDto healthData)
        {
            try
            {
                string nurseSql = @"
                    SELECT STAFF_ID, NAME, CONTACT_PHONE, EMAIL 
                    FROM STAFFINFO 
                    WHERE POSITION LIKE '%护理%' OR POSITION LIKE '%医生%'";
                
                var medicalStaff = await _dbService.QueryAsync<STAFFINFO>(nurseSql);

                foreach (var staff in medicalStaff)
                {
                    _logger.LogInformation($"发送健康警报给 {staff.NAME}({staff.CONTACT_PHONE}): 老人 {elderlyId} 健康指标异常");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送健康警报失败");
            }
        }

        /// <summary>
        /// 根据设备ID获取设备状态
        /// </summary>
        public async Task<DeviceStatus?> GetDeviceStatusByIdAsync(int deviceId)
        {
            try
            {
                var sql = @"
                    SELECT device_id, device_name, device_type, status, 
                           installation_date, last_maintenance_date, location
                    FROM DeviceStatus 
                    WHERE device_id = :DeviceId";

                return await _dbService.QueryFirstOrDefaultAsync<DeviceStatus>(sql, new { DeviceId = deviceId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取设备状态失败: {deviceId}");
                return null;
            }
        }

        /// <summary>
        /// 获取老人健康历史数据
        /// </summary>
        public async Task<List<HealthMonitoring>> GetElderlyHealthHistoryAsync(int elderlyId, int days = 7)
        {
            try
            {
                var sql = @"
                    SELECT monitoring_id, elderly_id, heart_rate, blood_pressure, 
                           oxygen_level, temperature, measurement_time, alerts
                    FROM HealthMonitoring 
                    WHERE elderly_id = :ElderlyId 
                    AND measurement_time >= :StartDate
                    ORDER BY measurement_time DESC";

                var startDate = DateTime.Now.AddDays(-days);
                return (await _dbService.QueryAsync<HealthMonitoring>(sql, new { ElderlyId = elderlyId, StartDate = startDate })).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取老人健康历史失败: {elderlyId}");
                return new List<HealthMonitoring>();
            }
        }

        /// <summary>
        /// 获取围栏日志
        /// </summary>
        public async Task<List<FenceLog>> GetFenceLogsAsync(int elderlyId, int hours = 24)
        {
            try
            {
                var sql = @"
                    SELECT log_id, elderly_id, fence_id, event_type, 
                           event_time, latitude, longitude, description
                    FROM FenceLog 
                    WHERE elderly_id = :ElderlyId 
                    AND event_time >= :StartTime
                    ORDER BY event_time DESC";

                var startTime = DateTime.Now.AddHours(-hours);
                return (await _dbService.QueryAsync<FenceLog>(sql, new { ElderlyId = elderlyId, StartTime = startTime })).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取围栏日志失败: {elderlyId}");
                return new List<FenceLog>();
            }
        }

        /// <summary>
        /// 获取老人位置状态
        /// </summary>
        public async Task<object> GetElderlyLocationStatusAsync()
        {
            try
            {
                var sql = @"
                    SELECT e.ELDERLY_ID, e.NAME, sl.LATITUDE, sl.LONGITUDE, 
                           sl.LOCATION_TIME, r.ROOM_NUMBER
                    FROM ElderlyInfo e
                    LEFT JOIN STAFFLOCATION sl ON e.ROOM_ID = sl.ROOM_ID
                    LEFT JOIN RoomManagement r ON e.ROOM_ID = r.ROOM_ID
                    ORDER BY e.ELDERLY_ID";

                var locations = await _dbService.QueryAsync<dynamic>(sql);
                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取老人位置状态失败");
                return new { };
            }
        }

        /// <summary>
        /// 同步所有设备状态
        /// </summary>
        public async Task<object> SyncAllDeviceStatusAsync()
        {
            try
            {
                var sql = @"
                    UPDATE DeviceStatus 
                    SET last_maintenance_date = :CurrentTime
                    WHERE status = '正常'";

                var affectedRows = await _dbService.ExecuteAsync(sql, new { CurrentTime = DateTime.Now });

                return new
                {
                    Message = "设备状态同步完成",
                    UpdatedDevices = affectedRows,
                    SyncTime = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "同步设备状态失败");
                return new { Message = "同步失败", Error = ex.Message };
            }
        }
    }
}
