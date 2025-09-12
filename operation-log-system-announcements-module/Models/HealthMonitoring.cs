using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("HEALTHMONITORINGS")]
public class HealthMonitoring
{
    [Key]
    [Column("MONITORING_ID")]
    public int monitoring_id { get; set; } // 检测记录ID 主键PK

    [Column("ELDERLY_ID")]
    public int elderly_id { get; set; } // 老人ID 外键

    [Column("MONITORING_DATE")]
    public DateTime monitoring_date { get; set; } // 检测日期

    [Column("HEART_RATE")]
    public int heart_rate { get; set; } // 心率

    [Column("BLOOD_PRESSURE")]
    public string blood_pressure { get; set; } = string.Empty; // 血压

    [Column("OXYGEN_LEVEL")]
    public float oxygen_level { get; set; } // 血氧水平

    [Column("TEMPERATURE")]
    public float temperature { get; set; } // 体温

    [Column("STATUS")]
    public string status { get; set; } = string.Empty; // 检测状态 如正常、异常
}