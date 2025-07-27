// Models/SosNotification.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareManagement.Models
{
    public class SosNotification
    {
        [Key]
        public int NotificationId { get; set; }
        public int CallId { get; set; }
        public int StaffId { get; set; }
        public DateTime NotificationTime { get; set; }
        public bool IsResponded { get; set; }
        public DateTime? ResponseTime { get; set; }
        
        // 导航属性
        public EmergencySOS EmergencySos { get; set; }
        public StaffInfo Staff { get; set; }
    }
}