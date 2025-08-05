using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 设备监控业务逻辑服务
    /// </summary>
    public class DeviceMonitoringService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<DeviceMonitoringService> _logger;

        public DeviceMonitoringService(DatabaseService databaseService, ILogger<DeviceMonitoringService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 轮询所有设备状态
        /// </summary>
        public async Task<object> PollAllDeviceStatusAsync()
        {
            var sql = @"
                SELECT device_id, device_type, status, location, last_maintenance_date
                FROM DeviceStatus 
                ORDER BY last_maintenance_date DESC";

            var devices = await _databaseService.QueryAsync<DeviceStatus>(sql);
            
            var faultDevices = devices.Where(d => d.Status == "故障").ToList();
            
            if (faultDevices.Any())
            {
                _logger.LogWarning($"发现 {faultDevices.Count} 个故障设备");
                
                // 通知维修人员
                await NotifyMaintenanceStaffAsync(faultDevices);
            }

            return new
            {
                TotalDevices = devices.Count(),
                OnlineDevices = devices.Count(d => d.Status == "正常"),
                FaultDevices = faultDevices.Count,
                Devices = devices,
                FaultDeviceList = faultDevices
            };
        }

        /// <summary>
        /// 处理设备故障上报
        /// </summary>
        public async Task<object> HandleDeviceFaultAsync(DeviceFaultReportDto faultReport)
        {
            // 更新设备状态
            var updateSql = @"
                UPDATE DeviceStatus 
                SET Status = :FaultStatus, 
                    MaintenanceStatus = :FaultDescription, 
                    LastMaintenanceDate = :ReportTime
                WHERE DeviceId = :DeviceId";

            var parameters = new
            {
                FaultStatus = faultReport.FaultStatus,
                FaultDescription = faultReport.FaultDescription,
                ReportTime = faultReport.ReportTime,
                DeviceId = faultReport.DeviceId
            };

            await _databaseService.ExecuteAsync(updateSql, parameters);

            // 获取设备信息
            var deviceInfo = await GetDeviceStatusByIdAsync(faultReport.DeviceId);
            
            // 如果是故障状态，通知维修人员
            if (faultReport.FaultStatus == "故障" && deviceInfo != null)
            {
                await NotifyMaintenanceStaffAsync(new List<DeviceStatus> { deviceInfo });
            }

            _logger.LogInformation($"设备 {faultReport.DeviceId} 状态已更新为: {faultReport.FaultStatus}");

            return new
            {
                DeviceId = faultReport.DeviceId,
                UpdatedStatus = faultReport.FaultStatus,
                UpdatedTime = faultReport.ReportTime,
                NotificationSent = faultReport.FaultStatus == "故障"
            };
        }

        /// <summary>
        /// 获取设备状态详情
        /// </summary>
        public async Task<DeviceStatus?> GetDeviceStatusByIdAsync(int deviceId)
        {
            var sql = @"
                SELECT DeviceId, DeviceType, Status, Location, LastMaintenanceDate, MaintenanceStatus
                FROM DeviceStatus 
                WHERE DeviceId = :DeviceId";

            return await _databaseService.QueryFirstAsync<DeviceStatus>(sql, new { DeviceId = deviceId });
        }

        /// <summary>
        /// 同步所有设备状态
        /// </summary>
        public async Task<object> SyncAllDeviceStatusAsync()
        {
            // 更新所有设备的检查时间
            var updateSql = @"
                UPDATE DeviceStatus 
                SET LastMaintenanceDate = :CurrentTime 
                WHERE Status != '故障'";

            var currentTime = DateTime.Now;
            var updatedCount = await _databaseService.ExecuteAsync(updateSql, new { CurrentTime = currentTime });

            _logger.LogInformation($"同步了 {updatedCount} 个设备的状态");

            return new
            {
                SyncTime = currentTime,
                UpdatedDeviceCount = updatedCount,
                Message = "设备状态同步完成"
            };
        }

        /// <summary>
        /// 获取设备故障历史记录
        /// </summary>
        public async Task<List<object>> GetDeviceFaultHistoryAsync(int? deviceId, int days)
        {
            var sql = @"
                SELECT DeviceId, DeviceType, Status, MaintenanceStatus, LastMaintenanceDate
                FROM DeviceStatus 
                WHERE LastMaintenanceDate >= :StartDate";

            object parameters = new { StartDate = DateTime.Now.AddDays(-days) };

            if (deviceId.HasValue)
            {
                sql += " AND DeviceId = :DeviceId";
                parameters = new { StartDate = DateTime.Now.AddDays(-days), DeviceId = deviceId.Value };
            }

            sql += " ORDER BY LastMaintenanceDate DESC";

            var history = await _databaseService.QueryAsync<dynamic>(sql, parameters);
            
            return history.Select(h => new
            {
                DeviceId = h.DeviceId,
                DeviceType = h.DeviceType,
                Status = h.Status,
                MaintenanceStatus = h.MaintenanceStatus,
                CheckTime = h.LastMaintenanceDate
            }).ToList<object>();
        }

        /// <summary>
        /// 通知维修人员
        /// </summary>
        private async Task NotifyMaintenanceStaffAsync(List<DeviceStatus> faultDevices)
        {
            try
            {
                var sql = @"
                    SELECT StaffId, Name, ContactPhone, Email 
                    FROM StaffInfo 
                    WHERE Position = '维修人员'";

                var maintenanceStaff = await _databaseService.QueryAsync<StaffInfo>(sql);

                foreach (var staff in maintenanceStaff)
                {
                    foreach (var device in faultDevices)
                    {
                        _logger.LogInformation(
                            $"通知维修人员 {staff.Name} (电话: {staff.ContactPhone}): " +
                            $"设备 {device.DeviceId} ({device.DeviceType}) 在 {device.Location} 发生故障 - {device.MaintenanceStatus}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "通知维修人员失败");
            }
        }
    }
}
