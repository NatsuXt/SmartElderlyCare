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
        /// 删除老人信息
        /// </summary>
        public async Task<bool> DeleteElderlyAsync(int elderlyId)
        {
            var elderly = await _context.ElderlyInfos.FindAsync(elderlyId);
            if (elderly == null) return false;

            // 删除所有相关信息
            _context.FamilyInfos.RemoveRange(_context.FamilyInfos.Where(f => f.ElderlyId == elderlyId));
            _context.HealthMonitorings.RemoveRange(_context.HealthMonitorings.Where(h => h.ElderlyId == elderlyId));
            _context.HealthAssessmentReports.RemoveRange(_context.HealthAssessmentReports.Where(h => h.ElderlyId == elderlyId));
            _context.MedicalOrders.RemoveRange(_context.MedicalOrders.Where(m => m.ElderlyId == elderlyId));
            _context.NursingPlans.RemoveRange(_context.NursingPlans.Where(n => n.ElderlyId == elderlyId));
            _context.FeeSettlements.RemoveRange(_context.FeeSettlements.Where(f => f.ElderlyId == elderlyId));
            _context.ActivityParticipations.RemoveRange(_context.ActivityParticipations.Where(a => a.ElderlyId == elderlyId));
            _context.DietRecommendations.RemoveRange(_context.DietRecommendations.Where(d => d.ElderlyId == elderlyId));
            _context.EmergencySOSs.RemoveRange(_context.EmergencySOSs.Where(e => e.ElderlyId == elderlyId));
            _context.HealthAlerts.RemoveRange(_context.HealthAlerts.Where(h => h.ElderlyId == elderlyId));
            _context.HealthThresholds.RemoveRange(_context.HealthThresholds.Where(h => h.ElderlyId == elderlyId));
            _context.VoiceAssistantReminders.RemoveRange(_context.VoiceAssistantReminders.Where(v => v.ElderlyId == elderlyId));

            // 删除老人自身
            _context.ElderlyInfos.Remove(elderly);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
