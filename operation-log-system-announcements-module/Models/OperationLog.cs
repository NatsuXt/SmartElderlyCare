using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OperationLog
{
    [Key]
    public int log_id { get; set; } // 日志ID 主键PK

    public int staff_id { get; set; } // 员工ID 外键

    public DateTime operation_time { get; set; } // 操作时间

    [StringLength(50)]
    public string operation_type { get; set; } = string.Empty; // 操作类型 如登录、数据修改

    [Column(TypeName = "CLOB")]
    public string operation_description { get; set; } = string.Empty; // 操作描述

    [StringLength(100)]
    public string affected_entity { get; set; } = string.Empty; // 影响的实体 如老人信息、系统公告

    [StringLength(20)]
    public string operation_status { get; set; } = string.Empty; // 操作状态 如成功、失败

    [StringLength(50)]
    public string ip_address { get; set; } = string.Empty; // IP地址

    [StringLength(50)]
    public string device_type { get; set; } = string.Empty; // 设备类型 如Web、移动端
}