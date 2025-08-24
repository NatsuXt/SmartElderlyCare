using ElderlyCareSystem.Models;
using ElderlyCareSystem.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ELDERLYACCOUNT")]
public class ElderlyAccount
{
    [Key]
    [Column("ACCOUNT_ID")]
    public int AccountId { get; set; }  // 主键，自增

    [Column("ELDERLY_ID")]
    [ForeignKey(nameof(ElderlyInfo))]
    public int ElderlyId { get; set; }  // 老人ID，用作账号，一对一绑定

    [Column("PASSWORD_HASH"), MaxLength(200)]
    public string PasswordHash { get; set; }  // 存储加密后的密码

    // 一对一导航属性
    public ElderlyInfo ElderlyInfo { get; set; }

    // 构造函数，初始化默认密码为 "0000" 的哈希
    public ElderlyAccount()
    {
        PasswordHash = PasswordHelper.HashPassword("0000");
    }
}
