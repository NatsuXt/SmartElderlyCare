using System.Collections.Generic;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Interfaces
{
    /// <summary>
    /// 房间管理服务接口
    /// </summary>
    public interface IRoomManagementService
    {
        /// <summary>
        /// 获取所有房间信息
        /// </summary>
        /// <returns>房间列表</returns>
        List<RoomManagement> GetAllRooms();

        /// <summary>
        /// 根据房间ID获取房间信息
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>房间信息</returns>
        RoomManagement? GetRoomById(int roomId);

        /// <summary>
        /// 根据房间状态获取房间列表
        /// </summary>
        /// <param name="status">房间状态</param>
        /// <returns>房间列表</returns>
        List<RoomManagement> GetRoomsByStatus(string status);

        /// <summary>
        /// 根据楼层获取房间列表
        /// </summary>
        /// <param name="floor">楼层</param>
        /// <returns>房间列表</returns>
        List<RoomManagement> GetRoomsByFloor(int floor);

        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="room">房间信息</param>
        /// <returns>操作是否成功</returns>
        bool AddRoom(RoomManagement room);

        /// <summary>
        /// 更新房间信息
        /// </summary>
        /// <param name="room">房间信息</param>
        /// <returns>操作是否成功</returns>
        bool UpdateRoom(RoomManagement room);

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>操作是否成功</returns>
        bool DeleteRoom(int roomId);

        /// <summary>
        /// 获取空闲房间列表
        /// </summary>
        /// <returns>空闲房间列表</returns>
        List<RoomManagement> GetAvailableRooms();

        /// <summary>
        /// 检查房间是否可入住
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>是否可入住</returns>
        bool IsRoomAvailable(int roomId);
    }
}
