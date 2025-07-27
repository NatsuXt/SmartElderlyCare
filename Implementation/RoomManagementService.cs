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
    /// 房间管理服务实现类
    /// </summary>
    public class RoomManagementService : IRoomManagementService
    {
        private readonly DatabaseService _databaseService;

        public RoomManagementService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<RoomManagement> GetAllRooms()
        {
            var rooms = new List<RoomManagement>();
            var sql = @"SELECT room_id, room_number, room_type, capacity, status, rate, 
                              bed_type, floor_num, created_time, updated_time 
                       FROM RoomManagement ORDER BY floor_num, room_number";

            try
            {
                var dataTable = _databaseService.ExecuteQuery(sql);
                foreach (DataRow row in dataTable.Rows)
                {
                    rooms.Add(MapDataRowToRoom(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有房间信息失败：{ex.Message}");
            }

            return rooms;
        }

        public RoomManagement? GetRoomById(int roomId)
        {
            var sql = @"SELECT room_id, room_number, room_type, capacity, status, rate, 
                              bed_type, floor_num, created_time, updated_time 
                       FROM RoomManagement WHERE room_id = :roomId";

            try
            {
                var parameters = new[] { new OracleParameter("roomId", roomId) };
                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                
                if (dataTable.Rows.Count > 0)
                {
                    return MapDataRowToRoom(dataTable.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据ID获取房间信息失败：{ex.Message}");
            }

            return null;
        }

        public List<RoomManagement> GetRoomsByStatus(string status)
        {
            var rooms = new List<RoomManagement>();
            var sql = @"SELECT room_id, room_number, room_type, capacity, status, rate, 
                              bed_type, floor_num, created_time, updated_time 
                       FROM RoomManagement WHERE status = :status ORDER BY floor_num, room_number";

            try
            {
                var parameters = new[] { new OracleParameter("status", status) };
                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    rooms.Add(MapDataRowToRoom(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据状态获取房间信息失败：{ex.Message}");
            }

            return rooms;
        }

        public List<RoomManagement> GetRoomsByFloor(int floor)
        {
            var rooms = new List<RoomManagement>();
            var sql = @"SELECT room_id, room_number, room_type, capacity, status, rate, 
                              bed_type, floor_num, created_time, updated_time 
                       FROM RoomManagement WHERE floor_num = :floor ORDER BY room_number";

            try
            {
                var parameters = new[] { new OracleParameter("floor", floor) };
                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    rooms.Add(MapDataRowToRoom(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据楼层获取房间信息失败：{ex.Message}");
            }

            return rooms;
        }

        public bool AddRoom(RoomManagement room)
        {
            var sql = @"INSERT INTO RoomManagement 
                       (room_id, room_number, room_type, capacity, status, rate, 
                        bed_type, floor_num, created_time, updated_time) 
                       VALUES 
                       (:room_id, :room_number, :room_type, :capacity, :status, :rate, 
                        :bed_type, :floor_num, :created_time, :updated_time)";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("room_id", room.RoomId),
                    new OracleParameter("room_number", room.RoomNumber),
                    new OracleParameter("room_type", room.RoomType),
                    new OracleParameter("capacity", room.Capacity),
                    new OracleParameter("status", room.Status),
                    new OracleParameter("rate", room.Rate),
                    new OracleParameter("bed_type", room.BedType),
                    new OracleParameter("floor_num", room.FloorNum),
                    new OracleParameter("created_time", room.CreatedTime),
                    new OracleParameter("updated_time", room.UpdatedTime)
                };

                return _databaseService.ExecuteNonQuery(sql, parameters) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加房间失败：{ex.Message}");
                return false;
            }
        }

        public bool UpdateRoom(RoomManagement room)
        {
            var sql = @"UPDATE RoomManagement SET 
                       room_number = :room_number, room_type = :room_type, capacity = :capacity, 
                       status = :status, rate = :rate, bed_type = :bed_type, 
                       floor_num = :floor_num, updated_time = :updated_time 
                       WHERE room_id = :room_id";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("room_number", room.RoomNumber),
                    new OracleParameter("room_type", room.RoomType),
                    new OracleParameter("capacity", room.Capacity),
                    new OracleParameter("status", room.Status),
                    new OracleParameter("rate", room.Rate),
                    new OracleParameter("bed_type", room.BedType),
                    new OracleParameter("floor_num", room.FloorNum),
                    new OracleParameter("updated_time", room.UpdatedTime),
                    new OracleParameter("room_id", room.RoomId)
                };

                return _databaseService.ExecuteNonQuery(sql, parameters) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新房间信息失败：{ex.Message}");
                return false;
            }
        }

        public bool DeleteRoom(int roomId)
        {
            var sql = "DELETE FROM RoomManagement WHERE room_id = :roomId";

            try
            {
                var parameters = new[] { new OracleParameter("roomId", roomId) };
                return _databaseService.ExecuteNonQuery(sql, parameters) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除房间失败：{ex.Message}");
                return false;
            }
        }

        public List<RoomManagement> GetAvailableRooms()
        {
            return GetRoomsByStatus("Available");
        }

        public bool IsRoomAvailable(int roomId)
        {
            var room = GetRoomById(roomId);
            return room != null && room.Status == "Available";
        }

        private RoomManagement MapDataRowToRoom(DataRow row)
        {
            return new RoomManagement
            {
                RoomId = Convert.ToInt32(row["room_id"]),
                RoomNumber = row["room_number"].ToString()!,
                RoomType = row["room_type"].ToString()!,
                Capacity = Convert.ToInt32(row["capacity"]),
                Status = row["status"].ToString()!,
                Rate = Convert.ToDecimal(row["rate"]),
                BedType = row["bed_type"].ToString()!,
                FloorNum = Convert.ToInt32(row["floor_num"]),
                CreatedTime = Convert.ToDateTime(row["created_time"]),
                UpdatedTime = Convert.ToDateTime(row["updated_time"])
            };
        }
    }
}
