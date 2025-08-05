// StaffInfo.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    public class StaffInfo
    {
        [Key]public int StaffId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public string ContactPhone { get; set; }
        public string Email { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string SkillLevel { get; set; }
        public string WorkSchedule { get; set; }
        
        // Navigation properties
    }
}