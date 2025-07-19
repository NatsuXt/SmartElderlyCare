using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("FamilyInfo")]
    public class FamilyInfo
    {
        [Key]
        [Column("family_id")]
        public int FamilyId { get; set; }

        [Column("elderly_id")]
        public int ElderlyId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("relationship")]
        public string Relationship { get; set; }

        [Column("contact_phone")]
        public string ContactPhone { get; set; }

        [Column("contact_email")]
        public string ContactEmail { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("is_primary_contact")]
        public string IsPrimaryContact { get; set; }
    }
}