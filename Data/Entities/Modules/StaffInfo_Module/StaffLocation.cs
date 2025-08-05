// Models/StaffLocation.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareManagement.Models
{
    public class StaffLocation
    {
        [Key]
        public int LocationId { get; set; }
        
        [Required]
        public int StaffId { get; set; }
        
        public int? RoomId { get; set; } // 可为空，表示不在任何房间
        
        [Required]
        public DateTime UpdateTime { get; set; } = DateTime.Now;
        
        // 导航属性
        public virtual StaffInfo Staff { get; set; }
        public virtual Room Room { get; set; }
    }
}