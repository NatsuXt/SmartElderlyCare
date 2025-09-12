using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class DISINFECTIONRECORD
{
    [Key]public decimal DISINFECTION_ID { get; set; }
    public string AREA { get; set; }
    public DateTime DISINFECTION_TIME { get; set; }
    public decimal STAFF_ID { get; set; }
    public string METHODS { get; set; }

    // 外键导航属性
    public STAFFINFO STAFF { get; set; }  // 对应 fk_disinfection_staff
}
