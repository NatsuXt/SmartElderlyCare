using System;
using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 健康监测记录实体类
    /// </summary>
    public class HealthMonitoring
    {
        /// <summary>
        /// 监测记录ID（主键）
        /// </summary>
        [Key]
        public int MonitoringId { get; set; }

        /// <summary>
        /// 老人ID（外键）
        /// </summary>
        [Required]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 监测日期
        /// </summary>
        [Required]
        public DateTime MonitoringDate { get; set; }

        /// <summary>
        /// 心率
        /// </summary>
        public int? HeartRate { get; set; }

        /// <summary>
        /// 血压
        /// </summary>
        [StringLength(20)]
        public string? BloodPressure { get; set; }

        /// <summary>
        /// 血氧水平
        /// </summary>
        public decimal? OxygenLevel { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        public decimal? Temperature { get; set; }

        /// <summary>
        /// 检测状态（如：正常、异常、危险）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 关联的老人信息
        /// </summary>
        public virtual ElderlyInfo? Elderly { get; set; }
    }
}
