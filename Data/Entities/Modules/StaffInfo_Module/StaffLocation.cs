using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class STAFFLOCATION
{
    [Key]public decimal LOCATION_ID { get; set; }
    public decimal STAFF_ID { get; set; }
    public decimal? FLOOR { get; set; }
    public DateTime UPDATE_TIME { get; set; }

    // 外键导航属性
    public STAFFINFO STAFF { get; set; }         // 对应 fk_stafflocation_staff
    public ROOMMANAGEMENT ROOM { get; set; }     // 对应 fk_stafflocation_room
}
