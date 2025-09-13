using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
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
    [Table("ROOMOCCUPANCY")]
    public class RoomOccupancy
    {
        [Key]
        [Column("OCCUPANCY_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OccupancyId { get; set; }

        [Required]
        [Column("ROOM_ID")]
        public int RoomId { get; set; }

        [Required]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required]
        [Column("CHECK_IN_DATE")]
        public DateTime CheckInDate { get; set; }

        [Column("CHECK_OUT_DATE")]
        public DateTime? CheckOutDate { get; set; }

        [Required]
        [Column("STATUS")]
        [MaxLength(20)]
        public string Status { get; set; } = "入住中";

        [Column("BED_NUMBER")]
        [MaxLength(10)]
        public string? BedNumber { get; set; }

        [Column("REMARKS")]
        [MaxLength(200)]
        public string? Remarks { get; set; }

        [Required]
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Column("UPDATED_DATE")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [ForeignKey("RoomId")]
        public virtual RoomManagement? Room { get; set; }

        [ForeignKey("ElderlyId")]
        public virtual ElderlyInfo? Elderly { get; set; }
    }


    [Table("ROOMMANAGEMENT")]
    public class RoomManagement
    {
        [Key]
        [Column("ROOM_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }

        [Required]
        [Column("ROOM_NUMBER")]
        [MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Column("ROOM_TYPE")]
        [MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;

        [Required]
        [Column("CAPACITY")]
        public int Capacity { get; set; }

        [Required]
        [Column("STATUS")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [Required]
        [Column("RATE")]
        public decimal Rate { get; set; }

        [Required]
        [Column("BED_TYPE")]
        [MaxLength(50)]
        public string BedType { get; set; } = string.Empty;

        [Required]
        [Column("FLOOR")]
        public int Floor { get; set; }
    }
    [Table("ROOMBILLING")]
    public class RoomBilling
    {
        [Key]
        [Column("BILLING_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillingId { get; set; }

        [Required]
        [Column("OCCUPANCY_ID")]
        public int OccupancyId { get; set; }

        [Required]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required]
        [Column("ROOM_ID")]
        public int RoomId { get; set; }

        [Required]
        [Column("BILLING_START_DATE")]
        public DateTime BillingStartDate { get; set; }

        [Required]
        [Column("BILLING_END_DATE")]
        public DateTime BillingEndDate { get; set; }

        [Required]
        [Column("DAYS")]
        public int Days { get; set; }

        [Required]
        [Column("DAILY_RATE")]
        public decimal DailyRate { get; set; }

        [Required]
        [Column("TOTAL_AMOUNT")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("PAYMENT_STATUS")]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "未支付";

        [Column("PAID_AMOUNT")]
        public decimal PaidAmount { get; set; } = 0;

        [Column("UNPAID_AMOUNT")]
        public decimal UnpaidAmount { get; set; }

        [Required]
        [Column("BILLING_DATE")]
        public DateTime BillingDate { get; set; } = DateTime.Now;

        [Column("PAYMENT_DATE")]
        public DateTime? PaymentDate { get; set; }

        [Column("REMARKS")]
        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [Column("UPDATED_DATE")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // Navigation Properties

        [ForeignKey("OccupancyId")]
        public virtual RoomOccupancy? Occupancy { get; set; }

        [ForeignKey("RoomId")]
        public virtual RoomManagement? Room { get; set; }

        [ForeignKey("ElderlyId")]
        public virtual ElderlyInfo? Elderly { get; set; }
    }

    [Table("ELECTRONICFENCE")]
    public class ElectronicFence
    {
        /// <summary>
        /// 围栏ID（主键，自增）
        /// </summary>
        [Key]
        [Column("FENCE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FenceId { get; set; }

        /// <summary>
        /// 区域范围定义，如坐标点集合
        /// </summary>
        [Required]
        [Column("AREA_DEFINITION", TypeName = "TEXT")]
        public string AreaDefinition { get; set; } = string.Empty;
    }

    [Table("FENCELOG")]
    public class FenceLog
    {
        /// <summary>
        /// 事件记录ID（主键，自增）
        /// </summary>
        [Key]
        [Column("EVENT_LOG_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventLogId { get; set; }

        /// <summary>
        /// 老人ID（外键）
        /// </summary>
        [Required]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 围栏ID（外键）
        /// </summary>
        [Required]
        [Column("FENCE_ID")]
        public int FenceId { get; set; }

        /// <summary>
        /// 进入时间
        /// </summary>
        [Required]
        [Column("ENTRY_TIME")]
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// 离开时间（可为空，表示尚未离开）
        /// </summary>
        [Column("EXIT_TIME")]
        public DateTime? ExitTime { get; set; }

        // --- 导航属性 ---

        /// <summary>
        /// 关联的老人信息
        /// </summary>
        [ForeignKey("ELDERLY_ID")]
        public virtual ElderlyInfo? Elderly { get; set; }

        /// <summary>
        /// 关联的围栏信息
        /// </summary>
        [ForeignKey("FENCE_ID")]
        public virtual ElectronicFence? Fence { get; set; }
    }

}

