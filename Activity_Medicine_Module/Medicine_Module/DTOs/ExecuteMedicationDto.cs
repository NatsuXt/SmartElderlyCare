using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class ExecuteMedicationDto
    {
        [Required] public int quantity { get; set; }
        [Required] public int executor_staff_id { get; set; }
        public string? notes { get; set; }
    }
}
