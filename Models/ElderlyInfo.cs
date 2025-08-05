using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    [Table("ELDERLYINFO")]
    public class ElderlyInfo
    {
        [Key]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Required]
        [Column("NAME"), MaxLength(100)]
        public string Name { get; set; }

        [Column("GENDER"), MaxLength(10)]
        public string Gender { get; set; }

        [Column("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }

        [Column("ID_CARD_NUMBER"), MaxLength(18)]
        public string IdCardNumber { get; set; }

        [Column("CONTACT_PHONE"), MaxLength(20)]
        public string ContactPhone { get; set; }

        [Column("ADDRESS"), MaxLength(200)]
        public string Address { get; set; }

        [Column("EMERGENCY_CONTACT"), MaxLength(200)]
        public string EmergencyContact { get; set; }
    }
}
