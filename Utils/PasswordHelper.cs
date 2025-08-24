using System;
using System.Security.Cryptography;
using System.Text;

namespace ElderlyCareSystem.Utils
{
    public static class PasswordHelper
    {
        // 哈希密码
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        // 验证密码是否匹配
        public static bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}
