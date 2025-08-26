using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ElderlyCareSystem.Services;

namespace ElderlyCareSystem.Controllers
{
    /// <summary>
    /// 账号管理控制器
    /// 提供老人账号和家属账号的登录校验及修改密码接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        #region 登录接口

        /// <summary>
        /// 老人登录
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="password">密码</param>
        [HttpPost("elderly/login")]
        public async Task<IActionResult> ElderlyLogin(
            [FromQuery] int elderlyId,
            [FromQuery] string password)
        {
            var success = await _accountService.ElderlyLoginAsync(elderlyId, password);
            if (!success)
                return BadRequest("账号不存在或密码错误");

            return Ok("登录成功");
        }

        /// <summary>
        /// 家属登录
        /// </summary>
        /// <param name="familyId">家属ID</param>
        /// <param name="password">密码</param>
        [HttpPost("family/login")]
        public async Task<IActionResult> FamilyLogin(
            [FromQuery] int familyId,
            [FromQuery] string password)
        {
            var success = await _accountService.FamilyLoginAsync(familyId, password);
            if (!success)
                return BadRequest("账号不存在或密码错误");

            return Ok("登录成功");
        }

        #endregion

        #region 修改密码接口

        /// <summary>
        /// 老人修改密码
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        [HttpPost("elderly/change-password")]
        public async Task<IActionResult> ChangeElderlyPassword(
            [FromQuery] int elderlyId,
            [FromQuery] string oldPassword,
            [FromQuery] string newPassword)
        {
            var success = await _accountService.ChangeElderlyPasswordAsync(elderlyId, oldPassword, newPassword);
            if (!success)
                return BadRequest("账号不存在或旧密码错误");

            return Ok("密码修改成功");
        }

        /// <summary>
        /// 家属修改密码
        /// </summary>
        /// <param name="familyId">家属ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        [HttpPost("family/change-password")]
        public async Task<IActionResult> ChangeFamilyPassword(
            [FromQuery] int familyId,
            [FromQuery] string oldPassword,
            [FromQuery] string newPassword)
        {
            var success = await _accountService.ChangeFamilyPasswordAsync(familyId, oldPassword, newPassword);
            if (!success)
                return BadRequest("账号不存在或旧密码错误");

            return Ok("密码修改成功");
        }

        #endregion
    }
}
