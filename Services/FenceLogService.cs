using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 围栏日志服务
    /// </summary>
    public class FenceLogService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<FenceLogService> _logger;

        public FenceLogService(DatabaseService databaseService, ILogger<FenceLogService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 创建围栏日志
        /// </summary>
        public async Task<ApiResponse<FenceLogCreateDto>> CreateFenceLogAsync(FenceLogCreateDto logDto)
        {
            try
            {
                var sql = @"
                    INSERT INTO FenceLog (
                        elderly_id, fence_id, event_type, event_time, location, notes
                    ) VALUES (
                        :ElderlyId, :FenceId, :EventType, :EventTime, :Location, :Notes
                    )";
                
                var parameters = new
                {
                    logDto.ElderlyId,
                    logDto.FenceId,
                    logDto.EventType,
                    EventTime = logDto.EventTime ?? DateTime.Now,
                    logDto.Location,
                    logDto.Notes
                };
                
                await _databaseService.ExecuteAsync(sql, parameters);

                return new ApiResponse<FenceLogCreateDto>
                {
                    Success = true,
                    Message = "围栏日志创建成功",
                    Data = logDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建围栏日志失败");
                return new ApiResponse<FenceLogCreateDto>
                {
                    Success = false,
                    Message = $"创建围栏日志失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取围栏日志列表
        /// </summary>
        public async Task<ApiResponse<PagedResult<FenceLogInfoDTO>>> GetFenceLogsAsync(PagedRequest request, int? elderlyId = null, int? fenceId = null)
        {
            try
            {
                var whereConditions = new List<string>();
                var parameters = new Dictionary<string, object>
                {
                    ["Offset"] = (request.Page - 1) * request.PageSize,
                    ["PageSize"] = request.PageSize
                };

                if (elderlyId.HasValue)
                {
                    whereConditions.Add("elderly_id = :ElderlyId");
                    parameters["ElderlyId"] = elderlyId.Value;
                }

                if (fenceId.HasValue)
                {
                    whereConditions.Add("fence_id = :FenceId");
                    parameters["FenceId"] = fenceId.Value;
                }

                var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                var countSql = $"SELECT COUNT(*) FROM FenceLog {whereClause}";
                var totalCount = await _databaseService.QuerySingleAsync<int>(countSql, parameters);

                var sql = $@"
                    SELECT log_id as LogId, elderly_id as ElderlyId, fence_id as FenceId,
                           event_type as EventType, event_time as EventTime, location as Location, notes as Notes
                    FROM FenceLog
                    {whereClause}
                    ORDER BY event_time DESC
                    OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

                var logs = await _databaseService.QueryAsync<FenceLogInfoDTO>(sql, parameters);

                var result = new PagedResult<FenceLogInfoDTO>
                {
                    Items = logs,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                return new ApiResponse<PagedResult<FenceLogInfoDTO>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取围栏日志列表失败");
                return new ApiResponse<PagedResult<FenceLogInfoDTO>>
                {
                    Success = false,
                    Message = $"获取围栏日志列表失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据ID获取围栏日志详情
        /// </summary>
        public async Task<ApiResponse<FenceLogInfoDTO>> GetFenceLogByIdAsync(int logId)
        {
            try
            {
                var sql = @"
                    SELECT log_id as LogId, elderly_id as ElderlyId, fence_id as FenceId,
                           event_type as EventType, event_time as EventTime, location as Location, notes as Notes
                    FROM FenceLog
                    WHERE log_id = :LogId";

                var log = await _databaseService.QuerySingleOrDefaultAsync<FenceLogInfoDTO>(sql, new { LogId = logId });
                
                if (log == null)
                {
                    return new ApiResponse<FenceLogInfoDTO>
                    {
                        Success = false,
                        Message = "围栏日志不存在"
                    };
                }

                return new ApiResponse<FenceLogInfoDTO>
                {
                    Success = true,
                    Data = log
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取围栏日志详情失败");
                return new ApiResponse<FenceLogInfoDTO>
                {
                    Success = false,
                    Message = $"获取围栏日志详情失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 删除围栏日志
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteFenceLogAsync(int logId)
        {
            try
            {
                var sql = "DELETE FROM FenceLog WHERE log_id = :LogId";
                var rowsAffected = await _databaseService.ExecuteAsync(sql, new { LogId = logId });
                
                if (rowsAffected == 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "围栏日志不存在"
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "围栏日志删除成功",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除围栏日志失败");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"删除围栏日志失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取围栏日志统计信息
        /// </summary>
        public async Task<ApiResponse<object>> GetFenceLogStatisticsAsync()
        {
            try
            {
                var statisticsQuery = @"
                    SELECT 
                        COUNT(*) as TotalLogs,
                        SUM(CASE WHEN event_type = '进入' THEN 1 ELSE 0 END) as EnterEvents,
                        SUM(CASE WHEN event_type = '离开' THEN 1 ELSE 0 END) as ExitEvents,
                        SUM(CASE WHEN event_type = '报警' THEN 1 ELSE 0 END) as AlarmEvents
                    FROM FenceLog";

                var result = await _databaseService.QuerySingleAsync<object>(statisticsQuery);

                return new ApiResponse<object>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取围栏日志统计失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取围栏日志统计失败: {ex.Message}"
                };
            }
        }
    }
}
