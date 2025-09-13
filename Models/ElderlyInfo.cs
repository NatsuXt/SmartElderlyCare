using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class ELDERLYINFO
{
    [Key]public decimal ELDERLY_ID { get; set; }
    public string NAME { get; set; }
    public string GENDER { get; set; }
    public DateTime? BIRTH_DATE { get; set; }
    public string ID_CARD_NUMBER { get; set; }
    public string CONTACT_PHONE { get; set; }
    public string ADDRESS { get; set; }
    public string EMERGENCY_CONTACT { get; set; }

    // 反向导航属性（被哪些表引用）
    public ICollection<EMERGENCYSOS> EMERGENCYSOS { get; set; }
    public ICollection<MEDICALORDER> MEDICALORDERS { get; set; }
    public ICollection<NURSINGPLAN> NURSINGPLANS { get; set; }
}