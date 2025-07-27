using System;
using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 电子围栏出入记录实体类
    /// </summary>
    public class FenceLog
    {
        /// <summary>
        /// 事件记录ID（主键）
        /// </summary>
        [Key]
        public int EventLogId { get; set; }

        /// <summary>
        /// 老人ID（外键）
        /// </summary>
        [Required]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 围栏ID（外键）
        /// </summary>
        [Required]
        public int FenceId { get; set; }

        /// <summary>
        /// 进入时间
        /// </summary>
        [Required]
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// 离开时间（可为空，表示尚未离开）
        /// </summary>
        public DateTime? ExitTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 关联的老人信息
        /// </summary>
        public virtual ElderlyInfo? Elderly { get; set; }

        /// <summary>
        /// 关联的围栏信息
        /// </summary>
        public virtual ElectronicFence? Fence { get; set; }
    }
}
