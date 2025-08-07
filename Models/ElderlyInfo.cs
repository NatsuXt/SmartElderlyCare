using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    public class ElderlyInfo
    {
        [Key]
        public decimal ELDERLY_ID { get; set; }
        public string NAME { get; set; } = "";
        public string GENDER { get; set; } = "";
        public DateTime? BIRTH_DATE { get; set; }
        public string CONTACT_PHONE { get; set; } = "";
        public string EMERGENCY_CONTACT { get; set; } = "";
        public string HEALTH_STATUS { get; set; } = "";
        public string MEDICAL_HISTORY { get; set; } = "";
        public DateTime? ADMISSION_DATE { get; set; }
        public decimal? ROOM_ID { get; set; }
        public string CARE_LEVEL { get; set; } = "";
        public string SPECIAL_NEEDS { get; set; } = "";
        
        // Navigation property
        public RoomManagement? Room { get; set; }
    }
}
