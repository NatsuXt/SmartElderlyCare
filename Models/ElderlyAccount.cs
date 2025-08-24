using ElderlyCareSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ELDERLYACCOUNT")]
public class ElderlyAccount
{
    [Key]
    [Column("ACCOUNT_ID")]
    public int AccountId { get; set; }  // 主键，独立的账号ID（自增）

    [Column("ELDERLY_ID")]
    [ForeignKey(nameof(ElderlyInfo))]
    public int ElderlyId { get; set; }  // 老人ID，用作账号，一对一绑定

    [Column("PASSWORD"), MaxLength(100)]
    public string Password { get; set; } = "0000";  // 初始密码为0000

    // 一对一导航属性
    public ElderlyInfo ElderlyInfo { get; set; }
}
