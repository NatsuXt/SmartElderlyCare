using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class SYSTEMANNOUNCEMENTS
{
    [Key]public decimal ANNOUNCEMENT_ID { get; set; }
    public DateTime ANNOUNCEMENT_DATE { get; set; }
    public string ANNOUNCEMENT_TYPE { get; set; }
    public string ANNOUNCEMENT_CONTENT { get; set; }
    public string STATUS { get; set; }
    public string AUDIENCE { get; set; }
    public decimal CREATED_BY { get; set; }
    public string READ_STATUS { get; set; }
    public string COMMENTS { get; set; }

    // 外键导航属性
    public STAFFINFO STAFF { get; set; }  // 对应 fk_systemannouncements_staff
}