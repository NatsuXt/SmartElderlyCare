using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class CreateMedicalOrderDto
    {
        [Required] public int elderly_id { get; set; }
        [Required] public int staff_id { get; set; }
        [Required] public int medicine_id { get; set; }
        [Required] public DateTime order_date { get; set; }
        public string dosage { get; set; } = string.Empty;
        public string frequency { get; set; } = string.Empty;
        public string duration { get; set; } = string.Empty;
    }
}
