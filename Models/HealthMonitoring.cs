using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 健康监测记录实体类
    /// </summary>
    public class HealthMonitoring
    {
        /// <summary>
        /// 检测记录ID（主键，自增）
        /// </summary>
        [Key]
        public int MonitoringId { get; set; }

        /// <summary>
        /// 老人ID（外键，关联老人基本信息表）
        /// </summary>
        [Required]
        [ForeignKey("Elderly")]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 检测日期
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
        [MaxLength(20)]
        public string? BloodPressure { get; set; }

        /// <summary>
        /// 血氧水平
        /// </summary>
        public float? OxygenLevel { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// 检测状态（如：正常、异常）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 导航属性：关联的老人信息
        /// </summary>
        public virtual ElderlyInfo? Elderly { get; set; }
    }
}
