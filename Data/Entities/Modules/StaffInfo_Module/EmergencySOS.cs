using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class EMERGENCYSOS
{
    [Key]public decimal CALL_ID { get; set; }
    public decimal ELDERLY_ID { get; set; }
    public DateTime CALL_TIME { get; set; }
    public string CALL_TYPE { get; set; }
    public decimal? ROOM_ID { get; set; }
    public DateTime? RESPONSE_TIME { get; set; }
    public decimal? RESPONSE_STAFF_ID { get; set; }
    public bool FOLLOW_UP_REQUIRED { get; set; }
    public string CALL_STATUS { get; set; }
    public string HANDLING_RESULT { get; set; }

    // 外键导航属性
    public ELDERLYINFO ELDERLY { get; set; }
    public ROOMMANAGEMENT ROOM { get; set; }
    public STAFFINFO RESPONSE_STAFF { get; set; }

}