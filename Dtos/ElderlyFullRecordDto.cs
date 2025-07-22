using System;
using System.Collections.Generic;

namespace ElderlyCareSystem.Dtos
{
    public class ElderlyFullRecordDtos
    {
        public ElderlyDto ElderlyInfo { get; set; }
        public List<FamilyDto> FamilyInfos { get; set; }
        public List<HealthMonitoringDto> HealthMonitorings { get; set; }
        public List<HealthAssessmentDto> HealthAssessments { get; set; }
        public List<MedicalOrderDto> MedicalOrders { get; set; }
        public List<NursingPlanDto> NursingPlans { get; set; }
        public List<FeeSettlementDto> FeeSettlements { get; set; }
        public List<ActivityParticipationDto> ActivityParticipations { get; set; }
    }

    public class MedicalOrderDto
    {
        public DateTime OrderDate { get; set; }
        public int StaffId { get; set; }
        public int MedicineId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
    }

    public class NursingPlanDto
    {
        public int StaffId { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string CareType { get; set; }
        public string Priority { get; set; }
        public string EvaluationStatus { get; set; }
    }

    public class FeeSettlementDto
    {
        public decimal TotalAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal PersonalPayment { get; set; }
        public DateTime SettlementDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public int StaffId { get; set; }
    }

    public class ActivityParticipationDto
    {
        public int ActivityId { get; set; }
        public string Status { get; set; }
        public DateTime? RegistrationTime { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string Feedback { get; set; }
    }
}
