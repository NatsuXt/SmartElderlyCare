using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;


[Table("RoomManagement")]
public class RoomManagement
{
    [Key]
    public int room_id { get; set; }
    public int elderly_id { get; set; }
    public string room_number { get; set; } = string.Empty;
    public string room_type { get; set; } = string.Empty; // 单人间、双人间等
                                                          // 添加精度配置
    [Column(TypeName = "NUMBER(18, 2)")]
    public decimal daily_rate { get; set; }
    public DateTime check_in_date { get; set; }
    public DateTime? check_out_date { get; set; }

    [ForeignKey("elderly_id")]
    public ElderlyInfo? ElderlyInfo { get; set; }
}
