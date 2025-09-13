using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class CreateMedicineDto
    {
        [Required] public string medicine_name { get; set; } = string.Empty;
        [Required] public string medicine_type { get; set; } = string.Empty;
        [Required] public decimal unit_price { get; set; }
        [Required] public int quantity_in_stock { get; set; }
        [Required] public int minimum_stock_level { get; set; }
        [Required] public string supplier { get; set; } = string.Empty;
        [Required] public DateTime expiration_date { get; set; }
        public string? description { get; set; }
    }

    public sealed class UpdateMedicineDto
    {
        public string? medicine_name { get; set; }
        public string? medicine_type { get; set; }
        public decimal? unit_price { get; set; }
        public int? quantity_in_stock { get; set; }
        public int? minimum_stock_level { get; set; }
        public string? supplier { get; set; }
        public DateTime? expiration_date { get; set; }
        public string? description { get; set; }
    }
}
