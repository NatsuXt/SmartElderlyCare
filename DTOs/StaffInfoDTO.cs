// DTOs/StaffInfoDTO.cs
namespace ElderlyCareManagement.DTOs
{
    public class StaffInfoDTO
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public string ContactPhone { get; set; }
        public string Email { get; set; }
        public DateTime HireDate { get; set; }
        public string SkillLevel { get; set; }
        public int? CurrentRoomId { get; set; }
        public int Distance { get; set; }
    }

    public class StaffCreateDTO
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public string ContactPhone { get; set; }
        public string Email { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string SkillLevel { get; set; }
        public string WorkSchedule { get; set; }
    }

    public class StaffUpdateDTO : StaffCreateDTO
    {
        public int StaffId { get; set; }
    }

    public class StaffDetailDTO : StaffInfoDTO
    {
        public decimal Salary { get; set; }
        public string WorkSchedule { get; set; }
    }
}