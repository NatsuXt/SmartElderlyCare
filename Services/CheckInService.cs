using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCareSystem.Services
{
    public interface ICheckInService
    {
        Task<int> FullRegisterAsync(ElderlyFullRegistrationDto dto);
    }

    public class CheckInService : ICheckInService
    {
        private readonly AppDbContext _context;

        public CheckInService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> FullRegisterAsync(ElderlyFullRegistrationDto dto)
        {
            var elderly = new ElderlyInfo
            {
                Name = dto.Elderly.Name,
                Gender = dto.Elderly.Gender,
                BirthDate = dto.Elderly.BirthDate,
                IdCardNumber = dto.Elderly.IdCardNumber,
                ContactPhone = dto.Elderly.ContactPhone,
                Address = dto.Elderly.Address,
                EmergencyContact = dto.Elderly.EmergencyContact
            };

            _context.ElderlyInfo.Add(elderly);
            await _context.SaveChangesAsync();

            var assessment = new HealthAssessmentReport
            {
                ElderlyId = elderly.ElderlyId,
                AssessmentDate = DateTime.Now,
                PhysicalHealthFunction = dto.Assessment.PhysicalHealthFunction,
                PsychologicalFunction = dto.Assessment.PsychologicalFunction,
                CognitiveFunction = dto.Assessment.CognitiveFunction,
                HealthGrade = dto.Assessment.HealthGrade
            };
            _context.HealthAssessmentReport.Add(assessment);

            var monitoring = new HealthMonitoring
            {
                ElderlyId = elderly.ElderlyId,
                MonitoringDate = DateTime.Now,
                HeartRate = dto.Monitoring.HeartRate,
                BloodPressure = dto.Monitoring.BloodPressure,
                OxygenLevel = (decimal)dto.Monitoring.OxygenLevel,
                Temperature = (decimal)dto.Monitoring.Temperature,
                Status = dto.Monitoring.Status
            };
            _context.HealthMonitoring.Add(monitoring);

            if (dto.Families != null && dto.Families.Any())
            {
                foreach (var family in dto.Families)
                {
                    _context.FamilyInfo.Add(new FamilyInfo
                    {
                        ElderlyId = elderly.ElderlyId,
                        Name = family.Name,
                        Relationship = family.Relationship,
                        ContactPhone = family.ContactPhone,
                        ContactEmail = family.ContactEmail,
                        Address = family.Address,
                        IsPrimaryContact = family.IsPrimaryContact
                    });
                }
            }

            await _context.SaveChangesAsync();

            return elderly.ElderlyId;
        }
    }
}
