namespace ElderCare.Api.Modules.Medical
{
    public sealed class UpdateMedicalOrderDto
    {
        public string? dosage { get; set; }
        public string? frequency { get; set; }
        public string? duration { get; set; }
    }
}
