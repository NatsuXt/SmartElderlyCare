using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.DTOs;
using RoomDeviceManagement.Services;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 房间入住管理控制器
    /// 提供入住登记、退房登记、账单管理等API接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoomOccupancyController : ControllerBase
    {
        private readonly RoomManagementService _roomManagementService;
        private readonly ChineseCompatibleDatabaseService _chineseDbService;
        private readonly ILogger<RoomOccupancyController> _logger;

        public RoomOccupancyController(
            RoomManagementService roomManagementService,
            ChineseCompatibleDatabaseService chineseDbService,
            ILogger<RoomOccupancyController> logger)
        {
            _roomManagementService = roomManagementService;
            _chineseDbService = chineseDbService;
            _logger = logger;
        }

        /// <summary>
        /// 测试接口
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { 
                message = "房间入住管理系统正常运行！", 
                timestamp = DateTime.Now,
                version = "1.0.0"
            });
        }

        /// <summary>
        /// 根据老人ID获取入住记录
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>入住记录列表</returns>
        [HttpGet("elderly/{elderlyId}/occupancy-records")]
        public async Task<ActionResult<ApiResponse<List<OccupancyRecordDto>>>> GetOccupancyRecordsByElderlyId(decimal elderlyId)
        {
            try
            {
                _logger.LogInformation($"🔍 API请求：获取老人ID={elderlyId}的入住记录");
                
                var result = await _roomManagementService.GetOccupancyRecordsByElderlyIdAsync(elderlyId);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取老人ID={elderlyId}入住记录API异常");
                return StatusCode(500, new ApiResponse<List<OccupancyRecordDto>>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = new List<OccupancyRecordDto>()
                });
            }
        }

        /// <summary>
        /// 办理入住登记
        /// </summary>
        /// <param name="checkInDto">入住信息</param>
        /// <returns>入住记录</returns>
        [HttpPost("check-in")]
        public async Task<ActionResult<ApiResponse<OccupancyRecordDto>>> CheckIn([FromBody] CheckInDto checkInDto)
        {
            try
            {
                _logger.LogInformation($"🏠 API请求：办理入住登记，老人ID={checkInDto.ElderlyId}，房间ID={checkInDto.RoomId}");
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<OccupancyRecordDto>
                    {
                        Success = false,
                        Message = "请求参数验证失败",
                        Data = null
                    });
                }

                var result = await _roomManagementService.CheckInAsync(checkInDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "入住登记API异常");
                return StatusCode(500, new ApiResponse<OccupancyRecordDto>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 办理退房登记
        /// </summary>
        /// <param name="checkOutDto">退房信息</param>
        /// <returns>退房记录</returns>
        [HttpPost("check-out")]
        public async Task<ActionResult<ApiResponse<OccupancyRecordDto>>> CheckOut([FromBody] CheckOutDto checkOutDto)
        {
            try
            {
                _logger.LogInformation($"🚪 API请求：办理退房登记，入住记录ID={checkOutDto.OccupancyId}");
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<OccupancyRecordDto>
                    {
                        Success = false,
                        Message = "请求参数验证失败",
                        Data = null
                    });
                }

                var result = await _roomManagementService.CheckOutAsync(checkOutDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "退房登记API异常");
                return StatusCode(500, new ApiResponse<OccupancyRecordDto>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 一键生成所有房间账单
        /// </summary>
        /// <param name="generateDto">账单生成参数</param>
        /// <returns>生成的账单列表</returns>
        [HttpPost("billing/generate-all")]
        public async Task<ActionResult<ApiResponse<List<BillingRecordDto>>>> GenerateAllBillings([FromBody] GenerateBillDto generateDto)
        {
            try
            {
                _logger.LogInformation("💰 API请求：智能一键生成所有入住记录的账单（无时间限制，防重复生成）");
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<List<BillingRecordDto>>
                    {
                        Success = false,
                        Message = "请求参数验证失败",
                        Data = new List<BillingRecordDto>()
                    });
                }

                var result = await _roomManagementService.GenerateAllBillingsAsync(generateDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成所有账单API异常");
                return StatusCode(500, new ApiResponse<List<BillingRecordDto>>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = new List<BillingRecordDto>()
                });
            }
        }

        /// <summary>
        /// 根据老人ID生成账单
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="generateDto">账单生成参数</param>
        /// <returns>生成的账单列表</returns>
        [HttpPost("elderly/{elderlyId}/billing/generate")]
        public async Task<ActionResult<ApiResponse<List<BillingRecordDto>>>> GenerateBillingsForElderly(
            decimal elderlyId, 
            [FromBody] GenerateBillDto generateDto)
        {
            try
            {
                _logger.LogInformation($"💰 API请求：生成老人ID={elderlyId}的账单，时间段={generateDto.BillingStartDate:yyyy-MM-dd}到{generateDto.BillingEndDate:yyyy-MM-dd}");
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<List<BillingRecordDto>>
                    {
                        Success = false,
                        Message = "请求参数验证失败",
                        Data = new List<BillingRecordDto>()
                    });
                }

                var result = await _roomManagementService.GenerateBillingsForElderlyAsync(elderlyId, generateDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"生成老人ID={elderlyId}账单API异常");
                return StatusCode(500, new ApiResponse<List<BillingRecordDto>>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = new List<BillingRecordDto>()
                });
            }
        }

        /// <summary>
        /// 获取所有账单记录（分页）
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>分页的账单记录</returns>
        [HttpGet("billing/records")]
        public async Task<ActionResult<ApiResponse<PagedResult<BillingRecordDto>>>> GetBillingRecords(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation($"🔍 API请求：获取账单记录，页码={page}，每页={pageSize}");
                
                var request = new PagedRequest { Page = page, PageSize = pageSize };
                var result = await _roomManagementService.GetBillingRecordsAsync(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取账单记录API异常");
                return StatusCode(500, new ApiResponse<PagedResult<BillingRecordDto>>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = new PagedResult<BillingRecordDto>()
                });
            }
        }

        /// <summary>
        /// 根据老人ID查询账单记录（分页）
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>分页的账单记录</returns>
        [HttpGet("elderly/{elderlyId}/billing/records")]
        public async Task<ActionResult<ApiResponse<PagedResult<BillingRecordDto>>>> GetBillingRecordsByElderlyId(
            decimal elderlyId,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                _logger.LogInformation($"🔍 API请求：获取老人ID={elderlyId}的账单记录，页码={page}，每页={pageSize}");
                
                var request = new PagedRequest { Page = page, PageSize = pageSize };
                var result = await _roomManagementService.GetBillingRecordsAsync(request, elderlyId);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取老人ID={elderlyId}账单记录API异常");
                return StatusCode(500, new ApiResponse<PagedResult<BillingRecordDto>>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = new PagedResult<BillingRecordDto>()
                });
            }
        }

        /// <summary>
        /// 获取所有入住记录（分页）
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="status">入住状态筛选</param>
        /// <returns>分页的入住记录</returns>
        [HttpGet("occupancy-records")]
        public async Task<ActionResult<ApiResponse<PagedResult<OccupancyRecordDto>>>> GetAllOccupancyRecords(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null)
        {
            try
            {
                _logger.LogInformation($"🔍 API请求：获取所有入住记录，页码={page}，每页={pageSize}，状态筛选={status}");
                
                // 创建分页请求
                var request = new PagedRequest 
                { 
                    Page = page, 
                    PageSize = pageSize 
                };

                // 调用服务层获取入住记录
                var result = await _roomManagementService.GetAllOccupancyRecordsAsync(request, status);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取所有入住记录API异常");
                return StatusCode(500, new ApiResponse<PagedResult<OccupancyRecordDto>>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = new PagedResult<OccupancyRecordDto>()
                });
            }
        }

        #region 💰 支付管理API

        /// <summary>
        /// 处理账单支付
        /// </summary>
        [HttpPut("billing/{billingId}/payment")]
        public async Task<IActionResult> ProcessBillingPayment(int billingId, [FromBody] BillingPaymentDto request)
        {
            try
            {
                _logger.LogInformation($"💰 处理账单支付请求: 账单ID={billingId}");

                var result = await _chineseDbService.ProcessBillingPaymentAsync(
                    billingId, 
                    request.PaymentAmount, 
                    request.PaymentMethod, 
                    request.Remarks);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "账单支付成功",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"⚠️ 账单支付参数错误: {ex.Message}");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 账单支付失败: 账单ID={billingId}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "账单支付失败",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 部分支付
        /// </summary>
        [HttpPut("billing/{billingId}/partial-payment")]
        public async Task<IActionResult> ProcessPartialPayment(int billingId, [FromBody] PartialPaymentDto request)
        {
            try
            {
                _logger.LogInformation($"💸 处理部分支付请求: 账单ID={billingId}");

                var result = await _chineseDbService.ProcessBillingPaymentAsync(
                    billingId, 
                    request.PaymentAmount, 
                    request.PaymentMethod, 
                    $"【部分支付】{request.Remarks}");

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "部分支付成功",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"⚠️ 部分支付参数错误: {ex.Message}");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 部分支付失败: 账单ID={billingId}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "部分支付失败",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 获取支付历史
        /// </summary>
        [HttpGet("billing/{billingId}/payment-history")]
        public async Task<IActionResult> GetPaymentHistory(int billingId)
        {
            try
            {
                _logger.LogInformation($"📋 获取支付历史: 账单ID={billingId}");

                var history = await _chineseDbService.GetPaymentHistoryAsync(billingId);

                return Ok(new ApiResponse<List<object>>
                {
                    Success = true,
                    Message = $"成功获取支付历史，共{history.Count}条记录",
                    Data = history
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 获取支付历史失败: 账单ID={billingId}");
                return StatusCode(500, new ApiResponse<List<object>>
                {
                    Success = false,
                    Message = "获取支付历史失败",
                    Data = new List<object>()
                });
            }
        }

        #endregion
    }
}
