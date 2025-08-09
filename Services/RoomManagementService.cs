using Oracle.ManagedDataAccess.Client;
using RoomDeviceManagement.DTOs;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Services
{
    public class RoomManagementService
    {
        private readonly ChineseCompatibleDatabaseService _chineseDbService;
        private readonly ILogger<RoomManagementService> _logger;

        public RoomManagementService(ChineseCompatibleDatabaseService chineseDbService, ILogger<RoomManagementService> logger)
        {
            _chineseDbService = chineseDbService;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有房间（支持分页和搜索）- 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<List<RoomDetailDto>>> GetRoomsAsync(PagedRequest request)
        {
            try
            {
                _logger.LogInformation($"🔍 获取房间列表: 页码={request.Page}, 大小={request.PageSize}, 搜索='{request.Search}'");
                
                // 使用中文兼容数据库服务
                var rooms = await _chineseDbService.GetRoomsAsync(request.Search);
                
                // 手动分页
                var totalCount = rooms.Count;
                var pagedRooms = rooms
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();
                
                var roomDetails = pagedRooms.Select(room => new RoomDetailDto
                {
                    RoomId = room.RoomId,
                    RoomNumber = room.RoomNumber,
                    RoomType = room.RoomType,
                    Capacity = room.Capacity,
                    Status = room.Status,
                    Rate = room.Rate,
                    BedType = room.BedType,
                    Floor = room.Floor,
                    Description = $"房间 {room.RoomNumber}，{room.RoomType}",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList();

                _logger.LogInformation($"✅ 成功获取 {roomDetails.Count} 个房间，总计 {totalCount} 个");
                
                return new ApiResponse<List<RoomDetailDto>>
                {
                    Success = true,
                    Message = "获取房间列表成功",
                    Data = roomDetails,
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
        /// 根据房间号获取房间信息
        /// </summary>
        public async Task<ApiResponse<RoomDetailDto>> GetRoomByNumberAsync(string roomNumber)
        {
            try
            {
                _logger.LogInformation($"🔍 获取房间信息: {roomNumber}");
                
                var room = await _chineseDbService.GetRoomByNumberAsync(roomNumber);
                
                if (room == null)
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = $"未找到房间号 {roomNumber}",
                        Data = null
                    };
                }

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
                    Description = $"房间 {room.RoomNumber}，{room.RoomType}",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _logger.LogInformation($"✅ 成功获取房间信息: {roomDetail.RoomNumber} - {roomDetail.RoomType}");
                
                return new ApiResponse<RoomDetailDto>
                {
                    Success = true,
                    Message = "获取房间信息成功",
                    Data = roomDetail
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取房间信息失败: {roomNumber}");
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = $"获取房间信息失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据ID获取房间信息
        /// </summary>
        public async Task<ApiResponse<RoomDetailDto>> GetRoomByIdAsync(int roomId)
        {
            try
            {
                _logger.LogInformation($"🔍 根据ID获取房间信息: {roomId}");
                
                var rooms = await _chineseDbService.GetRoomsAsync("");
                var room = rooms.FirstOrDefault(r => r.RoomId == roomId);
                
                if (room == null)
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = $"未找到房间 ID: {roomId}",
                        Data = null
                    };
                }

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
                    Description = $"房间 {room.RoomNumber}，{room.RoomType}",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _logger.LogInformation($"✅ 成功获取房间信息: {roomDetail.RoomNumber} - {roomDetail.RoomType}");
                
                return new ApiResponse<RoomDetailDto>
                {
                    Success = true,
                    Message = "获取房间信息成功",
                    Data = roomDetail
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"根据ID获取房间信息失败: {roomId}");
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = $"获取房间信息失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 创建新房间 - 使用中文兼容服务
        /// </summary>
        public async Task<ApiResponse<RoomDetailDto>> CreateRoomAsync(RoomCreateDto createRoomDto)
        {
            try
            {
                _logger.LogInformation($"🏠 创建新房间: {createRoomDto.RoomNumber} - {createRoomDto.RoomType}");
                
                // 检查房间号是否已存在
                var existingRoom = await GetRoomByNumberAsync(createRoomDto.RoomNumber);
                if (existingRoom.Success && existingRoom.Data != null)
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = $"房间号 {createRoomDto.RoomNumber} 已存在"
                    };
                }

                // 使用中文兼容数据库服务创建房间
                var roomId = await _chineseDbService.CreateRoomAsync(
                    createRoomDto.RoomNumber,
                    createRoomDto.RoomType,
                    createRoomDto.Capacity,
                    createRoomDto.Status,
                    createRoomDto.Rate,
                    createRoomDto.BedType,
                    createRoomDto.Floor
                );

                // 获取创建的房间信息
                var createdRoom = await _chineseDbService.GetRoomByNumberAsync(createRoomDto.RoomNumber);
                
                if (createdRoom == null)
                {
                    throw new Exception("房间创建成功但无法获取创建的房间信息");
                }

                var roomDetail = new RoomDetailDto
                {
                    RoomId = createdRoom.RoomId,
                    RoomNumber = createdRoom.RoomNumber,
                    RoomType = createdRoom.RoomType,
                    Capacity = createdRoom.Capacity,
                    Status = createdRoom.Status,
                    Rate = createdRoom.Rate,
                    BedType = createdRoom.BedType,
                    Floor = createdRoom.Floor,
                    Description = $"房间 {createdRoom.RoomNumber}，{createdRoom.RoomType}",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _logger.LogInformation($"✅ 房间创建成功: ID={roomId}, 房间号={roomDetail.RoomNumber}, 类型={roomDetail.RoomType}");
                
                return new ApiResponse<RoomDetailDto>
                {
                    Success = true,
                    Message = "房间创建成功",
                    Data = roomDetail
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建房间失败: {createRoomDto.RoomNumber}");
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = $"创建房间失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 更新房间信息
        /// </summary>
        public async Task<ApiResponse<RoomDetailDto>> UpdateRoomAsync(int roomId, RoomUpdateDto updateRoomDto)
        {
            try
            {
                _logger.LogInformation($"📝 更新房间信息: ID={roomId}");
                
                // 先获取现有房间信息
                var existingRoom = await GetRoomByIdAsync(roomId);
                if (!existingRoom.Success || existingRoom.Data == null)
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = $"未找到房间 ID: {roomId}"
                    };
                }

                // 这里需要在ChineseCompatibleDatabaseService中添加更新方法
                // 暂时返回未实现消息
                return new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "更新功能正在开发中，请稍后再试"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新房间失败: ID={roomId}");
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
                _logger.LogInformation($"🗑️ 删除房间: ID={roomId}");
                
                // 先获取现有房间信息
                var existingRoom = await GetRoomByIdAsync(roomId);
                if (!existingRoom.Success || existingRoom.Data == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"未找到房间 ID: {roomId}",
                        Data = false
                    };
                }

                // 这里需要在ChineseCompatibleDatabaseService中添加删除方法
                // 暂时返回未实现消息
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "删除功能正在开发中，请稍后再试",
                    Data = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除房间失败: ID={roomId}");
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
                _logger.LogInformation("📊 获取房间统计信息");
                
                var rooms = await _chineseDbService.GetRoomsAsync("");
                
                var statistics = new
                {
                    TotalRooms = rooms.Count,
                    AvailableRooms = rooms.Count(r => r.Status == "空闲" || r.Status == "可用"),
                    OccupiedRooms = rooms.Count(r => r.Status == "已占用" || r.Status == "入住"),
                    MaintenanceRooms = rooms.Count(r => r.Status == "维修" || r.Status == "停用"),
                    RoomTypeStats = rooms.GroupBy(r => r.RoomType).Select(g => new
                    {
                        RoomType = g.Key,
                        Count = g.Count(),
                        AvailableCount = g.Count(r => r.Status == "空闲" || r.Status == "可用")
                    }).ToList(),
                    FloorStats = rooms.GroupBy(r => r.Floor).Select(g => new
                    {
                        Floor = g.Key,
                        Count = g.Count(),
                        AvailableCount = g.Count(r => r.Status == "空闲" || r.Status == "可用")
                    }).OrderBy(f => f.Floor).ToList(),
                    AverageRate = rooms.Any() ? rooms.Average(r => r.Rate) : 0
                };

                _logger.LogInformation($"✅ 房间统计: 总计{statistics.TotalRooms}间，可用{statistics.AvailableRooms}间，已占用{statistics.OccupiedRooms}间");
                
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
