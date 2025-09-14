using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class VisitorRegistration
{
    [Key]
    [Column("REGISTRATION_ID")]
    public int RegistrationId { get; set; }

    [Column("VISITOR_ID")]
    public int VisitorId { get; set; } // 负责人访客ID，外键关联VisitorLogin

    [Column("ELDERLY_ID")]
    public int ElderlyId { get; set; } // 老人ID 外键

    [Required(ErrorMessage = "访客姓名不能为空")]
    [StringLength(100, ErrorMessage = "访客姓名不能超过100个字符")]
    [Column("VISITOR_NAME")]
    public string VisitorName { get; set; } = string.Empty;

    [Column("VISIT_TIME")]
    public DateTime VisitTime { get; set; }

    [Required(ErrorMessage = "与老人关系不能为空")]
    [StringLength(50, ErrorMessage = "与老人关系不能超过50个字符")]
    [Column("RELATIONSHIP_TO_ELDERLY")]
    public string RelationshipToElderly { get; set; } = string.Empty;

    [Required(ErrorMessage = "探访原因不能为空")]
    [Column("VISIT_REASON", TypeName = "CLOB")]
    public string VisitReason { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("VISIT_TYPE")]
    public string VisitType { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("APPROVAL_STATUS")]
    public string ApprovalStatus { get; set; } = string.Empty;
}
