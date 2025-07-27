using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;


[Table("StaffInfo")]
public class StaffInfo
{
    [Key]
    public int staff_id { get; set; }
    public string name { get; set; } = string.Empty;
    public string gender { get; set; } = string.Empty;
    public string position { get; set; } = string.Empty;
    public string contact_phone { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public DateTime hire_date { get; set; }
    // 添加精度配置
    [Column(TypeName = "decimal(18, 2)")]
    public decimal salary { get; set; }
}
