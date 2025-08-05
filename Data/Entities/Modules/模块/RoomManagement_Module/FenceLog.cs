using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 电子围栏出入记录实体类
    /// </summary>
    [Table("FenceLog")]
    public class FenceLog
    {
        /// <summary>
        /// 事件记录ID（主键，自增）
        /// </summary>
        [Key]
        [Column("event_log_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventLogId { get; set; }

        /// <summary>
        /// 老人ID（外键）
        /// </summary>
        [Required]
        [Column("elderly_id")]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 围栏ID（外键）
        /// </summary>
        [Required]
        [Column("fence_id")]
        public int FenceId { get; set; }

        /// <summary>
        /// 进入时间
        /// </summary>
        [Required]
        [Column("entry_time")]
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// 离开时间（可为空，表示尚未离开）
        /// </summary>
        [Column("exit_time")]
        public DateTime? ExitTime { get; set; }

        // --- 关键：与公用表建立外键关系 ---

        /// <summary>
        /// 导航属性：关联的老人信息
        /// </summary>
        [ForeignKey("ElderlyId")]
        public virtual ElderlyInfo? Elderly { get; set; }

        /// <summary>
        /// 导航属性：关联的围栏信息
        /// </summary>
        [ForeignKey("FenceId")]
        public virtual ElectronicFence? Fence { get; set; }
    }
}
