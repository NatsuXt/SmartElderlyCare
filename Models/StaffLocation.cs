using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models;

public class STAFFLOCATION
{
    [Key]public decimal LOCATION_ID { get; set; }
    public decimal STAFF_ID { get; set; }
    public decimal? FLOOR { get; set; }
    public DateTime UPDATE_TIME { get; set; }

    // 外键导航属性
    public STAFFINFO STAFF { get; set; } = new STAFFINFO();         // 对应 fk_stafflocation_staff
}