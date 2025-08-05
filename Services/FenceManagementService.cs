using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 电子围栏管理服务
    /// </summary>
    public class FenceManagementService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<FenceManagementService> _logger;

        public FenceManagementService(DatabaseService databaseService, ILogger<FenceManagementService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 创建电子围栏
        /// </summary>
        public async Task<ApiResponse<FenceCreateDto>> CreateFenceAsync(FenceCreateDto fenceDto)
        {
            try
            {
                var sql = @"
                    INSERT INTO ElectronicFence (
                        fence_name, area_definition, status, created_date, updated_date
                    ) VALUES (
                        :FenceName, :AreaDefinition, :Status, :CreatedDate, :UpdatedDate
                    )";
                
                var parameters = new
                {
                    FenceName = fenceDto.FenceName,
                    fenceDto.AreaDefinition,
                    fenceDto.Status,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
                
                await _databaseService.ExecuteAsync(sql, parameters);

                return new ApiResponse<FenceCreateDto>
                {
                    Success = true,
                    Message = "电子围栏创建成功",
                    Data = fenceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建电子围栏失败");
                return new ApiResponse<FenceCreateDto>
                {
                    Success = false,
                    Message = $"创建电子围栏失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 更新电子围栏
        /// </summary>
        public async Task<ApiResponse<FenceUpdateDto>> UpdateFenceAsync(int fenceId, FenceUpdateDto fenceDto)
        {
            try
            {
                var sql = @"
                    UPDATE ElectronicFence SET
                        fence_name = :FenceName,
                        area_definition = :AreaDefinition,
                        status = :Status,
                        updated_date = :UpdatedDate
                    WHERE fence_id = :FenceId";
                
                var parameters = new
                {
                    FenceName = fenceDto.FenceName,
                    fenceDto.AreaDefinition,
                    fenceDto.Status,
                    UpdatedDate = DateTime.Now,
                    FenceId = fenceId
                };
                
                var rowsAffected = await _databaseService.ExecuteAsync(sql, parameters);
                
                if (rowsAffected == 0)
                {
                    return new ApiResponse<FenceUpdateDto>
                    {
                        Success = false,
                        Message = "电子围栏不存在"
                    };
                }

                return new ApiResponse<FenceUpdateDto>
                {
                    Success = true,
                    Message = "电子围栏更新成功",
                    Data = fenceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新电子围栏失败");
                return new ApiResponse<FenceUpdateDto>
                {
                    Success = false,
                    Message = $"更新电子围栏失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 删除电子围栏
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteFenceAsync(int fenceId)
        {
            try
            {
                // 检查是否有关联的日志记�?
                var logCheckSql = "SELECT COUNT(*) FROM FenceLog WHERE fence_id = :FenceId";
                var logCount = await _databaseService.QuerySingleAsync<int>(logCheckSql, new { FenceId = fenceId });
                
                if (logCount > 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "电子围栏有关联的日志记录，无法删除"
                    };
                }

                var sql = "DELETE FROM ElectronicFence WHERE fence_id = :FenceId";
                var rowsAffected = await _databaseService.ExecuteAsync(sql, new { FenceId = fenceId });
                
                if (rowsAffected == 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "电子围栏不存在"
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "电子围栏删除成功",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除电子围栏失败");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"删除电子围栏失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取电子围栏列表
        /// </summary>
        public async Task<ApiResponse<PagedResult<FenceInfoDTO>>> GetFencesAsync(PagedRequest request)
        {
            try
            {
                var countSql = "SELECT COUNT(*) FROM ElectronicFence";
                var totalCount = await _databaseService.QuerySingleAsync<int>(countSql);

                var offset = (request.Page - 1) * request.PageSize;
                var sql = @"
                    SELECT fence_id as FenceId, fence_name as FenceName, area_definition as AreaDefinition,
                           status as Status, created_date as CreatedDate, updated_date as UpdatedDate
                    FROM ElectronicFence
                    ORDER BY fence_id
                    OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

                var fences = await _databaseService.QueryAsync<FenceInfoDTO>(sql, new { Offset = offset, request.PageSize });

                var result = new PagedResult<FenceInfoDTO>
                {
                    Items = fences,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                return new ApiResponse<PagedResult<FenceInfoDTO>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取电子围栏列表失败");
                return new ApiResponse<PagedResult<FenceInfoDTO>>
                {
                    Success = false,
                    Message = $"获取电子围栏列表失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据ID获取电子围栏详情
        /// </summary>
        public async Task<ApiResponse<FenceInfoDTO>> GetFenceByIdAsync(int fenceId)
        {
            try
            {
                var sql = @"
                    SELECT fence_id as FenceId, fence_name as FenceName, area_definition as AreaDefinition,
                           status as Status, created_date as CreatedDate, updated_date as UpdatedDate
                    FROM ElectronicFence
                    WHERE fence_id = :FenceId";

                var fence = await _databaseService.QuerySingleOrDefaultAsync<FenceInfoDTO>(sql, new { FenceId = fenceId });
                
                if (fence == null)
                {
                    return new ApiResponse<FenceInfoDTO>
                    {
                        Success = false,
                        Message = "电子围栏不存在"
                    };
                }

                return new ApiResponse<FenceInfoDTO>
                {
                    Success = true,
                    Data = fence
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取电子围栏详情失败");
                return new ApiResponse<FenceInfoDTO>
                {
                    Success = false,
                    Message = $"获取电子围栏详情失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取电子围栏统计信息
        /// </summary>
        public async Task<ApiResponse<object>> GetFenceStatisticsAsync()
        {
            try
            {
                var statisticsQuery = @"
                    SELECT 
                        COUNT(*) as TotalFences,
                        SUM(CASE WHEN status = '启用' THEN 1 ELSE 0 END) as ActiveFences,
                        SUM(CASE WHEN status = '禁用' THEN 1 ELSE 0 END) as DisabledFences
                    FROM ElectronicFence";

                var result = await _databaseService.QuerySingleAsync<object>(statisticsQuery);

                return new ApiResponse<object>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取电子围栏统计失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取电子围栏统计失败: {ex.Message}"
                };
            }
        }
    }
}
