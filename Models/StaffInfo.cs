using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class STAFFINFO
{
    [Key]public decimal STAFF_ID { get; set; }
    public string NAME { get; set; }
    public string GENDER { get; set; }
    public string POSITION { get; set; }
    public string CONTACT_PHONE { get; set; }
    public string EMAIL { get; set; }
    public DateTime? HIRE_DATE { get; set; }
    public decimal? SALARY { get; set; }
    public string SKILL_LEVEL { get; set; }
    public string WORK_SCHEDULE { get; set; }

    // 完整反向导航属性
    public ICollection<ACTIVITYSCHEDULE> ACTIVITYSCHEDULES { get; set; }
    public ICollection<DISINFECTIONRECORD> DISINFECTIONRECORDS { get; set; }
    public ICollection<EMERGENCYSOS> RESPONSIBLE_EMERGENCYSOS { get; set; } // 处理的紧急呼叫
    public ICollection<MEDICALORDER> MEDICALORDERS { get; set; }
    public ICollection<NURSINGPLAN> NURSINGPLANS { get; set; }
    public ICollection<OPERATIONLOG> OPERATIONLOGS { get; set; }
    public ICollection<SOSNOTIFICATION> SOSNOTIFICATIONS { get; set; }
    public ICollection<SYSTEMANNOUNCEMENTS> CREATED_ANNOUNCEMENTS { get; set; }
    public ICollection<STAFFLOCATION> STAFFLOCATIONS { get; set; }
    public ICollection<STAFFSCHEDULE> STAFFSCHEDULES { get; set; }
}