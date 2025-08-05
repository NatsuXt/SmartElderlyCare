using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Interfaces;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 房间管理API控制器
    /// </summary>
    [ApiController]
    [Route("api/rooms")]
    [Produces("application/json")]
    public class RoomManagementApiController : ControllerBase
    {
        private readonly IRoomManagementService _roomService;

        public RoomManagementApiController(IRoomManagementService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// 获取所有房间
        /// </summary>
        /// <returns>房间列表</returns>
        [HttpGet]
        public ActionResult<List<RoomManagement>> GetAllRooms()
        {
            try
            {
                var rooms = _roomService.GetAllRooms();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间列表失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>房间信息</returns>
        [HttpGet("{roomId:int}")]
        public ActionResult<RoomManagement> GetRoomById(int roomId)
        {
            try
            {
                var room = _roomService.GetRoomById(roomId);
                if (room == null)
                {
                    return NotFound(new { message = $"未找到ID为 {roomId} 的房间" });
                }
                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据房间号获取房间
        /// </summary>
        /// <param name="roomNumber">房间号</param>
        /// <returns>房间信息</returns>
        [HttpGet("number/{roomNumber}")]
        public ActionResult<RoomManagement> GetRoomByNumber(string roomNumber)
        {
            try
            {
                var room = _roomService.GetRoomByNumber(roomNumber);
                if (room == null)
                {
                    return NotFound(new { message = $"未找到房间号为 {roomNumber} 的房间" });
                }
                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据楼层获取房间
        /// </summary>
        /// <param name="floor">楼层</param>
        /// <returns>房间列表</returns>
        [HttpGet("floor/{floor:int}")]
        public ActionResult<List<RoomManagement>> GetRoomsByFloor(int floor)
        {
            try
            {
                var rooms = _roomService.GetRoomsByFloor(floor);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取楼层房间失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据房间类型获取房间
        /// </summary>
        /// <param name="roomType">房间类型</param>
        /// <returns>房间列表</returns>
        [HttpGet("type/{roomType}")]
        public ActionResult<List<RoomManagement>> GetRoomsByType(string roomType)
        {
            try
            {
                var rooms = _roomService.GetRoomsByType(roomType);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间类型失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据状态获取房间
        /// </summary>
        /// <param name="status">房间状态</param>
        /// <returns>房间列表</returns>
        [HttpGet("status/{status}")]
        public ActionResult<List<RoomManagement>> GetRoomsByStatus(string status)
        {
            try
            {
                var rooms = _roomService.GetRoomsByStatus(status);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间状态失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="room">房间信息</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult AddRoom([FromBody] RoomManagement room)
        {
            try
            {
                if (room == null)
                {
                    return BadRequest(new { message = "房间信息不能为空" });
                }

                var result = _roomService.AddRoom(room);
                if (result)
                {
                    return Ok(new { message = "房间添加成功", roomId = room.RoomId });
                }
                else
                {
                    return BadRequest(new { message = "房间添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "添加房间失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="room">房间信息</param>
        /// <returns>操作结果</returns>
        [HttpPut("{roomId:int}")]
        public ActionResult UpdateRoom(int roomId, [FromBody] RoomManagement room)
        {
            try
            {
                if (room == null)
                {
                    return BadRequest(new { message = "房间信息不能为空" });
                }

                room.RoomId = roomId;
                var result = _roomService.UpdateRoom(room);
                if (result)
                {
                    return Ok(new { message = "房间更新成功" });
                }
                else
                {
                    return NotFound(new { message = "房间不存在或更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新房间失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{roomId:int}")]
        public ActionResult DeleteRoom(int roomId)
        {
            try
            {
                var result = _roomService.DeleteRoom(roomId);
                if (result)
                {
                    return Ok(new { message = "房间删除成功" });
                }
                else
                {
                    return NotFound(new { message = "房间不存在或删除失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "删除房间失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取可用房间
        /// </summary>
        /// <returns>可用房间列表</returns>
        [HttpGet("available")]
        public ActionResult<List<RoomManagement>> GetAvailableRooms()
        {
            try
            {
                var availableRooms = _roomService.GetAvailableRooms();
                return Ok(availableRooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取可用房间失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        [HttpGet("statistics")]
        public ActionResult GetRoomStatistics()
        {
            try
            {
                var statistics = _roomService.GetRoomStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间统计失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据价格范围获取房间
        /// </summary>
        /// <param name="minPrice">最低价格</param>
        /// <param name="maxPrice">最高价格</param>
        /// <returns>房间列表</returns>
        [HttpGet("price-range")]
        public ActionResult<List<RoomManagement>> GetRoomsByPriceRange([FromQuery] float minPrice, [FromQuery] float maxPrice)
        {
            try
            {
                var rooms = _roomService.GetRoomsByPriceRange(minPrice, maxPrice);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取价格范围房间失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 入住房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="checkInInfo">入住信息</param>
        /// <returns>操作结果</returns>
        [HttpPost("{roomId:int}/check-in")]
        public ActionResult CheckInRoom(int roomId, [FromBody] dynamic checkInInfo)
        {
            try
            {
                var result = _roomService.CheckInRoom(roomId, checkInInfo);
                if (result)
                {
                    return Ok(new { message = "入住成功" });
                }
                else
                {
                    return BadRequest(new { message = "入住失败，房间可能不可用" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "入住失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 退房
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{roomId:int}/check-out")]
        public ActionResult CheckOutRoom(int roomId)
        {
            try
            {
                var result = _roomService.CheckOutRoom(roomId);
                if (result)
                {
                    return Ok(new { message = "退房成功" });
                }
                else
                {
                    return BadRequest(new { message = "退房失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "退房失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 房间维护
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="maintenanceInfo">维护信息</param>
        /// <returns>操作结果</returns>
        [HttpPost("{roomId:int}/maintenance")]
        public ActionResult SetRoomMaintenance(int roomId, [FromBody] dynamic maintenanceInfo)
        {
            try
            {
                var result = _roomService.SetRoomMaintenance(roomId, maintenanceInfo);
                if (result)
                {
                    return Ok(new { message = "房间维护设置成功" });
                }
                else
                {
                    return BadRequest(new { message = "房间维护设置失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "房间维护设置失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 搜索房间
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public ActionResult<List<RoomManagement>> SearchRooms([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new { message = "搜索关键词不能为空" });
                }

                var rooms = _roomService.SearchRooms(keyword);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜索房间失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 批量更新房间状态
        /// </summary>
        /// <param name="roomIds">房间ID列表</param>
        /// <param name="status">新状态</param>
        /// <returns>操作结果</returns>
        [HttpPut("batch-status")]
        public ActionResult BatchUpdateRoomStatus([FromBody] List<int> roomIds, [FromQuery] string status)
        {
            try
            {
                if (roomIds == null || !roomIds.Any())
                {
                    return BadRequest(new { message = "房间ID列表不能为空" });
                }

                var result = _roomService.BatchUpdateRoomStatus(roomIds, status);
                return Ok(new { message = $"批量更新成功，共更新 {result} 个房间" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "批量更新房间状态失败", error = ex.Message });
            }
        }
    }
}
