using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("HEALTHMONITORING")]
    public class HealthMonitoring
    {
        [Key]
        [Column("MONITORING_ID")]
        public int MonitoringId { get; set; }

        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("MONITORING_DATE")]
        public DateTime MonitoringDate { get; set; }

        [Column("HEART_RATE")]
        public int HeartRate { get; set; }

        [MaxLength(20)]
        [Column("BLOOD_PRESSURE")]
        public string BloodPressure { get; set; }

        [Column("OXYGEN_LEVEL")]
        public decimal OxygenLevel { get; set; }

        [Column("TEMPERATURE")]
        public decimal Temperature { get; set; }

        [MaxLength(50)]
        [Column("STATUS")]
        public string Status { get; set; }

        // 可选：导航属性
        // [ForeignKey("ElderlyId")]
        // public ElderlyInfo Elderly { get; set; }
    }
}
