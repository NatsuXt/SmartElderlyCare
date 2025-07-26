using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("MedicalOrders")]
    public class MedicalOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        public int ElderlyId { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        public int? StaffId { get; set; }

        public int MedicineId { get; set; }

        [StringLength(20)]
        public string Dosage { get; set; }

        [StringLength(50)]
        public string Frequency { get; set; }

        [StringLength(50)]
        public string Duration { get; set; }

        // 导航属性
        //[ForeignKey("ElderlyId")]
        //public ElderlyInfo Elderly { get; set; }

        [ForeignKey("StaffId")]
        public StaffInfo Staff { get; set; }

        //[ForeignKey("MedicineId")]
        //public MedicineInventory Medicine { get; set; }

        //public ICollection<VoiceAssistantReminder> VoiceAssistantReminders { get; set; }
    }
}