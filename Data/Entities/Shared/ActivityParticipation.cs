using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class ActivityParticipation
    {
        [Key]
        public int participation_id { get; set; } // 参与记录ID 主键PK

        public int activity_id { get; set; } // 活动ID 外键 关联 ActivitySchedule 表

        public int elderly_id { get; set; } // 老人ID 外键 关联 ElderlyInfo 表

        [StringLength(20)]
        public string status { get; set; } = string.Empty; // 参与状态 如：已报名、已参加、缺席

        public DateTime registration_time { get; set; } // 报名时间

        public DateTime? check_in_time { get; set; } // 签到时间 可为空

        [Column(TypeName = "TEXT")]
        public string? feedback { get; set; } // 活动反馈 可选字段，记录评价或感想

        [ForeignKey("elderly_id")]
        public ElderlyInfo? Elderly { get; set; } // 导航属性：老人信息

        [ForeignKey("activity_id")]
        public ActivitySchedule? Activity { get; set; } // 导航属性：活动安排信息
    }
}
