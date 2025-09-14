using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ElderlyCareSystem.Models;

[ApiController]
[Route("api/[controller]")]
public class VisitorLoginController : ControllerBase
{
    private readonly VisitorLoginService _visitorLoginService;
    private readonly OperationLogService _logService;

    public VisitorLoginController(VisitorLoginService visitorLoginService, OperationLogService logService)
    {
        _visitorLoginService = visitorLoginService;
        _logService = logService;
    }

    /// <summary>
    /// 访客账号注册
    /// </summary>
    /// <remarks>
    /// **功能说明：** 为访客创建新的登录账号，用于后续的访客登录和探视预约。
    /// 
    /// **业务逻辑：**
    /// 1. 验证输入参数的有效性（姓名、手机号、密码格式）
    /// 2. 检查手机号是否已被注册（每个手机号只能注册一次）
    /// 3. 对密码进行SHA256加密存储
    /// 4. 创建访客账号记录并返回访客信息
    /// 
    /// **输入要求：**
    /// - 访客姓名：1-50个字符，不能为空
    /// - 手机号：符合手机号格式，最多20位，全系统唯一
    /// - 密码：1-100个字符，不能为空
    /// 
    /// **成功返回：** 201 Created + 访客信息
    /// **失败返回：** 400 Bad Request（手机号已注册/参数验证失败） 或 500 Internal Server Error
    /// 
    /// **示例请求：**
    /// ```json
    /// {
    ///   "visitorName": "张三",
    ///   "visitorPhone": "13800138000",
    ///   "visitorPassword": "myPassword123"
    /// }
    /// ```
    /// 
    /// **示例响应：**
    /// ```json
    /// {
    ///   "visitorId": 1,
    ///   "visitorName": "张三",
    ///   "visitorPhone": "13800138000"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">访客注册请求对象</param>
    /// <returns>注册成功返回访客信息，失败返回错误信息</returns>
    /// <response code="201">注册成功，返回访客信息</response>
    /// <response code="400">请求参数错误或手机号已被注册</response>
    /// <response code="500">系统内部错误</response>
    [HttpPost("register")]
    public IActionResult Register([FromBody] VisitorRegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = _visitorLoginService.Register(request);
            return CreatedAtAction(nameof(GetVisitorInfo), new { id = result.VisitorId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { 
                Message = "注册失败", 
                Error = ex.Message,
                Type = "ValidationError"
            });
        }
        catch (Exception ex)
        {
            // 记录详细错误信息到控制台
            Console.WriteLine($"访客注册错误: {ex.Message}");
            Console.WriteLine($"错误详情: {ex}");
            
            return StatusCode(500, new { 
                Message = "系统内部错误，请稍后重试", 
                Error = ex.Message,  // 在开发环境下显示具体错误
                Type = "SystemError"
            });
        }
    }

    /// <summary>
    /// 访客账号登录
    /// </summary>
    /// <remarks>
    /// **功能说明：** 访客使用手机号和密码进行身份验证，获取访问权限。
    /// 
    /// **业务逻辑：**
    /// 1. 根据手机号查找访客账号
    /// 2. 验证密码是否正确（SHA256哈希比对）
    /// 3. 登录成功返回访客基本信息
    /// 4. 登录失败返回具体失败原因
    /// 
    /// **输入要求：**
    /// - 手机号：必填，已注册的手机号
    /// - 密码：必填，注册时设置的密码
    /// 
    /// **验证规则：**
    /// - 手机号必须在系统中存在
    /// - 密码必须与注册时的密码一致
    /// 
    /// **成功返回：** 200 OK + 访客信息
    /// **失败返回：** 400 Bad Request（手机号不存在/密码错误） 或 500 Internal Server Error
    /// 
    /// **示例请求：**
    /// ```json
    /// {
    ///   "visitorPhone": "13800138000",
    ///   "visitorPassword": "myPassword123"
    /// }
    /// ```
    /// 
    /// **示例响应（成功）：**
    /// ```json
    /// {
    ///   "message": "登录成功",
    ///   "visitorId": 1,
    ///   "visitorName": "张三",
    ///   "visitorPhone": "13800138000"
    /// }
    /// ```
    /// 
    /// **示例响应（失败）：**
    /// ```json
    /// {
    ///   "message": "登录失败",
    ///   "error": "手机号不存在" // 或 "密码错误",
    ///   "type": "ValidationError"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">访客登录请求对象</param>
    /// <returns>登录成功返回访客信息，失败返回错误信息</returns>
    /// <response code="200">登录成功，返回访客信息</response>
    /// <response code="400">手机号不存在或密码错误</response>
    /// <response code="500">系统内部错误</response>
    [HttpPost("login")]
    public IActionResult Login([FromBody] VisitorLoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = _visitorLoginService.Login(request);
            return Ok(new { 
                Message = "登录成功",
                VisitorId = result.VisitorId,
                VisitorName = result.VisitorName,
                VisitorPhone = result.VisitorPhone
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { 
                Message = "登录失败", 
                Error = ex.Message,
                Type = "ValidationError"
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new { 
                Message = "系统内部错误，请稍后重试", 
                Error = "Internal Server Error",
                Type = "SystemError"
            });
        }
    }

    /// <summary>
    /// 获取访客信息
    /// </summary>
    /// <remarks>
    /// **功能说明：** 根据访客ID查询访客的详细信息。
    /// 
    /// **业务逻辑：**
    /// 1. 根据访客ID从数据库查找访客记录
    /// 2. 如果访客存在，返回访客的基本信息
    /// 3. 如果访客不存在，返回404错误
    /// 
    /// **输入要求：**
    /// - 访客ID：路径参数，正整数，必填
    /// 
    /// **返回信息：**
    /// - 访客ID、姓名、手机号（不包含密码等敏感信息）
    /// 
    /// **成功返回：** 200 OK + 访客信息
    /// **失败返回：** 404 Not Found（访客不存在） 或 500 Internal Server Error
    /// 
    /// **使用场景：**
    /// - 访客登录后获取个人信息
    /// - 管理员查看访客详情
    /// - 探视预约时确认访客身份
    /// 
    /// **示例请求：**
    /// ```
    /// GET /api/VisitorLogin/1
    /// ```
    /// 
    /// **示例响应（成功）：**
    /// ```json
    /// {
    ///   "visitorId": 1,
    ///   "visitorName": "张三",
    ///   "visitorPhone": "13800138000"
    /// }
    /// ```
    /// 
    /// **示例响应（失败）：**
    /// ```json
    /// "访客ID 999 不存在"
    /// ```
    /// </remarks>
    /// <param name="id">访客ID，正整数</param>
    /// <returns>访客信息或错误信息</returns>
    /// <response code="200">查询成功，返回访客信息</response>
    /// <response code="404">访客ID不存在</response>
    /// <response code="500">系统内部错误</response>
    [HttpGet("{id}")]
    public IActionResult GetVisitorInfo(int id)
    {
        try
        {
            var visitor = _visitorLoginService.GetVisitorById(id);
            if (visitor == null)
                return NotFound($"访客ID {id} 不存在");

            return Ok(visitor);
        }
        catch (Exception)
        {
            return StatusCode(500, new { 
                Message = "获取访客信息失败", 
                Error = "Internal Server Error"
            });
        }
    }

    /// <summary>
    /// 修改访客密码
    /// </summary>
    /// <remarks>
    /// **功能说明：** 访客修改登录密码，需要验证原密码。
    /// 
    /// **业务逻辑：**
    /// 1. 根据访客ID查找访客账号
    /// 2. 验证原密码是否正确
    /// 3. 如果原密码正确，更新为新密码（SHA256加密）
    /// 4. 保存更改并返回操作结果
    /// 
    /// **输入要求：**
    /// - 访客ID：路径参数，正整数，必填
    /// - 原密码：请求体，必填，当前登录密码
    /// - 新密码：请求体，1-100个字符，不能为空
    /// 
    /// **安全规则：**
    /// - 必须验证原密码正确才能修改
    /// - 新密码会重新进行SHA256哈希加密
    /// - 密码修改后立即生效
    /// 
    /// **成功返回：** 200 OK + 成功消息
    /// **失败返回：** 400 Bad Request（原密码错误/访客不存在） 或 500 Internal Server Error
    /// 
    /// **使用场景：**
    /// - 访客忘记密码后重设
    /// - 定期更换密码提高安全性
    /// - 账号安全维护
    /// 
    /// **示例请求：**
    /// ```
    /// PUT /api/VisitorLogin/1/change-password
    /// ```
    /// ```json
    /// {
    ///   "oldPassword": "myOldPassword123",
    ///   "newPassword": "myNewPassword456"
    /// }
    /// ```
    /// 
    /// **示例响应（成功）：**
    /// ```json
    /// {
    ///   "message": "密码修改成功"
    /// }
    /// ```
    /// 
    /// **示例响应（失败）：**
    /// ```json
    /// "原密码错误或访客不存在"
    /// ```
    /// </remarks>
    /// <param name="id">访客ID，正整数</param>
    /// <param name="request">密码修改请求对象，包含原密码和新密码</param>
    /// <returns>修改成功返回成功消息，失败返回错误信息</returns>
    /// <response code="200">密码修改成功</response>
    /// <response code="400">原密码错误、访客不存在或参数验证失败</response>
    /// <response code="500">系统内部错误</response>
    [HttpPut("{id}/change-password")]
    public IActionResult ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var success = _visitorLoginService.ChangePassword(id, request.OldPassword, request.NewPassword);
            if (!success)
                return BadRequest("原密码错误或访客不存在");

            return Ok(new { Message = "密码修改成功" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { 
                Message = "密码修改失败", 
                Error = "Internal Server Error"
            });
        }
    }
}

// 请求模型

/// <summary>
/// 访客注册请求模型
/// </summary>
public class VisitorRegisterRequest
{
    /// <summary>
    /// 访客姓名
    /// </summary>
    /// <example>张三</example>
    [Required(ErrorMessage = "访客姓名不能为空")]
    [StringLength(50, ErrorMessage = "访客姓名不能超过50个字符")]
    public string VisitorName { get; set; } = string.Empty;

    /// <summary>
    /// 访客手机号（全系统唯一）
    /// </summary>
    /// <example>13800138000</example>
    [Required(ErrorMessage = "访客手机号不能为空")]
    [Phone(ErrorMessage = "手机号格式不正确")]
    [StringLength(20, ErrorMessage = "手机号不能超过20个字符")]
    public string VisitorPhone { get; set; } = string.Empty;

    /// <summary>
    /// 访客登录密码（会进行SHA256加密存储）
    /// </summary>
    /// <example>myPassword123</example>
    [Required(ErrorMessage = "访客密码不能为空")]
    [StringLength(100, ErrorMessage = "密码不能超过100个字符")]
    public string VisitorPassword { get; set; } = string.Empty;
}

/// <summary>
/// 访客登录请求模型
/// </summary>
public class VisitorLoginRequest
{
    /// <summary>
    /// 访客手机号（注册时使用的手机号）
    /// </summary>
    /// <example>13800138000</example>
    [Required(ErrorMessage = "访客手机号不能为空")]
    public string VisitorPhone { get; set; } = string.Empty;

    /// <summary>
    /// 访客登录密码（注册时设置的密码）
    /// </summary>
    /// <example>myPassword123</example>
    [Required(ErrorMessage = "访客密码不能为空")]
    public string VisitorPassword { get; set; } = string.Empty;
}

/// <summary>
/// 修改密码请求模型
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// 原密码（当前登录密码，用于验证身份）
    /// </summary>
    /// <example>myOldPassword123</example>
    [Required(ErrorMessage = "原密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码（要设置的新登录密码）
    /// </summary>
    /// <example>myNewPassword456</example>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, ErrorMessage = "新密码不能超过100个字符")]
    public string NewPassword { get; set; } = string.Empty;
}

