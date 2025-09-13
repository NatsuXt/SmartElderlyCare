namespace ElderCare.Api.Modules.Medical
{
    // 查询结果用
    public sealed class MedicineDto
    {
        public int medicine_id { get; set; }
        public string medicine_name { get; set; } = string.Empty;
        public string? specification { get; set; }
        public string? dosage_form { get; set; }
        public string? unit { get; set; }
        public string? category { get; set; }
        public string? description { get; set; }
    }
}
