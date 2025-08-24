using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElderlyCareSystem.Utils; // 假设密码工具类在这里

namespace ElderlyCareSystem.Models
{
    [Table("FAMILYACCOUNT")]
    public class FamilyAccount
    {
        [Key]
        [Column("ACCOUNT_ID")]
        public int AccountId { get; set; }

        [Required]
        [Column("USERNAME"), MaxLength(50)]
        public string Username { get; set; }  // 可以用 FamilyId 或自定义账号名

        [Required]
        [Column("PASSWORD_HASH"), MaxLength(200)]
        public string PasswordHash { get; set; }

        // 外键指向 FamilyInfo
        [ForeignKey(nameof(FamilyInfo))]
        [Column("FAMILY_ID")]
        public int FamilyId { get; set; }

        public FamilyInfo FamilyInfo { get; set; }

        // 构造函数，初始化默认密码为 "0000" 的哈希
        public FamilyAccount()
        {
            PasswordHash = PasswordHelper.HashPassword("0000");
        }
    }
}
