using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class VisitorRegistration
{
    [Key]
    public int visitor_id { get; set; }

    public int family_id { get; set; } // 家属ID 外键，关联FamilyInfo

    public int elderly_id { get; set; } // 老人ID 外键

    [Required(ErrorMessage = "访客姓名不能为空")]
    [StringLength(100, ErrorMessage = "访客姓名不能超过100个字符")]
    public string visitor_name { get; set; } = string.Empty;

    public DateTime visit_time { get; set; }

    [Required(ErrorMessage = "与老人关系不能为空")]
    [StringLength(50, ErrorMessage = "与老人关系不能超过50个字符")]
    public string relationship_to_elderly { get; set; } = string.Empty;

    [Required(ErrorMessage = "探访原因不能为空")]
    [Column(TypeName = "CLOB")]
    public string visit_reason { get; set; } = string.Empty;

    [StringLength(20)]
    public string visit_type { get; set; } = string.Empty;

    [StringLength(20)]
    public string approval_status { get; set; } = string.Empty;

    // 注意：不包含导航属性，通过family_id和elderly_id外键引用，由其他模块管理相关实体
}