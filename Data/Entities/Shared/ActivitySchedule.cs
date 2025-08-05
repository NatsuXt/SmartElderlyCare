using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class ActivitySchedule
    {
        [Key]
        public int activity_id { get; set; } // 活动ID 主键PK

        [StringLength(100)]
        public string activity_name { get; set; } = string.Empty; // 活动名称

        [Column(TypeName = "DATE")]
        public DateTime activity_date { get; set; } // 活动日期，如片剂、注射液等

        [Column(TypeName = "TIME")]
        public TimeSpan activity_time { get; set; } // 活动时间

        [StringLength(100)]
        public string location { get; set; } = string.Empty; // 活动地点

        public int staff_id { get; set; } // 员工ID 外键，关联员工信息表

        [Column(TypeName = "TEXT")]
        public string? elderly_participants { get; set; } // 参与活动的老人列表（以逗号分隔）

        [Column(TypeName = "TEXT")]
        public string? activity_description { get; set; } // 活动描述
    }
}
