using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;


[Table("NursingPlan")]
public class NursingPlan
{
    [Key]
    public int plan_id { get; set; }
    public int elderly_id { get; set; }
    public int staff_id { get; set; }
    public DateTime plan_start_date { get; set; }
    public DateTime plan_end_date { get; set; }
    public string care_type { get; set; } = string.Empty;
    public string priority { get; set; } = string.Empty;
    public string evaluation_status { get; set; } = string.Empty;

    [ForeignKey("elderly_id")]
    public ElderlyInfo? ElderlyInfo { get; set; }

    [ForeignKey("staff_id")]
    public StaffInfo? StaffInfo { get; set; }

    public ICollection<FeeSettlement>? FeeSettlements { get; set; }
}