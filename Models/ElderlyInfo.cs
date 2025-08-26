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

    [Table("VISITORREGISTRATION")]
    public class VisitorRegistration
    {
        [Key]
        [Column("VISITOR_ID")]
        public int visitor_id { get; set; }

        [Column("FAMILY_ID")]
        public int family_id { get; set; } // 家属ID 外键

        [Column("ELDERLY_ID")]
        public int elderly_id { get; set; } // 老人ID 外键

        [Required(ErrorMessage = "访客姓名不能为空")]
        [StringLength(100, ErrorMessage = "访客姓名不能超过100个字符")]
        [Column("VISITOR_NAME")]
        public string visitor_name { get; set; } = string.Empty;

        [Column("VISIT_TIME")]
        public DateTime visit_time { get; set; }

        [Required(ErrorMessage = "与老人关系不能为空")]
        [StringLength(50, ErrorMessage = "与老人关系不能超过50个字符")]
        [Column("RELATIONSHIP_TO_ELDERLY")]
        public string relationship_to_elderly { get; set; } = string.Empty;

        [Required(ErrorMessage = "探访原因不能为空")]
        [Column("VISIT_REASON", TypeName = "CLOB")]
        public string visit_reason { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("VISIT_TYPE")]
        public string visit_type { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("APPROVAL_STATUS")]
        public string approval_status { get; set; } = string.Empty;
    }



}

