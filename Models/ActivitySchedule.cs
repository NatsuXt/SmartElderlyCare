using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("ActivitySchedule")]
public class ActivitySchedule
{
    [Key]
    public int activity_id { get; set; }
    public string activity_name { get; set; } = string.Empty;
    public DateTime activity_date { get; set; }
    public bool is_chargeable { get; set; }
    // 添加精度配置
    [Column(TypeName = "NUMBER(18, 2)")]
    public decimal fee { get; set; }

    // 多对多关系：一个活动可以有多个老人参加
    public ICollection<ElderlyActivity> ElderlyActivities { get; set; } = new List<ElderlyActivity>();
}

[Table("ElderlyActivity")]
// 多对多关系的连接表
public class ElderlyActivity
{
    [Key]
    public int id { get; set; }
    public int elderly_id { get; set; }
    public int activity_id { get; set; }

    [ForeignKey("elderly_id")]
    public ElderlyInfo ElderlyInfo { get; set; } = null!;

    [ForeignKey("activity_id")]
    public ActivitySchedule ActivitySchedule { get; set; } = null!;
}
