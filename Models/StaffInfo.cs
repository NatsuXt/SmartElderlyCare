using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models;

public class STAFFINFO
{
    [Key]public decimal STAFF_ID { get; set; }
    public string NAME { get; set; } = "";
    public string GENDER { get; set; } = "";
    public string POSITION { get; set; } = "";
    public string CONTACT_PHONE { get; set; } = "";
    public string EMAIL { get; set; } = "";
    public DateTime? HIRE_DATE { get; set; }
    public decimal? SALARY { get; set; }
    public string SKILL_LEVEL { get; set; } = "";
    public string WORK_SCHEDULE { get; set; } = "";

    // 完整反向导航属性
    public ICollection<STAFFLOCATION> STAFFLOCATIONS { get; set; } = new List<STAFFLOCATION>();
}
