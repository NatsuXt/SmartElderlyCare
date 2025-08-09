using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class OPERATIONLOG
{
    [Key]public decimal LOG_ID { get; set; }
    public decimal STAFF_ID { get; set; }
    public DateTime OPERATION_TIME { get; set; }
    public string OPERATION_TYPE { get; set; }
    public string OPERATION_DESCRIPTION { get; set; }
    public string AFFECTED_ENTITY { get; set; }
    public string OPERATION_STATUS { get; set; }
    public string IP_ADDRESS { get; set; }
    public string DEVICE_TYPE { get; set; }

    // 外键导航属性
    public STAFFINFO STAFF { get; set; }  // 对应 fk_operationlog_staff
}