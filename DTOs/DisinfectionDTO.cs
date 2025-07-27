// DTOs/DisinfectionDTO.cs
namespace ElderlyCareManagement.DTOs
{
    public class DisinfectionRecordDTO
    {
        public int DisinfectionId { get; set; }
        public string Area { get; set; }
        public DateTime DisinfectionTime { get; set; }
        public string StaffName { get; set; }
        public string Method { get; set; }
    }

    public class DisinfectionCreateDTO
    {
        public string Area { get; set; }
        public int StaffId { get; set; }
        public string Method { get; set; }
    }

    public class DisinfectionReportDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalDisinfections { get; set; }
        public List<AreaDisinfectionSummaryDTO> AreasDisinfected { get; set; }
        public List<StaffDisinfectionSummaryDTO> StaffParticipation { get; set; }
    }

    public class AreaDisinfectionSummaryDTO
    {
        public string AreaName { get; set; }
        public int DisinfectionCount { get; set; }
        public DateTime LastDisinfection { get; set; }
    }

    public class StaffDisinfectionSummaryDTO
    {
        public string StaffName { get; set; }
        public int DisinfectionCount { get; set; }
    }
}