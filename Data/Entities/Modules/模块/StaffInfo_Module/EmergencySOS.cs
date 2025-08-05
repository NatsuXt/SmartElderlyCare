// EmergencySOS.cs

using System.ComponentModel.DataAnnotations;

namespace ElderlyCareManagement.Models
{
    public class EmergencySOS
    {
        [Key]public int CallId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime CallTime { get; set; }
        public string CallType { get; set; }
        public int? RoomId { get; set; }
        public DateTime? ResponseTime { get; set; }
        public int? ResponseStaffId { get; set; }
        public bool FollowUpRequired { get; set; }
        public string CallStatus { get; set; }
        public string HandlingResult { get; set; }
        
        // Navigation properties
        //public ElderlyInfo Elderly { get; set; }
        //public RoomManagement Room { get; set; }
        public StaffInfo ResponseStaff { get; set; }
    }
}