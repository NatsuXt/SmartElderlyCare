using Oracle.ManagedDataAccess.Client;
using RoomDeviceManagement.DTOs;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Services
{
    public class RoomManagementService
    {
        private readonly ChineseCompatibleDatabaseService _chineseDbService;
        private readonly ILogger<RoomManagementService> _logger;
        private readonly RoomOccupancyService _roomOccupancyService;

        public RoomManagementService(
            ChineseCompatibleDatabaseService chineseDbService, 
            ILogger<RoomManagementService> logger,
            RoomOccupancyService roomOccupancyService)
        {
            _chineseDbService = chineseDbService;
            _logger = logger;
            _roomOccupancyService = roomOccupancyService;
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
                
                var room = await _chineseDbService.GetRoomByIdAsync(roomId);
                
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
                var existingRoom = await _chineseDbService.GetRoomByIdAsync(roomId);
                if (existingRoom == null)
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = $"未找到房间 ID: {roomId}"
                    };
                }

                // 构建更新字段
                var updateFields = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(updateRoomDto.RoomNumber))
                {
                    updateFields["roomNumber"] = updateRoomDto.RoomNumber.Trim();
                }
                
                if (!string.IsNullOrWhiteSpace(updateRoomDto.RoomType))
                {
                    updateFields["roomType"] = updateRoomDto.RoomType.Trim();
                }
                
                if (updateRoomDto.Capacity.HasValue)
                {
                    updateFields["capacity"] = updateRoomDto.Capacity.Value;
                }
                
                if (!string.IsNullOrWhiteSpace(updateRoomDto.Status))
                {
                    updateFields["status"] = updateRoomDto.Status.Trim();
                }
                
                if (updateRoomDto.Rate.HasValue)
                {
                    updateFields["rate"] = updateRoomDto.Rate.Value;
                }
                
                if (!string.IsNullOrWhiteSpace(updateRoomDto.BedType))
                {
                    updateFields["bedType"] = updateRoomDto.BedType.Trim();
                }
                
                if (updateRoomDto.Floor.HasValue)
                {
                    updateFields["floor"] = updateRoomDto.Floor.Value;
                }

                // 如果有房间号更新，检查是否重复
                if (updateFields.ContainsKey("roomNumber"))
                {
                    var newRoomNumber = updateFields["roomNumber"]?.ToString();
                    if (!string.IsNullOrEmpty(newRoomNumber))
                    {
                        var duplicateRoom = await _chineseDbService.GetRoomByNumberAsync(newRoomNumber);
                        if (duplicateRoom != null && duplicateRoom.RoomId != roomId)
                        {
                            return new ApiResponse<RoomDetailDto>
                            {
                                Success = false,
                                Message = $"房间号 {newRoomNumber} 已存在"
                            };
                        }
                    }
                }

                // 执行更新
                if (updateFields.Any())
                {
                    var rowsAffected = await _chineseDbService.UpdateRoomAsync(roomId, updateFields);
                    
                    if (rowsAffected > 0)
                    {
                        // 获取更新后的房间信息
                        var updatedRoom = await _chineseDbService.GetRoomByIdAsync(roomId);
                        
                        if (updatedRoom == null)
                        {
                            return new ApiResponse<RoomDetailDto>
                            {
                                Success = false,
                                Message = "房间更新成功但无法获取更新后的信息"
                            };
                        }
                        
                        return new ApiResponse<RoomDetailDto>
                        {
                            Success = true,
                            Message = "房间信息更新成功",
                            Data = new RoomDetailDto
                            {
                                RoomId = updatedRoom.RoomId,
                                RoomNumber = updatedRoom.RoomNumber,
                                RoomType = updatedRoom.RoomType,
                                Capacity = updatedRoom.Capacity,
                                Status = updatedRoom.Status,
                                Rate = updatedRoom.Rate,
                                BedType = updatedRoom.BedType,
                                Floor = updatedRoom.Floor
                            }
                        };
                    }
                    else
                    {
                        return new ApiResponse<RoomDetailDto>
                        {
                            Success = false,
                            Message = "房间信息未发生变化"
                        };
                    }
                }
                else
                {
                    return new ApiResponse<RoomDetailDto>
                    {
                        Success = false,
                        Message = "没有提供需要更新的字段"
                    };
                }
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

                // 调用数据库服务删除房间
                var deletedRows = await _chineseDbService.DeleteRoomAsync(roomId);
                
                if (deletedRows > 0)
                {
                    _logger.LogInformation($"✅ 成功删除房间: ID={roomId}");
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Message = "房间删除成功",
                        Data = true
                    };
                }
                else
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "房间删除失败，没有找到对应记录",
                        Data = false
                    };
                }
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

        // ===================================================================================
        // 🏠 房间入住管理功能 - 集成到RoomManagementService中
        // ===================================================================================

        /// <summary>
        /// 根据老人ID获取入住记录
        /// </summary>
        public async Task<ApiResponse<List<OccupancyRecordDto>>> GetOccupancyRecordsByElderlyIdAsync(decimal elderlyId)
        {
            try
            {
                _logger.LogInformation($"🔍 获取老人ID={elderlyId}的入住记录");

                // 调用数据库服务获取真实数据
                var records = await _chineseDbService.GetOccupancyRecordsByElderlyIdAsync((int)elderlyId);

                var recordDtos = records.Select(r => new OccupancyRecordDto
                {
                    OccupancyId = r.OccupancyId,
                    RoomId = r.RoomId,
                    ElderlyId = r.ElderlyId,
                    RoomNumber = r.RoomNumber,
                    ElderlyName = r.ElderlyName,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    Status = r.Status,
                    BedNumber = r.BedNumber,
                    Remarks = r.Remarks,
                    CreatedDate = r.CreatedDate,
                    UpdatedDate = r.UpdatedDate
                }).ToList();

                _logger.LogInformation($"✅ 成功获取到 {recordDtos.Count} 条入住记录");
                return new ApiResponse<List<OccupancyRecordDto>>
                {
                    Success = true,
                    Message = $"成功获取到 {recordDtos.Count} 条入住记录",
                    Data = recordDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取老人ID={elderlyId}的入住记录失败");
                return new ApiResponse<List<OccupancyRecordDto>>
                {
                    Success = false,
                    Message = $"获取入住记录失败: {ex.Message}",
                    Data = new List<OccupancyRecordDto>()
                };
            }
        }

        /// <summary>
        /// 获取所有入住记录（分页）
        /// </summary>
        public async Task<ApiResponse<PagedResult<OccupancyRecordDto>>> GetAllOccupancyRecordsAsync(PagedRequest request, string? status = null)
        {
            try
            {
                _logger.LogInformation($"🔍 获取所有入住记录: 页码={request.Page}, 大小={request.PageSize}, 状态筛选={status}");

                // 调用数据库服务获取分页入住记录
                var (records, totalCount) = await _chineseDbService.GetAllOccupancyRecordsAsync(
                    request.Page, 
                    request.PageSize, 
                    status
                );

                var occupancyDtos = records.Select(r => new OccupancyRecordDto
                {
                    OccupancyId = r.OccupancyId,
                    ElderlyId = r.ElderlyId,
                    ElderlyName = r.ElderlyName,
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    Status = r.Status,
                    Remarks = r.Remarks,
                    CreatedDate = r.CreatedDate,
                    UpdatedDate = r.UpdatedDate
                }).ToList();

                var result = new PagedResult<OccupancyRecordDto>
                {
                    Items = occupancyDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                return new ApiResponse<PagedResult<OccupancyRecordDto>>
                {
                    Success = true,
                    Message = $"成功获取入住记录，共{totalCount}条",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 获取所有入住记录失败");
                return new ApiResponse<PagedResult<OccupancyRecordDto>>
                {
                    Success = false,
                    Message = $"获取入住记录失败: {ex.Message}",
                    Data = new PagedResult<OccupancyRecordDto>()
                };
            }
        }

        /// <summary>
        /// 办理入住登记
        /// </summary>
        public async Task<ApiResponse<OccupancyRecordDto>> CheckInAsync(CheckInDto checkInDto)
        {
            try
            {
                _logger.LogInformation($"🏠 办理入住登记: 老人ID={checkInDto.ElderlyId}, 房间ID={checkInDto.RoomId}");

                // 调用数据库服务创建入住记录
                var occupancyId = await _chineseDbService.CreateOccupancyRecordAsync(
                    checkInDto.RoomId, 
                    (int)checkInDto.ElderlyId, 
                    checkInDto.CheckInDate, 
                    checkInDto.BedNumber ?? "", 
                    checkInDto.Remarks ?? ""
                );

                // 获取创建的入住记录详情
                var createdRecord = await _chineseDbService.GetOccupancyRecordByIdAsync(occupancyId);
                
                if (createdRecord == null)
                {
                    throw new Exception("入住记录创建成功但无法获取详细信息");
                }

                var result = new OccupancyRecordDto
                {
                    OccupancyId = createdRecord.OccupancyId,
                    RoomId = createdRecord.RoomId,
                    ElderlyId = createdRecord.ElderlyId,
                    RoomNumber = createdRecord.RoomNumber,
                    ElderlyName = createdRecord.ElderlyName,
                    CheckInDate = createdRecord.CheckInDate,
                    CheckOutDate = createdRecord.CheckOutDate,
                    Status = createdRecord.Status,
                    BedNumber = createdRecord.BedNumber,
                    Remarks = createdRecord.Remarks,
                    CreatedDate = createdRecord.CreatedDate,
                    UpdatedDate = createdRecord.UpdatedDate
                };

                return new ApiResponse<OccupancyRecordDto>
                {
                    Success = true,
                    Message = "入住登记成功",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 入住登记失败");
                return new ApiResponse<OccupancyRecordDto>
                {
                    Success = false,
                    Message = $"入住登记失败: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// 办理退房登记
        /// </summary>
        public async Task<ApiResponse<OccupancyRecordDto>> CheckOutAsync(CheckOutDto checkOutDto)
        {
            try
            {
                _logger.LogInformation($"🚪 办理退房登记: 入住记录ID={checkOutDto.OccupancyId}");

                // 调用数据库服务更新入住记录
                var success = await _chineseDbService.UpdateOccupancyRecordAsync(
                    checkOutDto.OccupancyId, 
                    checkOutDto.CheckOutDate, 
                    checkOutDto.Remarks ?? ""
                );

                if (!success)
                {
                    throw new Exception("未找到对应的入住记录或更新失败");
                }

                // 获取更新后的入住记录详情
                var updatedRecord = await _chineseDbService.GetOccupancyRecordByIdAsync(checkOutDto.OccupancyId);
                
                if (updatedRecord == null)
                {
                    throw new Exception("退房记录更新成功但无法获取详细信息");
                }

                var result = new OccupancyRecordDto
                {
                    OccupancyId = updatedRecord.OccupancyId,
                    RoomId = updatedRecord.RoomId,
                    ElderlyId = updatedRecord.ElderlyId,
                    RoomNumber = updatedRecord.RoomNumber,
                    ElderlyName = updatedRecord.ElderlyName,
                    CheckInDate = updatedRecord.CheckInDate,
                    CheckOutDate = updatedRecord.CheckOutDate,
                    Status = updatedRecord.Status,
                    BedNumber = updatedRecord.BedNumber,
                    Remarks = updatedRecord.Remarks,
                    CreatedDate = updatedRecord.CreatedDate,
                    UpdatedDate = updatedRecord.UpdatedDate
                };

                return new ApiResponse<OccupancyRecordDto>
                {
                    Success = true,
                    Message = "退房登记成功",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 退房登记失败");
                return new ApiResponse<OccupancyRecordDto>
                {
                    Success = false,
                    Message = $"退房登记失败: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// 生成所有房间账单（仅为已退房的入住记录生成）
        /// 按照occupancy_id防重复，直接使用入住和退房日期
        /// </summary>
        public async Task<ApiResponse<List<BillingRecordDto>>> GenerateAllBillingsAsync(GenerateBillDto generateDto)
        {
            try
            {
                _logger.LogInformation($"💰 调用新的月度账单逻辑生成所有房间账单: {generateDto.BillingStartDate:yyyy-MM-dd} 到 {generateDto.BillingEndDate:yyyy-MM-dd}");

                // 直接调用RoomOccupancyService的新月度账单逻辑
                var result = await _roomOccupancyService.GenerateAllBillingsAsync(generateDto);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 生成所有房间的月度账单失败");
                return new ApiResponse<List<BillingRecordDto>>
                {
                    Success = false,
                    Message = $"生成账单失败: {ex.Message}",
                    Data = new List<BillingRecordDto>()
                };
            }
        }

        /// <summary>
        /// 根据老人ID生成账单
        /// </summary>
        public async Task<ApiResponse<List<BillingRecordDto>>> GenerateBillingsForElderlyAsync(decimal elderlyId, GenerateBillDto generateDto)
        {
            try
            {
                _logger.LogInformation($"💰 调用新的月度账单逻辑为老人ID={elderlyId}生成账单: {generateDto.BillingStartDate:yyyy-MM-dd} 到 {generateDto.BillingEndDate:yyyy-MM-dd}");

                // 直接调用RoomOccupancyService的新月度账单逻辑
                var result = await _roomOccupancyService.GenerateBillingsForElderlyAsync(elderlyId, generateDto);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 生成老人ID={elderlyId}的账单失败");
                return new ApiResponse<List<BillingRecordDto>>
                {
                    Success = false,
                    Message = $"生成账单失败: {ex.Message}",
                    Data = new List<BillingRecordDto>()
                };
            }
        }

        /// <summary>
        /// 获取账单记录（支持按老人ID筛选）
        /// </summary>
        public async Task<ApiResponse<PagedResult<BillingRecordDto>>> GetBillingRecordsAsync(PagedRequest request, decimal? elderlyId = null)
        {
            try
            {
                _logger.LogInformation($"🔍 获取账单记录，页码={request.Page}，每页={request.PageSize}，老人ID筛选={elderlyId}");

                // 调用数据库服务获取分页账单记录
                var (records, totalCount) = await _chineseDbService.GetBillingRecordsAsync(
                    request.Page, 
                    request.PageSize, 
                    elderlyId.HasValue ? (int)elderlyId.Value : null
                );

                var billingDtos = records.Select(r => new BillingRecordDto
                {
                    BillingId = r.BillingId,
                    OccupancyId = r.OccupancyId,
                    ElderlyId = r.ElderlyId,
                    ElderlyName = r.ElderlyName,
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    BillingStartDate = r.BillingStartDate,
                    BillingEndDate = r.BillingEndDate,
                    Days = r.Days,
                    DailyRate = r.RoomRate,
                    TotalAmount = r.TotalAmount,
                    PaymentStatus = r.PaymentStatus,
                    PaidAmount = r.PaidAmount,  // 使用数据库中的实际已支付金额
                    UnpaidAmount = r.UnpaidAmount,  // 使用数据库中的实际未支付金额
                    BillingDate = r.CreatedDate,
                    PaymentDate = r.PaymentDate,
                    CreatedDate = r.CreatedDate,
                    UpdatedDate = r.UpdatedDate
                }).ToList();

                var result = new PagedResult<BillingRecordDto>
                {
                    Items = billingDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                _logger.LogInformation($"✅ 成功获取 {billingDtos.Count} 条账单记录，总数={totalCount}");

                return new ApiResponse<PagedResult<BillingRecordDto>>
                {
                    Success = true,
                    Message = $"成功获取 {billingDtos.Count} 条账单记录",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 获取账单记录失败");
                return new ApiResponse<PagedResult<BillingRecordDto>>
                {
                    Success = false,
                    Message = $"获取账单记录失败: {ex.Message}",
                    Data = new PagedResult<BillingRecordDto>()
                };
            }
        }
    }
}
