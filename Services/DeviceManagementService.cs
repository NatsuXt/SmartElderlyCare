using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;
using System.Linq;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 设备管理服务 - 使用中文兼容数据库服务
    /// </summary>
    public class DeviceManagementService
    {
        private readonly ChineseCompatibleDatabaseService _chineseDbService;
        private readonly ILogger<DeviceManagementService> _logger;

        public DeviceManagementService(ChineseCompatibleDatabaseService chineseDbService, ILogger<DeviceManagementService> logger)
        {
            _chineseDbService = chineseDbService;
            _logger = logger;
        }

        /// <summary>
        /// 创建设备 - 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<DeviceDetailDto>> CreateDeviceAsync(DeviceCreateDto deviceDto)
        {
            try
            {
                _logger.LogInformation($"📱 创建新设备: {deviceDto.DeviceName} - {deviceDto.DeviceType}");
                _logger.LogInformation($"🔍 调试 - 接收到的DTO数据:");
                _logger.LogInformation($"   - DeviceName: '{deviceDto.DeviceName}' (长度: {deviceDto.DeviceName?.Length})");
                _logger.LogInformation($"   - DeviceType: '{deviceDto.DeviceType}' (长度: {deviceDto.DeviceType?.Length})");
                _logger.LogInformation($"   - Location: '{deviceDto.Location}' (长度: {deviceDto.Location?.Length})");
                _logger.LogInformation($"   - Status: '{deviceDto.Status}' (长度: {deviceDto.Status?.Length})");

                // 使用中文兼容数据库服务创建设备，确保非空值
                await _chineseDbService.CreateDeviceAsync(
                    deviceDto.DeviceName ?? "未命名设备", 
                    deviceDto.DeviceType ?? "未知类型", 
                    deviceDto.Location ?? "", 
                    deviceDto.Status ?? "正常",
                    deviceDto.LastMaintenanceDate ?? DateTime.Now,
                    DateTime.Now
                );

                // 通过设备名称获取新创建的设备
                var createdDevice = await GetDeviceByNameAsync(deviceDto.DeviceName ?? "未命名设备");
                if (createdDevice.Success)
                {
                    createdDevice.Message = "设备创建成功";
                    _logger.LogInformation($"✅ 设备创建成功: {deviceDto.DeviceName} - {deviceDto.DeviceType}");
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
        /// 更新设备 - 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<DeviceDetailDto>> UpdateDeviceAsync(int deviceId, DeviceUpdateDto deviceDto)
        {
            try
            {
                var updateFields = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(deviceDto.DeviceName))
                {
                    updateFields["device_name"] = deviceDto.DeviceName;
                }
                if (!string.IsNullOrEmpty(deviceDto.DeviceType))
                {
                    updateFields["device_type"] = deviceDto.DeviceType;
                }
                if (!string.IsNullOrEmpty(deviceDto.Location))
                {
                    updateFields["location"] = deviceDto.Location;
                }
                if (!string.IsNullOrEmpty(deviceDto.Status))
                {
                    updateFields["status"] = deviceDto.Status;
                }
                if (deviceDto.LastMaintenanceDate.HasValue)
                {
                    updateFields["last_maintenance_date"] = deviceDto.LastMaintenanceDate.Value;
                }

                if (!updateFields.Any())
                {
                    return new ApiResponse<DeviceDetailDto>
                    {
                        Success = false,
                        Message = "没有提供需要更新的字段"
                    };
                }

                // 使用中文兼容数据库服务更新设备
                var rowsAffected = await _chineseDbService.UpdateDeviceAsync(deviceId, updateFields);
                
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
        /// 删除设备 - 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteDeviceAsync(int deviceId)
        {
            try
            {
                // 使用中文兼容数据库服务删除设备
                var rowsAffected = await _chineseDbService.DeleteDeviceAsync(deviceId);
                
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
        /// 获取设备列表 - 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<List<DeviceDetailDto>>> GetDevicesAsync(PagedRequest request)
        {
            try
            {
                _logger.LogInformation($"🔍 获取设备列表: 页码={request.Page}, 大小={request.PageSize}, 搜索='{request.Search}'");
                
                // 使用中文兼容数据库服务，确保搜索参数非空
                var devices = await _chineseDbService.GetDevicesAsync(request.Search ?? "");
                
                // 手动分页
                var totalCount = devices.Count;
                var offset = (request.Page - 1) * request.PageSize;
                var pagedDevices = devices
                    .Skip(offset)
                    .Take(request.PageSize)
                    .ToList();
                
                var deviceList = pagedDevices.Select(d => new DeviceDetailDto
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

                _logger.LogInformation($"✅ 成功获取 {deviceList.Count} 个设备，总计 {totalCount} 个");

                return new ApiResponse<List<DeviceDetailDto>>
                {
                    Success = true,
                    Message = "获取设备列表成功",
                    Data = deviceList,
                    TotalCount = totalCount
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
        /// 根据ID获取设备详情 - 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<DeviceDetailDto>> GetDeviceByIdAsync(int deviceId)
        {
            try
            {
                // 使用中文兼容数据库服务获取设备
                var device = await _chineseDbService.GetDeviceByIdAsync(deviceId);
                
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
        /// 根据设备名称获取设备 - 使用中文兼容服务
        /// </summary>
        private async Task<ApiResponse<DeviceDetailDto>> GetDeviceByNameAsync(string deviceName)
        {
            try
            {
                // 使用中文兼容数据库服务获取设备
                var device = await _chineseDbService.GetDeviceByNameAsync(deviceName);
                
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
        /// 获取设备统计信息 - 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<object>> GetDeviceStatisticsAsync()
        {
            try
            {
                // 使用中文兼容数据库服务获取统计信息
                var result = await _chineseDbService.GetDeviceStatisticsAsync();

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

        /// <summary>
        /// 轮询所有设备状态 - 从IoT模块迁移
        /// </summary>
        public async Task<ApiResponse<object>> PollAllDeviceStatusAsync()
        {
            try
            {
                _logger.LogInformation("🔄 开始轮询所有设备状态");

                var devices = await _chineseDbService.GetDevicesAsync();
                
                var faultDevices = devices.Where(d => d.Status == "故障" || d.Status == "异常" || d.Status == "ERROR").ToList();
                
                // 如果有故障设备，记录日志（通知功能由其他同学的警报模块处理）
                if (faultDevices.Any())
                {
                    _logger.LogWarning($"⚠️ 发现 {faultDevices.Count} 个故障设备");
                    foreach (var device in faultDevices)
                    {
                        _logger.LogWarning($"故障设备: {device.DeviceName} ({device.DeviceType}) - 状态: {device.Status}");
                    }
                }

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "设备状态轮询完成",
                    Data = new
                    {
                        TotalDevices = devices.Count,
                        FaultDevices = faultDevices.Count,
                        OnlineDevices = devices.Count(d => d.Status == "正常" || d.Status == "在线"),
                        Devices = devices,
                        LastPolled = DateTime.Now
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备状态轮询失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"设备状态轮询失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 处理设备故障上报 - 从IoT模块迁移
        /// </summary>
        public async Task<ApiResponse<object>> HandleDeviceFaultAsync(DeviceFaultReportDto faultReport)
        {
            try
            {
                _logger.LogWarning($"⚠️ 接收到设备故障上报: 设备ID={faultReport.DeviceId}, 故障类型={faultReport.FaultStatus}");

                // 检查设备是否存在
                var device = await _chineseDbService.GetDeviceByIdAsync(faultReport.DeviceId);
                if (device == null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"设备 {faultReport.DeviceId} 不存在"
                    };
                }

                // 更新设备状态为故障
                await _chineseDbService.UpdateDeviceStatusAsync(faultReport.DeviceId, "故障");

                _logger.LogInformation($"✅ 设备 {faultReport.DeviceId} 状态已更新为故障");

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "设备故障上报处理成功",
                    Data = new
                    {
                        DeviceId = faultReport.DeviceId,
                        DeviceName = device.DeviceName,
                        FaultType = faultReport.FaultStatus,
                        ReportTime = faultReport.ReportTime,
                        ProcessedAt = DateTime.Now
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理设备故障上报失败: DeviceId={faultReport.DeviceId}");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"处理设备故障上报失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 手动触发设备状态同步 - 从IoT模块迁移
        /// </summary>
        public async Task<ApiResponse<object>> SyncAllDeviceStatusAsync()
        {
            try
            {
                _logger.LogInformation("🔄 开始手动同步所有设备状态");

                // 重用轮询功能进行同步
                var pollResult = await PollAllDeviceStatusAsync();
                
                if (pollResult.Success)
                {
                    _logger.LogInformation("✅ 设备状态同步完成");
                    return new ApiResponse<object>
                    {
                        Success = true,
                        Message = "设备状态同步完成",
                        Data = pollResult.Data
                    };
                }

                return pollResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备状态同步失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"设备状态同步失败: {ex.Message}"
                };
            }
        }
    }
}

