using ElderlyCareSystem.Dtos;
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

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required]
        [Column("NAME"), MaxLength(100)]
        public string Name { get; set; }

        [Column("RELATIONSHIP"), MaxLength(50)]
        public string Relationship { get; set; }

        [Column("CONTACT_PHONE"), MaxLength(20)]
        public string ContactPhone { get; set; }

        [Column("CONTACT_EMAIL"), MaxLength(50)]
        public string ContactEmail { get; set; }

        [Column("ADDRESS"), MaxLength(200)]
        public string Address { get; set; }

        [Column("IS_PRIMARY_CONTACT"), MaxLength(1)]
        public string IsPrimaryContact { get; set; }  // 'Y' or 'N'

        public ElderlyInfo Elderly { get; set; }

        
    }
}
