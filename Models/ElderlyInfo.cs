using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("ElderlyInfo")]
    public class ElderlyInfo
    {
        [Key]
        [Column("elderly_id")]
        public int ElderlyId { get; set; }

        [Required, Column("name")]
        public string Name { get; set; }

        [Column("gender")]
        public string Gender { get; set; }

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }

        [Column("id_card_number")]
        public string IdCardNumber { get; set; }

        [Column("contact_phone")]
        public string ContactPhone { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("emergency_contact")]
        public string EmergencyContact { get; set; }
    }
}