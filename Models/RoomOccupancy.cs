using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    

    /// <summary>
    /// 房间入住信息实体类
    /// 用于记录老人与房间的入住关系，作为两表之间的关联表
    /// </summary>
    [Table("RoomOccupancy")]
    public class RoomOccupancy
    {
        /// <summary>
        /// 入住记录ID（主键，自增）
        /// </summary>
        [Key]
        [Column("occupancy_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OccupancyId { get; set; }

        /// <summary>
        /// 房间ID（外键，关联RoomManagement表的room_id）
        /// </summary>
        [Required]
        [Column("room_id")]
        public int RoomId { get; set; }

        /// <summary>
        /// 老人ID（外键，关联ElderlyInfo表的ELDERLY_ID）
        /// </summary>
        [Required]
        [Column("elderly_id")]
        public decimal ElderlyId { get; set; }

        /// <summary>
        /// 入住日期
        /// </summary>
        [Required]
        [Column("check_in_date")]
        public DateTime CheckInDate { get; set; }

        /// <summary>
        /// 退房日期（可为空，表示仍在入住）
        /// </summary>
        [Column("check_out_date")]
        public DateTime? CheckOutDate { get; set; }

        /// <summary>
        /// 入住状态（入住中、已退房）
        /// </summary>
        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = "入住中";

        /// <summary>
        /// 床位号（在房间内的具体床位，可选）
        /// </summary>
        [Column("bed_number")]
        [MaxLength(10)]
        public string? BedNumber { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Column("remarks")]
        [MaxLength(200)]
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
