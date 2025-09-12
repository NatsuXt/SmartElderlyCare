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
        [Column("monitoring_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MonitoringId { get; set; }

        /// <summary>
        /// 老人ID（外键）
        /// </summary>
        [Required]
        [Column("elderly_id")]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 检测日期
        /// </summary>
        [Required]
        [Column("monitoring_date")]
        public DateTime MonitoringDate { get; set; }

        /// <summary>
        /// 心率
        /// </summary>
        [Column("heart_rate")]
        public int? HeartRate { get; set; }

        /// <summary>
        /// 血压
        /// </summary>
        [Column("blood_pressure")]
        [MaxLength(20)]
        public string? BloodPressure { get; set; }

        /// <summary>
        /// 血氧水平
        /// </summary>
        [Column("oxygen_level")]
        public float? OxygenLevel { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        [Column("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// 检测状态（如：正常、异常）
        /// </summary>
        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        // --- 关键：与公用表建立外键关系 ---

        /// <summary>
        /// 导航属性：关联的老人信息
        /// </summary>
        [ForeignKey("ElderlyId")]
        public virtual ElderlyInfo? Elderly { get; set; }
    }
}
