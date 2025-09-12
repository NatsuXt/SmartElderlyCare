using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;


[Table("MedicalOrder")]
public class MedicalOrder
{
    [Key]
    public int order_id { get; set; }
    public int elderly_id { get; set; }
    public int staff_id { get; set; }
    public DateTime order_date { get; set; }
    public string medicine_name { get; set; } = string.Empty;
    public int quantity { get; set; }
    // 添加精度配置
    [Column(TypeName = "NUMBER(18, 2)")]
    public decimal unit_price { get; set; }
    public string status { get; set; } = string.Empty; // 已领用、未领用

    [ForeignKey("elderly_id")]
    public ElderlyInfo? ElderlyInfo { get; set; }

    [ForeignKey("staff_id")]
    public StaffInfo? StaffInfo { get; set; }
}
