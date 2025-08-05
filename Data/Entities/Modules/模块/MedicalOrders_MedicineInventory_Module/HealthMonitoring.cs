using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HealthMonitoring
{
    [Key]
    public int monitoring_id { get; set; } // 检测记录ID 主键PK

    public int elderly_id { get; set; } // 老人ID 外键，关联 ElderlyInfo 表

    [Required(ErrorMessage = "检测日期不能为空")]
    [Column(TypeName = "DATE")]
    public DateTime monitoring_date { get; set; } // 检测日期

    public int? heart_rate { get; set; } // 心率，可为空

    [StringLength(20)]
    public string? blood_pressure { get; set; } // 血压，可为空

    public float? oxygen_level { get; set; } // 血氧水平，可为空

    public float? temperature { get; set; } // 体温，可为空

    [Required(ErrorMessage = "检测状态不能为空")]
    [StringLength(20, ErrorMessage = "状态不能超过20个字符")]
    public string status { get; set; } = string.Empty; // 检测状态，如正常、异常
}
