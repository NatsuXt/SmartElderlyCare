using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staff_Info.Models;

[Table("STAFFACCOUNT")] 
public class STAFFACCOUNT
{
    //[Key] public decimal ACCOUNT_ID { get; set; }

    [Required]
    [Key]
    public decimal STAFF_ID { get; set; }
    
    [Required]
    [StringLength(255)]
    public string PASSWORD_HASH { get; set; }

    // 导航属性 - 关联到STAFFINFO
    [ForeignKey("STAFF_ID")]
    public STAFFINFO STAFFINFO { get; set; }

}