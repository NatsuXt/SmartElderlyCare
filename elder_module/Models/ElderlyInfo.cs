using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("ELDERLYINFO")]
    public class ElderlyInfo
    {
        [Key]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("NAME"), MaxLength(100)]
        public string Name { get; set; }

        [Column("GENDER"), MaxLength(10)]
        public string? Gender { get; set; }

        [Column("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }

        [Column("ID_CARD_NUMBER"), MaxLength(18)]
        public string? IdCardNumber { get; set; }

        [Column("CONTACT_PHONE"), MaxLength(20)]
        public string? ContactPhone { get; set; }

        [Column("ADDRESS"), MaxLength(200)]
        public string? Address { get; set; }

        [Column("EMERGENCY_CONTACT"), MaxLength(200)]
        public string? EmergencyContact { get; set; }

        public ICollection<FamilyInfo> Families { get; set; } = new List<FamilyInfo>();
        public ICollection<HealthMonitoring> HealthMonitorings { get; set; } = new List<HealthMonitoring>();
        public ICollection<HealthAssessmentReport> HealthAssessmentReports { get; set; } = new List<HealthAssessmentReport>();
        public ICollection<MedicalOrder> MedicalOrders { get; set; } = new List<MedicalOrder>();
        public ICollection<NursingPlan> NursingPlans { get; set; } = new List<NursingPlan>();
        public ICollection<FeeSettlement> FeeSettlements { get; set; } = new List<FeeSettlement>();
        public ICollection<ActivityParticipation> ActivityParticipations { get; set; } = new List<ActivityParticipation>();
        public ICollection<DietRecommendation> DietRecommendations { get; set; } = new List<DietRecommendation>();
        public ICollection<EmergencySOS> EmergencySOSCalls { get; set; } = new List<EmergencySOS>();

        public ElderlyAccount ElderlyAccount { get; set; }

        public ICollection<HealthAlert> HealthAlerts { get; set; } = new List<HealthAlert>();
        public ICollection<HealthThreshold> HealthThresholds { get; set; } = new List<HealthThreshold>();
        public ICollection<VoiceAssistantReminder> VoiceAssistantReminders { get; set; } = new List<VoiceAssistantReminder>();
    }

}

