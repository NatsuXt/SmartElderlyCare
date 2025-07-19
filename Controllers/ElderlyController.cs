using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

[ApiController]
[Route("api/[controller]")]
public class ElderlyController : ControllerBase
{
    private readonly ILogger<ElderlyController> _logger;
    private readonly IConfiguration _configuration;

    public ElderlyController(ILogger<ElderlyController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public IActionResult RegisterElderly([FromBody] ElderlyDto dto)
    {
        try
        {
            var service = new ElderlyRegistrationService(_configuration);
            int 老人ID = service.RegisterElderly(dto);
            return Ok(new { 成功 = true, 老人ID = 老人ID });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登记失败");
            return StatusCode(500, new { 成功 = false, 错误信息 = ex.Message });
        }
    }
}
