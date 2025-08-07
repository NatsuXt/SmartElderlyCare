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
                    "floor" => $"ORDER BY floor {(request.SortDesc ? "DESC" : "ASC")}",
                    _ => "ORDER BY room_id ASC"
                };

                var sql = $@"
                    SELECT * FROM (
                        SELECT room_id, room_number, room_type, capacity, status, rate, bed_type, floor,
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
                var totalCountResults = await _databaseService.QueryAsync<dynamic>(countSql, countParameters);
                var totalCount = Convert.ToInt32(totalCountResults.First().GetType().GetProperty("COUNT(*)") != null ?
                    totalCountResults.First().GetType().GetProperty("COUNT(*)")?.GetValue(totalCountResults.First()) : 0);

                // 获取房间数据 - 直接使用RoomManagement模型
                var rooms = await _databaseService.QueryAsync<RoomManagement>(sql, parameters);
                
                var roomList = rooms.Select(r => new RoomDetailDto
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    Capacity = r.Capacity,
                    Status = r.Status,
                    Rate = r.Rate,
                    BedType = r.BedType,
                    Floor = r.Floor,
                    Description = null, // 不在数据库模型中
                    CreatedAt = DateTime.Now,   // 不在数据库模型中
                    UpdatedAt = DateTime.Now    // 不在数据库模型中
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
                    SELECT room_id, room_number, room_type, capacity, status, rate, bed_type, floor
                    FROM RoomManagement 
                    WHERE room_id = :roomId";

                var rooms = await _databaseService.QueryAsync<RoomManagement>(sql, new { roomId });
                var room = rooms.FirstOrDefault();
                
                if (room != null)
                {
                    var roomDetail = new RoomDetailDto
                    {
                        RoomId = room.RoomId,
                        RoomNumber = room.RoomNumber,
                        RoomType = room.RoomType,
                        Capacity = room.Capacity,
                        Status = room.Status,
                        Rate = room.Rate,
                        BedType = room.BedType,
                        Floor = room.Floor,
                        Description = null,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
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
                    INSERT INTO RoomManagement (
                        room_number, room_type, capacity, status, rate, bed_type, floor
                    ) VALUES (
                        :RoomNumber, :RoomType, :Capacity, :Status, :Rate, :BedType, :Floor
                    )";

                var parameters = new
                {
                    RoomNumber = dto.RoomNumber,
                    RoomType = dto.RoomType,
                    Capacity = dto.Capacity,
                    Status = dto.Status,
                    Rate = dto.Rate,
                    BedType = dto.BedType,
                    Floor = dto.Floor
                };

                var result = await _databaseService.ExecuteAsync(sql, parameters);
                
                if (result > 0)
                {
                    // 通过房间号获取新创建的房间
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
                    SELECT room_id, room_number, room_type, capacity, status, rate, bed_type, floor
                    FROM RoomManagement 
                    WHERE room_number = :roomNumber";

                var rooms = await _databaseService.QueryAsync<RoomManagement>(sql, new { roomNumber });
                var room = rooms.FirstOrDefault();
                
                if (room != null)
                {
                    var roomDetail = new RoomDetailDto
                    {
                        RoomId = room.RoomId,
                        RoomNumber = room.RoomNumber,
                        RoomType = room.RoomType,
                        Capacity = room.Capacity,
                        Status = room.Status,
                        Rate = room.Rate,
                        BedType = room.BedType,
                        Floor = room.Floor,
                        Description = null,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
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
                if (dto.Rate.HasValue)
                {
                    setParts.Add("rate = :rate");
                    parameters["rate"] = dto.Rate.Value;
                }
                if (!string.IsNullOrEmpty(dto.BedType))
                {
                    setParts.Add("bed_type = :bedType");
                    parameters["bedType"] = dto.BedType;
                }
                if (dto.Floor.HasValue)
                {
                    setParts.Add("floor = :floor");
                    parameters["floor"] = dto.Floor.Value;
                }

                if (!setParts.Any())
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = "没有提供需要更新的字段"
                    };
                }

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
                    Message = "房间不存在",
                    Data = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除房间失败，房间ID: {RoomId}", roomId);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"删除房间失败: {ex.Message}",
                    Data = false
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
                var sql = @"
                    SELECT 
                        COUNT(*) as TotalRooms,
                        COUNT(CASE WHEN status = '已入住' THEN 1 END) as OccupiedRooms,
                        COUNT(CASE WHEN status = '空闲' THEN 1 END) as AvailableRooms,
                        AVG(capacity) as AverageCapacity,
                        AVG(rate) as AverageRate,
                        COUNT(DISTINCT floor) as TotalFloors
                    FROM RoomManagement";

                var results = await _databaseService.QueryAsync<dynamic>(sql);
                var stats = results.First();

                var statistics = new
                {
                    TotalRooms = Convert.ToInt32(stats.GetType().GetProperty("TOTALROOMS")?.GetValue(stats) ?? 0),
                    OccupiedRooms = Convert.ToInt32(stats.GetType().GetProperty("OCCUPIEDROOMS")?.GetValue(stats) ?? 0),
                    AvailableRooms = Convert.ToInt32(stats.GetType().GetProperty("AVAILABLEROOMS")?.GetValue(stats) ?? 0),
                    AverageCapacity = Convert.ToDouble(stats.GetType().GetProperty("AVERAGECAPACITY")?.GetValue(stats) ?? 0),
                    AverageRate = Convert.ToDecimal(stats.GetType().GetProperty("AVERAGERATE")?.GetValue(stats) ?? 0),
                    TotalFloors = Convert.ToInt32(stats.GetType().GetProperty("TOTALFLOORS")?.GetValue(stats) ?? 0),
                    LastUpdated = DateTime.Now
                };

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "获取房间统计成功",
                    Data = statistics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间统计失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取房间统计失败: {ex.Message}"
                };
            }
        }
    }
}
