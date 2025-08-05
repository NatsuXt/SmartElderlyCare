using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class VoiceAssistantReminder
{
    [Key]
    public int reminder_id { get; set; } // 提示ID 主键PK

    public int order_id { get; set; } // 医嘱ID 外键，关联 MedicalOrder 表

    public int elderly_id { get; set; } // 老人ID 外键，关联 ElderlyInfo 表

    [Required(ErrorMessage = "提醒时间不能为空")]
    public DateTime reminder_time { get; set; } // 提醒时间

    [Required(ErrorMessage = "提醒次数不能为空")]
    public int reminder_count { get; set; } // 提醒次数

    [Required(ErrorMessage = "提醒状态不能为空")]
    [StringLength(20, ErrorMessage = "提醒状态不能超过20个字符")]
    public string reminder_status { get; set; } = string.Empty; // 提醒状态，如已提醒、未提醒
}
