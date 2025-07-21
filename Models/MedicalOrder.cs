using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("MEDICALORDERS")]
    public class MedicalOrder
    {
        [Key]
        [Column("ORDER_ID")]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("ORDER_DATE")]
        public DateTime OrderDate { get; set; }

        [Column("STAFF_ID")]
        public int StaffId { get; set; }

        [Column("MEDICINE_ID")]
        public int MedicineId { get; set; }

        [Column("DOSAGE"), MaxLength(20)]
        public string Dosage { get; set; }

        [Column("FREQUENCY"), MaxLength(50)]
        public string Frequency { get; set; }

        [Column("DURATION"), MaxLength(50)]
        public string Duration { get; set; }

        public ElderlyInfo Elderly { get; set; }
    }
}
