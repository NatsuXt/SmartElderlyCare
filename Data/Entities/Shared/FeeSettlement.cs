using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace api.Models;

[Table("FeeSettlement")]
public class FeeSettlement
{
    [Key]
    public int settlement_id { get; set; }
    public int elderly_id { get; set; }
    [Column(TypeName = "NUMBER(18, 2)")]
    public decimal total_amount { get; set; }
    [Column(TypeName = "NUMBER(18, 2)")]
    public decimal insurance_amount { get; set; }
    [Column(TypeName = "NUMBER(18, 2)")]
    public decimal personal_payment { get; set; }
    public DateTime settlement_date { get; set; }
    public string payment_status { get; set; } = string.Empty;
    public string payment_method { get; set; } = string.Empty;
    public int staff_id { get; set; }

    [ForeignKey("elderly_id")]
    public ElderlyInfo? ElderlyInfo { get; set; }

    [ForeignKey("staff_id")]
    public StaffInfo? StaffInfo { get; set; }

    public ICollection<NursingPlan>? NursingPlans { get; set; }
    public ICollection<FeeDetail>? FeeDetails { get; set; }
}