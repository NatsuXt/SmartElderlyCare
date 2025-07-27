// Models/StaffSchedule.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareManagement.Models
{
    public class StaffSchedule
    {
        [Key]
        public int ScheduleId { get; set; }
        
        [Required]
        public int StaffId { get; set; }
        
        [Required]
        public DayOfWeek DayOfWeek { get; set; } // 星期几
        
        [Required]
        public TimeSpan StartTime { get; set; } // 开始时间
        
        [Required]
        public TimeSpan EndTime { get; set; } // 结束时间
        
        // 导航属性
        public virtual StaffInfo Staff { get; set; }
    }
}