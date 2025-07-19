using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("FAMILYINFO")]
    public class FamilyInfo
    {
        [Key]
        [Column("FAMILY_ID")]
        public int FamilyId { get; set; }

        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required, Column("NAME"), MaxLength(50)]
        public string Name { get; set; }

        [Required, Column("RELATIONSHIP"), MaxLength(20)]
        public string Relationship { get; set; }

        [Column("CONTACT_PHONE"), MaxLength(20)]
        public string ContactPhone { get; set; }

        [Column("CONTACT_EMAIL"), MaxLength(100)]
        public string ContactEmail { get; set; }

        [Column("ADDRESS"), MaxLength(200)]
        public string Address { get; set; }

        // 如果数据库是CHAR(1)且存的是 'Y'/'N'，你可以用string，也可以改成bool映射
        [Column("IS_PRIMARY_CONTACT")]
        public string IsPrimaryContact { get; set; }

        
    }
}
