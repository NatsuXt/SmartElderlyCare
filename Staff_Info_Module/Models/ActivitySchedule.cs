using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class ACTIVITYSCHEDULE
{
    [Key]public decimal ACTIVITY_ID { get; set; }
    public string ACTIVITY_NAME { get; set; }
    public DateTime ACTIVITY_DATE { get; set; }
    public TimeSpan ACTIVITY_TIME { get; set; }
    public string LOCATION { get; set; }
    public decimal STAFF_ID { get; set; }
    public string ELDERLY_PARTICIPANTS { get; set; }
    public string ACTIVITY_DESCRIPTION { get; set; }

    // 外键导航属性
    public STAFFINFO STAFF { get; set; }  // 对应 fk_activityschedule_staff
}