using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ElderlyCareSystem.Models;

public class VisitorLoginService
{
    private readonly AppDbContext _context;

    public VisitorLoginService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 访客账号注册
    /// </summary>
    /// <param name="request">注册请求</param>
    /// <returns>注册结果</returns>
    public VisitorLogin Register(VisitorRegisterRequest request)
    {
        try
        {
            // 检查手机号是否已存在 - 修改为Count方式避免Oracle的Any()问题
            var existingCount = _context.VisitorLogins.Count(v => v.VisitorPhone == request.VisitorPhone);
            if (existingCount > 0)
            {
                throw new ArgumentException("该手机号已被注册");
            }

            // 创建新访客账号
            var visitor = new VisitorLogin
            {
                VisitorName = request.VisitorName,
                VisitorPhone = request.VisitorPhone,
                VisitorPassword = HashPassword(request.VisitorPassword)
            };

            _context.VisitorLogins.Add(visitor);
            _context.SaveChanges();

            return visitor;
        }
        catch (Exception ex)
        {
            // 记录详细错误信息
            Console.WriteLine($"注册服务错误: {ex.Message}");
            Console.WriteLine($"完整异常: {ex}");
            throw; // 重新抛出异常
        }
    }

    /// <summary>
    /// 访客账号登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    public VisitorLogin Login(VisitorLoginRequest request)
    {
        var visitor = _context.VisitorLogins.FirstOrDefault(v => v.VisitorPhone == request.VisitorPhone);
        
        if (visitor == null)
        {
            throw new ArgumentException("手机号不存在");
        }

        if (!VerifyPassword(request.VisitorPassword, visitor.VisitorPassword))
        {
            throw new ArgumentException("密码错误");
        }

        return visitor;
    }

    /// <summary>
    /// 根据ID获取访客信息
    /// </summary>
    /// <param name="id">访客ID</param>
    /// <returns>访客信息</returns>
    public VisitorLogin? GetVisitorById(int id)
    {
        return _context.VisitorLogins.FirstOrDefault(v => v.VisitorId == id);
    }

    /// <summary>
    /// 根据手机号获取访客信息
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <returns>访客信息</returns>
    public VisitorLogin? GetVisitorByPhone(string phone)
    {
        return _context.VisitorLogins.FirstOrDefault(v => v.VisitorPhone == phone);
    }

    /// <summary>
    /// 修改访客密码
    /// </summary>
    /// <param name="id">访客ID</param>
    /// <param name="oldPassword">原密码</param>
    /// <param name="newPassword">新密码</param>
    /// <returns>是否修改成功</returns>
    public bool ChangePassword(int id, string oldPassword, string newPassword)
    {
        var visitor = _context.VisitorLogins.FirstOrDefault(v => v.VisitorId == id);
        
        if (visitor == null)
        {
            return false;
        }

        if (!VerifyPassword(oldPassword, visitor.VisitorPassword))
        {
            return false;
        }

        visitor.VisitorPassword = HashPassword(newPassword);
        _context.SaveChanges();

        return true;
    }

    /// <summary>
    /// 验证访客是否存在
    /// </summary>
    /// <param name="visitorId">访客ID</param>
    /// <returns>是否存在</returns>
    public bool VisitorExists(int visitorId)
    {
        return _context.VisitorLogins.Count(v => v.VisitorId == visitorId) > 0;
    }

    /// <summary>
    /// 哈希密码
    /// </summary>
    /// <param name="password">原始密码</param>
    /// <returns>哈希后的密码</returns>
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="inputPassword">输入的密码</param>
    /// <param name="hashedPassword">哈希后的密码</param>
    /// <returns>是否匹配</returns>
    private bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        var inputHash = HashPassword(inputPassword);
        return inputHash == hashedPassword;
    }
}

