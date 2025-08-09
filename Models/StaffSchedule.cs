using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class STAFFSCHEDULE
{
    [Key]public decimal SCHEDULE_ID { get; set; }
    public decimal STAFF_ID { get; set; }
    public byte DAY_OF_WEEK { get; set; }  // 0=周日，1=周一
    public TimeSpan START_TIME { get; set; }
    public TimeSpan END_TIME { get; set; }

    // 外键导航属性
    public STAFFINFO STAFF { get; set; }  // 对应 fk_staffschedule_staff
}