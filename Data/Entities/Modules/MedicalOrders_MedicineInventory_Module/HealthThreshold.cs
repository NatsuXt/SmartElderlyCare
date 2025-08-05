using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HealthThreshold
{
    [Key]
    public int threshold_id { get; set; } // 阈值ID 主键PK

    public int elderly_id { get; set; } // 老人ID 外键，关联 ElderlyInfo 表

    [Required(ErrorMessage = "健康数据类型不能为空")]
    [StringLength(50, ErrorMessage = "健康数据类型不能超过50个字符")]
    public string data_type { get; set; } = string.Empty; // 健康数据类型，如高血压、低血压、心率

    [Required(ErrorMessage = "阈值下限不能为空")]
    public float min_value { get; set; } // 阈值下限

    [Required(ErrorMessage = "阈值上限不能为空")]
    public float max_value { get; set; } // 阈值上限

    [Column(TypeName = "TEXT")]
    public string? description { get; set; } // 描述（可选）
}
