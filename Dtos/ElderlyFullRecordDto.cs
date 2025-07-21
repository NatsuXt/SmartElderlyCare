using System;
using System.Collections.Generic;

namespace ElderlyFullRecordDto.Dtos
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

    public class ElderlyDto
    {
        public int ElderlyId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IdCardNumber { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
    }

    public class FamilyDto
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string IsPrimaryContact { get; set; }
    }

    public class HealthMonitoringDto
    {
        public int HeartRate { get; set; }
        public string BloodPressure { get; set; }
        public float OxygenLevel { get; set; }
        public float Temperature { get; set; }
        public string Status { get; set; }
        public DateTime MonitoringDate { get; set; }
    }

    public class HealthAssessmentDto
    {
        public int PhysicalHealthFunction { get; set; }
        public int PsychologicalFunction { get; set; }
        public int CognitiveFunction { get; set; }
        public string HealthGrade { get; set; }
        public DateTime AssessmentDate { get; set; }
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
