using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 房间住宿账单实体类
    /// 用于记录老人的住宿费用账单信息
    /// </summary>
    [Table("RoomBilling")]
    public class RoomBilling
    {
        /// <summary>
        /// 账单ID（主键，自增）
        /// </summary>
        [Key]
        [Column("billing_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillingId { get; set; }

        /// <summary>
        /// 入住记录ID（外键，关联RoomOccupancy表）
        /// </summary>
        [Required]
        [Column("occupancy_id")]
        public int OccupancyId { get; set; }

        /// <summary>
        /// 老人ID（外键，关联ElderlyInfo表）
        /// </summary>
        [Required]
        [Column("elderly_id")]
        public decimal ElderlyId { get; set; }

        /// <summary>
        /// 房间ID（外键，关联RoomManagement表）
        /// </summary>
        [Required]
        [Column("room_id")]
        public int RoomId { get; set; }

        /// <summary>
        /// 账单开始日期
        /// </summary>
        [Required]
        [Column("billing_start_date")]
        public DateTime BillingStartDate { get; set; }

        /// <summary>
        /// 账单结束日期
        /// </summary>
        [Required]
        [Column("billing_end_date")]
        public DateTime BillingEndDate { get; set; }

        /// <summary>
        /// 天数
        /// </summary>
        [Required]
        [Column("days")]
        public int Days { get; set; }

        /// <summary>
        /// 每日收费标准
        /// </summary>
        [Required]
        [Column("daily_rate")]
        public decimal DailyRate { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        [Required]
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 支付状态（未支付、已支付、部分支付）
        /// </summary>
        [Required]
        [Column("payment_status")]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "未支付";

        /// <summary>
        /// 已支付金额
        /// </summary>
        [Column("paid_amount")]
        public decimal PaidAmount { get; set; } = 0;

        /// <summary>
        /// 未支付金额
        /// </summary>
        [Column("unpaid_amount")]
        public decimal UnpaidAmount { get; set; }

        /// <summary>
        /// 账单生成日期
        /// </summary>
        [Required]
        [Column("billing_date")]
        public DateTime BillingDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 支付日期
        /// </summary>
        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Column("remarks")]
        [MaxLength(500)]
        public string? Remarks { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [Required]
        [Column("updated_date")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // Navigation Properties - 导航属性
        
        /// <summary>
        /// 关联的入住记录
        /// </summary>
        [ForeignKey("OccupancyId")]
        public virtual RoomOccupancy? Occupancy { get; set; }

        /// <summary>
        /// 关联的房间信息
        /// </summary>
        [ForeignKey("RoomId")]
        public virtual RoomManagement? Room { get; set; }

        /// <summary>
        /// 关联的老人信息
        /// </summary>
        [ForeignKey("ElderlyId")]
        public virtual ElderlyInfo? Elderly { get; set; }
    }
}
