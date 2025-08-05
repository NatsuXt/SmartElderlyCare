// Models/Room.cs
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareManagement.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        
        [Required]
        public string RoomName { get; set; }
        
        [Required]
        public int Floor { get; set; } // 楼层
        
        [Required]
        public int RoomNumber { get; set; } // 房间号
        
        public string Description { get; set; }
    }
}