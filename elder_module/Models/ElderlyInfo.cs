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

        [Required]
        [Column("NAME"), MaxLength(100)]
        public string Name { get; set; }

        [Column("GENDER"), MaxLength(10)]
        public string Gender { get; set; }

        [Column("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }

        [Column("ID_CARD_NUMBER"), MaxLength(18)]
        public string IdCardNumber { get; set; }

        [Column("CONTACT_PHONE"), MaxLength(20)]
        public string ContactPhone { get; set; }

        [Column("ADDRESS"), MaxLength(200)]
        public string Address { get; set; }

        [Column("EMERGENCY_CONTACT"), MaxLength(200)]
        public string EmergencyContact { get; set; }

        public List<FamilyInfo> Families { get; set; }
        public List<HealthMonitoring> HealthMonitorings { get; set; }
        public List<HealthAssessmentReport> HealthAssessmentReports { get; set; }
        public List<MedicalOrder> MedicalOrders { get; set; }
        public List<NursingPlan> NursingPlans { get; set; }
        public List<FeeSettlement> FeeSettlements { get; set; }
        public List<ActivityParticipation> ActivityParticipations { get; set; }
        public List<DietRecommendation> DietRecommendations { get; set; }
        public List<EmergencySOS> EmergencySOSCalls { get; set; }
    }
}
