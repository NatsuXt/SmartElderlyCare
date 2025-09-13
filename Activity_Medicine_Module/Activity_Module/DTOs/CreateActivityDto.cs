using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Activities
{
    public sealed class CreateActivityDto
    {
        [Required] public string activity_name { get; set; } = string.Empty;
        [Required] public DateTime activity_date { get; set; }
        [Required] public string activity_time { get; set; } = string.Empty; // HH:mm:ss
        [Required] public string location { get; set; } = string.Empty;
        [Required] public int staff_id { get; set; }
        public string? activity_description { get; set; }
    }

    public sealed class UpdateActivityDto
    {
        public string? activity_name { get; set; }
        public DateTime? activity_date { get; set; }
        public string? activity_time { get; set; }
        public string? location { get; set; }
        public string? activity_description { get; set; }
    }
}
