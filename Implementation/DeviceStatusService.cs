using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using RoomDeviceManagement.Interfaces;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;

namespace RoomDeviceManagement.Implementation
{
    /// <summary>
    /// 设备状态服务实现类
    /// </summary>
    public class DeviceStatusService : IDeviceStatusService
    {
        private readonly DatabaseService _databaseService;

        public DeviceStatusService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<DeviceStatus> GetAllDevices()
        {
            var devices = new List<DeviceStatus>();
            var sql = @"SELECT device_id, device_name, device_type, installation_date, 
                              status, last_maintenance_date, maintenance_status, 
                              location, created_time, updated_time 
                       FROM DeviceStatus ORDER BY device_id";

            try
            {
                var dataTable = _databaseService.ExecuteQuery(sql);
                foreach (DataRow row in dataTable.Rows)
                {
                    devices.Add(MapDataRowToDevice(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有设备信息失败：{ex.Message}");
            }

            return devices;
        }

        public DeviceStatus? GetDeviceById(string deviceId)
        {
            var sql = @"SELECT device_id, device_name, device_type, installation_date, 
                              status, last_maintenance_date, maintenance_status, 
                              location, created_time, updated_time 
                       FROM DeviceStatus WHERE device_id = :deviceId";

            try
            {
                var parameters = new[] { new OracleParameter("deviceId", deviceId) };

                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                if (dataTable.Rows.Count > 0)
                {
                    return MapDataRowToDevice(dataTable.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据设备ID获取设备信息失败：{ex.Message}");
            }

            return null;
        }

        public List<DeviceStatus> GetDevicesByRoomId(string roomId)
        {
            var devices = new List<DeviceStatus>();
            // 假设location字段包含房间信息或者有关联表
            var sql = @"SELECT device_id, device_name, device_type, installation_date, 
                              status, last_maintenance_date, maintenance_status, 
                              location, created_time, updated_time 
                       FROM DeviceStatus WHERE location LIKE :roomLocation
                       ORDER BY device_id";

            try
            {
                var parameters = new[] { new OracleParameter("roomLocation", $"%{roomId}%") };

                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                foreach (DataRow row in dataTable.Rows)
                {
                    devices.Add(MapDataRowToDevice(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据房间ID获取设备列表失败：{ex.Message}");
            }

            return devices;
        }

        public List<DeviceStatus> GetDevicesByStatus(string status)
        {
            var devices = new List<DeviceStatus>();
            var sql = @"SELECT device_id, device_name, device_type, installation_date, 
                              status, last_maintenance_date, maintenance_status, 
                              location, created_time, updated_time 
                       FROM DeviceStatus WHERE status = :status
                       ORDER BY device_id";

            try
            {
                var parameters = new[] { new OracleParameter("status", status) };

                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                foreach (DataRow row in dataTable.Rows)
                {
                    devices.Add(MapDataRowToDevice(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据设备状态获取设备列表失败：{ex.Message}");
            }

            return devices;
        }

        public List<DeviceStatus> GetDevicesByType(string deviceType)
        {
            var devices = new List<DeviceStatus>();
            var sql = @"SELECT device_id, device_name, device_type, installation_date, 
                              status, last_maintenance_date, maintenance_status, 
                              location, created_time, updated_time 
                       FROM DeviceStatus WHERE device_type = :deviceType
                       ORDER BY device_id";

            try
            {
                var parameters = new[] { new OracleParameter("deviceType", deviceType) };

                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                foreach (DataRow row in dataTable.Rows)
                {
                    devices.Add(MapDataRowToDevice(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据设备类型获取设备列表失败：{ex.Message}");
            }

            return devices;
        }

        public bool AddDevice(DeviceStatus device)
        {
            var sql = @"INSERT INTO DeviceStatus (device_name, device_type, installation_date, 
                              status, last_maintenance_date, maintenance_status, 
                              location, created_time, updated_time)
                       VALUES (:deviceName, :deviceType, :installationDate, 
                              :status, :lastMaintenanceDate, :maintenanceStatus, 
                              :location, SYSDATE, SYSDATE)";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("deviceName", device.DeviceName ?? string.Empty),
                    new OracleParameter("deviceType", device.DeviceType ?? string.Empty),
                    new OracleParameter("installationDate", device.InstallationDate),
                    new OracleParameter("status", device.Status ?? string.Empty),
                    new OracleParameter("lastMaintenanceDate", (object?)device.LastMaintenanceDate ?? DBNull.Value),
                    new OracleParameter("maintenanceStatus", device.MaintenanceStatus ?? string.Empty),
                    new OracleParameter("location", device.Location ?? string.Empty)
                };

                int result = _databaseService.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加设备失败：{ex.Message}");
                return false;
            }
        }

        public bool UpdateDevice(DeviceStatus device)
        {
            var sql = @"UPDATE DeviceStatus SET 
                              device_name = :deviceName,
                              device_type = :deviceType,
                              installation_date = :installationDate,
                              status = :status,
                              last_maintenance_date = :lastMaintenanceDate,
                              maintenance_status = :maintenanceStatus,
                              location = :location,
                              updated_time = SYSDATE
                       WHERE device_id = :deviceId";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("deviceName", device.DeviceName ?? string.Empty),
                    new OracleParameter("deviceType", device.DeviceType ?? string.Empty),
                    new OracleParameter("installationDate", device.InstallationDate),
                    new OracleParameter("status", device.Status ?? string.Empty),
                    new OracleParameter("lastMaintenanceDate", (object?)device.LastMaintenanceDate ?? DBNull.Value),
                    new OracleParameter("maintenanceStatus", device.MaintenanceStatus ?? string.Empty),
                    new OracleParameter("location", device.Location ?? string.Empty),
                    new OracleParameter("deviceId", device.DeviceId)
                };

                int result = _databaseService.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新设备失败：{ex.Message}");
                return false;
            }
        }

        public bool DeleteDevice(string deviceId)
        {
            var sql = "DELETE FROM DeviceStatus WHERE device_id = :deviceId";

            try
            {
                var parameters = new[] { new OracleParameter("deviceId", deviceId) };

                int result = _databaseService.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除设备失败：{ex.Message}");
                return false;
            }
        }

        public List<DeviceStatus> GetFaultyDevices()
        {
            return GetDevicesByStatus("Fault");
        }

        public List<DeviceStatus> GetOfflineDevices()
        {
            return GetDevicesByStatus("Offline");
        }

        public List<DeviceStatus> GetLowBatteryDevices(int threshold = 20)
        {
            // 这里假设有电量字段，如果没有可以返回空列表或者增加电量字段
            var devices = new List<DeviceStatus>();
            // 暂时返回维护状态为"Low Battery"的设备
            return GetDevicesByStatus("Low Battery");
        }

        public bool UpdateDeviceOnlineStatus(string deviceId, bool isOnline)
        {
            var status = isOnline ? "Normal" : "Offline";
            var sql = @"UPDATE DeviceStatus SET 
                              status = :status,
                              updated_time = SYSDATE
                       WHERE device_id = :deviceId";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("status", status),
                    new OracleParameter("deviceId", deviceId)
                };

                int result = _databaseService.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新设备在线状态失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 将数据行映射到设备对象
        /// </summary>
        private DeviceStatus MapDataRowToDevice(DataRow row)
        {
            return new DeviceStatus
            {
                DeviceId = Convert.ToInt32(row["device_id"]),
                DeviceName = row["device_name"].ToString(),
                DeviceType = row["device_type"].ToString(),
                InstallationDate = Convert.ToDateTime(row["installation_date"]),
                Status = row["status"].ToString(),
                LastMaintenanceDate = row["last_maintenance_date"] != DBNull.Value ? 
                    Convert.ToDateTime(row["last_maintenance_date"]) : null,
                MaintenanceStatus = row["maintenance_status"].ToString(),
                Location = row["location"].ToString(),
                CreatedTime = Convert.ToDateTime(row["created_time"]),
                UpdatedTime = Convert.ToDateTime(row["updated_time"])
            };
        }
    }
}
