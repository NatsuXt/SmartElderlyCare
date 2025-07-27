using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("ElderlyInfo")]
public class ElderlyInfo
{
    [Key]
    public int elderly_id { get; set; }
    public string name { get; set; } = string.Empty;
    public string gender { get; set; } = string.Empty;
    public DateTime birth_date { get; set; }
    public string id_card_number { get; set; } = string.Empty;
    public string contact_phone { get; set; } = string.Empty;
    public string address { get; set; } = string.Empty;
    public string emergency_contact { get; set; } = string.Empty;

    // 多对多关系：一个活动可以有多个老人参加
    public ICollection<ElderlyActivity> ElderlyActivities { get; set; } = new List<ElderlyActivity>();
}
