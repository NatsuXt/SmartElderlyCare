using System;
namespace ElderlyCareSystem.Dtos
{
    public class ElderlyInfoCreateDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IdCardNumber { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
    }

    public class FamilyInfoCreateDto
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string IsPrimaryContact { get; set; }
    }

    public class HealthMonitoringCreateDto
    {
        public DateTime MonitoringDate { get; set; }
        public int HeartRate { get; set; }
        public string BloodPressure { get; set; }
        public float OxygenLevel { get; set; }
        public float Temperature { get; set; }
        public string Status { get; set; }
    }
    public class HealthAssessmentReportCreateDto
    {
        public DateTime AssessmentDate { get; set; }
        public int PhysicalHealthFunction { get; set; }
        public int PsychologicalFunction { get; set; }
        public int CognitiveFunction { get; set; }
        public string HealthGrade { get; set; }
    }
}
namespace ElderlyCareSystem.Dtos
{
    public class ElderlyInfoDto
    {
        public int ElderlyId { get; set; }
        public string? Name { get; set; }             // ? 表示可空
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? IdCardNumber { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
    }

    public class FamilyInfoDto
    {
        public int FamilyId { get; set; }
        public int ElderlyId { get; set; }
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string IsPrimaryContact { get; set; }
    }

    public class HealthMonitoringDto
    {
        public int MonitoringId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime MonitoringDate { get; set; }
        public int HeartRate { get; set; }
        public string BloodPressure { get; set; }
        public float OxygenLevel { get; set; }
        public float Temperature { get; set; }
        public string Status { get; set; }
    }

    public class MedicalOrderDto
    {
        public int OrderId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime OrderDate { get; set; }
        public int StaffId { get; set; }
        public int MedicineId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
    }

    public class NursingPlanDto
    {
        public int PlanId { get; set; }
        public int ElderlyId { get; set; }
        public int? StaffId { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string CareType { get; set; }
        public string Priority { get; set; }
        public string EvaluationStatus { get; set; }
    }

    public class FeeSettlementDto
    {
        public int SettlementId { get; set; }
        public int ElderlyId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal PersonalPayment { get; set; }
        public DateTime SettlementDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public int StaffId { get; set; }
    }

    public class HealthAssessmentReportDto
    {
        public int AssessmentId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime AssessmentDate { get; set; }
        public int PhysicalHealthFunction { get; set; }
        public int PsychologicalFunction { get; set; }
        public int CognitiveFunction { get; set; }
        public string HealthGrade { get; set; }
    }

    public class ActivityParticipationDto
    {
        public int ParticipationId { get; set; }
        public int ActivityId { get; set; }
        public int ElderlyId { get; set; }
        public string Status { get; set; }
        public DateTime? RegistrationTime { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string Feedback { get; set; }
    }

    public class DietRecommendationDto
    {
        public int RecommendationId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime RecommendationDate { get; set; }
        public string RecommendedFood { get; set; }
        public string ExecutionStatus { get; set; }
    }

    public class EmergencySOSDto
    {
        public int CallId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime CallTime { get; set; }
        public string CallType { get; set; }
        public int RoomId { get; set; }
        public DateTime? ResponseTime { get; set; }
        public int ResponseStaff { get; set; }
        public bool FollowUpRequired { get; set; }
        public string CallStatus { get; set; }
        public string HandlingResult { get; set; }
    }
    
}
