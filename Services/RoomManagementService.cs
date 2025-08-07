using Oracle.ManagedDataAccess.Client;
using RoomDeviceManagement.DTOs;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Services
{
    public class RoomManagementService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<RoomManagementService> _logger;

        public RoomManagementService(DatabaseService databaseService, ILogger<RoomManagementService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有房间（支持分页和搜索）
        /// </summary>
        public async Task<ApiResponse<List<RoomDetailDto>>> GetRoomsAsync(PagedRequest request)
        {
            try
            {
                var offset = (request.Page - 1) * request.PageSize;
                var whereClause = string.IsNullOrEmpty(request.Search) ? "" : 
                    "WHERE UPPER(room_number) LIKE '%' || UPPER(:search) || '%' OR UPPER(room_type) LIKE '%' || UPPER(:search) || '%'";
                
                var orderClause = request.SortBy switch
                {
                    "roomNumber" => $"ORDER BY room_number {(request.SortDesc ? "DESC" : "ASC")}",
                    "roomType" => $"ORDER BY room_type {(request.SortDesc ? "DESC" : "ASC")}",
                    "capacity" => $"ORDER BY capacity {(request.SortDesc ? "DESC" : "ASC")}",
                    _ => "ORDER BY room_id ASC"
                };

                var sql = $@"
                    SELECT * FROM (
                        SELECT room_id, room_number, room_type, capacity, status,
                               ROW_NUMBER() OVER ({orderClause}) as rn
                        FROM RoomManagement 
                        {whereClause}
                    ) WHERE rn > :offset AND rn <= :endRow";

                var countSql = $@"SELECT COUNT(*) FROM RoomManagement {whereClause}";

                object parameters;
                object countParameters;
                if (string.IsNullOrEmpty(request.Search))
                {
                    parameters = new { offset = offset, endRow = offset + request.PageSize };
                    countParameters = new { };
                }
                else
                {
                    parameters = new { search = request.Search, offset = offset, endRow = offset + request.PageSize };
                    countParameters = new { search = request.Search };
                }
                
                // 获取总数
                var totalCountResult = await _databaseService.QueryAsync<dynamic>(countSql, countParameters);
                var totalCount = Convert.ToInt32(totalCountResult.First().GetType().GetProperty("COUNT(*)")?.GetValue(totalCountResult.First()));

                // 获取数据
                var rooms = await _databaseService.QueryAsync<RoomManagement>(sql, parameters);
                
                var roomList = rooms.Select(r => new RoomDetailDto
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    Capacity = r.Capacity,
                    Status = r.Status,
                    Description = null, // Field not available in current database schema
                    CreatedAt = DateTime.Now,   // Field not available in current database schema
                    UpdatedAt = DateTime.Now    // Field not available in current database schema
                }).ToList();

                return new ApiResponse<List<RoomDetailDto>>
                {
                    Success = true,
                    Message = "获取房间列表成功",
                    Data = roomList,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间列表失败");
                return new ApiResponse<List<RoomDetailDto>>
                {
                    Success = false,
                    Message = $"获取房间列表失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据ID获取房间详情
        /// </summary>
        public async Task<ApiResponse<RoomDetailDto>> GetRoomByIdAsync(int roomId)
        {
            try
            {
                var sql = @"
                    SELECT room_id, room_number, room_type, capacity, status, description, 
                           created_at, updated_at
                    FROM RoomManagement 
                    WHERE room_id = :roomId";

                var results = await _databaseService.QueryAsync<dynamic>(sql, new { roomId });
                var room = results.FirstOrDefault();
                
                if (room != null)
                {
                    var roomDetail = new RoomDetailDto
                    {
                        RoomId = Convert.ToInt32(room.ROOM_ID),
                        RoomNumber = room.ROOM_NUMBER?.ToString() ?? "",
                        RoomType = room.ROOM_TYPE?.ToString() ?? "",
                        Capacity = Convert.ToInt32(room.CAPACITY),
                        Status = room.STATUS?.ToString() ?? "",
                        Description = room.DESCRIPTION?.ToString(),
                        CreatedAt = Convert.ToDateTime(room.CREATED_AT),
                        UpdatedAt = Convert.ToDateTime(room.UPDATED_AT)
                    };

                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = true,
                        Message = "获取房间详情成功",
                        Data = roomDetail
                    };
                }

                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "房间不存在"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间详情失败，房间ID: {RoomId}", roomId);
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = $"获取房间详情失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        public async Task<ApiResponse<RoomDetailDto>> CreateRoomAsync(RoomCreateDto dto)
        {
            try
            {
                var sql = @"
                    INSERT INTO RoomManagement (room_number, room_type, capacity, status, description, created_at, updated_at)
                    VALUES (:roomNumber, :roomType, :capacity, :status, :description, SYSDATE, SYSDATE)";

                var parameters = new
                {
                    roomNumber = dto.RoomNumber,
                    roomType = dto.RoomType,
                    capacity = dto.Capacity,
                    status = dto.Status,
                    description = dto.Description
                };

                var result = await _databaseService.ExecuteAsync(sql, parameters);
                
                if (result > 0)
                {
                    // 获取新创建的房间（通过房间号查找）
                    var createdRoom = await GetRoomByRoomNumberAsync(dto.RoomNumber);
                    if (createdRoom.Success)
                    {
                        createdRoom.Message = "创建房间成功";
                        return createdRoom;
                    }
                }

                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "创建房间失败"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建房间失败");
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = $"创建房间失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据房间号获取房间
        /// </summary>
        private async Task<ApiResponse<RoomDetailDto>> GetRoomByRoomNumberAsync(string roomNumber)
        {
            try
            {
                var sql = @"
                    SELECT room_id, room_number, room_type, capacity, status, description, 
                           created_at, updated_at
                    FROM RoomManagement 
                    WHERE room_number = :roomNumber";

                var results = await _databaseService.QueryAsync<dynamic>(sql, new { roomNumber });
                var room = results.FirstOrDefault();
                
                if (room != null)
                {
                    var roomDetail = new RoomDetailDto
                    {
                        RoomId = Convert.ToInt32(room.ROOM_ID),
                        RoomNumber = room.ROOM_NUMBER?.ToString() ?? "",
                        RoomType = room.ROOM_TYPE?.ToString() ?? "",
                        Capacity = Convert.ToInt32(room.CAPACITY),
                        Status = room.STATUS?.ToString() ?? "",
                        Description = room.DESCRIPTION?.ToString(),
                        CreatedAt = Convert.ToDateTime(room.CREATED_AT),
                        UpdatedAt = Convert.ToDateTime(room.UPDATED_AT)
                    };

                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = true,
                        Message = "获取房间成功",
                        Data = roomDetail
                    };
                }

                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "房间不存在"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据房间号获取房间失败");
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = $"获取房间失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 更新房间
        /// </summary>
        public async Task<ApiResponse<RoomDetailDto>> UpdateRoomAsync(int roomId, RoomUpdateDto dto)
        {
            try
            {
                var setParts = new List<string>();
                var parameters = new Dictionary<string, object> { { "roomId", roomId } };

                if (!string.IsNullOrEmpty(dto.RoomNumber))
                {
                    setParts.Add("room_number = :roomNumber");
                    parameters["roomNumber"] = dto.RoomNumber;
                }
                if (!string.IsNullOrEmpty(dto.RoomType))
                {
                    setParts.Add("room_type = :roomType");
                    parameters["roomType"] = dto.RoomType;
                }
                if (dto.Capacity.HasValue)
                {
                    setParts.Add("capacity = :capacity");
                    parameters["capacity"] = dto.Capacity.Value;
                }
                if (!string.IsNullOrEmpty(dto.Status))
                {
                    setParts.Add("status = :status");
                    parameters["status"] = dto.Status;
                }
                if (dto.Description != null)
                {
                    setParts.Add("description = :description");
                    parameters["description"] = dto.Description;
                }

                if (!setParts.Any())
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = "没有提供需要更新的字段"
                    };
                }

                setParts.Add("updated_at = SYSDATE");
                var sql = $"UPDATE RoomManagement SET {string.Join(", ", setParts)} WHERE room_id = :roomId";

                var result = await _databaseService.ExecuteAsync(sql, parameters);
                
                if (result > 0)
                {
                    var updatedRoom = await GetRoomByIdAsync(roomId);
                    if (updatedRoom.Success)
                    {
                        updatedRoom.Message = "更新房间成功";
                        return updatedRoom;
                    }
                }

                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "房间不存在"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新房间失败，房间ID: {RoomId}", roomId);
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = $"更新房间失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteRoomAsync(int roomId)
        {
            try
            {
                // 检查房间是否有关联设备
                var deviceCheckSql = "SELECT COUNT(*) as device_count FROM DeviceStatus WHERE room_id = :roomId";
                var deviceResults = await _databaseService.QueryAsync<dynamic>(deviceCheckSql, new { roomId });
                var deviceCount = Convert.ToInt32(deviceResults.First().DEVICE_COUNT);
                
                if (deviceCount > 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"无法删除房间，该房间还有 {deviceCount} 个关联设备，请先移除设备"
                    };
                }

                var sql = "DELETE FROM RoomManagement WHERE room_id = :roomId";
                var result = await _databaseService.ExecuteAsync(sql, new { roomId });

                if (result > 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "删除房间成功",
                        Data = true
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "房间不存在"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除房间失败，房间ID: {RoomId}", roomId);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"删除房间失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        public async Task<ApiResponse<object>> GetRoomStatisticsAsync()
        {
            try
            {
                var totalRoomsSql = "SELECT COUNT(*) FROM ROOMMANAGEMENT";
                var occupiedRoomsSql = "SELECT COUNT(*) FROM ROOMMANAGEMENT WHERE STATUS = '已入住'";
                var availableRoomsSql = "SELECT COUNT(*) FROM ROOMMANAGEMENT WHERE STATUS = '空闲'";
                var maintenanceRoomsSql = "SELECT COUNT(*) FROM ROOMMANAGEMENT WHERE STATUS = '维护中'";

                var totalRooms = await _databaseService.QueryAsync<int>(totalRoomsSql);
                var occupiedRooms = await _databaseService.QueryAsync<int>(occupiedRoomsSql);
                var availableRooms = await _databaseService.QueryAsync<int>(availableRoomsSql);
                var maintenanceRooms = await _databaseService.QueryAsync<int>(maintenanceRoomsSql);

                var statistics = new
                {
                    TotalRooms = totalRooms.FirstOrDefault(),
                    OccupiedRooms = occupiedRooms.FirstOrDefault(),
                    AvailableRooms = availableRooms.FirstOrDefault(),
                    MaintenanceRooms = maintenanceRooms.FirstOrDefault(),
                    OccupancyRate = totalRooms.FirstOrDefault() > 0 
                        ? (double)occupiedRooms.FirstOrDefault() / totalRooms.FirstOrDefault() * 100 
                        : 0
                };

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "获取房间统计信息成功",
                    Data = statistics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间统计信息失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取房间统计信息失败: {ex.Message}"
                };
            }
        }
    }
}