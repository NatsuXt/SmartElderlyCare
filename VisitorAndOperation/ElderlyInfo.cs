using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("ELDERLYINFO")]
    public class ElderlyInfo
    {
        [Key]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required]
        [Column("NAME"), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("GENDER"), MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Column("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }

        [Column("ID_CARD_NUMBER"), MaxLength(18)]
        public string IdCardNumber { get; set; } = string.Empty;

        [Column("CONTACT_PHONE"), MaxLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [Column("ADDRESS"), MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Column("EMERGENCY_CONTACT"), MaxLength(200)]
        public string EmergencyContact { get; set; } = string.Empty;

        // 注意：移除了导航属性，避免复杂的依赖关系
        // 在当前模块中，我们通过外键ID来引用相关实体
    }
}
