using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// 系统公告实体模型
/// </summary>
/// <remarks>
/// 用于管理养老院系统中的各类公告信息，包括针对家属、员工和全体人员的通知。
/// 支持公告的发布、撤回、评论等功能。
/// </remarks>
public class SystemAnnouncements
{
    /// <summary>
    /// 公告ID（主键）
    /// </summary>
    [Key]
    [Column("ANNOUNCEMENT_ID")]
    public int announcement_id { get; set; }

    /// <summary>
    /// 公告发布日期
    /// </summary>
    [Column("ANNOUNCEMENT_DATE")]
    public DateTime announcement_date { get; set; }

    /// <summary>
    /// 公告类型
    /// </summary>
    /// <example>紧急通知</example>
    [StringLength(50)]
    [Column("ANNOUNCEMENT_TYPE")]
    public string announcement_type { get; set; } = string.Empty;

    /// <summary>
    /// 公告内容
    /// </summary>
    /// <example>请注意：明天上午9点将进行消防演练，请各位配合。</example>
    [Column("ANNOUNCEMENT_CONTENT", TypeName = "CLOB")]
    public string announcement_content { get; set; } = string.Empty;

    /// <summary>
    /// 公告状态，限制为：已发布、已撤回
    /// </summary>
    /// <example>已发布</example>
    [StringLength(20)]
    [Column("STATUS")]
    [RegularExpression("^(已发布|已撤回)$", ErrorMessage = "公告状态必须是：已发布、已撤回")]
    public string status { get; set; } = "已发布";

    /// <summary>
    /// 受众类型，限制为：家属、员工、全体
    /// </summary>
    /// <example>全体</example>
    [StringLength(50)]
    [Column("AUDIENCE")]
    [RegularExpression("^(家属|员工|全体)$", ErrorMessage = "受众类型必须是：家属、员工、全体")]
    public string audience { get; set; } = "全体";

    /// <summary>
    /// 发布人员工ID（外键）
    /// </summary>
    /// <example>1</example>
    [Column("CREATED_BY")]
    public int created_by { get; set; }

    /// <summary>
    /// 公告评论内容
    /// </summary>
    /// <example>谢谢提醒，我们会准时参加。</example>
    [Column("COMMENTS", TypeName = "CLOB")]
    public string comments { get; set; } = string.Empty;

    /// <summary>
    /// 阅读状态，用于标记公告是否已被阅读
    /// </summary>
    /// <example>未读</example>
    [StringLength(20)]
    [Column("READ_STATUS")]
    public string read_status { get; set; } = "未读";

    /// <summary>
    /// 获取所有有效的受众类型选项
    /// </summary>
    public static readonly string[] ValidAudienceTypes = { "家属", "员工", "全体" };

    /// <summary>
    /// 获取所有有效的公告状态选项
    /// </summary>
    public static readonly string[] ValidStatusTypes = { "已发布", "已撤回" };
}