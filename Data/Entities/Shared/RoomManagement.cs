using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 房间管理实体类
    /// </summary>
    public class RoomManagement
    {
        /// <summary>
        /// 房间ID（主键）
        /// </summary>
        [Key]
        public int RoomId { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        /// <summary>
        /// 房间类型（如：单人间、多人间）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;

        /// <summary>
        /// 房间容量，最多容纳人数
        /// </summary>
        [Required]
        public int Capacity { get; set; }

        /// <summary>
        /// 房间状态（如：已入住、空闲）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 房间收费标准
        /// </summary>
        [Required]
        public decimal Rate { get; set; }

        /// <summary>
        /// 床铺类型（如：单人床、双人床）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string BedType { get; set; } = string.Empty;

        /// <summary>
        /// 楼层
        /// </summary>
        [Required]
        public int Floor { get; set; }
    }
}
