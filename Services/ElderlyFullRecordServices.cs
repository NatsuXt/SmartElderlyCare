using System.Threading.Tasks;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;

namespace ElderlyCareSystem.Services
{
    public class ElderlyRecordService
    {
        private readonly AppDbContext _context;

        public ElderlyRecordService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ElderlyFullRecordDtos> GetElderlyFullRecordAsync(int elderlyId)
        {
            // 先查基本信息
            var elderlyEntity = await _context.ElderlyInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ElderlyId == elderlyId);

            if (elderlyEntity == null)
                return null;

            var elderlyDto = new ElderlyInfoDto
            {
                ElderlyId = elderlyEntity.ElderlyId,
                Name = elderlyEntity.Name,
                Gender = elderlyEntity.Gender,
                BirthDate = elderlyEntity.BirthDate,
                IdCardNumber = elderlyEntity.IdCardNumber,
                ContactPhone = elderlyEntity.ContactPhone,
                Address = elderlyEntity.Address,
                EmergencyContact = elderlyEntity.EmergencyContact
            };

            // 关联查询各表数据
            var familyInfos = await _context.FamilyInfos
                .Where(f => f.ElderlyId == elderlyId)
                .AsNoTracking()
                .Select(f => new FamilyInfoDto
                {
                    Name = f.Name,
                    Relationship = f.Relationship,
                    ContactPhone = f.ContactPhone,
                    ContactEmail = f.ContactEmail,
                    Address = f.Address,
                    IsPrimaryContact = f.IsPrimaryContact
                }).ToListAsync();

            var healthMonitorings = await _context.HealthMonitorings
                .Where(h => h.ElderlyId == elderlyId)
                .AsNoTracking()
                .Select(h => new HealthMonitoringDto
                {
                    HeartRate = (int)h.HeartRate,
                    BloodPressure = h.BloodPressure,
                    OxygenLevel = (float)h.OxygenLevel,
                    Temperature = (float)h.Temperature,
                    Status = h.Status,
                    MonitoringDate = h.MonitoringDate
                }).ToListAsync();

            var healthAssessments = await _context.HealthAssessmentReports
                .Where(a => a.ElderlyId == elderlyId)
                .AsNoTracking()
                .Select(a => new HealthAssessmentReportDto
                {
                    PhysicalHealthFunction = a.PhysicalHealthFunction,
                    PsychologicalFunction = a.PsychologicalFunction,
                    CognitiveFunction = a.CognitiveFunction,
                    HealthGrade = a.HealthGrade,
                    AssessmentDate = a.AssessmentDate
                }).ToListAsync();

            var medicalOrders = await _context.MedicalOrders
                .Where(m => m.ElderlyId == elderlyId)
                .AsNoTracking()
                .Select(m => new MedicalOrderDto
                {
                    OrderDate = m.OrderDate,
                    StaffId = m.StaffId,
                    MedicineId = m.MedicineId,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    Duration = m.Duration
                }).ToListAsync();

            var nursingPlans = await _context.NursingPlans
                .Where(n => n.ElderlyId == elderlyId)
                .AsNoTracking()
                .Select(n => new NursingPlanDto
                {
                    StaffId = n.StaffId,
                    PlanStartDate = n.PlanStartDate,
                    PlanEndDate = n.PlanEndDate,
                    CareType = n.CareType,
                    Priority = n.Priority,
                    EvaluationStatus = n.EvaluationStatus
                }).ToListAsync();

            var feeSettlements = await _context.FeeSettlements
                .Where(f => f.ElderlyId == elderlyId)
                .AsNoTracking()
                .Select(f => new FeeSettlementDto
                {
                    TotalAmount = f.TotalAmount,
                    InsuranceAmount = f.InsuranceAmount,
                    PersonalPayment = f.PersonalPayment,
                    SettlementDate = f.SettlementDate,
                    PaymentStatus = f.PaymentStatus,
                    PaymentMethod = f.PaymentMethod,
                    StaffId = f.StaffId
                }).ToListAsync();

            var activityParticipations = await _context.ActivityParticipations
                .Where(a => a.ElderlyId == elderlyId)
                .AsNoTracking()
                .Select(a => new ActivityParticipationDto
                {
                    ActivityId = a.ActivityId,
                    Status = a.Status,
                    RegistrationTime = a.RegistrationTime,
                    CheckInTime = a.CheckInTime,
                    Feedback = a.Feedback
                }).ToListAsync();

            return new ElderlyFullRecordDtos
            {
                ElderlyInfo = elderlyDto,
                FamilyInfos = familyInfos,
                HealthMonitorings = healthMonitorings,
                HealthAssessments = healthAssessments,
                MedicalOrders = medicalOrders,
                NursingPlans = nursingPlans,
                FeeSettlements = feeSettlements,
                ActivityParticipations = activityParticipations
            };
        }
    }
}
