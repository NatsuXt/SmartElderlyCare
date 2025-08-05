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

        /// <summary>
        /// 根据房间号获取房间信息
        /// </summary>
        /// <param name="roomNumber">房间号</param>
        /// <returns>房间信息</returns>
        RoomManagement? GetRoomByNumber(string roomNumber);

        /// <summary>
        /// 根据房间类型获取房间列表
        /// </summary>
        /// <param name="roomType">房间类型</param>
        /// <returns>房间列表</returns>
        List<RoomManagement> GetRoomsByType(string roomType);

        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        object GetRoomStatistics();

        /// <summary>
        /// 根据价格范围获取房间
        /// </summary>
        /// <param name="minPrice">最低价格</param>
        /// <param name="maxPrice">最高价格</param>
        /// <returns>房间列表</returns>
        List<RoomManagement> GetRoomsByPriceRange(float minPrice, float maxPrice);

        /// <summary>
        /// 入住房间
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="checkInInfo">入住信息</param>
        /// <returns>操作是否成功</returns>
        bool CheckInRoom(int roomId, object checkInInfo);

        /// <summary>
        /// 退房
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>操作是否成功</returns>
        bool CheckOutRoom(int roomId);

        /// <summary>
        /// 设置房间维护状态
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="maintenanceInfo">维护信息</param>
        /// <returns>操作是否成功</returns>
        bool SetRoomMaintenance(int roomId, object maintenanceInfo);

        /// <summary>
        /// 搜索房间
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>房间列表</returns>
        List<RoomManagement> SearchRooms(string keyword);

        /// <summary>
        /// 批量更新房间状态
        /// </summary>
        /// <param name="roomIds">房间ID列表</param>
        /// <param name="status">新状态</param>
        /// <returns>更新成功的房间数量</returns>
        int BatchUpdateRoomStatus(List<int> roomIds, string status);
    }
}
