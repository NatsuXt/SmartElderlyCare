using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 设备管理服务
    /// </summary>
    public class DeviceManagementService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<DeviceManagementService> _logger;

        public DeviceManagementService(DatabaseService databaseService, ILogger<DeviceManagementService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 创建设备
        /// </summary>
        public async Task<ApiResponse<DeviceDetailDto>> CreateDeviceAsync(DeviceCreateDto deviceDto)
        {
            try
            {
                var sql = @"
                    INSERT INTO DeviceStatus (
                        device_name, device_type, location, status, 
                        last_maintenance_date, installation_date
                    ) VALUES (
                        :DeviceName, :DeviceType, :Location, :Status,
                        :LastMaintenanceDate, :InstallationDate
                    )";
                
                var parameters = new
                {
                    deviceDto.DeviceName,
                    deviceDto.DeviceType,
                    deviceDto.Location,
                    deviceDto.Status,
                    LastMaintenanceDate = deviceDto.LastMaintenanceDate ?? DateTime.Now,
                    InstallationDate = DateTime.Now
                };
                
                await _databaseService.ExecuteAsync(sql, parameters);

                // 通过设备名称获取新创建的设备
                var createdDevice = await GetDeviceByNameAsync(deviceDto.DeviceName);
                if (createdDevice.Success)
                {
                    createdDevice.Message = "设备创建成功";
                    return createdDevice;
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "设备创建失败"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建设备失败");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"创建设备失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        public async Task<ApiResponse<DeviceDetailDto>> UpdateDeviceAsync(int deviceId, DeviceUpdateDto deviceDto)
        {
            try
            {
                var setParts = new List<string>();
                var parameters = new Dictionary<string, object> { { "deviceId", deviceId } };

                if (!string.IsNullOrEmpty(deviceDto.DeviceName))
                {
                    setParts.Add("device_name = :deviceName");
                    parameters["deviceName"] = deviceDto.DeviceName;
                }
                if (!string.IsNullOrEmpty(deviceDto.DeviceType))
                {
                    setParts.Add("device_type = :deviceType");
                    parameters["deviceType"] = deviceDto.DeviceType;
                }
                if (!string.IsNullOrEmpty(deviceDto.Location))
                {
                    setParts.Add("location = :location");
                    parameters["location"] = deviceDto.Location;
                }
                // RoomId field is not available in current database schema
                if (!string.IsNullOrEmpty(deviceDto.Status))
                {
                    setParts.Add("status = :status");
                    parameters["status"] = deviceDto.Status;
                }
                if (deviceDto.LastMaintenanceDate.HasValue)
                {
                    setParts.Add("last_maintenance_date = :lastMaintenanceDate");
                    parameters["lastMaintenanceDate"] = deviceDto.LastMaintenanceDate.Value;
                }

                if (!setParts.Any())
                {
                    return new ApiResponse<DeviceDetailDto>
                    {
                        Success = false,
                        Message = "没有提供需要更新的字段"
                    };
                }

                var sql = $"UPDATE DeviceStatus SET {string.Join(", ", setParts)} WHERE device_id = :deviceId";
                var rowsAffected = await _databaseService.ExecuteAsync(sql, parameters);
                
                if (rowsAffected == 0)
                {
                    return new ApiResponse<DeviceDetailDto>
                    {
                        Success = false,
                        Message = "设备不存在"
                    };
                }

                var updatedDevice = await GetDeviceByIdAsync(deviceId);
                if (updatedDevice.Success)
                {
                    updatedDevice.Message = "设备更新成功";
                    return updatedDevice;
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "更新成功但获取设备信息失败"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新设备失败");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"更新设备失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteDeviceAsync(int deviceId)
        {
            try
            {
                var sql = "DELETE FROM DeviceStatus WHERE device_id = :deviceId";
                var rowsAffected = await _databaseService.ExecuteAsync(sql, new { deviceId });
                
                if (rowsAffected == 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "设备不存在"
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "设备删除成功",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除设备失败");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"删除设备失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        public async Task<ApiResponse<List<DeviceDetailDto>>> GetDevicesAsync(PagedRequest request)
        {
            try
            {
                var offset = (request.Page - 1) * request.PageSize;
                var whereClause = string.IsNullOrEmpty(request.Search) ? "" : 
                    "WHERE UPPER(device_name) LIKE '%' || UPPER(:search) || '%' OR UPPER(device_type) LIKE '%' || UPPER(:search) || '%'";
                
                var orderClause = request.SortBy switch
                {
                    "deviceName" => $"ORDER BY device_name {(request.SortDesc ? "DESC" : "ASC")}",
                    "deviceType" => $"ORDER BY device_type {(request.SortDesc ? "DESC" : "ASC")}",
                    "status" => $"ORDER BY status {(request.SortDesc ? "DESC" : "ASC")}",
                    _ => "ORDER BY device_id ASC"
                };

                var sql = $@"
                    SELECT * FROM (
                        SELECT device_id, device_name, device_type, location, status,
                               last_maintenance_date, installation_date,
                               ROW_NUMBER() OVER ({orderClause}) as rn
                        FROM DeviceStatus 
                        {whereClause}
                    ) WHERE rn > :offset AND rn <= :endRow";

                object parameters;
                if (string.IsNullOrEmpty(request.Search))
                {
                    parameters = new { offset = offset, endRow = offset + request.PageSize };
                }
                else
                {
                    parameters = new { search = request.Search, offset = offset, endRow = offset + request.PageSize };
                }
                
                var devices = await _databaseService.QueryAsync<DeviceStatus>(sql, parameters);
                
                var deviceList = devices.Select(d => new DeviceDetailDto
                {
                    DeviceId = d.DeviceId,
                    DeviceName = d.DeviceName,
                    DeviceType = d.DeviceType,
                    Location = d.Location,
                    RoomId = null, // Field not available in current database schema
                    Status = d.Status,
                    LastMaintenanceDate = d.LastMaintenanceDate,
                    InstallationDate = d.InstallationDate
                }).ToList();

                return new ApiResponse<List<DeviceDetailDto>>
                {
                    Success = true,
                    Message = "获取设备列表成功",
                    Data = deviceList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备列表失败");
                return new ApiResponse<List<DeviceDetailDto>>
                {
                    Success = false,
                    Message = $"获取设备列表失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据ID获取设备详情
        /// </summary>
        public async Task<ApiResponse<DeviceDetailDto>> GetDeviceByIdAsync(int deviceId)
        {
            try
            {
                var sql = @"
                    SELECT device_id, device_name, device_type, location, status,
                           last_maintenance_date, installation_date
                    FROM DeviceStatus
                    WHERE device_id = :deviceId";

                var results = await _databaseService.QueryAsync<DeviceStatus>(sql, new { deviceId });
                var device = results.FirstOrDefault();
                
                if (device != null)
                {
                    var deviceDetail = new DeviceDetailDto
                    {
                        DeviceId = device.DeviceId,
                        DeviceName = device.DeviceName,
                        DeviceType = device.DeviceType,
                        Location = device.Location,
                        RoomId = null, // Field not available in current database schema
                        Status = device.Status,
                        LastMaintenanceDate = device.LastMaintenanceDate,
                        InstallationDate = device.InstallationDate
                    };

                    return new ApiResponse<DeviceDetailDto>
                    {
                        Success = true,
                        Message = "获取设备详情成功",
                        Data = deviceDetail
                    };
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "设备不存在"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备详情失败");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"获取设备详情失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据设备名称获取设备
        /// </summary>
        private async Task<ApiResponse<DeviceDetailDto>> GetDeviceByNameAsync(string deviceName)
        {
            try
            {
                var sql = @"
                    SELECT device_id, device_name, device_type, location, status,
                           last_maintenance_date, installation_date
                    FROM DeviceStatus
                    WHERE device_name = :deviceName";

                var results = await _databaseService.QueryAsync<DeviceStatus>(sql, new { deviceName });
                var device = results.FirstOrDefault();
                
                if (device != null)
                {
                    var deviceDetail = new DeviceDetailDto
                    {
                        DeviceId = device.DeviceId,
                        DeviceName = device.DeviceName,
                        DeviceType = device.DeviceType,
                        Location = device.Location,
                        RoomId = null, // Field not available in current database schema
                        Status = device.Status,
                        LastMaintenanceDate = device.LastMaintenanceDate,
                        InstallationDate = device.InstallationDate
                    };

                    return new ApiResponse<DeviceDetailDto>
                    {
                        Success = true,
                        Message = "获取设备成功",
                        Data = deviceDetail
                    };
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "设备不存在"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据设备名称获取设备失败");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"获取设备失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取设备统计信息
        /// </summary>
        public async Task<ApiResponse<object>> GetDeviceStatisticsAsync()
        {
            try
            {
                var statisticsSql = @"
                    SELECT 
                        COUNT(*) as total_devices,
                        SUM(CASE WHEN UPPER(status) = '正常' OR UPPER(status) = 'NORMAL' THEN 1 ELSE 0 END) as normal_devices,
                        SUM(CASE WHEN UPPER(status) = '故障' OR UPPER(status) = 'ERROR' OR UPPER(status) = 'FAULT' THEN 1 ELSE 0 END) as fault_devices,
                        SUM(CASE WHEN UPPER(status) = '维护中' OR UPPER(status) = 'MAINTENANCE' THEN 1 ELSE 0 END) as maintenance_devices
                    FROM DeviceStatus";

                var typeDistributionSql = @"
                    SELECT device_type, COUNT(*) as count
                    FROM DeviceStatus
                    GROUP BY device_type
                    ORDER BY count DESC";

                var statistics = await _databaseService.QueryFirstOrDefaultAsync<dynamic>(statisticsSql);
                var typeDistribution = await _databaseService.QueryAsync<dynamic>(typeDistributionSql);

                var result = new
                {
                    总设备数 = statistics?.total_devices ?? 0,
                    正常设备 = statistics?.normal_devices ?? 0,
                    故障设备 = statistics?.fault_devices ?? 0,
                    维护中设备 = statistics?.maintenance_devices ?? 0,
                    设备类型分布 = typeDistribution?.Select(t => new { 
                        设备类型 = t.device_type, 
                        数量 = t.count 
                    }).ToList()
                };

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "获取设备统计信息成功",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备统计信息失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取设备统计信息失败: {ex.Message}"
                };
            }
        }
    }
}

