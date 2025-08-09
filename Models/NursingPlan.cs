using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_Info.Models;

public class NURSINGPLAN
{
    [Key]public decimal PLAN_ID { get; set; }
    public decimal ELDERLY_ID { get; set; }
    public decimal? STAFF_ID { get; set; }
    public DateTime PLAN_START_DATE { get; set; }
    public DateTime PLAN_END_DATE { get; set; }
    public string CARE_TYPE { get; set; }
    public string PRIORITY { get; set; }
    public string EVALUATION_STATUS { get; set; }

    // 外键导航属性
    [ForeignKey("ELDERLY_ID")]
    public virtual ELDERLYINFO ELDERLY { get; set; }
    
    [ForeignKey("STAFF_ID")]
    public virtual STAFFINFO STAFF { get; set; }

}