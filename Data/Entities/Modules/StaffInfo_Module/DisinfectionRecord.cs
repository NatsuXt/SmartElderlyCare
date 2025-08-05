// DisinfectionRecord.cs

using System.ComponentModel.DataAnnotations;

namespace ElderlyCareManagement.Models
{
    public class DisinfectionRecord
    {
        [Key]public int DisinfectionId { get; set; }
        public string Area { get; set; }
        public DateTime DisinfectionTime { get; set; }
        public int StaffId { get; set; }
        public string Method { get; set; }
        
        // Navigation property
        public StaffInfo Staff { get; set; }
    }
}