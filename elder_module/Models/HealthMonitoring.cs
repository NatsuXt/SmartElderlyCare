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

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("MONITORING_DATE")]
        public DateTime MonitoringDate { get; set; }

        [Column("HEART_RATE")]
        public int? HeartRate { get; set; }

        [Column("BLOOD_PRESSURE"), MaxLength(20)]
        public string? BloodPressure { get; set; }

        [Column("OXYGEN_LEVEL")]
        public float? OxygenLevel { get; set; }

        [Column("TEMPERATURE")]
        public float? Temperature { get; set; }

        [Column("STATUS"), MaxLength(20)]
        public string Status { get; set; }

        public ElderlyInfo Elderly { get; set; }
    }
}
