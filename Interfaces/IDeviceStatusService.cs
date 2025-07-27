using System.Collections.Generic;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Interfaces
{
    /// <summary>
    /// 设备状态服务接口
    /// </summary>
    public interface IDeviceStatusService
    {
        /// <summary>
        /// 获取所有设备状态
        /// </summary>
        /// <returns>设备状态列表</returns>
        List<DeviceStatus> GetAllDevices();

        /// <summary>
        /// 根据设备ID获取设备状态
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备状态</returns>
        DeviceStatus? GetDeviceById(string deviceId);

        /// <summary>
        /// 根据房间ID获取设备列表
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <returns>设备列表</returns>
        List<DeviceStatus> GetDevicesByRoomId(string roomId);

        /// <summary>
        /// 根据设备状态获取设备列表
        /// </summary>
        /// <param name="status">设备状态</param>
        /// <returns>设备列表</returns>
        List<DeviceStatus> GetDevicesByStatus(string status);

        /// <summary>
        /// 根据设备类型获取设备列表
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <returns>设备列表</returns>
        List<DeviceStatus> GetDevicesByType(string deviceType);

        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="device">设备信息</param>
        /// <returns>操作是否成功</returns>
        bool AddDevice(DeviceStatus device);

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="device">设备信息</param>
        /// <returns>操作是否成功</returns>
        bool UpdateDevice(DeviceStatus device);

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>操作是否成功</returns>
        bool DeleteDevice(string deviceId);

        /// <summary>
        /// 获取故障设备列表
        /// </summary>
        /// <returns>故障设备列表</returns>
        List<DeviceStatus> GetFaultyDevices();

        /// <summary>
        /// 获取离线设备列表
        /// </summary>
        /// <returns>离线设备列表</returns>
        List<DeviceStatus> GetOfflineDevices();

        /// <summary>
        /// 获取低电量设备列表
        /// </summary>
        /// <param name="threshold">电量阈值</param>
        /// <returns>低电量设备列表</returns>
        List<DeviceStatus> GetLowBatteryDevices(int threshold = 20);

        /// <summary>
        /// 更新设备在线状态
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="isOnline">是否在线</param>
        /// <returns>操作是否成功</returns>
        bool UpdateDeviceOnlineStatus(string deviceId, bool isOnline);
    }
}
