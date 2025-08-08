using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HealthAlert
{
    [Key]
    public int alert_id { get; set; } // 预警ID 主键PK

    public int elderly_id { get; set; } // 老人ID 外键，关联 ElderlyInfo

    [Required(ErrorMessage = "预警类型不能为空")]
    [StringLength(50, ErrorMessage = "预警类型不能超过50个字符")]
    public string alert_type { get; set; } = string.Empty; // 预警类型，如高血压预警

    [Required(ErrorMessage = "预警时间不能为空")]
    public DateTime alert_time { get; set; } // 预警时间

    [Required(ErrorMessage = "预警数值不能为空")]
    [StringLength(50, ErrorMessage = "预警数值不能超过50个字符")]
    public string alert_value { get; set; } = string.Empty; // 触发预警的数值

    public int notified_staff_id { get; set; } // 通知医护人员ID 外键，关联 StaffInfo

    [Required(ErrorMessage = "处理状态不能为空")]
    [StringLength(20, ErrorMessage = "处理状态不能超过20个字符")]
    public string status { get; set; } = string.Empty; // 处理状态，如待处理、已处理
}
