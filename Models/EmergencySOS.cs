using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_Info.Models;

public class EMERGENCYSOS
{
    [Key]
    [Column("CALL_ID")]
    public decimal CALL_ID { get; set; }  // 改为int以匹配NUMBER
    
    [ForeignKey("ELDERLY")]  // This links to the navigation property
    [Column("ELDERLY_ID")]   // Explicit column name
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
    
    [ForeignKey("ROOM_ID")]
    public ROOMMANAGEMENT ROOM { get; set; }
    
    [ForeignKey("RESPONSE_STAFF_ID")]
    public STAFFINFO RESPONSE_STAFF { get; set; }
}