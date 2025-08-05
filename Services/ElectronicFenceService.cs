using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 电子围栏监控业务逻辑服务
    /// </summary>
    public class ElectronicFenceService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<ElectronicFenceService> _logger;

        public ElectronicFenceService(DatabaseService databaseService, ILogger<ElectronicFenceService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 处理GPS位置上报
        /// </summary>
        public async Task<object> HandleGpsLocationAsync(GpsLocationReportDto gpsReport)
        {
            // 检查是否超出围栏范围
            var isOutOfFence = await CheckIfOutOfFenceAsync(gpsReport.ElderlyId, gpsReport.Latitude, gpsReport.Longitude);

            if (isOutOfFence)
            {
                // 创建围栏日志记录
                await CreateFenceLogAsync(gpsReport.ElderlyId, gpsReport.Latitude, gpsReport.Longitude, "越界");
                
                // 通知护理人员
                await NotifyNursingStaffAsync(gpsReport.ElderlyId, gpsReport.Latitude, gpsReport.Longitude);
                
                _logger.LogWarning($"老人 {gpsReport.ElderlyId} 越界警报: 位置 ({gpsReport.Latitude}, {gpsReport.Longitude})");
            }

            return new
            {
                ElderlyId = gpsReport.ElderlyId,
                Latitude = gpsReport.Latitude,
                Longitude = gpsReport.Longitude,
                LocationTime = gpsReport.LocationTime,
                IsOutOfFence = isOutOfFence,
                AlertTriggered = isOutOfFence
            };
        }

        /// <summary>
        /// 获取围栏进出记录
        /// </summary>
        public async Task<List<FenceLog>> GetFenceLogsAsync(int? elderlyId, int hours)
        {
            var sql = @"
                SELECT event_log_id, elderly_id, latitude, longitude, entry_time, event_type
                FROM FenceLog 
                WHERE entry_time >= :StartTime";

            object parameters = new { StartTime = DateTime.Now.AddHours(-hours) };

            if (elderlyId.HasValue)
            {
                sql += " AND elderly_id = :ElderlyId";
                parameters = new { StartTime = DateTime.Now.AddHours(-hours), ElderlyId = elderlyId.Value };
            }

            sql += " ORDER BY entry_time DESC";

            var result = await _databaseService.QueryAsync<FenceLog>(sql, parameters);
            return result.ToList();
        }

        /// <summary>
        /// 获取所有老人当前位置状态
        /// </summary>
        public async Task<List<object>> GetElderlyLocationStatusAsync()
        {
            var sql = @"
                SELECT DISTINCT 
                    e.ElderlyId,
                    e.Name,
                    f.Latitude,
                    f.Longitude,
                    f.LogTime,
                    f.EventType
                FROM ElderlyInfo e
                LEFT JOIN (
                    SELECT ElderlyId, Latitude, Longitude, LogTime, EventType,
                           ROW_NUMBER() OVER (PARTITION BY ElderlyId ORDER BY LogTime DESC) as rn
                    FROM FenceLog
                ) f ON e.ElderlyId = f.ElderlyId AND f.rn = 1
                ORDER BY e.ElderlyId";

            var results = await _databaseService.QueryAsync<dynamic>(sql);
            
            return results.Select(r => new
            {
                ElderlyId = r.ElderlyId,
                Name = r.Name,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                LastLocationTime = r.LogTime,
                Status = r.EventType ?? "未知"
            }).ToList<object>();
        }

        /// <summary>
        /// 获取围栏配置信息
        /// </summary>
        public async Task<List<ElectronicFence>> GetFenceConfigAsync()
        {
            var sql = @"
                SELECT FenceId, AreaDefinition
                FROM ElectronicFence 
                ORDER BY FenceId";

            var result = await _databaseService.QueryAsync<ElectronicFence>(sql);
            return result.ToList();
        }

        /// <summary>
        /// 获取老人位置轨迹
        /// </summary>
        public async Task<List<object>> GetElderlyTrajectoryAsync(int elderlyId, int hours)
        {
            var sql = @"
                SELECT Latitude, Longitude, LogTime, EventType
                FROM FenceLog 
                WHERE ElderlyId = :ElderlyId 
                  AND LogTime >= :StartTime
                ORDER BY LogTime ASC";

            var parameters = new 
            { 
                ElderlyId = elderlyId, 
                StartTime = DateTime.Now.AddHours(-hours) 
            };

            var trajectory = await _databaseService.QueryAsync<dynamic>(sql, parameters);
            
            return trajectory.Select(t => new
            {
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                Time = t.LogTime,
                EventType = t.EventType
            }).ToList<object>();
        }

        /// <summary>
        /// 获取围栏警报
        /// </summary>
        public async Task<List<object>> GetFenceAlertsAsync(bool activeOnly)
        {
            var sql = @"
                SELECT 
                    f.LogId,
                    f.ElderlyId,
                    e.Name as ElderlyName,
                    f.Latitude,
                    f.Longitude,
                    f.LogTime,
                    f.EventType
                FROM FenceLog f
                INNER JOIN ElderlyInfo e ON f.ElderlyId = e.ElderlyId
                WHERE f.EventType = '越界'";

            if (activeOnly)
            {
                sql += " AND f.LogTime >= :RecentTime";
                var parameters = new { RecentTime = DateTime.Now.AddHours(-24) };
                var alerts = await _databaseService.QueryAsync<dynamic>(sql, parameters);
                
                return alerts.Select(a => new
                {
                    LogId = a.LogId,
                    ElderlyId = a.ElderlyId,
                    ElderlyName = a.ElderlyName,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    AlertTime = a.LogTime,
                    AlertType = a.EventType,
                    IsRecent = true
                }).ToList<object>();
            }
            else
            {
                sql += " ORDER BY f.LogTime DESC";
                var alerts = await _databaseService.QueryAsync<dynamic>(sql);
                
                return alerts.Select(a => new
                {
                    LogId = a.LogId,
                    ElderlyId = a.ElderlyId,
                    ElderlyName = a.ElderlyName,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    AlertTime = a.LogTime,
                    AlertType = a.EventType,
                    IsRecent = a.LogTime >= DateTime.Now.AddHours(-24)
                }).ToList<object>();
            }
        }

        /// <summary>
        /// 检查是否越界
        /// </summary>
        private Task<bool> CheckIfOutOfFenceAsync(int elderlyId, double latitude, double longitude)
        {
            // 简化的围栏检查逻辑
            // 这里应该根据实际的AreaDefinition字段格式来实现围栏检查
            // 暂时返回一个简单的范围检查作为示例
            
            // 假设一个简单的矩形围栏范围（实际应根据AreaDefinition解析）
            var minLat = 39.9; // 北京市区范围示例
            var maxLat = 40.1;
            var minLng = 116.3;
            var maxLng = 116.5;
            
            var isOutOfFence = latitude < minLat || latitude > maxLat || 
                              longitude < minLng || longitude > maxLng;
            
            return Task.FromResult(isOutOfFence);
        }

        /// <summary>
        /// 创建围栏日志
        /// </summary>
        private async Task CreateFenceLogAsync(int elderlyId, double latitude, double longitude, string eventType)
        {
            var sql = @"
                INSERT INTO FenceLog (elderly_id, latitude, longitude, entry_time, event_type)
                VALUES (:ElderlyId, :Latitude, :Longitude, :LogTime, :EventType)";

            var parameters = new
            {
                ElderlyId = elderlyId,
                Latitude = latitude,
                Longitude = longitude,
                LogTime = DateTime.Now,
                EventType = eventType
            };

            await _databaseService.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// 通知护理人员
        /// </summary>
        private async Task NotifyNursingStaffAsync(int elderlyId, double latitude, double longitude)
        {
            try
            {
                // 获取老人信息
                var elderlyInfo = await _databaseService.QueryFirstAsync<ElderlyInfo>(
                    "SELECT Name FROM ElderlyInfo WHERE ElderlyId = :ElderlyId", 
                    new { ElderlyId = elderlyId });

                // 获取护理人员信息
                var sql = @"
                    SELECT StaffId, Name, ContactPhone, Email 
                    FROM StaffInfo 
                    WHERE Position LIKE '%护理%' OR Position LIKE '%护士%'";

                var nursingStaff = await _databaseService.QueryAsync<StaffInfo>(sql);

                foreach (var staff in nursingStaff)
                {
                    _logger.LogInformation(
                        $"通知护理人员 {staff.Name} (电话: {staff.ContactPhone}): " +
                        $"老人 {elderlyInfo?.Name ?? elderlyId.ToString()} 越界警报，当前位置: ({latitude}, {longitude})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通知护理人员失败");
            }
        }

        /// <summary>
        /// 计算两点间距离（单位：米）
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // 地球半径（米）
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
