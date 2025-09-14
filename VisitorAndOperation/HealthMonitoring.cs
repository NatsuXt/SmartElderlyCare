using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 健康监测记录实体类
    /// </summary>
    [Table("HealthMonitoring")]
    public class HealthMonitoring
    {
        /// <summary>
        /// 检测记录ID（主键，自增）
        /// </summary>
        [Key]
        [Column("MONITORING_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MonitoringId { get; set; }

        /// <summary>
        /// 老人ID（外键）
        /// </summary>
        [Required]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 检测日期
        /// </summary>
        [Required]
        [Column("MONITORING_DATE")]
        public DateTime MonitoringDate { get; set; }

        /// <summary>
        /// 心率
        /// </summary>
        [Column("HEART_RATE")]
        public int? HeartRate { get; set; }

        /// <summary>
        /// 血压
        /// </summary>
        [Column("BLOOD_PRESSURE")]
        [MaxLength(20)]
        public string? BloodPressure { get; set; }

        /// <summary>
        /// 血氧水平
        /// </summary>
        [Column("OXYGEN_LEVEL")]
        public float? OxygenLevel { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        [Column("TEMPERATURE")]
        public float? Temperature { get; set; }

        /// <summary>
        /// 检测状态（如：正常、异常）
        /// </summary>
        [Required]
        [Column("STATUS")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        // 注意：移除了导航属性，避免复杂的依赖关系
        // 在当前模块中，我们通过外键ID来引用相关实体
    }
}
