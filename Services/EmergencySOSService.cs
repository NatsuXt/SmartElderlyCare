using Staff_Info.Data;
using Staff_Info.DTOs;
using Staff_Info.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Staff_Info.Services
{
    public interface IEmergencySOSService
    {
        Task<decimal> CreateSOSRecordAsync(EmergencySOSCreateDto dto);
        Task<bool> AcceptSOSAsync(EmergencySOSAcceptDto dto);
        Task<bool> CompleteSOSAsync(EmergencySOSCompleteDto dto);
       // Task<List<SOSNotificationDto>> GetActiveSOSNotificationsAsync(decimal staffId);
    }

    public class EmergencySOSService : IEmergencySOSService
    {
        private readonly StaffInfoDbContext _context;
        private readonly ILogger<EmergencySOSService> _logger;

        public EmergencySOSService(
            StaffInfoDbContext context,
            ILogger<EmergencySOSService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<decimal> CreateSOSRecordAsync(EmergencySOSCreateDto dto)
        {
            try
            {
                // 修改验证方式 - 使用Oracle兼容的查询
                var elderlyExists = await _context.ElderlyInfos
                    .Where(e => e.ELDERLY_ID == dto.ElderlyId)
                    .Select(e => 1)
                    .FirstOrDefaultAsync() == 1;

                if (!elderlyExists)
                {
                    _logger.LogWarning($"尝试为不存在的老人创建SOS记录: {dto.ElderlyId}");
                    throw new ArgumentException("指定的老人不存在");
                }

                // 获取下一个ID
                var nextId = await _context.EmergencySOS
                    .MaxAsync(e => (decimal?)e.CALL_ID) ?? 0;
                nextId += 1;

                // 创建SOS记录
                var sosRecord = new EMERGENCYSOS
                {
                    CALL_ID = nextId,
                    ELDERLY_ID = dto.ElderlyId,
                    CALL_TYPE = dto.CallType,
                    ROOM_ID = dto.RoomId,
                    CALL_TIME = DateTime.Now,
                    CALL_STATUS = "Pending",
                    HANDLING_RESULT = "等待响应",
                    FOLLOW_UP_REQUIRED = true
                };

                _context.EmergencySOS.Add(sosRecord);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"已创建SOS记录: {sosRecord.CALL_ID}");
                return sosRecord.CALL_ID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建SOS记录时发生错误");
                throw;
            }
        }

        public async Task<bool> AcceptSOSAsync(EmergencySOSAcceptDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 使用Oracle兼容的查询方式检查SOS记录
                var sosRecord = await _context.EmergencySOS
                    .FromSqlRaw(
                        @"SELECT * FROM EMERGENCYSOS 
                WHERE CALL_ID = :callId AND CALL_STATUS = 'Pending'",
                        new OracleParameter("callId", dto.CallId))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (sosRecord == null)
                {
                    _logger.LogWarning($"未找到待处理的SOS记录: {dto.CallId}");
                    return false;
                }

                // Oracle兼容的员工存在性检查（使用COUNT而不是布尔值）
                var staffExists = await _context.StaffInfos
                    .Where(s => s.STAFF_ID == dto.ResponseStaffId)
                    .CountAsync() > 0;

                if (!staffExists)
                {
                    _logger.LogWarning($"无效的员工ID: {dto.ResponseStaffId}");
                    return false;
                }

                // 使用原生SQL执行更新（避免LINQ转换问题）
                var updateSql = @"
            UPDATE EMERGENCYSOS 
            SET RESPONSE_STAFF_ID = :staffId,
                RESPONSE_TIME = :responseTime,
                CALL_STATUS = 'InProgress',
                HANDLING_RESULT = '处理中'
            WHERE CALL_ID = :callId AND CALL_STATUS = 'Pending'";

                var parameters = new[]
                {
                    new OracleParameter("staffId", dto.ResponseStaffId),
                    new OracleParameter("responseTime", DateTime.Now),
                    new OracleParameter("callId", dto.CallId)
                };

                var updatedRows = await _context.Database.ExecuteSqlRawAsync(updateSql, parameters);

                if (updatedRows == 0)
                {
                    await transaction.RollbackAsync();
                    _logger.LogWarning($"更新SOS记录失败，可能已被其他进程修改: {dto.CallId}");
                    return false;
                }

                await transaction.CommitAsync();
                _logger.LogInformation($"SOS呼叫 {dto.CallId} 已由员工 {dto.ResponseStaffId} 接单");
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"并发冲突: SOS记录 {dto.CallId} 已被其他进程修改");
                return false;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"数据库更新失败: {dto.CallId}");
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"处理SOS呼叫时发生意外错误: {dto.CallId}");
                throw;
            }
        }

        public async Task<bool> CompleteSOSAsync(EmergencySOSCompleteDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 查找SOS记录
                var sosRecord = await _context.EmergencySOS
                    .FirstOrDefaultAsync(e => e.CALL_ID == dto.CallId &&
                                              e.CALL_STATUS == "InProgress" &&
                                              e.RESPONSE_STAFF_ID == dto.StaffId);

                if (sosRecord == null)
                {
                    _logger.LogWarning($"尝试完成不存在的或非InProgress状态的SOS呼叫: {dto.CallId}");
                    return false;
                }

                // 更新SOS记录
                sosRecord.CALL_STATUS = "Completed";
                sosRecord.HANDLING_RESULT = dto.HandlingResult;
                sosRecord.FOLLOW_UP_REQUIRED = false;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"员工 {dto.StaffId} 已完成SOS呼叫 {dto.CallId}");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"完成SOS呼叫时发生错误: {dto.CallId}");
                throw;
            }
        }
    }
}
