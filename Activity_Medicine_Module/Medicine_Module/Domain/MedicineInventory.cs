namespace ElderCare.Api.Modules.Medical
{
    public class MedicineInventory
    {
        public int medicine_id { get; set; }             // PK
        public string medicine_name { get; set; } = string.Empty;
        public string? specification { get; set; }              // 规格（0.25g×24片/盒）
        public string? dosage_form { get; set; }              // 剂型（片剂/胶囊/注射液…）
        public string? unit { get; set; }              // 计量单位（盒/瓶/片…）
        public string? category { get; set; }              // 药品类别（处方药/OTC…）
        public string? description { get; set; }              // 说明
    }
}
