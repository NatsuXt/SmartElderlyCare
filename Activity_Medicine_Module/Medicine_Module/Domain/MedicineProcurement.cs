namespace ElderCare.Api.Modules.Medical
{
    public sealed class MedicineProcurement
    {
        public int procurement_id { get; set; }
        public int medicine_id { get; set; }
        public int purchase_quantity { get; set; }
        public DateTime purchase_time { get; set; }
        public int staff_id { get; set; }
        public string status { get; set; } = string.Empty;
    }
}
