using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("HealthMonitoring")]
    public class HealthMonitoring
    {
        [Key]
        [Column("monitoring_id")]
        public int MonitoringId { get; set; }

        [Column("elderly_id")]
        public int ElderlyId { get; set; }

        [Column("monitoring_date")]
        public DateTime MonitoringDate { get; set; }

        [Column("heart_rate")]
        public int HeartRate { get; set; }

        [Column("blood_pressure")]
        public string BloodPressure { get; set; }

        [Column("oxygen_level")]
        public float OxygenLevel { get; set; }

        [Column("temperature")]
        public float Temperature { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }
}
