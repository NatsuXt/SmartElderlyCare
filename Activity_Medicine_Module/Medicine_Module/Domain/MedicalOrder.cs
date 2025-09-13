namespace ElderCare.Api.Modules.Medical
{
    public sealed class MedicalOrder
    {
        public int order_id { get; set; }
        public int elderly_id { get; set; }
        public int staff_id { get; set; }
        public int medicine_id { get; set; }
        public DateTime order_date { get; set; }
        public string dosage { get; set; } = string.Empty;
        public string frequency { get; set; } = string.Empty;
        public string duration { get; set; } = string.Empty;
    }
}
