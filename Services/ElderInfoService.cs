using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ElderlyCareSystem.Services
{
    public class ElderlyInfoService
    {
        private readonly AppDbContext _context;

        public ElderlyInfoService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根据ID获取老人信息
        /// </summary>
        // 根据 ElderlyId 获取老人基本信息
        public async Task<ElderlyInfoDto?> GetElderlyByIdAsync(int elderlyId)
        {
            var elderly = await _context.ElderlyInfos
                .Where(e => e.ElderlyId == elderlyId)
                .Select(e => new ElderlyInfoDto
                {
                    ElderlyId = e.ElderlyId,
                    Name = e.Name,
                    Gender = e.Gender,
                    BirthDate = e.BirthDate,
                    IdCardNumber = e.IdCardNumber,
                    ContactPhone = e.ContactPhone,
                    Address = e.Address,
                    EmergencyContact = e.EmergencyContact
                })
                .SingleOrDefaultAsync();

            return elderly;
        }

        /// <summary>
        /// 获取所有老人信息
        /// </summary>
        public async Task<List<ElderlyInfoDto>> GetAllElderliesAsync()
        {
            return await _context.ElderlyInfos
                .Select(e => new ElderlyInfoDto
                {
                    ElderlyId = e.ElderlyId,
                    Name = e.Name,
                    Gender = e.Gender,
                    BirthDate = e.BirthDate,
                    IdCardNumber = e.IdCardNumber,
                    ContactPhone = e.ContactPhone,
                    Address = e.Address,
                    EmergencyContact = e.EmergencyContact
                })
                .ToListAsync();
        }

        public async Task<bool> UpdatePropertyAsync(int elderlyId, string propertyName, string value)
        {
            // 只查询 ELDERLYINFO 表，不 Include 任何导航属性
            var elderly = await _context.ElderlyInfos
                .AsNoTracking() // 不追踪整个实体，避免加载导航属性
                .FirstOrDefaultAsync(e => e.ElderlyId == elderlyId);

            if (elderly == null)
                return false;

            var prop = typeof(ElderlyInfo).GetProperty(propertyName);
            if (prop == null)
                return false;

            object? convertedValue = null;
            if (!string.IsNullOrEmpty(value))
                convertedValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            // 创建一个新的实体实例，只更新这一列
            var updateEntity = new ElderlyInfo { ElderlyId = elderlyId };
            prop.SetValue(updateEntity, convertedValue);

            // Attach 并标记指定属性为修改
            _context.Attach(updateEntity);
            _context.Entry(updateEntity).Property(propertyName).IsModified = true;

            await _context.SaveChangesAsync();
            return true;
        }




        /// <summary>
        /// 删除老人信息及其所有相关数据（手动级联删除，支持事务回滚）
        /// </summary>
        public async Task<bool> DeleteElderlyAsync(int elderlyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var elderly = await _context.ElderlyInfos.FindAsync(elderlyId);
                if (elderly == null) return false;

                var occupancyIds = await _context.RoomOccupancys
                    .Where(r => r.ElderlyId == elderlyId)
                    .Select(r => r.OccupancyId)
                    .ToListAsync();

                if (occupancyIds.Any())
                {
                    await _context.RoomBillings
                        .Where(b => occupancyIds.Contains(b.OccupancyId))
                        .ExecuteDeleteAsync();
                }

                // 删除 RoomOccupancy
                await _context.RoomOccupancys
                    .Where(r => r.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                // 删除 VoiceAssistantReminders
                await _context.VoiceAssistantReminders
                    .Where(v => v.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                // 删除 MedicalOrders
                await _context.MedicalOrders
                    .Where(m => m.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                // 删除其他直接依赖 ElderlyInfo 的表
                await _context.ActivityParticipations
                    .Where(a => a.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                await _context.DietRecommendations
                    .Where(d => d.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                await _context.EmergencySOSs
                    .Where(e => e.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                await _context.HealthAlerts
                    .Where(h => h.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                await _context.HealthThresholds
                    .Where(h => h.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                await _context.HealthMonitorings
                    .Where(h => h.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                await _context.HealthAssessmentReports
                    .Where(h => h.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                await _context.NursingPlans
                    .Where(n => n.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                // 删除 FamilyAccount（必须先删子表）
                await _context.FamilyAccounts
                    .Where(fa => _context.FamilyInfos
                        .Where(fi => fi.ElderlyId == elderlyId)
                        .Select(fi => fi.FamilyId)
                        .Contains(fa.FamilyId))
                    .ExecuteDeleteAsync();

                // 删除 FamilyInfo
                await _context.FamilyInfos
                    .Where(f => f.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                // 删除 FeeDetails 与 FeeSettlements
                var feeSettlementIds = await _context.FeeSettlements
                    .Where(f => f.ElderlyId == elderlyId)
                    .Select(f => f.SettlementId)
                    .ToListAsync();

                if (feeSettlementIds.Any())
                {
                    await _context.FeeDetails
                        .Where(fd => feeSettlementIds.Contains(fd.FeeSettlementId))
                        .ExecuteDeleteAsync();

                    await _context.FeeSettlements
                        .Where(f => f.ElderlyId == elderlyId)
                        .ExecuteDeleteAsync();
                }

                // 删除 ElderlyAccounts
                await _context.ElderlyAccounts
                    .Where(a => a.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                // --- 新增删除 FENCELOG ---
                await _context.FenceLogs
                    .Where(f => f.ElderlyId == elderlyId)
                    .ExecuteDeleteAsync();

                // --- 新增删除 VISITORREGISTRATION ---
                await _context.VisitorRegistrations
                    .Where(v => v.elderly_id == elderlyId)
                    .ExecuteDeleteAsync();

                // 删除 ElderlyInfo 本身
                _context.ElderlyInfos.Remove(elderly);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



    }
}
