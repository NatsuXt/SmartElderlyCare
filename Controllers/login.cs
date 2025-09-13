using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staff_Info.Data;
using Staff_Info.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Staff_Info.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StaffInfoDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(StaffInfoDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                // 基本验证
                if (loginModel.STAFF_ID <= 0 || string.IsNullOrEmpty(loginModel.Password))
                {
                    return BadRequest("STAFF_ID和密码不能为空");
                }

                // 直接查询账户
                var account = await _context.StaffAccount
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sa => sa.STAFF_ID == loginModel.STAFF_ID);

                // 验证账户是否存在和密码是否正确
                if (account == null || account.PASSWORD_HASH != loginModel.Password)
                {
                    _logger.LogWarning($"登录失败: STAFF_ID {loginModel.STAFF_ID}");
                    return Unauthorized("STAFF_ID或密码错误");
                }

                // 仅返回STAFF_ID表示登录成功
                return Ok(new { STAFF_ID = account.STAFF_ID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登录过程中发生错误");
                return StatusCode(500, "登录过程中发生错误");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                // 基本验证
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 查询账户
                var account = await _context.StaffAccount
                    .FirstOrDefaultAsync(sa => sa.STAFF_ID == model.STAFF_ID);

                // 验证账户是否存在和旧密码是否正确
                if (account == null || account.PASSWORD_HASH != model.OldPassword)
                {
                    _logger.LogWarning($"密码修改失败: STAFF_ID {model.STAFF_ID}");
                    return Unauthorized("STAFF_ID或旧密码错误");
                }

                // 更新密码
                account.PASSWORD_HASH = model.NewPassword;
                _context.StaffAccount.Update(account);

                await _context.SaveChangesAsync();
                _logger.LogInformation($"密码修改成功: STAFF_ID {model.STAFF_ID}");
                return Ok(new { Message = "密码修改成功" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "修改密码时发生数据库错误");
                return StatusCode(500, "修改密码时发生错误");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "修改密码时发生未知错误");
                return StatusCode(500, "修改密码时发生错误");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                // 基本验证
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 查询账户
                var account = await _context.StaffAccount
                    .FirstOrDefaultAsync(sa => sa.STAFF_ID == model.STAFF_ID);

                // 验证账户是否存在
                if (account == null)
                {
                    _logger.LogWarning($"密码重置失败: STAFF_ID {model.STAFF_ID} 不存在");
                    return NotFound("STAFF_ID不存在");
                }

                // 更新密码
                account.PASSWORD_HASH = model.NewPassword;
                _context.StaffAccount.Update(account);

                await _context.SaveChangesAsync();
                _logger.LogInformation($"密码重置成功: STAFF_ID {model.STAFF_ID}");
                return Ok(new { Message = "密码重置成功" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "重置密码时发生数据库错误");
                return StatusCode(500, "重置密码时发生错误");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置密码时发生未知错误");
                return StatusCode(500, "重置密码时发生错误");
            }
        }

        [HttpPost("add-password")]
        public async Task<IActionResult> AddPassword([FromBody] AddPasswordModel model)
        {
            try
            {
                // 基本验证
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 检查是否已存在该STAFF_ID的记录
                var existingAccount = await _context.StaffAccount
                    .FirstOrDefaultAsync(sa => sa.STAFF_ID == model.STAFF_ID);

                if (existingAccount != null)
                {
                    _logger.LogWarning($"添加密码失败: STAFF_ID {model.STAFF_ID} 已存在");
                    return Conflict("该STAFF_ID已存在密码记录");
                }

                // 创建新记录
                var newAccount = new STAFFACCOUNT
                {
                    STAFF_ID = model.STAFF_ID,
                    PASSWORD_HASH = model.Password
                };

                _context.StaffAccount.Add(newAccount);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"密码添加成功: STAFF_ID {model.STAFF_ID}");
                return Ok(new { Message = "密码添加成功" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "添加密码时发生数据库错误");
                return StatusCode(500, "添加密码时发生错误");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加密码时发生未知错误");
                return StatusCode(500, "添加密码时发生错误");
            }
        }
    }

    public class LoginModel
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "STAFF_ID必须大于0")]
        public decimal STAFF_ID { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "STAFF_ID必须大于0")]
        public decimal STAFF_ID { get; set; }

        [Required(ErrorMessage = "旧密码不能为空")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "新密码不能为空")]
        [MinLength(6, ErrorMessage = "新密码至少需要6个字符")]
        public string NewPassword { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "STAFF_ID必须大于0")]
        public decimal STAFF_ID { get; set; }

        [Required(ErrorMessage = "新密码不能为空")]
        [MinLength(6, ErrorMessage = "新密码至少需要6个字符")]
        public string NewPassword { get; set; }
    }

    public class AddPasswordModel
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "STAFF_ID必须大于0")]
        public decimal STAFF_ID { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码至少需要6个字符")]
        public string Password { get; set; }
    }
}