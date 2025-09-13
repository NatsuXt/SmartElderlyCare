using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class CreateDispenseDto
    {
        [Required] public int Elderly_Id { get; set; }
        [Required] public int Medicine_Id { get; set; }
        [Range(1, int.MaxValue)] public int Quantity { get; set; } = 1;

        public string? Bill_Id { get; set; }
        public int? Order_Id { get; set; }
        public int? Staff_Id { get; set; }
        public int? Settlement_Id { get; set; }

        public string? Payment_Method { get; set; }
        public string? Remarks { get; set; }
    }

    public sealed class UpdatePayStatusDto
    {
        [Required]
        [RegularExpression("^(UNPAID|PAID|REFUNDED)$")]
        public string Pay_Status { get; set; } = "PAID";
    }
}
