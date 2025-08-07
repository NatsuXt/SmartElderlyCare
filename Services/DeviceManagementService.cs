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
        /// �����豸
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

                // ͨ���豸���ƻ�ȡ�´������豸
                var createdDevice = await GetDeviceByNameAsync(deviceDto.DeviceName);
                if (createdDevice.Success)
                {
                    createdDevice.Message = "�豸�����ɹ�";
                    return createdDevice;
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "�豸����ʧ��"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�����豸ʧ��");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"�����豸ʧ��: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// �����豸
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
                        Message = "û���ṩ��Ҫ���µ��ֶ�"
                    };
                }

                var sql = $"UPDATE DeviceStatus SET {string.Join(", ", setParts)} WHERE device_id = :deviceId";
                var rowsAffected = await _databaseService.ExecuteAsync(sql, parameters);
                
                if (rowsAffected == 0)
                {
                    return new ApiResponse<DeviceDetailDto>
                    {
                        Success = false,
                        Message = "�豸������"
                    };
                }

                var updatedDevice = await GetDeviceByIdAsync(deviceId);
                if (updatedDevice.Success)
                {
                    updatedDevice.Message = "�豸���³ɹ�";
                    return updatedDevice;
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "���³ɹ�������ȡ�豸��Ϣʧ��"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�����豸ʧ��");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"�����豸ʧ��: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// ɾ���豸
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
                        Message = "�豸������"
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "�豸ɾ���ɹ�",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ɾ���豸ʧ��");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"ɾ���豸ʧ��: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// ��ȡ�豸�б�
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
                    Message = "��ȡ�豸�б��ɹ�",
                    Data = deviceList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "��ȡ�豸�б�ʧ��");
                return new ApiResponse<List<DeviceDetailDto>>
                {
                    Success = false,
                    Message = $"��ȡ�豸�б�ʧ��: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// ����ID��ȡ�豸����
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
                        Message = "��ȡ�豸����ɹ�",
                        Data = deviceDetail
                    };
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "�豸������"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "��ȡ�豸����ʧ��");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"��ȡ�豸����ʧ��: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// �����豸���ƻ�ȡ�豸
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
                        Message = "��ȡ�豸�ɹ�",
                        Data = deviceDetail
                    };
                }

                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = "�豸������"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�����豸���ƻ�ȡ�豸ʧ��");
                return new ApiResponse<DeviceDetailDto>
                {
                    Success = false,
                    Message = $"��ȡ�豸ʧ��: {ex.Message}"
                };
            }
        }
    }
}

