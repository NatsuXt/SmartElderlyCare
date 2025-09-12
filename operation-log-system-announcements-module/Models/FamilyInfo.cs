using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FamilyInfo
{
    [Key]
    public int family_id { get; set; }

    public int elderly_id { get; set; } // 外键，与其他地方保持一致

    [StringLength(100)]
    public string name { get; set; } = string.Empty;

    [StringLength(50)]
    public string relationship { get; set; } = string.Empty;

    [StringLength(20)]
    public string contact_phone { get; set; } = string.Empty;

    [StringLength(50)]
    public string contact_email { get; set; } = string.Empty;

    [StringLength(200)]
    public string address { get; set; } = string.Empty;

    [Column(TypeName = "NUMBER(1)")]
    public bool is_primary_contact { get; set; }

    // 导航属性
    public virtual ElderlyInfo? ElderlyInfo { get; set; }
}