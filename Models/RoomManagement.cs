using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 房间管理实体类
    /// </summary>
    [Table("RoomManagement")]
    public class RoomManagement
    {
        /// <summary>
        /// 房间ID（主键，自增）
        /// </summary>
        [Key]
        [Column("room_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        [Required]
        [Column("room_number")]
        [MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        /// <summary>
        /// 房间类型（如：单人间、多人间）
        /// </summary>
        [Required]
        [Column("room_type")]
        [MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        /// 房间容量，最多容纳人数
        /// </summary>
        [Required]
        [Column("capacity")]
        public int Capacity { get; set; }

        /// <summary>
        /// 房间状态（如：已入住、空闲）
        /// </summary>
        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 房间收费标准
        /// </summary>
        [Required]
        [Column("rate")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 床铺类型（如：单人床、双人床）
        /// </summary>
        [Required]
        [Column("bed_type")]
        [MaxLength(50)]
        public string BedType { get; set; } = string.Empty;

        /// <summary>
        /// 楼层
        /// </summary>
        [Required]
        [Column("floor")]
        public int Floor { get; set; }
    }
}
