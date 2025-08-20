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

        public List<FamilyInfo> Families { get; set; }
        public List<HealthMonitoring> HealthMonitorings { get; set; }
        public List<HealthAssessmentReport> HealthAssessmentReports { get; set; }
        public List<MedicalOrder> MedicalOrders { get; set; }
        public List<NursingPlan> NursingPlans { get; set; }
        public List<FeeSettlement> FeeSettlements { get; set; }
        public List<ActivityParticipation> ActivityParticipations { get; set; }
        public List<DietRecommendation> DietRecommendations { get; set; }
        public List<EmergencySOS> EmergencySOSCalls { get; set; }

        public ICollection<HealthAlert> HealthAlerts { get; set; } = new List<HealthAlert>();
        public ICollection<HealthThreshold> HealthThresholds { get; set; } = new List<HealthThreshold>();
        public ICollection<VoiceAssistantReminder> VoiceAssistantReminders { get; set; } = new List<VoiceAssistantReminder>();
    }

    [Table("HEALTHALERT")]
    public class HealthAlert
    {
        [Key]
        [Column("ALERT_ID")]
        public int AlertId { get; set; }

        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required, StringLength(50)]
        [Column("ALERT_TYPE")]
        public string AlertType { get; set; } = string.Empty;

        [Required]
        [Column("ALERT_TIME")]
        public DateTime AlertTime { get; set; }

        [Required, StringLength(50)]
        [Column("ALERT_VALUE")]
        public string AlertValue { get; set; } = string.Empty;

        [Column("NOTIFIED_STAFF_ID")]
        public int NotifiedStaffId { get; set; }

        [Required, StringLength(20)]
        [Column("STATUS")]
        public string Status { get; set; } = string.Empty;

        // 导航属性，指向 C# 外键属性名
        [ForeignKey(nameof(ElderlyId))]
        public ElderlyInfo Elderly { get; set; } = null!;
    }

    [Table("HEALTHTHRESHOLD")]
    public class HealthThreshold
    {
        [Key]
        [Column("THRESHOLD_ID")]
        public int ThresholdId { get; set; }

        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required, StringLength(50)]
        [Column("DATA_TYPE")]
        public string DataType { get; set; } = string.Empty;

        [Required]
        [Column("MIN_VALUE")]
        public float MinValue { get; set; }

        [Required]
        [Column("MAX_VALUE")]
        public float MaxValue { get; set; }

        [Column("DESCRIPTION", TypeName = "TEXT")]
        public string? Description { get; set; }

        // 导航属性
        [ForeignKey(nameof(ElderlyId))]
        public ElderlyInfo Elderly { get; set; } = null!;
    }

    [Table("VOICEASSISTANTREMINDER")]
    public class VoiceAssistantReminder
    {
        [Key]
        [Column("REMINDER_ID")]
        public int ReminderId { get; set; }

        [Column("ORDER_ID")]
        public int OrderId { get; set; }

        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required]
        [Column("REMINDER_TIME")]
        public DateTime ReminderTime { get; set; }

        [Required]
        [Column("REMINDER_COUNT")]
        public int ReminderCount { get; set; }

        [Required, StringLength(20)]
        [Column("REMINDER_STATUS")]
        public string ReminderStatus { get; set; } = string.Empty;

        // 导航属性
        [ForeignKey(nameof(ElderlyId))]
        public ElderlyInfo Elderly { get; set; } = null!;
    }
    [Table("FEEDETAIL")]
    public class FeeDetail
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("FEE_SETTLEMENT_ID")]
        [ForeignKey(nameof(FeeSettlement))]
        public int FeeSettlementId { get; set; }

        [Required]
        [Column("FEE_TYPE", TypeName = "NVARCHAR2(50)")]
        public string FeeType { get; set; } = string.Empty;

        [Required]
        [Column("DESCRIPTION", TypeName = "NVARCHAR2(200)")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("AMOUNT", TypeName = "NUMBER(18,2)")]
        public decimal Amount { get; set; }

        [Column("START_DATE", TypeName = "DATE")]
        public DateTime? StartDate { get; set; }

        [Column("END_DATE", TypeName = "DATE")]
        public DateTime? EndDate { get; set; }

        [Column("QUANTITY")]
        public int? Quantity { get; set; }

        [Column("UNIT_PRICE", TypeName = "NUMBER(18,2)")]
        public decimal? UnitPrice { get; set; }

        // 导航属性
        public FeeSettlement FeeSettlement { get; set; } = null!;
    }

}

