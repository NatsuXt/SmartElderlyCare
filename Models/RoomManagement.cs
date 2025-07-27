using System;
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
        [StringLength(20)]
        public string? RoomNumber { get; set; }

        /// <summary>
        /// 房间类型（如：单人间、双人间、套房等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string? RoomType { get; set; }

        /// <summary>
        /// 房间容量，最多容纳人数
        /// </summary>
        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }

        /// <summary>
        /// 房间状态（如：空闲、已入住、维护中、清洁中）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// 房间收费标准
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Rate { get; set; }

        /// <summary>
        /// 床铺类型（如：单人床、双人床）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string? BedType { get; set; }

        /// <summary>
        /// 楼层
        /// </summary>
        [Required]
        [Range(1, 50)]
        public int FloorNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
