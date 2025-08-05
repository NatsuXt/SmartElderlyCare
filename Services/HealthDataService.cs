using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 健康数据服务
    /// </summary>
    public class HealthDataService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<HealthDataService> _logger;

        public HealthDataService(DatabaseService databaseService, ILogger<HealthDataService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 创建健康监测记录
        /// </summary>
        public async Task<ApiResponse<HealthDataCreateDto>> CreateHealthMonitoringAsync(HealthDataCreateDto healthDto)
        {
            try
            {
                var sql = @"
                    INSERT INTO HealthMonitoring (
                        elderly_id, monitoring_date, heart_rate, blood_pressure_high, 
                        blood_pressure_low, blood_sugar, body_temperature, activity_level, notes
                    ) VALUES (
                        :ElderlyId, :MonitoringDate, :HeartRate, :BloodPressureHigh,
                        :BloodPressureLow, :BloodSugar, :BodyTemperature, :ActivityLevel, :Notes
                    )";
                
                var parameters = new
                {
                    healthDto.ElderlyId,
                    MonitoringDate = healthDto.MonitoringDate ?? DateTime.Now,
                    healthDto.HeartRate,
                    healthDto.BloodPressureHigh,
                    healthDto.BloodPressureLow,
                    healthDto.BloodSugar,
                    healthDto.BodyTemperature,
                    healthDto.ActivityLevel,
                    healthDto.Notes
                };
                
                await _databaseService.ExecuteAsync(sql, parameters);

                return new ApiResponse<HealthDataCreateDto>
                {
                    Success = true,
                    Message = "健康监测记录创建成功",
                    Data = healthDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建健康监测记录失败");
                return new ApiResponse<HealthDataCreateDto>
                {
                    Success = false,
                    Message = $"创建健康监测记录失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取健康监测记录列表
        /// </summary>
        public async Task<ApiResponse<PagedResult<HealthDataDetailDto>>> GetHealthMonitoringAsync(
            PagedRequest request, int? elderlyId = null, DateTime? startDate = null, DateTime? endDate = null)
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

                if (startDate.HasValue)
                {
                    whereConditions.Add("monitoring_date >= :StartDate");
                    parameters["StartDate"] = startDate.Value;
                }

                if (endDate.HasValue)
                {
                    whereConditions.Add("monitoring_date <= :EndDate");
                    parameters["EndDate"] = endDate.Value;
                }

                var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                var countSql = $"SELECT COUNT(*) FROM HealthMonitoring {whereClause}";
                var totalCount = await _databaseService.QuerySingleAsync<int>(countSql, parameters);

                var sql = $@"
                    SELECT monitoring_id as MonitoringId, elderly_id as ElderlyId, monitoring_date as MonitoringDate,
                           heart_rate as HeartRate, blood_pressure_high as BloodPressureHigh, 
                           blood_pressure_low as BloodPressureLow, blood_sugar as BloodSugar,
                           body_temperature as BodyTemperature, activity_level as ActivityLevel, notes as Notes
                    FROM HealthMonitoring
                    {whereClause}
                    ORDER BY monitoring_date DESC
                    OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY";

                var healthRecords = await _databaseService.QueryAsync<HealthDataDetailDto>(sql, parameters);

                var result = new PagedResult<HealthDataDetailDto>
                {
                    Items = healthRecords,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                return new ApiResponse<PagedResult<HealthDataDetailDto>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取健康监测记录列表失败");
                return new ApiResponse<PagedResult<HealthDataDetailDto>>
                {
                    Success = false,
                    Message = $"获取健康监测记录列表失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 根据ID获取健康监测记录详情
        /// </summary>
        public async Task<ApiResponse<HealthDataDetailDto>> GetHealthMonitoringByIdAsync(int monitoringId)
        {
            try
            {
                var sql = @"
                    SELECT monitoring_id as MonitoringId, elderly_id as ElderlyId, monitoring_date as MonitoringDate,
                           heart_rate as HeartRate, blood_pressure_high as BloodPressureHigh, 
                           blood_pressure_low as BloodPressureLow, blood_sugar as BloodSugar,
                           body_temperature as BodyTemperature, activity_level as ActivityLevel, notes as Notes
                    FROM HealthMonitoring
                    WHERE monitoring_id = :MonitoringId";

                var healthRecord = await _databaseService.QuerySingleOrDefaultAsync<HealthDataDetailDto>(sql, new { MonitoringId = monitoringId });
                
                if (healthRecord == null)
                {
                    return new ApiResponse<HealthDataDetailDto>
                    {
                        Success = false,
                        Message = "健康监测记录不存在"
                    };
                }

                return new ApiResponse<HealthDataDetailDto>
                {
                    Success = true,
                    Data = healthRecord
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取健康监测记录详情失败");
                return new ApiResponse<HealthDataDetailDto>
                {
                    Success = false,
                    Message = $"获取健康监测记录详情失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 删除健康监测记录
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteHealthMonitoringAsync(int monitoringId)
        {
            try
            {
                var sql = "DELETE FROM HealthMonitoring WHERE monitoring_id = :MonitoringId";
                var rowsAffected = await _databaseService.ExecuteAsync(sql, new { MonitoringId = monitoringId });
                
                if (rowsAffected == 0)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "健康监测记录不存在"
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "健康监测记录删除成功",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除健康监测记录失败");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"删除健康监测记录失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取健康数据统计信息
        /// </summary>
        public async Task<ApiResponse<object>> GetHealthDataStatisticsAsync(int? elderlyId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var whereConditions = new List<string>();
                var parameters = new Dictionary<string, object>();

                if (elderlyId.HasValue)
                {
                    whereConditions.Add("elderly_id = :ElderlyId");
                    parameters["ElderlyId"] = elderlyId.Value;
                }

                if (startDate.HasValue)
                {
                    whereConditions.Add("monitoring_date >= :StartDate");
                    parameters["StartDate"] = startDate.Value;
                }

                if (endDate.HasValue)
                {
                    whereConditions.Add("monitoring_date <= :EndDate");
                    parameters["EndDate"] = endDate.Value;
                }

                var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                var statisticsQuery = $@"
                    SELECT 
                        COUNT(*) as TotalRecords,
                        AVG(heart_rate) as AvgHeartRate,
                        AVG(blood_pressure_high) as AvgBloodPressureHigh,
                        AVG(blood_pressure_low) as AvgBloodPressureLow,
                        AVG(blood_sugar) as AvgBloodSugar,
                        AVG(body_temperature) as AvgBodyTemperature,
                        AVG(activity_level) as AvgActivityLevel
                    FROM HealthMonitoring {whereClause}";

                var result = await _databaseService.QuerySingleAsync<object>(statisticsQuery, parameters);

                return new ApiResponse<object>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取健康数据统计失败");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取健康数据统计失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取健康数据（与GetHealthMonitoringAsync相同，为控制器提供兼容性）
        /// </summary>
        public async Task<ApiResponse<PagedResult<HealthDataDetailDto>>> GetHealthDataAsync(
            PagedRequest request, 
            int? elderlyId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            return await GetHealthMonitoringAsync(request, elderlyId, startDate, endDate);
        }

        /// <summary>
        /// 创建健康数据（与CreateHealthMonitoringAsync相同，为控制器提供兼容性）
        /// </summary>
        public async Task<ApiResponse<HealthDataCreateDto>> CreateHealthDataAsync(HealthDataCreateDto healthDto)
        {
            return await CreateHealthMonitoringAsync(healthDto);
        }

        /// <summary>
        /// 删除健康数据（与DeleteHealthMonitoringAsync相同，为控制器提供兼容性）
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteHealthDataAsync(int id)
        {
            return await DeleteHealthMonitoringAsync(id);
        }
    }
}
