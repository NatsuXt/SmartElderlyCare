using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ElderlyController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ElderlyController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // 1. 登记老人
    [HttpPost("register")]
    public IActionResult RegisterElderly([FromBody] RegisterElderlyDto dto)
    {
        int elderlyId = ElderlyService.Register(_configuration, dto);
        return Ok(new { success = true, elderly_id = elderlyId });
    }

    // 2. 健康评估与监测
    [HttpPost("assess")]
    public IActionResult AssessHealth([FromBody] HealthAssessmentDto dto)
    {
        ElderlyService.SaveAssessment(_configuration, dto);
        return Ok(new { success = true });
    }

    // 3. 家属关联
    [HttpPost("add-family")]
    public IActionResult AddFamily([FromBody] FamilyDto dto)
    {
        ElderlyService.SaveFamily(_configuration, dto);
        return Ok(new { success = true });
    }
}
