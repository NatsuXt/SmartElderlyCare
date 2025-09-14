using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class STAFFINFO
{
    [Key]public decimal STAFF_ID { get; set; }
    public string NAME { get; set; } = string.Empty;
    public string GENDER { get; set; } = string.Empty;
    public string POSITION { get; set; } = string.Empty;
    public string CONTACT_PHONE { get; set; } = string.Empty;
    public string EMAIL { get; set; } = string.Empty;
    public DateTime? HIRE_DATE { get; set; }
    public decimal? SALARY { get; set; }
    public string SKILL_LEVEL { get; set; } = string.Empty;
    public string WORK_SCHEDULE { get; set; } = string.Empty;

    // 注意：移除了导航属性，避免复杂的依赖关系
    // 在当前模块中，我们通过外键ID来引用相关实体
}
