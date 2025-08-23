using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class MEDICALORDER
{
    [Key]public decimal ORDER_ID { get; set; }
    public decimal ELDERLY_ID { get; set; }
    public decimal STAFF_ID { get; set; }
    public decimal MEDICINE_ID { get; set; }
    public DateTime ORDER_DATE { get; set; }
    public string DOSAGE { get; set; }
    public string FREQUENCY { get; set; }
    public string DURATION { get; set; }

    // 外键导航属性
    public ELDERLYINFO ELDERLY { get; set; }
    public STAFFINFO STAFF { get; set; }
    public MEDICINEINVENTORY MEDICINE { get; set; }
    
    // (注意：ELDERLYINFO 和 MedicineInventory 表未提供，如需引用需额外定义)
}