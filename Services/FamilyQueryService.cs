using System;
using System.Linq;
using System.Threading.Tasks;
using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareSystem.Services
{
    public class FamilyQueryService
    {
        private readonly AppDbContext _context;

        public FamilyQueryService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根据老人ID和查询类型查询信息，只返回对应类型的数据，不返回其他null字段
        /// </summary>
        public async Task<object> QueryByTypeAsync(int elderlyId, string queryType)
        {
            if (queryType == "ElderlyInfo")
            {
                var elderly = await _context.ElderlyInfos
                    .AsNoTracking()
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
                    }).FirstOrDefaultAsync();

                if (elderly == null)
                    throw new ArgumentException("老人信息不存在");

                return new { elderlyInfo = elderly };
            }
            else if (queryType == "HealthMonitoring")
            {
                var list = await _context.HealthMonitorings
                    .AsNoTracking()
                    .Where(h => h.ElderlyId == elderlyId)
                    .OrderByDescending(h => h.MonitoringDate)
                    .Select(h => new HealthMonitoringDto
                    {
                        MonitoringId = h.MonitoringId,
                        ElderlyId = h.ElderlyId,
                        MonitoringDate = h.MonitoringDate,
                        HeartRate = (int)h.HeartRate,
                        BloodPressure = h.BloodPressure,
                        OxygenLevel = (float)h.OxygenLevel,
                        Temperature = (float)h.Temperature,
                        Status = h.Status
                    }).ToListAsync();

                return new { healthMonitorings = list };
            }
            else if (queryType == "FeeSettlement")
            {
                var list = await _context.FeeSettlements
                    .AsNoTracking()
                    .Where(f => f.ElderlyId == elderlyId)
                    .OrderByDescending(f => f.SettlementDate)
                    .Select(f => new FeeSettlementDto
                    {
                        SettlementId = f.SettlementId,
                        ElderlyId = f.ElderlyId,
                        TotalAmount = f.TotalAmount,
                        InsuranceAmount = f.InsuranceAmount,
                        PersonalPayment = f.PersonalPayment,
                        SettlementDate = f.SettlementDate,
                        PaymentStatus = f.PaymentStatus,
                        PaymentMethod = f.PaymentMethod,
                        StaffId = f.StaffId
                    }).ToListAsync();

                return new { feeSettlements = list };
            }
            else if (queryType == "ActivityParticipation")
            {
                var list = await _context.ActivityParticipations
                    .AsNoTracking()
                    .Where(a => a.ElderlyId == elderlyId)
                    .OrderByDescending(a => a.RegistrationTime)
                    .Select(a => new ActivityParticipationDto
                    {
                        ParticipationId = a.ParticipationId,
                        ActivityId = a.ActivityId,
                        ElderlyId = a.ElderlyId,
                        Status = a.Status,
                        RegistrationTime = a.RegistrationTime,
                        CheckInTime = a.CheckInTime,
                        Feedback = a.Feedback
                    }).ToListAsync();

                return new { activityParticipations = list };
            }
            else
            {
                throw new ArgumentException("查询类型无效");
            }
        }
    }
}
