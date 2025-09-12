using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SystemAnnouncements
{
    [Key]
    public int announcement_id { get; set; } // 公告ID 主键PK

    public DateTime announcement_date { get; set; } // 公告发布日期

    [StringLength(50)]
    public string announcement_type { get; set; } = string.Empty; // 公告类型 如紧急、常规

    [Column(TypeName = "CLOB")]
    public string announcement_content { get; set; } = string.Empty; // 公告内容

    [StringLength(20)]
    public string status { get; set; } = string.Empty; // 状态 如已发布、草稿

    [StringLength(50)]
    public string audience { get; set; } = string.Empty; // 受众 如员工、家属

    public int created_by { get; set; } // 员工ID 外键，发布公告的员工ID

    [StringLength(20)]
    public string read_status { get; set; } = string.Empty; // 阅读状态 如已阅读、未阅读

    [Column(TypeName = "CLOB")]
    public string comments { get; set; } = string.Empty; // 家属对公告的评论
}