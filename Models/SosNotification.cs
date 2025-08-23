using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class SOSNOTIFICATION
{
    [Key]public decimal NOTIFICATION_ID { get; set; }
    public decimal CALL_ID { get; set; }
    public decimal STAFF_ID { get; set; }
    public DateTime NOTIFICATION_TIME { get; set; }
    public bool IS_RESPONDED { get; set; }
    public DateTime? RESPONSE_TIME { get; set; }

    // 外键导航属性
    public EMERGENCYSOS CALL { get; set; }    // 对应 fk_sosnotification_call
    public STAFFINFO STAFF { get; set; }      // 对应 fk_sosnotification_staff
}