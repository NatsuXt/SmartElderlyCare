// MedicalOrder.cs
using System.Collections.Generic;

namespace ElderlyCareManagement.Models
{
    public class MedicalOrder
    {
        public int OrderId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? StaffId { get; set; }
        public int MedicineId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        
        // Navigation properties
        //public ElderlyInfo Elderly { get; set; }
        public StaffInfo Staff { get; set; }
        //public MedicineInventory Medicine { get; set; }
        //public ICollection<VoiceAssistantReminder> VoiceAssistantReminders { get; set; }
    }
}