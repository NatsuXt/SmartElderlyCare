using ElderlyCareSystem.Data;
using ElderlyCareSystem.Models;
using ElderlyCareSystem.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ElderlyCareSystem.Services
{
    public class AccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 老人登录校验：根据老人ID和密码验证
        /// </summary>
        public async Task<bool> ElderlyLoginAsync(int elderlyId, string password)
        {
            var account = await _context.ElderlyAccounts
                .FirstOrDefaultAsync(a => a.ElderlyId == elderlyId);

            if (account == null)
            {
                Console.WriteLine($"老人ID {elderlyId} 不存在");
                return false;
            }

            if (!PasswordHelper.VerifyPassword(password, account.PasswordHash))
            {
                Console.WriteLine($"老人ID {elderlyId} 密码错误");
                return false;
            }

            Console.WriteLine($"老人ID {elderlyId} 登录成功");
            return true;
        }

        /// <summary>
        /// 家属登录校验：根据家属ID和密码验证
        /// </summary>
        public async Task<bool> FamilyLoginAsync(int familyId, string password)
        {
            var account = await _context.FamilyAccounts
                .FirstOrDefaultAsync(a => a.FamilyId == familyId);

            if (account == null)
            {
                Console.WriteLine($"家属ID {familyId} 不存在");
                return false;
            }

            if (!PasswordHelper.VerifyPassword(password, account.PasswordHash))
            {
                Console.WriteLine($"家属ID {familyId} 密码错误");
                return false;
            }

            Console.WriteLine($"家属ID {familyId} 登录成功");
            return true;
        }

        /// <summary>
        /// 修改老人账号密码
        /// </summary>
        public async Task<bool> ChangeElderlyPasswordAsync(int elderlyId, string oldPassword, string newPassword)
        {
            var account = await _context.ElderlyAccounts
                .FirstOrDefaultAsync(a => a.ElderlyId == elderlyId); // 按老人ID查找
            if (account == null)
            {
                Console.WriteLine($"老人ID {elderlyId} 不存在");
                return false;
            }

            // 打印调试信息
            Console.WriteLine($"输入旧密码明文: {oldPassword}");
            Console.WriteLine($"旧密码的哈希: {PasswordHelper.HashPassword(oldPassword)}");
            Console.WriteLine($"数据库存储哈希: {account.PasswordHash}");

            // 校验旧密码
            if (!PasswordHelper.VerifyPassword(oldPassword, account.PasswordHash))
            {
                Console.WriteLine("旧密码校验失败");
                return false;
            }

            // 设置新密码
            account.PasswordHash = PasswordHelper.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            Console.WriteLine($"老人ID {elderlyId} 密码已更新");
            return true;
        }

        /// <summary>
        /// 修改家属账号密码
        /// </summary>
        public async Task<bool> ChangeFamilyPasswordAsync(int familyId, string oldPassword, string newPassword)
        {
            var account = await _context.FamilyAccounts.FirstOrDefaultAsync(a => a.FamilyId == familyId);
            if (account == null)
            {
                Console.WriteLine($"家属ID {familyId} 不存在");
                return false;
            }

            // 打印调试信息
            Console.WriteLine($"输入旧密码明文: {oldPassword}");
            Console.WriteLine($"旧密码的哈希: {PasswordHelper.HashPassword(oldPassword)}");
            Console.WriteLine($"数据库存储哈希: {account.PasswordHash}");

            // 校验旧密码
            if (!PasswordHelper.VerifyPassword(oldPassword, account.PasswordHash))
            {
                Console.WriteLine("旧密码校验失败");
                return false;
            }

            // 设置新密码
            account.PasswordHash = PasswordHelper.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            Console.WriteLine($"家属ID {familyId} 密码已更新");
            return true;
        }
    }
}
