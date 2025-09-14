using RoomDeviceManagement.DTOs;
using RoomDeviceManagement.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 房间入住管理服务 - 中文兼容版本
    /// 负责处理入住登记、退房登记、账单生成等核心业务
    /// </summary>
    public class RoomOccupancyService
    {
        private readonly ChineseCompatibleDatabaseService _chineseDbService;
        private readonly ILogger<RoomOccupancyService> _logger;
        
        // 使用与ChineseCompatibleDatabaseService相同的连接字符串
        private const string ConnectionString = "Data Source=47.96.238.102:1521/orcl;User Id=application_user;Password=20252025;Connection Timeout=30;Pooling=false;";

        public RoomOccupancyService(
            ChineseCompatibleDatabaseService chineseDbService, 
            ILogger<RoomOccupancyService> logger)
        {
            _chineseDbService = chineseDbService;
            _logger = logger;
        }

        /// <summary>
        /// 根据老人ID获取入住记录
        /// </summary>
        public async Task<ApiResponse<List<OccupancyRecordDto>>> GetOccupancyRecordsByElderlyIdAsync(decimal elderlyId)
        {
            try
            {
                _logger.LogInformation($"🔍 获取老人ID={elderlyId}的入住记录");

                var records = new List<OccupancyRecordDto>();

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        ro.occupancy_id,
                        ro.room_id,
                        ro.elderly_id,
                        rm.room_number,
                        ei.name as elderly_name,
                        ro.check_in_date,
                        ro.check_out_date,
                        ro.status,
                        ro.bed_number,
                        ro.remarks,
                        ro.created_date,
                        ro.updated_date
                    FROM RoomOccupancy ro
                    LEFT JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    LEFT JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                    WHERE ro.elderly_id = :elderlyId
                    ORDER BY ro.check_in_date DESC";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = elderlyId;

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    records.Add(new OccupancyRecordDto
                    {
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        RoomId = reader.GetInt32("room_id"),
                        ElderlyId = reader.GetDecimal("elderly_id"),
                        RoomNumber = reader.IsDBNull("room_number") ? "" : reader.GetString("room_number"),
                        ElderlyName = reader.IsDBNull("elderly_name") ? "" : reader.GetString("elderly_name"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.IsDBNull("check_out_date") ? null : reader.GetDateTime("check_out_date"),
                        Status = reader.GetString("status"),
                        BedNumber = reader.IsDBNull("bed_number") ? null : reader.GetString("bed_number"),
                        Remarks = reader.IsDBNull("remarks") ? null : reader.GetString("remarks"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date")
                    });
                }

                _logger.LogInformation($"✅ 成功获取到 {records.Count} 条入住记录");
                return new ApiResponse<List<OccupancyRecordDto>>
                {
                    Success = true,
                    Message = $"成功获取到 {records.Count} 条入住记录",
                    Data = records
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
        /// 办理入住登记
        /// </summary>
        public async Task<ApiResponse<OccupancyRecordDto>> CheckInAsync(CheckInDto checkInDto)
        {
            try
            {
                _logger.LogInformation($"🏠 办理入住登记: 老人ID={checkInDto.ElderlyId}, 房间ID={checkInDto.RoomId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 检查房间是否可用
                var roomCheckSql = "SELECT status, capacity FROM RoomManagement WHERE room_id = :roomId";
                using var roomCheckCmd = new OracleCommand(roomCheckSql, connection);
                roomCheckCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = checkInDto.RoomId;
                
                using var roomReader = await roomCheckCmd.ExecuteReaderAsync();
                if (!await roomReader.ReadAsync())
                {
                    return new ApiResponse<OccupancyRecordDto>
                    {
                        Success = false,
                        Message = "房间不存在",
                        Data = null
                    };
                }

                var roomStatus = roomReader.GetString("status");
                if (roomStatus != "空闲" && roomStatus != "可用")
                {
                    return new ApiResponse<OccupancyRecordDto>
                    {
                        Success = false,
                        Message = $"房间状态为 {roomStatus}，无法入住",
                        Data = null
                    };
                }
                roomReader.Close();

                // 检查老人是否已有未退房的记录
                var existingCheckSql = @"
                    SELECT COUNT(*) as count 
                    FROM RoomOccupancy 
                    WHERE elderly_id = :elderlyId AND status = '入住中'";
                
                using var existingCmd = new OracleCommand(existingCheckSql, connection);
                existingCmd.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = checkInDto.ElderlyId;
                
                var existingCount = Convert.ToInt32(await existingCmd.ExecuteScalarAsync());
                if (existingCount > 0)
                {
                    return new ApiResponse<OccupancyRecordDto>
                    {
                        Success = false,
                        Message = "该老人已有未退房的入住记录",
                        Data = null
                    };
                }

                // 插入入住记录
                var insertSql = @"
                    INSERT INTO RoomOccupancy (
                        room_id, elderly_id, check_in_date, status, 
                        bed_number, remarks, created_date, updated_date
                    ) VALUES (
                        :roomId, :elderlyId, :checkInDate, '入住中',
                        :bedNumber, :remarks, SYSDATE, SYSDATE
                    ) RETURNING occupancy_id INTO :occupancyId";

                using var insertCmd = new OracleCommand(insertSql, connection);
                insertCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = checkInDto.RoomId;
                insertCmd.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = checkInDto.ElderlyId;
                insertCmd.Parameters.Add(":checkInDate", OracleDbType.Date).Value = checkInDto.CheckInDate;
                insertCmd.Parameters.Add(":bedNumber", OracleDbType.Varchar2).Value = (object?)checkInDto.BedNumber ?? DBNull.Value;
                insertCmd.Parameters.Add(":remarks", OracleDbType.Varchar2).Value = (object?)checkInDto.Remarks ?? DBNull.Value;
                
                var occupancyIdParam = new OracleParameter(":occupancyId", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                insertCmd.Parameters.Add(occupancyIdParam);

                await insertCmd.ExecuteNonQueryAsync();
                var occupancyId = ((OracleDecimal)occupancyIdParam.Value).ToInt32();

                // 更新房间状态
                var updateRoomSql = "UPDATE RoomManagement SET status = '入住', updated_at = SYSDATE WHERE room_id = :roomId";
                using var updateCmd = new OracleCommand(updateRoomSql, connection);
                updateCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = checkInDto.RoomId;
                await updateCmd.ExecuteNonQueryAsync();

                _logger.LogInformation($"✅ 入住登记成功，入住记录ID={occupancyId}");

                // 返回创建的记录
                var result = new OccupancyRecordDto
                {
                    OccupancyId = occupancyId,
                    RoomId = checkInDto.RoomId,
                    ElderlyId = checkInDto.ElderlyId,
                    CheckInDate = checkInDto.CheckInDate,
                    Status = "入住中",
                    BedNumber = checkInDto.BedNumber,
                    Remarks = checkInDto.Remarks,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
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

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 获取入住记录信息
                var selectSql = @"
                    SELECT room_id, elderly_id, status 
                    FROM RoomOccupancy 
                    WHERE occupancy_id = :occupancyId";

                using var selectCmd = new OracleCommand(selectSql, connection);
                selectCmd.Parameters.Add(":occupancyId", OracleDbType.Int32).Value = checkOutDto.OccupancyId;

                using var reader = await selectCmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return new ApiResponse<OccupancyRecordDto>
                    {
                        Success = false,
                        Message = "入住记录不存在",
                        Data = null
                    };
                }

                var roomId = reader.GetInt32("room_id");
                var elderlyId = reader.GetDecimal("elderly_id");
                var currentStatus = reader.GetString("status");

                if (currentStatus == "已退房")
                {
                    return new ApiResponse<OccupancyRecordDto>
                    {
                        Success = false,
                        Message = "该记录已办理退房",
                        Data = null
                    };
                }
                reader.Close();

                // 更新入住记录
                var updateSql = @"
                    UPDATE RoomOccupancy 
                    SET check_out_date = :checkOutDate, 
                        status = '已退房',
                        remarks = :remarks,
                        updated_date = SYSDATE
                    WHERE occupancy_id = :occupancyId";

                using var updateCmd = new OracleCommand(updateSql, connection);
                updateCmd.Parameters.Add(":checkOutDate", OracleDbType.Date).Value = checkOutDto.CheckOutDate;
                updateCmd.Parameters.Add(":remarks", OracleDbType.Varchar2).Value = (object?)checkOutDto.Remarks ?? DBNull.Value;
                updateCmd.Parameters.Add(":occupancyId", OracleDbType.Int32).Value = checkOutDto.OccupancyId;

                await updateCmd.ExecuteNonQueryAsync();

                // 检查房间是否还有其他入住者
                var countSql = @"
                    SELECT COUNT(*) as count 
                    FROM RoomOccupancy 
                    WHERE room_id = :roomId AND status = '入住中'";

                using var countCmd = new OracleCommand(countSql, connection);
                countCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = roomId;

                var remainingCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                // 如果没有其他入住者，更新房间状态为空闲
                if (remainingCount == 0)
                {
                    var updateRoomSql = "UPDATE RoomManagement SET status = '空闲', updated_at = SYSDATE WHERE room_id = :roomId";
                    using var updateRoomCmd = new OracleCommand(updateRoomSql, connection);
                    updateRoomCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = roomId;
                    await updateRoomCmd.ExecuteNonQueryAsync();
                }

                _logger.LogInformation($"✅ 退房登记成功");

                var result = new OccupancyRecordDto
                {
                    OccupancyId = checkOutDto.OccupancyId,
                    RoomId = roomId,
                    ElderlyId = elderlyId,
                    CheckOutDate = checkOutDto.CheckOutDate,
                    Status = "已退房",
                    Remarks = checkOutDto.Remarks,
                    UpdatedDate = DateTime.Now
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
        /// 生成所有房间的月度账单
        /// </summary>
        public async Task<ApiResponse<List<BillingRecordDto>>> GenerateAllBillingsAsync(GenerateBillDto generateDto)
        {
            try
            {
                _logger.LogInformation($"💰 生成所有房间的月度账单: {generateDto.BillingStartDate:yyyy-MM-dd} 到 {generateDto.BillingEndDate:yyyy-MM-dd}");

                var billings = new List<BillingRecordDto>();

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 获取指定时间段内的所有入住记录
                var sql = @"
                    SELECT DISTINCT
                        ro.occupancy_id,
                        ro.elderly_id,
                        ro.room_id,
                        rm.room_number,
                        rm.rate as room_rate,
                        ei.name as elderly_name,
                        ro.check_in_date,
                        ro.check_out_date
                    FROM RoomOccupancy ro
                    LEFT JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    LEFT JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                    WHERE ro.check_in_date <= :endDate 
                      AND (ro.check_out_date IS NULL OR ro.check_out_date >= :startDate)";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(":startDate", OracleDbType.Date).Value = generateDto.BillingStartDate;
                command.Parameters.Add(":endDate", OracleDbType.Date).Value = generateDto.BillingEndDate;

                using var reader = await command.ExecuteReaderAsync();
                var occupancies = new List<dynamic>();

                while (await reader.ReadAsync())
                {
                    occupancies.Add(new
                    {
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        ElderlyId = reader.GetDecimal("elderly_id"),
                        RoomId = reader.GetInt32("room_id"),
                        RoomNumber = reader.IsDBNull("room_number") ? "" : reader.GetString("room_number"),
                        RoomRate = reader.IsDBNull("room_rate") ? 100m : reader.GetDecimal("room_rate"),
                        ElderlyName = reader.IsDBNull("elderly_name") ? "" : reader.GetString("elderly_name"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.IsDBNull("check_out_date") ? (DateTime?)null : reader.GetDateTime("check_out_date")
                    });
                }
                reader.Close();

                // 为每个入住记录生成月度账单
                foreach (var occupancy in occupancies)
                {
                    var monthlyBillings = await GenerateMonthlyBillingsForOccupancy(connection, occupancy, generateDto);
                    billings.AddRange(monthlyBillings);
                }

                _logger.LogInformation($"✅ 成功生成 {billings.Count} 条月度账单记录");

                return new ApiResponse<List<BillingRecordDto>>
                {
                    Success = true,
                    Message = $"成功生成 {billings.Count} 条月度账单记录",
                    Data = billings
                };
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
        /// 根据老人ID生成月度账单
        /// </summary>
        public async Task<ApiResponse<List<BillingRecordDto>>> GenerateBillingsForElderlyAsync(decimal elderlyId, GenerateBillDto generateDto)
        {
            try
            {
                _logger.LogInformation($"💰 为老人ID={elderlyId}生成月度账单: {generateDto.BillingStartDate:yyyy-MM-dd} 到 {generateDto.BillingEndDate:yyyy-MM-dd}");

                var billings = new List<BillingRecordDto>();

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                // 获取指定老人在指定时间段内的入住记录
                var sql = @"
                    SELECT 
                        ro.occupancy_id,
                        ro.elderly_id,
                        ro.room_id,
                        rm.room_number,
                        rm.rate as room_rate,
                        ei.name as elderly_name,
                        ro.check_in_date,
                        ro.check_out_date
                    FROM RoomOccupancy ro
                    LEFT JOIN RoomManagement rm ON ro.room_id = rm.room_id
                    LEFT JOIN ElderlyInfo ei ON ro.elderly_id = ei.elderly_id
                    WHERE ro.elderly_id = :elderlyId
                      AND ro.check_in_date <= :endDate 
                      AND (ro.check_out_date IS NULL OR ro.check_out_date >= :startDate)";

                using var command = new OracleCommand(sql, connection);
                command.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = elderlyId;
                command.Parameters.Add(":startDate", OracleDbType.Date).Value = generateDto.BillingStartDate;
                command.Parameters.Add(":endDate", OracleDbType.Date).Value = generateDto.BillingEndDate;

                using var reader = await command.ExecuteReaderAsync();
                var occupancies = new List<dynamic>();

                while (await reader.ReadAsync())
                {
                    occupancies.Add(new
                    {
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        ElderlyId = reader.GetDecimal("elderly_id"),
                        RoomId = reader.GetInt32("room_id"),
                        RoomNumber = reader.IsDBNull("room_number") ? "" : reader.GetString("room_number"),
                        RoomRate = reader.IsDBNull("room_rate") ? 100m : reader.GetDecimal("room_rate"),
                        ElderlyName = reader.IsDBNull("elderly_name") ? "" : reader.GetString("elderly_name"),
                        CheckInDate = reader.GetDateTime("check_in_date"),
                        CheckOutDate = reader.IsDBNull("check_out_date") ? (DateTime?)null : reader.GetDateTime("check_out_date")
                    });
                }
                reader.Close();

                // 为每个入住记录生成月度账单
                foreach (var occupancy in occupancies)
                {
                    var monthlyBillings = await GenerateMonthlyBillingsForOccupancy(connection, occupancy, generateDto);
                    billings.AddRange(monthlyBillings);
                }

                _logger.LogInformation($"✅ 成功为老人ID={elderlyId}生成 {billings.Count} 条月度账单记录");

                return new ApiResponse<List<BillingRecordDto>>
                {
                    Success = true,
                    Message = $"成功生成 {billings.Count} 条月度账单记录",
                    Data = billings
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 生成老人ID={elderlyId}的月度账单失败");
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
                _logger.LogInformation($"🔍 获取账单记录，老人ID筛选={elderlyId}");

                using var connection = new OracleConnection(ConnectionString);
                await connection.OpenAsync();

                var whereClause = elderlyId.HasValue ? "WHERE rb.elderly_id = :elderlyId" : "";
                var countSql = $@"
                    SELECT COUNT(*) as total_count
                    FROM RoomBilling rb
                    {whereClause}";

                var dataSql = $@"
                    SELECT * FROM (
                        SELECT rb.*, rm.room_number, ei.name as elderly_name,
                               ROW_NUMBER() OVER (ORDER BY rb.billing_date DESC) as rn
                        FROM RoomBilling rb
                        LEFT JOIN RoomManagement rm ON rb.room_id = rm.room_id
                        LEFT JOIN ElderlyInfo ei ON rb.elderly_id = ei.elderly_id
                        {whereClause}
                    ) WHERE rn BETWEEN :startRow AND :endRow";

                // 获取总数
                using var countCmd = new OracleCommand(countSql, connection);
                if (elderlyId.HasValue)
                {
                    countCmd.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = elderlyId.Value;
                }

                var totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                // 获取分页数据
                var startRow = (request.Page - 1) * request.PageSize + 1;
                var endRow = request.Page * request.PageSize;

                using var dataCmd = new OracleCommand(dataSql, connection);
                if (elderlyId.HasValue)
                {
                    dataCmd.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = elderlyId.Value;
                }
                dataCmd.Parameters.Add(":startRow", OracleDbType.Int32).Value = startRow;
                dataCmd.Parameters.Add(":endRow", OracleDbType.Int32).Value = endRow;

                var billings = new List<BillingRecordDto>();
                using var reader = await dataCmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    billings.Add(new BillingRecordDto
                    {
                        BillingId = reader.GetInt32("billing_id"),
                        OccupancyId = reader.GetInt32("occupancy_id"),
                        ElderlyId = reader.GetDecimal("elderly_id"),
                        ElderlyName = reader.IsDBNull("elderly_name") ? "" : reader.GetString("elderly_name"),
                        RoomId = reader.GetInt32("room_id"),
                        RoomNumber = reader.IsDBNull("room_number") ? "" : reader.GetString("room_number"),
                        BillingStartDate = reader.GetDateTime("billing_start_date"),
                        BillingEndDate = reader.GetDateTime("billing_end_date"),
                        Days = reader.GetInt32("days"),
                        DailyRate = reader.GetDecimal("daily_rate"),
                        TotalAmount = reader.GetDecimal("total_amount"),
                        PaymentStatus = reader.GetString("payment_status"),
                        PaidAmount = reader.GetDecimal("paid_amount"),
                        UnpaidAmount = reader.GetDecimal("unpaid_amount"),
                        BillingDate = reader.GetDateTime("billing_date"),
                        PaymentDate = reader.IsDBNull("payment_date") ? null : reader.GetDateTime("payment_date"),
                        Remarks = reader.IsDBNull("remarks") ? null : reader.GetString("remarks"),
                        CreatedDate = reader.GetDateTime("created_date"),
                        UpdatedDate = reader.GetDateTime("updated_date")
                    });
                }

                var result = new PagedResult<BillingRecordDto>
                {
                    Items = billings,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                return new ApiResponse<PagedResult<BillingRecordDto>>
                {
                    Success = true,
                    Message = $"成功获取 {billings.Count} 条账单记录",
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

        /// <summary>
        /// 为单个入住记录生成月度账单
        /// </summary>
        private async Task<List<BillingRecordDto>> GenerateMonthlyBillingsForOccupancy(OracleConnection connection, dynamic occupancy, GenerateBillDto generateDto)
        {
            try
            {
                _logger.LogInformation($"🗓️ 为入住记录ID={occupancy.OccupancyId}生成月度账单");

                var billings = new List<BillingRecordDto>();
                var dailyRate = generateDto.DailyRate ?? occupancy.RoomRate;

                // 获取月度计费时间段
                var monthlyPeriods = GetMonthlyBillingPeriods(
                    occupancy.CheckInDate, 
                    occupancy.CheckOutDate, 
                    generateDto.BillingStartDate, 
                    generateDto.BillingEndDate
                );

                foreach (var period in monthlyPeriods)
                {
                    // 检查该月是否已存在账单
                    if (await IsMonthlyBillingExists(connection, occupancy.OccupancyId, period.Year, period.Month))
                    {
                        _logger.LogInformation($"入住记录ID={occupancy.OccupancyId}在{period.Year}年{period.Month}月的账单已存在，跳过");
                        continue;
                    }

                    // 创建月度账单
                    var billing = await CreateMonthlyBilling(connection, occupancy, period, dailyRate, generateDto.Remarks);
                    if (billing != null)
                    {
                        billings.Add(billing);
                        _logger.LogInformation($"✅ 成功生成{period.Year}年{period.Month}月账单，金额={billing.TotalAmount}");
                    }
                }

                return billings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"为入住记录 {occupancy.OccupancyId} 生成月度账单失败");
                return new List<BillingRecordDto>();
            }
        }

        /// <summary>
        /// 获取月度计费时间段
        /// </summary>
        private List<MonthlyBillingPeriod> GetMonthlyBillingPeriods(DateTime checkInDate, DateTime? checkOutDate, DateTime requestStartDate, DateTime requestEndDate)
        {
            var periods = new List<MonthlyBillingPeriod>();
            
            // 确定实际计费的开始和结束时间
            var actualStartDate = checkInDate > requestStartDate ? checkInDate : requestStartDate;
            var actualEndDate = checkOutDate.HasValue ? 
                (checkOutDate.Value < requestEndDate ? checkOutDate.Value : requestEndDate) : 
                requestEndDate;

            if (actualStartDate > actualEndDate)
            {
                return periods; // 无有效计费时间段
            }

            // 按月分割时间段
            var currentMonth = new DateTime(actualStartDate.Year, actualStartDate.Month, 1);
            
            while (currentMonth <= actualEndDate)
            {
                var monthStart = currentMonth;
                var monthEnd = monthStart.AddMonths(1).AddDays(-1); // 该月最后一天

                // 计算该月的实际计费时间段
                var billingStart = monthStart < actualStartDate ? actualStartDate : monthStart;
                var billingEnd = monthEnd > actualEndDate ? actualEndDate : monthEnd;

                if (billingStart <= billingEnd)
                {
                    periods.Add(new MonthlyBillingPeriod
                    {
                        Year = currentMonth.Year,
                        Month = currentMonth.Month,
                        StartDate = billingStart,
                        EndDate = monthEnd, // 总是到月底，这是关键！
                        ActualStartDate = billingStart,
                        ActualEndDate = billingEnd
                    });
                }

                currentMonth = currentMonth.AddMonths(1);
            }

            return periods;
        }

        /// <summary>
        /// 检查月度账单是否已存在
        /// </summary>
        private async Task<bool> IsMonthlyBillingExists(OracleConnection connection, int occupancyId, int year, int month)
        {
            var sql = @"
                SELECT COUNT(*) as count 
                FROM RoomBilling 
                WHERE occupancy_id = :occupancyId 
                  AND EXTRACT(YEAR FROM billing_start_date) = :year
                  AND EXTRACT(MONTH FROM billing_start_date) = :month";

            using var command = new OracleCommand(sql, connection);
            command.Parameters.Add(":occupancyId", OracleDbType.Int32).Value = occupancyId;
            command.Parameters.Add(":year", OracleDbType.Int32).Value = year;
            command.Parameters.Add(":month", OracleDbType.Int32).Value = month;

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        /// <summary>
        /// 创建月度账单
        /// </summary>
        private async Task<BillingRecordDto?> CreateMonthlyBilling(
            OracleConnection connection, 
            dynamic occupancy, 
            MonthlyBillingPeriod period, 
            decimal dailyRate, 
            string? remarks)
        {
            try
            {
                // 计算天数和总金额（从实际入住日期到月底）
                var days = (period.EndDate - period.ActualStartDate).Days + 1;
                var totalAmount = days * dailyRate;

                // 插入账单记录
                var insertSql = @"
                    INSERT INTO RoomBilling (
                        occupancy_id, elderly_id, room_id, billing_start_date, billing_end_date,
                        days, daily_rate, total_amount, payment_status, paid_amount, unpaid_amount,
                        billing_date, remarks, created_date, updated_date
                    ) VALUES (
                        :occupancyId, :elderlyId, :roomId, :startDate, :endDate,
                        :days, :dailyRate, :totalAmount, '未支付', 0, :unpaidAmount,
                        SYSDATE, :remarks, SYSDATE, SYSDATE
                    ) RETURNING billing_id INTO :billingId";

                using var insertCmd = new OracleCommand(insertSql, connection);
                insertCmd.Parameters.Add(":occupancyId", OracleDbType.Int32).Value = occupancy.OccupancyId;
                insertCmd.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = occupancy.ElderlyId;
                insertCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = occupancy.RoomId;
                insertCmd.Parameters.Add(":startDate", OracleDbType.Date).Value = period.ActualStartDate;
                insertCmd.Parameters.Add(":endDate", OracleDbType.Date).Value = period.EndDate;
                insertCmd.Parameters.Add(":days", OracleDbType.Int32).Value = days;
                insertCmd.Parameters.Add(":dailyRate", OracleDbType.Decimal).Value = dailyRate;
                insertCmd.Parameters.Add(":totalAmount", OracleDbType.Decimal).Value = totalAmount;
                insertCmd.Parameters.Add(":unpaidAmount", OracleDbType.Decimal).Value = totalAmount;
                insertCmd.Parameters.Add(":remarks", OracleDbType.Varchar2).Value = (object?)remarks ?? DBNull.Value;

                var billingIdParam = new OracleParameter(":billingId", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                insertCmd.Parameters.Add(billingIdParam);

                await insertCmd.ExecuteNonQueryAsync();
                var billingId = ((OracleDecimal)billingIdParam.Value).ToInt32();

                return new BillingRecordDto
                {
                    BillingId = billingId,
                    OccupancyId = occupancy.OccupancyId,
                    ElderlyId = occupancy.ElderlyId,
                    ElderlyName = occupancy.ElderlyName,
                    RoomId = occupancy.RoomId,
                    RoomNumber = occupancy.RoomNumber,
                    BillingStartDate = period.ActualStartDate,
                    BillingEndDate = period.EndDate,
                    Days = days,
                    DailyRate = dailyRate,
                    TotalAmount = totalAmount,
                    PaymentStatus = "未支付",
                    PaidAmount = 0,
                    UnpaidAmount = totalAmount,
                    BillingDate = DateTime.Now,
                    Remarks = remarks,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建{period.Year}年{period.Month}月账单失败");
                return null;
            }
        }

        /// <summary>
        /// 月度计费时间段类
        /// </summary>
        private class MonthlyBillingPeriod
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime ActualStartDate { get; set; }
            public DateTime ActualEndDate { get; set; }
        }

        /// <summary>
        /// 为单个入住记录生成账单（原有方法，保持兼容性）
        /// </summary>
        private async Task<BillingRecordDto?> GenerateBillingForOccupancy(OracleConnection connection, dynamic occupancy, GenerateBillDto generateDto)
        {
            try
            {
                // 计算实际计费时间段
                var actualStartDate = new DateTime(Math.Max(occupancy.CheckInDate.Ticks, generateDto.BillingStartDate.Ticks));
                var actualEndDate = occupancy.CheckOutDate?.Ticks != null 
                    ? new DateTime(Math.Min(occupancy.CheckOutDate.Value.Ticks, generateDto.BillingEndDate.Ticks))
                    : generateDto.BillingEndDate;

                if (actualStartDate >= actualEndDate)
                {
                    return null; // 无有效计费时间段
                }

                var days = (actualEndDate - actualStartDate).Days + 1;
                var dailyRate = generateDto.DailyRate ?? occupancy.RoomRate;
                var totalAmount = days * dailyRate;

                // 检查是否已存在相同时间段的账单
                var existingSql = @"
                    SELECT COUNT(*) as count 
                    FROM RoomBilling 
                    WHERE occupancy_id = :occupancyId 
                      AND billing_start_date = :startDate 
                      AND billing_end_date = :endDate";

                using var existingCmd = new OracleCommand(existingSql, connection);
                existingCmd.Parameters.Add(":occupancyId", OracleDbType.Int32).Value = occupancy.OccupancyId;
                existingCmd.Parameters.Add(":startDate", OracleDbType.Date).Value = actualStartDate;
                existingCmd.Parameters.Add(":endDate", OracleDbType.Date).Value = actualEndDate;

                var existingCount = Convert.ToInt32(await existingCmd.ExecuteScalarAsync());
                if (existingCount > 0)
                {
                    return null; // 已存在账单，跳过
                }

                // 插入账单记录
                var insertSql = @"
                    INSERT INTO RoomBilling (
                        occupancy_id, elderly_id, room_id, billing_start_date, billing_end_date,
                        days, daily_rate, total_amount, payment_status, paid_amount, unpaid_amount,
                        billing_date, remarks, created_date, updated_date
                    ) VALUES (
                        :occupancyId, :elderlyId, :roomId, :startDate, :endDate,
                        :days, :dailyRate, :totalAmount, '未支付', 0, :totalAmount,
                        SYSDATE, :remarks, SYSDATE, SYSDATE
                    ) RETURNING billing_id INTO :billingId";

                using var insertCmd = new OracleCommand(insertSql, connection);
                insertCmd.Parameters.Add(":occupancyId", OracleDbType.Int32).Value = occupancy.OccupancyId;
                insertCmd.Parameters.Add(":elderlyId", OracleDbType.Decimal).Value = occupancy.ElderlyId;
                insertCmd.Parameters.Add(":roomId", OracleDbType.Int32).Value = occupancy.RoomId;
                insertCmd.Parameters.Add(":startDate", OracleDbType.Date).Value = actualStartDate;
                insertCmd.Parameters.Add(":endDate", OracleDbType.Date).Value = actualEndDate;
                insertCmd.Parameters.Add(":days", OracleDbType.Int32).Value = days;
                insertCmd.Parameters.Add(":dailyRate", OracleDbType.Decimal).Value = dailyRate;
                insertCmd.Parameters.Add(":totalAmount", OracleDbType.Decimal).Value = totalAmount;
                insertCmd.Parameters.Add(":remarks", OracleDbType.Varchar2).Value = (object?)generateDto.Remarks ?? DBNull.Value;

                var billingIdParam = new OracleParameter(":billingId", OracleDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                insertCmd.Parameters.Add(billingIdParam);

                await insertCmd.ExecuteNonQueryAsync();
                var billingId = ((OracleDecimal)billingIdParam.Value).ToInt32();

                return new BillingRecordDto
                {
                    BillingId = billingId,
                    OccupancyId = occupancy.OccupancyId,
                    ElderlyId = occupancy.ElderlyId,
                    ElderlyName = occupancy.ElderlyName,
                    RoomId = occupancy.RoomId,
                    RoomNumber = occupancy.RoomNumber,
                    BillingStartDate = actualStartDate,
                    BillingEndDate = actualEndDate,
                    Days = days,
                    DailyRate = dailyRate,
                    TotalAmount = totalAmount,
                    PaymentStatus = "未支付",
                    PaidAmount = 0,
                    UnpaidAmount = totalAmount,
                    BillingDate = DateTime.Now,
                    Remarks = generateDto.Remarks,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"为入住记录 {occupancy.OccupancyId} 生成账单失败");
                return null;
            }
        }
    }
}
